using Microsoft.Extensions.Logging;
using System;

namespace CoreX.Extensions.Logging
{
    public class HttpLogger : ILogger
    {
        private string name;
        private LogMiddleware logMiddleware;

        public HttpLogger(string name, LogMiddleware logMiddleware)
        {
            this.logMiddleware = logMiddleware;
            this.name = name;
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
            string div = "<div>";
            if(logLevel == LogLevel.Error || logLevel == LogLevel.Critical)
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

            logMiddleware.LogMessage($"{div}{formatter(state, exception)}</div>");
            if (exception != null)
            {
                // Clearly print the message for the exception
                logMiddleware.LogMessage($"{div}{exception.Message.ToString().Replace("\r\n", "<br>").Replace("  ", "&nbsp;&nbsp;").Replace("\t", "&nbsp;&nbsp;")}</div>");

                // Slightly darker color for the stacktrace
                if (logLevel == LogLevel.Error || logLevel == LogLevel.Critical)
                {
                    div = "<div style='color:#A00'>";
                }
                else
                {
                    div = "<div style='color:#AA0'>";
                }
                
                logMiddleware.LogMessage($"{div}{exception.ToString().Replace("\r\n","<br>").Replace("  ", "&nbsp;&nbsp;").Replace("\t", "&nbsp;&nbsp;")}</div>");
            }
        }
    }
}
