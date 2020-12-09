using CoreX.Extensions.Metrics.Models;

using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CoreX.Extensions.Metrics
{
    public class MetricsService : IMetricsService
    {
        public List<Request> Requests { get; } = new List<Request>();

        public Request BeginRequest(HttpContext context)
        {
            var request = new Request
            {
                Method = context.Request.Method,
                Date = DateTime.Now,
                Path = context.Request.Path,
                User = context.User.Identity.Name,
                Id = context.TraceIdentifier
            };
            
            Requests.Add(request);
            return request;
        }

        public Request EndRequest(HttpContext context, TimeSpan elapsed)
        {
            var request = Requests.Find(r => r.Id == context.TraceIdentifier);
            
            request.Status = context.Response.StatusCode.ToString();
            request.Duration = elapsed;

            return request;
        }
    }
}
