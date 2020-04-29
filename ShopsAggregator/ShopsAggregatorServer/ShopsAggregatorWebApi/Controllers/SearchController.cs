using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShopsAggregatorLib;
using ShopsAggregatorWebApi.Services;

namespace ShopsAggregatorWebApi.Controllers
{
    [Route("api/search")]
    public class SearchController : Controller
    {

        private ShopsAggregatorService _service;
        public SearchController(ShopsAggregatorService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<User>> SearchUser(String q)
        {
            var users = _service.SearchUsers(q).Value;
            if (users != null)
            {
                foreach (var user in users)
                {
                    user.Password = null;
                }
            }

            return users;
        }

        [HttpGet("getUser")]
        public ActionResult<User> GetUserById(String id)
        {
            User user = _service.GetUserById(id);
            if (user != null)
                user.Password = null;
            return user;
        }
    }
}