using FilesCom;
using FilesCom.Transfers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FilesTests
{
    // Directory uploads whose automatic files share one target: what each file still does on its own (its part sizes,
    // its serial parts) and what the shared target reports (its decisions), observed at the loopback site and in the
    // trace and progress the caller asked for.
    public partial class ParallelUploadTest
    {
        // A file of a shared directory keeps its size feedback under the settled shared target, on the clock the
        // controllers are given (ControllerDriver), where each request takes exactly 25 ms whatever its size. The real
        // shared controller, at its ceiling of 2 from the start, and the files' real size controllers decide everything.
        // The file whose sizes are judged, the first attached, doubles its parts from 64 KiB to the 512 KiB ceiling, each
        // probe kept, each judged by what its own requests delivered against its own level before it, so that no part is
        // smaller than the one before. The other file's 256 KiB parts share every round and deliver more than the first
        // file's at first: none of them counts toward the first file's level or probes, and the other file's size is not
        // judged while the first file's is.
        [TestMethod]
        public void ASharedDirectoryFileGrowsItsPartsFromItsOwnParts()
        {
            TimeSpan duration = TimeSpan.FromMilliseconds(25);
            var driver = new ControllerDriver(Tuning(start: 2, max: 2));
            driver.AttachSizing(GrowingFrom(64 * KiB, max: 512 * KiB), 64 * KiB, 512 * KiB, "up/a.bin");
            driver.AttachSizing(GrowingFrom(64 * KiB, max: 512 * KiB), 256 * KiB, 512 * KiB, "up/b.bin");
            for (int round = 0; round < 200 && !driver.Sizing.Any(d => d.Decision == PartSizeDecision.ProbeRejected || (d.Decision == PartSizeDecision.ProbeAccepted && d.Size == 512 * KiB)); round++)
            {
                driver.SharedRound(duration);
            }

            string evidence = driver.Describe();
            List<UploadSizingTrace> sizing = driver.Sizing.ToList();
            CollectionAssert.AreEqual(
                new[] { "up/a.bin Grew 128", "up/a.bin ProbeAccepted 128", "up/a.bin Grew 256", "up/a.bin ProbeAccepted 256", "up/a.bin Grew 512", "up/a.bin ProbeAccepted 512" },
                sizing.Select(d => $"{d.Path} {d.Decision} {d.Size / KiB}").ToArray(),
                "The first file's parts doubled to the ceiling, each probe kept, and only its sizes were judged.\n" + evidence);
            foreach (UploadSizingTrace kept in sizing.Where(d => d.Decision == PartSizeDecision.ProbeAccepted))
            {
                Assert.AreEqual(kept.Size / duration.TotalSeconds, kept.ThroughputBytesPerSecond, 1, $"The {kept.Size} probe was judged by what the file's own requests delivered.\n" + evidence);
                Assert.AreEqual(kept.Size / 2 / duration.TotalSeconds, kept.BaselineBytesPerSecond, 1, $"Against the level its own requests delivered before the {kept.Size} probe.\n" + evidence);
            }
            long[] first = driver.SentSizes("up/a.bin").ToArray();
            CollectionAssert.AreEqual(first.OrderBy(size => size).ToArray(), first, "No part of the first file is smaller than the one before.\n" + evidence);
            Assert.AreEqual(64 * KiB, first.First(), evidence);
            Assert.AreEqual(512 * KiB, first.Last(), evidence);
            Assert.IsTrue(driver.SentSizes("up/b.bin").All(size => size == 256 * KiB) && driver.SizeOf("up/b.bin") == 256 * KiB,
                "The other file kept its size while the first file's was judged.\n" + evidence);
            Assert.AreEqual(2, driver.Target, "The shared target stayed settled at its ceiling.\n" + evidence);
        }

        // The scope a default directory upload's files share warm-starts (ConcurrencySettings.WarmStarts), on the clock the
        // controllers are given (ControllerDriver), and a file's sizes are still judged by its own parts alone: the shared
        // windows of the warm start's rests, which hold every file's parts, neither judge the file's size nor count toward
        // the windows it must measure before trying larger parts. Over a link of 4 MiB/s however many parts share it, the
        // warm start goes back to 1 part and rests; when the link becomes fast (200 MiB/s a request), it goes to the
        // policy's start, here MaxConcurrency 16, a settled target, and only from then on does the file measure its own
        // windows, of the policy's 32 parts: it tries larger parts once it has measured the two its sizing asks for there,
        // and not before, which it would after one had the rests' windows counted.
        [TestMethod]
        public void ASharedWarmStartLeavesEachFilesSizeToItsOwnParts()
        {
            var sizing = new AutomaticPartSizingOptions();
            var driver = new ControllerDriver(S3Settings(null, maxConcurrency: 16), warmStart: true);
            driver.AttachSizing(sizing, 16 * MiB, 256 * MiB, "up/a.bin");
            Assert.AreEqual(1, driver.Target, "The shared scope starts at 1 part at a time.");
            for (int round = 0; round < 400 && driver.Decisions.Count(d => d.Decision == ConcurrencyDecision.WarmStartRaised) < 2; round++)
            {
                driver.SizedRound(4 * MiB / (double)driver.Target);
            }
            for (int round = 0; round < 100 && !driver.Sizing.Any(d => d.Decision == PartSizeDecision.Grew); round++)
            {
                driver.SizedRound(200 * MiB);
            }

            string evidence = driver.Describe();
            UploadDecisionTrace settled = driver.Decisions.FirstOrDefault(d => d.Decision == ConcurrencyDecision.WarmStartSettled);
            UploadDecisionTrace promoted = driver.Decisions.FirstOrDefault(d => d.Decision == ConcurrencyDecision.WarmStartFastLink);
            UploadSizingTrace grew = driver.Sizing.FirstOrDefault(d => d.Decision == PartSizeDecision.Grew);
            Assert.IsTrue(settled != null && promoted != null && grew != null, "It went back and rested, then reached the policy's start and tried larger parts.\n" + evidence);
            Assert.AreEqual(16, promoted.Target, evidence);
            Assert.IsTrue(driver.Sizing.All(d => d.Elapsed > promoted.Elapsed), "No size decision while the warm start rested.\n" + evidence);
            Assert.IsTrue(driver.PartsAnswered(promoted.Elapsed, grew.Elapsed) >= sizing.GrowthIntervalWindows * AutomaticConcurrencyOptions.ScaledS3().WindowAttempts,
                "The file measured its own windows at the settled target, none from the warm start's rests, before larger parts.\n" + evidence);
        }

        // Files that choose their part sizes keep their size feedback, with the tuning the caller gave, under the shared
        // target, through the real directory upload and loopback requests: every byte where it belongs, never more requests
        // at once than the shared target, each file starting at its preferred size within the plan's limits, and each size
        // decision its own file's, following from the one before and matching the sizes its parts were sent in. What a probe
        // of larger parts delivers here depends on the host as well as on the stand-in's 25 ms, so a probe may be given up,
        // the size going back; that requests taking the same time make the parts grow, judged by the file's own parts alone,
        // is shown by ASharedDirectoryFileGrowsItsPartsFromItsOwnParts, on a clock the test controls.
        [TestMethod]
        [Timeout(60000)]
        public async Task SharedDirectoryFilesKeepTheirPartSizeFeedback()
        {
            using (var server = new UploadServer())
            {
                UseAws(server, 16 * KiB, 1 * MiB, 10000, 1L << 30);
                server.RespondToPart = async request =>
                {
                    await DestinationClock.Wait(TimeSpan.FromMilliseconds(25));
                    return UploadServer.Accept(request);
                };
                string local = Path.Combine(directory, "sized");
                Directory.CreateDirectory(local);
                byte[] a = Pattern(4 * MiB, 3);
                byte[] b = Pattern(4 * MiB, 5);
                File.WriteAllBytes(Path.Combine(local, "a.bin"), a);
                File.WriteAllBytes(Path.Combine(local, "b.bin"), b);
                var trace = new TraceLog();
                var options = new DirectoryUploadOptions
                {
                    MaxConcurrentFiles = 2,
                    FileOptions = new UploadOptions { AutomaticConcurrency = Tuning(start: 2, max: 2), AutomaticPartSizing = GrowingFrom(64 * KiB, max: 512 * KiB), Trace = trace },
                };

                DirectoryTransferResult result = await Within(Client(server).Transfers.UploadDirectoryAsync(local, "up", options));

                Assert.AreEqual(2, result.Files.Count);
                CollectionAssert.AreEqual(a, server.AcceptedBytes("up/a.bin"));
                CollectionAssert.AreEqual(b, server.AcceptedBytes("up/b.bin"));
                var lengths = new Dictionary<string, long[]>();
                foreach (string file in new[] { "up/a.bin", "up/b.bin" })
                {
                    long[] accepted = server.Parts.Where(part => part.File == file && part.Accepted).OrderBy(part => part.Number).Select(part => (long)part.Bytes.Length).ToArray();
                    lengths[file] = accepted.Take(accepted.Length - 1).ToArray();
                }
                string evidence = string.Join("; ", lengths.Select(entry => $"{entry.Key}: {string.Join(",", entry.Value.Distinct())}")) + "\n" + trace.Evidence(server);
                Assert.IsTrue(trace.Sizing.All(d => lengths.ContainsKey(d.Path)), "Each size decision is its file's.\n" + evidence);
                Assert.IsTrue(trace.Sizing.Any(d => d.Decision == PartSizeDecision.Grew), "A file tried larger parts under the shared target.\n" + evidence);
                foreach (var entry in lengths)
                {
                    long[] nonfinal = entry.Value;
                    Assert.AreEqual(64 * KiB, nonfinal[0], "Each file starts with its preferred size.\n" + evidence);
                    Assert.IsTrue(nonfinal.All(size => size <= 512 * KiB), evidence);
                    // The file's decisions follow one another: a probe grows the size in force, and is then kept at its
                    // size or given up for the size before it.
                    long current = 64 * KiB, probedFrom = 0;
                    foreach (UploadSizingTrace decision in trace.Sizing.Where(d => d.Path == entry.Key && d.Decision != PartSizeDecision.Chosen).OrderBy(d => d.Elapsed))
                    {
                        bool follows = decision.Decision == PartSizeDecision.Grew ? probedFrom == 0 && decision.PreviousSize == current && decision.Size > current
                            : decision.Decision == PartSizeDecision.ProbeAccepted ? probedFrom != 0 && decision.Size == current
                            : decision.Decision == PartSizeDecision.ProbeRejected && probedFrom != 0 && decision.PreviousSize == current && decision.Size == probedFrom;
                        Assert.IsTrue(follows, $"{entry.Key}'s {decision.Decision} {decision.PreviousSize}->{decision.Size} follows from its size {current}.\n" + evidence);
                        probedFrom = decision.Decision == PartSizeDecision.Grew ? current : 0;
                        current = decision.Size;
                    }
                    var chosen = new HashSet<long>(trace.Sizing.Where(d => d.Path == entry.Key).Select(d => d.Size)) { 64 * KiB };
                    Assert.IsTrue(nonfinal.All(chosen.Contains), $"{entry.Key}'s parts were sent in the sizes its own decisions chose.\n" + evidence);
                }
                Assert.IsTrue(trace.Sizing.Where(d => d.Decision == PartSizeDecision.ProbeAccepted).All(d => lengths[d.Path].Contains(d.Size)), "A file's accepted size is the size of its own parts.\n" + evidence);
                Assert.IsTrue(server.MaxPartsInFlight <= 2, $"Never more than the shared target: {server.MaxPartsInFlight}.\n" + evidence);
            }
        }

        // A shared target the directory's limiter cannot fill gives no size feedback: with one request slot for a target
        // of 2, every window waits for the limiter, so however much faster larger parts would be, the files keep the size
        // they started with, with every byte where it belongs and never more than the one request at once.
        [TestMethod]
        [Timeout(60000)]
        public async Task ASharedTargetTheLimiterCannotFillGivesNoSizeFeedback()
        {
            using (var server = new UploadServer())
            {
                UseAws(server, 16 * KiB, 1 * MiB, 10000, 1L << 30);
                server.RespondToPart = async request =>
                {
                    await DestinationClock.Wait(TimeSpan.FromMilliseconds(25));
                    return UploadServer.Accept(request);
                };
                string local = Path.Combine(directory, "limited");
                Directory.CreateDirectory(local);
                byte[] a = Pattern(2 * MiB, 3);
                byte[] b = Pattern(2 * MiB, 5);
                File.WriteAllBytes(Path.Combine(local, "a.bin"), a);
                File.WriteAllBytes(Path.Combine(local, "b.bin"), b);
                var trace = new TraceLog();
                var options = new DirectoryUploadOptions
                {
                    MaxConcurrentFiles = 2,
                    Limiter = new TransferLimiter(1),
                    FileOptions = new UploadOptions { AutomaticConcurrency = Tuning(start: 2, max: 2), AutomaticPartSizing = GrowingFrom(64 * KiB, max: 512 * KiB), Trace = trace },
                };

                await Within(Client(server).Transfers.UploadDirectoryAsync(local, "up", options));

                CollectionAssert.AreEqual(a, server.AcceptedBytes("up/a.bin"));
                CollectionAssert.AreEqual(b, server.AcceptedBytes("up/b.bin"));
                string evidence = trace.Evidence(server);
                foreach (string file in new[] { "up/a.bin", "up/b.bin" })
                {
                    long[] accepted = server.Parts.Where(part => part.File == file && part.Accepted).OrderBy(part => part.Number).Select(part => (long)part.Bytes.Length).ToArray();
                    Assert.IsTrue(accepted.Take(accepted.Length - 1).All(size => size == 64 * KiB), $"{file} kept its size: {string.Join(",", accepted.Distinct())}\n" + evidence);
                }
                Assert.IsTrue(trace.Sizing.All(d => d.Decision == PartSizeDecision.Chosen), "No size was tried: " + string.Join(",", trace.Sizing.Select(d => d.Decision)) + "\n" + evidence);
                Assert.IsTrue(trace.Decisions.Any(d => d.Decision == ConcurrencyDecision.WindowNotMeasured && d.BelowTargetReason == BelowTargetReason.SharedLimiter), "The limiter kept the target from being filled.\n" + evidence);
                Assert.IsTrue(server.MaxPartsInFlight <= 1, $"Never more than the limiter's one request: {server.MaxPartsInFlight}.\n" + evidence);
            }
        }

        // A rise of the shared target is not taken for a gain from larger parts, on the clock the controllers are given
        // (ControllerDriver), where each of the file's requests takes exactly its bytes at 2 MiB/s, whatever its size and
        // however many are in flight, so only more requests at once can deliver more. The real shared controller and the
        // file's real size controller decide everything: the target grows from 1 to 2 and keeps the gain, the two requests
        // delivering 4 MiB/s together; the file's sizes are judged only once that target is settled, from a level measured
        // at it as what the file's own requests deliver, 2 MiB/s, never the target's 4 MiB/s; its probe of larger parts
        // finds the same 2 MiB/s and is given up. A whole window's worth of the file's parts is answered while the target
        // is still changing, and none of it counts: had it, the probe could start before, or as soon as, the target
        // settled; it needs a whole window of parts at the settled target. Windows of 4 parts are whole rounds at either
        // target, so that how the paired answers fall in a window does not look like a gain.
        [TestMethod]
        public void ASharedTargetChangeIsNotTakenForASizeGain()
        {
            const double bytesPerSecond = 2 * MiB;
            AutomaticConcurrencyOptions tuning = Tuning(start: 1, max: 2, change: t => t.WindowAttempts = 4);
            var driver = new ControllerDriver(tuning);
            driver.AttachSizing(GrowingFrom(64 * KiB, max: 512 * KiB), 64 * KiB, 512 * KiB);
            for (int round = 0; round < 100 && !driver.Sizing.Any(d => d.Decision == PartSizeDecision.ProbeRejected || d.Decision == PartSizeDecision.ProbeAccepted); round++)
            {
                driver.SizedRound(bytesPerSecond);
            }

            string evidence = driver.Describe();
            UploadDecisionTrace grew = driver.Decisions.FirstOrDefault(d => d.Decision == ConcurrencyDecision.Grew && d.PreviousTarget == 1 && d.Target == 2);
            UploadDecisionTrace kept = driver.Decisions.FirstOrDefault(d => d.Decision == ConcurrencyDecision.ProbeAccepted && d.Target == 2);
            Assert.IsTrue(grew != null && kept != null, "More requests at once delivered more.\n" + evidence);
            Assert.AreEqual(2 * bytesPerSecond, kept.ThroughputBytesPerSecond, 1, "The target's two requests deliver 4 MiB/s together.\n" + evidence);
            Assert.IsTrue(driver.PartsAnswered(TimeSpan.Zero, kept.Elapsed) >= tuning.WindowAttempts, "A whole window of the file's parts was answered while the target changed.\n" + evidence);
            UploadSizingTrace probe = driver.Sizing.FirstOrDefault(d => d.Decision == PartSizeDecision.Grew);
            Assert.IsNotNull(probe, "The file still tried larger parts at the settled target.\n" + evidence);
            Assert.AreEqual(64 * KiB, probe.PreviousSize, evidence);
            Assert.AreEqual(128 * KiB, probe.Size, evidence);
            Assert.IsTrue(probe.Elapsed > kept.Elapsed, "The size probe started only once the target was settled.\n" + evidence);
            Assert.IsTrue(driver.PartsAnswered(kept.Elapsed, probe.Elapsed) >= tuning.WindowAttempts,
                "The file's level was measured anew at the settled target, from a whole window of its parts there, none from before.\n" + evidence);
            Assert.AreEqual(bytesPerSecond, probe.BaselineBytesPerSecond, 1, "The level is what each of the file's requests delivers, not what the target delivers.\n" + evidence);
            Assert.AreEqual(1, driver.Sizing.Count(d => d.Decision == PartSizeDecision.Grew), evidence);
            UploadSizingTrace rejected = driver.Sizing.FirstOrDefault(d => d.Decision == PartSizeDecision.ProbeRejected);
            Assert.IsNotNull(rejected, "Larger parts delivered nothing more, so the probe was given up.\n" + evidence);
            Assert.AreEqual(bytesPerSecond, rejected.ThroughputBytesPerSecond, 1, "The probe was judged by what each of the file's requests delivers.\n" + evidence);
            Assert.AreEqual(64 * KiB, rejected.Size, "The size went back.\n" + evidence);
            Assert.IsFalse(driver.Sizing.Any(d => d.Decision == PartSizeDecision.ProbeAccepted), evidence);
            Assert.AreEqual(2, driver.Target, "The shared target kept its gain.\n" + evidence);
        }

        // The same shared target and file through the real directory upload and loopback requests: every byte where it
        // belongs, never more requests at once than the target, the target's rise traced as the directory's, and the
        // file's size decisions its own, made only once the target was settled, with its parts sent in the sizes those
        // decisions chose. What the requests deliver here depends on the host as well as on the stand-in's waits, so how
        // the sizes are judged is shown by ASharedTargetChangeIsNotTakenForASizeGain, on a clock the test controls.
        [TestMethod]
        [Timeout(60000)]
        public async Task ASharedTargetChangeSizesTheFilesPartsOnlyOnceItIsSettled()
        {
            using (var server = new UploadServer())
            {
                UseAws(server, 16 * KiB, 1 * MiB, 10000, 1L << 30);
                const double bytesPerSecond = 2 * MiB;
                server.RespondToPart = async request =>
                {
                    await DestinationClock.Wait(TimeSpan.FromSeconds(request.Bytes.Length / bytesPerSecond));
                    return UploadServer.Accept(request);
                };
                string local = Path.Combine(directory, "rising");
                Directory.CreateDirectory(local);
                byte[] a = Pattern(4 * MiB, 7);
                File.WriteAllBytes(Path.Combine(local, "a.bin"), a);
                var trace = new TraceLog();
                var options = new DirectoryUploadOptions
                {
                    FileOptions = new UploadOptions { AutomaticConcurrency = Tuning(start: 1, max: 2, change: t => t.WindowAttempts = 4), AutomaticPartSizing = GrowingFrom(64 * KiB, max: 512 * KiB), Trace = trace },
                };

                await Within(Client(server).Transfers.UploadDirectoryAsync(local, "up", options));

                CollectionAssert.AreEqual(a, server.AcceptedBytes("up/a.bin"));
                string evidence = trace.Evidence(server);
                UploadDecisionTrace grew = trace.Decisions.FirstOrDefault(d => d.Decision == ConcurrencyDecision.Grew && d.PreviousTarget == 1 && d.Target == 2);
                UploadDecisionTrace kept = trace.Decisions.FirstOrDefault(d => d.Decision == ConcurrencyDecision.ProbeAccepted && d.Target == 2);
                Assert.IsTrue(grew != null && kept != null, "More requests at once delivered more.\n" + evidence);
                List<UploadSizingTrace> probes = trace.Sizing.Where(d => d.Decision == PartSizeDecision.Grew).ToList();
                Assert.IsTrue(probes.Count >= 1, "The file tried larger parts at the settled target.\n" + evidence);
                Assert.IsTrue(probes.All(p => p.Elapsed > kept.Elapsed), "Each size probe started once the target was settled.\n" + evidence);
                Assert.IsTrue(trace.Sizing.All(d => d.Path == "up/a.bin"), "Each size decision is the file's.\n" + evidence);
                long[] accepted = server.Parts.Where(part => part.File == "up/a.bin" && part.Accepted).OrderBy(part => part.Number).Select(part => (long)part.Bytes.Length).ToArray();
                var chosen = new HashSet<long>(trace.Sizing.Select(d => d.Size)) { 64 * KiB };
                Assert.IsTrue(accepted.Take(accepted.Length - 1).All(chosen.Contains), $"The file's parts were sent in the sizes its decisions chose: {string.Join(",", accepted.Distinct())}\n" + evidence);
                Assert.IsTrue(server.MaxPartsInFlight <= 2, $"Never more than the target: {server.MaxPartsInFlight}.\n" + evidence);
            }
        }

        // The shared target's decisions reach the files' trace once each, under the directory's path: as many growth and
        // throttling decisions as the shared target reports making, never one per file.
        [TestMethod]
        [Timeout(60000)]
        public async Task SharedDirectoryDecisionsAreTracedOnceEach()
        {
            using (var server = new UploadServer())
            {
                string local = Path.Combine(directory, "traced");
                Directory.CreateDirectory(local);
                for (int i = 0; i < 160; i++)
                {
                    File.WriteAllBytes(Path.Combine(local, $"f{i:D3}.bin"), Pattern(8, i));
                }
                int throttled = 0;
                server.RespondToPart = async request =>
                {
                    await Task.Delay(40);
                    if (request.File == "up/f080.bin" && System.Threading.Interlocked.Exchange(ref throttled, 1) == 0)
                    {
                        return LoopbackSite.Status(503);
                    }
                    return UploadServer.Accept(request);
                };
                var trace = new TraceLog();
                UploadConcurrency latest = null;
                var options = new DirectoryUploadOptions
                {
                    FileOptions = new UploadOptions
                    {
                        AutomaticConcurrency = Tuning(start: 2, max: 8, change: t => t.BackpressurePause = TimeSpan.Zero),
                        Trace = trace,
                        Progress = new Synchronous<TransferProgress>(report =>
                        {
                            lock (trace)
                            {
                                if (report.Concurrency != null && (latest == null || report.Concurrency.Grows + report.Concurrency.Shrinks >= latest.Grows + latest.Shrinks))
                                {
                                    latest = report.Concurrency;
                                }
                            }
                        }),
                    },
                };

                DirectoryTransferResult result = await Within(Client(server, retries: 2).Transfers.UploadDirectoryAsync(local, "up", options));

                Assert.AreEqual(160, result.Files.Count);
                CollectionAssert.AreEqual(Pattern(8, 80), server.AcceptedBytes("up/f080.bin"));
                IReadOnlyList<UploadDecisionTrace> decisions = trace.Decisions;
                string seen = trace.Describe();
                Assert.IsTrue(latest != null && latest.Shared, "The files report the shared target.");
                Assert.IsTrue(decisions.All(d => d.Path == "up"), "Decisions belong to the directory's shared target, not to a file: " + string.Join(",", decisions.Select(d => d.Path).Distinct()));
                Assert.IsTrue(decisions.Count(d => d.Decision == ConcurrencyDecision.Grew) >= 1, seen);
                Assert.IsTrue(decisions.Count(d => d.Decision == ConcurrencyDecision.ShrankAfterThrottling) == 1, seen);
                Assert.AreEqual(latest.Grows, decisions.Count(d => d.Decision == ConcurrencyDecision.Grew), "One trace record for each growth the shared target made: " + seen);
                Assert.AreEqual(latest.Throttles, decisions.Count(d => d.Decision == ConcurrencyDecision.CooldownStarted), "One for each throttling: " + seen);
                Assert.IsTrue(trace.Attempts.Count >= 160 && trace.Attempts.All(a => a.Path.StartsWith("up/f", StringComparison.Ordinal)), "Each file's attempts are still its own.");
            }
        }

        // A trace callback that fails fails the transfer, as it does for one upload, when the shared target's decision
        // reaches it.
        [TestMethod]
        [Timeout(60000)]
        public async Task ASharedDecisionTraceCallbackFailureFailsTheTransfer()
        {
            using (var server = new UploadServer())
            {
                string local = Path.Combine(directory, "failing");
                Directory.CreateDirectory(local);
                for (int i = 0; i < 80; i++)
                {
                    File.WriteAllBytes(Path.Combine(local, $"f{i:D3}.bin"), Pattern(8, i));
                }
                server.RespondToPart = async request =>
                {
                    await Task.Delay(30);
                    return UploadServer.Accept(request);
                };
                int decisions = 0;
                var options = new DirectoryUploadOptions
                {
                    FileOptions = new UploadOptions
                    {
                        AutomaticConcurrency = Tuning(start: 2, max: 8),
                        Trace = new Synchronous<UploadTraceEvent>(record =>
                        {
                            if (record is UploadDecisionTrace && System.Threading.Interlocked.Increment(ref decisions) == 1)
                            {
                                throw new TraceSinkFailure();
                            }
                        }),
                    },
                };

                await Assert.ThrowsExactlyAsync<TraceSinkFailure>(() => Within(Client(server).Transfers.UploadDirectoryAsync(local, "up", options)));
                Assert.AreEqual(1, decisions);
            }
        }

        // A file whose grant makes it send its parts one at a time does so and reports it, with limit 1 and its reason,
        // while the other files keep the shared target's parallel parts.
        [TestMethod]
        [Timeout(60000)]
        public async Task ASerialFileOfASharedDirectoryReportsItsOwnLimit()
        {
            using (var server = new UploadServer())
            {
                string local = Path.Combine(directory, "mixed");
                Directory.CreateDirectory(local);
                var contents = new Dictionary<string, byte[]> { ["a.bin"] = Pattern(64, 1), ["s.bin"] = Pattern(64, 2), ["z.bin"] = Pattern(64, 3) };
                foreach (var entry in contents)
                {
                    File.WriteAllBytes(Path.Combine(local, entry.Key), entry.Value);
                }
                server.ParallelPartsForFile = (file, number, negotiation) => file == "up/s.bin" ? "false" : "true";
                server.RespondToPart = async request =>
                {
                    await Task.Delay(40);
                    return UploadServer.Accept(request);
                };
                var reports = new List<TransferProgress>();
                var options = new DirectoryUploadOptions
                {
                    MaxConcurrentFiles = 3,
                    FileOptions = new UploadOptions
                    {
                        AutomaticConcurrency = Tuning(start: 4, max: 4),
                        Progress = new Synchronous<TransferProgress>(report =>
                        {
                            lock (reports)
                            {
                                reports.Add(report);
                            }
                        }),
                    },
                };

                DirectoryTransferResult result = await Within(Client(server).Transfers.UploadDirectoryAsync(local, "up", options));

                Assert.AreEqual(3, result.Files.Count);
                foreach (var entry in contents)
                {
                    CollectionAssert.AreEqual(entry.Value, server.AcceptedBytes("up/" + entry.Key));
                }
                Assert.AreEqual(1, MostAtOnce(server, "up/s.bin"), "The serial file sent one part at a time.");
                Assert.IsTrue(MostAtOnce(server, "up/a.bin") >= 2 || MostAtOnce(server, "up/z.bin") >= 2, "The other files kept parallel parts.");
                List<UploadConcurrency> serial = reports.Where(r => r.Path == "up/s.bin" && r.Concurrency != null).Select(r => r.Concurrency).ToList();
                List<UploadConcurrency> others = reports.Where(r => r.Path != "up/s.bin" && r.Concurrency != null).Select(r => r.Concurrency).ToList();
                Assert.IsTrue(serial.Count > 0 && serial.All(c => c.Shared && c.Limit == 1 && c.SerialReason == UploadSerialReason.ServerRequiresSerialParts),
                    "The serial file reports its own limit and reason: " + string.Join(",", serial.Select(c => $"{c.Limit}/{c.SerialReason}").Distinct()));
                Assert.IsTrue(others.Count > 0 && others.All(c => c.Shared && c.Limit == 4 && c.SerialReason == UploadSerialReason.None),
                    "The other files report the shared limit: " + string.Join(",", others.Select(c => $"{c.Limit}/{c.SerialReason}").Distinct()));
            }
        }

        // A directory upload of one-part files whose shared target, 4, and limiter, 4 requests, allow 4 parts at once,
        // where Files.com takes 100 ms to begin each file and 100 ms to finish it, and storage first 50 ms to answer its
        // part: feeding 4 parts at once takes some 20 files in progress, where the target's own limit allows 6, and files
        // end far more often than every 100 ms. Past that limit, the upload starts a file in place of each that ends, and
        // one more each 100 ms, however often files end, until enough parts are ready: once 60 parts have been answered,
        // storage has 4 parts to answer most of the time. Once storage takes 400 ms a part, some 8 files feed it, and
        // the files the upload allows fall back a file each 100 ms, never at once, while storage still has 4 parts to
        // answer. Never more files than MaxConcurrentFiles (32), more files than one a 100 ms past the target's limit,
        // or more parts than the limiter's 4. Cancelled, the upload stops, its limiter left with nothing in use.
        [TestMethod]
        [Timeout(60000)]
        public async Task OnePartFilesSlowerToBeginAndFinishThanToSendFeedTheTargetWithAFileAtATimeMoreOrFewer()
        {
            using (var server = new UploadServer())
            {
                string local = Path.Combine(directory, "slow-files");
                Directory.CreateDirectory(local);
                for (int i = 0; i < 400; i++)
                {
                    File.WriteAllBytes(Path.Combine(local, $"f{i:D3}.bin"), Pattern(8, i));
                }
                int answerMilliseconds = 50;
                server.HoldNegotiation = file => Task.Delay(100);
                server.HoldFinish = file => Task.Delay(100);
                server.RespondToPart = async request =>
                {
                    await Task.Delay(Volatile.Read(ref answerMilliseconds));
                    return UploadServer.Accept(request);
                };
                var reports = new List<(TimeSpan At, int Files, int Limit)>();
                var limiter = new TransferLimiter(maxConcurrentRequests: 4, maxBufferedBytes: 1024 * 1024);
                var options = new DirectoryUploadOptions
                {
                    MaxConcurrentFiles = 32,
                    Limiter = limiter,
                    Progress = new Synchronous<DirectoryTransferProgress>(report =>
                    {
                        lock (reports)
                        {
                            reports.Add((UploadServer.Clock, report.FilesActive, report.FileLimit));
                        }
                    }),
                    FileOptions = new UploadOptions { AutomaticConcurrency = Tuning(start: 4, max: 4) },
                };
                List<(TimeSpan At, int Files, int Limit)> Reports()
                {
                    lock (reports)
                    {
                        return reports.ToList();
                    }
                }
                // How many parts storage had to answer, on average, from one time to another.
                double Fed(TimeSpan from, TimeSpan to)
                {
                    return server.Parts.Sum(part => Math.Max(0, (Min(part.AnsweredAt ?? to, to) - Max(part.ArrivedAt, from)).TotalSeconds)) / (to - from).TotalSeconds;
                }
                var cancellation = new CancellationTokenSource();
                TimeSpan started = UploadServer.Clock;

                Task<DirectoryTransferResult> upload = Client(server).Transfers.UploadDirectoryAsync(local, "up", options, cancellation.Token);
                await server.WaitUntil(s => s.Parts.Count(part => part.AnsweredAt != null) >= 160 || upload.IsCompleted, "160 parts are answered");
                List<PartRequest> fast = server.Parts.OrderBy(part => part.ArrivedAt).ToList();
                double fed = Fed(fast[60].ArrivedAt, fast[159].ArrivedAt);
                int peak = Reports().Max(r => r.Limit);
                TimeSpan slowed = UploadServer.Clock;
                Volatile.Write(ref answerMilliseconds, 400);
                await server.WaitUntil(s => Reports().Any(r => r.At > slowed && r.Limit <= 10) || upload.IsCompleted, "the files allowed fall back", () => $"limit {Reports().Last().Limit}");
                TimeSpan fewer = Reports().First(r => r.At > slowed && r.Limit <= 10).At;
                await Task.Delay(1200);
                double fedWithFewer = Fed(fewer + TimeSpan.FromMilliseconds(400), UploadServer.Clock);
                cancellation.Cancel();
                await Assert.ThrowsAsync<OperationCanceledException>(() => Within(upload));

                List<(TimeSpan At, int Files, int Limit)> all = Reports();
                Assert.IsTrue(fed >= 3, $"Storage had {fed:0.00} of the target's 4 parts to answer, on average, once 60 had been answered.");
                Assert.IsTrue(peak > 6, $"More files than the target's own limit allowed: {peak}.");
                Assert.IsTrue(all.All(r => r.Files <= 32 && r.Limit <= 32), "Never more than MaxConcurrentFiles.");
                string tooSoon = string.Join(", ", all.Where(r => r.Files > 8 + ((r.At - started).TotalMilliseconds / 100)).Select(r => $"{r.Files} after {(r.At - started).TotalMilliseconds:0} ms"));
                Assert.AreEqual("", tooSoon, "No more than one file each 100 ms past the target's limit.");
                List<(TimeSpan At, int Files, int Limit)> falling = all.Where(r => r.At > slowed).ToList();
                string atOnce = string.Join(", ", falling.Zip(falling.Skip(1), (a, b) => (a, b)).Where(p => p.a.Limit - p.b.Limit > 2 + ((p.b.At - p.a.At).TotalMilliseconds / 100)).Select(p => $"{p.a.Limit} to {p.b.Limit} in {(p.b.At - p.a.At).TotalMilliseconds:0} ms"));
                Assert.AreEqual("", atOnce, $"The files allowed fell from {peak} a file each 100 ms, not at once.");
                Assert.IsTrue(fedWithFewer >= 3, $"Storage still had {fedWithFewer:0.00} of the target's 4 parts to answer with fewer files.");
                Assert.IsTrue(server.MaxPartsInFlight <= 4, $"The limiter holds the parts to its 4 slots: {server.MaxPartsInFlight} at once.");
                Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes + limiter.QueuedRequests + limiter.QueuedBufferReservations);
            }
        }

        // A directory upload straight to S3, whose one-part files share one warm start, which rises no further than the
        // files' MaxConcurrency of 2: its one raise, to 2, is its last, which ends it, and is never withdrawn. Files.com
        // holds the files' beginnings until twenty have begun, the files the upload grows to, a file each 100 ms, while
        // none of their parts can be ready, past the six the target alone allows (TargetFileLimit), and holds every file's
        // finish throughout, so that no file ends. Storage takes 10 ms to answer each part: from when the upload has had
        // time to look at the warm start's first target, so that the raise comes after it has, or at once, so that the
        // raise usually comes before the upload first sees a target. The raise needs more parts than the files in progress
        // have left, and the files that bring them begin with it, before any file finishes: at least eight within 300 ms,
        // where the ordinary pacing, a file each 100 ms while too few parts are ready, begins four at most. Never more parts
        // at once than MaxConcurrency, nor more files than MaxConcurrentFiles (the most the transfer allows); cancelled,
        // the upload stops, its limiter left with nothing in use.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(true, DisplayName = "Answered once the warm start's first target could be seen")]
        [DataRow(false, DisplayName = "Answered at once")]
        public async Task TheLastRaiseOfASharedWarmStartBeginsFilesBeforeAnyFinishes(bool answerLater)
        {
            const int most = 2;
            using (var server = new UploadServer { Aws = true })
            {
                // Parts no smaller than the files: each is sent as one part.
                UploadGrant.AwsLimitsForTests.Value = new PartSizeLimits(8, 8, 100, 4096);
                server.UploadUri = (file, number, negotiation) => UploadServer.AwsPartUrl(file.Replace('/', '~'), number, negotiation);
                string local = Path.Combine(directory, "warm-files");
                Directory.CreateDirectory(local);
                for (int i = 0; i < 200; i++)
                {
                    File.WriteAllBytes(Path.Combine(local, $"f{i:D3}.bin"), Pattern(8, i));
                }
                const int grown = 20;
                var begun = new List<TimeSpan>();
                var beginning = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                var answering = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                var finishing = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                if (!answerLater)
                {
                    answering.SetResult(true);
                }
                server.HoldNegotiation = file =>
                {
                    lock (begun)
                    {
                        begun.Add(UploadServer.Clock);
                        if (!answerLater && begun.Count == grown)
                        {
                            beginning.TrySetResult(true);
                        }
                    }
                    return beginning.Task;
                };
                server.HoldFinish = file => finishing.Task;
                server.RespondToPart = async request =>
                {
                    await answering.Task;
                    await Task.Delay(10);
                    return UploadServer.Accept(request);
                };
                var reports = new List<(int Files, int Limit)>();
                var decisions = new List<(TimeSpan At, UploadDecisionTrace Decision)>();
                var limiter = new TransferLimiter(maxConcurrentRequests: 128, maxBufferedBytes: 1024 * 1024);
                var options = new DirectoryUploadOptions
                {
                    MaxConcurrentFiles = DirectoryTransferOptions.MaxAllowedConcurrentFiles,
                    Limiter = limiter,
                    Progress = new Synchronous<DirectoryTransferProgress>(report =>
                    {
                        lock (reports)
                        {
                            reports.Add((report.FilesActive, report.FileLimit));
                        }
                    }),
                    FileOptions = new UploadOptions
                    {
                        MaxConcurrency = most,
                        Trace = new Synchronous<UploadTraceEvent>(record =>
                        {
                            if (record is UploadDecisionTrace decision)
                            {
                                lock (reports)
                                {
                                    decisions.Add((UploadServer.Clock, decision));
                                }
                            }
                        }),
                    },
                };
                TimeSpan window = TimeSpan.FromMilliseconds(300);
                int Begun(TimeSpan after = default)
                {
                    lock (begun)
                    {
                        return after == default ? begun.Count : begun.Count(at => at > after && at <= after + window);
                    }
                }
                List<(TimeSpan At, UploadDecisionTrace Decision)> Raises()
                {
                    lock (reports)
                    {
                        return decisions.Where(d => d.Decision.Decision == ConcurrencyDecision.WarmStartRaised).ToList();
                    }
                }
                string Seen()
                {
                    List<TimeSpan> starts;
                    lock (begun)
                    {
                        starts = begun.ToList();
                    }
                    TimeSpan origin = starts.Count > 0 ? starts[0] : UploadServer.Clock;
                    lock (reports)
                    {
                        return string.Join("; ", decisions.Select(d => $"{d.Decision.Decision} {d.Decision.PreviousTarget}->{d.Decision.Target} at {(d.At - origin).TotalMilliseconds:0} ms"
                            + (d.Decision.Decision == ConcurrencyDecision.WarmStartRaised ? $", {starts.Count(at => at <= d.At)} files begun, {Begun(d.At)} more within 300 ms" : "")));
                    }
                }
                var cancellation = new CancellationTokenSource();

                Task<DirectoryTransferResult> upload = server.Client().Transfers.UploadDirectoryAsync(local, "up", options, cancellation.Token);
                await server.WaitUntil(s => Begun() >= grown || upload.IsCompleted, $"{grown} files began");
                if (answerLater)
                {
                    beginning.TrySetResult(true);
                    await server.WaitUntil(s => s.Parts.Count > 0 || upload.IsCompleted, "a part was sent");
                    // The upload looks at the target at least each 100 ms while it waits to begin more files.
                    await Task.Delay(TimeSpan.FromMilliseconds(250));
                    answering.TrySetResult(true);
                }
                await server.WaitUntil(s => Raises().Any(r => UploadServer.Clock > r.At + window) || upload.IsCompleted, "300 ms after the warm start's raise", Seen);
                cancellation.Cancel();
                finishing.TrySetResult(true);
                await Assert.ThrowsAsync<OperationCanceledException>(() => Within(upload));

                Assert.IsTrue(Raises().Count == 1 && Raises()[0].Decision.Target == most && Begun(Raises()[0].At) >= 8, "At least eight files began within 300 ms of the warm start's raise to 2: " + Seen());
                Assert.IsTrue(server.MaxPartsInFlight <= most, $"Never more parts at once than MaxConcurrency: {server.MaxPartsInFlight}.");
                lock (reports)
                {
                    Assert.IsTrue(reports.All(r => r.Files <= DirectoryTransferOptions.MaxAllowedConcurrentFiles && r.Limit <= DirectoryTransferOptions.MaxAllowedConcurrentFiles), "Never more than MaxConcurrentFiles.");
                }
                Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes + limiter.QueuedRequests + limiter.QueuedBufferReservations);
            }
        }

        private static TimeSpan Min(TimeSpan a, TimeSpan b) => a < b ? a : b;

        private static TimeSpan Max(TimeSpan a, TimeSpan b) => a > b ? a : b;

        // The most part requests of file the site was handling at once.
        private static int MostAtOnce(UploadServer server, string file)
        {
            PartRequest[] parts = server.Parts.Where(part => part.File == file).ToArray();
            return parts.Select(part => parts.Count(other => other.ArrivedAt <= part.ArrivedAt && (other.AnsweredAt == null || other.AnsweredAt > part.ArrivedAt))).DefaultIfEmpty(0).Max();
        }

        private sealed class TraceSinkFailure : Exception
        {
        }

        // Reports on the calling thread, as they are made.
        private sealed class Synchronous<T> : IProgress<T>
        {
            private readonly Action<T> report;

            internal Synchronous(Action<T> report)
            {
                this.report = report;
            }

            public void Report(T value) => report(value);
        }
    }
}