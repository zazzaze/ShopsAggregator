using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopsAggregatorWebApi.Models
{
    /// <summary>
    /// Тип пользователя-продавца.
    /// </summary>
    [Table("sellers")]
    public class Seller : User
    {
        /// <summary>
        /// Id записей, созданных пользователем.
        /// </summary>
        [Column("items")]
        public List<Int32> Items { get; set; } = new List<Int32>();
        /// <summary>
        /// Id подписчиков, которые подписались на данного пользователя.
        /// </summary>
        [Column("subscribers")]
        public List<Int32> Subscribers { get; set; } = new List<Int32>();
        
        /// <summary>
        /// Пустой конструктор.
        /// </summary>
        public Seller() : base()
        {
            
        }

        /// <summary>
        /// Конструктор от формы создания пользователя.
        /// </summary>
        /// <param name="data">Форма регистрации.</param>
        public Seller(RegistrationForm data) : base(data)
        {
            
        }
    }
}