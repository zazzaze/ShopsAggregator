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
using ShopsAggregator.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShopsAggregator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SellerMainPage : ContentPage
    {
        private const String FailedConnectionToServerAlertTitle = "Ошибка подключения к серверу";
        private Seller seller;
        public SellerMainPage(Seller seller)
        {
            InitializeComponent();
            this.seller = seller;
            this.BindingContext = seller;
        }

        protected override void OnAppearing()
        {
            SubscribersCounter.Text = seller.Subscribers.Count.ToString();
            base.OnAppearing();
        }

        private void OnExitToolbarItemTapped(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private async void SendUpdateUserPut(IconPostForm form)
        {
            String json = JsonConvert.SerializeObject(form);
            var client = new RestClient($"{App.ServerUrl}addSellerIcon");
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

            seller.IconPath = seller.Username + "icon.jpeg";
            Icon.Source = App.BaseUrl + seller.IconPath;
        }
        
        private async void GetPhoto(object sender, EventArgs e)
        { 
            String iconPath = String.Empty;
            if (CrossMedia.Current.IsPickPhotoSupported)
            {
                MediaFile photo = await CrossMedia.Current.PickPhotoAsync();
                iconPath = photo.Path;
                Icon.Source = ImageSource.FromFile(iconPath);
                IconPostForm form = new IconPostForm(seller.Id);
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
    }
}