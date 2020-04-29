using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using ShopsAggregatorWebApi.Services;
using Newtonsoft.Json;
using ShopsAggregatorLib;

namespace ShopsAggregatorWebApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class AuthorizationContoller : ControllerBase
    {
        private const String UsersIconsDirPath = "../usersicons";
        private readonly ShopsAggregatorService _service;
        public AuthorizationContoller(ShopsAggregatorService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<User>> GetAllUsers() => _service.GetAllUsers();
        
        [HttpPost("reg")]
        public ActionResult RegistrateNewUser(Seller user)
        {
            User registredUser = _service.CreateUser(user);
            if (registredUser != null)
            {
                return Ok("Аккаунт успешно создан");
            }

            return BadRequest("Такой аккаунт уже существует!");
        }
        
        [HttpPost("reg")]
        public ActionResult RegistrateNewUser(Buyer user)
        {
            User registredUser = _service.CreateUser(user);
            if (registredUser != null)
            {
                return Ok("Аккаунт успешно создан");
            }

            return BadRequest("Такой аккаунт уже существует!");
        }
        

        private void AddUserIconInDir(User user)
        {
            if (user.IconBytesArr == null)
            {
                user.IconPath = "standarticon.jpeg";
                return;
            }
            if (!Directory.Exists(UsersIconsDirPath))
                Directory.CreateDirectory(UsersIconsDirPath);
            String iconName = user.Username + "icon.jpeg";
            using (FileStream fs = new FileStream(UsersIconsDirPath + $"/{iconName}", FileMode.OpenOrCreate))
            {
                foreach (Int32 oneByte in user.IconBytesArr)
                {
                    fs.WriteByte((Byte)oneByte);
                }
            }

            user.IconBytesArr = null;
            user.IconPath = iconName;
        }

        [HttpGet("auth")]
        public ActionResult<String> TryAuth(String login, String password)
        {
            return JsonConvert.SerializeObject(_service.GetUser(login, password));
        }

        [HttpPut("update")]
        public ActionResult<String> UpdateUser([FromBody]User user)
        {
            AddUserIconInDir(user);
            _service.UpdateUser(user);
            user = _service.GetUser(user.Email, user.Password);
            return JsonConvert.SerializeObject(user);
        }


        [HttpPost]
        public String Test(String name)
        {
            return name;
        }
    }
}