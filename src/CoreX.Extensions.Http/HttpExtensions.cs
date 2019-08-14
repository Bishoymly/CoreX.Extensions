using System;
using System.IO;
using System.Text;

namespace Microsoft.AspNetCore.Http
{
    public static class HttpExtensions
    {
        public static string ToHtml(this HttpRequest request, bool showHeaders = true, bool showBody = true)
        {
            return request.ToStringContent(showHeaders, showBody, true);
        }

        public static string ToStringContent(this HttpRequest request, bool showHeaders = true, bool showBody = true, bool html = false)
        {
            var builder = new StringBuilder();
            builder.Append($"{IsHtml("<i>", html)}Request:{IsHtml("</i>", html)}");
            builder.Append($"{Environment.NewLine}{IsHtml("<b>", html)}{request.Method} {request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}{IsHtml("</b>", html)}");

            if (showHeaders && request.Headers.Count > 0)
            {
                builder.Append($"{Environment.NewLine}{IsHtml("<i>", html)}Headers:{IsHtml("</i>", html)}");
                foreach (var header in request.Headers)
                {
                    builder.Append($"{Environment.NewLine}{IsHtml("<span style='opacity:0.5;'>", html)}{header.Key}:{string.Join(",", header.Value)}{IsHtml("</span>", html)}");
                }
            }

            if (showBody && request.ContentLength > 0)
            {
                request.EnableBuffering();
                builder.Append($"{Environment.NewLine}{IsHtml("<i>", html)}Content:{IsHtml("</i>", html)}");
                builder.Append($"{Environment.NewLine}{ IsHtml("<span style='opacity:0.5;'>", html)}");
                using (StreamReader reader = new StreamReader(request.Body))
                {
                    builder.Append(reader.ReadToEnd());
                }
                builder.Append(IsHtml("</span>", html));
            }
            return builder.ToString();
        }

        private static string IsHtml(string text, bool html)
        {
            if (html)
                return text;
            else
                return string.Empty;
        }
    }
}
