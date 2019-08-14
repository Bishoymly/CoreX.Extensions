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
        private readonly HttpClientLoggingOptions _options;
        private readonly ILoggerFactory _loggerFactory;

        public HttpClientLoggingHandlerBuilderFilter(IOptions<HttpClientLoggingOptions> options, ILoggerFactory loggerFactory)
        {
            _options = options.Value;
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
