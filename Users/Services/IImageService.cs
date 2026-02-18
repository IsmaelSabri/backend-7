using Images.Models;

namespace Users.Services
{
    public interface IImageService
    {
        Task<Image?> GetProfileImageByUserIdAsync(Guid userId);
    }
}
