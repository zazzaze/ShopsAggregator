using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp;
using ShopsAggregator.Models;
using ShopsAggregator.Views;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace ShopsAggregator
{
    public partial class Authentification : ContentPage
    {
        private const String IncorrectUsernameInputErrorMessage = "Имя ползователя не может быть пустым";
        private const String IncorrectPasswordInputErrorMessage = "Необходимо ввести пароль";
        private const String BadSignInTryAlertTitle = "Неудачная попытка входа";
        private const String BadSignInTryAlertCancelText = "Попробовать снова";
        public Authentification()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            buyerSignInButton.BorderColor = sellerSignInButton.BorderColor =  Color.FromRgba(12, 12, 12, 1);
        }

        private void OnBuyerSignInButtonClicked(object sender, EventArgs e)
        {
            if (!CheckIsBoxesAndConnectionCorrect())
                return;
            Buyer user = null;
            try
            {
                user = TrySignIn<Buyer>(username.Text, password.Text, "authBuyer");
            }
            catch (Exception exception)
            {
                DisplayAlert(BadSignInTryAlertTitle, "Ошибка сервера", BadSignInTryAlertCancelText);
                return;
            } 
            if (user == null)
            {
                DisplayAlert(BadSignInTryAlertTitle, "Проверьте введеные данные", BadSignInTryAlertCancelText);
                return;
            }
            
            var mainTabbedPage = new MainTabbedPage(user);
            NavigationPage.SetHasNavigationBar(mainTabbedPage, true);
            NavigationPage.SetHasBackButton(mainTabbedPage, false);
            Navigation.PushAsync(mainTabbedPage);
        }

        private void OnSellerSignInButtonClicked(object sender, EventArgs e)
        {
            if (!CheckIsBoxesAndConnectionCorrect())
                return;
            Seller user = null;
            try
            {
                user = TrySignIn<Seller>(username.Text, password.Text, "authSeller");
            }
            catch (Exception)
            {
                DisplayAlert(BadSignInTryAlertTitle, "Ошибка сервера", BadSignInTryAlertCancelText);
                return;
            } 
            if(user == null)
            {
                DisplayAlert(BadSignInTryAlertTitle, "Проверьте введеные данные", BadSignInTryAlertCancelText);
                return;
            }
            var mainTabbedPage = new MainTabbedPage(user);
            NavigationPage.SetHasNavigationBar(mainTabbedPage, true);
            NavigationPage.SetHasBackButton(mainTabbedPage, false);
            Navigation.PushAsync(mainTabbedPage);
        }

        private Boolean CheckIsBoxesAndConnectionCorrect()
        {
            if (!App.IsConnected())
            {
                DisplayAlert("Нет подключения к интернету", "Проверьте подключение к интернету", "Попробовать снова");
                return false;
            }
            if (String.IsNullOrWhiteSpace(username.Text))
            {
                MessageAboutIncorrectBox(username, IncorrectUsernameInputErrorMessage);
                return false;
            }

            if (String.IsNullOrEmpty(password.Text))
            {
                MessageAboutIncorrectBox(password, IncorrectPasswordInputErrorMessage);
                return false;
            }

            return true;
        }

        private T TrySignIn<T>(String login, String password, String authType) where T : class
        {
            
            var client = new RestClient($"{App.ServerUrl}{authType}?login={login}&password={password}");
            client.Timeout = 10000;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            User user;
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return null;
            }
            return App.Deserialize<T>(response.Content);
        }
        
        private void MessageAboutIncorrectBox(Entry view, String errorMsg)
        {
            App.Shake(view);
            StatusOfSignIn.Text = errorMsg;
            StatusOfSignIn.IsVisible = true;
            view.TextChanged += OnEntryTextChanged;
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Entry entry)
            {
                StatusOfSignIn.IsVisible = false;
                entry.TextChanged -= OnEntryTextChanged;
            }
        }

        private void OnRegistrationButtonClick(object sender, EventArgs e)
        {
            Navigation.PushAsync(new RegistrationPage());
        }
    }
}