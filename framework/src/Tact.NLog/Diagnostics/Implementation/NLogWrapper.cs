using System;
using System.Collections.Concurrent;
using NLog;
using NLogLevel = NLog.LogLevel;

namespace Tact.Diagnostics.Implementation
{
    public class NLogWrapper : ILog
    {
        private static readonly ConcurrentDictionary<string, ILog> NameMap 
            = new ConcurrentDictionary<string, ILog>();

        private readonly ILogger _logger;
        
        public NLogWrapper(ILogger logger)
        {
            _logger = logger;
        }

        public static ILog GetLog(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            return NameMap.GetOrAdd(name, n => new NLogWrapper(LogManager.GetLogger(n)));
        }

        ILog ILog.GetLog(string name)
        {
            return GetLog(name);
        }

        public bool IsEnabled(LogLevel level)
        {
            return _logger.IsEnabled(Convert(level));
        }

        public void Log(LogLevel level, string message)
        {
            _logger.Log(Convert(level), message);
        }

        public void Log(LogLevel level, string format, params object[] args)
        {
            _logger.Log(Convert(level), format, args);
        }

        public void Log(LogLevel level, Exception ex, string message)
        {
            _logger.Log(Convert(level), ex, message);
        }

        public void Log(LogLevel level, Exception ex, string format, params object[] args)
        {
            _logger.Log(Convert(level), ex, format, args);
        }

        private static NLogLevel Convert(LogLevel logLevel)
        {
            return NLogLevel.FromOrdinal((int) logLevel);
        }
    }
}
