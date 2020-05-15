using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using RestSharp;
using ShopsAggregator.Models;
using ShopsAggregator.Services;
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
            try
            {
                String imagePath = String.Empty;
                await CrossMedia.Current.Initialize();
                if (CrossMedia.Current.IsPickPhotoSupported)
                {
                    MediaFile photo = await CrossMedia.Current.PickPhotoAsync();
                    imagePath = photo.Path;
                    PostImage.Source = ImageSource.FromFile(imagePath);
                    GetImageBytesFromPath(imagePath, form);
                    if (!App.IsConnected())
                    {
                        return;
                    }

                    GetPostPhoto.Text = "Изменить фотографию";
                }
            }
            catch (Exception)
            {
                await DisplayAlert("Ошибка", "Что-то пошло не так", "Попробовать снова");
            }
        }

        /// <summary>
        /// Получает байты изображения, выбранного пользователем. 
        /// </summary>
        /// <param name="path">Путь к фотографии.</param>
        /// <param name="form">Форма создания записи.</param>
        private void GetImageBytesFromPath(String path, CreatePostForm form)
        {
            List<Int32> bytes = new List<Int32>();
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                while(fs.Position != fs.Length)
                    bytes.Add(fs.ReadByte());
            }

            form.ImageBytes= bytes.ToList();
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