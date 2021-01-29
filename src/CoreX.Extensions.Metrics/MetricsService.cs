using CoreX.Extensions.Metrics.Events;
using CoreX.Extensions.Metrics.Models;

using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            if (!IsPathEnabled(context.Request.Path))
                return null;

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
            if (!IsPathEnabled(context.Request.Path))
                return null;

            var request = Requests.Find(r => r.Id == context.TraceIdentifier);
            if (request != null)
            {
                request.Status = context.Response.StatusCode.ToString();
                request.Duration = (int)elapsed.TotalMilliseconds;

                RequestEnded?.Invoke(this, new RequestEventArgs { Request = request });
            }

            return request;
        }

        public List<RequestAggregate> GetTopRequests()
        {
            return Requests.Where(r => r.Status != null)
                .GroupBy(r => new { r.Method, r.Path })
                .OrderByDescending(r => r.Sum(s => s.Duration))
                .Select(r => new RequestAggregate
                {
                    Method = r.Key.Method,
                    Path = r.Key.Path,
                    Count = r.Count(),
                    DurationTotal = r.Sum(s=>s.Duration),
                    DurationAvg = (int)r.Average(s => s.Duration),
                    SuccessCount = r.Count(s=>s.HttpStatus == HttpStatus.Success),
                    FailedCount = r.Count(s=>s.HttpStatus == HttpStatus.Error || s.HttpStatus == HttpStatus.Warning)
                })
                .ToList();
        }

        protected bool IsPathEnabled(string path)
        {
            if (path.StartsWith("/devdash"))
                return false;

            if (path == "/" || path == "/log" || path == "/healthz")
                return false;

            return true;
        }
    }
}
