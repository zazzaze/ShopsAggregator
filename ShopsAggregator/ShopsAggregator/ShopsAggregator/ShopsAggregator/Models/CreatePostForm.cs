using System;
using System.Collections.Generic;

namespace ShopsAggregator.Models
{
    /// <summary>
    /// Модель для создания записи, которая отправляется на сервер.
    /// </summary>
    public class CreatePostForm
    {
        /// <summary>
        /// Id пользователя-продавца, который создает запись.
        /// </summary>
        public Int32 CreatorId { get; set; }
        /// <summary>
        /// Информация о записи.
        /// </summary>
        public String Info { get; set; }
        /// <summary>
        /// Массив байтов картинки записи.
        /// </summary>
        public List<Int32> ImageBytes { get; set; } = new List<Int32>();
        
        /// <summary>
        /// Создает экземпляр с id пользователя-продавца.
        /// </summary>
        /// <param name="id">Id пользователя-продавца.</param>
        public CreatePostForm(Int32 id)
        {
            CreatorId = id;
        }
    }
}