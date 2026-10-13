using FilesCom;
using FilesCom.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WireMock;

namespace FilesTests
{
    // Which failures are retried, how many times, and how long each retry waits: Files.com API requests back off
    // exponentially from InitialNetworkRequestDelay; storage requests wait all the server's Retry-After asks after
    // 429 or 503, or not at all without one, and are not retried when it asks for longer than the platform can wait,
    // and wait 100 ms after other failures. Every retry is logged, without secrets, and no retry follows a failure
    // that trying again cannot fix.
    [TestClass]
    public class RetryTest
    {
        private static readonly DateTime MTime = new DateTime(2026, 9, 24, 0, 0, 0, DateTimeKind.Utc);
        private static readonly TimeSpan Promptly = TimeSpan.FromSeconds(10);
        private LoopbackSite site;

        [TestInitialize]
        public void Start()
        {
            site = new LoopbackSite("key-a", "11");
        }

        [TestCleanup]
        public void Stop()
        {
            site.Dispose();
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(429, true)]
        [DataRow(408, true)]
        [DataRow(500, true)]
        [DataRow(502, true)]
        [DataRow(503, true)]
        [DataRow(504, true)]
        [DataRow(501, false)]
        [DataRow(400, false)]
        [DataRow(404, false)]
        [DataRow(422, false)]
        public async Task FailuresAreRetriedByTheirStatus(int status, bool retried)
        {
            site.On("GET", "/users/9", request => LoopbackSite.Status(status));
            site.On("POST", "/file_actions/begin_upload/remote.bin", request => LoopbackSite.Json($"[{{\"upload_uri\":\"{site.Url}/part\",\"http_method\":\"PUT\",\"partsize\":4,\"ref\":\"upload-ref\"}}]"));
            site.On("PUT", "//part", request => LoopbackSite.Status(status));
            site.On("GET", "/files/remote.bin", request => LoopbackSite.Json($"{{\"path\":\"remote.bin\",\"download_uri\":\"{site.Url}/download\"}}"));
            site.On("GET", "//download", request => LoopbackSite.Status(status));
            FilesClient client = FilesClient.Create(site.Configuration(maxNetworkRetries: 2));
            int expected = retried ? 3 : 1;

            await ThrowsSdkException(client.Users.FindAsync(9));
            await ThrowsSdkException(client.RemoteFiles.UploadFileAsync("remote.bin", new MemoryStream(new byte[4]), 4, MTime));
            await ThrowsSdkException(client.RemoteFiles.DownloadFileAsync("remote.bin", new MemoryStream()));

            Assert.AreEqual(expected, Count("GET /users/9"), "API requests");
            Assert.AreEqual(expected, Count("PUT /part"), "Upload parts");
            Assert.AreEqual(expected, Count("GET /download"), "Downloads");
        }

        // Storage's 400 saying the request timed out may be temporary, as a 5xx is, so storage requests are sent again;
        // an API request answered with it is not, as after any other 400.
        [TestMethod]
        [Timeout(60000)]
        public async Task StorageRequestsAreRetriedWhenStorageSaysTheRequestTimedOut()
        {
            site.On("GET", "/users/9", request => LoopbackSite.StorageError(400, LoopbackSite.S3RequestTimeout));
            site.On("POST", "/file_actions/begin_upload/remote.bin", request => LoopbackSite.Json($"[{{\"upload_uri\":\"{site.Url}/part\",\"http_method\":\"PUT\",\"partsize\":4,\"ref\":\"upload-ref\"}}]"));
            site.On("PUT", "//part", request => LoopbackSite.StorageError(400, LoopbackSite.S3RequestTimeout));
            site.On("GET", "/files/remote.bin", request => LoopbackSite.Json($"{{\"path\":\"remote.bin\",\"download_uri\":\"{site.Url}/download\"}}"));
            site.On("GET", "//download", request => LoopbackSite.StorageError(400, LoopbackSite.S3RequestTimeout));
            FilesClient client = FilesClient.Create(site.Configuration(maxNetworkRetries: 2));

            await ThrowsSdkException(client.Users.FindAsync(9));
            await ThrowsSdkException(client.RemoteFiles.UploadFileAsync("remote.bin", new MemoryStream(new byte[4]), 4, MTime));
            await ThrowsSdkException(client.RemoteFiles.DownloadFileAsync("remote.bin", new MemoryStream()));

            Assert.AreEqual(1, Count("GET /users/9"), "API requests");
            Assert.AreEqual(3, Count("PUT /part"), "Upload parts");
            Assert.AreEqual(3, Count("GET /download"), "Downloads");
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(401, false, false, DisplayName = "401: not retried")]
        [DataRow(401, false, true, DisplayName = "401 whose body stalls: not retried")]
        [DataRow(501, false, false, DisplayName = "501: not retried")]
        [DataRow(503, true, false, DisplayName = "503 with Retry-After: retried after its wait")]
        public async Task ErrorResponsesWhoseBodyBreaksOffKeepTheirStatus(int status, bool retried, bool stalls)
        {
            // Complete headers, then part of an error body, and the connection closes, or nothing more arrives.
            using (var server = new ScriptedServer(async connection =>
            {
                await connection.ReadRequestHeadAsync();
                await connection.ReadBodyAsync();
                await connection.SendHeadAsync(status, 1000, "Retry-After: 1\r\n");
                await connection.SendAsync("{\"error\":\"sentinel-body");
                if (stalls)
                {
                    await connection.HoldAsync();
                }
                connection.Dispose();
            }))
            {
                string url = $"http://127.0.0.1:{server.Port}/storage";
                site.On("GET", "/files/remote.bin", request => LoopbackSite.Json($"{{\"path\":\"remote.bin\",\"download_uri\":\"{url}\"}}"));
                site.On("POST", "/file_actions/begin_upload/remote.bin", request => LoopbackSite.Json($"[{{\"upload_uri\":\"{url}\",\"http_method\":\"PUT\",\"partsize\":4,\"ref\":\"upload-ref\"}}]"));
                FilesConfiguration configuration = site.Configuration(maxNetworkRetries: 2);
                FilesConfiguration api = new FilesConfiguration { BaseUrl = $"http://127.0.0.1:{server.Port}", ApiKey = "key", MaxNetworkRetries = 2, InitialNetworkRequestDelay = 0 };
                configuration.ReadTimeout = api.ReadTimeout = stalls ? 1 : configuration.ReadTimeout;
                int attempts = retried ? 3 : 1;

                using (var logs = new LogCapture())
                {
                    var failures = new List<InvalidResponseException>();
                    failures.Add(await Assert.ThrowsExactlyAsync<InvalidResponseException>(() => FilesClient.Create(api).Users.FindAsync(1)));
                    Assert.AreEqual(attempts, Received(server), "API requests");
                    Stopwatch storage = Stopwatch.StartNew();
                    failures.Add(await Assert.ThrowsExactlyAsync<InvalidResponseException>(() => FilesClient.Create(configuration).RemoteFiles.DownloadFileAsync("remote.bin", new MemoryStream())));
                    Assert.AreEqual(2 * attempts, Received(server), "Downloads");
                    failures.Add(await Assert.ThrowsExactlyAsync<InvalidResponseException>(() => FilesClient.Create(configuration).RemoteFiles.UploadFileAsync("remote.bin", new MemoryStream(new byte[4]), 4, MTime)));
                    Assert.AreEqual(3 * attempts, Received(server), "Upload parts");

                    // Storage waits the received Retry-After, a second before each retry, rather than 100 ms.
                    Assert.AreEqual(retried, storage.Elapsed >= TimeSpan.FromSeconds(3.9), $"The storage requests took {storage.Elapsed.TotalSeconds} s.");
                    Assert.IsTrue(failures.All(failure => failure.httpStatus == status && failure.httpHeaders.Contains("Retry-After")));
                    Assert.IsTrue(failures.All(failure => !failure.ToString().Contains("sentinel")), "No part of the body is in the error.");
                    Assert.IsFalse(logs.Normal.Contains("sentinel"), "No part of the body is logged below DEBUG.");
                }
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task ErrorResponsesWhoseBodyCannotBeDecodedAreRetriedByTheirStatus()
        {
            // A throttling answer whose body is in a charset that cannot be decoded.
            using (var server = new ScriptedServer(async connection =>
            {
                await connection.ReadRequestHeadAsync();
                await connection.ReadBodyAsync();
                byte[] body = Encoding.UTF8.GetBytes("{\"error\":\"sentinel-body\"}");
                await connection.SendAsync($"HTTP/1.1 503 Failed\r\nContent-Length: {body.Length}\r\nContent-Type: application/json; charset=no-such-charset\r\nRetry-After: 1\r\n\r\n");
                await connection.SendAsync(body);
                connection.Dispose();
            }))
            {
                string url = $"http://127.0.0.1:{server.Port}/storage";
                site.On("GET", "/files/remote.bin", request => LoopbackSite.Json($"{{\"path\":\"remote.bin\",\"download_uri\":\"{url}\"}}"));
                site.On("POST", "/file_actions/begin_upload/remote.bin", request => LoopbackSite.Json($"[{{\"upload_uri\":\"{url}\",\"http_method\":\"PUT\",\"partsize\":4,\"ref\":\"upload-ref\"}}]"));
                FilesConfiguration configuration = site.Configuration(maxNetworkRetries: 1);
                FilesConfiguration api = new FilesConfiguration { BaseUrl = $"http://127.0.0.1:{server.Port}", ApiKey = "key", MaxNetworkRetries = 1, InitialNetworkRequestDelay = 0 };

                using (var logs = new LogCapture())
                {
                    // Each is retried as a 503 is, then fails as HttpContent does.
                    await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => FilesClient.Create(api).Users.FindAsync(1));
                    Assert.AreEqual(2, Received(server), "API requests");
                    Stopwatch storage = Stopwatch.StartNew();
                    await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => FilesClient.Create(configuration).RemoteFiles.DownloadFileAsync("remote.bin", new MemoryStream()));
                    Assert.AreEqual(4, Received(server), "Downloads");
                    await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => FilesClient.Create(configuration).RemoteFiles.UploadFileAsync("remote.bin", new MemoryStream(new byte[4]), 4, MTime));
                    Assert.AreEqual(6, Received(server), "Upload parts");

                    Assert.IsTrue(storage.Elapsed >= TimeSpan.FromSeconds(1.9), $"Storage waited its Retry-After: the requests took {storage.Elapsed.TotalSeconds} s.");
                    Assert.AreEqual(3, logs.Lines("after HTTP 503").Length, "Each retry is logged with the status that was answered.");
                    Assert.AreEqual(2, logs.Lines("as the server's Retry-After asked").Length);
                    Assert.IsFalse(logs.Normal.Contains("sentinel"), "No part of the body is logged below DEBUG.");
                }
            }
        }

        // With MaxNetworkRetries 0 the SDK retries nothing: an answered failure is final, and nothing is logged as a
        // retry. (Within an attempt, the handler may still send a request that only asks for something again:
        // AGetWhoseAnswerIsLostMayBeSentAgainByTheHandlerWithoutRetries.)
        [TestMethod]
        [Timeout(60000)]
        public async Task WithoutRetriesTheSdkRetriesNothing()
        {
            site.On("GET", "/users/9", request => LoopbackSite.Status(503));
            site.On("GET", "/files/remote.bin", request => LoopbackSite.Json($"{{\"path\":\"remote.bin\",\"download_uri\":\"{site.Url}/download\"}}"));
            site.On("GET", "//download", request => LoopbackSite.Status(503));
            FilesClient client = FilesClient.Create(site.Configuration(maxNetworkRetries: 0));

            SdkException found;
            SdkException downloaded;
            using (var logs = new LogCapture())
            {
                found = await ThrowsSdkException(client.Users.FindAsync(9));
                downloaded = await ThrowsSdkException(client.RemoteFiles.DownloadFileAsync("remote.bin", new MemoryStream()));

                Assert.AreEqual(0, logs.Lines("Retrying").Length);
            }
            Assert.AreEqual(1, Count("GET /users/9"), Arrivals(found));
            Assert.AreEqual(1, Count("GET /download"), Arrivals(downloaded));
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task ApiRetriesBackOffFromOneHundredMilliseconds()
        {
            var arrivals = new List<TimeSpan>();
            site.On("GET", "/users/9", request =>
            {
                lock (arrivals)
                {
                    arrivals.Add(UploadServer.Clock);
                }
                return LoopbackSite.Status(503);
            });
            // The default retry settings.
            FilesClient client = FilesClient.Create(new FilesConfiguration { BaseUrl = site.Url, ApiKey = site.ApiKey });

            using (var logs = new LogCapture())
            {
                await ThrowsSdkException(client.Users.FindAsync(9));

                CollectionAssert.AreEqual(
                    new[] { "retry 1 of 3, in 0.1 seconds", "retry 2 of 3, in 0.2 seconds", "retry 3 of 3, in 0.4 seconds" },
                    logs.Lines("Retrying GET /users after HTTP 503").Select(line => line.Substring(line.IndexOf("retry ", StringComparison.Ordinal))).ToArray());
            }
            Assert.AreEqual(4, arrivals.Count);
            double[] waits = arrivals.Skip(1).Zip(arrivals, (later, earlier) => (later - earlier).TotalMilliseconds).ToArray();
            double[] expected = { 100, 200, 400 };
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.IsTrue(waits[i] >= expected[i] - 5 && waits[i] < expected[i] + 400, $"Retry {i + 1} came {waits[i]} ms after the attempt before it, not about {expected[i]} ms.");
            }
        }

        // A create answered with an error is sent once, whatever the answer says, and its exception, keeping its type,
        // status and headers, says it may have taken effect, with retries left or none: a proxy's page, a body cut short
        // or one that cannot be decoded, or a Files.com error, even its rate limit or a processing error, which it can
        // also answer after an operation has begun. A create is sent again only when it was shown not carried out:
        // ACreateThatCouldNotConnectIsSentAgain, and a move to the site's region in RegionRelocationTest.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(503, "html", 1, "InvalidResponseException", DisplayName = "A proxy's 503 page, with a retry left")]
        [DataRow(503, "html", 0, "InvalidResponseException", DisplayName = "A proxy's 503 page, without retries")]
        [DataRow(503, "cut short", 1, "InvalidResponseException", DisplayName = "A 503 whose body is cut short")]
        [DataRow(503, "undecodable", 1, "InvalidResponseException", DisplayName = "A 503 whose body cannot be decoded")]
        [DataRow(500, "html", 1, "InvalidResponseException", DisplayName = "A 500 page, with a retry left")]
        [DataRow(500, "html", 0, "InvalidResponseException", DisplayName = "A 500 page, without retries")]
        [DataRow(408, "html", 1, "InvalidResponseException", DisplayName = "A proxy's 408 page")]
        [DataRow(429, "rate limit", 1, "TooManyRequestsException", DisplayName = "Files.com's rate limit, with a retry left")]
        [DataRow(503, "json", 1, "ApiException", DisplayName = "A JSON error object with 503")]
        [DataRow(422, "exists", 0, "DestinationExistsException", DisplayName = "A processing error")]
        public async Task ACreateAnsweredWithAnErrorIsSentOnceAndSaysItMayHaveTakenEffect(int status, string body, int retries, string exception)
        {
            int creates = 0;
            using (var server = new ScriptedServer(async connection =>
            {
                while (true)
                {
                    await connection.ReadRequestHeadAsync();
                    await connection.ReadBodyAsync();
                    if (Interlocked.Increment(ref creates) > 1)
                    {
                        await connection.RespondAsync(200, "{\"id\":2,\"username\":\"new\"}");
                        continue;
                    }
                    const string Answer = "Connection: close\r\nX-Request-ID: request-1\r\n";
                    string head = $"HTTP/1.1 {status} Failed\r\n{Answer}";
                    switch (body)
                    {
                        case "html":
                            await connection.SendAsync($"{head}Content-Type: text/html\r\nContent-Length: 33\r\n\r\n<html>upstream unavailable</html>");
                            break;
                        case "cut short":
                            await connection.SendAsync($"{head}Content-Type: text/html\r\nContent-Length: 100\r\n\r\n<html>proxy");
                            break;
                        case "undecodable":
                            await connection.SendAsync($"{head}Content-Type: application/json; charset=no-such-charset\r\nContent-Length: 25\r\n\r\n{{\"error\":\"sentinel-body\"}}");
                            break;
                        case "rate limit":
                            await connection.RespondAsync(status, "{\"error\":\"Rate limited.\",\"type\":\"rate-limited/too-many-requests\",\"http-code\":429}", Answer);
                            break;
                        case "json":
                            await connection.RespondAsync(status, "{\"error\":\"Unavailable\",\"http-code\":503}", Answer);
                            break;
                        default:
                            await connection.RespondAsync(status, "{\"error\":\"Exists\",\"type\":\"processing-failure/destination-exists\",\"http-code\":422}", Answer);
                            break;
                    }
                    connection.Dispose();
                    return;
                }
            }))
            {
                FilesClient client = FilesClient.Create(new FilesConfiguration { BaseUrl = $"http://127.0.0.1:{server.Port}", ApiKey = "key", MaxNetworkRetries = retries, InitialNetworkRequestDelay = 0 });

                SdkException error = null;
                try
                {
                    await client.Users.CreateAsync(new Dictionary<string, object> { { "username", "new" } });
                }
                catch (SdkException e)
                {
                    error = e;
                }

                Assert.AreEqual(1, Volatile.Read(ref creates), "Sent once.");
                Assert.IsNotNull(error, "The create failed.");
                Assert.AreEqual(exception, error.GetType().Name);
                Assert.IsTrue(error.RequestMayHaveBeenProcessed);
                HttpHeaders headers = error is ApiException answered ? answered.httpHeaders : ((InvalidResponseException)error).httpHeaders;
                Assert.AreEqual(status, error is ApiException typed ? typed.httpStatus : ((InvalidResponseException)error).httpStatus);
                Assert.AreEqual("request-1", headers.GetValues("X-Request-ID").Single());
                Assert.IsFalse(error.ToString().Contains("sentinel"), "No part of the body is in the error.");
            }
        }

        // A request that can change something, whose answer is lost after it was received whole, reaches the network
        // once, whatever the retries left and whatever its shape: a create has a JSON body; a delete has none of its own
        // and goes with an empty one (Content-Length: 0), so that the handler does not send it again itself, as it would
        // a request without content (up to three times, below every handler). Through the default handlers and a
        // shared SocketsHttpHandler alike.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("create", 0, false, false, DisplayName = "A create, without retries")]
        [DataRow("create", 3, false, false, DisplayName = "A create, with the default three retries")]
        [DataRow("create", 3, true, false, DisplayName = "A create, on a connection an earlier request used")]
        [DataRow("delete", 0, false, false, DisplayName = "A delete, without retries")]
        [DataRow("delete", 3, false, false, DisplayName = "A delete, with the default three retries")]
        [DataRow("delete", 3, true, false, DisplayName = "A delete, on a connection an earlier request used")]
        [DataRow("delete", 0, false, true, DisplayName = "A delete through a shared SocketsHttpHandler")]
        public async Task AChangeWhoseAnswerIsLostIsSentOnceAndSaysItMayHaveTakenEffect(string change, int retries, bool reused, bool sharedHandler)
        {
            var changes = new System.Collections.Concurrent.ConcurrentQueue<string>();
            using (var server = new ScriptedServer(async connection =>
            {
                while (true)
                {
                    string head = await connection.ReadRequestHeadAsync();
                    await connection.ReadBodyAsync();
                    if (head.StartsWith("GET", StringComparison.Ordinal))
                    {
                        await connection.RespondAsync(200, "{\"id\":1,\"username\":\"one\"}");
                        continue;
                    }
                    // The change is received whole, and its answer is lost.
                    changes.Enqueue(head + "\r\n" + string.Join("\r\n", connection.HeaderLines));
                    connection.Dispose();
                    return;
                }
            }))
            using (var shared = new SocketsHttpHandler { AllowAutoRedirect = false })
            {
                var configuration = new FilesConfiguration { BaseUrl = $"http://127.0.0.1:{server.Port}", ApiKey = "key", InitialNetworkRequestDelay = 0 };
                if (retries != 3)
                {
                    configuration.MaxNetworkRetries = retries;
                }
                FilesClient client = sharedHandler ? FilesClient.Create(configuration, HttpTransport.FromSharedHandler(shared)) : FilesClient.Create(configuration);
                if (reused)
                {
                    await client.Users.FindAsync(1);
                }

                ApiConnectionException error;
                using (var logs = new LogCapture())
                {
                    error = await Assert.ThrowsExactlyAsync<ApiConnectionException>(() => change == "create"
                        ? client.Users.CreateAsync(new Dictionary<string, object> { { "username", "new" } })
                        : client.Users.DeleteAsync(1));
                    Assert.AreEqual(0, logs.Lines("Retrying").Length, "The SDK sent it no retry.");
                }
                await client.DisposeAsync();

                Assert.AreEqual(1, changes.Count, "Received once: " + string.Join(" | ", changes.Select(head => head.Split('\r')[0])));
                string received = changes.Single();
                StringAssert.StartsWith(received, change == "create" ? "POST /api/rest/v1/users " : "DELETE /api/rest/v1/users/1");
                if (change == "delete")
                {
                    StringAssert.Contains(received, "Content-Length: 0", "An empty body, declared.");
                    Assert.IsFalse(received.Contains("Content-Type:") || received.Contains("Transfer-Encoding:"), "Nothing more than an empty body: " + received);
                }
                Assert.IsTrue(error.RequestMayHaveBeenProcessed);
                StringAssert.Contains(error.Message, "may have reached Files.com and taken effect, so the SDK did not send it again");
            }
        }

        // A change that Files.com redirects with 307 or 308 is sent again where the redirect says, with the same method and
        // body, inside the same attempt: that is routing, not a retry. Once the routed request has been sent, a lost or
        // failed answer ends it: with retries left, it is not sent a third time, and it says it may have taken effect.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("create", 307, "answered", DisplayName = "A create redirected with 307, answered where it was sent")]
        [DataRow("delete", 308, "answered", DisplayName = "A delete redirected with 308, answered where it was sent")]
        [DataRow("create", 308, "lost", DisplayName = "A create redirected with 308 whose routed answer is lost")]
        [DataRow("delete", 307, "lost", DisplayName = "A delete redirected with 307 whose routed answer is lost")]
        [DataRow("create", 307, "error", DisplayName = "A create redirected with 307 whose routed answer is a server error")]
        public async Task ARedirectedChangeIsSentWhereItIsRedirectedAndNotAgainAfterItsAnswer(string change, int status, string answer)
        {
            var sent = new System.Collections.Concurrent.ConcurrentQueue<string>();
            using (var server = new ScriptedServer(async connection =>
            {
                while (true)
                {
                    string head = await connection.ReadRequestHeadAsync();
                    string body = Encoding.UTF8.GetString(await connection.ReadBodyAsync());
                    string[] line = head.Split(' ');
                    string path = line[1].Split('?')[0];
                    sent.Enqueue($"{line[0]} {path} {connection.ContentLength} {body}");
                    if (!path.StartsWith("/moved/", StringComparison.Ordinal))
                    {
                        await connection.SendAsync($"HTTP/1.1 {status} Redirect\r\nLocation: /moved{line[1]}\r\nContent-Length: 0\r\n\r\n");
                        continue;
                    }
                    switch (answer)
                    {
                        case "answered":
                            await connection.RespondAsync(200, change == "create" ? "{\"id\":2,\"username\":\"new\"}" : "{}");
                            continue;
                        case "error":
                            await connection.RespondAsync(500, "{\"error\":\"Internal error.\",\"http-code\":500}");
                            continue;
                        default:
                            connection.Dispose();
                            return;
                    }
                }
            }))
            {
                var configuration = new FilesConfiguration { BaseUrl = $"http://127.0.0.1:{server.Port}", ApiKey = "key", InitialNetworkRequestDelay = 0, MaxNetworkRetryDelay = 0 };
                FilesClient client = FilesClient.Create(configuration);
                Func<Task> send = () => change == "create"
                    ? client.Users.CreateAsync(new Dictionary<string, object> { { "username", "new" } })
                    : client.Users.DeleteAsync(1);

                using (var logs = new LogCapture())
                {
                    if (answer == "answered")
                    {
                        await send();
                    }
                    else
                    {
                        Exception error = await Assert.ThrowsAsync<Exception>(send);
                        Assert.IsInstanceOfType(error, answer == "lost" ? typeof(ApiConnectionException) : typeof(ApiException), error.ToString());
                        Assert.IsTrue(((SdkException)error).RequestMayHaveBeenProcessed, "It may have taken effect.");
                    }
                    Assert.AreEqual(0, logs.Lines("Retrying").Length, "The SDK sent it no retry, with three left.");
                }
                await client.DisposeAsync();

                string[] received = sent.ToArray();
                Assert.AreEqual(2, received.Length, "Sent where it was addressed, then where it was redirected, and no more: " + string.Join(" | ", received));
                string method = change == "create" ? "POST" : "DELETE";
                string original = change == "create" ? "/api/rest/v1/users" : "/api/rest/v1/users/1";
                StringAssert.StartsWith(received[0], $"{method} {original} ");
                StringAssert.StartsWith(received[1], $"{method} /moved{original} ");
                Assert.AreEqual(received[0].Substring(received[0].IndexOf(' ', method.Length + 1)), received[1].Substring(received[1].IndexOf(' ', method.Length + 1)), "The same body, both times.");
                if (change == "delete")
                {
                    StringAssert.EndsWith(received[1], " 0 ", "An empty body, declared, both times.");
                }
                else
                {
                    StringAssert.Contains(received[1], "\"username\":\"new\"");
                }
            }
        }

        // A request that only asks for something may be sent again by the handler, within the SDK's one attempt, when its
        // connection closes before any answer: MaxNetworkRetries counts the SDK's attempts, and 0 retries is not one
        // request on the network. SocketsHttpHandler does this for a request without content; the SDK logs no retry.
        [TestMethod]
        [Timeout(60000)]
        public async Task AGetWhoseAnswerIsLostMayBeSentAgainByTheHandlerWithoutRetries()
        {
            int gets = 0;
            using (var server = new ScriptedServer(async connection =>
            {
                while (true)
                {
                    await connection.ReadRequestHeadAsync();
                    if (Interlocked.Increment(ref gets) == 1)
                    {
                        connection.Dispose();
                        return;
                    }
                    await connection.RespondAsync(200, "{\"id\":1,\"username\":\"one\"}");
                }
            }))
            using (var logs = new LogCapture())
            {
                FilesClient client = FilesClient.Create(new FilesConfiguration { BaseUrl = $"http://127.0.0.1:{server.Port}", ApiKey = "key", MaxNetworkRetries = 0, InitialNetworkRequestDelay = 0 });

                User user = await client.Users.FindAsync(1);

                Assert.AreEqual(1, user.Id);
                Assert.AreEqual(2, Volatile.Read(ref gets), "Sent twice on the network, within one attempt.");
                Assert.AreEqual(0, logs.Lines("Retrying").Length, "The SDK retried nothing.");
                await client.DisposeAsync();
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task AGetWhoseAnswerIsLostIsSentAgain()
        {
            int gets = 0;
            using (var server = new ScriptedServer(async connection =>
            {
                while (true)
                {
                    await connection.ReadRequestHeadAsync();
                    if (Interlocked.Increment(ref gets) == 1)
                    {
                        connection.Dispose();
                        return;
                    }
                    await connection.RespondAsync(200, "{\"id\":1,\"username\":\"one\"}");
                }
            }))
            {
                FilesClient client = FilesClient.Create(new FilesConfiguration { BaseUrl = $"http://127.0.0.1:{server.Port}", ApiKey = "key", MaxNetworkRetries = 1, InitialNetworkRequestDelay = 0 });

                User user = await client.Users.FindAsync(1);

                Assert.AreEqual(1, user.Id);
                Assert.AreEqual(2, Volatile.Read(ref gets), "A request that only asks for something is sent again.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(true, DisplayName = "Reported as a connection error")]
        [DataRow(false, DisplayName = "Reported only by its socket's error")]
        public async Task ACreateThatCouldNotConnectIsSentAgain(bool asConnectionError)
        {
            site.On("POST", "/users", request => LoopbackSite.Json("{\"id\":7,\"username\":\"new\"}"));
            int refused = 0;
            // The first attempt fails to connect, as a refused connection does, before anything is sent.
            HttpTransport transport = HttpTransport.FromHandlerFactory(() => new FirstConnectionRefused(() => Interlocked.Increment(ref refused) == 1, asConnectionError));
            FilesClient client = FilesClient.Create(site.Configuration(maxNetworkRetries: 1), transport);

            User user = await client.Users.CreateAsync(new Dictionary<string, object> { { "username", "new" } });

            Assert.AreEqual(7, user.Id);
            Assert.AreEqual(1, site.Requests.Count(request => request.Method == "POST"), "Sent once, after the connection that failed.");
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task AnUploadsNegotiationWhoseAnswerIsLostIsSentAgain()
        {
            int begins = 0, parts = 0, finals = 0;
            ScriptedServer server = null;
            server = new ScriptedServer(async connection =>
            {
                while (true)
                {
                    string target = (await connection.ReadRequestHeadAsync()).Split(' ')[1];
                    await connection.ReadBodyAsync();
                    if (target.Contains("begin_upload"))
                    {
                        if (Interlocked.Increment(ref begins) == 1)
                        {
                            // Received whole, and its answer is lost: asking again only issues another reference.
                            connection.Dispose();
                            return;
                        }
                        await connection.RespondAsync(200, $"[{{\"ref\":\"ref-{begins}\",\"partsize\":3,\"parallel_parts\":false,\"upload_uri\":\"http://127.0.0.1:{server.Port}/part\",\"http_method\":\"PUT\"}}]");
                    }
                    else if (target == "/part")
                    {
                        Interlocked.Increment(ref parts);
                        await connection.RespondAsync(200, "", "ETag: \"part1\"\r\n");
                    }
                    else
                    {
                        Interlocked.Increment(ref finals);
                        await connection.RespondAsync(200, "{\"path\":\"remote.bin\",\"size\":3}");
                    }
                }
            });
            using (server)
            {
                FilesClient client = FilesClient.Create(new FilesConfiguration { BaseUrl = $"http://127.0.0.1:{server.Port}", ApiKey = "key", MaxNetworkRetries = 1, InitialNetworkRequestDelay = 0 });

                await client.Transfers.UploadStreamAsync(new MemoryStream(new byte[] { 1, 2, 3 }), 3, "remote.bin");

                Assert.AreEqual("2 1 1", $"{begins} {parts} {finals}", "Negotiated again, then one part and one final request.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(503, null, 0, 400, DisplayName = "503 without Retry-After: no wait")]
        [DataRow(503, "1", 1000, 1400, DisplayName = "503 with Retry-After in seconds")]
        [DataRow(429, "date", 1000, 2400, DisplayName = "429 with Retry-After as a date")]
        [DataRow(503, "soon", 0, 400, DisplayName = "503 with a Retry-After that cannot be read: no wait")]
        [DataRow(500, null, 100, 500, DisplayName = "500: 100 ms")]
        [DataRow(500, "1", 100, 500, DisplayName = "500 with Retry-After: 100 ms")]
        public async Task StorageRetriesWaitAsTheServerAsks(int status, string retryAfter, int fromMilliseconds, int toMilliseconds)
        {
            var arrivals = new List<TimeSpan>();
            site.On("POST", "/file_actions/begin_upload/remote.bin", request => LoopbackSite.Json($"[{{\"upload_uri\":\"{site.Url}/part\",\"http_method\":\"PUT\",\"partsize\":4,\"ref\":\"upload-ref\"}}]"));
            site.On("PUT", "//part", request =>
            {
                lock (arrivals)
                {
                    arrivals.Add(UploadServer.Clock);
                    if (arrivals.Count > 1)
                    {
                        return LoopbackSite.Status(200);
                    }
                }
                ResponseMessage failure = LoopbackSite.Status(status);
                if (retryAfter != null)
                {
                    // Two seconds from now in whole seconds, so at least one second away.
                    failure.AddHeader("Retry-After", retryAfter == "date" ? DateTimeOffset.UtcNow.AddSeconds(2).ToString("r", CultureInfo.InvariantCulture) : retryAfter);
                }
                return failure;
            });
            site.On("POST", "/files/remote.bin", request => LoopbackSite.Json("{\"path\":\"remote.bin\"}"));
            // The API retry delay is long, to show storage waits do not use it.
            FilesConfiguration configuration = site.Configuration(maxNetworkRetries: 1);
            configuration.InitialNetworkRequestDelay = 5;
            configuration.MaxNetworkRetryDelay = 5;

            await FilesClient.Create(configuration).RemoteFiles.UploadFileAsync("remote.bin", new MemoryStream(new byte[4]), 4, MTime);

            Assert.AreEqual(2, arrivals.Count);
            double waited = (arrivals[1] - arrivals[0]).TotalMilliseconds;
            Assert.IsTrue(waited >= fromMilliseconds - 5 && waited < toMilliseconds, $"The retry came {waited} ms after the failure, not {fromMilliseconds} to {toMilliseconds} ms.");
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(429)]
        [DataRow(503)]
        public async Task StorageRetryAfterLongerThanThePlatformCanWaitIsNotRetried(int status)
        {
            // About 3,170 years: longer than any platform's timers can wait, though a TimeSpan can hold it.
            ResponseMessage Throttled()
            {
                ResponseMessage throttled = LoopbackSite.Status(status);
                throttled.AddHeader("Retry-After", "99999999999");
                return throttled;
            }
            site.On("POST", "/file_actions/begin_upload/remote.bin", request => LoopbackSite.Json($"[{{\"upload_uri\":\"{site.Url}/part\",\"http_method\":\"PUT\",\"partsize\":4,\"ref\":\"upload-ref\"}}]"));
            site.On("PUT", "//part", request => Throttled());
            site.On("GET", "/files/remote.bin", request => LoopbackSite.Json($"{{\"path\":\"remote.bin\",\"download_uri\":\"{site.Url}/download\"}}"));
            site.On("GET", "//download", request => Throttled());
            FilesClient client = FilesClient.Create(site.Configuration(maxNetworkRetries: 3));

            using (var logs = new LogCapture())
            {
                Task downloading = client.RemoteFiles.DownloadFileAsync("remote.bin", new MemoryStream());
                Assert.AreSame(downloading, await Task.WhenAny(downloading, Task.Delay(Promptly)), "The download did not fail at once.");
                await ThrowsSdkException(downloading);
                await ThrowsSdkException(client.RemoteFiles.UploadFileAsync("remote.bin", new MemoryStream(new byte[4]), 4, MTime));

                Assert.AreEqual(1, Count("GET /download"), "Downloads");
                Assert.AreEqual(1, Count("PUT /part"), "Upload parts");
                Assert.AreEqual(0, logs.Lines("Retrying").Length, string.Join("\n", logs.Lines("Retrying")));
                Assert.AreEqual(
                    $"Not retrying the download after HTTP {status}: the server's Retry-After asked for 99999999999 seconds, longer than this platform can wait",
                    logs.Lines("Not retrying the download").Single());
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task FailedResponseIsReleasedBeforeTheRetryWait()
        {
            using (var api = new ScriptedServer(async connection =>
            {
                while (true)
                {
                    string request = await connection.ReadRequestHeadAsync();
                    if (request.Contains("/users/9"))
                    {
                        await connection.RespondAsync(503, "{\"error\":\"unavailable\"}");
                    }
                    else
                    {
                        await connection.RespondAsync(200, "{\"id\":2,\"username\":\"second\"}");
                    }
                }
            }))
            {
                var configuration = new FilesConfiguration { BaseUrl = $"http://127.0.0.1:{api.Port}", ApiKey = "key", MaxNetworkRetries = 1, InitialNetworkRequestDelay = 30, MaxNetworkRetryDelay = 30 };
                FilesClient client = FilesClient.Create(configuration);
                var cancellation = new CancellationTokenSource();

                Task<User> retrying = client.Users.FindAsync(9, cancellationToken: cancellation.Token);
                await WaitFor(() => api.Connections.Count == 1 && api.Connections[0].Requests.Count == 1);
                await Task.Delay(300); // The failed attempt is now waiting 30 seconds for its retry.
                User second = await client.Users.FindAsync(2);
                cancellation.Cancel();

                // The failed response was read and released before the wait, so its connection went back to the pool
                // and the second request could use it.
                Assert.AreEqual("second", second.Username);
                Assert.AreEqual(1, api.Connections.Count, "The second request needed a new connection: the failed response still held the first.");
                await Assert.ThrowsAsync<OperationCanceledException>(() => retrying);
                await Task.Delay(300);
                Assert.AreEqual(2, api.Connections.Single().Requests.Count, "The cancelled retry was never sent.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task CertificateFailureIsNotRetried()
        {
            using (var tls = new UntrustedTlsServer())
            {
                var configuration = new FilesConfiguration { BaseUrl = $"https://localhost:{tls.Port}", ApiKey = "key", MaxNetworkRetries = 3, InitialNetworkRequestDelay = 0 };

                await Assert.ThrowsExactlyAsync<ApiConnectionException>(() => FilesClient.Create(configuration).Users.FindAsync(1));

                Assert.AreEqual(1, tls.Handshakes, "An untrusted certificate is not tried again.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task RequestThatCannotBeSentIsNotRetried()
        {
            var configuration = new FilesConfiguration { BaseUrl = "ftp://127.0.0.1/", ApiKey = "key", MaxNetworkRetries = 3, InitialNetworkRequestDelay = 5 };
            Stopwatch elapsed = Stopwatch.StartNew();

            await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => FilesClient.Create(configuration).Users.FindAsync(1));

            Assert.IsTrue(elapsed.Elapsed < TimeSpan.FromSeconds(4), "It failed without waiting to retry.");
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task RedirectsBeyondTheLimitAreNotRetried()
        {
            site.On("GET", "/users/9", request =>
            {
                ResponseMessage redirect = LoopbackSite.Status(302);
                redirect.AddHeader("Location", site.Url + "/api/rest/v1/users/9");
                return redirect;
            });
            FilesClient client = FilesClient.Create(site.Configuration(maxNetworkRetries: 3));

            await ThrowsSdkException(client.Users.FindAsync(9));

            Assert.AreEqual(51, Count("GET /users/9"), "The request and its 50 redirects, once.");
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task FailuresOfTheCallersStreamsAreNotRetried()
        {
            site.ServeDownload(new byte[4]);
            site.ServeUpload(partsize: 4);
            FilesClient client = FilesClient.Create(site.Configuration(maxNetworkRetries: 3));
            var diskFull = new IOException("The destination disk is full.");
            var unreadable = new IOException("The source cannot be read.");
            // A destination that sends what it is given elsewhere: it keeps the bytes, then fails as a connection would,
            // quoting where it sends them.
            var disconnected = new HttpRequestException("The remote destination disconnected.", new IOException("https://user:sentinel-secret@destination.example.test/path?token=sentinel-secret"));
            var remote = new FailingStream(disconnected, keepsWrites: true);

            Exception destinationError = await Assert.ThrowsExactlyAsync<IOException>(() => client.RemoteFiles.DownloadFileAsync("remote.bin", new FailingStream(diskFull)));
            Exception remoteError;
            using (var logs = new LogCapture())
            {
                remoteError = await Assert.ThrowsExactlyAsync<HttpRequestException>(() => client.RemoteFiles.DownloadFileAsync("remote.bin", remote));

                // The caller gets the exception to report; below DEBUG, only that the download failed is logged.
                Assert.IsFalse(logs.Normal.Contains("sentinel"), logs.Normal);
                CollectionAssert.Contains(logs.Lines("DownloadFile failed"), "DownloadFile failed");
                StringAssert.Contains(logs.Debug, "sentinel-secret", "The details stay available at DEBUG.");
            }
            Exception sourceError = await Assert.ThrowsExactlyAsync<IOException>(() => client.RemoteFiles.UploadFileAsync("remote.bin", new FailingStream(unreadable), 4, MTime));

            Assert.AreSame(diskFull, destinationError);
            Assert.AreSame(disconnected, remoteError, "The caller's own exception, not a connection failure of the download.");
            Assert.AreSame(unreadable, sourceError);
            Assert.AreEqual(2, Count("GET /download"), "Each download was sent once.");
            Assert.AreEqual(1, remote.Kept, "The bytes the destination kept were not written again.");
            Assert.AreEqual(4, remote.KeptBytes);
            Assert.IsFalse(remote.Closed, "The destination is the caller's, and stays open.");
            Assert.AreEqual(0, Count("PUT /part"));
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task OrdinaryLogsNameTheResourceNotThePath()
        {
            int attempts = 0;
            site.On("GET", "/file_actions/metadata/*", request => ++attempts == 1 ? LoopbackSite.Status(503) : LoopbackSite.Json("{\"path\":\"secret-folder/report.pdf\"}"));
            FilesClient client = FilesClient.Create(site.Configuration(maxNetworkRetries: 1));

            using (var logs = new LogCapture())
            {
                await client.RemoteFiles.FindAsync("secret-folder/report.pdf");

                CollectionAssert.Contains(logs.Lines("Sending"), "Sending GET /file_actions");
                CollectionAssert.Contains(logs.Lines("Retrying"), "Retrying GET /file_actions after HTTP 503: retry 1 of 1, in 0 seconds");
                Assert.IsFalse(logs.Normal.Contains("secret-folder"), logs.Normal);
                StringAssert.Contains(logs.Debug, "secret-folder", "The path stays available at DEBUG.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task DebugLogsHaveTheBodyOfEveryResponse()
        {
            site.On("GET", "/users", request => LoopbackSite.Json("[{\"id\":1,\"username\":\"page-sentinel\"}]"));
            site.On("POST", "/users/1/unlock", request => LoopbackSite.Json("{\"note\":\"void-sentinel\"}"));
            FilesClient client = FilesClient.Create(site.Configuration());

            using (var logs = new LogCapture())
            {
                List<User> users = await client.Users.List().AllAsync();
                await users[0].UnlockAsync();

                StringAssert.Contains(logs.Debug, "Response (HTTP 200): [{\"id\":1,\"username\":\"page-sentinel\"}]");
                StringAssert.Contains(logs.Debug, "Response (HTTP 200): {\"note\":\"void-sentinel\"}");
                Assert.IsFalse(logs.Normal.Contains("sentinel"), logs.Normal);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task RetryLogsHideSignedUrlsAndCredentials()
        {
            string signedUrl = site.Url + "/download?X-Amz-Credential=sentinel-credential&X-Amz-Signature=sentinel-signature";
            int downloads = 0;
            site.On("GET", "/users/9", request => LoopbackSite.Status(503));
            site.On("GET", "/files/remote.bin", request => LoopbackSite.Json($"{{\"path\":\"remote.bin\",\"download_uri\":\"{signedUrl}\"}}"));
            site.On("GET", "//download", request => ++downloads == 1 ? LoopbackSite.Status(503) : LoopbackSite.Json("{}"));
            var configuration = new FilesConfiguration { BaseUrl = site.Url, ApiKey = "sentinel-key", MaxNetworkRetries = 1, InitialNetworkRequestDelay = 0 };
            FilesClient client = FilesClient.Create(configuration);

            using (var logs = new LogCapture())
            {
                await ThrowsSdkException(client.Users.FindAsync(9));
                await client.RemoteFiles.DownloadFileAsync("remote.bin", new MemoryStream());

                StringAssert.Contains(logs.Normal, "Retrying GET /users after HTTP 503: retry 1 of 1, in 0 seconds");
                StringAssert.Contains(logs.Normal, "Retrying the download after HTTP 503: retry 1 of 1, in 0 seconds, without a Retry-After to wait for");
                Assert.IsFalse(logs.Normal.Contains("sentinel"), logs.Normal);
                StringAssert.Contains(logs.Debug, "sentinel-signature");
            }
        }

        // A handler whose attempts fail to connect, before anything is sent, while refuse says so; the rest are sent.
        internal sealed class FirstConnectionRefused : DelegatingHandler
        {
            private readonly Func<bool> refuse;
            private readonly bool asConnectionError;

            internal FirstConnectionRefused(Func<bool> refuse, bool asConnectionError)
                : base(new SocketsHttpHandler { AllowAutoRedirect = false })
            {
                this.refuse = refuse;
                this.asConnectionError = asConnectionError;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (refuse())
                {
                    var socket = new SocketException((int)SocketError.ConnectionRefused);
                    throw asConnectionError
                        ? new HttpRequestException(HttpRequestError.ConnectionError, "Connection refused", socket)
                        : new HttpRequestException("Connection refused", socket);
                }
                return base.SendAsync(request, cancellationToken);
            }
        }

        private static int Received(ScriptedServer server)
        {
            return server.Connections.Sum(connection => connection.Requests.Count);
        }

        private int Count(string request)
        {
            return site.Requests.Count(received => received.Method + " " + received.Path.Split('?')[0] == request);
        }

        // The SdkException the operation failed with, which a failed check can then report.
        private static async Task<SdkException> ThrowsSdkException(Task operation)
        {
            try
            {
                await operation;
            }
            catch (SdkException e)
            {
                return e;
            }
            Assert.Fail("The operation succeeded.");
            return null;
        }

        // What a request count check reports: what the site received, and how the operation failed, inner exceptions
        // and their endpoints included.
        private string Arrivals(SdkException failure)
        {
            return $"Requests received by the site at {site.Url}: [{string.Join(", ", site.Requests)}]; its server's log: {site.Log()}. The operation failed with {LoopbackSite.Describe(failure)}";
        }

        private static async Task WaitFor(Func<bool> condition)
        {
            Stopwatch waited = Stopwatch.StartNew();
            while (!condition())
            {
                Assert.IsTrue(waited.Elapsed < Promptly, "Timed out waiting for the test condition.");
                await Task.Delay(20);
            }
        }

        // A stream whose reads and writes fail with error. With keepsWrites, each write first keeps its bytes.
        private sealed class FailingStream : Stream
        {
            private readonly Exception error;
            private readonly bool keepsWrites;

            public FailingStream(Exception error, bool keepsWrites = false)
            {
                this.error = error;
                this.keepsWrites = keepsWrites;
            }

            // How many writes the destination kept, and their bytes.
            public int Kept { get; private set; }

            public long KeptBytes { get; private set; }

            public bool Closed { get; private set; }

            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => true;
            public override long Length => throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => Task.FromException<int>(error);
            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                Kept += keepsWrites ? 1 : 0;
                KeptBytes += keepsWrites ? count : 0;
                return Task.FromException(error);
            }

            public override int Read(byte[] buffer, int offset, int count) => throw error;
            public override void Write(byte[] buffer, int offset, int count) => throw error;
            public override void Flush()
            {
            }
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();

            protected override void Dispose(bool disposing)
            {
                Closed = true;
                base.Dispose(disposing);
            }
        }

        // A TLS server on loopback whose certificate no client trusts. It counts the handshakes clients start.
        private sealed class UntrustedTlsServer : IDisposable
        {
            private readonly TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
            private readonly X509Certificate2 certificate;
            private int handshakes;

            public UntrustedTlsServer()
            {
                using (RSA key = RSA.Create(2048))
                {
                    var request = new CertificateRequest("CN=localhost", key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    using (X509Certificate2 created = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(1)))
                    {
                        // A password: macOS cannot import a PKCS #12 blob with an empty one.
                        byte[] pfx = created.Export(X509ContentType.Pkcs12, "loopback");
#if NET9_0_OR_GREATER
                        certificate = X509CertificateLoader.LoadPkcs12(pfx, "loopback");
#else
                        certificate = new X509Certificate2(pfx, "loopback");
#endif
                    }
                }
                listener.Start();
                Port = ((IPEndPoint)listener.LocalEndpoint).Port;
                _ = AcceptAsync();
            }

            public int Port { get; }

            public int Handshakes => Volatile.Read(ref handshakes);

            public void Dispose()
            {
                listener.Stop();
                certificate.Dispose();
            }

            private async Task AcceptAsync()
            {
                while (true)
                {
                    TcpClient client;
                    try
                    {
                        client = await listener.AcceptTcpClientAsync();
                    }
                    catch (Exception)
                    {
                        return;
                    }
                    Interlocked.Increment(ref handshakes);
                    _ = Task.Run(async () =>
                    {
                        using (client)
                        using (var tls = new SslStream(client.GetStream()))
                        {
                            try
                            {
                                await tls.AuthenticateAsServerAsync(certificate);
                            }
                            catch (Exception)
                            {
                                // The client refused the certificate.
                            }
                        }
                    });
                }
            }
        }

#if DEBUG
        // These tests set FilesCom.Http.RetryPolicy.WaitClock, which exists only in Debug builds.
        // Retry waits observed at the timer (FilesCom.Http.RetryPolicy.WaitClock), on the real client path: with
        // InitialNetworkRequestDelay 0.5, MaxNetworkRetryDelay 1 and two retries, the SDK's DelayGenerator asks Polly for
        // exactly a 0.5 s and then a 1 s wait; each wait's timer fires no earlier than asked; and no attempt is sent while
        // a wait's timer is pending. Expectations come from the configuration, never from logs or elapsed totals.
        private static readonly TimeSpan[] TimerProofExpectedWaits = { TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(1) };

        private sealed class TimerRecord
        {
            public TimeSpan Due;
            public long Created;
            public long Fired;
        }

        // Wraps the system timers and records each requested due time, its creation and its callback. fireAtOnce makes
        // every timer fire immediately while still recording what was asked: a premature-timer control.
        private sealed class RecordingClock : TimeProvider
        {
            private readonly bool fireAtOnce;
            public readonly List<TimerRecord> Timers = new List<TimerRecord>();

            public RecordingClock(bool fireAtOnce)
            {
                this.fireAtOnce = fireAtOnce;
            }

            public override ITimer CreateTimer(TimerCallback callback, object state, TimeSpan dueTime, TimeSpan period)
            {
                var record = new TimerRecord { Due = dueTime, Created = Stopwatch.GetTimestamp() };
                lock (Timers)
                {
                    Timers.Add(record);
                }
                return base.CreateTimer(s =>
                {
                    Interlocked.CompareExchange(ref record.Fired, Stopwatch.GetTimestamp(), 0);
                    callback(s);
                }, state, fireAtOnce ? TimeSpan.Zero : dueTime, period);
            }
        }

        // Holds every timer until the test releases it, whatever its due time.
        private sealed class HeldClock : TimeProvider
        {
            public readonly List<(TimeSpan Due, HeldTimer Timer)> Timers = new List<(TimeSpan, HeldTimer)>();

            public override ITimer CreateTimer(TimerCallback callback, object state, TimeSpan dueTime, TimeSpan period)
            {
                var timer = new HeldTimer(callback, state);
                lock (Timers)
                {
                    Timers.Add((dueTime, timer));
                }
                return timer;
            }
        }

        // A timer that fires only when released, once, and never after its owner disposes it.
        private sealed class HeldTimer : ITimer
        {
            private readonly TimerCallback callback;
            private readonly object state;
            private int done;

            public HeldTimer(TimerCallback callback, object state)
            {
                this.callback = callback;
                this.state = state;
            }

            public bool Released { get; private set; }

            public void Release()
            {
                if (Interlocked.Exchange(ref done, 1) == 0)
                {
                    Released = true;
                    ThreadPool.QueueUserWorkItem(_ => callback(state));
                }
            }

            public bool Change(TimeSpan dueTime, TimeSpan period) => Volatile.Read(ref done) == 0;

            public void Dispose()
            {
                Interlocked.Exchange(ref done, 1);
            }

            public ValueTask DisposeAsync()
            {
                Dispose();
                return default;
            }
        }

        private sealed class TimerProofEvidence
        {
            public List<TimerRecord> Timers;
            public List<long> Arrivals;
            public string[] Retries;
            public long Start;
            public long End;
        }

        private static double TimerProofMs(long from, long to) => (to - from) * 1000.0 / Stopwatch.Frequency;

        private FilesClient TimerProofClient(IReadOnlyList<TimeSpan> responseStalls, List<long> arrivals)
        {
            site.On("GET", "/users/9", async request =>
            {
                int index;
                lock (arrivals)
                {
                    arrivals.Add(Stopwatch.GetTimestamp());
                    index = arrivals.Count - 1;
                }
                if (index < responseStalls.Count && responseStalls[index] > TimeSpan.Zero)
                {
                    await Task.Delay(responseStalls[index]);
                }
                return LoopbackSite.Status(503);
            });
            return FilesClient.Create(new FilesConfiguration { BaseUrl = site.Url, ApiKey = site.ApiKey, MaxNetworkRetries = 2, InitialNetworkRequestDelay = 0.5, MaxNetworkRetryDelay = 1 });
        }

        private async Task<TimerProofEvidence> TimerProofRun(RecordingClock clock, IReadOnlyList<TimeSpan> responseStalls)
        {
            var arrivals = new List<long>();
            FilesClient client = TimerProofClient(responseStalls, arrivals);
            var evidence = new TimerProofEvidence();
            using (var logs = new LogCapture())
            {
                TimeProvider previous = FilesCom.Http.RetryPolicy.WaitClock.Value;
                FilesCom.Http.RetryPolicy.WaitClock.Value = clock;
                try
                {
                    evidence.Start = Stopwatch.GetTimestamp();
                    await ThrowsSdkException(client.Users.FindAsync(9));
                    evidence.End = Stopwatch.GetTimestamp();
                }
                finally
                {
                    FilesCom.Http.RetryPolicy.WaitClock.Value = previous;
                }
                evidence.Retries = logs.Lines("Retrying GET /users after HTTP 503").Select(line => line.Substring(line.IndexOf("retry ", StringComparison.Ordinal))).ToArray();
            }
            lock (arrivals)
            {
                evidence.Arrivals = arrivals.ToList();
            }
            lock (clock.Timers)
            {
                evidence.Timers = clock.Timers.ToList();
            }
            return evidence;
        }

        // The honored-wait oracle: requested waits equal the configured ones, each timer fired no earlier than asked
        // (2 ms for the stamp taken just before the system timer starts), nothing was sent while a timer was pending,
        // and an attempt followed each timer.
        private static List<string> TimerProofViolations(TimerProofEvidence evidence)
        {
            var violations = new List<string>();
            if (!evidence.Timers.Select(timer => timer.Due).SequenceEqual(TimerProofExpectedWaits))
            {
                violations.Add($"requested waits [{string.Join(", ", evidence.Timers.Select(timer => timer.Due))}], expected [00:00:00.5000000, 00:00:01]");
            }
            for (int k = 0; k < Math.Min(evidence.Timers.Count, TimerProofExpectedWaits.Length); k++)
            {
                TimerRecord timer = evidence.Timers[k];
                if (timer.Fired == 0)
                {
                    violations.Add($"timer {k + 1} never fired");
                    continue;
                }
                double held = TimerProofMs(timer.Created, timer.Fired);
                if (held < TimerProofExpectedWaits[k].TotalMilliseconds - 2)
                {
                    violations.Add($"timer {k + 1} fired {held:F1} ms after it was created, before its {TimerProofExpectedWaits[k].TotalMilliseconds} ms");
                }
                int sentWhilePending = evidence.Arrivals.Count(arrival => arrival > timer.Created && arrival < timer.Fired);
                if (sentWhilePending != 0)
                {
                    violations.Add($"{sentWhilePending} attempt(s) sent while timer {k + 1} was pending");
                }
                if (!evidence.Arrivals.Any(arrival => arrival >= timer.Fired))
                {
                    violations.Add($"no attempt after timer {k + 1} fired");
                }
            }
            return violations;
        }

        private static string TimerProofDescribe(TimerProofEvidence evidence) =>
            $"timers [{string.Join("; ", evidence.Timers.Select(timer => $"due {timer.Due.TotalMilliseconds} ms, created {TimerProofMs(evidence.Start, timer.Created):F1}, fired {(timer.Fired == 0 ? "never" : TimerProofMs(evidence.Start, timer.Fired).ToString("F1", CultureInfo.InvariantCulture))}"))}], " +
            $"arrivals [{string.Join(", ", evidence.Arrivals.Select(arrival => TimerProofMs(evidence.Start, arrival).ToString("F1", CultureInfo.InvariantCulture)))}], end {TimerProofMs(evidence.Start, evidence.End):F1} ms, retries [{string.Join(" | ", evidence.Retries)}]";

        [TestMethod]
        [Timeout(60000)]
        public async Task TimerProofRetryWaitsAreRequestedAndHonoredAtTheTimer()
        {
            TimerProofEvidence evidence = await TimerProofRun(new RecordingClock(fireAtOnce: false), Array.Empty<TimeSpan>());
            Console.WriteLine("TIMERPROOF honored: " + TimerProofDescribe(evidence));

            List<string> violations = TimerProofViolations(evidence);
            Assert.AreEqual(0, violations.Count, string.Join("; ", violations) + " -- " + TimerProofDescribe(evidence));
            // Policy retry events (the SDK's OnRetry log) and HTTP transmissions are counted separately.
            CollectionAssert.AreEqual(new[] { "retry 1 of 2, in 0.5 seconds", "retry 2 of 2, in 1 seconds" }, evidence.Retries);
            Assert.AreEqual(3, evidence.Arrivals.Count, TimerProofDescribe(evidence));
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task TimerProofNoAttemptIsSentUntilItsTimerFires()
        {
            var arrivals = new List<long>();
            FilesClient client = TimerProofClient(Array.Empty<TimeSpan>(), arrivals);
            var clock = new HeldClock();
            int Sent()
            {
                lock (arrivals)
                {
                    return arrivals.Count;
                }
            }
            int Held()
            {
                lock (clock.Timers)
                {
                    return clock.Timers.Count;
                }
            }
            using (var lifetime = new CancellationTokenSource())
            {
                Task<User> finding;
                TimeProvider previous = FilesCom.Http.RetryPolicy.WaitClock.Value;
                FilesCom.Http.RetryPolicy.WaitClock.Value = clock;
                try
                {
                    finding = client.Users.FindAsync(9, cancellationToken: lifetime.Token);
                }
                finally
                {
                    FilesCom.Http.RetryPolicy.WaitClock.Value = previous;
                }
                Exception primary = null;
                try
                {
                    for (int k = 0; k < 2; k++)
                    {
                        await WaitFor(() => Held() == k + 1);
                        (TimeSpan due, HeldTimer timer) = clock.Timers[k];
                        Assert.AreEqual(TimerProofExpectedWaits[k], due);
                        // Held well past its due time: the next attempt must not be sent until the timer fires.
                        await Task.Delay(due + TimeSpan.FromSeconds(1));
                        Assert.AreEqual(k + 1, Sent(), $"An attempt was sent while wait {k + 1} ({due.TotalMilliseconds} ms) was held past its due time.");
                        Console.WriteLine($"TIMERPROOF held: wait {k + 1} due {due.TotalMilliseconds} ms held {(due + TimeSpan.FromSeconds(1)).TotalMilliseconds} ms with {Sent()} attempt(s) sent");
                        timer.Release();
                        await WaitFor(() => Sent() == k + 2);
                    }
                    await ThrowsSdkException(finding);
                    Assert.AreEqual(3, Sent());
                    Assert.AreEqual(2, Held());
                }
                catch (Exception e)
                {
                    primary = e;
                }
                // On every exit: release whatever is still held, cancel the operation, and wait a bounded time for it
                // to finish, so no held SDK work outlives the test. A cleanup failure is reported with, not instead of,
                // a primary failure.
                var cleanup = new List<Exception>();
                List<HeldTimer> held;
                lock (clock.Timers)
                {
                    held = clock.Timers.Select(entry => entry.Timer).ToList();
                }
                foreach (HeldTimer timer in held)
                {
                    timer.Release();
                }
                lifetime.Cancel();
                if (await Task.WhenAny(finding, Task.Delay(TimeSpan.FromSeconds(10))) != finding)
                {
                    cleanup.Add(new TimeoutException("The SDK operation did not finish within 10 s after its timers were released and it was cancelled."));
                }
                else if (finding.IsFaulted && !(finding.Exception.GetBaseException() is SdkException) && !(finding.Exception.GetBaseException() is OperationCanceledException))
                {
                    cleanup.Add(finding.Exception.GetBaseException());
                }
                Console.WriteLine($"TIMERPROOF held cleanup: operation {finding.Status}, {held.Count} timer(s) released or already fired, cleanup failures {cleanup.Count}");
                if (primary != null && cleanup.Count > 0)
                {
                    throw new AggregateException("The test failed and its cleanup also failed.", new[] { primary }.Concat(cleanup));
                }
                if (primary != null)
                {
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(primary).Throw();
                }
                if (cleanup.Count > 0)
                {
                    throw new AggregateException("The test passed but its cleanup failed.", cleanup);
                }
            }
        }

        // Control: timers that fire at once while the configured waits are still asked for, with request stalls that
        // keep the whole operation longer than the waits it skipped. The honored-wait oracle must reject it.
        [TestMethod]
        [Timeout(60000)]
        public async Task TimerProofControlAPrematureTimerIsRejectedEvenWhenRequestsStall()
        {
            TimerProofEvidence evidence = await TimerProofRun(new RecordingClock(fireAtOnce: true), new[] { TimeSpan.Zero, TimeSpan.FromMilliseconds(600), TimeSpan.FromMilliseconds(1100) });
            Console.WriteLine("TIMERPROOF premature control: " + TimerProofDescribe(evidence));

            Assert.IsTrue(TimerProofMs(evidence.Start, evidence.End) >= 1500, "The stalls did not make the operation as long as the skipped waits: " + TimerProofDescribe(evidence));
            CollectionAssert.AreEqual(new[] { "retry 1 of 2, in 0.5 seconds", "retry 2 of 2, in 1 seconds" }, evidence.Retries);
            List<string> violations = TimerProofViolations(evidence);
            Console.WriteLine("TIMERPROOF premature control violations: " + string.Join("; ", violations));
            Assert.IsTrue(violations.Any(v => v.StartsWith("timer 1 fired", StringComparison.Ordinal)) && violations.Any(v => v.StartsWith("timer 2 fired", StringComparison.Ordinal)),
                "The oracle accepted premature timers: " + string.Join("; ", violations));
        }

        // A storage Retry-After of more than a minute is waited for in full: the retry's timer is asked for the whole
        // 61 seconds, nothing is sent while it has not fired, and cancelling meanwhile ends the download at once.
        [TestMethod]
        [Timeout(60000)]
        public async Task StorageWaitsTheWholeRetryAfterAndSaysSo()
        {
            site.On("GET", "/files/remote.bin", request => LoopbackSite.Json($"{{\"path\":\"remote.bin\",\"download_uri\":\"{site.Url}/download\"}}"));
            site.On("GET", "//download", request =>
            {
                ResponseMessage throttled = LoopbackSite.Status(429);
                throttled.AddHeader("Retry-After", "61");
                return throttled;
            });
            FilesClient client = FilesClient.Create(site.Configuration(maxNetworkRetries: 3));
            var clock = new HeldClock();
            int Held()
            {
                lock (clock.Timers)
                {
                    return clock.Timers.Count;
                }
            }

            using (var logs = new LogCapture())
            using (var cancellation = new CancellationTokenSource())
            {
                Task downloading;
                TimeProvider previous = FilesCom.Http.RetryPolicy.WaitClock.Value;
                FilesCom.Http.RetryPolicy.WaitClock.Value = clock;
                try
                {
                    downloading = client.RemoteFiles.DownloadFileAsync("remote.bin", new MemoryStream(), cancellationToken: cancellation.Token);
                }
                finally
                {
                    FilesCom.Http.RetryPolicy.WaitClock.Value = previous;
                }
                await WaitFor(() => Held() == 1);
                Assert.AreEqual(TimeSpan.FromSeconds(61), clock.Timers[0].Due);
                Assert.AreEqual(
                    "Retrying the download after HTTP 429: retry 1 of 3, in 61 seconds, as the server's Retry-After asked",
                    logs.Lines("Retrying the download").Single());
                await Task.Delay(300);
                Assert.AreEqual(1, Count("GET /download"), "Nothing is sent before the wait ends.");

                cancellation.Cancel();
                Assert.AreSame(downloading, await Task.WhenAny(downloading, Task.Delay(Promptly)), "Cancelling did not end the wait.");
                await Assert.ThrowsAsync<OperationCanceledException>(() => downloading);
            }
            await Task.Delay(300);
            Assert.AreEqual(1, Count("GET /download"), "Nothing is sent after cancellation during the wait.");
            Assert.IsFalse(clock.Timers[0].Timer.Released);
        }
#endif
    }
}