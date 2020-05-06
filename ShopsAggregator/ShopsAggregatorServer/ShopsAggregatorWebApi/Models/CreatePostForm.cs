using System;
using System.Collections.Generic;

namespace ShopsAggregatorWebApi.Models
{
    public class CreatePostForm
    {
        public Int32 CreatorId { get; set; }
        public String Info { get; set; } = String.Empty;
        public List<Int32> ImageBytes { get; set; }
    }
}