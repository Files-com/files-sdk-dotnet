using FilesCom;
using FilesCom.Models;
using FilesCom.Transfers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FilesTests
{
    // Disposing a client closes it once: new operations of the client, and of the objects, lists and transfers it
    // returned, throw ObjectDisposedException; running ones are cancelled, and DisposeAsync waits for them before it
    // closes the client's connections. Other clients, and the default client, carry on. Handlers the client owns are
    // disposed once; a shared handler never is.
    [TestClass]
    public class ClientLifetimeTest
    {
        private static readonly DateTime MTime = new DateTime(2026, 9, 24, 0, 0, 0, DateTimeKind.Utc);
        private static readonly TimeSpan Promptly = TimeSpan.FromSeconds(10);
        private readonly List<TaskCompletionSource<bool>> held = new List<TaskCompletionSource<bool>>();
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
            foreach (TaskCompletionSource<bool> response in held)
            {
                response.TrySetResult(true);
            }
            site.Dispose();
            Directory.Delete(directory, true);
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task DisposedClientStartsNothingNew()
        {
            site.ServeUpload(partsize: 4);
            FilesClient client = FilesClient.Create(site.Configuration());
            User user = await client.Users.FindAsync(1);
            FilesList<User> list = client.Users.List();
            await list.LoadNextPageAsync();

            await client.DisposeAsync();

            var legacySource = new MemoryStream(new byte[4]);
            var transferSource = new MemoryStream(new byte[4]);
            string localPath = Path.Combine(directory, "never-created.bin");
            await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => client.Users.FindAsync(2));
            await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => user.UpdateAsync(new Dictionary<string, object> { { "username", "changed" } }));
            await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => list.LoadNextPageAsync());
            await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => list.AllAsync());
            Assert.ThrowsExactly<ObjectDisposedException>(() => list.ListAutoPaging(CancellationToken.None));
            await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => client.Transfers.UploadStreamAsync(transferSource, 4, "remote.bin"));
            await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => client.RemoteFiles.UploadFileAsync("remote.bin", legacySource, 4, MTime));
            await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => client.RemoteFiles.DownloadFileAsync("remote.bin", localPath));

            site.AssertReceived("GET /users/1?id=1 key=key-a workspace=11", "GET /users key=key-a workspace=11");
            Assert.IsFalse(legacySource.CanRead, "The legacy upload still disposes the stream it owns.");
            Assert.IsTrue(transferSource.CanRead, "The transfer leaves its borrowed stream open.");
            Assert.IsFalse(File.Exists(localPath), "No local file is created.");
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task ClosingHappensOnceHoweverOftenItIsAsked()
        {
            var handlers = new TrackedHandlers();
            FilesClient client = FilesClient.Create(site.Configuration(), HttpTransport.FromHandlerFactory(handlers.Create));
            await client.Users.FindAsync(1);

            client.Dispose();
            await Task.WhenAll(Enumerable.Range(0, 8).Select(i => Task.Run(async () =>
            {
                if (i % 2 == 0)
                {
                    client.Dispose();
                }
                await client.DisposeAsync();
            })));
            await client.DisposeAsync();

            Assert.AreEqual(1, handlers.Created);
            Assert.AreEqual("1", string.Join(",", handlers.All.Select(handler => handler.Disposals)), "Each handler the client owned was disposed exactly once.");
        }

        // What DisposeAsync waits for is the operation's work: when it completes, the request it cancelled has ended, the
        // operation no longer holds the client, and the client's connections are closed. The operation's task can
        // complete after DisposeAsync does, so it is awaited here rather than taken to be complete already (an earlier
        // version of this test asserted that, and failed once on .NET 8).
        [TestMethod]
        [Timeout(60000)]
        public async Task DisposeAsyncCancelsRunningWorkAndWaitsForIt()
        {
            TaskCompletionSource<bool> arrived = HoldResponses("GET", "/users/9");
            var handlers = new TrackedHandlers();
            FilesClient client = FilesClient.Create(site.Configuration(maxNetworkRetries: 3), HttpTransport.FromHandlerFactory(handlers.Create));

            Task<User> finding = client.Users.FindAsync(9);
            await arrived.Task;
            Stopwatch elapsed = Stopwatch.StartNew();
            client.Dispose();
            TimeSpan disposeTook = elapsed.Elapsed;
            Task closing = client.DisposeAsync().AsTask();

            Assert.AreSame(closing, await Task.WhenAny(closing, Task.Delay(Promptly)), "DisposeAsync finished once the request had ended.");
            Assert.AreEqual(0, handlers.All.Sum(handler => handler.Sending), "No request was still being sent when DisposeAsync completed.");
            Assert.AreEqual(0, client.Lifetime.RunningOperations, "No operation still held the client.");
            Assert.AreEqual("1", string.Join(",", handlers.All.Select(handler => handler.Disposals)), "The client's connections were closed.");
            await Assert.ThrowsAsync<OperationCanceledException>(() => finding);
            Assert.IsTrue(disposeTook < TimeSpan.FromSeconds(1), $"Dispose returned after {disposeTook.TotalMilliseconds} ms instead of at once.");
            Assert.AreEqual(1, site.Requests.Count, "A request the client's closing cancelled is not retried, and nothing is sent after it.");
        }

        // The stream a legacy upload owns is disposed while the upload still holds its client: closing waits for that
        // disposal, however long it takes, and closes the client's connections after it.
        [TestMethod]
        [Timeout(60000)]
        public async Task DisposeAsyncWaitsForAStreamAnUploadOwnsToBeDisposed()
        {
            TaskCompletionSource<bool> arrived = HoldResponses("POST", "/file_actions/begin_upload/remote.bin");
            var handlers = new TrackedHandlers();
            FilesClient client = FilesClient.Create(site.Configuration(), HttpTransport.FromHandlerFactory(handlers.Create));
            var source = new HeldDisposalStream(client);

            Task<bool> uploading = client.RemoteFiles.UploadFileAsync("remote.bin", source, 4, MTime);
            await arrived.Task;
            Task closing = client.DisposeAsync().AsTask();
            await source.Disposing;

            Assert.AreEqual(1, source.RunningWhileDisposing, "The upload held its client while it disposed the stream it owns.");
            Assert.IsFalse(closing.IsCompleted, "DisposeAsync has not completed while the stream is being disposed.");
            Assert.AreEqual(0, handlers.All.Sum(handler => handler.Disposals), "The client's connections are still open.");
            source.Finish();

            Assert.AreSame(closing, await Task.WhenAny(closing, Task.Delay(Promptly)), "DisposeAsync finished once the stream was disposed.");
            Assert.IsTrue(source.Disposed, "The stream was disposed before DisposeAsync completed.");
            Assert.AreEqual("1", string.Join(",", handlers.All.Select(handler => handler.Disposals)), "The client's connections were closed after.");
            await Assert.ThrowsAsync<OperationCanceledException>(() => uploading);
        }

        // A response the SDK reads for an operation (an object, a page of a list, or an answer it does not use) is disposed
        // while the operation still holds its client, so closing waits for that too.
        [TestMethod]
        [Timeout(60000)]
        public async Task DisposeAsyncWaitsForResponsesOperationsReadToBeDisposed()
        {
            site.On("DELETE", "/users/*", request => LoopbackSite.Status(200));
            var operations = new (string Path, Func<FilesClient, Task> Run)[]
            {
                ("/users/1", client => client.Users.FindAsync(1)),
                ("/users", client => client.Users.List().LoadNextPageAsync()),
                ("/users/3", client => client.Users.DeleteAsync(3)),
            };
            foreach ((string path, Func<FilesClient, Task> run) in operations)
            {
                var handler = new HeldResponseHandler(path);
                FilesClient client = FilesClient.Create(site.Configuration(), HttpTransport.FromHandlerFactory(() => handler));
                handler.Client = client;

                Task operation = run(client);
                await handler.Disposing;
                Task closing = client.DisposeAsync().AsTask();

                Assert.AreEqual(1, handler.RunningWhileDisposing, $"{path}: the operation held its client while its response was disposed.");
                Assert.IsFalse(closing.IsCompleted, $"{path}: DisposeAsync has not completed while the response is being disposed.");
                handler.Finish();

                Assert.AreSame(closing, await Task.WhenAny(closing, Task.Delay(Promptly)), $"{path}: DisposeAsync finished once the response was disposed.");
                Assert.IsTrue(handler.Disposed, $"{path}: the response was disposed before DisposeAsync completed.");
                await operation;
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task ListThatIsNotLoadingDoesNotDelayClosing()
        {
            FilesClient client = FilesClient.Create(site.Configuration());
            IAsyncEnumerator<User> users = client.Users.List().ListAutoPagingAsync().GetAsyncEnumerator();
            Assert.IsTrue(await users.MoveNextAsync());

            Task closing = client.DisposeAsync().AsTask();

            Assert.AreSame(closing, await Task.WhenAny(closing, Task.Delay(Promptly)), "An enumerator between pages does not keep the client open.");
            await Assert.ThrowsExactlyAsync<ObjectDisposedException>(async () => await users.MoveNextAsync());
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task DisposingStopsAnUploadWithoutCompletingIt()
        {
            using (var server = new UploadServer { HoldParts = true })
            {
                var limiter = new TransferLimiter();
                var source = new MemoryStream(new byte[20]);
                FilesClient client = FilesClient.Create(server.Site.Configuration(maxNetworkRetries: 3));

                Task<UploadResult> upload = client.Transfers.UploadStreamAsync(source, 20, "remote.bin", new UploadOptions { Limiter = limiter });
                await server.WaitUntil(s => s.PartsInFlight > 0, "a part is being sent");
                await client.DisposeAsync();

                Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes, "When DisposeAsync completed, the upload had given back everything it held.");
                Assert.AreEqual(0, client.Lifetime.RunningOperations, "The upload no longer held the client.");
                await Assert.ThrowsAsync<OperationCanceledException>(() => upload);
                Assert.AreEqual(0, server.Finishes.Count, "The upload was not completed after its client closed.");
                Assert.IsTrue(source.CanRead, "The borrowed stream is left open.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task DisposeFromAProgressHandlerReturnsAtOnce()
        {
            // Held, so that the upload cannot finish before its client closes.
            using (var server = new UploadServer { HoldParts = true })
            {
                FilesClient client = FilesClient.Create(server.Site.Configuration());
                TimeSpan disposeTook = TimeSpan.MaxValue;
                var progress = new SynchronousProgress(report =>
                {
                    Stopwatch elapsed = Stopwatch.StartNew();
                    client.Dispose();
                    disposeTook = elapsed.Elapsed;
                });

                Task<UploadResult> upload = client.Transfers.UploadStreamAsync(new MemoryStream(new byte[20]), 20, "remote.bin", new UploadOptions { Progress = progress });

                await Assert.ThrowsAsync<OperationCanceledException>(() => upload);
                Assert.IsTrue(disposeTook < TimeSpan.FromSeconds(1), $"Dispose took {disposeTook.TotalMilliseconds} ms inside the upload's own progress report.");
                await client.DisposeAsync();
                Assert.AreEqual(0, server.Finishes.Count);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task OtherClientsAndTheDefaultClientCarryOn()
        {
            new FilesClient(site.Configuration());
            User found = await User.Find(1);
            FilesClient first = FilesClient.Instance;
            var second = new FilesClient(site.Configuration());
            FilesClient independent = FilesClient.Create(site.Configuration());

            await first.DisposeAsync();

            Assert.AreSame(second, FilesClient.Instance, "Disposing an earlier default client leaves the current one in place.");
            await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => found.UpdateAsync(new Dictionary<string, object> { { "username", "changed" } }));
            Assert.AreEqual(2, (await User.Find(2)).Id);
            Assert.AreEqual(3, (await independent.Users.FindAsync(3)).Id);

            await second.DisposeAsync();
            Assert.AreSame(second, FilesClient.Instance, "Disposing the default client does not replace it.");
            // An object made without a client is not bound to a disposed default, so it can use the next one.
            var unbound = new User(new Dictionary<string, object> { { "id", 5L } }, null);
            await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => unbound.UpdateAsync(new Dictionary<string, object> { { "username", "changed" } }));
            new FilesClient(site.Configuration());
            Assert.AreEqual("changed", (await unbound.UpdateAsync(new Dictionary<string, object> { { "username", "changed" } })).Username);
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task OwnedHandlersAreDisposedAndSharedOnesAreNot()
        {
            site.ServeDownload(new byte[64]);
            site.ServeUpload(partsize: 4);
            var owned = new TrackedHandlers();
            FilesClient owner = FilesClient.Create(site.Configuration(), HttpTransport.FromHandlerFactory(owned.Create));
            var shared = new TrackedHandler(new HttpClientHandler { AllowAutoRedirect = false });
            HttpTransport sharing = HttpTransport.FromSharedHandler(shared);
            FilesClient borrower = FilesClient.Create(site.Configuration(), sharing);

            foreach (FilesClient client in new[] { owner, borrower })
            {
                await client.Users.FindAsync(1);
                await client.RemoteFiles.DownloadFileAsync("remote.bin", new MemoryStream());
                await client.RemoteFiles.UploadFileAsync("remote.bin", new MemoryStream(new byte[4]), 4, MTime);
                await client.DisposeAsync();
            }

            Assert.AreEqual(2, owned.Created, "One handler for API requests and downloads, one for upload parts.");
            Assert.IsTrue(owned.All.All(handler => handler.Disposals == 1), "Each handler the client owned was disposed once.");
            Assert.AreEqual(0, shared.Disposals, "The shared handler was not disposed.");
            FilesClient next = FilesClient.Create(site.Configuration(), sharing);
            Assert.AreEqual(2, (await next.Users.FindAsync(2)).Id, "Another client can still use the shared handler.");
            using (var direct = new HttpClient(shared, disposeHandler: false))
            {
                Assert.IsTrue((await direct.GetAsync(site.Url + "/api/rest/v1/users/3")).IsSuccessStatusCode, "So can the application.");
            }
            await next.DisposeAsync();
            shared.Dispose();
        }

        [TestMethod]
        [Timeout(120000)]
        public async Task ClientsCloseTheirConnectionsWhenDisposed()
        {
            using (var api = new ScriptedServer(async connection =>
            {
                while (true)
                {
                    await connection.ReadRequestHeadAsync();
                    await connection.RespondAsync(200, "{\"id\":1}");
                }
            }))
            {
                var handlers = new TrackedHandlers();
                var configuration = new FilesConfiguration { BaseUrl = $"http://127.0.0.1:{api.Port}", ApiKey = "key", MaxNetworkRetries = 0 };
                const int clients = 40;

                for (int i = 0; i < clients; i++)
                {
                    FilesClient client = i % 2 == 0 ? FilesClient.Create(configuration) : FilesClient.Create(configuration, HttpTransport.FromHandlerFactory(handlers.Create));
                    await client.Users.FindAsync(1);
                    await client.DisposeAsync();
                }

                // Each client made one connection, and closed it once disposed, rather than leaving it to time out.
                Assert.AreEqual(clients, api.Connections.Count);
                Task closed = Task.WhenAll(api.Connections.Select(connection => connection.Closed));
                Assert.AreSame(closed, await Task.WhenAny(closed, Task.Delay(Promptly)), $"{api.Connections.Count(c => !c.Closed.IsCompleted)} of {clients} connections are still open.");
                Assert.AreEqual(clients / 2, handlers.Created);
                Assert.IsTrue(handlers.All.All(handler => handler.Disposals == 1));
            }
        }

        [TestMethod]
        public async Task TransportsThatCannotBeUsedSafelyAreRefused()
        {
            bool created = false;
            FilesConfiguration invalid = site.Configuration();
            invalid.ConnectTimeout = 0;
            Assert.ThrowsExactly<InvalidParameterException>(() => FilesClient.Create(invalid, HttpTransport.FromHandlerFactory(() =>
            {
                created = true;
                return new HttpClientHandler();
            })));
            Assert.IsFalse(created, "A client whose settings are invalid creates no handler.");

            // .NET's own redirects keep the Files.com credential headers on the way to another origin.
            Assert.AreEqual("handler", Assert.ThrowsExactly<InvalidParameterException>(() => HttpTransport.FromSharedHandler(new HttpClientHandler())).ParamName);
            Assert.AreEqual("handler", Assert.ThrowsExactly<InvalidParameterException>(() => HttpTransport.FromSharedHandler(new TrackedHandler(null))).ParamName);
            Assert.AreEqual("createHandler", Assert.ThrowsExactly<MissingParameterException>(() => HttpTransport.FromHandlerFactory(null)).ParamName);

            // A factory must hand the client handlers it can dispose. One it returns again is refused, and stays with
            // the pipeline it was returned to first, which disposes it once.
            var one = new TrackedHandler(new HttpClientHandler());
            site.ServeUpload(partsize: 4);
            FilesClient client = FilesClient.Create(site.Configuration(), HttpTransport.FromHandlerFactory(() => one));
            await client.Users.FindAsync(1);
            InvalidParameterException reused = await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => client.RemoteFiles.UploadFileAsync("remote.bin", new MemoryStream(new byte[4]), 4, MTime));
            StringAssert.Contains(reused.Message, "returned a handler it had returned before");
            Assert.AreEqual("createHandler", reused.ParamName);
            Assert.AreEqual(0, one.Disposals, "Refusing it again did not dispose a handler its first pipeline still uses.");
            await client.DisposeAsync();
            Assert.AreEqual(1, one.Disposals);

            // A new handler that cannot be used is the client's all the same, so it is disposed rather than left behind.
            var unusable = new TrackedHandler(null);
            FilesClient refusing = FilesClient.Create(site.Configuration(), HttpTransport.FromHandlerFactory(() => unusable));
            InvalidParameterException incomplete = await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => refusing.Users.FindAsync(1));
            StringAssert.Contains(incomplete.Message, "has no InnerHandler");
            await refusing.DisposeAsync();
            Assert.AreEqual(1, unusable.Disposals);
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task CleanupFailuresAreLoggedWithoutWhatTheyQuote()
        {
            const string secret = "https://user:sentinel-secret@storage.example.test/path?token=sentinel-secret";
            var handler = new FailingCleanupHandler(secret);
            FilesClient client = FilesClient.Create(site.Configuration(), HttpTransport.FromHandlerFactory(() => handler));
            FilesClient refusing = FilesClient.Create(site.Configuration(), HttpTransport.FromHandlerFactory(() => new FailingCleanupHandler(secret, inner: false)));

            using (var logs = new LogCapture())
            {
                // Its request's cancellation callback throws when the client closes, and so does its Dispose.
                Task finding = client.Users.FindAsync(1);
                await handler.Sending;
                await client.DisposeAsync();
                await client.DisposeAsync();
                await Assert.ThrowsAsync<OperationCanceledException>(() => finding);
                // Disposing a handler that could not be set up throws; the reason it could not be is what is thrown.
                InvalidParameterException setup = await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => refusing.Users.FindAsync(1));
                StringAssert.Contains(setup.Message, "has no InnerHandler");
                await refusing.DisposeAsync();

                Assert.IsFalse(logs.Normal.Contains("sentinel"), logs.Normal);
                // Once each, however many times the client is disposed.
                CollectionAssert.AreEquivalent(
                    new[]
                    {
                        "Closing a client: cancelling its operations failed (AggregateException: InvalidOperationException); the details are logged at DEBUG.",
                        "Closing a client: disposing one of its HTTP handlers failed (InvalidOperationException); the details are logged at DEBUG.",
                        "Setting up an HTTP handler: disposing a handler that could not be used failed (InvalidOperationException); the details are logged at DEBUG.",
                    },
                    logs.Lines("failed ("));
                StringAssert.Contains(logs.Debug, "sentinel-secret", "The details stay available at DEBUG.");
            }
        }

        // Holds responses to method requests for path until the test ends.
        private TaskCompletionSource<bool> HoldResponses(string method, string path)
        {
            var arrived = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var response = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            held.Add(response);
            site.On(method, path, async request =>
            {
                arrived.TrySetResult(true);
                await response.Task;
                return LoopbackSite.Json("{\"id\":9}");
            }, priority: 1);
            return arrived;
        }

        // A custom transport whose cleanup fails with an exception quoting a signed URL: the cancellation callback it
        // registers for a request, which it holds until cancelled, and its Dispose. Without inner, it is a chain with
        // no handler at its bottom, which the SDK refuses.
        private sealed class FailingCleanupHandler : DelegatingHandler
        {
            private readonly string quoted;
            private readonly TaskCompletionSource<bool> sending = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            public FailingCleanupHandler(string quoted, bool inner = true)
            {
                this.quoted = quoted;
                if (inner)
                {
                    InnerHandler = new HttpClientHandler();
                }
            }

            public Task Sending => sending.Task;

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                using (cancellationToken.Register(() => throw new InvalidOperationException("Could not stop " + quoted)))
                {
                    sending.TrySetResult(true);
                    await Task.Delay(Timeout.Infinite, cancellationToken);
                    return null;
                }
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (disposing)
                {
                    throw new InvalidOperationException("Could not close " + quoted);
                }
            }
        }

        // A stream for an upload to own, whose disposal waits until the test lets it finish, and which records how many
        // operations held its client when the disposal began.
        private sealed class HeldDisposalStream : MemoryStream
        {
            private readonly FilesClient client;
            private readonly TaskCompletionSource<bool> disposing = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            private readonly ManualResetEventSlim finish = new ManualResetEventSlim(false);
            private int started;

            public HeldDisposalStream(FilesClient client) : base(new byte[4])
            {
                this.client = client;
            }

            public Task Disposing => disposing.Task;

            public int RunningWhileDisposing { get; private set; } = -1;

            public bool Disposed { get; private set; }

            public void Finish()
            {
                finish.Set();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing && Interlocked.Exchange(ref started, 1) == 0)
                {
                    RunningWhileDisposing = client.Lifetime.RunningOperations;
                    this.disposing.TrySetResult(true);
                    finish.Wait(Promptly);
                    Disposed = true;
                }
                base.Dispose(disposing);
            }
        }

        // A transport whose response to requests for path is disposed only when the test lets it finish, recording how
        // many operations held its client when the disposal began. Other responses pass as they are.
        private sealed class HeldResponseHandler : DelegatingHandler
        {
            private readonly string path;
            private readonly TaskCompletionSource<bool> disposing = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            private readonly ManualResetEventSlim finish = new ManualResetEventSlim(false);
            private int started;

            public HeldResponseHandler(string path) : base(new SocketsHttpHandler())
            {
                this.path = path;
            }

            public FilesClient Client { get; set; }

            public Task Disposing => disposing.Task;

            public int RunningWhileDisposing { get; private set; } = -1;

            public bool Disposed { get; private set; }

            public void Finish()
            {
                finish.Set();
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
                if (!request.RequestUri.AbsolutePath.EndsWith(path, StringComparison.Ordinal))
                {
                    return response;
                }
                var held = new HeldResponse(this, response.StatusCode) { RequestMessage = request, Content = response.Content };
                foreach (KeyValuePair<string, IEnumerable<string>> header in response.Headers)
                {
                    held.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
                response.Content = null;
                response.Dispose();
                return held;
            }

            private void OnResponseDisposing(bool disposing)
            {
                if (disposing && Interlocked.Exchange(ref started, 1) == 0)
                {
                    RunningWhileDisposing = Client.Lifetime.RunningOperations;
                    this.disposing.TrySetResult(true);
                    finish.Wait(Promptly);
                    Disposed = true;
                }
            }

            private sealed class HeldResponse : HttpResponseMessage
            {
                private readonly HeldResponseHandler owner;

                public HeldResponse(HeldResponseHandler owner, System.Net.HttpStatusCode status) : base(status)
                {
                    this.owner = owner;
                }

                protected override void Dispose(bool disposing)
                {
                    owner.OnResponseDisposing(disposing);
                    base.Dispose(disposing);
                }
            }
        }

        // An IProgress that reports on the upload's own thread, as a handler that does its work at once would.
        private sealed class SynchronousProgress : IProgress<TransferProgress>
        {
            private readonly Action<TransferProgress> report;

            public SynchronousProgress(Action<TransferProgress> report)
            {
                this.report = report;
            }

            public void Report(TransferProgress value)
            {
                report(value);
            }
        }
    }

    // The handlers an HttpTransport factory has created, each around a new SocketsHttpHandler.
    public sealed class TrackedHandlers
    {
        private readonly List<TrackedHandler> created = new List<TrackedHandler>();

        public int Created => All.Count;

        public IReadOnlyList<TrackedHandler> All
        {
            get
            {
                lock (created)
                {
                    return created.ToList();
                }
            }
        }

        public HttpMessageHandler Create()
        {
            var handler = new TrackedHandler(new SocketsHttpHandler());
            lock (created)
            {
                created.Add(handler);
            }
            return handler;
        }
    }

    // Counts how often it is disposed, and the requests being sent through it.
    public sealed class TrackedHandler : DelegatingHandler
    {
        private int disposals;
        private int sending;

        public TrackedHandler(HttpMessageHandler inner)
        {
            if (inner != null)
            {
                InnerHandler = inner;
            }
        }

        public int Disposals => Volatile.Read(ref disposals);

        public int Sending => Volatile.Read(ref sending);

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref sending);
            try
            {
                return await base.SendAsync(request, cancellationToken);
            }
            finally
            {
                Interlocked.Decrement(ref sending);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Interlocked.Increment(ref disposals);
            }
            base.Dispose(disposing);
        }
    }
}