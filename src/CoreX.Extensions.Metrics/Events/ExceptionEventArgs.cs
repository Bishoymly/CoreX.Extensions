using CoreX.Extensions.Metrics.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreX.Extensions.Metrics.Events
{
    public class ExceptionEventArgs : EventArgs
    {
        public MetricsException Exception { get; set; }
    }
}
