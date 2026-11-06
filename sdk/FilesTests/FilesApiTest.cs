using FilesCom;
using FilesCom.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace FilesTests
{
    public class ExposedFilesApiService : FilesApiService
    {
        private readonly IHttpClientFactory _clientFactory;
        public ExposedFilesApiService(IHttpClientFactory clientFactory) : base(clientFactory)
        {
            // _clientFactory = clientFactory;
        }

        public static string ExposedParsePathParameters(string path, Dictionary<string, object> parameters)
        {
            return ParsePathParameters(path, parameters);
        }
    }

    public class CapturingHttpMessageHandler : HttpMessageHandler
    {
        public HttpRequestMessage Request { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]")
            });
        }
    }

    public class CapturingHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpMessageHandler handler;
        private readonly string baseUrl;

        public CapturingHttpClientFactory(HttpMessageHandler handler, string baseUrl)
        {
            this.handler = handler;
            this.baseUrl = baseUrl;
        }

        public HttpClient CreateClient(string name)
        {
            HttpMessageHandler clientHandler = handler;
            if (name == FilesClient.HttpFilesApi)
            {
                clientHandler = new FilesRedirectHandler
                {
                    InnerHandler = handler
                };
            }

            return new HttpClient(clientHandler)
            {
                BaseAddress = new Uri(baseUrl)
            };
        }
    }

    // Answers every request with a not-found error whose body is exactly the given bytes, with the given charset.
    public class ErrorBodyHttpMessageHandler : HttpMessageHandler
    {
        private readonly byte[] body;
        private readonly string charset;

        public ErrorBodyHttpMessageHandler(byte[] body, string charset)
        {
            this.body = body;
            this.charset = charset;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new ByteArrayContent(body) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json") { CharSet = charset };
            return Task.FromResult(response);
        }
    }

    public class RedirectingHttpMessageHandler : HttpMessageHandler
    {
        private readonly Queue<Uri> redirectUris = new Queue<Uri>();
        private readonly HttpStatusCode redirectStatusCode;
        public List<HttpRequestMessage> Requests { get; } = new List<HttpRequestMessage>();

        public RedirectingHttpMessageHandler(params string[] redirectUrls)
            : this(HttpStatusCode.Redirect, redirectUrls)
        {
        }

        public RedirectingHttpMessageHandler(HttpStatusCode redirectStatusCode, params string[] redirectUrls)
        {
            this.redirectStatusCode = redirectStatusCode;
            foreach (string redirectUrl in redirectUrls)
            {
                redirectUris.Enqueue(new Uri(redirectUrl));
            }
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestSnapshot = new HttpRequestMessage(request.Method, request.RequestUri);
            foreach (var header in request.Headers)
            {
                requestSnapshot.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
            Requests.Add(requestSnapshot);

            if (redirectUris.Count > 0)
            {
                var redirect = new HttpResponseMessage(redirectStatusCode)
                {
                    Content = new StringContent("{\"error\":\"redirect\"}")
                };
                redirect.Headers.Location = redirectUris.Dequeue();
                return Task.FromResult(redirect);
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]")
            });
        }
    }

    [TestClass]
    public class FilesApiServiceTest
    {
        [TestMethod]
        public void ParsePathParameters()
        {
            var path = "/users/{id}.json";
            var parameters = new Dictionary<string, object>()
            {
                { "id", 1234 },
            };

            var resultPath = ExposedFilesApiService.ExposedParsePathParameters(path, parameters);

            Assert.AreEqual("/users/1234.json", resultPath);
        }

        [TestMethod]
        public async Task TestUsesConfiguredWorkspaceId()
        {
            var handler = new CapturingHttpMessageHandler();
            var service = new FilesApiService(new CapturingHttpClientFactory(handler, "http://example.test"));
            var config = new FilesConfiguration
            {
                ApiKey = "my-key",
                BaseUrl = "http://example.test",
                WorkspaceId = "123"
            };
            new FilesClient(config);

            await service.SendRequest("/api_keys", HttpMethod.Get, new Dictionary<string, object>(), new Dictionary<string, object>());

            Assert.AreEqual("123", new List<string>(handler.Request.Headers.GetValues("X-Files-Workspace-Id"))[0]);
        }

        [TestMethod]
        public async Task TestUsesPerCallWorkspaceId()
        {
            var handler = new CapturingHttpMessageHandler();
            var service = new FilesApiService(new CapturingHttpClientFactory(handler, "http://example.test"));
            var config = new FilesConfiguration
            {
                ApiKey = "my-key",
                BaseUrl = "http://example.test",
                WorkspaceId = "123"
            };
            new FilesClient(config);
            var options = new Dictionary<string, object>()
            {
                { "workspace_id", 456 },
            };

            await service.SendRequest("/api_keys", HttpMethod.Get, new Dictionary<string, object>(), options);

            Assert.AreEqual("456", new List<string>(handler.Request.Headers.GetValues("X-Files-Workspace-Id"))[0]);
        }

        [TestMethod]
        public async Task TestAllowsPerCallWorkspaceIdToBeCleared()
        {
            var handler = new CapturingHttpMessageHandler();
            var service = new FilesApiService(new CapturingHttpClientFactory(handler, "http://example.test"));
            var config = new FilesConfiguration
            {
                ApiKey = "my-key",
                BaseUrl = "http://example.test",
                WorkspaceId = "123"
            };
            new FilesClient(config);
            var options = new Dictionary<string, object>()
            {
                { "workspace_id", null },
            };

            await service.SendRequest("/api_keys", HttpMethod.Get, new Dictionary<string, object>(), options);

            Assert.IsFalse(handler.Request.Headers.Contains("X-Files-Workspace-Id"));
        }

        [TestMethod]
        public async Task TestCrossOriginRedirectStripsFilesAuthHeaders()
        {
            var authOptions = new[]
            {
                new Dictionary<string, object> { { "api_key", "my-key" } },
                new Dictionary<string, object> { { "session_id", "my-session" } }
            };

            foreach (var options in authOptions)
            {
                var handler = new RedirectingHttpMessageHandler(
                    "http://storage.example.test/download",
                    "http://api.example.test/returned"
                );
                var service = new FilesApiService(new CapturingHttpClientFactory(handler, "http://api.example.test"));
                var config = new FilesConfiguration
                {
                    ApiKey = "configured-key",
                    BaseUrl = "http://api.example.test",
                    WorkspaceId = "123"
                };
                new FilesClient(config);

                await service.SendRequest("/files/test", HttpMethod.Get, new Dictionary<string, object>(), options);

                Assert.AreEqual(3, handler.Requests.Count);
                Assert.IsTrue(
                    handler.Requests[0].Headers.Contains(options.ContainsKey("api_key") ? "X-FilesAPI-Key" : "X-FilesAPI-Auth")
                );
                Assert.IsTrue(handler.Requests[0].Headers.Contains("X-Files-Workspace-Id"));
                for (int i = 1; i < handler.Requests.Count; i++)
                {
                    Assert.IsFalse(handler.Requests[i].Headers.Contains("X-FilesAPI-Key"));
                    Assert.IsFalse(handler.Requests[i].Headers.Contains("X-FilesAPI-Auth"));
                    Assert.IsFalse(handler.Requests[i].Headers.Contains("X-Files-Workspace-Id"));
                }
            }
        }

        [TestMethod]
        public async Task TestSameOriginRedirectKeepsFilesAuthHeaders()
        {
            var redirectUrls = new string[50];
            for (int i = 0; i < redirectUrls.Length; i++)
            {
                redirectUrls[i] = "http://api.example.test/redirected";
            }
            var handler = new RedirectingHttpMessageHandler(HttpStatusCode.MultipleChoices, redirectUrls);
            var service = new FilesApiService(new CapturingHttpClientFactory(handler, "http://api.example.test"));
            var config = new FilesConfiguration
            {
                ApiKey = "my-key",
                BaseUrl = "http://api.example.test",
                WorkspaceId = "123"
            };
            new FilesClient(config);

            await service.SendRequest("/files/test", HttpMethod.Get, new Dictionary<string, object>(), new Dictionary<string, object>());

            Assert.AreEqual(51, handler.Requests.Count);
            Assert.AreEqual("my-key", new List<string>(handler.Requests[50].Headers.GetValues("X-FilesAPI-Key"))[0]);
            Assert.AreEqual("123", new List<string>(handler.Requests[50].Headers.GetValues("X-Files-Workspace-Id"))[0]);
        }

        // A redirect from HTTPS to HTTP is not followed on any runtime, .NET Framework included, whose own handler
        // follows it: a 307 would send the request again, its body included, in the clear.
        [TestMethod]
        public async Task TestDoesNotFollowHttpsToHttpRedirect()
        {
            var handler = new RedirectingHttpMessageHandler(HttpStatusCode.TemporaryRedirect, "http://storage.example.test/sessions");
            var service = new FilesApiService(new CapturingHttpClientFactory(handler, "https://api.example.test"));
            var config = new FilesConfiguration
            {
                ApiKey = "my-key",
                BaseUrl = "https://api.example.test"
            };
            new FilesClient(config);

            await Assert.ThrowsExactlyAsync<ApiException>(() =>
                service.SendRequest("/sessions", HttpMethod.Post, new Dictionary<string, object> { { "username", "bob" }, { "password", "secret" } }, new Dictionary<string, object>())
            );

            Assert.AreEqual(1, handler.Requests.Count, "The request was sent once, to the HTTPS address only.");
            Assert.AreEqual("https", handler.Requests[0].RequestUri.Scheme);
        }

        // Error bodies are decoded as HttpContent decodes text: by the declared charset, else by a byte order mark,
        // else as UTF-8. API, upload and download errors all read them the same way.
        [TestMethod]
        [DataRow("utf-8", "utf-8", false, DisplayName = "declared UTF-8")]
        [DataRow("utf-16", "utf-16", false, DisplayName = "declared UTF-16 without a byte order mark")]
        [DataRow("iso-8859-1", "iso-8859-1", false, DisplayName = "declared Latin-1")]
        [DataRow("utf-16", null, true, DisplayName = "UTF-16 byte order mark without a charset")]
        [DataRow("utf-8", null, false, DisplayName = "no charset or byte order mark")]
        public async Task ErrorResponsesKeepTheirTypeAndTextInTheirCharset(string encodingName, string charset, bool byteOrderMark)
        {
            Encoding encoding = Encoding.GetEncoding(encodingName);
            byte[] body = encoding.GetBytes(@"{""type"":""not-found"",""error"":""café"",""http-code"":404}");
            if (byteOrderMark)
            {
                body = encoding.GetPreamble().Concat(body).ToArray();
            }
            FilesApiService service = ServiceAnswering(body, charset);

            NotFoundException apiError = await Assert.ThrowsExactlyAsync<NotFoundException>(() =>
                service.SendRequest("/users/1", HttpMethod.Get, new Dictionary<string, object>(), new Dictionary<string, object>()));
            NotFoundException uploadError = await Assert.ThrowsExactlyAsync<NotFoundException>(() =>
                service.ChunkUpload(HttpMethod.Put, "http://example.test/part", new MemoryStream(new byte[] { 1 }), 1));
            NotFoundException downloadError = await Assert.ThrowsExactlyAsync<NotFoundException>(() =>
                service.StreamDownload("http://example.test/file", new MemoryStream()));

            Assert.AreEqual("café", apiError.Message);
            Assert.AreEqual("Transfer request failed (HTTP 404)", uploadError.Message);
            Assert.AreEqual("Transfer request failed (HTTP 404)", downloadError.Message);
        }

        [TestMethod]
        public async Task ErrorResponseWithAnUnknownCharsetFailsAsHttpContentDoes()
        {
            FilesApiService service = ServiceAnswering(Encoding.UTF8.GetBytes(@"{""type"":""not-found"",""error"":""Not Found""}"), "no-such-charset");

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() =>
                service.SendRequest("/users/1", HttpMethod.Get, new Dictionary<string, object>(), new Dictionary<string, object>()));
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() =>
                service.ChunkUpload(HttpMethod.Put, "http://example.test/part", new MemoryStream(new byte[] { 1 }), 1));
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() =>
                service.StreamDownload("http://example.test/file", new MemoryStream()));
        }

        // A storage server's error answer, which a caller's URL or a redirect can choose, is read only to its bound
        // (FilesApiService.StorageAnswerBytes), however long it goes on: a body that never ends is read to the bound and
        // the one byte more that shows it goes on, and nothing after; it is still the error of its status, with its
        // headers, which decide retrying and waiting; and whether it says its URL has expired is judged by its start.
        // This is the failure the transfer engine's downloads take (StorageFailureAsync).
        [TestMethod]
        [Timeout(60000)]
        [DataRow(403, "<Error><Code>AccessDenied</Code><Message>Request has expired</Message>", true)]
        [DataRow(403, "<Error><Code>AccessDenied</Code><Message>Access Denied</Message>", false)]
        [DataRow(503, "<Error><Code>SlowDown</Code><Message>Reduce your request rate</Message>", false)]
        public async Task AStorageErrorThatNeverEndsIsReadOnlyToItsBound(int status, string start, bool expired)
        {
            var body = new EndlessStream(Encoding.UTF8.GetBytes(start));
            using (HttpResponseMessage response = EndlessBodyHttpMessageHandler.Answer((HttpStatusCode)status, body))
            using (var watch = new FilesCom.Http.NetworkWatch(FilesCom.Http.NetworkSettings.From(new FilesConfiguration()), CancellationToken.None))
            {
                Exception failure = await FilesApiService.StorageFailureAsync(response, watch);

                InvalidResponseException error = failure as InvalidResponseException;
                Assert.IsNotNull(error, $"An error of the answer's status: {failure}");
                Assert.AreEqual(status, error.httpStatus);
                Assert.AreEqual(expired ? $"Transfer request failed (HTTP {status}): the storage server says the transfer URL has expired" : $"Transfer request failed (HTTP {status})", error.Message);
                Assert.AreEqual(expired, error.StorageSaysExpired, "Judged by the start of the body that was read.");
                AssertAnswerHeaders(error);
                Assert.IsTrue(body.BytesRead >= FilesApiService.StorageAnswerBytes && body.BytesRead <= FilesApiService.StorageAnswerBytes + 1, $"{body.BytesRead} bytes of the endless body were read.");
            }
        }

        // Every error answer the service reads is bounded the same way, on each of its paths: an API request's (to
        // FilesApiService.ApiErrorBytes, as a response too long to be a Files.com error), a legacy upload chunk's and
        // download's, and a transfer engine's upload part's (each to StorageAnswerBytes, as a transfer failure of its
        // status). Each fails with its status and headers once its bound is read, and its response is disposed.
        [TestMethod]
        [Timeout(60000)]
        public async Task ErrorAnswersThatNeverEndAreReadOnlyToTheirBoundsOnEveryPath()
        {
            var handler = new EndlessBodyHttpMessageHandler(HttpStatusCode.Forbidden, "{\"type\":\"not-authorized\",\"error\":\"");
            new FilesClient(new FilesConfiguration { ApiKey = "my-key", BaseUrl = "http://example.test", MaxNetworkRetries = 0 });
            var service = new FilesApiService(new CapturingHttpClientFactory(handler, "http://example.test"));

            InvalidResponseException api = await Assert.ThrowsExactlyAsync<InvalidResponseException>(() =>
                service.SendRequest("/users/1", HttpMethod.Get, new Dictionary<string, object>(), new Dictionary<string, object>()));
            Assert.AreEqual($"HTTP request failed with code 403, and its response is longer than the {FilesApiService.ApiErrorBytes} bytes an error response is read to", api.Message);
            AssertBoundedAnswer(handler, api, FilesApiService.ApiErrorBytes);

            InvalidResponseException chunk = await Assert.ThrowsExactlyAsync<InvalidResponseException>(() =>
                service.ChunkUpload(HttpMethod.Put, "http://example.test/part", new MemoryStream(new byte[] { 1 }), 1));
            Assert.AreEqual("Transfer request failed (HTTP 403)", chunk.Message);
            AssertBoundedAnswer(handler, chunk, FilesApiService.StorageAnswerBytes);

            InvalidResponseException download = await Assert.ThrowsExactlyAsync<InvalidResponseException>(() =>
                service.StreamDownload("http://example.test/file", new MemoryStream()));
            Assert.AreEqual("Transfer request failed (HTTP 403)", download.Message);
            AssertBoundedAnswer(handler, download, FilesApiService.StorageAnswerBytes);

            FilesCom.Http.NetworkSettings network = FilesCom.Http.NetworkSettings.From(new FilesConfiguration());
            UploadPartAttempt part = await service.SendUploadPart(network, HttpMethod.Put, "http://example.test/part", new KeyValuePair<string, string>[0], new ByteArrayContent(new byte[] { 1 }), CancellationToken.None);
            InvalidResponseException partError = part.Error as InvalidResponseException;
            Assert.IsNotNull(partError, $"An error of the answer's status: {part.Error}");
            Assert.AreEqual(403, part.StatusCode);
            Assert.AreEqual(TimeSpan.FromSeconds(7), part.RetryAfter, "The answer's Retry-After still decides the wait.");
            AssertBoundedAnswer(handler, partError, FilesApiService.StorageAnswerBytes);
        }

        // A body is read whole when it ends within the bound, exactly at it included, and cut at the bound when it holds
        // even one byte more; never more than one byte past the bound is read.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(9, false)]
        [DataRow(10, false)]
        [DataRow(11, true)]
        [DataRow(100000, true)]
        public async Task ABodyIsReadWholeWithinItsBoundAndCutJustPastIt(int length, bool truncated)
        {
            const int limit = 10;
            byte[] contents = Enumerable.Range(0, length).Select(i => (byte)i).ToArray();
            var body = new EndlessStream(contents, endsAfter: length);
            using (HttpResponseMessage response = EndlessBodyHttpMessageHandler.Answer(HttpStatusCode.Forbidden, body))
            using (var watch = new FilesCom.Http.NetworkWatch(FilesCom.Http.NetworkSettings.From(new FilesConfiguration()), CancellationToken.None))
            {
                (byte[] read, bool cut) = await FilesCom.Http.ResponseBody.ReadPrefixAsync(response, watch, limit);

                CollectionAssert.AreEqual(contents.Take(limit).ToArray(), read);
                Assert.AreEqual(truncated, cut);
                Assert.IsTrue(body.BytesRead <= limit + 1, $"{body.BytesRead} bytes were read.");
            }
        }

        private static void AssertAnswerHeaders(InvalidResponseException error)
        {
            Assert.IsTrue(error.httpHeaders.TryGetValues("x-amz-request-id", out IEnumerable<string> ids) && ids.Single() == "R45", "The answer's headers are kept.");
            Assert.AreEqual(TimeSpan.FromSeconds(7), ((HttpResponseHeaders)error.httpHeaders).RetryAfter?.Delta, "Its Retry-After is kept.");
        }

        private static void AssertBoundedAnswer(EndlessBodyHttpMessageHandler handler, InvalidResponseException error, int bound)
        {
            Assert.AreEqual(403, error.httpStatus);
            AssertAnswerHeaders(error);
            Assert.IsTrue(handler.LastBody.BytesRead >= bound && handler.LastBody.BytesRead <= bound + 1, $"{handler.LastBody.BytesRead} bytes of the endless body were read for a bound of {bound}.");
            Assert.IsTrue(handler.LastBody.Disposed, "The response was disposed.");
        }

        private static FilesApiService ServiceAnswering(byte[] errorBody, string charset)
        {
            new FilesClient(new FilesConfiguration { ApiKey = "my-key", BaseUrl = "http://example.test", MaxNetworkRetries = 0 });
            return new FilesApiService(new CapturingHttpClientFactory(new ErrorBodyHttpMessageHandler(errorBody, charset), "http://example.test"));
        }
    }

    // Answers every request with status and a body that starts with start and then never ends, as a hostile or broken
    // storage server can, with a request id and a Retry-After of 7 seconds; LastBody counts what was read of it.
    public class EndlessBodyHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode status;
        private readonly byte[] start;

        public EndlessBodyHttpMessageHandler(HttpStatusCode status, string start)
        {
            this.status = status;
            this.start = Encoding.UTF8.GetBytes(start);
        }

        public EndlessStream LastBody { get; private set; }

        public static HttpResponseMessage Answer(HttpStatusCode status, EndlessStream body)
        {
            var response = new HttpResponseMessage(status) { Content = new StreamContent(body) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
            response.Headers.Add("x-amz-request-id", "R45");
            response.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(7));
            return response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastBody = new EndlessStream(start);
            return Task.FromResult(Answer(status, LastBody));
        }
    }

    // A forward-only body of start followed by filler, without end unless endsAfter is given, which counts the bytes
    // read from it and whether it was disposed.
    public sealed class EndlessStream : Stream
    {
        private readonly byte[] start;
        private readonly long endsAfter;
        private long read;

        public EndlessStream(byte[] start, long endsAfter = long.MaxValue)
        {
            this.start = start;
            this.endsAfter = endsAfter;
        }

        public long BytesRead => Interlocked.Read(ref read);

        public bool Disposed { get; private set; }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int n = (int)Math.Min(count, endsAfter - read);
            for (int i = 0; i < n; i++)
            {
                long at = read + i;
                buffer[offset + i] = at < start.Length ? start[at] : (byte)'x';
            }
            Interlocked.Add(ref read, n);
            return n;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return Task.FromResult(Read(buffer, offset, count));
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            Disposed = true;
            base.Dispose(disposing);
        }
    }

    [TestClass]
    [DoNotParallelize]
    public class FilesApiListTest
    {
        private WireMockServer _server;
        private FilesClient client;
        private HttpClient simulator;
        private long simulatorEpoch;
        private string simulatorInstance;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public async Task PrepareTest()
        {
            // The nested-null sync regression keeps its deliberately shaped response. The ordinary folder-list
            // tests use the same simulator and canonical root-listing policy as the shared API scenarios.
            if (TestContext.TestName == nameof(TestSyncListWithInProgressSyncRun))
            {
                _server = WireMockServer.Start(new WireMockServerSettings { Urls = new[] { "http://127.0.0.1:0" }, ReadStaticMappings = true });
                client = new FilesClient(new FilesConfiguration { ApiKey = "my-key", BaseUrl = _server.Urls[0] });
                return;
            }

            string Required(string name) => Environment.GetEnvironmentVariable(name) is string value && value.Length > 0
                ? value : throw new InvalidOperationException($"Run the list tests through with-scenario.rb; {name} is missing.");
            string host = Required("FILES_MOCK_SERVER_HOST");
            string port = Required("FILES_MOCK_SERVER_PORT");
            string origin = Required("FILES_MOCK_SERVER_URL");
            string schema = Required("FILES_MOCK_SCHEMA_SHA256");
            string instance = Required("FILES_MOCK_INSTANCE");
            string key = Required("FILES_MOCK_API_KEY");
            Assert.IsTrue(IPAddress.TryParse(host, out IPAddress address) && IPAddress.IsLoopback(address));
            Assert.IsTrue(int.TryParse(port, out int number) && number >= 1 && number <= 65535);
            Assert.AreEqual(new UriBuilder("http", host, number).Uri.GetLeftPart(UriPartial.Authority), origin);
            Assert.IsTrue(System.Text.RegularExpressions.Regex.IsMatch(schema, "\\A[0-9a-f]{64}\\z"));
            Assert.IsTrue(System.Text.RegularExpressions.Regex.IsMatch(instance, "\\A[0-9a-f]{12}\\z"));
            Assert.IsTrue(System.Text.RegularExpressions.Regex.IsMatch(key, "\\A[A-Za-z0-9]+\\z"));
            simulator = new HttpClient(new HttpClientHandler { UseProxy = false, AllowAutoRedirect = false })
            {
                BaseAddress = new Uri(origin),
                Timeout = TimeSpan.FromSeconds(10),
                MaxResponseContentBufferSize = 1024 * 1024,
            };
            simulator.DefaultRequestHeaders.UserAgent.ParseAdd("files-dotnet-list-fixture/1");

            var before = await Control("ready");
            void CheckReady(System.Text.Json.Nodes.JsonNode ready)
            {
                Assert.AreEqual("ready", ready["status"].GetValue<string>());
                Assert.AreEqual("simulation", ready["mode"].GetValue<string>());
                Assert.AreEqual(3, ready["contract_version"].GetValue<int>());
                Assert.AreEqual(schema, ready["schema_sha256"].GetValue<string>());
                Assert.AreEqual(instance, ready["instance"].GetValue<string>());
            }
            CheckReady(before);
            var preset = System.Text.Json.Nodes.JsonNode.Parse(System.IO.File.ReadAllText(Required("FILES_MOCK_SCENARIO_FILE")));
            Assert.AreEqual("api-scenarios-canonical", preset["name"].GetValue<string>());
            var reset = preset["reset"].DeepClone();
            reset["fixtures"] = System.Text.Json.Nodes.JsonNode.Parse(TestContext.TestName == nameof(TestSimpleList)
                ? "{\"files\":[{\"path\":\"test.txt\",\"text\":\"list fixture\"}]}"
                : TestContext.TestName == nameof(TestEmptyList) ? "{\"folders\":[\"empty\"]}" : "{}");
            var answer = await Control("reset", reset);
            var after = await Control("ready");
            CheckReady(after);
            Assert.AreEqual(before["epoch"].GetValue<long>() + 1, answer["epoch"].GetValue<long>());
            Assert.AreEqual(answer["epoch"].GetValue<long>(), after["epoch"].GetValue<long>());
            Assert.IsTrue(System.Text.Json.Nodes.JsonNode.DeepEquals(reset["profile"]["api_scenarios"], after["profile"]["api_scenarios"]));
            simulatorEpoch = after["epoch"].GetValue<long>();
            simulatorInstance = instance;
            if (TestContext.TestName == nameof(TestNotFoundList))
            {
                using (var fault = await simulator.PostAsync("/__files_mock/v1/faults", new StringContent(
                    "{\"operation\":\"folders.list\",\"match\":{\"path\":\"notfound\"},\"status\":404,\"type\":\"not-found\",\"message\":\"Not Found.  This may be related to your permissions.\"}", Encoding.UTF8, "application/json")))
                {
                    Assert.AreEqual(HttpStatusCode.Created, fault.StatusCode);
                }
            }
            client = new FilesClient(new FilesConfiguration { ApiKey = key, BaseUrl = origin });
        }

        private async Task<System.Text.Json.Nodes.JsonNode> Control(string name, System.Text.Json.Nodes.JsonNode reset = null)
        {
            using (HttpResponseMessage response = reset == null ? await simulator.GetAsync("/__files_mock/v1/" + name)
                : await simulator.PostAsync("/__files_mock/v1/" + name, new StringContent(reset.ToJsonString(), Encoding.UTF8, "application/json")))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                return System.Text.Json.Nodes.JsonNode.Parse(await response.Content.ReadAsStringAsync());
            }
        }

        [TestCleanup]
        public async Task TearDown()
        {
            try
            {
                if (simulator != null && simulatorEpoch > 0)
                {
                    var journal = await Control("journal");
                    Assert.IsTrue(journal["complete"].GetValue<bool>());
                    Assert.AreEqual(0, journal["dropped"].GetValue<int>());
                    Assert.AreEqual(simulatorEpoch, journal["epoch"].GetValue<long>());
                    Assert.AreEqual(simulatorInstance, journal["instance"].GetValue<string>());
                    var calls = journal["entries"].AsArray().Where(entry => entry["operation"]?.GetValue<string>() == "folders.list").ToArray();
                    Assert.AreEqual(1, calls.Length, "The static SDK list call reached the shared simulator once.");
                    Assert.AreEqual(TestContext.TestName == nameof(TestNotFoundList) ? 404 : 200, calls[0]["status"].GetValue<int>());
                    if (TestContext.TestName == nameof(TestNotFoundList))
                    {
                        Assert.IsNotNull(calls[0]["fault_id"], "The queued error was consumed by the SDK call.");
                    }
                }
            }
            finally
            {
                client?.Dispose();
                simulator?.Dispose();
                _server?.Stop();
            }
        }

        [TestMethod]
        [TestCategory("SharedSimulatorList")]
        public void TestSimpleList()
        {

            Dictionary<string, object> parameters = new Dictionary<string, object>() { };
            var files = Folder.ListFor("/", parameters, null);
            var itemCount = 0;
            foreach (var file in files.ListAutoPaging())
            {
                itemCount++;
            }

            // assert our count matches
            Assert.AreEqual(1, itemCount);
        }

        [TestMethod]
        [TestCategory("SharedSimulatorList")]
        public void TestNotFoundList()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>() { };
            var itemCount = 0;
            try
            {
                var files = Folder.ListFor("notfound", parameters, null);
                foreach (var file in files.ListAutoPaging())
                {
                    itemCount++;
                }
                Assert.Fail("no exception thrown");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(typeof(NotFoundException), ex.InnerException.GetType());
                Assert.AreEqual("Not Found.  This may be related to your permissions.", ex.InnerException.Message);
            }

            // assert our count matches
            Assert.AreEqual(0, itemCount);
        }

        [TestMethod]
        [TestCategory("SharedSimulatorList")]
        public void TestEmptyList()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>() { };
            var files = Folder.ListFor("empty", parameters, null);
            var itemCount = 0;
            foreach (var file in files.ListAutoPaging())
            {
                itemCount++;
            }

            // assert our count matches
            Assert.AreEqual(0, itemCount);
        }

        [TestMethod]
        public void TestSyncListWithInProgressSyncRun()
        {
            _server
                .Given(WireMock.RequestBuilders.Request.Create().WithPath("/api/rest/v1/syncs").UsingGet())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(@"[{""id"":3803,""name"":""Batchsync"",""latest_sync_run"":{""id"":41970631,""completed_at"":null,""log_url"":null,""runtime"":null,""status"":""in_progress"",""successful_files"":0}}]")
                );

            var syncs = Sync.List(null, null).All().Result;

            Assert.AreEqual(1, syncs.Count);
            Assert.AreEqual(3803, syncs[0].Id);
            Assert.IsNull(syncs[0].LatestSyncRun.Runtime);
        }
    }
}