using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;

namespace CoreX.Extensions.Metrics
{
    public class MetricsLoggerProvider : ILoggerProvider
    {
        private IServiceProvider _services;

        public MetricsLoggerProvider(IServiceProvider services)
        {
            _services = services;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _services.GetService(typeof(MetricsLogger)) as ILogger;
        }

        public void Dispose()
        {
        }
    }
}
