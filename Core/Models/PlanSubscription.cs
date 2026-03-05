using System.ComponentModel.DataAnnotations.Schema;
using Images.Models;
using Sieve.Attributes;
using Core.Enums;

namespace Core.Models
{
    public class PlanSubscription
    {
        [Sieve(CanFilter = true)]
        public Guid Id { get; private set; }
        [Sieve(CanFilter = true)]
        public string? StripePaymentIntentId { get; set; }
        [Sieve(CanFilter = true)]
        public Guid OrganizationId { get; set; }
        [Sieve(CanFilter = true)]
        public Guid UserId { get; private set; }
        [Sieve(CanFilter = true)]
        public Guid PlanId { get; private set; }
        [Sieve(CanFilter = true)]
        public string? LineaTransaccionId { get; set; }
        [Sieve(CanFilter = true)]
        public string? StripeSubscriptionId { get; set; }

        public bool AutoRenew { get; set; }
        public SubscriptionStatus Status { get; private set; }
        public SubscriptionPeriod Period { get; set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }

        public DateTime CreatedAt { get; private set; }

        [NotMapped]
        public Image? BrandImage { get; private set; }
        public string? Color { get; private set; }
        public string? WebUrl { get; private set; }
    }

}