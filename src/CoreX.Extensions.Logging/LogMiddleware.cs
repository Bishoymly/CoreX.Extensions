using Microsoft.AspNetCore.Authentication;
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
                    if (CheckAllowedFor(context))
                    {
                        HttpLoggerProcessor processor = null;
                        try
                        {
                            context.Response.StatusCode = 200;
                            context.Response.ContentType = "text/html; charset=UTF-8";
                            context.RequestAborted.ThrowIfCancellationRequested();

                            processor = new HttpLoggerProcessor(this, context);
                            _logProcessors.Add(processor);

                            // limit to max active listeners
                            if (_logProcessors.Count > MaxActiveListeners)
                            {
                                _logProcessors.RemoveAt(0);
                            }

                            processor.InitializeRemotes();
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
                    else
                    {
                        throw new UnauthorizedAccessException("You are not authorized to access logs..");
                    }
                }
            }

            await next(context);
        }

        private bool CheckAllowedFor(HttpContext context)
        {
            // Anonymous
            if (!_options.CurrentValue.AllowForAnonymous && !context.User.Identity.IsAuthenticated)
                return false;

            // User
            if (!string.IsNullOrWhiteSpace(_options.CurrentValue.AllowForUser) && context.User.Identity.Name != _options.CurrentValue.AllowForUser)
                return false;

            // Role
            if (!string.IsNullOrWhiteSpace(_options.CurrentValue.AllowForRole) && !context.User.IsInRole(_options.CurrentValue.AllowForRole))
                return false;

            return true;
        }

        public void LogMessage(LogMessageEntry message)
        {
            for (int i = 0; i < _logProcessors.Count; i++)
            {
                if(_logProcessors[i].AcceptsMessage(message))
                {
                    _logProcessors[i].EnqueueMessage(message);
                }
            }
        }
    }
}
