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
using ShopsAggregator.Services;
using ShopsAggregatorLib;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;

namespace ShopsAggregator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserSettingsPage : ContentPage
    {
        private String iconPath;
        private User user;
        private const String FailedConnectionToServerAlertTitle = "Ошибка подключения к серверу";
        public UserSettingsPage(User user)
        {
            InitializeComponent();
            this.user = user;
            this.BindingContext = this.user;
            Icon.Source = App.BaseUrl + user.IconPath;
            if (user.Id != App.mainUser.Id)
            {
                UpdateUserInfoButton.IsVisible = false;
                if (App.mainUser.Subscribed.Contains(user.Id))
                    UnsubscribeButton.IsVisible = true;
                else
                    SubscribeButton.IsVisible = true;
            }
            SetSubscribers();
        }

        private void SetSubscribers()
        {
            SubscribersCounter.Text = user.Subscribers.Count.ToString();
            SubsribedCounter.Text = user.Subscribed.Count.ToString();
        }

        private void SaveUserInfo(object sender, EventArgs e)
        {
            if (!App.IsConnected())
            {
                
            }
            SendUpdateUserPut(user);
        }

        private void Subscribe(object sender, EventArgs e)
        {
            if (!App.IsConnected())
            {
                
            }
            SubscribeResponce();
        }

        private async void SubscribeResponce()
        {
            RestClient client = new RestClient(App.BaseUrl+$"api/sub/addSub?id={App.mainUser.Id}&toId={user.Id}");
            client.Timeout = -1;
            var request = new RestRequest(Method.PUT);
            IRestResponse response = await client.ExecuteAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                DisplayAlert("Ошибка подписки", "Не удалось подписаться", "Жаль:(");
                return;
            }

            if (App.mainUser.Subscribed != null)
                App.mainUser.Subscribed = new List<String>();
            App.mainUser.Subscribed.Add(user.Id);
            SubscribeButton.IsVisible = false;
            UnsubscribeButton.IsVisible = true;
            SetSubscribers();
        } 
        
        private async void SendUpdateUserPut(User user)
        {
            String json = JsonConvert.SerializeObject(user);
            var client = new RestClient($"{App.ServerUrl}update");
            client.Timeout = -1;
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", json,  ParameterType.RequestBody);
            IRestResponse response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.RequestEntityTooLarge)
            {
                DisplayAlert(FailedConnectionToServerAlertTitle, response.Content, "Попробовать снова");
            } 
            try
            {
                user = App.DeserializeUser(response.Content);
            }
            catch (Exception)
            {
                DisplayAlert("Все плохо", "Put не сработал", "Жаль");
            }
        }
        
        private async void GetPhoto(object sender, EventArgs e)
        {
            if (CrossMedia.Current.IsPickPhotoSupported)
            {
                MediaFile photo = await CrossMedia.Current.PickPhotoAsync();
                iconPath = photo.Path;
                Icon.Source = ImageSource.FromFile(iconPath);
                GetPhotoBytes(iconPath, this.user);
            }
        }
        
        private void GetPhotoBytes(String path, User user)
        {
            List<Int32> bytes = new List<Int32>();
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                while(fs.Position != fs.Length)
                    bytes.Add(fs.ReadByte());
            }

            user.IconBytesArr = bytes.ToArray();
        }

        private void OnExitToolbarItemTapped(object sender, EventArgs e)
        {
            Navigation.PopToRootAsync();
        }

        private void Unsubscribe(object sender, EventArgs e)
        {
            //TODO: Написать запрос отписки
            throw new NotImplementedException();
        }
    }
}