using FilesCom;
using FilesCom.Transfers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FilesTests
{
    // Automatic download concurrency, observed where it acts: how many part requests the loopback storage holds at once,
    // when they arrive, what the destination is doing, and what the download reports. Answers come in loopback time,
    // after delays the test chooses, which proves the mechanics, not production tuning.
    public partial class DownloadTest
    {
        private const int SmallPart = 256;

        private static byte[] ManyParts(int parts) => Enumerable.Range(0, parts * SmallPart).Select(i => (byte)(i * 13 + 5)).ToArray();

        private static AutomaticConcurrencyOptions DownloadTuning(int start, int max, int step = 1, Action<AutomaticConcurrencyOptions> change = null)
        {
            AutomaticConcurrencyOptions tuning = AutomaticConcurrencyOptions.For(UploadPerformanceProfile.S3);
            tuning.MinTarget = 1;
            tuning.StartTarget = start;
            tuning.SoftCeiling = max;
            tuning.MaxTarget = max;
            tuning.PlateauTarget = max;
            tuning.WindowAttempts = 4;
            tuning.GrowthIntervalWindows = 1;
            tuning.GrowthStep = step;
            tuning.ProbeGainPercent = 3;
            tuning.ProbeWindows = 2;
            tuning.HoldWindows = 1;
            tuning.LatencyGrowthQueueLimit = 0;
            tuning.LatencyBackoffQueueLimit = 0;
            change?.Invoke(tuning);
            return tuning;
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task AnAutomaticDownloadFetchesMorePartsAtOnceWhileThatPaysPastTheOld64()
        {
            byte[] contents = ManyParts(1500);
            using (var server = new DownloadServer(contents))
            {
                var storage = new StorageGate();
                var reports = new DownloadReports();
                // The download's cancellation, the test's own from the start: cleanup cancels a download still running.
                var stopping = new CancellationTokenSource();
                // Storage that answers in cohorts, each held as long as the first, at the start target, sets (CohortStorage).
                var cohorts = new CohortStorage(startTarget: 56);
                Task<DownloadResult> download = null;
                DownloadResult result;
                bool reachedAssertions = false;
                try
                {
                    server.Intercept = (request, connection) => request.Range?.First == 0 ? cohorts.FirstPart() : storage.HoldAsync(cohorts.Arrive());
                    var options = new DownloadOptions { PartSize = SmallPart, Progress = reports, AutomaticConcurrency = DownloadTuning(start: 56, max: 72, step: 8) };

                    download = Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options, stopping.Token);
                    await Until(() => storage.Peak >= 72 || download.IsCompleted, TimeSpan.FromSeconds(30));
                    cohorts.Drain();
                    if (!await CohortStorage.EndsWithinAsync(download, CohortStorage.CompletionLimit))
                    {
                        Assert.Fail($"The download had not ended {CohortStorage.CompletionLimit.TotalSeconds:F0} s after every answer was released: {cohorts.State(download)}");
                    }
                    result = await download;
                    reachedAssertions = true;
                }
                finally
                {
                    // The test's lifetime of the download and the pacing ends here: every answer held goes, later requests
                    // are answered at once and a download still running is cancelled, and then both must be seen to have
                    // ended (SettleAsync). Work not seen to end fails the test, saying what was still running, unless the
                    // test is already failing: that failure is kept, and this is recorded with it. The token's source is
                    // disposed only once the download has been seen to end, since one still running may use it.
                    string unsettled = await cohorts.SettleAsync(download, stopping);
                    Console.WriteLine(cohorts.Describe(storage.Peak, reports.All));
                    if (unsettled == null)
                    {
                        stopping.Dispose();
                    }
                    else
                    {
                        Console.WriteLine(unsettled);
                        if (reachedAssertions)
                        {
                            Assert.Fail(unsettled);
                        }
                    }
                }

                CollectionAssert.AreEqual(contents, File.ReadAllBytes(Destination));
                DownloadConcurrency concurrency = result.FinalProgress.Concurrency;
                Assert.AreEqual(72, storage.Peak, $"More requests at once, past 64, up to the maximum target and no further: {Describe(concurrency)}");
                Assert.AreEqual(DownloadConcurrencyMode.Automatic, concurrency.Mode);
                Assert.AreEqual(72, concurrency.MaxTarget);
                Assert.AreEqual(72, concurrency.PeakActiveRequests);
                Assert.AreEqual(72, concurrency.RequestLimit, "The download's own limiter has a slot for every request its target may reach.");
                Assert.IsTrue(concurrency.Grows >= 2, Describe(concurrency));
            }
        }

        // The storage of AnAutomaticDownloadFetchesMorePartsAtOnceWhileThatPaysPastTheOld64: it answers part requests in
        // cohorts, as storage whose time for an answer does not grow with how many requests it serves at once, where more
        // requests at once deliver more, as the download's raises must find. It holds every request that arrives and, once
        // the hold has passed since the last arrival, answers all it holds together, however many: it never waits for a
        // number of requests, so cohorts of 56 and 64 are answered as surely as one of 72. As each answer is written, the
        // download sends its next request: those make the next cohort.
        //
        // The hold is the same for every cohort, so that a cohort's time grows only with the download's own refill, which
        // this storage measures: from the binding part's answer, or a cohort's release, to the next cohort's last arrival.
        // The first cohort, at the start target, sets the hold to twelve times its refill. The target is then on the
        // network for about 12/13 (92 %) of each cohort's time, where a window is judged only above 75 %; and 8 requests
        // more, whose refill should take at most 64/56 as long, add about 1 % to a cohort's time while it delivers 14 %
        // more, a gain of about 13 % where a raise needs 3 %. A later cohort no larger than the start target whose refill
        // takes more than a sixth of the hold, leaving the target on the network for less than 6/7 of the time, raises the
        // hold to twelve times that refill. Those shares rest on that scaling of the refill, not on any host's timing; each
        // cohort's size, refill, hold and lateness is recorded (Describe). The hold is at most 500 ms. The 1,499 parts after
        // the binding one make about 27 cohorts if each holds at least 56: at most 13.5 s of holds, plus their refills.
        // Cohorts can be smaller, so that is an estimate for full cohorts, not a bound: the test's 30-second wait bounds the
        // pacing, and its cleanup ends it (SettleAsync).
        private sealed class CohortStorage
        {
            private const int HoldPerRefill = 12;
            private const int RefillsPerHoldBeforeRaise = 6;
            // The most cohorts, and changes of target, Describe records.
            private const int Recorded = 40;
            private static readonly TimeSpan MaxHold = TimeSpan.FromMilliseconds(500);
            // The longest the download may take to end once every answer is released (Drain), and the longest cleanup then
            // waits for the download and the pacing to end (SettleAsync): after the test's 30-second wait, both together stay
            // within its 60 seconds.
            public static readonly TimeSpan CompletionLimit = TimeSpan.FromSeconds(10);
            private static readonly TimeSpan SettleLimit = TimeSpan.FromSeconds(10);

            private readonly object gate = new object();
            private readonly Stopwatch clock = Stopwatch.StartNew();
            private readonly int startTarget;
            private readonly SemaphoreSlim arrived = new SemaphoreSlim(0);
            private readonly CancellationTokenSource stopping = new CancellationTokenSource();
            private readonly List<string> record = new List<string>();
            private readonly Task pacing;
            private TaskCompletionSource<bool> answer = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            // When the refill of the cohort now arriving began, its last arrival so far, and how many it holds.
            private TimeSpan? refillFrom;
            private TimeSpan lastArrival;
            private int size;
            private int answered;
            // Every cohort's hold, once the first has set it.
            private TimeSpan? hold;
            private bool draining;

            public CohortStorage(int startTarget)
            {
                this.startTarget = startTarget;
                pacing = Task.Run(PaceAsync);
            }

            // The binding part, answered at once: the first cohort's refill begins with it.
            public Task<bool> FirstPart()
            {
                lock (gate)
                {
                    refillFrom = refillFrom ?? clock.Elapsed;
                }
                return Task.FromResult(false);
            }

            // A part request arrives: it is held until its cohort is answered, or not at all once draining.
            public Task Arrive()
            {
                lock (gate)
                {
                    if (draining)
                    {
                        return Task.CompletedTask;
                    }
                    lastArrival = clock.Elapsed;
                    refillFrom = refillFrom ?? lastArrival;
                    if (size++ == 0)
                    {
                        arrived.Release();
                    }
                    return answer.Task;
                }
            }

            // Answers every request held, and every later one at once.
            public void Drain()
            {
                lock (gate)
                {
                    if (draining)
                    {
                        return;
                    }
                    draining = true;
                    answer.TrySetResult(true);
                }
                stopping.Cancel();
            }

            // Ends the test's lifetime of the pacing and the download: drains, cancels the download, when it was started and
            // has not ended, then waits, at most SettleLimit, for the download and the pacing to end, checking which ended
            // rather than assuming it. Returns null once both have ended, the pacing without failing, or else what was seen.
            // Nothing is thrown here: the download's own outcome is the test's, which awaits it, and a failure the test
            // already has stays its own. A failure no one awaited, such as a cancelled download's, is observed.
            public async Task<string> SettleAsync(Task download, CancellationTokenSource cancellation)
            {
                Drain();
                var problems = new List<string>();
                if (download != null && !download.IsCompleted)
                {
                    try
                    {
                        cancellation.Cancel();
                    }
                    catch (AggregateException e)
                    {
                        problems.Add($"cancelling the download failed: {e.GetBaseException().Message}");
                    }
                }
                Task ended = download == null ? pacing : Task.WhenAll(pacing, download);
                bool settled = await EndsWithinAsync(ended, SettleLimit);
                _ = ended.ContinueWith(task => { _ = task.Exception; }, TaskContinuationOptions.OnlyOnFaulted);
                if (!settled)
                {
                    problems.Add($"not ended {SettleLimit.TotalSeconds:F0} s after cleanup began");
                }
                else if (pacing.IsFaulted)
                {
                    problems.Add($"the pacing failed: {pacing.Exception.GetBaseException().Message}");
                }
                return problems.Count == 0 ? null : $"Cleanup: {string.Join("; ", problems)}; {State(download)}";
            }

            // Whether task ended within limit: the task that finished first is checked, so a limit that ran out is never taken
            // for an end.
            public static async Task<bool> EndsWithinAsync(Task task, TimeSpan limit)
            {
                using (var timer = new CancellationTokenSource())
                {
                    Task first = await Task.WhenAny(task, Task.Delay(limit, timer.Token));
                    timer.Cancel();
                    return first == task;
                }
            }

            // What the download and the storage were doing, for a failure to say.
            public string State(Task download)
            {
                lock (gate)
                {
                    return $"download {(download == null ? "not started" : download.Status.ToString())}, pacing {pacing.Status}, {size} requests in the last cohort held, {answered} cohorts answered, {(draining ? "drained" : "not drained")}";
                }
            }

            // The cohorts answered, the peak the storage held, the hold, and each change of target the download reported.
            public string Describe(int peak, IReadOnlyList<DownloadProgress> reports)
            {
                var targets = new List<int>();
                foreach (DownloadProgress report in reports)
                {
                    if (report.Concurrency != null && targets.Count < Recorded && (targets.Count == 0 || targets[targets.Count - 1] != report.Concurrency.Target))
                    {
                        targets.Add(report.Concurrency.Target);
                    }
                }
                lock (gate)
                {
                    string held = hold.HasValue ? $"{hold.Value.TotalMilliseconds:F1} ms" : "never set";
                    return $"Cohort storage: peak {peak}, hold {held}, {answered} cohorts answered: {string.Join("; ", record)}. Targets reported: {string.Join(" > ", targets)}";
                }
            }

            // Answers the cohort held once its hold has passed since its last arrival, until draining.
            private async Task PaceAsync()
            {
                try
                {
                    while (true)
                    {
                        TimeSpan? wait = null;
                        lock (gate)
                        {
                            if (draining)
                            {
                                return;
                            }
                            if (size > 0)
                            {
                                TimeSpan due = lastArrival + (hold ?? HoldFor(lastArrival - refillFrom.Value));
                                wait = due - clock.Elapsed;
                                if (wait <= TimeSpan.Zero)
                                {
                                    Answer(due);
                                    continue;
                                }
                            }
                        }
                        if (wait.HasValue)
                        {
                            await Task.Delay(wait.Value, stopping.Token);
                        }
                        else
                        {
                            await arrived.WaitAsync(stopping.Token);
                        }
                    }
                }
                catch (OperationCanceledException) when (stopping.IsCancellationRequested)
                {
                    // Drained.
                }
            }

            // Answers the cohort held and records it. Its refill sets the hold, if it is the first, or raises it, if it holds
            // no more than the start target and took more than a sixth of the hold. Called under gate.
            private void Answer(TimeSpan due)
            {
                TimeSpan now = clock.Elapsed;
                TimeSpan refill = lastArrival - refillFrom.Value;
                string change = "";
                if (size <= startTarget && (!hold.HasValue || refill.Ticks * RefillsPerHoldBeforeRaise > hold.Value.Ticks))
                {
                    hold = HoldFor(refill);
                    change = $", hold now {hold.Value.TotalMilliseconds:F1} ms";
                }
                answered++;
                if (record.Count < Recorded)
                {
                    record.Add($"{size} after a {refill.TotalMilliseconds:F1} ms refill, answered {(now - lastArrival).TotalMilliseconds:F1} ms after the last, {(now - due).TotalMilliseconds:F1} ms late{change}");
                }
                answer.TrySetResult(true);
                answer = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                size = 0;
                refillFrom = now;
            }

            // HoldPerRefill times refill, at most MaxHold.
            private static TimeSpan HoldFor(TimeSpan refill) => TimeSpan.FromTicks(Math.Min(refill.Ticks * HoldPerRefill, MaxHold.Ticks));
        }

        // Without AutomaticConcurrency, the tuning follows the storage the download is bound to, as the Go SDK chooses it
        // by the storage's host: the scaled S3 policy for a download fetched in parts from Amazon S3, the only storage
        // this SDK binds a download to. Tuning the caller gives, the conservative S3 preset included, is used as it is.
        // The choice is settled when the parts start and holds for the whole download. Storage that cannot bind a download
        // has the file fetched whole, in one request, so there is no concurrency to choose.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(true, null, UploadPerformanceProfile.S3, 150, 1024, 1024, DisplayName = "Amazon S3 without tuning: the scaled S3 policy")]
        [DataRow(true, "S3", UploadPerformanceProfile.S3, 8, 64, 64, DisplayName = "The caller's conservative S3 preset wins")]
        [DataRow(true, "Generic", UploadPerformanceProfile.Generic, 4, 8, 8, DisplayName = "The caller's own tuning wins")]
        [DataRow(false, null, UploadPerformanceProfile.Generic, 0, 0, 0, DisplayName = "Storage that cannot bind a download: fetched whole")]
        public async Task TheStorageTheDownloadIsBoundToChoosesTheDefaultTuning(bool bound, string callerTuning, UploadPerformanceProfile profile, int start, int max, int requestLimit)
        {
            byte[] contents = ManyParts(40);
            using (var server = new DownloadServer(contents) { Bound = bound })
            {
                var reports = new DownloadReports();
                var options = new DownloadOptions { PartSize = SmallPart, Progress = reports };
                if (callerTuning != null)
                {
                    options.AutomaticConcurrency = AutomaticConcurrencyOptions.For((UploadPerformanceProfile)Enum.Parse(typeof(UploadPerformanceProfile), callerTuning));
                }

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options);

                CollectionAssert.AreEqual(contents, File.ReadAllBytes(Destination));
                DownloadConcurrency concurrency = result.FinalProgress.Concurrency;
                if (!bound)
                {
                    Assert.IsNull(concurrency, "Fetched whole, in one request: no parts to choose a concurrency for.");
                    Assert.IsTrue(reports.All.All(report => report.Concurrency == null));
                    return;
                }
                Assert.AreEqual(DownloadConcurrencyMode.Automatic, concurrency.Mode);
                Assert.AreEqual(profile, concurrency.Profile, Describe(concurrency));
                Assert.AreEqual(max, concurrency.MaxTarget, Describe(concurrency));
                Assert.AreEqual(requestLimit, concurrency.RequestLimit, "The download's own limiter has a slot for every request its target may reach: " + Describe(concurrency));
                List<DownloadConcurrency> reported = reports.All.Select(report => report.Concurrency).Where(c => c != null).ToList();
                Assert.IsTrue(reported.Count > 0, "The reports carry the concurrency once parts are fetched.");
                Assert.AreEqual(start, reported[0].Target, "Settled, at the tuning's start, before the first part: " + Describe(reported[0]));
                Assert.IsTrue(reported.All(c => c.Profile == profile && c.MaxTarget == max && c.Mode == DownloadConcurrencyMode.Automatic), "The choice holds for the whole download.");
            }
        }

        // A lower MaxConcurrency, or a limiter with fewer request slots, is a hard limit; waiting for the limiter teaches the
        // target nothing.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("max-concurrency")]
        [DataRow("limiter")]
        public async Task LowerCapsStayHard(string cap)
        {
            byte[] contents = ManyParts(300);
            using (var server = new DownloadServer(contents))
            {
                var storage = new StorageGate();
                server.Intercept = (request, connection) => request.Range?.First == 0 ? Task.FromResult(false) : storage.HoldAsync(Task.Delay(15));
                var options = new DownloadOptions { PartSize = SmallPart, AutomaticConcurrency = DownloadTuning(start: 4, max: 8) };
                if (cap == "max-concurrency")
                {
                    options.MaxConcurrency = 3;
                }
                else
                {
                    options.Limiter = new TransferLimiter(maxConcurrentRequests: 2);
                }

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options);

                CollectionAssert.AreEqual(contents, File.ReadAllBytes(Destination));
                DownloadConcurrency concurrency = result.FinalProgress.Concurrency;
                if (cap == "max-concurrency")
                {
                    Assert.IsTrue(storage.Peak <= 3, $"{storage.Peak} at once: {Describe(concurrency)}");
                    Assert.AreEqual(3, concurrency.MaxTarget);
                }
                else
                {
                    Assert.IsTrue(storage.Peak <= 2, $"{storage.Peak} at once: {Describe(concurrency)}");
                    Assert.AreEqual(2, concurrency.Limit);
                    Assert.AreEqual(0, concurrency.Grows, "Requests waiting for the limiter are not the network's: " + Describe(concurrency));
                    Assert.IsTrue(concurrency.TimeWaitingForLimiter > TimeSpan.Zero);
                }
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task AStaticDownloadKeepsItsNumberOfParts()
        {
            byte[] contents = ManyParts(200);
            using (var server = new DownloadServer(contents))
            {
                var storage = new StorageGate();
                server.Intercept = (request, connection) => request.Range?.First == 0 ? Task.FromResult(false) : storage.HoldAsync(Task.Delay(15));
                var options = new DownloadOptions { PartSize = SmallPart, ConcurrencyMode = DownloadConcurrencyMode.Static, MaxConcurrency = 5 };

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options);

                CollectionAssert.AreEqual(contents, File.ReadAllBytes(Destination));
                DownloadConcurrency concurrency = result.FinalProgress.Concurrency;
                Assert.AreEqual(5, storage.Peak);
                Assert.AreEqual(DownloadConcurrencyMode.Static, concurrency.Mode);
                Assert.AreEqual(5, concurrency.Target);
                Assert.AreEqual(0, concurrency.Grows + concurrency.Shrinks);
            }
        }

        // A throttling answer pauses every new part request, the throttled part's retry included, for the server's
        // Retry-After, and lowers the target; once the pause is over the download fetches more again as that pays off.
        [TestMethod]
        [Timeout(60000)]
        public async Task ThrottlingPausesAndLowersTheTargetUntilItRecovers()
        {
            byte[] contents = ManyParts(600);
            using (var server = new DownloadServer(contents))
            {
                var storage = new StorageGate();
                int throttled = 0;
                TimeSpan throttledAt = TimeSpan.Zero;
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range?.First == 40 * SmallPart && Interlocked.Exchange(ref throttled, 1) == 0)
                    {
                        throttledAt = storage.Now;
                        await connection.RespondAsync(503, "<Error><Code>SlowDown</Code></Error>", "Retry-After: 1\r\n");
                        return true;
                    }
                    return request.Range?.First == 0 ? false : await storage.HoldAsync(Task.Delay(20));
                };
                var options = new DownloadOptions { PartSize = SmallPart, AutomaticConcurrency = DownloadTuning(start: 8, max: 8, change: t => t.BackpressureShrinkPercent = 50) };

                DownloadResult result = await Client(server, retries: 2).Transfers.DownloadFileAsync("remote.bin", Destination, options);

                CollectionAssert.AreEqual(contents, File.ReadAllBytes(Destination));
                DownloadConcurrency concurrency = result.FinalProgress.Concurrency;
                var paused = storage.Arrivals.Where(a => a.At > throttledAt + TimeSpan.FromMilliseconds(200) && a.At < throttledAt + TimeSpan.FromMilliseconds(900)).ToList();
                Assert.AreEqual(0, paused.Count, "No part request starts during the pause the server asked for.");
                Assert.AreEqual(1, concurrency.Throttles);
                Assert.IsTrue(concurrency.Shrinks >= 1 && concurrency.Grows >= 1, "Lowered, then raised again: " + Describe(concurrency));
                Assert.AreEqual(8, storage.Arrivals.Where(a => a.At > throttledAt + TimeSpan.FromSeconds(1)).Max(a => a.Held), "Back to the full target once the pause was over.");
            }
        }

        // The same HTTP answers, first with a destination that holds every write, then with a free destination and
        // answers that take as long: writing teaches the target nothing, neither a gain nor a loss, while the network's
        // own pace makes it grow.
        [TestMethod]
        [Timeout(60000)]
        public async Task ADestinationThatHoldsWritesTeachesTheTargetNothing()
        {
            byte[] contents = ManyParts(240);
            using (var server = new DownloadServer(contents))
            {
                var options = new DownloadOptions { PartSize = SmallPart, AutomaticConcurrency = DownloadTuning(start: 2, max: 8, change: t => t.ProbeWindows = 1) };
                DownloadCoordinator.DestinationWriteFaults.Value = () => Task.Delay(30);
                DownloadResult held;
                try
                {
                    held = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options);
                }
                finally
                {
                    DownloadCoordinator.DestinationWriteFaults.Value = null;
                }
                CollectionAssert.AreEqual(contents, File.ReadAllBytes(Destination));
                DownloadConcurrency writing = held.FinalProgress.Concurrency;
                Assert.AreEqual(0, writing.Grows + writing.Shrinks, "A destination-bound download learns nothing about the network: " + Describe(writing));
                Assert.AreEqual(2, writing.Target);
                Assert.IsTrue(writing.TimeWritingDestination > TimeSpan.FromSeconds(1), Describe(writing));

                File.Delete(Destination);
                server.Intercept = (request, connection) => request.Range?.First == 0 ? Task.FromResult(false) : DelayThenServe(30);
                DownloadResult free = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options);
                CollectionAssert.AreEqual(contents, File.ReadAllBytes(Destination));
                DownloadConcurrency network = free.FinalProgress.Concurrency;
                Assert.IsTrue(network.Grows >= 1 && network.PeakActiveRequests > 2, "The network's own pace does: " + Describe(network));
            }
        }

        // A server that ignores Range answers the first part with the whole file: the download then comes in one
        // request, and while that request is under way its progress reports no part concurrency, as for any file that
        // comes in one request.
        [TestMethod]
        [Timeout(60000)]
        public async Task AWholeFileFallbackReportsNoPartConcurrency()
        {
            using (var server = new DownloadServer(Contents) { IgnoresRange = true })
            {
                var reports = new DownloadReports();
                int wholeRequestAt = -1;
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range != null)
                    {
                        return false;
                    }
                    wholeRequestAt = reports.Count;
                    await connection.SendAsync($"HTTP/1.1 200 OK\r\nContent-Length: {Contents.Length}\r\nContent-Type: application/octet-stream\r\nETag: {server.ETag}\r\n\r\n");
                    for (int offset = 0; offset < Contents.Length; offset += 16384)
                    {
                        await connection.SendAsync(Contents.Skip(offset).Take(16384).ToArray());
                        await Task.Delay(150);
                    }
                    return true;
                };

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, new DownloadOptions { PartSize = 16 * 1024, Progress = reports });

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual(1, result.Parts);
                Assert.IsTrue(wholeRequestAt >= 0, "The file came in one request after the first answer.");
                List<DownloadProgress> during = reports.All.Skip(wholeRequestAt).ToList();
                Assert.IsTrue(during.Any(p => p.ReceivedBytes > 0 && p.WrittenBytes < Contents.Length), "A report came while the one request was under way.");
                Assert.IsTrue(during.All(p => p.Concurrency == null), "No stale part concurrency is reported for the one request.");
                Assert.IsNull(result.FinalProgress.Concurrency);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task ConcurrencySettingsThatCannotApplyFailBeforeAnyRequest()
        {
            using (var server = new DownloadServer(Contents))
            {
                FilesClient client = Client(server);
                AutomaticConcurrencyOptions floorWithoutRate = DownloadTuning(start: 2, max: 8, change: t => t.ProbeFloorTarget = 4);
                AutomaticConcurrencyOptions tooTolerant = DownloadTuning(start: 2, max: 8, change: t => t.ProbeLossTolerancePercent = 101);

                await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => client.Transfers.DownloadFileAsync("remote.bin", Destination, new DownloadOptions { ConcurrencyMode = DownloadConcurrencyMode.Static, AutomaticConcurrency = AutomaticConcurrencyOptions.ScaledS3() }));
                Assert.AreEqual(nameof(DownloadOptions.MaxConcurrency), (await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => client.Transfers.DownloadFileAsync("remote.bin", Destination, new DownloadOptions { MaxConcurrency = DownloadOptions.MaxAllowedConcurrency + 1 }))).ParamName);
                Assert.AreEqual("AutomaticConcurrency.ProbeFloorBytesPerSecond", (await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => client.Transfers.DownloadFileAsync("remote.bin", Destination, new DownloadOptions { AutomaticConcurrency = floorWithoutRate }))).ParamName);
                Assert.AreEqual("AutomaticConcurrency.ProbeLossTolerancePercent", (await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => client.Transfers.DownloadFileAsync("remote.bin", Destination, new DownloadOptions { AutomaticConcurrency = tooTolerant }))).ParamName);

                Assert.AreEqual(0, server.ApiRequests.Count + server.StorageRequests.Count, "Nothing is sent.");
                AssertNothingPublished();
            }
        }

        // A real download with a loss tolerance, against storage whose two workers make more requests at once queue, and
        // each one queued costs them more time: every byte arrives where it belongs, the soft ceiling of 2 unlocks at the
        // first window and the download sends more requests at once than it, and the storage never holds more than the
        // maximum of 6, for each of which the download's own limiter has a slot. Whether each raise is then kept depends on
        // the loss its window measures, which here depends on the host as well as on the workers' waits; how the
        // tolerance judges it is shown by EachRaiseIsKeptOnlyWhenItsMeasuredLossIsWithinTheTolerance, on a clock the test
        // controls.
        [TestMethod]
        [Timeout(60000)]
        public async Task ALossTolerantDownloadRaisesPastItsSoftCeilingWithinItsMaximum()
        {
            byte[] contents = ManyParts(500);
            using (var server = new DownloadServer(contents))
            {
                var storage = new StorageGate();
                var workers = new SemaphoreSlim(2, 2);
                server.Intercept = (request, connection) => request.Range?.First == 0 ? Task.FromResult(false) : storage.HoldAsync(Worker(workers, () => QueuedWorkerMilliseconds(storage.Held)));
                var options = new DownloadOptions
                {
                    PartSize = SmallPart,
                    AutomaticConcurrency = LossToleranceTuning(50),
                };

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options);

                CollectionAssert.AreEqual(contents, File.ReadAllBytes(Destination));
                DownloadConcurrency concurrency = result.FinalProgress.Concurrency;
                string seen = $"{storage.Peak} at once: {Describe(concurrency)}";
                Assert.IsTrue(concurrency.Grows >= 1 && storage.Peak >= 3, "Raised past the soft ceiling: " + seen);
                Assert.IsTrue(storage.Peak <= 6 && concurrency.PeakActiveRequests <= 6, "Never more than the maximum at once: " + seen);
                Assert.AreEqual(6, concurrency.MaxTarget, seen);
                Assert.AreEqual(6, concurrency.RequestLimit, "The download's own limiter has a slot for every request its target may reach: " + seen);
            }
        }

        // The storage and tuning of ALossTolerantDownloadRaisesPastItsSoftCeilingWithinItsMaximum, with the tolerance
        // varied, on a clock the test controls (LossDriver), where the two workers take exactly their time for the requests
        // held, so that each raise above two loses what the storage's queueing costs, from 23 % (2 to 3) down to 14 % (5 to
        // 6), and nothing else. The download's real controller judges every raise by the loss it measured: within a
        // tolerance of 50 %, each is kept, up to the maximum, and none is withdrawn; with a tolerance of 20 %, less than the
        // first raise loses, or with none, the first raise is withdrawn each time it is made, so that the storage never
        // holds more than 3 requests.
        [TestMethod]
        [DataRow(50.0, DisplayName = "Raises that lose less than the tolerance are all kept")]
        [DataRow(20.0, DisplayName = "A raise that loses more than the tolerance is withdrawn")]
        [DataRow(0.0, DisplayName = "Without a tolerance, a raise that does not gain is withdrawn")]
        public void EachRaiseIsKeptOnlyWhenItsMeasuredLossIsWithinTheTolerance(double tolerance)
        {
            var driver = new LossDriver(LossToleranceTuning(tolerance), QueuedWorkerMilliseconds);
            for (int turn = 0; turn < 600; turn++)
            {
                driver.Turn();
            }

            string evidence = driver.Describe();
            List<UploadDecisionTrace> judged = driver.Decisions.Where(d => d.Decision == ConcurrencyDecision.ProbeAccepted || d.Decision == ConcurrencyDecision.ProbeKeptWithinLossTolerance || d.Decision == ConcurrencyDecision.ProbeRejected).ToList();
            Assert.IsTrue(judged.Count > 0 && judged.All(d => d.ThroughputBytesPerSecond < d.BaselineBytesPerSecond), "Every raise was judged by a loss it measured.\n" + evidence);
            if (tolerance == 50)
            {
                CollectionAssert.AreEqual(
                    new[] { "2->3 Grew", "3->3 ProbeKeptWithinLossTolerance", "3->4 Grew", "4->4 ProbeKeptWithinLossTolerance", "4->5 Grew", "5->5 ProbeKeptWithinLossTolerance", "5->6 Grew", "6->6 ProbeKeptWithinLossTolerance" },
                    driver.Decisions.Where(d => d.Decision != ConcurrencyDecision.SoftCeilingUnlocked).Select(d => $"{d.PreviousTarget}->{d.Target} {d.Decision}").ToArray(),
                    "Each raise kept, up to the maximum, none withdrawn.\n" + evidence);
                Assert.AreEqual(6, driver.Target, evidence);
            }
            else
            {
                Assert.IsTrue(judged.Count >= 5 && judged.All(d => d.Decision == ConcurrencyDecision.ProbeRejected && d.PreviousTarget == 3 && d.Target == 2), "The raise to 3 was withdrawn each time it was made.\n" + evidence);
                Assert.AreEqual(3, driver.PeakHeld, evidence);
            }
        }

        // The scaled S3 policy at loopback scale: a soft ceiling of 2 that unlocks at once, then one request more at a time,
        // each raise judged by one window of 12 requests, kept for a gain of 5 % or a loss within tolerance.
        private static AutomaticConcurrencyOptions LossToleranceTuning(double tolerance)
        {
            return DownloadTuning(start: 2, max: 6, change: t =>
            {
                t.WindowAttempts = 12;
                t.SoftCeiling = 2;
                t.SoftCeilingUnlockBytes = 1;
                t.SoftCeilingUnlockBytesPerSecond = 0;
                t.ProbeGainPercent = 5;
                t.ProbeWindows = 1;
                t.HoldWindows = 0;
                t.ProbeLossTolerancePercent = tolerance;
            });
        }

        // A worker's time for each request with held requests at the storage: 12 ms, and 30 % more for each one held
        // beyond the two the workers serve.
        private static double QueuedWorkerMilliseconds(int held) => 12 * (1 + (0.3 * Math.Max(0, held - 2)));

        // Feeds a download's real controller part requests on a clock it controls, as two workers serve them: the storage
        // holds as many as the target allows, and each turn takes a worker's time for the requests held (workerMilliseconds)
        // and answers two of them, each delivering one part, after which as many are sent as the target then allows. Two
        // answers a turn make every window of an even number of requests a whole number of turns, so that how answers
        // fall in a window does not look like a gain or a loss.
        private sealed class LossDriver
        {
            private readonly ConcurrencyController controller;
            private readonly Func<int, double> workerMilliseconds;
            private readonly List<UploadTraceEvent> records = new List<UploadTraceEvent>();
            private long now = Stopwatch.Frequency;
            private int held;

            public LossDriver(AutomaticConcurrencyOptions tuning, Func<int, double> workerMilliseconds)
            {
                controller = new ConcurrencyController(ConcurrencySettings.Automatic(tuning.Validated(), null, UploadProfileSource.Options), "remote.bin", now);
                this.workerMilliseconds = workerMilliseconds;
            }

            public int Target => controller.Target;

            // The most requests the storage held at once.
            public int PeakHeld { get; private set; }

            public IReadOnlyList<UploadDecisionTrace> Decisions => records.OfType<UploadDecisionTrace>().ToList();

            public void Turn()
            {
                for (; held < controller.Target; held++)
                {
                    controller.Granted(now);
                    controller.Sending(now);
                }
                PeakHeld = Math.Max(PeakHeld, held);
                TimeSpan turn = TimeSpan.FromMilliseconds(workerMilliseconds(held));
                // Each request answered waited for those ahead of it, two served at a time.
                TimeSpan duration = TimeSpan.FromTicks(turn.Ticks * held / 2);
                int epoch = controller.Epoch;
                now += (long)(turn.TotalSeconds * Stopwatch.Frequency);
                for (int answered = 0; answered < 2 && held > 0; answered++, held--)
                {
                    controller.Record(now, epoch, new AttemptSample(accepted: true, throttled: false, transient: false, retryAfter: null, bytes: SmallPart, duration: duration), records);
                    controller.Released(now, sending: true);
                }
            }

            public string Describe()
            {
                return string.Join("; ", Decisions.Select(d => $"{d.Decision} {d.PreviousTarget}->{d.Target} {d.ThroughputBytesPerSecond:0}/{d.BaselineBytesPerSecond:0}"));
            }
        }

        // Against the same queueing storage, with a gain no raise can show: a fast link's raises up to the probe floor
        // need no gain, and a rejected raise above it is never lowered below the floor; a rejection shrink without a fast
        // link lowers the target below where the raise began; without either, a rejected raise goes back to where it was.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(6, 50.0, "floor", DisplayName = "Fast-link probe floor")]
        [DataRow(0, 50.0, "below-level", DisplayName = "Rejection shrink without a fast link")]
        [DataRow(0, -1.0, "back-to-level", DisplayName = "Neither: back to the level")]
        public async Task ScaledProbeFloorsAndShrinksHaveTheirOwnOutcomes(int floor, double shrink, string outcome)
        {
            byte[] contents = ManyParts(400);
            using (var server = new DownloadServer(contents))
            {
                var storage = new StorageGate();
                var workers = new SemaphoreSlim(2, 2);
                server.Intercept = (request, connection) => request.Range?.First == 0 ? Task.FromResult(false) : storage.HoldAsync(Worker(workers, 10));
                var reports = new DownloadReports();
                var options = new DownloadOptions
                {
                    PartSize = SmallPart,
                    Progress = reports,
                    AutomaticConcurrency = DownloadTuning(start: 2, max: 8, change: t =>
                    {
                        t.ProbeGainPercent = 1000;
                        t.ProbeWindows = 1;
                        t.HoldWindows = 0;
                        t.ProbeFloorTarget = floor > 0 ? floor : (int?)null;
                        t.ProbeFloorBytesPerSecond = floor > 0 ? 1 : 0;
                        t.ProbeRejectionShrinkPercent = shrink >= 0 ? shrink : (double?)null;
                        t.ThroughputFloorTarget = 1;
                    }),
                };

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options);

                CollectionAssert.AreEqual(contents, File.ReadAllBytes(Destination));
                List<int> targets = reports.All.Where(p => p.Concurrency != null).Select(p => p.Concurrency.Target).ToList();
                string seen = $"targets {string.Join(",", targets.Distinct())}; peak {storage.Peak}; {Describe(result.FinalProgress.Concurrency)}";
                switch (outcome)
                {
                    case "floor":
                        int reached = targets.IndexOf(6);
                        Assert.IsTrue(reached >= 0 && storage.Peak >= 6, "Raised to the floor without a gain: " + seen);
                        Assert.IsTrue(targets.Skip(reached).All(t => t >= 6), "Never lowered below the floor: " + seen);
                        Assert.IsTrue(result.FinalProgress.Concurrency.Shrinks >= 1, "A raise above the floor was rejected: " + seen);
                        break;
                    case "below-level":
                        Assert.AreEqual(1, targets.Min(), "Lowered below the level the raise began at: " + seen);
                        break;
                    default:
                        Assert.AreEqual(2, targets.Min(), "Back to the level, not below it: " + seen);
                        Assert.IsTrue(targets.Max() <= 3 && storage.Peak <= 3, seen);
                        break;
                }
            }
        }

        // A rejected raise only lowers load: with a throughput floor above where the target starts, and a soft ceiling
        // still locked below that floor, every rejection goes back to (or below) where the raise began, never up to the
        // floor and never past the ceiling.
        [TestMethod]
        [Timeout(60000)]
        public async Task ARejectedRaiseNeverLiftsTheTargetToItsFloorOrPastItsCeiling()
        {
            byte[] contents = ManyParts(300);
            using (var server = new DownloadServer(contents))
            {
                var storage = new StorageGate();
                var workers = new SemaphoreSlim(2, 2);
                server.Intercept = (request, connection) => request.Range?.First == 0 ? Task.FromResult(false) : storage.HoldAsync(Worker(workers, 10));
                var reports = new DownloadReports();
                var options = new DownloadOptions
                {
                    PartSize = SmallPart,
                    Progress = reports,
                    AutomaticConcurrency = DownloadTuning(start: 2, max: 8, change: t =>
                    {
                        t.SoftCeiling = 4;
                        t.SoftCeilingUnlockBytes = 1L << 50;
                        t.ProbeGainPercent = 1000;
                        t.ProbeWindows = 1;
                        t.HoldWindows = 0;
                        t.ProbeRejectionShrinkPercent = 50;
                        t.ThroughputFloorTarget = 6;
                    }),
                };

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options);

                CollectionAssert.AreEqual(contents, File.ReadAllBytes(Destination));
                List<int> targets = reports.All.Where(p => p.Concurrency != null).Select(p => p.Concurrency.Target).ToList();
                DownloadConcurrency concurrency = result.FinalProgress.Concurrency;
                string seen = $"targets {string.Join(",", targets.Distinct())}; peak {storage.Peak}; {Describe(concurrency)}";
                Assert.IsTrue(concurrency.Shrinks >= 1, "Raises were rejected: " + seen);
                Assert.IsTrue(targets.Max() <= 3 && storage.Peak <= 3, "No rejection lifted the target to its floor or past its locked ceiling: " + seen);
                Assert.IsTrue(targets.Min() <= 2, seen);
            }
        }

        private static Task Worker(SemaphoreSlim workers, int milliseconds) => Worker(workers, () => milliseconds);

        // One worker's time for a request, kept on the Stopwatch clock (DestinationClock): Task.Delay rounds to the host's
        // timer tick, about 15.6 ms on Windows, which would flatten the cost of each queued request these policies measure.
        private static async Task Worker(SemaphoreSlim workers, Func<double> milliseconds)
        {
            await workers.WaitAsync();
            try
            {
                await DestinationClock.Wait(TimeSpan.FromMilliseconds(milliseconds()));
            }
            finally
            {
                workers.Release();
            }
        }

        private static async Task<bool> DelayThenServe(int milliseconds)
        {
            await Task.Delay(milliseconds);
            return false;
        }

        private static async Task Until(Func<bool> condition, TimeSpan limit)
        {
            var clock = Stopwatch.StartNew();
            while (!condition() && clock.Elapsed < limit)
            {
                await Task.Delay(10);
            }
        }

        private static string Describe(DownloadConcurrency c) => c == null ? "no concurrency"
            : $"mode {c.Mode}, target {c.Target}/{c.MaxTarget}, limit {c.Limit}, peak {c.PeakActiveRequests}, grows {c.Grows}, shrinks {c.Shrinks}, throttles {c.Throttles}, last {c.LastDecision}, at target {c.TimeAtTarget.TotalMilliseconds:F0} ms, limiter {c.TimeWaitingForLimiter.TotalMilliseconds:F0} ms, no part {c.TimeWithoutReadyPart.TotalMilliseconds:F0} ms, writing {c.TimeWritingDestination.TotalMilliseconds:F0} ms";

        // Holds storage requests at the loopback server, counting how many it holds at once and when each arrived.
        private sealed class StorageGate
        {
            private readonly object gate = new object();
            private readonly Stopwatch clock = Stopwatch.StartNew();
            private readonly List<(TimeSpan At, int Held)> arrivals = new List<(TimeSpan, int)>();
            private int held;

            public int Peak { get; private set; }

            public int Held
            {
                get
                {
                    lock (gate)
                    {
                        return held;
                    }
                }
            }

            public TimeSpan Now => clock.Elapsed;

            public IReadOnlyList<(TimeSpan At, int Held)> Arrivals
            {
                get
                {
                    lock (gate)
                    {
                        return arrivals.ToList();
                    }
                }
            }

            // Holds a request while wait runs, then leaves it to the server.
            public async Task<bool> HoldAsync(Task wait)
            {
                lock (gate)
                {
                    held++;
                    Peak = Math.Max(Peak, held);
                    arrivals.Add((clock.Elapsed, held));
                }
                try
                {
                    await wait;
                }
                finally
                {
                    lock (gate)
                    {
                        held--;
                    }
                }
                return false;
            }
        }

        private sealed class DownloadReports : IProgress<DownloadProgress>
        {
            private readonly List<DownloadProgress> reports = new List<DownloadProgress>();

            public int Count
            {
                get
                {
                    lock (reports)
                    {
                        return reports.Count;
                    }
                }
            }

            public IReadOnlyList<DownloadProgress> All
            {
                get
                {
                    lock (reports)
                    {
                        return reports.ToList();
                    }
                }
            }

            public void Report(DownloadProgress value)
            {
                lock (reports)
                {
                    reports.Add(value);
                }
            }
        }
    }
}