using Microsoft.Extensions.Logging;

namespace CoreX.Extensions.Logging
{
    public class HttpLoggerProvider : ILoggerProvider
    {
        private LogMiddleware logMiddleware;

        public HttpLoggerProvider(LogMiddleware logMiddleware)
        {
            this.logMiddleware = logMiddleware;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new HttpLogger(categoryName, logMiddleware);
        }

        public void Dispose()
        {
        }
    }
}
