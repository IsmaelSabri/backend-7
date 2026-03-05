using Core.Models;

namespace Core.Collections
{
    public interface IPlanCollection
    {
        Task<Plan?> GetByIdAsync(Guid id);
        Task<List<Plan>> GetAllAsync();
        Task<Plan> AddAsync(Plan plan);
        Task<Plan> UpdateAsync(Plan plan);
        Task<bool> DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}
