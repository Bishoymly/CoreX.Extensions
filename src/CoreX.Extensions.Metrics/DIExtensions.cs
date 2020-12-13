using CoreX.Extensions.Metrics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DIExtensions
    {
        public static IServiceCollection AddMetrics(this IServiceCollection services, IConfiguration config = null)
        {
            services.AddSingleton<IMetricsService, MetricsService>();
            services.AddTransient<MetricsLogger>();
            services.AddSingleton<MetricsMiddleware>();

            return services;
        }

        public static IApplicationBuilder UseMetrics(this IApplicationBuilder app)
        {
            var loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(new MetricsLoggerProvider(app.ApplicationServices));

            app.UseMiddleware<MetricsMiddleware>();

            return app;
        }
    }
}
