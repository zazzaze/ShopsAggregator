using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ShopsAggregatorWebApi.Models;
using ShopsAggregatorWebApi.Services;

namespace ShopsAggregatorWebApi.Controllers
{
    /// <summary>
    /// Контроллер взаимодействия с записями.
    /// </summary>
    [Route("api/posts")]
    public class PostController  : ControllerBase
    {
        /// <summary>
        /// Сервис базы данных.
        /// </summary>
        private readonly DatabaseContext _service;
        /// <summary>
        /// Конструктор контроллера.
        /// </summary>
        /// <param name="service">Сервис баз данных.</param>
        public PostController(DatabaseContext service)
        {
            _service = service;
        }

        /// <summary>
        /// Создает новую запись.
        /// </summary>
        /// <param name="postForm">Форма для создания записи.</param>
        /// <returns>Результат создания записи.</returns>
        [HttpPost("create")]
        public IActionResult CreatePost([FromBody]CreatePostForm postForm)
        {
            if (_service.CreatePost(postForm))
                return Ok();
            return BadRequest();
        }

        /// <summary>
        /// Получает запись по id.
        /// </summary>
        /// <param name="id">Id записи.</param>
        /// <returns>полученная запись.</returns>
        [HttpGet("getPost")]
        public Post GetPostById(Int32 id)
        {
            return _service.GetPostById(id);
        }

        /// <summary>
        /// Получает записи, опубликованные пользователем-продавцом.
        /// </summary>
        /// <param name="sellerName">Имя пользователя-продавца.</param>
        /// <returns>Список записей пользователя-продавца.</returns>
        [HttpGet("getSellerPosts")]
        public List<Post> GetPostsByCreatorId(String sellerName)
        {
            return _service.GetPostsByCreatorName(sellerName);
        }

        /// <summary>
        /// Получает новые записи пользователя-покупателя.
        /// </summary>
        /// <param name="buyerId">Id пользователя-покупателя.</param>
        /// <returns>Список новых записей пользователя-покупателя.</returns>
        [HttpGet("getBuyerNewPost")]
        public List<BuyerPostView> GetBuyerNewPosts(Int32 buyerId)
        {
            return _service.GetBuyerNewPosts(buyerId);
        }

        /// <summary>
        /// Добавляет лайк пользователя-покупателя к записи.
        /// </summary>
        /// <param name="likerId">Id пользователя-покупателя.</param>
        /// <param name="postId">Id записи.</param>
        /// <returns>Результат добавления лайка.</returns>
        [HttpPut("addLike")]
        public IActionResult AddLikeToPost(Int32 likerId, Int32 postId)
        {
            _service.AddLikeToPost(likerId, postId);
            return Ok();
        }

        /// <summary>
        /// Удаляет лайк пользователя-покупателя у записи.
        /// </summary>
        /// <param name="likerId">Id пользователя-покупателя.</param>
        /// <param name="postId">Id записи.</param>
        /// <returns>Результат удаления лайка.</returns>
        [HttpPut("removeLike")]
        public IActionResult RemoveLikeFromPost(Int32 likerId, Int32 postId)
        {
            _service.RemoveLikeFromPost(likerId, postId);
            return Ok();
        }

        /// <summary>
        /// Получает записи, которые понравились пользователю-покупателю.
        /// </summary>
        /// <param name="buyerId">Id пользователя-покупателя.</param>
        /// <returns>Список понравившихся записей.</returns>
        [HttpGet("getBuyerLikedPosts")]
        public List<BuyerPostView> GetBuyerLikedPosts(Int32 buyerId)
        {
            return _service.GetBuyerLikedPosts(buyerId);
        }
    }
}