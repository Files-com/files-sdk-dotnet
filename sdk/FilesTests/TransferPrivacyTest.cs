using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using FilesCom;
using log4net.Appender;
using log4net.Core;

namespace FilesTests
{
    [TestClass]
    public class TransferPrivacyTest
    {
        private const string SignedUrl = "https://transfer.example.test/private-part?X-Amz-Credential=credential&X-Amz-Signature=signature";

        private class TransferHandler : HttpMessageHandler
        {
            public bool Fail;
            public bool NotFound;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
            {
                if (Fail)
                {
                    throw new HttpRequestException($"Connection reset for {request.RequestUri}");
                }
                if (NotFound)
                {
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent($"{{\"type\":\"not-found\",\"error\":\"{request.RequestUri}\"}}")
                    });
                }
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("part") });
            }
        }

        [TestMethod]
        public async Task TransferLogsAndErrorsHideSignedUrlsBelowDebug()
        {
            var appender = new MemoryAppender { Fix = FixFlags.Message | FixFlags.Exception };
            var repository = (log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository(typeof(FilesClient).Assembly);
            var originalLevel = repository.Root.Level;
            repository.Root.Level = Level.Debug;
            log4net.Config.BasicConfigurator.Configure(repository, appender);
            try
            {
                new FilesClient(new FilesConfiguration());
                var handler = new TransferHandler();
                var service = new FilesApiService(new CapturingHttpClientFactory(handler, "https://app.files.com"));
                await service.ChunkUpload(HttpMethod.Put, SignedUrl, new MemoryStream(new byte[] { 1 }), 1);
                handler.Fail = true;
                var uploadError = await Assert.ThrowsExceptionAsync<ApiConnectionException>(() =>
                    service.ChunkUpload(HttpMethod.Put, SignedUrl, new MemoryStream(new byte[] { 1 }), 1));
                var downloadError = await Assert.ThrowsExceptionAsync<ApiConnectionException>(() =>
                    service.StreamDownload(SignedUrl, new MemoryStream()));
                handler.Fail = false;
                handler.NotFound = true;
                var apiError = await Assert.ThrowsExceptionAsync<NotFoundException>(() =>
                    service.ChunkUpload(HttpMethod.Put, SignedUrl, new MemoryStream(new byte[] { 1 }), 1));
                Assert.AreEqual(404, apiError.httpStatus);

                var listener = new TcpListener(IPAddress.Loopback, 0);
                listener.Start();
                int port = ((IPEndPoint)listener.LocalEndpoint).Port;
                listener.Stop();
                string unavailableUrl = $"http://127.0.0.1:{port}/private-part?signature=signature&credential=credential";
                new FilesClient(new FilesConfiguration { MaxNetworkRetries = 0, ConnectTimeout = 1 });
                await Assert.ThrowsExceptionAsync<ApiConnectionException>(() =>
                    FilesClient.ChunkUpload(HttpMethod.Put, unavailableUrl, new MemoryStream(new byte[] { 1 }), 1));
                await Assert.ThrowsExceptionAsync<ApiConnectionException>(() =>
                    FilesClient.StreamDownload(unavailableUrl, new MemoryStream()));

                var normalLogs = string.Join("\n", appender.GetEvents().Where(e => e.Level > Level.Debug).Select(e => e.RenderedMessage + e.GetExceptionString()));
                foreach (string secret in new[] { "transfer.example.test", "private-part", "credential", "signature" })
                {
                    Assert.IsFalse(normalLogs.Contains(secret));
                    Assert.IsFalse(uploadError.ToString().Contains(secret));
                    Assert.IsFalse(downloadError.ToString().Contains(secret));
                    Assert.IsFalse(apiError.ToString().Contains(secret));
                }
                var debugLogs = string.Join("\n", appender.GetEvents().Where(e => e.Level == Level.Debug).Select(e => e.RenderedMessage + e.GetExceptionString()));
                StringAssert.Contains(debugLogs, SignedUrl);
                StringAssert.Contains(normalLogs, "Sending PUT upload request");
                StringAssert.Contains(normalLogs, "Failed to upload chunk");
                StringAssert.Contains(normalLogs, "Failed to stream download");
            }
            finally
            {
                repository.Root.RemoveAppender(appender);
                repository.Root.Level = originalLevel;
            }
        }
    }
}
