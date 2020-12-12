using CoreX.Extensions.Metrics.Events;
using CoreX.Extensions.Metrics.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace CoreX.Extensions.Metrics
{
    public interface IMetricsService
    {
        List<Request> Requests { get; }
        Request BeginRequest(HttpContext context);
        Request EndRequest(HttpContext context, TimeSpan elapsed);

        event EventHandler<RequestEventArgs> RequestStarted;
        event EventHandler<RequestEventArgs> RequestEnded;
    }
}