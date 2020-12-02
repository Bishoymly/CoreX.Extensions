using CoreX.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DIExtensions
    {
        public static IServiceCollection AddHttpLog(this IServiceCollection services, IConfiguration config = null)
        {
            if (config != null)
            {
                services.Configure<HttpLoggerOptions>(config.GetSection("HttpLogger"));
            }

            services.Add(new ServiceDescriptor(typeof(LogMiddleware), typeof(LogMiddleware), ServiceLifetime.Singleton));
            return services;
        }

        public static IApplicationBuilder UseHttpLog(this IApplicationBuilder app)
        {
            var loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();
            var contextAccessor = app.ApplicationServices.GetService<IHttpContextAccessor>();
            var logMiddleware = app.ApplicationServices.GetService<LogMiddleware>();

            app.UseMiddleware<LogMiddleware>();
            loggerFactory.AddProvider(new HttpLoggerProvider(logMiddleware, contextAccessor));
            return app;
        }
    }
}
