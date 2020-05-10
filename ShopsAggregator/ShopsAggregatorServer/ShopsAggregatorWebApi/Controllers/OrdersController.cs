using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ShopsAggregatorWebApi.Models;
using ShopsAggregatorWebApi.Services;

namespace ShopsAggregatorWebApi.Controllers
{
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly DatabaseContext _service;

        public OrdersController(DatabaseContext service)
        {
            _service = service;
        }
        
        [HttpPost("addOrder")]
        public IActionResult AddOrder(Int32 buyerId, Int32 postId)
        {
            _service.AddOrder(buyerId, postId);
            return Ok();
        }

        [HttpGet("getBuyerOrders")]
        public List<BuyerOrderView> GetBuyerOrders(Int32 buyerId)
        {
            return _service.GetBuyerOrders(buyerId);
        }

        [HttpGet("getSellerOrders")]
        public List<SellerOrderView> GetSellerorders(Int32 sellerId)
        {
            return _service.GetSellerOrders(sellerId);
        }

        [HttpPut("setOrderSuccess")]
        public IActionResult SetOrderSuccess(Int32 orderId)
        {
            _service.SetOrderSuccess(orderId);
            return Ok();
        }
        
        [HttpPut("setOrderCanceled")]
        public IActionResult SetOrderCanceled(Int32 orderId)
        {
            _service.SetOrderCanceled(orderId);
            return Ok();
        }

        [HttpDelete("deleteOrder")]
        public IActionResult DeleteOrder(Int32 orderId)
        {
            _service.DeleteOrder(orderId);
            return Ok();
        }
    }
}