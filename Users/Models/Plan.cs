namespace Users.Models
{
    public class Plan
    {
        public Guid Id { get; private set; }
        public string? Name { get; private set; }
        public decimal MonthlyPrice { get; private set; }
        public decimal YearlyPrice { get; private set; }

        public int MaxActiveListings { get; private set; }
        public int MaxImagesPerListing { get; private set; }
        public string? BrandImage { get; private set; }
        public string? Color { get; private set; }
        public string? WebUrl { get; private set; }

        public bool HasHighlightedListings { get; private set; }
        public bool HasAnalytics { get; private set; }
    }

}