using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Users.Models;

namespace Users.Collections
{
    public interface ILineaTransaccionCollection
    {
        Task<LineaTransaccion?> GetByIdAsync(Guid id);
        Task<List<LineaTransaccion>> GetAllAsync();
        Task<List<LineaTransaccion>> GetByTransaccionIdAsync(Guid transaccionId);
        Task<LineaTransaccion> AddAsync(LineaTransaccion linea);
        Task<LineaTransaccion> UpdateAsync(LineaTransaccion linea);
        Task<bool> DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}
