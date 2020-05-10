using System;
using System.Collections.Generic;

namespace ShopsAggregator.Models
{
    /// <summary>
    /// Тип пользователя-покупателя.
    /// </summary>
    public class Buyer : User
    {
        /// <summary>
        /// Id понравившихся записей в базе данных.
        /// </summary>
        public List<Int32> LikedPosts { get; set; } = new List<Int32>();
    
        /// <summary>
        /// Id пользователей-продавцов, на которых подписан пользователь.
        /// </summary>
        public List<Int32> Subscribed { get; set; } = new List<Int32>();
        
        /// <summary>
        /// Id записей, которые добавли пользователи-продавцы, на которых подписан данный пользователь.
        /// </summary>
        public List<Int32> NewPosts { get; set; } = new List<Int32>();

        /// <summary>
        /// Пустой конструктор.
        /// </summary>
        public Buyer()
        {
            
        }
    }
}