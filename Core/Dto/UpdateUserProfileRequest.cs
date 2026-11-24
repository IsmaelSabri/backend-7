namespace Core.Dto
{
    public sealed class UpdateUserProfileRequest
    {
        public string? Firstname { get; init; }
        public string? Lastname { get; init; }
        public string? Phone { get; init; }
        public ImageDto? ProfileImage { get; set; }
    }
}
