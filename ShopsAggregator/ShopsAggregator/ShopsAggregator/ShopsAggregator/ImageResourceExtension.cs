using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShopsAggregator
{
    [ContentProperty("Source")]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return null;
            }
            var imageSource = ImageSource.FromResource(Source);
 
            return imageSource;
        }
    }
    
    [ContentProperty("Data")]
    public class ImageDataExtension : IMarkupExtension
    {
        public Int32 Data { get; set; }
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return Data;
        }
    }
}