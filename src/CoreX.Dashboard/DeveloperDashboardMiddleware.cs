using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CoreX.Dashboard
{
    public class DeveloperDashboardMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path == "/" && context.Request.Method == "GET")
            {
                await context.WriteResultAsync(new ViewResult { ViewName = "Dashboard" });
            }
            else
            {
                if (context.Request.Path.StartsWithSegments(new PathString("/icon")) && context.Request.Method == "GET")
                {
                    await context.WriteResultAsync(new PhysicalFileResult("/wwwroot/icon/manifest.json", "application/json"));
                }
                else
                {
                    await next(context);
                }
            }
        }
    }
}
