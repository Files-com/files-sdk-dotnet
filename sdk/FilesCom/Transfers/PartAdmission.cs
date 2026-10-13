using System;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Transfers
{
    // How many parts of one upload may be in flight at once: up to its concurrency while the server allows parallel
    // parts, and one at a time, in order, from the first part it does not. Parts are admitted in order by a single
    // caller. The limiter's shared request slots are separate and are taken after admission.
    internal sealed class PartAdmission
    {
        private readonly int concurrency;
        private readonly SemaphoreSlim slots;

        internal PartAdmission(int concurrency, bool parallel)
        {
            this.concurrency = parallel ? concurrency : 1;
            slots = new SemaphoreSlim(this.concurrency, this.concurrency);
            Parallel = parallel;
        }

        internal bool Parallel { get; private set; }

        internal int Limit => Parallel ? concurrency : 1;

        // Waits until the part may be sent. Dispose the result once the part will not be sent again.
        //
        // A parallel part takes one slot. A serial part takes every slot, which waits for the parts in flight and holds
        // back later parts until it is done. Once a part is serial, so is the rest of the upload. Only the dispatcher,
        // one part at a time, calls this, so two parts never each hold some slots while waiting for the rest.
        internal async Task<Admitted> AdmitAsync(PartTarget part, CancellationToken cancellationToken)
        {
            Parallel &= part.ParallelParts;
            int permits = Parallel ? 1 : concurrency;
            int taken = 0;
            try
            {
                for (; taken < permits; taken++)
                {
                    await slots.WaitAsync(cancellationToken);
                }
            }
            catch
            {
                if (taken > 0)
                {
                    slots.Release(taken);
                }
                throw;
            }
            return new Admitted(slots, permits, alone: permits == concurrency);
        }

        internal sealed class Admitted : IDisposable
        {
            private readonly SemaphoreSlim slots;
            private readonly int permits;
            private int disposed;

            internal Admitted(SemaphoreSlim slots, int permits, bool alone)
            {
                this.slots = slots;
                this.permits = permits;
                Alone = alone;
            }

            // Whether the part holds every slot, so no other part of the upload is sent while it is.
            internal bool Alone { get; }

            public void Dispose()
            {
                if (Interlocked.Exchange(ref disposed, 1) == 0)
                {
                    slots.Release(permits);
                }
            }
        }
    }
}