using System;

namespace ShopsAggregator.Models
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
        /// Возвращает текст со статусом заказа.
        /// </summary>
        public String OrderStatus
        {
            get 
            {
                if (this.IsSucceded)
                    return "Заказ одобрен";
                if (this.IsCanceled)
                    return "Заказ отклонен";
                return "Ожидание";
            }
        }
    }
}