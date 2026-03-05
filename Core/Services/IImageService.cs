using Images.Models;
using Core.Dto;

namespace Core.Services
{
    public interface IImageService
    {
        Task<Image?> GetProfileImageByUserIdAsync(string userId);
        Task<HomeImagesDto?> GetHomeImagesByViviendaIdAsync(string viviendaId);
        Task<string?> ComposeImageAsync(Guid imageId);
    }
}

