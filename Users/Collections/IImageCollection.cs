using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Users.Models;

namespace Users.Collections
{
    public interface IImageCollection
    {
        Task<Image?> GetImageById(string id);
        Task NewImage(Image image);
        Task UpdateImage(Image image);
    }
}