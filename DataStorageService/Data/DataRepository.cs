using DataStorageService.Interfaces;
using DataStorageService.Models;
using Microsoft.EntityFrameworkCore;

namespace DataStorageService.Data
{
    public class DataRepository<T>:IDataRepository<T> where T : class, IEntity
    {
        private readonly AppDbContext _context;
        public DataRepository(AppDbContext context)
        {
            _context = context;
        } 

        public async Task<Guid> AddAsync(T record)
        {
            if(record.Id == Guid.Empty)
            {
                record.Id = Guid.NewGuid();
            }

            if (record is DataRecord dataRecord)
            {
                dataRecord.CreatedAt = DateTime.UtcNow;
            }

            _context.Set<T>().Add(record);
            await _context.SaveChangesAsync();
            return record.Id;
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            Console.WriteLine($"[DATABASE] Fetching {typeof(T).Name} from DB. ID: {id}");
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
