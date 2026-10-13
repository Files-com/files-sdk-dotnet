using FilesCom;
using FilesCom.Transfers;
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

namespace FilesTests
{
    // Timeouts limit how long a request may stall, not how long it may take: ReadTimeout waiting for a response and
    // between its reads, WriteTimeout while an upload part cannot send, ConnectTimeout while connecting. A request
    // that times out fails with NetworkTimeoutException, after the retries a connection failure gets, and its
    // connection is closed rather than left behind. Time spent on the caller's own streams is not counted.
    [TestClass]
    public class NetworkTimeoutTest
    {
        private static readonly TimeSpan Promptly = TimeSpan.FromSeconds(10);
        // The receive buffer of a storage server that stops reading an upload part: a few KiB, against a 16 MiB part.
        private const int StalledReceiveBuffer = 4096;
        private static readonly byte[] Contents = Enumerable.Range(0, 5000).Select(value => (byte)(value % 251)).ToArray();
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
        public async Task ResponseThatDoesNotArriveTimesOutAndIsRetried()
        {
            using (var api = new ScriptedServer(async connection =>
            {
                await connection.ReadRequestHeadAsync();
                await connection.HoldAsync();
            }))
            {
                FilesConfiguration configuration = Configuration(api, maxNetworkRetries: 1);
                configuration.ReadTimeout = 1;
                Stopwatch elapsed = Stopwatch.StartNew();

                NetworkTimeoutException timeout = await Assert.ThrowsExactlyAsync<NetworkTimeoutException>(() => FilesClient.Create(configuration).Users.FindAsync(1));

                Assert.AreEqual("No response arrived within 1 second (ReadTimeout).", timeout.Message);
                Assert.AreEqual(2, api.Connections.Count, "The request was sent again after its first attempt timed out.");
                Assert.IsTrue(elapsed.Elapsed >= TimeSpan.FromSeconds(1.9) && elapsed.Elapsed < Promptly, $"Took {elapsed.Elapsed.TotalSeconds} s for two one-second timeouts.");
                await AssertClosed(api);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task ApiResponseThatStopsArrivingTimesOut()
        {
            using (var api = new ScriptedServer(async connection =>
            {
                await connection.ReadRequestHeadAsync();
                await connection.SendHeadAsync(200, 100);
                await connection.SendAsync("{\"id\":1,");
                await connection.HoldAsync();
            }))
            {
                FilesConfiguration configuration = Configuration(api);
                configuration.ReadTimeout = 1;

                NetworkTimeoutException timeout = await Assert.ThrowsExactlyAsync<NetworkTimeoutException>(() => FilesClient.Create(configuration).Users.FindAsync(1));

                Assert.AreEqual("The response stopped arriving: no data arrived for 1 second (ReadTimeout).", timeout.Message);
                await AssertClosed(api);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task ResponseTakesAsLongAsItKeepsArriving()
        {
            using (var storage = new ScriptedServer(async connection =>
            {
                await connection.ReadRequestHeadAsync();
                await connection.SendHeadAsync(200, Contents.Length);
                for (int sent = 0; sent < Contents.Length; sent += 1000)
                {
                    await Task.Delay(400);
                    await connection.SendAsync(Contents.Skip(sent).Take(1000).ToArray());
                }
            }))
            {
                ServeDownloadOf(storage.Url);
                FilesConfiguration configuration = site.Configuration();
                configuration.ReadTimeout = 1;
                var destination = new MemoryStream();
                Stopwatch elapsed = Stopwatch.StartNew();

                await FilesClient.Create(configuration).RemoteFiles.DownloadFileAsync("remote.bin", destination);

                Assert.IsTrue(elapsed.Elapsed > TimeSpan.FromSeconds(1.5), "The download lasted longer than its ReadTimeout.");
                CollectionAssert.AreEqual(Contents, destination.ToArray());
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task DownloadThatStopsAfterWritingIsNotStartedAgain()
        {
            using (var storage = new ScriptedServer(async connection =>
            {
                await connection.ReadRequestHeadAsync();
                await connection.SendHeadAsync(200, Contents.Length);
                await connection.SendAsync(Contents.Take(1000).ToArray());
                await connection.HoldAsync();
            }))
            {
                ServeDownloadOf(storage.Url);
                FilesConfiguration configuration = site.Configuration(maxNetworkRetries: 3);
                configuration.ReadTimeout = 1;
                var destination = new MemoryStream();

                await Assert.ThrowsExactlyAsync<NetworkTimeoutException>(() => FilesClient.Create(configuration).RemoteFiles.DownloadFileAsync("remote.bin", destination));

                Assert.AreEqual(1, storage.Connections.Count, "A download that has written bytes is not started again, which would write them twice.");
                CollectionAssert.AreEqual(Contents.Take(1000).ToArray(), destination.ToArray());
                Assert.IsTrue(destination.CanWrite, "The destination is left open.");
                await AssertClosed(storage);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task DownloadThatStallsBeforeItsFirstByteIsRetried()
        {
            int connections = 0;
            using (var storage = new ScriptedServer(async connection =>
            {
                bool first = Interlocked.Increment(ref connections) == 1;
                await connection.ReadRequestHeadAsync();
                await connection.SendHeadAsync(200, Contents.Length);
                if (first)
                {
                    await connection.HoldAsync();
                }
                await connection.SendAsync(Contents);
            }))
            {
                ServeDownloadOf(storage.Url);
                FilesConfiguration configuration = site.Configuration(maxNetworkRetries: 1);
                configuration.ReadTimeout = 1;
                var destination = new MemoryStream();

                await FilesClient.Create(configuration).RemoteFiles.DownloadFileAsync("remote.bin", destination);

                Assert.AreEqual(2, storage.Connections.Count);
                CollectionAssert.AreEqual(Contents, destination.ToArray());
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task ErrorResponseThatStopsArrivingTimesOut()
        {
            using (var storage = new ScriptedServer(async connection =>
            {
                await connection.ReadRequestHeadAsync();
                await connection.SendHeadAsync(503, 100);
                await connection.SendAsync("{\"error\":");
                await connection.HoldAsync();
            }))
            {
                ServeDownloadOf(storage.Url);
                FilesConfiguration configuration = site.Configuration();
                configuration.ReadTimeout = 1;
                var destination = new MemoryStream();

                // The stall ends the response, whose status is what failed.
                InvalidResponseException error = await Assert.ThrowsExactlyAsync<InvalidResponseException>(() => FilesClient.Create(configuration).RemoteFiles.DownloadFileAsync("remote.bin", destination));

                Assert.AreEqual(503, error.httpStatus);
                Assert.AreEqual(0, destination.Length, "An error response is never written to the destination.");
                await AssertClosed(storage);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task SlowDestinationIsNotANetworkStall()
        {
            using (var storage = new ScriptedServer(async connection =>
            {
                await connection.ReadRequestHeadAsync();
                await connection.SendHeadAsync(200, Contents.Length);
                await connection.SendAsync(Contents);
            }))
            {
                ServeDownloadOf(storage.Url);
                FilesConfiguration configuration = site.Configuration();
                configuration.ReadTimeout = 1;
                var destination = new SlowStream(TimeSpan.FromSeconds(1.5));

                await FilesClient.Create(configuration).RemoteFiles.DownloadFileAsync("remote.bin", destination);

                CollectionAssert.AreEqual(Contents, destination.Written.ToArray());
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task UploadPartThatCannotSendTimesOut()
        {
            // Far larger than the connection can take once the server stops reading.
            byte[] part = new byte[16 * 1024 * 1024];
            using (var storage = new ScriptedServer(async connection =>
            {
                await connection.ReadRequestHeadAsync();
                await connection.StallAsync();
            }, receiveBuffer: StalledReceiveBuffer))
            using (var server = new UploadServer { PartSize = number => part.Length, UploadUri = (file, number, negotiation) => storage.Url })
            {
                FilesConfiguration configuration = server.Site.Configuration();
                configuration.WriteTimeout = 1;
                Stopwatch elapsed = Stopwatch.StartNew();

                NetworkTimeoutException timeout = await Assert.ThrowsExactlyAsync<NetworkTimeoutException>(() =>
                    FilesClient.Create(configuration).Transfers.UploadStreamAsync(new MemoryStream(part), part.Length, "remote.bin"));

                Assert.AreEqual("The request stopped sending: no data could be sent for 1 second (WriteTimeout).", timeout.Message);
                Assert.IsTrue(elapsed.Elapsed < Promptly);
                Assert.AreEqual(0, server.Finishes.Count);
                storage.Connections.Single().StopStalling();
                await AssertClosed(storage);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task UploadTakesAsLongAsItKeepsSending()
        {
            // Read 64 KiB at a time, 40 ms apart: the part keeps moving for about two seconds, twice its WriteTimeout,
            // while no single write waits long.
            byte[] part = Enumerable.Range(0, 3 * 1024 * 1024).Select(value => (byte)(value % 253)).ToArray();
            byte[] received = null;
            using (var storage = new ScriptedServer(async connection =>
            {
                await connection.ReadRequestHeadAsync();
                received = await connection.ReadBodyAsync(chunk: 64 * 1024, pause: TimeSpan.FromMilliseconds(40));
                await connection.RespondAsync(200, "", "ETag: \"slow\"\r\n");
            }))
            using (var server = new UploadServer { PartSize = number => part.Length, UploadUri = (file, number, negotiation) => storage.Url })
            {
                FilesConfiguration configuration = server.Site.Configuration();
                configuration.WriteTimeout = 1;
                Stopwatch elapsed = Stopwatch.StartNew();

                UploadResult result = await FilesClient.Create(configuration).Transfers.UploadStreamAsync(new MemoryStream(part), part.Length, "remote.bin");

                Assert.IsTrue(elapsed.Elapsed > TimeSpan.FromSeconds(1.5), $"The part took {elapsed.Elapsed.TotalSeconds} s, longer than its WriteTimeout.");
                CollectionAssert.AreEqual(part, received);
                Assert.AreEqual("\"slow\"", result.Parts.Single().ETag);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task LegacyUploadPartThatCannotSendTimesOut()
        {
            byte[] part = new byte[16 * 1024 * 1024];
            using (var storage = new ScriptedServer(async connection =>
            {
                await connection.ReadRequestHeadAsync();
                await connection.StallAsync();
            }, receiveBuffer: StalledReceiveBuffer))
            {
                site.On("POST", "/file_actions/begin_upload/remote.bin", request => LoopbackSite.Json($"[{{\"upload_uri\":\"{storage.Url}\",\"http_method\":\"PUT\",\"partsize\":{part.Length},\"ref\":\"upload-ref\"}}]"));
                FilesConfiguration configuration = site.Configuration();
                configuration.WriteTimeout = 1;
                Stopwatch elapsed = Stopwatch.StartNew();

                NetworkTimeoutException timeout = await Assert.ThrowsExactlyAsync<NetworkTimeoutException>(() =>
                    FilesClient.Create(configuration).RemoteFiles.UploadFileAsync("remote.bin", new MemoryStream(part), part.Length, DateTime.UtcNow));

                // Not the ReadTimeout of a part that was sent whole and then got no answer.
                Assert.AreEqual("The request stopped sending: no data could be sent for 1 second (WriteTimeout).", timeout.Message);
                Assert.IsTrue(elapsed.Elapsed < Promptly);
                storage.Connections.Single().StopStalling();
                await AssertClosed(storage);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task SlowSourceIsNotANetworkStall()
        {
            using (var server = new UploadServer())
            {
                FilesConfiguration configuration = server.Site.Configuration();
                configuration.WriteTimeout = 1;
                var source = new SlowStream(TimeSpan.FromSeconds(1.2), Contents.Take(20).ToArray());

                UploadResult result = await FilesClient.Create(configuration).Transfers.UploadStreamAsync(source, 20, "remote.bin");

                CollectionAssert.AreEqual(Contents.Take(20).ToArray(), server.AcceptedBytes("remote.bin"));
                Assert.AreEqual(3, result.Parts.Count);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task ConnectionThatCannotBeMadeTimesOut()
        {
            using (var unanswered = new UnansweredPort())
            {
                var configuration = new FilesConfiguration { BaseUrl = $"http://127.0.0.1:{unanswered.Port}", ApiKey = "key", ConnectTimeout = 1, ReadTimeout = 30, MaxNetworkRetries = 0 };
                Stopwatch elapsed = Stopwatch.StartNew();

                NetworkTimeoutException timeout = await Assert.ThrowsExactlyAsync<NetworkTimeoutException>(() => FilesClient.Create(configuration).Users.FindAsync(1));

                StringAssert.Contains(timeout.Message, "ConnectTimeout");
                Assert.IsTrue(elapsed.Elapsed < TimeSpan.FromSeconds(10), $"Took {elapsed.Elapsed.TotalSeconds} s with a one-second ConnectTimeout and a 30-second ReadTimeout.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task SlowAnswerIsNotAConnectTimeout()
        {
            using (var api = new ScriptedServer(async connection =>
            {
                await connection.ReadRequestHeadAsync();
                await Task.Delay(2000);
                await connection.RespondAsync(200, "{\"id\":1,\"username\":\"slow\"}");
            }))
            {
                FilesConfiguration configuration = Configuration(api);
                configuration.ConnectTimeout = 1;
                configuration.ReadTimeout = 10;

                FilesCom.Models.User user = await FilesClient.Create(configuration).Users.FindAsync(1);

                Assert.AreEqual("slow", user.Username);
            }
        }

        [TestMethod]
        public void TimeoutsAndDelaysOutOfRangeAreRefusedBeforeAnythingIsSent()
        {
            foreach (Action<FilesConfiguration> invalid in new Action<FilesConfiguration>[]
            {
                c => c.ReadTimeout = 0,
                c => c.WriteTimeout = -1,
                c => c.ConnectTimeout = int.MaxValue,
                c => c.MaxNetworkRetries = -1,
                c => c.InitialNetworkRequestDelay = double.NaN,
                c => c.MaxNetworkRetryDelay = double.PositiveInfinity,
                c => c.InitialNetworkRequestDelay = -0.5,
            })
            {
                FilesConfiguration configuration = site.Configuration();
                invalid(configuration);

                Assert.ThrowsExactly<InvalidParameterException>(() => FilesClient.Create(configuration));
            }

            // A default client reads its configuration as each operation starts, and checks it then.
            FilesConfiguration live = site.Configuration();
            FilesClient client = new FilesClient(live);
            live.ReadTimeout = 0;
            Assert.AreEqual(nameof(FilesConfiguration.ReadTimeout), Assert.ThrowsExactly<InvalidParameterException>(() => client.Users.FindAsync(1)).ParamName);
            site.AssertReceived();
        }

        private static FilesConfiguration Configuration(ScriptedServer api, int maxNetworkRetries = 0)
        {
            return new FilesConfiguration { BaseUrl = $"http://127.0.0.1:{api.Port}", ApiKey = "key", MaxNetworkRetries = maxNetworkRetries, InitialNetworkRequestDelay = 0 };
        }

        private void ServeDownloadOf(string url)
        {
            site.On("GET", "/files/remote.bin", request => LoopbackSite.Json($"{{\"path\":\"remote.bin\",\"download_uri\":\"{url}\"}}"));
        }

        // Every connection the client opened has been closed by it.
        private static async Task AssertClosed(ScriptedServer server)
        {
            Task closed = Task.WhenAll(server.Connections.Select(connection => connection.Closed));
            Assert.AreSame(closed, await Task.WhenAny(closed, Task.Delay(Promptly)), "The client left a connection open after its request ended.");
        }

        // A stream whose every read and write first waits delay, like a slow disk. Reads return from contents.
        private sealed class SlowStream : Stream
        {
            private readonly TimeSpan delay;
            private readonly MemoryStream contents;

            public SlowStream(TimeSpan delay, byte[] contents = null)
            {
                this.delay = delay;
                this.contents = new MemoryStream(contents ?? new byte[0]);
            }

            public MemoryStream Written { get; } = new MemoryStream();

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
                await Task.Delay(delay, cancellationToken);
                return contents.Read(buffer, offset, count);
            }

            public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                await Task.Delay(delay, cancellationToken);
                Written.Write(buffer, offset, count);
            }

            public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
            public override void Flush()
            {
            }
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
        }

        // A loopback port that accepts no more connections: its backlog is full and nothing accepts, so a new
        // connection gets no answer, as when a server is unreachable.
        private sealed class UnansweredPort : IDisposable
        {
            private readonly Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            private readonly List<Socket> waiting = new List<Socket>();

            public UnansweredPort()
            {
                listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                listener.Listen(1);
                Port = ((IPEndPoint)listener.LocalEndPoint).Port;
                for (int i = 0; i < 16; i++)
                {
                    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    waiting.Add(socket);
                    Task connecting = socket.ConnectAsync(IPAddress.Loopback, Port);
                    if (!connecting.Wait(TimeSpan.FromMilliseconds(300)))
                    {
                        return;
                    }
                }
                Assert.Inconclusive("The loopback listener kept accepting connections, so a connection that gets no answer cannot be made.");
            }

            public int Port { get; }

            public void Dispose()
            {
                foreach (Socket socket in waiting)
                {
                    socket.Dispose();
                }
                listener.Dispose();
            }
        }
    }
}