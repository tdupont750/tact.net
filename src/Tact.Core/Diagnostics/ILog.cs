using System;

namespace Tact.Diagnostics
{
    public interface ILog
    {
        bool IsEnabled(LogLevel level);
        void Log(LogLevel level, string message);
        void Log(LogLevel level, string format, params object[] args);
        void Log(LogLevel level, Exception ex, string message);
        void Log(LogLevel level, Exception ex, string format, params object[] args);
    }
}
