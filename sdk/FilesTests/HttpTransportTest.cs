using FilesCom;
using FilesCom.Models;
using FilesCom.Transfers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WireMock;

namespace FilesTests
{
    // A client given an HttpTransport sends every request through its handlers: API requests, downloads, and the parts
    // and final request of both kinds of upload. The SDK's own rules stay above them: redirects and their credential
    // headers, retries, and the single final request of an upload, so each request reaches the handlers once per
    // attempt, exactly as the SDK sends it.
    [TestClass]
    public class HttpTransportTest
    {
        private static readonly DateTime MTime = new DateTime(2026, 9, 24, 0, 0, 0, DateTimeKind.Utc);
        private UploadServer server;

        [TestInitialize]
        public void Start()
        {
            server = new UploadServer();
            server.Site.ServeUsers();
            server.Site.On("GET", "/files/download.bin", request => LoopbackSite.Json($"{{\"path\":\"download.bin\",\"download_uri\":\"{server.Site.Url}/download?X-Files-Signature=stand-in\"}}"));
            server.Site.On("GET", "//download", request => LoopbackSite.Json("\"contents\""));
            server.Site.On("POST", "/file_actions/begin_upload/legacy.bin", request => LoopbackSite.Json($"[{{\"upload_uri\":\"{server.Site.Url}/legacy-part\",\"http_method\":\"PUT\",\"partsize\":8,\"ref\":\"legacy-ref\"}}]"), priority: 1);
            server.Site.On("PUT", "//legacy-part", request => LoopbackSite.Status(200));
        }

        [TestCleanup]
        public void Stop()
        {
            server.Dispose();
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task EveryRequestGoesThroughTheTransport()
        {
            var seen = new SeenRequests();
            FilesClient client = FilesClient.Create(server.Site.Configuration(), HttpTransport.FromHandlerFactory(() => new RecordingHandler(seen, new SocketsHttpHandler())));

            await RunEveryKindOfRequest(client);

            CollectionAssert.AreEqual(
                server.Site.Requests.Select(request => request.Method + " " + request.Path.Split('?')[0]).ToArray(),
                seen.All.Select(request => request.Method + " " + request.Url.AbsolutePath.Replace("/api/rest/v1", "")).ToArray(),
                "The handlers saw every request the site received, in the same order.");
            Assert.IsTrue(seen.All.Where(request => !request.Url.AbsolutePath.StartsWith("/api/")).All(request => !request.HasFilesCredentials), "Storage requests carry no Files.com credentials.");
            PartRequest part = server.Parts.First();
            Assert.AreEqual(server.PartUrl("remote.bin", part.Number, 1), seen.All.First(request => request.Url.AbsolutePath.StartsWith("/part/")).OriginalUrl, "Signed URLs reach the handlers exactly as negotiated.");
            CollectionAssert.AreEqual(
                server.Site.Requests.Select(request => ClientConfigurationTest.SdkProduct + ClientConfigurationTest.Metadata()).ToArray(),
                server.Site.Requests.Select(request => request.Headers["User-Agent"]).ToArray(),
                "Every request, to Files.com and to storage, of the download and of both kinds of upload, carries the SDK's own User-Agent with its metadata, once.");
            await client.DisposeAsync();
        }

        // A User-Agent the application or Files.com sets is sent as it is, without the SDK's metadata, however much it
        // looks like the SDK's: the one the application's handler gives every request, and the one Files.com negotiates
        // for a part, which goes to storage with that part alone, the client's own going everywhere else. Storage still
        // gets no Files.com credential.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("handler", DisplayName = "Set by the application's handler")]
        [DataRow("grant", DisplayName = "Negotiated for a part")]
        public async Task AUserAgentSetOutsideTheSdkIsSentAsItIs(string setBy)
        {
            const string literal = "Files.com DOTNET SDK v9.9.9 (set outside the SDK)";
            HttpTransport transport = null;
            if (setBy == "handler")
            {
                transport = HttpTransport.FromHandlerFactory(() => new UserAgentHandler(literal, new SocketsHttpHandler()));
            }
            else
            {
                server.ExtraPartFields = $",\"headers\":{{\"User-Agent\":\"{literal}\"}}";
            }
            FilesClient client = FilesClient.Create(server.Site.Configuration(), transport);

            await RunEveryKindOfRequest(client);

            string sdk = ClientConfigurationTest.SdkProduct + ClientConfigurationTest.Metadata();
            IReadOnlyList<ReceivedRequest> requests = server.Site.Requests;
            Assert.IsTrue(requests.Any(request => request.Path.StartsWith("/part/", StringComparison.Ordinal)), "The negotiated part was sent.");
            foreach (ReceivedRequest request in requests)
            {
                string path = request.Path.Split('?')[0];
                bool negotiatedPart = path.StartsWith("/part/", StringComparison.Ordinal);
                Assert.AreEqual(setBy == "handler" || negotiatedPart ? literal : sdk, request.Headers["User-Agent"], $"{request.Method} {path}");
                if (negotiatedPart || path == "/legacy-part" || path == "/download")
                {
                    Assert.IsTrue(request.ApiKey == null && request.SessionId == null, $"{request.Method} {path} carries no Files.com credentials.");
                }
            }
            await client.DisposeAsync();
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task ProxyCarriesEveryRequest()
        {
            using (var proxy = new ForwardingProxy(server.Site.Port))
            {
                // Hosts only the proxy knows; the requests reach the site through it.
                server.UploadUri = (file, number, negotiation) => server.PartUrl(file, number, negotiation).Replace(server.Site.Url, "http://storage.example.test");
                server.Site.On("GET", "/files/download.bin", request => LoopbackSite.Json("{\"path\":\"download.bin\",\"download_uri\":\"http://storage.example.test/download\"}"), priority: 1);
                server.Site.On("POST", "/file_actions/begin_upload/legacy.bin", request => LoopbackSite.Json("[{\"upload_uri\":\"http://storage.example.test/legacy-part\",\"http_method\":\"PUT\",\"partsize\":8,\"ref\":\"legacy-ref\"}]"), priority: 0);
                FilesConfiguration configuration = server.Site.Configuration();
                configuration.BaseUrl = "http://files.example.test";
                FilesClient client = FilesClient.Create(configuration, HttpTransport.FromHandlerFactory(() => new HttpClientHandler { Proxy = new WebProxy(proxy.Url), UseProxy = true }));

                await RunEveryKindOfRequest(client);

                CollectionAssert.AreEqual(
                    new[]
                    {
                        "GET http://files.example.test/api/rest/v1/users/1",
                        "GET http://files.example.test/api/rest/v1/files/download.bin",
                        "GET http://storage.example.test/download",
                        "POST http://files.example.test/api/rest/v1/file_actions/begin_upload/legacy.bin",
                        "PUT http://storage.example.test/legacy-part",
                        "POST http://files.example.test/api/rest/v1/files/legacy.bin",
                        "POST http://files.example.test/api/rest/v1/file_actions/begin_upload/remote.bin",
                        "PUT http://storage.example.test/part/remote.bin/1",
                        "POST http://files.example.test/api/rest/v1/files/remote.bin",
                    },
                    proxy.Requests.Select(request => request.Split('?')[0]).ToArray());
                await client.DisposeAsync();
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(false, DisplayName = "To another origin")]
        [DataRow(true, DisplayName = "To the same origin")]
        public async Task RedirectsAreFollowedAboveTheTransport(bool sameOrigin)
        {
            var redirecting = new RedirectingHttpMessageHandler(sameOrigin ? "http://api.example.test/moved" : "http://storage.example.test/moved");
            var configuration = new FilesConfiguration { BaseUrl = "http://api.example.test", ApiKey = "key", WorkspaceId = "11", MaxNetworkRetries = 0 };
            FilesClient client = FilesClient.Create(configuration, HttpTransport.FromHandlerFactory(() => redirecting));

            await client.Users.List().LoadNextPageAsync();

            Assert.AreEqual(2, redirecting.Requests.Count, "The SDK followed the redirect, one request at a time through the transport.");
            Assert.IsTrue(redirecting.Requests[0].Headers.Contains("X-FilesAPI-Key"));
            Assert.AreEqual(sameOrigin, redirecting.Requests[1].Headers.Contains("X-FilesAPI-Key"), "Credentials go with a redirect only within the same origin.");
            Assert.AreEqual(sameOrigin, redirecting.Requests[1].Headers.Contains("X-Files-Workspace-Id"));
            await client.DisposeAsync();
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task EachAttemptReachesTheTransportOnce()
        {
            var seen = new SeenRequests();
            server.Site.On("GET", "/users/9", request => LoopbackSite.Status(503), priority: 1);
            server.RespondToFinish = body => Task.FromResult(LoopbackSite.Status(503));
            FilesClient client = FilesClient.Create(server.Site.Configuration(maxNetworkRetries: 2), HttpTransport.FromHandlerFactory(() => new RecordingHandler(seen, new SocketsHttpHandler())));

            await Assert.ThrowsExactlyAsync<ApiException>(() => client.Users.FindAsync(9));
            await Assert.ThrowsExactlyAsync<UploadCompletionUnknownException>(() => client.Transfers.UploadStreamAsync(new MemoryStream(new byte[8]), 8, "remote.bin"));

            Assert.AreEqual(3, seen.All.Count(request => request.Url.AbsolutePath == "/api/rest/v1/users/9"), "The request and its two retries, each once.");
            Assert.AreEqual(1, seen.All.Count(request => request.Method == "POST" && request.Url.AbsolutePath == "/api/rest/v1/files/remote.bin"), "The final request of an upload is sent once.");
            Assert.AreEqual(1, server.Finishes.Count);
            await client.DisposeAsync();
        }

        // What an application's handler or handler factory throws is the application's: the SDK throws it on as it is
        // and, below DEBUG, logs only what kind of exception it is. Its type proves nothing about its message, which can
        // quote URLs and credentials, even when it is one of the SDK's own types. A connection failure, which the SDK
        // maps, is the exception.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("/api/rest/v1/users/1", "ApiException", "shared", 1, "API request failed (ApiException, HTTP 501)", DisplayName = "API request: an SDK exception type")]
        [DataRow("/download", "ApiException", "shared", 1, "Download request failed (ApiException, HTTP 501)", DisplayName = "Download: an SDK exception type")]
        [DataRow("/legacy-part", "ApiException", "shared", 1, "Upload request failed (ApiException, HTTP 501)", DisplayName = "Legacy upload part: an SDK exception type")]
        [DataRow("/part/", "ApiException", "shared", 1, null, DisplayName = "Upload part: an SDK exception type")]
        [DataRow("/api/rest/v1/users/1", "NetworkTimeoutException", "shared", 2, "Retrying GET /users after a timeout: retry 1 of 1, in 0 seconds", DisplayName = "API request: a timeout, retried")]
        [DataRow("/api/rest/v1/users/1", "IOException", "factory", 1, "Failed to send Files API Request (IOException)", DisplayName = "API request: another exception, from a handler the client owns")]
        [DataRow("/api/rest/v1/users/1", "IOException", "failing factory", 0, "Failed to send Files API Request (IOException)", DisplayName = "API request: the handler factory fails")]
        [DataRow("/api/rest/v1/users/1", "HttpRequestException", "factory", 2, "Failed to send Files API Request (ApiConnectionException)", DisplayName = "API request: a connection failure, retried and mapped")]
        public async Task ApplicationFailuresAreThrownAsTheyAreAndNotQuotedInLogs(string failingPath, string failureType, string transport, int failedSends, string logged)
        {
            const string secret = "https://user:sentinel-secret@callback.example.test/path?token=sentinel-secret";
            Exception failure =
                failureType == "ApiException" ? new ApiException($"The transport refused {secret}", 501)
                : failureType == "NetworkTimeoutException" ? new NetworkTimeoutException($"No answer from {secret}")
                : failureType == "IOException" ? new IOException("The transport failed.", new ApplicationException(secret))
                : (Exception)new HttpRequestException("The transport failed.", new IOException(secret));
            var handlers = new List<FailingHandler>();
            int factoryCalls = 0;
            HttpTransport httpTransport =
                transport == "shared" ? HttpTransport.FromSharedHandler(Add(handlers, new FailingHandler(failingPath, failure)))
                : HttpTransport.FromHandlerFactory(() =>
                {
                    factoryCalls++;
                    return transport == "factory" ? Add(handlers, new FailingHandler(failingPath, failure)) : throw failure;
                });
            FilesClient client = FilesClient.Create(server.Site.Configuration(maxNetworkRetries: 1), httpTransport);

            Exception thrown;
            using (var logs = new LogCapture())
            {
                thrown = await Assert.ThrowsAsync<Exception>(() =>
                    failingPath == "/download" ? client.RemoteFiles.DownloadFileAsync("download.bin", new MemoryStream())
                    : failingPath == "/legacy-part" ? client.RemoteFiles.UploadFileAsync("legacy.bin", new MemoryStream(new byte[8]), 8, MTime)
                    : failingPath == "/part/" ? client.Transfers.UploadStreamAsync(new MemoryStream(new byte[8]), 8, "remote.bin")
                    : (Task)client.Users.FindAsync(1));
                await client.DisposeAsync();

                Assert.IsFalse(logs.Normal.Contains("sentinel"), logs.Normal);
                StringAssert.Contains(logs.Debug, "sentinel-secret", "The details stay available at DEBUG.");
                if (logged != null)
                {
                    CollectionAssert.Contains(logs.Lines(logged), logged);
                }
            }

            if (failure is HttpRequestException)
            {
                Assert.IsInstanceOfType(thrown, typeof(ApiConnectionException));
            }
            else
            {
                Assert.AreSame(failure, thrown);
            }
            Assert.AreEqual(failedSends, handlers.Sum(handler => handler.Failed), "Requests the transport failed.");
            Assert.AreEqual(transport == "shared" ? 0 : 1, factoryCalls);
            Assert.AreEqual(transport == "factory" ? 1 : 0, handlers.Sum(handler => handler.Disposals), "The client disposes the handlers it owns, and only those.");
            Assert.AreEqual(0, server.Finishes.Count, "An upload whose part failed is not finished.");
            handlers.ForEach(handler => handler.Dispose());
        }

        private static async Task RunEveryKindOfRequest(FilesClient client)
        {
            Assert.AreEqual(1, (await client.Users.FindAsync(1)).Id);
            var downloaded = new MemoryStream();
            await client.RemoteFiles.DownloadFileAsync("download.bin", downloaded);
            Assert.AreEqual("\"contents\"", Encoding.UTF8.GetString(downloaded.ToArray()));
            await client.RemoteFiles.UploadFileAsync("legacy.bin", new MemoryStream(new byte[8]), 8, MTime);
            await client.Transfers.UploadStreamAsync(new MemoryStream(new byte[8]), 8, "remote.bin");
        }

        private sealed class SeenRequest
        {
            public string Method;
            public Uri Url;
            public string OriginalUrl;
            public bool HasFilesCredentials;
        }

        private sealed class SeenRequests
        {
            private readonly List<SeenRequest> seen = new List<SeenRequest>();

            public IReadOnlyList<SeenRequest> All
            {
                get
                {
                    lock (seen)
                    {
                        return seen.ToList();
                    }
                }
            }

            public void Add(HttpRequestMessage request)
            {
                lock (seen)
                {
                    seen.Add(new SeenRequest
                    {
                        Method = request.Method.Method,
                        Url = request.RequestUri,
                        OriginalUrl = request.RequestUri.OriginalString,
                        HasFilesCredentials = request.Headers.Contains("X-FilesAPI-Key") || request.Headers.Contains("X-FilesAPI-Auth"),
                    });
                }
            }
        }

        private static FailingHandler Add(List<FailingHandler> handlers, FailingHandler handler)
        {
            handlers.Add(handler);
            return handler;
        }

        // An application's handler that sends requests on, but fails those to paths starting with failingPath, with
        // failure, as its own code might.
        private sealed class FailingHandler : DelegatingHandler
        {
            private readonly string failingPath;
            private readonly Exception failure;
            private int failed;
            private int disposals;

            public FailingHandler(string failingPath, Exception failure) : base(new SocketsHttpHandler { AllowAutoRedirect = false })
            {
                this.failingPath = failingPath;
                this.failure = failure;
            }

            public int Failed => Volatile.Read(ref failed);

            public int Disposals => Volatile.Read(ref disposals);

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request.RequestUri.AbsolutePath.StartsWith(failingPath, StringComparison.Ordinal))
                {
                    Interlocked.Increment(ref failed);
                    return Task.FromException<HttpResponseMessage>(failure);
                }
                return base.SendAsync(request, cancellationToken);
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

        // Instrumentation: records each request it passes on.
        private sealed class RecordingHandler : DelegatingHandler
        {
            private readonly SeenRequests seen;

            public RecordingHandler(SeenRequests seen, HttpMessageHandler inner) : base(inner)
            {
                this.seen = seen;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                seen.Add(request);
                return base.SendAsync(request, cancellationToken);
            }
        }

        // An application's handler that sends every request with a User-Agent of its own, in place of the client's.
        private sealed class UserAgentHandler : DelegatingHandler
        {
            private readonly string userAgent;

            public UserAgentHandler(string userAgent, HttpMessageHandler inner) : base(inner)
            {
                this.userAgent = userAgent;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Remove("User-Agent");
                request.Headers.TryAddWithoutValidation("User-Agent", userAgent);
                return base.SendAsync(request, cancellationToken);
            }
        }

        // An HTTP proxy on loopback that sends every request it is given to the site on sitePort, whatever host it
        // names, and records each as "METHOD absolute-url".
        private sealed class ForwardingProxy : IDisposable
        {
            private readonly ScriptedServer listener;
            private readonly List<string> requests = new List<string>();

            public ForwardingProxy(int sitePort)
            {
                listener = new ScriptedServer(async connection =>
                {
                    while (true)
                    {
                        string requestLine = await connection.ReadRequestHeadAsync();
                        string[] parts = requestLine.Split(' ');
                        lock (requests)
                        {
                            requests.Add(parts[0] + " " + parts[1]);
                        }
                        byte[] body = await connection.ReadBodyAsync();
                        await connection.SendAsync(await ForwardAsync(sitePort, parts[0], new Uri(parts[1]).PathAndQuery, connection.HeaderLines, body));
                    }
                });
            }

            public Uri Url => new Uri($"http://127.0.0.1:{listener.Port}");

            public string[] Requests
            {
                get
                {
                    lock (requests)
                    {
                        return requests.ToArray();
                    }
                }
            }

            public void Dispose()
            {
                listener.Dispose();
            }

            private static async Task<byte[]> ForwardAsync(int sitePort, string method, string pathAndQuery, string[] headerLines, byte[] body)
            {
                using (var site = new TcpClient())
                {
                    await site.ConnectAsync(IPAddress.Loopback, sitePort);
                    NetworkStream stream = site.GetStream();
                    var head = new StringBuilder($"{method} {pathAndQuery} HTTP/1.1\r\nHost: 127.0.0.1:{sitePort}\r\nConnection: close\r\n");
                    foreach (string line in headerLines.Where(line => !line.StartsWith("Host:", StringComparison.OrdinalIgnoreCase) && !line.StartsWith("Proxy-Connection:", StringComparison.OrdinalIgnoreCase) && !line.StartsWith("Connection:", StringComparison.OrdinalIgnoreCase)))
                    {
                        head.Append(line).Append("\r\n");
                    }
                    byte[] headBytes = Encoding.ASCII.GetBytes(head.Append("\r\n").ToString());
                    await stream.WriteAsync(headBytes, 0, headBytes.Length);
                    await stream.WriteAsync(body, 0, body.Length);
                    var response = new MemoryStream();
                    await stream.CopyToAsync(response);
                    return response.ToArray();
                }
            }
        }
    }
}