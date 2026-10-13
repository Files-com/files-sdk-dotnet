using FilesCom;
using FilesCom.Transfers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WireMock;

namespace FilesTests
{
    // Where an upload chooses its part sizes: only for an ordinary multipart upload straight to Amazon S3, as its part
    // URLs show, within the SDK's planning limits; and streams of unknown length. Elsewhere it sends the parts Files.com
    // sizes. The tests observe what arrives at a loopback site that negotiates parts like Files.com, at its own address or
    // at an S3 endpoint the test client reaches it through: part sizes, URLs, bytes, and what completes the upload. Most
    // plan within small limits (UseAws), so that parts are small; a few use the SDK's own limits for S3.
    public partial class ParallelUploadTest
    {
        private const int KiB = 1024;
        private const int MiB = 1024 * KiB;

        // S3's smallest part but the last, and the largest file the SDK plans an S3 upload for.
        private const long AwsMinPart = 5242880;
        private const long FilesMaxFile = 5497558138880;

        // The real upload of a file, two parts at a time, to loopback S3 that answers each part 25 ms after it arrives,
        // choosing the sizes of parts not yet read: every byte is where it belongs, sent as negotiated; the upload starts
        // with its preferred size; each size decision follows from the one before (a probe grows the size in force, and
        // is then kept, or given up for the size before it); the first probe's larger parts are sent and judged; and the
        // parts' sizes follow the sizes in force, in order, so that a part already read keeps its size, none larger than
        // MaxPartSize. How long a request takes here depends on the host as well as on the stand-in's 25 ms, so a window
        // the host holds up can make a probe of larger parts deliver less, and the probe is then given up; that requests
        // taking the same time make the parts grow, each probe kept, up to MaxPartSize, is shown by
        // ChosenPartSizesGrowToMaxPartSizeWhileLargerPartsRaiseThroughput, on a clock the test controls.
        [TestMethod]
        [Timeout(60000)]
        public async Task ChosenPartSizesFollowTheirDecisionsForPartsNotYetRead()
        {
            using (var server = new UploadServer())
            {
                UseAws(server, 16 * KiB, 1 * MiB, 10000, 1L << 30);
                // Every request takes the same time whatever its size, as when latency limits more than bandwidth: larger
                // parts deliver more. The time is kept on the Stopwatch clock, whatever the host's timers do.
                server.RespondToPart = async request =>
                {
                    await DestinationClock.Wait(TimeSpan.FromMilliseconds(25));
                    return UploadServer.Accept(request);
                };
                byte[] contents = Pattern(6 * MiB);
                string localPath = WriteFile("local.bin", contents);
                var trace = new TraceLog();

                UploadResult result = await Within(Client(server).Transfers.UploadFileAsync(localPath, "remote.bin", new UploadOptions
                {
                    ConcurrencyMode = UploadConcurrencyMode.Static,
                    MaxConcurrency = 2,
                    AutomaticPartSizing = GrowingFrom(64 * KiB, max: 512 * KiB),
                    Trace = trace,
                }));

                long[] nonfinal = result.Parts.Take(result.Parts.Count - 1).Select(part => part.Length).ToArray();
                string evidence = $"Chosen lengths: {string.Join(", ", result.Parts.Select(part => part.Length))}\n{trace.Evidence(server)}";
                List<UploadSizingTrace> decisions = trace.Sizing.Where(d => d.Decision != PartSizeDecision.Chosen).OrderBy(d => d.Elapsed).ToList();
                Assert.AreEqual(64 * KiB, nonfinal[0], "The upload starts with its preferred size.\n" + evidence);
                Assert.IsTrue(nonfinal.All(size => size <= 512 * KiB), "No part is larger than MaxPartSize.\n" + evidence);
                var inForce = new List<long> { 64 * KiB };
                long probedFrom = 0;
                foreach (UploadSizingTrace decision in decisions)
                {
                    long current = inForce.Last();
                    bool follows = decision.Decision == PartSizeDecision.Grew ? probedFrom == 0 && decision.PreviousSize == current && decision.Size > current
                        : decision.Decision == PartSizeDecision.ProbeAccepted ? probedFrom != 0 && decision.Size == current
                        : decision.Decision == PartSizeDecision.ProbeRejected && probedFrom != 0 && decision.PreviousSize == current && decision.Size == probedFrom;
                    Assert.IsTrue(follows, $"{decision.Decision} {decision.PreviousSize}->{decision.Size} follows from the size {current}.\n" + evidence);
                    probedFrom = decision.Decision == PartSizeDecision.Grew ? current : 0;
                    if (decision.Size != current)
                    {
                        inForce.Add(decision.Size);
                    }
                }
                Assert.IsTrue(decisions.Count >= 2 && nonfinal.Contains(decisions[0].Size), "The first probe's larger parts were sent and judged.\n" + evidence);
                int at = 0;
                foreach (long size in nonfinal)
                {
                    while (at < inForce.Count && inForce[at] != size)
                    {
                        at++;
                    }
                    Assert.IsTrue(at < inForce.Count, $"Each part has the size in force when it was read, in order ({string.Join(", ", inForce)}).\n" + evidence);
                }
                AssertExactUpload(server, result, contents);
                AssertSentAsNegotiated(server);
                UploadPartSizing sizing = result.FinalProgress.PartSizing;
                Assert.AreEqual(PartSizeSource.Automatic, sizing.Source);
                Assert.AreEqual(ServerPlannedReason.None, sizing.ServerPlannedReason);
                Assert.AreEqual(1 * MiB, sizing.Limits.MaxPartBytes);
                Assert.IsFalse(sizing.SendsPartOffsets);
                Assert.AreEqual(result.Parts.Max(part => part.Length), sizing.LargestPartSize);
                Assert.AreEqual($"{decisions.Count(d => d.Decision == PartSizeDecision.Grew)} {decisions.Count(d => d.Decision == PartSizeDecision.ProbeAccepted)} {decisions.Count(d => d.Decision == PartSizeDecision.ProbeRejected)}",
                    $"{sizing.Grows} {sizing.ProbesAccepted} {sizing.ProbesRejected}", "The upload reports the decisions it made.\n" + evidence);
            }
        }

        // The destination ChosenPartSizesFollowTheirDecisionsForPartsNotYetRead stands in for, on a clock the test
        // controls (SendPlanned): two requests at once, each taking exactly 25 ms whatever its size, so that larger parts
        // deliver more. The upload's own plan for a file twice as large, with the same limits and tuning, so that the
        // 512 KiB probe is judged too, sizes each part as it is planned, and its real controllers judge every window: each
        // probe is kept, by what its own parts delivered against the level before it, the size doubling from 64 KiB to its
        // 512 KiB MaxPartSize, below the 1 MiB the limits allow, and holding there for the rest of the file; a part planned
        // before a change keeps its size, so no part is smaller than the one before.
        [TestMethod]
        public void ChosenPartSizesGrowToMaxPartSizeWhileLargerPartsRaiseThroughput()
        {
            TimeSpan duration = TimeSpan.FromMilliseconds(25);
            PartSizingSettings settings = PartSizingSettings.Of(UploadPartSizingMode.Automatic, null, GrowingFrom(64 * KiB, max: 512 * KiB));
            PartSizes plan = PartSizes.Plan(new PartSizeLimits(16 * KiB, 1 * MiB, 10000, 1L << 30), 12 * MiB, null, null, settings, null, "remote.bin", Stopwatch.Frequency, out ServerPlannedReason fallback);
            Assert.AreEqual(ServerPlannedReason.None, fallback);

            (List<long> lengths, List<UploadSizingTrace> sizing) = SendPlanned(plan, 12 * MiB, slots: 2, duration);

            string evidence = $"Lengths: {string.Join(", ", lengths)}\nDecisions: {string.Join("; ", sizing.Select(d => $"{d.Decision} {d.PreviousSize}->{d.Size} {d.ThroughputBytesPerSecond:0}/{d.BaselineBytesPerSecond:0}"))}";
            CollectionAssert.AreEqual(
                new[] { "Grew 128", "ProbeAccepted 128", "Grew 256", "ProbeAccepted 256", "Grew 512", "ProbeAccepted 512" },
                sizing.Select(d => $"{d.Decision} {d.Size / KiB}").ToArray(),
                "Each probe kept, up to MaxPartSize and no further.\n" + evidence);
            foreach (UploadSizingTrace kept in sizing.Where(d => d.Decision == PartSizeDecision.ProbeAccepted))
            {
                Assert.AreEqual(2 * kept.Size / duration.TotalSeconds, kept.ThroughputBytesPerSecond, 1, $"The {kept.Size} probe was judged by what its own parts delivered.\n" + evidence);
                Assert.AreEqual(kept.Size / duration.TotalSeconds, kept.BaselineBytesPerSecond, 1, $"Against the level before the {kept.Size} probe.\n" + evidence);
            }
            long[] nonfinal = lengths.Take(lengths.Count - 1).ToArray();
            Assert.AreEqual(64 * KiB, nonfinal[0], evidence);
            CollectionAssert.AreEqual(nonfinal.OrderBy(size => size).ToArray(), nonfinal, "Parts only grew, each for parts not yet planned.\n" + evidence);
            // The 512 KiB probe's own parts, and more windows of them after it was kept: the size held at MaxPartSize.
            Assert.IsTrue(nonfinal.Count(size => size == 512 * KiB) >= 12, "Parts kept MaxPartSize for windows after it was reached.\n" + evidence);
            Assert.AreEqual(12 * MiB, lengths.Sum(), evidence);
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task LargerPartsThatDoNotRaiseThroughputAreGivenUp()
        {
            using (var server = new UploadServer())
            {
                UseAws(server, 16 * KiB, 1 * MiB, 10000, 1L << 30);
                // Bandwidth limits instead: a request's time follows its size, so larger parts deliver no more. The time is
                // kept on the Stopwatch clock: with a timer, the host's lateness would lengthen the smallest parts the most
                // for their size, so that larger parts would seem to deliver more.
                server.RespondToPart = async request =>
                {
                    TimeSpan wait = TimeSpan.FromMilliseconds(request.Bytes.Length / (3.0 * KiB));
                    await DestinationClock.Wait(wait);
                    request.Mark($"served (asked {wait.TotalMilliseconds:0.0}ms)");
                    return UploadServer.Accept(request);
                };
                byte[] contents = Pattern(3 * MiB);
                string localPath = WriteFile("local.bin", contents);
                var trace = new TraceLog();
                AutomaticPartSizingOptions sizing = GrowingFrom(64 * KiB, max: 512 * KiB);
                sizing.ProbeGainPercent = 50;
                sizing.HoldWindows = 100;
                // The level larger parts are compared with is three windows, so that one window a busy host held up, as a
                // shared CI runner can, is not taken for the level.
                sizing.GrowthIntervalWindows = 3;

                UploadResult result = await Within(Client(server).Transfers.UploadFileAsync(localPath, "remote.bin", new UploadOptions
                {
                    ConcurrencyMode = UploadConcurrencyMode.Static,
                    MaxConcurrency = 2,
                    AutomaticPartSizing = sizing,
                    Trace = trace,
                }));

                string evidence = $"Chosen lengths: {string.Join(", ", result.Parts.Select(part => part.Length))}\n{trace.Evidence(server)}";
                UploadSizingTrace rejected = trace.Sizing.FirstOrDefault(d => d.Decision == PartSizeDecision.ProbeRejected);
                Assert.IsNotNull(rejected, Describe(trace) + "\n" + evidence);
                Assert.AreEqual(64 * KiB, rejected.Size, "The size went back.\n" + evidence);
                Assert.AreEqual(1, trace.Sizing.Count(d => d.Decision == PartSizeDecision.Grew), "Growth then held.\n" + evidence);
                long[] nonfinal = result.Parts.Take(result.Parts.Count - 1).Select(part => part.Length).ToArray();
                Assert.AreEqual(64 * KiB, nonfinal.Last(), "Parts read after the probe have the size it went back to.\n" + evidence);
                AssertExactUpload(server, result, contents);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task ChosenPartsKeepTheirBytesAndNumbersThroughReorderingRetriesAndRenewalWithNewHints()
        {
            using (var server = new UploadServer())
            {
                UseAws(server, 4 * KiB, 64 * KiB, 10000, 1L << 30);
                // The server's own hints, which chosen sizes do not follow, change when part 3 is renewed.
                server.PartSizeFor = (number, negotiation) => negotiation == 1 ? 5 * KiB : 9 * KiB;
                server.Expires = (number, negotiation) => number == 3 && negotiation == 1 ? DateTime.UtcNow.AddSeconds(5).ToString("o") : null;
                var part3Answered = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                server.RespondToPart = async request =>
                {
                    // Part 2 fails once; part 1 is answered only after part 3.
                    if (request.Number == 2 && request.Attempt == 1)
                    {
                        return LoopbackSite.Status(503);
                    }
                    if (request.Number == 1)
                    {
                        await part3Answered.Task;
                    }
                    ResponseMessage accepted = UploadServer.Accept(request);
                    if (request.Number == 3)
                    {
                        part3Answered.TrySetResult(true);
                    }
                    return accepted;
                };
                byte[] contents = Pattern(100 * KiB + 17);
                string localPath = WriteFile("local.bin", contents);

                UploadResult result = await Within(Client(server, retries: 1).Transfers.UploadFileAsync(localPath, "remote.bin", new UploadOptions
                {
                    ConcurrencyMode = UploadConcurrencyMode.Static,
                    MaxConcurrency = 3,
                    AutomaticPartSizing = GrowingFrom(16 * KiB, max: 16 * KiB),
                }));

                Assert.IsTrue(result.Parts.Take(result.Parts.Count - 1).All(part => part.Length == 16 * KiB), "Chosen sizes, not the server's hints.");
                PartRequest[] part2 = server.Parts.Where(part => part.Number == 2).ToArray();
                Assert.AreEqual(2, part2.Length);
                CollectionAssert.AreEqual(part2[0].Bytes, part2[1].Bytes, "The retry resends the same bytes as the same part.");
                Assert.AreEqual(2, result.Parts[1].Attempts);
                PartRequest part3 = server.Parts.Single(part => part.Number == 3);
                StringAssert.EndsWith(part3.Request.Path, "&negotiation=2", "Part 3 was sent to its renewed URL.");
                Assert.AreEqual(16 * KiB, part3.Bytes.Length, "Renewal with new hints kept the part's length.");
                AssertExactUpload(server, result, contents);
                AssertSentAsNegotiated(server);
                Assert.AreEqual("\"etag-remote.bin-2-2\"", EtagOf(server.Finishes.Single(), 2), "Part 2 completes with the ETag of its accepted attempt.");
            }
        }

        // Only an ordinary write whose every part URL is a signed S3 UploadPart URL for one upload, on a general-purpose S3
        // endpoint, signing only its host, with nothing else required of the part, lets the upload choose its sizes.
        // Anything short of that, however much it resembles S3, uses Files.com's sizes, and the URL is still sent exactly
        // as given. Fields an earlier Files.com design proposed, which current Files.com does not send, grant nothing.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("s3", DisplayName = "Straight to S3: sizes chosen")]
        [DataRow("s3, path-style", DisplayName = "Straight to S3, path-style: sizes chosen")]
        [DataRow("s3, server sizes asked for", DisplayName = "Straight to S3, ServerPlanned: Files.com's sizes")]
        [DataRow("site", DisplayName = "To Files.com, with parallel parts: Files.com's sizes")]
        [DataRow("http", DisplayName = "To S3 over HTTP")]
        [DataRow("custom endpoint", DisplayName = "To a custom endpoint")]
        [DataRow("s3-compatible", DisplayName = "To S3-compatible storage")]
        [DataRow("lookalike", DisplayName = "To a host that only contains an S3 host")]
        [DataRow("accelerate", DisplayName = "To S3 Transfer Acceleration")]
        [DataRow("length signed", DisplayName = "With Content-Length signed")]
        [DataRow("checksum", DisplayName = "With a checksum in the query")]
        [DataRow("another part number", DisplayName = "Naming another part number")]
        [DataRow("no upload id", DisplayName = "Without an upload ID")]
        [DataRow("required header", DisplayName = "With a required header")]
        [DataRow("append", DisplayName = "An append")]
        [DataRow("direct connection", DisplayName = "With direct Agent connection information")]
        [DataRow("proposed fields", DisplayName = "Proposed limit, class and offset fields on Files.com's own URLs")]
        public async Task OnlyOrdinaryWritesStraightToAmazonS3ChooseTheirPartSizes(string route)
        {
            using (var server = new UploadServer())
            {
                UseAws(server, 1, 64, 100, 4096);
                server.Aws = route.StartsWith("s3", StringComparison.Ordinal);
                server.PartSize = number => number == 1 ? 5 : 6;
                switch (route)
                {
                    case "s3, path-style":
                        server.UploadUri = (file, number, negotiation) => UploadServer.AwsPartUrl(file, number, negotiation, host: "s3.us-east-1.amazonaws.com").Replace("/part/", "/files-test-bucket/part/");
                        break;
                    case "http":
                        server.UploadUri = (file, number, negotiation) => UploadServer.AwsPartUrl(file, number, negotiation).Replace("https://", "http://");
                        break;
                    case "custom endpoint":
                        server.UploadUri = (file, number, negotiation) => UploadServer.AwsPartUrl(file, number, negotiation, host: "storage.example.com");
                        break;
                    case "s3-compatible":
                        server.UploadUri = (file, number, negotiation) => UploadServer.AwsPartUrl(file, number, negotiation, host: "files-test-bucket.s3.us-east-1.wasabisys.com");
                        break;
                    case "lookalike":
                        server.UploadUri = (file, number, negotiation) => UploadServer.AwsPartUrl(file, number, negotiation, host: UploadServer.AwsHost + ".example.com");
                        break;
                    case "accelerate":
                        server.UploadUri = (file, number, negotiation) => UploadServer.AwsPartUrl(file, number, negotiation, host: "files-test-bucket.s3-accelerate.amazonaws.com");
                        break;
                    case "length signed":
                        server.UploadUri = (file, number, negotiation) => UploadServer.AwsPartUrl(file, number, negotiation, signedHeaders: "content-length;host");
                        break;
                    case "checksum":
                        server.UploadUri = (file, number, negotiation) => UploadServer.AwsPartUrl(file, number, negotiation, extra: "&x-amz-checksum-crc32=AAAAAA%3D%3D");
                        break;
                    case "another part number":
                        server.UploadUri = (file, number, negotiation) => UploadServer.AwsPartUrl(file, number, negotiation).Replace($"&partNumber={number}&", $"&partNumber={number + 1}&");
                        break;
                    case "no upload id":
                        server.UploadUri = (file, number, negotiation) => UploadServer.AwsPartUrl(file, number, negotiation, uploadId: "");
                        break;
                    case "required header":
                        server.Aws = true;
                        server.ExtraPartFields = ",\"headers\":{\"x-amz-server-side-encryption\":\"AES256\"}";
                        break;
                    case "append":
                        server.Aws = true;
                        server.Action = "put/append";
                        break;
                    case "direct connection":
                        server.Aws = true;
                        server.ExtraPartFields = ",\"direct_connection_info\":{}";
                        break;
                    case "proposed fields":
                        server.ExtraPartFields = ",\"upload_target_class\":\"s3\",\"part_offset_query\":true,\"variable_part_limits\":{\"min_nonfinal_bytes\":1,\"max_part_bytes\":64,\"max_parts\":100,\"max_file_bytes\":4096}";
                        break;
                }
                bool chosen = route == "s3" || route == "s3, path-style";
                byte[] contents = Pattern(25);

                UploadResult result = await Within(Client(server).Transfers.UploadStreamAsync(new MemoryStream(contents), contents.Length, "remote.bin", new UploadOptions
                {
                    PartSizingMode = route == "s3, server sizes asked for" ? UploadPartSizingMode.ServerPlanned : UploadPartSizingMode.Automatic,
                    AutomaticPartSizing = GrowingFrom(10, max: 10),
                }));

                UploadPartSizing sizing = result.FinalProgress.PartSizing;
                CollectionAssert.AreEqual(chosen ? new long[] { 10, 10, 5 } : new long[] { 5, 6, 6, 6, 2 }, result.Parts.Select(part => part.Length).ToArray());
                Assert.AreEqual(chosen ? PartSizeSource.Automatic : PartSizeSource.ServerPlanned, sizing.Source);
                Assert.AreEqual(chosen ? ServerPlannedReason.None : route == "s3, server sizes asked for" ? ServerPlannedReason.Requested : ServerPlannedReason.NotAllowed, sizing.ServerPlannedReason);
                Assert.AreEqual(chosen || route == "s3, server sizes asked for" ? UploadPerformanceProfile.S3 : UploadPerformanceProfile.Generic, result.FinalProgress.Concurrency.Profile);
                Assert.IsFalse(sizing.SendsPartOffsets);
                AssertExactUpload(server, result, contents);
                AssertSentAsNegotiated(server);
            }
        }

        // While the upload chooses its sizes, every later answer, for more parts or a renewed URL, must be for the same S3
        // upload and part, signing only the host: otherwise the part it is for is not sent, and the upload stops.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("continuation", "another upload", DisplayName = "A later part's URL is for another S3 upload")]
        [DataRow("continuation", "another object", DisplayName = "A later part's URL is for another object")]
        [DataRow("continuation", "not s3", DisplayName = "A later part's URL is not S3's")]
        [DataRow("continuation", "length signed", DisplayName = "A later part's URL signs its length")]
        [DataRow("renewal", "another upload", DisplayName = "A renewed URL is for another S3 upload")]
        [DataRow("renewal", "not s3", DisplayName = "A renewed URL is not S3's")]
        public async Task RouteChangesAfterTheUploadStartedStopItBeforeTheAffectedPart(string when, string change)
        {
            using (var server = new UploadServer())
            {
                UseAws(server, 1 * KiB, 4 * KiB, 100, 1 * MiB);
                server.UploadUri = (file, number, negotiation) =>
                {
                    bool affected = when == "continuation" ? number >= 3 : negotiation >= 2;
                    if (!affected)
                    {
                        return UploadServer.AwsPartUrl(file, number, negotiation);
                    }
                    switch (change)
                    {
                        case "another upload":
                            return UploadServer.AwsPartUrl(file, number, negotiation, uploadId: "up~2");
                        case "another object":
                            return UploadServer.AwsPartUrl("other.bin", number, negotiation);
                        case "length signed":
                            return UploadServer.AwsPartUrl(file, number, negotiation, signedHeaders: "content-length;host");
                        default:
                            return server.PartUrl(file, number, negotiation);
                    }
                };
                server.Expires = (number, negotiation) => when == "renewal" && number == 3 && negotiation == 1 ? DateTime.UtcNow.AddSeconds(5).ToString("o") : null;
                byte[] contents = Pattern(16 * KiB);

                await ThrowsAsync<InvalidResponseException>(Client(server).Transfers.UploadStreamAsync(new MemoryStream(contents), contents.Length, "remote.bin", new UploadOptions
                {
                    ConcurrencyMode = UploadConcurrencyMode.Static,
                    MaxConcurrency = 1,
                    AutomaticPartSizing = GrowingFrom(2 * KiB, max: 2 * KiB),
                }));

                Assert.IsFalse(server.Parts.Any(part => part.Number >= 3), "The part the change affects is not sent, wherever its URL points.");
                Assert.AreEqual(0, server.Finishes.Count);
            }
        }

        // Where Files.com's sizes could not help either, an upload the limits cannot plan fails before anything is sent:
        // with a fixed size, or with parts held in memory, whose buffer space would hold none of the larger parts
        // Files.com would need.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(UploadPartSizingMode.Automatic, null, 3L, 4 * KiB, UploadLimit.PartCount, DisplayName = "No plan fits the file in the parts allowed, with parts in memory")]
        [DataRow(UploadPartSizingMode.Fixed, 1024L, 3L, 0, UploadLimit.PartCount, DisplayName = "No plan fits the file in the parts allowed, whatever the fixed size")]
        [DataRow(UploadPartSizingMode.Fixed, 1024L, 10L, 0, UploadLimit.FixedPartSize, DisplayName = "A fixed size needs more parts than allowed")]
        [DataRow(UploadPartSizingMode.Fixed, 512L, 10L, 0, UploadLimit.FixedPartSize, DisplayName = "A fixed size below the smallest part allowed")]
        public async Task UploadsThatCannotBePlannedWithinTheLimitsFailBeforeAnyPartIsSent(UploadPartSizingMode mode, long? fixedSize, long maxParts, int bufferBytes, UploadLimit limit)
        {
            using (var server = new UploadServer())
            {
                // Files.com says how many parts are available, fewer than S3's. Parts in memory are held to bufferBytes,
                // well below the largest part the limits allow.
                UseAws(server, 1 * KiB, bufferBytes > 0 ? 64 * KiB : 4 * KiB, 10000, 1 * MiB);
                server.ExtraPartFields = $",\"available_parts\":{maxParts}";
                var source = new ShortReadStream(Pattern(13 * KiB));
                var options = new UploadOptions { PartSizingMode = mode, PartSize = fixedSize };
                if (bufferBytes > 0)
                {
                    options.Limiter = new TransferLimiter(8, bufferBytes);
                }

                UploadLimitException error = await ThrowsAsync<UploadLimitException>(Client(server).Transfers.UploadStreamAsync(source, 13 * KiB, "remote.bin", options));

                Assert.AreEqual(limit, error.Limit);
                Assert.AreEqual(0, source.BytesRead);
                Assert.AreEqual(0, server.Parts.Count);
            }
        }

        // A fixed part size no smaller than the file, or than a stream's bound, sends it as one last part. That part may be
        // smaller than the smallest the limits allow, but not larger than the largest: a file or bounded stream larger
        // than that is refused before any of it is read or sent, though the size would be valid for a larger file.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("file", 6 * KiB, DisplayName = "A file larger than a part may be, no larger than the fixed size")]
        [DataRow("stream", 6 * KiB, DisplayName = "A stream in temporary files whose bound is larger than a part may be, no larger than the fixed size")]
        [DataRow("file", 512, DisplayName = "A file smaller than the smallest part but the last: one part")]
        public async Task AFixedSizeSendsAFileNoLargerThanItAsOnePartOnlyWithinTheLimits(string source, int length)
        {
            using (var server = new UploadServer())
            {
                UseAws(server, 1 * KiB, 4 * KiB, 100, 1 * MiB);
                byte[] contents = Pattern(length);
                string partFiles = Path.Combine(directory, "parts");
                Directory.CreateDirectory(partFiles);
                var stream = new ShortReadStream(contents);
                var options = new UploadOptions { PartSizingMode = UploadPartSizingMode.Fixed, PartSize = 8 * KiB };
                if (source == "stream")
                {
                    options.MaxStreamLength = length;
                    options.StreamPartStorage = StreamPartStorage.TemporaryFiles;
                    options.TemporaryDirectory = partFiles;
                }
                Task<UploadResult> upload = source == "file"
                    ? Client(server).Transfers.UploadFileAsync(WriteFile("local.bin", contents), "remote.bin", options)
                    : Client(server).Transfers.UploadStreamAsync(stream, "remote.bin", options);

                if (length <= 4 * KiB)
                {
                    UploadResult result = await Within(upload);
                    CollectionAssert.AreEqual(new long[] { length }, result.Parts.Select(part => part.Length).ToArray());
                    Assert.AreEqual(PartSizeSource.Fixed, result.FinalProgress.PartSizing.Source);
                    AssertExactUpload(server, result, contents);
                    AssertSentAsNegotiated(server);
                    return;
                }
                UploadLimitException error = await ThrowsAsync<UploadLimitException>(upload);
                Assert.AreEqual(UploadLimit.FixedPartSize, error.Limit);
                StringAssert.Contains(error.Message, "4096");
                Assert.AreEqual(0, stream.BytesRead, "Nothing of the stream is read.");
                Assert.AreEqual(0, server.Parts.Count, "No part is sent.");
                Assert.AreEqual(0, server.Finishes.Count);
            }
        }

        // The limits are the SDK's own: a file, or a stream kept in temporary files, that they cannot fit in the parts
        // Files.com makes available is sent as Files.com sizes it, which may be in larger parts than the SDK chooses.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(true, DisplayName = "A file")]
        [DataRow(false, DisplayName = "A stream of unknown length in temporary files")]
        public async Task UploadsTheLimitsCannotPlanAreSentAsFilesComSizesThem(bool file)
        {
            using (var server = new UploadServer())
            {
                UseAws(server, 1 * KiB, 4 * KiB, 10000, 1 * MiB);
                server.ExtraPartFields = ",\"available_parts\":3";
                server.PartSize = number => 5 * KiB;
                byte[] contents = Pattern(13 * KiB);
                string partFiles = Path.Combine(directory, "parts");
                Directory.CreateDirectory(partFiles);

                UploadResult result = await Within(file
                    ? Client(server).Transfers.UploadFileAsync(WriteFile("local.bin", contents), "remote.bin")
                    : Client(server).Transfers.UploadStreamAsync(new ForwardOnlyStream(contents, 3000), "remote.bin", new UploadOptions { StreamPartStorage = StreamPartStorage.TemporaryFiles, TemporaryDirectory = partFiles }));

                UploadPartSizing sizing = result.FinalProgress.PartSizing;
                CollectionAssert.AreEqual(new long[] { 5 * KiB, 5 * KiB, 3 * KiB }, result.Parts.Select(part => part.Length).ToArray());
                Assert.AreEqual(PartSizeSource.ServerPlanned, sizing.Source);
                Assert.AreEqual(ServerPlannedReason.NotAllowed, sizing.ServerPlannedReason);
                Assert.IsNull(sizing.MaxStreamLength, "Nothing bounds the stream.");
                AssertExactUpload(server, result, contents);
                AssertSentAsNegotiated(server);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task PartsAreMadeLargerWhenSmallerOnesWouldLeaveTooFewForTheRestOfTheFile()
        {
            using (var server = new UploadServer())
            {
                UseAws(server, 1 * KiB, 64 * KiB, 10000, 1 * MiB);
                server.ExtraPartFields = ",\"available_parts\":8";
                byte[] contents = Pattern(300 * KiB);

                UploadResult result = await Within(Client(server).Transfers.UploadStreamAsync(new MemoryStream(contents), contents.Length, "remote.bin", new UploadOptions
                {
                    AutomaticPartSizing = GrowingFrom(4 * KiB, max: 4 * KiB),
                }));

                Assert.IsTrue(result.Parts.Count <= 8, $"{result.Parts.Count} parts");
                Assert.IsTrue(result.Parts.All(part => part.Length <= 64 * KiB));
                Assert.AreEqual(PartSizeConstraint.PartLimit, result.FinalProgress.PartSizing.StartConstraint);
                AssertExactUpload(server, result, contents);
            }
        }

        // Complete direct connection information, as Files.com supplies it for a destination it can be sent to directly
        // (a Files.com Agent or Integration Worker), and the same without its CA certificate, which is not complete.
        private const string DirectConnection = ",\"direct_connection_info\":{\"version\":1,\"server_name\":\"agent.example\",\"addresses\":[\"10.0.0.5:443\"],\"direct_uri\":\"https://agent.example/upload\",\"ca_pem\":\"PEM\"}";
        private const string IncompleteDirectConnection = ",\"direct_connection_info\":{\"version\":1,\"server_name\":\"agent.example\",\"addresses\":[\"10.0.0.5:443\"],\"direct_uri\":\"https://agent.example/upload\"}";

        // The default tuning follows the destination the negotiated parts show, as the Go SDK chooses it: direct connection
        // information first, then an S3 URL, which takes the scaled S3 policy without the caller naming it, then anything
        // else; tuning the caller gives, the conservative S3 preset included, is used as it is, whatever the destination.
        // The scaled S3 policy the SDK chose warm-starts at 1 part at a time (within MaxConcurrency), until its answered
        // parts show how fast the link is; given as tuning, or chosen with UploadStartup.Aggressive, it starts at its own
        // 150. Aggressive changes nothing else: other destinations keep their defaults, and tuning the caller gives wins.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(true, null, null, null, UploadPerformanceProfile.S3, UploadProfileSource.Server, ConcurrencyController.WarmStartTarget, 1024, DisplayName = "The scaled S3 policy for parts that go straight to S3, warm-starting")]
        [DataRow(true, null, "ScaledS3", null, UploadPerformanceProfile.S3, UploadProfileSource.Options, 150, 1024, DisplayName = "The scaled S3 policy given as tuning starts at its own start")]
        [DataRow(true, null, "+Aggressive", null, UploadPerformanceProfile.S3, UploadProfileSource.Server, 150, 1024, DisplayName = "Startup Aggressive starts the scaled S3 policy chosen for S3 at its start")]
        [DataRow(false, null, "+Aggressive", null, UploadPerformanceProfile.Generic, UploadProfileSource.Server, 4, 8, DisplayName = "Startup Aggressive keeps the generic defaults for parts that go to Files.com")]
        [DataRow(true, DirectConnection, "+Aggressive", null, UploadPerformanceProfile.Agent, UploadProfileSource.Server, 2, 8, DisplayName = "Startup Aggressive keeps the Agent defaults for a direct destination")]
        [DataRow(true, null, "S3+Aggressive", null, UploadPerformanceProfile.S3, UploadProfileSource.Options, 8, 64, DisplayName = "The caller's tuning wins over Startup Aggressive")]
        [DataRow(false, null, null, null, UploadPerformanceProfile.Generic, UploadProfileSource.Server, 4, 8, DisplayName = "Generic defaults for parts that go to Files.com")]
        [DataRow(true, null, "Generic", null, UploadPerformanceProfile.Generic, UploadProfileSource.Options, 4, 8, DisplayName = "The caller's own tuning wins")]
        [DataRow(true, null, "S3", null, UploadPerformanceProfile.S3, UploadProfileSource.Options, 8, 64, DisplayName = "The caller's conservative S3 preset wins over the scaled default")]
        [DataRow(true, null, null, 3, UploadPerformanceProfile.S3, UploadProfileSource.Server, ConcurrencyController.WarmStartTarget, 3, DisplayName = "MaxConcurrency still narrows the destination's defaults")]
        [DataRow(false, DirectConnection, null, null, UploadPerformanceProfile.Agent, UploadProfileSource.Server, 2, 8, DisplayName = "Agent defaults for a destination with direct connection information")]
        [DataRow(true, DirectConnection, null, null, UploadPerformanceProfile.Agent, UploadProfileSource.Server, 2, 8, DisplayName = "Direct connection information comes before an S3 URL")]
        [DataRow(true, DirectConnection, "Generic", null, UploadPerformanceProfile.Generic, UploadProfileSource.Options, 4, 8, DisplayName = "The caller's own tuning wins over a direct destination too")]
        [DataRow(false, IncompleteDirectConnection, null, null, UploadPerformanceProfile.Generic, UploadProfileSource.Server, 4, 8, DisplayName = "Incomplete direct connection information is no direct destination")]
        public async Task TheDestinationThePartURLsShowChoosesTheDefaultTuning(bool aws, string directConnection, string callerTuning, int? maxConcurrency, UploadPerformanceProfile profile, UploadProfileSource source, int start, int max)
        {
            using (var server = new UploadServer())
            {
                server.Aws = aws;
                server.ExtraPartFields = directConnection ?? "";
                var progress = new ProgressLog();
                byte[] contents = Pattern(40);
                // callerTuning names the tuning given, if any, then "+Aggressive" for that startup.
                string[] choice = (callerTuning ?? "").Split('+');
                string tuning = choice[0] == "" ? null : choice[0];

                UploadResult result = await Within(Client(server).Transfers.UploadStreamAsync(new MemoryStream(contents), contents.Length, "remote.bin", new UploadOptions
                {
                    AutomaticConcurrency = tuning == null ? null : tuning == "ScaledS3" ? AutomaticConcurrencyOptions.ScaledS3() : AutomaticConcurrencyOptions.For((UploadPerformanceProfile)Enum.Parse(typeof(UploadPerformanceProfile), tuning)),
                    Startup = choice.Length > 1 ? (UploadStartup)Enum.Parse(typeof(UploadStartup), choice[1]) : UploadStartup.Default,
                    MaxConcurrency = maxConcurrency,
                    Progress = progress,
                }));

                UploadConcurrency first = progress.Reports.First().Concurrency;
                Assert.AreEqual(profile, first.Profile);
                Assert.AreEqual(source, first.ProfileSource);
                Assert.AreEqual(start, first.Target, "Settled before the first part is sent.");
                Assert.AreEqual(max, result.FinalProgress.Concurrency.MaxTarget);
                Assert.AreEqual(profile, result.FinalProgress.Concurrency.Profile, "The profile holds for the whole upload.");
                Assert.AreEqual(source, result.FinalProgress.Concurrency.ProfileSource);
                Assert.IsTrue(server.Negotiations.All(negotiation => negotiation.GetProperty("with_direct_connection_info").GetBoolean()), "Every negotiation asks for direct connection information.");
                AssertExactUpload(server, result, contents);
            }
        }

        // The destination is described once, from the first negotiation: a later negotiation whose parts show another
        // destination changes neither the profile nor the tuning the upload started with.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(null, UploadPerformanceProfile.Generic, UploadProfileSource.Server, 8, DisplayName = "The destination's defaults hold through later negotiations")]
        [DataRow("S3", UploadPerformanceProfile.S3, UploadProfileSource.Options, 64, DisplayName = "The caller's tuning holds through later negotiations")]
        public async Task TheTuningHoldsWhenALaterNegotiationShowsAnotherDestination(string callerTuning, UploadPerformanceProfile profile, UploadProfileSource source, int max)
        {
            using (var server = new UploadServer())
            {
                // The first negotiation returns part 1 alone, without direct connection information; every part negotiated
                // after it carries it.
                server.ExtraPartFieldsFor = (number, negotiation) => number > 1 ? DirectConnection : "";
                var progress = new ProgressLog();
                byte[] contents = Pattern(40);

                UploadResult result = await Within(Client(server).Transfers.UploadStreamAsync(new MemoryStream(contents), contents.Length, "remote.bin", new UploadOptions
                {
                    AutomaticConcurrency = callerTuning == null ? null : AutomaticConcurrencyOptions.For((UploadPerformanceProfile)Enum.Parse(typeof(UploadPerformanceProfile), callerTuning)),
                    Progress = progress,
                }));

                Assert.IsTrue(server.Negotiations.Count > 1, $"{server.Negotiations.Count} negotiations: the later parts were negotiated after the first.");
                Assert.IsTrue(progress.Reports.All(report => report.Concurrency.Profile == profile && report.Concurrency.ProfileSource == source && report.Concurrency.MaxTarget == max), "Every report keeps the profile, its source and the tuning: " + string.Join(" | ", progress.Reports.Select(report => $"{report.Concurrency.Profile}/{report.Concurrency.ProfileSource}/{report.Concurrency.MaxTarget}").Distinct()));
                Assert.AreEqual(profile, result.FinalProgress.Concurrency.Profile);
                Assert.AreEqual(source, result.FinalProgress.Concurrency.ProfileSource);
                AssertExactUpload(server, result, contents);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        // No offset is ever added to a part's URL, whatever the answer says: an offset permission proposed for an earlier
        // Files.com design, which current Files.com does not send, grants nothing, and a URL that already names the part's
        // own offset is sent as it is.
        [DataRow(",\"upload_target_class\":\"agent_fiw\"", "", DisplayName = "An Agent destination")]
        [DataRow(",\"upload_target_class\":\"agent_fiw\",\"part_offset_query\":true", "&cap=offset-v1", DisplayName = "A proposed offset permission grants nothing")]
        [DataRow("", "&part_offset={offset}", DisplayName = "The URL already has the part's offset")]
        public async Task NoOffsetIsAddedToAPartsUrl(string fields, string query)
        {
            using (var server = new UploadServer())
            {
                server.ExtraPartFields = fields;
                server.UploadUri = (file, number, negotiation) => $"{server.Site.Url}/part/{file}/{number}?jwt=j{query.Replace("{offset}", ((number - 1) * 8).ToString())}";
                byte[] contents = Pattern(20);

                UploadResult result = await Within(Client(server).Transfers.UploadStreamAsync(new MemoryStream(contents), contents.Length, "remote.bin"));

                foreach (PartRequest part in server.Parts)
                {
                    Assert.AreEqual($"/part/remote.bin/{part.Number}?jwt=j{query.Replace("{offset}", ((part.Number - 1) * 8).ToString())}", part.Request.Path);
                }
                Assert.IsFalse(result.FinalProgress.PartSizing.SendsPartOffsets);
                Assert.AreEqual(UploadPerformanceProfile.Generic, result.FinalProgress.Concurrency.Profile, "Nothing Files.com sends identifies an Agent destination.");
                AssertExactUpload(server, result, contents);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("&part_offset=5", DisplayName = "The URL names another offset")]
        [DataRow("&part_offset=0&part_offset=0", DisplayName = "The URL names part_offset twice")]
        [DataRow("renewed&part_offset=5", DisplayName = "The part's renewed URL names another offset")]
        public async Task PartsWhoseUrlsNameAnotherOffsetStopTheUploadBeforeTheyAreSent(string change)
        {
            using (var server = new UploadServer())
            {
                bool renewed = change.StartsWith("renewed", StringComparison.Ordinal);
                string query = renewed ? change.Substring("renewed".Length) : change;
                server.Expires = (number, negotiation) => renewed && number == 1 && negotiation == 1 ? DateTime.UtcNow.AddSeconds(5).ToString("o") : null;
                server.UploadUri = (file, number, negotiation) => $"{server.Site.Url}/part/{file}/{number}?jwt=j" + (number == 1 && negotiation == (renewed ? 2 : 1) ? query : "");
                byte[] contents = Pattern(20);

                await ThrowsAsync<InvalidResponseException>(Client(server).Transfers.UploadStreamAsync(new MemoryStream(contents), contents.Length, "remote.bin", new UploadOptions
                {
                    ConcurrencyMode = UploadConcurrencyMode.Static,
                    MaxConcurrency = 1,
                }));

                Assert.IsFalse(server.Parts.Any(part => part.Number == 1), "The part is not sent.");
                Assert.AreEqual(0, server.Finishes.Count);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(0, DisplayName = "An empty stream")]
        [DataRow(21, DisplayName = "A final part shorter than the others")]
        [DataRow(24, DisplayName = "Ending exactly at the end of a part")]
        public async Task StreamOfUnknownLengthIsSentOnePartAtATimeAsFilesComPlansIt(int length)
        {
            using (var server = new UploadServer())
            {
                // The server says parts may be sent in parallel, as a legacy answer for a stream of unknown length can.
                server.ParallelParts = "true";
                server.RespondToPart = request => Task.FromResult(request.Number == 2 && request.Attempt == 1 ? LoopbackSite.Status(503) : UploadServer.Accept(request));
                byte[] contents = Pattern(length);
                var source = new ShortReadStream(contents);
                var progress = new ProgressLog();

                UploadResult result = await Within(Client(server, retries: 1).Transfers.UploadStreamAsync(source, "remote.bin", new UploadOptions { Progress = progress }));

                Assert.IsFalse(server.Negotiations[0].TryGetProperty("size", out _), "The upload starts without a size.");
                Assert.AreEqual(1, server.MaxPartsInFlight, "One part at a time.");
                Assert.AreEqual(Math.Max(1, (length + 7) / 8), result.Parts.Count, "No extra part at the end.");
                Assert.AreEqual(length, result.Size);
                Assert.AreEqual(length, server.Finishes.Single().GetProperty("size").GetInt64());
                Assert.AreEqual(UploadSerialReason.LengthUnknown, result.FinalProgress.Concurrency.SerialReason);
                Assert.AreEqual(ServerPlannedReason.LengthUnknown, result.FinalProgress.PartSizing.ServerPlannedReason);
                Assert.IsFalse(progress.Reports.First().TotalBytesKnown);
                Assert.IsTrue(result.FinalProgress.TotalBytesKnown);
                Assert.AreEqual(length, result.FinalProgress.TotalBytes);
                if (length > 8)
                {
                    PartRequest[] part2 = server.Parts.Where(part => part.Number == 2).ToArray();
                    CollectionAssert.AreEqual(part2[0].Bytes, part2[1].Bytes, "The retry resends the bytes read.");
                }
                Assert.IsTrue(source.CanRead, "The borrowed stream is left open.");
                AssertExactUpload(server, result, contents);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task StreamOfUnknownLengthGrowsItsPartsBeforeItEndsWhereFilesComAllowsChoosingThem()
        {
            using (var server = new UploadServer())
            {
                UseAws(server, 16 * KiB, 1 * MiB, 10000, 1L << 30);
                server.RespondToPart = async request =>
                {
                    await Task.Delay(25);
                    return UploadServer.Accept(request);
                };
                byte[] contents = Pattern(4 * MiB + 1234);
                var source = new ForwardOnlyStream(contents, 7919);
                var trace = new TraceLog();

                UploadResult result = await Within(Client(server).Transfers.UploadStreamAsync(source, "remote.bin", new UploadOptions
                {
                    ConcurrencyMode = UploadConcurrencyMode.Static,
                    MaxConcurrency = 2,
                    AutomaticPartSizing = GrowingFrom(64 * KiB, max: 512 * KiB, unknownStart: 64 * KiB),
                    Trace = trace,
                }));

                Assert.IsFalse(server.Negotiations[0].TryGetProperty("size", out _), "The upload starts without a size.");
                long[] nonfinal = result.Parts.Take(result.Parts.Count - 1).Select(part => part.Length).ToArray();
                Assert.AreEqual(64 * KiB, nonfinal[0]);
                Assert.IsTrue(nonfinal.Distinct().Count() >= 2, "At least two unequal parts before the stream ended: " + string.Join(", ", nonfinal));
                Assert.IsTrue(trace.Sizing.Any(d => d.Decision == PartSizeDecision.ProbeAccepted), Describe(trace));
                Assert.AreEqual(2, server.MaxPartsInFlight, "Parts of a stream sized within limits go several at a time.");
                Assert.AreEqual(contents.Length, result.Size);
                Assert.AreEqual(contents.Length, server.Finishes.Single().GetProperty("size").GetInt64());
                UploadPartSizing sizing = result.FinalProgress.PartSizing;
                Assert.AreEqual(PartSizeSource.Automatic, sizing.Source);
                Assert.AreEqual(1L << 30, sizing.MaxStreamLength, "The bound is the largest file the limits allow.");
                Assert.AreEqual(UploadSerialReason.None, result.FinalProgress.Concurrency.SerialReason);
                AssertExactUpload(server, result, contents);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(0, DisplayName = "An empty stream: one empty part")]
        [DataRow(48 * KiB + 5, DisplayName = "A final part shorter than the smallest allowed")]
        [DataRow(48 * KiB, DisplayName = "Ending exactly at the end of a part: no extra part")]
        public async Task StreamOfUnknownLengthSizedWithinLimitsEndsExactlyWhereItDoes(int length)
        {
            using (var server = new UploadServer())
            {
                UseAws(server, 16 * KiB, 64 * KiB, 10000, 1L << 30);
                byte[] contents = Pattern(length);

                UploadResult result = await Within(Client(server).Transfers.UploadStreamAsync(new ForwardOnlyStream(contents, 3000), "remote.bin", new UploadOptions
                {
                    ConcurrencyMode = UploadConcurrencyMode.Static,
                    MaxConcurrency = 3,
                    MaxStreamLength = 1 * MiB,
                    AutomaticPartSizing = GrowingFrom(16 * KiB, max: 16 * KiB, unknownStart: 16 * KiB),
                }));

                Assert.AreEqual(PartSizeSource.Automatic, result.FinalProgress.PartSizing.Source);
                Assert.IsFalse(server.Negotiations[0].TryGetProperty("size", out _), "The upload starts without a size.");
                Assert.AreEqual(Math.Max(1, (length + 16 * KiB - 1) / (16 * KiB)), result.Parts.Count, "No padding and no extra part at the end.");
                Assert.IsTrue(result.Parts.Take(result.Parts.Count - 1).All(part => part.Length == 16 * KiB));
                Assert.AreEqual(length, result.Size);
                AssertExactUpload(server, result, contents);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(256 * KiB, DisplayName = "Ending at the bound")]
        [DataRow(200 * KiB, DisplayName = "Ending before the bound")]
        public async Task PartsOfAStreamOfUnknownLengthLeaveEnoughPartsForTheMostItMayHold(int length)
        {
            using (var server = new UploadServer())
            {
                UseAws(server, 1 * KiB, 64 * KiB, 8, 256 * KiB);
                byte[] contents = Pattern(length);

                UploadResult result = await Within(Client(server).Transfers.UploadStreamAsync(new ForwardOnlyStream(contents, 1000), "remote.bin", new UploadOptions
                {
                    AutomaticPartSizing = GrowingFrom(4 * KiB, max: 4 * KiB, unknownStart: 4 * KiB),
                }));

                Assert.IsTrue(result.Parts.Count <= 8, $"{result.Parts.Count} parts");
                Assert.IsTrue(result.Parts.All(part => part.Length <= 64 * KiB));
                Assert.IsTrue(result.Parts.Take(result.Parts.Count - 1).Select(part => part.Length).Distinct().Count() >= 2, "Parts grew to keep the bound within the parts left.");
                AssertExactUpload(server, result, contents);
            }
        }

        // A failure of your stream is its own, whatever its type: an InvalidOperationException from a read is thrown as
        // it is, never as an UploadLimitException, whether the stream's parts are read as they are sent, it has no
        // known length, or it is copied to a spool file first.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("known length")]
        [DataRow("unknown length")]
        [DataRow("spooled")]
        public async Task AStreamsOwnInvalidOperationExceptionIsNotALimit(string kind)
        {
            using (var server = new UploadServer())
            {
                var own = new InvalidOperationException("The source's own state does not allow another read.");
                var source = new ShortReadStream(Pattern(20 * KiB), 6 * KiB, own);
                TransferOperations transfers = Client(server).Transfers;
                Task<UploadResult> upload = kind == "known length" ? transfers.UploadStreamAsync(source, 20 * KiB, "remote.bin")
                    : kind == "unknown length" ? transfers.UploadStreamAsync(source, "remote.bin")
                    : transfers.UploadStreamAsync(source, "remote.bin", new UploadOptions { SpoolMaxBytes = 1 * MiB, TemporaryDirectory = directory });

                InvalidOperationException error = await ThrowsAsync<InvalidOperationException>(upload);

                Assert.AreSame(own, error);
                Assert.AreEqual(0, server.Finishes.Count);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("adaptive", DisplayName = "Sized within limits, beyond the largest file allowed")]
        [DataRow("declared", DisplayName = "Sized within limits, beyond MaxStreamLength")]
        [DataRow("serial", DisplayName = "Sized by Files.com, beyond MaxStreamLength")]
        public async Task StreamLongerThanItsBoundFailsWithoutSendingTheExcessOrCompleting(string bound)
        {
            using (var server = new UploadServer())
            {
                if (bound != "serial")
                {
                    UseAws(server, 1 * KiB, 8 * KiB, 100, bound == "adaptive" ? 20 * KiB : 1 * MiB);
                }
                server.PartSize = number => 8 * KiB;
                byte[] contents = Pattern(20 * KiB + 1);

                UploadLimitException error = await ThrowsAsync<UploadLimitException>(Client(server).Transfers.UploadStreamAsync(new ForwardOnlyStream(contents, 3000), "remote.bin", new UploadOptions
                {
                    MaxStreamLength = bound == "adaptive" ? (long?)null : 20 * KiB,
                    AutomaticPartSizing = GrowingFrom(8 * KiB, max: 8 * KiB, unknownStart: 8 * KiB),
                }));

                Assert.AreEqual(UploadLimit.StreamLength, error.Limit);
                Assert.AreEqual(0, server.Finishes.Count, "A stream longer than its bound is never completed.");
                Assert.IsTrue(server.Parts.Sum(part => part.Bytes.Length) <= 20 * KiB, "The excess is never sent.");
            }
        }

        // Where a stream's parts go straight to S3 but are Files.com's, asked for or because parts in memory could not be
        // large enough, the SDK's limits do not bind them: a stream with more parts, and more bytes, than the limits
        // allow is sent as Files.com sizes it, one part at a time, and completes. Only MaxStreamLength and the parts
        // Files.com makes available bound it.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(UploadPartSizingMode.ServerPlanned, 0, null, null, true, DisplayName = "Files.com's sizes, asked for: a stream beyond the limits completes")]
        [DataRow(UploadPartSizingMode.Automatic, 4 * KiB, null, null, true, DisplayName = "Files.com's sizes, as parts in memory could not be large enough: a stream beyond the limits completes")]
        [DataRow(UploadPartSizingMode.ServerPlanned, 0, 2L, null, false, DisplayName = "Files.com's sizes: a stream longer than the parts Files.com makes available hold")]
        [DataRow(UploadPartSizingMode.ServerPlanned, 0, null, 16 * KiB, false, DisplayName = "Files.com's sizes: a stream longer than MaxStreamLength")]
        public async Task StreamOfUnknownLengthOnFilesComsSizesIsBoundOnlyByItsCallerAndFilesCom(UploadPartSizingMode mode, int bufferBytes, long? availableParts, int? maxStreamLength, bool completes)
        {
            using (var server = new UploadServer())
            {
                // The limits allow 2 parts and 12 KiB. Files.com plans parts of 8 KiB, or of 4 KiB where parts in memory
                // hold at most that.
                long partsize = bufferBytes > 0 ? bufferBytes : 8 * KiB;
                UseAws(server, 1 * KiB, 64 * KiB, 2, 12 * KiB);
                server.PartSize = number => partsize;
                if (availableParts.HasValue)
                {
                    server.ExtraPartFields = $",\"available_parts\":{availableParts}";
                }
                // Part 3's URL is renewed; the part keeps its bytes.
                server.Expires = (number, negotiation) => number == 3 && negotiation == 1 ? DateTime.UtcNow.AddSeconds(5).ToString("o") : null;
                byte[] contents = Pattern(16 * KiB + 1);
                var options = new UploadOptions { PartSizingMode = mode, MaxStreamLength = maxStreamLength };
                if (bufferBytes > 0)
                {
                    options.Limiter = new TransferLimiter(8, bufferBytes);
                }
                Task<UploadResult> upload = Client(server).Transfers.UploadStreamAsync(new ForwardOnlyStream(contents, 3000), "remote.bin", options);

                if (completes)
                {
                    UploadResult result = await Within(upload);
                    UploadPartSizing sizing = result.FinalProgress.PartSizing;
                    Assert.AreEqual(PartSizeSource.ServerPlanned, sizing.Source);
                    Assert.AreEqual(mode == UploadPartSizingMode.ServerPlanned ? ServerPlannedReason.Requested : ServerPlannedReason.MemoryLimit, sizing.ServerPlannedReason);
                    Assert.IsNull(sizing.MaxStreamLength, "Nothing bounds the stream.");
                    Assert.AreEqual(1, server.MaxPartsInFlight, "One part at a time.");
                    CollectionAssert.AreEqual(Enumerable.Repeat(partsize, (int)(16 * KiB / partsize)).Concat(new[] { 1L }).ToArray(), result.Parts.Select(part => part.Length).ToArray());
                    StringAssert.EndsWith(server.Parts.Single(part => part.Number == 3).Request.Path, "&negotiation=2", "Part 3 was sent to its renewed URL.");
                    AssertExactUpload(server, result, contents);
                    AssertSentAsNegotiated(server);
                }
                else
                {
                    UploadLimitException error = await ThrowsAsync<UploadLimitException>(upload);
                    Assert.AreEqual(availableParts.HasValue ? UploadLimit.PartCount : UploadLimit.StreamLength, error.Limit);
                    Assert.AreEqual(0, server.Finishes.Count, "A stream longer than its bound is never completed.");
                    Assert.IsTrue(server.Parts.Sum(part => part.Bytes.Length) <= 16 * KiB, "The excess is never sent.");
                }
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(0, DisplayName = "An empty stream is one empty part")]
        [DataRow(1, DisplayName = "A stream with one byte is too long")]
        public async Task StreamWithABoundOfZeroMayOnlyBeEmpty(int length)
        {
            using (var server = new UploadServer())
            {
                byte[] contents = Pattern(length);
                Task<UploadResult> upload = Client(server).Transfers.UploadStreamAsync(new ForwardOnlyStream(contents, 10), "remote.bin", new UploadOptions { MaxStreamLength = 0 });

                if (length == 0)
                {
                    UploadResult result = await Within(upload);
                    Assert.AreEqual(1, result.Parts.Count);
                    AssertExactUpload(server, result, contents);
                }
                else
                {
                    Assert.AreEqual(UploadLimit.StreamLength, (await ThrowsAsync<UploadLimitException>(upload)).Limit);
                    Assert.AreEqual(0, server.Parts.Count + server.Finishes.Count, "Nothing is sent.");
                }
            }
        }

        // Parts Files.com plans for a stream going straight to S3 are read and sent as planned, larger or smaller than
        // the limits allow the parts the SDK chooses.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(96 * KiB, 200 * KiB, DisplayName = "Parts larger than the limits allow")]
        [DataRow(512, 2 * KiB + 100, DisplayName = "Parts smaller than the limits allow any part but the last")]
        public async Task PartsFilesComPlansForAStreamAreSentAsPlannedWhateverTheLimits(int partsize, int length)
        {
            using (var server = new UploadServer())
            {
                UseAws(server, 1 * KiB, 64 * KiB, 100, 1 * MiB);
                server.PartSize = number => partsize;
                byte[] contents = Pattern(length);

                UploadResult result = await Within(Client(server).Transfers.UploadStreamAsync(new ShortReadStream(contents), "remote.bin", new UploadOptions { PartSizingMode = UploadPartSizingMode.ServerPlanned }));

                long[] expected = Enumerable.Repeat((long)partsize, length / partsize).Concat(new[] { (long)(length % partsize) }).ToArray();
                CollectionAssert.AreEqual(expected, result.Parts.Select(part => part.Length).ToArray());
                Assert.AreEqual(ServerPlannedReason.Requested, result.FinalProgress.PartSizing.ServerPlannedReason);
                AssertExactUpload(server, result, contents);
                AssertSentAsNegotiated(server);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(false, DisplayName = "Every upload completes")]
        [DataRow(true, DisplayName = "One upload is cancelled; the others complete")]
        public async Task StreamsSharingATinyLimiterReadOnlyWhatItsBufferSpaceCovers(bool cancelOne)
        {
            using (var serial = new UploadServer("key-a", "11"))
            using (var chosen = new UploadServer("key-b", "22"))
            using (var spooled = new UploadServer("key-c", "33"))
            {
                // Sixteen bytes of buffer space for three uploads: one on Files.com's sizes, one choosing its sizes, and
                // one spooled first. The limiter must be able to hand every part out in turn.
                serial.PartSize = number => 8;
                UseAws(chosen, 4, 16, 100, 1 * KiB);
                foreach (UploadServer site in new[] { serial, chosen, spooled })
                {
                    AnswerAfter(site, TimeSpan.FromMilliseconds(5));
                }
                var limiter = new TransferLimiter(maxConcurrentRequests: 2, maxBufferedBytes: 16);
                var overdrawn = new ConcurrentQueue<string>();
                MeteredStream serialSource = null;
                MeteredStream chosenSource = null;
                // Each byte a stream gives goes into a buffer the limiter has handed out, and stays there until its site
                // has accepted it. The limiter is read first: any release since was preceded by its site's acceptance.
                Action<string, int> check = (name, count) =>
                {
                    long charged = limiter.BufferedBytes;
                    long held = serialSource.BytesRead - serial.AcceptedBytes("serial.bin").Length + chosenSource.BytesRead - chosen.AcceptedBytes("chosen.bin").Length;
                    if (held + (name == "spooled" ? count : 0) > charged || count > limiter.MaxBufferedBytes)
                    {
                        overdrawn.Enqueue($"{name} read {count} with {held} held and {charged} charged");
                    }
                };
                byte[] a = Pattern(50);
                byte[] b = Pattern(45, seed: 3);
                byte[] c = Pattern(60, seed: 5);
                serialSource = new MeteredStream("serial", a, 5, check);
                chosenSource = new MeteredStream("chosen", b, 7, check);
                var spooledSource = new MeteredStream("spooled", c, 100, check);
                var cancellation = new CancellationTokenSource();
                if (cancelOne)
                {
                    chosen.RespondToPart = async request =>
                    {
                        await Task.Delay(5);
                        if (request.Number == 2)
                        {
                            cancellation.Cancel();
                        }
                        return UploadServer.Accept(request);
                    };
                }

                Task<UploadResult> uploadA = Client(serial).Transfers.UploadStreamAsync(serialSource, "serial.bin", new UploadOptions { Limiter = limiter });
                Task<UploadResult> uploadB = Client(chosen).Transfers.UploadStreamAsync(chosenSource, "chosen.bin", new UploadOptions { Limiter = limiter, AutomaticPartSizing = GrowingFrom(8, max: 8, unknownStart: 8) }, cancellation.Token);
                Task<UploadResult> uploadC = Client(spooled).Transfers.UploadStreamAsync(spooledSource, "spooled.bin", new UploadOptions { Limiter = limiter, SpoolMaxBytes = 100 });

                AssertExactUpload(serial, await Within(uploadA), a, "serial.bin");
                AssertExactUpload(spooled, await Within(uploadC), c, "spooled.bin");
                if (cancelOne)
                {
                    await AssertCancelledPromptly(uploadB);
                    Assert.AreEqual(0, chosen.Finishes.Count);
                }
                else
                {
                    AssertExactUpload(chosen, await Within(uploadB), b, "chosen.bin");
                }
                Assert.IsTrue(overdrawn.IsEmpty, string.Join("; ", overdrawn));
                Assert.AreEqual(0, limiter.BufferedBytes + limiter.ActiveRequests + limiter.QueuedBufferReservations + limiter.QueuedRequests, "Everything is released.");
                Assert.IsTrue(serialSource.CanRead && chosenSource.CanRead && spooledSource.CanRead, "Borrowed streams are left open.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task SpoolCopyWaitsForTheSharedLimitersBufferSpaceBeforeAnythingIsSent()
        {
            using (var busy = new UploadServer("key-a", "11") { HoldParts = true, PartSize = number => 16 })
            using (var spooled = new UploadServer("key-b", "22"))
            {
                var limiter = new TransferLimiter(maxConcurrentRequests: 4, maxBufferedBytes: 16);
                byte[] held = Pattern(16);
                byte[] copied = Pattern(40, seed: 7);
                var source = new ShortReadStream(copied);

                // The first upload holds all of the buffer space with a part the site has not answered.
                Task<UploadResult> holding = Client(busy).Transfers.UploadStreamAsync(new MemoryStream(held), held.Length, "held.bin", new UploadOptions { Limiter = limiter });
                await busy.WaitUntil(site => site.Parts.Count == 1, "the first upload's part is held");
                Task<UploadResult> spooling = Client(spooled).Transfers.UploadStreamAsync(source, "copied.bin", new UploadOptions { Limiter = limiter, SpoolMaxBytes = 100 });
                await Task.Delay(200);

                Assert.AreEqual(0, source.BytesRead, "Nothing is read before the copy has buffer space.");
                Assert.AreEqual(0, spooled.Site.Requests.Count, "Nothing is sent before the copy is complete.");
                Assert.AreEqual(1, limiter.QueuedBufferReservations, "The copy waits for its turn.");
                busy.ReleaseHeldParts();
                await Within(holding);
                UploadResult result = await Within(spooling);
                Assert.AreEqual(copied.Length, spooled.Negotiations[0].GetProperty("size").GetInt64(), "Uploaded with its size known.");
                AssertExactUpload(spooled, result, copied, "copied.bin");
                Assert.AreEqual(0, limiter.BufferedBytes);
            }
        }

        [TestMethod]
        public void SizeProbesAreJudgedOnlyByPartsOfTheNewSize()
        {
            var driver = new SizingDriver(slots: 4);
            driver.StartProbe(baseline: 100);
            driver.Complete(epoch: 1, bytes: 200, after: TimeSpan.FromSeconds(1));
            // Parts sized before the probe finish after its first part, and quickly.
            for (int i = 0; i < 3; i++)
            {
                driver.Complete(epoch: 0, bytes: 100, after: TimeSpan.FromSeconds(0.1));
            }
            // The new size delivers no more than the old: 800 bytes in 8 seconds.
            for (int i = 0; i < 4; i++)
            {
                driver.Complete(epoch: 1, bytes: 200, after: TimeSpan.FromSeconds(2));
            }

            CollectionAssert.AreEqual(new[] { PartSizeDecision.Grew, PartSizeDecision.ProbeRejected }, driver.Decisions.Select(d => d.Decision).ToArray(), driver.Describe());
            Assert.AreEqual(100, driver.Decisions[1].ThroughputBytesPerSecond, 1, "Judged by the new size's parts alone.");
        }

        [TestMethod]
        public void LevelAfterAProbeWentBackIsMeasuredWithPartsOfTheSizeItWentBackTo()
        {
            var driver = new SizingDriver(slots: 4);
            driver.StartProbe(baseline: 100);
            driver.Complete(epoch: 1, bytes: 200, after: TimeSpan.FromSeconds(1));
            // A failure interrupts the probe: the size goes back, in epoch 2.
            driver.Fail(epoch: 1, after: TimeSpan.FromSeconds(0.1));
            // Larger parts still being sent finish, and quickly.
            for (int i = 0; i < 3; i++)
            {
                driver.Complete(epoch: 1, bytes: 200, after: TimeSpan.FromSeconds(0.1));
            }
            // Parts of the size it went back to deliver 100 bytes a second.
            for (int i = 0; i < 5; i++)
            {
                driver.Complete(epoch: 2, bytes: 100, after: TimeSpan.FromSeconds(1));
            }

            CollectionAssert.AreEqual(new[] { PartSizeDecision.Grew, PartSizeDecision.ProbeRejected, PartSizeDecision.Grew }, driver.Decisions.Select(d => d.Decision).ToArray(), driver.Describe());
            Assert.AreEqual(100, driver.Decisions[2].ThroughputBytesPerSecond, 1, "The level after going back is measured with parts of that size alone.");
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task NextPartSizeIsReportedUntilAStreamOfUnknownLengthEnds()
        {
            using (var server = new UploadServer())
            {
                UseAws(server, 1 * KiB, 64 * KiB, 10000, 1L << 30);
                var progress = new ProgressLog();
                byte[] contents = Pattern(64 * KiB + 5);

                UploadResult result = await Within(Client(server).Transfers.UploadStreamAsync(new SlowStream(contents, TimeSpan.FromMilliseconds(10)), "remote.bin", new UploadOptions
                {
                    ConcurrencyMode = UploadConcurrencyMode.Static,
                    MaxConcurrency = 2,
                    MaxStreamLength = 1 * MiB,
                    AutomaticPartSizing = GrowingFrom(16 * KiB, max: 16 * KiB, unknownStart: 16 * KiB),
                    Progress = progress,
                }));

                TransferProgress[] open = progress.Reports.Where(report => !report.TotalBytesKnown).ToArray();
                Assert.IsTrue(open.Any(report => report.PartsAcknowledged > 0), "Parts were accepted before the stream ended.");
                Assert.IsTrue(open.All(report => report.PartSizing.NextPartSize == 16 * KiB), string.Join(", ", open.Select(report => report.PartSizing.NextPartSize)));
                Assert.IsTrue(result.FinalProgress.TotalBytesKnown);
                Assert.AreEqual(0, result.FinalProgress.PartSizing.NextPartSize, "No part is left once the stream has ended.");
                AssertExactUpload(server, result, contents);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(StreamPartStorage.Memory, null, PartSizeSource.ServerPlanned, DisplayName = "In memory, the bound needs parts too large: Files.com's sizes")]
        [DataRow(StreamPartStorage.Memory, 1000000000L, PartSizeSource.Automatic, DisplayName = "In memory, with a bound declared small enough")]
        [DataRow(StreamPartStorage.TemporaryFiles, null, PartSizeSource.Automatic, DisplayName = "In temporary files, parts can be large enough")]
        public async Task PartsOfAStreamOfUnknownLengthAreChosenOnlyWhereTheirStorageCanHoldThem(StreamPartStorage storage, long? maxStreamLength, PartSizeSource sizes)
        {
            using (var server = new UploadServer())
            {
                // The SDK's own limits for S3: 10,000 parts of at most 1 GiB, for files of up to 5 TiB, which would need
                // parts larger than the limiter's 64 MiB of buffer space.
                server.Aws = true;
                // Files.com's own sizes are within the limits too.
                server.PartSize = number => AwsMinPart;
                byte[] contents = Pattern((int)AwsMinPart + 3);
                string partFiles = Path.Combine(directory, "parts");
                Directory.CreateDirectory(partFiles);

                UploadResult result = await Within(Client(server).Transfers.UploadStreamAsync(new ForwardOnlyStream(contents, 5000), "remote.bin", new UploadOptions
                {
                    Limiter = new TransferLimiter(8, 64 * MiB),
                    StreamPartStorage = storage,
                    TemporaryDirectory = partFiles,
                    MaxStreamLength = maxStreamLength,
                }));

                UploadPartSizing sizing = result.FinalProgress.PartSizing;
                Assert.AreEqual(sizes, sizing.Source);
                if (sizes == PartSizeSource.ServerPlanned)
                {
                    Assert.AreEqual(ServerPlannedReason.MemoryLimit, sizing.ServerPlannedReason);
                    CollectionAssert.AreEqual(new[] { AwsMinPart, 3L }, result.Parts.Select(part => part.Length).ToArray(), "Files.com's sizes, one at a time.");
                    Assert.AreEqual(UploadSerialReason.LengthUnknown, result.FinalProgress.Concurrency.SerialReason);
                }
                else
                {
                    Assert.AreEqual(1, result.Parts.Count, "One part within the AWS minimum, which the last part is exempt from.");
                    Assert.AreEqual(maxStreamLength ?? FilesMaxFile, sizing.MaxStreamLength);
                }
                Assert.AreEqual(0, Directory.GetFiles(partFiles).Length, "Part files are deleted.");
                AssertExactUpload(server, result, contents);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(true, false, DisplayName = "Retried and renewed parts are resent from their files, which go once the upload completes")]
        [DataRow(false, false, DisplayName = "When a part is refused, the upload fails and its part files go too")]
        [DataRow(true, true, DisplayName = "Part files the netstandard2.0 build creates are resent from and go the same way")]
        public async Task StreamPartsInTemporaryFilesAreResentFromThemAndDeletedOnceSettled(bool completes, bool netstandard)
        {
            if (netstandard && OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("The netstandard2.0 build's private creation is for systems other than Windows.");
            }
            using (var server = new UploadServer())
            {
                UseAws(server, 4 * KiB, 64 * KiB, 10000, 1L << 30);
                server.Expires = (number, negotiation) => number == 3 && negotiation == 1 ? DateTime.UtcNow.AddSeconds(5).ToString("o") : null;
                byte[] contents = Pattern(100 * KiB + 17);
                string partFiles = Path.Combine(directory, "parts");
                Directory.CreateDirectory(partFiles);
                // Each part's file, as its request arrives: on a system other than Windows, this user's alone.
                var modes = new List<UnixFileMode>();
                // Part 2 fails once, and then, when the upload is not to complete, for good.
                server.RespondToPart = request =>
                {
                    if (!OperatingSystem.IsWindows())
                    {
                        foreach (string file in Directory.GetFiles(partFiles))
                        {
                            try
                            {
                                UnixFileMode mode = File.GetUnixFileMode(file);
                                lock (modes)
                                {
                                    modes.Add(mode);
                                }
                            }
                            catch (System.IO.FileNotFoundException)
                            {
                                // A settled part's file, deleted as it was looked at.
                            }
                        }
                    }
                    return Task.FromResult(request.Number != 2 ? UploadServer.Accept(request)
                        : request.Attempt == 1 ? LoopbackSite.Status(503)
                        : completes ? UploadServer.Accept(request)
                        : LoopbackSite.Status(403));
                };

                TemporaryPartFile.NetstandardCreationForTests.Value = netstandard;
                Task<UploadResult> upload = Client(server, retries: 1).Transfers.UploadStreamAsync(new ForwardOnlyStream(contents, 5000), "remote.bin", new UploadOptions
                {
                    ConcurrencyMode = UploadConcurrencyMode.Static,
                    MaxConcurrency = 3,
                    StreamPartStorage = StreamPartStorage.TemporaryFiles,
                    TemporaryDirectory = partFiles,
                    MaxStreamLength = 1 * MiB,
                    AutomaticPartSizing = GrowingFrom(16 * KiB, max: 16 * KiB, unknownStart: 16 * KiB),
                });
                TemporaryPartFile.NetstandardCreationForTests.Value = false;

                PartRequest[] part2;
                if (completes)
                {
                    UploadResult result = await Within(upload);

                    Assert.AreEqual(PartSizeSource.Automatic, result.FinalProgress.PartSizing.Source);
                    part2 = server.Parts.Where(part => part.Number == 2).ToArray();
                    Assert.AreEqual(2, result.Parts[1].Attempts);
                    PartRequest part3 = server.Parts.Single(part => part.Number == 3);
                    StringAssert.EndsWith(part3.Request.Path, "&negotiation=2", "Part 3 was sent to its renewed URL.");
                    CollectionAssert.AreEqual(contents.Skip(32 * KiB).Take(16 * KiB).ToArray(), part3.Bytes, "The renewed part is sent from its file.");
                    AssertExactUpload(server, result, contents);
                }
                else
                {
                    await ThrowsAsync<ApiException>(upload);

                    part2 = server.Parts.Where(part => part.Number == 2).ToArray();
                    Assert.AreEqual(0, server.Finishes.Count, "The upload is not completed.");
                }
                Assert.AreEqual(2, part2.Length);
                CollectionAssert.AreEqual(contents.Skip(16 * KiB).Take(16 * KiB).ToArray(), part2[1].Bytes, "The retry resends the part's bytes from its file.");
                Assert.AreEqual(0, Directory.GetFiles(partFiles).Length, "Part files are deleted.");
                if (!OperatingSystem.IsWindows())
                {
                    Assert.IsTrue(modes.Count > 0 && modes.All(mode => mode == (UnixFileMode.UserRead | UnixFileMode.UserWrite)), $"Each part file is this user's alone: {string.Join(", ", modes.Distinct())}");
                }
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(90, false, false, DisplayName = "Within its bound: copied, then uploaded with its size known")]
        [DataRow(101, false, false, DisplayName = "Longer than its bound: nothing sent")]
        [DataRow(90, true, false, DisplayName = "Cancelled while copying: nothing sent")]
        [DataRow(90, false, true, DisplayName = "Copied to a file the netstandard2.0 build creates, then uploaded")]
        [DataRow(90, true, true, DisplayName = "Cancelled while copying to a file the netstandard2.0 build creates: nothing sent")]
        public async Task SpooledStreamIsCopiedBeforeAnythingIsSent(int length, bool cancel, bool netstandard)
        {
            if (netstandard && OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("The netstandard2.0 build's private creation is for systems other than Windows.");
            }
            using (var server = new UploadServer())
            {
                byte[] contents = Pattern(length);
                string spool = Path.Combine(directory, "spool");
                Directory.CreateDirectory(spool);
                var source = cancel ? (Stream)new StalledFirstReadStream() : new ShortReadStream(contents);
                var cancellation = new CancellationTokenSource();
                var progress = new ProgressLog();

                TemporaryPartFile.NetstandardCreationForTests.Value = netstandard;
                Task<UploadResult> upload = Client(server).Transfers.UploadStreamAsync(source, "remote.bin", new UploadOptions { SpoolMaxBytes = 100, TemporaryDirectory = spool, Progress = progress }, cancellation.Token);
                TemporaryPartFile.NetstandardCreationForTests.Value = false;
                if (cancel)
                {
                    await Within(((StalledFirstReadStream)source).FirstReadStarted);
                    Assert.AreEqual(1, Directory.GetFiles(spool).Length, "The spool file exists while the stream is copied.");
                    string spooled = Directory.GetFiles(spool)[0];
                    Assert.AreEqual(0, new FileInfo(spooled).Length, "Nothing of the stream is in it yet.");
                    if (!OperatingSystem.IsWindows())
                    {
                        Assert.AreEqual(UnixFileMode.UserRead | UnixFileMode.UserWrite, File.GetUnixFileMode(spooled), "The spool file is this user's alone before anything is written to it.");
                    }
                    cancellation.Cancel();
                    ((StalledFirstReadStream)source).ReleaseFirstRead();
                    await AssertCancelledPromptly(upload);
                }
                else if (length > 100)
                {
                    Assert.AreEqual(UploadLimit.SpoolLength, (await ThrowsAsync<UploadLimitException>(upload)).Limit);
                }
                else
                {
                    UploadResult result = await Within(upload);
                    Assert.AreEqual(length, server.Negotiations[0].GetProperty("size").GetInt64(), "Uploaded with its size known.");
                    AssertExactUpload(server, result, contents);
                    Assert.AreEqual(length, progress.Reports.Last().SpooledBytes);
                    Assert.IsFalse(progress.Reports.First().TotalBytesKnown, "Its size is not known while it is copied.");
                }
                if (cancel || length > 100)
                {
                    Assert.AreEqual(0, server.Site.Requests.Count, "Nothing reaches Files.com.");
                }
                Assert.AreEqual(0, Directory.GetFiles(spool).Length, "The spool file is deleted.");
                Assert.IsTrue(source.CanRead, "The borrowed stream is left open.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [UnsupportedOSPlatform("windows")]
        public async Task SpoolFileThatCannotBeDeletedIsReportedWithoutItsPathAndTheUploadStillCompletes()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.Inconclusive("Makes a folder read-only with Unix permissions.");
            }
            using (var server = new UploadServer())
            using (var logs = new FilesTests.LogCapture())
            {
                byte[] contents = Pattern(40);
                string spool = Path.Combine(directory, "sentinel-secret-spool");
                Directory.CreateDirectory(spool);
                // Once the spool file exists, its folder no longer lets it be deleted.
                var progress = new ProgressLog(report =>
                {
                    if (report.SpooledBytes == 0)
                    {
                        File.SetUnixFileMode(spool, UnixFileMode.UserRead | UnixFileMode.UserExecute);
                    }
                    return null;
                });
                try
                {
                    UploadResult result = await Within(Client(server).Transfers.UploadStreamAsync(new MemoryStream(contents), "remote.bin", new UploadOptions { SpoolMaxBytes = 100, TemporaryDirectory = spool, Progress = progress }));

                    AssertExactUpload(server, result, contents);
                    CollectionAssert.AreEqual(new[] { "Uploading a stream: deleting one of its temporary files failed (UnauthorizedAccessException); the details are logged at DEBUG." }, logs.Lines("temporary files failed"));
                    Assert.IsFalse(logs.Normal.Contains("sentinel"), logs.Normal);
                    StringAssert.Contains(logs.Debug, "sentinel-secret-spool");
                }
                finally
                {
                    File.SetUnixFileMode(spool, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
                }
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public void PlansForFilesBeyondATerabyteKeepEveryPartWithinTheLimitsWithoutOverflow()
        {
            // The SDK's own limits for S3.
            PartSizeLimits aws = UploadGrant.AwsLimits;
            // Three parts of up to half of Int64 hold the largest file: the parts left times the largest part overflow.
            var extremes = new PartSizeLimits(1, long.MaxValue / 2, 3, long.MaxValue);
            PartSizingSettings automatic = PartSizingSettings.Of(UploadPartSizingMode.Automatic, null, null);
            foreach (long total in new[] { FilesMaxFile, FilesMaxFile - 1, (1L << 40) + 12345 })
            {
                AssertWholePlan(PartSizes.Plan(aws, total, null, null, automatic, null, "remote.bin", 0, out _), total, aws);
            }
            AssertWholePlan(PartSizes.Plan(extremes, long.MaxValue - 7, null, null, automatic, null, "remote.bin", 0, out _), long.MaxValue - 7, extremes);

            // A stream of unknown length: every part leaves room for the rest of the bound, however large.
            PartSizes stream = PartSizes.Plan(aws, null, null, null, automatic, null, "remote.bin", 0, out _);
            long offset = 0;
            for (long number = 1; offset < FilesMaxFile; number++)
            {
                long most = stream.LengthFor(number, offset, out _);
                Assert.IsTrue(most >= 1 && most <= aws.MaxPartBytes, $"Part {number}: {most}");
                Assert.IsTrue(number <= 10000);
                offset += most;
            }
        }

        private static void AssertWholePlan(PartSizes sizes, long total, PartSizeLimits limits)
        {
            long offset = 0;
            long number = 1;
            for (; offset < total; number++)
            {
                long length = sizes.LengthFor(number, offset, out _);
                Assert.IsTrue(length >= 1 && length <= limits.MaxPartBytes, $"Part {number}: {length}");
                Assert.IsTrue(offset + length == total || length >= limits.MinNonfinalBytes, $"Part {number} is too small: {length}");
                offset = checked(offset + length);
            }
            Assert.AreEqual(total, offset);
            Assert.IsTrue(number - 1 <= limits.MaxParts, $"{number - 1} parts");
        }

        // The server's parts go to its Amazon S3 endpoint, and uploads within this test's flow plan within the limits given
        // instead of the SDK's own for S3, so that their parts can be small.
        private static void UseAws(UploadServer server, long minNonfinal, long maxPart, long maxParts, long maxFile)
        {
            server.Aws = true;
            UploadGrant.AwsLimitsForTests.Value = new PartSizeLimits(minNonfinal, maxPart, maxParts, maxFile);
        }

        // Every part was sent to the URL its negotiation gave, exactly as given, as a PUT with a Content-Length of exactly
        // its bytes.
        private static void AssertSentAsNegotiated(UploadServer server)
        {
            foreach (PartRequest part in server.Parts)
            {
                string query = part.Request.Path.Substring(part.Request.Path.IndexOf('?') + 1);
                int negotiation = int.Parse(query.Split('&').Single(pair => pair.StartsWith("negotiation=", StringComparison.Ordinal)).Substring("negotiation=".Length));
                string negotiated = server.NegotiatedUri(part.File, part.Number, negotiation);
                string pathAndQuery = negotiated.Substring(negotiated.IndexOf('/', negotiated.IndexOf("//", StringComparison.Ordinal) + 2));
                Assert.AreEqual(pathAndQuery, part.Request.Path, "Sent exactly as negotiated.");
                Assert.AreEqual(part.Bytes.Length.ToString(System.Globalization.CultureInfo.InvariantCulture), part.Request.Headers["Content-Length"], $"Part {part.Number}'s Content-Length.");
            }
        }

        // Automatic sizing that starts at start bytes and doubles, one judged window at a time, up to max.
        private static AutomaticPartSizingOptions GrowingFrom(long start, long max, long unknownStart = 8 * MiB)
        {
            return new AutomaticPartSizingOptions
            {
                KnownSizeTiers = new List<PartSizeTier> { new PartSizeTier(long.MaxValue, start) },
                WorkloadParts = 1,
                UnknownLengthStartSize = unknownStart,
                MaxPartSize = max,
                GrowthIntervalWindows = 1,
                ProbeWindows = 1,
            };
        }

        // The site received exactly contents, as the upload's parts, and the upload was completed once, with every part
        // in order and the ETag of each part's accepted attempt.
        private static void AssertExactUpload(UploadServer server, UploadResult result, byte[] contents, string file = "remote.bin")
        {
            CollectionAssert.AreEqual(contents, server.AcceptedBytes(file), "The bytes the server accepted.");
            long offset = 0;
            foreach (UploadedPart part in result.Parts)
            {
                Assert.AreEqual(offset, part.Offset, $"Part {part.Number} follows the one before.");
                offset += part.Length;
            }
            Assert.AreEqual(contents.Length, offset);
            JsonElement finish = server.Finishes.Single();
            Assert.AreEqual(contents.Length, finish.GetProperty("size").GetInt64());
            JsonElement[] etags = finish.GetProperty("etags").EnumerateArray().ToArray();
            CollectionAssert.AreEqual(result.Parts.Select(part => part.Number).ToArray(), etags.Select(etag => etag.GetProperty("part").GetInt64()).ToArray());
            foreach (UploadedPart part in result.Parts)
            {
                PartRequest accepted = server.Parts.Single(request => request.Number == part.Number && request.Accepted);
                Assert.AreEqual($"\"etag-{file}-{part.Number}-{accepted.Attempt}\"", EtagOf(finish, part.Number));
                Assert.AreEqual(part.Length, accepted.Bytes.Length);
            }
        }

        private static string EtagOf(JsonElement finish, long number)
        {
            return finish.GetProperty("etags").EnumerateArray().Single(etag => etag.GetProperty("part").GetInt64() == number).GetProperty("etag").GetString();
        }

        private static string Describe(TraceLog trace)
        {
            return string.Join(", ", trace.Sizing.Select(d => $"{d.Elapsed.TotalMilliseconds:0}ms {d.Decision} {d.PreviousSize}->{d.Size} {d.ThroughputBytesPerSecond:0}/{d.BaselineBytesPerSecond:0}"))
                + " | parts: " + string.Join(", ", trace.Attempts.Select(a => $"{a.PartNumber}:{a.PartBytes}"));
        }

        // A forward-only stream whose reads return at most maxRead bytes; onRead sees each read's size before it is read.
        private sealed class MeteredStream : Stream
        {
            private readonly string name;
            private readonly byte[] data;
            private readonly int maxRead;
            private readonly Action<string, int> onRead;
            private int position;
            private bool disposed;

            public MeteredStream(string name, byte[] data, int maxRead, Action<string, int> onRead)
            {
                this.name = name;
                this.data = data;
                this.maxRead = maxRead;
                this.onRead = onRead;
            }

            public int BytesRead => Volatile.Read(ref position);

            public override bool CanRead => !disposed;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException("Only asynchronous reads are expected.");

            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                int length = Math.Min(Math.Min(count, maxRead), data.Length - position);
                onRead(name, count);
                Array.Copy(data, position, buffer, offset, length);
                Volatile.Write(ref position, position + length);
                return Task.FromResult(length);
            }

            public override void Flush()
            {
            }

            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

            protected override void Dispose(bool disposing)
            {
                disposed = true;
                base.Dispose(disposing);
            }
        }

        // Feeds a static upload's controller, which judges its windows for part size alone, parts of given size epochs on
        // a clock it controls, keeping slots requests in flight: each part that ends is replaced by another.
        private sealed class SizingDriver
        {
            private readonly ConcurrencyController controller;
            private readonly List<UploadTraceEvent> records = new List<UploadTraceEvent>();
            private readonly PartSizeController sizes;
            private long now = Stopwatch.Frequency;

            public SizingDriver(int slots)
            {
                var tuning = new AutomaticPartSizingOptions { GrowthIntervalWindows = 1, ProbeGainPercent = 10, ProbeWindows = 1, HoldWindows = 0 };
                sizes = new PartSizeController(tuning.Validated(), 100, 400, "remote.bin", now);
                controller = new ConcurrencyController(ConcurrencySettings.Static(slots), "remote.bin", now, sizes);
                for (int i = 0; i < slots; i++)
                {
                    controller.Granted(now);
                    controller.Sending(now);
                }
            }

            public IReadOnlyList<UploadSizingTrace> Decisions => records.OfType<UploadSizingTrace>().ToList();

            // The size is due to grow from a level of baseline bytes a second: parts planned from now on are larger.
            public void StartProbe(double baseline)
            {
                sizes.WindowJudged();
                Assert.IsTrue(sizes.TryGrow(now, baseline, baseline, records));
            }

            public void Complete(int epoch, long bytes, TimeSpan after)
            {
                End(new AttemptSample(accepted: true, throttled: false, transient: false, retryAfter: null, bytes: bytes, duration: TimeSpan.FromMilliseconds(10), sizeEpoch: epoch), after);
            }

            public void Fail(int epoch, TimeSpan after)
            {
                End(new AttemptSample(accepted: false, throttled: false, transient: true, retryAfter: null, bytes: 0, duration: TimeSpan.FromMilliseconds(10), sizeEpoch: epoch), after);
            }

            public string Describe()
            {
                return string.Join(", ", Decisions.Select(d => $"{d.Decision} {d.PreviousSize}->{d.Size} {d.ThroughputBytesPerSecond:0}/{d.BaselineBytesPerSecond:0}"));
            }

            private void End(AttemptSample sample, TimeSpan after)
            {
                now += (long)(after.TotalSeconds * Stopwatch.Frequency);
                controller.Record(now, 0, sample, records);
                controller.Released(now, sending: true);
                controller.Granted(now);
                controller.Sending(now);
            }
        }

        // Sends the parts of a static upload's file of total bytes, as its plan sizes them, to the upload's real controller
        // on a clock it controls, slots at a time, each request taking duration whatever its size: as each part is
        // answered, the next is planned (PartSizes.LengthFor), in the size chosen then, and sent in its place. Returns the
        // parts' lengths, in order, and the size decisions.
        private static (List<long> Lengths, List<UploadSizingTrace> Sizing) SendPlanned(PartSizes plan, long total, int slots, TimeSpan duration)
        {
            long now = Stopwatch.Frequency;
            var records = new List<UploadTraceEvent>();
            var controller = new ConcurrencyController(ConcurrencySettings.Static(slots), "remote.bin", now, plan.Controller);
            var lengths = new List<long>();
            var sending = new List<(long Length, int Epoch)>();
            long offset = 0;
            void SendNext()
            {
                if (offset < total)
                {
                    long length = plan.LengthFor(lengths.Count + 1, offset, out int epoch);
                    lengths.Add(length);
                    offset += length;
                    sending.Add((length, epoch));
                    controller.Granted(now);
                    controller.Sending(now);
                }
            }
            for (int i = 0; i < slots; i++)
            {
                SendNext();
            }
            while (sending.Count > 0)
            {
                now += (long)(duration.TotalSeconds * Stopwatch.Frequency);
                var answered = sending.ToList();
                sending.Clear();
                foreach ((long length, int epoch) in answered)
                {
                    controller.Record(now, 0, new AttemptSample(accepted: true, throttled: false, transient: false, retryAfter: null, bytes: length, duration: duration, sizeEpoch: epoch), records);
                    controller.Released(now, sending: true);
                    SendNext();
                }
            }
            return (lengths, records.OfType<UploadSizingTrace>().ToList());
        }

        // A forward-only stream whose reads return at most maxRead bytes.
        private sealed class ForwardOnlyStream : Stream
        {
            private readonly byte[] data;
            private readonly int maxRead;
            private int position;
            private bool disposed;

            public ForwardOnlyStream(byte[] data, int maxRead)
            {
                this.data = data;
                this.maxRead = maxRead;
            }

            public override bool CanRead => !disposed;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException("Only asynchronous reads are expected.");

            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                int length = Math.Min(Math.Min(count, maxRead), data.Length - position);
                Array.Copy(data, position, buffer, offset, length);
                position += length;
                return Task.FromResult(length);
            }

            public override void Flush()
            {
            }

            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

            protected override void Dispose(bool disposing)
            {
                disposed = true;
                base.Dispose(disposing);
            }
        }
    }
}