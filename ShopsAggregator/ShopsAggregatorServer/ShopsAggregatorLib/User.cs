using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace ShopsAggregatorLib
{
    [Serializable]
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public String Id { get; set; }
        public String Username { get; set; }
        public String Email { get; set; }
        public String Password { get; set; }
        
        public String Info { get; set; }
        
        public Boolean IsSeller { get; set; }
        public Int32[] IconBytesArr { get; set; }
        public String IconPath { get; set; }
        
        public List<String> Subscribers { get; set; } = new List<String>();
        
        public List<String> Subscribed { get; set; } = new List<String>();


        public User()
        {
            
        }
        public User(User user)
        {
            this.Email = user.Email;
            this.Id = user.Id;
            this.Username = user.Username;
            this.Password = user.Password;
            this.IconPath = user.IconPath;
            this.Subscribers = Subscribers;
            this.Subscribed = Subscribed;
        }
    }
}