using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopsAggregator.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShopsAggregator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PostPage : ContentView
    {
        public PostPage(Post post)
        {
            InitializeComponent();
            StackLayout.HeightRequest = App.Current.MainPage.Height;
            StackLayout.WidthRequest = App.Current.MainPage.Width;
            PostImage.Source = App.BaseUrl + "Egoricon.jpeg";
            IconsLayout.BackgroundColor = Color.FromRgba(149, 165, 166, 0.4);
        }
    }
}