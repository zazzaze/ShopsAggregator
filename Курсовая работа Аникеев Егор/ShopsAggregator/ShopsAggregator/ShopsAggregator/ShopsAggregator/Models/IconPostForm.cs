using System;
using System.Collections.Generic;

namespace ShopsAggregator.Models
{
    /// <summary>
    /// Экземпляр типа установки иконки пользователю.
    /// </summary>
    public class IconPostForm
    {
        /// <summary>
        /// Байты фотографии, выбранной пользователем.
        /// </summary>
        public List<Int32> IconBytesArr { get; set; }
        
        /// <summary>
        /// Id пользователя, который изменяет иконку.
        /// </summary>
        public Int32 ToId { get; set; }
        
        /// <summary>
        /// Пустой конструктор.
        /// </summary>
        public IconPostForm()
        {
            
        }

        /// <summary>
        /// Конструктор, который создает экземпляр с id пользователя.
        /// </summary>
        /// <param name="toId">Id пользователя</param>
        public IconPostForm(Int32 toId)
        {
            this.ToId = toId;
        }
    }
}