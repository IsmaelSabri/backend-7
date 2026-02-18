using AutoMapper;
using Images.Models;

namespace Images.Profiles
{
    public class ImageProfile : Profile
    {
        public ImageProfile()
        {
            // Mapeos entre Image y DTOs (si los necesitas en el futuro)
            CreateMap<Image, Image>().ReverseMap();
        }
    }
}
