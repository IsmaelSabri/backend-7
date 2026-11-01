namespace Core.Dto
{
    public class AddCartItemDto
    {
        public string ExtraId { get; set; } = string.Empty;
        public string ViviendaId { get; set; } = string.Empty;
    }

    public class UpdateCartItemDto
    {
        public int Cantidad { get; set; }
    }

    public class CheckoutRequestDto
    {
        public string UsuarioId { get; set; } = string.Empty;
    }
}
