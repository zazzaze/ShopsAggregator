using System;
using System.Collections.Generic;

namespace ShopsAggregatorWebApi.Models
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
    }
}