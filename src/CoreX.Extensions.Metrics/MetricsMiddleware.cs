using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace CoreX.Extensions.Metrics
{
    public class MetricsMiddleware : IMiddleware
    {
        private IConfiguration _config;
        private IMetricsService _metricsService; 

        public MetricsMiddleware(IConfiguration config, IMetricsService metricsService)
        {
            _config = config;
            _metricsService = metricsService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _metricsService.BeginRequest(context);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            await next(context);

            stopwatch.Stop();
            
            _metricsService.EndRequest(context, stopwatch.Elapsed);
        }
    }
}
