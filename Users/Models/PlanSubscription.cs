using Users.Enums;

namespace Users.Models
{
    public class PlanSubscription
    {
        public Guid Id { get; private set; }
        public string? StripePaymentIntentId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid UserId { get; private set; }
        public Guid PlanId { get; private set; }
        public string? LineaTransaccionId { get; set; }

        public bool AutoRenew { get; set; }
        public string? StripeSubscriptionId { get; set; }
        public SubscriptionStatus Status { get; private set; }
        public SubscriptionPeriod Period { get; set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }

        public DateTime CreatedAt { get; private set; }
    }

}