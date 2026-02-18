using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Users.Models;

namespace Users.Collections
{
    public interface IPlanSubscriptionCollection
    {
        Task<PlanSubscription?> GetByIdAsync(Guid id);
        Task<IEnumerable<PlanSubscription>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<PlanSubscription>> GetByOrganizationIdAsync(Guid organizationId);
        Task<IEnumerable<PlanSubscription>> GetAllAsync();
        Task<PlanSubscription> AddAsync(PlanSubscription subscription);
        Task<PlanSubscription> UpdateAsync(PlanSubscription subscription);
        Task<bool> DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}
