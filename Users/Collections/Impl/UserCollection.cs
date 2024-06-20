using Homes.Data;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using Users.Models;

namespace Users.Repositories
{
    public class UserCollection(UserDb db) : IUserCollection
    {
        private readonly UserDb dbc = db;

        public async Task DeleteUser(User user)
        {
            dbc.Users.Remove(user);
            await dbc.SaveChangesAsync();
        }
        public async Task<User?> GetUserById(string id)
        {
            return await dbc.Users.FindAsync(id);
        }
        public async Task<User?> GetUserByEmail(string email)
        {
            return await db.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetUserByUserId(string userId)
        {
            return await db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
        }
        public async Task<User?> GetUserByUsername(string username)
        {
            return await db.Users.FirstOrDefaultAsync(x => x.Username == username);
        }
        public async Task<User?> GetUserByRefreshToken(string refreshToken)
        {
            return await db.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
        }
        public async Task<User?> GetUserByToken(string token)
        {
            return await db.Users.FirstOrDefaultAsync(x => x.Token == token);
        }
        public async Task<List<User>> GetAllUsers()
        {
            return await dbc.Users.ToListAsync();
        }
        public async Task NewUser(User user)
        {
            dbc.Users.Add(user);
            await dbc.SaveChangesAsync();
        }
        public async Task UpdateUser(User user)
        {
            dbc.Entry(user).State = EntityState.Modified;
            await dbc.SaveChangesAsync();
        }

        public IQueryable<User> GetPagedUsers()
        {
            return dbc.Users.AsQueryable();
        }

        public string GenerateRandomAlphanumericString()
        {
            const string chars = "1234567890";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 17)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public void SendWelcomeEmail(User user)
        {
            try
            {
                var client = new RestClient("https://localhost:4040/api/email/setpassword");
                var request = new RestRequest
                {
                    Method = Method.Get
                };
                request.AddHeader("Content-Type", "application/json");
                request.AddJsonBody(new
                {
                    Name = user.Firstname,
                    ToEmail = user.Email,
                    Subject = "Activar cuenta",
                    Message = user.UserId,
                });
                var response = client.Execute(request);
                Console.WriteLine($"Content response: {response.Content}");
                Console.WriteLine($"Status: {response.StatusCode}");
                if (response.Headers != null)
                {
                    foreach (var header in response.Headers)
                    {
                        Console.WriteLine(header);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SendResetEmail(User user)
        {
            try
            {
                var client = new RestClient("https://localhost:4040/api/email/resendpassword");
                var request = new RestRequest
                {
                    Method = Method.Get
                };
                request.AddHeader("Content-Type", "application/json");
                request.AddJsonBody(new
                {
                    Name = user.Firstname,
                    ToEmail = user.Email,
                    Subject = "Cambiar contraseña",
                    Message = user.UserId,
                });
                var response = client.Execute(request);
                Console.WriteLine($"Content response: {response.Content}");
                Console.WriteLine($"Status: {response.StatusCode}");
                if (response.Headers != null)
                {
                    foreach (var header in response.Headers)
                    {
                        Console.WriteLine(header);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}