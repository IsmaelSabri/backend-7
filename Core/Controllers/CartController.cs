using Core.Dto;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController(ICartService cartService) : ControllerBase
    {
        [HttpGet("{usuarioId}")]
        public async Task<ActionResult<Cart>> Get(string usuarioId)
            => Ok(await cartService.GetCartAsync(usuarioId));

        [HttpPost("{usuarioId}/items")]
        public async Task<ActionResult<Cart>> AddItem(string usuarioId, [FromBody] AddCartItemDto dto)
        {
            var cart = await cartService.AddItemAsync(usuarioId, dto.ExtraId, dto.ViviendaId);
            return cart is null ? BadRequest("Extra no disponible o vivienda inválida") : Ok(cart);
        }

        [HttpPut("{usuarioId}/items/{itemId:guid}")]
        public async Task<ActionResult<Cart>> UpdateItem(string usuarioId, Guid itemId, [FromBody] UpdateCartItemDto dto)
        {
            var cart = await cartService.UpdateQuantityAsync(usuarioId, itemId, dto.Cantidad);
            return cart is null ? NotFound() : Ok(cart);
        }

        [HttpDelete("{usuarioId}/items/{itemId:guid}")]
        public async Task<ActionResult<Cart>> RemoveItem(string usuarioId, Guid itemId)
        {
            var cart = await cartService.RemoveItemAsync(usuarioId, itemId);
            return cart is null ? NotFound() : Ok(cart);
        }

        [HttpDelete("{usuarioId}")]
        public async Task<IActionResult> Clear(string usuarioId)
        {
            await cartService.ClearAsync(usuarioId);
            return NoContent();
        }
    }
}
