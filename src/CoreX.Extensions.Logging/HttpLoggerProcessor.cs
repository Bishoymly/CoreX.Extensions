using System;
using System.Collections.Concurrent;
using System.IO;

namespace CoreX.Extensions.Logging
{
    public class HttpLoggerProcessor : IDisposable
    {
        private const int _maxQueuedMessages = 1024;

        private readonly BlockingCollection<string> _messageQueue = new BlockingCollection<string>(_maxQueuedMessages);
        private StreamWriter _writer = null;

        public HttpLoggerProcessor(StreamWriter writer)
        {
            _writer = writer;
            _writer.AutoFlush = true;
            _writer.WriteLine("<header><style>body{background:#000;color:#fff;line-height:14px;font-size:12px;font-family:'Lucida Console', Monaco, monospace}</style></header>");
        }

        public virtual bool IsValid()
        {
            return _writer.BaseStream.CanWrite;
        }

        public virtual void EnqueueMessage(string message)
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

        internal virtual void WriteMessage(string message)
        {
            _writer.WriteLine(message);
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
