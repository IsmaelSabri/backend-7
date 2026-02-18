using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Users.Models;

namespace Users.Collections
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
