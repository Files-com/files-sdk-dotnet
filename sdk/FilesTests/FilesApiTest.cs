using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System;
using System.Threading;
using System.Threading.Tasks;
using FilesCom;
using FilesCom.Models;
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

        [TestMethod]
        public async Task TestDoesNotFollowHttpsToHttpRedirectOnModernDotNet()
        {
            var handler = new RedirectingHttpMessageHandler("http://storage.example.test/download");
            var service = new FilesApiService(new CapturingHttpClientFactory(handler, "https://api.example.test"));
            var config = new FilesConfiguration
            {
                ApiKey = "my-key",
                BaseUrl = "https://api.example.test"
            };
            new FilesClient(config);

            await Assert.ThrowsExceptionAsync<ApiException>(() =>
                service.SendRequest("/files/test", HttpMethod.Get, new Dictionary<string, object>(), new Dictionary<string, object>())
            );

            Assert.AreEqual(1, handler.Requests.Count);
        }

    }

    [TestClass]
    public class FilesApiListTest
    {
        private static WireMockServer _server;
        private static string _baseUrl;
        private static FilesClient client;

        [ClassInitialize]
        public static void PrepareClass(TestContext context)
        {
            var port = new Random().Next(5000, 6000);
            _baseUrl = "http://localhost:" + port;

            _server = WireMockServer.Start(new WireMockServerSettings
            {
                Urls = new[] { "http://+:" + port },
                ReadStaticMappings = true
            });

            // client config
            FilesConfiguration filesConfig = new FilesConfiguration();
            filesConfig.ApiKey = "my-key";
            filesConfig.BaseUrl = _baseUrl;
            client = new FilesClient(filesConfig);
        }

        [ClassCleanup]
        public static void TearDown()
        {
            _server.Stop();
        }

        public void TestSimpleList()
        {

            // stub
            var bodyContent = new[] {
                new { path = "test.txt", type = "file", size = 81920, mtime = "2014-12-05T06:56:30+00:00", provided_mtime = "2013-11-05T06:56:30+00:00", crc32 = "a7a90a69", md5 = "d41d8cd98f00b204e9800998ecf8427e", permissions = "rwd" }
            };
            _server.Given(WireMock.RequestBuilders.Request.Create().WithPath("/api/rest/v1/folders/").UsingGet())
                            .RespondWith(
                                Response.Create()
                                .WithStatusCode(200)
                                .WithHeader("Content-Type", "application/json")
                                .WithBodyAsJson(bodyContent)
                            );

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
        public void TestNotFoundList()
        {
            _server
              .Given(WireMock.RequestBuilders.Request.Create().WithPath("/api/rest/v1/folders/notfound").UsingGet())
              .RespondWith(
                Response.Create()
                  .WithStatusCode(404)
                  .WithHeader("Content-Type", "application/json")
                  .WithBody(@"{""type"": ""not-found"",""http-code"": 404,""error"": ""Not Found.  This may be related to your permissions.""}")
              );

            Dictionary<string, object> parameters = new Dictionary<string, object>() { };
            var itemCount = 0;
            try{
                var files = Folder.ListFor("notfound", parameters, null);
                foreach (var file in files.ListAutoPaging())
                {
                    itemCount++;
                }
                Assert.Fail("no exception thrown");
            } catch (Exception ex) {
                Assert.AreEqual(typeof(NotFoundException), ex.InnerException.GetType());
                Assert.AreEqual("Not Found.  This may be related to your permissions.", ex.InnerException.Message);
            }

            // assert our count matches
            Assert.AreEqual(0, itemCount);
        }

        [TestMethod]
        public void TestEmptyList()
        {
            // stub
            _server.Given(WireMock.RequestBuilders.Request.Create().WithPath("/api/rest/v1/folders/empty").UsingGet())
                            .RespondWith(
                                Response.Create()
                                .WithStatusCode(200)
                                .WithHeader("Content-Type", "application/json")
                                .WithBody(@"[]")
                            );

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
