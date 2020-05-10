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
using ShopsAggregator.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShopsAggregator.Views
{
    /// <summary>
    /// Код страницы пользователя-продавца.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SellerMainPage : ContentPage
    {
        /// <summary>
        /// Заголовок сообщения об отсутствии подключения.
        /// </summary>
        private const String FailedConnectionToServerAlertTitle = "Ошибка подключения к серверу";
        /// <summary>
        /// Экземпляр типа пользователя-продавца.
        /// </summary>
        private Seller _seller;
        /// <summary>
        /// Список записей пользователя.
        /// </summary>
        private List<Post> sellerPosts = new List<Post>();

        /// <summary>
        /// Конструктор страницы.
        /// </summary>
        /// <param name="seller">Экземпляр типа пользователя-продавца.</param>
        public SellerMainPage(Seller seller)
        {
            InitializeComponent();
            this._seller = seller;
            this.BindingContext = seller;
            RefreshView.Refreshing += PostsOnRefreshing;
            Posts.WidthRequest = App.Current.MainPage.Width;
        }

        
        /// <summary>
        /// Обработчик события обновления данных.
        /// </summary>
        /// <param name="sender">Издатель события - RefreshView.</param>
        /// <param name="e">Аргументы события.</param>
        private void PostsOnRefreshing(object sender, EventArgs e)
        {
            GetSellerPosts(_seller.Username);
        }

        /// <summary>
        /// Получает записи пользователя и вызывает базовое поведение метода.
        /// </summary>
        protected override void OnAppearing()
        {
            SubscribersCounter.Text = _seller.Subscribers.Count.ToString();
            GetSellerPosts(_seller.Username);
            base.OnAppearing();
        }
        
        /// <summary>
        /// Получает записи пользователя с сервера.
        /// </summary>
        /// <param name="sellerName">Имя пользователя.</param>
        private async void GetSellerPosts(String sellerName)
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
                return;
            }

            try
            {
                sellerPosts = JsonConvert.DeserializeObject<List<Post>>(response.Content);
            }
            catch(Exception e){}
            Posts.ItemsSource = sellerPosts;
            RefreshView.IsRefreshing = false;
        }

        /// <summary>
        /// Обработчик нажатия на кнопку выхода.
        /// </summary>
        /// <param name="sender">Издатель события - Button.</param>
        /// <param name="e">Аргументы события.</param>
        private void OnExitToolbarItemTapped(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        /// <summary>
        /// Отправляет запрос на сервер для обновления фотографии пользователя.
        /// </summary>
        /// <param name="form">Модель формы с информацией о фотографии.</param>
        private async void SendUpdateUserPut(IconPostForm form)
        {
            if (!App.IsConnected())
            { 
                return;
            }
            String json = JsonConvert.SerializeObject(form);
            var client = new RestClient($"{App.ServerUrl}addSellerIcon");
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

            _seller.IconPath = _seller.Username + "icon.jpeg";
        }
        
        /// <summary>
        /// Обработчик события получения пользователем фотографии.
        /// </summary>
        /// <param name="sender">Издатель события - Frame.</param>
        /// <param name="e">Аргументы события.</param>
        private async void GetPhoto(object sender, EventArgs e)
        { 
            String iconPath = String.Empty;
            if (CrossMedia.Current.IsPickPhotoSupported)
            {
                MediaFile photo = await CrossMedia.Current.PickPhotoAsync();
                iconPath = photo.Path;
                Icon.Source = ImageSource.FromFile(iconPath);
                IconPostForm form = new IconPostForm(_seller.Id);
                GetPhotoBytes(iconPath, form);
                if (!App.IsConnected())
                {
                    return;
                }
                SendUpdateUserPut(form);
            }
        }
        
        /// <summary>
        /// Получает байты фотографии.
        /// </summary>
        /// <param name="path">Путь к файлу с фотографией.</param>
        /// <param name="form">Форма для запроса, в которую записываются байты фотографии.</param>
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
        /// Отправляет запрос, чтобы изменить информацию о пользователе.
        /// </summary>
        /// <param name="sender">Издатель события - Editor.</param>
        /// <param name="e">Аргументы события.</param>
        private async void VisualElement_OnUnfocused(object sender, FocusEventArgs e)
        {
            if (!App.IsConnected() || String.IsNullOrWhiteSpace(_seller.Info))
            {
                return;
            }
            RestClient client = new RestClient($"{App.BaseUrl}api/users/setSellerInfo?sellerId={_seller.Id}&info={_seller.Info}");
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Content-Type", "application/json");
            var response = await client.ExecuteAsync(request);
        }
    }
}