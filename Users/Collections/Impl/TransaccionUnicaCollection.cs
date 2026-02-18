using Microsoft.EntityFrameworkCore;
using Users.Data;
using Users.Models;

namespace Users.Collections.Impl
{
    public class TransaccionCollection(UserDb db) : ITransaccionCollection
    {
        private readonly UserDb db = db;

        public async Task<Transaccion?> GetByIdAsync(Guid id)
        {
            return await db.Transacciones.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Transaccion>> GetAllAsync()
        {
            return await db.Transacciones.ToListAsync();
        }

        public async Task<List<Transaccion>> GetByUsuarioIdAsync(string usuarioId)
        {
            return await db.Transacciones
                .Where(t => t.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<Transaccion?> GetByStripePaymentIntentIdAsync(string paymentIntentId)
        {
            return await db.Transacciones
                .FirstOrDefaultAsync(t => t.StripePaymentIntentId == paymentIntentId);
        }

        public async Task<Transaccion> AddAsync(Transaccion transaccion)
        {
            db.Transacciones.Add(transaccion);
            await db.SaveChangesAsync();
            return transaccion;
        }

        public async Task<Transaccion> UpdateAsync(Transaccion transaccion)
        {
            db.Transacciones.Update(transaccion);
            await db.SaveChangesAsync();
            return transaccion;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var transaccion = await GetByIdAsync(id);
            if (transaccion == null) return false;

            db.Transacciones.Remove(transaccion);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task SaveChangesAsync()
        {
            await db.SaveChangesAsync();
        }
    }
}