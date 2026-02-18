namespace Users.Models
{
    // Modelo que representa el contenido extra
    // reutilizable, estable, no transaccional
    public class Extra
    {
        public Guid Id { get; set; }
        public string? Nombre { get; set; }
        public decimal Precio { get; set; }
        public bool PuedeCaducar { get; set; }
        public int DuracionPorDefecto { get; set; }
    }
}