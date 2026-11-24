using Images.Collections;
using Images.Dto;
using Images.Models;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace Images.Services.Impl;

public sealed class ImageManager(
    IImageCollection collection,
    IImageStorageService storage,
    SieveProcessor sieve,
    ILogger<ImageManager> logger) : IImageManager
{
    public async Task<ImageResponse?> GetAsync(Guid id, CancellationToken ct)
    {
        var image = await collection.GetByIdAsync(id, ct);
        return image == null ? null : ToResponse(image);
    }

    public async Task<IReadOnlyList<ImageResponse>> QueryAsync(SieveModel model, CancellationToken ct)
    {
        var query = collection.GetPagedImages()
            .OrderBy(image => image.Status == ImageStatuses.Ready ? 0 : image.Status == ImageStatuses.Processing ? 1 : 2)
            .ThenByDescending(image => image.IsCover)
            .ThenBy(image => image.DisplayOrder).ThenBy(image => image.CreatedAt).ThenBy(image => image.Id);
        var images = await sieve.Apply(model, query).ToListAsync(ct);
        return images.Select(ToResponse).ToList();
    }

    public async Task<ImageResponse> UploadAsync(IFormFile file, string ownerId, string ownerType, CancellationToken ct)
    {
        ValidateOwner(ownerId, 64);
        ValidateOwner(ownerType, 32);
        if (file.Length <= 0 || file.Length > 32L * 1024 * 1024)
            throw new ArgumentException("El archivo debe tener entre 1 byte y 32 MB.");

        var name = Path.GetFileName(file.FileName);
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El archivo debe tener un nombre valido.");
        var existing = await collection.GetByOwnerAsync(ownerId, [ownerType], ct);
        using var stream = file.OpenReadStream();
        var key = await storage.UploadAsync(stream, ownerType, ownerId, ct);
        try
        {
            var image = new Image
            {
                Id = Guid.NewGuid(),
                StorageKey = key,
                AltText = name.Length <= 150 ? name : name[..150],
                OwnerId = ownerId,
                OwnerType = ownerType,
                Status = ImageStatuses.Processing,
                DisplayOrder = existing.Count == 0 ? 0 : checked(existing.Max(item => item.DisplayOrder) + 1),
                IsCover = existing.Count == 0
            };
            await collection.AddAsync(image, ct);
            return ToResponse(image);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "No se pudo confirmar el registro de {StorageKey}; se conserva el original por si se guardo", key);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(Guid id, ImageUpdateRequest request, CancellationToken ct)
    {
        var image = await collection.GetByIdAsync(id, ct);
        if (image == null) return false;
        if (request.IsCover)
        {
            var group = await collection.GetByOwnerAsync(image.OwnerId, [image.OwnerType], ct);
            foreach (var other in group.Where(other => other.Id != image.Id && other.IsCover))
                other.IsCover = false;
        }
        image.AltText = request.AltText;
        image.DisplayOrder = request.Order;
        image.IsCover = request.IsCover;
        await collection.UpdateAsync(image, ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        return await collection.WithLockedImageAsync(id, image => DeleteAsync(image, ct), ct);
    }

    public async Task<bool> DeleteByNameAsync(string name, CancellationToken ct)
    {
        var image = await collection.GetByNameAsync(name, ct);
        if (image == null) return false;
        return await DeleteAsync(image.Id, ct);
    }

    private async Task DeleteAsync(Image image, CancellationToken ct)
    {
        await storage.DeleteAsync(image.StorageKey, ct);
        await collection.DeleteAsync(image, ct);
    }

    private ImageResponse ToResponse(Image image)
    {
        var urls = image.Status == ImageStatuses.Ready
            ? storage.GetUrls(image.StorageKey)
            : new ImageUrls(null, null, null, null, null);
        return new ImageResponse(image.Id, urls.ThumbnailUrl, urls.CardUrl, urls.FullUrl,
            image.DisplayOrder, image.IsCover, image.OwnerId, image.OwnerType, image.AltText,
            urls.AvatarUrl, urls.CropperUrl, image.Status);
    }

    private static void ValidateOwner(string value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > maxLength
            || !value.All(c => char.IsAsciiLetterOrDigit(c) || c is '-' or '_'))
            throw new ArgumentException("El propietario o su tipo no son validos.");
    }
}
