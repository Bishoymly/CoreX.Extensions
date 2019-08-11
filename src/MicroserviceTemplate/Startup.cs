using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace MicroserviceTemplate
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MicroserviceTemplate",
                    Version = "v1",
                    Description = "<p>This template provides loads of developer friendly features that makes dotnet core ready for microservices and containers scenarios.</p>" +
                    "<ul>" +
                    "<li><a target='_blank' href='/log?level=all'>/log</a> monitor your beautiful logs from the comfort of your browser </li>" +
                    "<li><a target='_blank' href='/health'>/health</a> monitor your application health status </li>" +
                    "</ul>"
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            // Enable the logging for common 400 bad request errors, for easier traceability
            services.EnableLoggingForBadRequests();

            // Register HttpClientFactory
            services.AddHttpClient();

            // Register HttpLog middleware for "/log"
            services.AddHttpLog(Configuration);

            // Register Health checks
            services.AddHealthChecks();

            // Configure ForwardedHeaders
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Enables ForwardedHeaders to enable hosting behind reverse proxies like NGINX or inside Kubernetes
            app.UseForwardedHeaders();

            // Enable HttpLog middleware for "/log"
            app.UseHttpLog();

            // Enable middleware for "/health"
            app.UseHealthChecks("/health");

            // Enable developer exception page for debugging
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MicroserviceTemplate V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseMvc();
        }
    }
}
