
using DataStorageService.Data;
using DataStorageService.Decorators;
using DataStorageService.Infrastructure;
using DataStorageService.Interfaces;
using DataStorageService.Models;
using DataStorageService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace DataStorageService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddSingleton<SdcsCache<DataRecord>>(provider =>
            {
                return new SdcsCache<DataRecord>(10);
            });

            var redisConnectionString = builder.Configuration.GetSection("RedisSettings:ConnectionString").Value;
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "DataService_";
            });

            builder.Services.AddScoped(typeof(IDataRepository<>), typeof(DataRepository<>));

            builder.Services.AddScoped<IDataService<DataRecord>>(provider =>
            {
                var repository = provider.GetRequiredService<IDataRepository<DataRecord>>();
                var coreService = new DataService<DataRecord>(repository);

                var sdcsCache = provider.GetRequiredService<SdcsCache<DataRecord>>();
                var sdcsLayer = new SdcsCachingDecorator<DataRecord>(coreService, sdcsCache);

                var redisCache = provider.GetRequiredService<IDistributedCache>();
                return new RedisCachingDecorator<DataRecord>(sdcsLayer, redisCache);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
