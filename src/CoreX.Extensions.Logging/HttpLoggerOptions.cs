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
        public List<Remote> Remotes { get; set; } = new List<Remote>();
    }

    public class Remote
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
