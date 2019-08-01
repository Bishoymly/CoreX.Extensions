using CoreX.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Extensions
    {
        public static IServiceCollection AddHttpLog(this IServiceCollection services)
        {
            services.Add(new ServiceDescriptor(typeof(LogMiddleware), typeof(LogMiddleware), ServiceLifetime.Singleton));
            return services;
        }

        public static IApplicationBuilder UseHttpLog(this IApplicationBuilder app)
        {
            var loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();
            var logMiddleware = app.ApplicationServices.GetService<LogMiddleware>();

            app.UseMiddleware<LogMiddleware>();
            loggerFactory.AddProvider(new HttpLoggerProvider(logMiddleware));
            return app;
        }
    }
}
