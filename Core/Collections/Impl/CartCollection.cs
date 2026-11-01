using Core.Data;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Collections.Impl
{
    public class CartCollection(CoreDb db) : ICartCollection
    {
        public async Task<Cart?> GetByUsuarioIdAsync(string usuarioId)
        {
            return await db.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
        }

        public async Task<Cart> AddAsync(Cart cart)
        {
            db.Carts.Add(cart);
            await db.SaveChangesAsync();
            return cart;
        }

        public async Task<CartItem> AddItemAsync(CartItem item)
        {
            db.CartItems.Add(item);
            await db.SaveChangesAsync();
            return item;
        }

        public async Task RemoveItemAsync(CartItem item)
        {
            db.CartItems.Remove(item);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Cart cart)
        {
            db.Carts.Remove(cart);
            await db.SaveChangesAsync();
        }

        public Task SaveChangesAsync() => db.SaveChangesAsync();
    }
}
