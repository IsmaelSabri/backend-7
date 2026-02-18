namespace Users.Dto
{
    public class CustomHttpResponseDto
    {
        public int HttpStatusCode { get; set; }
        public string? HttpStatus { get; set; }
        public string? Reason { get; set; }
        public string? Message { get; set; }
    }
}
