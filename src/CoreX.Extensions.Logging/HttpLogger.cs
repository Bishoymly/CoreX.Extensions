using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace CoreX.Extensions.Logging
{
    public class HttpLogger : ILogger
    {
        private string _name;
        private LogMiddleware _logMiddleware;
        private IHttpContextAccessor _contextAccessor;

        protected IOptionsMonitor<HttpLoggerOptions> Options { get; set; }

        public HttpLogger(string name, LogMiddleware logMiddleware, IOptionsMonitor<HttpLoggerOptions> options, IHttpContextAccessor contextAccessor)
        {
            _logMiddleware = logMiddleware;
            _name = name;
            _contextAccessor = contextAccessor;
            Options = options;
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
            if (Options.CurrentValue.Enabled)
            {
                string key = null;
                if (_contextAccessor.HttpContext.Request.Cookies.ContainsKey("HttpLogger"))
                {
                    key = _contextAccessor.HttpContext.Request.Cookies["HttpLogger"];
                }

                if (_contextAccessor.HttpContext.Request.Headers.ContainsKey("HttpLogger"))
                {
                    key = _contextAccessor.HttpContext.Request.Headers["HttpLogger"];
                }

                _logMiddleware.LogMessage(new LogMessageEntry(DateTime.Now, logLevel, eventId, exception, formatter(state, exception), key));
            }
        }
    }
}
