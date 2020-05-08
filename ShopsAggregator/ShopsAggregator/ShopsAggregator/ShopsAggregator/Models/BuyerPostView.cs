using System;
using System.Collections.Generic;

namespace ShopsAggregator.Models
{
    public class BuyerPostView
    {
        public Int32 BuyerId { get; set; }
        public String Username { get; set; }
        public Int32 PostId { get; set; }
        public String Info { get; set; }
        public Boolean IsUserLikePost => Likers.Contains(BuyerId);
        public List<Int32> Likers { get; set; } = new List<Int32>();

        public String IconPath => $"{App.BaseUrl}{PostId}photo.jpeg";
        public String SellerIconPath => $"{App.BaseUrl}{Username}icon.jpeg";
        public Boolean IsUserDontLikePost => !IsUserLikePost;
        
        public BuyerPostView(Post post)
        {
            PostId = post.Id;
            Info = post.Info;
            Likers = post.Likers;
        }

        public BuyerPostView()
        {
            
        }
    }
}