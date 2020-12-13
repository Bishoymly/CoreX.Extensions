using System;
using System.Collections.Generic;
using System.Text;

namespace CoreX.Extensions.Metrics.Models
{
    public class MetricsException
    {
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }

        public MetricsException()
        {
        }

        public MetricsException(Exception ex)
        {
            // take the most inner exception
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }

            Date = DateTime.Now;
            Type = ex.GetType().Name;
            Message = ex.Message;
            StackTrace = ex.StackTrace;
        }

        public override bool Equals(object obj)
        {
            if(obj is MetricsException)
            {
                var ex = obj as MetricsException;
                if (this.Type == ex.Type && this.StackTrace == ex.StackTrace)
                    return true;
                else
                    return false;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (Type + StackTrace).GetHashCode();
        }
    }
}
