using System;

namespace Tact.Diagnostics.Implementation
{
    public class EmptyLog : ILog
    {
        public ILog GetLog(string name)
        {
            return this;
        }

        public bool IsEnabled(LogLevel level)
        {
            return false;
        }

        public void Log(LogLevel level, string message)
        {
        }

        public void Log(LogLevel level, string format, params object[] args)
        {
        }

        public void Log(LogLevel level, Exception ex, string message)
        {
        }

        public void Log(LogLevel level, Exception ex, string format, params object[] args)
        {
        }
    }
}
