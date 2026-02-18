using DataStorageService.Interfaces;

namespace DataStorageService.Models
{
    public class DataRecord: IEntity
    {
        public Guid Id { get; set; }    
        public string Content { get; set; } 
        public DateTime CreatedAt { get; set; } 
    }
}
