using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApi.Models;

namespace WebApi.Repositories
{
    public class UserCollection : IUserCollection
    {
        internal UserRepository userRepository = new();
        private IMongoCollection<User> Collection;

        public UserCollection()
        {
            Collection = userRepository.mongoDatabase.GetCollection<User>("Users");
        }

        public async Task DeleteUser(string id)
        {
            var filter = Builders<User>.Filter.Eq(s => s.Id, new ObjectId(id));
            await Collection.DeleteOneAsync(filter);
        }

        public async Task<User> GetUserById(string id)
        {
            return await Collection.FindAsync(new BsonDocument { { "_id", new ObjectId(id) } }).Result.FirstAsync();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await Collection.FindAsync(s => s.Email == email).Result.FirstAsync();
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
    }
}