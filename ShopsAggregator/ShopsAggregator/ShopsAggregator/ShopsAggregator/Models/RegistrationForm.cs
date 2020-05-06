using System;

namespace ShopsAggregator.Models
{
    public class RegistrationForm
    {
        public String Username { get; set; }
        public String Email { get; set; }
        public String Password { get; set; }
        public Boolean IsSeller { get; set; }
    }
}