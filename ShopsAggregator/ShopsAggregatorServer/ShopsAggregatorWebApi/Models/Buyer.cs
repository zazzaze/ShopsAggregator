using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopsAggregatorWebApi.Models
{
    /// <summary>
    /// Тип пользователя-покупателя.
    /// </summary>
    [Table("buyers")]
    public class Buyer : User
    {
        /// <summary>
        /// Id понравившихся записей в базе данных.
        /// </summary>
        [Column("likedposts")]
        public List<Int32> LikedPosts { get; set; } = new List<Int32>();
        /// <summary>
        /// Id пользователей-продавцов, на которых подписан пользователь.
        /// </summary>
        [Column("subscribed")]
        public List<Int32> Subscribed { get; set; } = new List<Int32>(); 
        
        /// <summary>
        /// Id записей, которые добавли пользователи-продавцы, на которых подписан данный пользователь.
        /// </summary>
        [Column("newposts")]
        public List<Int32> NewPosts { get; set; } = new List<Int32>();

        /// <summary>
        /// Конструктор от формы создания пользователя.
        /// </summary>
        /// <param name="data">Форма регистрации.</param>
        public Buyer(RegistrationForm data) : base(data)
        {
            
        }

        /// <summary>
        /// Пустой конструктор.
        /// </summary>
        public Buyer()
        {
            
        }
    }
}