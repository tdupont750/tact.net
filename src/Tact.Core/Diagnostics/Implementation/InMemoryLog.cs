using System;
using System.Collections.Concurrent;

namespace Tact.Diagnostics.Implementation
{
    public class InMemoryLog: ILog
    {
        private readonly LogLevel _minLogLevel;
        private readonly int _maxQueueCount;

        public InMemoryLog(LogLevel minLogLevel = LogLevel.Trace, int maxQueueCount = 1000)
        {
            _minLogLevel = minLogLevel;
            _maxQueueCount = maxQueueCount;
        }

        public  ConcurrentQueue<Tuple<LogLevel, string, Exception>> Logs { get; } = new ConcurrentQueue<Tuple<LogLevel, string, Exception>>();

        public bool IsEnabled(LogLevel level)
        {
            return level >= _minLogLevel;
        }

        public void Log(LogLevel level, string message)
        {
            if (!IsEnabled(level)) return;
            EnsureCount();
            Logs.Enqueue(Tuple.Create(level, message, (Exception)null));
        }

        public void Log(LogLevel level, string format, params object[] args)
        {
            if (!IsEnabled(level)) return;
            EnsureCount();
            Logs.Enqueue(Tuple.Create(level, string.Format(format, args), (Exception)null));
        }

        public void Log(LogLevel level, Exception ex, string message)
        {
            if (!IsEnabled(level)) return;
            EnsureCount();
            Logs.Enqueue(Tuple.Create(level, message, ex));
        }

        public void Log(LogLevel level, Exception ex, string format, params object[] args)
        {
            if (!IsEnabled(level)) return;
            EnsureCount();
            Logs.Enqueue(Tuple.Create(level, string.Format(format, args), ex));
        }

        private void EnsureCount()
        {
            if (Logs.Count < _maxQueueCount) return;
            Tuple<LogLevel, string, Exception> tuple;
            Logs.TryDequeue(out tuple);
        }
    }
}
