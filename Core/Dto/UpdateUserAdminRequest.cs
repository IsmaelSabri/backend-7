namespace Core.Dto
{
    public sealed class UpdateUserAdminRequest
    {
        public string? Firstname { get; init; }
        public string? Lastname { get; init; }
        public string? Username { get; init; }
        public string? Email { get; init; }
        public string? Phone { get; init; }
        public string? Role { get; init; }
        public bool? Isactive { get; init; }
        public ImageDto? ProfileImage { get; set; }
    }
}
