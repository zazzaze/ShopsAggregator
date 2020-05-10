using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ShopsAggregatorWebApi.Models;

namespace ShopsAggregatorWebApi.Services
{
    public class DatabaseContext : DbContext
    {
        private const String UsersIconsDirPath = "../images";
        private DbSet<Seller> Sellers { get; set; }
        private DbSet<Buyer> Buyers { get; set; }
        
        private DbSet<Post> Posts { get; set; }

        private DbSet<Comment> Comments { get; set; }

        private DbSet<Order> Orders { get; set; }
        
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            
        }

        public List<User> GetAllUsers()
        {
            List<User> users = Sellers.ToList().ConvertAll(x => (User) x);
            users.AddRange(Buyers.ToList().ConvertAll(x => (User)x));
            return users;
        }

        public User CreateUser(Buyer user)
        {
            if (IsSameSellerCreated(user) || IsSameBuyerCreated(user))
                return null;
            Buyers.Add(user);
            SaveChangesAsync();
            return user;
        }

        public User CreateUser(Seller user)
        {
            if (IsSameSellerCreated(user) || IsSameBuyerCreated(user))
                return null;
            Sellers.Add(user);
            SaveChangesAsync();
            return user;
        }

        private Boolean IsSameSellerCreated(User user)
        {
            return (from seller in Sellers.ToList() where seller.Username == user.Username 
                                                          || seller.Email == user.Email select seller)
                .FirstOrDefault() != null;
        }

        private Boolean IsSameBuyerCreated(User user)
        {
            return (from buyer in Buyers.ToList() where buyer.Username == user.Username 
                                                   || buyer.Email == user.Email select buyer)
                .FirstOrDefault() != null;
        }

        private Boolean IsSameBuyerCreated(Int32 id)
        {
            return (from buyer in Buyers.ToList() where buyer.Id == id select buyer).FirstOrDefault() != null;
        }
        
        public Buyer GetBuyer(string login, string password)
        {
            return (from buyer in Buyers.ToList()
                where (buyer.Username == login || buyer.Username == login)
                      && buyer.Password == password
                select buyer).FirstOrDefault();
        }

        private Buyer GetBuyer(Int32 id)
        {
            return (from buyer in Buyers.ToList() where buyer.Id == id select buyer).FirstOrDefault();
        }

        public Seller GetSeller(string login, string password)
        {
            return (from seller in Sellers.ToList()
                where (seller.Username == login || seller.Username == login)
                      && seller.Password == password
                select seller).FirstOrDefault();
        }

        private Seller GetSeller(Int32 id)
        {
            return (from seller in Sellers.ToList() where seller.Id == id select seller).FirstOrDefault();
        }

        public Seller GetSeller(String sellerName)
        {
            Seller currentSeller = (from seller in Sellers.ToList() where seller.Username == sellerName select seller)
                .FirstOrDefault();
            if (currentSeller == null)
                return null;
            currentSeller.Id = 0;
            currentSeller.Password = "";
            return currentSeller;
        }

        public Boolean AddIconToSeller(IconPostForm icon, Int32 sellerId)
        {
            Seller toSeller =
                (from seller in Sellers.ToList() where seller.Id == sellerId select seller).FirstOrDefault();
            if (toSeller == null)
                return false;
            try
            {
                AddIconToUser(icon, toSeller);
            }
            catch (Exception e)
            {
                return false;
            }
            toSeller.IconPath = $"{toSeller.Username}icon.jpeg";
            SaveChangesAsync();
            return true;
        }

        public Boolean AddIconToBuyer(IconPostForm icon, Int32 buyerId)
        {
            Buyer toBuyer = (from buyer in Buyers.ToList() where buyer.Id == buyerId select buyer).FirstOrDefault();
            if (toBuyer == null)
                return false;
            try
            {
                AddIconToUser(icon, toBuyer);
            }
            catch (Exception e)
            {
                return false;
            }

            toBuyer.IconPath = $"{toBuyer.Username}icon.jpeg";
            SaveChangesAsync();
            return true;
        }

        private void AddIconToUser(IconPostForm icon, User toUser)
        {
            if (!Directory.Exists(UsersIconsDirPath))
                Directory.CreateDirectory(UsersIconsDirPath);
            using (FileStream fs = new FileStream(UsersIconsDirPath + $"/{toUser.Username}icon.jpeg",
                FileMode.OpenOrCreate,
                FileAccess.Write))
            {
                foreach (Int32 iconByte in icon.IconBytesArr)
                {
                    fs.WriteByte((Byte) iconByte);
                }
            }
        }

        public List<String> SearchSellers(String s)
        {
            return (from seller in Sellers.ToList() where seller.Username.Contains(s) select seller.Username).ToList();
        }

        public Boolean AddSubscriber(Int32 buyerId, String sellerName)
        {
            Seller currentSeller = (from seller in Sellers.ToList() where seller.Username == sellerName select seller)
                .FirstOrDefault();
            if (currentSeller == null)
                return false;
            return AddSubscriber(buyerId, currentSeller.Id);
        }
        
        private Boolean AddSubscriber(Int32 buyerId, Int32 sellerId)
        {
            Buyer buyer = GetBuyer(buyerId);
            Seller seller = GetSeller(sellerId);
            if(buyer == null || seller == null)
                return false;
            if(buyer.Subscribed == null)
                buyer.Subscribed = new List<Int32>();
            if(seller.Subscribers == null)
                seller.Subscribers = new List<Int32>();
            buyer.Subscribed.Add(sellerId);
            seller.Subscribers.Add(buyerId);
            SaveChangesAsync();
            return true;
        }

        public Boolean RemoveSubscriber(Int32 buyerId, String sellerName)
        {
            Seller currentSeller = (from seller in Sellers.ToList() where seller.Username == sellerName select seller)
                .FirstOrDefault();
            if (currentSeller == null)
                return false;
            return RemoveSubscriber(buyerId, currentSeller.Id);
        }

        private Boolean RemoveSubscriber(Int32 buyerId, Int32 sellerId)
        {
            Buyer buyer = GetBuyer(buyerId);
            Seller seller = GetSeller(sellerId);
            if(buyer == null || seller == null)
                return false;
            if (buyer.Subscribed == null || seller.Subscribers == null)
                return false;
            buyer.Subscribed.Remove(sellerId);
            seller.Subscribers.Remove(buyerId);
            SaveChangesAsync();
            return true;
        }

        public Boolean CreatePost(CreatePostForm postForm)
        {
            if (postForm.ImageBytes == null || postForm.ImageBytes.Count == 0 || postForm.CreatorId < 0)
                return false;
            Post newPost = new Post();
            Posts.Add(newPost);
            SaveChanges();
            try
            {
                newPost.CreatePostFromForm(postForm);
            }
            catch (Exception)
            {
                Posts.Remove(newPost);
                return false;
            }

            Seller creator = GetSeller(postForm.CreatorId);
            if (creator == null)
                return false;
            if(creator.Items == null)
                creator.Items = new List<Int32>();
            creator.Items.Insert(0,newPost.Id);
            AddPostForSubscribers(creator, newPost.Id);
            SaveChangesAsync();
            return true;
        }

        private void AddPostForSubscribers(Seller creator, Int32 postId)
        {
            if(creator.Subscribers == null)
                creator.Subscribers = new List<Int32>();
            foreach (Int32 buyerId in creator.Subscribers)
            {
                Buyer subscriber = (from buyer in Buyers.ToList() where buyer.Id == buyerId select buyer).FirstOrDefault();
                if(subscriber == null)
                    continue;
                if(subscriber.NewPosts == null)
                    subscriber.NewPosts = new List<Int32>();
                subscriber.NewPosts.Insert(0,postId);
            }
        }

        public Post GetPostById(Int32 id)
        {
            return (from post in Posts.ToList() where post.Id == id select post).FirstOrDefault();
        }
        
        public List<Post> GetPostsByCreatorId(String sellerName)
        {
            Int32 creatorId = (from seller in Sellers.ToList() where seller.Username == sellerName select seller.Id)
                .FirstOrDefault();
            return (from post in Posts.ToList() where post.CreatorId == creatorId select post).ToList();
        }

        public List<BuyerPostView> GetBuyerNewPosts(Int32 buyerId)
        {
            Buyer buyer = GetBuyer(buyerId);
            if(buyer.NewPosts == null)
                buyer.NewPosts = new List<Int32>();
            List<BuyerPostView> newPosts = new List<BuyerPostView>();
            foreach (Int32 postId in buyer.NewPosts)
            {
                Post post = GetPostById(postId);
                Seller seller = GetSeller(post.CreatorId);
                newPosts.Insert(0, new BuyerPostView(post, seller));
            }
            return newPosts;
        }

        public void AddLikeToPost(Int32 likerId, Int32 postId)
        {
            Post post = GetPostById(postId);
            Buyer liker = GetBuyer(likerId);
            if (liker == null)
                return;
            if(post.Likers == null)
                post.Likers = new List<Int32>();
            if(liker.LikedPosts == null)
                liker.LikedPosts = new List<Int32>();
            if(!liker.LikedPosts.Contains(postId))
                liker.LikedPosts.Add(postId);
            if(!post.Likers.Contains(likerId))
                post.Likers.Add(likerId);
            SaveChangesAsync();
        }

        public void RemoveLikeFromPost(Int32 likerId, Int32 postId)
        {
            Post post = GetPostById(postId);
            Buyer liker = GetBuyer(likerId);
            if (liker == null)
                return;
            if(post.Likers == null)
                post.Likers = new List<Int32>();
            if(liker.LikedPosts == null)
                liker.LikedPosts = new List<Int32>();
            liker.LikedPosts.Remove(postId);
            post.Likers.Remove(likerId);
            SaveChangesAsync();
        }

        public List<BuyerPostView> GetBuyerLikedPosts(Int32 buyerId)
        {
            Buyer buyer = GetBuyer(buyerId);
            if (buyer == null)
                return null;
            if(buyer.LikedPosts == null)
                buyer.LikedPosts = new List<Int32>();
            List<BuyerPostView> posts = new List<BuyerPostView>();
            foreach (Int32 postId in buyer.LikedPosts)
            {
                Post post = GetPostById(postId);
                Seller seller = GetSeller(post.CreatorId);
                posts.Add(new BuyerPostView(post, seller));
            }

            return posts;
        }

        public void AddOrder(Int32 buyerId, Int32 postId)
        {
            Buyer buyer = GetBuyer(buyerId);
            Post post = GetPostById(postId);
            if (buyer == null || post == null)
                return;
            Seller seller = GetSeller(post.CreatorId);
            if (seller == null)
                return;
            Order newOrder = new Order
            {
                PostId = post.Id,
                BuyerId = buyer.Id
            };
            Orders.Add(newOrder);
            SaveChanges();
            if(buyer.Orders == null)
                buyer.Orders = new List<Int32>();
            buyer.Orders.Add(newOrder.Id);
            if(seller.Orders == null)
                seller.Orders = new List<Int32>();
            seller.Orders.Add(newOrder.Id);
            SaveChangesAsync();
        }

        public List<BuyerOrderView> GetBuyerOrders(Int32 buyerId)
        {
            List<Order> orders = (from order in Orders.ToList() where order.BuyerId == buyerId select order).ToList();
            List<BuyerOrderView> views = new List<BuyerOrderView>();
            foreach (Order order in orders)
            {
                Post post = GetPostById(order.PostId);
                Seller seller = GetSeller(post.CreatorId);
                views.Add(new BuyerOrderView(order, post, seller));
            }

            return views;
        }

        public List<SellerOrderView> GetSellerOrders(Int32 sellerId)
        {
            Seller seller = GetSeller(sellerId);
            List<Order> orders = (from order in Orders.ToList() where seller.Orders.Contains(order.Id) select order)
                .ToList();
            List<SellerOrderView> views = new List<SellerOrderView>();
            foreach (Order order in orders)
            {
                Buyer buyer = GetBuyer(order.BuyerId);
                Post post = GetPostById(order.PostId);
                views.Add(new SellerOrderView(order, post, buyer));
            }

            return views;
        }

        private Order GetOrder(Int32 orderId)
        {
            return (from order in Orders.ToList() where order.Id == orderId select order).FirstOrDefault();
        }
        
        public void SetOrderSuccess(Int32 orderId)
        {
            Order current = GetOrder(orderId);
            if (current == null)
                return;
            current.IsSucceded = true;
            current.IsCanceled = false;
            SaveChangesAsync();
        }

        public void SetOrderCanceled(Int32 orderId)
        {
            Order current = GetOrder(orderId);
            if (current == null)
                return;
            current.IsSucceded = false;
            current.IsCanceled = true;
            SaveChangesAsync();
        }

        public void DeleteOrder(Int32 orderId)
        {
            Order current = GetOrder(orderId);
            if (current == null)
                return;
            Orders.Remove(current);
        }

        public void SetBuyerInfo(Int32 buyerId, String info)
        {
            Buyer buyer = GetBuyer(buyerId);
            if (buyer == null)
                return;
            buyer.Info = info;
            SaveChangesAsync();
        }

        public void SetSellerInfo(Int32 sellerId, String info)
        {
            Seller seller = GetSeller(sellerId);
            if (seller == null)
                return;
            seller.Info = info;
            SaveChangesAsync();
        }
    }
}