using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Core.Enums;

namespace Core.Models
{
    [Table("ExtraContent")]
    public class ExtraContent
    {
        public Guid Id { get; set; }
        public string ExtraId { get; set; } = string.Empty;
        public string ViviendaId { get; set; } = string.Empty;
        public string TransaccionId { get; set; } = string.Empty;
        [Column("Estado")]
        [JsonConverter(typeof(JsonStringEnumConverter<EstadoExtraContent>))]
        public EstadoExtraContent Estado { get; set; } = EstadoExtraContent.PendientePago;
        public DateTime? FechaActivacion { get; set; }
        public DateTime? FechaExpiracion { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
    }
}
