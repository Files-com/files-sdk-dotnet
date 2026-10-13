using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Transfers
{
    // Bytes passed from a writer to a reader through a bounded buffer, in order: a write waits while the buffer is full,
    // a read while it is empty. The writer ends the bytes (Complete), with or without a failure, which the reader receives
    // once it has read everything before it; the reader leaving (Leave) makes later writes fail. It connects a transfer
    // that writes into or reads from a stream with a caller that reads or writes at its own pace (RemoteFileSystem).
    internal sealed class TransferPipe
    {
        private readonly object gate = new object();
        private readonly Queue<byte[]> chunks = new Queue<byte[]>();
        private readonly int capacity;
        private byte[] current;
        private int currentOffset;
        private long buffered;
        private bool completed;
        private bool left;
        private ExceptionDispatchInfo failure;
        private TaskCompletionSource<bool> changed = NewSignal();

        internal TransferPipe(int capacity)
        {
            this.capacity = capacity;
        }

        // Whether anything is waiting to be read, or the bytes have ended.
        internal bool Readable
        {
            get
            {
                lock (gate)
                {
                    return buffered > 0 || completed;
                }
            }
        }

        // Waits until something can be read, or the bytes have ended.
        internal Task WaitUntilReadableAsync(CancellationToken cancellationToken)
        {
            return WaitAsync(() => buffered > 0 || completed, cancellationToken);
        }

        internal async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            while (count > 0)
            {
                await WaitAsync(() => left || buffered < capacity, cancellationToken);
                lock (gate)
                {
                    if (left)
                    {
                        throw new IOException("The reader of the bytes has closed its stream.");
                    }
                    if (completed)
                    {
                        throw new InvalidOperationException("The bytes have ended.");
                    }
                    int taken = (int)Math.Min(count, capacity - buffered);
                    var chunk = new byte[taken];
                    Buffer.BlockCopy(buffer, offset, chunk, 0, taken);
                    chunks.Enqueue(chunk);
                    buffered += taken;
                    offset += taken;
                    count -= taken;
                    Signal();
                }
            }
        }

        // Reads up to count bytes: 0 once the bytes have ended, or the writer's failure.
        internal async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (count == 0)
            {
                return 0;
            }
            await WaitUntilReadableAsync(cancellationToken);
            lock (gate)
            {
                if (buffered == 0)
                {
                    failure?.Throw();
                    return 0;
                }
                int read = 0;
                while (read < count && buffered > 0)
                {
                    if (current == null)
                    {
                        current = chunks.Dequeue();
                        currentOffset = 0;
                    }
                    int taken = Math.Min(count - read, current.Length - currentOffset);
                    Buffer.BlockCopy(current, currentOffset, buffer, offset + read, taken);
                    currentOffset += taken;
                    read += taken;
                    buffered -= taken;
                    if (currentOffset == current.Length)
                    {
                        current = null;
                    }
                }
                Signal();
                return read;
            }
        }

        // Throws the writer's failure if it ended the bytes with one before writing anything.
        internal void ThrowIfFailedEmpty()
        {
            lock (gate)
            {
                if (completed && buffered == 0)
                {
                    failure?.Throw();
                }
            }
        }

        // Ends the bytes: the reader reads what was written, then reaches the end, or failure if there is one.
        internal void Complete(Exception failure = null)
        {
            lock (gate)
            {
                if (completed)
                {
                    return;
                }
                completed = true;
                this.failure = failure == null ? null : ExceptionDispatchInfo.Capture(failure);
                Signal();
            }
        }

        // The reader has gone: later writes fail, and what was buffered is dropped.
        internal void Leave()
        {
            lock (gate)
            {
                left = true;
                chunks.Clear();
                current = null;
                buffered = 0;
                Signal();
            }
        }

        private async Task WaitAsync(Func<bool> ready, CancellationToken cancellationToken)
        {
            while (true)
            {
                Task signal;
                lock (gate)
                {
                    if (ready())
                    {
                        return;
                    }
                    signal = changed.Task;
                }
                cancellationToken.ThrowIfCancellationRequested();
                using (cancellationToken.Register(() => WakeOnCancel(signal)))
                {
                    await signal.ConfigureAwait(false);
                }
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        // Wakes a wait for signal when its token is cancelled; every waiter then checks again.
        private void WakeOnCancel(Task signal)
        {
            lock (gate)
            {
                if (changed.Task == signal)
                {
                    Signal();
                }
            }
        }

        // Wakes every waiter; called with gate held.
        private void Signal()
        {
            TaskCompletionSource<bool> woken = changed;
            changed = NewSignal();
            woken.TrySetResult(true);
        }

        private static TaskCompletionSource<bool> NewSignal()
        {
            return new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        }
    }
}