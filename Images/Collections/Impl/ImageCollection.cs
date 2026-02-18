using Microsoft.EntityFrameworkCore;
using Images.Data;
using Images.Models;
using Sieve.Services;

namespace Images.Collections.Impl
{
    public class ImageCollection(ImageDb db) : IImageCollection, ISieveCustomFilterMethods
    {
        private readonly ImageDb db = db;

        public async Task<Image?> GetImageById(string id)
        {
            return await db.Images.FindAsync(id);
        }

        public async Task<Image?> GetByIdAsync(Guid id)
        {
            return await db.Images.FindAsync(id);
        }

        public async Task<IEnumerable<Image>> GetAllAsync()
        {
            return await db.Images.ToListAsync();
        }

        public async Task NewImage(Image image)
        {
            db.Images.Add(image);
            await db.SaveChangesAsync();
        }

        public async Task AddAsync(Image image)
        {
            db.Images.Add(image);
            await db.SaveChangesAsync();
        }

        public async Task UpdateImage(Image image)
        {
            db.Images.Update(image);
            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Image image)
        {
            db.Images.Update(image);
            await db.SaveChangesAsync();
        }

        public async Task DeleteImage(Image image)
        {
            db.Images.Remove(image);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var image = await GetByIdAsync(id);
            if (image != null)
            {
                db.Images.Remove(image);
                await db.SaveChangesAsync();
            }
        }

        public async Task<Image?> GetImageByNameAsync(string imageName)
        {
            if (string.IsNullOrWhiteSpace(imageName))
                return null;

            return await db.Images.FirstOrDefaultAsync(i => i.ImageName == imageName);
        }

        public async Task DeleteImageByNameAsync(string imageName)
        {
            var image = await GetImageByNameAsync(imageName);
            if (image != null)
            {
                db.Images.Remove(image);
                await db.SaveChangesAsync();
            }
        }

        public IQueryable<Image> GetPagedImages()
        {
            return db.Images
            .AsQueryable();
        }
    }
}