using Core.Models;
namespace Core.Collections
{
    public interface IExtraContentCollection
    {
        Task Add(ExtraContent extraContent);
        Task<ExtraContent?> GetByIdAsync(Guid id);
        Task<IEnumerable<ExtraContent>> GetByTransaccionIdAsync(string transaccionId);
        Task<IEnumerable<ExtraContent>> GetByViviendaIdAsync(string viviendaId);
        Task UpdateAsync(ExtraContent extraContent);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<ExtraContent>> GetAllAsync();
    }
}