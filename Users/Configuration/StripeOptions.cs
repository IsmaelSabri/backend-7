using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Users.Configuration
{
    public class StripeOptions
    {
        public string? PublishableKey { get; set; }
        public string? SecretKey { get;set; }
        public string? WebhookSecret { get; set; }   
    }
}