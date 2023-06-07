using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApi.Models
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set;}
        public string Name { get; set;} = string.Empty;
        public int Edad { get; set;}
        public DateTime DateRegistry { get; set;}
    }
}