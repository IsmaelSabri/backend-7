using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using RestSharp;
using Users.Models;

namespace Users.Repositories
{
    public class UserCollection : ValidationAttribute, IUserCollection
    {
        internal UserRepository userRepository = new();
        private readonly IMongoCollection<User> Collection;

        public UserCollection()
        {
            Collection = userRepository.mongoDatabase.GetCollection<User>("Users");
        }

        public async Task DeleteUser(string id)
        {
            await Collection.DeleteOneAsync(x => x.Id == id);
        }

        public async Task<User> GetUserById(string id)
        {
            return await Collection.FindAsync(new BsonDocument { { "_id", new ObjectId(id) } }).Result.FirstAsync();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await Collection.FindAsync(s => s.Email == email).Result.FirstAsync();
        }

        public async Task<User> GetUserByUserId(string userId)
        {
            return await Collection.FindAsync(s => s.UserId == userId).Result.FirstAsync();
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await Collection.FindAsync(s => s.Username == username).Result.FirstAsync();
        }

        public async Task<User> GetUserByRefreshToken(string refreshToken)
        {
            return await Collection.FindAsync(s => s.RefreshToken == refreshToken).Result.FirstAsync();
        }

        public async Task<User> GetUserByToken(string token)
        {
            return await Collection.FindAsync(s => s.Token == token).Result.FirstAsync();
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await Collection.FindAsync(new BsonDocument()).Result.ToListAsync();
        }

        public async Task NewUser(User user)
        {
            await Collection.InsertOneAsync(user);
        }

        public async Task UpdateUser(User user)
        {
            var filter = Builders<User>.Filter.Eq(s => s.Id, user.Id);
            await Collection.ReplaceOneAsync(filter, user);
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
                var client = new RestClient("http://host.docker.internal:3030/api/email/setpassword");
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
                var client = new RestClient("http://host.docker.internal:3030/api/email/resendpassword");
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