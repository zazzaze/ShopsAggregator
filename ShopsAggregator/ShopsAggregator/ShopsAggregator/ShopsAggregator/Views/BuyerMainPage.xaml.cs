using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using RestSharp;
using ShopsAggregator.Services;
using ShopsAggregator.Models;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;

namespace ShopsAggregator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BuyerMainPage : ContentPage
    {
        private String iconPath;
        private Buyer _buyer;
        private const String FailedConnectionToServerAlertTitle = "Ошибка подключения к серверу";
        private List<BuyerPostView> likedPosts = new List<BuyerPostView>();
        public BuyerMainPage(Buyer buyer)
        {
            InitializeComponent();
            _buyer = buyer;
            this.BindingContext = _buyer;
        }

        protected override void OnAppearing()
        {
            Posts.WidthRequest = App.Current.MainPage.Width;
            SubscribedCounter.Text = _buyer.Subscribed.Count.ToString();
            GetBuyerLikedPosts();
            base.OnAppearing();
        }

        private async void GetBuyerLikedPosts()
        {
            RefreshView.IsRefreshing = true;
            var client = new RestClient($"{App.BaseUrl}api/posts/getBuyerLikedPosts?buyerId={_buyer.Id}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/text");
            IRestResponse response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                // TODO: написать сообщение о том что не получилось получить записи
                return;
            }

            try
            {
                likedPosts = JsonConvert.DeserializeObject<List<BuyerPostView>>(response.Content);
            }
            catch (Exception e)
            {
                // TODO: написать сообщение о том что не получилось получить записи
                return;
            }

            Posts.ItemsSource = likedPosts;
            RefreshView.IsRefreshing = false;

        }
        
        private async void SendUpdateUserPut(IconPostForm form)
        {
            String json = JsonConvert.SerializeObject(form);
            var client = new RestClient($"{App.ServerUrl}addBuyerIcon");
            client.Timeout = -1;
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", json,  ParameterType.RequestBody);
            IRestResponse response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.RequestEntityTooLarge)
            {
                DisplayAlert(FailedConnectionToServerAlertTitle, response.Content, "Попробовать снова");
                return;
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                DisplayAlert(FailedConnectionToServerAlertTitle, response.Content, "Попробовать снова");
                return;
            }
        }
        
        private async void GetPhoto(object sender, EventArgs e)
        {
            if (CrossMedia.Current.IsPickPhotoSupported)
            {
                MediaFile photo = await CrossMedia.Current.PickPhotoAsync();
                iconPath = photo.Path;
                Icon.Source = ImageSource.FromFile(iconPath);
                IconPostForm form = new IconPostForm(_buyer.Id);
                GetPhotoBytes(iconPath, form);
                if (!App.IsConnected())
                {
                    return;
                }
                SendUpdateUserPut(form);
            }
        }
        
        private void GetPhotoBytes(String path, IconPostForm form)
        {
            List<Int32> bytes = new List<Int32>();
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                while(fs.Position != fs.Length)
                    bytes.Add(fs.ReadByte());
            }

            form.IconBytesArr = bytes.ToList();
        }

        private void OnExitToolbarItemTapped(object sender, EventArgs e)
        {
            Navigation.PopToRootAsync();
        }
        private void RefreshView_OnRefreshing(object sender, EventArgs e)
        {
            this.BindingContext = _buyer;
            GetBuyerLikedPosts();
        }
        
        private async void OnPostHeaderTapped(object sender, EventArgs e)
        {
            if (sender is FlexLayout flexLayout)
            {
                await Navigation.PushAsync(new WatchSellerPage((flexLayout.Children[1] as Label).Text,_buyer));
            }
        }

        private async void SendDislikeImageTapped(object sender, EventArgs e)
        {
            if (sender is Image image)
            {
                BuyerPostView bindingPost = (BuyerPostView) (image.ParentView.BindingContext);
                BuyerPostView post =
                    (from selectionPost in likedPosts
                        where selectionPost.PostId == bindingPost.PostId
                        select selectionPost).FirstOrDefault();
                if (post == null)
                    return;
                likedPosts.Remove(post);
                RestClient client = new RestClient($"{App.BaseUrl}api/posts/removeLike?likerId={_buyer.Id}&postId={post.PostId}");
                var request = new RestRequest(Method.PUT);
                request.AddHeader("Content-Type", "application/text");
                var response = await client.ExecuteAsync(request);
                Posts.ItemsSource = null;
                Posts.ItemsSource = likedPosts;
            }
        }
    }
}