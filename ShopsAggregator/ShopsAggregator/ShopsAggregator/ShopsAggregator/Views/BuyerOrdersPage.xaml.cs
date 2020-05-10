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
    /// Код страницы заказов пользователя.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BuyerOrdersPage : ContentPage
    {
        /// <summary>
        /// Экземпляр типа пользователя.
        /// </summary>
        private Buyer _buyer;
        /// <summary>
        /// Список заказов пользователя.
        /// </summary>
        private List<BuyerOrderView> buyerOrders = new List<BuyerOrderView>();
        /// <summary>
        /// Конструктор страницы.
        /// </summary>
        /// <param name="buyer">Экземпляр типа пользователя-покупателя.</param>
        public BuyerOrdersPage(Buyer buyer)
        {
            InitializeComponent();
            _buyer = buyer;
        }

        /// <summary>
        /// Получает заказы пользователя и вызывает базовое поведение метода.
        /// </summary>
        protected override void OnAppearing()
        {
            UpdateOrdersAsync();
            base.OnAppearing();
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
        /// Событие обновления заказов.
        /// </summary>
        /// <param name="sender">Издатель события - ListView.</param>
        /// <param name="e">Аргументы события.</param>
        private void OnOrdersUpdate(object sender, EventArgs e)
        {
            UpdateOrdersAsync();
        }


        /// <summary>
        /// Отправляет на сервер запрос, чтобы получить заказы пользователя.
        /// </summary>
        private async void UpdateOrdersAsync()
        {
            Orders.IsRefreshing = true;
            if (!App.IsConnected())
            {
                await DisplayAlert("Ошибка", "Осутствует подключение к интернету", "Поробовать снова");
                return;
            }
            RestClient client = new RestClient($"{App.BaseUrl}api/orders/getBuyerOrders?buyerId={_buyer.Id}");
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
                buyerOrders = JsonConvert.DeserializeObject<List<BuyerOrderView>>(response.Content);
            }
            catch (Exception)
            {
                await DisplayAlert("Ошибка", "Ошибка в обработке данных", "Поробовать снова");
                return;
            }

            Orders.ItemsSource = buyerOrders;
            Orders.IsRefreshing = false;
        }

        /// <summary>
        /// Отменяет нажатие на элемент ListView.
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
        /// Обработчик события удаления заказа.
        /// </summary>
        /// <param name="sender">Издатель события - Button.</param>
        /// <param name="e">Аргументы события.</param>
        private async void DeleteButtonClickedAsync(object sender, EventArgs e)
        {
            if (!App.IsConnected())
            {
                await DisplayAlert("Ошибка", "Осутствует подключение к интернету", "Поробовать снова");
                return;
            }

            if (sender is Button button)
            {
                BuyerOrderView order = (BuyerOrderView) (button.ParentView.BindingContext);
                if (order == null)
                    return;
                RestClient client = new RestClient($"{App.BaseUrl}/api/orders/deleteOrder?orderId={order.OrderId}");
                var request = new RestRequest(Method.DELETE);
                request.AddHeader("Content-Type", "application/text");
                var response = await client.ExecuteAsync(request);
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    await DisplayAlert("Ошибка", "Не удалить заказ", "Поробовать снова");
                    return;
                }

                BuyerOrderView orderFromList =
                    (from orderList in buyerOrders where orderList.OrderId == order.OrderId select orderList)
                    .FirstOrDefault();
                buyerOrders.Remove(orderFromList);
            }
        }
    }
}