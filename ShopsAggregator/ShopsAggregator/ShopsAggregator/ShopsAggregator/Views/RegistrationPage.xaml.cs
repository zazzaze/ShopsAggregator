using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Libmongocrypt;
using Newtonsoft.Json;
using RestSharp;
using ShopsAggregatorLib;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShopsAggregator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationPage : ContentPage
    {
        private const String IncorrectEmailMessage = "Email должен иметь вид example@example.com";
        private const String IncorrectUsernameMessage = "Имя пользователя может содержать только латинские символы или цифры";

        private const String IncorrectPasswordMessage=
            "Пароль должен содержать не менее 8 символов и состоять из заглавных и строчных латинских букв и цифр";

        private const String IncorrectPasswordAgainMessage = "Введеные пароли не совпадают";
        private const String BadAccountCreateAlertTitle = "Не удалось создать акканут";
        private const String BadAccountCreateAlertCancelText = "Попробовать снова";
        private User newUser = new User();
        public RegistrationPage()
        {
            InitializeComponent();
            CheckBox.CheckedChanged += (sender, args) => CheckBox.Color = (Color)App.Current.Resources["purple"];
            this.BindingContext = newUser;
        }

        private void OnRegistrationButtonCLicked(object sender, EventArgs e)
        {
            if (!App.IsConnected())
            {
                // TODO: Написать DisplayAlert о том, что нет подключения к интернету
                return;
            }
            if (!IsBoxesCorrect())
            {
                return;
            }

            if (IsSeller.IsToggled)
                newUser = new Seller(newUser);
            else
                newUser = new Buyer(newUser);
            User registredUser = SendRegistrationPost(newUser);
            if (registredUser == null)
                return;
            var mainTabbedPage = new MainTabbedPage(newUser);
            NavigationPage.SetHasNavigationBar(mainTabbedPage, true);
            NavigationPage.SetHasBackButton(mainTabbedPage, false);
            Navigation.PushAsync(mainTabbedPage);
        }

        private User SendRegistrationPost(User user)
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
                return null;
            }


            return HelpMethods<User>.GetFullCopy(user);
        }

        private void CheckBoxChanged(object sender, EventArgs e)
        {
            CheckBox.Color = CheckBoxText.TextColor = Color.Default;
        }

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

        private void MessageAboutIncorrectBox(Entry view, String errorMsg)
        {
            App.Shake(view);
            ErrorRegistration.Text = errorMsg;
            ErrorRegistration.IsVisible = true;
            view.TextChanged += OnEntryTextChanged;
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Entry entry)
            {
                ErrorRegistration.IsVisible = false;
                entry.TextChanged -= OnEntryTextChanged;
            }
        }
        
        private Boolean IsTextboxCorrect(Entry box, Func<Char, Boolean> func)
        {
            if (String.IsNullOrWhiteSpace(box.Text))
                return false;
            Char[] symbols = box.Text.ToCharArray();
            return Array.TrueForAll(symbols, symb => func(symb));
        }
        
        private Boolean IsTextboxCorrect(Entry box, Func<String, Boolean> func)
        {
            if (String.IsNullOrWhiteSpace(box.Text))
                return false;

            return func(box.Text);
        }

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

        private Boolean ValidatePassword(String input)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum8Chars = new Regex(@".{8,}");

            return hasNumber.IsMatch(input) && hasUpperChar.IsMatch(input) && hasMinimum8Chars.IsMatch(input);
        }
        
    }
}