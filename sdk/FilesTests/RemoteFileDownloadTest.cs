using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FilesCom;
using FilesCom.Models;

namespace FilesTests
{
    [TestClass]
    public class RemoteFileDownloadTest
    {
        private static readonly byte[] Payload = Enumerable.Range(0, 32768).Select(i => (byte)(i % 251)).ToArray();
        private string directory;

        [TestInitialize]
        public void Setup()
        {
            directory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(directory);
            new FilesClient(new FilesConfiguration { MaxNetworkRetries = 0 });
        }

        [TestCleanup]
        public void Cleanup()
        {
            Directory.Delete(directory, true);
        }

        [TestMethod]
        public async Task DownloadToPathWritesBytesAndReleasesFile()
        {
            string destination = Path.Combine(directory, "download.bin");

            using (var server = new OneResponseServer(Payload, Payload.Length))
            {
                await DownloadableFile(server.Url).DownloadFile(destination);
            }

            AssertFileReleased(destination);
            CollectionAssert.AreEqual(Payload, File.ReadAllBytes(destination));
        }

        [TestMethod]
        public async Task DownloadToPathReleasesFileWhenResponseIsTruncated()
        {
            string destination = Path.Combine(directory, "download.bin");
            Exception error;

            using (var server = new OneResponseServer(Payload, 4096))
            {
                error = await CaptureException(() => DownloadableFile(server.Url).DownloadFile(destination));
            }

            Assert.IsInstanceOfType(error, typeof(IOException));
            AssertFileReleased(destination);
        }

        [TestMethod]
        public async Task DownloadToPathReleasesFileWhenDownloadUriIsInvalid()
        {
            string destination = Path.Combine(directory, "download.bin");

            Exception error = await CaptureException(() => DownloadableFile("not a uri").DownloadFile(destination));

            Assert.IsInstanceOfType(error, typeof(UriFormatException));
            AssertFileReleased(destination);
        }

        [TestMethod]
        public async Task DownloadToPathReportsFailureToWriteFileWhenClosing()
        {
            RequireDevFull();
            byte[] body = Payload.Take(11).ToArray();
            Exception error;

            // The body fits in the FileStream buffer, so it is first written, and fails, when the file closes.
            using (var server = new OneResponseServer(body, body.Length))
            {
                error = await CaptureException(() => DownloadableFile(server.Url).DownloadFile("/dev/full"));
            }

            Assert.IsInstanceOfType(error, typeof(IOException));
            AssertFileReleased("/dev/full");
        }

        [TestMethod]
        public async Task DownloadToPathKeepsTransferErrorWhenClosingFileAlsoFails()
        {
            RequireDevFull();
            Exception transferError;
            Exception error;

            using (var server = new OneResponseServer(Payload, 11))
            {
                transferError = await CaptureException(() => DownloadableFile(server.Url).DownloadFile(new MemoryStream()));
            }
            using (var server = new OneResponseServer(Payload, 11))
            {
                error = await CaptureException(() => DownloadableFile(server.Url).DownloadFile("/dev/full"));
            }

            Assert.AreEqual(transferError.GetType(), error.GetType(), error.Message);
            Assert.AreEqual(transferError.Message, error.Message);
            AssertFileReleased("/dev/full");
        }

        [TestMethod]
        public async Task DownloadToStreamLeavesCallerStreamOpen()
        {
            using (var destination = new MemoryStream())
            {
                using (var server = new OneResponseServer(Payload, Payload.Length))
                {
                    await DownloadableFile(server.Url).DownloadFile(destination);
                }
                CollectionAssert.AreEqual(Payload, destination.ToArray());

                using (var server = new OneResponseServer(Payload, 4096))
                {
                    await CaptureException(() => DownloadableFile(server.Url).DownloadFile(destination));
                }
                Assert.IsTrue(destination.CanWrite);
            }
        }

        private static RemoteFile DownloadableFile(string downloadUri)
        {
            return new RemoteFile(new Dictionary<string, object> { { "path", "remote.bin" }, { "download_uri", downloadUri } }, null);
        }

        private static async Task<Exception> CaptureException(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception e)
            {
                return e;
            }
            Assert.Fail("Expected the download to fail");
            return null;
        }

        private static void AssertFileReleased(string path)
        {
            // An exclusive open fails while any other handle to the file is still open.
            using (new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
            }
        }

        private static void RequireDevFull()
        {
            if (!File.Exists("/dev/full"))
            {
                Assert.Inconclusive("Requires /dev/full, where every write fails with no space left on device.");
            }
        }

        // Serves one HTTP response on loopback, sending only the first bytesToSend bytes of the body before closing the connection.
        private sealed class OneResponseServer : IDisposable
        {
            private readonly TcpListener listener = new TcpListener(IPAddress.Loopback, 0);

            public OneResponseServer(byte[] body, int bytesToSend)
            {
                listener.Start();
                Url = $"http://127.0.0.1:{((IPEndPoint)listener.LocalEndpoint).Port}/download";
                _ = Serve(body, bytesToSend);
            }

            public string Url { get; }

            private async Task Serve(byte[] body, int bytesToSend)
            {
                using (TcpClient client = await listener.AcceptTcpClientAsync())
                using (NetworkStream stream = client.GetStream())
                {
                    var request = new StreamReader(stream, Encoding.ASCII);
                    while (!string.IsNullOrEmpty(await request.ReadLineAsync()))
                    {
                    }
                    byte[] headers = Encoding.ASCII.GetBytes($"HTTP/1.1 200 OK\r\nContent-Length: {body.Length}\r\nConnection: close\r\n\r\n");
                    await stream.WriteAsync(headers, 0, headers.Length);
                    await stream.WriteAsync(body, 0, bytesToSend);
                }
            }

            public void Dispose()
            {
                listener.Stop();
            }
        }
    }
}
