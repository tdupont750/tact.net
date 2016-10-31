﻿using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
 
// ReSharper disable once CheckNamespace
namespace Tact
{
    using Diagnostics;

    namespace Diagnostics
    {
        public enum LogLevel
        {
            Trace = 0,
            Debug = 1,
            Info = 2,
            Warn = 3,
            Error = 4,
            Fatal = 5,
        }
    }

    public static class LogExtensions
    {
        private static readonly char[] DirectorySeparatorChars = {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar};
        private static readonly Regex ClassNameRegex = new Regex(@"([\w]+)\.[\w]+$", RegexOptions.Compiled);
 
        public static void Trace(
            this ILog log,
            string message,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Trace)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(message, memberName, sourceFilePath, sourceLineNumber) : message;
            log.Log(LogLevel.Trace, newFormat);
        }
 
        public static void Trace(
            this ILog log,
            Exception ex,
            string message,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Trace)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(message, memberName, sourceFilePath, sourceLineNumber) : message;
            log.Log(LogLevel.Trace, ex, newFormat);
        }
 
        public static void Trace(
            this ILog log,
            string format,
            object arg0,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Trace)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Trace, newFormat, arg0);
        }
 
        public static void Trace(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Trace)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Trace, ex, newFormat, arg0);
        }
 
        public static void Trace(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Trace)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Trace, newFormat, arg0, arg1);
        }
 
        public static void Trace(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Trace)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Trace, ex, newFormat, arg0, arg1);
        }
 
        public static void Trace(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Trace)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Trace, newFormat, arg0, arg1, arg2);
        }
 
        public static void Trace(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Trace)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Trace, ex, newFormat, arg0, arg1, arg2);
        }
 
        public static void Trace(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Trace)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Trace, newFormat, arg0, arg1, arg2, arg3);
        }
 
        public static void Trace(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Trace)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Trace, ex, newFormat, arg0, arg1, arg2, arg3);
        }
 
        public static void Trace(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Trace)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Trace, newFormat, arg0, arg1, arg2, arg3, arg4);
        }
 
        public static void Trace(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Trace)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Trace, ex, newFormat, arg0, arg1, arg2, arg3, arg4);
        }
 
        public static void Trace(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Trace)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Trace, newFormat, arg0, arg1, arg2, arg3, arg4, arg5);
        }
 
        public static void Trace(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Trace)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Trace, ex, newFormat, arg0, arg1, arg2, arg3, arg4, arg5);
        }
 
        public static void Trace(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Trace)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Trace, newFormat, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }
 
        public static void Trace(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Trace)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Trace, ex, newFormat, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }
 
        public static void Trace(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            object arg7,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Trace)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Trace, newFormat, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
 
        public static void Trace(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            object arg7,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Trace)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Trace, ex, newFormat, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
 
        public static void Debug(
            this ILog log,
            string message,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Debug)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(message, memberName, sourceFilePath, sourceLineNumber) : message;
            log.Log(LogLevel.Debug, newFormat);
        }
 
        public static void Debug(
            this ILog log,
            Exception ex,
            string message,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Debug)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(message, memberName, sourceFilePath, sourceLineNumber) : message;
            log.Log(LogLevel.Debug, ex, newFormat);
        }
 
        public static void Debug(
            this ILog log,
            string format,
            object arg0,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Debug)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Debug, newFormat, arg0);
        }
 
        public static void Debug(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Debug)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Debug, ex, newFormat, arg0);
        }
 
        public static void Debug(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Debug)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Debug, newFormat, arg0, arg1);
        }
 
        public static void Debug(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Debug)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Debug, ex, newFormat, arg0, arg1);
        }
 
        public static void Debug(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Debug)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Debug, newFormat, arg0, arg1, arg2);
        }
 
        public static void Debug(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Debug)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Debug, ex, newFormat, arg0, arg1, arg2);
        }
 
        public static void Debug(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Debug)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Debug, newFormat, arg0, arg1, arg2, arg3);
        }
 
        public static void Debug(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Debug)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Debug, ex, newFormat, arg0, arg1, arg2, arg3);
        }
 
        public static void Debug(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Debug)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Debug, newFormat, arg0, arg1, arg2, arg3, arg4);
        }
 
        public static void Debug(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Debug)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Debug, ex, newFormat, arg0, arg1, arg2, arg3, arg4);
        }
 
        public static void Debug(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Debug)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Debug, newFormat, arg0, arg1, arg2, arg3, arg4, arg5);
        }
 
        public static void Debug(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Debug)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Debug, ex, newFormat, arg0, arg1, arg2, arg3, arg4, arg5);
        }
 
        public static void Debug(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Debug)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Debug, newFormat, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }
 
        public static void Debug(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Debug)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Debug, ex, newFormat, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }
 
        public static void Debug(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            object arg7,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Debug)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Debug, newFormat, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
 
        public static void Debug(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            object arg7,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Debug)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Debug, ex, newFormat, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
 
        public static void Info(
            this ILog log,
            string message,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Info)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(message, memberName, sourceFilePath, sourceLineNumber) : message;
            log.Log(LogLevel.Info, newFormat);
        }
 
        public static void Info(
            this ILog log,
            Exception ex,
            string message,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Info)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(message, memberName, sourceFilePath, sourceLineNumber) : message;
            log.Log(LogLevel.Info, ex, newFormat);
        }
 
        public static void Info(
            this ILog log,
            string format,
            object arg0,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Info)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Info, newFormat, arg0);
        }
 
        public static void Info(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Info)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Info, ex, newFormat, arg0);
        }
 
        public static void Info(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Info)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Info, newFormat, arg0, arg1);
        }
 
        public static void Info(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Info)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Info, ex, newFormat, arg0, arg1);
        }
 
        public static void Info(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Info)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Info, newFormat, arg0, arg1, arg2);
        }
 
        public static void Info(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Info)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Info, ex, newFormat, arg0, arg1, arg2);
        }
 
        public static void Info(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Info)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Info, newFormat, arg0, arg1, arg2, arg3);
        }
 
        public static void Info(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Info)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Info, ex, newFormat, arg0, arg1, arg2, arg3);
        }
 
        public static void Info(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Info)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Info, newFormat, arg0, arg1, arg2, arg3, arg4);
        }
 
        public static void Info(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Info)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Info, ex, newFormat, arg0, arg1, arg2, arg3, arg4);
        }
 
        public static void Info(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Info)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Info, newFormat, arg0, arg1, arg2, arg3, arg4, arg5);
        }
 
        public static void Info(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Info)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Info, ex, newFormat, arg0, arg1, arg2, arg3, arg4, arg5);
        }
 
        public static void Info(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Info)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Info, newFormat, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }
 
        public static void Info(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Info)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Info, ex, newFormat, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }
 
        public static void Info(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            object arg7,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Info)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Info, newFormat, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
 
        public static void Info(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            object arg7,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Info)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Info, ex, newFormat, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
 
        public static void Warn(
            this ILog log,
            string message,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Warn)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(message, memberName, sourceFilePath, sourceLineNumber) : message;
            log.Log(LogLevel.Warn, newFormat);
        }
 
        public static void Warn(
            this ILog log,
            Exception ex,
            string message,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Warn)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(message, memberName, sourceFilePath, sourceLineNumber) : message;
            log.Log(LogLevel.Warn, ex, newFormat);
        }
 
        public static void Warn(
            this ILog log,
            string format,
            object arg0,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Warn)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Warn, newFormat, arg0);
        }
 
        public static void Warn(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Warn)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Warn, ex, newFormat, arg0);
        }
 
        public static void Warn(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Warn)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Warn, newFormat, arg0, arg1);
        }
 
        public static void Warn(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Warn)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Warn, ex, newFormat, arg0, arg1);
        }
 
        public static void Warn(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Warn)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Warn, newFormat, arg0, arg1, arg2);
        }
 
        public static void Warn(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Warn)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Warn, ex, newFormat, arg0, arg1, arg2);
        }
 
        public static void Warn(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Warn)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Warn, newFormat, arg0, arg1, arg2, arg3);
        }
 
        public static void Warn(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Warn)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Warn, ex, newFormat, arg0, arg1, arg2, arg3);
        }
 
        public static void Warn(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Warn)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Warn, newFormat, arg0, arg1, arg2, arg3, arg4);
        }
 
        public static void Warn(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Warn)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Warn, ex, newFormat, arg0, arg1, arg2, arg3, arg4);
        }
 
        public static void Warn(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Warn)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Warn, newFormat, arg0, arg1, arg2, arg3, arg4, arg5);
        }
 
        public static void Warn(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Warn)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Warn, ex, newFormat, arg0, arg1, arg2, arg3, arg4, arg5);
        }
 
        public static void Warn(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Warn)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Warn, newFormat, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }
 
        public static void Warn(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Warn)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Warn, ex, newFormat, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }
 
        public static void Warn(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            object arg7,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Warn)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Warn, newFormat, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
 
        public static void Warn(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            object arg7,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Warn)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Warn, ex, newFormat, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
 
        public static void Error(
            this ILog log,
            string message,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Error)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(message, memberName, sourceFilePath, sourceLineNumber) : message;
            log.Log(LogLevel.Error, newFormat);
        }
 
        public static void Error(
            this ILog log,
            Exception ex,
            string message,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Error)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(message, memberName, sourceFilePath, sourceLineNumber) : message;
            log.Log(LogLevel.Error, ex, newFormat);
        }
 
        public static void Error(
            this ILog log,
            string format,
            object arg0,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Error)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Error, newFormat, arg0);
        }
 
        public static void Error(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Error)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Error, ex, newFormat, arg0);
        }
 
        public static void Error(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Error)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Error, newFormat, arg0, arg1);
        }
 
        public static void Error(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Error)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Error, ex, newFormat, arg0, arg1);
        }
 
        public static void Error(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Error)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Error, newFormat, arg0, arg1, arg2);
        }
 
        public static void Error(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Error)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Error, ex, newFormat, arg0, arg1, arg2);
        }
 
        public static void Error(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Error)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Error, newFormat, arg0, arg1, arg2, arg3);
        }
 
        public static void Error(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Error)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Error, ex, newFormat, arg0, arg1, arg2, arg3);
        }
 
        public static void Error(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Error)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Error, newFormat, arg0, arg1, arg2, arg3, arg4);
        }
 
        public static void Error(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Error)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Error, ex, newFormat, arg0, arg1, arg2, arg3, arg4);
        }
 
        public static void Error(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Error)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Error, newFormat, arg0, arg1, arg2, arg3, arg4, arg5);
        }
 
        public static void Error(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Error)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Error, ex, newFormat, arg0, arg1, arg2, arg3, arg4, arg5);
        }
 
        public static void Error(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Error)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Error, newFormat, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }
 
        public static void Error(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Error)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Error, ex, newFormat, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }
 
        public static void Error(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            object arg7,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Error)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Error, newFormat, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
 
        public static void Error(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            object arg7,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Error)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Error, ex, newFormat, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
 
        public static void Fatal(
            this ILog log,
            string message,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Fatal)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(message, memberName, sourceFilePath, sourceLineNumber) : message;
            log.Log(LogLevel.Fatal, newFormat);
        }
 
        public static void Fatal(
            this ILog log,
            Exception ex,
            string message,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Fatal)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(message, memberName, sourceFilePath, sourceLineNumber) : message;
            log.Log(LogLevel.Fatal, ex, newFormat);
        }
 
        public static void Fatal(
            this ILog log,
            string format,
            object arg0,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Fatal)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Fatal, newFormat, arg0);
        }
 
        public static void Fatal(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Fatal)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Fatal, ex, newFormat, arg0);
        }
 
        public static void Fatal(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Fatal)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Fatal, newFormat, arg0, arg1);
        }
 
        public static void Fatal(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Fatal)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Fatal, ex, newFormat, arg0, arg1);
        }
 
        public static void Fatal(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Fatal)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Fatal, newFormat, arg0, arg1, arg2);
        }
 
        public static void Fatal(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Fatal)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Fatal, ex, newFormat, arg0, arg1, arg2);
        }
 
        public static void Fatal(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Fatal)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Fatal, newFormat, arg0, arg1, arg2, arg3);
        }
 
        public static void Fatal(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Fatal)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Fatal, ex, newFormat, arg0, arg1, arg2, arg3);
        }
 
        public static void Fatal(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Fatal)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Fatal, newFormat, arg0, arg1, arg2, arg3, arg4);
        }
 
        public static void Fatal(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Fatal)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Fatal, ex, newFormat, arg0, arg1, arg2, arg3, arg4);
        }
 
        public static void Fatal(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Fatal)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Fatal, newFormat, arg0, arg1, arg2, arg3, arg4, arg5);
        }
 
        public static void Fatal(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Fatal)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Fatal, ex, newFormat, arg0, arg1, arg2, arg3, arg4, arg5);
        }
 
        public static void Fatal(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Fatal)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Fatal, newFormat, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }
 
        public static void Fatal(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Fatal)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Fatal, ex, newFormat, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }
 
        public static void Fatal(
            this ILog log,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            object arg7,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Fatal)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Fatal, newFormat, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
 
        public static void Fatal(
            this ILog log,
            Exception ex,
            string format,
            object arg0,
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            object arg7,
            LogCallSite logCallSite = default(LogCallSite),
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!log.IsEnabled(LogLevel.Fatal)) return;
            var newFormat = logCallSite == LogCallSite.Enabled ? GetFormat(format, memberName, sourceFilePath, sourceLineNumber) : format;
            log.Log(LogLevel.Fatal, ex, newFormat, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetFormat(string format, string memberName, string sourceFilePath, int sourceLineNumber)
        {
            var lastIndex = sourceFilePath.LastIndexOfAny(DirectorySeparatorChars);
            if (lastIndex > 0) sourceFilePath = sourceFilePath.Substring(lastIndex + 1);
            var match = ClassNameRegex.Match(sourceFilePath);
            return match.Success
                ? $"{match.Groups[1].Value}.{memberName}({sourceLineNumber}) - {format}"
                : format;
        }
 
        public enum LogCallSite
        {
            Enabled = 0,
            Disabled = 1
        }
    }
}