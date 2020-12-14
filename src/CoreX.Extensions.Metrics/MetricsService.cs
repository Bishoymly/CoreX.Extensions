using CoreX.Extensions.Metrics.Events;
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

        public List<MetricsException> Exceptions { get; } = new List<MetricsException>();

        public event EventHandler<RequestEventArgs> RequestStarted;
        public event EventHandler<RequestEventArgs> RequestEnded;
        public event EventHandler<ExceptionEventArgs> ExceptionAdded;

        public MetricsException AddException(MetricsException exception)
        {            
            Exceptions.Add(exception);
            ExceptionAdded?.Invoke(this, new ExceptionEventArgs { Exception = exception });
            return exception;
        }

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
            RequestStarted?.Invoke(this, new RequestEventArgs { Request = request });
            return request;
        }

        public Request EndRequest(HttpContext context, TimeSpan elapsed)
        {
            var request = Requests.Find(r => r.Id == context.TraceIdentifier);
            if (request != null)
            {
                request.Status = context.Response.StatusCode.ToString();
                request.Duration = (int)elapsed.TotalMilliseconds;

                RequestEnded?.Invoke(this, new RequestEventArgs { Request = request });
            }

            return request;
        }
    }
}
