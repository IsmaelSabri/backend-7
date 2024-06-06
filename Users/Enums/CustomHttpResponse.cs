namespace Users.Enums
{
    public interface CustomHttpResponse
    {
        public string? Headers { get; set; }
        public string? Content { get; set; }
        public string? StatusCode { get; set; }
        public string? Message { get; set; }
    }
}