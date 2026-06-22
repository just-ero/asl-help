using System;
using System.Runtime.CompilerServices;

namespace AslHelp.Logging;

/// <summary>
///     Provides extension members for <see cref="Logger"/>.
/// </summary>
public static class LoggerExtensions
{
    extension(Logger logger)
    {
        /// <summary>Logs a message at the <see cref="LogLevel.Trace"/> level.</summary>
        public void LogTrace(
            string message,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            Log(logger, LogLevel.Trace, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>Logs a message at the <see cref="LogLevel.Debug"/> level.</summary>
        public void LogDebug(
            string message,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            Log(logger, LogLevel.Debug, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>Logs a message at the <see cref="LogLevel.Information"/> level.</summary>
        public void LogInformation(
            string message,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            Log(logger, LogLevel.Information, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>Logs a message at the <see cref="LogLevel.Warning"/> level.</summary>
        public void LogWarning(
            string message,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            Log(logger, LogLevel.Warning, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>Logs a message at the <see cref="LogLevel.Error"/> level.</summary>
        public void LogError(
            string message,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            Log(logger, LogLevel.Error, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>Logs a message at the <see cref="LogLevel.Critical"/> level.</summary>
        public void LogCritical(
            string message,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            Log(logger, LogLevel.Critical, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>Logs a header at <see cref="LogLevel.Trace"/> and opens an indented scope.</summary>
        public IDisposable BeginScopeTrace(
            string message,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return Scope(logger, LogLevel.Trace, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>Logs a header at <see cref="LogLevel.Debug"/> and opens an indented scope.</summary>
        public IDisposable BeginScopeDebug(
            string message,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return Scope(logger, LogLevel.Debug, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>Logs a header at <see cref="LogLevel.Information"/> and opens an indented scope.</summary>
        public IDisposable BeginScopeInformation(
            string message,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return Scope(logger, LogLevel.Information, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>Logs a header at <see cref="LogLevel.Warning"/> and opens an indented scope.</summary>
        public IDisposable BeginScopeWarning(
            string message,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return Scope(logger, LogLevel.Warning, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>Logs a header at <see cref="LogLevel.Error"/> and opens an indented scope.</summary>
        public IDisposable BeginScopeError(
            string message,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return Scope(logger, LogLevel.Error, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>Logs a header at <see cref="LogLevel.Critical"/> and opens an indented scope.</summary>
        public IDisposable BeginScopeCritical(
            string message,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return Scope(logger, LogLevel.Critical, message, callerMemberName, callerFilePath, callerLineNumber);
        }
    }

    private static void Log(
        Logger logger,
        LogLevel level,
        string message,
        string callerMemberName,
        string callerFilePath,
        int callerLineNumber)
    {
        logger.Log(new LogEvent(level, message, callerMemberName, callerFilePath, callerLineNumber));
    }

    private static IndentScope Scope(
        Logger logger,
        LogLevel level,
        string message,
        string callerMemberName,
        string callerFilePath,
        int callerLineNumber)
    {
        Log(logger, level, message, callerMemberName, callerFilePath, callerLineNumber);
        return logger.BeginScope(level);
    }
}
