using Microsoft.EntityFrameworkCore;
using Core.Data;
using Core.Models;

namespace Core.Collections.Impl
{
    public class ExtraContentCollection(CoreDb db) : IExtraContentCollection
    {
        private readonly CoreDb db = db;
        
        public async Task Add(ExtraContent extraContent)
        {
            db.ExtraContents.Add(extraContent);
            await db.SaveChangesAsync();
        }

        public async Task<ExtraContent?> GetByIdAsync(Guid id)
        {
            return await db.ExtraContents.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<ExtraContent>> GetByTransaccionIdAsync(string transaccionId)
        {
            return await db.ExtraContents
                .Where(e => e.TransaccionId == transaccionId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExtraContent>> GetByViviendaIdAsync(string viviendaId)
        {
            return await db.ExtraContents
                .Where(e => e.ViviendaId == viviendaId)
                .ToListAsync();
        }

        public async Task UpdateAsync(ExtraContent extraContent)
        {
            extraContent.LastUpdate = DateTime.UtcNow;
            db.ExtraContents.Update(extraContent);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var extraContent = await GetByIdAsync(id);
            if (extraContent != null)
            {
                db.ExtraContents.Remove(extraContent);
                await db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ExtraContent>> GetAllAsync()
        {
            return await db.ExtraContents.ToListAsync();
        }
    }
}