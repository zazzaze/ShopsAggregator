using System;
using System.Collections.Generic;

namespace ShopsAggregator.Models
{
    /// <summary>
    /// Модель данных записи для пользователя-покупателя.
    /// </summary>
    public class BuyerPostView
    {
        /// <summary>
        /// Id пользователя-покупателя.
        /// </summary>
        public Int32 BuyerId { get; set; }
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
        /// Понравилась ли запись данному пользователю-покупателю.
        /// </summary>
        public Boolean IsUserLikePost => Likers.Contains(BuyerId);
        
        /// <summary>
        /// Пользователи-покупатели, которым понравилась запись.
        /// </summary>
        public List<Int32> Likers { get; set; } = new List<Int32>();

        /// <summary>
        /// Ссылка на фотографию к записи.
        /// </summary>
        public String IconPath => $"{App.BaseUrl}{PostId}photo.jpeg";
        /// <summary>
        /// Сслыка на иконку пользователя-продавца, опубликовавшего эту запись.
        /// </summary>
        public String SellerIconPath => $"{App.BaseUrl}{Username}icon.jpeg";
        
        /// <summary>
        /// Не ставил ли пользователь-покупатель лайк.
        /// </summary>
        public Boolean IsUserDontLikePost => !IsUserLikePost;
        
        /// <summary>
        /// Конструктор модели данных в зависимости от экземпляра записи.
        /// </summary>
        /// <param name="post">Экземпляр записи.</param>
        public BuyerPostView(Post post)
        {
            PostId = post.Id;
            Info = post.Info;
            Likers = post.Likers;
        }

        /// <summary>
        /// Пустой конструктор.
        /// </summary>
        public BuyerPostView()
        {
            
        }
    }
}