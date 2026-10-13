using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Transfers
{
    // Admits the network attempts of one upload, or of the downloads sharing a scope (TransferScope), first attempts and
    // retries alike: at most the controller's target at once, and none while the controller pauses after throttling.
    // Waiting attempts are admitted in the order they asked, except that retries of parts already under way go before
    // new parts, so a part holding a replay buffer, or buffer space, is never kept waiting behind parts that took new
    // ones. A waiting attempt can be cancelled; a cancelled wait takes nothing.
    //
    // An admitted attempt still has to get a request slot from the shared limiter, and perhaps a renewed URL, before
    // it is sent; it holds its permit meanwhile, which is how the controller tells the limiter's other users from a
    // shortage of work. Those waits can outlast what admitted it: a throttle may start a cooldown, or a failure lower
    // the target, while it waits. So an attempt starts only through TryStart, which checks the cooldown and the target
    // again at the moment it would be sent; an attempt that may no longer start gives back its permit and its request
    // slot, rather than holding a slot through the pause, and asks again. The congestion epoch an attempt belongs to is
    // the one it starts in. An attempt gives its permit back when its request ends, before any retry delay, so a part
    // waiting to retry takes no capacity. Every state change reaches the controller under one lock, with the time it
    // happened.
    internal sealed class AttemptAdmission : IDisposable
    {
        private readonly object gate = new object();
        private readonly ConcurrencyController controller;
        private readonly LinkedList<Waiter> waiters = new LinkedList<Waiter>();
        private readonly bool tracing;
        private Timer resume;
        private bool disposed;

        internal AttemptAdmission(ConcurrencyController controller, bool tracing)
        {
            this.controller = controller;
            this.tracing = tracing;
        }

        // Completes with an attempt once one may be sent. Start it with TryStart; Dispose it, or Complete it, when its
        // request has ended or it will not be sent.
        internal async Task<Attempt> AdmitAsync(bool retry, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Waiter waiter;
            lock (gate)
            {
                long now = Stopwatch.GetTimestamp();
                if (waiters.Count == 0 && CanAdmit(now))
                {
                    return Grant(now);
                }
                waiter = new Waiter { Retry = retry };
                // A new part goes last; a retry goes after the retries already waiting, ahead of the new parts.
                LinkedListNode<Waiter> firstNewPart = null;
                if (retry)
                {
                    firstNewPart = waiters.First;
                    while (firstNewPart != null && firstNewPart.Value.Retry)
                    {
                        firstNewPart = firstNewPart.Next;
                    }
                }
                waiter.Node = firstNewPart != null ? waiters.AddBefore(firstNewPart, waiter) : waiters.AddLast(waiter);
                ScheduleResume(now);
            }
            // Disposed only after the wait ends, outside the lock, so it never waits on a Cancel that needs the lock.
            using (cancellationToken.Register(() => Cancel(waiter, cancellationToken)))
            {
                return await waiter.Completion.Task;
            }
        }

        // A file of a shared scope that chooses its own part sizes joins or leaves size feedback
        // (ConcurrencyController.AttachSizing).
        internal void AttachSizing(PartSizeController sizer)
        {
            lock (gate)
            {
                controller.AttachSizing(Stopwatch.GetTimestamp(), sizer);
            }
        }

        internal void DetachSizing(PartSizeController sizer)
        {
            lock (gate)
            {
                controller.DetachSizing(Stopwatch.GetTimestamp(), sizer);
            }
        }

        // The server now requires parts one at a time.
        internal void UseSerial(UploadSerialReason reason)
        {
            lock (gate)
            {
                controller.UseSerial(Stopwatch.GetTimestamp(), reason);
            }
        }

        internal int Target
        {
            get
            {
                lock (gate)
                {
                    return controller.Target;
                }
            }
        }

        // The attempts waiting to be admitted: parts ready to send, and retries, beyond what the target allows now.
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

        // Read the target and startup phase together so a promotion cannot pair an older target with an ended phase.
        internal (int Target, bool Warming, bool WarmStarted) WarmTarget
        {
            get
            {
                lock (gate)
                {
                    return (controller.Target, controller.Warming, controller.WarmStarted);
                }
            }
        }

        internal UploadConcurrency Snapshot()
        {
            lock (gate)
            {
                return controller.Snapshot(Stopwatch.GetTimestamp());
            }
        }

        public void Dispose()
        {
            lock (gate)
            {
                disposed = true;
                resume?.Dispose();
                resume = null;
            }
        }

        private bool CanAdmit(long now)
        {
            return controller.Permitted < controller.Target && !controller.IsPaused(now);
        }

        private Attempt Grant(long now)
        {
            controller.Granted(now);
            return new Attempt(this);
        }

        // Admits waiters in order for as long as the target and the pause allow.
        private void Pump()
        {
            var granted = new List<KeyValuePair<Waiter, Attempt>>();
            lock (gate)
            {
                long now = Stopwatch.GetTimestamp();
                while (waiters.First != null && CanAdmit(now))
                {
                    Waiter waiter = waiters.First.Value;
                    waiters.RemoveFirst();
                    waiter.Node = null;
                    granted.Add(new KeyValuePair<Waiter, Attempt>(waiter, Grant(now)));
                }
                ScheduleResume(now);
            }
            foreach (KeyValuePair<Waiter, Attempt> grant in granted)
            {
                grant.Key.Completion.TrySetResult(grant.Value);
            }
        }

        // While attempts wait only for a pause to end, wakes them when it does.
        private void ScheduleResume(long now)
        {
            if (disposed || waiters.Count == 0 || !controller.IsPaused(now) || controller.Permitted >= controller.Target)
            {
                return;
            }
            long milliseconds = ((controller.PausedUntil - now) * 1000 / Stopwatch.Frequency) + 1;
            if (resume == null)
            {
                resume = new Timer(_ => Pump(), null, milliseconds, Timeout.Infinite);
            }
            else
            {
                resume.Change(milliseconds, Timeout.Infinite);
            }
        }

        private void Cancel(Waiter waiter, CancellationToken cancellationToken)
        {
            lock (gate)
            {
                if (waiter.Node == null)
                {
                    return; // Already admitted.
                }
                waiters.Remove(waiter.Node);
                waiter.Node = null;
            }
            waiter.Completion.TrySetCanceled(cancellationToken);
        }

        private sealed class Waiter
        {
            internal bool Retry { get; set; }

            // Continuations run on the thread pool, not inside whoever releases capacity.
            internal TaskCompletionSource<Attempt> Completion { get; } = new TaskCompletionSource<Attempt>(TaskCreationOptions.RunContinuationsAsynchronously);

            // In the queue while not null. Guarded by the gate's lock.
            internal LinkedListNode<Waiter> Node { get; set; }
        }

        // One admitted network attempt. Holds its permit, and its limiter request slot once it has one, until it is
        // completed or disposed, or TryStart finds it may no longer be sent.
        internal sealed class Attempt : IDisposable
        {
            private readonly AttemptAdmission admission;
            private int epoch;
            // The number the controller gave it as it started (ConcurrencyController.Sending).
            private long number;
            private IDisposable requestSlot;
            private bool started;
            private bool ended;
            private bool writing;

            internal Attempt(AttemptAdmission admission)
            {
                this.admission = admission;
            }

            // When the part started asking for this attempt, on the upload's clock.
            internal TimeSpan AskedAt { get; private set; }

            // How long the attempt waited, from AskedAt until it started.
            internal TimeSpan Waited { get; private set; }

            // The attempt now holds this limiter request slot, which it gives back when it ends.
            internal void Hold(IDisposable slot, TimeSpan askedAt)
            {
                lock (admission.gate)
                {
                    requestSlot = slot;
                    AskedAt = askedAt;
                }
            }

            // Starts the attempt, if the upload still allows it now that it holds its request slot: no cooldown, and
            // fewer attempts being sent than the target. Otherwise ends it, giving back its permit and request slot,
            // and returns false; the part must ask again.
            internal bool TryStart(TimeSpan waited)
            {
                IDisposable slot;
                lock (admission.gate)
                {
                    long now = Stopwatch.GetTimestamp();
                    ConcurrencyController controller = admission.controller;
                    if (!controller.IsPaused(now) && controller.InFlight < controller.Target)
                    {
                        started = true;
                        epoch = controller.Epoch;
                        Waited = waited;
                        number = controller.Sending(now);
                        return true;
                    }
                    ended = true;
                    controller.Released(now, sending: false);
                    slot = requestSlot;
                    requestSlot = null;
                }
                slot?.Dispose();
                admission.Pump();
                return false;
            }

            // The started attempt starts (true) or stops (false) writing what it received to the local destination, which
            // moves no bytes over the network (ConcurrencyController.Local). Ending it stops that too.
            internal void Writing(bool now)
            {
                lock (admission.gate)
                {
                    if (!started || ended || writing == now)
                    {
                        return;
                    }
                    writing = now;
                    admission.controller.Local(Stopwatch.GetTimestamp(), now);
                }
            }

            // Ends the attempt with what its request found out, and returns the decisions that made, if tracing.
            internal List<UploadTraceEvent> Complete(AttemptSample sample)
            {
                List<UploadTraceEvent> decisions = admission.tracing ? new List<UploadTraceEvent>() : null;
                End(sample, decisions);
                return decisions;
            }

            // Ends the attempt without a measurement: it was cancelled, or its part could not be read.
            public void Dispose()
            {
                End(null, null);
            }

            private void End(AttemptSample sample, List<UploadTraceEvent> decisions)
            {
                IDisposable slot;
                lock (admission.gate)
                {
                    if (ended)
                    {
                        return;
                    }
                    ended = true;
                    long now = Stopwatch.GetTimestamp();
                    if (writing)
                    {
                        writing = false;
                        admission.controller.Local(now, false);
                    }
                    if (sample != null && started)
                    {
                        admission.controller.Record(now, epoch, sample, decisions);
                    }
                    admission.controller.Released(now, started, number);
                    slot = requestSlot;
                    requestSlot = null;
                }
                slot?.Dispose();
                admission.Pump();
            }
        }
    }
}