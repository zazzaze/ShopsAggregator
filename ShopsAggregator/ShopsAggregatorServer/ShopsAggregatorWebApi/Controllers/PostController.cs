using System;
using Microsoft.AspNetCore.Mvc;
using ShopsAggregatorWebApi.Models;

namespace ShopsAggregatorWebApi.Controllers
{
    [Route("api/posts")]
    public class PostController  : ControllerBase
    {

        private DatabaseContext _service;
        public PostController(DatabaseContext service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public IActionResult CreatePost([FromBody]CreatePostForm postForm)
        {
            if (_service.CreatePost(postForm))
                return Ok();
            return BadRequest();
        }
    }
}