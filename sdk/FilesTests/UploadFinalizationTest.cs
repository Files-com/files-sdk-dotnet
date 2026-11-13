using FilesCom;
using FilesCom.Transfers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WireMock;

namespace FilesTests
{
    // Uploads whose final request Files.com answers, as it may for a remote server mount or a Files.com Agent, that it is
    // completing the file in the background (HTTP 202 {"status": "pending", "ref": ...}): the upload asks again, with the
    // same request, until the file or an error comes, and what it reports and throws meanwhile.
    public partial class ParallelUploadTest
    {
        [TestMethod]
        [Timeout(60000)]
        public async Task AnUploadFilesComCompletesInTheBackgroundAsksAgainWithTheSameRequestUntilItsFileComes()
        {
            using (var server = new UploadServer())
            using (var waits = new FinalizationWaits())
            {
                server.RespondToFinish = InTurn(
                    body => UploadServer.Pending("ref-remote.bin"),
                    body => UploadServer.Pending("ref-remote.bin", "42"),
                    body => UploadServer.Pending("ref-remote.bin", "100"),
                    body => UploadServer.Pending("ref-remote.bin", "100"),
                    body => UploadServer.Completed(body));
                FilesConfiguration configuration = server.Site.Configuration();
                configuration.Language = "fr";
                // Each report, with the number of final requests the site had received when it was made.
                var reports = new List<(TransferProgress Report, int Finishes)>();
                var progress = new FailingProgress(report =>
                {
                    lock (reports)
                    {
                        reports.Add((report, server.Finishes.Count));
                    }
                });

                UploadResult result = await Within(server.Client(configuration).Transfers.UploadFileAsync(WriteFile("local.bin", Pattern(20)), "remote.bin", new UploadOptions { Progress = progress }));

                Assert.AreEqual(5, server.Finishes.Count, "Asked again after each pending answer, until the file came.");
                Assert.IsTrue(server.Finishes[0].GetProperty("async_finalize").GetBoolean(), "The final request asks Files.com to complete the file in the background.");
                Assert.AreEqual("ref-remote.bin", server.Finishes[0].GetProperty("ref").GetString());
                Assert.IsTrue(server.Finishes.All(finish => finish.GetRawText() == server.Finishes[0].GetRawText()), "Every request is the same request: " + string.Join(" | ", server.Finishes.Select(finish => finish.GetRawText())));
                ReceivedRequest[] finishes = server.Site.Requests.Where(request => request.Method == "POST" && request.Path.StartsWith("/files/")).ToArray();
                Assert.AreEqual(5, finishes.Length);
                foreach (ReceivedRequest finish in finishes)
                {
                    Assert.AreEqual("POST /files/remote.bin key=key-a workspace=11 fr", $"{finish} {finish.Language}", "The upload's own credentials, workspace and language, every time.");
                    Assert.AreEqual(finishes[0].Headers["User-Agent"], finish.Headers["User-Agent"]);
                }
#if DEBUG
                CollectionAssert.AreEqual(new[] { 1, 2, 3, 3 }.Select(seconds => TimeSpan.FromSeconds(seconds)).ToArray(), waits.Dues.ToArray(),
                    "One, two, then three seconds' wait after each pending answer, a pending 100 included.");
#endif

                TransferProgress lastBytes = reports.Last(entry => !entry.Report.Finalizing).Report;
                Assert.AreEqual(20, lastBytes.AcknowledgedBytes);
                Assert.AreEqual(0, reports.Count(entry => entry.Report.Finalizing && entry.Finishes == 0), "Nothing is finalizing before the final request.");
                (TransferProgress Report, int Finishes)[] finalizing = reports.SkipWhile(entry => !entry.Report.Finalizing).ToArray();
                Assert.AreEqual("1:none 2:42 3:100 4:100", string.Join(" ", finalizing.Select(entry => $"{entry.Finishes}:{entry.Report.FinalizationProgress?.ToString() ?? "none"}")),
                    "One report for each pending answer, once it came, with its progress; 100 is still pending.");
                foreach ((TransferProgress report, _) in finalizing)
                {
                    Assert.IsTrue(report.Finalizing);
                    Assert.AreEqual($"{lastBytes.TotalBytes}/{lastBytes.AcknowledgedBytes}/{lastBytes.SentBytes}/{lastBytes.PartsAcknowledged}", $"{report.TotalBytes}/{report.AcknowledgedBytes}/{report.SentBytes}/{report.PartsAcknowledged}", "The byte counts stay as they were.");
                }
                Assert.AreEqual("remote.bin", result.File.Path, "The file the last answer gave.");
                Assert.AreEqual(20, result.File.Size);
                Assert.IsFalse(result.FinalProgress.Finalizing, "Completed: no longer finalizing.");
                Assert.IsNull(result.FinalProgress.FinalizationProgress);

                // Another upload's finalization starts again from one second.
                int before = waits.Dues.Count;
                server.RespondToFinish = InTurn(body => UploadServer.Pending("ref-next.bin"), body => UploadServer.Completed(body));
                await Within(server.Client(configuration).Transfers.UploadFileAsync(WriteFile("next.bin", Pattern(20)), "next.bin"));
#if DEBUG
                CollectionAssert.AreEqual(new[] { TimeSpan.FromSeconds(1) }, waits.Dues.Skip(before).ToArray(), "The next upload's first wait.");
#endif
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("0", 0)]
        [DataRow("100", 100)]
        [DataRow("42.5", -1, DisplayName = "A fraction")]
        [DataRow("42.0", -1, DisplayName = "Not an integer literal")]
        [DataRow("\"42\"", -1, DisplayName = "A string")]
        [DataRow("101", -1, DisplayName = "Above 100")]
        [DataRow("-1", -1, DisplayName = "Below 0")]
        [DataRow("true", -1, DisplayName = "A boolean")]
        [DataRow("null", -1, DisplayName = "Null")]
        [DataRow("{}", -1, DisplayName = "An object")]
        public async Task APendingAnswersProgressCountsOnlyAsAnIntegerFrom0To100(string given, int expected)
        {
            using (var server = new UploadServer())
            using (new FinalizationWaits())
            {
                server.RespondToFinish = InTurn(body => UploadServer.Pending("ref-remote.bin", given), body => UploadServer.Completed(body));
                var progress = new ProgressLog();

                await Within(Client(server).Transfers.UploadFileAsync(WriteFile("local.bin", Pattern(20)), "remote.bin", new UploadOptions { Progress = progress }));

                TransferProgress finalizing = progress.Reports.Single(report => report.Finalizing);
                Assert.AreEqual(expected < 0 ? null : expected, finalizing.FinalizationProgress);
                Assert.AreEqual(2, server.Finishes.Count, "Any other value is no progress, not a failure.");
            }
        }

        // Once Files.com has accepted the final request with HTTP 202, an answer that is not the upload's pending completion
        // breaks the protocol: whether the file was completed is unknown, and nothing is sent again.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("[]", false, DisplayName = "Not an object")]
        [DataRow("{\"status\":\"completed\",\"ref\":\"ref-remote.bin\"}", false, DisplayName = "Not pending")]
        [DataRow("{\"status\":\"pending\"}", false, DisplayName = "Without a ref")]
        [DataRow("{\"status\":\"pending\",\"ref\":\"ref-other.bin\"}", false, DisplayName = "Another upload's ref")]
        [DataRow("{\"status\":\"pending\",\"ref\":7}", false, DisplayName = "A ref that is not a string")]
        [DataRow("pending", false, DisplayName = "Not JSON")]
        [DataRow("{\"status\":\"pending\",\"ref\":\"ref-other.bin\"}", true, DisplayName = "Another upload's ref, after a pending answer")]
        public async Task AnAcceptedFinalRequestWhoseAnswerIsNotItsPendingCompletionIsUnknownAndNotSentAgain(string answer, bool afterPending)
        {
            using (var server = new UploadServer())
            using (new FinalizationWaits())
            {
                Func<JsonElement, ResponseMessage> broken = body => LoopbackSite.Json(answer, status: 202);
                server.RespondToFinish = afterPending ? InTurn(body => UploadServer.Pending("ref-remote.bin", "10"), broken) : InTurn(broken);

                UploadCompletionUnknownException error = await ThrowsAsync<UploadCompletionUnknownException>(Client(server, retries: 3).Transfers.UploadFileAsync(WriteFile("local.bin", Pattern(20)), "remote.bin"));

                Assert.AreEqual(afterPending ? 2 : 1, server.Finishes.Count, "Not sent again after that answer.");
                Assert.IsInstanceOfType(error.InnerException, typeof(InvalidResponseException));
                Assert.AreEqual("remote.bin", error.Path);
                Assert.AreEqual("ref-remote.bin", error.Ref);
                Assert.AreEqual(3, error.Parts.Count);
                StringAssert.Contains(error.Message, "may still complete it");
            }
        }

        // Files.com keeps an upload it completes in the background, and its outcome, by the upload's ref: an error it answers
        // then is the upload's outcome, as Files.com gives it, and the upload was not completed, so its checkpoint no
        // longer records the final request as sent.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(422, "{\"error\":\"File not uploaded.\",\"type\":\"processing-failure/file-not-uploaded\",\"http-code\":422}", typeof(FileNotUploadedException), DisplayName = "An error Files.com kept for the upload")]
        [DataRow(404, UploadGone, typeof(FileUploadNotFoundException), DisplayName = "Not found")]
        public async Task AnErrorFilesComAnswersWhileCompletingTheFileIsTheUploadsOutcome(int status, string body, Type type)
        {
            using (var server = new UploadServer())
            using (new FinalizationWaits())
            {
                string checkpoint = Path.Combine(directory, "upload.checkpoint");
                server.RespondToFinish = InTurn(finish => UploadServer.Pending("ref-remote.bin"), finish => LoopbackSite.Json(body, status: status));

                ApiException error = await ThrowsAsync<ApiException>(Client(server, retries: 3).Transfers.UploadFileAsync(WriteFile("local.bin", Pattern(20)), "remote.bin", new UploadOptions { CheckpointPath = checkpoint }));

                Assert.AreEqual(type, error.GetType());
                Assert.AreEqual(status, error.httpStatus);
                Assert.AreEqual(2, server.Finishes.Count, "The error is not asked about again.");
                Assert.IsFalse(JsonDocument.Parse(File.ReadAllText(checkpoint)).RootElement.GetProperty("final_request_sent").GetBoolean());
            }
        }

        // A refusal of the client's credentials or access once Files.com is completing the file says nothing of the file,
        // which Files.com may still be completing or have completed, and has then forgotten the upload's ref. So the
        // checkpoint keeps the final request recorded as sent, and a later run, even one that may start over, sends
        // nothing: no begin_upload for the ref, which Files.com would answer as not found, no part and no new upload.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(401, "{\"error\":\"Authentication required.\",\"http-code\":401,\"type\":\"not-authenticated/authentication-required\"}", typeof(AuthenticationRequiredException), DisplayName = "401")]
        [DataRow(403, "{\"error\":\"Denied\",\"http-code\":403,\"type\":\"not-authorized/folder-admin-permission-required\"}", typeof(FolderAdminPermissionRequiredException), DisplayName = "403")]
        public async Task AnAccessRefusalWhileFilesComCompletesTheFileKeepsTheFinalRequestRecordedAsSent(int status, string body, Type type)
        {
            using (var server = new UploadServer())
            using (new FinalizationWaits())
            {
                string localPath = WriteFile("local.bin", Pattern(20));
                string checkpoint = Path.Combine(directory, "upload.checkpoint");
                var options = new UploadOptions { CheckpointPath = checkpoint, CheckpointRestartPolicy = CheckpointRestartPolicy.StartOver };
                server.RespondToFinish = InTurn(finish => UploadServer.Pending("ref-remote.bin", "42"), finish => LoopbackSite.Json(body, status: status));

                ApiException denied = await ThrowsAsync<ApiException>(Client(server).Transfers.UploadFileAsync(localPath, "remote.bin", options));

                Assert.AreEqual(type, denied.GetType());
                Assert.AreEqual(2, server.Finishes.Count);
                Assert.IsTrue(JsonDocument.Parse(File.ReadAllText(checkpoint)).RootElement.GetProperty("final_request_sent").GetBoolean(), "Still recorded as sent.");
                // As Files.com answers once it has completed the upload in the background: it no longer knows its ref, and
                // completes a new upload as any other.
                server.RespondToFinish = finish => Task.FromResult(UploadServer.Completed(finish));
                server.RefuseNegotiation = negotiation => negotiation.TryGetProperty("ref", out JsonElement reference) && reference.GetString() == "ref-remote.bin" ? LoopbackSite.Json(UploadGone, status: 404) : null;
                int requestsBefore = server.Site.Requests.Count;
                int negotiationsBefore = server.Negotiations.Count;
                int partsBefore = server.Parts.Count;

                UploadCompletionUnknownException error = await ThrowsAsync<UploadCompletionUnknownException>(Client(server).Transfers.UploadFileAsync(localPath, "remote.bin", options));

                Assert.AreEqual(requestsBefore, server.Site.Requests.Count, "Nothing is sent: " + string.Join(", ", server.Site.Requests.Skip(requestsBefore)));
                Assert.AreEqual(negotiationsBefore, server.Negotiations.Count, "No begin_upload.");
                Assert.AreEqual(partsBefore, server.Parts.Count, "No part is sent again.");
                Assert.AreEqual("ref-remote.bin", error.Ref);
                StringAssert.Contains(error.Message, checkpoint);
                Assert.IsTrue(JsonDocument.Parse(File.ReadAllText(checkpoint)).RootElement.GetProperty("final_request_sent").GetBoolean(), "Still recorded as sent.");
            }
        }

        // Asking again is safe once Files.com has accepted the final request, so a failure that may be temporary is retried,
        // as the client retries any request it can send twice; once no retry is left, or for an answer that cannot be read,
        // whether the file was completed is unknown.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("503", 2, 1, true, DisplayName = "A server error, retried")]
        [DataRow("429", 2, 1, true, DisplayName = "Throttled, retried")]
        [DataRow("503", 1, 2, false, DisplayName = "A server error, with no retry left")]
        [DataRow("unreadable", 3, 1, false, DisplayName = "An answer that cannot be read")]
        public async Task AFailureWhileFilesComCompletesTheFileIsRetriedAndThenLeavesTheOutcomeUnknown(string failure, int retries, int failures, bool completes)
        {
            using (var server = new UploadServer())
            using (new FinalizationWaits())
            {
                int asked = 0;
                server.RespondToFinish = body =>
                {
                    int number = Interlocked.Increment(ref asked);
                    if (number == 1)
                    {
                        return Task.FromResult(UploadServer.Pending("ref-remote.bin"));
                    }
                    if (number <= 1 + failures)
                    {
                        return Task.FromResult(failure == "unreadable" ? new ResponseMessage { FaultType = WireMock.ResponseBuilders.FaultType.EMPTY_RESPONSE } : LoopbackSite.Status(int.Parse(failure)));
                    }
                    return Task.FromResult(UploadServer.Completed(body));
                };
                Task<UploadResult> upload = Client(server, retries).Transfers.UploadFileAsync(WriteFile("local.bin", Pattern(20)), "remote.bin");

                if (completes)
                {
                    UploadResult result = await Within(upload);
                    Assert.AreEqual(20, result.File.Size);
                    Assert.AreEqual(3, server.Finishes.Count);
                }
                else
                {
                    UploadCompletionUnknownException error = await ThrowsAsync<UploadCompletionUnknownException>(upload);
                    Assert.AreEqual(1 + failures, server.Finishes.Count, "Asked again only as the client's retries allow.");
                    if (failure == "unreadable")
                    {
                        Assert.IsInstanceOfType(error.InnerException, typeof(InvalidResponseException));
                    }
                    else
                    {
                        Assert.AreEqual(503, ((ApiException)error.InnerException).httpStatus);
                    }
                    StringAssert.Contains(error.Message, "may still complete it");
                }
            }
        }

        // Cancelling stops the wait, not Files.com: the upload ends as cancelled, saying Files.com may still complete the
        // file, and its checkpoint keeps the final request recorded as sent, so a later run sends nothing.
        [TestMethod]
        [Timeout(60000)]
        public async Task CancellingTheWaitWhileFilesComCompletesTheFileKeepsTheFinalRequestRecordedAsSent()
        {
            using (var server = new UploadServer())
            using (var cancellation = new CancellationTokenSource())
            {
                string localPath = WriteFile("local.bin", Pattern(20));
                string checkpoint = Path.Combine(directory, "upload.checkpoint");
                server.RespondToFinish = body => Task.FromResult(UploadServer.Pending("ref-remote.bin", "42"));
                var options = new UploadOptions
                {
                    CheckpointPath = checkpoint,
                    Progress = new FailingProgress(report =>
                    {
                        if (report.Finalizing)
                        {
                            cancellation.Cancel();
                        }
                    }),
                };

                OperationCanceledException stopped = await ThrowsAsync<OperationCanceledException>(Client(server).Transfers.UploadFileAsync(localPath, "remote.bin", options, cancellation.Token));

                StringAssert.Contains(stopped.Message, "may still complete the file");
                Assert.AreEqual(1, server.Finishes.Count, "Not asked again once cancelled.");
                Assert.IsTrue(JsonDocument.Parse(File.ReadAllText(checkpoint)).RootElement.GetProperty("final_request_sent").GetBoolean(), "Still recorded as sent.");
                int requestsBefore = server.Site.Requests.Count;

                UploadCompletionUnknownException error = await ThrowsAsync<UploadCompletionUnknownException>(Client(server).Transfers.UploadFileAsync(localPath, "remote.bin", new UploadOptions { CheckpointPath = checkpoint }));

                Assert.AreEqual(requestsBefore, server.Site.Requests.Count, "Nothing is sent.");
                Assert.AreEqual("ref-remote.bin", error.Ref);
            }
        }

        // A progress handler that throws while Files.com completes the file stops the upload's wait, as it stops the upload
        // anywhere else: Files.com had accepted the final request, so whether the file was completed is unknown, whatever
        // the handler threw, even an exception that before the final request would show it was not sent.
        [TestMethod]
        [Timeout(60000)]
        public async Task AProgressHandlerThatThrowsWhileFilesComCompletesTheFileLeavesTheOutcomeUnknown()
        {
            using (var server = new UploadServer())
            {
                string checkpoint = Path.Combine(directory, "upload.checkpoint");
                server.RespondToFinish = body => Task.FromResult(UploadServer.Pending("ref-remote.bin"));
                var thrown = new ArgumentException("the handler's own failure");
                var options = new UploadOptions { CheckpointPath = checkpoint, Progress = new ProgressLog(report => report.Finalizing ? thrown : null) };

                UploadCompletionUnknownException error = await ThrowsAsync<UploadCompletionUnknownException>(Client(server).Transfers.UploadFileAsync(WriteFile("local.bin", Pattern(20)), "remote.bin", options));

                Assert.AreSame(thrown, error.InnerException);
                Assert.AreEqual(1, server.Finishes.Count, "Not asked again.");
                Assert.IsTrue(JsonDocument.Parse(File.ReadAllText(checkpoint)).RootElement.GetProperty("final_request_sent").GetBoolean(), "Still recorded as sent.");
            }
        }

        // Answers the final requests in turn, the last answer repeating.
        private static Func<JsonElement, Task<ResponseMessage>> InTurn(params Func<JsonElement, ResponseMessage>[] answers)
        {
            int answered = 0;
            return body => Task.FromResult(answers[Math.Min(Interlocked.Increment(ref answered), answers.Length) - 1](body));
        }
    }

    // Records the waits an upload makes while Files.com completes its file in the background, at the timer, and ends each at
    // once, for the async flow it is made in until disposed (FilesCom.Http.RetryPolicy.WaitClock, Debug builds only; in
    // others the waits take their time and none is recorded).
    internal sealed class FinalizationWaits : TimeProvider, IDisposable
    {
#if DEBUG
        private readonly TimeProvider previous;
#endif
        private readonly List<TimeSpan> dues = new List<TimeSpan>();

        public FinalizationWaits()
        {
#if DEBUG
            previous = FilesCom.Http.RetryPolicy.WaitClock.Value;
            FilesCom.Http.RetryPolicy.WaitClock.Value = this;
#endif
        }

        public IReadOnlyList<TimeSpan> Dues
        {
            get
            {
                lock (dues)
                {
                    return dues.ToList();
                }
            }
        }

        public override ITimer CreateTimer(TimerCallback callback, object state, TimeSpan dueTime, TimeSpan period)
        {
            lock (dues)
            {
                dues.Add(dueTime);
            }
            return System.CreateTimer(callback, state, TimeSpan.Zero, period);
        }

        public void Dispose()
        {
#if DEBUG
            FilesCom.Http.RetryPolicy.WaitClock.Value = previous;
#endif
        }
    }
}