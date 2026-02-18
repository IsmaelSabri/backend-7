using Microsoft.EntityFrameworkCore;
using Users.Data;
using Users.Models;

namespace Users.Collections.Impl
{
    public class ExtraContentCollection(UserDb db) : IExtraContent
    {
        private readonly UserDb db = db;
        
        public async Task Add(ExtraContent extraSubscription)
        {
            db.ExtraSubscriptions.Add(extraSubscription);
            await db.SaveChangesAsync();
        }

        public async Task<ExtraContent?> GetByIdAsync(Guid id)
        {
            return await db.ExtraSubscriptions.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<ExtraContent>> GetByUsuarioIdAsync(string usuarioId)
        {
            return await db.ExtraSubscriptions
                .Where(e => e.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExtraContent>> GetByLineaTransaccionIdAsync(string lineaTransaccionId)
        {
            return await db.ExtraSubscriptions
                .Where(e => e.LineaTransaccionId == lineaTransaccionId)
                .ToListAsync();
        }

        public async Task UpdateAsync(ExtraContent extraSubscription)
        {
            extraSubscription.LastUpdatedAt = DateTime.UtcNow;
            db.ExtraSubscriptions.Update(extraSubscription);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var extraContenido = await GetByIdAsync(id);
            if (extraContenido != null)
            {
                db.ExtraSubscriptions.Remove(extraContenido);
                await db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ExtraContent>> GetAllAsync()
        {
            return await db.ExtraSubscriptions.ToListAsync();
        }
    }
}