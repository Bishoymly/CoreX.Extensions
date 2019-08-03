using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CoreX.Extensions.Logging
{
    public class LogMiddleware : IMiddleware
    {
        public const int MaxActiveListeners = 10;

        private List<HttpLoggerProcessor> _logProcessors = new List<HttpLoggerProcessor>();
        private IConfiguration _config;
        internal readonly IOptionsMonitor<HttpLoggerOptions> _options;

        public LogMiddleware(IConfiguration config, IOptionsMonitor<HttpLoggerOptions> options)
        {
            _options = options;
            _config = config;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (_options.CurrentValue.Enabled)
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
                        _logProcessors.Add(processor);

                        // limit to max active listeners
                        if (_logProcessors.Count > MaxActiveListeners)
                        {
                            _logProcessors.RemoveAt(0);
                        }

                        processor.ProcessLogQueue();
                    }
                    catch (OperationCanceledException)
                    {
                        if (processor != null)
                        {
                            _logProcessors.Remove(processor);
                        }
                    }
                }
            }

            await next(context);
        }

        public void LogMessage(string message)
        {
            for (int i = 0; i < _logProcessors.Count; i++)
            {
                if(_logProcessors[i].IsValid())
                {
                    _logProcessors[i].EnqueueMessage(message);
                }
                else
                {
                    _logProcessors.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
