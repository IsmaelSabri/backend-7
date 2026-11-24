namespace Core.Dto
{
    public sealed class UserSummaryDto
    {
        public Guid Id { get; init; }
        public string? UserId { get; set; }
        public string? Username { get; init; }
        public string? Status { get; set; }
        public ImageDto? ProfileImage { get; init; }
    }
}
