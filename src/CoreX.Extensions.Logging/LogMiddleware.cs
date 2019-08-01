using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CoreX.Extensions.Logging
{
    public class LogMiddleware : IMiddleware
    {
        public const int MaxActiveListeners = 10;
        private List<HttpLoggerProcessor> logProcessors = new List<HttpLoggerProcessor>();

        public LogMiddleware()
        {
            
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path == "/log" && context.Request.Method == "GET")
            {
                HttpLoggerProcessor processor = null;
                try
                {
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "text/html; charset=UTF-8";
                    context.RequestAborted.ThrowIfCancellationRequested();

                    var writer = new StreamWriter(context.Response.Body);
                    processor = new HttpLoggerProcessor(writer);
                    logProcessors.Add(processor);

                    // limit to max active listeners
                    if (logProcessors.Count > MaxActiveListeners)
                    {
                        logProcessors.RemoveAt(0);
                    }

                    processor.ProcessLogQueue();
                }
                catch(OperationCanceledException)
                {
                    if (processor != null)
                    {
                        logProcessors.Remove(processor);
                    }
                }
            }

            await next(context);
        }

        public void LogMessage(string message)
        {
            for (int i = 0; i < logProcessors.Count; i++)
            {
                if(logProcessors[i].IsValid())
                {
                    logProcessors[i].EnqueueMessage(message);
                }
                else
                {
                    logProcessors.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
