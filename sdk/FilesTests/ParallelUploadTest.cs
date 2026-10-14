using FilesCom;
using FilesCom.Transfers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WireMock;

namespace FilesTests
{
    // Uploads through client.Transfers against a loopback site that negotiates parts like Files.com. The tests
    // observe what arrives: which bytes, as which part, how many at once, how often and with what credentials.
    [TestClass]
    public partial class ParallelUploadTest
    {
        private static readonly TimeSpan Promptly = TimeSpan.FromSeconds(10);
        private string directory;

        // On Windows the test's folder is first made private, so that no other account can change the checkpoints kept in
        // it, or in a folder created in it, which inherits its list, whatever this user's temporary directory allows.
        [TestInitialize]
        public void CreateDirectory()
        {
            directory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(directory);
            if (OperatingSystem.IsWindows())
            {
                WindowsTestAccess.MakePrivate(directory);
            }
        }

        [TestCleanup]
        public void DeleteDirectory()
        {
            Directory.Delete(directory, true);
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task PartsSentTogetherKeepTheirOwnBytesAndETagsWhateverOrderTheyFinishIn()
        {
            using (var server = new UploadServer())
            {
                // Sizes the server plans, growing like the service's: 5, 6, 6, then 8.
                server.PartSize = number => number == 1 ? 5 : number <= 3 ? 6 : 8;
                server.ExtraPartFields = ",\"headers\":{\"Content-Type\":\"application/x-part\",\"X-Part-Hint\":\"kept\"}";
                // Every part waits until all four are in flight at once, then they are answered last to first.
                var answered = Enumerable.Range(1, 5).Select(_ => new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously)).ToArray();
                answered[4].SetResult(true);
                server.RespondToPart = async request =>
                {
                    await server.WaitUntil(s => s.MaxPartsInFlight == 4, "four parts are in flight together");
                    await answered[request.Number].Task;
                    answered[request.Number - 1].SetResult(true);
                    return UploadServer.Accept(request);
                };
                byte[] contents = Pattern(25);
                string localPath = WriteFile("local.bin", contents);

                UploadResult result = await Within(Client(server).Transfers.UploadFileAsync(localPath, "remote.bin"));

                Assert.AreEqual(4, server.MaxPartsInFlight);
                CollectionAssert.AreEqual(new[] { "5@0", "6@5", "6@11", "8@17" }, result.Parts.Select(part => $"{part.Length}@{part.Offset}").ToArray());
                foreach (PartRequest part in server.Parts)
                {
                    UploadedPart planned = result.Parts[(int)part.Number - 1];
                    CollectionAssert.AreEqual(contents.Skip((int)planned.Offset).Take((int)planned.Length).ToArray(), part.Bytes, $"Bytes of part {part.Number}");
                    Assert.AreEqual($"/part/remote.bin/{part.Number}?{UploadServer.SignedQuery}&negotiation=1", part.Request.Path, "The signed URL is sent exactly as negotiated.");
                    Assert.AreEqual("application/x-part", part.Request.Headers["Content-Type"]);
                    Assert.AreEqual("kept", part.Request.Headers["X-Part-Hint"]);
                    Assert.AreEqual(planned.Length.ToString(), part.Request.Headers["Content-Length"]);
                    AssertNoFilesCredentials(part.Request);
                }

                JsonElement finish = server.Finishes.Single();
                Assert.AreEqual("end", finish.GetProperty("action").GetString());
                Assert.AreEqual("ref-remote.bin", finish.GetProperty("ref").GetString());
                Assert.AreEqual(25, finish.GetProperty("size").GetInt64());
                Assert.AreEqual(
                    "1=\"etag-remote.bin-1-1\" 2=\"etag-remote.bin-2-1\" 3=\"etag-remote.bin-3-1\" 4=\"etag-remote.bin-4-1\"",
                    string.Join(" ", finish.GetProperty("etags").EnumerateArray().Select(etag => $"{etag.GetProperty("part").GetInt64()}={etag.GetProperty("etag").GetString()}")),
                    "Each part is completed with the ETag of its own response, although part 4 was answered first.");
                Assert.AreEqual(File.GetLastWriteTimeUtc(localPath).ToString("u"), finish.GetProperty("provided_mtime").GetString());

                JsonElement[] negotiations = server.Negotiations.ToArray();
                Assert.AreEqual(25, negotiations[0].GetProperty("size").GetInt64(), "The first negotiation gives the upload's size.");
                Assert.IsFalse(negotiations[0].TryGetProperty("ref", out _));
                Assert.IsTrue(negotiations.Skip(1).All(n => n.GetProperty("ref").GetString() == "ref-remote.bin"));
                Assert.IsTrue(server.Site.Requests.Where(r => !r.Path.StartsWith("/part/")).All(r => r.ApiKey == "key-a" && r.WorkspaceId == "11"));
                Assert.IsTrue(result.ParallelParts);
                Assert.AreEqual(25, result.Size);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("false", false)]
        [DataRow(null, false)]
        [DataRow("false", true, DisplayName = "Even when tuned to start with eight parts at once")]
        public async Task PartsTheServerDoesNotAllowInParallelAreSentOneAtATimeInOrder(string parallelParts, bool tunedForS3)
        {
            using (var server = new UploadServer { ParallelParts = parallelParts })
            {
                server.RespondToPart = async request =>
                {
                    await Task.Delay(150); // Long enough for a second part to arrive, if one were sent.
                    return UploadServer.Accept(request);
                };
                byte[] contents = Pattern(20);
                // Forward-only, and never more than three bytes per read.
                var source = new ShortReadStream(contents);

                var options = tunedForS3 ? new UploadOptions { AutomaticConcurrency = AutomaticConcurrencyOptions.For(UploadPerformanceProfile.S3) } : new UploadOptions { MaxConcurrency = 4 };

                UploadResult result = await Within(Client(server).Transfers.UploadStreamAsync(source, 20, "remote.bin", options));

                Assert.AreEqual(1, server.MaxPartsInFlight);
                Assert.AreEqual(UploadSerialReason.ServerRequiresSerialParts, result.FinalProgress.Concurrency.SerialReason);
                Assert.AreEqual(1, result.FinalProgress.ConcurrencyLimit);
                CollectionAssert.AreEqual(new long[] { 1, 2, 3 }, server.Parts.Select(part => part.Number).ToArray());
                CollectionAssert.AreEqual(contents, server.AcceptedBytes("remote.bin"));
                Assert.IsTrue(server.Negotiations.All(n => !n.TryGetProperty("parts", out _)), "Serial parts are negotiated one at a time.");
                Assert.IsFalse(result.ParallelParts);
                Assert.IsTrue(source.CanRead, "The upload leaves the caller's stream open.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(503, "{\"error\":\"failed\"}", DisplayName = "A 503")]
        [DataRow(400, LoopbackSite.S3RequestTimeout, DisplayName = "Amazon S3's 400 saying the request timed out")]
        public async Task FailedPartIsResentWithTheSameBytesAndAcceptedPartsAreNotSentAgain(int status, string body)
        {
            using (var server = new UploadServer())
            {
                server.RespondToPart = request => Task.FromResult(request.Number == 2 && request.Attempt == 1 ? LoopbackSite.StorageError(status, body) : UploadServer.Accept(request));
                byte[] contents = Pattern(20);
                var progress = new ProgressLog();

                UploadResult result = await Within(Client(server, retries: 2).Transfers.UploadFileAsync(WriteFile("local.bin", contents), "remote.bin", new UploadOptions { Progress = progress }));

                CollectionAssert.AreEqual(new[] { "1:1", "2:2", "3:1" }, server.Parts.GroupBy(part => part.Number).OrderBy(g => g.Key).Select(g => $"{g.Key}:{g.Count()}").ToArray(), "part:times sent");
                PartRequest[] part2 = server.Parts.Where(part => part.Number == 2).ToArray();
                CollectionAssert.AreEqual(contents.Skip(8).Take(8).ToArray(), part2[0].Bytes);
                CollectionAssert.AreEqual(part2[0].Bytes, part2[1].Bytes, "The retry resends the same bytes.");
                Assert.AreEqual(part2[0].Request.Path, part2[1].Request.Path, "The retry goes to the same part.");
                CollectionAssert.AreEqual(contents, server.AcceptedBytes("remote.bin"));
                Assert.AreEqual("\"etag-remote.bin-2-2\"", server.Finishes.Single().GetProperty("etags")[1].GetProperty("etag").GetString());
                Assert.AreEqual(2, result.Parts[1].Attempts);

                TransferProgress last = progress.Reports.Last();
                Assert.AreEqual(20, last.AcknowledgedBytes);
                Assert.AreEqual(28, last.SentBytes, "Sent bytes count the resent part again.");
                Assert.AreEqual(1, last.PartRetries);
                CollectionAssert.AreEqual(progress.Reports.Select(r => r.AcknowledgedBytes).OrderBy(b => b).ToArray(), progress.Reports.Select(r => r.AcknowledgedBytes).ToArray(), "Progress never goes backward.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(503, "{\"error\":\"failed\"}", DisplayName = "A 503")]
        [DataRow(400, LoopbackSite.S3RequestTimeout, DisplayName = "Amazon S3's 400 saying the request timed out")]
        public async Task PartTheServerDoesNotAllowRetryingFailsTheUploadWithoutBeingSentAgain(int status, string body)
        {
            using (var server = new UploadServer { RetryParts = "false" })
            {
                server.RespondToPart = request => Task.FromResult(request.Number == 2 ? LoopbackSite.StorageError(status, body) : UploadServer.Accept(request));

                SdkException error = await ThrowsAsync<SdkException>(Client(server, retries: 3).Transfers.UploadFileAsync(WriteFile("local.bin", Pattern(20)), "remote.bin"));

                StringAssert.Contains(error.Message, $"HTTP {status}");
                Assert.IsFalse(error.Message.Contains("Signature"), "The error does not reveal the signed URL.");
                Assert.AreEqual(1, server.Parts.Count(part => part.Number == 2));
                Assert.AreEqual(0, server.Finishes.Count);
            }
        }

        // Of the answers with HTTP 400, only storage's whole XML error saying the request timed out sends the part again,
        // as many times as the client's retries allow. Another code, a Files.com error object, XML cut short, an error
        // longer than a storage error is read to, or the same error with another status fails the upload at once.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(400, LoopbackSite.S3RequestTimeout, true, DisplayName = "RequestTimeout every time: sent until the retries run out")]
        [DataRow(400, "<Error><Code>InvalidArgument</Code><Message>Invalid Argument</Message></Error>", false, DisplayName = "Another code")]
        [DataRow(400, "{\"type\":\"bad-request\",\"error\":\"RequestTimeout\"}", false, DisplayName = "A Files.com error object")]
        [DataRow(400, "<Error><Code>RequestTimeout</Code><Message>Your socket connection", false, DisplayName = "XML cut short")]
        [DataRow(400, null, false, DisplayName = "RequestTimeout longer than a storage error is read to")]
        [DataRow(403, LoopbackSite.S3RequestTimeout, false, DisplayName = "RequestTimeout with HTTP 403")]
        public async Task APartIsSentAgainAfterA400OnlyWhenStorageSaysTheRequestTimedOut(int status, string body, bool timedOut)
        {
            // Whitespace after the error: what is read of it, to the bound, is whole XML too.
            body = body ?? LoopbackSite.S3RequestTimeout + new string(' ', FilesApiService.StorageAnswerBytes);
            using (var server = new UploadServer())
            {
                server.RespondToPart = request => Task.FromResult(request.Number == 2 ? LoopbackSite.StorageError(status, body) : UploadServer.Accept(request));

                SdkException error = await ThrowsAsync<SdkException>(Client(server, retries: 2).Transfers.UploadFileAsync(WriteFile("local.bin", Pattern(20)), "remote.bin"));

                Assert.AreEqual(timedOut ? $"Transfer request failed (HTTP {status}): the storage server says the request timed out" : $"Transfer request failed (HTTP {status})", error.Message);
                Assert.AreEqual(timedOut ? 3 : 1, server.Parts.Count(part => part.Number == 2), "Times part 2 was sent");
                Assert.AreEqual(0, server.Finishes.Count);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task UploadsSharingALimiterStayWithinItAndTakeTurns()
        {
            using (var server = new UploadServer())
            {
                var held = new ConcurrentQueue<(PartRequest request, TaskCompletionSource<ResponseMessage> response)>();
                server.RespondToPart = request =>
                {
                    var response = new TaskCompletionSource<ResponseMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
                    held.Enqueue((request, response));
                    return response.Task;
                };
                // Each upload may send four parts at once, but the limiter allows two in all.
                var limiter = new TransferLimiter(maxConcurrentRequests: 2);
                var options = new UploadOptions { Limiter = limiter, MaxConcurrency = 4 };
                FilesClient client = Client(server);
                byte[] a = Pattern(48);
                byte[] b = Pattern(48, seed: 5);

                Task<UploadResult> uploadA = client.Transfers.UploadFileAsync(WriteFile("a.bin", a), "a.bin", options);
                await server.WaitUntil(s => s.Parts.Count == 2 && limiter.QueuedRequests == 1, "a.bin has two parts in flight and a third waiting");
                Task<UploadResult> uploadB = client.Transfers.UploadFileAsync(WriteFile("b.bin", b), "b.bin", options);
                await server.WaitUntil(s => limiter.QueuedRequests == 2, "b.bin waits for a slot too");

                // Answer the oldest part; then wait for the part given its slot to arrive, and for every upload with
                // parts left to be waiting again.
                while (server.Parts.Count < 12)
                {
                    Assert.IsTrue(held.TryDequeue(out var oldest));
                    int next = server.Parts.Count + 1;
                    oldest.response.SetResult(UploadServer.Accept(oldest.request));
                    await server.WaitUntil(s => s.Parts.Count == next && limiter.QueuedRequests == new[] { "a.bin", "b.bin" }.Count(file => s.Parts.Count(p => p.File == file) < 6), $"part {next} arrives");
                    Assert.IsTrue(limiter.ActiveRequests <= 2);
                }
                while (held.TryDequeue(out var remaining))
                {
                    remaining.response.SetResult(UploadServer.Accept(remaining.request));
                }
                await Within(Task.WhenAll(uploadA, uploadB));

                Assert.AreEqual("a a a b a b a b a b b b", string.Join(" ", server.Parts.Select(part => part.File[0])), "Once both wait, they take turns.");
                Assert.AreEqual(2, server.MaxPartsInFlight);
                CollectionAssert.AreEqual(a, server.AcceptedBytes("a.bin"));
                CollectionAssert.AreEqual(b, server.AcceptedBytes("b.bin"));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task UploadsOfDifferentClientsSharingALimiterKeepTheirOwnSitesAndCredentials()
        {
            using (var siteA = new UploadServer("key-a", "11"))
            using (var siteB = new UploadServer("key-b", "22"))
            using (var siteC = new UploadServer("key-c", "44"))
            {
                // The third client's stream has no length, and its site's parts go straight to S3, where it chooses their sizes.
                UseAws(siteC, 16, 64, 10000, 4096);
                var limiter = new TransferLimiter(maxConcurrentRequests: 3);
                byte[] a = Pattern(40);
                byte[] b = Pattern(40, seed: 9);
                byte[] c = Pattern(100, seed: 3);

                await Within(Task.WhenAll(
                    Client(siteA).Transfers.UploadFileAsync(WriteFile("a.bin", a), "a.bin", new UploadOptions { Limiter = limiter }),
                    Client(siteB).Transfers.UploadStreamAsync(new MemoryStream(b), b.Length, "b.bin", new UploadOptions { Limiter = limiter, RequestOptions = new Dictionary<string, object> { { "workspace_id", "33" } } }),
                    Client(siteC).Transfers.UploadStreamAsync(new ForwardOnlyStream(c, 7), "c.bin", new UploadOptions { Limiter = limiter, AutomaticPartSizing = GrowingFrom(16, max: 16, unknownStart: 16) })));

                CollectionAssert.AreEqual(a, siteA.AcceptedBytes("a.bin"));
                CollectionAssert.AreEqual(b, siteB.AcceptedBytes("b.bin"));
                CollectionAssert.AreEqual(c, siteC.AcceptedBytes("c.bin"));
                Assert.AreEqual(100, siteC.Finishes.Single().GetProperty("size").GetInt64());
                Assert.IsTrue(siteA.Parts.All(part => part.File == "a.bin") && siteB.Parts.All(part => part.File == "b.bin") && siteC.Parts.All(part => part.File == "c.bin"));
                AssertApiRequests(siteA, "key-a", "11");
                AssertApiRequests(siteB, "key-b", "33");
                AssertApiRequests(siteC, "key-c", "44");
                foreach (PartRequest part in siteA.Parts.Concat(siteB.Parts).Concat(siteC.Parts))
                {
                    AssertNoFilesCredentials(part.Request);
                }
                Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(1000, 2, 1024, 4, 0, DisplayName = "At most MaxReadyParts parts are prepared ahead")]
        [DataRow(1000, 1, 1024, 3, 0, DisplayName = "One part ahead")]
        [DataRow(1000, 4, 12, 3, 0, DisplayName = "Parts ahead stay within MaxReadyBytes")]
        [DataRow(1000, 4, 4, 3, 0, DisplayName = "A part larger than MaxReadyBytes is prepared once no other part waits")]
        [DataRow(24, 2, 1024, 3, 1, DisplayName = "Prepared parts stay within the buffer budget")]
        public async Task PreparedPartsAreLimitedByCountAndBufferedBytes(int maxBufferedBytes, int maxReadyParts, int maxReadyBytes, int partsRead, int partsWaitingForBuffer)
        {
            using (var server = new UploadServer { HoldParts = true })
            {
                byte[] contents = Pattern(80);
                var source = new ShortReadStream(contents);
                var limiter = new TransferLimiter(maxConcurrentRequests: 2, maxBufferedBytes: maxBufferedBytes);

                // Automatic, so its target could grow; parts read ahead are limited all the same.
                Task<UploadResult> upload = Client(server).Transfers.UploadStreamAsync(source, contents.Length, "remote.bin", new UploadOptions { Limiter = limiter, MaxConcurrency = 2, MaxReadyParts = maxReadyParts, MaxReadyBytes = maxReadyBytes });
                await server.WaitUntil(s => s.PartsInFlight == 2 && source.BytesRead >= partsRead * 8, $"{partsRead} parts have been read");
                await Task.Delay(300); // Time to read further, if it would.

                Assert.AreEqual(partsRead * 8, source.BytesRead, "Bytes read from the stream");
                Assert.AreEqual(partsRead * 8, limiter.BufferedBytes, "Every part read is held, and charged, until it is sent");
                Assert.AreEqual(partsWaitingForBuffer, limiter.QueuedBufferReservations);
                Assert.AreEqual(2, limiter.ActiveRequests);

                server.ReleaseHeldParts();
                await Within(upload);
                CollectionAssert.AreEqual(contents, server.AcceptedBytes("remote.bin"));
                Assert.AreEqual(0, limiter.BufferedBytes);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task StreamThatEndsEarlyFailsTheUploadWithoutPaddingOrCompletingIt()
        {
            using (var server = new UploadServer())
            {
                var limiter = new TransferLimiter();

                EndOfStreamException error = await ThrowsAsync<EndOfStreamException>(Client(server).Transfers.UploadStreamAsync(new ShortReadStream(Pattern(13)), 20, "remote.bin", new UploadOptions { Limiter = limiter }));

                StringAssert.Contains(error.Message, "13 of the 20 bytes");
                // Part 1 may or may not have been sent before the failure stopped the upload; the incomplete part 2 never is.
                Assert.IsTrue(server.Parts.All(part => part.Number == 1 && part.Bytes.SequenceEqual(Pattern(8))), string.Join(", ", server.Events));
                Assert.AreEqual(0, server.Finishes.Count);
                Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task ACallbackOnTheUploadsTokenThatThrowsLeavesTheUploadsOwnFailure()
        {
            using (var server = new UploadServer())
            using (var logs = new global::FilesTests.LogCapture())
            {
                var limiter = new TransferLimiter();
                var failure = new IOException("The caller's stream failed.");
                var source = new CallbackRegisteringStream(new ShortReadStream(Pattern(20), failAfter: 9, failure));

                Exception error = await ThrowsAsync<Exception>(Client(server).Transfers.UploadStreamAsync(source, 20, "remote.bin", new UploadOptions { Limiter = limiter }));

                Assert.IsTrue(source.Registered, "The stream registered its callback on the token it was given.");
                Assert.AreSame(failure, error, "The stream's own failure, not its callback's: " + error);
                StringAssert.Contains(logs.Normal, "A callback registered on the token of stopped work failed");
                StringAssert.Contains(logs.Normal, "InvalidOperationException");
                Assert.AreEqual(0, server.Finishes.Count);
                Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task FileThatShrinksDuringTheUploadFailsItWithoutRetrying()
        {
            using (var server = new UploadServer { HoldParts = true })
            {
                string localPath = WriteFile("local.bin", Pattern(32));
                var limiter = new TransferLimiter(maxConcurrentRequests: 2);
                var trace = new TraceLog();

                Task<UploadResult> upload = Client(server, retries: 3).Transfers.UploadFileAsync(localPath, "remote.bin", new UploadOptions { Limiter = limiter, Trace = trace });
                await server.WaitUntil(s => s.PartsInFlight == 2, "parts 1 and 2 have been sent");
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // The upload's handle keeps other programs from changing the file.
                    Assert.ThrowsExactly<IOException>(() => new FileStream(localPath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite).Dispose());
                    server.ReleaseHeldParts();
                    await Within(upload);
                    return;
                }
                using (var writer = new FileStream(localPath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                {
                    writer.SetLength(10);
                }
                server.ReleaseHeldParts();

                // Reading the file failed, not the network, so the part is not retried.
                EndOfStreamException error = await ThrowsAsync<EndOfStreamException>(upload);
                StringAssert.Matches(error.Message, new System.Text.RegularExpressions.Regex("part [34],"), "Parts 3 and 4 start past the new end of the file.");
                Assert.IsTrue(server.Parts.All(part => part.Number <= 2));
                Assert.IsTrue(trace.Attempts.All(attempt => attempt.PartNumber <= 2 && attempt.Accepted), "The failed read is not measured as a network attempt.");
                Assert.AreEqual(0, server.Finishes.Count);
                Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes);
            }
        }

        // A file a program appends to, or rewrites, while it is uploaded is not completed: every part was accepted, but
        // what they hold is not the file now, so the final request is not sent and the failure says why.
        [TestMethod]
        [Timeout(60000)]
        public async Task FileThatGrowsDuringTheUploadIsNotCompleted()
        {
            using (var server = new UploadServer { HoldParts = true })
            {
                string localPath = WriteFile("local.bin", Pattern(32));

                Task<UploadResult> upload = Client(server, retries: 3).Transfers.UploadFileAsync(localPath, "remote.bin", new UploadOptions { Limiter = new TransferLimiter(maxConcurrentRequests: 2) });
                await server.WaitUntil(s => s.PartsInFlight == 2, "parts 1 and 2 have been sent");
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // The upload's handle keeps other programs from changing the file, so there is nothing to detect.
                    server.ReleaseHeldParts();
                    await Within(upload);
                    return;
                }
                using (var writer = new FileStream(localPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    writer.Write(Pattern(8, seed: 2), 0, 8);
                }
                server.ReleaseHeldParts();

                IOException error = await ThrowsAsync<IOException>(upload);
                StringAssert.Contains(error.Message, "changed while it was being uploaded");
                StringAssert.Contains(error.Message, "was 32 bytes");
                StringAssert.Contains(error.Message, "is 40 bytes");
                Assert.AreEqual(0, server.Finishes.Count, "The final request is not sent for a file that changed.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task CancellingStopsTheUploadEverywhereAndReleasesEverythingBeforeItEnds()
        {
            using (var server = new UploadServer { HoldParts = true })
            {
                // Parts of a file hold an 8-byte copy buffer each: two in flight, one waiting for a request slot and
                // one waiting for buffer space when the upload is cancelled.
                var limiter = new TransferLimiter(maxConcurrentRequests: 2, maxBufferedBytes: 24);
                string localPath = WriteFile("local.bin", Pattern(64));
                var cancellation = new CancellationTokenSource();

                Task<UploadResult> upload = Client(server).Transfers.UploadFileAsync(localPath, "remote.bin", new UploadOptions { Limiter = limiter }, cancellation.Token);
                await server.WaitUntil(s => s.PartsInFlight == 2 && limiter.QueuedRequests == 1 && limiter.QueuedBufferReservations == 1, "every stage is waiting");
                cancellation.Cancel();

                await AssertCancelledPromptly(upload);
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
        public async Task ProgressHandlerThatThrowsStopsTheUpload()
        {
            using (var server = new UploadServer())
            using (var cancellation = new CancellationTokenSource(CallerBound))
            {
                // Part 1 and part 2 are in flight and part 3 waits for one of the limiter's two request slots when part
                // 1 is accepted and the handler throws. The site holds every other answer until the test ends.
                var releasePart1 = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                var releaseOthers = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                server.RespondToPart = async request =>
                {
                    await (request.Number == 1 ? releasePart1.Task : releaseOthers.Task);
                    return UploadServer.Accept(request);
                };
                var handlerError = new InvalidOperationException("The progress handler failed.");
                var progress = new ProgressLog(report => report.AcknowledgedBytes > 0 ? handlerError : null);
                var limiter = new TransferLimiter(maxConcurrentRequests: 2);
                var trace = new TraceLog();
                Task<UploadResult> upload = null;
                Func<string> state = () => Observe(upload, server, limiter, trace);
                bool passed = false;
                try
                {
                    upload = Client(server).Transfers.UploadFileAsync(WriteFile("local.bin", Pattern(24)), "remote.bin", new UploadOptions { Limiter = limiter, Progress = progress, Trace = trace }, cancellation.Token);
                    await server.WaitUntil(s => s.PartsInFlight == 2 && limiter.QueuedRequests == 1, "parts 1 and 2 are in flight and part 3 waits for a request slot", state);
                    releasePart1.SetResult(true);

                    Exception error = await ThrowsAsync<InvalidOperationException>(upload, state);

                    Assert.AreSame(handlerError, error);
                    Assert.AreEqual(0, progress.Reports[0].AcknowledgedBytes);
                    Assert.AreEqual(24, progress.Reports[0].TotalBytes);
                    Assert.IsFalse(releaseOthers.Task.IsCompleted, "The site still held the other answers when the upload ended.");
                    Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes + limiter.QueuedRequests + limiter.QueuedBufferReservations, "The parts in flight and waiting were stopped and released while the site still held their answers: " + state());
                    Assert.AreEqual(0, server.Finishes.Count);
                    int requests = server.Site.Requests.Count;
                    await Task.Delay(150);
                    Assert.AreEqual(requests, server.Site.Requests.Count, "Nothing is sent after the upload ends: " + state());
                    passed = true;
                }
                finally
                {
                    await SettleAsync(passed, upload, cancellation, server, limiter, state, releasePart1, releaseOthers);
                }
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task PartsAcceptedWithoutETagsCompleteWithoutThemButAMixtureIsRefused()
        {
            using (var serial = new UploadServer { ParallelParts = "false" })
            {
                serial.RespondToPart = request => Task.FromResult(UploadServer.AcceptWithoutETag());

                UploadResult result = await Within(Client(serial).Transfers.UploadFileAsync(WriteFile("serial.bin", Pattern(20)), "remote.bin"));

                Assert.IsFalse(serial.Finishes.Single().TryGetProperty("etags", out _), "No ETags are invented.");
                Assert.IsTrue(result.Parts.All(part => part.ETag == null));
            }
            using (var mixed = new UploadServer())
            {
                mixed.RespondToPart = request => Task.FromResult(request.Number == 2 ? UploadServer.AcceptWithoutETag() : UploadServer.Accept(request));

                InvalidResponseException error = await ThrowsAsync<InvalidResponseException>(Client(mixed).Transfers.UploadFileAsync(WriteFile("mixed.bin", Pattern(20)), "remote.bin"));

                StringAssert.Contains(error.Message, "part 2");
                Assert.AreEqual(0, mixed.Finishes.Count);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task PartWhoseUrlIsAboutToExpireGetsANewOneAndKeepsItsBytes()
        {
            using (var server = new UploadServer())
            {
                server.Expires = (number, negotiation) => (number == 2 && negotiation == 1 ? DateTime.UtcNow.AddSeconds(5) : DateTime.UtcNow.AddHours(1)).ToString("o");
                byte[] contents = Pattern(20);

                await Within(Client(server).Transfers.UploadFileAsync(WriteFile("local.bin", contents), "remote.bin"));

                PartRequest part2 = server.Parts.Single(part => part.Number == 2);
                StringAssert.EndsWith(part2.Request.Path, "&negotiation=2", "Part 2 goes to its renewed URL, never to the expiring one.");
                CollectionAssert.AreEqual(contents.Skip(8).Take(8).ToArray(), part2.Bytes);
                Assert.AreEqual(1, server.Events.Count(e => e == "begin remote.bin part=2 parts=1"), "Part 2 alone is negotiated again.");
                CollectionAssert.AreEqual(contents, server.AcceptedBytes("remote.bin"));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("unreadable", typeof(InvalidResponseException), DisplayName = "The answer cannot be read")]
        [DataRow("503", typeof(ApiException), DisplayName = "Server error")]
        [DataRow("rate limit", typeof(TooManyRequestsException), DisplayName = "Files.com's rate limit, which can come after it began")]
        [DataRow("not uploaded", typeof(FileNotUploadedException), DisplayName = "A processing error, which can come after storage completed the file")]
        public async Task FinalRequestIsSentOnceAndAFailureThatMayHaveCompletedTheUploadSaysSo(string failure, Type cause)
        {
            using (var server = new UploadServer())
            {
                server.RespondToFinish = body => Task.FromResult(
                    failure == "unreadable" ? new ResponseMessage { FaultType = WireMock.ResponseBuilders.FaultType.EMPTY_RESPONSE }
                    : failure == "rate limit" ? LoopbackSite.Json("{\"error\":\"Rate limited.\",\"type\":\"rate-limited/too-many-requests\",\"http-code\":429}", status: 429)
                    : failure == "not uploaded" ? LoopbackSite.Json("{\"error\":\"File not uploaded.\",\"type\":\"processing-failure/file-not-uploaded\",\"http-code\":422}", status: 422)
                    : LoopbackSite.Status(503));

                UploadCompletionUnknownException error = await ThrowsAsync<UploadCompletionUnknownException>(Client(server, retries: 3).Transfers.UploadFileAsync(WriteFile("local.bin", Pattern(20)), "remote.bin"));

                Assert.AreEqual(1, server.Finishes.Count, "The final request is not repeated, although the client retries other requests.");
                Assert.IsInstanceOfType(error.InnerException, cause);
                // An error says the request may have taken effect; an answer that it succeeded, which cannot be read, took effect.
                Assert.AreEqual(failure != "unreadable", ((SdkException)error.InnerException).RequestMayHaveBeenProcessed);
                if (failure == "unreadable")
                {
                    Assert.AreEqual(0, ((InvalidResponseException)error.InnerException).httpStatus);
                }
                Assert.AreEqual("remote.bin", error.Path);
                Assert.AreEqual(3, error.Parts.Count);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task FinalRequestWhoseConnectionClosesWithoutAnAnswerIsNotRepeated()
        {
            using (var storage = new UploadServer())
            using (var api = new SilentFinishApi(storage.PartUrl("remote.bin", 1, 1)))
            {
                FilesConfiguration configuration = storage.Site.Configuration(maxNetworkRetries: 3);
                configuration.BaseUrl = api.Url;

                UploadCompletionUnknownException error = await ThrowsAsync<UploadCompletionUnknownException>(FilesClient.Create(configuration).Transfers.UploadFileAsync(WriteFile("local.bin", Pattern(8)), "remote.bin"));

                Assert.IsInstanceOfType(error.InnerException, typeof(ApiConnectionException));
                Assert.AreEqual(1, storage.Parts.Count);
                Assert.AreEqual("begin, end", string.Join(", ", api.Requests), "The final request was received once and not sent again.");
            }
        }

        // Files.com may redirect the final request with 307 or 308: it is sent again where the redirect says, with the same
        // body and reference, inside the same attempt, and the upload completes. Once the routed request has been sent, a
        // lost or failed answer leaves the upload uncertain: with retries left, it is not sent a third time, and no part
        // or new upload is sent again.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(307, "answered", DisplayName = "Redirected with 307, and completed where it was sent")]
        [DataRow(308, "answered", DisplayName = "Redirected with 308, and completed where it was sent")]
        [DataRow(307, "lost", DisplayName = "Redirected with 307, and its routed answer lost")]
        [DataRow(308, "error", DisplayName = "Redirected with 308, and its routed answer a server error")]
        public async Task ARedirectedFinalRequestIsSentWhereItIsRedirectedAndNotAgainAfterItsAnswer(int status, string routed)
        {
            using (var storage = new UploadServer())
            using (var api = new SilentFinishApi(storage.PartUrl("remote.bin", 1, 1), redirect: status, routed: routed))
            {
                FilesConfiguration configuration = storage.Site.Configuration(maxNetworkRetries: 3);
                configuration.BaseUrl = api.Url;
                Task<UploadResult> upload = FilesClient.Create(configuration).Transfers.UploadFileAsync(WriteFile("local.bin", Pattern(8)), "remote.bin");

                if (routed == "answered")
                {
                    await upload;
                }
                else
                {
                    UploadCompletionUnknownException error = await ThrowsAsync<UploadCompletionUnknownException>(upload);
                    Assert.IsInstanceOfType(error.InnerException, routed == "lost" ? typeof(ApiConnectionException) : typeof(ApiException));
                    Assert.IsTrue(((SdkException)error.InnerException).RequestMayHaveBeenProcessed);
                }

                Assert.AreEqual("begin, end, end at /moved", string.Join(", ", api.Requests), "Sent where it was addressed, then where it was redirected, and no more.");
                Assert.AreEqual(1, storage.Parts.Count, "No part is sent again.");
                IReadOnlyList<string> bodies = api.FinishBodies;
                Assert.AreEqual(bodies[0], bodies[1], "The same final request, both times.");
                StringAssert.Contains(bodies[1], "ref-remote.bin");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(503)]
        [DataRow(429)]
        public async Task ThrottledPartIsSentAgainAfterTheServersRetryAfter(int status)
        {
            using (var server = new UploadServer())
            {
                server.RespondToPart = request =>
                {
                    if (request.Number != 2 || request.Attempt > 1)
                    {
                        return Task.FromResult(UploadServer.Accept(request));
                    }
                    ResponseMessage throttled = LoopbackSite.Status(status);
                    throttled.AddHeader("Retry-After", "1");
                    return Task.FromResult(throttled);
                };
                byte[] contents = Pattern(20);

                // The client's own retry delay is zero; the server asked for one second.
                UploadResult result = await Within(Client(server, retries: 1).Transfers.UploadFileAsync(WriteFile("local.bin", contents), "remote.bin"));

                PartRequest[] part2 = server.Parts.Where(part => part.Number == 2).ToArray();
                Assert.AreEqual(2, part2.Length);
                Assert.IsTrue(part2[1].ArrivedAt - part2[0].ArrivedAt >= TimeSpan.FromMilliseconds(900), $"Waited {(part2[1].ArrivedAt - part2[0].ArrivedAt).TotalMilliseconds} ms");
                CollectionAssert.AreEqual(part2[0].Bytes, part2[1].Bytes);
                CollectionAssert.AreEqual(contents, server.AcceptedBytes("remote.bin"));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(503)]
        [DataRow(429)]
        public async Task ThrottledPartWhoseRetryAfterIsLongerThanThePlatformCanWaitFailsTheUploadWithItsAnswer(int status)
        {
            using (var server = new UploadServer())
            {
                server.RespondToPart = request =>
                {
                    if (request.Number != 2)
                    {
                        return Task.FromResult(UploadServer.Accept(request));
                    }
                    ResponseMessage throttled = LoopbackSite.Status(status);
                    throttled.AddHeader("Retry-After", "99999999999");
                    return Task.FromResult(throttled);
                };

                // No timer can wait so long: the part is not sent again sooner than the server asked, and its answer
                // ends the upload at once, as when no retry is left.
                ApiException error = await ThrowsAsync<ApiException>(Client(server, retries: 3).Transfers.UploadFileAsync(WriteFile("local.bin", Pattern(20)), "remote.bin"));

                StringAssert.Contains(error.Message, status.ToString());
                Assert.AreEqual(1, server.Parts.Count(part => part.Number == 2));
                Assert.AreEqual(0, server.Finishes.Count);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(true, DisplayName = "To the same origin")]
        [DataRow(false, DisplayName = "To another origin")]
        public async Task RedirectedPartIsSentOnUnchangedWithinTheSameAttempt(bool sameOrigin)
        {
            using (var elsewhere = new UploadServer())
            using (var server = new UploadServer { RetryParts = "false" })
            {
                server.ExtraPartFields = ",\"headers\":{\"Authorization\":\"Part part-token\",\"X-Part-Hint\":\"kept\"}";
                string location = (sameOrigin ? server : elsewhere).PartUrl("moved", 1, 1);
                server.RespondToPart = request =>
                {
                    if (request.File == "moved" || request.Number != 1)
                    {
                        return Task.FromResult(UploadServer.Accept(request));
                    }
                    ResponseMessage redirect = LoopbackSite.Status(307);
                    redirect.AddHeader("Location", location);
                    return Task.FromResult(redirect);
                };
                byte[] contents = Pattern(20);

                UploadResult result = await Within(Client(server).Transfers.UploadFileAsync(WriteFile("local.bin", contents), "remote.bin"));

                PartRequest moved = (sameOrigin ? server : elsewhere).Parts.Single(part => part.File == "moved");
                CollectionAssert.AreEqual(contents.Take(8).ToArray(), moved.Bytes, "The redirect target receives the part's exact bytes.");
                Assert.AreEqual(location.Substring(location.IndexOf("/part/")), moved.Request.Path, "The Location is followed exactly, signed query included.");
                Assert.AreEqual("kept", moved.Request.Headers["X-Part-Hint"]);
                Assert.AreEqual(sameOrigin ? "Part part-token" : null, moved.Request.Headers.TryGetValue("Authorization", out string authorization) ? authorization : null, "Credentials stay within the origin they were negotiated for.");
                AssertNoFilesCredentials(moved.Request);
                Assert.AreEqual(1, result.Parts[0].Attempts, "Following a redirect is not a retry, and does not need the server's permission to retry.");
                Assert.AreEqual("\"etag-moved-1-1\"", result.Parts[0].ETag);
                Assert.AreEqual(28, result.SentBytes, "Part 1 was sent twice.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task RedirectThatWouldChangeThePartIsNotFollowed()
        {
            using (var elsewhere = new UploadServer())
            using (var server = new UploadServer())
            {
                string location = elsewhere.PartUrl("moved", 1, 1);
                server.RespondToPart = request =>
                {
                    // 303 turns the request into a GET, which would not upload the part.
                    ResponseMessage redirect = LoopbackSite.Status(303);
                    redirect.AddHeader("Location", location);
                    return Task.FromResult(redirect);
                };

                InvalidResponseException error = await ThrowsAsync<InvalidResponseException>(Client(server, retries: 3).Transfers.UploadFileAsync(WriteFile("local.bin", Pattern(20)), "remote.bin"));

                StringAssert.Contains(error.Message, "303");
                Assert.IsFalse(error.Message.Contains("Signature"), "The error does not repeat the signed Location.");
                Assert.AreEqual(0, elsewhere.Site.Requests.Count);
                Assert.AreEqual(0, server.Finishes.Count);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task StalledPartRequestsEndWhenTheCallerCancels()
        {
            using (var storage = new StallingStorage())
            using (var server = new UploadServer())
            {
                var never = new TaskCompletionSource<ResponseMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
                server.RespondToPart = request =>
                {
                    if (request.Number != 1)
                    {
                        return never.Task;
                    }
                    ResponseMessage redirect = LoopbackSite.Status(307);
                    redirect.AddHeader("Location", storage.Url);
                    return Task.FromResult(redirect);
                };
                server.PartSize = number => 16 * 1024 * 1024;
                // Part 1 is redirected to storage that stops reading its body; part 2 waits for an answer that never comes.
                string localPath = WriteFile("local.bin", new byte[32 * 1024 * 1024]);
                var limiter = new TransferLimiter();
                var cancellation = new CancellationTokenSource();
                var trace = new TraceLog();

                Task<UploadResult> upload = Client(server).Transfers.UploadFileAsync(localPath, "remote.bin", new UploadOptions { Limiter = limiter, Trace = trace }, cancellation.Token);
                await server.WaitUntil(s => s.PartsInFlight == 1 && storage.Stalled, "one part waits for its answer and the redirected one cannot send its body");
                cancellation.Cancel();

                await AssertCancelledPromptly(upload);
                Assert.AreEqual(0, trace.Attempts.Count, "Cancelled requests are not measured.");
                Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes + limiter.QueuedRequests + limiter.QueuedBufferReservations);
                using (new FileStream(localPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    // Every reader of the file has been closed.
                }
                Assert.AreEqual(0, server.Finishes.Count);
                never.SetResult(UploadServer.AcceptWithoutETag());
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task RetriedFilePartIsReadFromTheOpenedFileEvenIfItsNameIsReplaced()
        {
            using (var server = new UploadServer())
            {
                byte[] original = Pattern(16);
                string localPath = WriteFile("local.bin", original);
                bool replaced = false;
                server.RespondToPart = request =>
                {
                    if (request.Number == 1 && request.Attempt == 1)
                    {
                        // Give the name to a different file of the same length. Windows refuses while the upload holds the file.
                        try
                        {
                            File.Move(localPath, localPath + ".old");
                            File.WriteAllBytes(localPath, Pattern(16, seed: 100));
                            replaced = true;
                        }
                        catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
                        {
                        }
                        return Task.FromResult(LoopbackSite.Status(503));
                    }
                    return Task.FromResult(UploadServer.Accept(request));
                };

                await Within(Client(server, retries: 1).Transfers.UploadFileAsync(localPath, "remote.bin"));

                Assert.AreEqual(!RuntimeInformation.IsOSPlatform(OSPlatform.Windows), replaced, "The file's name was given to another file (except on Windows, which prevents it).");
                PartRequest[] part1 = server.Parts.Where(part => part.Number == 1).ToArray();
                Assert.AreEqual(2, part1.Length);
                CollectionAssert.AreEqual(part1[0].Bytes, part1[1].Bytes, "The retry resends the opened file's bytes.");
                CollectionAssert.AreEqual(original, server.AcceptedBytes("remote.bin"));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(false, DisplayName = "The part waits for the part in flight")]
        [DataRow(true, DisplayName = "Cancelled while it waits")]
        public async Task RenewedUrlThatWithdrawsParallelPermissionIsFollowedBeforeThePartIsSent(bool cancel)
        {
            using (var server = new UploadServer())
            {
                // Part 2's first URL is about to expire; its renewal says parts must now go one at a time.
                server.Expires = (number, negotiation) => (number == 2 && negotiation == 1 ? DateTime.UtcNow.AddSeconds(5) : DateTime.UtcNow.AddHours(1)).ToString("o");
                server.ParallelPartsFor = (number, negotiation) => number == 2 && negotiation == 2 ? "false" : "true";
                var releasePart1 = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                server.RespondToPart = async request =>
                {
                    if (request.Number == 1)
                    {
                        await releasePart1.Task;
                    }
                    return UploadServer.Accept(request);
                };
                byte[] contents = Pattern(16);
                var limiter = new TransferLimiter();
                var cancellation = new CancellationTokenSource();

                Task<UploadResult> upload = Client(server).Transfers.UploadStreamAsync(new MemoryStream(contents), 16, "remote.bin", new UploadOptions { Limiter = limiter, MaxConcurrency = 2 }, cancellation.Token);
                await server.WaitUntil(s => s.PartsInFlight == 1 && s.Events.Count(e => e == "begin remote.bin part=2 parts=1") == 2, "part 1 is in flight and part 2's URL has been renewed");
                await Task.Delay(300); // Time for part 2 to be sent, if it would be.
                Assert.IsFalse(server.Parts.Any(part => part.Number == 2), "Part 2 is not sent alongside part 1 once the server withdrew permission.");

                if (cancel)
                {
                    cancellation.Cancel();
                    await AssertCancelledPromptly(upload);
                    Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes + limiter.QueuedRequests + limiter.QueuedBufferReservations);
                    releasePart1.SetResult(true);
                    Assert.IsFalse(server.Parts.Any(part => part.Number == 2));
                    Assert.AreEqual(0, server.Finishes.Count);
                    return;
                }
                releasePart1.SetResult(true);
                UploadResult result = await Within(upload);

                Assert.IsFalse(result.ParallelParts);
                Assert.AreEqual(UploadSerialReason.ServerWithdrewParallelParts, result.FinalProgress.Concurrency.SerialReason);
                Assert.AreEqual(1, server.Parts.Count(part => part.Number == 2));
                CollectionAssert.AreEqual(contents, server.AcceptedBytes("remote.bin"));
                Assert.AreEqual(1, server.Finishes.Count);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task RenewalThatWithdrawsParallelPermissionFromAPartAlreadySentAlongsideOthersStopsTheUpload()
        {
            using (var server = WithdrawingParallelPermissionWhenPart2IsRetried())
            using (var cancellation = new CancellationTokenSource(CallerBound))
            {
                // The site holds part 1's answer, so after the 503 lowers the target from 2 to 1, part 1 fills it: part
                // 2's retry waits for room, before its URL is renewed, until the test releases part 1.
                var releasePart1 = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                Func<PartRequest, Task<ResponseMessage>> respond = server.RespondToPart;
                server.RespondToPart = async request =>
                {
                    if (request.Number == 1)
                    {
                        await releasePart1.Task;
                    }
                    return await respond(request);
                };
                var limiter = new TransferLimiter();
                var trace = new TraceLog();
                Task<UploadResult> upload = null;
                Func<string> state = () => Observe(upload, server, limiter, trace);
                bool passed = false;
                try
                {
                    upload = Client(server, retries: 1).Transfers.UploadStreamAsync(new MemoryStream(Pattern(16)), 16, "remote.bin", new UploadOptions { Limiter = limiter, MaxConcurrency = 2, Trace = trace }, cancellation.Token);
                    await server.WaitUntil(s => trace.Decisions.Any(d => d.Decision == ConcurrencyDecision.CooldownStarted), "part 2's 503 starts a cooldown", state);
                    UploadDecisionTrace cooldown = trace.Decisions.First(d => d.Decision == ConcurrencyDecision.CooldownStarted);
                    Assert.AreEqual("2->1", $"{cooldown.PreviousTarget}->{cooldown.Target}", "The 503 lowers the target to 1: " + state());
                    TimeSpan untilAfterCooldown = trace.ReceivedAt(cooldown) + cooldown.Cooldown + TimeSpan.FromMilliseconds(300) - UploadServer.Clock;
                    await Task.Delay(untilAfterCooldown > TimeSpan.Zero ? untilAfterCooldown : TimeSpan.Zero);

                    Assert.IsFalse(upload.IsCompleted, "After the cooldown the upload is waiting, not failing: " + state());
                    Assert.AreEqual("1 0", $"{limiter.ActiveRequests} {limiter.QueuedRequests}", "Part 1 fills the lowered target; part 2's retry waits for room, not for the limiter: " + state());
                    Assert.AreEqual(1, server.Events.Count(e => e == "begin remote.bin part=2 parts=1"), "Part 2's URL has not been renewed yet: " + state());
                    releasePart1.SetResult(true);

                    InvalidResponseException error = await ThrowsAsync<InvalidResponseException>(upload, state);

                    StringAssert.Contains(error.Message, "part 2");
                    Assert.AreEqual(2, server.Events.Count(e => e == "begin remote.bin part=2 parts=1"), "The failure came from renewing part 2's URL: " + state());
                    Assert.AreEqual(1, server.Parts.Count(part => part.Number == 2), "Part 2 is not sent against the renewed instruction.");
                    Assert.AreEqual(0, server.Finishes.Count);
                    Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes + limiter.QueuedRequests + limiter.QueuedBufferReservations, "Everything was released: " + state());
                    passed = true;
                }
                finally
                {
                    await SettleAsync(passed, upload, cancellation, server, limiter, state, releasePart1);
                }
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task RenewalThatWithdrawsParallelPermissionFromAPartSentAloneIsFollowedAndReported()
        {
            using (var server = WithdrawingParallelPermissionWhenPart2IsRetried())
            {
                byte[] contents = Pattern(16);
                var limiter = new TransferLimiter();

                UploadResult result = await Within(Client(server, retries: 1).Transfers.UploadStreamAsync(new MemoryStream(contents), 16, "remote.bin", new UploadOptions { Limiter = limiter, MaxConcurrency = 1 }));

                List<string> events = server.Events.ToList();
                Assert.IsTrue(events.LastIndexOf("begin remote.bin part=2 parts=1") > events.IndexOf("put remote.bin/2"), $"Part 2's URL is renewed after the part was first sent. Events: {string.Join(", ", events)}");
                Assert.IsFalse(result.ParallelParts, "The result reports that the server withdrew permission to send parts in parallel.");
                Assert.AreEqual(2, server.Parts.Count(part => part.Number == 2));
                CollectionAssert.AreEqual(contents, server.AcceptedBytes("remote.bin"));
                Assert.AreEqual(1, server.Finishes.Count);
                Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes);
            }
        }

        // Part 2's URL is fresh when it is admitted, but has nearly expired by its retry, whose renewal withdraws
        // permission to send parts in parallel.
        private static UploadServer WithdrawingParallelPermissionWhenPart2IsRetried()
        {
            var server = new UploadServer();
            server.Expires = (number, negotiation) => (number == 2 && negotiation == 1 ? DateTime.UtcNow.AddSeconds(31) : DateTime.UtcNow.AddHours(1)).ToString("o");
            server.ParallelPartsFor = (number, negotiation) => number == 2 && negotiation == 2 ? "false" : "true";
            server.RespondToPart = request =>
            {
                if (request.Number == 2 && request.Attempt == 1)
                {
                    ResponseMessage throttled = LoopbackSite.Status(503);
                    throttled.AddHeader("Retry-After", "2");
                    return Task.FromResult(throttled);
                }
                return Task.FromResult(UploadServer.Accept(request));
            };
            return server;
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(false, DisplayName = "Of known length")]
        [DataRow(true, DisplayName = "Of unknown length")]
        public async Task CancellingDuringAStreamReadThatIgnoresItStartsNoFurtherRead(bool unknownLength)
        {
            using (var server = new UploadServer())
            {
                var source = new StalledFirstReadStream();
                var limiter = new TransferLimiter();
                var cancellation = new CancellationTokenSource();

                Task<UploadResult> upload = unknownLength
                    ? Client(server).Transfers.UploadStreamAsync(source, "remote.bin", new UploadOptions { Limiter = limiter }, cancellation.Token)
                    : Client(server).Transfers.UploadStreamAsync(source, 8, "remote.bin", new UploadOptions { Limiter = limiter }, cancellation.Token);
                await Within(source.FirstReadStarted);
                cancellation.Cancel();
                await Task.Delay(200);
                Assert.IsFalse(upload.IsCompleted, "The upload waits for the read it started, which ignores cancellation.");
                source.ReleaseFirstRead();

                await AssertCancelledPromptly(upload);
                Assert.AreEqual(1, source.Reads, "No read starts after cancellation.");
                Assert.AreEqual(0, limiter.BufferedBytes);
                Assert.AreEqual(0, server.Parts.Count);
                Assert.IsTrue(source.CanRead, "The borrowed stream is left open.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task UploadsThatCannotBeSentAsNegotiatedFailBeforeAnyPartIsSent()
        {
            using (var server = new UploadServer { ExtraPartFields = ",\"parameters\":{\"signature_field\":\"value\"}" })
            {
                await ThrowsAsync<FilesCom.NotImplementedException>(Client(server).Transfers.UploadFileAsync(WriteFile("parameters.bin", Pattern(20)), "remote.bin"));

                Assert.AreEqual(0, server.Parts.Count);
                Assert.AreEqual(0, server.Finishes.Count);
            }
            using (var server = new UploadServer())
            {
                var source = new ShortReadStream(Pattern(20));

                UploadLimitException error = await ThrowsAsync<UploadLimitException>(Client(server).Transfers.UploadStreamAsync(source, 20, "remote.bin", new UploadOptions { Limiter = new TransferLimiter(maxBufferedBytes: 4) }));

                Assert.AreEqual(UploadLimit.BufferSpace, error.Limit);
                StringAssert.Contains(error.Message, "MaxBufferedBytes");
                Assert.AreEqual(0, source.BytesRead, "A part that cannot be held is not read.");
                Assert.AreEqual(0, server.Parts.Count);
            }
        }

        // Files.com starts small and grows its server-planned parts. The first size alone would project five parts,
        // but this plan needs only the four slots available, including when the caller selects server sizing for S3.
        [TestMethod]
        [Timeout(60000)]
        public async Task GrowingServerPartsFitWithinTheAvailableSlots()
        {
            using (var server = new UploadServer { Aws = true, ExtraPartFields = ",\"available_parts\":4" })
            {
                server.PartSize = number => number == 1 ? 5 : number <= 3 ? 6 : 8;
                byte[] contents = Pattern(25);
                string localPath = WriteFile("local.bin", contents);

                UploadResult result = await Within(Client(server).Transfers.UploadFileAsync(localPath, "remote.bin", new UploadOptions { PartSizingMode = UploadPartSizingMode.ServerPlanned }));

                CollectionAssert.AreEqual(new long[] { 5, 6, 6, 8 }, result.Parts.Select(part => part.Length).ToArray());
                CollectionAssert.AreEqual(contents, server.AcceptedBytes("remote.bin"));
                Assert.AreEqual(PartSizeSource.ServerPlanned, result.FinalProgress.PartSizing.Source);
                Assert.AreEqual(1, server.Finishes.Count, "The valid plan completed.");
            }
        }

        private static FilesClient Client(UploadServer server, int retries = 0)
        {
            return server.Client(retries);
        }

        private string WriteFile(string name, byte[] contents)
        {
            string path = Path.Combine(directory, name);
            File.WriteAllBytes(path, contents);
            return path;
        }

        private static byte[] Pattern(int length, int seed = 1)
        {
            return Enumerable.Range(0, length).Select(i => (byte)((i * 7) + seed)).ToArray();
        }

        private static void AssertNoFilesCredentials(ReceivedRequest request)
        {
            Assert.IsNull(request.ApiKey, "No API key on a part request");
            Assert.IsNull(request.SessionId, "No session on a part request");
            Assert.IsNull(request.WorkspaceId, "No workspace on a part request");
        }

        private static void AssertApiRequests(UploadServer server, string apiKey, string workspaceId)
        {
            ReceivedRequest[] api = server.Site.Requests.Where(request => !request.Path.StartsWith("/part/")).ToArray();
            Assert.IsTrue(api.Length >= 2);
            foreach (ReceivedRequest request in api)
            {
                Assert.AreEqual($"{apiKey}/{workspaceId}", $"{request.ApiKey}/{request.WorkspaceId}", request.ToString());
            }
        }

        private static async Task<T> Within<T>(Task<T> operation)
        {
            if (await Task.WhenAny(operation, Task.Delay(Promptly)) != operation)
            {
                Assert.Fail($"The upload was still running after {Promptly.TotalSeconds} seconds.");
            }
            return await operation;
        }

        // state, if given, describes the upload's stage when it has not ended in time.
        private static async Task Within(Task operation, Func<string> state = null)
        {
            if (await Task.WhenAny(operation, Task.Delay(Promptly)) != operation)
            {
                Assert.Fail($"The uploads were still running after {Promptly.TotalSeconds} seconds." + (state != null ? " " + state() : ""));
            }
            await operation;
        }

        private static async Task<T> ThrowsAsync<T>(Task operation, Func<string> state = null) where T : Exception
        {
            try
            {
                await Within(operation, state);
            }
            catch (T e)
            {
                return e;
            }
            Assert.Fail($"The upload did not fail with {typeof(T).Name}." + (state != null ? " " + state() : ""));
            return null;
        }

        // A backstop for tests that end their upload themselves: it is cancelled if it is still running this long after it
        // started.
        private static readonly TimeSpan CallerBound = TimeSpan.FromSeconds(30);

        // How long cleanup waits for an upload to end once cancelled, and for the site's part handlers to finish.
        private static readonly TimeSpan SettleWithin = TimeSpan.FromSeconds(5);

        // The upload's stage in one line, for failure messages: its task, the limiter, the site's requests and the trace.
        private static string Observe(Task upload, UploadServer server, TransferLimiter limiter, TraceLog trace)
        {
            return $"upload {(upload == null ? "not started" : upload.Status.ToString())}; " +
                $"limiter active={limiter.ActiveRequests} queued={limiter.QueuedRequests} buffered={limiter.BufferedBytes} waiting-for-buffer={limiter.QueuedBufferReservations}; " +
                $"site in flight={server.PartsInFlight} events: {string.Join(", ", server.Events)}; " +
                $"decisions: {string.Join(", ", trace.Decisions.Select(d => $"{d.Decision} {d.PreviousTarget}->{d.Target}"))}; " +
                $"attempts: {string.Join(", ", trace.Attempts.Select(a => $"{a.PartNumber}#{a.Attempt}={a.StatusCode}"))}";
        }

        // Ends what a test started, whether or not it passed: cancels the upload, releases the answers the test holds,
        // and waits for the upload and the site's part handlers to finish. A test that failed keeps its own failure; the
        // stage it failed in and anything left unsettled are written to its output. A test that passed fails if anything
        // could not be settled.
        private static async Task SettleAsync(bool passed, Task upload, CancellationTokenSource cancellation, UploadServer server, TransferLimiter limiter, Func<string> state, params TaskCompletionSource<bool>[] holds)
        {
            if (!passed)
            {
                Console.WriteLine("Stage when the test failed: " + state());
            }
            var unsettled = new List<string>();
            try
            {
                cancellation.Cancel();
                foreach (TaskCompletionSource<bool> hold in holds)
                {
                    hold.TrySetResult(true);
                }
                if (upload != null && await Task.WhenAny(upload, Task.Delay(SettleWithin)) != upload)
                {
                    unsettled.Add($"the upload had not ended {SettleWithin.TotalSeconds} seconds after it was cancelled");
                }
                Stopwatch waited = Stopwatch.StartNew();
                while (server.PartsInFlight > 0 && waited.Elapsed < SettleWithin)
                {
                    await Task.Delay(10);
                }
                if (server.PartsInFlight > 0)
                {
                    unsettled.Add($"{server.PartsInFlight} of the site's part handlers had not finished");
                }
                if (limiter.ActiveRequests + limiter.BufferedBytes + limiter.QueuedRequests + limiter.QueuedBufferReservations > 0)
                {
                    unsettled.Add("the limiter still held or queued requests or buffers");
                }
            }
            catch (Exception e)
            {
                unsettled.Add("cleanup failed: " + e.Message);
            }
            if (unsettled.Count == 0)
            {
                if (!passed)
                {
                    Console.WriteLine("Cleanup after the failure ended the upload, the site's part handlers and the limiter's use.");
                }
                return;
            }
            string message = $"Cleanup: {string.Join("; ", unsettled)}. {state()}";
            if (passed)
            {
                Assert.Fail(message);
            }
            Console.WriteLine(message);
        }

        private static async Task AssertCancelledPromptly(Task operation)
        {
            try
            {
                await Within(operation);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            Assert.Fail("The upload did not report its cancellation.");
        }

        // Storage on a raw loopback socket that accepts a part request and its headers, then never reads the body, like
        // a stalled connection. Stalled is set once the request has arrived.
        private sealed class StallingStorage : IDisposable
        {
            private readonly TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
            private readonly List<TcpClient> connections = new List<TcpClient>();
            private volatile bool stalled;

            public StallingStorage()
            {
                listener.Start();
                Url = $"http://127.0.0.1:{((IPEndPoint)listener.LocalEndpoint).Port}/stalled";
                _ = AcceptAsync();
            }

            public string Url { get; }

            public bool Stalled => stalled;

            public void Dispose()
            {
                listener.Stop();
                lock (connections)
                {
                    connections.ForEach(connection => connection.Dispose());
                }
            }

            private async Task AcceptAsync()
            {
                try
                {
                    while (true)
                    {
                        TcpClient connection = await listener.AcceptTcpClientAsync();
                        lock (connections)
                        {
                            connections.Add(connection);
                        }
                        var one = new byte[1];
                        var ending = new Queue<byte>();
                        while (await connection.GetStream().ReadAsync(one, 0, 1) == 1)
                        {
                            ending.Enqueue(one[0]);
                            if (ending.Count > 4)
                            {
                                ending.Dequeue();
                            }
                            if (Encoding.ASCII.GetString(ending.ToArray()) == "\r\n\r\n")
                            {
                                stalled = true;
                                break;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // Stopped by the test.
                }
            }
        }

        // The API side of a one-part upload on a raw loopback socket: it answers begin_upload with a part sent to
        // partUrl, and closes the connection after receiving the final request, without answering it, as a network
        // failure after the request reached Files.com would.
        // The API of an upload of one part: it answers the request that begins it, and never the one that completes it, whose
        // connection it closes; or, given redirect, answers that one with that status to /moved and the path, and the
        // request sent there as routed says: "answered" with the file, "error" with 503, "lost" by closing the connection.
        private sealed class SilentFinishApi : IDisposable
        {
            private readonly TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
            private readonly List<string> requests = new List<string>();
            private readonly List<string> finishBodies = new List<string>();
            private readonly string partUrl;
            private readonly int redirect;
            private readonly string routed;

            public SilentFinishApi(string partUrl, int redirect = 0, string routed = "lost")
            {
                this.partUrl = partUrl;
                this.redirect = redirect;
                this.routed = routed;
                listener.Start();
                Url = $"http://127.0.0.1:{((IPEndPoint)listener.LocalEndpoint).Port}";
                _ = ServeAsync();
            }

            public string Url { get; }

            public IReadOnlyList<string> Requests
            {
                get
                {
                    lock (requests)
                    {
                        return requests.ToList();
                    }
                }
            }

            // The body of each request to complete the upload, in the order received.
            public IReadOnlyList<string> FinishBodies
            {
                get
                {
                    lock (requests)
                    {
                        return finishBodies.ToList();
                    }
                }
            }

            public void Dispose()
            {
                listener.Stop();
            }

            private async Task ServeAsync()
            {
                try
                {
                    while (true)
                    {
                        using (TcpClient connection = await listener.AcceptTcpClientAsync())
                        using (NetworkStream stream = connection.GetStream())
                        {
                            (string requestLine, string body) = await ReadRequestAsync(stream);
                            string target = requestLine.Split(' ')[1];
                            bool moved = target.StartsWith("/moved/", StringComparison.Ordinal);
                            bool finish = requestLine.StartsWith("POST /api/rest/v1/files/") || (moved && requestLine.StartsWith("POST "));
                            lock (requests)
                            {
                                requests.Add(!finish ? "begin" : moved ? "end at /moved" : "end");
                                if (finish)
                                {
                                    finishBodies.Add(body);
                                }
                            }
                            if (!finish)
                            {
                                await AnswerAsync(stream, 200, $"[{{\"upload_uri\":\"{partUrl}\",\"http_method\":\"PUT\",\"ref\":\"ref-remote.bin\",\"part_number\":1,\"partsize\":8,\"parallel_parts\":true,\"retry_parts\":true}}]");
                            }
                            else if (redirect != 0 && !moved)
                            {
                                byte[] head = Encoding.ASCII.GetBytes($"HTTP/1.1 {redirect} Redirect\r\nLocation: /moved{target}\r\nContent-Length: 0\r\nConnection: close\r\n\r\n");
                                await stream.WriteAsync(head, 0, head.Length);
                            }
                            else if (moved && routed == "answered")
                            {
                                await AnswerAsync(stream, 200, "{\"path\":\"remote.bin\",\"type\":\"file\",\"size\":8}");
                            }
                            else if (moved && routed == "error")
                            {
                                await AnswerAsync(stream, 503, "{\"error\":\"Unavailable.\",\"http-code\":503}");
                            }
                        }
                    }
                }
                catch (Exception) when (!listener.Server.IsBound)
                {
                    // Stopped by the test.
                }
                catch (ObjectDisposedException)
                {
                    // Stopped by the test.
                }
            }

            private static async Task AnswerAsync(NetworkStream stream, int status, string json)
            {
                byte[] answer = Encoding.UTF8.GetBytes(json);
                byte[] head = Encoding.ASCII.GetBytes($"HTTP/1.1 {status} {(status < 400 ? "OK" : "Failed")}\r\nContent-Type: application/json\r\nContent-Length: {answer.Length}\r\nConnection: close\r\n\r\n");
                await stream.WriteAsync(head, 0, head.Length);
                await stream.WriteAsync(answer, 0, answer.Length);
            }

            // Reads one request's line, headers and body, and returns its request line and body.
            private static async Task<(string Line, string Body)> ReadRequestAsync(NetworkStream stream)
            {
                var headers = new List<byte>();
                var one = new byte[1];
                while (!EndsWithBlankLine(headers) && await stream.ReadAsync(one, 0, 1) == 1)
                {
                    headers.Add(one[0]);
                }
                string text = Encoding.ASCII.GetString(headers.ToArray());
                string length = text.Split(new[] { "\r\n" }, StringSplitOptions.None)
                    .FirstOrDefault(line => line.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase))?.Split(':')[1].Trim();
                var body = new byte[length == null ? 0 : int.Parse(length)];
                for (int read = 0; read < body.Length;)
                {
                    int count = await stream.ReadAsync(body, read, body.Length - read);
                    if (count == 0)
                    {
                        break;
                    }
                    read += count;
                }
                return (text.Split(new[] { "\r\n" }, StringSplitOptions.None)[0], Encoding.UTF8.GetString(body));
            }

            private static bool EndsWithBlankLine(List<byte> bytes)
            {
                int n = bytes.Count;
                return n >= 4 && bytes[n - 4] == '\r' && bytes[n - 3] == '\n' && bytes[n - 2] == '\r' && bytes[n - 1] == '\n';
            }
        }

        // Records every report, in order; can throw from Report to act as a failing handler.
        private sealed class ProgressLog : IProgress<TransferProgress>
        {
            private readonly Func<TransferProgress, Exception> failure;
            private readonly List<TransferProgress> reports = new List<TransferProgress>();

            public ProgressLog(Func<TransferProgress, Exception> failure = null)
            {
                this.failure = failure;
            }

            public IReadOnlyList<TransferProgress> Reports
            {
                get
                {
                    lock (reports)
                    {
                        return reports.ToList();
                    }
                }
            }

            public void Report(TransferProgress value)
            {
                lock (reports)
                {
                    reports.Add(value);
                }
                Exception error = failure?.Invoke(value);
                if (error != null)
                {
                    throw error;
                }
            }
        }

        // A caller's stream that, on its first read, registers a callback that throws on the token it was given, then
        // reads as inner does.
        private sealed class CallbackRegisteringStream : Stream
        {
            private readonly Stream inner;

            internal CallbackRegisteringStream(Stream inner)
            {
                this.inner = inner;
            }

            internal bool Registered { get; private set; }

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

            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                if (!Registered)
                {
                    Registered = true;
                    cancellationToken.Register(() => throw new InvalidOperationException("The stream's cancellation callback failed."));
                }
                return inner.ReadAsync(buffer, offset, count, cancellationToken);
            }

            public override void Flush()
            {
            }

            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

            public override void SetLength(long value) => throw new NotSupportedException();

            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        }

        // A forward-only stream whose reads return at most three bytes and complete asynchronously.
        // A caller's forward-only stream that gives at most 3 bytes a read and, with failure, throws it from the first read
        // once failAfter bytes have been read.
        private sealed class ShortReadStream : Stream
        {
            private readonly byte[] data;
            private readonly int failAfter;
            private readonly Exception failure;
            private int position;
            private bool disposed;

            public ShortReadStream(byte[] data, int failAfter = int.MaxValue, Exception failure = null)
            {
                this.data = data;
                this.failAfter = failAfter;
                this.failure = failure;
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

            public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                await Task.Yield();
                if (failure != null && position >= failAfter)
                {
                    throw failure;
                }
                int length = Math.Min(Math.Min(count, 3), data.Length - position);
                Array.Copy(data, position, buffer, offset, length);
                Volatile.Write(ref position, position + length);
                return length;
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