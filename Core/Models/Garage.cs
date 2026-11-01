using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Core.Models
{
    [Table("Garages")]
    public class Garage
{
    public Guid Id { get; set; }

    public Guid HomeId { get; set; }
    [ForeignKey("HomeId")]
    [JsonIgnore]
    public Home? Home { get; set; }

    public bool IsIncluded { get; set; }

    public decimal? Price { get; set; }
}
}