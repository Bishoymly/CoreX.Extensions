using CoreX.Extensions.Metrics.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;

namespace CoreX.Extensions.Metrics
{
    public class MetricsLogger : ILogger
    {
        public string Name { get; protected set; }
        private IMetricsService _metrics;
        private IHttpContextAccessor _contextAccessor;

        public MetricsLogger(IMetricsService metrics, IHttpContextAccessor contextAccessor)
        {
            _metrics = metrics;
            _contextAccessor = contextAccessor;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (_contextAccessor != null && _contextAccessor.HttpContext != null)
            {
                if (exception != null)
                {
                    _metrics.AddException(new MetricsException(exception, _contextAccessor.HttpContext));
                }
                else if(logLevel == LogLevel.Error || logLevel == LogLevel.Warning)
                {
                    _metrics.AddException(new MetricsException(formatter(state, exception), _contextAccessor.HttpContext));
                }
            }
        }
    }
}
