using DataStorageService.Interfaces;
using DataStorageService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DataStorageService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly IDataService<DataRecord> _dataService;
        public DataController(IDataService<DataRecord> dataService) { 
            _dataService = dataService;
        }

        /// <summary>
        /// Saves data and returns a unique identifier.
        /// </summary>
        /// <param name="content">The string content to save.</param>
        /// <returns>The generated Guid Id.</returns>
        [HttpPost]
        public async Task<IActionResult> SaveData([FromBody] string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest("Content cannot be empty.");
            }

            var id = await _dataService.SaveDataAsync(content);
            return Ok(new { id });
        }

        /// <summary>
        /// Retrieves data by its unique identifier.
        /// Checks Redis, then SDCS, then DB
        /// </summary>
        /// <param name="id">The unique identifier of the data.</param>
        /// <returns>The data record if found</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetData(Guid id)
        {
            var data = await _dataService.GetDataAsync(id);
            
            if (data == null)
            {
                return NotFound($"Data with ID {id} was not found.");
            }

            return Ok(data);
        }
    }
}
