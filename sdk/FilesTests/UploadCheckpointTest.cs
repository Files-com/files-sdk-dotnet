using FilesCom.Transfers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using WireMock;

namespace FilesTests
{
    // File uploads with a checkpoint: stopped, then run again with the same arguments. The loopback site remembers every
    // upload it began, so what a later run reuses, resends or begins anew is what arrives.
    public partial class ParallelUploadTest
    {
        private const string UploadGone = "{\"error\":\"Upload not found\",\"http-code\":404,\"type\":\"not-found/file-upload-not-found\"}";
        private const string DestinationLocked = "{\"error\":\"Destination is locked\",\"http-code\":423,\"type\":\"processing-failure/file-locked\"}";
        private const string RegionMismatch = "{\"error\":\"You are attempting to access a site in a different region.\",\"http-code\":403,\"title\":\"Lockout Region Mismatch\",\"type\":\"not-authenticated/lockout-region-mismatch\",\"data\":{\"host\":\"eu.relocation.test\"}}";

        // A client whose policy trusts the site's answer that it is served from eu.relocation.test, although it comes
        // over plain HTTP.
        private static FilesCom.FilesClient RelocatingClient(UploadServer server)
        {
            FilesCom.FilesConfiguration configuration = server.Site.Configuration();
            configuration.RegionRelocation = FilesCom.RegionRelocationPolicy.OnlyBetween(new Uri(server.Site.Url).GetLeftPart(UriPartial.Authority), "https://eu.relocation.test");
            return FilesCom.FilesClient.Create(configuration);
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(false, DisplayName = "Parts the server plans")]
        [DataRow(true, DisplayName = "Parts the upload chooses")]
        public async Task CancelledUploadContinuesWithoutSendingAcceptedPartsAgain(bool chooseSizes)
        {
            using (var server = new UploadServer { ParallelParts = "false", PartSize = number => KiB })
            {
                if (chooseSizes)
                {
                    UseAws(server, 1, FilesMaxFile, 10000, FilesMaxFile);
                }
                byte[] contents = Pattern(5 * KiB);
                string localPath = WriteFile("local.bin", contents);
                string checkpoint = Path.Combine(directory, "upload.checkpoint");
                var options = new UploadOptions { CheckpointPath = checkpoint, AutomaticPartSizing = chooseSizes ? GrowingFrom(KiB, max: KiB) : null };

                await CancelAtPart(server, localPath, options, 3);

                string recorded = File.ReadAllText(checkpoint);
                Assert.AreEqual(1, JsonDocument.Parse(recorded).RootElement.GetProperty("version").GetInt32(), "Uploads keep writing their checkpoints in format 1.");
                foreach (string secret in new[] { server.Site.ApiKey, "X-Amz", "Signature", "/part/", UploadServer.AwsHost, "up~1" })
                {
                    Assert.IsFalse(recorded.Contains(secret), $"The checkpoint holds no credentials or storage URLs ({secret}).");
                }
                Assert.AreEqual(chooseSizes, JsonDocument.Parse(recorded).RootElement.GetProperty("route").ValueKind == JsonValueKind.String, "An upload choosing its sizes records which S3 upload its parts go to, as a digest.");
                int sentBefore = server.Parts.Count;

                // Its own negotiations are known by its run: the cancelled run may still have had one on its way.
                UploadResult result = await Within(server.Client(run: "again").Transfers.UploadFileAsync(localPath, "remote.bin", options));

                CollectionAssert.AreEqual(new long[] { 3, 4, 5 }, server.Parts.Skip(sentBefore).Select(part => part.Number).ToArray(), "Only the parts not accepted are sent.");
                JsonElement resumed = server.NegotiationsOf("again")[0];
                Assert.AreEqual("ref-remote.bin", resumed.GetProperty("ref").GetString(), "The run continues the upload it started.");
                Assert.AreEqual(3, resumed.GetProperty("part").GetInt64());
                Assert.AreEqual("ref-remote.bin", server.Finishes.Single().GetProperty("ref").GetString());
                Assert.AreEqual("\"etag-remote.bin-1-1\"", EtagOf(server.Finishes.Single(), 1), "Parts accepted before are completed with their ETags.");
                Assert.AreEqual("\"etag-remote.bin-3-2\"", EtagOf(server.Finishes.Single(), 3));
                AssertExactUpload(server, result, contents);
                Assert.AreEqual(2, result.ResumedParts);
                Assert.AreEqual(2 * KiB, result.ResumedBytes);
                CollectionAssert.AreEqual(new[] { 0, 0, 1, 1, 1 }, result.Parts.Select(part => part.Attempts).ToArray());
                Assert.AreEqual(contents.Length, result.FinalProgress.AcknowledgedBytes);
                Assert.IsFalse(File.Exists(checkpoint), "A completed upload's checkpoint is deleted.");
            }
        }

        // An upload sent with Files.com's sizes goes on with them wherever its parts now go: the limits and the S3 upload
        // matter only to an upload that chose its sizes.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(true, DisplayName = "Its parts went straight to S3, and now go to Files.com")]
        [DataRow(false, DisplayName = "Its parts now go straight to S3")]
        public async Task UploadOnFilesComsSizesContinuesWhereverItsPartsNowGo(bool awsBefore)
        {
            using (var server = new UploadServer { ParallelParts = "false", PartSize = number => KiB })
            {
                UseAws(server, 1, FilesMaxFile, 10000, FilesMaxFile);
                server.Aws = awsBefore;
                byte[] contents = Pattern(5 * KiB);
                string localPath = WriteFile("local.bin", contents);
                string checkpoint = Path.Combine(directory, "upload.checkpoint");
                var options = new UploadOptions { CheckpointPath = checkpoint, PartSizingMode = UploadPartSizingMode.ServerPlanned };

                await CancelAtPart(server, localPath, options, 3);
                server.Aws = !awsBefore;
                int sentBefore = server.Parts.Count;

                UploadResult result = await Within(Client(server).Transfers.UploadFileAsync(localPath, "remote.bin", options));

                CollectionAssert.AreEqual(new long[] { 3, 4, 5 }, server.Parts.Skip(sentBefore).Select(part => part.Number).ToArray(), "Only the parts not accepted are sent.");
                Assert.AreEqual("ref-remote.bin", server.Finishes.Single().GetProperty("ref").GetString(), "The run continues the upload it started.");
                Assert.AreEqual(2, result.ResumedParts);
                Assert.AreEqual(PartSizeSource.ServerPlanned, result.FinalProgress.PartSizing.Source);
                AssertExactUpload(server, result, contents);
            }
        }

        // An Agent/FIW upload may continue beyond S3's part-number limit. Seed the state of an earlier run from an
        // actual checkpoint so only the one new part is sent, rather than making 10,000 requests in this test.
        [TestMethod]
        [Timeout(60000)]
        public async Task AgentUploadContinuesBeyondTenThousandAcceptedParts()
        {
            using (var server = new UploadServer { ParallelParts = "false", PartSize = number => 1, ExtraPartFields = DirectConnection })
            {
                byte[] contents = Pattern(10001);
                string localPath = WriteFile("local.bin", contents);
                string checkpoint = Path.Combine(directory, "upload.checkpoint");
                var options = new UploadOptions { CheckpointPath = checkpoint };
                await CancelAtPart(server, localPath, options, 2);

                JsonNode recorded = JsonNode.Parse(File.ReadAllText(checkpoint));
                var planned = new JsonArray();
                var accepted = new JsonArray();
                string[] hashes = Enumerable.Range(0, 256).Select(value => UploadCoordinator.Hex(System.Security.Cryptography.SHA256.HashData(new[] { (byte)value }))).ToArray();
                for (int i = 0; i < 10000; i++)
                {
                    planned.Add(new JsonArray(i + 1, i, 1, 0));
                    accepted.Add(new JsonObject { ["part"] = i + 1, ["etag"] = $"\"earlier-part-{i + 1}\"", ["sha256"] = hashes[contents[i]] });
                }
                recorded["planned"] = planned;
                recorded["accepted"] = accepted;
                File.WriteAllText(checkpoint, recorded.ToJsonString());
                int sentBefore = server.Parts.Count;

                UploadResult result = await Within(server.Client(run: "again").Transfers.UploadFileAsync(localPath, "remote.bin", options));

                PartRequest sent = server.Parts.Skip(sentBefore).Single();
                Assert.AreEqual(10001L, sent.Number);
                CollectionAssert.AreEqual(new[] { contents[10000] }, sent.Bytes);
                Assert.AreEqual(10000, result.ResumedParts);
                Assert.AreEqual(10000L, result.ResumedBytes);
                Assert.AreEqual(UploadPerformanceProfile.Agent, result.FinalProgress.Concurrency.Profile);
                Assert.AreEqual(PartSizeSource.ServerPlanned, result.FinalProgress.PartSizing.Source);
                Assert.AreEqual(1, server.NegotiationsOf("again").Count, "Only the new part was negotiated.");
                Assert.AreEqual(10001, server.Finishes.Single().GetProperty("etags").GetArrayLength());
                Assert.IsFalse(File.Exists(checkpoint), "The upload completed and its checkpoint was removed.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task InterruptedUploadContinuesFromWhatItsCheckpointLastRecorded()
        {
            using (var server = new UploadServer { ParallelParts = "false", PartSize = number => KiB })
            {
                byte[] contents = Pattern(5 * KiB);
                string localPath = WriteFile("local.bin", contents);
                string checkpoint = Path.Combine(directory, "upload.checkpoint");
                var options = new UploadOptions { CheckpointPath = checkpoint };
                string onDisk = null;
                var cancellation = new CancellationTokenSource();
                server.RespondToPart = async request =>
                {
                    if (request.Number == 4)
                    {
                        // What a process that ended now would leave behind.
                        onDisk = File.ReadAllText(checkpoint);
                        cancellation.Cancel();
                        await Task.Delay(100);
                        return LoopbackSite.Status(503);
                    }
                    return UploadServer.Accept(request);
                };
                await ThrowsAsync<OperationCanceledException>(Client(server).Transfers.UploadFileAsync(localPath, "remote.bin", options, cancellation.Token));
                File.WriteAllText(checkpoint, onDisk);
                long[] recorded = JsonDocument.Parse(onDisk).RootElement.GetProperty("accepted").EnumerateArray().Select(part => part.GetProperty("part").GetInt64()).ToArray();
                server.RespondToPart = request => Task.FromResult(UploadServer.Accept(request));
                int sentBefore = server.Parts.Count;

                UploadResult result = await Within(Client(server).Transfers.UploadFileAsync(localPath, "remote.bin", options));

                long[] resent = server.Parts.Skip(sentBefore).Select(part => part.Number).ToArray();
                CollectionAssert.AreEqual(Enumerable.Range(1, 5).Select(n => (long)n).Except(recorded).ToArray(), resent, "Every part the checkpoint does not record as accepted is sent again, and only those.");
                Assert.AreEqual(recorded.Length, result.ResumedParts);
                Assert.AreEqual("ref-remote.bin", server.Finishes.Single().GetProperty("ref").GetString());
                JsonElement finish = server.Finishes.Single();
                foreach (UploadedPart part in result.Parts)
                {
                    PartRequest last = server.Parts.Last(request => request.Number == part.Number && request.Accepted);
                    Assert.AreEqual($"\"etag-remote.bin-{part.Number}-{last.Attempt}\"", EtagOf(finish, part.Number), $"Part {part.Number} is completed with the ETag of its last accepted attempt.");
                    CollectionAssert.AreEqual(contents.Skip((int)part.Offset).Take((int)part.Length).ToArray(), last.Bytes);
                }
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("contents", TransferCheckpointProblem.SourceChanged, DisplayName = "The file has changed")]
        [DataRow("same-size-same-time", TransferCheckpointProblem.SourceChanged, DisplayName = "The file has changed, keeping its length and modification time")]
        [DataRow("length", TransferCheckpointProblem.SourceChanged, DisplayName = "The file has another length")]
        [DataRow("gone", TransferCheckpointProblem.UploadExpired, DisplayName = "Files.com no longer knows the upload")]
        [DataRow("limits", TransferCheckpointProblem.GrantChanged, DisplayName = "Its parts now go straight to S3, where the upload chooses part sizes")]
        [DataRow("another s3 upload", TransferCheckpointProblem.GrantChanged, DisplayName = "Its parts, whose sizes it chose, now go to another S3 upload")]
        [DataRow("an oversized last part", TransferCheckpointProblem.GrantChanged, DisplayName = "It records a last part, whose size it chose, larger than the limits allow")]
        public async Task UploadThatCannotBeContinuedFailsUnlessTheCallerChoseToStartOver(string change, TransferCheckpointProblem problem)
        {
            using (var server = new UploadServer { ParallelParts = "false", PartSize = number => KiB })
            {
                byte[] contents = Pattern(5 * KiB);
                string localPath = WriteFile("local.bin", contents);
                string checkpoint = Path.Combine(directory, "upload.checkpoint");
                var options = new UploadOptions { CheckpointPath = checkpoint };
                if (change == "another s3 upload" || change == "an oversized last part")
                {
                    // Parts of at most 2 KiB for an oversized last part, so that the 5 KiB file is larger than any part.
                    UseAws(server, 1, change == "an oversized last part" ? 2 * KiB : FilesMaxFile, 10000, FilesMaxFile);
                    options.AutomaticPartSizing = GrowingFrom(KiB, max: KiB);
                }
                await CancelAtPart(server, localPath, options, 3);
                DateTime modified = File.GetLastWriteTimeUtc(localPath);
                switch (change)
                {
                    case "contents":
                        contents = Pattern(5 * KiB, seed: 2);
                        File.WriteAllBytes(localPath, contents);
                        File.SetLastWriteTimeUtc(localPath, DateTime.UtcNow.AddMinutes(1));
                        break;
                    case "same-size-same-time":
                        // A change inside a part already accepted, as a tool that restores timestamps makes.
                        contents[KiB + 1] ^= 0xFF;
                        File.WriteAllBytes(localPath, contents);
                        File.SetLastWriteTimeUtc(localPath, modified);
                        break;
                    case "length":
                        contents = contents.Concat(new byte[] { 7 }).ToArray();
                        File.WriteAllBytes(localPath, contents);
                        break;
                    case "gone":
                        server.RefuseNegotiation = body => body.TryGetProperty("ref", out JsonElement reference) && reference.GetString() == "ref-remote.bin"
                            ? LoopbackSite.Json(UploadGone, status: 404)
                            : null;
                        break;
                    case "limits":
                        UseAws(server, 1, FilesMaxFile, 10000, FilesMaxFile);
                        options.AutomaticPartSizing = GrowingFrom(KiB, max: KiB);
                        break;
                    case "another s3 upload":
                        server.UploadUri = (file, number, negotiation) => UploadServer.AwsPartUrl(file, number, negotiation, uploadId: "up~2");
                        break;
                    case "an oversized last part":
                        // What an earlier version recorded for a fixed part size no smaller than the file: all of it as one
                        // last part, not yet accepted, larger than the limits allow. This run sizes its parts automatically,
                        // within the limits, but a recorded part is sent as it was planned.
                        Rewrite(checkpoint, "planned", new JsonArray(new JsonArray(1, 0, contents.Length, 1)));
                        Rewrite(checkpoint, "accepted", new JsonArray());
                        break;
                }
                int sentBefore = server.Parts.Count;
                string recorded = File.ReadAllText(checkpoint);

                // Its own negotiations are known by its run: the cancelled run may still have had one on its way.
                TransferCheckpointException refused = await ThrowsAsync<TransferCheckpointException>(server.Client(run: "again").Transfers.UploadFileAsync(localPath, "remote.bin", options));

                Assert.AreEqual(problem, refused.Problem);
                StringAssert.Contains(refused.Message, "CheckpointRestartPolicy");
                Assert.AreEqual(sentBefore, server.Parts.Count, "No part is sent.");
                Assert.AreEqual(0, server.NegotiationsOf("again").Count(n => !n.TryGetProperty("ref", out _)), "No new upload is begun.");
                Assert.AreEqual(problem == TransferCheckpointProblem.SourceChanged ? 0 : 1, server.NegotiationsOf("again").Count,
                    "A changed file is found before anything is sent; otherwise the upload is asked about once.");
                Assert.AreEqual(0, server.Finishes.Count);
                Assert.AreEqual(recorded, File.ReadAllText(checkpoint), "The checkpoint is left as it was.");

                options.CheckpointRestartPolicy = CheckpointRestartPolicy.StartOver;
                sentBefore = server.Parts.Count;
                UploadResult result = await Within(Client(server).Transfers.UploadFileAsync(localPath, "remote.bin", options));

                PartRequest[] sent = server.Parts.Skip(sentBefore).ToArray();
                CollectionAssert.AreEqual(Enumerable.Range(1, (contents.Length + KiB - 1) / KiB).Select(n => (long)n).ToArray(), sent.Select(part => part.Number).ToArray(), "Asked to, every part is sent for a new upload.");
                CollectionAssert.AreEqual(contents, sent.SelectMany(part => part.Bytes).ToArray());
                JsonElement finish = server.Finishes.Single();
                Assert.AreEqual("ref-remote.bin-2", finish.GetProperty("ref").GetString(), "The new upload is completed, with none of the old one's parts.");
                foreach (PartRequest part in sent)
                {
                    Assert.AreEqual($"\"etag-remote.bin-{part.Number}-{part.Attempt}\"", EtagOf(finish, part.Number));
                }
                Assert.AreEqual(0, result.ResumedParts);
                Assert.AreEqual(contents.Length, result.FinalProgress.AcknowledgedBytes, "What the old upload had sent no longer counts.");
                Assert.IsFalse(File.Exists(checkpoint));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task UploadContinuesWhenOnlyTheFilesModificationTimeChanged()
        {
            using (var server = new UploadServer { ParallelParts = "false", PartSize = number => KiB })
            {
                byte[] contents = Pattern(5 * KiB);
                string localPath = WriteFile("local.bin", contents);
                string checkpoint = Path.Combine(directory, "upload.checkpoint");
                var options = new UploadOptions { CheckpointPath = checkpoint };
                await CancelAtPart(server, localPath, options, 3);
                File.SetLastWriteTimeUtc(localPath, DateTime.UtcNow.AddHours(1));
                int sentBefore = server.Parts.Count;

                UploadResult result = await Within(Client(server).Transfers.UploadFileAsync(localPath, "remote.bin", options));

                CollectionAssert.AreEqual(new long[] { 3, 4, 5 }, server.Parts.Skip(sentBefore).Select(part => part.Number).ToArray(), "The accepted parts still have the bytes that were sent.");
                Assert.AreEqual(2, result.ResumedParts);
                Assert.AreEqual("ref-remote.bin", server.Finishes.Single().GetProperty("ref").GetString());
            }
        }

        // A final request that fails after it was sent, even with an error such as a locked destination, which Files.com
        // can answer after completing the file in storage, stays recorded as sent: the next run sends nothing.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("unavailable", DisplayName = "Answered 503")]
        [DataRow("locked", DisplayName = "Answered with an error")]
        public async Task FinalRequestRecordedAsSentIsNeverSentAgain(string answer)
        {
            using (var server = new UploadServer { PartSize = number => KiB })
            {
                string localPath = WriteFile("local.bin", Pattern(3 * KiB));
                string checkpoint = Path.Combine(directory, "upload.checkpoint");
                var options = new UploadOptions { CheckpointPath = checkpoint };
                string onDisk = null;
                server.RespondToFinish = body =>
                {
                    onDisk = File.ReadAllText(checkpoint);
                    return Task.FromResult(answer == "locked" ? LoopbackSite.Json(DestinationLocked, status: 423) : LoopbackSite.Status(503));
                };

                UploadCompletionUnknownException first = await ThrowsAsync<UploadCompletionUnknownException>(Client(server).Transfers.UploadFileAsync(localPath, "remote.bin", options));

                Assert.IsTrue(JsonDocument.Parse(onDisk).RootElement.GetProperty("final_request_sent").GetBoolean(), "That the final request was sent is recorded before it arrives.");
                Assert.IsTrue(((FilesCom.SdkException)first.InnerException).RequestMayHaveBeenProcessed);
                Assert.IsTrue(JsonDocument.Parse(File.ReadAllText(checkpoint)).RootElement.GetProperty("final_request_sent").GetBoolean(), "It stays recorded as sent.");
                int requestsBefore = server.Site.Requests.Count;

                UploadCompletionUnknownException error = await ThrowsAsync<UploadCompletionUnknownException>(Client(server).Transfers.UploadFileAsync(localPath, "remote.bin", options));

                Assert.AreEqual(requestsBefore, server.Site.Requests.Count, "Nothing is sent.");
                StringAssert.Contains(error.Message, checkpoint);
                Assert.AreEqual("remote.bin", error.Path);
                Assert.AreEqual(3 * KiB, error.Size);
                CollectionAssert.AreEqual(new[] { "\"etag-remote.bin-1-1\"", "\"etag-remote.bin-2-1\"", "\"etag-remote.bin-3-1\"" }, error.Parts.Select(part => part.ETag).ToArray());
                Assert.IsTrue(File.Exists(checkpoint), "The checkpoint is kept until the caller has checked the file.");
            }
        }

        // Files.com's trusted answer that the site is served from another region comes before the request reaches the
        // application, so the final request it answers was not carried out, and the next run sends it again. The
        // final request, sent without retries, does not move.
        [TestMethod]
        [Timeout(60000)]
        public async Task RefusedFinalRequestIsSentAgainByTheNextRunWithoutResendingParts()
        {
            using (var server = new UploadServer { PartSize = number => KiB })
            {
                byte[] contents = Pattern(3 * KiB);
                string localPath = WriteFile("local.bin", contents);
                string checkpoint = Path.Combine(directory, "upload.checkpoint");
                var options = new UploadOptions { CheckpointPath = checkpoint };
                server.RespondToFinish = body => Task.FromResult(LoopbackSite.Json(RegionMismatch, status: 403));
                FilesCom.LockoutRegionMismatchException refused = await ThrowsAsync<FilesCom.LockoutRegionMismatchException>(RelocatingClient(server).Transfers.UploadFileAsync(localPath, "remote.bin", options));
                Assert.AreEqual(FilesCom.RegionRelocationRefusal.RetriesExhausted, refused.Refusal);
                Assert.IsFalse(refused.RequestMayHaveBeenProcessed);
                Assert.IsFalse(JsonDocument.Parse(File.ReadAllText(checkpoint)).RootElement.GetProperty("final_request_sent").GetBoolean(), "Files.com refused the request, so the upload was not completed.");
                server.RespondToFinish = null;
                int sentBefore = server.Parts.Count;
                int negotiatedBefore = server.Negotiations.Count;

                UploadResult result = await Within(RelocatingClient(server).Transfers.UploadFileAsync(localPath, "remote.bin", options));

                Assert.AreEqual(sentBefore, server.Parts.Count, "No part is sent again.");
                JsonElement asked = server.Negotiations.Skip(negotiatedBefore).Single();
                Assert.AreEqual("ref-remote.bin", asked.GetProperty("ref").GetString());
                Assert.AreEqual(3, asked.GetProperty("part").GetInt64(), "Only the last part is asked about, and it is not sent.");
                Assert.AreEqual(2, server.Finishes.Count);
                Assert.AreEqual(server.Finishes[0].GetRawText(), server.Finishes[1].GetRawText(), "The same parts are completed.");
                Assert.AreEqual(3, result.ResumedParts);
                Assert.AreEqual(contents.Length, result.Size);
                Assert.IsFalse(File.Exists(checkpoint));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("remote", DisplayName = "Another remote path")]
        [DataRow("local", DisplayName = "Another local file")]
        [DataRow("workspace", DisplayName = "Another workspace")]
        [DataRow("site", DisplayName = "Another site")]
        [DataRow("download", DisplayName = "A download's checkpoint")]
        [DataRow("garbage", DisplayName = "Not a checkpoint")]
        [DataRow("version", DisplayName = "A later checkpoint version")]
        public async Task CheckpointOfAnotherTransferIsRefusedBeforeAnythingIsSent(string other)
        {
            using (var server = new UploadServer { ParallelParts = "false", PartSize = number => KiB })
            using (var elsewhere = new UploadServer { PartSize = number => KiB })
            {
                string localPath = WriteFile("local.bin", Pattern(5 * KiB));
                string checkpoint = Path.Combine(directory, "upload.checkpoint");
                var options = new UploadOptions { CheckpointPath = checkpoint };
                await CancelAtPart(server, localPath, options, 3);
                string remotePath = "remote.bin";
                UploadServer site = server;
                switch (other)
                {
                    case "remote":
                        remotePath = "other.bin";
                        break;
                    case "local":
                        localPath = WriteFile("copy.bin", File.ReadAllBytes(localPath));
                        File.SetLastWriteTimeUtc(localPath, File.GetLastWriteTimeUtc(Path.Combine(directory, "local.bin")));
                        break;
                    case "workspace":
                        options.RequestOptions = new Dictionary<string, object> { { "workspace_id", "12" } };
                        break;
                    case "site":
                        site = elsewhere;
                        break;
                    case "download":
                        Rewrite(checkpoint, "kind", "download");
                        break;
                    case "garbage":
                        File.WriteAllText(checkpoint, "{\"not\":\"a checkpoint\"}");
                        break;
                    case "version":
                        Rewrite(checkpoint, "version", 2);
                        break;
                }
                string before = File.ReadAllText(checkpoint);
                int requestsBefore = server.Site.Requests.Count;

                TransferCheckpointException error = await ThrowsAsync<TransferCheckpointException>(Client(site).Transfers.UploadFileAsync(localPath, remotePath, options));

                Assert.AreEqual(checkpoint, error.CheckpointPath);
                Assert.AreEqual(other == "garbage" || other == "version" ? TransferCheckpointProblem.Unreadable : TransferCheckpointProblem.OtherTransfer, error.Problem);
                Assert.AreEqual(requestsBefore, server.Site.Requests.Count, "Nothing is sent.");
                Assert.AreEqual(0, elsewhere.Site.Requests.Count);
                Assert.AreEqual(before, File.ReadAllText(checkpoint), "The checkpoint is left as it was.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("caller", DisplayName = "The caller's own failure")]
        [DataRow("refusal", DisplayName = "Files.com's refusal to complete the upload")]
        public async Task CheckpointFailureAfterAStopLeavesTheStopsOwnFailure(string stop)
        {
            using (var server = new UploadServer { ParallelParts = "false", PartSize = number => KiB })
            using (var logs = new global::FilesTests.LogCapture())
            using (var injection = new FailureAtTwoParts())
            {
                string localPath = WriteFile("local.bin", Pattern(3 * KiB));
                string checkpoints = Path.Combine(directory, "checkpoints");
                Directory.CreateDirectory(checkpoints);
                var options = new UploadOptions { CheckpointPath = Path.Combine(checkpoints, "upload.checkpoint") };
                var failure = new IOException("The caller's progress handler failed.");
                if (stop == "caller")
                {
                    injection.Gate(server);
                    options.Progress = injection.Progress(() =>
                    {
                        Directory.Move(checkpoints, checkpoints + "-moved");
                        throw failure;
                    });
                }
                else
                {
                    server.RespondToFinish = body =>
                    {
                        Directory.Move(checkpoints, checkpoints + "-moved");
                        return Task.FromResult(LoopbackSite.Json(RegionMismatch, status: 403));
                    };
                }

                Exception error = await ThrowsAsync<Exception>(RelocatingClient(server).Transfers.UploadFileAsync(localPath, "remote.bin", options), injection.Describe);

                if (stop == "caller")
                {
                    Assert.AreSame(failure, error, "The caller's own exception, not the checkpoint's.");
                }
                else
                {
                    Assert.IsInstanceOfType(error, typeof(FilesCom.LockoutRegionMismatchException), "Files.com's refusal, not the checkpoint's failure.");
                    StringAssert.Contains(error.Message, "different region");
                }
                StringAssert.Contains(logs.Normal, "Recording the checkpoint of a stopped upload failed", "The checkpoint's failure is still reported.");
                StringAssert.Contains(logs.Normal, "DirectoryNotFoundException");
                Assert.IsFalse(logs.Normal.Contains(checkpoints), "Ordinary logs name its type, not its path.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("factory", DisplayName = "The logger factory throws")]
        [DataRow("enabled", DisplayName = "IsEnabled throws")]
        [DataRow("log", DisplayName = "Log throws")]
        public async Task ALoggerThatThrowsLeavesTheStopsOwnFailure(string where)
        {
            using (var server = new UploadServer { ParallelParts = "false", PartSize = number => KiB })
            using (var injection = new FailureAtTwoParts())
            {
                string localPath = WriteFile("local.bin", Pattern(3 * KiB));
                string checkpoints = Path.Combine(directory, "checkpoints");
                Directory.CreateDirectory(checkpoints);
                var failure = new IOException("The caller's progress handler failed.");
                injection.Gate(server);
                var options = new UploadOptions
                {
                    CheckpointPath = Path.Combine(checkpoints, "upload.checkpoint"),
                    Progress = injection.Progress(() =>
                    {
                        // The checkpoint cannot be recorded now, and its failure is logged on the way out.
                        Directory.Move(checkpoints, checkpoints + "-moved");
                        throw failure;
                    }),
                };
                FilesCom.FilesConfiguration configuration = server.Site.Configuration();
                configuration.LoggerFactory = new ThrowingLoggerFactory(where);

                Exception error = await ThrowsAsync<Exception>(FilesCom.FilesClient.Create(configuration).Transfers.UploadFileAsync(localPath, "remote.bin", options), injection.Describe);

                Assert.AreSame(failure, error, "The caller's own exception, not the logger's: " + error);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task AcceptedPartsAreReadAgainWithinTheLimitersBufferSpace()
        {
            using (var server = new UploadServer { ParallelParts = "false", PartSize = number => KiB })
            {
                string localPath = WriteFile("local.bin", Pattern(5 * KiB));
                var limiter = new TransferLimiter(maxConcurrentRequests: 2, maxBufferedBytes: KiB);
                var options = new UploadOptions { CheckpointPath = Path.Combine(directory, "upload.checkpoint"), Limiter = limiter };
                await CancelAtPart(server, localPath, options, 3);
                int requestsBefore = server.Site.Requests.Count;
                IDisposable held = await limiter.ReserveBufferAsync(KiB, CancellationToken.None);

                Task<UploadResult> upload = Client(server).Transfers.UploadFileAsync(localPath, "remote.bin", options);
                while (limiter.QueuedBufferReservations == 0)
                {
                    Assert.IsFalse(upload.IsCompleted, "The accepted parts were read again without buffer space from the limiter: " + upload.Status);
                    await Task.Delay(5);
                }

                Assert.AreEqual(requestsBefore, server.Site.Requests.Count, "The run waits for buffer space to read the parts again, before anything is sent.");
                held.Dispose();
                UploadResult result = await Within(upload);
                Assert.AreEqual(2, result.ResumedParts);
                Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes + limiter.QueuedBufferReservations);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task ACheckpointIsNotReliedOnWhereItsDirectoryCannotBeFlushed()
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("Directories are flushed on macOS and Linux only.");
                return;
            }
            using (var server = new UploadServer { ParallelParts = "false", PartSize = number => KiB })
            {
                string localPath = WriteFile("local.bin", Pattern(3 * KiB));
                var options = new UploadOptions { CheckpointPath = Path.Combine(directory, "upload.checkpoint") };
                Native.FlushFaults.Value = call => call == Native.FlushCall.FullSyncDirectory || call == Native.FlushCall.SyncDirectory ? 22 : 0;
                IOException error;
                try
                {
                    error = await ThrowsAsync<IOException>(Client(server).Transfers.UploadFileAsync(localPath, "remote.bin", options));
                }
                finally
                {
                    Native.FlushFaults.Value = null;
                }

                StringAssert.Contains(error.Message, "cannot flush a directory");
                Assert.AreEqual(0, server.Parts.Count, "The first checkpoint fails, before any part is sent.");
                Assert.AreEqual(0, server.Finishes.Count);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task TheFinalRequestIsNotSentUnlessItsRecordIsDurable()
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("Directories are flushed on macOS and Linux only.");
                return;
            }
            using (var server = new UploadServer { ParallelParts = "false", PartSize = number => KiB })
            {
                string localPath = WriteFile("local.bin", Pattern(3 * KiB));
                string checkpoint = Path.Combine(directory, "upload.checkpoint");
                int partsAccepted = 0;
                var options = new UploadOptions
                {
                    CheckpointPath = checkpoint,
                    Progress = new FailingProgress(progress =>
                    {
                        if (progress.AcknowledgedBytes == 3 * KiB)
                        {
                            Volatile.Write(ref partsAccepted, 1);
                        }
                    }),
                };
                // Once every part is accepted, the next flush of the checkpoint's directory, the final record's, fails.
                Native.FlushFaults.Value = call => Volatile.Read(ref partsAccepted) == 1 && (call == Native.FlushCall.FullSyncDirectory || call == Native.FlushCall.SyncDirectory) ? 5 : 0;
                IOException error;
                try
                {
                    error = await ThrowsAsync<IOException>(Client(server).Transfers.UploadFileAsync(localPath, "remote.bin", options));
                }
                finally
                {
                    Native.FlushFaults.Value = null;
                }

                StringAssert.Contains(error.Message, "its final request was not sent");
                Assert.AreEqual(0, server.Finishes.Count, "It was not sent.");
                Assert.IsTrue(JsonDocument.Parse(File.ReadAllText(checkpoint)).RootElement.GetProperty("final_request_sent").GetBoolean(), "The checkpoint in place records it as maybe sent,");
                await ThrowsAsync<UploadCompletionUnknownException>(Client(server).Transfers.UploadFileAsync(localPath, "remote.bin", options));
                Assert.AreEqual(0, server.Finishes.Count, "so no later run sends it.");
            }
        }

        [TestMethod]
        public void StreamUploadCannotHaveACheckpoint()
        {
            using (var server = new UploadServer())
            {
                var options = new UploadOptions { CheckpointPath = Path.Combine(directory, "upload.checkpoint") };

                Assert.AreEqual(nameof(UploadOptions.CheckpointPath), Assert.ThrowsExactly<FilesCom.InvalidParameterException>(() => Client(server).Transfers.UploadStreamAsync(new MemoryStream(new byte[4]), 4, "remote.bin", options)).ParamName);
                Assert.AreEqual(nameof(UploadOptions.CheckpointPath), Assert.ThrowsExactly<FilesCom.InvalidParameterException>(() => Client(server).Transfers.UploadStreamAsync(new MemoryStream(new byte[4]), "remote.bin", options)).ParamName);

                Assert.AreEqual(0, server.Site.Requests.Count);
                Assert.IsFalse(File.Exists(options.CheckpointPath));
            }
        }

        // Fails a serial upload of three 1 KiB parts from its progress handler exactly once, at the report of 2 KiB
        // acknowledged. Sending is serial, but the next part may be sent as soon as the previous one is accepted, before
        // that acceptance is reported, so the third part could be accepted before the report of the second takes its
        // snapshot, which then shows 3 KiB and is never 2 KiB. Gate holds the third part's answer, and once the failure
        // is injected keeps holding it until the test disposes the helper, after the upload has stopped: accepted while
        // the injected failure is still on its way to the upload, the third part could save a checkpoint that had come
        // due into the moved folder, and that failure, not the injected one, could be the upload's first. If the 2 KiB
        // report never comes, the answer goes after ten seconds, so that a missed phase fails the test instead of
        // hanging it. The handler injects at most once, so that a later report of the same total cannot move the folder
        // or throw again.
        private sealed class FailureAtTwoParts : IDisposable
        {
            private readonly TaskCompletionSource<bool> injected = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            private readonly TaskCompletionSource<bool> released = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            private readonly List<long> reported = new List<long>();
            private int fired;

            public void Gate(UploadServer server)
            {
                server.RespondToPart = async request =>
                {
                    if (request.Number == 3)
                    {
                        // Until the failure is injected, at most ten seconds; once it is, until the upload has stopped.
                        if (await Task.WhenAny(injected.Task, Task.Delay(TimeSpan.FromSeconds(10))) == injected.Task)
                        {
                            await released.Task;
                        }
                    }
                    return UploadServer.Accept(request);
                };
            }

            // Releases the third part's answer, once the upload has stopped and can no longer accept it.
            public void Dispose()
            {
                released.TrySetResult(true);
            }

            public FailingProgress Progress(Action inject)
            {
                return new FailingProgress(progress =>
                {
                    lock (reported)
                    {
                        reported.Add(progress.AcknowledgedBytes);
                    }
                    if (progress.AcknowledgedBytes == 2 * KiB && Interlocked.Exchange(ref fired, 1) == 0)
                    {
                        try
                        {
                            inject();
                        }
                        finally
                        {
                            injected.TrySetResult(true);
                        }
                    }
                });
            }

            // The acknowledged bytes of every report, for failure messages.
            public string Describe()
            {
                lock (reported)
                {
                    return $"Reports acknowledged {string.Join(", ", reported)} bytes; the failure was {(injected.Task.IsCompleted ? "" : "not ")}injected.";
                }
            }
        }

        private sealed class FailingProgress : IProgress<TransferProgress>
        {
            private readonly Action<TransferProgress> report;

            internal FailingProgress(Action<TransferProgress> report)
            {
                this.report = report;
            }

            public void Report(TransferProgress value)
            {
                report(value);
            }
        }

        // An application's logger factory that fails: where says whether creating a logger, asking whether a level is
        // enabled, or logging throws.
        private sealed class ThrowingLoggerFactory : Microsoft.Extensions.Logging.ILoggerFactory
        {
            private readonly string where;

            internal ThrowingLoggerFactory(string where)
            {
                this.where = where;
            }

            public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
            {
                if (where == "factory")
                {
                    throw new InvalidOperationException("The logger factory failed.");
                }
                return new Logger(where);
            }

            public void AddProvider(Microsoft.Extensions.Logging.ILoggerProvider provider)
            {
            }

            public void Dispose()
            {
            }

            private sealed class Logger : Microsoft.Extensions.Logging.ILogger
            {
                private readonly string where;

                internal Logger(string where)
                {
                    this.where = where;
                }

                public IDisposable BeginScope<TState>(TState state) where TState : notnull
                {
                    return null;
                }

                public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
                {
                    if (where == "enabled")
                    {
                        throw new InvalidOperationException("The logger failed.");
                    }
                    return true;
                }

                public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
                {
                    throw new InvalidOperationException("The logger failed.");
                }
            }
        }

        private static void Rewrite(string checkpoint, string name, JsonNode value)
        {
            JsonNode recorded = JsonNode.Parse(File.ReadAllText(checkpoint));
            recorded[name] = value;
            File.WriteAllText(checkpoint, recorded.ToJsonString());
        }

        // Runs the upload until part number arrives, then cancels it: parts before it are accepted, and it is not.
        private async Task CancelAtPart(UploadServer server, string localPath, UploadOptions options, long number)
        {
            var cancellation = new CancellationTokenSource();
            server.RespondToPart = async request =>
            {
                if (request.Number == number && request.Attempt == 1)
                {
                    cancellation.Cancel();
                    await Task.Delay(100);
                    return LoopbackSite.Status(503);
                }
                return UploadServer.Accept(request);
            };
            await ThrowsAsync<OperationCanceledException>(Client(server).Transfers.UploadFileAsync(localPath, "remote.bin", options, cancellation.Token));
            server.RespondToPart = request => Task.FromResult(UploadServer.Accept(request));
            Assert.IsTrue(File.Exists(options.CheckpointPath), "A cancelled upload keeps its checkpoint.");
            CollectionAssert.AreEqual(Enumerable.Range(1, (int)number - 1).Select(n => (long)n).ToArray(),
                JsonDocument.Parse(File.ReadAllText(options.CheckpointPath)).RootElement.GetProperty("accepted").EnumerateArray().Select(part => part.GetProperty("part").GetInt64()).ToArray(),
                "Once the upload has stopped, its checkpoint records every part accepted.");
        }
    }
}