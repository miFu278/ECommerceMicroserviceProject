using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;

namespace ProductApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(
            this IServiceCollection services, IConfiguration configuration)
        {
            // add database connection
            // add authentication scheme
            SharedServiceContainer.AddSharedServices<ProductDbContext>(
                services, configuration, configuration["MySerilog:FileName"]!);

            // create dependency injection
            services.AddScoped<IProduct, ProductRepository>();

            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            // register middleware such as:
            // global exception: handles external errors.
            // listen to only api gateway: blocks all outsider calls.
            SharedServiceContainer.UseSharedPolicies(app);
            return app;
        }

    }
}