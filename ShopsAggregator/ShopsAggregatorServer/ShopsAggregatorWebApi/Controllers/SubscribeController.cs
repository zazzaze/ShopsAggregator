using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ShopsAggregatorWebApi.Models;
using ShopsAggregatorWebApi.Services;

namespace ShopsAggregatorWebApi.Controllers
{
    /// <summary>
    /// Контроллер подписок.
    /// </summary>
    [Route("api/sub")]
    public class SubscribeController : Controller
    {
        /// <summary>
        /// Сервис взаимодействия с базой данных.
        /// </summary>
        private DatabaseContext _service;
        
        /// <summary>
        /// Конструктор контроллера подписок.
        /// </summary>
        /// <param name="service">Сервис взаимодействия с базой данных.</param>
        public SubscribeController(DatabaseContext service)
        {
            _service = service;
        }


        [HttpPut("addSub")]
        public IActionResult AddSubscriberToSeller(Int32 buyerId, String sellerName)
        {
            if (_service.AddSubscriber(buyerId, sellerName))
                return Ok();
            return BadRequest();
        }

        [HttpPut("rmSub")]
        public IActionResult RemoveSellerSubscriber(Int32 buyerId, String sellerName)
        {
            if (_service.RemoveSubscriber(buyerId, sellerName))
                return Ok();
            return BadRequest();
        }
        
    }
}