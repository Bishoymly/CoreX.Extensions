using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace CoreX.Extensions.Metrics.Models
{
    public class Request
    {
        public string Id { get; set; }
        public string CorrelationId { get; set; }
        public DateTime Date { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public string User { get; set; }
        public string Status { get; set; }
        public int Duration { get; set; }
    }
}
