using System.IO;
using System.Threading.Tasks;

namespace ShopsAggregator.Services
{
    /// <summary>
    /// Интерфейся получения фотографии с устройства.
    /// </summary>
    public interface IPhotoPickerService
    {
        /// <summary>
        /// Ассинхроное получение потока изображения.
        /// </summary>
        /// <returns>Поток изображения</returns>
        Task<Stream> GetImageStreamAsync();
    }
}