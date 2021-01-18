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

        public HttpStatus HttpStatus
        {
            get
            {
                if (string.IsNullOrEmpty(Status))
                    return HttpStatus.Pending;

                switch(Status[0])
                {
                    case '1':
                    case '2':
                    case '3':
                        return HttpStatus.Success;

                    case '4':
                        return HttpStatus.Warning;

                    case '5':
                        return HttpStatus.Error;

                    default:
                        return HttpStatus.Pending;
                }
            }
        }
    }
}
