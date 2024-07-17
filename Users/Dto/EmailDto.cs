namespace Email.Dto
{
    public class EmailDto
    {
        public string? FromName { get; set; }
        public string? FromAddress { get; set; }
        public string? ToEmail { get; set; }
        public string? CcEmail { get; set; }
        public string? BccEmail { get; set; }
        public string? Username { get; set; }
        public string? Name { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }
        public string? Phone { get; set; }
    }
}