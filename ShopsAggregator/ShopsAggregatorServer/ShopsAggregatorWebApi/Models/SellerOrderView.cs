using System;

namespace ShopsAggregatorWebApi.Models
{
    public class SellerOrderView
    {
        public Int32 OrderId { get; set; }
        public Buyer OrderBuyer { get; set; }
        public Post CurrentPost { get; set; }
        public Boolean IsSucceded { get; set; }
        public Boolean IsCanceled { get; set; }

        public SellerOrderView()
        {
            
        }

        public SellerOrderView(Order order, Post post, Buyer buyer)
        {
            OrderBuyer = buyer;
            CurrentPost = post;
            IsSucceded = order.IsSucceded;
            IsCanceled = order.IsCanceled;
            OrderId = order.Id;
        }
    }
}