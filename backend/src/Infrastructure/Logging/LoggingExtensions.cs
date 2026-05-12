using Microsoft.Extensions.Logging;

namespace Medicare.Infrastructure.Logging
{
    /// <summary>
    /// Logging configuration helper for setting up structured logging.
    /// </summary>
    public static class LoggingExtensions
    {
        public static ILoggingBuilder AddApplicationLogging(this ILoggingBuilder builder)
        {
            builder.ClearProviders();
            builder.AddConsole();
            builder.AddDebug();
            
            // Configure log levels by category
            builder.AddFilter("Microsoft", LogLevel.Warning)
                   .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Information)
                   .AddFilter("Medicare", LogLevel.Debug);

            return builder;
        }
    }
}
