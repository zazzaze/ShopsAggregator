using System;
using System.Globalization;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using ShopsAggregator.Models;
using ShopsAggregator.Services;
using ShopsAggregator.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;


[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
[assembly: ExportFont("SchlangeBold.ttf", Alias = "SchlangeBold")]
namespace ShopsAggregator
{
    /// <summary>
    /// Основной код приложения.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Ссылка сервера на контроллер users.
        /// </summary>
        public static String ServerUrl = "http://62.113.116.228/api/users/";
        /// <summary>
        /// Базовый адрес сервера.
        /// </summary>
        public static String BaseUrl = "http://62.113.116.228/";
        /// <summary>
        /// Конструктор. Устанавливает язык приложения и интерфейса.
        /// </summary>
        public App()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentCulture;
            InitializeComponent();
            
            var navPage = new NavigationPage(new Authentification());
            MainPage = navPage;
        }

        /// <summary>
        /// Анимирует встряхивание элемента.
        /// </summary>
        /// <param name="element">View элемент.</param>
        public static async void Shake(View element)
        {
            UInt32 timer = 50;
            await element.TranslateTo(-15, 0, timer);
            await element.TranslateTo(15, 0, timer);
            await element.TranslateTo(-10, 0, timer);
            await element.TranslateTo(10, 0, timer);
            await element.TranslateTo(-5, 0, timer);
            await element.TranslateTo(5, 0, timer);
            element.TranslationX = 0;
        }

        /// <summary>
        /// Проверяет, есть ли у приложения подключение к интернету.
        /// </summary>
        /// <returns>Результат проверки.</returns>
        public static Boolean IsConnected()
        {
            var current = Connectivity.NetworkAccess;
            return current != NetworkAccess.None;
        }
    }
}