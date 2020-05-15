using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ShopsAggregatorWebApi.Models;

namespace ShopsAggregatorWebApi.Services
{
    /// <summary>
    /// Сервис работы с базой данных.
    /// </summary>
    public class DatabaseContext : DbContext
    {
        /// <summary>
        /// Путь для сохранения картинок.
        /// </summary>
        private const String UsersIconsDirPath = "../images";
        /// <summary>
        /// База данных пользователей-продавцов.
        /// </summary>
        private DbSet<Seller> Sellers { get; set; }
        /// <summary>
        /// База данных пользователкей-покупателей.
        /// </summary>
        private DbSet<Buyer> Buyers { get; set; }
        
        /// <summary>
        /// База данных записей пользователей-продавцов.
        /// </summary>
        private DbSet<Post> Posts { get; set; }
        /// <summary>
        /// База данных заказов пользователей-покупателей.
        /// </summary>
        private DbSet<Order> Orders { get; set; }
        
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="options">Опции для сервиса.</param>
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            
        }

        /// <summary>
        /// Получает всех пользователей, зарегестрированных в системе.
        /// </summary>
        /// <returns>Список пользователей</returns>
        public List<User> GetAllUsers()
        {
            List<User> users = Sellers.ToList().ConvertAll(x => (User) x);
            users.AddRange(Buyers.ToList().ConvertAll(x => (User)x));
            return users;
        }

        /// <summary>
        /// Создает нового пользователя-покупателя.
        /// </summary>
        /// <param name="user">Экземпляр пользователя-покупателя.</param>
        /// <returns>Созданный пользователь.</returns>
        public User CreateUser(Buyer user)
        {
            if (IsSameSellerCreated(user) || IsSameBuyerCreated(user))
                return null;
            Buyers.Add(user);
            SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// Создает нового пользователя-продавца.
        /// </summary>
        /// <param name="user">Экземпляр пользователя-продавца.</param>
        /// <returns>Созданный пользователь.</returns>
        public User CreateUser(Seller user)
        {
            if (IsSameSellerCreated(user) || IsSameBuyerCreated(user))
                return null;
            Sellers.Add(user);
            SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// Проверяет создан ли пользователь-продавец с таким же email или username.
        /// </summary>
        /// <param name="user">Экземпляр проверяемого пользователя-продавца.</param>
        /// <returns>Результат проверки.</returns>
        private Boolean IsSameSellerCreated(User user)
        {
            return (from seller in Sellers.ToList() where seller.Username == user.Username 
                                                          || seller.Email == user.Email select seller)
                .FirstOrDefault() != null;
        }
        /// <summary>
        /// Проверяет создан ли пользователь-покупатель с таким же email или username.
        /// </summary>
        /// <param name="user">Экземпляр проверяемого пользователя-покупатель.</param>
        /// <returns>Результат проверки.</returns>
        private Boolean IsSameBuyerCreated(User user)
        {
            return (from buyer in Buyers.ToList() where buyer.Username == user.Username 
                                                   || buyer.Email == user.Email select buyer)
                .FirstOrDefault() != null;
        }

        /// <summary>
        /// Получает пользователя по логину и паролю.
        /// </summary>
        /// <param name="login">Логин.</param>
        /// <param name="password">Пароль.</param>
        /// <returns>Полученный пользователь.</returns>
        public Buyer GetBuyer(String login, String password)
        {
            return (from buyer in Buyers.ToList()
                where (buyer.Username == login || buyer.Username == login)
                      && buyer.Password == password
                select buyer).FirstOrDefault();
        }

        /// <summary>
        /// Получает пользователя-покупателя по его id.
        /// </summary>
        /// <param name="id">Id пользователя.</param>
        /// <returns>Экземпляр типа пользователя-покупателя.</returns>
        private Buyer GetBuyer(Int32 id)
        {
            return (from buyer in Buyers.ToList() where buyer.Id == id select buyer).FirstOrDefault();
        }

        /// <summary>
        /// Получает пользователя по логину и паролю.
        /// </summary>
        /// <param name="login">Логин.</param>
        /// <param name="password">Пароль.</param>
        /// <returns>Полученный пользователь.</returns>
        public Seller GetSeller(string login, string password)
        {
            return (from seller in Sellers.ToList()
                where (seller.Username == login || seller.Username == login)
                      && seller.Password == password
                select seller).FirstOrDefault();
        }

        /// <summary>
        /// Получает пользователя-продавца по его id.
        /// </summary>
        /// <param name="id">Id пользователя.</param>
        /// <returns>Экземпляр типа пользователя-продавца.</returns>
        private Seller GetSeller(Int32 id)
        {
            return (from seller in Sellers.ToList() where seller.Id == id select seller).FirstOrDefault();
        }

        /// <summary>
        /// Получает пользователя по имени.
        /// </summary>
        /// <param name="sellerName">Имя пользователя.</param>
        /// <returns>Полученный пользователь.</returns>
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

        /// <summary>
        /// Добавляет иконку пользователю-продавцу.
        /// </summary>
        /// <param name="icon">Форма с информацией об изображении.</param>
        /// <param name="sellerId">Id пользователя-продавца.</param>
        /// <returns>Результат добавления.</returns>
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
            catch (Exception)
            {
                return false;
            }
            toSeller.IconPath = $"{toSeller.Username}icon.jpeg";
            SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Добавляет иконку пользователю-покупателю.
        /// </summary>
        /// <param name="icon">Форма с информацией об изображении.</param>
        /// <param name="buyerId">Id пользователя-покупателя.</param>
        /// <returns>Результат добавления.</returns>
        public Boolean AddIconToBuyer(IconPostForm icon, Int32 buyerId)
        {
            Buyer toBuyer = (from buyer in Buyers.ToList() where buyer.Id == buyerId select buyer).FirstOrDefault();
            if (toBuyer == null)
                return false;
            try
            {
                AddIconToUser(icon, toBuyer);
            }
            catch (Exception)
            {
                return false;
            }

            toBuyer.IconPath = $"{toBuyer.Username}icon.jpeg";
            SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Добавляет иконку пользователю.
        /// </summary>
        /// <param name="icon">Форма с информацией об изображении.</param>
        /// <param name="toUser">Экземпляр пользователя.</param>
        /// <returns>Результат добавления.</returns>
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

        /// <summary>
        /// Ищет пользователя-покупателя, который содержит в username поисковую строку.
        /// </summary>
        /// <param name="s">Поисковая строка.</param>
        /// <returns>Результат поиска.</returns>
        public List<String> SearchSellers(String s)
        {
            return (from seller in Sellers.ToList() where seller.Username.Contains(s) select seller.Username).ToList();
        }

        /// <summary>
        /// Добваляет подписчика.
        /// </summary>
        /// <param name="buyerId">Id пользователя-покупателя.</param>
        /// <param name="sellerName">Имя пользователя-продавца.</param>
        /// <returns>Результат добавления.</returns>
        public Boolean AddSubscriber(Int32 buyerId, String sellerName)
        {
            Seller currentSeller = (from seller in Sellers.ToList() where seller.Username == sellerName select seller)
                .FirstOrDefault();
            if (currentSeller == null)
                return false;
            return AddSubscriber(buyerId, currentSeller.Id);
        }
        
        /// <summary>
        /// Добваляет подписчика.
        /// </summary>
        /// <param name="buyerId">Id пользователя-покупателя.</param>
        /// <param name="sellerId">Id пользователя-продавца.</param>
        /// <returns>Результат добавления.</returns>
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

        /// <summary>
        /// Удаляет подписчика.
        /// </summary>
        /// <param name="buyerId">Id пользователя-покупателя.</param>
        /// <param name="sellerName">Имя пользователя-продавца.</param>
        /// <returns>Результат добавления.</returns>
        public Boolean RemoveSubscriber(Int32 buyerId, String sellerName)
        {
            Seller currentSeller = (from seller in Sellers.ToList() where seller.Username == sellerName select seller)
                .FirstOrDefault();
            if (currentSeller == null)
                return false;
            return RemoveSubscriber(buyerId, currentSeller.Id);
        }

        /// <summary>
        /// Удаляет подписчика.
        /// </summary>
        /// <param name="buyerId">Id пользователя-покупателя.</param>
        /// <param name="sellerId">Id пользователя-продавца.</param>
        /// <returns>Результат добавления.</returns>
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

        /// <summary>
        /// Создает новую запись и добавляет ее в базу данных.
        /// </summary>
        /// <param name="postForm">Форма новой записи.</param>
        /// <returns>Результат создания записи</returns>
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

        /// <summary>
        /// Добавляет новую запись всем подписчикам пользователя-продавца.
        /// </summary>
        /// <param name="creator">Пользователь-создатель.</param>
        /// <param name="postId">Id записи.</param>
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

        /// <summary>
        /// Получает экземпляр записи по id.
        /// </summary>
        /// <param name="id">Id записи.</param>
        /// <returns>Экземпляр типа записи.</returns>
        public Post GetPostById(Int32 id)
        {
            return (from post in Posts.ToList() where post.Id == id select post).FirstOrDefault();
        }
        
        /// <summary>
        /// Получаем записи пользователя-продавца по имени.
        /// </summary>
        /// <param name="sellerName">Имя пользователя-продавца</param>
        /// <returns>Список записей.</returns>
        public List<Post> GetPostsByCreatorName(String sellerName)
        {
            Int32 creatorId = (from seller in Sellers.ToList() where seller.Username == sellerName select seller.Id)
                .FirstOrDefault();
            return (from post in Posts.ToList() where post.CreatorId == creatorId select post).ToList();
        }

        /// <summary>
        /// Получает новые записи пользователя-покупателя по его id.
        /// </summary>
        /// <param name="buyerId">Id пользователя-покупателя.</param>
        /// <returns>Список новых записей.</returns>
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

        /// <summary>
        /// Добавляет лайк пользователя-покупателя к записи.
        /// </summary>
        /// <param name="likerId">Id пользователя-покупателя.</param>
        /// <param name="postId">Id записи.</param>
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

        /// <summary>
        /// Удаляет лайк пользователя-покупателя к записи.
        /// </summary>
        /// <param name="likerId">Id пользователя-покупателя.</param>
        /// <param name="postId">Id записи.</param>
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

        /// <summary>
        /// Получает записи, понравившиеся пользователю-покупателю.
        /// </summary>
        /// <param name="buyerId">Id пользователя.</param>
        /// <returns>Список записей.</returns>
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

        /// <summary>
        /// Создает новый заказ и добавляет его в базу данных.
        /// </summary>
        /// <param name="buyerId">Id пользователя-покупателч, который создает заказ.</param>
        /// <param name="postId">Id записи, по которой создают заказ.</param>
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

        /// <summary>
        /// Получает заказы пользователя-покупателя.
        /// </summary>
        /// <param name="buyerId">Id пользователя-покупателя.</param>
        /// <returns>Список заказов.</returns>
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

        /// <summary>
        /// Получает заказы пользователя-продавца.
        /// </summary>
        /// <param name="sellerId">Id пользователя-продавца.</param>
        /// <returns>Список заказов.</returns>
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

        /// <summary>
        /// Получает заказ по Id. 
        /// </summary>
        /// <param name="orderId">Id заказа.</param>
        /// <returns>Экземпляр типа заказа.</returns>
        private Order GetOrder(Int32 orderId)
        {
            return (from order in Orders.ToList() where order.Id == orderId select order).FirstOrDefault();
        }
        
        /// <summary>
        /// Устанавливает одобренный статус заказа.
        /// </summary>
        /// <param name="orderId">Id заказа.</param>
        public void SetOrderSuccess(Int32 orderId)
        {
            Order current = GetOrder(orderId);
            if (current == null)
                return;
            current.IsSucceded = true;
            current.IsCanceled = false;
            SaveChangesAsync();
        }

        /// <summary>
        /// Устанавливает отклоненный статус заказа.
        /// </summary>
        /// <param name="orderId">Id заказа.</param>
        public void SetOrderCanceled(Int32 orderId)
        {
            Order current = GetOrder(orderId);
            if (current == null)
                return;
            current.IsSucceded = false;
            current.IsCanceled = true;
            SaveChangesAsync();
        }

        /// <summary>
        /// Удаляет заказ.
        /// </summary>
        /// <param name="orderId">Id заказа.</param>
        public void DeleteOrder(Int32 orderId)
        {
            Order current = GetOrder(orderId);
            if (current == null)
                return;
            Orders.Remove(current);
            SaveChangesAsync();
        }

        /// <summary>
        /// Изменяет информацию о пользователе-покупателе.
        /// </summary>
        /// <param name="buyerId">Id пользователя-покупателя.</param>
        /// <param name="info">Новая информация.</param>
        public void SetBuyerInfo(Int32 buyerId, String info)
        {
            Buyer buyer = GetBuyer(buyerId);
            if (buyer == null)
                return;
            buyer.Info = info;
            SaveChangesAsync();
        }

        /// <summary>
        /// Изменяет информацию о пользователе-продавце.
        /// </summary>
        /// <param name="sellerId">Id пользователя-покупателя.</param>
        /// <param name="info">Новая информация.</param>
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