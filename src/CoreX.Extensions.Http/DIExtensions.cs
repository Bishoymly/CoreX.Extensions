using CoreX.Extensions.Http.HeaderPropagation;
using CoreX.Extensions.Http.HttpClientLogging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DIExtensions
    {
        /// <summary>
        /// A utility extension that enables logging for Bad Request (400) responses as warnings, which by default are not logged. 
        /// This is not specific to the HttpLogger and can work with other logging providers.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection EnableLoggingForBadRequests(this IServiceCollection services)
        {
            services.PostConfigure<ApiBehaviorOptions>(options =>
            {
                var builtInFactory = options.InvalidModelStateResponseFactory;

                options.InvalidModelStateResponseFactory = context =>
                {
                    var loggerFactory = context.HttpContext.RequestServices.GetService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger(context.ActionDescriptor.DisplayName);

                    var builder = new StringBuilder();
                    builder.AppendLine("BadRequest: One or more validation errors occurred.");
                    foreach (var modelState in context.ModelState)
                    {
                        foreach (var error in modelState.Value.Errors)
                        {
                            builder.AppendLine($"{modelState.Key}: {error.ErrorMessage}");
                        }
                    }

                    builder.AppendLine(context.HttpContext.Request.ToStringContent(showHeaders: false, showBody: false).Result);

                    logger.LogWarning(builder.ToString());
                    return builtInFactory(context);
                };
            });

            return services;
        }

        public static IServiceCollection AddHeaderPropagation(this IServiceCollection services, Action<HeaderPropagationOptions> configure)
        {
            services.AddHttpContextAccessor();
            services.Configure(configure);
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IHttpMessageHandlerBuilderFilter, HeaderPropagationMessageHandlerBuilderFilter>());
            return services;
        }

        public static IHttpClientBuilder AddHeaderPropagation(this IHttpClientBuilder builder, Action<HeaderPropagationOptions> configure)
        {
            builder.Services.AddHttpContextAccessor();
            builder.Services.Configure(configure);
            builder.AddHttpMessageHandler((sp) =>
            {
                var options = sp.GetRequiredService<IOptions<HeaderPropagationOptions>>();
                var contextAccessor = sp.GetRequiredService<IHttpContextAccessor>();

                return new HeaderPropagationMessageHandler(options.Value, contextAccessor);
            });

            return builder;
        }

        public static IServiceCollection AddHttpClientLogging(this IServiceCollection services, IConfiguration config = null)
        {
            if (config != null)
            {
                services.Configure<HttpClientLoggingOptions>(config.GetSection("HttpClientLogging"));
            }

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IHttpMessageHandlerBuilderFilter, HttpClientLoggingHandlerBuilderFilter>());
            return services;
        }
    }
}
