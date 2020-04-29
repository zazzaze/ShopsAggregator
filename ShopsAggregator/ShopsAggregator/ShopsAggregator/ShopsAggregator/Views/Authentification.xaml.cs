using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp;
using ShopsAggregatorLib;
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
            signInButton.CornerRadius = 5;
            signInButton.BorderColor = Color.FromRgba(12, 12, 12, 1);
        }

        private void OnLoginButtonClicked(object sender, EventArgs e)
        {
            if (!App.IsConnected())
            {
                DisplayAlert("Нет подключения к интернету", "Проверьте подключение к интернету", "Попробовать снова");
                return;
            }
            if (String.IsNullOrWhiteSpace(username.Text))
            {
                MessageAboutIncorrectBox(username, IncorrectUsernameInputErrorMessage);
                return;
            }

            if (String.IsNullOrEmpty(password.Text))
            {
                MessageAboutIncorrectBox(password, IncorrectPasswordInputErrorMessage);
                return;
            }

            User user = null;
            try
            {
                user = TrySignIn(username.Text, password.Text);
            }
            catch (Exception)
            {
                DisplayAlert(BadSignInTryAlertTitle, "Ошибка сервера", BadSignInTryAlertCancelText);
                return;
            } 
            if (user == null)
            {
                DisplayAlert(BadSignInTryAlertTitle, "Проверьте введеные данные", BadSignInTryAlertCancelText);
                return;
            }

            App.mainUser = user;
            var mainTabbedPage = new MainTabbedPage(user);
            NavigationPage.SetHasNavigationBar(mainTabbedPage, true);
            NavigationPage.SetHasBackButton(mainTabbedPage, false);
            Navigation.PushAsync(mainTabbedPage);
        }

        private User TrySignIn(String login, String password)
        {
            
            var client = new RestClient($"{App.ServerUrl}auth?login={login}&password={password}");
            client.Timeout = 10000;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            User user;
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return null;
            }
            return App.DeserializeUser(response.Content);
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