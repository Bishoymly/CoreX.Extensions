﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CoreX.Extensions.Metrics.Models
{
    public class RequestAggregate
    {
        public string Id 
        { 
            get
            {
                return Method + " " + Path;
            } 
        }

        public string Method { get; set; }
        public string Path { get; set; }
        public int Count { get; set; }
        public int DurationTotal { get; set; }
        public int DurationAvg { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
    }
}
