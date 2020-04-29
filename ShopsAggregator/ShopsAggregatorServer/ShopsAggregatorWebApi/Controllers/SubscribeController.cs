using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ShopsAggregatorLib;
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
        private ShopsAggregatorService _service;
        /// <summary>
        /// Конструктор контроллера подписок.
        /// </summary>
        /// <param name="service">Сервис взаимодействия с базой данных.</param>
        public SubscribeController(ShopsAggregatorService service)
        {
            _service = service;
        }
        
        /// <summary>
        /// Добавляет подписчика пользователю.
        /// </summary>
        /// <param name="id">Id подписчика.</param>
        /// <param name="toId">Id пользователь, на которого подписываются.</param>
        /// <returns>Результат подписки.</returns>
        [HttpPut("addSub")]
        public ActionResult<String> AddSubscriber(String id, String toId)
        {
            if (!_service.IsSameUserCreated(toId) || !_service.IsSameUserCreated(id))
            {
                return BadRequest("Не удалось подписаться");
            }
            AddToSubscribedUser(id, toId);
            AddUserToSubscribed(toId, id);
            return Ok("Подписка успешно оформлена");
        }

        /// <summary>
        /// Удаляет подписку на пользователя.
        /// </summary>
        /// <param name="id"> Id пользователя, который отменят подписку.</param>
        /// <param name="fromId">Id пользователя, от которого отписываются.</param>
        /// <returns>Результат отписки.</returns>
        [HttpPut("remSub")]
        public ActionResult<String> RemoveSubscriber(String id, String fromId)
        {
            if (!_service.IsSameUserCreated(fromId) || !_service.IsSameUserCreated(id))
            {
                return BadRequest("Не удалось отписаться");
            }
            RemoveFromSubscribers(id,fromId);
            RemoveFromSubscribed(fromId, id);
            return Ok("Успешно отписан");
        }
        
        /// <summary>
        /// Удаляет подписчика пользователя.
        /// </summary>
        /// <param name="id">Id подписчика.</param>
        /// <param name="from">Id пользователя, у которого необходимо удалить подписку.</param>
        private void RemoveFromSubscribers(String id, String from)
        {
            User user = _service.GetUserById(from);
            if (user == null)
                return;
            if(user.Subscribers == null)
                user.Subscribers = new List<String>();
            user.Subscribers.Remove(id);
            _service.UpdateUser(user);
        }

        /// <summary>
        /// Удаление пользователя из подписаных.
        /// </summary>
        /// <param name="id">Пользователь, от которого отписываются.</param>
        /// <param name="from">Пользователь, отменивший подписку.</param>
        private void RemoveFromSubscribed(String id, String from)
        {
            User user = _service.GetUserById(from);
            if (user == null)
                return;
            if(user.Subscribed == null)
                user.Subscribed = new List<String>();
            user.Subscribed.Remove(id);
            _service.UpdateUser(user);
        }

        /// <summary>
        /// Добавляет подписчика пользователю.
        /// </summary>
        /// <param name="subscriberId">Id добавляемого пользователя.</param>
        /// <param name="toUserId">Id пользователя, на которого подписались.</param>
        private void AddToSubscribedUser(String subscriberId, String toUserId)
        {
            User toUserAdd = _service.GetUserById(toUserId);
            if(toUserAdd.Subscribers == null)
                toUserAdd.Subscribers = new List<String>();
            if (toUserAdd.Subscribers.Contains(subscriberId))
                return;
            toUserAdd.Subscribers.Add(subscriberId);
            _service.UpdateUser(toUserAdd);
        }

        /// <summary>
        /// Добваляет пользователя в подписанных.
        /// </summary>
        /// <param name="userId">Id пользователя, на которого подписались.</param>
        /// <param name="subscriberId">Id подписчика</param>
        private void AddUserToSubscribed(String userId, String subscriberId)
        {
            User subscriber = _service.GetUserById(subscriberId);
            if(subscriber.Subscribed == null)
                subscriber.Subscribed = new List<String>();
            if (subscriber.Subscribed.Contains(userId))
                return;
            subscriber.Subscribed.Add(userId);
            _service.UpdateUser(subscriber);
        }
    }
}