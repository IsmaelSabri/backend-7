using Microsoft.EntityFrameworkCore;
using Users.Data;
using Users.Models;
using System.Collections.Generic;

namespace Users.Collections.Impl
{
    public class PlanSubscriptionCollection : IPlanSubscriptionCollection
    {
        private readonly UserDb _db;

        public PlanSubscriptionCollection(UserDb db)
        {
            _db = db;
        }

        public async Task<PlanSubscription?> GetByIdAsync(Guid id)
        {
            return await _db.PlanSubscriptions.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<PlanSubscription>> GetByUserIdAsync(Guid userId)
        {
            return await _db.PlanSubscriptions
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlanSubscription>> GetByOrganizationIdAsync(Guid organizationId)
        {
            return await _db.PlanSubscriptions
                .Where(p => p.OrganizationId == organizationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlanSubscription>> GetAllAsync()
        {
            return await _db.PlanSubscriptions.ToListAsync();
        }

        public async Task<PlanSubscription> AddAsync(PlanSubscription subscription)
        {
            _db.PlanSubscriptions.Add(subscription);
            await _db.SaveChangesAsync();
            return subscription;
        }

        public async Task<PlanSubscription> UpdateAsync(PlanSubscription subscription)
        {
            _db.PlanSubscriptions.Update(subscription);
            await _db.SaveChangesAsync();
            return subscription;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;
            _db.PlanSubscriptions.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
