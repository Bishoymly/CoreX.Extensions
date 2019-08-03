using System;
using System.Collections.Generic;
using System.Text;

namespace CoreX.Extensions.Logging
{
    public class HttpLoggerOptions
    {
        public bool Enabled { get; set; } = true;
        public string TimestampFormat { get; set; } = "hh:mm:ss.fff";
    }
}
