namespace Users.Models
{
    public class Organization
    {
        public Guid Id { get; private set; }
        public string? Name { get; private set; }
        public string? CIF { get; private set; }
        public string? OfficeAddress { get; private set; }


        public DateTime CreatedAt { get; private set; }

        public Guid ActiveSubscriptionId { get; private set; } // plan
    }

}