using CoreX.Extensions.Metrics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DIExtensions
    {
        public static IServiceCollection AddMetrics(this IServiceCollection services, IConfiguration config = null)
        {
            services.AddSingleton<IMetricsService, MetricsService>();
            services.AddSingleton<MetricsMiddleware>();

            return services;
        }

        public static IApplicationBuilder UseMetrics(this IApplicationBuilder app)
        {
            app.UseMiddleware<MetricsMiddleware>();

            return app;
        }
    }
}
