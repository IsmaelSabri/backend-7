namespace Images.Dto
{
    public sealed class ImageDto
    {
        public string? OwnerId { get; set; }
        public string? OwnerType { get; set; }
        public List<IFormFile> files { get; set; } = new List<IFormFile>();
    }
}