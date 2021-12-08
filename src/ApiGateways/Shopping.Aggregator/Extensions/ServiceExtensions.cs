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
            var apiSettings = config.GetValue<ApiSettings>("ApiSettings");
            services.AddHttpClient<ICatalogService, CatalogService>(o =>
            {
                o.BaseAddress = new Uri(apiSettings.CatalogUrl);
            });
            services.AddHttpClient<IBasketService, BasketService>( o =>
            { 
                o.BaseAddress = new Uri(apiSettings.BasketUrl);
            });
            services.AddHttpClient<IOrderService, OrderService>(o =>
            {
                o.BaseAddress = new Uri(apiSettings.OrdersUrl);
            });

            return services;
        }
    }
}
