using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shopping.Aggregator.ExtensionClasses;
using Shopping.Aggregator.Services.Implementations;
using Shopping.Aggregator.Services.Interfaces;

namespace Shopping.Aggregator.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddHttpClientCustom(this IServiceCollection services, IConfiguration config)
        {
            
            services.AddHttpClient<ICatalogService, CatalogService>(o =>
            {
                o.BaseAddress = new Uri(config["ApiSettings:CatalogUrl"]);
            });
            services.AddHttpClient<IBasketService, BasketService>( o =>
            { 
                o.BaseAddress = new Uri(config["ApiSettings:BasketUrl"]);
            });
            services.AddHttpClient<IOrderService, OrderService>(o =>
            {
                o.BaseAddress = new Uri(config["ApiSettings:OrdersUrl"]);
            });

            return services;
        }
    }
}
