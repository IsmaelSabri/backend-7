using MongoDB.Driver;
using RestSharp;
using Users.Models;

namespace Users.Repositories
{
    public class UserCollection : IUserCollection
    {
        internal UserRepository userRepository = new();
        private readonly IMongoCollection<User> Collection;
        public UserCollection()
        {
            Collection = userRepository.mongoDatabase.GetCollection<User>("Users");
        }

        public async Task DeleteUser(string id) => await Collection.DeleteOneAsync(x => x.Id == id);
        public async Task<User> GetUserById(string id) => await Collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        public async Task<User> GetUserByEmail(string email) => await Collection.Find(x => x.Email == email).FirstOrDefaultAsync();
        public async Task<User> GetUserByUserId(string userId) => await Collection.Find(x => x.UserId == userId).FirstOrDefaultAsync();
        public async Task<User> GetUserByUsername(string username) => await Collection.Find(x => x.Username == username).FirstOrDefaultAsync();
        public async Task<User> GetUserByRefreshToken(string refreshToken) => await Collection.Find(x => x.RefreshToken == refreshToken).FirstOrDefaultAsync();
        public async Task<User> GetUserByToken(string token) => await Collection.Find(x => x.Token == token).FirstOrDefaultAsync();
        public async Task<List<User>> GetAllUsers() => await Collection.Find(_ => true).ToListAsync();
        public async Task NewUser(User user) => await Collection.InsertOneAsync(user);
        public async Task UpdateUser(User user, string id) => await Collection.ReplaceOneAsync(x => x.Id == id, user);

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