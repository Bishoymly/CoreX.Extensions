using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CoreX.Extensions.Logging
{
    public class HttpLoggerProvider : ILoggerProvider
    {
        private LogMiddleware _logMiddleware;
        private IHttpContextAccessor _contextAccessor;

        public HttpLoggerProvider(LogMiddleware logMiddleware, IHttpContextAccessor contextAccessor)
        {
            _logMiddleware = logMiddleware;
            _contextAccessor = contextAccessor;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new HttpLogger(categoryName, _logMiddleware, _logMiddleware._options, _contextAccessor);
        }

        public void Dispose()
        {
        }
    }
}
