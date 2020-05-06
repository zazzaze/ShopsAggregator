using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ShopsAggregatorWebApi.Models
{
    public class DatabaseContext : DbContext
    {
        private const String UsersIconsDirPath = "../images";
        private DbSet<Seller> Sellers { get; set; }
        private DbSet<Buyer> Buyers { get; set; }
        
        private DbSet<Post> Posts { get; set; }
        
        private DbSet<Comment> Comments { get; set; }

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

        public Seller GetSeller(Int32 id)
        {
            return (from seller in Sellers.ToList() where seller.Id == id select seller).FirstOrDefault();
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

        public List<Seller> SearchSellers(string s)
        {
            return (from seller in Sellers.ToList() where seller.Username.Contains(s) select seller).ToList();
        }

        public Boolean AddSubscriber(Int32 buyerId, Int32 sellerId)
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

        public Boolean RemoveSubscriber(Int32 buyerId, Int32 sellerId)
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
            SaveChangesAsync();
            return true;
        }
    }
}