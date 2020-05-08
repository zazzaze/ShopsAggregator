using System;
using System.Collections.Generic;

namespace ShopsAggregator.Models
{
    public class Post
    {
        public Int32 Id { get; set; }
        
        public Int32 CreatorId { get; set; }
        public String Info { get; set; }

        public String ImagePath => App.BaseUrl+Id + "photo.jpeg";
        
        
        public List<Int32> Comments { get; set; } = new List<Int32>();
        
        public List<Int32> Likers { get; set; } = new List<Int32>();
    }
}