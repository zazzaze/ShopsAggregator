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
        public ActionResult<List<String>> SearchSellers(String q)
        {
            var users = _service.SearchSellers(q);
            return users;
        }

        [HttpGet("getSeller")]
        public ActionResult<User> GetSellerByName(String sellerName)
        {
            User user = _service.GetSeller(sellerName);
            return user;
        }
    }
}