using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShopsAggregatorWebApi.Models;
using ShopsAggregatorWebApi.Services;

namespace ShopsAggregatorWebApi.Controllers
{
    [Route("api/search")]
    public class SearchController : Controller
    {

        private DatabaseContext _service;
        public SearchController(DatabaseContext service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<Seller>> SearchSellers(String q)
        {
            var users = _service.SearchSellers(q);
            if (users != null)
            {
                foreach (var user in users)
                {
                    user.Password = null;
                }
            }

            return users;
        }

        [HttpGet("getSeller")]
        public ActionResult<User> GetSellerById(Int32 id)
        {
            User user = _service.GetSeller(id);
            if (user != null)
                user.Password = null;
            return user;
        }
    }
}