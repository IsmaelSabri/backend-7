using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("Garage")]
    public class Garage
{
    public Guid Id { get; set; }

    public Guid HomeId { get; set; }

    public Home Home { get; set; } = null!;

    public bool IsIncluded { get; set; }

    public decimal? Price { get; set; }
}
}