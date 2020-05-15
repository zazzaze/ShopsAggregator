using System;

namespace ShopsAggregator.Models
{
    /// <summary>
    /// Общий тип пользователей.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Поле значения иконки пользователя.
        /// </summary>
        private String _iconPath = "standarticon.jpeg";
        /// <summary>
        /// Id пользователя.
        /// </summary>
        public Int32 Id { get; set; }
        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public String Username { get; set; }
        /// <summary>
        /// Email пользователя.
        /// </summary>
        public String Email { get; set; }
        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        public String Password { get; set; }
        /// <summary>
        /// Информация о пользователе.
        /// </summary>
        public String Info { get; set; }
        /// <summary>
        /// Устанавливает имя иконки пользователя на сервер и возвращает ссылку на него.
        /// </summary>
        public String IconPath { get => $"{App.BaseUrl}{_iconPath}"; set => _iconPath = value; }

        
        /// <summary>
        /// Пустой конструктор. 
        /// </summary>
        public User()
        {

        }
    }
}