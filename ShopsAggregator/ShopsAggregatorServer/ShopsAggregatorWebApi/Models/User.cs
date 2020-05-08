using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace ShopsAggregatorWebApi.Models
{
    public class User
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public Int32 Id { get; set; }
        [Column("username")]
        public String Username { get; set; }
        [Column("email")]
        public String Email { get; set; }
        [Column("password")]
        public String Password { get; set; }

        [Column("iconpath")] 
        public String IconPath { get; set; } = "standarticon.jpeg";
        


        public User()
        {

        }

        public User(RegistrationForm data)
        {
            this.Email = data.Email;
            this.Username = data.Username;
            this.Password = data.Password;
        }
    }
}