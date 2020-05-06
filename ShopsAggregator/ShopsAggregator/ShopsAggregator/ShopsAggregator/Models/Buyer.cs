using System;
using System.Collections.Generic;

namespace ShopsAggregator.Models
{
    public class Buyer : User
    {
        public List<Int32> LikedPosts { get; set; } = new List<Int32>();
    
        public List<Int32> Subscribed { get; set; } = new List<Int32>();

        public Buyer()
        {
            
        }
    }
}