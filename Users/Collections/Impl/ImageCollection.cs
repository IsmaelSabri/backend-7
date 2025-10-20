using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Users.Data;
using Users.Models;

namespace Users.Collections.Impl
{
    public class ImageCollection(UserDb db) : IImageCollection
    {
        private readonly UserDb db = db;

        public async Task<Image?> GetImageById(string id)
        {
            return await db.Images.FindAsync(id);
        }

        public async Task NewImage(Image image)
        {
            db.Images.Add(image);
            await db.SaveChangesAsync();
        }

        public async Task UpdateImage(Image image)
        {
            db.Images.Update(image);
            //db.Entry(image).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }

        public async Task DeleteImage(Image image)
        {
            db.Images.Remove(image);
            await db.SaveChangesAsync();
        }
    }
}