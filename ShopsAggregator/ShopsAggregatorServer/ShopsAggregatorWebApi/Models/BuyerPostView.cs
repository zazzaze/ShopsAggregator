using System;
using System.Collections.Generic;

namespace ShopsAggregatorWebApi.Models
{
    /// <summary>
    /// Модель данных записи для пользователя-покупателя.
    /// </summary>
    public class BuyerPostView
    {
        /// <summary>
        /// Имя пользователя-продавца, который опубликовал запись.
        /// </summary>
        public String Username { get; set; }
        /// <summary>
        /// Id записи.
        /// </summary>
        public Int32 PostId { get; set; }
        /// <summary>
        /// Информация о записи.
        /// </summary>
        public String Info { get; set; }

        /// <summary>
        /// Пользователи-покупатели, которым понравилась запись.
        /// </summary>
        public List<Int32> Likers { get; set; } = new List<Int32>();

        /// <summary>
        /// Конструктор от записи и пользователя-продавца.
        /// </summary>
        /// <param name="post">Экземпляр типа записи.</param>
        /// <param name="creator">Экземпляр типа пользователя-продавца.</param>
        public BuyerPostView(Post post, Seller creator)
        {
            Username = creator.Username;
            PostId = post.Id;
            Info = post.Info;
            Likers = post.Likers;
        }
    }
}