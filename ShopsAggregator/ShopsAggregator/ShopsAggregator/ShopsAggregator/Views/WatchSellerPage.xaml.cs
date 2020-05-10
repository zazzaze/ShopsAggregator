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
    /// <summary>
    /// Код страницы просмотра страницы пользователя-продавца пользователем-покупателем.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WatchSellerPage : ContentPage
    {
        /// <summary>
        /// Экземпляр типа пользователя-продавца.
        /// </summary>
        private Seller _seller;
        /// <summary>
        /// Имя пользователя-продавца.
        /// </summary>
        private String _sellerName;
        /// <summary>
        /// Список записей пользователя-продавца.
        /// </summary>
        private List<BuyerPostView> _sellerPosts = new List<BuyerPostView>();
        /// <summary>
        /// Экземпляр типа пользователя-покупателя.
        /// </summary>
        private Buyer _buyer;
        /// <summary>
        /// Конструктор страницы. Получает экземпляр пользователя-продавца с сервера.
        /// </summary>
        /// <param name="sellerName">Имя пользователя-продавца.</param>
        /// <param name="buyer">Экземпляр типа пользователя-покупателя.</param>
        public WatchSellerPage(String sellerName, Buyer buyer)
        {
            InitializeComponent();
            _buyer = buyer;
            GetSellerByNameAsync(sellerName);
            _sellerName = sellerName;
            RefreshView.Refreshing += RefreshViewOnRefreshing;
        }

        /// <summary>
        /// Устанавливает кнопки статус подписки.
        /// </summary>
        private void SetSubscribeButton()
        {
            SubscribeSatusButton.BackgroundColor = (Color) App.Current.Resources["purple"];
            SubscribeSatusButton.TextColor = (Color) App.Current.Resources["buttonTextColor"];
            SubscribeSatusButton.Text = "+Подписаться";
            SubscribeSatusButton.Clicked -= UnsubscribeFromSeller;
            SubscribeSatusButton.Clicked += SubscribeToSeller;
        }
        /// <summary>
        /// Устанавливает кнопки статус отписки.
        /// </summary>
        private void SetUnsubscribeButton()
        {
            SubscribeSatusButton.BackgroundColor = (Color) App.Current.Resources["cloudWhite"];
            SubscribeSatusButton.TextColor = (Color) App.Current.Resources["grey"];
            SubscribeSatusButton.Text = "Отписаться";
            SubscribeSatusButton.Clicked += UnsubscribeFromSeller;
            SubscribeSatusButton.Clicked -= SubscribeToSeller;
        }
        
        /// <summary>
        /// Обработчик собтыия подписки на пользователя-продавца.
        /// </summary>
        /// <param name="sender">Издатель события - Button.</param>
        /// <param name="e">Аргументы события.</param>
        private async void SubscribeToSeller(object sender, EventArgs e)
        {
            if (!App.IsConnected())
            {
                await DisplayAlert("Нет подключения к интернету", "Проверьте подключение к интернету", "Попробовать снова");
                return;
            }
            SetUnsubscribeButton();
            RestClient client = new RestClient($"{App.BaseUrl}api/sub/addSub?buyerId={_buyer.Id}&sellerName={_seller.Username}");
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Content-Type", "application/text");
            var response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                SetSubscribeButton();
                await DisplayAlert("Ошибка подписки", $"Не удалось подписаться на пользователя ${_seller.Username}", "Жаль");
            }
        }
        /// <summary>
        /// Обработчик собтыия отписки от пользователя-продавца.
        /// </summary>
        /// <param name="sender">Издатель события - Button.</param>
        /// <param name="e">Аргументы события.</param>
        private async void UnsubscribeFromSeller(object sender, EventArgs e)
        {
            if (!App.IsConnected())
            {
                await DisplayAlert("Нет подключения к интернету", "Проверьте подключение к интернету", "Попробовать снова");
                return;
            }
            SetSubscribeButton();
            RestClient client = new RestClient($"{App.BaseUrl}api/sub/rmSub?buyerId={_buyer.Id}&sellerName={_seller.Username}");
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Content-Type", "application/text");
            var response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                SetUnsubscribeButton();
                await DisplayAlert("Ошибка отписки", $"Не удалось отписаться на пользователя ${_seller.Username}", "Жаль");
            }
        }

        /// <summary>
        /// Обработчик события обновления информации о пользователе-продавце.
        /// </summary>
        /// <param name="sender">Издатель события - RefreshView.</param>
        /// <param name="e">Аргументы события.</param>
        private void RefreshViewOnRefreshing(object sender, EventArgs e)
        {
            GetSellerByNameAsync(_sellerName);
        }

        
        /// <summary>
        /// Получает с сервера пользователя-продавца по имени.
        /// </summary>
        /// <param name="sellerName">Имя пользователя-продавца.</param>
        private async void GetSellerByNameAsync(String sellerName)
        {
            if (!App.IsConnected())
            {
                await DisplayAlert("Нет подключения к интернету", "Проверьте подключение к интернету", "Попробовать снова");
                return;
            }
            RefreshView.IsRefreshing = true;
            RestClient client =new RestClient($"{App.BaseUrl}api/search/getSeller?sellerName={sellerName}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/text");
            var response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                await DisplayAlert("Ошибка", "Не получилось получить данные о пользователе", "Поробовать снова");
                await Navigation.PopAsync();
                return;
            }

            try
            {
                _seller = JsonConvert.DeserializeObject<Seller>(response.Content);
            }
            catch (Exception e)
            {
                await DisplayAlert("Ошибка", "Не получилось получить данные о пользователе", "Поробовать снова");
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

        /// <summary>
        /// Получает записи пользователя-продавца с сервера.
        /// </summary>
        /// <param name="sellerName">Имя пользователя-продавца.</param>
        private async void GetSellerPostsAsync(String sellerName)
        {
            if (!App.IsConnected())
            {
                await DisplayAlert("Нет подключения к интернету", "Проверьте подключение к интернету", "Попробовать снова");
                return;
            }
            RefreshView.IsRefreshing = true;
            var client = new RestClient($"{App.BaseUrl}api/posts/getSellerPosts?sellerName={sellerName}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/text");
            IRestResponse response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                await DisplayAlert("Ошибка", "Не получилось получить данные о пользователе", "Поробовать снова");
                return;
            }

            List<Post> posts = new List<Post>();
            try
            {
                posts = JsonConvert.DeserializeObject<List<Post>>(response.Content);
            }
            catch (Exception e)
            {
                await DisplayAlert("Ошибка", "Не получилось получить данные о пользователе", "Поробовать снова");
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

        /// <summary>
        /// Отменяет событие нажатия на элемент из ListView.
        /// </summary>
        /// <param name="sender">Издатель события - ListView.</param>
        /// <param name="e">Аргументы события.</param>
        private void Posts_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (sender is ListView listView)
            {
                listView.SelectedItem = null;
            }
        }

        /// <summary>
        /// Событие нажатия пользователем на картинку "понравилась запись(лайк)".
        /// </summary>
        /// <param name="sender">Издатель события - картинка "понравилась запись"</param>
        /// <param name="e">Аргументы события.</param>
        private void OnLikeImageTapped(object sender, EventArgs e)
        {
            if (sender is Image image)
            {
                BuyerPostView post = (BuyerPostView) image.ParentView.BindingContext;
                Task.Run(() =>
                {
                    BuyerPostView currentPost =
                        (from newPost in _sellerPosts where newPost.PostId == post.PostId select newPost).FirstOrDefault();
                    if (currentPost == null)
                        return;
                    currentPost.Likers.Add(_buyer.Id);
                });
                SendLikeTapped("addLike", post);
            }
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
                    (from newPost in _sellerPosts where newPost.PostId == post.PostId select newPost).FirstOrDefault();
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
        private async void SendLikeTapped(string param, BuyerPostView post)
        {
            if (!App.IsConnected())
            {
                await DisplayAlert("Нет подключения к интернету", "Проверьте подключение к интернету", "Попробовать снова");
                return;
            }
            RestClient client = new RestClient($"{App.BaseUrl}api/posts/{param}?likerId={_buyer.Id}&postId={post.PostId}");
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Content-Type", "application/text");
            var response = await client.ExecuteAsync(request);
            Posts.ItemsSource = null;
            Posts.ItemsSource = _sellerPosts;
        }
    }
}