using System;
using System.Collections.Concurrent;

namespace Tact.Diagnostics.Implementation
{
    public class InMemoryLog : ILog
    {
        private const LogLevel DefaultMinLogLevel = LogLevel.Trace;

        private const int DefaultMaxQueueCount = 1000;

        private static readonly ConcurrentDictionary<string, ILog> NameMap
            = new ConcurrentDictionary<string, ILog>();

        private readonly LogLevel _minLogLevel;

        private readonly int _maxQueueCount;

        public InMemoryLog(LogLevel minLogLevel = DefaultMinLogLevel, int maxQueueCount = DefaultMaxQueueCount)
        {
            _minLogLevel = minLogLevel;
            _maxQueueCount = maxQueueCount;
        }

        public ConcurrentQueue<LogLine> LogLines { get; } = new ConcurrentQueue<LogLine>();

        public static ILog GetLog(string name, LogLevel minLogLevel = DefaultMinLogLevel, int maxQueueCount = DefaultMaxQueueCount)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            return NameMap.GetOrAdd(name, n => new InMemoryLog(minLogLevel, maxQueueCount));
        }
        
        ILog ILog.GetLog(string name)
        {
            return GetLog(name);
        }

        public bool IsEnabled(LogLevel level)
        {
            return level >= _minLogLevel;
        }

        public void Log(LogLevel level, string message)
        {
            if (!IsEnabled(level)) return;
            EnsureCount();
            LogLines.Enqueue(new LogLine(level, message));
        }

        public void Log(LogLevel level, string format, params object[] args)
        {
            if (!IsEnabled(level)) return;
            EnsureCount();
            LogLines.Enqueue(new LogLine(level, string.Format(format, args)));
        }

        public void Log(LogLevel level, Exception ex, string message)
        {
            if (!IsEnabled(level)) return;
            EnsureCount();
            LogLines.Enqueue(new LogLine(level, message, ex));
        }

        public void Log(LogLevel level, Exception ex, string format, params object[] args)
        {
            if (!IsEnabled(level)) return;
            EnsureCount();
            LogLines.Enqueue(new LogLine(level, string.Format(format, args), ex));
        }

        private void EnsureCount()
        {
            if (LogLines.Count < _maxQueueCount) return;
            LogLine logLine;
            LogLines.TryDequeue(out logLine);
        }

        public struct LogLine
        {
            public LogLine(LogLevel logLevel, string message, Exception exception = null)
            {
                LogLevel = logLevel;
                Message = message;
                Exception = exception;
            }
            
            public readonly LogLevel LogLevel;
            public readonly string Message;
            public readonly Exception Exception;
        }
    }
}
