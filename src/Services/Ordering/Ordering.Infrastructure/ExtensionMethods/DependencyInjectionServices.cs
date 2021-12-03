using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Infrastructure.Mails;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Repository;

namespace Ordering.Infrastructure.ExtensionMethods
{
    public static class DependencyInjectionServices
    {
        public static IServiceCollection AddInfrastructureInjections(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
            services.Configure<EmailSettings>(c => configuration.GetSection("EmailSettings"));
            services.AddTransient<IEmailService, EmailService>();
            services.AddDbContext<OrderContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("OrderingConnectionString"));
            });
            return services;
        }
    }
}
