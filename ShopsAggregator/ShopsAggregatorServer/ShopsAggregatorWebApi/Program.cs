using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ShopsAggregatorWebApi
{
    /// <summary>
    /// Класс с Main.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main метод.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Создает хост.
        /// </summary>
        /// <param name="args">Аргументы.</param>
        /// <returns>Созданый хост.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}