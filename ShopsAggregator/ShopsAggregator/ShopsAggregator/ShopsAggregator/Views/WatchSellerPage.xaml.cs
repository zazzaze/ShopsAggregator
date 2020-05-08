using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using ShopsAggregator.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShopsAggregator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WatchSellerPage : ContentPage
    {
        private Seller _seller;
        private String _sellerName;
        private List<BuyerPostView> _sellerPosts = new List<BuyerPostView>();
        private Buyer _buyer;
        public WatchSellerPage(String sellerName, Buyer buyer)
        {
            InitializeComponent();
            _buyer = buyer;
            GetSellerByNameAsync(sellerName);
            _sellerName = sellerName;
            RefreshView.Refreshing += RefreshViewOnRefreshing;
        }

        private void SetSubscribeButton()
        {
            SubscribeSatusButton.BackgroundColor = (Color) App.Current.Resources["purple"];
            SubscribeSatusButton.TextColor = (Color) App.Current.Resources["buttonTextColor"];
            SubscribeSatusButton.Text = "+Подписаться";
            SubscribeSatusButton.Clicked -= UnsubscribeFromSeller;
            SubscribeSatusButton.Clicked += SubscribeToSeller;
        }

        private void SetUnsubscribeButton()
        {
            SubscribeSatusButton.BackgroundColor = (Color) App.Current.Resources["cloudWhite"];
            SubscribeSatusButton.TextColor = (Color) App.Current.Resources["grey"];
            SubscribeSatusButton.Text = "Отписаться";
            SubscribeSatusButton.Clicked += UnsubscribeFromSeller;
            SubscribeSatusButton.Clicked -= SubscribeToSeller;
        }
        
        private async void SubscribeToSeller(object sender, EventArgs e)
        {
            SetUnsubscribeButton();
            RestClient client = new RestClient($"{App.BaseUrl}api/sub/addSub?buyerId={_buyer.Id}&sellerName={_seller.Username}");
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Content-Type", "application/text");
            var response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                SetSubscribeButton();
                DisplayAlert("Ошибка подписки", $"Не удалось подписаться на пользователя ${_seller.Username}", "Жаль");
            }
        }

        private async void UnsubscribeFromSeller(object sender, EventArgs e)
        {
            SetSubscribeButton();
            RestClient client = new RestClient($"{App.BaseUrl}api/sub/rmSub?buyerId={_buyer.Id}&sellerName={_seller.Username}");
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Content-Type", "application/text");
            var response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                SetUnsubscribeButton();
                DisplayAlert("Ошибка отписки", $"Не удалось отписаться на пользователя ${_seller.Username}", "Жаль");
            }
        }

        private void RefreshViewOnRefreshing(object sender, EventArgs e)
        {
            GetSellerByNameAsync(_sellerName);
        }

        private async void GetSellerByNameAsync(String sellerName)
        {
            RefreshView.IsRefreshing = true;
            RestClient client =new RestClient($"{App.BaseUrl}api/search/getSeller?sellerName={sellerName}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/text");
            var response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                // TODO:Написать сообщение о неудачном получении даанных
                await Navigation.PopAsync();
                return;
            }

            try
            {
                _seller = JsonConvert.DeserializeObject<Seller>(response.Content);
            }
            catch (Exception e)
            {
                // TODO: Написать сообщение о неадачном получении данных
                await Navigation.PopAsync();
                return;
            }

            this.BindingContext = _seller;
            GetSellerPostsAsync(_seller.Username);
            if (_seller.Subscribers == null || _seller.Subscribers.Contains(_buyer.Id))
            {
                SetUnsubscribeButton();
            }
            else
            {
                SetSubscribeButton();
            }
            RefreshView.IsRefreshing = false;
        }

        private async void GetSellerPostsAsync(String sellerName)
        {
            RefreshView.IsRefreshing = true;
            var client = new RestClient($"{App.BaseUrl}api/posts/getSellerPosts?sellerName={sellerName}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/text");
            IRestResponse response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                //TODO: сообщение о неудачном получении данных
                return;
            }

            List<Post> posts = new List<Post>();
            try
            {
                posts = JsonConvert.DeserializeObject<List<Post>>(response.Content);
            }
            catch (Exception e)
            {
                //TODO: сообщение о неудачном получении данных
            }

            foreach (Post post in posts)
            {
                BuyerPostView postView = new BuyerPostView(post);
                postView.BuyerId = _buyer.Id;
                _sellerPosts.Add(postView);
            }
            Posts.ItemsSource = _sellerPosts;
            RefreshView.IsRefreshing = false;
        }

        private void Posts_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (sender is ListView listView)
            {
                listView.SelectedItem = null;
            }
        }

        private void OnLikeImageTapped(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}