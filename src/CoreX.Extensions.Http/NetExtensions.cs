using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.Encodings.Web;

namespace System.Net.Http
{
    public static class NetExtensions
    {
        public static string ToHtml(this HttpRequestMessage request, bool showHeaders = true, bool showBody = true)
        {
            return request.ToStringContent(showHeaders, showBody, true);
        }

        public static string ToStringContent(this HttpRequestMessage request, bool showHeaders = true, bool showBody = true, bool html = false)
        {
            var builder = new StringBuilder();
            builder.Append($"{IsHtml("<i>", html)}Request:{IsHtml("</i>", html)}");
            builder.Append($"{Environment.NewLine}{IsHtml("<b>", html)}{request.Method} {request.RequestUri.ToString()}{IsHtml("</b>", html)}");

            if (showHeaders && request.Headers.Count() > 0)
            {
                builder.Append($"{Environment.NewLine}{IsHtml("<i>", html)}Headers:{IsHtml("</i>", html)}");
                foreach (var header in request.Headers)
                {
                    builder.Append($"{Environment.NewLine}{IsHtml("<span style='opacity:0.5;'>", html)}{header.Key}:{string.Join(",", header.Value)}{IsHtml("</span>", html)}");
                }
            }

            if (showBody && request.Content != null)
            {
                builder.Append($"{Environment.NewLine}{IsHtml("<i>", html)}Content:{IsHtml("</i>", html)}");
                builder.Append($"{Environment.NewLine}{ IsHtml("<span style='opacity:0.5;'>", html)}");
                builder.Append(HtmlEncoder.Default.Encode(request.Content.ReadAsStringAsync().Result));
                builder.Append(IsHtml("</span>", html));
            }
            return builder.ToString();
        }

        public static string ToHtml(this HttpResponseMessage response, bool showHeaders = true, bool showBody = true)
        {
            return response.ToStringContent(showHeaders, showBody, true);
        }

        public static string ToStringContent(this HttpResponseMessage response, bool showHeaders = true, bool showBody = true, bool html = false)
        {
            var builder = new StringBuilder();
            builder.Append($"{IsHtml("<i>", html)}Reponse:{IsHtml("</i>", html)}");
            builder.Append($"{Environment.NewLine}{IsHtml("<b>", html)}{response.StatusCode} {response.ReasonPhrase}{IsHtml("</b>", html)}");

            if (showHeaders && response.Headers.Count() > 0)
            {
                builder.Append($"{Environment.NewLine}{IsHtml("<i>", html)}Headers:{IsHtml("</i>", html)}");
                foreach (var header in response.Headers)
                {
                    builder.Append($"{Environment.NewLine}{IsHtml("<span style='opacity:0.5;'>", html)}{header.Key}:{string.Join(",", header.Value)}{IsHtml("</span>", html)}");
                }
            }

            if (showBody && response.Content != null)
            {
                builder.Append($"{Environment.NewLine}{IsHtml("<i>", html)}Content:{IsHtml("</i>", html)}");
                builder.Append($"{Environment.NewLine}{ IsHtml("<span style='opacity:0.5;'>", html)}");
                builder.Append(HtmlEncoder.Default.Encode(response.Content.ReadAsStringAsync().Result));
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
