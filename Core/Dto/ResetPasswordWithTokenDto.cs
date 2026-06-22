namespace Core.Dto
{
    public sealed class ResetPasswordWithTokenDto
    {
        public string Token { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}