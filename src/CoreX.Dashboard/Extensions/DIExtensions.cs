using CoreX.Dashboard;
using CoreX.Dashboard.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Reflection;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DIExtensions
    {
        public static IServiceCollection AddDeveloperDashboard(this IServiceCollection services, IConfiguration config = null)
        {
            var assembly = typeof(DeveloperDashboardController).GetTypeInfo().Assembly;
            // This creates an AssemblyPart, but does not create any related parts for items such as views.
            var part = new AssemblyPart(assembly);

            services.AddControllersWithViews()
                .ConfigureApplicationPartManager(apm => apm.ApplicationParts.Add(part));

            //services.Add(new ServiceDescriptor(typeof(DeveloperDashboardMiddleware), typeof(DeveloperDashboardMiddleware), ServiceLifetime.Singleton));
            return services;
        }

        public static IApplicationBuilder UseDeveloperDashboard(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            var assembly = typeof(DeveloperDashboardController).GetTypeInfo().Assembly;
            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = "/devdashcontent",
                FileProvider = new EmbeddedFileProvider(assembly, "CoreX.Dashboard.wwwroot")
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=DeveloperDashboard}/{action=Index}/{id?}");
            });

            //app.UseMiddleware<DeveloperDashboardMiddleware>();
            return app;
        }
    }
}
