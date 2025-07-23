using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System;
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
    }
}
