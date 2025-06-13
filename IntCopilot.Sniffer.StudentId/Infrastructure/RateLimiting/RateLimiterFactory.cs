using System;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Options;
using IntCopilot.Sniffer.StudentId.Configuration;

namespace IntCopilot.Sniffer.StudentId.Infrastructure.RateLimiting
{
    public static class RateLimiterFactory
    {
        public static RateLimiter CreateTokenBucketRateLimiter(IOptions<SnifferConfiguration> options)
        {
            var config = options.Value;
            
            return new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
            {
                TokenLimit = 1,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 1,
                ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                TokensPerPeriod = config.RateLimitPerSecond,
                AutoReplenishment = true
            });
        }
    }
}