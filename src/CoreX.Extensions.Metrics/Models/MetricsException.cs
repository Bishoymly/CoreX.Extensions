using Microsoft.AspNetCore.Http;
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
        public string Path { get; set; }
        public string RequestId { get; set; }

        public MetricsException()
        {
        }

        public MetricsException(Exception ex, HttpContext context)
        {
            // take the most inner exception
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }

            Date = DateTime.Now;
            Type = ex.GetType().Name;
            Message = ToHtml(ex.Message);
            StackTrace = ToHtml(ex.StackTrace);

            if(context != null && context.Request != null)
            {
                Path = context.Request.Path;
                RequestId = context.TraceIdentifier;
            }
        }

        public MetricsException(string error, HttpContext context)
        {
            Date = DateTime.Now;
            Type = "Error";
            Message = ToHtml(error);
            StackTrace = "";
            
            if (context != null && context.Request != null)
            {
                Path = context.Request.Path;
                RequestId = context.TraceIdentifier;
            }
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

        protected string ToHtml(string body)
        {
            return body.Replace("\r\n", "<br>").Replace("  ", "&nbsp;&nbsp;").Replace("\t", "&nbsp;&nbsp;");
        }
    }
}
