using System;

namespace ShopsAggregator.Models
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