using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace CoreX.Extensions.Logging
{
    public class HttpLogger : ILogger
    {
        private string _name;
        private LogMiddleware _logMiddleware;

        protected IOptionsMonitor<HttpLoggerOptions> Options { get; set; }

        public HttpLogger(string name, LogMiddleware logMiddleware, IOptionsMonitor<HttpLoggerOptions> options)
        {
            _logMiddleware = logMiddleware;
            _name = name;
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
                _logMiddleware.LogMessage(new LogMessageEntry(DateTime.Now, logLevel, eventId, exception, formatter(state, exception)));
            }
        }
    }
}
