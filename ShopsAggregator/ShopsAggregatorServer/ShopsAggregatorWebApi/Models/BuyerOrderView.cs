using System;

namespace ShopsAggregatorWebApi.Models
{
    /// <summary>
    /// Модель данных для заказа пользователя-покупателя.
    /// </summary>
    public class BuyerOrderView
    {
        /// <summary>
        /// Id заказа.
        /// </summary>
        public Int32 OrderId { get; set; }
        /// <summary>
        /// Экземпляр пользователя-покупателя, который создал запись.
        /// </summary>
        public Seller OrderSeller { get; set; }
        /// <summary>
        /// Запись, которую опубликовал пользователь-продавец.
        /// </summary>
        public Post SellerPost { get; set; }
        /// <summary>
        /// Одобрил ли пользователь-продавец заказ.
        /// </summary>
        public Boolean IsSucceded { get; set; }
        /// <summary>
        /// Отклонил ли пользователь-продавец заказ.
        /// </summary>
        public Boolean IsCanceled { get; set; }

        /// <summary>
        /// Пустой конструкток.
        /// </summary>
        public BuyerOrderView()
        {
            
        }

        /// <summary>
        /// Конструктор от записи, заказа и пользователя-продавца.
        /// </summary>
        /// <param name="order">Экземпляр типа заказа.</param>
        /// <param name="post">Экземпляр типа записи.</param>
        /// <param name="seller">Экземпляр типа пользователя-продавца.</param>
        public BuyerOrderView(Order order, Post post,  Seller seller)
        {
            OrderSeller = seller;
            SellerPost = post;
            IsSucceded = order.IsSucceded;
            IsCanceled = order.IsCanceled;
            OrderId = order.Id;
        }
    }
}