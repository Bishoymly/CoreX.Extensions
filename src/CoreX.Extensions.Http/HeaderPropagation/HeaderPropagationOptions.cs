using System;
using System.Collections.Generic;
using System.Text;

namespace CoreX.Extensions.Http.HeaderPropagation
{
    public class HeaderPropagationOptions
    {
        public IList<string> HeaderNames { get; set; }

        public HeaderPropagationOptions()
        {
            HeaderNames = new List<string>
            {
                "Request-ID",
                "X-Request-ID",
                "X-Correlation-ID",
                "x-b3-traceid",
                "x-b3-spanId",
                "x-b3-parentspanid",
                "x-b3-sampled",
                "x-b3-flags",
                "b3",
                "x-ot-span-context",
                "HttpLogger"
            };
        }
    }
}
