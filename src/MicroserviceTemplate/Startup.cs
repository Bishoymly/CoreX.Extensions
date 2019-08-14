using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CoreX.Extensions.Metrics;
using CorrelationId;
using HealthChecks.UI.Client;
using MicroserviceTemplate.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
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
        public OpenApiInfo OpenApiInfo { get; set; } = new OpenApiInfo
        {
            Title = "MicroserviceTemplate",
            Version = "v1"
        };

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Register Feature Management
            services.AddFeatureManagement();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", OpenApiInfo);

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.EnableAnnotations();
            });

            // Enable the logging for common 400 bad request errors, for easier traceability
            services.EnableLoggingForBadRequests();

            // Register CorrelationId middleware
            services.AddCorrelationId();

            // Register HttpClientFactory
            services.AddHttpClient();

            // Register global header propagation for any HttpClient that comes from HttpClientFactory
            // default headers are already added like x-correlation-id
            services.AddHeaderPropagation(options => { });

            // Register Logging for HttpClient
            services.AddHttpClientLogging(Configuration);

            // Register HttpLog middleware for "/log"
            services.AddHttpLog(Configuration);

            // Register Health checks
            services.AddHealthChecks()
                .AddSqlServer(Configuration["ConnectionStrings:DefaultConnection"], name: "DefaultConnection");

            if (Configuration["FeatureManagement:HealthUI"] == "True")
            {
                // Register HealthChecks UI
                services.AddHealthChecksUI();
            }

            // Register Application Insights with W3C Distributed Tracing
            services.AddApplicationInsightsTelemetry(o =>
                o.RequestCollectionOptions.EnableW3CDistributedTracing = true);

            // Register the DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    Configuration["ConnectionStrings:DefaultConnection"]);
            });

            // Configure ForwardedHeaders
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IFeatureManager featureManager)
        {
            app.UseCorrelationId(new CorrelationIdOptions
            {
                Header = "X-Correlation-ID",
                UseGuidForCorrelationId = true,
                UpdateTraceIdentifier = false
            });

            if (featureManager.IsEnabled(Features.ForwardedHeaders))
            {
                // Enables ForwardedHeaders to enable hosting behind reverse proxies like NGINX or inside Kubernetes
                app.UseForwardedHeaders();
            }

            if (featureManager.IsEnabled(Features.HttpLogger))
            {
                // Enable HttpLog middleware for "/log"
                app.UseHttpLog();
            }

            if (featureManager.IsEnabled(Features.Healthz))
            {
                // Enable middleware for "/healthz"
                app.UseHealthChecks("/healthz", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            }

            if (featureManager.IsEnabled(Features.HealthUI))
            {
                app.UseHealthChecksUI();
            }

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

            if (featureManager.IsEnabled(Features.Swagger))
            {
                // Set Swagger description to match enabled features
                this.OpenApiInfo.Description = GetSwaggerHomepage(featureManager);

                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MicroserviceTemplate V1");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseMvc();
        }

        // This method is used to generate the description in the swagger UI page, according to the available features
        public string GetSwaggerHomepage(IFeatureManager featureManager)
        {
            var description = new StringBuilder();
            if (featureManager.IsEnabled(Features.Metrics))
                description.Append(HomeGenerator.BasicHtml());
            description.Append("<p>This template provides loads of developer friendly features that makes dotnet core ready for microservices and containers scenarios.</p>");
            description.Append("<ul>");
            if (featureManager.IsEnabled(Features.HttpLogger))
                description.Append("<li><a target='_blank' href='/log?level=all'>/log</a> monitor your beautiful logs from the comfort of your browser </li>");
            if (featureManager.IsEnabled(Features.Healthz))
                description.Append("<li><a target='_blank' href='/healthz'>/healthz</a> url to monitor your application health status </li>");
            if (featureManager.IsEnabled(Features.HealthUI))
                description.Append("<li><a target='_blank' href='/healthchecks-ui'>/healthchecks-ui</a> UI to monitor your application health status </li>");
            description.Append("</ul>");

            return description.ToString();
        }
    }
}
