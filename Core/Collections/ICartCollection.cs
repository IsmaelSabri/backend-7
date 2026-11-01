using Core.Models;

namespace Core.Collections
{
    public interface ICartCollection
    {
        Task<Cart?> GetByUsuarioIdAsync(string usuarioId);
        Task<Cart> AddAsync(Cart cart);
        Task<CartItem> AddItemAsync(CartItem item);
        Task RemoveItemAsync(CartItem item);
        Task DeleteAsync(Cart cart);
        Task SaveChangesAsync();
    }
}
