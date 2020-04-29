using System;
using System.Collections.Generic;

namespace ShopsAggregatorLib
{
    public class Buyer : User
    {
        public List<String> LikedPosts { get; set; } = new List<String>();

        public Buyer(User user) : base(user)
        {
            
        }
    }
}