using System;

namespace FilesCom.Http
{
    // The retry and timeout settings one operation runs with, read from its client's configuration when it starts and
    // checked then, so that a value no request could use fails before anything is sent.
    internal sealed class NetworkSettings
    {
        // The longest timeout or delay the timers of every supported runtime can wait: Int32.MaxValue milliseconds,
        // about 24.8 days.
        private const double LongestWaitSeconds = int.MaxValue / 1000d;

        private NetworkSettings(int maxNetworkRetries, TimeSpan initialRetryDelay, TimeSpan maxRetryDelay, TimeSpan readTimeout, TimeSpan writeTimeout)
        {
            MaxNetworkRetries = maxNetworkRetries;
            InitialRetryDelay = initialRetryDelay;
            MaxRetryDelay = maxRetryDelay;
            ReadTimeout = readTimeout;
            WriteTimeout = writeTimeout;
        }

        internal int MaxNetworkRetries { get; }

        internal TimeSpan InitialRetryDelay { get; }

        internal TimeSpan MaxRetryDelay { get; }

        internal TimeSpan ReadTimeout { get; }

        internal TimeSpan WriteTimeout { get; }

        internal static NetworkSettings From(FilesConfiguration configuration)
        {
            if (configuration.MaxNetworkRetries < 0)
            {
                throw InvalidParameterException.OutOfRange(nameof(FilesConfiguration.MaxNetworkRetries), configuration.MaxNetworkRetries, "MaxNetworkRetries cannot be negative. Use 0 to send each request once");
            }
            return new NetworkSettings(
                configuration.MaxNetworkRetries,
                Delay(nameof(FilesConfiguration.InitialNetworkRequestDelay), configuration.InitialNetworkRequestDelay),
                Delay(nameof(FilesConfiguration.MaxNetworkRetryDelay), configuration.MaxNetworkRetryDelay),
                Timeout(nameof(FilesConfiguration.ReadTimeout), configuration.ReadTimeout),
                Timeout(nameof(FilesConfiguration.WriteTimeout), configuration.WriteTimeout));
        }

        // The same settings, for a request that must be sent at most once.
        internal NetworkSettings WithoutRetries()
        {
            return new NetworkSettings(0, InitialRetryDelay, MaxRetryDelay, ReadTimeout, WriteTimeout);
        }

        // A timeout setting, in whole seconds, as a TimeSpan: at least one second, and within the timers' range.
        internal static TimeSpan Timeout(string name, int seconds)
        {
            if (seconds < 1 || seconds > LongestWaitSeconds)
            {
                throw InvalidParameterException.OutOfRange(name, seconds, $"{name} must be from 1 to {(int)LongestWaitSeconds} seconds");
            }
            return TimeSpan.FromSeconds(seconds);
        }

        private static TimeSpan Delay(string name, double seconds)
        {
            if (double.IsNaN(seconds) || seconds < 0 || seconds > LongestWaitSeconds)
            {
                throw InvalidParameterException.OutOfRange(name, seconds, $"{name} must be from 0 to {LongestWaitSeconds} seconds");
            }
            return TimeSpan.FromSeconds(seconds);
        }
    }
}