using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FilesCom;
using WireMock;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Types;
using WireMock.Util;
using Request = WireMock.RequestBuilders.Request;

namespace FilesTests
{
    // A stand-in for one Files.com site on a loopback port, with its own API key and workspace. Routes answer
    // with whatever a test's handler returns; every request is recorded in arrival order, with the credentials it
    // carried, so tests can check which site and principal each request of a journey used.
    public sealed class LoopbackSite : IDisposable
    {
        private readonly WireMockServer server = WireMockServer.Start();
        private readonly List<ReceivedRequest> requests = new List<ReceivedRequest>();

        public LoopbackSite(string apiKey, string workspaceId)
        {
            ApiKey = apiKey;
            WorkspaceId = workspaceId;
        }

        public string ApiKey { get; }

        public string WorkspaceId { get; }

        public string Url => server.Urls[0];

        public IReadOnlyList<ReceivedRequest> Requests
        {
            get
            {
                lock (requests)
                {
                    return requests.ToList();
                }
            }
        }

        public FilesConfiguration Configuration(int maxNetworkRetries = 0)
        {
            return new FilesConfiguration
            {
                BaseUrl = Url,
                ApiKey = ApiKey,
                WorkspaceId = WorkspaceId,
                MaxNetworkRetries = maxNetworkRetries,
                InitialNetworkRequestDelay = 0,
                MaxNetworkRetryDelay = 0,
            };
        }

        // Answers method requests to path, which is relative to /api/rest/v1 unless it starts with "//" and may end
        // with a "*" wildcard. When several routes match, the one with the lowest priority number answers.
        public void On(string method, string path, Func<ReceivedRequest, ResponseMessage> respond, int priority = 5)
        {
            On(method, path, request => Task.FromResult(respond(request)), priority);
        }

        public void On(string method, string path, Func<ReceivedRequest, Task<ResponseMessage>> respond, int priority = 5)
        {
            string fullPath = path.StartsWith("//") ? path.Substring(1) : "/api/rest/v1" + path;
            server.Given(Request.Create().WithPath(fullPath).UsingMethod(method))
                .AtPriority(priority)
                .RespondWith(Response.Create().WithCallback(message => respond(Record(message))));
        }

        // Standard routes for User: find, update, create and a two-page list.
        public void ServeUsers()
        {
            On("GET", "/users/*", request => Json($"{{\"id\":{request.Id},\"username\":\"user\"}}"), priority: 10);
            On("PATCH", "/users/*", request => Json($"{{\"id\":{request.Id},\"username\":\"changed\"}}"), priority: 10);
            On("POST", "/users", request => Json("{\"id\":7,\"username\":\"new\"}"), priority: 10);
            On("GET", "/users", request => request.Path.Contains("cursor=page-2")
                ? Json("[{\"id\":2,\"username\":\"second\"}]")
                : Json("[{\"id\":1,\"username\":\"first\"}]", cursor: "page-2"), priority: 10);
        }

        // Routes for a RemoteFile upload of remote.bin in parts of partsize bytes, sent to this site's /part.
        public void ServeUpload(int partsize, Action onPart = null)
        {
            On("POST", "/file_actions/begin_upload/remote.bin", request => Json($"[{{\"upload_uri\":\"{Url}/part\",\"http_method\":\"PUT\",\"partsize\":{partsize},\"ref\":\"upload-ref\"}}]"));
            On("PUT", "//part", request =>
            {
                onPart?.Invoke();
                return Status(200);
            });
            On("POST", "/files/remote.bin", request => Json("{\"path\":\"remote.bin\",\"size\":10}"));
        }

        // Routes for downloading remote.bin: its details, then its contents from this site's /download.
        public void ServeDownload(byte[] contents)
        {
            On("GET", "/files/remote.bin", request => Json($"{{\"path\":\"remote.bin\",\"download_uri\":\"{Url}/download\"}}"));
            On("GET", "//download", request => new ResponseMessage
            {
                StatusCode = 200,
                BodyData = new BodyData { BodyAsBytes = contents, DetectedBodyType = BodyType.Bytes },
            });
        }

        public void AssertReceived(params string[] expected)
        {
            Assert.AreEqual(string.Join("\n", expected), string.Join("\n", Requests), "Requests received by the site at " + Url);
        }

        public static ResponseMessage Json(string json, string cursor = null, int status = 200)
        {
            var response = new ResponseMessage
            {
                StatusCode = status,
                BodyData = new BodyData { BodyAsString = json, DetectedBodyType = BodyType.String, Encoding = Encoding.UTF8 },
            };
            response.AddHeader("Content-Type", "application/json");
            if (cursor != null)
            {
                response.AddHeader("X-Files-Cursor", cursor);
            }
            return response;
        }

        public static ResponseMessage Status(int status)
        {
            return Json(status < 400 ? "{}" : "{\"error\":\"failed\"}", status: status);
        }

        public void Dispose()
        {
            server.Stop();
        }

        private ReceivedRequest Record(IRequestMessage message)
        {
            var request = new ReceivedRequest(this, message);
            lock (requests)
            {
                requests.Add(request);
            }
            return request;
        }
    }

    public sealed class ReceivedRequest
    {
        internal ReceivedRequest(LoopbackSite site, IRequestMessage message)
        {
            Site = site;
            Method = message.Method;
            Path = message.Path.Replace("/api/rest/v1", "") + (string.IsNullOrEmpty(message.RawQuery) ? "" : "?" + message.RawQuery.TrimStart('?'));
            ApiKey = Header(message, "X-FilesAPI-Key");
            SessionId = Header(message, "X-FilesAPI-Auth");
            WorkspaceId = Header(message, "X-Files-Workspace-Id");
            Language = Header(message, "Accept-Language");
            Body = message.Body;
            BodyBytes = message.BodyAsBytes ?? new byte[0];
        }

        public LoopbackSite Site { get; }

        public string Method { get; }

        public string Path { get; }

        public string ApiKey { get; }

        public string SessionId { get; }

        public string WorkspaceId { get; }

        public string Language { get; }

        public string Body { get; }

        public byte[] BodyBytes { get; }

        // The ID in a path such as /users/5.
        public string Id => Path.Split('?')[0].Split('/').Last();

        // For example "PATCH /users/1 key=key-a workspace=11".
        public override string ToString()
        {
            string credentials = (ApiKey != null ? " key=" + ApiKey : "") + (SessionId != null ? " session=" + SessionId : "") + (WorkspaceId != null ? " workspace=" + WorkspaceId : "");
            return Method + " " + Path + credentials;
        }

        private static string Header(IRequestMessage message, string name)
        {
            if (message.Headers == null)
            {
                return null;
            }
            foreach (var header in message.Headers)
            {
                if (string.Equals(header.Key, name, StringComparison.OrdinalIgnoreCase))
                {
                    return string.Join(",", header.Value);
                }
            }
            return null;
        }
    }
}
