using System;
using System.Collections.Generic;

namespace ShopsAggregatorLib
{
    public class Seller : Buyer
    {

        public Seller(User user) : base(user)
        {
            
        }
        public List<String> Items { get; set; } = new List<String>();
    }
}