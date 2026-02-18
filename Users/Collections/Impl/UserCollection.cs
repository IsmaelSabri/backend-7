using Users.Data;
using Microsoft.EntityFrameworkCore;
using Users.Models;
using System.Linq.Dynamic.Core;
using AutoMapper;
using Sieve.Services;
using Users.Services;

namespace Users.Collections.impl
{
    public class UserCollection(UserDb db, IImageService imageService) : IUserCollection, ISieveCustomFilterMethods
    {
        private readonly UserDb db = db;
        private readonly IImageService imageService = imageService;

        public async Task DeleteUser(User user)
        {
            // el historial de pedidos no se puede borrar por ley
            // var model = await db.Users.OrderBy(e => e.Username).Include(e => e.ProfileImage).Include(e => e.BrandImage).FirstAsync();
            // db.Remove(model);
            db.Users.Remove(user);
            await db.SaveChangesAsync();
        }
        public async Task<User?> GetUserById(string id)
        {
            var user = await db.Users.FirstOrDefaultAsync(m => m.Id.Equals(id));
            if (user != null)
            {
                await InitializeProfileImageAsync(user);
            }
            return user;
        }
        public async Task<User?> GetUserByEmail(string email)
        {
            var user = await db.Users
                .FirstOrDefaultAsync(x => x.Email == email);
            if (user != null)
            {
                await InitializeProfileImageAsync(user);
            }
            return user;
        }

        public async Task<User?> GetUserByUserId(string userId)
        {
            var user = await db.Users
                .FirstOrDefaultAsync(x => x.UserId == userId);
            if (user != null)
            {
                await InitializeProfileImageAsync(user);
            }
            return user;
        }
        public async Task<User?> GetUserByUsername(string username)
        {
            var user = await db.Users
                .FirstOrDefaultAsync(x => x.Username == username);
            if (user != null)
            {
                await InitializeProfileImageAsync(user);
            }
            return user;
        }
        public async Task<User?> GetUserByRefreshToken(string refreshToken)
        {
            var user = await db.Users
                .FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
            if (user != null)
            {
                await InitializeProfileImageAsync(user);
            }
            return user;
        }
        public async Task<User?> GetUserByToken(string token)
        {
            var user = await db.Users
                .FirstOrDefaultAsync(x => x.Token == token);
            if (user != null)
            {
                await InitializeProfileImageAsync(user);
            }
            return user;
        }
        public async Task<List<User>> GetAllUsers()
        {
            var users = await db.Users
                .ToListAsync();
            
            // Inicializar imágenes para todos los usuarios
            foreach (var user in users)
            {
                await InitializeProfileImageAsync(user);
            }
            
            return users;
        }
        public async Task NewUser(User user)
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }

        public async Task UpdateUser(User user)
        {
            // debería actualizar las orders[]
            db.Update(user);
            await db.SaveChangesAsync();
        }

        public IQueryable<User> GetPagedUsers()
        {
            return db.Users
            .AsQueryable();
        }

        public IQueryable<User> GetBlockYou(IQueryable<User> source, string op, string[] values)
        {
            var blocked = values.First();
            return source.Where(p => p.BlockedUsers.Contains(blocked));
        }

        private async Task InitializeProfileImageAsync(User user)
        {
            if (user == null)
                return;

            try
            {
                var image = await imageService.GetProfileImageByUserIdAsync(user.Id);
                if (image != null)
                {
                    user.ProfileImage = image;
                }
            }
            catch (Exception ex)
            {
                // Log o manejar el error sin romper el flujo
                System.Console.WriteLine($"Error initializing profile image for user {user.Id}: {ex.Message}");
            }
        }
    }
}