using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopsAggregator.Views;
using ShopsAggregatorLib;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShopsAggregator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainTabbedPage : TabbedPage
    {
        public MainTabbedPage(User user)
        {
            this.Title = "ShopYou";
            var userSettingsPage = new UserSettingsPage(user);
            userSettingsPage.IconImageSource = ImageSource.FromResource("ShopsAggregator.images.accounticon.png");
            var searchPage = new SearchPage();
            searchPage.IconImageSource = ImageSource.FromResource("ShopsAggregator.images.searchicon.png");
            var postsPage = new AllPostsPage();
            postsPage.IconImageSource = ImageSource.FromResource("ShopsAggregator.images.gallery.png");
            Children.Add(postsPage);
            Children.Add(searchPage);
            Children.Add(userSettingsPage);
        }
    }
}