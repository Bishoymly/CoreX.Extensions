using CorrelationId;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoreX.Extensions.Http.HeaderPropagation
{
    public class HeaderPropagationMessageHandler : DelegatingHandler
    {
        private readonly HeaderPropagationOptions _options;
        private readonly IHttpContextAccessor _contextAccessor;
        public HeaderPropagationMessageHandler(HeaderPropagationOptions options, IHttpContextAccessor contextAccessor)
        {
            _options = options;
            _contextAccessor = contextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (_contextAccessor != null && _contextAccessor.HttpContext != null)
            {
                // REVIEW: This logic likely gets more fancy and allows mapping headers in more complex ways
                foreach (var headerName in _options.HeaderNames)
                {
                    // Get the incoming header value
                    var headerValue = _contextAccessor.HttpContext.Request.Headers[headerName];
                    if (StringValues.IsNullOrEmpty(headerValue))
                    {
                        continue;
                    }

                    request.Headers.TryAddWithoutValidation(headerName, (string[])headerValue);
                }

                // Add correlation header if present
                var accessor = _contextAccessor.HttpContext.RequestServices.GetService(typeof(ICorrelationContextAccessor)) as ICorrelationContextAccessor;
                if (accessor != null && accessor.CorrelationContext != null)
                {
                    if (!request.Headers.Contains(accessor.CorrelationContext.Header))
                    {
                        request.Headers.TryAddWithoutValidation(accessor.CorrelationContext.Header, accessor.CorrelationContext.CorrelationId);
                    }
                }
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
