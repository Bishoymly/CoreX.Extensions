using System;
using System.Collections.Generic;
using System.Text;

namespace CoreX.Extensions.Logging
{
    public class HttpLoggerOptions
    {
        public bool Enabled { get; set; } = true;
        public string TimestampFormat { get; set; } = "hh:mm:ss.fff";
        public bool AllowForAnonymous { get; set; } = true;
        public string AllowForUser { get; set; }
        public string AllowForRole { get; set; }
    }
}
