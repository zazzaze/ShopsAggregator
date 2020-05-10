using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace ShopsAggregatorWebApi.Models
{
    /// <summary>
    /// Общий тип пользователей.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Id пользователя.
        /// </summary>
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public Int32 Id { get; set; }
        /// <summary>
        /// Имя пользователя.
        /// </summary>
        [Column("username")]
        public String Username { get; set; }
        /// <summary>
        /// Email пользователя.
        /// </summary>
        [Column("email")]
        public String Email { get; set; }
        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        [Column("password")]
        public String Password { get; set; }
        
        /// <summary>
        /// Имя файла с иконкой пользователя.
        /// </summary>
        [Column("iconpath")] 
        public String IconPath { get; set; } = "standarticon.jpeg";
        /// <summary>
        /// Информация о пользователе.
        /// </summary>
        [Column("info")]
        public String Info { get; set; }

        /// <summary>
        /// Id заказов пользователя.
        /// </summary>
        [Column("orders")]
        public List<Int32> Orders { get; set; } = new List<Int32>();
        
        /// <summary>
        /// Пустой конструктор. 
        /// </summary>
        public User()
        {

        }
        /// <summary>
        /// Конструктор от формы создания пользователя.
        /// </summary>
        /// <param name="data">Форма регистрации.</param>
        public User(RegistrationForm data)
        {
            this.Email = data.Email;
            this.Username = data.Username;
            this.Password = data.Password;
        }
    }
}