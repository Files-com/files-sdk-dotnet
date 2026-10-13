using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FilesCom.Transfers
{
    // The concurrency target a transfer's part requests are admitted to, with the controller that learns it from them
    // (AttemptAdmission, ConcurrencyController): a download's own, or one that every automatic file of a directory transfer
    // with the same profile shares, in either direction, so that their parts teach one target, which they share rather
    // than each having all of it, and a file of one part still adds to what the next learns. The limiter stays a
    // separate, hard bound on request slots and buffer space.
    internal sealed class TransferScope : IDisposable
    {
        // tracing: whether attempts return the decisions they make (AttemptAdmission), for an upload whose options ask for a
        // trace; each decision then goes to the one file whose attempt made it, attributed to the scope's path.
        // warmStart: whether the shared controller warm-starts (ConcurrencyController), once for all the files that
        // share it.
        internal TransferScope(ConcurrencySettings settings, DownloadConcurrencyMode mode, string path, bool shared, bool tracing = false, bool warmStart = false)
        {
            Mode = mode;
            Shared = shared;
            Admission = new AttemptAdmission(new ConcurrencyController(settings, path, Stopwatch.GetTimestamp(), warmStart: warmStart), tracing);
        }

        internal AttemptAdmission Admission { get; }

        internal DownloadConcurrencyMode Mode { get; }

        internal bool Shared { get; }

        internal DownloadConcurrency Snapshot(TransferLimiter limiter)
        {
            return new DownloadConcurrency(Admission.Snapshot(), Mode, limiter.MaxConcurrentRequests, Shared);
        }

        public void Dispose()
        {
            Admission.Dispose();
        }
    }

    // The scopes of one directory transfer, one for each profile its automatic files use. They last as long as the
    // transfer, and nothing outside it uses them: its files learn together, and from no other transfer. A transfer goes
    // one way, to one endpoint, with one client and one set of file options, so the profile is all that tells its
    // compatible files apart.
    internal sealed class TransferScopes : IDisposable
    {
        private readonly object gate = new object();
        private readonly Dictionary<UploadPerformanceProfile, TransferScope> scopes = new Dictionary<UploadPerformanceProfile, TransferScope>();
        private readonly string path;

        internal TransferScopes(string path)
        {
            this.path = path;
        }

        // The scope for an automatic file's settings, made from them by the first file that asks. The files of one
        // transfer share their options, so settings of the same profile are the same, and so is warmStart: a directory
        // upload's files ask for the warm start their settings choose (ConcurrencySettings.WarmStarts); a directory
        // download's never do.
        internal TransferScope For(ConcurrencySettings settings, bool tracing = false, bool warmStart = false)
        {
            lock (gate)
            {
                if (!scopes.TryGetValue(settings.Profile, out TransferScope scope))
                {
                    scope = new TransferScope(settings, DownloadConcurrencyMode.Automatic, path, shared: true, tracing, warmStart);
                    scopes.Add(settings.Profile, scope);
                }
                return scope;
            }
        }

        // The attempts of the transfer's scopes that wait to be admitted (AttemptAdmission.Waiting).
        internal int Waiting()
        {
            lock (gate)
            {
                int waiting = 0;
                foreach (TransferScope scope in scopes.Values)
                {
                    waiting += scope.Admission.Waiting;
                }
                return waiting;
            }
        }

        // The lowest target and the startup phase of the scopes holding it, OR-ing ties. Each admission reads its
        // target and phase together. Null until a file starts sending or fetching parts.
        internal (int Target, bool Warming, bool WarmStarted)? WarmTarget()
        {
            lock (gate)
            {
                (int Target, bool Warming, bool WarmStarted)? lowest = null;
                foreach (TransferScope scope in scopes.Values)
                {
                    (int Target, bool Warming, bool WarmStarted) seen = scope.Admission.WarmTarget;
                    if (!lowest.HasValue || seen.Target < lowest.Value.Target)
                    {
                        lowest = seen;
                    }
                    else if (seen.Target == lowest.Value.Target)
                    {
                        lowest = (seen.Target, seen.Warming || lowest.Value.Warming, seen.WarmStarted || lowest.Value.WarmStarted);
                    }
                }
                return lowest;
            }
        }

        // The lowest target of the transfer's scopes, or null before any file has started sending or fetching parts.
        internal int? Target()
        {
            lock (gate)
            {
                int? lowest = null;
                foreach (TransferScope scope in scopes.Values)
                {
                    int target = scope.Admission.Target;
                    lowest = lowest.HasValue ? Math.Min(lowest.Value, target) : target;
                }
                return lowest;
            }
        }

        public void Dispose()
        {
            lock (gate)
            {
                foreach (TransferScope scope in scopes.Values)
                {
                    scope.Dispose();
                }
                scopes.Clear();
            }
        }
    }
}