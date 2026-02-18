using Microsoft.EntityFrameworkCore;
using Users.Data;

namespace Users.Collections.Impl
{
    public class ExtraCollection : IExtraCollection
    {
        private readonly UserDb _db;

        public ExtraCollection(UserDb db)
        {
            _db = db;
        }

        public async Task<Models.Extra?> GetByIdAsync(Guid id)
        {
            return await _db.Extras.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Models.Extra>> GetAllAsync()
        {
            return await _db.Extras.ToListAsync();
        }

        public async Task<Models.Extra> AddAsync(Models.Extra Extra)
        {
            _db.Extras.Add(Extra);
            await _db.SaveChangesAsync();
            return Extra;
        }

        public async Task<Models.Extra> UpdateAsync(Models.Extra Extra)
        {
            _db.Extras.Update(Extra);
            await _db.SaveChangesAsync();
            return Extra;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var Extra = await GetByIdAsync(id);
            if (Extra == null)
            {
                return false;
            }
            _db.Extras.Remove(Extra);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
