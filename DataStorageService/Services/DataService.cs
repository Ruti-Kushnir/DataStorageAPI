using DataStorageService.Interfaces;
using DataStorageService.Models;

namespace DataStorageService.Services
{
    public class DataService<T>:IDataService<T> where T : class, IEntity, new()
    {
        private readonly IDataRepository<T> _repository;
        public DataService(IDataRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<Guid> SaveDataAsync(string content)
        {
            var entity = new T();

            if(entity is DataRecord record)
            {
                record.Content = content;
            }

            return await _repository.AddAsync(entity);
        }

        public async Task<T?>GetDataAsync(Guid id)
        {
            var result = await _repository.GetByIdAsync(id);
            return result as T;
        }
    }
}
