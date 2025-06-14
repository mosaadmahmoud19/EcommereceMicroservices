using eCommerece.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerece.SharedLibrary.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<TContext>
            (this IServiceCollection services, IConfiguration config, string fileName) where TContext:DbContext
        {
            // add generic database context

            services.AddDbContext<TContext>(options => options.UseSqlServer(
                config.GetConnectionString("eCommerceConnection"), sqlServerOption =>
                sqlServerOption.EnableRetryOnFailure())

                );

            // configure serilog logging

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{fileName}-.text",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}[{Level:u3}]{message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // add jwt Authentication Scheme

            JWTAuthenticationScheme.AddJWTAuthentication(services , config);
            return services;
                
                
        }
        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            // use global exception

            app.UseMiddleware<GlobalException>();

            // Register middleware to block all outsiders API calls

           // app.UseMiddleware<ListenToOnlyApiGateway>();

            return app;
        }

    }

}
