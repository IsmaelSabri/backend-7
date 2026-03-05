using Images.Models;

namespace Core.Dto
{
    /// <summary>
    /// DTO que agrupa imágenes por su tipo (HomeImages, HomeSchemes, EnergyCertImage)
    /// </summary>
    public class HomeImagesDto
    {
        public Image[]? Images { get; set; }
        public Image[]? Schemes { get; set; }
        public Image? EnergyCert { get; set; }
    }
}
