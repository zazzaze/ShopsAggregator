using System;

namespace ShopsAggregator.Models
{
    /// <summary>
    /// Форма для регистрации пользователя в сервисе.
    /// </summary>
    public class RegistrationForm
    {
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
        /// Является ли пользователь пользователем-продавцом.
        /// </summary>
        public Boolean IsSeller { get; set; }
    }
}