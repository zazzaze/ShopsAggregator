using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopsAggregatorWebApi.Models
{
    /// <summary>
    /// Экземпляр типа заказ.
    /// </summary>
    [Table("orders")]
    public class Order
    {
        /// <summary>
        /// Id заказа.
        /// </summary>
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public Int32 Id { get; set; }
        /// <summary>
        /// Id записи, на которую создали запись.
        /// </summary>
        [Column("postid")]
        public Int32 PostId { get; set; }
        /// <summary>
        /// Id пользователя-покупателя, который создал заказ.
        /// </summary>
        [Column("customerid")]
        public Int32 BuyerId { get; set; }
        /// <summary>
        /// Подтвердил ли пользователь-продавец заказ.
        /// </summary>
        [Column("issucceded")]
        public Boolean IsSucceded { get; set; } = false;
        /// <summary>
        /// Отклонил ли пользователь-продавец заказ.
        /// </summary>
        [Column("iscanceled")]
        public Boolean IsCanceled { get; set; }= false;
    }
}