﻿using eCommerce.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace eCommerce.SharedLibrary.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<TContext>(this IServiceCollection services,
            IConfiguration configuration, string fileName) where TContext : DbContext
        {
            //Add Generic DB context

            services.AddDbContext<TContext>(option => option.UseSqlServer(
                configuration
                    .GetConnectionString("eCommerceConnection"), sqlserverOption =>
                    sqlserverOption.EnableRetryOnFailure()));

            //Config seri log
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{fileName}-.text",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{Timestamp:yyyy-MM--dd HH:mm:ss:fff zzz} [{Level:u3}] {message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day)
                .CreateLogger();

            //Add JWT authentication scheme
            JWTAuthenticationScheme.AddJWTAuthenticationScheme(services, configuration);
            return services;
        }

        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            //Use global ex
            app.UseMiddleware<GlobalException>();

            //Register middelwares to block outsiders API calls
            //app.UseMiddleware<ListenToApiGateway>();
            return app;
        }
    }
}
