using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using DataStorageService.Models;

namespace DataStorageService.Client
{
    public class DataApiClient
    {
        private readonly HttpClient _httpClient;

        public DataApiClient(string baseUrl)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        }

        public async Task RunTestSequence()
        {
            Console.WriteLine("--- Starting API Test Sequence ---");

            // 1. קריאת POST
            string contentToSend = "Hello from automated test!";
            Console.WriteLine($"[POST] Sending: {contentToSend}");

            var postResponse = await _httpClient.PostAsJsonAsync("api/data", contentToSend);
            postResponse.EnsureSuccessStatusCode();

            var postResult = await postResponse.Content.ReadFromJsonAsync<PostResponse>();
            Guid newId = postResult!.Id;
            Console.WriteLine($"[POST] Success! Received ID: {newId}");

            Console.WriteLine("----------------------------------");

            // 2. קריאת GET
            Console.WriteLine($"[GET] Fetching data for ID: {newId}");
            var getResponse = await _httpClient.GetAsync($"api/data/{newId}");

            if (getResponse.IsSuccessStatusCode)
            {
                var record = await getResponse.Content.ReadFromJsonAsync<DataRecord>();
                Console.WriteLine($"[GET] Success! Content: {record?.Content}");
                Console.WriteLine($"[GET] Created At: {record?.CreatedAt}");
            }
        }

        private class PostResponse { public Guid Id { get; set; } }
    }
}
