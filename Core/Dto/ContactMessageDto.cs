namespace Core.Dto
{
    public class ContactMessageDto
    {
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Company { get; set; }
        public string? Subject { get; set; }
        public string? Category { get; set; }
        public string? Message { get; set; }
        public string? Phone { get; set; }
        public string? DialCodePlus { get; set; }
        public string? Country { get; set; }
        public string? Status { get; set; } = "pending";
    }
}
