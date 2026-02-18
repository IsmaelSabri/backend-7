using Users.Models;

namespace Users.Collections
{
    public interface ITransaccionCollection
    {
        Task<Transaccion?> GetByIdAsync(Guid id);
        Task<List<Transaccion>> GetAllAsync();
        Task<List<Transaccion>> GetByUsuarioIdAsync(string usuarioId);
        Task<Transaccion?> GetByStripePaymentIntentIdAsync(string paymentIntentId);
        Task<Transaccion> AddAsync(Transaccion transaccion);
        Task<Transaccion> UpdateAsync(Transaccion transaccion);
        Task<bool> DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}