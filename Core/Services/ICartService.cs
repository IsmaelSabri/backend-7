using Core.Models;

namespace Core.Services
{
    public interface ICartService
    {
        const decimal IvaRate = 0.21m;

        Task<Cart> GetCartAsync(string usuarioId);
        Task<Cart?> AddItemAsync(string usuarioId, string extraId, string viviendaId);
        Task<Cart?> UpdateQuantityAsync(string usuarioId, Guid itemId, int cantidad);
        Task<Cart?> RemoveItemAsync(string usuarioId, Guid itemId);
        Task ClearAsync(string usuarioId);
    }
}
