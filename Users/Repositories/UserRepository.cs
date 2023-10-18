
using MongoDB.Driver;

namespace Users.Repositories
{
    public class UserRepository
    {
        public MongoClient mongoClient;
        public IMongoDatabase mongoDatabase;
        public UserRepository()
        {
            var mongoUrl = MongoUrl.Create("mongodb://yo:admin@host.docker.internal:27017");
            mongoClient = new MongoClient(mongoUrl);
            mongoDatabase = mongoClient.GetDatabase("users");

            //          host.docker.internal  ---   localhost
        }
    }
}