using Newtonsoft.Json;

namespace Users.Dto
{
    public class PaymentIntentCreateRequestDto
    {
        [JsonProperty("items")]
        public Item[]? Items { get; set; }
    }
    public class Item
    {
        [JsonProperty("id")] // id 
        public string? Id { get; set; }
        [JsonProperty("amount")]
        public long Amount { get; set; }
        [JsonProperty("customer")]
        public string? Customer { get; set; }
    }
}
