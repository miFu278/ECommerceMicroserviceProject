using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Interfaces;
using OrderApi.Infrastructure.Data;
using OrderApi.Infrastructure.Repositories;

namespace OrderApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(
            this IServiceCollection services, IConfiguration configuration)
        {
            // add database connectivity
            // add authentication scheme
            SharedServiceContainer.AddSharedServices<OrderDbContext>(
                services, configuration, configuration["MySerilog:FileName"]!);

            // create dependency injection
            services.AddScoped<IOrder, OrderRepository>();

            return services;
        }

        public static IApplicationBuilder UserInfrastructurePolicy(this IApplicationBuilder app)
        {
            // register middleware such as:
            // global exception -> handle external errors
            // listen to apigateway only -> block all outsiders calls

            SharedServiceContainer.UseSharedPolicies(app);

            return app;

        }
    }
}
