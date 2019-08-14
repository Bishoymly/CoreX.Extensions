using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly IOptionsMonitor<HttpClientLoggingOptions> _options;
        private readonly ILoggerFactory _loggerFactory;

        public HttpClientLoggingHandler(IOptionsMonitor<HttpClientLoggingOptions> options, ILoggerFactory loggerFactory)
        {
            _options = options;
            _loggerFactory = loggerFactory;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var logger = _loggerFactory.CreateLogger<HttpClient>();

            if (_options.CurrentValue.Enabled)
            {
                logger.LogInformation(request.ToStringContent(_options.CurrentValue.Headers, _options.CurrentValue.Body, _options.CurrentValue.Html));
            }

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            if (_options.CurrentValue.Enabled)
            {
                logger.LogInformation(response.ToStringContent(_options.CurrentValue.Headers, _options.CurrentValue.Body, _options.CurrentValue.Html));
            }

            return response;
        }
    }
}
