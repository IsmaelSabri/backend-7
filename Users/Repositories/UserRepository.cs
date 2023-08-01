using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            //mongoClient=new MongoClient("mongodb://yo:admin@localhost:27017");
            //mongoDatabase=mongoClient.GetDatabase("users"); // crear la base de datos si no existiera
        }
    }
}