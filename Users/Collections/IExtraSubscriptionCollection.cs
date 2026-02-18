using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Users.Models;
namespace Users.Collections
{
    public interface IExtraContent
    {
        Task Add(ExtraContent extraSubscription);
        Task<ExtraContent?> GetByIdAsync(Guid id);
        Task<IEnumerable<ExtraContent>> GetByUsuarioIdAsync(string usuarioId);
        Task<IEnumerable<ExtraContent>> GetByLineaTransaccionIdAsync(string lineaTransaccionId);
        Task UpdateAsync(ExtraContent extraSubscription);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<ExtraContent>> GetAllAsync();
    }
}