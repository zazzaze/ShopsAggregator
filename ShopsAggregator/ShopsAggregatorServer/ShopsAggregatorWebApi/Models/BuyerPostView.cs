using System;
using System.Collections.Generic;

namespace ShopsAggregatorWebApi.Models
{
    public class BuyerPostView
    {
        public String Username { get; set; }
        public Int32 PostId { get; set; }
        public String Info { get; set; }

        public List<Int32> Likers { get; set; } = new List<Int32>();

        public BuyerPostView(Post post, Seller creator)
        {
            Username = creator.Username;
            PostId = post.Id;
            Info = post.Info;
            Likers = post.Likers;
        }
    }
}