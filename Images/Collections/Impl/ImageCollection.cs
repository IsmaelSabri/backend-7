using Images.Data;
using Images.Models;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;

namespace Images.Collections.Impl;

public sealed class ImageCollection(ImageDb db) : IImageCollection, ISieveCustomFilterMethods
{
    public async Task<Image?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await db.Images.FindAsync([id], ct);

    public Task<Image?> GetByNameAsync(string name, CancellationToken ct) =>
        db.Images.FirstOrDefaultAsync(image => image.AltText == name, ct);

    public Task<List<Image>> GetByOwnerAsync(string ownerId, string[] ownerTypes, CancellationToken ct) =>
        db.Images.Where(image => image.OwnerId == ownerId && ownerTypes.Contains(image.OwnerType))
            .OrderBy(image => image.DisplayOrder).ToListAsync(ct);

    public async Task AddAsync(Image image, CancellationToken ct)
    {
        db.Images.Add(image);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Image image, CancellationToken ct)
    {
        if (image.IsCover)
        {
            var remaining = await db.Images
                .Where(other => other.OwnerId == image.OwnerId && other.OwnerType == image.OwnerType && other.Id != image.Id)
                .OrderByDescending(other => other.IsCover).ThenBy(other => other.DisplayOrder)
                .ThenBy(other => other.CreatedAt).ThenBy(other => other.Id)
                .FirstOrDefaultAsync(ct);
            if (remaining != null)
                remaining.IsCover = true;
        }
        db.Images.Remove(image);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Image image, CancellationToken ct)
    {
        await db.SaveChangesAsync(ct);
    }

    public async Task<bool> WithLockedImageAsync(Guid id, Func<Image, Task> action, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        var image = await db.Images.FromSqlInterpolated($"SELECT * FROM \"Images\" WHERE \"Id\" = {id} FOR UPDATE")
            .SingleOrDefaultAsync(ct);
        if (image == null) return false;
        await action(image);
        await transaction.CommitAsync(ct);
        return true;
    }

    public IQueryable<Image> GetPagedImages() => db.Images.AsNoTracking();
}
