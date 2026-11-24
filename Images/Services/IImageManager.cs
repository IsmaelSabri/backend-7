using Images.Dto;
using Sieve.Models;

namespace Images.Services;

public interface IImageManager
{
    Task<ImageResponse?> GetAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<ImageResponse>> QueryAsync(SieveModel model, CancellationToken ct);
    Task<ImageResponse> UploadAsync(IFormFile file, string ownerId, string ownerType, CancellationToken ct);
    Task<bool> UpdateAsync(Guid id, ImageUpdateRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<bool> DeleteByNameAsync(string name, CancellationToken ct);
}
