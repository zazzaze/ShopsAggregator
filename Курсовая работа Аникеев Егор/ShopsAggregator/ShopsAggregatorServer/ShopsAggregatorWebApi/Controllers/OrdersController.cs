using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ShopsAggregatorWebApi.Models;
using ShopsAggregatorWebApi.Services;

namespace ShopsAggregatorWebApi.Controllers
{
    /// <summary>
    /// Контроллер взаимодействия с заказами.
    /// </summary>
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        /// <summary>
        /// Сервис базы данных.
        /// </summary>
        private readonly DatabaseContext _service;
        /// <summary>
        /// Конструктор контроллера.
        /// </summary>
        /// <param name="service">Сервис баз данных.</param>
        public OrdersController(DatabaseContext service)
        {
            _service = service;
        }
        
        /// <summary>
        /// Добавляет новую запись.
        /// </summary>
        /// <param name="buyerId">Id пользователя-покупателя.</param>
        /// <param name="postId">Id записи.</param>
        /// <returns>Резульат добавления.</returns>
        [HttpPost("addOrder")]
        public IActionResult AddOrder(Int32 buyerId, Int32 postId)
        {
            _service.AddOrder(buyerId, postId);
            return Ok();
        }

        /// <summary>
        /// Получает список заказов пользователя-покупателя.
        /// </summary>
        /// <param name="buyerId">Id пользователя-покупателя.</param>
        /// <returns>Список заказов.</returns>
        [HttpGet("getBuyerOrders")]
        public List<BuyerOrderView> GetBuyerOrders(Int32 buyerId)
        {
            return _service.GetBuyerOrders(buyerId);
        }

        /// <summary>
        /// Получает список заказов пользователя-продавца.
        /// </summary>
        /// <param name="sellerId">Id пользователя-продавца.</param>
        /// <returns>Список заказов.</returns>
        [HttpGet("getSellerOrders")]
        public List<SellerOrderView> GetSellerorders(Int32 sellerId)
        {
            return _service.GetSellerOrders(sellerId);
        }

        /// <summary>
        /// Ставит одобренный статус заказу.
        /// </summary>
        /// <param name="orderId">Id заказа, которому устанавливается статус.</param>
        /// <returns>Резльтат изменения статуса.</returns>
        [HttpPut("setOrderSuccess")]
        public IActionResult SetOrderSuccess(Int32 orderId)
        {
            _service.SetOrderSuccess(orderId);
            return Ok();
        }
        
        /// <summary>
        /// Ставит отклоненный статус заказу.
        /// </summary>
        /// <param name="orderId">Id заказа, которому устанавливается статус.</param>
        /// <returns>Резльтат изменения статуса.</returns>
        [HttpPut("setOrderCanceled")]
        public IActionResult SetOrderCanceled(Int32 orderId)
        {
            _service.SetOrderCanceled(orderId);
            return Ok();
        }

        /// <summary>
        /// Удаляет заказ.
        /// </summary>
        /// <param name="orderId">Id заказа, который удаляют.</param>
        /// <returns>Результат удаления.</returns>
        [HttpDelete("deleteOrder")]
        public IActionResult DeleteOrder(Int32 orderId)
        {
            _service.DeleteOrder(orderId);
            return Ok();
        }
    }
}