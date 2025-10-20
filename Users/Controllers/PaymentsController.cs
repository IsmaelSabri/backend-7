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
namespace Users.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    public class PaymentsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly UserDb _context;
        private readonly IUserCollection db;
        private readonly IOrderCollection db2;

        //private User user1; // user1

        public PaymentsController(IConfiguration configuration, UserDb appContext, UserDb hdb)
        {
            _configuration = configuration;
            _context = appContext;
            db = new UserCollection(hdb);
            db2 = new OrderCollection(appContext);
        }

        [HttpPost("create-payment-intent")]
        public async Task<ActionResult> Create([FromBody] PaymentIntentCreateRequestDto request)
        {
            StripeConfiguration.ApiKey = _configuration.GetSection("StripeSettings").GetSection("STRIPE_SECRET_KEY").Value!;
            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
            {
                Amount = CalculateOrderAmount(request.Items),
                Currency = "eur",
                //Customer = request.Items[0].Customer,
                // In the latest version of the API, specifying the `automatic_payment_methods` parameter is optional because Stripe enables its functionality by default.
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            });
            var options = new ProductCreateOptions
            {
                
                Name = "Gold Plan",
                Description = "Any description"
            };
            var service = new ProductService();
            Product product = service.Create(options);
            /*
            * request.Items[0] ok
            * var user ok
            * var order ok 
            */
            // var user = await db.GetUserByUserId(request.Items[0].Customer);

            // if (user == null)
            // {
            //     return NotFound();
            // }

            var order = new Order
            {
                UserId = request.Items[0].Customer,
                PaymentIntentId = paymentIntent.Id,
                Amount = paymentIntent.Amount,
                IsPaid = false,
                CreatedAt = DateTime.UtcNow,
            };
            var dump = ObjectDumper.Dump(product);
            Console.WriteLine(dump);
            //await db2.Add(order);
            //await db2.SaveChangesAsync();

            return Json(new
            {
                clientSecret = paymentIntent.ClientSecret,
                // [DEV]: For demo purposes only, you should avoid exposing the PaymentIntent ID in the client-side code.
                dpmCheckerLink = $"https://dashboard.stripe.com/settings/payment_methods/review?transaction_id={paymentIntent.Id}"
            });
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

                // If on SDK version < 46, use class Events instead of EventTypes
                if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    Console.WriteLine("A successful payment for {0} was made.", paymentIntent.Amount);
                    // Then define and call a method to handle the successful payment intent.

                    var order = await _context.Orders
                        .Include(o => o.UserId)
                        .FirstOrDefaultAsync(o => o.PaymentIntentId == paymentIntent.Id);
                    if (order != null)
                    {
                        order.IsPaid = true;
                        // habilitar extra
                        //order.User.Credits += 2000;
                    }
                    await _context.SaveChangesAsync();
                    // handlePaymentIntentSucceeded(paymentIntent);
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
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }

        private long CalculateOrderAmount(Item[] items)
        {
            // Calculate the order total on the server to prevent
            // people from directly manipulating the amount on the client
            long total = 0;
            foreach (Item item in items)
            {
                total += item.Amount;
            }
            return total;
        }

    }
}
