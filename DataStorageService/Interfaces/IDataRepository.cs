using DataStorageService.Models;

namespace DataStorageService.Interfaces
{
    public interface IDataRepository<T> where T : class, IEntity
    {
        Task<Guid> AddAsync(T record);
        Task<T?> GetByIdAsync(Guid id);
    }
}
