using FilesCom.Util;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom
{
    // Whether a client is open or closed, and how many of its operations are running. An operation is admitted while
    // the client is open and holds it until the operation ends: an upload until its last part and final request, a
    // download until its last byte is written, a page until it is loaded, and each of them until the responses, files
    // and streams the SDK holds for it are let go. Nothing else holds it, so a model, list or enumerator that is not
    // loading anything never delays closing. What an operation's own code does after that, such as returning the
    // object it read or the cancellation, sends and holds nothing, so its task can complete after closing has.
    //
    // Closing happens once, however many times it is asked for and from however many threads: it admits no new
    // operation, cancels the ones running through Closing, waits for them to end, then releases what the client owns.
    // The cancellation and the release run on the thread pool, never in the thread that asked, so asking cannot block,
    // even from a callback of an operation being cancelled.
    internal sealed class ClientLifetime
    {
        private static readonly SdkLog log = SdkLog.For(typeof(ClientLifetime));

        private readonly object gate = new object();
        private readonly CancellationTokenSource closing = new CancellationTokenSource();
        private readonly TaskCompletionSource<bool> operationsEnded = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        private int running;
        private Task closed;

        // Cancelled when the client starts closing.
        internal CancellationToken Closing => closing.Token;

        // How many operations hold the client open now.
        internal int RunningOperations
        {
            get
            {
                lock (gate)
                {
                    return running;
                }
            }
        }

        internal bool IsClosing
        {
            get
            {
                lock (gate)
                {
                    return closed != null;
                }
            }
        }

        // Admits an operation, which must call End when it ends.
        internal void Begin()
        {
            lock (gate)
            {
                if (closed != null)
                {
                    throw Disposed();
                }
                running++;
            }
        }

        internal void End()
        {
            lock (gate)
            {
                if (--running == 0 && closed != null)
                {
                    operationsEnded.TrySetResult(true);
                }
            }
        }

        // Closes the client, then runs release. Every call returns the same task, which never fails: a failure to
        // cancel or release is logged instead, so that no caller of Dispose leaves an exception unobserved.
        internal Task CloseAsync(Func<Task> release)
        {
            lock (gate)
            {
                if (closed == null)
                {
                    if (running == 0)
                    {
                        operationsEnded.TrySetResult(true);
                    }
                    closed = Task.Run(() => CloseAndReleaseAsync(release));
                }
                return closed;
            }
        }

        internal static ObjectDisposedException Disposed()
        {
            return new ObjectDisposedException(nameof(FilesClient), "The client has been disposed. Make a new client for further requests.");
        }

        private async Task CloseAndReleaseAsync(Func<Task> release)
        {
            try
            {
                closing.Cancel();
            }
            catch (AggregateException e)
            {
                DiagnosticText.WarnCleanupFailed(log, "Closing a client: cancelling its operations failed", e);
            }
            await operationsEnded.Task;
            try
            {
                await release();
            }
            catch (Exception e)
            {
                DiagnosticText.WarnCleanupFailed(log, "Closing a client: releasing its connections failed", e);
            }
        }
    }
}