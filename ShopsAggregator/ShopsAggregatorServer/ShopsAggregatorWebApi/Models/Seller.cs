using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopsAggregatorWebApi.Models
{
    [Table("sellers")]
    public class Seller : User
    {
        [Column("items")]
        public List<Int32> Items { get; set; } = new List<Int32>();
        [Column("subscribers")]
        public List<Int32> Subscribers { get; set; } = new List<Int32>();
        

        public Seller() : base()
        {
            
        }

        public Seller(RegistrationForm data) : base(data)
        {
            
        }
    }
}