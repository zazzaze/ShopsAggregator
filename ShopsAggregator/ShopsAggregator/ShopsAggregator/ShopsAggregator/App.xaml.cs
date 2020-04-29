using System;
using System.Globalization;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using ShopsAggregator.Services;
using ShopsAggregator.Views;
using ShopsAggregatorLib;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;


[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
[assembly: ExportFont("SchlangeBold.ttf", Alias = "SchlangeBold")]
namespace ShopsAggregator
{
    public partial class App : Application
    {
        public static String ServerUrl = "http://62.113.116.228/api/users/";
        public static String BaseUrl = "http://62.113.116.228/";
        public static User mainUser;
        public App()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentCulture;
            InitializeComponent();
            
            var navPage = new NavigationPage(new Authentification());
            MainPage = navPage;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
        
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

        public static Boolean IsConnected()
        {
            var current = Connectivity.NetworkAccess;
            return current != NetworkAccess.None;
        }
        
        public static User DeserializeUser(String json)
        {
            json = json.Remove(json.Length - 1, 1).Replace("\\", "")
                .Remove(0, 1);

                return JsonConvert.DeserializeObject<User>(json);
        }
    }
}