using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using ShopsAggregator.Models;
using ShopsAggregator.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShopsAggregator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreatePostPage : ContentPage
    {
        private Seller seller;
        private CreatePostForm form;
        public CreatePostPage(Seller seller)
        {
            InitializeComponent();
            this.seller = seller;
            form = new CreatePostForm(seller.Id);
            this.BindingContext = form;
        }

        private async void OnGetPostPhotoClicked(object sender, EventArgs e)
        {
            Stream stream = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
            if (stream != null)
            {
                PostImage.Source = ImageSource.FromStream(() => stream);
                GetImageBytesFromStream(stream);
            }
        }

        private void GetImageBytesFromStream(Stream stream)
        {
            while (stream.Position != stream.Length)
            {
                form.ImageBytes.Add(stream.ReadByte());
            }
        }
        
        
        private async void CreatePostButtonClicked(object sender, EventArgs e)
        {
            if (form.ImageBytes.Count == 0)
            {
                //TODO: Написать сообщение о том что фотография не выбрана
                return;
            }

            if (form.CreatorId == 0)
            {
                //TODO: Написать сообщение об ошибке
                return;
            }
            var client = new RestClient(App.BaseUrl+"api/posts/create");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            String json = JsonConvert.SerializeObject(form);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                //TODO: Написать сообщение о неудачном добавлении записи
                return;
            }
            //TODO: Написать сообщение что все успешно
        }
    }
}