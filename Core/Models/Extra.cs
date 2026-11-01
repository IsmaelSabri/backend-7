namespace Core.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("Extras")]
    public class Extra
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public bool PuedeCaducar { get; set; }
        public int DuracionPorDefecto { get; set; }
        public string? Info { get; set; }
        public bool IsEnabled { get; set; }
        public string? Description { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
