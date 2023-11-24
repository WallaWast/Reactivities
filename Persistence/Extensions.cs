using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Persistence
{
    public static class Extensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
            });

            return services;
        }

        public static async void MigrateDb(this IServiceProvider services)
        {
            try
            {
                var context = services.GetRequiredService<DataContext>();
                await context.Database.MigrateAsync();
                await Seed.SeedData(context);
            }
            catch (System.Exception ex)
            {
                var logger = services.GetRequiredService<ILogger>();
                logger.LogError(ex, "An error ocurred during migration");
            }
        }

        // public static IServiceCollection AddRepository<T>(this IServiceCollection services, string collectionName) where T : IEntity
        // {
        //     services.AddSingleton<IRepository<T>>(serviceProvider =>
        //                {
        //                    var database = serviceProvider.GetService<IMongoDatabase>();
        //                    return new MongoRepository<T>(database, collectionName);
        //                });

        //     return services;
        // }
    }
}