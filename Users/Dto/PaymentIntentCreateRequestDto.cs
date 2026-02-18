using Newtonsoft.Json;

namespace Users.Dto
{
    public class PaymentIntentCreateRequestDto
    {
        [JsonProperty("id")] // id 
        public string? Id { get; set; }
        [JsonProperty("usuarioId")]
        public string? UsuarioId { get; set; }
        [JsonProperty("items")]
        public Item[]? Items { get; set; }
    }

    public class Item
    {
        public string? ExtraContenidoId { get; set; }
        public string? PropiedadId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
