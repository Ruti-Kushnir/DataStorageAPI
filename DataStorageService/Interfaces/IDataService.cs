using DataStorageService.Models;

namespace DataStorageService.Interfaces
{
    public interface IDataService<T>
    {
        Task<Guid> SaveDataAsync(string content);
        Task<T?> GetDataAsync(Guid id);
    }
}
