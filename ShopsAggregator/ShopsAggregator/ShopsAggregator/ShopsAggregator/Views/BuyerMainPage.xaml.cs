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
using ShopsAggregator.Models;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;

namespace ShopsAggregator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BuyerMainPage : ContentPage
    {
        private String iconPath;
        private Buyer buyer;
        private const String FailedConnectionToServerAlertTitle = "Ошибка подключения к серверу";
        public BuyerMainPage(Buyer buyer)
        {
            InitializeComponent();
            this.buyer = buyer;
            this.BindingContext = this.buyer;
        }

        protected override void OnAppearing()
        {
            Icon.Source = App.BaseUrl + buyer.IconPath;
            Posts.RowHeight = (Int32)(App.Current.MainPage.Height / 2);
            Posts.WidthRequest = App.Current.MainPage.Width;
            Posts.ItemsSource = new []{new Post{Id = 1}};
            SubscribedCounter.Text = buyer.Subscribed.Count.ToString();
            base.OnAppearing();
        }

        private async void SendUpdateUserPut(IconPostForm form)
        {
            String json = JsonConvert.SerializeObject(form);
            var client = new RestClient($"{App.ServerUrl}addBuyerIcon");
            client.Timeout = -1;
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", json,  ParameterType.RequestBody);
            IRestResponse response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.RequestEntityTooLarge)
            {
                DisplayAlert(FailedConnectionToServerAlertTitle, response.Content, "Попробовать снова");
                return;
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                DisplayAlert(FailedConnectionToServerAlertTitle, response.Content, "Попробовать снова");
                return;
            }

            buyer.IconPath = buyer.Username + "icon.jpeg";
            Icon.Source = App.BaseUrl + buyer.IconPath;
        }
        
        private async void GetPhoto(object sender, EventArgs e)
        {
            if (CrossMedia.Current.IsPickPhotoSupported)
            {
                MediaFile photo = await CrossMedia.Current.PickPhotoAsync();
                iconPath = photo.Path;
                Icon.Source = ImageSource.FromFile(iconPath);
                IconPostForm form = new IconPostForm(buyer.Id);
                GetPhotoBytes(iconPath, form);
                if (!App.IsConnected())
                {
                    return;
                }
                SendUpdateUserPut(form);
            }
        }
        
        private void GetPhotoBytes(String path, IconPostForm form)
        {
            List<Int32> bytes = new List<Int32>();
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                while(fs.Position != fs.Length)
                    bytes.Add(fs.ReadByte());
            }

            form.IconBytesArr = bytes.ToList();
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