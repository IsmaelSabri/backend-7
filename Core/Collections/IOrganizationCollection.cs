using Core.Models;

namespace Core.Collections
{
    public interface IOrganizationCollection
    {
        Task<Organization?> GetByIdAsync(Guid id);
        Task<IEnumerable<Organization>> GetAllAsync();
        Task<Organization> AddAsync(Organization organization);
        Task<Organization> UpdateAsync(Organization organization);
        Task<bool> DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}
