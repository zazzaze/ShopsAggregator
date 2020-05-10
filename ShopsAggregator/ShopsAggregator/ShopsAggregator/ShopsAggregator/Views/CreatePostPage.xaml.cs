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
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShopsAggregator.Views
{
    /// <summary>
    /// Код страницы создания записи.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreatePostPage : ContentPage
    {
        /// <summary>
        /// Экземпляр типа пользователя-покупателя.
        /// </summary>
        private Seller seller;
        /// <summary>
        /// Формы создания записи.
        /// </summary>
        private CreatePostForm form;
        /// <summary>
        /// Конструктор страницы.
        /// </summary>
        /// <param name="seller">Экземпляр типа пользователя-покупателя.</param>
        public CreatePostPage(Seller seller)
        {
            InitializeComponent();
            this.seller = seller;
            form = new CreatePostForm(seller.Id);
            this.BindingContext = form;
        }

        /// <summary>
        /// Позволяет пользователю выбрать фотографию после нажатия на кнопку.
        /// </summary>
        /// <param name="sender">Издатель события - Button.</param>
        /// <param name="e">Аргументы события.</param>
        private async void OnGetPostPhotoClicked(object sender, EventArgs e)
        {
            Stream stream = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
            if (stream != null)
            {
                await DisplayAlert("Подождите", "Фотография обрабатывается", "Жду");
                GetImageBytesFromStream(stream);
                PostImage.Source = ImageSource.FromStream(() => stream);
                GetPostPhoto.Text = "Изменить фотографию";
            }
        }

        /// <summary>
        /// Получает байты изображения, выбранного пользователем. 
        /// </summary>
        /// <param name="stream">Поток выбранной фотографии.</param>
        private void GetImageBytesFromStream(Stream stream)
        {
            form.ImageBytes = new List<Int32>();
            while (stream.Position != stream.Length)
            {
                form.ImageBytes.Add(stream.ReadByte());
            }

            stream.Seek(0, SeekOrigin.Begin);
        }
        
        /// <summary>
        /// Обработчик события нажатия на кнопку создания записи.
        /// </summary>
        /// <param name="sender">Издатель события - Button.</param>
        /// <param name="e">Аргументы события.</param>
        private async void CreatePostButtonClicked(object sender, EventArgs e)
        {
            if (form.ImageBytes.Count == 0)
            {
                await DisplayAlert("Ошибка", "Выберите фотографию", "Выбрать");
                return;
            }

            if (!App.IsConnected())
            {
                await DisplayAlert("Нет подключения к интернету", "Проверьте подключение к интернету", "Попробовать снова");
                return;
            }
            
            if (form.CreatorId == 0)
            {
                await DisplayAlert("Ошибка в данных пользователя", "Зайдите снова, чтобы ее устранить", "Ок");
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
                await DisplayAlert("Ошибка", "Не удалось добавить запись", "Попробовать снова");
                return;
            }

            await DisplayAlert("Запись успешно добавлена", "", "Хорошо");
        }
    }
}