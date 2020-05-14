using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShopsAggregator
{
    /// <summary>
    /// Расширение функционала Image view.
    /// </summary>
    [ContentProperty("Source")]
    public class ImageResourceExtension : IMarkupExtension
    {
        /// <summary>
        /// Путь к файлу с изображением.
        /// </summary>
        public String Source { get; set; }

        /// <summary>
        /// Получает значени из view и возращает изображение.
        /// </summary>
        /// <param name="serviceProvider">Механизм извлечения данных.</param>
        /// <returns>Расположение изображения.</returns>
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
}