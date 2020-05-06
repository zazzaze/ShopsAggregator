using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using ShopsAggregatorWebApi.Services;
using Newtonsoft.Json;
using ShopsAggregatorWebApi.Models;

namespace ShopsAggregatorWebApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class AuthorizationContoller : ControllerBase
    {
        private readonly DatabaseContext _service;
        public AuthorizationContoller(DatabaseContext service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<User>> GetAllUsers() => _service.GetAllUsers();
        
        [HttpPost("reg")]
        public ActionResult RegistrateNewUser([FromBody]RegistrationForm user)
        {
            User registredUser = null;
            if (user.IsSeller)
                registredUser = _service.CreateUser(new Seller(user));
            else
               registredUser = _service.CreateUser(new Buyer(user));
            if (registredUser != null)
            {
                return Ok("Аккаунт успешно создан");
            }
        
            return BadRequest("Такой аккаунт уже существует!");
        }
        
        [HttpGet("authBuyer")]
        public ActionResult<User>TryAuthBuyer(String login, String password)
        {
            return _service.GetBuyer(login, password);
        }
        
        [HttpGet("authSeller")]
        public ActionResult<User> TryAuthSeller(String login, String password)
        {
            return _service.GetSeller(login, password);
        }
        
        [HttpPut("addSellerIcon")]
        public IActionResult UpdateSellerIcon(IconPostForm icon)
        {
            if (_service.AddIconToSeller(icon, icon.ToId))
                return Ok();
            return BadRequest();
        }

        [HttpPut("addBuyerIcon")]
        public IActionResult UpdateBuyerIcon([FromBody]IconPostForm icon)
        {
            if (_service.AddIconToBuyer(icon, icon.ToId))
                return Ok();
            return BadRequest();
        }
        
        
        [HttpPost]
        public String Test(String name)
        { 
            return name;
        }
    }
}