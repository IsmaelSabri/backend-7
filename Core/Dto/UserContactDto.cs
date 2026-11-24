namespace Core.Dto
{
    public sealed class UserContactDto
    {
        public Guid Id { get; init; }
        public string? UserId { get; set; }
        public string? Username { get; init; }
        public string? Email { get; init; }
        public string? Phone { get; init; }
    }
}
