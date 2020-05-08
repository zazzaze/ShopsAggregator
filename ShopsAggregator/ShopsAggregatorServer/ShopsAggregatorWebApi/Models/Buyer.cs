using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopsAggregatorWebApi.Models
{
    [Table("buyers")]
    public class Buyer : User
    {
        [Column("likedposts")]
        public List<Int32> LikedPosts { get; set; } = new List<Int32>();
        [Column("subscribed")]
        public List<Int32> Subscribed { get; set; } = new List<Int32>(); 
        
        [Column("newposts")]
        public List<Int32> NewPosts { get; set; } = new List<Int32>();

        public Buyer(RegistrationForm data) : base(data)
        {
            
        }

        public Buyer()
        {
            
        }
    }
}