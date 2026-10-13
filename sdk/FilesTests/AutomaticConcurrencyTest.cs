using FilesCom;
using FilesCom.Transfers;
using log4net.Appender;
using log4net.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WireMock;

namespace FilesTests
{
    // Automatic concurrency, observed on the wire: how many parts the loopback site is handling at once, when they
    // arrive, and the upload's own trace of what it measured and decided. The site's answers make adding connections
    // pay off, or not; they are answered in loopback time, which proves the mechanics, not production tuning.
    public partial class ParallelUploadTest
    {
        [TestMethod]
        [Timeout(60000)]
        [DataRow(0, 5, DisplayName = "Up to its maximum target")]
        [DataRow(4, 4, DisplayName = "Within the caller's MaxConcurrency")]
        public async Task AutomaticUploadSendsMorePartsAtOnceWhileThatRaisesThroughput(int maxConcurrency, int reached)
        {
            using (var server = new UploadServer())
            {
                // Parts are answered after 80 ms until the test has seen what it needs, then at once. The file has far
                // more parts than growing needs, so every target the upload chooses can be filled; a finite upload does
                // not otherwise promise to reach every target before it runs out of parts.
                var drain = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                server.RespondToPart = async request =>
                {
                    await Task.WhenAny(drain.Task, Task.Delay(80));
                    return UploadServer.Accept(request);
                };
                const int partCount = 400;
                byte[] contents = Pattern(8 * partCount);
                var trace = new TraceLog();
                var options = new UploadOptions
                {
                    AutomaticConcurrency = Tuning(start: 2, max: 5),
                    MaxConcurrency = maxConcurrency > 0 ? maxConcurrency : (int?)null,
                    Trace = trace,
                };

                Task<UploadResult> upload = Client(server).Transfers.UploadFileAsync(WriteFile("local.bin", contents), "remote.bin", options);
                await server.WaitUntil(s => trace.Decisions.Any(d => d.Decision == ConcurrencyDecision.Grew && d.Target == reached), $"the target grows to {reached}");
                TimeSpan grewAt = trace.ReceivedAt(trace.Decisions.First(d => d.Decision == ConcurrencyDecision.Grew && d.Target == reached));
                Stopwatch filling = Stopwatch.StartNew();
                while (server.MaxPartsInFlightBetween(grewAt, UploadServer.Clock) < reached && filling.Elapsed < TimeSpan.FromSeconds(3))
                {
                    await Task.Delay(10);
                }
                int unsent = partCount - server.Parts.Count;
                int filled = server.MaxPartsInFlightBetween(grewAt, UploadServer.Clock);
                drain.SetResult(true);
                UploadResult result = await Within(upload);

                Assert.AreEqual(reached, filled, $"Once the target was {reached}, with {unsent} parts still to send, that many were in flight: {trace.Describe()}");
                Assert.AreEqual(reached, server.MaxPartsInFlight, "The upload grew to its limit and no further: " + trace.Describe());
                UploadDecisionTrace[] grew = trace.Decisions.Where(d => d.Decision == ConcurrencyDecision.Grew).ToArray();
                Assert.IsTrue(grew.All(d => d.Target == d.PreviousTarget + 1 && d.Target <= reached), string.Join(", ", grew.Select(d => $"{d.PreviousTarget}->{d.Target}")));
                Assert.AreEqual(reached, grew.Max(d => d.Target));
                Assert.IsTrue(trace.Decisions.Any(d => d.Decision == ConcurrencyDecision.ProbeAccepted && d.ThroughputBytesPerSecond > d.BaselineBytesPerSecond * 1.1), "A raised target was kept because it measured more throughput.");
                CollectionAssert.AreEqual(contents, server.AcceptedBytes("remote.bin"));

                UploadConcurrency concurrency = result.FinalProgress.Concurrency;
                Assert.AreEqual(UploadConcurrencyMode.Automatic, concurrency.Mode);
                Assert.AreEqual(UploadPerformanceProfile.Generic, concurrency.Profile);
                Assert.AreEqual(reached, concurrency.MaxTarget);
                Assert.AreEqual(reached, concurrency.PeakActiveAttempts);
                Assert.IsTrue(concurrency.MeasuredWindows > 0 && concurrency.LastThroughputBytesPerSecond > 0);
                Assert.AreEqual(result.FinalProgress.AcknowledgedBytes, result.FinalProgress.SentBytes, "Nothing was sent twice.");
                Assert.IsTrue(result.FinalProgress.Process == null || result.FinalProgress.Process.WorkingSetBytes > 0, "Process resources are sampled for the whole process.");
            }
        }

        // The real upload of a file to loopback storage with two workers, where more connections only queue: every byte
        // arrives once, each raise adds one part and is kept or withdrawn to the target it came from, at least one is
        // withdrawn, and the destination never has more parts at once than the upload's highest target. Which raises are
        // kept depends on the host as well as on the workers. The upload's first window, the baseline of its first raise,
        // also carries what starting the transfer takes: in a CI run on net8.0 its first two parts took 115 and 117 ms
        // against 60 ms of service, though an upload through the same client had already run, and its later parts took 61
        // ms, so that first raise measured a gain the workers did not give and was kept. That a raise more connections do
        // not speed up is withdrawn, and never kept, is shown by RaisedTargetThatDoesNotRaiseThroughputIsWithdrawn, on a
        // clock the test controls.
        [TestMethod]
        [Timeout(60000)]
        public async Task RaisesOnADestinationThatCannotServeMorePartsAreKeptOrWithdrawnToWhereTheyCameFrom()
        {
            using (var server = new UploadServer())
            {
                // Two workers at the destination: more connections only queue there.
                AnswerThrough(server, workers: 2, TimeSpan.FromMilliseconds(60));
                byte[] contents = Pattern(8 * 64);
                var trace = new TraceLog();
                var options = new UploadOptions
                {
                    AutomaticConcurrency = WithdrawalTuning(),
                    Trace = trace,
                };

                UploadResult result = await Within(Client(server).Transfers.UploadFileAsync(WriteFile("local.bin", contents), "remote.bin", options));
                string evidence = trace.Evidence(server, "remote.bin");

                UploadDecisionTrace raise = null;
                foreach (UploadDecisionTrace decision in trace.Decisions.Where(d => d.Decision == ConcurrencyDecision.Grew || d.Decision == ConcurrencyDecision.ProbeRejected))
                {
                    if (decision.Decision == ConcurrencyDecision.Grew)
                    {
                        Assert.AreEqual(decision.PreviousTarget + 1, decision.Target, "Each raise adds one part.\n" + evidence);
                        raise = decision;
                        continue;
                    }
                    Assert.IsTrue(raise != null && decision.PreviousTarget == raise.Target && decision.Target == raise.PreviousTarget, "A withdrawn raise goes back to the target it came from.\n" + evidence);
                    raise = null;
                }
                UploadDecisionTrace[] rejected = trace.Decisions.Where(d => d.Decision == ConcurrencyDecision.ProbeRejected).ToArray();
                Assert.IsTrue(rejected.Length >= 1, "A raise was measured and withdrawn.\n" + evidence);
                Assert.AreEqual(rejected.Length, result.FinalProgress.Concurrency.ProbesRejected, evidence);
                int highest = trace.Decisions.Select(d => d.Target).Append(2).Max();
                Assert.IsTrue(server.MaxPartsInFlight <= highest, $"The destination never had more parts at once than the upload's highest target, {highest}; it had {server.MaxPartsInFlight}.\n{evidence}");
                CollectionAssert.AreEqual(contents, server.AcceptedBytes("remote.bin"));
            }
        }

        // The destination RaisesOnADestinationThatCannotServeMorePartsAreKeptOrWithdrawnToWhereTheyCameFrom stands in for,
        // on a clock the test controls (ControllerDriver): two workers, so that however many requests are in flight, a
        // window's requests take as long as the workers need for all of them, and it delivers the same throughput. The real
        // controller, with the same tuning, raises its target from 2 to 3 after a window, measures no more in the raised
        // target's windows, and withdraws it, each time: never more than 3 parts at once, and no raise kept.
        [TestMethod]
        public void RaisedTargetThatDoesNotRaiseThroughputIsWithdrawn()
        {
            var driver = new ControllerDriver(WithdrawalTuning());

            for (int round = 0; round < 32; round++)
            {
                // The target's requests, of 100 bytes each, served two at a time in 60 ms.
                driver.Window(TimeSpan.FromMilliseconds(30 * driver.Target));
            }

            string evidence = driver.Describe();
            List<UploadDecisionTrace> rejected = driver.Decisions.Where(d => d.Decision == ConcurrencyDecision.ProbeRejected).ToList();
            Assert.IsTrue(rejected.Count >= 2, "Raises were measured and withdrawn, again and again: " + evidence);
            Assert.IsTrue(rejected.All(d => d.PreviousTarget == 3 && d.Target == 2 && d.ThroughputBytesPerSecond < d.BaselineBytesPerSecond * 1.3), "Each went back to 2, short of the 30% more the tuning asks for: " + evidence);
            Assert.IsFalse(driver.Decisions.Any(d => d.Decision == ConcurrencyDecision.ProbeAccepted), "No raise was kept: " + evidence);
            Assert.IsTrue(driver.Decisions.All(d => d.Target <= 3), "Never more than 3 parts at once: " + evidence);
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(64L, 1e12, false, DisplayName = "Enough bytes, too slow")]
        [DataRow(1L << 40, 1.0, false, DisplayName = "Fast enough, too few bytes")]
        [DataRow(64L, 1.0, true, DisplayName = "Enough bytes, fast enough")]
        public async Task TargetGrowsPastItsSoftCeilingOnlyOnceTheUploadHasMovedEnoughBytesFastEnough(long unlockBytes, double unlockBytesPerSecond, bool unlocks)
        {
            using (var server = new UploadServer())
            {
                AnswerAfter(server, TimeSpan.FromMilliseconds(60));
                byte[] contents = Pattern(8 * 50);
                var trace = new TraceLog();
                var options = new UploadOptions
                {
                    AutomaticConcurrency = Tuning(start: 2, max: 5, t =>
                    {
                        t.SoftCeiling = 3;
                        t.SoftCeilingUnlockBytes = unlockBytes;
                        t.SoftCeilingUnlockBytesPerSecond = unlockBytesPerSecond;
                    }),
                    Trace = trace,
                };

                UploadResult result = await Within(Client(server).Transfers.UploadFileAsync(WriteFile("local.bin", contents), "remote.bin", options));

                if (unlocks)
                {
                    Assert.IsTrue(server.MaxPartsInFlight > 3, "Past the soft ceiling once both thresholds were met.");
                    Assert.IsTrue(trace.Decisions.Any(d => d.Decision == ConcurrencyDecision.SoftCeilingUnlocked));
                }
                else
                {
                    Assert.AreEqual(3, server.MaxPartsInFlight, "Up to the soft ceiling, and no further.");
                    Assert.IsFalse(trace.Decisions.Any(d => d.Decision == ConcurrencyDecision.SoftCeilingUnlocked));
                }
                Assert.AreEqual(unlocks, result.FinalProgress.Concurrency.SoftCeilingUnlocked);
                CollectionAssert.AreEqual(contents, server.AcceptedBytes("remote.bin"));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task ThrottledUploadPausesEveryPartRequestIncludingRetriesThenRecovers()
        {
            using (var server = new UploadServer())
            {
                AnswerAfter(server, TimeSpan.FromMilliseconds(60), request => request.Number == 10 && request.Attempt == 1 ? LoopbackSite.Status(429) : null);
                byte[] contents = Pattern(8 * 80);
                var trace = new TraceLog();
                var options = new UploadOptions
                {
                    AutomaticConcurrency = Tuning(start: 4, max: 4, t =>
                    {
                        t.BackpressurePause = TimeSpan.FromMilliseconds(700);
                        t.BackpressureShrinkPercent = 50;
                    }),
                    Trace = trace,
                };

                UploadResult result;
                string ordinaryLogs;
                using (var logs = new LogCapture())
                {
                    // The client's own retry delay is zero; only the cooldown holds the retry back.
                    result = await Within(Client(server, retries: 1).Transfers.UploadFileAsync(WriteFile("local.bin", contents), "remote.bin", options));
                    ordinaryLogs = logs.AboveDebug();
                }

                UploadDecisionTrace cooldown = trace.Decisions.Single(d => d.Decision == ConcurrencyDecision.CooldownStarted);
                Assert.AreEqual(TimeSpan.FromMilliseconds(700), cooldown.Cooldown);
                TimeSpan pausedAt = trace.ReceivedAt(cooldown);
                PartRequest[] duringCooldown = server.Parts.Where(p => p.ArrivedAt > pausedAt + TimeSpan.FromMilliseconds(100) && p.ArrivedAt < pausedAt + TimeSpan.FromMilliseconds(650)).ToArray();
                Assert.AreEqual(0, duringCooldown.Length, "No part request, new or retried, starts during the cooldown: " + string.Join(", ", duringCooldown.Select(p => $"{p.Number}#{p.Attempt}")));
                PartRequest retry = server.Parts.Single(p => p.Number == 10 && p.Attempt == 2);
                Assert.IsTrue(retry.ArrivedAt >= pausedAt + TimeSpan.FromMilliseconds(650), $"The throttled part's retry waited {(retry.ArrivedAt - pausedAt).TotalMilliseconds} ms.");

                UploadDecisionTrace[] shrank = trace.Decisions.Where(d => d.Decision == ConcurrencyDecision.ShrankAfterThrottling).ToArray();
                Assert.AreEqual("4->2", string.Join(" ", shrank.Select(d => $"{d.PreviousTarget}->{d.Target}")), "Throttling lowered the target once.");
                Assert.IsTrue(trace.Decisions.Any(d => d.Decision == ConcurrencyDecision.Grew && d.Target == 4 && trace.ReceivedAt(d) > pausedAt), "The target grew back after the cooldown.");
                Assert.AreEqual(4, server.MaxPartsInFlightBetween(pausedAt + TimeSpan.FromMilliseconds(700), UploadServer.Clock), "Four parts were in flight again later.");
                Assert.AreEqual(1, result.FinalProgress.Concurrency.Throttles);
                CollectionAssert.AreEqual(contents, server.AcceptedBytes("remote.bin"));

                foreach (string secret in new[] { "X-Amz", "Signature", "negotiation=" })
                {
                    Assert.IsFalse(ordinaryLogs.Contains(secret), $"Ordinary logs show {secret}: {ordinaryLogs}");
                    Assert.IsFalse(trace.Records.Any(record => record.GetType().GetProperties().Any(property => property.GetValue(record) is string text && text.Contains(secret))), $"A trace record shows {secret}.");
                }
                StringAssert.Contains(ordinaryLogs, "Throttled (HTTP 429) sending part 10");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task CancellingDuringACooldownEndsTheUploadPromptly()
        {
            using (var server = new UploadServer())
            {
                AnswerAfter(server, TimeSpan.FromMilliseconds(20), request =>
                {
                    if (request.Number != 3 || request.Attempt != 1)
                    {
                        return null;
                    }
                    ResponseMessage throttled = LoopbackSite.Status(429);
                    throttled.AddHeader("Retry-After", "30");
                    return throttled;
                });
                string localPath = WriteFile("local.bin", Pattern(8 * 10));
                var limiter = new TransferLimiter();
                var trace = new TraceLog();
                var cancellation = new CancellationTokenSource();

                Task<UploadResult> upload = Client(server, retries: 1).Transfers.UploadFileAsync(localPath, "remote.bin", new UploadOptions { Limiter = limiter, AutomaticConcurrency = Tuning(start: 2, max: 2), Trace = trace }, cancellation.Token);
                await server.WaitUntil(s => trace.Decisions.Any(d => d.Decision == ConcurrencyDecision.CooldownStarted), "the upload is cooling down");
                Assert.AreEqual(TimeSpan.FromSeconds(30), trace.Decisions.Single(d => d.Decision == ConcurrencyDecision.CooldownStarted).Cooldown, "The server's Retry-After sets the cooldown.");
                Stopwatch cancelled = Stopwatch.StartNew();
                cancellation.Cancel();

                await AssertCancelledPromptly(upload);
                Assert.IsTrue(cancelled.Elapsed < TimeSpan.FromSeconds(2), $"Ended {cancelled.Elapsed.TotalMilliseconds} ms after cancelling.");
                Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes + limiter.QueuedRequests + limiter.QueuedBufferReservations, "Nothing is held or waiting once the upload has ended.");
                using (new FileStream(localPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    // Opens only when no other handle to the file remains.
                }
                int events = server.Events.Count;
                await Task.Delay(300);
                Assert.AreEqual(events, server.Events.Count, "Nothing is sent after the upload ends.");
                Assert.AreEqual(0, server.Finishes.Count);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task WaitingForTheSharedLimiterNeitherGrowsTheTargetNorEndsItsProbes()
        {
            using (var fast = new UploadServer())
            using (var held = new UploadServer { HoldParts = true })
            {
                AnswerAfter(fast, TimeSpan.FromMilliseconds(60));
                var limiter = new TransferLimiter(maxConcurrentRequests: 8);
                // Another upload takes seven of the limiter's eight request slots and keeps them.
                Task<UploadResult> other = Client(held).Transfers.UploadFileAsync(WriteFile("other.bin", Pattern(8 * 7)), "other.bin", new UploadOptions { Limiter = limiter, ConcurrencyMode = UploadConcurrencyMode.Static, MaxConcurrency = 7 });
                await held.WaitUntil(s => s.PartsInFlight == 7, "the other upload holds seven request slots");
                var trace = new TraceLog();
                var progress = new ProgressLog();

                Task<UploadResult> upload = Client(fast).Transfers.UploadFileAsync(WriteFile("local.bin", Pattern(8 * 60)), "remote.bin", new UploadOptions { Limiter = limiter, AutomaticConcurrency = Tuning(start: 2, max: 6), Trace = trace, Progress = progress });
                await fast.WaitUntil(s => s.Parts.Count >= 20, "twenty parts have gone through the one free slot");
                UploadConcurrency starved = progress.Reports.Last().Concurrency;
                UploadDecisionTrace[] whileStarved = trace.Decisions.ToArray();
                TimeSpan released = UploadServer.Clock;
                held.ReleaseHeldParts();
                await Within(Task.WhenAll(upload, other));

                Assert.AreEqual(2, starved.Target, "Measurements taken while waiting for the limiter do not grow the target.");
                Assert.IsFalse(whileStarved.Any(d => d.Decision == ConcurrencyDecision.Grew || d.Decision == ConcurrencyDecision.ProbeRejected), "Nor do they establish a plateau: " + string.Join(", ", whileStarved.Select(d => d.Decision)));
                Assert.IsTrue(whileStarved.Count(d => d.Decision == ConcurrencyDecision.WindowNotMeasured && d.BelowTargetReason == BelowTargetReason.SharedLimiter) >= 3);
                Assert.IsTrue(starved.TimeWaitingForLimiter > starved.TimeAtTarget, "The snapshot says where the time went.");
                Assert.IsTrue(fast.MaxPartsInFlightBetween(released, released + TimeSpan.FromMilliseconds(50)) <= 3, "Once slots free up, the upload continues from its measured target instead of jumping ahead.");
                Assert.IsTrue(fast.MaxPartsInFlightBetween(TimeSpan.Zero, released) <= 1);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task WaitingForTheSourceNeitherGrowsTheTargetNorEndsItsProbes()
        {
            using (var server = new UploadServer())
            {
                AnswerAfter(server, TimeSpan.FromMilliseconds(10));
                byte[] contents = Pattern(8 * 30);
                // Each part takes 60 ms to read; the network answers in 10.
                var source = new SlowStream(contents, TimeSpan.FromMilliseconds(60));
                var trace = new TraceLog();

                UploadResult result = await Within(Client(server).Transfers.UploadStreamAsync(source, contents.Length, "remote.bin", new UploadOptions { AutomaticConcurrency = Tuning(start: 2, max: 6), Trace = trace }));

                UploadConcurrency concurrency = result.FinalProgress.Concurrency;
                Assert.AreEqual(2, concurrency.Target, "Measurements taken while no part was ready do not grow the target.");
                Assert.AreEqual(0, concurrency.Grows + concurrency.ProbesRejected);
                Assert.IsTrue(trace.Decisions.Count(d => d.Decision == ConcurrencyDecision.WindowNotMeasured && d.BelowTargetReason == BelowTargetReason.NoPartReady) >= 3);
                Assert.IsTrue(concurrency.TimeWithoutReadyPart > concurrency.TimeAtTarget);
                CollectionAssert.AreEqual(contents, server.AcceptedBytes("remote.bin"));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task StaticUploadKeepsItsConcurrencyWhateverItMeasures()
        {
            using (var server = new UploadServer())
            {
                TimeSpan throttledAt = TimeSpan.Zero;
                AnswerAfter(server, TimeSpan.FromMilliseconds(40), request =>
                {
                    if (request.Number != 5 || request.Attempt != 1)
                    {
                        return null;
                    }
                    throttledAt = UploadServer.Clock;
                    return LoopbackSite.Status(429);
                });
                byte[] contents = Pattern(8 * 30);

                UploadResult result = await Within(Client(server, retries: 1).Transfers.UploadFileAsync(WriteFile("local.bin", contents), "remote.bin", new UploadOptions { ConcurrencyMode = UploadConcurrencyMode.Static, MaxConcurrency = 3 }));

                Assert.AreEqual(3, server.MaxPartsInFlight, "Never more parts at once, although more would have been faster.");
                Assert.AreEqual(3, server.MaxPartsInFlightBetween(throttledAt, throttledAt + TimeSpan.FromMilliseconds(300)), "Nor fewer after throttling, and no pause.");
                Assert.IsTrue(server.Parts.Single(p => p.Number == 5 && p.Attempt == 2).ArrivedAt - throttledAt < TimeSpan.FromMilliseconds(300), "The retry is not held back beyond its own delay.");
                UploadConcurrency concurrency = result.FinalProgress.Concurrency;
                Assert.AreEqual(UploadConcurrencyMode.Static, concurrency.Mode);
                Assert.AreEqual("3 0 0 1", $"{concurrency.Target} {concurrency.Grows} {concurrency.Shrinks} {concurrency.Throttles}", "target grows shrinks throttles");
                Assert.IsNull(concurrency.LastDecision);
                CollectionAssert.AreEqual(contents, server.AcceptedBytes("remote.bin"));
            }
        }

        // A default upload straight to S3 warm-starts: until storage has answered parts that show how fast its link is,
        // it sends 1 part at a time rather than the scaled S3 policy's 150. Here every answer is held, though storage has
        // read every byte of the part it holds, so the bytes the upload wrote are no answer and it stays at 1; every byte
        // arrives once the answers come. The scaled S3 policy given as tuning, the policy the SDK chose started with
        // UploadStartup.Aggressive, and a static upload, send as many at once as they ask for from the start, within
        // MaxConcurrency. The wait after the parts arrive only gives an upload that did not hold back the time to send more.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("default", 1, DisplayName = "Default: 1 part at a time until storage answers")]
        [DataRow("scaled", 8, DisplayName = "The scaled S3 policy given as tuning: every part at once")]
        [DataRow("aggressive", 8, DisplayName = "Startup Aggressive: every part at once")]
        [DataRow("aggressive-capped", 5, DisplayName = "Startup Aggressive, with MaxConcurrency 5: 5 at once")]
        [DataRow("static", 3, DisplayName = "Static, with MaxConcurrency 3: 3 at once")]
        public async Task ADefaultUploadToS3SendsOnePartAtATimeUntilStorageAnswers(string tuning, int atOnce)
        {
            using (var server = new UploadServer { HoldParts = true })
            {
                UseAws(server, 1 * MiB, 4 * MiB, 10000, 1L << 30);
                byte[] contents = Pattern(8 * 4 * MiB);
                var options = new UploadOptions { AutomaticPartSizing = GrowingFrom(4 * MiB, max: 4 * MiB) };
                if (tuning == "scaled")
                {
                    options.AutomaticConcurrency = AutomaticConcurrencyOptions.ScaledS3();
                }
                else if (tuning.StartsWith("aggressive"))
                {
                    options.Startup = UploadStartup.Aggressive;
                    options.MaxConcurrency = tuning == "aggressive-capped" ? 5 : (int?)null;
                }
                else if (tuning == "static")
                {
                    options.ConcurrencyMode = UploadConcurrencyMode.Static;
                    options.MaxConcurrency = 3;
                }

                Task<UploadResult> upload = Client(server).Transfers.UploadFileAsync(WriteFile("local.bin", contents), "remote.bin", options);
                TimeSpan held;
                try
                {
                    await server.WaitUntil(s => s.PartsInFlight >= atOnce, $"{atOnce} parts are held");
                    await Task.Delay(250);
                    held = UploadServer.Clock;
                }
                finally
                {
                    server.ReleaseHeldParts();
                }
                UploadResult result = await Within(upload);

                Assert.AreEqual(atOnce, server.MaxPartsInFlightBetween(TimeSpan.Zero, held), "Parts at once before any was answered.");
                AssertExactUpload(server, result, contents);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task EachNetworkAttemptIsMeasuredOnItsOwnBeforeItsRetry()
        {
            using (var server = new UploadServer())
            {
                server.RespondToPart = async request =>
                {
                    if (request.Number != 2)
                    {
                        return UploadServer.Accept(request);
                    }
                    await Task.Delay(request.Attempt == 1 ? 150 : 50);
                    return request.Attempt == 1 ? LoopbackSite.Status(500) : UploadServer.Accept(request);
                };
                FilesConfiguration configuration = server.Site.Configuration(maxNetworkRetries: 1);
                var trace = new TraceLog();

                UploadResult result = await Within(FilesClient.Create(configuration).Transfers.UploadFileAsync(WriteFile("local.bin", Pattern(24)), "remote.bin", new UploadOptions { Trace = trace }));

                // After a server error, a part waits 100 ms before its retry.
                UploadAttemptTrace[] part2 = trace.Attempts.Where(a => a.PartNumber == 2).ToArray();
                Assert.AreEqual("1:500:False 2:200:True", string.Join(" ", part2.Select(a => $"{a.Attempt}:{a.StatusCode}:{a.Accepted}")));
                Assert.IsTrue(part2[0].Duration >= TimeSpan.FromMilliseconds(140) && part2[0].Duration < TimeSpan.FromMilliseconds(900), $"The failed attempt took {part2[0].Duration.TotalMilliseconds} ms on the network.");
                Assert.IsTrue(part2[1].Duration >= TimeSpan.FromMilliseconds(40) && part2[1].Duration < TimeSpan.FromMilliseconds(900), $"The retry took {part2[1].Duration.TotalMilliseconds} ms on the network, without the retry delay before it.");
                Assert.IsTrue(trace.ReceivedAt(part2[0]) < server.Parts.Single(p => p.Number == 2 && p.Attempt == 2).ArrivedAt - TimeSpan.FromMilliseconds(50), "The failed attempt was measured before its retry delay.");
                Assert.IsTrue(part2.All(a => a.PartBytes == 8 && a.SentBytes == 8));
                Assert.AreEqual("1:1 3:1", string.Join(" ", trace.Attempts.Where(a => a.PartNumber != 2).OrderBy(a => a.PartNumber).Select(a => $"{a.PartNumber}:{a.Attempt}")));
                Assert.AreEqual("24 32 1 1", $"{result.FinalProgress.AcknowledgedBytes} {result.FinalProgress.SentBytes} {result.FinalProgress.PartRetries} {result.FinalProgress.Concurrency.TransientFailures}", "acknowledged sent retries failures");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("MaxConcurrency=0")]
        [DataRow("MaxConcurrency=1025")]
        [DataRow("MaxReadyParts=0")]
        [DataRow("MaxReadyBytes=0")]
        [DataRow("Startup=7")]
        [DataRow("MinTarget=0")]
        [DataRow("MaxTarget=1025")]
        [DataRow("MinTarget>StartTarget")]
        [DataRow("StartTarget>SoftCeiling")]
        [DataRow("SoftCeiling>MaxTarget")]
        [DataRow("PlateauTarget>MaxTarget")]
        [DataRow("PlateauTarget<MinTarget")]
        [DataRow("SoftCeilingUnlockBytes=0")]
        [DataRow("SoftCeilingUnlockBytesPerSecond=NaN")]
        [DataRow("SoftCeilingUnlockBytesPerSecond=-1")]
        [DataRow("WindowAttempts=0")]
        [DataRow("GrowthIntervalWindows=0")]
        [DataRow("GrowthStep=0")]
        [DataRow("ProbeWindows=0")]
        [DataRow("HoldWindows=-1")]
        [DataRow("ProbeGainPercent=Infinity")]
        [DataRow("ProbeGainPerAddedPartPercent=-1")]
        [DataRow("ShrinkPercent=101")]
        [DataRow("BackpressureShrinkPercent=-1")]
        [DataRow("LatencyBackoffQueueLimit<LatencyGrowthQueueLimit")]
        [DataRow("BackpressurePause=-1s")]
        [DataRow("BackpressurePause=61s")]
        public void ConcurrencySettingsThatCannotWorkAreRefusedBeforeAnythingIsSent(string setting)
        {
            using (var server = new UploadServer())
            {
                var tuning = AutomaticConcurrencyOptions.For(UploadPerformanceProfile.S3);
                var options = new UploadOptions { AutomaticConcurrency = tuning };
                switch (setting)
                {
                    case "MaxConcurrency=0": options.MaxConcurrency = 0; break;
                    case "MaxConcurrency=1025": options.MaxConcurrency = 1025; break;
                    case "MaxReadyParts=0": options.MaxReadyParts = 0; break;
                    case "MaxReadyBytes=0": options.MaxReadyBytes = 0; break;
                    case "Startup=7": options.Startup = (UploadStartup)7; break;
                    case "MinTarget=0": tuning.MinTarget = 0; break;
                    case "MaxTarget=1025": tuning.MaxTarget = 1025; break;
                    case "MinTarget>StartTarget": tuning.MinTarget = tuning.StartTarget + 1; break;
                    case "StartTarget>SoftCeiling": tuning.StartTarget = tuning.SoftCeiling + 1; break;
                    case "SoftCeiling>MaxTarget": tuning.SoftCeiling = tuning.MaxTarget + 1; break;
                    case "PlateauTarget>MaxTarget": tuning.PlateauTarget = tuning.MaxTarget + 1; break;
                    case "PlateauTarget<MinTarget": tuning.PlateauTarget = tuning.MinTarget - 1; break;
                    case "SoftCeilingUnlockBytes=0": tuning.SoftCeilingUnlockBytes = 0; break;
                    case "SoftCeilingUnlockBytesPerSecond=NaN": tuning.SoftCeilingUnlockBytesPerSecond = double.NaN; break;
                    case "SoftCeilingUnlockBytesPerSecond=-1": tuning.SoftCeilingUnlockBytesPerSecond = -1; break;
                    case "WindowAttempts=0": tuning.WindowAttempts = 0; break;
                    case "GrowthIntervalWindows=0": tuning.GrowthIntervalWindows = 0; break;
                    case "GrowthStep=0": tuning.GrowthStep = 0; break;
                    case "ProbeWindows=0": tuning.ProbeWindows = 0; break;
                    case "HoldWindows=-1": tuning.HoldWindows = -1; break;
                    case "ProbeGainPercent=Infinity": tuning.ProbeGainPercent = double.PositiveInfinity; break;
                    case "ProbeGainPerAddedPartPercent=-1": tuning.ProbeGainPerAddedPartPercent = -1; break;
                    case "ShrinkPercent=101": tuning.ShrinkPercent = 101; break;
                    case "BackpressureShrinkPercent=-1": tuning.BackpressureShrinkPercent = -1; break;
                    case "LatencyBackoffQueueLimit<LatencyGrowthQueueLimit": tuning.LatencyBackoffQueueLimit = tuning.LatencyGrowthQueueLimit / 2; break;
                    case "BackpressurePause=-1s": tuning.BackpressurePause = TimeSpan.FromSeconds(-1); break;
                    case "BackpressurePause=61s": tuning.BackpressurePause = TimeSpan.FromSeconds(61); break;
                    default: Assert.Fail(setting); break;
                }
                string name = setting.Split('=', '<', '>')[0];
                string localPath = WriteFile("local.bin", Pattern(24));
                FilesClient client = Client(server);

                foreach (Func<Task> upload in new Func<Task>[]
                {
                    () => client.Transfers.UploadFileAsync(localPath, "remote.bin", options),
                    () => client.Transfers.UploadStreamAsync(new MemoryStream(Pattern(24)), 24, "remote.bin", options),
                })
                {
                    InvalidParameterException refused = null;
                    try
                    {
                        upload();
                    }
                    catch (InvalidParameterException e)
                    {
                        refused = e;
                    }
                    Assert.IsNotNull(refused, $"{setting} was accepted.");
                    StringAssert.Contains(refused.ParamName + " " + refused.Message, name);
                }
                Assert.AreEqual(0, server.Site.Requests.Count, "Nothing was sent, not even the first negotiation.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(429, DisplayName = "After throttling, it waits for the cooldown")]
        [DataRow(500, DisplayName = "After a failure, it waits for room under the lowered target")]
        public async Task RequestWaitingForTheLimiterStartsOnlyWhenTheUploadStillAllowsIt(int status)
        {
            using (var server = new UploadServer())
            {
                // Three request slots for a target of four: part 4 waits for one. Part 1 fails once that is so, which
                // gives part 4 its slot; parts 2 and 3 stay in flight until released.
                var limiter = new TransferLimiter(maxConcurrentRequests: 3);
                var failed = new TaskCompletionSource<TimeSpan>(TaskCreationOptions.RunContinuationsAsynchronously);
                var release = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                server.RespondToPart = async request =>
                {
                    if (request.Number == 1 && request.Attempt == 1)
                    {
                        await server.WaitUntil(s => s.PartsInFlight == 3 && limiter.QueuedRequests == 1, "three parts are in flight and a fourth waits for the limiter");
                        failed.SetResult(UploadServer.Clock);
                        return LoopbackSite.Status(status);
                    }
                    if (request.Number == 2 || request.Number == 3)
                    {
                        await release.Task;
                    }
                    return UploadServer.Accept(request);
                };
                byte[] contents = Pattern(8 * 5);
                var options = new UploadOptions
                {
                    Limiter = limiter,
                    AutomaticConcurrency = Tuning(start: 4, max: 4, t =>
                    {
                        // Throttling pauses without lowering the target, so only the cooldown can hold part 4 back; a
                        // failure lowers the target to the two parts still in flight.
                        t.BackpressurePause = TimeSpan.FromMilliseconds(900);
                        t.BackpressureShrinkPercent = 0;
                        t.ShrinkPercent = 50;
                    }),
                };

                Task<UploadResult> upload = Client(server, retries: 1).Transfers.UploadFileAsync(WriteFile("local.bin", contents), "remote.bin", options);
                TimeSpan failedAt = await Within(failed.Task);
                await Task.Delay(1300);
                PartRequest[] startedSince = server.Parts.Where(p => p.ArrivedAt > failedAt).ToArray();
                release.SetResult(true);
                UploadResult result = await Within(upload);

                string started = string.Join(", ", startedSince.Select(p => $"{p.Number}#{p.Attempt} at {(p.ArrivedAt - failedAt).TotalMilliseconds:0} ms"));
                if (status == 429)
                {
                    Assert.IsTrue(startedSince.Length > 0 && startedSince.All(p => p.ArrivedAt - failedAt >= TimeSpan.FromMilliseconds(800)), "Nothing started during the 900 ms cooldown, and the waiting parts started after it: " + started);
                }
                else
                {
                    Assert.AreEqual(0, startedSince.Length, "Nothing started while the two parts in flight filled the lowered target: " + started);
                }
                CollectionAssert.AreEqual(contents, server.AcceptedBytes("remote.bin"));
                Assert.AreEqual(2, result.Parts[0].Attempts);
                Assert.AreEqual(0, limiter.ActiveRequests + limiter.QueuedRequests + limiter.BufferedBytes);
            }
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task RetriesAreAdmittedBeforeNewPartsAndEachInTheOrderTheyAsked()
        {
            var controller = new ConcurrencyController(ConcurrencySettings.Static(1), "remote.bin", Stopwatch.GetTimestamp());
            using (var admission = new AttemptAdmission(controller, tracing: false))
            {
                AttemptAdmission.Attempt running = await admission.AdmitAsync(retry: false, CancellationToken.None);
                var cancellation = new CancellationTokenSource();
                var waiting = new Dictionary<Task<AttemptAdmission.Attempt>, string>();
                waiting.Add(admission.AdmitAsync(retry: true, CancellationToken.None), "retry 1");
                waiting.Add(admission.AdmitAsync(retry: false, CancellationToken.None), "new part 1");
                waiting.Add(admission.AdmitAsync(retry: true, CancellationToken.None), "retry 2");
                Task<AttemptAdmission.Attempt> cancelledRetry = admission.AdmitAsync(retry: true, cancellation.Token);
                waiting.Add(admission.AdmitAsync(retry: false, CancellationToken.None), "new part 2");
                cancellation.Cancel();
                await Assert.ThrowsExactlyAsync<TaskCanceledException>(() => cancelledRetry);
                var admitted = new List<string>();

                running.Dispose();
                while (waiting.Count > 0)
                {
                    Task<AttemptAdmission.Attempt> next = await Task.WhenAny(waiting.Keys);
                    admitted.Add(waiting[next]);
                    waiting.Remove(next);
                    Assert.AreEqual(0, waiting.Keys.Count(wait => wait.IsCompleted), "One attempt at a time, as each ends.");
                    (await next).Dispose();
                }

                Assert.AreEqual("retry 1, retry 2, new part 1, new part 2", string.Join(", ", admitted), "The cancelled retry took no turn.");
            }
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task FailureOfAnAttemptStartedAfterTheTargetWasLoweredLowersItAgain()
        {
            var controller = new ConcurrencyController(ConcurrencySettings.Automatic(Tuning(start: 8, max: 8).Validated(), null, UploadProfileSource.Options), "remote.bin", Stopwatch.GetTimestamp());
            using (var admission = new AttemptAdmission(controller, tracing: false))
            {
                AttemptAdmission.Attempt[] burst = { await admission.AdmitAsync(false, CancellationToken.None), await admission.AdmitAsync(false, CancellationToken.None) };
                AttemptAdmission.Attempt late = await admission.AdmitAsync(false, CancellationToken.None);
                foreach (AttemptAdmission.Attempt attempt in burst.Concat(new[] { late }))
                {
                    attempt.Hold(new RequestSlot(), TimeSpan.Zero);
                }
                Assert.IsTrue(burst.All(attempt => attempt.TryStart(TimeSpan.Zero)));

                // Both failures of attempts started together count once, lowering 8 to 4.
                burst[0].Complete(TemporaryFailure());
                burst[1].Complete(TemporaryFailure());
                Assert.AreEqual(4, controller.Target);
                // Admitted before the failures but started after, the last attempt belongs to the lowered target.
                Assert.IsTrue(late.TryStart(TimeSpan.Zero));
                late.Complete(TemporaryFailure());
                Assert.AreEqual(2, controller.Target);
            }
        }

        // A default upload to S3 warm-starts at 1 part (ConcurrencySettings.WarmStarts). Its attempts go through
        // AttemptAdmission, as an upload's do, each answered 20 ms after it starts, two kept in flight once the second window
        // at 1, its baseline of two answers, has raised the target to 2. While one of the two attempts started as it rose to 2 has not ended, the
        // windows the other slot's answers close judge nothing; once it has ended, answered or cancelled, the first window
        // that opens after that does.
        [TestMethod]
        [Timeout(10000)]
        [DataRow(true, DisplayName = "Answered")]
        [DataRow(false, DisplayName = "Cancelled")]
        public async Task AWarmStartJudgesARaiseOnlyOnceTheAttemptsStartedAsItRoseHaveEnded(bool answered)
        {
            var controller = new ConcurrencyController(S3Settings(null), "remote.bin", Stopwatch.GetTimestamp(), warmStart: true);
            using (var admission = new AttemptAdmission(controller, tracing: true))
            {
                var decisions = new List<UploadDecisionTrace>();
                var inFlight = new Queue<AttemptAdmission.Attempt>();
                async Task<AttemptAdmission.Attempt> Start()
                {
                    AttemptAdmission.Attempt attempt = await admission.AdmitAsync(false, CancellationToken.None);
                    attempt.Hold(new RequestSlot(), TimeSpan.Zero);
                    Assert.IsTrue(attempt.TryStart(TimeSpan.Zero));
                    return attempt;
                }
                void Answer(AttemptAdmission.Attempt attempt)
                {
                    decisions.AddRange(attempt.Complete(new AttemptSample(accepted: true, throttled: false, transient: false, retryAfter: null, bytes: 1, duration: TimeSpan.FromMilliseconds(1))).OfType<UploadDecisionTrace>());
                }
                // Answers the attempt in flight longest, after 20 ms, and starts another in its place unless that judged the
                // raise.
                async Task Turn()
                {
                    await Task.Delay(20);
                    int judged = decisions.Count;
                    Answer(inFlight.Dequeue());
                    if (decisions.Count == judged)
                    {
                        inFlight.Enqueue(await Start());
                    }
                }
                for (int i = 0; i < 3; i++)
                {
                    AttemptAdmission.Attempt first = await Start();
                    await Task.Delay(20);
                    Answer(first);
                }
                Assert.AreEqual("WarmStartRaised 1->2", string.Join(", ", decisions.Select(d => $"{d.Decision} {d.PreviousTarget}->{d.Target}")));
                AttemptAdmission.Attempt late = await Start();
                inFlight.Enqueue(await Start());

                for (int i = 0; i < 6 && decisions.Count == 1; i++)
                {
                    await Turn();
                }
                Assert.AreEqual(1, decisions.Count, "No window judged the raise while an attempt started as it rose was in flight: " + string.Join(", ", decisions.Select(d => d.Decision)));
                if (answered)
                {
                    Answer(late);
                }
                else
                {
                    late.Dispose();
                }
                inFlight.Enqueue(await Start());
                for (int i = 0; i < 6 && decisions.Count == 1; i++)
                {
                    await Turn();
                }

                Assert.IsTrue(decisions.Count > 1 && decisions[1].PreviousTarget == 2, "The first window after it ended judged the raise: " + string.Join(", ", decisions.Select(d => $"{d.Decision} {d.PreviousTarget}->{d.Target}")));
                while (inFlight.Count > 0)
                {
                    inFlight.Dequeue().Dispose();
                }
                late.Dispose();
            }
        }

        [TestMethod]
        public void PartsAddedAboveThePlateauMustEachEarnTheirGain()
        {
            var driver = new ControllerDriver(Tuning(start: 2, max: 4, t =>
            {
                t.WindowAttempts = 2;
                t.GrowthStep = 2;
                t.PlateauTarget = 2;
                t.ProbeGainPercent = 1;
                t.ProbeGainPerAddedPartPercent = 20;
                t.ProbeWindows = 1;
            }));

            driver.Window(TimeSpan.FromSeconds(1));
            Assert.AreEqual(4, driver.Target, "The first window grows the target by two.");
            // Four parts deliver 25 % more than two: more than ProbeGainPercent, less than 2 × 20 %.
            driver.Window(TimeSpan.FromSeconds(1.6));
            Assert.AreEqual(2, driver.Target);
            Assert.AreEqual(ConcurrencyDecision.ProbeRejected, driver.Decisions.Last().Decision);
        }

        [TestMethod]
        [DataRow(0.0, ConcurrencyDecision.GrowthHeldForLatency, 4, DisplayName = "Queuing past LatencyGrowthQueueLimit holds growth")]
        [DataRow(1.5, ConcurrencyDecision.ShrankForLatency, 2, DisplayName = "Queuing past LatencyBackoffQueueLimit, with less throughput, lowers the target")]
        public void RisingLatencyHoldsGrowthOrLowersTheTarget(double backoffQueueLimit, ConcurrencyDecision decision, int target)
        {
            var driver = new ControllerDriver(Tuning(start: 4, max: 8, t =>
            {
                t.WindowAttempts = 4;
                t.GrowthIntervalWindows = 10;
                t.LatencyGrowthQueueLimit = 1;
                t.LatencyBackoffQueueLimit = backoffQueueLimit;
            }));

            driver.Window(TimeSpan.FromSeconds(1));
            // The same four requests take twice as long per byte: an estimated two of them are queued.
            driver.Window(TimeSpan.FromSeconds(2));

            Assert.AreEqual(decision, driver.Decisions.Last().Decision);
            Assert.AreEqual(target, driver.Target);
        }

        // A default upload straight to S3, with the settings the SDK chooses for it (UploadOptions.UseDestination), over the
        // link of a slow connection, modeled on a clock the test controls (LinkDriver) after the one a live default upload
        // flooded: 7.8 MB/s, of which one request carries 6.8, for the 23 parts of 16 MiB of a 371 MB file, where each new
        // connection carries nothing for its first 3 seconds. Its first window, on cold connections, measures little more
        // than half of what the link carries, so a later one, on warm connections, gains from them whatever the number of
        // parts. Judging only windows on warm connections, one part at a time is its baseline; it tries one part more, whose
        // two requests only wait for each other, and goes back to 1: never more than 2 parts at once. The scaled S3 policy
        // given as tuning sends every part at once instead.
        [TestMethod]
        [DataRow(false, DisplayName = "Default: never more than 2 at once, ending at 1")]
        [DataRow(true, DisplayName = "The scaled S3 policy given as tuning: every part at once")]
        public void AWarmStartOnASlowLinkSendsAtMostOnePartMoreThanItsWarmConnectionsPaidFor(bool scaledGiven)
        {
            var driver = new LinkDriver(S3Settings(scaledGiven ? AutomaticConcurrencyOptions.ScaledS3() : null), Parts(371030394, 16 * MiB), linkBytesPerSecond: 7.8e6, requestBytesPerSecond: 6.8e6, connect: TimeSpan.FromSeconds(3));

            driver.Run(TimeSpan.FromMinutes(10));

            string evidence = driver.Describe();
            Assert.IsTrue(driver.Done, evidence);
            if (scaledGiven)
            {
                Assert.AreEqual(23, driver.PeakInFlight, "Every part at once: " + evidence);
                Assert.IsFalse(driver.Decisions.Any(IsWarmStart), evidence);
                return;
            }
            Assert.IsTrue(driver.PeakInFlight <= 2, evidence);
            Assert.AreEqual(1, driver.Target, evidence);
            Assert.IsFalse(driver.Decisions.Any(d => d.Decision == ConcurrencyDecision.WarmStartFastLink), evidence);
            Assert.AreEqual(1, driver.Snapshot.MinTarget, evidence);
        }

        // The default upload of a live run that went on to flood its slow link: 2 GiB in 128 parts of 16 MiB, over the link
        // of AWarmStartOnASlowLinkSendsAtMostOnePartMoreThanItsWarmConnectionsPaidFor. There, once its warm start had gone
        // back to 2, the policy itself grew it by 4 parts to 6, and parts failed; and starting at 2, which this link's one
        // request nearly fills, it could stay at 2, where both requests wait for each other. Instead, it measures one part at
        // a time, tries one part more until the two requests it then sends have been answered, the one on a new connection
        // after it connects, and then for at most the window under way and the next, which judges it, goes back to 1
        // because its two requests only waited, rests in windows of the policy's size, two of 32 answered parts, and tries
        // again, as often as the upload lasts: never more than 2 parts at once. Where the link becomes fast (2 GB/s,
        // 100 MB/s a request) while it rests, the first window of answered parts that shows it takes it to the policy's
        // start, 150, at once.
        [TestMethod]
        [DataRow(false, DisplayName = "Slow throughout: one part more, again and again, never more than 2 at once")]
        [DataRow(true, DisplayName = "Fast once it rests: the policy's start")]
        public void AWarmStartThatWentBackOnASlowLinkTriesOnlyOnePartMoreAgainUntilTheLinkIsFast(bool becomesFast)
        {
            var driver = new LinkDriver(S3Settings(null), Parts(2L << 30, 16 * MiB), linkBytesPerSecond: 7.8e6, requestBytesPerSecond: 6.8e6, connect: TimeSpan.FromSeconds(3));
            if (becomesFast)
            {
                driver.Run(TimeSpan.FromMinutes(10), () => driver.Decisions.Any(d => d.Decision == ConcurrencyDecision.WarmStartSettled));
                int peakWhileSlow = driver.PeakInFlight;
                driver.ChangeLink(2e9, 100e6);
                driver.Run(TimeSpan.FromMinutes(10));

                string fastEvidence = driver.Describe();
                Assert.AreEqual(2, peakWhileSlow, fastEvidence);
                UploadDecisionTrace promoted = driver.Decisions.Last(IsWarmStart);
                Assert.AreEqual($"{ConcurrencyDecision.WarmStartFastLink} 1->150", $"{promoted.Decision} {promoted.PreviousTarget}->{promoted.Target}", fastEvidence);
                Assert.AreEqual(8, driver.Snapshot.MinTarget, "The policy's minimum again: " + fastEvidence);
                Assert.IsTrue(driver.Done, fastEvidence);
                return;
            }

            driver.Run(TimeSpan.FromMinutes(10));

            string evidence = driver.Describe();
            Assert.IsTrue(driver.Done, evidence);
            Assert.IsTrue(driver.PeakInFlight <= 2, evidence);
            List<UploadDecisionTrace> changes = driver.Decisions.Where(d => d.Target != d.PreviousTarget).ToList();
            Assert.IsTrue(changes.All(d => (d.Decision == ConcurrencyDecision.WarmStartRaised && d.PreviousTarget == 1 && d.Target == 2) || (d.Decision == ConcurrencyDecision.WarmStartSettled && d.PreviousTarget == 2 && d.Target == 1)), "Only one part more at a time, and back: " + evidence);
            List<UploadDecisionTrace> raised = changes.Where(d => d.Decision == ConcurrencyDecision.WarmStartRaised).ToList();
            Assert.IsTrue(raised.Count >= 2, "It tried one part more again after resting: " + evidence);
            foreach (UploadDecisionTrace settled in changes.Where(d => d.Decision == ConcurrencyDecision.WarmStartSettled))
            {
                Assert.IsTrue(settled.BaselineBytesPerSecond > 0 && settled.ThroughputBytesPerSecond < settled.BaselineBytesPerSecond * 1.5, "Back, short of the 50% more a second part had to add to the window before it: " + evidence);
                Assert.IsTrue(settled.QueueEstimate >= 0.5, "Its two requests waited for each other: " + evidence);
                UploadDecisionTrace tried = raised.Last(d => d.Elapsed < settled.Elapsed);
                TimeSpan sentThenAnswered = driver.Requests.Where(r => r.Sent == tried.Elapsed).Max(r => r.Answered);
                Assert.IsTrue(driver.AnsweredBy(sentThenAnswered) - driver.AnsweredBy(tried.Elapsed) < 32, "The requests it sent then were answered within the policy's window: " + evidence);
                int windows = driver.MeasuredBy(settled.Elapsed) - driver.MeasuredBy(sentThenAnswered);
                Assert.IsTrue(settled.Elapsed >= sentThenAnswered && windows >= 1 && windows <= 2, $"Judged once the requests it sent then had been answered, by the window then under way or the next, not {windows} windows later: " + evidence);
                UploadDecisionTrace next = raised.FirstOrDefault(d => d.Elapsed > settled.Elapsed);
                Assert.IsTrue(next == null || driver.AnsweredBy(next.Elapsed) - driver.AnsweredBy(settled.Elapsed) >= 64, "It rested for two windows of 32 answered parts first: " + evidence);
            }
            Assert.AreEqual(1, driver.Snapshot.MinTarget, evidence);
        }

        // A later live default upload of 2 GiB in 128 parts of 16 MiB, over a link (LinkDriver) of 7.8 MB/s of which one
        // request carries 5.9, where its second request stays slow for its whole part. There, starting at 2, that slow
        // connection held the baseline down, so when it finished, the window at 3 delivered far more than the baseline,
        // though the third request only waited, and a part added was once kept on that gain alone. One part at a time,
        // the second request is in the baseline, the start target's second window; which, as the answers' times differ,
        // holds more answers until their variation is known, so the slow one does not make the baseline alone (as it once
        // did, when the window at 2 then delivered more than the 50% more a second part must add, though both its requests
        // only waited). Somewhat slow (3.6 MB/s), it holds the baseline down less than it would alone, and the window at 2
        // shows both its requests waiting: each byte takes much longer than in the fastest window at 1, and the queue
        // estimate is more than half a request. The second part goes back. Very slow (1.1 MB/s), it varies so much from
        // the answers after it that the baseline holds a window of the policy's size, 32 answers, before the warm start
        // tries one part more, which goes back for the same queue. Either way, no part is added from a window whose
        // requests were queued, and never more than 2 at once.
        [TestMethod]
        [DataRow(3.6e6, DisplayName = "A slow second request: the second part goes back for its queued requests")]
        [DataRow(1.1e6, DisplayName = "A very slow second request: no part more until a window of the policy's size, then back")]
        public void AWarmStartKeepsNoPartAddedWhileItsRequestsQueueWhateverTheThroughputGained(double slowRequestBytesPerSecond)
        {
            var driver = new LinkDriver(S3Settings(null), Parts(2L << 30, 16 * MiB), linkBytesPerSecond: 7.8e6, requestBytesPerSecond: 5.9e6, connect: TimeSpan.FromMilliseconds(100));
            driver.SlowRequest(2, slowRequestBytesPerSecond);

            driver.Run(TimeSpan.FromMinutes(10));

            string evidence = driver.Describe() + "; queue estimates " + string.Join(", ", driver.Decisions.Select(d => $"{d.Decision} {d.QueueEstimate:0.00}"));
            Assert.IsTrue(driver.Done, evidence);
            Assert.IsTrue(driver.PeakInFlight <= 2, evidence);
            List<UploadDecisionTrace> raised = driver.Decisions.Where(d => d.Decision == ConcurrencyDecision.WarmStartRaised).ToList();
            Assert.IsTrue(raised.Count > 0 && raised.All(d => d.QueueEstimate < 0.5), "Parts added only from windows whose requests were not queued: " + evidence);
            UploadDecisionTrace settled = driver.Decisions.FirstOrDefault(d => d.Decision == ConcurrencyDecision.WarmStartSettled);
            Assert.IsNotNull(settled, evidence);
            Assert.AreEqual("2->1", $"{settled.PreviousTarget}->{settled.Target}", evidence);
            Assert.IsTrue(settled.QueueEstimate >= 0.5, "Back for its queued requests: " + evidence);
            Assert.IsTrue(settled.BaselineBytesPerSecond > slowRequestBytesPerSecond * 1.2, "The slow request was not the baseline alone: " + evidence);
            if (slowRequestBytesPerSecond < 2e6)
            {
                Assert.IsTrue(driver.AnsweredBy(raised[0].Elapsed) >= 33, "No part more until the baseline held a window of the policy's size, 32 answered parts: " + evidence);
            }
            Assert.AreEqual(1, driver.Snapshot.MinTarget, evidence);
        }

        // The same link of 7.8 MB/s, of which one request carries 5.9, for 2 GiB in parts of 16 MiB. A second part only
        // waits there, so the warm start's first one part more goes back, and it rests at 1 for two windows of 32 answered
        // parts. In the second, four requests travel on a slow connection, of 2 MB/s, for their whole parts: they hold that
        // window's throughput, against which the warm start's next one part more is judged, down to some 4.7 MB/s, though
        // its other requests, most of them, still carry 5.9. The second part then delivers more than the 50% more it must
        // add to that window, only because the window was held down: both its requests only wait, each byte taking half
        // again as long, by the median of their answers, as in the fastest window at 1. It goes back for that queue, though
        // its gain alone would have kept it, and the upload never has more than 2 parts at once.
        [TestMethod]
        public void AWarmStartKeepsNoPartAddedOnTheGainOverAWindowASlowConnectionHeldDown()
        {
            var driver = new LinkDriver(S3Settings(null), Parts(2L << 30, 16 * MiB), linkBytesPerSecond: 7.8e6, requestBytesPerSecond: 5.9e6, connect: TimeSpan.FromMilliseconds(100));
            driver.Run(TimeSpan.FromMinutes(10), () => driver.Decisions.Any(d => d.Decision == ConcurrencyDecision.WarmStartSettled));
            UploadDecisionTrace first = driver.Decisions.Last();
            // Requests sent from the 40th after the first part goes back are answered in the rest's second window.
            for (int number = driver.Sent + 40; number < driver.Sent + 44; number++)
            {
                driver.SlowRequest(number, 2e6);
            }

            driver.Run(TimeSpan.FromMinutes(10));

            string evidence = driver.Describe() + "; " + string.Join(", ", driver.Decisions.Where(IsWarmStart).Select(d => $"{d.Decision} {d.PreviousTarget}->{d.Target} {d.ThroughputBytesPerSecond:0}/{d.BaselineBytesPerSecond:0} queue {d.QueueEstimate:0.00}"));
            Assert.IsTrue(driver.Done, evidence);
            Assert.AreEqual($"{ConcurrencyDecision.WarmStartSettled} 2->1", $"{first.Decision} {first.PreviousTarget}->{first.Target}", evidence);
            UploadDecisionTrace held = driver.Decisions.FirstOrDefault(d => d.Decision == ConcurrencyDecision.WarmStartSettled && d.Elapsed > first.Elapsed);
            Assert.IsNotNull(held, "The second part went back again: " + evidence);
            Assert.AreEqual("2->1", $"{held.PreviousTarget}->{held.Target}", evidence);
            Assert.IsTrue(held.ThroughputBytesPerSecond >= held.BaselineBytesPerSecond * 1.5, "Its gain over the window held down would have kept it: " + evidence);
            Assert.IsTrue(held.QueueEstimate >= 0.5, "Back for its queued requests: " + evidence);
            Assert.IsTrue(driver.PeakInFlight <= 2, evidence);
        }

        // A default upload straight to S3 of 64 parts of 1 MiB over a link (LinkDriver) of 7 MB/s of which one request
        // carries 2.92 MB/s: a part takes 0.36 s on a warm connection, and two parts at once deliver twice as much. The
        // request the warm start adds at 2 opens a connection, which carries nothing for connect. Answered 0.36 s later
        // than the others (about one part), it is judged as before. Answered 0.5 s later, with that time counted in its
        // request's duration as FilesApi.SendUploadPart times it, a window that holds its answer takes its connecting for
        // queueing (each byte taking longer than in the fastest window) though the window delivered twice as much; and
        // answered 1.1 s later, after two windows of the other request's answers, a window that closes before it is
        // answered credits that request alone, as much as one part at a time. Neither window judges the part added: the
        // first window that opened once both requests had been answered does, and the part is kept. Where the link carries
        // only what one request does, the part added still only waits, and goes. Where the request added is not answered
        // before the target has had a window of the policy's size, 32 answered parts, the window then closing judges it
        // as it is, and the part goes back: no raise waits on a request that may never be answered.
        [TestMethod]
        [DataRow(350, 7e6, "kept", DisplayName = "Answered about one part later: kept")]
        [DataRow(500, 7e6, "kept", DisplayName = "Its connecting counted in its request's time: kept, not taken for queueing")]
        [DataRow(1100, 7e6, "kept", DisplayName = "Answered after two windows of the other request's: kept")]
        [DataRow(500, 2.92e6, "withdrawn", DisplayName = "A link one request fills: withdrawn")]
        [DataRow(30000, 7e6, "judged without it", DisplayName = "Not answered within a window of the policy's size: judged without it")]
        public void AWarmStartJudgesAPartAddedOnceTheRequestsUnderWayWhenItWasAddedHaveBeenAnswered(int connectMilliseconds, double linkBytesPerSecond, string outcome)
        {
            var driver = new LinkDriver(S3Settings(null), Parts(64 * MiB, MiB), linkBytesPerSecond, requestBytesPerSecond: 2.92e6, connect: TimeSpan.FromMilliseconds(connectMilliseconds));

            driver.Run(TimeSpan.FromMinutes(10));

            string evidence = driver.Describe();
            Assert.IsTrue(driver.Done, evidence);
            UploadDecisionTrace raised = driver.Decisions.First(d => d.Decision == ConcurrencyDecision.WarmStartRaised);
            Assert.AreEqual("1->2", $"{raised.PreviousTarget}->{raised.Target}", evidence);
            UploadDecisionTrace withdrawn = driver.Decisions.FirstOrDefault(d => d.Decision == ConcurrencyDecision.WarmStartSettled && d.PreviousTarget == 2 && d.Target == 1);
            if (outcome == "kept")
            {
                Assert.IsNull(withdrawn, "The part added delivers its share once both requests are answered: " + evidence);
                Assert.IsTrue(driver.Decisions.Any(d => d.Decision == ConcurrencyDecision.WarmStartRaised && d.PreviousTarget == 2), "It tried one part more from 2: " + evidence);
                return;
            }
            Assert.IsNotNull(withdrawn, evidence);
            Assert.IsTrue(driver.PeakInFlight <= 2, evidence);
            if (outcome == "judged without it")
            {
                Assert.IsTrue(driver.AnsweredBy(withdrawn.Elapsed) - driver.AnsweredBy(raised.Elapsed) >= 32, "Judged once the target had a window of the policy's size: " + evidence);
                Assert.IsTrue(withdrawn.Elapsed < raised.Elapsed + TimeSpan.FromMilliseconds(connectMilliseconds), "Before the request added was answered: " + evidence);
            }
        }

        // The same default upload of 8 GiB in 32 MiB parts over a fast link (LinkDriver: 2 GB/s). Where each connection
        // moves 120 MB/s and connects in 40 ms, its first answered part already shows a fast link, and it goes straight to
        // the policy's start, 150, or MaxConcurrency. Where each moves 100 MB/s, just under the 96 MiB/s of the fast link,
        // its first window at 2, with the connection it added still cold, shows it: one part more first, after its
        // baseline of two answers. Where each moves
        // only 25 MB/s and takes a second to start carrying, the first window of each raised target, with the connection it
        // added still cold, delivers less than the window before; once that connection is warm, every part added delivers
        // its share, so it rises by half again as many parts at a time (one from 1 and from 2, two from 3) until a window
        // shows the link is fast. Either way it reaches the policy's start and is never left lower.
        [TestMethod]
        [DataRow(120e6, 40, null, 0, 1, DisplayName = "A connection fast enough alone: the policy's start after the first answer")]
        [DataRow(100e6, 40, null, 1, 5, DisplayName = "Connections just under the fast link alone: one part more first")]
        [DataRow(25e6, 1000, null, -1, -1, DisplayName = "Slower connections that warm up slowly: half again as many parts at a time to the policy's start")]
        [DataRow(120e6, 40, 12, 0, 1, DisplayName = "Within MaxConcurrency 12")]
        public void AWarmStartOnAFastLinkReachesThePolicysStart(double requestBytesPerSecond, int connectMilliseconds, int? maxConcurrency, int raises, int answeredAtStart)
        {
            var driver = new LinkDriver(S3Settings(null, maxConcurrency), Parts(8L << 30, 32 * MiB), linkBytesPerSecond: 2e9, requestBytesPerSecond: requestBytesPerSecond, connect: TimeSpan.FromMilliseconds(connectMilliseconds));

            driver.Run(TimeSpan.FromMinutes(5));

            string evidence = driver.Describe();
            int start = maxConcurrency ?? 150;
            List<UploadDecisionTrace> warm = driver.Decisions.Where(IsWarmStart).ToList();
            Assert.IsTrue(warm.Count > 0 && warm.Last().Target == start && warm.All(d => d.Decision != ConcurrencyDecision.WarmStartSettled), "Up to the policy's start, never settling below it: " + evidence);
            Assert.IsTrue(warm.Take(warm.Count - 1).All(d => d.Decision == ConcurrencyDecision.WarmStartRaised && d.Target == d.PreviousTarget + Math.Max(1, (d.PreviousTarget + 1) / 2)), "Only raises by half again, rounded up, before it: " + evidence);
            if (raises >= 0)
            {
                Assert.AreEqual(raises + 1, warm.Count, evidence);
                // In the first window that could show it: at 1, the first answer; after one raise, the 2 answers of the
                // first window at 2, which follow the 3 of the windows at 1 (the first, and the baseline of two).
                Assert.AreEqual(answeredAtStart, driver.AnsweredBy(warm.Last().Elapsed), "Decided on the first answers that could show it: " + evidence);
            }
            else
            {
                Assert.IsTrue(warm.Count > 2, evidence);
            }
            Assert.AreEqual(start, driver.PeakInFlight, evidence);
            Assert.IsTrue(driver.Done, evidence);
        }

        // A default upload straight to S3 of 1 GiB in parts of 1 MiB, as a cloud batch of 1,024 one-part files of 1 MiB sent
        // them, over a link (LinkDriver) of 2 GB/s where no request waits for another, but each takes as long to answer as
        // one of the cloud's requests did, from 44 to 108 ms (CloudAnswerTimes, twelve fixed draws); a new connection takes
        // 70 ms more. Eight parts at once carry the fast link's 96 MiB/s. There, judging its start by one answer at 1, and
        // each raised target by one answer per request, the warm start took a fast answer, or a window that showed a raise
        // by its variation alone, for the level the next raise fell short of; and against the fastest of all its windows,
        // short ones and those that ended before a request added was answered, later windows seemed queued: it took raises
        // back for variation alone, and stayed at 2 or 3 parts in eleven of the twelve draws, as the cloud upload stayed at
        // 2 to 4 for the 1,024 files. Judging windows that hold enough answers for that variation, its baseline at 1 two at
        // least, compared with windows of the target a raise came from by the median of their answers' times, and raising
        // by half again, it reaches the policy's start once its answered parts show the fast link, within a quarter of the
        // upload, taking back at most one raise for variation alone.
        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        [DataRow(7)]
        [DataRow(8)]
        [DataRow(9)]
        [DataRow(10)]
        [DataRow(11)]
        [DataRow(12)]
        public void AWarmStartOnAFastLinkWhoseAnswersVaryReachesThePolicysStart(int draw)
        {
            var driver = new LinkDriver(S3Settings(null), Parts(1L << 30, MiB), linkBytesPerSecond: 2e9, requestBytesPerSecond: 1e9, connect: TimeSpan.FromMilliseconds(70));
            driver.VaryRequests(CloudAnswerTimes(draw, 1024, TimeSpan.FromMilliseconds(1)));

            driver.Run(TimeSpan.FromMinutes(5));

            List<UploadDecisionTrace> warm = driver.Decisions.Where(IsWarmStart).ToList();
            string evidence = driver.Describe() + "; " + string.Join(", ", warm.Select(d => $"{d.Decision} {d.PreviousTarget}->{d.Target} at {driver.AnsweredBy(d.Elapsed)} answered, queue {d.QueueEstimate:0.00}"));
            Assert.IsTrue(driver.Done, evidence);
            Assert.IsTrue(warm.Count > 0 && driver.AnsweredBy(warm[0].Elapsed) >= 3, "No part more before the baseline held two answers after the first: " + evidence);
            Assert.IsTrue(warm.Count(d => d.Decision == ConcurrencyDecision.WarmStartSettled) <= 1, "At most one raise taken back for the answers' variation: " + evidence);
            UploadDecisionTrace promoted = warm.LastOrDefault();
            Assert.IsTrue(promoted != null && promoted.Decision == ConcurrencyDecision.WarmStartFastLink && promoted.Target == 150, "The policy's start, once answered parts showed the fast link: " + evidence);
            Assert.IsTrue(driver.AnsweredBy(promoted.Elapsed) <= 256, "Within a quarter of the upload's 1,024 parts: " + evidence);
            Assert.AreEqual(150, driver.PeakInFlight, evidence);
        }

        // A default upload straight to S3 of 2 GiB in parts of 32 MiB over a link (LinkDriver) of 2 GB/s, where a request
        // carries its part in some 280 ms, 120 MB/s, but each takes as long as the times of CloudAnswerTimes scaled to that,
        // from 190 to 465 ms, and a new connection takes 300 ms to start carrying, so that the first answer, on a cold
        // connection, does not show the fast link. The windows it judges hold more answers for that variation, up to 32,
        // but the first answers that show the fast link still decide: it goes to the policy's start once the answers of
        // the baseline's window, so far, have delivered 96 MiB/s, within its first six answers, not once that window is
        // over (which, in seven of these eight draws, would be after 9 to 29).
        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        [DataRow(7)]
        [DataRow(8)]
        public void AWarmStartWhoseAnswersVaryTakesTheFastLinkFromTheFirstAnswersThatShowIt(int draw)
        {
            var driver = new LinkDriver(S3Settings(null), Parts(2L << 30, 32 * MiB), linkBytesPerSecond: 2e9, requestBytesPerSecond: 1e9, connect: TimeSpan.FromMilliseconds(300));
            driver.VaryRequests(CloudAnswerTimes(draw, 64, TimeSpan.FromMilliseconds(280.0 / 65)));

            driver.Run(TimeSpan.FromMinutes(5));

            List<UploadDecisionTrace> warm = driver.Decisions.Where(IsWarmStart).ToList();
            string evidence = driver.Describe();
            Assert.IsTrue(driver.Done, evidence);
            Assert.AreEqual($"{ConcurrencyDecision.WarmStartFastLink} 1->150", string.Join(", ", warm.Select(d => $"{d.Decision} {d.PreviousTarget}->{d.Target}")), evidence);
            Assert.IsTrue(driver.AnsweredBy(warm[0].Elapsed) <= 6, "Decided by the first answers that showed it: " + evidence);
        }

        // How long the requests of a cloud batch of one-part files of 1 MiB took to answer, from the time each was sent, in
        // milliseconds, each times scale, count of them: each the fifth to the ninety-fifth percentile of one upload's
        // 1,024, a tenth apart, as a fixed sequence of pseudo-random draws picks them (a linear congruential generator from
        // draw), so that the answers vary from one to the next as the cloud's did, and every run of the test draws the same.
        private static List<TimeSpan> CloudAnswerTimes(int draw, int count, TimeSpan scale)
        {
            int[] percentiles = { 44, 47, 51, 56, 60, 65, 71, 78, 89, 108 };
            var times = new List<TimeSpan>();
            uint state = (uint)draw;
            for (int i = 0; i < count; i++)
            {
                state = (state * 1664525) + 1013904223;
                times.Add(TimeSpan.FromTicks(scale.Ticks * percentiles[(state >> 16) % percentiles.Length]));
            }
            return times;
        }

        // Two separate uploads. In the first, a failure before any answer has shown how fast the link is lowers the target
        // to 1, and the warm start rests there: it tries one part more only after windows of the policy's size, two of 32
        // answered parts, since one request alone delivers just under 96 MiB/s; at 2 its answered parts show the fast
        // link, and it goes to the policy's start, where failures lower it only to the policy's minimum. In the second, the
        // first answer shows a fast link (120 MB/s), so it is at the policy's start with the policy's minimum, and a failure
        // lowers it as the policy would without a warm start.
        [TestMethod]
        public void AFailureDuringAWarmStartLowersItAsFarAs1WhereItRestsBeforeTryingOnePartMore()
        {
            var failed = new LinkDriver(S3Settings(null), Parts(8L << 30, 32 * MiB), 2e9, 100e6, TimeSpan.Zero);
            failed.Admit();
            failed.Fail();
            Assert.AreEqual("1 1", $"{failed.Target} {failed.Snapshot.MinTarget}", "target, lowest target: " + failed.Describe());
            failed.Run(TimeSpan.FromMinutes(5));
            List<UploadDecisionTrace> warm = failed.Decisions.Where(IsWarmStart).ToList();
            Assert.IsTrue(warm.Count > 0 && warm[0].Decision == ConcurrencyDecision.WarmStartRaised && warm[0].PreviousTarget == 1 && warm[0].Target == 2, "One part more first: " + failed.Describe());
            Assert.IsTrue(failed.AnsweredBy(warm[0].Elapsed) >= 64, "After resting for two windows of 32 answered parts: " + failed.Describe());
            Assert.AreEqual($"{ConcurrencyDecision.WarmStartFastLink} 150 8", $"{warm.Last().Decision} {failed.Target} {failed.Snapshot.MinTarget}", "The policy's start, with its minimum, once answers show the fast link: " + failed.Describe());

            var fast = new LinkDriver(S3Settings(null), Parts(8L << 30, 32 * MiB), 2e9, 120e6, TimeSpan.Zero);
            fast.Run(TimeSpan.FromSeconds(1));
            Assert.AreEqual("150 8", $"{fast.Target} {fast.Snapshot.MinTarget}", "target, lowest target: " + fast.Describe());
            fast.Fail();
            Assert.AreEqual(97, fast.Target, "35% lower, as the policy lowers it: " + fast.Describe());
        }

        // A default upload straight to S3 that chooses its part sizes as the SDK does, 4 GiB in 16 MiB parts to start with,
        // over a slow link (LinkDriver) of 7 MB/s, of which one request carries 6.8, where storage takes 0.8 s to answer
        // each part. A second part never adds the 50% more its warm start asks for, so it goes back to 1 and rests; larger
        // parts do pay, 32 MiB parts 14% more than 16 MiB ones. So while it rests, and only then, it tries larger parts: a
        // size probe starts in a rest's hold window, and its windows leave the rest where it was, so no part is added while
        // it is under way and no size changes while a part added is judged, and the rest still ends in one part more, tried
        // from a window of the size the probe settled on. Where larger parts never pay (no wait to answer, 16 GiB in 32 MiB
        // parts, the 7.8 MB/s link of AWarmStartThatWentBackOnASlowLinkTriesOnlyOnePartMoreAgainUntilTheLinkIsFast) and the
        // caller's sizing starts a probe as soon as it may, every rest still tries one part more after its one size probe.
        // Where the link becomes fast as a size probe's first part is answered, the window of the probe's parts shows it and
        // takes the upload to the policy's start, and the size goes back.
        [TestMethod]
        [DataRow("slow", DisplayName = "Slow throughout: larger parts tried only while it rests")]
        [DataRow("eager", DisplayName = "Size probes that never pay, started as soon as allowed: one part more after every rest")]
        [DataRow("fast", DisplayName = "Fast during a size probe: the policy's start from the probe's window, and the size goes back")]
        public void AWarmStartTriesLargerPartsOnlyWhileItRests(string link)
        {
            bool eager = link == "eager";
            long total = eager ? 16L << 30 : 4L << 30;
            AutomaticPartSizingOptions sizing = eager ? new AutomaticPartSizingOptions { GrowthIntervalWindows = 1, ProbeWindows = 1, HoldWindows = 0 } : new AutomaticPartSizingOptions();
            PartSizes plan = PartSizes.Plan(UploadGrant.AwsLimits, total, null, null, PartSizingSettings.Of(UploadPartSizingMode.Automatic, null, sizing), null, "remote.bin", Stopwatch.Frequency, out ServerPlannedReason fallback);
            Assert.AreEqual(ServerPlannedReason.None, fallback);
            var driver = new LinkDriver(S3Settings(null), plan, total, linkBytesPerSecond: eager ? 7.8e6 : 7e6, requestBytesPerSecond: 6.8e6, connect: TimeSpan.FromSeconds(3), answerWait: TimeSpan.FromSeconds(eager ? 0 : 0.8));
            int answeredAtChange = -1;
            if (link == "fast")
            {
                driver.Run(TimeSpan.FromHours(1), () => driver.AnsweredLengths.Contains(32 * MiB));
                answeredAtChange = driver.AnsweredLengths.Count;
                driver.ChangeLink(2e9, 1e9, TimeSpan.Zero);
            }

            driver.Run(TimeSpan.FromHours(1));

            string evidence = driver.Describe();
            Assert.IsTrue(driver.Done, evidence);
            // While it warm-starts, never a size decision while one part more is being judged, nor one part more while a size
            // probe is under way.
            bool partAdded = false;
            bool sizeProbe = false;
            foreach (UploadTraceEvent record in driver.Records.TakeWhile(r => !(r is UploadDecisionTrace d && d.Decision == ConcurrencyDecision.WarmStartFastLink)))
            {
                if (record is UploadSizingTrace size)
                {
                    Assert.IsFalse(partAdded, $"No size changed while one part more was judged ({size.Elapsed}): " + evidence);
                    sizeProbe = size.Decision == PartSizeDecision.Grew;
                }
                else if (record is UploadDecisionTrace target && IsWarmStart(target))
                {
                    Assert.IsFalse(sizeProbe && target.Decision == ConcurrencyDecision.WarmStartRaised, $"No part added during a size probe ({target.Elapsed}): " + evidence);
                    partAdded = target.Decision == ConcurrencyDecision.WarmStartRaised;
                }
            }
            List<UploadSizingTrace> sizes = driver.Records.OfType<UploadSizingTrace>().ToList();
            List<UploadDecisionTrace> raised = driver.Decisions.Where(d => d.Decision == ConcurrencyDecision.WarmStartRaised).ToList();
            if (link == "slow")
            {
                Assert.IsTrue(driver.PeakInFlight <= 2, evidence);
                CollectionAssert.AreEqual(new[] { "Grew 32", "ProbeAccepted 32" }, sizes.Take(2).Select(s => $"{s.Decision} {s.Size / MiB}").ToArray(), "Larger parts, kept for what they delivered: " + evidence);
                Assert.IsTrue(sizes[0].Elapsed > driver.Decisions.First(d => d.Decision == ConcurrencyDecision.WarmStartSettled).Elapsed, "Only once it had gone back and rested: " + evidence);
                UploadDecisionTrace next = raised.FirstOrDefault(d => d.Elapsed > sizes[1].Elapsed);
                Assert.IsNotNull(next, "One part more again, from a window after the size probe's: " + evidence);
                Assert.IsTrue(next.ThroughputBytesPerSecond >= sizes[1].BaselineBytesPerSecond * 1.1, "Tried from a window of the larger parts: " + evidence);
                Assert.IsTrue(driver.AnsweredLengths.Count(length => length == 32 * MiB) >= 32, evidence);
            }
            else if (eager)
            {
                Assert.IsTrue(driver.PeakInFlight <= 2, evidence);
                Assert.IsTrue(raised.Count >= 3, "One part more again after every rest: " + evidence);
                foreach (UploadDecisionTrace settled in driver.Decisions.Where(d => d.Decision == ConcurrencyDecision.WarmStartSettled))
                {
                    UploadDecisionTrace again = raised.FirstOrDefault(d => d.Elapsed > settled.Elapsed);
                    TimeSpan restEnd = again?.Elapsed ?? TimeSpan.MaxValue;
                    Assert.IsTrue(sizes.Count(s => s.Decision == PartSizeDecision.Grew && s.Elapsed > settled.Elapsed && s.Elapsed < restEnd) <= 1, $"One size probe a rest ({settled.Elapsed}): " + evidence);
                }
                Assert.IsTrue(sizes.Count(s => s.Decision == PartSizeDecision.Grew) >= 2 && sizes.All(s => s.Decision != PartSizeDecision.ProbeAccepted), evidence);
            }
            else
            {
                UploadDecisionTrace promoted = driver.Decisions.Single(d => d.Decision == ConcurrencyDecision.WarmStartFastLink);
                Assert.AreEqual("1->150", $"{promoted.PreviousTarget}->{promoted.Target}", evidence);
                Assert.AreEqual(32, driver.AnsweredBy(promoted.Elapsed) - answeredAtChange, "The window of the probe's parts showed it: " + evidence);
                var interrupted = driver.Records.SkipWhile(record => record != promoted).Skip(1).FirstOrDefault() as UploadSizingTrace;
                Assert.AreEqual("ProbeRejected 32->16", interrupted == null ? "none" : $"{interrupted.Decision} {interrupted.PreviousSize / MiB}->{interrupted.Size / MiB}", "The size went back: " + evidence);
                Assert.IsFalse(sizes.TakeWhile(s => s != interrupted).Any(s => s.Decision == PartSizeDecision.ProbeAccepted), evidence);
                Assert.AreEqual(8, driver.Snapshot.MinTarget, evidence);
            }
        }

        private static bool IsWarmStart(UploadDecisionTrace decision)
        {
            return decision.Decision == ConcurrencyDecision.WarmStartRaised || decision.Decision == ConcurrencyDecision.WarmStartFastLink || decision.Decision == ConcurrencyDecision.WarmStartSettled;
        }

        // The settings an automatic upload to S3 settles on (UploadOptions.UseDestination): the SDK's choice, or tuning.
        private static ConcurrencySettings S3Settings(AutomaticConcurrencyOptions tuning, int? maxConcurrency = null)
        {
            UploadOptions options = new UploadOptions { AutomaticConcurrency = tuning, MaxConcurrency = maxConcurrency }.Snapshot();
            options.UseDestination(UploadPerformanceProfile.S3);
            return options.Concurrency;
        }

        // The lengths of a file's parts of size bytes, the last holding the rest.
        private static IEnumerable<long> Parts(long length, long size)
        {
            for (long offset = 0; offset < length; offset += size)
            {
                yield return Math.Min(size, length - offset);
            }
        }

        private static AttemptSample TemporaryFailure()
        {
            return new AttemptSample(accepted: false, throttled: false, transient: true, retryAfter: null, bytes: 8, duration: TimeSpan.FromMilliseconds(10));
        }

        // Stands in for a limiter request slot.
        private sealed class RequestSlot : IDisposable
        {
            public void Dispose()
            {
            }
        }

        // Feeds an automatic upload's controller whole windows of successful requests on a clock it controls: every
        // window runs the target's requests together, each delivering 100 bytes over the window's duration. As a shared
        // directory's controller, it can have files' size controllers attached in order (AttachSizing) and be fed rounds
        // of the first file's parts (SizedRound), in the sizes it chooses, each request taking exactly its bytes at a
        // given rate, or shared rounds of every attached file's parts (SharedRound), each request taking the same time
        // whatever its size.
        private sealed class ControllerDriver
        {
            private readonly ConcurrencyController controller;
            private readonly long startedAt = Stopwatch.Frequency;
            private long now = Stopwatch.Frequency;
            // The first attached file's size controller: the one SizedRound feeds and the shared scope judges.
            private PartSizeController sizer;
            // Every attached file in attach order, with the sizes of its parts SharedRound sent, in the order sent.
            private readonly List<(string Path, PartSizeController Sizer, List<long> Sent)> files = new List<(string Path, PartSizeController Sizer, List<long> Sent)>();
            // When each of the first attached file's parts was answered, on the clock the trace's Elapsed uses.
            private readonly List<TimeSpan> answered = new List<TimeSpan>();

            public ControllerDriver(AutomaticConcurrencyOptions tuning)
                : this(ConcurrencySettings.Automatic(tuning.Validated(), null, UploadProfileSource.Options), warmStart: false)
            {
            }

            // A shared directory's controller with the settings an upload settled on, warm-starting as a directory upload's
            // scope does when they say so (ConcurrencySettings.WarmStarts).
            public ControllerDriver(ConcurrencySettings settings, bool warmStart)
            {
                controller = new ConcurrencyController(settings, "remote.bin", now, warmStart: warmStart);
            }

            private readonly List<UploadTraceEvent> records = new List<UploadTraceEvent>();

            public IReadOnlyList<UploadDecisionTrace> Decisions => records.OfType<UploadDecisionTrace>().ToList();

            public IReadOnlyList<UploadSizingTrace> Sizing => records.OfType<UploadSizingTrace>().ToList();

            public int Target => controller.Target;

            // Attaches the size controller of a file of the shared scope, which chooses sizes from start up to ceiling, as
            // a directory transfer attaches each of its files that chooses its sizes. The first attached stays the one
            // SizedRound feeds and PartsAnswered counts.
            public void AttachSizing(AutomaticPartSizingOptions sizing, long start, long ceiling, string path = "up/a.bin")
            {
                var file = new PartSizeController(sizing.Validated(), start, ceiling, path, startedAt);
                files.Add((path, file, new List<long>()));
                if (sizer == null)
                {
                    sizer = file;
                }
                controller.AttachSizing(now, file);
            }

            // The size the attached file at path chooses now.
            public long SizeOf(string path)
            {
                return files.First(file => file.Path == path).Sizer.Current.Size;
            }

            // The sizes of the attached file's parts at path that SharedRound sent, in the order sent.
            public IReadOnlyList<long> SentSizes(string path)
            {
                return files.First(file => file.Path == path).Sent;
            }

            // One round of the target's requests, shared by the attached files in turn in attach order (request i is a
            // part of file i % count), each part in the size its file chooses as the round starts: they start together,
            // all take exactly duration whatever their sizes, and they are answered together in request order.
            public void SharedRound(TimeSpan duration)
            {
                int requests = controller.Target;
                int epoch = controller.Epoch;
                List<PartSizeController.SizeChoice> sizes = files.Select(file => file.Sizer.Current).ToList();
                List<long> attempts = Send(requests);
                now += (long)(duration.TotalSeconds * Stopwatch.Frequency);
                for (int i = 0; i < requests; i++)
                {
                    var file = files[i % files.Count];
                    PartSizeController.SizeChoice size = sizes[i % files.Count];
                    controller.Record(now, epoch, new AttemptSample(accepted: true, throttled: false, transient: false, retryAfter: null, bytes: size.Size, duration: duration, sizeEpoch: size.Epoch, sizer: file.Sizer), records);
                    controller.Released(now, sending: true, attempts[i]);
                    file.Sent.Add(size.Size);
                    if (file.Sizer == sizer)
                    {
                        answered.Add(TimeSpan.FromSeconds((now - startedAt) / (double)Stopwatch.Frequency));
                    }
                }
            }

            // One round of the target's requests, all of them parts of the first attached file in the size it chooses
            // now: they start together, each takes exactly its bytes at bytesPerSecond, however many are in flight, and
            // they are answered together. Parts answered after the file's size changed keep the size they were sent with.
            public void SizedRound(double bytesPerSecond)
            {
                int requests = controller.Target;
                int epoch = controller.Epoch;
                PartSizeController.SizeChoice size = sizer.Current;
                List<long> attempts = Send(requests);
                TimeSpan duration = TimeSpan.FromSeconds(size.Size / bytesPerSecond);
                now += (long)(duration.TotalSeconds * Stopwatch.Frequency);
                for (int i = 0; i < requests; i++)
                {
                    controller.Record(now, epoch, new AttemptSample(accepted: true, throttled: false, transient: false, retryAfter: null, bytes: size.Size, duration: duration, sizeEpoch: size.Epoch, sizer: sizer), records);
                    controller.Released(now, sending: true, attempts[i]);
                    answered.Add(TimeSpan.FromSeconds((now - startedAt) / (double)Stopwatch.Frequency));
                }
            }

            // How many of the first attached file's parts were answered after from and no later than to.
            public int PartsAnswered(TimeSpan from, TimeSpan to)
            {
                return answered.Count(at => at > from && at <= to);
            }

            public string Describe()
            {
                return string.Join("; ", records.Select(record => record switch
                {
                    UploadDecisionTrace d => $"{d.Elapsed.TotalMilliseconds:0.###}ms target {d.Decision} {d.PreviousTarget}->{d.Target} {d.ThroughputBytesPerSecond:0}",
                    UploadSizingTrace s => $"{s.Elapsed.TotalMilliseconds:0.###}ms size {s.Decision} {s.PreviousSize}->{s.Size} {s.ThroughputBytesPerSecond:0}/{s.BaselineBytesPerSecond:0}",
                    _ => record.GetType().Name,
                }));
            }

            public void Window(TimeSpan duration)
            {
                int requests = controller.Target;
                int epoch = controller.Epoch;
                List<long> attempts = Send(requests);
                now += (long)(duration.TotalSeconds * Stopwatch.Frequency);
                for (int i = 0; i < requests; i++)
                {
                    controller.Record(now, epoch, new AttemptSample(accepted: true, throttled: false, transient: false, retryAfter: null, bytes: 100, duration: duration), records);
                    controller.Released(now, sending: true, attempts[i]);
                }
            }

            // Starts requests together, and returns the number the controller gave each.
            private List<long> Send(int requests)
            {
                var attempts = new List<long>();
                for (int i = 0; i < requests; i++)
                {
                    controller.Granted(now);
                    attempts.Add(controller.Sending(now));
                }
                return attempts;
            }
        }

        // Sends one upload's parts through its real controller over a modeled link, on a clock the test controls, a
        // millisecond at a time, as AttemptAdmission would: a part starts while fewer than the target are admitted and
        // being sent, and nothing is paused; its answer is recorded and its permit given back. A request that finds no
        // connection idle opens one, which carries nothing for connect (connecting and warming up); an answered request
        // leaves its connection idle, and the next request carries on it at once. Each request has its share of the link,
        // never more than requestBytesPerSecond, and is answered once the link has carried all of it, with the time it
        // since it was sent, its connecting included, as its duration, as FilesApi.SendUploadPart times it. The controller
        // warm-starts as an upload's own does when the settings say so
        // (ConcurrencySettings.WarmStarts). An upload that chooses its part sizes plans each part as it sends it, in the
        // size chosen then, and storage may take a moment to answer a request the link has carried all of, during which
        // it carries nothing, so that larger parts deliver more.
        private sealed class LinkDriver
        {
            private static readonly long Tick = Stopwatch.Frequency / 1000;
            private readonly ConcurrencyController controller;
            private readonly Queue<long> parts;
            // The upload's plan when it chooses its part sizes, and the bytes of its file planned and not yet planned; null
            // and 0 otherwise.
            private readonly PartSizes sizes;
            private long planned;
            private long unplanned;
            private readonly List<Request> sending = new List<Request>();
            private readonly List<UploadTraceEvent> records = new List<UploadTraceEvent>();
            // When each part was answered, on the clock the trace's Elapsed uses.
            private readonly List<TimeSpan> answered = new List<TimeSpan>();
            private readonly List<long> answeredLengths = new List<long>();
            private readonly List<(TimeSpan Sent, TimeSpan Answered)> requests = new List<(TimeSpan Sent, TimeSpan Answered)>();
            private readonly List<(TimeSpan At, int Windows)> measured = new List<(TimeSpan At, int Windows)>();
            private double linkBytesPerSecond;
            private double requestBytesPerSecond;
            private long answerTicks;
            private readonly long connectTicks;
            private int idleConnections;
            // The requests that stay slow, by the order they were sent in (from 1), and how fast each carries at most.
            private readonly Dictionary<int, double> slowRequests = new Dictionary<int, double>();
            // How long each request takes to carry its part alone, by the order it was sent in, in turn (VaryRequests).
            private IReadOnlyList<TimeSpan> requestTimes;
            private int sent;
            private long now = Stopwatch.Frequency;

            public LinkDriver(ConcurrencySettings settings, IEnumerable<long> partLengths, double linkBytesPerSecond, double requestBytesPerSecond, TimeSpan connect)
                : this(settings, null, partLengths, linkBytesPerSecond, requestBytesPerSecond, connect, TimeSpan.Zero)
            {
            }

            // An upload of a file of sizedBytes whose parts sizes plans (PartSizes.Plan, started at Stopwatch.Frequency),
            // each answered answerWait after the link has carried all of it.
            public LinkDriver(ConcurrencySettings settings, PartSizes sizes, long sizedBytes, double linkBytesPerSecond, double requestBytesPerSecond, TimeSpan connect, TimeSpan answerWait)
                : this(settings, sizes, Array.Empty<long>(), linkBytesPerSecond, requestBytesPerSecond, connect, answerWait)
            {
                unplanned = sizedBytes;
            }

            private LinkDriver(ConcurrencySettings settings, PartSizes sizes, IEnumerable<long> partLengths, double linkBytesPerSecond, double requestBytesPerSecond, TimeSpan connect, TimeSpan answerWait)
            {
                controller = new ConcurrencyController(settings, "remote.bin", now, sizes?.Controller, warmStart: settings.WarmStarts);
                this.sizes = sizes;
                parts = new Queue<long>(partLengths);
                this.linkBytesPerSecond = linkBytesPerSecond;
                this.requestBytesPerSecond = requestBytesPerSecond;
                answerTicks = (long)(answerWait.TotalSeconds * Stopwatch.Frequency);
                connectTicks = (long)(connect.TotalSeconds * Stopwatch.Frequency);
            }

            public bool Done => parts.Count == 0 && unplanned == 0 && sending.Count == 0;

            public int Target => controller.Target;

            public int PeakInFlight { get; private set; }

            // The requests sent so far, answered or not.
            public int Sent => sent;

            public UploadConcurrency Snapshot => controller.Snapshot(now);

            public IReadOnlyList<UploadDecisionTrace> Decisions => records.OfType<UploadDecisionTrace>().ToList();

            // Every decision, of the target and of the part size, in the order they were made.
            public IReadOnlyList<UploadTraceEvent> Records => records.ToList();

            // The length of each part answered, in the order answered.
            public IReadOnlyList<long> AnsweredLengths => answeredLengths;

            // When each request answered was sent and answered, in the order answered, on the clock the trace's Elapsed
            // uses.
            public IReadOnlyList<(TimeSpan Sent, TimeSpan Answered)> Requests => requests;

            // How many windows the controller had measured by elapsed.
            public int MeasuredBy(TimeSpan elapsed)
            {
                return measured.LastOrDefault(at => at.At <= elapsed).Windows;
            }

            // How many parts had been answered by elapsed.
            public int AnsweredBy(TimeSpan elapsed)
            {
                return answered.Count(at => at <= elapsed);
            }

            // Runs until every part has been answered, for limit, or until stop says so.
            public void Run(TimeSpan limit, Func<bool> stop = null)
            {
                long end = now + (long)(limit.TotalSeconds * Stopwatch.Frequency);
                while (!Done && now < end && !(stop?.Invoke() ?? false))
                {
                    Admit();
                    now += Tick;
                    Transfer();
                }
            }

            // From now on, the link carries linkBytesPerSecond, of which a request carries at most requestBytesPerSecond, and
            // storage answers a request it has all of after answerWait, if given.
            public void ChangeLink(double linkBytesPerSecond, double requestBytesPerSecond, TimeSpan? answerWait = null)
            {
                this.linkBytesPerSecond = linkBytesPerSecond;
                this.requestBytesPerSecond = requestBytesPerSecond;
                answerTicks = answerWait.HasValue ? (long)(answerWait.Value.TotalSeconds * Stopwatch.Frequency) : answerTicks;
            }

            // The number-th request sent (from 1) carries at most bytesPerSecond: a slow connection, whose unused share of
            // the link the other requests carry.
            public void SlowRequest(int number, double bytesPerSecond)
            {
                slowRequests[number] = bytesPerSecond;
            }

            // Each request sent carries its part alone in the next of times, from the first again after the last, however
            // many are in flight while the link has room: answers that vary from one request to the next as a real link's
            // do, though none waits for another.
            public void VaryRequests(IReadOnlyList<TimeSpan> times)
            {
                requestTimes = times;
            }

            public void Admit()
            {
                while ((parts.Count > 0 || unplanned > 0) && controller.Permitted < controller.Target && controller.InFlight < controller.Target && !controller.IsPaused(now))
                {
                    controller.Granted(now);
                    long attempt = controller.Sending(now);
                    bool reused = idleConnections > 0;
                    idleConnections -= reused ? 1 : 0;
                    long length;
                    int sizeEpoch = 0;
                    if (sizes == null)
                    {
                        length = parts.Dequeue();
                    }
                    else
                    {
                        length = sizes.LengthFor(sent + 1, planned, out sizeEpoch);
                        planned += length;
                        unplanned -= length;
                    }
                    sending.Add(new Request { Length = length, SizeEpoch = sizeEpoch, StartedAt = now, Epoch = controller.Epoch, Reused = reused, Number = ++sent, Attempt = attempt });
                }
                PeakInFlight = Math.Max(PeakInFlight, sending.Count);
            }

            // The first request being sent fails as a connection failure would.
            public void Fail()
            {
                Request request = sending[0];
                sending.RemoveAt(0);
                controller.Record(now, request.Epoch, new AttemptSample(accepted: false, throttled: false, transient: true, retryAfter: null, bytes: 0, duration: Elapsed(request.StartedAt)), records);
                controller.Released(now, sending: true, request.Attempt);
            }

            public string Describe()
            {
                return $"peak {PeakInFlight}, target {controller.Target}, at {Elapsed(Stopwatch.Frequency).TotalSeconds:0.###} s; "
                    + string.Join("; ", records.Select(record => record switch
                    {
                        UploadDecisionTrace d => $"{d.Elapsed.TotalMilliseconds:0}ms {d.Decision} {d.PreviousTarget}->{d.Target} {d.ThroughputBytesPerSecond:0}/{d.BaselineBytesPerSecond:0}",
                        UploadSizingTrace s => $"{s.Elapsed.TotalMilliseconds:0}ms size {s.Decision} {s.PreviousSize}->{s.Size} {s.ThroughputBytesPerSecond:0}/{s.BaselineBytesPerSecond:0}",
                        _ => record.GetType().Name,
                    }));
            }

            private void Transfer()
            {
                List<Request> connected = sending.Where(request => request.Carried < request.Length && (request.Reused || now - request.StartedAt >= connectTicks)).ToList();
                // A request that cannot carry its equal share carries what it can; the others share the rest of the link.
                double equal = connected.Count == 0 ? 0 : linkBytesPerSecond / connected.Count;
                List<Request> limited = connected.Where(request => Limit(request) < equal).ToList();
                double left = linkBytesPerSecond - limited.Sum(Limit);
                int others = connected.Count - limited.Count;
                foreach (Request request in connected)
                {
                    double bytesPerSecond = limited.Contains(request) ? Limit(request) : Math.Min(Limit(request), left / others);
                    request.Carried = Math.Min(request.Length, request.Carried + (bytesPerSecond / 1000));
                    request.CarriedAt = request.Carried >= request.Length ? now : 0;
                }
                foreach (Request answered in sending.Where(request => request.Carried >= request.Length && now - request.CarriedAt >= answerTicks).ToList())
                {
                    sending.Remove(answered);
                    idleConnections++;
                    this.answered.Add(Elapsed(Stopwatch.Frequency));
                    answeredLengths.Add(answered.Length);
                    requests.Add((TimeSpan.FromSeconds((answered.StartedAt - Stopwatch.Frequency) / (double)Stopwatch.Frequency), Elapsed(Stopwatch.Frequency)));
                    controller.Record(now, answered.Epoch, new AttemptSample(accepted: true, throttled: false, transient: false, retryAfter: null, bytes: answered.Length, duration: Elapsed(answered.StartedAt), sizeEpoch: answered.SizeEpoch), records);
                    controller.Released(now, sending: true, answered.Attempt);
                    measured.Add((Elapsed(Stopwatch.Frequency), controller.Snapshot(now).MeasuredWindows));
                }
            }

            // How fast a request can carry, by itself.
            private double Limit(Request request)
            {
                double own = requestTimes == null ? requestBytesPerSecond : Math.Min(requestBytesPerSecond, request.Length / requestTimes[(request.Number - 1) % requestTimes.Count].TotalSeconds);
                return slowRequests.TryGetValue(request.Number, out double slow) ? Math.Min(slow, own) : own;
            }

            private TimeSpan Elapsed(long since)
            {
                return TimeSpan.FromSeconds((now - since) / (double)Stopwatch.Frequency);
            }

            private sealed class Request
            {
                public long Length;
                public int SizeEpoch;
                public long StartedAt;
                public long CarriedAt;
                public int Epoch;
                public double Carried;
                public bool Reused;
                public int Number;
                // The number the controller gave it as it was sent.
                public long Attempt;
            }
        }

        // Answers every part after delay, however many arrive together, with answer's response if it gives one: a
        // destination with room for more connections.
        private static void AnswerAfter(UploadServer server, TimeSpan delay, Func<PartRequest, ResponseMessage> answer = null)
        {
            server.RespondToPart = async request =>
            {
                await Task.Delay(delay);
                return answer?.Invoke(request) ?? UploadServer.Accept(request);
            };
        }

        // Answers parts with a fixed number of workers, each taking delay per part, kept on the Stopwatch clock
        // (DestinationClock) so that the workers' speed does not change with the host's timers: a destination whose
        // throughput more connections cannot raise.
        private static void AnswerThrough(UploadServer server, int workers, TimeSpan delay)
        {
            var available = new SemaphoreSlim(workers);
            server.RespondToPart = async request =>
            {
                await available.WaitAsync();
                request.Mark("worker");
                try
                {
                    await DestinationClock.Wait(delay);
                    request.Mark("served");
                    return UploadServer.Accept(request);
                }
                finally
                {
                    available.Release();
                }
            };
        }

        // The tuning RaisedTargetThatDoesNotRaiseThroughputIsWithdrawn and the real upload it stands in for share: windows
        // of 4 attempts, and a raise kept only for 30% more throughput.
        private static AutomaticConcurrencyOptions WithdrawalTuning()
        {
            return Tuning(start: 2, max: 8, t =>
            {
                t.WindowAttempts = 4;
                t.ProbeGainPercent = 30;
            });
        }

        // Automatic tuning that isolates the behavior a test is about: short windows, a step at a time after each
        // measured window, and no latency rules.
        private static AutomaticConcurrencyOptions Tuning(int start, int max, Action<AutomaticConcurrencyOptions> change = null)
        {
            AutomaticConcurrencyOptions tuning = AutomaticConcurrencyOptions.For(UploadPerformanceProfile.Generic);
            tuning.MinTarget = 1;
            tuning.StartTarget = start;
            tuning.SoftCeiling = max;
            tuning.MaxTarget = max;
            tuning.PlateauTarget = max;
            tuning.WindowAttempts = 3;
            tuning.GrowthIntervalWindows = 1;
            tuning.GrowthStep = 1;
            tuning.ProbeGainPercent = 10;
            tuning.ProbeWindows = 2;
            tuning.HoldWindows = 1;
            tuning.LatencyGrowthQueueLimit = 0;
            tuning.LatencyBackoffQueueLimit = 0;
            change?.Invoke(tuning);
            return tuning;
        }

        // Records an upload's trace, and when each record arrived on the Stopwatch clock of PartRequest.ArrivedAt.
        private sealed class TraceLog : IProgress<UploadTraceEvent>
        {
            private readonly List<KeyValuePair<UploadTraceEvent, TimeSpan>> records = new List<KeyValuePair<UploadTraceEvent, TimeSpan>>();

            public IReadOnlyList<UploadTraceEvent> Records
            {
                get
                {
                    lock (records)
                    {
                        return records.Select(record => record.Key).ToList();
                    }
                }
            }

            public IReadOnlyList<UploadDecisionTrace> Decisions => Records.OfType<UploadDecisionTrace>().ToList();

            public IReadOnlyList<UploadAttemptTrace> Attempts => Records.OfType<UploadAttemptTrace>().ToList();

            public IReadOnlyList<UploadSizingTrace> Sizing => Records.OfType<UploadSizingTrace>().ToList();

            public TimeSpan ReceivedAt(UploadTraceEvent record)
            {
                lock (records)
                {
                    return records.Single(r => ReferenceEquals(r.Key, record)).Value;
                }
            }

            // The decisions, for failure messages.
            public string Describe()
            {
                return string.Join(", ", Decisions.Select(d => $"{d.Elapsed.TotalMilliseconds:0}ms {d.Decision} {d.PreviousTarget}->{d.Target} {d.ThroughputBytesPerSecond:0}/{d.BaselineBytesPerSecond:0}"));
            }

            // What the upload measured and decided, beside what server saw, for the failure messages of tests whose outcome
            // rests on measurement. Times are on the Stopwatch clock, from the first part's arrival or the first record:
            // each concurrency and part size decision, with the throughput it compared; each attempt, with its part's size,
            // duration, throughput, wait for admission and the upload's target once it was taken into account; the length
            // of each part accepted, by number; each part request's arrival and answer at the site, with how many it was
            // then handling, as their times show, beside the peak its counter recorded; and the steps of answering each
            // request that the test's handler marked (PartRequest.Mark), after its arrival. With file, the site's parts are
            // only that file's.
            public string Evidence(UploadServer server, string file = null)
            {
                KeyValuePair<UploadTraceEvent, TimeSpan>[] timed;
                lock (records)
                {
                    timed = records.ToArray();
                }
                PartRequest[] parts = server.Parts.Where(part => file == null || part.File == file).ToArray();
                TimeSpan origin = parts.Select(part => part.ArrivedAt).Concat(timed.Select(record => record.Value)).DefaultIfEmpty(TimeSpan.Zero).Min();
                string At(TimeSpan moment) => $"+{(moment - origin).TotalMilliseconds:0.0}ms";
                int InFlightAt(TimeSpan moment) => parts.Count(part => part.ArrivedAt <= moment && (part.AnsweredAt == null || part.AnsweredAt > moment));

                var decisions = new List<string>();
                var sizing = new List<string>();
                var attempts = new List<string>();
                foreach (KeyValuePair<UploadTraceEvent, TimeSpan> record in timed)
                {
                    switch (record.Key)
                    {
                        case UploadDecisionTrace d:
                            decisions.Add($"{At(record.Value)} {d.Decision} {d.PreviousTarget}->{d.Target}, {d.ThroughputBytesPerSecond:0} vs {d.BaselineBytesPerSecond:0} B/s, queue {d.QueueEstimate:0.00}"
                                + (d.BelowTargetReason != BelowTargetReason.None ? $", below target: {d.BelowTargetReason}" : "")
                                + (d.Cooldown > TimeSpan.Zero ? $", cooldown {d.Cooldown.TotalMilliseconds:0}ms" : ""));
                            break;
                        case UploadSizingTrace s:
                            sizing.Add($"{At(record.Value)} {s.Decision} {s.PreviousSize}->{s.Size} ({s.Constraint}), {s.ThroughputBytesPerSecond:0} vs {s.BaselineBytesPerSecond:0} B/s");
                            break;
                        case UploadAttemptTrace a:
                            double throughput = a.Duration > TimeSpan.Zero ? a.PartBytes / a.Duration.TotalSeconds : 0;
                            attempts.Add($"{At(record.Value)} part {a.PartNumber}#{a.Attempt} {a.PartBytes} B in {a.Duration.TotalMilliseconds:0.0}ms = {throughput:0} B/s, admitted after {a.AdmissionWait.TotalMilliseconds:0.0}ms, target {a.Target}, "
                                + (a.Accepted ? "accepted" : $"refused ({a.StatusCode?.ToString() ?? "no status"})") + (a.Throttled ? ", throttled" : ""));
                            break;
                    }
                }
                var site = new List<KeyValuePair<TimeSpan, string>>();
                foreach (PartRequest part in parts)
                {
                    site.Add(new KeyValuePair<TimeSpan, string>(part.ArrivedAt, $"part {part.Number}#{part.Attempt} arrived"));
                    if (part.AnsweredAt is TimeSpan answered)
                    {
                        site.Add(new KeyValuePair<TimeSpan, string>(answered, $"part {part.Number}#{part.Attempt} answered"));
                    }
                }
                int timedPeak = parts.Select(part => InFlightAt(part.ArrivedAt)).DefaultIfEmpty(0).Max();
                var service = parts.Where(part => part.Marks.Count > 0).OrderBy(part => part.ArrivedAt)
                    .Select(part => $"part {part.Number}#{part.Attempt} " + string.Join(", ", part.Marks.Select(mark => $"{mark.Key} {(mark.Value - part.ArrivedAt).TotalMilliseconds:0.0}ms")));

                return string.Join("\n", new[]
                {
                    "Concurrency decisions: " + string.Join("; ", decisions),
                    "Part size decisions: " + string.Join("; ", sizing),
                    "Attempts: " + string.Join("; ", attempts),
                    "Accepted part lengths: " + string.Join(", ", parts.Where(part => part.Accepted).OrderBy(part => part.Number).Select(part => $"{part.Number}:{part.Bytes.Length}")),
                    $"Site: at most {server.MaxPartsInFlight} in flight by its counter, {timedPeak} by arrival and answer times: "
                        + string.Join("; ", site.OrderBy(entry => entry.Key).Select(entry => $"{At(entry.Key)} {entry.Value} ({InFlightAt(entry.Key)} in flight)")),
                    "Service after arrival: " + string.Join("; ", service),
                });
            }

            public void Report(UploadTraceEvent value)
            {
                lock (records)
                {
                    records.Add(new KeyValuePair<UploadTraceEvent, TimeSpan>(value, UploadServer.Clock));
                }
            }
        }

        // Captures the SDK's log messages while it is not disposed.
        private sealed class LogCapture : IDisposable
        {
            private readonly MemoryAppender appender = new MemoryAppender { Fix = FixFlags.Message | FixFlags.Exception };
            private readonly log4net.Repository.Hierarchy.Hierarchy repository = (log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository(typeof(FilesClient).Assembly);
            private readonly Level originalLevel;

            public LogCapture()
            {
                originalLevel = repository.Root.Level;
                repository.Root.Level = Level.Debug;
                log4net.Config.BasicConfigurator.Configure(repository, appender);
            }

            public string AboveDebug()
            {
                return string.Join("\n", appender.GetEvents().Where(e => e.Level > Level.Debug).Select(e => e.RenderedMessage + e.GetExceptionString()));
            }

            public void Dispose()
            {
                repository.Root.RemoveAppender(appender);
                repository.Root.Level = originalLevel;
            }
        }

        // A forward-only stream that takes readDelay for each read.
        private sealed class SlowStream : Stream
        {
            private readonly byte[] data;
            private readonly TimeSpan readDelay;
            private int position;

            public SlowStream(byte[] data, TimeSpan readDelay)
            {
                this.data = data;
                this.readDelay = readDelay;
            }

            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException("Only asynchronous reads are expected.");

            public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                await Task.Delay(readDelay, cancellationToken);
                int length = Math.Min(count, data.Length - position);
                Array.Copy(data, position, buffer, offset, length);
                position += length;
                return length;
            }

            public override void Flush()
            {
            }

            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        }
    }
}