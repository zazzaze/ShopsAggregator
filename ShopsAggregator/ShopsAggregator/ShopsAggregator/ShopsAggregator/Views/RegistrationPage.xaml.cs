using System;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using RestSharp;
using ShopsAggregator.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShopsAggregator.Views
{
    /// <summary>
    /// Код страницы регистрации пользователя.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationPage : ContentPage
    {
        /// <summary>
        /// Текст сообщения о неверном Email.
        /// </summary>
        private const String IncorrectEmailMessage = "Email должен иметь вид example@example.com";
        /// <summary>
        /// Текст сообщения о неверном Username.
        /// </summary>
        private const String IncorrectUsernameMessage = "Имя пользователя может содержать только латинские символы или цифры";
        /// <summary>
        /// Текст сообщения о неверном пароле.
        /// </summary>
        private const String IncorrectPasswordMessage=
            "Пароль должен содержать не менее 8 символов и состоять из заглавных и строчных латинских букв и цифр";
        /// <summary>
        /// Текст сообщения о неверном повторном пароле.
        /// </summary>
        private const String IncorrectPasswordAgainMessage = "Введеные пароли не совпадают";
        /// <summary>
        /// Заголовок сообщения о неудачном создании аккаунта.
        /// </summary>
        private const String BadAccountCreateAlertTitle = "Не удалось создать акканут";
        /// <summary>
        /// Текст кнопки сообщения о неудачном создании аккаунта.
        /// </summary>
        private const String BadAccountCreateAlertCancelText = "Попробовать снова";
        /// <summary>
        /// Текст сообщения о неудачном создании аккаунта.
        /// </summary>
        private const String BadRequestAlertMessage = "Ошибка подключения к серверу";
        /// <summary>
        /// Текст кнопки сообщения об удачном создании аккаунта.
        /// </summary>
        private const String SuccessAccountCreateTitle = "Поздравляю!!";
        /// <summary>
        /// Текст сообщения об удачном создании аккаунта.
        /// </summary>
        private const String SuccessAccountCreateMessage = "Аккаунт успешно создан";
        /// <summary>
        /// Заголовок сообщения об удачном создании аккаунта.
        /// </summary>
        private const String SuccessAccountCreateCancel = "Ура"; 
        /// <summary>
        /// Модель регистрации пользователя.
        /// </summary>
        private RegistrationForm form = new RegistrationForm();
        /// <summary>
        /// Конструктор страницы.
        /// </summary>
        public RegistrationPage()
        {
            InitializeComponent();
            CheckBox.CheckedChanged += (sender, args) => CheckBox.Color = (Color)App.Current.Resources["purple"];
            this.BindingContext = form;
        }

        /// <summary>
        /// Проверяет все ли поля заполнены верно и вызывает метод отправки формы на сервер.
        /// </summary>
        /// <param name="sender">Издатель события - Button.</param>
        /// <param name="e">Аргументы события.</param>
        private async void OnRegistrationButtonCLicked(object sender, EventArgs e)
        {
            if (!App.IsConnected())
            {
                await DisplayAlert("Ошибка", "Осутствует подключение к интернету", "Поробовать снова");
                return;
            }
            if (!IsBoxesCorrect())
            {
                return;
            }

            Boolean result = false;
            try
            {
                result = SendRegistrationPost(form);
            }
            catch (Exception)
            {
                await DisplayAlert(BadAccountCreateAlertTitle,BadRequestAlertMessage, BadAccountCreateAlertCancelText);
            }
            if (!result)
                return;
            await DisplayAlert(SuccessAccountCreateTitle, SuccessAccountCreateMessage, SuccessAccountCreateCancel);
            await Navigation.PopAsync();
        }

        /// <summary>
        /// Отправляет на сервер запрос создания аккаунта.
        /// </summary>
        /// <param name="user">Модель регистрацции пользователя, которая отправляется на сервер.</param>
        /// <returns>Результат отправки запроса на сервер.</returns>
        private Boolean SendRegistrationPost(RegistrationForm user)
        {
            String json = JsonConvert.SerializeObject(user);
            var client = new RestClient($"{App.ServerUrl}reg");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", json,  ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                DisplayAlert(BadAccountCreateAlertTitle, response.Content, BadAccountCreateAlertCancelText);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Изменяет цвет CheckBox и его текста при изменении полей ввода данных.
        /// </summary>
        /// <param name="sender">Издатель события - Entry.</param>
        /// <param name="e">Аргументы события.</param>
        private void CheckBoxChanged(object sender, EventArgs e)
        {
            CheckBox.Color = CheckBoxText.TextColor = Color.Default;
        }

        /// <summary>
        /// Проверяет все ли поля для заполнения заполнены верно.
        /// </summary>
        /// <returns>Результат проверки.</returns>
        private Boolean IsBoxesCorrect()
        {
            if (!IsTextboxCorrect(EmailEntry, ValidateEmail))
            {
                MessageAboutIncorrectBox(EmailEntry, IncorrectEmailMessage);
                return false;
            }

            if (!IsTextboxCorrect(UsernameEntry,
                (Char symb) => { return symb >= 'A' && symb <= 'z' || Char.IsDigit(symb); }))
            {
                MessageAboutIncorrectBox(UsernameEntry, IncorrectUsernameMessage);
                return false;
            }

            if (!IsTextboxCorrect(PasswordEntry, ValidatePassword))
            {
                MessageAboutIncorrectBox(PasswordEntry, IncorrectPasswordMessage);
                return false;
            }

            if (!IsTextboxCorrect(PasswordAgainEntry, (String pwd) => pwd == PasswordEntry.Text))
            {
                MessageAboutIncorrectBox(PasswordAgainEntry, IncorrectPasswordAgainMessage);
                App.Shake(PasswordEntry);
                return false;
            }
            if (!CheckBox.IsChecked)
            {
                CheckBoxText.TextColor = CheckBox.Color = Color.Red;
                App.Shake(CheckBoxText);
                App.Shake(CheckBox);
                
                CheckBox.CheckedChanged += (sender, e) => CheckBoxText.TextColor = Color.Default;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Встряхивает неверное поле с данными и выводит соответствующее сообщение.
        /// </summary>
        /// <param name="view">Невернре поле данных.</param>
        /// <param name="errorMsg">Сообщени об ошибке.</param>
        private void MessageAboutIncorrectBox(Entry view, String errorMsg)
        {
            App.Shake(view);
            ErrorRegistration.Text = errorMsg;
            ErrorRegistration.IsVisible = true;
            view.TextChanged += OnEntryTextChanged;
        }

        /// <summary>
        /// Событие об изменении Entry.
        /// </summary>
        /// <param name="sender">Издатель события - Entry.</param>
        /// <param name="e">Аргументы события.</param>
        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Entry entry)
            {
                ErrorRegistration.IsVisible = false;
                entry.TextChanged -= OnEntryTextChanged;
            }
        }
        
        /// <summary>
        /// Проверяет введенные данные.
        /// </summary>
        /// <param name="box">Entry для проверки.</param>
        /// <param name="func">Метод проверки данных.</param>
        /// <returns>Результат проверки.</returns>
        private Boolean IsTextboxCorrect(Entry box, Func<Char, Boolean> func)
        {
            if (String.IsNullOrWhiteSpace(box.Text))
                return false;
            Char[] symbols = box.Text.ToCharArray();
            return Array.TrueForAll(symbols, symb => func(symb));
        }
        
        /// <summary>
        /// Проверяет введенные данные.
        /// </summary>
        /// <param name="box">Entry для проверки.</param>
        /// <param name="func">Метод проверки данных.</param>
        /// <returns>Результат проверки.</returns>
        private Boolean IsTextboxCorrect(Entry box, Func<String, Boolean> func)
        {
            if (String.IsNullOrWhiteSpace(box.Text))
                return false;

            return func(box.Text);
        }

        /// <summary>
        /// Проверяет, является ли введеные email верным.
        /// </summary>
        /// <param name="email">Строка с email.</param>
        /// <returns>Результат проверки.</returns>
        private Boolean ValidateEmail(String email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                    RegexOptions.None, TimeSpan.FromMilliseconds(200));
                
                string DomainMapper(Match match)
                {
                    
                    var idn = new IdnMapping();
                    
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// Проверяет является ли введеный пароль допустимым. 
        /// </summary>
        /// <param name="input">Введеный пароль.</param>
        /// <returns>Результат проверки.</returns>
        private Boolean ValidatePassword(String input)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum8Chars = new Regex(@".{8,}");

            return hasNumber.IsMatch(input) && hasUpperChar.IsMatch(input) && hasMinimum8Chars.IsMatch(input);
        }
        
    }
}