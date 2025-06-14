﻿using eCommerece.SharedLibrary.Logs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Services;
using Polly;
using Polly.Retry;


namespace OrderApi.Application.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services , IConfiguration config) 
        {
            // register httpclient service
            // create dependency injection

            services.AddHttpClient<IOrderService , OrderService>(options => {

                options.BaseAddress = new Uri(config["ApiGateway:BaseAddress"]!);
                options.Timeout = TimeSpan.FromSeconds(1);


            });

            // create retry strategy

            var retryStrategy = new RetryStrategyOptions()
            {
                ShouldHandle = new PredicateBuilder().Handle<TaskCanceledException>(),
                BackoffType = DelayBackoffType.Constant,
                UseJitter = true,
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(500),
                OnRetry = args=> {

                    string message = $"OnRetry, Attrmpt: {args.AttemptNumber} Outcome {args.Outcome}";
                    LogException.LogToConsole(message);
                    LogException.LogToDebugger(message);
                    return ValueTask.CompletedTask;
                }
            };

            services.AddResiliencePipeline("my-retry-pipeline", builder => 
            {
                builder.AddRetry(retryStrategy);
            });

            return services;
        }
    }
}
