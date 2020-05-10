using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using ShopsAggregator.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShopsAggregator.Views
{
    /// <summary>
    /// Код для страницы всех записей.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllPostsPage : ContentPage
    {
        /// <summary>
        /// Экземпляр типа пользователя-покупателя.
        /// </summary>
        private Buyer _buyer;
        /// <summary>
        /// Новые записи, доступные пользователю-покупателю.
        /// </summary>
        private List<BuyerPostView> newPosts = new List<BuyerPostView>();
        
        /// <summary>
        /// Конструктор страницы.
        /// </summary>
        /// <param name="buyer">Экземпляр типа пользователя-покупателя.</param>
        public AllPostsPage(Buyer buyer)
        {
            InitializeComponent();
            _buyer = buyer;
            Posts.Refreshing += RefreshViewOnRefreshing;
            Posts.IsPullToRefreshEnabled = true;
        }

        /// <summary>
        /// Обновляет данные страницы по запросу пользователя.
        /// </summary>
        /// <param name="sender">Издатель события - RefreshView.</param>
        /// <param name="e">Аргументы события.</param>
        private void RefreshViewOnRefreshing(object sender, EventArgs e)
        {
            GetNewPostsAsync();
        }

        /// <summary>
        /// Получает данные о новых записях и вызывает базовое поведение метода.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            GetNewPostsAsync();
        }

        /// <summary>
        /// Получает новые записи, доступные пользователю.
        /// </summary>
        private async void GetNewPostsAsync()
        {
            if (!App.IsConnected())
            {
                await DisplayAlert("Ошибка", "Осутствует подключение к интернету", "Поробовать снова");
                return;
            }
            Posts.IsRefreshing = true;
            RestClient client = new RestClient($"{App.BaseUrl}api/posts/getBuyerNewPost?buyerId={_buyer.Id}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/text");
            var response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                await DisplayAlert("Ошибка", "Не получилось получить новые записи", "Попробовать снова");
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
                await DisplayAlert("Ошибка", "Не получилось получить новые записи", "Попробовать снова");
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

        /// <summary>
        /// Событие нажатия пользователем на картинку "понравилась запись(лайк)".
        /// </summary>
        /// <param name="sender">Издатель события - картинка "понравилась запись"</param>
        /// <param name="e">Аргументы события.</param>
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
        /// <summary>
        /// Событие нажатия пользователем на картинку "убрать лайк".
        /// </summary>
        /// <param name="sender">Издатель события - картинка "убрать лайк".</param>
        /// <param name="e">Аргументы события.</param>
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
        
        /// <summary>
        /// Отправляет на сервер PUT запрос, чтобы добавить в или убрать картинку из понравившееся у пользователя.
        /// </summary>
        /// <param name="param">Параметр с Url строки, в зависимости от которого будет добавляться или убираться лайк.</param>
        /// <param name="post">Запись, которая понравилась/разонравилась пользователю.</param>
        private async void SendLikeTapped(String param, BuyerPostView post)
        {
            if (!App.IsConnected())
            {
                await DisplayAlert("Ошибка", "Осутствует подключение к интернету", "Поробовать снова");
                return;
            }
            Posts.ItemsSource = null;
            Posts.ItemsSource = newPosts;
            RestClient client = new RestClient($"{App.BaseUrl}api/posts/{param}?likerId={_buyer.Id}&postId={post.PostId}");
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Content-Type", "application/text");
            var response = await client.ExecuteAsync(request);
        }


        /// <summary>
        /// Отменяет событие нажатия на элемент из ListView.
        /// </summary>
        /// <param name="sender">Издатель события - ListView Posts.</param>
        /// <param name="e">Аргументы события.</param>
        private void Posts_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (sender is ListView listView)
            {
                listView.SelectedItem = null;
            }
        }

        /// <summary>
        /// Обрабатывает событие нажатия на иконку или имя пользователя-продавца, который опубликовал запись.
        /// </summary>
        /// <param name="sender">Издатель события - FlexLayout.</param>
        /// <param name="e">Аргументы события.</param>
        private async void OnPostHeaderTapped(object sender, EventArgs e)
        {
            if (sender is FlexLayout flexLayout)
            {
                await Navigation.PushAsync(new WatchSellerPage((flexLayout.Children[1] as Label).Text,_buyer));
            }
        }

        /// <summary>
        /// Событие добавления пользователем записи к заказам.
        /// </summary>
        /// <param name="sender">Издатель события - Button</param>
        /// <param name="e">Аргументы события.</param>
        private async void DeliverButton_OnClicked(object sender, EventArgs e)
        {
            if (!App.IsConnected())
            {
                await DisplayAlert("Ошибка", "Нет подключения к интернету", "Попробовать снова");
                return;
            }

            if (String.IsNullOrEmpty(_buyer.Info))
            {
                await DisplayAlert("Заполните поле информации об аккаунте!",
                    "Так пользователь продавец будет знать как с вами связаться", "Хорошо");
                return;
            }
            if (sender is Button button)
            {
                BuyerPostView view = (BuyerPostView) button.ParentView.BindingContext;
                if (view == null)
                    return;
                RestClient client = new RestClient($"{App.BaseUrl}api/orders/addOrder?buyerId={_buyer.Id}&" +
                                                   $"postId={view.PostId}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/text");
                var response = await client.ExecuteAsync(request);
            }
        }
    }
}