using System;
using System.Threading.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using IntCopilot.Sniffer.StudentId.Configuration;
using IntCopilot.Sniffer.StudentId.Core;
using IntCopilot.Sniffer.StudentId.Infrastructure;
using IntCopilot.Sniffer.StudentId.Worker;

namespace IntCopilot.Sniffer.StudentId.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStudentSniffer(
            this IServiceCollection services,
            Action<SnifferConfiguration> configureOptions)
        {
            services.AddOptions<SnifferConfiguration>()
                .Configure(configureOptions)
                .ValidateDataAnnotations();

            // *** 已修复 ***
            // 明确告诉DI容器：当有服务请求 RateLimiter 时，
            // 就执行这个工厂方法，创建一个 TokenBucketRateLimiter 实例并返回。
            services.AddSingleton<RateLimiter>(sp => 
            {
                var config = sp.GetRequiredService<IOptions<SnifferConfiguration>>().Value;
                return new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 10,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = int.MaxValue,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                    TokensPerPeriod = config.RateLimitPerSecond,
                    AutoReplenishment = true
                });
            });

            // 注入真实的API客户端
            services.AddSingleton<IApiClient, LiveApiClient>();
            
            // 注入核心嗅探器服务
            services.AddSingleton<IStudentIdSniffer, StudentIdSniffer>();
            
            // 将我们的嗅探器Worker注册为后台服务
            services.AddHostedService<StudentIdSnifferWorker>();

            return services;
        }
    }
}