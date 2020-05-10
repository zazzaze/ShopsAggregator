using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ShopsAggregatorWebApi.Models;
using ShopsAggregatorWebApi.Services;

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

        [HttpGet("getPost")]
        public Post GetPostById(Int32 id)
        {
            return _service.GetPostById(id);
        }

        [HttpGet("getSellerPosts")]
        public List<Post> GetPostsByCreatorId(String sellerName)
        {
            return _service.GetPostsByCreatorId(sellerName);
        }

        [HttpGet("getBuyerNewPost")]
        public List<BuyerPostView> GetBuyerNewPosts(Int32 buyerId)
        {
            return _service.GetBuyerNewPosts(buyerId);
        }
        
        [HttpGet]

        [HttpPut("addLike")]
        public IActionResult AddLikeToPost(Int32 likerId, Int32 postId)
        {
            _service.AddLikeToPost(likerId, postId);
            return Ok();
        }

        [HttpPut("removeLike")]
        public IActionResult RemoveLikeFromPost(Int32 likerId, Int32 postId)
        {
            _service.RemoveLikeFromPost(likerId, postId);
            return Ok();
        }

        [HttpGet("getBuyerLikedPosts")]
        public List<BuyerPostView> GetBuyerLikedPosts(Int32 buyerId)
        {
            return _service.GetBuyerLikedPosts(buyerId);
        }
    }
}