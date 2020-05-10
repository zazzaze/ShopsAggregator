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
    /// Код страницы поиска пользователей-продавцов.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        /// <summary>
        /// Заголовок сообщения о неудачном запросе к серверу.
        /// </summary>
        private const String connectionAlertTitle = "Невозможно сделать запрос";
        /// <summary>
        /// Текст сообщения об отсутствии интернета.
        /// </summary>
        private String connectionAlertContent = "Отсутствует подключение к интернету";
        /// <summary>
        /// Текст кнопки сообщения.
        /// </summary>
        private const String alertCancel = "Попробовать снова";
        /// <summary>
        /// Заголовок сообщения о неудачном запросе к серверу.
        /// </summary>
        private const String getResponseAlertTitle = "Ошибка запроса";
        /// <summary>
        /// Текст сообщения о неудачном получении данных.
        /// </summary>
        private const String getResponseAlertContent = "Не удалось получить данныу";
        /// <summary>
        /// Список найденных пользователей.
        /// </summary>
        private List<String> searchedSellers;
        /// <summary>
        /// Экземпляр типа пользователя-покупателя.
        /// </summary>
        private Buyer _buyer;
        /// <summary>
        /// Конструктор страницы.
        /// </summary>
        /// <param name="buyer">Экземпляр типа пользователя-покупателя.</param>
        public SearchPage(Buyer buyer)
        {
            InitializeComponent();
            _buyer = buyer;
        }

        /// <summary>
        /// Обработчик события поиска пользователя по имени.
        /// </summary>
        /// <param name="sender">Издатель события - SearchBar.</param>
        /// <param name="e">Аргументы события.</param>
        private async void OnSearchButtonPressed(object sender, EventArgs e)
        {
            if (!App.IsConnected())
            {
                await DisplayAlert(connectionAlertTitle, connectionAlertContent, alertCancel);
                return;
            }
            if (sender is SearchBar searchBar)
            {
                if (String.IsNullOrWhiteSpace(searchBar.Text))
                    return;
                Search(searchBar.Text);
            }
        }

        /// <summary>
        /// Вызывает метод с запросом на сервер, получает результат работы сервера и обрабатывает их.
        /// </summary>
        /// <param name="searchText">Текст запроса пользователя.</param>
        private async void Search(String searchText)
        {
            String responseContent = String.Empty;
            try
            {
                responseContent = await TryGetSearchResult(searchText);
            }
            catch (Exception)
            {
                await DisplayAlert(getResponseAlertTitle, getResponseAlertContent, alertCancel);
                return;
            }

            List<String> sellers;
            try
            {
                sellers = JsonConvert.DeserializeObject<List<String>>(responseContent);
            }
            catch (Exception)
            {
                await DisplayAlert(getResponseAlertTitle, getResponseAlertContent, alertCancel);
                return;
            }

            searchedSellers = sellers;
            UsersListView.ItemsSource = sellers;
        }

        /// <summary>
        /// Отправляет запрос на сервер для поиска пользователя.
        /// </summary>
        /// <param name="searchLine">Строка поиска пользователя.</param>
        /// <returns>Результат запроса.</returns>
        private async Task<String> TryGetSearchResult(String searchLine)
        {
            var client = new RestClient($"{App.BaseUrl}api/search?q={searchLine}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = await client.ExecuteAsync(request);
            return response.Content;
        }

        /// <summary>
        /// Обработчик события нажатия на элемент списка найденных пользователей.
        /// </summary>
        /// <param name="sender">Издатель события - ListView.</param>
        /// <param name="e">Аргументы события.</param>
        private async void OnSearchItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (sender is ListView list)
            {
                if (searchedSellers == null)
                    return;
                Int32 index = e.SelectedItemIndex;
                if (index < 0 || index >= searchedSellers.Count)
                    return;
                list.SelectedItem = null;
                await Navigation.PushAsync(new WatchSellerPage(searchedSellers[index], _buyer));
            }
        }
    }
}