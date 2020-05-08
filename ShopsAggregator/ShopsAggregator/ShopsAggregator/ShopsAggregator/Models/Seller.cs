using System;
using System.Collections.Generic;

namespace ShopsAggregator.Models
{
    public class Seller : User
    {
        public List<Int32> Items { get; set; } = new List<Int32>();
        
        public List<Int32> Subscribers { get; set; } = new List<Int32>();
        
    }
}