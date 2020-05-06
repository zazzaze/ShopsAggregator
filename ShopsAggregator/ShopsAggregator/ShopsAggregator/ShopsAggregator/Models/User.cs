using System;

namespace ShopsAggregator.Models
{
    public class User
    {
        public Int32 Id { get; set; }
        public String Username { get; set; }
        public String Email { get; set; }
        public String Password { get; set; }
        
        public String IconPath { get; set; } = "standarticon.jpeg";


        public User()
        {

        }
    }
}