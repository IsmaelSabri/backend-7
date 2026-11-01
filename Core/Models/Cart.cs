namespace Core.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Cart")]
    public class Cart
    {
        public Guid Id { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
    [Table("CartItem")]
    public class CartItem
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public string ExtraId { get; set; } = string.Empty;
        public string ViviendaId { get; set; } = string.Empty;
        public string NombreExtra { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; } = 1;
    }
}
