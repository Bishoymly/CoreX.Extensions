using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreX.Extensions.Logging
{
    public readonly struct LogMessageEntry
    {
        public LogMessageEntry(DateTime timeStamp, LogLevel logLevel, EventId eventId, Exception exception, string message)
        {
            TimeStamp = timeStamp;
            LogLevel = logLevel;
            Message = message;
            EventId = eventId;
            Exception = exception;
        }

        public readonly DateTime TimeStamp;
        public readonly LogLevel LogLevel;
        public readonly string Message;
        public readonly EventId EventId;
        public readonly Exception Exception;
    }
}
