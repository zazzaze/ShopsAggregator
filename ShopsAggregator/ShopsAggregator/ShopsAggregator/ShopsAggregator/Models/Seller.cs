using System;
using System.Collections.Generic;

namespace ShopsAggregator.Models
{
    /// <summary>
    /// Тип пользователя-продавца.
    /// </summary>
    public class Seller : User
    {
        /// <summary>
        /// Id записей, созданных пользователем.
        /// </summary>
        public List<Int32> Items { get; set; } = new List<Int32>();
        
        /// <summary>
        /// Id подписчиков, которые подписались на данного пользователя.
        /// </summary>
        public List<Int32> Subscribers { get; set; } = new List<Int32>();
        
    }
}