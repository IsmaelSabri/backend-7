using Microsoft.EntityFrameworkCore;
using Core.Data;
using Core.Models;

namespace Core.Collections.Impl
{
    public class OrganizationCollection : IOrganizationCollection
    {
        private readonly CoreDb _db;

        public OrganizationCollection(CoreDb db)
        {
            _db = db;
        }

        public async Task<Organization?> GetByIdAsync(Guid id)
        {
            return await _db.Organizations.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Organization>> GetAllAsync()
        {
            return await _db.Organizations.ToListAsync();
        }

        public async Task<Organization> AddAsync(Organization organization)
        {
            _db.Organizations.Add(organization);
            await _db.SaveChangesAsync();
            return organization;
        }

        public async Task<Organization> UpdateAsync(Organization organization)
        {
            _db.Organizations.Update(organization);
            await _db.SaveChangesAsync();
            return organization;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;
            _db.Organizations.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
