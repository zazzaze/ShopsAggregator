using System;
using System.Collections.Generic;

namespace ShopsAggregatorWebApi.Models
{
    public class IconPostForm
    {
        public List<Int32> IconBytesArr { get; set; } = new List<Int32>();
        public  Int32 ToId { get; set; }
    }
}