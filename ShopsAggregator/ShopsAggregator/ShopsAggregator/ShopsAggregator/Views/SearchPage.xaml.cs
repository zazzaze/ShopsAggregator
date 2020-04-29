using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using ShopsAggregatorLib;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShopsAggregator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        private const String connectionAlertTitle = "Невозможно сделать запрос";
        private String connectionAlertContent = "Отсутствует подключение к интернету";
        private const String alertCancel = "Попробовать снова";
        private const String getResponseAlertTitle = "Ошибка запроса";
        private const String getResponseAlertContent = "Не удалось получить данныу";
        private List<User> searchedUsers;
        public SearchPage()
        {
            InitializeComponent();
        }

        private void OnSearchButtonPressed(object sender, EventArgs e)
        {
            if (!App.IsConnected())
            {
                DisplayAlert(connectionAlertTitle, connectionAlertContent, alertCancel);
                return;
            }
            if (sender is SearchBar searchBar)
            {
                if (String.IsNullOrWhiteSpace(searchBar.Text))
                    return;
                Search(searchBar.Text);
            }
        }

        private async void Search(String searchText)
        {
            String responseContent = String.Empty;
            try
            {
                responseContent = await TryGetSearchResult(searchText);
            }
            catch (Exception)
            {
                DisplayAlert(getResponseAlertTitle, getResponseAlertContent, alertCancel);
                return;
            }

            List<User> users;
            try
            {
                users = JsonConvert.DeserializeObject<List<User>>(responseContent);
            }
            catch (Exception)
            {
                DisplayAlert(getResponseAlertTitle, getResponseAlertContent, alertCancel);
                return;
            }

            searchedUsers = users;
            UsersListView.ItemsSource = users;
        }

        private async Task<String> TryGetSearchResult(String searchLine)
        {
            var client = new RestClient($"{App.BaseUrl}api/search?q={searchLine}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = await client.ExecuteAsync(request);
            return response.Content;
        }

        private void OnSearchItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (sender is ListView list)
            {
                if (searchedUsers == null)
                    return;
                Int32 index = e.SelectedItemIndex;
                if (index < 0 || index >= searchedUsers.Count)
                    return;
                Navigation.PushAsync(new UserSettingsPage(searchedUsers[e.SelectedItemIndex]));
                list.SelectedItem = null;
            }
        }
    }
}