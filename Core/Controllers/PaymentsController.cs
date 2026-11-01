using Core.Collections;
using Core.Dto;
using Core.Enums;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Core.Controllers
{
    [Route("api/[controller]")]
    public class PaymentsController(
        IConfiguration configuration,
        ICartCollection cartDb,
        ICartService cartService,
        IExtraCollection extraDb,
        IExtraContentCollection extraContentDb,
        ITransaccionCollection transaccionDb,
        IWebHookCollection webHookDb) : Controller
    {
        private const string Currency = "eur";

        private string StripeSecretKey =>
            configuration.GetSection("StripeSettings").GetSection("STRIPE_SECRET_KEY").Value!;

        // Fase 2: el Cart se convierte en una Transaccion Pendiente y se crea el PaymentIntent
        // con el importe calculado en servidor.
        [HttpPost("create-payment-intent")]
        public async Task<ActionResult> Create([FromBody] CheckoutRequestDto? request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.UsuarioId))
                return BadRequest("Invalid request: missing usuarioId");

            var cart = await cartDb.GetByUsuarioIdAsync(request.UsuarioId);
            if (cart is null || cart.Items.Count == 0)
                return BadRequest("El carrito está vacío");

            var transaccionId = Guid.NewGuid();
            var lineas = cart.Items.Select(i => new LineaTransaccion
            {
                Id = Guid.NewGuid(),
                TransaccionId = transaccionId,
                ExtraId = i.ExtraId,
                ViviendaId = i.ViviendaId,
                Concepto = i.NombreExtra,
                PrecioUnitario = i.PrecioUnitario,
                Cantidad = i.Cantidad,
                TotalLinea = i.PrecioUnitario * i.Cantidad
            }).ToList();

            var baseImponible = lineas.Sum(l => l.TotalLinea);
            var iva = Math.Round(baseImponible * ICartService.IvaRate, 2);
            var total = baseImponible + iva;

            StripeConfiguration.ApiKey = StripeSecretKey;
            var paymentIntent = await new PaymentIntentService().CreateAsync(new PaymentIntentCreateOptions
            {
                Amount = (long)Math.Round(total * 100m),
                Currency = Currency,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions { Enabled = true },
                Metadata = new Dictionary<string, string>
                {
                    ["transaccionId"] = transaccionId.ToString(),
                    ["usuarioId"] = request.UsuarioId
                }
            });

            await transaccionDb.AddAsync(new Transaccion
            {
                Id = transaccionId,
                UsuarioId = request.UsuarioId,
                StripePaymentIntentId = paymentIntent.Id,
                Estado = EstadoTransaccion.Pendiente,
                FechaCreacion = DateTime.UtcNow,
                BaseImponible = baseImponible,
                Iva = iva,
                Total = total,
                Moneda = Currency.ToUpperInvariant(),
                Lineas = lineas
            });

            return Json(new
            {
                clientSecret = paymentIntent.ClientSecret,
                transaccionId,
                baseImponible,
                iva,
                total
            });
        }

        // Fase 4: devolución del dinero y retirada inmediata del beneficio.
        [HttpPost("{transaccionId:guid}/refund")]
        public async Task<IActionResult> Refund(Guid transaccionId)
        {
            var transaccion = await transaccionDb.GetByIdAsync(transaccionId);
            if (transaccion is null) return NotFound();
            if (transaccion.Estado != EstadoTransaccion.Completada)
                return BadRequest("Solo se pueden reembolsar transacciones completadas");

            StripeConfiguration.ApiKey = StripeSecretKey;
            try
            {
                await new RefundService().CreateAsync(new RefundCreateOptions
                {
                    PaymentIntent = transaccion.StripePaymentIntentId
                });
            }
            catch (StripeException e)
            {
                return BadRequest(e.Message);
            }

            await MarkRefundedAsync(transaccion);
            return Ok(transaccion);
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var endpointSecret = configuration.GetSection("StripeSettings").GetSection("STRIPE_WEBHOOK_SECRET").Value!;
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], endpointSecret);

                await webHookDb.AddAsync(new StripeWebhookEvent
                {
                    StripeEventId = stripeEvent.Id,
                    Tipo = stripeEvent.Type,
                    Fecha = DateTime.UtcNow,
                    PayloadJson = json
                });

                if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded && stripeEvent.Data.Object is PaymentIntent succeeded)
                {
                    var transaccion = await transaccionDb.GetByStripePaymentIntentIdAsync(succeeded.Id);
                    if (transaccion is not null)
                        await CompleteAsync(transaccion, succeeded.LatestChargeId);
                }
                else if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed && stripeEvent.Data.Object is PaymentIntent failed)
                {
                    var transaccion = await transaccionDb.GetByStripePaymentIntentIdAsync(failed.Id);
                    if (transaccion is not null && transaccion.Estado == EstadoTransaccion.Pendiente)
                    {
                        transaccion.Estado = EstadoTransaccion.Fallida;
                        await transaccionDb.UpdateAsync(transaccion);
                    }
                }
                else if (stripeEvent.Type == EventTypes.ChargeRefunded && stripeEvent.Data.Object is Charge charge
                         && !string.IsNullOrEmpty(charge.PaymentIntentId))
                {
                    var transaccion = await transaccionDb.GetByStripePaymentIntentIdAsync(charge.PaymentIntentId);
                    if (transaccion is not null)
                        await MarkRefundedAsync(transaccion);
                }

                return Ok();
            }
            catch (StripeException e)
            {
                Console.WriteLine("Stripe webhook error: {0}", e.Message);
                return BadRequest();
            }
            catch (Exception e)
            {
                Console.WriteLine("Webhook error: {0}", e);
                return StatusCode(500);
            }
        }

        // Fase 3: Transaccion completada, Cart eliminado y ExtraContent activado.
        private async Task CompleteAsync(Transaccion transaccion, string? chargeId)
        {
            if (transaccion.Estado == EstadoTransaccion.Completada) return;

            var now = DateTime.UtcNow;
            transaccion.Estado = EstadoTransaccion.Completada;
            transaccion.FechaPago = now;
            transaccion.StripeChargeId = chargeId;
            await transaccionDb.UpdateAsync(transaccion);

            foreach (var linea in transaccion.Lineas)
            {
                DateTime? expiracion = null;
                if (Guid.TryParse(linea.ExtraId, out var extraGuid))
                {
                    var extra = await extraDb.GetByIdAsync(extraGuid);
                    if (extra is { PuedeCaducar: true, DuracionPorDefecto: > 0 })
                        expiracion = now.AddDays(extra.DuracionPorDefecto);
                }

                await extraContentDb.Add(new ExtraContent
                {
                    Id = Guid.NewGuid(),
                    ExtraId = linea.ExtraId,
                    ViviendaId = linea.ViviendaId,
                    TransaccionId = transaccion.Id.ToString(),
                    Estado = EstadoExtraContent.Activo,
                    FechaActivacion = now,
                    FechaExpiracion = expiracion,
                    CreatedAt = now,
                    LastUpdate = now
                });
            }

            await cartService.ClearAsync(transaccion.UsuarioId);
        }

        private async Task MarkRefundedAsync(Transaccion transaccion)
        {
            if (transaccion.Estado != EstadoTransaccion.Reembolsada)
            {
                transaccion.Estado = EstadoTransaccion.Reembolsada;
                await transaccionDb.UpdateAsync(transaccion);
            }

            foreach (var content in await extraContentDb.GetByTransaccionIdAsync(transaccion.Id.ToString()))
            {
                if (content.Estado == EstadoExtraContent.Reembolsado) continue;
                content.Estado = EstadoExtraContent.Reembolsado;
                await extraContentDb.UpdateAsync(content);
            }
        }
    }
}
