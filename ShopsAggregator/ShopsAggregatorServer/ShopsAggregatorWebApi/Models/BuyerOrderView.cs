using System;

namespace ShopsAggregatorWebApi.Models
{
    public class BuyerOrderView
    {
        public Int32 OrderId { get; set; }
        public Seller OrderSeller { get; set; }
        public Post SellerPost { get; set; }
        public Boolean IsSucceded { get; set; }
        public Boolean IsCanceled { get; set; }

        public BuyerOrderView()
        {
            
        }

        public BuyerOrderView(Order order, Post post,  Seller seller)
        {
            OrderSeller = seller;
            SellerPost = post;
            IsSucceded = order.IsSucceded;
            IsCanceled = order.IsCanceled;
            OrderId = order.Id;
        }
    }
}