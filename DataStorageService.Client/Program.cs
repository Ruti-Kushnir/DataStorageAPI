using Microsoft.Extensions.Configuration;

namespace DataStorageService.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Waiting for API to start...");
            await Task.Delay(2000);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) 
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string baseUrl = configuration["ApiSettings:BaseUrl"];

            if (string.IsNullOrEmpty(baseUrl))
            {
                Console.WriteLine("Error: BaseUrl not found in appsettings.json");
                return;
            }

            Console.WriteLine($"Connecting to API at: {baseUrl}");

            var client = new DataApiClient("https://localhost:7003/");
            await client.RunTestSequence();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
