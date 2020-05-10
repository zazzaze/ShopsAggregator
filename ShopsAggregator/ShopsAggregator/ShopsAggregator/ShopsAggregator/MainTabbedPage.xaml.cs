using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopsAggregator.Models;
using ShopsAggregator.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShopsAggregator
{
    /// <summary>
    /// Код для страницы с множеством вкладок.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainTabbedPage : TabbedPage
    {
        /// <summary>
        /// Общий конструктор. Устанавливает заголовок страницы.
        /// </summary>
        public MainTabbedPage()
        {
            Title = "ShopYou";
        }
        /// <summary>
        /// Добавляет страницы если пользователем является пользователь-покупатель.
        /// </summary>
        /// <param name="buyer">Экземпляр типа пользователя-покупателя.</param>
        public MainTabbedPage(Buyer buyer) : this()
        {
            var userSettingsPage = new BuyerMainPage(buyer);
            userSettingsPage.IconImageSource = ImageSource.FromResource("ShopsAggregator.images.accounticon.png");
            var searchPage = new SearchPage(buyer);
            searchPage.IconImageSource = ImageSource.FromResource("ShopsAggregator.images.searchicon.png");
            var postsPage = new AllPostsPage(buyer);
            postsPage.IconImageSource = ImageSource.FromResource("ShopsAggregator.images.gallery.png");
            var ordersPage = new BuyerOrdersPage(buyer);
            ordersPage.IconImageSource = ImageSource.FromResource("ShopsAggregator.images.orders.png");
            Children.Add(postsPage);
            Children.Add(searchPage);
            Children.Add(ordersPage);
            Children.Add(userSettingsPage);
        }
        /// <summary>
        /// Добавляет страницы если пользователем является пользователь-продавец.
        /// </summary>
        /// <param name="buyer">Экземпляр типа пользователя-продавца.</param>
        public MainTabbedPage(Seller seller) : this()
        {
            var sellerMainPage = new SellerMainPage(seller);
            sellerMainPage.IconImageSource = ImageSource.FromResource("ShopsAggregator.images.accounticon.png");
            var createPostPage = new CreatePostPage(seller);
            createPostPage.IconImageSource = ImageSource.FromResource("ShopsAggregator.images.plus.png");
            var ordersPage = new SellerOrdersPage(seller);
            ordersPage.IconImageSource = ImageSource.FromResource("ShopsAggregator.images.orders.png");
            Children.Add(ordersPage);
            Children.Add(createPostPage);
            Children.Add(sellerMainPage);
        }
    }
}