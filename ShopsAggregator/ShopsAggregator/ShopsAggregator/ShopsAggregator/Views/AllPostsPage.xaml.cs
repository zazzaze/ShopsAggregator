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
    public partial class AllPostsPage : ContentPage
    {
        private Buyer _buyer;
        private List<BuyerPostView> newPosts = new List<BuyerPostView>();
        public AllPostsPage(Buyer buyer)
        {
            InitializeComponent();
            _buyer = buyer;
            Posts.Refreshing += RefreshViewOnRefreshing;
            Posts.IsPullToRefreshEnabled = true;
        }

        private void RefreshViewOnRefreshing(object sender, EventArgs e)
        {
            GetNewPostsAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            GetNewPostsAsync();
        }

        private async void GetNewPostsAsync()
        {
            Posts.IsRefreshing = true;
            RestClient client = new RestClient($"{App.BaseUrl}api/posts/getBuyerNewPost?buyerId={_buyer.Id}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/text");
            var response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                // TODO: Написать сообщение о том, что не получилось получить новые записи
                Posts.IsRefreshing = false;
                return;
            }
            List<BuyerPostView> newPosts = new List<BuyerPostView>();
            try
            {
                newPosts = JsonConvert.DeserializeObject<List<BuyerPostView>>(response.Content);
            }
            catch (Exception e)
            {
                // TODO: Написать сообщение о том, что не получилось получить новые записи
                Posts.IsRefreshing = false;
                return;
            }

            foreach (BuyerPostView buyerPostView in newPosts)
            {
                buyerPostView.BuyerId = _buyer.Id;
            }

            this.newPosts = newPosts;
            Posts.ItemsSource = this.newPosts;
            Posts.IsRefreshing = false;
        }

        private void OnLikeImageTapped(object sender, EventArgs e)
        {
            BuyerPostView post = (BuyerPostView) (((Image) sender).ParentView.BindingContext);
            Task.Run(() =>
            {
                BuyerPostView currentPost =
                    (from newPost in newPosts where newPost.PostId == post.PostId select newPost).FirstOrDefault();
                if (currentPost == null)
                    return;
                currentPost.Likers.Add(_buyer.Id);
                Posts.ItemsSource = newPosts;
            });
            SendLikeTapped("addLike", post);
        }

        private void OnDislikeImageTapped(object sender, EventArgs e)
        {
            BuyerPostView post = (BuyerPostView) (((Image) sender).ParentView.BindingContext);
            Task.Run(() =>
            {
                BuyerPostView currentPost =
                    (from newPost in newPosts where newPost.PostId == post.PostId select newPost).FirstOrDefault();
                if (currentPost == null)
                    return;
                currentPost.Likers.Remove(_buyer.Id);
            });
            SendLikeTapped("removeLike", post);
        }
        
        private async void SendLikeTapped(String param, BuyerPostView post)
        {
            RestClient client = new RestClient($"{App.BaseUrl}api/posts/{param}?likerId={_buyer.Id}&postId={post.PostId}");
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Content-Type", "application/text");
            var response = await client.ExecuteAsync(request);
            Posts.ItemsSource = null;
            Posts.ItemsSource = newPosts;
        }


        private void Posts_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (sender is ListView listView)
            {
                listView.SelectedItem = null;
            }
        }

        private async void OnPostHeaderTapped(object sender, EventArgs e)
        {
            if (sender is FlexLayout flexLayout)
            {
                await Navigation.PushAsync(new WatchSellerPage((flexLayout.Children[1] as Label).Text,_buyer));
            }
        }
    }
}