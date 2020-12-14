using System;
using System.Collections.Generic;
using System.Text;

namespace CoreX.Extensions.Http.HttpClientLogging
{
    public class HttpClientLoggingOptions
    {
        public bool Enabled { get; set; } = true;
        public bool Headers { get; set; } = true;
        public bool Body { get; set; } = false;
    }
}
