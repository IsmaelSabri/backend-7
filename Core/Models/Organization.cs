using Sieve.Attributes;

namespace Core.Models
{
    public class Organization
    {
        [Sieve(CanFilter = true)]
        public Guid Id { get; private set; }
        [Sieve(CanFilter = true)]
        public string? Name { get; private set; }
        [Sieve(CanFilter = true)]
        public string? CIF { get; private set; }
        public string? OfficeAddress { get; private set; }


        public DateTime CreatedAt { get; private set; }
        [Sieve(CanFilter = true)]
        public Guid ActiveSubscriptionId { get; private set; } // plan
    }

}