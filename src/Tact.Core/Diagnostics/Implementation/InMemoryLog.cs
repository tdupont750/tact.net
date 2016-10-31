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

        public  ConcurrentQueue<Tuple<LogLevel, Exception, string>> Logs { get; } = new ConcurrentQueue<Tuple<LogLevel, Exception, string>>();

        public bool IsEnabled(LogLevel level)
        {
            return level >= _minLogLevel;
        }

        public void Log(LogLevel level, string message)
        {
            if (!IsEnabled(level)) return;
            EnsureCount();
            Logs.Enqueue(Tuple.Create(level, (Exception) null, message));
        }

        public void Log(LogLevel level, string format, params object[] args)
        {
            if (!IsEnabled(level)) return;
            EnsureCount();
            Logs.Enqueue(Tuple.Create(level, (Exception) null, string.Format(format, args)));
        }

        public void Log(LogLevel level, Exception ex, string message)
        {
            if (!IsEnabled(level)) return;
            EnsureCount();
            Logs.Enqueue(Tuple.Create(level, ex, message));
        }

        public void Log(LogLevel level, Exception ex, string format, params object[] args)
        {
            if (!IsEnabled(level)) return;
            EnsureCount();
            Logs.Enqueue(Tuple.Create(level, ex, string.Format(format, args)));
        }

        private void EnsureCount()
        {
            if (Logs.Count < _maxQueueCount) return;
            Tuple<LogLevel, Exception, string> tuple;
            Logs.TryDequeue(out tuple);
        }
    }
}
