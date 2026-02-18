using DataStorageService.Models;
using Microsoft.EntityFrameworkCore;

namespace DataStorageService.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<DataRecord> DataRecords { get; set; }  
    }
}
