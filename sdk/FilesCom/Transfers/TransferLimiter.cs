using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Transfers
{
    /// <summary>
    /// Limits the resources that transfers use together: how many part requests they send at once, and how many
    /// bytes of file contents they hold in memory. Give the same limiter to several uploads, including uploads made
    /// with different clients, to have them share one budget.
    /// </summary>
    /// <remarks>
    /// Uploads receive request slots and buffer space in the order they ask for them, and each upload asks for the
    /// next slot for a new part only once its previous one has been granted, so uploads sharing a limiter take turns
    /// instead of one using all of it. A request slot is held for one request: a part waiting to be sent again gives
    /// its slot back and asks again. An automatic upload whose requests wait here for other transfers does not treat
    /// that as a reason to send more at once. A limiter holds no credentials or connections: each upload sends its
    /// requests with its own client. The properties that describe current use are cheap to read at any time.
    /// </remarks>
    public sealed class TransferLimiter
    {
        /// <summary>The number of concurrent part requests a limiter allows unless another is given.</summary>
        public const int DefaultMaxConcurrentRequests = 8;

        /// <summary>The number of buffered bytes a limiter allows unless another is given: 128 MiB.</summary>
        public const long DefaultMaxBufferedBytes = 128L * 1024 * 1024;

        private readonly FifoBudget requests;
        private readonly FifoBudget bufferedBytes;

        /// <summary>
        /// Makes a limiter.
        /// </summary>
        /// <param name="maxConcurrentRequests">The most part requests its transfers send at the same time.</param>
        /// <param name="maxBufferedBytes">
        /// The most bytes of file contents its transfers hold in memory at the same time: whole parts read from
        /// streams, which are kept until the server accepts them so they can be resent, and the copy buffers of parts
        /// read from files.
        /// </param>
        /// <exception cref="InvalidParameterException">A limit is less than 1.</exception>
        public TransferLimiter(int maxConcurrentRequests = DefaultMaxConcurrentRequests, long maxBufferedBytes = DefaultMaxBufferedBytes)
        {
            if (maxConcurrentRequests < 1)
            {
                throw InvalidParameterException.OutOfRange(nameof(maxConcurrentRequests), maxConcurrentRequests, "A transfer limiter must allow at least one request");
            }
            if (maxBufferedBytes < 1)
            {
                throw InvalidParameterException.OutOfRange(nameof(maxBufferedBytes), maxBufferedBytes, "A transfer limiter must allow at least one buffered byte");
            }
            requests = new FifoBudget(maxConcurrentRequests);
            bufferedBytes = new FifoBudget(maxBufferedBytes);
        }

        /// <summary>The most part requests this limiter's transfers send at the same time.</summary>
        public int MaxConcurrentRequests => (int)requests.Capacity;

        /// <summary>The most bytes of file contents this limiter's transfers hold in memory at the same time.</summary>
        public long MaxBufferedBytes => bufferedBytes.Capacity;

        /// <summary>The number of part requests being sent now.</summary>
        public int ActiveRequests => (int)requests.InUse;

        /// <summary>The number of bytes of file contents held now.</summary>
        public long BufferedBytes => bufferedBytes.InUse;

        /// <summary>The number of part requests waiting for a request slot.</summary>
        public int QueuedRequests => requests.Waiting;

        /// <summary>The number of parts waiting for buffer space.</summary>
        public int QueuedBufferReservations => bufferedBytes.Waiting;

        // A slot for sending one part request. Dispose it when the request has ended, before any retry waits.
        internal Task<IDisposable> AcquireRequestAsync(CancellationToken cancellationToken)
        {
            return requests.AcquireAsync(1, cancellationToken);
        }

        // Space for holding bytes of file contents. Dispose it when the bytes are no longer needed.
        internal Task<IDisposable> ReserveBufferAsync(long bytes, CancellationToken cancellationToken)
        {
            return bufferedBytes.AcquireAsync(bytes, cancellationToken);
        }
    }

    // An amount of something (request slots, bytes) handed out first come, first served: a request waits while an
    // earlier one is waiting, even if the amount it wants is free, so a large request is never starved by a stream of
    // small ones. Waits can be cancelled; a cancelled wait takes nothing.
    internal sealed class FifoBudget
    {
        private readonly object gate = new object();
        private readonly LinkedList<Waiter> waiters = new LinkedList<Waiter>();
        private long available;

        internal FifoBudget(long capacity)
        {
            Capacity = capacity;
            available = capacity;
        }

        internal long Capacity { get; }

        internal long InUse
        {
            get
            {
                lock (gate)
                {
                    return Capacity - available;
                }
            }
        }

        internal int Waiting
        {
            get
            {
                lock (gate)
                {
                    return waiters.Count;
                }
            }
        }

        // Completes with a lease on amount once it is this request's turn and the amount is free. Disposing the lease
        // gives the amount back.
        internal async Task<IDisposable> AcquireAsync(long amount, CancellationToken cancellationToken)
        {
            if (amount < 0 || amount > Capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, $"Can take from 0 to {Capacity}.");
            }
            cancellationToken.ThrowIfCancellationRequested();

            Waiter waiter;
            lock (gate)
            {
                if (amount == 0 || (waiters.Count == 0 && available >= amount))
                {
                    available -= amount;
                    return new Lease(this, amount);
                }
                waiter = new Waiter(amount);
                waiter.Node = waiters.AddLast(waiter);
            }
            // Disposed only after the wait ends, outside the lock, so it never waits on a Cancel that needs the lock.
            using (cancellationToken.Register(() => Cancel(waiter, cancellationToken)))
            {
                return await waiter.Completion.Task;
            }
        }

        private void Release(long amount)
        {
            lock (gate)
            {
                available += amount;
            }
            GrantWaiters();
        }

        private void Cancel(Waiter waiter, CancellationToken cancellationToken)
        {
            lock (gate)
            {
                if (waiter.Node == null)
                {
                    return; // Already granted.
                }
                waiters.Remove(waiter.Node);
                waiter.Node = null;
            }
            waiter.Completion.TrySetCanceled(cancellationToken);
            // The cancelled waiter may have been holding up the ones behind it.
            GrantWaiters();
        }

        private void GrantWaiters()
        {
            var granted = new List<Waiter>();
            lock (gate)
            {
                while (waiters.First != null && waiters.First.Value.Amount <= available)
                {
                    Waiter waiter = waiters.First.Value;
                    waiters.RemoveFirst();
                    waiter.Node = null;
                    available -= waiter.Amount;
                    granted.Add(waiter);
                }
            }
            foreach (Waiter waiter in granted)
            {
                waiter.Completion.TrySetResult(new Lease(this, waiter.Amount));
            }
        }

        private sealed class Waiter
        {
            internal Waiter(long amount)
            {
                Amount = amount;
            }

            internal long Amount { get; }

            // Continuations run on the thread pool, not inside whoever releases the budget.
            internal TaskCompletionSource<IDisposable> Completion { get; } = new TaskCompletionSource<IDisposable>(TaskCreationOptions.RunContinuationsAsynchronously);

            // In the queue while not null. Guarded by the budget's gate.
            internal LinkedListNode<Waiter> Node { get; set; }
        }

        private sealed class Lease : IDisposable
        {
            private readonly FifoBudget budget;
            private readonly long amount;
            private int disposed;

            internal Lease(FifoBudget budget, long amount)
            {
                this.budget = budget;
                this.amount = amount;
            }

            public void Dispose()
            {
                if (Interlocked.Exchange(ref disposed, 1) == 0 && amount > 0)
                {
                    budget.Release(amount);
                }
            }
        }
    }
}