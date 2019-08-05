using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace CoreX.Extensions.Logging
{
    public class HttpLoggerProcessor : IDisposable
    {
        private const int _maxQueuedMessages = 1024;

        private readonly BlockingCollection<LogMessageEntry> _messageQueue = new BlockingCollection<LogMessageEntry>(_maxQueuedMessages);
        private StreamWriter _writer = null;
        private LogMiddleware _middleware;
        private string _myHttpLoggerKey;

        private LogLevel _logLevel = LogLevel.Trace;
        private string _key;

        public HttpLoggerProcessor(LogMiddleware middleware, HttpContext context)
        {
            _middleware = middleware;

            // HttpLogger Key Cookie
            if(context.Request.Cookies.ContainsKey("HttpLogger"))
            {
                _myHttpLoggerKey = context.Request.Cookies["HttpLogger"];
            }
            else
            {
                _myHttpLoggerKey = Guid.NewGuid().ToString();
                context.Response.Cookies.Append("HttpLogger", _myHttpLoggerKey);
            }

            // Start writer
            _writer = new StreamWriter(context.Response.Body);
            _writer.AutoFlush = true;
            _writer.WriteLine("<header><style>body{background:#000;color:#fff;line-height:14px;font-size:12px;font-family:'Lucida Console', Monaco, monospace}</style></header>");

            InitializeQuery(context.Request.Query);
        }

        public virtual bool AcceptsMessage(LogMessageEntry message)
        {
            if (message.LogLevel < _logLevel)
                return false;

            if (!string.IsNullOrEmpty(_key) && message.HttpLoggerKey != _key)
                return false;

            return true;
        }

        public virtual void EnqueueMessage(LogMessageEntry message)
        {
            if (!_messageQueue.IsAddingCompleted)
            {
                try
                {
                    _messageQueue.Add(message);
                    return;
                }
                catch (InvalidOperationException) { }
            }

            // Adding is completed so just log the message
            try
            {
                WriteMessage(message);
            }
            catch (Exception) { }
        }

        internal void InitializeQuery(IQueryCollection query)
        {
            if(query.ContainsKey("level"))
            {
                _logLevel = ToLogLevel(query["level"]);
            }

            if (query.ContainsKey("my"))
            {
                _key = _myHttpLoggerKey;
            }

            if (query.ContainsKey("key"))
            {
                if (query["key"].ToString().ToLower() == "my")
                {
                    _key = _myHttpLoggerKey;
                }
                else
                {
                    _key = query["level"];
                }
            }
        }

        internal LogLevel ToLogLevel(string level)
        {
            switch (level.ToLower())
            {
                case "information":
                case "info":
                    return LogLevel.Information;

                case "warning":
                case "warn":
                    return LogLevel.Warning;

                case "error":
                case "err":
                    return LogLevel.Error;

                case "critical":
                case "fatal":
                    return LogLevel.Critical;

                case "debug":
                case "dbg":
                    return LogLevel.Debug;

                case "trace":
                case "verbose":
                case "verb":
                case "trc":
                case "all":
                case "any":
                    return LogLevel.Trace;

                case "none":
                    return LogLevel.None;

                default:
                    return LogLevel.Trace;
            }
        }

        internal virtual void WriteMessage(LogMessageEntry message)
        {
            _writer.WriteLine($"<div style='color:{ToColor(message.LogLevel)}'>{message.TimeStamp.ToString(_middleware._options.CurrentValue.TimestampFormat) + ": "}{ToHtml(message.Message)}</div>");
            if (message.Exception != null)
            {
                _writer.WriteLine($"<div style='color:{ToColor(message.LogLevel)}'>{ToHtml(message.Exception.Message)}</div>");
                _writer.WriteLine($"<div style='color:{ToStackTraceColor(message.LogLevel)}'>{ToHtml(message.Exception.ToString())}</div>");
            }
        }

        protected string ToColor(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    return "gray";
                case LogLevel.Warning:
                    return "yellow";
                case LogLevel.Error:
                case LogLevel.Critical:
                    return "red";
                case LogLevel.Information:
                case LogLevel.None:
                default:
                    return "#fff";
            }
        }

        protected string ToStackTraceColor(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Warning:
                    return "#AA0";
                case LogLevel.Error:
                case LogLevel.Critical:
                case LogLevel.Information:
                case LogLevel.None:
                default:
                    return "#A00";
            }
        }

        protected string ToHtml(string body)
        {
            return body.Replace("\r\n", "<br>").Replace("  ", "&nbsp;&nbsp;").Replace("\t", "&nbsp;&nbsp;");
        }

        public void ProcessLogQueue()
        {
            try
            {
                foreach (var message in _messageQueue.GetConsumingEnumerable())
                {
                    WriteMessage(message);
                }
            }
            catch
            {
                try
                {
                    _messageQueue.CompleteAdding();
                }
                catch { }
            }
        }

        public void Dispose()
        {
            _messageQueue.CompleteAdding();
        }
    }
}
