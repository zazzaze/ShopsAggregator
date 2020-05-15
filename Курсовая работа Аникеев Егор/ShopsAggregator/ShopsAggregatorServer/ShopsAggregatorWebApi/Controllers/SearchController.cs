using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ShopsAggregatorWebApi.Models;
using ShopsAggregatorWebApi.Services;

namespace ShopsAggregatorWebApi.Controllers
{
    /// <summary>
    /// Контроллер взаимодействия с поиском.
    /// </summary>
    [Route("api/search")]
    public class SearchController : Controller
    {
        /// <summary>
        /// Сервис базы данных.
        /// </summary>
        private readonly DatabaseContext _service;
        /// <summary>
        /// Конструктор контроллера.
        /// </summary>
        /// <param name="service">Сервис баз данных.</param>
        public SearchController(DatabaseContext service)
        {
            _service = service;
        }

        /// <summary>
        /// Получает пользователей-продавцов, которые содержат введенную строку в имени.
        /// </summary>
        /// <param name="q">Строка поиска.</param>
        /// <returns>Список полученных пользователе-продавцов.</returns>
        [HttpGet]
        public ActionResult<List<String>> SearchSellers(String q)
        {
            var users = _service.SearchSellers(q);
            return users;
        }

        /// <summary>
        /// Получает пользователя-продавца по имени.
        /// </summary>
        /// <param name="sellerName">Имя пользователя-продавца.</param>
        /// <returns>Полученный пользователь-продавец.</returns>
        [HttpGet("getSeller")]
        public Seller GetSellerByName(String sellerName)
        {
            return _service.GetSeller(sellerName);
        }
    }
}