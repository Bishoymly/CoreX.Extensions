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
                var div = "<div>";
                if (logLevel == LogLevel.Error || logLevel == LogLevel.Critical)
                {
                    div = "<div style='color:red'>";
                }

                if (logLevel == LogLevel.Warning)
                {
                    div = "<div style='color:yellow'>";
                }

                if (logLevel == LogLevel.Trace || logLevel == LogLevel.Debug)
                {
                    div = "<div style='color:gray'>";
                }

                _logMiddleware.LogMessage($"{div}{DateTime.Now.ToString(Options.CurrentValue.TimestampFormat) + ": "}{formatter(state, exception)}</div>");
                if (exception != null)
                {
                    // Clearly print the message for the exception
                    _logMiddleware.LogMessage($"{div}{ToHtml(exception.Message)}</div>");

                    // Slightly darker color for the stacktrace
                    if (logLevel == LogLevel.Error || logLevel == LogLevel.Critical)
                    {
                        div = "<div style='color:#A00'>";
                    }
                    else
                    {
                        div = "<div style='color:#AA0'>";
                    }

                    _logMiddleware.LogMessage($"{div}{ToHtml(exception.ToString())}</div>");
                }
            }
        }

        protected string ToHtml(string body)
        {
            return body.Replace("\r\n", "<br>").Replace("  ", "&nbsp;&nbsp;").Replace("\t", "&nbsp;&nbsp;");
        }
    }
}
