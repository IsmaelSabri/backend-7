using Images.Models;

namespace Images.Collections;

public interface IImageCollection
{
    Task<Image?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Image?> GetByNameAsync(string name, CancellationToken ct);
    Task<List<Image>> GetByOwnerAsync(string ownerId, string[] ownerTypes, CancellationToken ct);
    Task AddAsync(Image image, CancellationToken ct);
    Task UpdateAsync(Image image, CancellationToken ct);
    Task DeleteAsync(Image image, CancellationToken ct);
    Task<bool> WithLockedImageAsync(Guid id, Func<Image, Task> action, CancellationToken ct);
    IQueryable<Image> GetPagedImages();
}
