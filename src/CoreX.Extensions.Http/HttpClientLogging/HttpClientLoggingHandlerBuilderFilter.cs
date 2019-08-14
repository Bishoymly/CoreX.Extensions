using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreX.Extensions.Http.HttpClientLogging
{
    internal class HttpClientLoggingHandlerBuilderFilter : IHttpMessageHandlerBuilderFilter
    {
        private readonly IOptionsMonitor<HttpClientLoggingOptions> _options;
        private readonly ILoggerFactory _loggerFactory;

        public HttpClientLoggingHandlerBuilderFilter(IOptionsMonitor<HttpClientLoggingOptions> options, ILoggerFactory loggerFactory)
        {
            _options = options;
            _loggerFactory = loggerFactory;
        }

        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            return builder =>
            {
                builder.AdditionalHandlers.Add(new HttpClientLoggingHandler(_options, _loggerFactory));
                next(builder);
            };
        }
    }
}
