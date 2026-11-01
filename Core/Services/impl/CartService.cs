using Core.Collections;
using Core.Models;

namespace Core.Services.impl
{
    public class CartService(ICartCollection carts, IExtraCollection extras) : ICartService
    {
        public async Task<Cart> GetCartAsync(string usuarioId)
        {
            var cart = await carts.GetByUsuarioIdAsync(usuarioId);
            return cart ?? await carts.AddAsync(new Cart { UsuarioId = usuarioId });
        }

        // El precio y el nombre se toman siempre del catálogo, nunca del cliente.
        public async Task<Cart?> AddItemAsync(string usuarioId, string extraId, string viviendaId)
        {
            if (!Guid.TryParse(extraId, out var extraGuid) || string.IsNullOrWhiteSpace(viviendaId))
                return null;

            var extra = await extras.GetByIdAsync(extraGuid);
            if (extra is null || !extra.IsEnabled)
                return null;

            var cart = await GetCartAsync(usuarioId);
            var exists = cart.Items.Any(i => i.ExtraId == extraId && i.ViviendaId == viviendaId);
            if (!exists)
            {
                await carts.AddItemAsync(new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ExtraId = extraId,
                    ViviendaId = viviendaId,
                    NombreExtra = extra.Nombre,
                    PrecioUnitario = extra.Precio,
                    Cantidad = 1
                });
                await Touch(cart);
            }
            return await GetCartAsync(usuarioId);
        }

        public async Task<Cart?> UpdateQuantityAsync(string usuarioId, Guid itemId, int cantidad)
        {
            var cart = await GetCartAsync(usuarioId);
            var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
            if (item is null) return null;

            if (cantidad <= 0)
                await carts.RemoveItemAsync(item);
            else
                item.Cantidad = cantidad;

            await Touch(cart);
            return await GetCartAsync(usuarioId);
        }

        public async Task<Cart?> RemoveItemAsync(string usuarioId, Guid itemId)
        {
            var cart = await GetCartAsync(usuarioId);
            var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
            if (item is null) return null;

            await carts.RemoveItemAsync(item);
            await Touch(cart);
            return await GetCartAsync(usuarioId);
        }

        public async Task ClearAsync(string usuarioId)
        {
            var cart = await carts.GetByUsuarioIdAsync(usuarioId);
            if (cart is not null)
                await carts.DeleteAsync(cart);
        }

        private async Task Touch(Cart cart)
        {
            cart.LastUpdate = DateTime.UtcNow;
            await carts.SaveChangesAsync();
        }
    }
}
