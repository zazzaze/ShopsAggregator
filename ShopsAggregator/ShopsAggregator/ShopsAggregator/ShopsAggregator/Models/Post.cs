using System;
using System.Collections.Generic;

namespace ShopsAggregator.Models
{
    /// <summary>
    /// Тип записи, которую создает пользователь-продавец
    /// </summary>
    public class Post
    {
        /// <summary>
        /// Строка информации о записи.
        /// </summary>
        private String _info;
        /// <summary>
        /// Id записи
        /// </summary>
        public Int32 Id { get; set; }
        
        /// <summary>
        /// Id создателя записи - пользователя-продавца.
        /// </summary>
        public Int32 CreatorId { get; set; }

        /// <summary>
        /// Устанавливает значение полю _info и в зависимости от информации в этом поле возрвращает данные о записи. 
        /// </summary>
        public String Info
        {
            get => String.IsNullOrEmpty(_info) ? "Пользователь не оставил информацию о товаре" : _info;
            set => _info = value;
        }

        /// <summary>
        /// Возвращает ссылку на картинку к записи на сервере.
        /// </summary>
        public String ImagePath => App.BaseUrl + Id + "photo.jpeg";

        /// <summary>
        /// Id пользователей-покупателей, которым понравилась эта запись.
        /// </summary>
        public List<Int32> Likers { get; set; } = new List<Int32>();

        /// <summary>
        /// Количество пользователей-покупателей, которым понравилась эта запись.
        /// </summary>
        public Int32 LikesCount => Likers.Count;
    }
}