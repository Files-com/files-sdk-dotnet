using System;
using System.Threading;

namespace FilesCom.Http
{
    // What one request is waiting for from the network, and so which timeout limits the wait.
    internal enum NetworkWait
    {
        // To connect and send the request, with each write of an upload's body: WriteTimeout.
        Sending,

        // The response's status and headers, once the request has been sent: ReadTimeout.
        ResponseHeaders,

        // Each read of the response's body: ReadTimeout.
        ResponseBody,
    }

    // Ends one request attempt when a network wait lasts longer than its timeout. The time between waits, when the
    // request is reading an upload's source or writing a download to its destination, is not timed, so neither a slow
    // disk nor a transfer that takes long but keeps moving is taken for a stalled connection; a caller's stream that
    // blocks is stopped only by cancellation. Token is given to every network call of the attempt: it is cancelled
    // when a wait runs out or when the operation is cancelled, and TimedOut tells the two apart.
    //
    // Waits are timed on a CancellationTokenSource timer, which counts elapsed time, not the clock.
    internal sealed class NetworkWatch : IDisposable
    {
        private readonly object gate = new object();
        private readonly NetworkSettings settings;
        private readonly CancellationToken operation;
        private readonly CancellationTokenSource source;
        private NetworkWait wait;
        private bool disposed;

        internal NetworkWatch(NetworkSettings settings, CancellationToken operation)
        {
            this.settings = settings;
            this.operation = operation;
            source = CancellationTokenSource.CreateLinkedTokenSource(operation);
        }

        internal CancellationToken Token => source.Token;

        // Starts timing wait, from now.
        internal void Begin(NetworkWait wait)
        {
            lock (gate)
            {
                this.wait = wait;
                Restart(wait == NetworkWait.Sending ? settings.WriteTimeout : settings.ReadTimeout);
            }
        }

        // Stops timing until the next Begin or WriteStarted.
        internal void Pause()
        {
            lock (gate)
            {
                Restart(System.Threading.Timeout.InfiniteTimeSpan);
            }
        }

        // The request body starts to be written: from now until BodySent, only its writes are timed.
        internal void SendingBody()
        {
            lock (gate)
            {
                wait = NetworkWait.Sending;
                Restart(System.Threading.Timeout.InfiniteTimeSpan);
            }
        }

        // A write of the request body starts, or ends: while the request is sending, only writes are timed. A write
        // that ends after the response has started to arrive, as HTTP allows, changes nothing.
        internal void WriteStarted()
        {
            lock (gate)
            {
                if (wait == NetworkWait.Sending)
                {
                    Restart(settings.WriteTimeout);
                }
            }
        }

        internal void WriteEnded()
        {
            lock (gate)
            {
                if (wait == NetworkWait.Sending)
                {
                    Restart(System.Threading.Timeout.InfiniteTimeSpan);
                }
            }
        }

        // The whole request body has been written; now the response is awaited.
        internal void BodySent()
        {
            lock (gate)
            {
                if (wait == NetworkWait.Sending)
                {
                    wait = NetworkWait.ResponseHeaders;
                    Restart(settings.ReadTimeout);
                }
            }
        }

        // Whether cancellation of Token came from a wait running out, or the handler's own ConnectTimeout, rather
        // than from the operation.
        internal bool TimedOut(OperationCanceledException cancellation)
        {
            return !operation.IsCancellationRequested && (source.IsCancellationRequested || cancellation?.InnerException is TimeoutException);
        }

        // The failure for a wait that ran out, or, when none did, for the handler's ConnectTimeout.
        internal NetworkTimeoutException Timeout()
        {
            if (!source.IsCancellationRequested)
            {
                return new NetworkTimeoutException("Could not connect to the server within its connect timeout (ConnectTimeout).") { BeforeSending = true };
            }
            lock (gate)
            {
                switch (wait)
                {
                    case NetworkWait.Sending:
                        return new NetworkTimeoutException($"The request stopped sending: no data could be sent for {Seconds(settings.WriteTimeout)} (WriteTimeout).");
                    case NetworkWait.ResponseHeaders:
                        return new NetworkTimeoutException($"No response arrived within {Seconds(settings.ReadTimeout)} (ReadTimeout).");
                    default:
                        return new NetworkTimeoutException($"The response stopped arriving: no data arrived for {Seconds(settings.ReadTimeout)} (ReadTimeout).");
                }
            }
        }

        // The failure for a network call ended by Token, other than by the OperationCanceledException that .NET
        // throws: the operation's cancellation, or a timeout.
        internal Exception Stopped()
        {
            return operation.IsCancellationRequested ? new OperationCanceledException(operation) : (Exception)Timeout();
        }

        public void Dispose()
        {
            lock (gate)
            {
                disposed = true;
                source.Dispose();
            }
        }

        // Called under gate. The HTTP stack may still write after the attempt has ended, so a disposed watch
        // ignores it.
        private void Restart(TimeSpan timeout)
        {
            if (!disposed)
            {
                source.CancelAfter(timeout);
            }
        }

        private static string Seconds(TimeSpan time)
        {
            return time.TotalSeconds == 1 ? "1 second" : $"{time.TotalSeconds} seconds";
        }
    }
}