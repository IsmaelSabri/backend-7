using Microsoft.EntityFrameworkCore;
using Core.Data;
using Core.Models;

namespace Core.Collections.Impl
{
    public class PlanCollection : IPlanCollection
    {
        private readonly CoreDb _db;

        public PlanCollection(CoreDb db)
        {
            _db = db;
        }

        public async Task<Plan?> GetByIdAsync(Guid id)
        {
            return await _db.Plans.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Plan>> GetAllAsync()
        {
            return await _db.Plans.ToListAsync();
        }

        public async Task<Plan> AddAsync(Plan plan)
        {
            _db.Plans.Add(plan);
            await _db.SaveChangesAsync();
            return plan;
        }

        public async Task<Plan> UpdateAsync(Plan plan)
        {
            _db.Plans.Update(plan);
            await _db.SaveChangesAsync();
            return plan;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;
            _db.Plans.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
