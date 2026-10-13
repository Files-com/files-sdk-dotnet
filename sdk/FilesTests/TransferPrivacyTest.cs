using FilesCom;
using log4net.Appender;
using log4net.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FilesTests
{
    [TestClass]
    public class TransferPrivacyTest
    {
        private const string SignedUrl = "https://transfer.example.test/private-part?X-Amz-Credential=credential&X-Amz-Signature=signature";
        // A signed URL an ordinary API error quotes. Its host and path stay in messages; its query does not.
        private const string QuotedSignedUrl = "https://storage.example.test/object?X-Amz-Credential=credential&X-Amz-Signature=signature";

        private class TransferHandler : HttpMessageHandler
        {
            public bool Fail;
            public bool NotFound;
            public bool Malformed;
            public bool QuotesSignedUrl;
            public bool FailsQuotingSignedUrl;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
            {
                if (Fail)
                {
                    throw new HttpRequestException($"Connection reset for {request.RequestUri}");
                }
                if (FailsQuotingSignedUrl)
                {
                    throw new HttpRequestException($"Connection reset for {QuotedSignedUrl}");
                }
                if (QuotesSignedUrl)
                {
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent($"{{\"type\":\"not-found\",\"error\":\"Nothing at {QuotedSignedUrl}\"}}")
                    });
                }
                if (Malformed)
                {
                    // Not a Files.com error: model_errors must hold arrays, and the key that fails names the URL.
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent($"{{\"type\":\"bad-request\",\"model_errors\":{{\"{request.RequestUri}\":\"invalid\"}}}}")
                    });
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
        public async Task LogsAndErrorsHideSignedUrlsBelowDebug()
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
                var uploadError = await Assert.ThrowsExactlyAsync<ApiConnectionException>(() =>
                    service.ChunkUpload(HttpMethod.Put, SignedUrl, new MemoryStream(new byte[] { 1 }), 1));
                var downloadError = await Assert.ThrowsExactlyAsync<ApiConnectionException>(() =>
                    service.StreamDownload(SignedUrl, new MemoryStream()));
                handler.Fail = false;
                handler.NotFound = true;
                var apiError = await Assert.ThrowsExactlyAsync<NotFoundException>(() =>
                    service.ChunkUpload(HttpMethod.Put, SignedUrl, new MemoryStream(new byte[] { 1 }), 1));
                Assert.AreEqual(404, apiError.httpStatus);
                handler.NotFound = false;
                handler.Malformed = true;
                var unreadableUpload = await Assert.ThrowsExactlyAsync<InvalidResponseException>(() =>
                    service.ChunkUpload(HttpMethod.Put, SignedUrl, new MemoryStream(new byte[] { 1 }), 1));
                var unreadableDownload = await Assert.ThrowsExactlyAsync<InvalidResponseException>(() =>
                    service.StreamDownload(SignedUrl, new MemoryStream()));
                Assert.AreEqual(400, unreadableDownload.httpStatus);
                handler.Malformed = false;
                handler.QuotesSignedUrl = true;
                var quotingError = await Assert.ThrowsExactlyAsync<NotFoundException>(() =>
                    service.SendRequest("/files/object", HttpMethod.Get, new Dictionary<string, object>(), new Dictionary<string, object> { { "api_key", "key" } }));
                Assert.AreEqual("Nothing at https://storage.example.test/object?[redacted]", quotingError.Message);
                Assert.AreEqual("Nothing at " + QuotedSignedUrl, quotingError.error, "The structured error keeps the text as sent.");
                handler.QuotesSignedUrl = false;
                handler.FailsQuotingSignedUrl = true;
                var quotingFailure = await Assert.ThrowsExactlyAsync<ApiConnectionException>(() =>
                    service.SendRequest("/files/object", HttpMethod.Get, new Dictionary<string, object>(), new Dictionary<string, object> { { "api_key", "key" } }));
                Assert.AreEqual("Connection reset for https://storage.example.test/object?[redacted]", quotingFailure.Message);

                var listener = new TcpListener(IPAddress.Loopback, 0);
                listener.Start();
                int port = ((IPEndPoint)listener.LocalEndpoint).Port;
                listener.Stop();
                string unavailableUrl = $"http://127.0.0.1:{port}/private-part?signature=signature&credential=credential";
                // A refused connection: Windows tries a refused loopback port again for about two seconds before it reports
                // the refusal, so the connect timeout leaves it room.
                new FilesClient(new FilesConfiguration { MaxNetworkRetries = 0, ConnectTimeout = 10 });
                await Assert.ThrowsExactlyAsync<ApiConnectionException>(() =>
                    FilesClient.ChunkUpload(HttpMethod.Put, unavailableUrl, new MemoryStream(new byte[] { 1 }), 1));
                await Assert.ThrowsExactlyAsync<ApiConnectionException>(() =>
                    FilesClient.StreamDownload(unavailableUrl, new MemoryStream()));

                var normalLogs = string.Join("\n", appender.GetEvents().Where(e => e.Level > Level.Debug).Select(e => e.RenderedMessage + e.GetExceptionString()));
                foreach (string secret in new[] { "transfer.example.test", "private-part", "credential", "signature" })
                {
                    Assert.IsFalse(normalLogs.Contains(secret));
                    foreach (Exception error in new Exception[] { uploadError, downloadError, apiError, unreadableUpload, unreadableDownload, quotingError, quotingFailure })
                    {
                        // Structured error reporters record every exception in the chain, not only ToString.
                        for (Exception e = error; e != null; e = e.InnerException)
                        {
                            Assert.IsFalse(e.Message.Contains(secret), e.GetType().Name + ": " + e.Message);
                        }
                        Assert.IsFalse(error.ToString().Contains(secret));
                    }
                }
                var debugLogs = string.Join("\n", appender.GetEvents().Where(e => e.Level == Level.Debug).Select(e => e.RenderedMessage + e.GetExceptionString()));
                StringAssert.Contains(debugLogs, SignedUrl);
                StringAssert.Contains(debugLogs, QuotedSignedUrl);
                // Below DEBUG, an error is logged by what kind it is, without its text, even with the URLs in it redacted.
                StringAssert.Contains(normalLogs, "API request failed (NotFoundException, HTTP 404)");
                Assert.IsFalse(normalLogs.Contains("Nothing at"), normalLogs);
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