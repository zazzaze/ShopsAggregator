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
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainTabbedPage : TabbedPage
    {
        public MainTabbedPage()
        {
            Title = "ShopYou";
        }
        public MainTabbedPage(Buyer buyer) : this()
        {
            var userSettingsPage = new BuyerMainPage(buyer);
            userSettingsPage.IconImageSource = ImageSource.FromResource("ShopsAggregator.images.accounticon.png");
            var searchPage = new SearchPage(buyer);
            searchPage.IconImageSource = ImageSource.FromResource("ShopsAggregator.images.searchicon.png");
            var postsPage = new AllPostsPage(buyer);
            postsPage.IconImageSource = ImageSource.FromResource("ShopsAggregator.images.gallery.png");
            Children.Add(postsPage);
            Children.Add(searchPage);
            Children.Add(userSettingsPage);
        }

        public MainTabbedPage(Seller seller) : this()
        {
            var sellerMainPage = new SellerMainPage(seller);
            var createPostPage = new CreatePostPage(seller);
            Children.Add(createPostPage);
            Children.Add(sellerMainPage);
        }
    }
}