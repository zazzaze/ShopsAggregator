using System;

namespace ShopsAggregatorWebApi.Models
{
    /// <summary>
    /// Модель заказа для пользователя-покупателя.
    /// </summary>
    public class SellerOrderView
    {
        /// <summary>
        /// Id заказа.
        /// </summary>
        public Int32 OrderId { get; set; }
        /// <summary>
        /// Экземпляр типа пользователя, который сделал заказ.
        /// </summary>
        public Buyer OrderBuyer { get; set; }
        /// <summary>
        /// Экземпляр типа записи, которую заказал пользователь.
        /// </summary>
        public Post CurrentPost { get; set; }
        /// <summary>
        /// Одобрил ли пользователь-продавец заказ.
        /// </summary>
        public Boolean IsSucceded { get; set; }
        /// <summary>
        /// Отклонил ли пользователь-продавец заказ.
        /// </summary>
        public Boolean IsCanceled { get; set; }

        /// <summary>
        /// Пустой конструктор.
        /// </summary>
        public SellerOrderView()
        {
            
        }

        /// <summary>
        /// Конструктор от заказа, записи и пользователя-покупателя.
        /// </summary>
        /// <param name="order">Экземпляр типа заказа.</param>
        /// <param name="post">Экземпляр типа записи.</param>
        /// <param name="buyer">Экземпляр типа пользователя-покупателя.</param>
        public SellerOrderView(Order order, Post post, Buyer buyer)
        {
            OrderBuyer = buyer;
            CurrentPost = post;
            IsSucceded = order.IsSucceded;
            IsCanceled = order.IsCanceled;
            OrderId = order.Id;
        }
    }
}