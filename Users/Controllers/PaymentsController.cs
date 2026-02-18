using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Security.Claims;
using Users.Data;
using Users.Models;
using Users.Collections.impl;
using Users.Collections;
using Users.Dto;
using Users.Collections.Impl;
using Users.Services;
using System.Numerics;
namespace Users.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    public class PaymentsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly UserDb _context;
        private readonly IUserCollection db;
        private readonly IExtraContent db2;
        private readonly ITransaccionCollection transaccionDb;
        private readonly IWebHookCollection webHookDb;


        //private User user1; // user1

        public PaymentsController(IConfiguration configuration, UserDb appContext, ITransaccionCollection transaccionCollection, IImageService imageService)
        {
            _configuration = configuration;
            _context = appContext;
            db = new UserCollection(appContext, imageService);
            db2 = new ExtraContentCollection(appContext);
            transaccionDb = transaccionCollection;
        }

        [HttpPost("create-payment-intent")]
        public async Task<ActionResult> Create([FromBody] PaymentIntentCreateRequestDto? request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request: missing items");
            }

            StripeConfiguration.ApiKey = _configuration.GetSection("StripeSettings").GetSection("STRIPE_SECRET_KEY").Value!;
            var paymentIntentService = new PaymentIntentService();

            var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
            {
                //Amount = request.Amount,
                Currency = "eur",

                //Customer = firstItem.Customer,
                // In the latest version of the API, specifying the `automatic_payment_methods` parameter is optional because Stripe enables its functionality by default.
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            });
            var transaccion = new Transaccion
            {
                UsuarioId = request.UsuarioId!,
                StripePaymentIntentId = paymentIntent.Id,
                Estado = Enums.EstadoTransaccion.Pendiente,
                FechaCreacion = DateTime.UtcNow,
                BaseImponible = 21,
                Iva = CalculateOrderAmount(request.Items!) / 100m * 0.21m,
                Total = CalculateOrderAmount(request.Items!) / 100m,
                Moneda = "EUR",

            };
            transaccion.Total += transaccion.Iva;



            var dump = ObjectDumper.Dump(transaccion);
            Console.WriteLine(dump);
            await transaccionDb.AddAsync(transaccion);
            return Json(new
            {
                clientSecret = paymentIntent.ClientSecret,
                // [DEV]: For demo purposes only, you should avoid exposing the PaymentIntent ID in the client-side code.
                dpmCheckerLink = $"https://dashboard.stripe.com/settings/payment_methods/review?transaction_id={paymentIntent.Id}"
            });
        }

        private decimal CalculateOrderAmount(Item[] items)
        {
            // Calculate the order total on the server to prevent
            // people from directly manipulating the amount on the client
            decimal total = 0;
            foreach (Item item in items)
            {
                total += item.PrecioUnitario * item.Cantidad * 100; // convertir a centimos
            }
            return total;
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            Console.WriteLine("esto funca");
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var endpointSecret = _configuration.GetSection("StripeSetting").GetSection("STRIPE_WEBHOOK_SECRET").Value!;
            try
            {
                var stripeEvent = EventUtility.ParseEvent(json);
                var signatureHeader = Request.Headers["Stripe-Signature"];

                stripeEvent = EventUtility.ConstructEvent(json,
                        signatureHeader, endpointSecret);
                // crear evento webhook
                var stripeWebhookEvent = new StripeWebhookEvent
                {
                    StripeEventId = stripeEvent.Id,
                    Tipo = stripeEvent.Type,
                    Fecha = DateTime.UtcNow,
                    PayloadJson = json
                };
                // guardar evento webhook
                await webHookDb.AddAsync(stripeWebhookEvent);
                // If on SDK version < 46, use class Events instead of EventTypes
                if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    if (paymentIntent is not null)
                    {
                        Console.WriteLine("A successful payment for {0} was made.", paymentIntent.Amount);
                        // Then define and call a method to handle the successful payment intent. ---> actualizar transacción
                        var transaccion = await transaccionDb.GetByStripePaymentIntentIdAsync(paymentIntent.Id);
                        if (transaccion != null)
                        {
                            transaccion.Estado = Enums.EstadoTransaccion.Completada;
                            transaccion.FechaPago = DateTime.UtcNow;
                            transaccion.StripeChargeId = paymentIntent.LatestChargeId;
                            await transaccionDb.UpdateAsync(transaccion);

                            // Crear ExtraSubscription para cada línea de transacción
                            // if (transaccion.Lineas != null && transaccion.Lineas.Count > 0)
                            // {
                            //     foreach (var linea in transaccion.Lineas)
                            //     {
                            //         var extraSubscription = new ExtraSubscription
                            //         {
                            //             Id = Guid.NewGuid(),
                            //             StripePaymentIntentId = paymentIntent.Id,
                            //             UsuarioId = transaccion.UsuarioId,
                            //             ExtraId = linea.ExtraContenidoId?.ToString(),
                            //             LineaTransaccionId = linea.Id.ToString(),
                            //             CreatedAt = DateTime.UtcNow,
                            //             LastUpdatedAt = DateTime.UtcNow
                            //         };
                            //         await db2.Add(extraSubscription);
                            //     }
                            // }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Webhook: payment intent is null in event data");
                    }
                }
                else if (stripeEvent.Type == EventTypes.PaymentMethodAttached)
                {
                    var paymentMethod = stripeEvent.Data.Object as PaymentMethod;
                    // Then define and call a method to handle the successful attachment of a PaymentMethod.
                    // handlePaymentMethodAttached(paymentMethod);
                }
                else
                {
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }
                return Ok();
            }
            catch (StripeException e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }



    }
}
