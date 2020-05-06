using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using ShopsAggregatorWebApi.Models;

namespace ShopsAggregatorWebApi.Services
{
    public class ShopsAggregatorService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Post> _posts;
        public ShopsAggregatorService(IShopsAggregatorDatabaseSettings options)
        {
            var client = new MongoClient(options.ConnectionString);
            var dataBase = client.GetDatabase(options.DatabaseName);
            _posts = dataBase.GetCollection<Post>(options.PostsCollectionName);
            _users = dataBase.GetCollection<User>(options.UsersCollectionName);
            BsonClassMap.RegisterClassMap<Buyer>();
            BsonClassMap.RegisterClassMap<Seller>();
        }

        public List<User> GetAllUsers() => _users.Find(_ => true).ToList();

        public User GetUser(String login, String password) => _users.Find(user => 
            (user.Email == login || user.Username == login ) 
            && user.Password == password).FirstOrDefault();

        public User CreateUser(User newUser)
        {
            if (!IsSameUserCreated(newUser))
            {
                _users.InsertOne(newUser);
                return GetUser(newUser.Email, newUser.Password);
            }

            return null;
        }

        public void UpdateUser(User updatedUser)
        {
            _users.ReplaceOne(user => user.Id == updatedUser.Id, updatedUser);
        }

        private Boolean IsSameUserCreated(User user)
        {
            User isCreatedUser = _users.Find(checkingUser => checkingUser.Email == user.Email ||
                                                             checkingUser.Username == user.Username).FirstOrDefault();
            return isCreatedUser != null;
        }

        // public Boolean IsSameUserCreated(String id)
        // {
        //     User isUserCreated = _users.Find(checkingUser => checkingUser.Id == id).FirstOrDefault();
        //     return isUserCreated != null;
        // }
        //
        // public User GetUserById(String id)
        // {
        //     return _users.Find(user => user.Id == id).FirstOrDefault();
        // }
        
        public ActionResult<List<User>> SearchUsers(String line)
        {
            return _users.Find(user => user.Username.Contains(line)).ToList();
        }
    }
}