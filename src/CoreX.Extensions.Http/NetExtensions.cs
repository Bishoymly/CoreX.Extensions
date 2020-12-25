using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.Encodings.Web;

namespace System.Net.Http
{
    public static class NetExtensions
    {
        public static string ToStringContent(this HttpRequestMessage request, bool showHeaders = true, bool showBody = true)
        {
            var builder = new StringBuilder();
            builder.Append($"Request: {request.Method} {request.RequestUri.ToString()}");

            if (showHeaders && request.Headers.Count() > 0)
            {
                builder.Append($"{Environment.NewLine}Headers:");
                foreach (var header in request.Headers)
                {
                    builder.Append($"{Environment.NewLine}{header.Key}:{string.Join(",", header.Value)}");
                }
            }

            if (showBody && request.Content != null)
            {
                builder.Append($"{Environment.NewLine}Content:");
                builder.Append(HtmlEncoder.Default.Encode(request.Content.ReadAsStringAsync().Result));
            }
            return builder.ToString();
        }

        public static string ToStringContent(this HttpResponseMessage response, bool showHeaders = true, bool showBody = true)
        {
            var builder = new StringBuilder();
            builder.Append($"Response: {(int)response.StatusCode} {response.ReasonPhrase}");
            builder.Append(" - for " + response.RequestMessage.ToStringContent(false, false));

            if (showHeaders && response.Headers.Count() > 0)
            {
                builder.Append($"{Environment.NewLine}Headers:");
                foreach (var header in response.Headers)
                {
                    builder.Append($"{Environment.NewLine}{header.Key}:{string.Join(",", header.Value)}");
                }
            }

            if (showBody && response.Content != null)
            {
                builder.Append($"{Environment.NewLine}Content:");
                builder.Append(HtmlEncoder.Default.Encode(response.Content.ReadAsStringAsync().Result));
            }
            return builder.ToString();
        }
    }
}
