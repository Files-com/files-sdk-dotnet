using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FilesCom;
using FilesCom.Models;
using WireMock;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Request = WireMock.RequestBuilders.Request;

namespace FilesTests
{
    [TestClass]
    public class StreamUploadTest
    {
        private static readonly DateTime MTime = new DateTime(2026, 9, 24, 0, 0, 0, DateTimeKind.Utc);
        private WireMockServer server;

        private string PartUrl => server.Urls[0] + "/part";

        [TestInitialize]
        public void StartServer()
        {
            server = WireMockServer.Start();
            server.Given(Request.Create().WithPath("/part").UsingPut())
                .RespondWith(Response.Create().WithStatusCode(200));
            UseClient(maxNetworkRetries: 0);
        }

        [TestCleanup]
        public void StopServer()
        {
            server.Stop();
        }

        [TestMethod]
        public async Task ChunkUploadSendsExactlyTheRequestedBytesFromShortReads()
        {
            var stream = new ShortReadStream(Bytes(1, 12));

            await FilesClient.ChunkUpload(HttpMethod.Put, PartUrl, stream, 8);

            AssertRequests("PUT " + Hex(Bytes(1, 8)));
            Assert.AreEqual(Hex(Bytes(9, 4)), Hex(ReadToEnd(stream)), "The borrowed stream is left open at the end of the part.");
        }

        [TestMethod]
        public async Task ChunkUploadRetryResendsTheBufferedPartWithoutRereadingTheStream()
        {
            UseClient(maxNetworkRetries: 1);
            server.Given(Request.Create().WithPath("/flaky-part").UsingPut())
                .InScenario("retry").WillSetStateTo("failed once")
                .RespondWith(Response.Create().WithStatusCode(503));
            server.Given(Request.Create().WithPath("/flaky-part").UsingPut())
                .InScenario("retry").WhenStateIs("failed once")
                .RespondWith(Response.Create().WithStatusCode(200));
            var stream = new ShortReadStream(Bytes(1, 12));

            await FilesClient.ChunkUpload(HttpMethod.Put, server.Urls[0] + "/flaky-part", stream, 8);

            AssertRequests("PUT " + Hex(Bytes(1, 8)), "PUT " + Hex(Bytes(1, 8)));
            Assert.AreEqual(Hex(Bytes(9, 4)), Hex(ReadToEnd(stream)));
        }

        [TestMethod]
        public async Task ChunkUploadFailsWithoutSendingWhenTheStreamEndsEarly()
        {
            var stream = new ShortReadStream(Bytes(1, 5));

            await Assert.ThrowsExceptionAsync<EndOfStreamException>(() =>
                FilesClient.ChunkUpload(HttpMethod.Put, PartUrl, stream, 8));

            AssertRequests();
            Assert.IsTrue(stream.CanRead, "The borrowed stream is not disposed.");
        }

        [TestMethod]
        public async Task ChunkUploadKeepsTheStreamReadErrorAndSendsNothing()
        {
            var readError = new IOException("The source disconnected.");
            var stream = new ShortReadStream(Bytes(1, 3), readError);

            Exception error = await Assert.ThrowsExceptionAsync<IOException>(() =>
                FilesClient.ChunkUpload(HttpMethod.Put, PartUrl, stream, 8));

            Assert.AreSame(readError, error);
            AssertRequests();
        }

        [TestMethod]
        public async Task ChunkUploadRejectsLengthsThatCannotBeOnePart()
        {
            foreach (long length in new[] { -1L, (long)int.MaxValue + 1 })
            {
                var stream = new ShortReadStream(Bytes(1, 4));

                await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() =>
                    FilesClient.ChunkUpload(HttpMethod.Put, PartUrl, stream, length));

                Assert.AreEqual(Hex(Bytes(1, 4)), Hex(ReadToEnd(stream)), "Nothing is read from the stream.");
            }
            AssertRequests();
        }

        [TestMethod]
        public async Task UploadFileSendsEachPartExactlyThenFinalizes()
        {
            ServeUpload(partsize: 4);
            var stream = new ShortReadStream(Bytes(1, 10));

            Assert.IsTrue(await RemoteFile.UploadFile("remote.bin", stream, 10, MTime));

            AssertRequests(
                "begin", "PUT " + Hex(Bytes(1, 4)),
                "begin", "PUT " + Hex(Bytes(5, 4)),
                "begin", "PUT " + Hex(Bytes(9, 2)),
                "end");
            using (JsonDocument end = JsonDocument.Parse(server.LogEntries.Last().RequestMessage.Body))
            {
                Assert.AreEqual(10, end.RootElement.GetProperty("size").GetInt64());
            }
            Assert.IsFalse(stream.CanRead, "UploadFile disposes the stream it is given.");
        }

        [TestMethod]
        public async Task UploadFileDoesNotFinalizeWhenTheStreamEndsEarly()
        {
            ServeUpload(partsize: 4);
            var stream = new ShortReadStream(Bytes(1, 6));

            await Assert.ThrowsExceptionAsync<EndOfStreamException>(() =>
                RemoteFile.UploadFile("remote.bin", stream, 10, MTime));

            AssertRequests("begin", "PUT " + Hex(Bytes(1, 4)), "begin");
        }

        [TestMethod]
        public async Task UploadFileOfAnEmptyStreamSendsOneEmptyPartThenFinalizes()
        {
            ServeUpload(partsize: 4);

            Assert.IsTrue(await RemoteFile.UploadFile("remote.bin", new ShortReadStream(new byte[0]), 0, MTime));

            AssertRequests("begin", "PUT " + Hex(new byte[0]), "end");
        }

        private void UseClient(int maxNetworkRetries)
        {
            new FilesClient(new FilesConfiguration
            {
                ApiKey = "test-key",
                BaseUrl = server.Urls[0],
                MaxNetworkRetries = maxNetworkRetries,
                InitialNetworkRequestDelay = 0,
            });
        }

        private void ServeUpload(int partsize)
        {
            server.Given(Request.Create().WithPath("/api/rest/v1/file_actions/begin_upload/remote.bin").UsingPost())
                .RespondWith(Response.Create().WithStatusCode(200).WithBodyAsJson(new[]
                {
                    new { upload_uri = PartUrl, http_method = "PUT", partsize, @ref = "upload-ref" },
                }));
            server.Given(Request.Create().WithPath("/api/rest/v1/files/remote.bin").UsingPost())
                .RespondWith(Response.Create().WithStatusCode(200).WithBodyAsJson(new { path = "remote.bin" }));
        }

        private void AssertRequests(params string[] expected)
        {
            string[] received = server.LogEntries.Select(entry => Describe(entry.RequestMessage)).ToArray();

            Assert.AreEqual(string.Join(", ", expected), string.Join(", ", received));
        }

        private static string Describe(IRequestMessage request)
        {
            if (request.Method == "PUT")
            {
                return "PUT " + Hex(request.BodyAsBytes ?? new byte[0]);
            }
            if (request.Path.StartsWith("/api/rest/v1/file_actions/begin_upload/"))
            {
                return "begin";
            }
            if (request.Path.StartsWith("/api/rest/v1/files/"))
            {
                return "end";
            }
            return request.Method + " " + request.Path;
        }

        private static byte[] Bytes(int first, int count)
        {
            return Enumerable.Range(first, count).Select(value => (byte)value).ToArray();
        }

        private static string Hex(byte[] bytes)
        {
            return "[" + BitConverter.ToString(bytes) + "]";
        }

        private static byte[] ReadToEnd(Stream stream)
        {
            var rest = new MemoryStream();
            stream.CopyTo(rest);
            return rest.ToArray();
        }

        // A forward-only source whose reads return at most three bytes, as Stream allows, with ReadAsync completing
        // asynchronously. After its data it reports the end of the stream, or throws readError if one is given.
        private sealed class ShortReadStream : Stream
        {
            private const int MaxReadLength = 3;
            private readonly byte[] data;
            private readonly Exception readError;
            private int position;
            private bool disposed;

            public ShortReadStream(byte[] data, Exception readError = null)
            {
                this.data = data;
                this.readError = readError;
            }

            public override bool CanRead => !disposed;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(nameof(ShortReadStream));
                }
                if (position == data.Length && readError != null)
                {
                    throw readError;
                }
                int length = Math.Min(Math.Min(count, MaxReadLength), data.Length - position);
                Array.Copy(data, position, buffer, offset, length);
                position += length;
                return length;
            }

            public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                await Task.Yield();
                return Read(buffer, offset, count);
            }

            public override void Flush()
            {
            }

            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

            public override void SetLength(long value) => throw new NotSupportedException();

            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

            protected override void Dispose(bool disposing)
            {
                disposed = true;
                base.Dispose(disposing);
            }
        }
    }
}
