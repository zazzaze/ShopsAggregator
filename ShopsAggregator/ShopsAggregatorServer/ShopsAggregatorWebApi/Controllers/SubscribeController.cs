using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ShopsAggregatorWebApi.Models;
using ShopsAggregatorWebApi.Services;

namespace ShopsAggregatorWebApi.Controllers
{
    /// <summary>
    /// Контроллер взаимодействия с подписками.
    /// </summary>
    [Route("api/sub")]
    public class SubscribeController : Controller
    {
        /// <summary>
        /// Сервис базы данных.
        /// </summary>
        private readonly DatabaseContext _service;
        /// <summary>
        /// Конструктор контроллера.
        /// </summary>
        /// <param name="service">Сервис баз данных.</param>
        public SubscribeController(DatabaseContext service)
        {
            _service = service;
        }

        /// <summary>
        /// Добавляет подписчика пользователя-покупателя к пользователю-продавцу.
        /// </summary>
        /// <param name="buyerId">Id пользователя-покупателя.</param>
        /// <param name="sellerName">Id пользователя-продавца.</param>
        /// <returns>Результат подписки.</returns>
        [HttpPut("addSub")]
        public IActionResult AddSubscriberToSeller(Int32 buyerId, String sellerName)
        {
            if (_service.AddSubscriber(buyerId, sellerName))
                return Ok();
            return BadRequest();
        }

        /// <summary>
        /// Удаляет из подписчиков пользователя-покупателя у пользователю-продавцу.
        /// </summary>
        /// <param name="buyerId">Id пользователя-покупателя.</param>
        /// <param name="sellerName">Id пользователя-продавца.</param>
        /// <returns>Результат отписки.</returns>
        [HttpPut("rmSub")]
        public IActionResult RemoveSellerSubscriber(Int32 buyerId, String sellerName)
        {
            if (_service.RemoveSubscriber(buyerId, sellerName))
                return Ok();
            return BadRequest();
        }
        
    }
}