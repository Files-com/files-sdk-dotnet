using FilesCom;
using FilesCom.Models;
using FilesCom.Transfers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WireMock;

namespace FilesTests
{
    // client.FileSystem against loopback sites: remote files read and written through streams, whole, and the folder,
    // existence, delete, copy and move operations that return once Files.com has finished.
    [TestClass]
    public class RemoteFileSystemTest
    {
        private static readonly byte[] Contents = Enumerable.Range(0, 50 * 1024).Select(i => (byte)(i * 13 + 5)).ToArray();

        private static DownloadOptions Parts(int concurrency = 2) => new DownloadOptions { PartSize = 16 * 1024, MaxConcurrency = concurrency };

        [TestMethod]
        [Timeout(60000)]
        public async Task FileOpenedToReadGivesItsBytesForwardOnly()
        {
            using (var server = new DownloadServer(Contents))
            {
                var read = new MemoryStream();
                using (Stream stream = await Client(server).FileSystem.OpenReadAsync("remote.bin", Parts()))
                {
                    Assert.IsTrue(stream.CanRead);
                    Assert.IsFalse(stream.CanSeek || stream.CanWrite);
                    Assert.ThrowsExactly<NotSupportedException>(() => stream.Length);
                    var buffer = new byte[1000];
                    int count;
                    while ((count = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        read.Write(buffer, 0, count);
                    }
                }

                CollectionAssert.AreEqual(Contents, read.ToArray());
                Assert.IsTrue(server.StorageRequests.All(request => request.Range.HasValue), "Read as a bound download, in parts.");
                Assert.IsTrue(server.StorageRequests.Skip(1).All(request => request.IfMatch == server.ETag), "Every part after the first is checked against the entity tag the first was answered with.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task FailureBeforeTheFirstByteIsThrownWhenOpening()
        {
            using (var server = new DownloadServer(Contents))
            {
                await Assert.ThrowsAsync<NotFoundException>(() => Client(server).FileSystem.OpenReadAsync("missing.bin"));
                Assert.AreEqual(0, server.StorageRequests.Count);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task FailurePartwayReachesTheReaderAfterEverythingBeforeIt()
        {
            using (var server = new DownloadServer(Contents))
            {
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range?.First == 16 * 1024)
                    {
                        // The part's headers and half its bytes, then the connection closes.
                        await connection.SendAsync($"HTTP/1.1 206 Partial Content\r\nContent-Length: {16 * 1024}\r\nContent-Range: bytes {16 * 1024}-{32 * 1024 - 1}/{Contents.Length}\r\nETag: {server.ETag}\r\n\r\n");
                        await connection.SendAsync(Contents.Skip(16 * 1024).Take(8 * 1024).ToArray());
                        connection.Dispose();
                        return true;
                    }
                    return false;
                };
                var read = new MemoryStream();
                Exception failure = null;
                using (Stream stream = await Client(server).FileSystem.OpenReadAsync("remote.bin", Parts(concurrency: 1)))
                {
                    try
                    {
                        await stream.CopyToAsync(read);
                    }
                    catch (Exception e)
                    {
                        failure = e;
                    }
                }

                Assert.IsNotNull(failure, "The reader learns the file did not come whole.");
                CollectionAssert.AreEqual(Contents.Take(16 * 1024).ToArray(), read.ToArray(), "Exactly the part before the failure.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task DisposingAStreamOpenedToReadStopsTheDownloadBehindIt()
        {
            using (var server = new DownloadServer(Contents))
            {
                var limiter = new TransferLimiter();
                Stream stream = await Client(server).FileSystem.OpenReadAsync("remote.bin", new DownloadOptions { PartSize = 1024, MaxConcurrency = 2, Limiter = limiter });
                var buffer = new byte[10];
                Assert.AreEqual(10, await stream.ReadAsync(buffer, 0, 10));
                Stopwatch disposing = Stopwatch.StartNew();

                await stream.DisposeAsync();
                // What the download sent until now was sent before the stream was disposed, wherever it is on its way.
                int sent = server.StorageSends.Count;

                Assert.IsTrue(disposing.Elapsed < TimeSpan.FromSeconds(5));
                // Everything it asked for before it stopped is counted, however late the server reads it.
                await server.SettledAsync();
                int requests = server.StorageRequests.Count;
                Assert.IsTrue(requests < Contents.Length / 1024, $"The download stopped early: {requests} parts were asked for.");
                Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes + limiter.QueuedRequests + limiter.QueuedBufferReservations, "Everything it held was released.");
                await Task.Delay(100);
                Assert.AreEqual(sent, server.StorageSends.Count, "Nothing is sent after the stream is disposed.");
                Assert.AreEqual(requests, server.StorageRequests.Count, "Nothing is asked for after the stream is disposed.");
                Assert.ThrowsExactly<ObjectDisposedException>(() => stream.Read(buffer, 0, 1));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task FileOpenedToWriteBecomesWhatWasWrittenWhenDisposed()
        {
            using (var server = new UploadServer())
            {
                Stream stream = await Client(server).FileSystem.OpenWriteAsync("written.bin");
                Assert.IsTrue(stream.CanWrite);
                Assert.IsFalse(stream.CanRead || stream.CanSeek);
                byte[] contents = Enumerable.Range(0, 30).Select(i => (byte)i).ToArray();
                await stream.WriteAsync(contents, 0, 10);
                stream.Write(contents, 10, 10);
                await stream.WriteAsync(contents, 20, 10);
                Assert.AreEqual(0, server.Finishes.Count, "Not completed before the stream is disposed.");

                await stream.DisposeAsync();

                CollectionAssert.AreEqual(contents, server.AcceptedBytes("written.bin"));
                Assert.AreEqual(30, server.Finishes.Single().GetProperty("size").GetInt64());
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task UploadFailureOfAStreamOpenedToWriteIsThrownByItsDisposal()
        {
            using (var server = new UploadServer { RetryParts = "false" })
            {
                server.RespondToPart = request => Task.FromResult(LoopbackSite.Status(403));
                Stream stream = await Client(server).FileSystem.OpenWriteAsync("written.bin");
                await stream.WriteAsync(new byte[20], 0, 20);

                await Assert.ThrowsAsync<ApiException>(async () => await stream.DisposeAsync());

                Assert.AreEqual(0, server.Finishes.Count);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task CancellingAStreamOpenedToWriteAbandonsTheFile()
        {
            using (var server = new UploadServer())
            using (var cancellation = new CancellationTokenSource())
            {
                Stream stream = await Client(server).FileSystem.OpenWriteAsync("written.bin", null, cancellation.Token);
                await stream.WriteAsync(new byte[20], 0, 20);
                cancellation.Cancel();

                await stream.DisposeAsync();

                Assert.AreEqual(0, server.Finishes.Count, "An abandoned file is not completed.");
            }
        }

        // One client writes a whole file and reads it back, against the simulator test.sh starts for this test alone
        // (SharedSimulator): what the simulator then stores, read apart from the SDK, and what the SDK read are both exactly
        // what was written.
        [TestMethod]
        [Timeout(60000)]
        [TestCategory("SharedSimulatorFilesystem")]
        public async Task WholeFilesAreWrittenAndRead()
        {
            using (SharedSimulator simulator = await SharedSimulator.StartAsync())
            {
                FilesClient client = simulator.CreateClient();
                byte[] read;
                try
                {
                    await client.FileSystem.WriteAllBytesAsync("whole.bin", Contents);
                    read = await client.FileSystem.ReadAllBytesAsync("whole.bin");
                }
                finally
                {
                    await client.DisposeAsync();
                }

                CollectionAssert.AreEqual(Contents, await simulator.StoredBytesAsync("whole.bin"));
                CollectionAssert.AreEqual(Contents, read);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task FoldersAreListedAloneOrWithEverythingBelowThem()
        {
            using (var server = new DownloadServer(Contents) { ListPageSize = 2 })
            {
                server.AddFile("backup/a.bin", new byte[1]);
                server.AddFile("backup/sub/b.bin", new byte[2]);
                server.AddFile("backup/sub/deeper/c.bin", new byte[3]);
                server.AddFile("backup/z.bin", new byte[4]);
                RemoteFileSystem files = Client(server).FileSystem;

                var alone = new List<string>();
                await foreach (RemoteFile entry in files.EnumerateAsync("backup"))
                {
                    alone.Add(entry.Path);
                }
                var all = new List<string>();
                await foreach (RemoteFile entry in files.EnumerateAsync("/backup/", recursive: true))
                {
                    all.Add($"{entry.Path} {entry.Type}");
                }

                CollectionAssert.AreEqual(new[] { "backup/a.bin", "backup/sub", "backup/z.bin" }, alone);
                CollectionAssert.AreEqual(new[] { "backup/a.bin file", "backup/sub directory", "backup/z.bin file", "backup/sub/b.bin file", "backup/sub/deeper directory", "backup/sub/deeper/c.bin file" }, all);
            }
        }

        // One client looks up paths and creates folders against the simulator test.sh starts for this test alone
        // (SharedSimulator), which holds the folder a-folder and the file a-file on a site that creates a missing
        // parent folder only when asked: what the SDK finds follows what is there, and the folder it creates is there
        // afterwards, with the missing parent the SDK asked for (mkdir_parents), both read apart from the SDK.
        [TestMethod]
        [Timeout(60000)]
        [TestCategory("SharedSimulatorFilesystem")]
        public async Task ExistenceAndFolderCreationFollowWhatIsThere()
        {
            using (SharedSimulator simulator = await SharedSimulator.StartWithFolderFixtureAsync())
            {
                Assert.IsNull(await simulator.TypeAtAsync("new"), "Nothing is at new before the SDK creates it.");
                var bodies = new RequestBodies(simulator.Url, "/api/rest/v1/folders/new/deeper");
                FilesClient client = simulator.CreateClient(bodies);
                try
                {
                    RemoteFileSystem files = client.FileSystem;

                    Assert.IsTrue(await files.FileExistsAsync("a-file"));
                    Assert.IsFalse(await files.FileExistsAsync("a-folder"));
                    Assert.IsFalse(await files.FileExistsAsync("nothing"));
                    Assert.IsTrue(await files.DirectoryExistsAsync("a-folder"));
                    Assert.IsFalse(await files.DirectoryExistsAsync("a-file"));
                    await files.CreateDirectoryAsync("new/deeper");
                    await files.CreateDirectoryAsync("a-folder");
                    await Assert.ThrowsExactlyAsync<IOException>(() => files.CreateDirectoryAsync("a-file"));
                }
                finally
                {
                    await client.DisposeAsync();
                }

                Assert.AreEqual("directory", await simulator.TypeAtAsync("new"), "The missing parent is created with the folder.");
                Assert.AreEqual("directory", await simulator.TypeAtAsync("new/deeper"));
                StringAssert.Contains(bodies.FirstBody(), "\"mkdir_parents\":true");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task DeleteCopyAndMoveReturnOnceFilesComHasFinished()
        {
            using (var site = new LoopbackSite("key-a", "11"))
            {
                int checks = 0;
                site.On("DELETE", "/files/*", request => LoopbackSite.Status(204));
                site.On("GET", "/file_actions/metadata/*", request => Interlocked.Increment(ref checks) < 2
                    ? LoopbackSite.Json("{\"path\":\"folder\",\"type\":\"directory\"}")
                    : LoopbackSite.Json("{\"error\":\"Not Found\",\"http-code\":404,\"type\":\"not-found/file-not-found\"}", status: 404));
                site.On("POST", "/file_actions/copy/*", request => LoopbackSite.Json("{\"status\":\"pending\",\"file_migration_id\":7}"));
                site.On("POST", "/file_actions/move/*", request => LoopbackSite.Json("{\"status\":\"pending\",\"file_migration_id\":8}"));
                int migrationChecks = 0;
                site.On("GET", "/file_migrations/*", request => LoopbackSite.Json(Interlocked.Increment(ref migrationChecks) % 2 == 1
                    ? "{\"id\":7,\"status\":\"processing\"}"
                    : "{\"id\":7,\"status\":\"completed\"}"));
                RemoteFileSystem files = FilesClient.Create(site.Configuration()).FileSystem;

                await files.DeleteAsync("folder", recursive: true);
                await files.CopyAsync("a.bin", "b.bin", overwrite: true);
                await files.MoveAsync("b.bin", "c.bin");

                Assert.AreEqual(2, checks, "The deletion is waited for until nothing is there.");
                StringAssert.Contains(site.Requests.Single(request => request.Method == "DELETE").Path.ToLowerInvariant(), "recursive=true");
                StringAssert.Contains(site.Requests.Single(request => request.Path.StartsWith("/file_actions/copy/")).Body, "\"overwrite\":true");
                Assert.AreEqual(4, migrationChecks, "The copy and the move are each waited for until their migration completes.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task CompoundOperationsKeepTheClientSettingsTheyStartedWith()
        {
            using (var site = new LoopbackSite("key-a", "11"))
            {
                // A client made with the constructor reads its configuration as each operation starts.
                FilesConfiguration configuration = site.Configuration();
                var client = new FilesClient(configuration);
                site.On("POST", "/file_actions/copy/*", request =>
                {
                    configuration.ApiKey = "key-b";
                    return LoopbackSite.Json("{\"status\":\"pending\",\"file_migration_id\":7}");
                });
                int polls = 0;
                site.On("GET", "/file_migrations/*", request => LoopbackSite.Json(Interlocked.Increment(ref polls) < 3 ? "{\"id\":7,\"status\":\"processing\"}" : "{\"id\":7,\"status\":\"completed\"}"));
                site.On("DELETE", "/files/*", request =>
                {
                    configuration.ApiKey = "key-c";
                    return LoopbackSite.Status(204);
                });
                site.On("GET", "/file_actions/metadata/*", request => LoopbackSite.Json("{\"error\":\"Not Found\",\"http-code\":404,\"type\":\"not-found/file-not-found\"}", status: 404));

                await client.FileSystem.CopyAsync("a", "b");
                await client.FileSystem.DeleteAsync("b");

                CollectionAssert.AreEqual(new[] { "key-a", "key-a", "key-a" }, site.Requests.Where(r => r.Path.StartsWith("/file_migrations/")).Select(r => r.ApiKey).ToArray(), "The copy's waits use the settings the copy started with.");
                ReceivedRequest[] delete = site.Requests.Where(r => r.Method == "DELETE" || r.Path.StartsWith("/file_actions/metadata/")).ToArray();
                Assert.IsTrue(delete.All(r => r.ApiKey == "key-b"), "The deletion and its wait use the settings the deletion started with: " + string.Join(", ", delete.Select(r => r.ApiKey)));
            }
        }

        private static FilesClient Client(DownloadServer server)
        {
            return server.Client();
        }

        private static FilesClient Client(UploadServer server)
        {
            return FilesClient.Create(server.Site.Configuration());
        }

        // The simulator with-scenario.rb started for the test, which test.sh runs there alone, as the launcher names it.
        // Before anything is sent, its URL must be http://HOST:PORT on one loopback address, and its schema SHA-256, instance
        // and synthetic API key of the launcher's forms; then readiness there must be this run's simulation server (contract
        // version 3, that schema and instance), which is reset to a state of its own, empty (StartAsync) or with a
        // folder and a file on a site that creates a missing parent only when asked (StartWithFolderFixtureAsync), and
        // must then report the reset's new epoch. Without that simulator the test fails: it never falls back to another
        // service. It also reads what is at a path (TypeAtAsync) and a file's stored bytes (StoredBytesAsync) apart from
        // the SDK. Its own requests name themselves (OracleAgent), take no proxy, follow no redirect and read a bounded
        // response, and only its lookups and a download's descriptor carry the synthetic key.
        private sealed class SharedSimulator : IDisposable
        {
            private const string OracleAgent = "files-dotnet-filesystem-oracle/1";
            // The most of a response that is read: readiness, the reset, a lookup, a download's descriptor or the file
            // itself.
            private const int MaxResponseBytes = 1024 * 1024;
            // The reset of StartWithFolderFixtureAsync: a site that creates a missing parent folder only when a request asks
            // for it (mkdir_parents), holding the folder a-folder and the file a-file.
            private const string FolderFixture = "{\"profile\":{\"site_policy\":{\"always_mkdir_parents\":false}},"
                + "\"fixtures\":{\"folders\":[\"a-folder\"],\"files\":[{\"path\":\"a-file\",\"text\":\"kept\"}]}}";
            private readonly System.Net.Http.HttpClient http;

            private SharedSimulator(Uri url, string apiKey)
            {
                Url = url;
                ApiKey = apiKey;
                http = new System.Net.Http.HttpClient(new System.Net.Http.HttpClientHandler { UseProxy = false, AllowAutoRedirect = false })
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    MaxResponseContentBufferSize = MaxResponseBytes,
                };
                http.DefaultRequestHeaders.UserAgent.ParseAdd(OracleAgent);
            }

            // http://HOST:PORT, as the launcher gives it.
            internal Uri Url { get; }

            // The launcher's synthetic key, the only credential sent, and only to Url.
            internal string ApiKey { get; }

            // This run's simulator, reset to an empty state with no fixtures and the default profile.
            internal static Task<SharedSimulator> StartAsync()
            {
                return StartAsync("{}", strictParents: false);
            }

            // This run's simulator, reset to FolderFixture; readiness after the reset must also report a site that creates a
            // missing parent folder only when asked.
            internal static Task<SharedSimulator> StartWithFolderFixtureAsync()
            {
                return StartAsync(FolderFixture, strictParents: true);
            }

            private static async Task<SharedSimulator> StartAsync(string reset, bool strictParents)
            {
                string url = Launcher("FILES_MOCK_SERVER_URL");
                string host = Launcher("FILES_MOCK_SERVER_HOST");
                string port = Launcher("FILES_MOCK_SERVER_PORT");
                string schema = Launcher("FILES_MOCK_SCHEMA_SHA256");
                string instance = Launcher("FILES_MOCK_INSTANCE");
                string apiKey = Launcher("FILES_MOCK_API_KEY");
                if (!System.Net.IPAddress.TryParse(host, out System.Net.IPAddress address) || !System.Net.IPAddress.IsLoopback(address) || address.ToString() != host)
                {
                    Assert.Fail($"FILES_MOCK_SERVER_HOST {host} is not a loopback address");
                }
                if (!int.TryParse(port, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out int number) || number < 1 || number > 65535
                    || number.ToString(System.Globalization.CultureInfo.InvariantCulture) != port)
                {
                    Assert.Fail($"FILES_MOCK_SERVER_PORT {port} is not a port");
                }
                if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri) || url != $"http://{host}:{port}")
                {
                    Assert.Fail($"FILES_MOCK_SERVER_URL {url} is not http://{host}:{port}");
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(schema, @"\A[0-9a-f]{64}\z"))
                {
                    Assert.Fail($"FILES_MOCK_SCHEMA_SHA256 {schema} is not a SHA-256");
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(instance, @"\A[A-Za-z0-9._-]{1,64}\z"))
                {
                    Assert.Fail("FILES_MOCK_INSTANCE is not an instance name");
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(apiKey, @"\Afilesmock[A-Za-z0-9]+\z"))
                {
                    Assert.Fail("FILES_MOCK_API_KEY is not the launcher's synthetic key");
                }
                var simulator = new SharedSimulator(uri, apiKey);
                try
                {
                    await simulator.ResetAsync(schema, instance, reset, strictParents);
                }
                catch
                {
                    simulator.Dispose();
                    throw;
                }
                return simulator;
            }

            // The SDK's own client for this simulator alone, with the launcher's key, through a real transport that takes no
            // proxy; with OBSERVED, each request it sends passes a BodyObserver first.
            internal FilesClient CreateClient(RequestBodies observed = null)
            {
                return FilesClient.Create(new FilesConfiguration { BaseUrl = Url.GetLeftPart(UriPartial.Authority), ApiKey = ApiKey },
                    observed == null
                        ? HttpTransport.FromHandlerFactory(() => new System.Net.Http.HttpClientHandler { UseProxy = false })
                        : HttpTransport.FromHandlerFactory(() => new BodyObserver(observed)
                        {
                            InnerHandler = new System.Net.Http.HttpClientHandler { UseProxy = false, AllowAutoRedirect = false },
                        }));
            }

            // What the simulator holds at PATH, read apart from the SDK (GET /file_actions/metadata/{path}): "file",
            // "directory", or null when nothing is there.
            internal async Task<string> TypeAtAsync(string path)
            {
                string escaped = string.Join("/", path.Split('/').Select(segment => Uri.EscapeDataString(segment)));
                using (var lookup = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, new Uri(Url, "/api/rest/v1/file_actions/metadata/" + escaped)))
                {
                    lookup.Headers.Add("X-FilesAPI-Key", ApiKey);
                    using (System.Net.Http.HttpResponseMessage response = await http.SendAsync(lookup))
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            return null;
                        }
                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            Assert.Fail($"GET /file_actions/metadata/{path} answered HTTP {(int)response.StatusCode}");
                        }
                        using (System.Text.Json.JsonDocument document = System.Text.Json.JsonDocument.Parse(await response.Content.ReadAsStringAsync()))
                        {
                            string type = Text(document.RootElement, "type");
                            if (Text(document.RootElement, "path") != path || (type != "file" && type != "directory"))
                            {
                                Assert.Fail($"GET /file_actions/metadata/{path} did not describe a file or folder at {path}");
                            }
                            return type;
                        }
                    }
                }
            }

            // The file at PATH as the simulator stores it, read apart from the SDK: its download descriptor
            // (GET /files/{path}), then the whole file from the download URL that names, which must be this simulator's own,
            // sent no credential.
            internal async Task<byte[]> StoredBytesAsync(string path)
            {
                var describe = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, new Uri(Url, "/api/rest/v1/files/" + Uri.EscapeDataString(path)));
                describe.Headers.Add("X-FilesAPI-Key", ApiKey);
                System.Text.Json.JsonElement file = await JsonAsync(describe, $"GET /files/{path}");
                if (!Uri.TryCreate(Text(file, "download_uri"), UriKind.Absolute, out Uri download) || Text(file, "path") != path || Text(file, "type") != "file"
                    || download.Scheme != Uri.UriSchemeHttp || download.Host != Url.Host || download.Port != Url.Port || download.UserInfo.Length > 0
                    || !download.AbsolutePath.StartsWith("/__files_mock/transfer/download/", StringComparison.Ordinal) || download.Query.Length > 0 || download.Fragment.Length > 0)
                {
                    Assert.Fail($"GET /files/{path} did not describe a file with a download URL of the simulator's own");
                }
                using (var fetch = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, download))
                using (System.Net.Http.HttpResponseMessage stored = await http.SendAsync(fetch))
                {
                    if (stored.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        Assert.Fail($"the download URL of {path} answered HTTP {(int)stored.StatusCode}");
                    }
                    return await stored.Content.ReadAsByteArrayAsync();
                }
            }

            // Readiness must be this run's server, as the launcher accepted it (with-scenario.rb#check_ready); the reset then
            // gives it RESET's state, and readiness must then report the reset's new epoch and, with STRICTPARENTS, a site
            // that creates a missing parent folder only when asked.
            private async Task ResetAsync(string schema, string instance, string reset, bool strictParents)
            {
                System.Text.Json.JsonElement before = await ControlAsync("ready");
                CheckReady(before, schema, instance, "before the reset");
                long? epoch = Number(await ControlAsync("reset", reset), "epoch");
                System.Text.Json.JsonElement after = await ControlAsync("ready");
                CheckReady(after, schema, instance, "after the reset");
                if (epoch == null || epoch <= Number(before, "epoch") || Number(after, "epoch") != epoch)
                {
                    Assert.Fail($"the reset of {Url} did not begin a new epoch that readiness then reports");
                }
                if (strictParents && !CreatesParentsOnlyWhenAsked(after))
                {
                    Assert.Fail($"readiness on {Url} after the reset does not report a site that creates a missing parent folder only when asked (namespace.site_policy.always_mkdir_parents false)");
                }
            }

            private void CheckReady(System.Text.Json.JsonElement ready, string schema, string instance, string when)
            {
                if (Text(ready, "status") != "ready" || Text(ready, "mode") != "simulation" || Number(ready, "contract_version") != 3
                    || Text(ready, "schema_sha256") != schema || Text(ready, "instance") != instance || Number(ready, "epoch") == null)
                {
                    Assert.Fail($"readiness on {Url} {when} is not this run's simulation server, of contract version 3 and the launcher's schema and instance");
                }
            }

            // A control request of the simulator: GET /ready, or POST /reset with BODY, {} for an empty state.
            private Task<System.Text.Json.JsonElement> ControlAsync(string name, string body = "{}")
            {
                bool reset = name == "reset";
                var request = new System.Net.Http.HttpRequestMessage(reset ? System.Net.Http.HttpMethod.Post : System.Net.Http.HttpMethod.Get, new Uri(Url, "/__files_mock/v1/" + name));
                if (reset)
                {
                    request.Content = new System.Net.Http.StringContent(body, System.Text.Encoding.UTF8, "application/json");
                }
                return JsonAsync(request, $"the simulator's {name}");
            }

            // The JSON object REQUEST is answered 200 with; anything else fails the test.
            private async Task<System.Text.Json.JsonElement> JsonAsync(System.Net.Http.HttpRequestMessage request, string what)
            {
                using (request)
                using (System.Net.Http.HttpResponseMessage response = await http.SendAsync(request))
                {
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        Assert.Fail($"{what} answered HTTP {(int)response.StatusCode}");
                    }
                    using (System.Text.Json.JsonDocument document = System.Text.Json.JsonDocument.Parse(await response.Content.ReadAsStringAsync()))
                    {
                        if (document.RootElement.ValueKind != System.Text.Json.JsonValueKind.Object)
                        {
                            Assert.Fail($"{what} answered no JSON object");
                        }
                        return document.RootElement.Clone();
                    }
                }
            }

            // NAME's string in ELEMENT, an object, or null.
            private static string Text(System.Text.Json.JsonElement element, string name)
            {
                return element.ValueKind == System.Text.Json.JsonValueKind.Object && element.TryGetProperty(name, out System.Text.Json.JsonElement value)
                    && value.ValueKind == System.Text.Json.JsonValueKind.String ? value.GetString() : null;
            }

            // NAME's whole number in ELEMENT, an object, or null.
            private static long? Number(System.Text.Json.JsonElement element, string name)
            {
                return element.ValueKind == System.Text.Json.JsonValueKind.Object && element.TryGetProperty(name, out System.Text.Json.JsonElement value)
                    && value.ValueKind == System.Text.Json.JsonValueKind.Number && value.TryGetInt64(out long number) ? number : (long?)null;
            }

            // Whether READY, readiness, reports namespace.site_policy.always_mkdir_parents false: a site that creates a
            // missing parent folder only when a request asks for it.
            private static bool CreatesParentsOnlyWhenAsked(System.Text.Json.JsonElement ready)
            {
                return ready.ValueKind == System.Text.Json.JsonValueKind.Object && ready.TryGetProperty("namespace", out System.Text.Json.JsonElement space)
                    && space.ValueKind == System.Text.Json.JsonValueKind.Object && space.TryGetProperty("site_policy", out System.Text.Json.JsonElement policy)
                    && policy.ValueKind == System.Text.Json.JsonValueKind.Object && policy.TryGetProperty("always_mkdir_parents", out System.Text.Json.JsonElement always)
                    && always.ValueKind == System.Text.Json.JsonValueKind.False;
            }

            // The launcher's value of NAME; the test fails without it.
            private static string Launcher(string name)
            {
                string value = Environment.GetEnvironmentVariable(name);
                if (string.IsNullOrEmpty(value))
                {
                    Assert.Fail($"{name} is not set: this test runs only against the simulator with-scenario.rb starts for it, as test.sh does");
                }
                return value;
            }

            public void Dispose()
            {
                http.Dispose();
            }
        }

        // The body of the first POST to one path on one simulator, as the SDK sent it through a BodyObserver.
        private sealed class RequestBodies
        {
            private readonly object gate = new object();
            private readonly Uri origin;
            private readonly string path;
            private string first;
            private string refused;

            // ORIGIN's scheme, host and port, and PATH once unescaped, name the requests whose bodies are kept.
            internal RequestBodies(Uri origin, string path)
            {
                this.origin = origin;
                this.path = path;
            }

            // Whether REQUEST is a POST to that path on that origin.
            internal bool Matches(System.Net.Http.HttpRequestMessage request)
            {
                Uri uri = request.RequestUri;
                return request.Method == System.Net.Http.HttpMethod.Post && uri != null && uri.IsAbsoluteUri && uri.Scheme == origin.Scheme
                    && uri.Host == origin.Host && uri.Port == origin.Port && Uri.UnescapeDataString(uri.AbsolutePath) == path;
            }

            internal void Keep(string body)
            {
                lock (gate)
                {
                    if (first == null)
                    {
                        first = body;
                    }
                }
            }

            // A body that could not be read as it was sent, which fails the test.
            internal void Refuse(string reason)
            {
                lock (gate)
                {
                    if (refused == null)
                    {
                        refused = reason;
                    }
                }
            }

            // The first body kept; the test fails when there was none, or one could not be read.
            internal string FirstBody()
            {
                lock (gate)
                {
                    if (refused != null)
                    {
                        Assert.Fail(refused);
                    }
                    if (first == null)
                    {
                        Assert.Fail($"the SDK sent no POST {path}");
                    }
                    return first;
                }
            }
        }

        // The first handler of a transport that keeps, for BODIES, the body of each request it names as the SDK sent it,
        // then passes on that same request with its own cancellation, changing nothing.
        private sealed class BodyObserver : System.Net.Http.DelegatingHandler
        {
            private readonly RequestBodies bodies;

            internal BodyObserver(RequestBodies bodies)
            {
                this.bodies = bodies;
            }

            protected override async Task<System.Net.Http.HttpResponseMessage> SendAsync(System.Net.Http.HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (bodies.Matches(request))
                {
                    if (request.Content is System.Net.Http.StringContent content)
                    {
                        bodies.Keep(await content.ReadAsStringAsync(cancellationToken));
                    }
                    else
                    {
                        bodies.Refuse($"POST {request.RequestUri.AbsolutePath} sent {(request.Content == null ? "no body" : request.Content.GetType().Name)}, not the JSON text the test reads");
                    }
                }
                return await base.SendAsync(request, cancellationToken);
            }
        }
    }
}