using eCommerece.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.infrastructure.Data;
using ProductApi.infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {

        //IServiceCollection => represents a collection of service registrations used for Dependency Injection (DI)
        // IConfiguration => read configuration settings from various sources
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            // add database connect

            services.AddSharedServices<ProductDbContext>(config, config["MySerilog:FineName"]!);

            services.AddScoped<IProduct, ProductRepository>();

            return services;

        }

        //IApplicationBuilder=>  that is used to configure
        //the request processing pipeline (middleware) in an application
        // It allows you to add middleware components like authentication, logging, CORS, routing, and exception handling.
        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            app.UseSharedPolicies();

            return app;
        }
    }
}
