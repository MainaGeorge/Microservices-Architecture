using Basket.API.GRPCService;
using Basket.API.Repository;
using Discount.GRPC.Protos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Basket.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<DiscountGrpcService>();


            services.AddStackExchangeRedisCache(opt =>
                {
                    opt.Configuration = Configuration["CacheSettings:ConnectionString"];
                });
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Basket.API", Version = "v1" });
                });

            services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(o =>
            {
                o.Address = new System.Uri(Configuration["GrpcServiceSetting:DiscountUri"]);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket.API v1"));
            }


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
