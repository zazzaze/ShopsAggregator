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
    /// <summary>
    /// Код страницы пользователя-покупателя.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BuyerMainPage : ContentPage
    {
        /// <summary>
        /// Ссылка на иконку пользователя.
        /// </summary>
        private String iconPath;
        /// <summary>
        /// Экземпляр типа пользователя-покупателя.
        /// </summary>
        private Buyer _buyer;
        /// <summary>
        /// Заголовок сообщения об ошибке подключения к серверу.
        /// </summary>
        private const String FailedConnectionToServerAlertTitle = "Ошибка подключения к серверу";
        /// <summary>
        /// Список записей, которые понравились пользователю.
        /// </summary>
        private List<BuyerPostView> likedPosts = new List<BuyerPostView>();
        /// <summary>
        /// Конструктор страницы.
        /// </summary>
        /// <param name="buyer">Экземпляр типа пользователя-покупателя.</param>
        public BuyerMainPage(Buyer buyer)
        {
            InitializeComponent();
            _buyer = buyer;
            this.BindingContext = _buyer;
        }

        /// <summary>
        /// Устанавливает ширину ListView Posts, получает понравившиеся записи и вызывает базовое поведение метода.
        /// </summary>
        protected override void OnAppearing()
        {
            Posts.WidthRequest = App.Current.MainPage.Width;
            SubscribedCounter.Text = _buyer.Subscribed.Count.ToString();
            GetBuyerLikedPosts();
            base.OnAppearing();
        }

        /// <summary>
        /// Получает с сервера записи, которые понравились пользователю.
        /// </summary>
        private async void GetBuyerLikedPosts()
        {
            if (!App.IsConnected())
            {
                await DisplayAlert("Нет подключения к интернету", "Проверьте подключение к интернету", "Попробовать снова");
                return;
            }
            RefreshView.IsRefreshing = true;
            var client = new RestClient($"{App.BaseUrl}api/posts/getBuyerLikedPosts?buyerId={_buyer.Id}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/text");
            IRestResponse response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                await DisplayAlert("Ошибка", "Не получилось получить понравившиеся записи", "Попробовать снова");
                return;
            }

            try
            {
                likedPosts = JsonConvert.DeserializeObject<List<BuyerPostView>>(response.Content);
            }
            catch (Exception e)
            {
                await DisplayAlert("Ошибка", "Не получилось получить понравившиеся записи", "Попробовать снова");
                return;
            }

            Posts.ItemsSource = likedPosts;
            RefreshView.IsRefreshing = false;

        }
        
        /// <summary>
        /// Отправляет запрос с обновлением фотографии пользователя.
        /// </summary>
        /// <param name="form">Форма с информацией о пользователе и его новой фотграфией.</param>
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
                await DisplayAlert(FailedConnectionToServerAlertTitle, response.Content, "Попробовать снова");
                return;
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                await DisplayAlert(FailedConnectionToServerAlertTitle, response.Content, "Попробовать снова");
                return;
            }

            _buyer.IconPath = _buyer.Username + "icon.jpeg";
        }
        
        /// <summary>
        /// При нажатии пользователя на иконку предоставляет пользователю выбрать фотографию.
        /// </summary>
        /// <param name="sender">Издатель события - Frame.</param>
        /// <param name="e">Аргументы события.</param>
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
        
        /// <summary>
        /// Получает байты фотографии, которую выбрал пользователь.
        /// </summary>
        /// <param name="path">Путь к файлу с указаной фотографией.</param>
        /// <param name="form">Модель, в которую записываются байты фотографии.</param>
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

        /// <summary>
        /// Обработчик события нажатия на кнопку выхода.
        /// </summary>
        /// <param name="sender">Издатель события -  Button.</param>
        /// <param name="e">Аргументы события.</param>
        private async void OnExitToolbarItemTapped(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
        
        /// <summary>
        /// Обновляет информацию о пользователе.
        /// </summary>
        /// <param name="sender">Издатель события - RefreshView.</param>
        /// <param name="e">Аргументы события.</param>
        private void RefreshView_OnRefreshing(object sender, EventArgs e)
        {
            this.BindingContext = _buyer;
            GetBuyerLikedPosts();
        }
        /// <summary>
        /// Открывает страницу пользователя, который опубликовал запись.
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
        /// Отправляет запрос на сервер, чтобы убрать запись из понравившегося.
        /// </summary>
        /// <param name="sender">Издатель события - Image.</param>
        /// <param name="e">Аргументы события.</param>
        private async void SendDislikeImageTapped(object sender, EventArgs e)
        {
            if (!App.IsConnected())
            {
                await DisplayAlert("Нет подключения к интернету", "Проверьте подключение к интернету", "Попробовать снова");
                return;
            }
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

        /// <summary>
        /// Отправляет запрос, чтобы изменить информацию о пользователе.
        /// </summary>
        /// <param name="sender">Издатель события - Editor.</param>
        /// <param name="e">Аргументы события.</param>
        private async void VisualElement_OnUnfocused(object sender, FocusEventArgs e)
        {
            if (!App.IsConnected() || String.IsNullOrWhiteSpace(_buyer.Info))
            {
                return;
            }
            RestClient client = new RestClient($"{App.BaseUrl}api/users/setBuyerInfo?buyerId={_buyer.Id}&info={_buyer.Info}");
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Content-Type", "application/text");
            var response = await client.ExecuteAsync(request);
        }
        
        /// <summary>
        /// Отменяет нажатие на элемент ListView.
        /// </summary>
        /// <param name="sender">Издатель события - ListView.</param>
        /// <param name="e">Аргументы события.</param>
        private void OnPostsItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (sender is ListView listView)
            {
                listView.SelectedItem = null;
            }
        }
    }
}