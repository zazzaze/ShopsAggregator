using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopsAggregatorWebApi.Models
{
    [Table("orders")]
    public class Order
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public Int32 Id { get; set; }
        [Column("postid")]
        public Int32 PostId { get; set; }
        [Column("customerid")]
        public Int32 BuyerId { get; set; }

        [Column("issucceded")]
        public Boolean IsSucceded { get; set; } = false;
        [Column("iscanceled")]
        public Boolean IsCanceled { get; set; }= false;
    }
}