using System.Text.Json;
using DataStorageService.Interfaces;
using DataStorageService.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace DataStorageService.Decorators
{
    /// <summary>
    /// The outermost decorator that handles Redis caching with a 5-minute TTL.
    /// This is the first layer of the data retrieval chain.
    /// </summary>
    public class RedisCachingDecorator<T>:IDataService<T>
    {
        private readonly IDataService<T> _innerService;
        private readonly IDistributedCache _redisCache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

        public RedisCachingDecorator(IDataService<T> innerService, IDistributedCache redisCache)
        {
            _innerService = innerService;   
            _redisCache = redisCache;
        }

        /// <summary>
        /// Retrieves data from Redis. If not found, delegates to SDCS/DB layer 
        /// and stores the result back in Redis.
        /// </summary>
        public async Task<T?> GetDataAsync(Guid id)
        {
            string key = $"{typeof(T).Name}:{id}";
            
            var cachedJson = await _redisCache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(cachedJson))
            {
                Console.WriteLine($"[REDIS HIT] Item found in Redis Cloud! ID: {id} Data: {cachedJson}");
                return JsonSerializer.Deserialize<T>(cachedJson);
            }

            Console.WriteLine($"[REDIS MISS] Iten not in Redis. Checking next layer... ID: {id}");
            var data = await _innerService.GetDataAsync(id);
            if(data != null)
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _cacheDuration
                };
                string jsonContent = JsonSerializer.Serialize(data);
                await _redisCache.SetStringAsync(key, jsonContent, options);
            }

            return data;
        }

        /// <summary>
        /// Saves data. POSTed data is only saved to the DB.
        /// </summary>
        public async Task<Guid> SaveDataAsync(string content)
        {
            return await _innerService.SaveDataAsync(content);
        }
    }
}
