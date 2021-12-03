using System;
using System.Threading;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ordering.API.HostExtensions
{
    public static class HostExtensionMethods
    {
        public static IHost MigrateDatabase<TContext>(this IHost host,
            Action<TContext, IServiceProvider> seeder, int? retry = 0) where TContext : DbContext
        {
            var retryForAvailability = retry ?? 0;
            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<TContext>>();
            var context = services.GetRequiredService<TContext>();

            try
            {
                logger.LogInformation($"Migration associated with {nameof(context)} initiated");
                InvokeSeeder(seeder, context, services);
                logger.LogInformation($"migrated the database associated with {nameof(context)}");
            }
            catch (SqlException e)
            {
                logger.LogError($"an error {e} occurred while performing migrations for {nameof(context)}");
                if (retryForAvailability++ < 50)
                {
                    Thread.Sleep(2000);
                    MigrateDatabase(host, seeder, retryForAvailability);
                }
            }

            return host;
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, 
            TContext context, IServiceProvider services) where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context, services);
        }
    }
}
