namespace Images.Enums
{
    public class OwnerType
    {
        public static readonly string ProfileImage = "ProfileImage";
        public static readonly string BrandImage = "BrandImage";
        public static readonly string HomeImages = "HomeImages";
        public static readonly string HomeSchemes = "HomeSchemes";
        public static readonly string EnergyCertImage = "EnergyCertImage";

        public string Value { get; private set; }

        private OwnerType(string value)
        {
            Value = value;
        }

        public static OwnerType FromString(string value)
        {
            return value switch
            {
                "profileImage" => new OwnerType(ProfileImage),
                "BrandImage" => new OwnerType(BrandImage),
                "HomeImages" => new OwnerType(HomeImages),
                "HomeSchemes" => new OwnerType(HomeSchemes),
                "EnergyCertImage" => new OwnerType(EnergyCertImage),
                _ => throw new ArgumentException($"Invalid OwnerType: {value}")
            };
        }

        public override string ToString() => Value;
        public override bool Equals(object? obj) => obj is OwnerType other && other.Value == Value;
        public override int GetHashCode() => Value.GetHashCode();
    }
}