using Microsoft.EntityFrameworkCore;
using Users.Data;
using Users.Models;

namespace Users.Collections.Impl
{
    public class LineaTransaccionCollection : ILineaTransaccionCollection
    {
        private readonly UserDb _db;

        public LineaTransaccionCollection(UserDb db)
        {
            _db = db;
        }

        public async Task<LineaTransaccion?> GetByIdAsync(Guid id)
        {
            return await _db.LineasTransaccion.FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<List<LineaTransaccion>> GetAllAsync()
        {
            return await _db.LineasTransaccion.ToListAsync();
        }

        public async Task<List<LineaTransaccion>> GetByTransaccionIdAsync(Guid transaccionId)
        {
            return await _db.LineasTransaccion
                .Where(l => l.TransaccionId == transaccionId)
                .ToListAsync();
        }

        public async Task<LineaTransaccion> AddAsync(LineaTransaccion linea)
        {
            _db.LineasTransaccion.Add(linea);
            await _db.SaveChangesAsync();
            return linea;
        }

        public async Task<LineaTransaccion> UpdateAsync(LineaTransaccion linea)
        {
            _db.LineasTransaccion.Update(linea);
            await _db.SaveChangesAsync();
            return linea;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var linea = await GetByIdAsync(id);
            if (linea == null) return false;
            _db.LineasTransaccion.Remove(linea);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
