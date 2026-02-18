using System.Text.Json;
using DataStorageService.Infrastructure;
using DataStorageService.Interfaces;
using DataStorageService.Models;

namespace DataStorageService.Decorators
{
    /// <summary>
    /// Decorator that adds SDCS (Self-Designed Caching System) capabilities to the data service.
    /// Implements the LRU caching logic before falling back to the database.
    /// </summary>
    public class SdcsCachingDecorator<T>:IDataService<T>
    {
        private readonly IDataService<T> _innerService;
        private readonly SdcsCache<T> _cache;

        public SdcsCachingDecorator(IDataService<T> innerService, SdcsCache<T> cache)
        {
            _innerService = innerService;
            _cache = cache;
        }

        /// <summary>
        /// Gets data by checking the SDCS first. If not found, retrieves from the inner service (DB).
        /// </summary>
        public async Task<T?> GetDataAsync(Guid id)
        {
            string key = id.ToString();

            var cachedData = _cache.Get(key);
            if (cachedData != null) 
            {
                Console.WriteLine($"[SDCS HIT] Item found in Local In-Memory Cache! ID: {id} Data: {JsonSerializer.Serialize(cachedData)}");
                return cachedData;
            }

            Console.WriteLine($"[SDCS MISS] Iten not in SDCS. Checking next layer... ID: {id}");
            var data = await _innerService.GetDataAsync(id);
            if (data != null) 
            {
                _cache.Put(key, data);
            }

            return data;    
        }

        /// <summary>
        /// Saves data. POSTed data is saved to the DB only.
        /// </summary>
        public async Task<Guid> SaveDataAsync(string content)
        {
            return await _innerService.SaveDataAsync(content);
        }
    }
}
