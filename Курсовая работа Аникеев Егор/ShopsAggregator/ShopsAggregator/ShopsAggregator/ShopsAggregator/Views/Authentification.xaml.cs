using System;
using System.Net;
using Newtonsoft.Json;
using RestSharp;
using ShopsAggregator.Models;
using Xamarin.Forms;

namespace ShopsAggregator.Views
{
    /// <summary>
    /// Код страницы авторизации пользователя.
    /// </summary>
    public partial class Authentification : ContentPage
    {
        /// <summary>
        /// Текст сообщения о том, что поле имени пользователя пустое.
        /// </summary>
        private const String IncorrectUsernameInputErrorMessage = "Имя ползователя не может быть пустым";
        /// <summary>
        /// Текст сообщения о том, что поля пароля пустое.
        /// </summary>
        private const String IncorrectPasswordInputErrorMessage = "Необходимо ввести пароль";
        /// <summary>
        /// Заголовок сообщения о неудачном входе.
        /// </summary>
        private const String BadSignInTryAlertTitle = "Неудачная попытка входа";
        /// <summary>
        /// Текст сообщения о неудачном входе.
        /// </summary>
        private const String BadSignInTryAlertCancelText = "Попробовать снова";
        
        /// <summary>
        /// Конструктор страницы.
        /// </summary>
        public Authentification()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        /// <summary>
        /// Устанавливает цвет границ кнопок авторизации и вызывает базовое поведение метода.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            buyerSignInButton.BorderColor = sellerSignInButton.BorderColor =  Color.FromRgba(12, 12, 12, 1);
        }

        /// <summary>
        /// Событие попытки входа в аккаунт пользователя-покупателя.
        /// </summary>
        /// <param name="sender">Издатель события - Button buyerSignInButton.</param>
        /// <param name="e">Аргументы события.</param>
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

        /// <summary>
        /// Событие попытки входа в аккаунт пользователя-продавца.
        /// </summary>
        /// <param name="sender">Издатель события - Button sellerSignInButton.</param>
        /// <param name="e">Аргументы события.</param>
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

        /// <summary>
        /// Проверяет есть ли соединение с интернетом и все ли поля заполнены верно.
        /// </summary>
        /// <returns>Результат проверки.</returns>
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

        /// <summary>
        /// Отправляет запрос на сервер для входа в аккаунт.
        /// </summary>
        /// <param name="login">Логин, под которым пользователь хочет войти в аккаунт.</param>
        /// <param name="password">Пароль, под которым пользователь хочет войти в аккаунт.</param>
        /// <param name="authType">Строка, под каким типом хочет войти пользователь.</param>
        /// <typeparam name="T">Ожидаемый тип возврата с сервера.</typeparam>
        /// <returns>Результат запроса в виде экземпляра типа Т.</returns>
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
            return JsonConvert.DeserializeObject<T>(response.Content);
        }
        
        /// <summary>
        /// Вызывает анимацию встряски View элемента и выводит сообщение о том что поле заполнено неверно.
        /// </summary>
        /// <param name="view">View элемент, который заполнен неверно.</param>
        /// <param name="errorMsg">Сообщение об ошибки, котороые необходимо вывести.</param>
        private void MessageAboutIncorrectBox(Entry view, String errorMsg)
        {
            App.Shake(view);
            StatusOfSignIn.Text = errorMsg;
            StatusOfSignIn.IsVisible = true;
            view.TextChanged += OnEntryTextChanged;
        }

        /// <summary>
        /// Убирает сообщение о статусе входа при измененнии полей входа.
        /// </summary>
        /// <param name="sender">Издатель события - Entry.</param>
        /// <param name="e">Аргументы события.</param>
        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Entry entry)
            {
                StatusOfSignIn.IsVisible = false;
                entry.TextChanged -= OnEntryTextChanged;
            }
        }

        
        /// <summary>
        /// Перенаправляет пользователя на страницу регистрации.
        /// </summary>
        /// <param name="sender">Издатель события - Button.</param>
        /// <param name="e">Аргументы события.</param>
        private async void OnRegistrationButtonClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegistrationPage());
        }
    }
}