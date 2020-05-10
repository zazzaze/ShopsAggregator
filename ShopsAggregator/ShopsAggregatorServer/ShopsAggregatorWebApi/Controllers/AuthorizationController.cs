using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ShopsAggregatorWebApi.Services;
using ShopsAggregatorWebApi.Models;

namespace ShopsAggregatorWebApi.Controllers
{
    /// <summary>
    /// Контроллер взаимодействия с пользователями.
    /// </summary>
    [Route("api/users")]
    [ApiController]
    public class AuthorizationContoller : ControllerBase
    {
        /// <summary>
        /// Сервис базы данных.
        /// </summary>
        private readonly DatabaseContext _service;
        /// <summary>
        /// Конструктор контроллера.
        /// </summary>
        /// <param name="service">Сервис баз данных.</param>
        public AuthorizationContoller(DatabaseContext service)
        {
            _service = service;
        }

        /// <summary>
        /// Метод получения всех пользователей.
        /// </summary>
        /// <returns>Список всех пользователей.</returns>
        [HttpGet]
        public ActionResult<List<User>> GetAllUsers() => _service.GetAllUsers();
        
        /// <summary>
        /// Регистрирует нового пользователя.
        /// </summary>
        /// <param name="user">Форма регистрации пользователя.</param>
        /// <returns>Результат регистрации.</returns>
        [HttpPost("reg")]
        public ActionResult RegistrateNewUser([FromBody]RegistrationForm user)
        {
            User registeredUser = null;
            if (user.IsSeller)
                registeredUser = _service.CreateUser(new Seller(user));
            else
               registeredUser = _service.CreateUser(new Buyer(user));
            if (registeredUser != null)
            {
                return Ok("Аккаунт успешно создан");
            }
        
            return BadRequest("Такой аккаунт уже существует!");
        }
        
        /// <summary>
        /// Аутентификация пользователя-покупателя.
        /// </summary>
        /// <param name="login">Логин для входа.</param>
        /// <param name="password">Пароль для входа.</param>
        /// <returns>Экземпляр пользователя.</returns>
        [HttpGet("authBuyer")]
        public ActionResult<User>TryAuthBuyer(String login, String password)
        {
            return _service.GetBuyer(login, password);
        }
        
        /// <summary>
        /// Аутентификация пользователя-продавца.
        /// </summary>
        /// <param name="login">Логин для входа.</param>
        /// <param name="password">Пароль для входа.</param>
        /// <returns>Экземпляр пользователя.</returns>
        [HttpGet("authSeller")]
        public ActionResult<User> TryAuthSeller(String login, String password)
        {
            return _service.GetSeller(login, password);
        }
        
        /// <summary>
        /// Добавление иконки пользователю-продавцу.
        /// </summary>
        /// <param name="icon">Форма добавления иконки.</param>
        /// <returns>Результат добавления.</returns>
        [HttpPut("addSellerIcon")]
        public IActionResult UpdateSellerIcon(IconPostForm icon)
        {
            if (_service.AddIconToSeller(icon, icon.ToId))
                return Ok();
            return BadRequest();
        }

        /// <summary>
        /// Добавление иконки пользователю-покупателю.
        /// </summary>
        /// <param name="icon">Форма добавления иконки.</param>
        /// <returns>Результат добавления.</returns>
        [HttpPut("addBuyerIcon")]
        public IActionResult UpdateBuyerIcon([FromBody]IconPostForm icon)
        {
            if (_service.AddIconToBuyer(icon, icon.ToId))
                return Ok();
            return BadRequest();
        }

        /// <summary>
        /// Обновляют информацию о пользователе-покупателе.
        /// </summary>
        /// <param name="buyerId">Id пользователя-покупателя.</param>
        /// <param name="info">Новая информация о пользователе.</param>
        /// <returns>Результат обновления.</returns>
        [HttpPut("setBuyerInfo")]
        public IActionResult SetBuyerInfo(Int32 buyerId, String info)
        {
            _service.SetBuyerInfo(buyerId, info);
            return Ok();
        }
        
        /// <summary>
        /// Обновляют информацию о пользователе-продавце.
        /// </summary>
        /// <param name="sellerId">Id пользователя-продавца.</param>
        /// <param name="info">Новая информация о пользователе.</param>
        /// <returns>Результат обновления.</returns>
        [HttpPut("setSellerInfo")]
        public IActionResult SetSellerInfo(Int32 sellerId, String info)
        {
            _service.SetSellerInfo(sellerId, info);
            return Ok();
        }
    }
}