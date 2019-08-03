using Microsoft.Extensions.Logging;

namespace CoreX.Extensions.Logging
{
    public class HttpLoggerProvider : ILoggerProvider
    {
        private LogMiddleware _logMiddleware;

        public HttpLoggerProvider(LogMiddleware logMiddleware)
        {
            _logMiddleware = logMiddleware;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new HttpLogger(categoryName, _logMiddleware, _logMiddleware._options);
        }

        public void Dispose()
        {
        }
    }
}
