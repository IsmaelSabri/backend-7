using Sieve.Attributes;

namespace Core.Models
{
    public class Plan
    {
        [Sieve(CanFilter = true)]
        public Guid Id { get; private set; }
        [Sieve(CanFilter = true)]
        public string? Name { get; private set; }
        public decimal MonthlyPrice { get; private set; }
        public decimal YearlyPrice { get; private set; }

        public string? Description { get; private set; }
        public string[]? Features { get; private set; }

        public int MaxActiveListings { get; private set; }
        public int MaxImagesPerListing { get; private set; }
        public int MaxHighlightedListings { get; private set; }
        public bool HasAnalytics { get; private set; }
    }

}