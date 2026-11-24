namespace Images.Services;

public static class ImageStorageLayout
{
    private static readonly (string Suffix, int Width)[] PropertyVariants =
        [(".thumbnail.webp", 500), (".card.webp", 800), (".webp", 1600)];
    private static readonly (string Suffix, int Width)[] ProfileVariants =
        [(".avatar.webp", 192), (".webp", 560)];
    private static readonly (string Suffix, int Width)[] EnergyCertificateVariants =
        [(".webp", 560)];

    public static string CreateKey(string ownerType, string ownerId)
    {
        var prefix = ownerType.Equals("ProfileImage", StringComparison.OrdinalIgnoreCase) ? "p2-"
            : ownerType.Equals("EnergyCertImage", StringComparison.OrdinalIgnoreCase) ? "e2-" : "";
        return $"{ownerType.ToLowerInvariant()}/{ownerId}/{prefix}{Guid.NewGuid():N}.webp";
    }

    public static bool IsNewProfileKey(string key) =>
        key.StartsWith("profileimage/", StringComparison.OrdinalIgnoreCase)
        && FileName(key).StartsWith("p2-", StringComparison.Ordinal);

    public static bool IsNewEnergyCertificateKey(string key) =>
        key.StartsWith("energycertimage/", StringComparison.OrdinalIgnoreCase)
        && FileName(key).StartsWith("e2-", StringComparison.Ordinal);

    public static bool IsProfileKey(string key) =>
        key.StartsWith("profileimage/", StringComparison.OrdinalIgnoreCase);

    public static IReadOnlyList<(string Suffix, int Width)> GetVariants(string key) =>
        IsNewProfileKey(key) ? ProfileVariants
        : IsNewEnergyCertificateKey(key) ? EnergyCertificateVariants : PropertyVariants;

    public static string VariantKey(string key, string suffix)
    {
        if (!key.EndsWith(".webp", StringComparison.Ordinal))
            throw new InvalidOperationException("La clave de almacenamiento no es valida.");
        return key[..^5] + suffix;
    }

    public static string RawKey(string key) => $"raw/{VariantKey(key, "")}";

    private static string FileName(string key) => key[(key.LastIndexOf('/') + 1)..];
}
