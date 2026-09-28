using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FilesCom;
using FilesCom.Models;

namespace FilesTests
{
    // Cancelling a token stops an operation promptly, whatever stage it is in, with OperationCanceledException, and
    // nothing further is sent: no retry, next page, next part or final upload request.
    [TestClass]
    public class CancellationTest
    {
        private static readonly DateTime MTime = new DateTime(2026, 9, 24, 0, 0, 0, DateTimeKind.Utc);
        private static readonly TimeSpan Promptly = TimeSpan.FromSeconds(10);
        private readonly List<TaskCompletionSource<bool>> heldResponses = new List<TaskCompletionSource<bool>>();
        private LoopbackSite site;
        private string directory;

        [TestInitialize]
        public void Start()
        {
            site = new LoopbackSite("key-a", "11");
            site.ServeUsers();
            directory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(directory);
        }

        [TestCleanup]
        public void Stop()
        {
            foreach (TaskCompletionSource<bool> response in heldResponses)
            {
                response.TrySetResult(true);
            }
            site.Dispose();
            Directory.Delete(directory, true);
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task CancelledTokenSendsNothing()
        {
            site.ServeUpload(partsize: 4);
            FilesClient client = FilesClient.Create(site.Configuration(maxNetworkRetries: 3));
            User user = client.Users.New(new Dictionary<string, object> { { "id", 1L } });
            var source = new MemoryStream(new byte[10]);
            string localPath = Path.Combine(directory, "never-created.bin");
            CancellationToken cancelled = new CancellationToken(true);

            await AssertCancelledPromptly(client.Users.FindAsync(1, cancellationToken: cancelled));
            await AssertCancelledPromptly(user.UpdateAsync(Changes(), cancelled));
            await AssertCancelledPromptly(client.Users.List().LoadNextPageAsync(cancelled));
            await AssertCancelledPromptly(client.Users.List().AllAsync(cancelled));
            AssertCancelled(() => client.Users.List().ListAutoPaging(cancelled));
            await AssertCancelledPromptly(client.RemoteFiles.UploadFileAsync("remote.bin", source, 10, MTime, cancellationToken: cancelled));
            await AssertCancelledPromptly(client.RemoteFiles.DownloadFileAsync("remote.bin", localPath, cancellationToken: cancelled));

            site.AssertReceived();
            Assert.IsFalse(source.CanRead, "The upload still disposes the stream it owns.");
            Assert.IsFalse(File.Exists(localPath), "No local file is created.");
            await client.Users.FindAsync(1, cancellationToken: new CancellationTokenSource().Token);
            site.AssertReceived("GET /users/1?id=1 key=key-a workspace=11");
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task CancelWhileWaitingForResponseHeadersStopsWithoutRetrying()
        {
            TaskCompletionSource<bool> arrived = HoldResponses("GET", "/users/1", out TaskCompletionSource<bool> respond);
            FilesClient client = FilesClient.Create(site.Configuration(maxNetworkRetries: 3));
            var cancellation = new CancellationTokenSource();

            Task<User> finding = client.Users.FindAsync(1, cancellationToken: cancellation.Token);
            await arrived.Task;
            cancellation.Cancel();

            await AssertCancelledPromptly(finding);
            respond.SetResult(true);
            await Task.Delay(300);
            Assert.AreEqual(1, site.Requests.Count, "A cancelled request is not retried.");
            Assert.AreEqual(1, (await client.Users.FindAsync(1, cancellationToken: new CancellationTokenSource().Token)).Id);
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task CancelDuringRetryWaitStartsNoFurtherAttempt()
        {
            var failures = new SemaphoreSlim(0);
            site.On("GET", "/users/9", request =>
            {
                failures.Release();
                return LoopbackSite.Status(503);
            });
            site.On("PUT", "//part", request =>
            {
                failures.Release();
                return LoopbackSite.Status(503);
            }, priority: 1);
            site.ServeUpload(partsize: 4);
            FilesConfiguration configuration = site.Configuration(maxNetworkRetries: 2);
            configuration.InitialNetworkRequestDelay = 30;
            configuration.MaxNetworkRetryDelay = 30;
            FilesClient client = FilesClient.Create(configuration);

            foreach (Func<CancellationToken, Task> operation in new Func<CancellationToken, Task>[]
            {
                token => client.Users.FindAsync(9, cancellationToken: token),
                token => client.RemoteFiles.UploadFileAsync("remote.bin", new MemoryStream(new byte[4]), 4, MTime, cancellationToken: token),
            })
            {
                var cancellation = new CancellationTokenSource();
                Task running = operation(cancellation.Token);
                Assert.IsTrue(await failures.WaitAsync(Promptly));
                await Task.Delay(300); // The client is now waiting 30 seconds before its retry.
                cancellation.Cancel();
                await AssertCancelledPromptly(running);
            }

            await Task.Delay(300);
            site.AssertReceived("GET /users/9?id=9 key=key-a workspace=11", "POST /file_actions/begin_upload/remote.bin key=key-a workspace=11", "PUT /part");
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task RetriesStillWorkWithAToken()
        {
            int attempts = 0;
            site.On("GET", "/users/9", request => ++attempts == 1 ? LoopbackSite.Status(503) : LoopbackSite.Json("{\"id\":9}"));
            FilesClient client = FilesClient.Create(site.Configuration(maxNetworkRetries: 1));

            User user = await client.Users.FindAsync(9, cancellationToken: new CancellationTokenSource().Token);

            Assert.AreEqual(9, user.Id);
            Assert.AreEqual(2, site.Requests.Count);
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task CancelDuringDownloadStopsWritingAndReleasesTheFile()
        {
            byte[] contents = Enumerable.Range(0, 65536).Select(value => (byte)(value % 251)).ToArray();
            FilesClient client = FilesClient.Create(site.Configuration(maxNetworkRetries: 3));

            using (var server = new SlowDownloadServer(contents, bytesBeforePause: 16384, pause: Timeout.InfiniteTimeSpan))
            {
                ServeDownloadDetails(server.Url);
                var destination = new MemoryStream();
                var cancellation = new CancellationTokenSource();

                Task downloading = client.RemoteFiles.DownloadFileAsync("remote.bin", destination, cancellationToken: cancellation.Token);
                await server.BodyStarted;
                await WaitUntil(() => destination.Length == 16384);
                cancellation.Cancel();

                Exception error = await AssertCancelledPromptly(downloading);
                await Task.Delay(300);
                Assert.AreEqual(16384, destination.Length, "Nothing is written after the download ends.");
                Assert.IsTrue(destination.CanWrite, "The download leaves the stream it is given open.");
                Assert.IsFalse(error.ToString().Contains("signature"), "The signed download URI is not in the error.");
            }

            string localPath = Path.Combine(directory, "download.bin");
            using (var server = new SlowDownloadServer(contents, bytesBeforePause: 16384, pause: Timeout.InfiniteTimeSpan))
            {
                ServeDownloadDetails(server.Url);
                var cancellation = new CancellationTokenSource();

                Task downloading = client.RemoteFiles.DownloadFileAsync("remote.bin", localPath, cancellationToken: cancellation.Token);
                await server.BodyStarted;
                await Task.Delay(300);
                cancellation.Cancel();

                await AssertCancelledPromptly(downloading);
                AssertFileReleased(localPath);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task ReadTimeoutLimitsOnlyTheWaitForResponseHeaders()
        {
            byte[] contents = Encoding.ASCII.GetBytes("contents sent after the read timeout");
            FilesConfiguration configuration = site.Configuration();
            configuration.ReadTimeout = 1;
            FilesClient client = FilesClient.Create(configuration);

            using (var server = new SlowDownloadServer(contents, bytesBeforePause: 0, pause: TimeSpan.FromSeconds(2)))
            {
                ServeDownloadDetails(server.Url);
                var destination = new MemoryStream();

                await client.RemoteFiles.DownloadFileAsync("remote.bin", destination, cancellationToken: new CancellationTokenSource().Token);

                CollectionAssert.AreEqual(contents, destination.ToArray());
            }

            using (var server = new SlowDownloadServer(contents, bytesBeforePause: 0, pause: Timeout.InfiniteTimeSpan, sendHeaders: false))
            {
                ServeDownloadDetails(server.Url);
                Stopwatch elapsed = Stopwatch.StartNew();

                await AssertCancelledPromptly(client.RemoteFiles.DownloadFileAsync("remote.bin", new MemoryStream()));

                Assert.IsTrue(elapsed.Elapsed < Promptly, "Waiting for headers still times out after ReadTimeout.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task CancelWhileReadingAnErrorResponseWritesNothing()
        {
            byte[] error = Encoding.UTF8.GetBytes(@"{""type"":""not-found"",""error"":""Not Found""}");
            FilesClient client = FilesClient.Create(site.Configuration(maxNetworkRetries: 3));

            using (var server = new SlowDownloadServer(error, bytesBeforePause: 8, pause: Timeout.InfiniteTimeSpan, status: "404 Not Found"))
            {
                ServeDownloadDetails(server.Url);
                var destination = new MemoryStream();
                var cancellation = new CancellationTokenSource();

                Task downloading = client.RemoteFiles.DownloadFileAsync("remote.bin", destination, cancellationToken: cancellation.Token);
                await server.BodyStarted;
                await Task.Delay(300); // The client is now reading the rest of the error body.
                cancellation.Cancel();

                await AssertCancelledPromptly(downloading);
                Assert.AreEqual(0, destination.Length, "An error response is never written to the destination.");
                Assert.IsTrue(destination.CanWrite, "The download leaves the stream it is given open.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task CancelDuringDestinationWriteStopsTheDownload()
        {
            site.ServeDownload(new byte[4096]);
            FilesClient client = FilesClient.Create(site.Configuration());
            var destination = new WaitingStream();
            var cancellation = new CancellationTokenSource();

            Task downloading = client.RemoteFiles.DownloadFileAsync("remote.bin", destination, cancellationToken: cancellation.Token);
            await destination.Waiting;
            cancellation.Cancel();

            await AssertCancelledPromptly(downloading);
            Assert.IsFalse(destination.Disposed, "The download leaves the stream it is given open.");
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task DownloadKeepsTheDestinationsOwnWriteError()
        {
            site.ServeDownload(new byte[4096]);
            FilesClient client = FilesClient.Create(site.Configuration());
            var writeError = new IOException("The destination disk is full.");

            Exception error = await Assert.ThrowsExceptionAsync<IOException>(() =>
                client.RemoteFiles.DownloadFileAsync("remote.bin", new WaitingStream(writeError), cancellationToken: new CancellationTokenSource().Token));

            Assert.AreSame(writeError, error);
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task CancelDuringUploadSourceReadSendsNoPart()
        {
            site.ServeUpload(partsize: 4);
            FilesClient client = FilesClient.Create(site.Configuration());
            var source = new WaitingStream();
            var cancellation = new CancellationTokenSource();

            Task uploading = client.RemoteFiles.UploadFileAsync("remote.bin", source, 10, MTime, cancellationToken: cancellation.Token);
            await source.Waiting;
            cancellation.Cancel();

            await AssertCancelledPromptly(uploading);
            site.AssertReceived("POST /file_actions/begin_upload/remote.bin key=key-a workspace=11");
            Assert.IsTrue(source.Disposed, "The upload disposes the stream it owns.");
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task CancelAfterAPartStartsNoFurtherPartOrFinalRequest()
        {
            string begin = "POST /file_actions/begin_upload/remote.bin key=key-a workspace=11";
            await AssertUploadCancelledAtPart(1, begin, "PUT /part");
            await AssertUploadCancelledAtPart(3, begin, "PUT /part", begin, "PUT /part", begin, "PUT /part");
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task CancelDuringTheFinalRequestIsReportedWithoutRetrying()
        {
            site.ServeUpload(partsize: 4);
            TaskCompletionSource<bool> arrived = HoldResponses("POST", "/files/remote.bin", out TaskCompletionSource<bool> respond);
            FilesClient client = FilesClient.Create(site.Configuration(maxNetworkRetries: 3));
            var source = new MemoryStream(new byte[4]);
            var cancellation = new CancellationTokenSource();

            Task uploading = client.RemoteFiles.UploadFileAsync("remote.bin", source, 4, MTime, cancellationToken: cancellation.Token);
            await arrived.Task;
            cancellation.Cancel();

            // The final request reached the site, so the upload may have completed there; the caller still sees
            // the cancellation rather than success.
            await AssertCancelledPromptly(uploading);
            respond.SetResult(true);
            await Task.Delay(300);
            Assert.AreEqual(1, site.Requests.Count(request => request.Path == "/files/remote.bin"));
            Assert.IsFalse(source.CanRead);
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task CancelBetweenPagesLoadsNoFurtherPage()
        {
            var cancellation = new CancellationTokenSource();
            site.On("GET", "/users", request =>
            {
                cancellation.Cancel();
                return LoopbackSite.Json("[{\"id\":1}]", cursor: "page-2");
            }, priority: 1);
            FilesClient client = FilesClient.Create(site.Configuration());

            await AssertCancelledPromptly(client.Users.List().AllAsync(cancellation.Token));

            var enumerating = new CancellationTokenSource();
            FilesListEnumerator<User> users = client.Users.List().ListAutoPaging(enumerating.Token);
            Assert.IsTrue(users.MoveNext());
            enumerating.Cancel();
            AssertCancelled(() => users.MoveNext());

            site.AssertReceived("GET /users key=key-a workspace=11", "GET /users key=key-a workspace=11");
        }

        [TestMethod]
        public void PagerWithATokenThrowsFailuresAsTheyAre()
        {
            site.On("GET", "/folders/missing", request => LoopbackSite.Json(@"{""type"":""not-found"",""http-code"":404,""error"":""Not Found""}", status: 404));
            FilesClient client = FilesClient.Create(site.Configuration());

            Assert.ThrowsException<NotFoundException>(() => client.Folders.ListFor("missing").ListAutoPaging(CancellationToken.None));
            AggregateException wrapped = Assert.ThrowsException<AggregateException>(() => client.Folders.ListFor("missing").ListAutoPaging());
            Assert.IsInstanceOfType(wrapped.InnerException, typeof(NotFoundException));
        }

        private async Task AssertUploadCancelledAtPart(int cancelledPart, params string[] expectedRequests)
        {
            using (var uploadSite = new LoopbackSite("key-a", "11"))
            {
                var cancellation = new CancellationTokenSource();
                int parts = 0;
                uploadSite.ServeUpload(partsize: 4, onPart: () =>
                {
                    if (++parts == cancelledPart)
                    {
                        cancellation.Cancel();
                    }
                });
                FilesClient client = FilesClient.Create(uploadSite.Configuration(maxNetworkRetries: 3));
                var source = new MemoryStream(new byte[10]);

                await AssertCancelledPromptly(client.RemoteFiles.UploadFileAsync("remote.bin", source, 10, MTime, cancellationToken: cancellation.Token));

                await Task.Delay(300);
                uploadSite.AssertReceived(expectedRequests);
                Assert.IsFalse(source.CanRead, "The upload disposes the stream it owns.");
            }
        }

        // Holds responses to method requests for path until the returned respond source is set.
        private TaskCompletionSource<bool> HoldResponses(string method, string path, out TaskCompletionSource<bool> respond)
        {
            var arrived = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var held = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            heldResponses.Add(held);
            site.On(method, path, async request =>
            {
                arrived.TrySetResult(true);
                await held.Task;
                return LoopbackSite.Json("{\"id\":1,\"path\":\"remote.bin\"}");
            }, priority: 1);
            respond = held;
            return arrived;
        }

        private void ServeDownloadDetails(string downloadUrl)
        {
            site.On("GET", "/files/remote.bin", request => LoopbackSite.Json($"{{\"path\":\"remote.bin\",\"download_uri\":\"{downloadUrl}\"}}"), priority: 1);
        }

        private static async Task<Exception> AssertCancelledPromptly(Task operation)
        {
            if (await Task.WhenAny(operation, Task.Delay(Promptly)) != operation)
            {
                Assert.Fail($"The operation was still running {Promptly.TotalSeconds} seconds after it was cancelled.");
            }
            try
            {
                await operation;
            }
            catch (OperationCanceledException e)
            {
                return e;
            }
            Assert.Fail("The operation completed instead of reporting its cancellation.");
            return null;
        }

        // A synchronous operation reports cancellation as OperationCanceledException itself, not wrapped.
        private static void AssertCancelled(System.Action operation)
        {
            try
            {
                operation();
            }
            catch (OperationCanceledException)
            {
                return;
            }
            Assert.Fail("The operation completed instead of reporting its cancellation.");
        }

        private static async Task WaitUntil(Func<bool> condition)
        {
            Stopwatch waited = Stopwatch.StartNew();
            while (!condition())
            {
                Assert.IsTrue(waited.Elapsed < Promptly, "Timed out waiting for the test condition.");
                await Task.Delay(20);
            }
        }

        private static void AssertFileReleased(string path)
        {
            // An exclusive open fails while any other handle to the file is still open.
            using (new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
            }
        }

        private static Dictionary<string, object> Changes()
        {
            return new Dictionary<string, object> { { "username", "changed" } };
        }

        // A stream whose reads and writes wait until they are cancelled, like a stalled network stream, or that fails
        // every write with writeError.
        private sealed class WaitingStream : Stream
        {
            private readonly TaskCompletionSource<bool> waiting = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            private readonly Exception writeError;

            public WaitingStream(Exception writeError = null)
            {
                this.writeError = writeError;
            }

            public Task Waiting => waiting.Task;

            public bool Disposed { get; private set; }

            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => true;
            public override long Length => throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                waiting.TrySetResult(true);
                await Task.Delay(Timeout.Infinite, cancellationToken);
                return 0;
            }

            public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                if (writeError != null)
                {
                    throw writeError;
                }
                waiting.TrySetResult(true);
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }

            public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
            public override void Flush()
            {
            }
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();

            protected override void Dispose(bool disposing)
            {
                Disposed = true;
                base.Dispose(disposing);
            }
        }

        // Serves one download on loopback: the headers with the given status (unless sendHeaders is false), the first
        // bytesBeforePause bytes of the body, then the rest after pause. Its URL carries a stand-in signature, like a
        // signed storage URL.
        private sealed class SlowDownloadServer : IDisposable
        {
            private readonly TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
            private readonly CancellationTokenSource stopping = new CancellationTokenSource();
            private readonly TaskCompletionSource<bool> bodyStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            public SlowDownloadServer(byte[] body, int bytesBeforePause, TimeSpan pause, bool sendHeaders = true, string status = "200 OK")
            {
                listener.Start();
                Url = $"http://127.0.0.1:{((IPEndPoint)listener.LocalEndpoint).Port}/download?signature=secret-signature";
                _ = Serve(body, bytesBeforePause, pause, sendHeaders, status);
            }

            public string Url { get; }

            public Task BodyStarted => bodyStarted.Task;

            public void Dispose()
            {
                stopping.Cancel();
                listener.Stop();
            }

            private async Task Serve(byte[] body, int bytesBeforePause, TimeSpan pause, bool sendHeaders, string status)
            {
                try
                {
                    using (TcpClient client = await listener.AcceptTcpClientAsync())
                    using (NetworkStream stream = client.GetStream())
                    {
                        var request = new StreamReader(stream, Encoding.ASCII);
                        while (!string.IsNullOrEmpty(await request.ReadLineAsync()))
                        {
                        }
                        if (sendHeaders)
                        {
                            byte[] headers = Encoding.ASCII.GetBytes($"HTTP/1.1 {status}\r\nContent-Length: {body.Length}\r\nConnection: close\r\n\r\n");
                            await stream.WriteAsync(headers, 0, headers.Length);
                            await stream.WriteAsync(body, 0, bytesBeforePause);
                            await stream.FlushAsync();
                        }
                        bodyStarted.TrySetResult(true);
                        await Task.Delay(pause, stopping.Token);
                        await stream.WriteAsync(body, bytesBeforePause, body.Length - bytesBeforePause);
                    }
                }
                catch (Exception) when (stopping.IsCancellationRequested)
                {
                    // Stopped by the test.
                }
            }
        }
    }
}
