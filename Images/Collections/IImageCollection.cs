using Images.Models;

namespace Images.Collections
{
    public interface IImageCollection
    {
        Task<Image?> GetImageById(string id);
        Task<Image?> GetByIdAsync(Guid id);
        Task<IEnumerable<Image>> GetAllAsync();
        Task NewImage(Image image);
        Task AddAsync(Image image);
        Task UpdateImage(Image image);
        Task UpdateAsync(Image image);
        Task DeleteImage(Image image);
        Task DeleteAsync(Guid id);
        Task<Image?> GetImageByNameAsync(string imageName);
        Task DeleteImageByNameAsync(string imageName);
        IQueryable<Image> GetPagedImages();

    }
}