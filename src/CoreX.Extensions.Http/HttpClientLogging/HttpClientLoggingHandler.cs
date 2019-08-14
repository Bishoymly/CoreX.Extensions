using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreX.Extensions.Http.HttpClientLogging
{
    public class HttpClientLoggingHandler : DelegatingHandler
    {
        private readonly HttpClientLoggingOptions _options;
        private readonly ILoggerFactory _loggerFactory;

        public HttpClientLoggingHandler(HttpClientLoggingOptions options, ILoggerFactory loggerFactory)
        {
            _options = options;
            _loggerFactory = loggerFactory;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var logger = _loggerFactory.CreateLogger<HttpClient>();

            if (_options.Enabled)
            {
                logger.LogInformation(request.ToStringContent(_options.Headers, _options.Body, _options.Html));
            }

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            if (_options.Enabled)
            {
                logger.LogInformation(response.ToStringContent(_options.Headers, _options.Body, _options.Html));
            }

            return response;
        }
    }
}
