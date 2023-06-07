using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace WebApi.Repositories
{
    public class UserRepository
    {
        public MongoClient mongoClient;
        public IMongoDatabase mongoDatabase;
        public UserRepository()
        {
            mongoClient=new MongoClient("mongodb://yo:admin@localhost:27017");
            mongoDatabase=mongoClient.GetDatabase("users"); // crear la base de datos si no existiera
        }
    }
}