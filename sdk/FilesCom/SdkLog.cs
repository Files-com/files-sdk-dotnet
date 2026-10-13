using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace FilesCom
{
    // The SDK's logger for one class, which it uses in place of a log4net logger. A message goes where the client of the
    // operation logging it says (ClientLogging): to the application's ILoggerFactory, under the class's name as its
    // category, when the client was given one, and otherwise to log4net, as it always has, whose configuration is the
    // application's. The client's MinimumLogLevel, when set, drops less severe messages either way. The client is the
    // one whose operation is running: an operation's scope (OperationContext.Begin) makes its client's logging the
    // current one for everything that runs within it, its own tasks included; outside any operation, messages go to
    // log4net.
    //
    // Logging never changes what an operation does: a factory, IsEnabled or Log call of the application's that throws
    // loses that message, and nothing else. In particular it cannot take the place of the failure an operation reports,
    // as when a cleanup failure is logged on the way to the caller's own exception.
    internal sealed class SdkLog
    {
        private static readonly AsyncLocal<ClientLogging> current = new AsyncLocal<ClientLogging>();

        private readonly log4net.ILog log4Net;
        private readonly string category;

        private SdkLog(Type type)
        {
            log4Net = log4net.LogManager.GetLogger(type);
            category = type.FullName;
        }

        internal static SdkLog For(Type type)
        {
            return new SdkLog(type);
        }

        // Makes logging the current logging until the returned scope is disposed.
        internal static IDisposable Use(ClientLogging logging)
        {
            ClientLogging previous = current.Value;
            current.Value = logging;
            return new Restore(previous);
        }

        internal bool IsDebugEnabled => Enabled(LogLevel.Debug);

        internal void Debug(object message, Exception exception = null)
        {
            Write(LogLevel.Debug, message, exception);
        }

        internal void Info(object message, Exception exception = null)
        {
            Write(LogLevel.Information, message, exception);
        }

        internal void Warn(object message, Exception exception = null)
        {
            Write(LogLevel.Warning, message, exception);
        }

        internal void Error(object message, Exception exception = null)
        {
            Write(LogLevel.Error, message, exception);
        }

        private bool Enabled(LogLevel level)
        {
            ClientLogging logging = current.Value;
            if (logging != null && logging.MinimumLevel.HasValue && level < logging.MinimumLevel.Value)
            {
                return false;
            }
            try
            {
                if (logging?.Factory != null)
                {
                    return logging.Logger(category).IsEnabled(level);
                }
                switch (level)
                {
                    case LogLevel.Debug:
                        return log4Net.IsDebugEnabled;
                    case LogLevel.Information:
                        return log4Net.IsInfoEnabled;
                    case LogLevel.Warning:
                        return log4Net.IsWarnEnabled;
                    default:
                        return log4Net.IsErrorEnabled;
                }
            }
            catch (Exception)
            {
                // A logger that cannot say is taken as disabled.
                return false;
            }
        }

        private void Write(LogLevel level, object message, Exception exception)
        {
            ClientLogging logging = current.Value;
            if (logging != null && logging.MinimumLevel.HasValue && level < logging.MinimumLevel.Value)
            {
                return;
            }
            try
            {
                if (logging?.Factory != null)
                {
                    ILogger logger = logging.Logger(category);
                    if (logger.IsEnabled(level))
                    {
                        // The text as it is, not a message template: it is never parsed for placeholders.
                        logger.Log(level, default(EventId), message?.ToString() ?? "", exception, (text, failure) => text);
                    }
                    return;
                }
                switch (level)
                {
                    case LogLevel.Debug:
                        log4Net.Debug(message, exception);
                        break;
                    case LogLevel.Information:
                        log4Net.Info(message, exception);
                        break;
                    case LogLevel.Warning:
                        log4Net.Warn(message, exception);
                        break;
                    default:
                        log4Net.Error(message, exception);
                        break;
                }
            }
            catch (Exception)
            {
                // The logger's own failure: the message is lost, and the operation logging it goes on as it would have.
            }
        }

        private sealed class Restore : IDisposable
        {
            private readonly ClientLogging previous;
            private int disposed;

            internal Restore(ClientLogging previous)
            {
                this.previous = previous;
            }

            public void Dispose()
            {
                if (Interlocked.Exchange(ref disposed, 1) == 0)
                {
                    current.Value = previous;
                }
            }
        }
    }

    // One client's logging, as it was configured when the client was made: the application's logger factory, or null for
    // log4net, and the least severe level logged, or null for whatever the logger enables.
    internal sealed class ClientLogging
    {
        private readonly ConcurrentDictionary<string, ILogger> loggers = new ConcurrentDictionary<string, ILogger>(StringComparer.Ordinal);

        internal ClientLogging(ILoggerFactory factory, LogLevel? minimumLevel)
        {
            Factory = factory;
            MinimumLevel = minimumLevel;
        }

        internal ILoggerFactory Factory { get; }

        internal LogLevel? MinimumLevel { get; }

        internal ILogger Logger(string category)
        {
            return loggers.GetOrAdd(category, name => Factory.CreateLogger(name));
        }
    }
}