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
    /// Код страницы заказы пользователя-продавца.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SellerOrdersPage : ContentPage
    {
        /// <summary>
        /// Экземпляр типа пользователя-продавца.
        /// </summary>
        private readonly Seller _seller;
        /// <summary>
        /// Список заказов пользователя-продавца.
        /// </summary>
        private List<SellerOrderView> sellerOrders = new List<SellerOrderView>();

        /// <summary>
        /// Конструктор страницы.
        /// </summary>
        /// <param name="seller">Экземпляр типа пользователя-продавца.</param>
        public SellerOrdersPage(Seller seller)
        {
            _seller = seller;
            InitializeComponent();
        }
        
        /// <summary>
        /// Вызывает метод получения заказов пользователя и вызывает базовое поведение метода.
        /// </summary>
        protected override void OnAppearing()
        {
            UpdateOrdersAsync();
            base.OnAppearing();
        }
        
        /// <summary>
        /// Обработчик события обновления заказов.
        /// </summary>
        /// <param name="sender">Издатель события - ListView.</param>
        /// <param name="e">Аргументы события.</param>
        private void OnOrdersUpdate(object sender, EventArgs e)
        {
            UpdateOrdersAsync();
        }

        /// <summary>
        /// Обновляет заказы пользователя.
        /// </summary>
        private async void UpdateOrdersAsync()
        {
            Orders.IsRefreshing = true;
            if (!App.IsConnected())
            {
                await DisplayAlert("Ошибка", "Осутствует подключение к интернету", "Поробовать снова");
                return;
            }
            RestClient client = new RestClient($"{App.BaseUrl}api/orders/getSellerOrders?sellerId={_seller.Id}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/text");
            var response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                await DisplayAlert("Ошибка", "Не удалось получить данные", "Поробовать снова");
                return;
            }

            try
            {
                sellerOrders = JsonConvert.DeserializeObject<List<SellerOrderView>>(response.Content);
            }
            catch (Exception)
            {
                await DisplayAlert("Ошибка", "Ошибка в обработке данных", "Поробовать снова");
                return;
            }

            Orders.ItemsSource = sellerOrders;
            Orders.IsRefreshing = false;
        }

        /// <summary>
        /// Отменяет событие нажатия на элемент из ListView.
        /// </summary>
        /// <param name="sender">Издатель события - ListView.</param>
        /// <param name="e">Аргументы события.</param>
        private void OnOrdersItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (sender is ListView listView)
            {
                listView.SelectedItem = null;
            }
        }
        
        /// <summary>
        /// Ставит статус заказа одобреным и отправляет запрос на сервер.
        /// </summary>
        /// <param name="sender">Издатель события - Button.</param>
        /// <param name="e">Аргументы события.</param>
        private void SetOrderSuccess(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                Int32 orderId = ((SellerOrderView) (button.ParentView.BindingContext)).OrderId;
                SendOrderStatusAsync("setOrderSuccess", orderId);
                foreach (SellerOrderView sellerOrderView in sellerOrders)
                {
                    if (sellerOrderView.OrderId == orderId)
                        sellerOrderView.IsSucceded = true;
                }
            }
        }

        /// <summary>
        /// Ставит статус заказа отклоненым и отправляет запрос на сервер.
        /// </summary>
        /// <param name="sender">Издатель события - Button.</param>
        /// <param name="e">Аргументы события.</param>
        private void SetOrderCanceledStatus(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                Int32 orderId = ((SellerOrderView) (button.ParentView.BindingContext)).OrderId;
                SendOrderStatusAsync("setOrderCanceled", orderId);
                foreach (SellerOrderView sellerOrderView in sellerOrders)
                {
                    if (sellerOrderView.OrderId == orderId)
                        sellerOrderView.IsCanceled = true;
                }
            }
        }

        /// <summary>
        /// Отправляет запрос на сервер чтобы обновить статус заказа.
        /// </summary>
        /// <param name="reqStatus">Строка запроса на сервер.</param>
        /// <param name="orderId">Id заказа.</param>
        private async void SendOrderStatusAsync(String reqStatus, Int32 orderId)
        {
            Orders.IsRefreshing = true;
            if (!App.IsConnected())
            {
                await DisplayAlert("Ошибка", "Осутствует подключение к интернету", "Поробовать снова");
                return;
            }
            RestClient client = new RestClient($"{App.BaseUrl}api/orders/{reqStatus}?orderId={orderId}");
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Content-Type", "application/text");
            var response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                await DisplayAlert("Ошибка", "Не удалось изменить статус заказа", "Поробовать снова");
                return;
            }

            Orders.ItemsSource = sellerOrders;
            Orders.IsRefreshing = false;
        }
    }
}