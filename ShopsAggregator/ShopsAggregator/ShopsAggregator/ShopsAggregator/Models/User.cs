using System;

namespace ShopsAggregator.Models
{
    public class User
    {
        private String _iconPath = "standarticon.jpeg";
        public Int32 Id { get; set; }
        public String Username { get; set; }
        public String Email { get; set; }
        public String Password { get; set; }

        public String IconPath { get => $"{App.BaseUrl}{_iconPath}"; set => _iconPath = value; }


        public User()
        {

        }
    }
}