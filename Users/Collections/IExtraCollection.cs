using Users.Models;

namespace Users.Collections
{
    public interface IExtraCollection
    {
        Task<Extra?> GetByIdAsync(Guid id);
        Task<List<Extra>> GetAllAsync();
        Task<Extra> AddAsync(Extra Extra);
        Task<Extra> UpdateAsync(Extra Extra);
        Task<bool> DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}