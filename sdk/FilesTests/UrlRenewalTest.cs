using FilesCom;
using FilesCom.Models;
using FilesCom.Transfers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WireMock;

namespace FilesTests
{
    // A signed storage URL that expires, or is about to, before a request is sent to it is replaced by asking Files.com
    // again, through the operation that provided it, with the same client and options: the same part of the same
    // upload, with the same bytes, or the same file to download, before anything has been written. Where no such
    // operation exists, as for a URL given directly, an expired URL is not sent again at all. A renewal that returns
    // an expired URL ends the transfer rather than asking forever.
    [TestClass]
    public class UrlRenewalTest
    {
        private static readonly DateTime MTime = new DateTime(2026, 9, 24, 0, 0, 0, DateTimeKind.Utc);
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
        public async Task TransferPartWhoseSignatureExpiresBeforeItsRetryIsRenewed()
        {
            using (var server = new UploadServer())
            {
                // Part 2's first URL is signed to expire soon after it is sent; its retry must wait three seconds.
                server.UploadUri = (file, number, negotiation) => server.PartUrl(file, number, negotiation) + (number == 2 && negotiation == 1 ? SignedFor(32) : SignedFor(3600));
                server.RespondToPart = request =>
                {
                    if (request.Number == 2 && request.Attempt == 1)
                    {
                        ResponseMessage unavailable = LoopbackSite.Status(503);
                        unavailable.AddHeader("Retry-After", "3");
                        return Task.FromResult(unavailable);
                    }
                    return Task.FromResult(UploadServer.Accept(request));
                };
                byte[] contents = Enumerable.Range(0, 20).Select(value => (byte)value).ToArray();

                UploadResult result = await FilesClient.Create(server.Site.Configuration(maxNetworkRetries: 1)).Transfers.UploadStreamAsync(new MemoryStream(contents), contents.Length, "remote.bin");

                PartRequest[] part2 = server.Parts.Where(part => part.Number == 2).ToArray();
                Assert.AreEqual(2, part2.Length);
                StringAssert.Contains(part2[0].Request.Path, "negotiation=1");
                StringAssert.Contains(part2[1].Request.Path, "negotiation=2", "The retry went to the renewed URL.");
                CollectionAssert.AreEqual(part2[0].Bytes, part2[1].Bytes);
                JsonElement renewal = server.Negotiations.Single(body => body.TryGetProperty("part", out JsonElement part) && part.GetInt64() == 2 && !body.TryGetProperty("parts", out _));
                Assert.AreEqual("ref-remote.bin", renewal.GetProperty("ref").GetString(), "The renewal belongs to the same upload.");
                Assert.AreEqual(1, server.Finishes.Count);
                CollectionAssert.AreEqual(contents, server.AcceptedBytes("remote.bin"));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task LegacyPartWhoseSignatureExpiresBeforeItsRetryIsRenewedWithTheSameBytes()
        {
            int negotiations = 0;
            int puts = 0;
            site.On("POST", "/file_actions/begin_upload/remote.bin", request =>
            {
                string signature = ++negotiations == 1 ? SignedFor(32) : SignedFor(3600);
                return LoopbackSite.Json($"[{{\"upload_uri\":\"{site.Url}/part?negotiation={negotiations}{signature}\",\"http_method\":\"PUT\",\"partsize\":4,\"ref\":\"upload-ref\"}}]");
            });
            site.On("PUT", "//part", request =>
            {
                if (++puts > 1)
                {
                    return LoopbackSite.Status(200);
                }
                ResponseMessage unavailable = LoopbackSite.Status(503);
                unavailable.AddHeader("Retry-After", "3");
                return unavailable;
            });
            site.On("POST", "/files/remote.bin", request => LoopbackSite.Json("{\"path\":\"remote.bin\"}"));
            byte[] contents = { 1, 2, 3, 4 };

            // The stream can be read only once, so the retry can only resend the part read before.
            await FilesClient.Create(site.Configuration(maxNetworkRetries: 1)).RemoteFiles.UploadFileAsync("remote.bin", new OnceReadableStream(contents), 4, MTime);

            ReceivedRequest[] begins = site.Requests.Where(request => request.Path.StartsWith("/file_actions")).ToArray();
            ReceivedRequest[] parts = site.Requests.Where(request => request.Path.StartsWith("/part")).ToArray();
            Assert.AreEqual(2, begins.Length);
            StringAssert.Contains(begins[1].Body, "\"ref\":\"upload-ref\"");
            StringAssert.Contains(begins[1].Body, "\"part\":1");
            StringAssert.Contains(parts[1].Path, "negotiation=2");
            CollectionAssert.AreEqual(contents, parts[0].BodyBytes);
            CollectionAssert.AreEqual(contents, parts[1].BodyBytes);
            Assert.AreEqual(1, site.Requests.Count(request => request.Path == "/files/remote.bin"));
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task DownloadWithAnExpiredUrlGetsANewOneFirst()
        {
            string expired = $"{site.Url}/expired?{Signed(DateTime.UtcNow.AddHours(-2), 3600)}";
            site.On("GET", "/folders*", request => LoopbackSite.Json($"[{{\"path\":\"remote.bin\",\"type\":\"file\",\"download_uri\":\"{expired}\"}}]"));
            site.On("GET", "/files/remote.bin", request => LoopbackSite.Json($"{{\"path\":\"remote.bin\",\"download_uri\":\"{site.Url}/fresh?{Signed(DateTime.UtcNow, 3600)}\"}}"));
            site.On("GET", "//fresh", request => LoopbackSite.Json("\"contents\""));
            FilesClient client = FilesClient.Create(site.Configuration());
            RemoteFile file = (await client.Folders.ListFor("").AllAsync()).Single();
            var destination = new MemoryStream();

            await file.DownloadFileAsync(destination);

            Assert.AreEqual("\"contents\"", System.Text.Encoding.UTF8.GetString(destination.ToArray()));
            Assert.AreEqual(0, site.Requests.Count(request => request.Path.StartsWith("/expired")), "The expired URL was never sent to.");
            Assert.AreEqual(1, site.Requests.Count(request => request.Path.StartsWith("/files/remote.bin")));
            StringAssert.Contains(file.DownloadUri, "/fresh?", "The file has the new URL.");
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task DownloadUrlThatExpiresDuringTheRetryWaitIsRenewed()
        {
            int details = 0;
            int downloads = 0;
            site.On("GET", "/files/remote.bin", request => LoopbackSite.Json($"{{\"path\":\"remote.bin\",\"download_uri\":\"{site.Url}/download?n={++details}{(details == 1 ? SignedFor(32) : SignedFor(3600))}\"}}"));
            site.On("GET", "//download", request =>
            {
                if (++downloads > 1)
                {
                    return LoopbackSite.Json("\"contents\"");
                }
                ResponseMessage throttled = LoopbackSite.Status(429);
                throttled.AddHeader("Retry-After", "3");
                return throttled;
            });
            var destination = new MemoryStream();

            await FilesClient.Create(site.Configuration(maxNetworkRetries: 1)).RemoteFiles.DownloadFileAsync("remote.bin", destination);

            Assert.AreEqual("\"contents\"", System.Text.Encoding.UTF8.GetString(destination.ToArray()));
            ReceivedRequest[] sent = site.Requests.Where(request => request.Path.StartsWith("/download")).ToArray();
            Assert.AreEqual(2, sent.Length);
            StringAssert.Contains(sent[1].Path, "n=2", "The retry used the renewed URL.");
            Assert.AreEqual(2, details);
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task UrlGivenDirectlyIsNotSentAgainOnceExpired()
        {
            string url = $"{site.Url}/download?{Signed(DateTime.UtcNow.AddSeconds(-58), 60)}";
            site.On("GET", "//download", request =>
            {
                ResponseMessage throttled = LoopbackSite.Status(503);
                throttled.AddHeader("Retry-After", "3");
                return throttled;
            });
            new FilesClient(site.Configuration(maxNetworkRetries: 3));

            // The URL expires before the retry would be sent, and this method cannot ask for a new one.
            ApiException failure = await Assert.ThrowsExactlyAsync<ApiException>(() => FilesClient.StreamDownload(url, new MemoryStream()));

            Assert.AreEqual(503, failure.httpStatus);
            Assert.AreEqual(1, site.Requests.Count);
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task RenewalThatReturnsAnExpiredUrlEndsTheDownload()
        {
            string expired = $"{site.Url}/download?{Signed(DateTime.UtcNow.AddHours(-2), 3600)}";
            site.On("GET", "/files/remote.bin", request => LoopbackSite.Json($"{{\"path\":\"remote.bin\",\"download_uri\":\"{expired}\"}}"));
            FilesClient client = FilesClient.Create(site.Configuration(maxNetworkRetries: 3));

            await Assert.ThrowsExactlyAsync<InvalidResponseException>(() => client.RemoteFiles.DownloadFileAsync("remote.bin", new MemoryStream()));

            // One request for the file's details, which had an expired URL, and one renewal, which did too.
            Assert.AreEqual(2, site.Requests.Count(request => request.Path.StartsWith("/files/remote.bin")));
            Assert.AreEqual(0, site.Requests.Count(request => request.Path.StartsWith("/download")));
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task LegacyUploadIsCompletedWithASingleRequest()
        {
            site.ServeUpload(partsize: 4);
            site.On("POST", "/files/remote.bin", request => LoopbackSite.Status(503), priority: 1);
            FilesClient client = FilesClient.Create(site.Configuration(maxNetworkRetries: 3));

            await Assert.ThrowsExactlyAsync<ApiException>(() => client.RemoteFiles.UploadFileAsync("remote.bin", new MemoryStream(new byte[4]), 4, MTime));

            // Files.com forgets an upload once it completes it, so the request is not repeated after a failure that
            // may have come after completing it.
            Assert.AreEqual(1, site.Requests.Count(request => request.Path == "/files/remote.bin"));
        }

        // A query signed now, valid for seconds, appended to a URL that already has one.
        // What storage answers when a signed URL has expired, as each provider words it.
        private const string AmazonExpired = "<Error><Code>AccessDenied</Code><Message>Request has expired</Message></Error>";
        private const string GoogleExpired = "<Error><Code>ExpiredToken</Code><Message>The provided token has expired.</Message></Error>";
        private const string AzureExpired = "<Error><Code>AuthenticationFailed</Code><Message>Server failed to authenticate the request. Signature not valid in the specified time frame: Start [Fri, 25 Sep 2026 12:00:00 GMT] - Expiry [Fri, 25 Sep 2026 13:00:00 GMT]</Message></Error>";
        private const string Denied = "<Error><Code>AccessDenied</Code><Message>Access Denied</Message></Error>";

        [TestMethod]
        [Timeout(60000)]
        [DataRow(403, AmazonExpired, DisplayName = "Amazon S3: 403, Request has expired")]
        [DataRow(400, GoogleExpired, DisplayName = "Google: 400, ExpiredToken")]
        [DataRow(403, AzureExpired, DisplayName = "Azure: 403, Signature not valid in the specified time frame")]
        public async Task APartStorageRefusesAsExpiredGetsANewUrlForTheSamePart(int status, string answer)
        {
            using (var server = new UploadServer())
            {
                // Every URL's own expiry is an hour away; storage refuses part 2's first URL as expired all the same.
                server.UploadUri = (file, number, negotiation) => server.PartUrl(file, number, negotiation) + SignedFor(3600);
                server.RespondToPart = request => Task.FromResult(request.Number == 2 && request.Attempt == 1 ? LoopbackSite.Json(answer, status: status) : UploadServer.Accept(request));
                byte[] contents = Enumerable.Range(0, 20).Select(value => (byte)value).ToArray();
                var limiter = new TransferLimiter();

                // Without retries: a new URL is not a retry of a failure that may be temporary.
                UploadResult result = await FilesClient.Create(server.Site.Configuration(maxNetworkRetries: 0)).Transfers.UploadStreamAsync(new MemoryStream(contents), contents.Length, "remote.bin", new UploadOptions { Limiter = limiter });

                PartRequest[] part2 = server.Parts.Where(part => part.Number == 2).ToArray();
                Assert.AreEqual(2, part2.Length);
                StringAssert.Contains(part2[0].Request.Path, "negotiation=1");
                StringAssert.Contains(part2[1].Request.Path, "negotiation=2", "Sent again to the renewed URL.");
                CollectionAssert.AreEqual(part2[0].Bytes, part2[1].Bytes, "With the same bytes.");
                JsonElement renewal = server.Negotiations.Single(body => body.TryGetProperty("part", out JsonElement part) && part.GetInt64() == 2 && !body.TryGetProperty("parts", out _));
                Assert.AreEqual("ref-remote.bin", renewal.GetProperty("ref").GetString(), "The renewal belongs to the same upload.");
                Assert.AreEqual(1, server.Finishes.Count);
                CollectionAssert.AreEqual(contents, server.AcceptedBytes("remote.bin"));
                Assert.AreEqual(2, result.Parts[1].Attempts);
                Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("denied", DisplayName = "An ordinary 403: the part's failure, without a new URL")]
        [DataRow("no-retry", DisplayName = "Expired, where the server does not allow retrying parts: the part's failure")]
        [DataRow("always", DisplayName = "Expired again and again: two new URLs, then the part's failure")]
        [DataRow("renewal-refused", DisplayName = "Expired, and Files.com refuses a new URL: its refusal")]
        public async Task APartRefusedOtherwiseOrTooOftenFailsTheUpload(string refusal)
        {
            using (var server = new UploadServer())
            {
                server.UploadUri = (file, number, negotiation) => server.PartUrl(file, number, negotiation) + SignedFor(3600);
                if (refusal == "no-retry")
                {
                    server.RetryParts = "false";
                }
                if (refusal == "renewal-refused")
                {
                    server.RefuseNegotiation = body => body.TryGetProperty("part", out JsonElement part) && part.GetInt64() == 2 && !body.TryGetProperty("parts", out _)
                        ? LoopbackSite.Json("{\"error\":\"Upload not found\",\"http-code\":404,\"type\":\"not-found/file-upload-not-found\"}", status: 404)
                        : null;
                }
                server.RespondToPart = request => Task.FromResult(request.Number == 2 && (refusal == "always" || request.Attempt == 1)
                    ? LoopbackSite.Json(refusal == "denied" ? Denied : AmazonExpired, status: 403)
                    : UploadServer.Accept(request));
                byte[] contents = Enumerable.Range(0, 20).Select(value => (byte)value).ToArray();
                var limiter = new TransferLimiter();

                SdkException error = await Assert.ThrowsAsync<SdkException>(() => FilesClient.Create(server.Site.Configuration(maxNetworkRetries: 1)).Transfers.UploadStreamAsync(new MemoryStream(contents), contents.Length, "remote.bin", new UploadOptions { Limiter = limiter }));

                int renewals = server.Negotiations.Count(body => body.TryGetProperty("part", out JsonElement part) && part.GetInt64() == 2 && !body.TryGetProperty("parts", out _));
                Assert.AreEqual(refusal == "always" ? 3 : 1, server.Parts.Count(part => part.Number == 2), "Times part 2 was sent.");
                Assert.AreEqual(refusal == "always" ? 2 : refusal == "renewal-refused" ? 1 : 0, renewals, "New URLs asked for.");
                if (refusal == "renewal-refused")
                {
                    Assert.IsInstanceOfType(error, typeof(FileUploadNotFoundException));
                }
                else
                {
                    Assert.IsInstanceOfType(error, typeof(InvalidResponseException));
                    Assert.AreEqual(refusal != "denied", error.Message.Contains("has expired"), error.Message);
                    Assert.IsFalse(error.Message.Contains("AccessDenied") || error.Message.Contains("X-Amz"), "Nothing of the answer or the URL is in the message.");
                }
                Assert.AreEqual(0, server.Finishes.Count);
                Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task LegacyTransfersStorageRefusesAsExpiredGetNewUrls()
        {
            int negotiations = 0, puts = 0, downloads = 0, lookups = 0;
            site.On("POST", "/file_actions/begin_upload/remote.bin", request => LoopbackSite.Json($"[{{\"upload_uri\":\"{site.Url}/part?negotiation={++negotiations}{SignedFor(3600)}\",\"http_method\":\"PUT\",\"partsize\":4,\"ref\":\"upload-ref\"}}]"));
            site.On("PUT", "//part", request => ++puts == 1 ? LoopbackSite.Json(AmazonExpired, status: 403) : LoopbackSite.Status(200));
            site.On("POST", "/files/remote.bin", request => LoopbackSite.Json("{\"path\":\"remote.bin\"}"));
            site.On("GET", "/files/remote.bin", request => LoopbackSite.Json($"{{\"path\":\"remote.bin\",\"download_uri\":\"{site.Url}/download?lookup={++lookups}{SignedFor(3600)}\"}}"));
            site.On("GET", "//download", request => ++downloads == 1 ? LoopbackSite.Json(AzureExpired, status: 403) : LoopbackSite.Json("\"contents\""));
            FilesClient client = FilesClient.Create(site.Configuration(maxNetworkRetries: 0));
            byte[] contents = { 1, 2, 3, 4 };
            var destination = new MemoryStream();

            await client.RemoteFiles.UploadFileAsync("remote.bin", new OnceReadableStream(contents), 4, MTime);
            await client.RemoteFiles.DownloadFileAsync("remote.bin", destination);

            ReceivedRequest[] parts = site.Requests.Where(request => request.Path.StartsWith("/part")).ToArray();
            Assert.AreEqual(2, negotiations, "The part's URL was renewed once.");
            StringAssert.Contains(parts[1].Path, "negotiation=2");
            CollectionAssert.AreEqual(contents, parts[1].BodyBytes, "The part read once is sent again unchanged.");
            Assert.AreEqual(2, lookups, "The download's URL was renewed once.");
            StringAssert.Contains(site.Requests.Last(request => request.Path.StartsWith("/download")).Path, "lookup=2");
            Assert.AreEqual("\"contents\"", System.Text.Encoding.UTF8.GetString(destination.ToArray()));
        }

        private static string SignedFor(int seconds)
        {
            return "&" + Signed(DateTime.UtcNow, seconds);
        }

        private static string Signed(DateTime signedAt, int seconds)
        {
            return $"X-Goog-Algorithm=GOOG4-RSA-SHA256&X-Goog-Date={signedAt.ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture)}&X-Goog-Expires={seconds}&X-Goog-Signature=stand-in";
        }

        // A forward-only stream that can be read once.
        private sealed class OnceReadableStream : Stream
        {
            private readonly MemoryStream contents;

            public OnceReadableStream(byte[] contents)
            {
                this.contents = new MemoryStream(contents);
            }

            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override int Read(byte[] buffer, int offset, int count) => contents.Read(buffer, offset, count);
            public override void Flush()
            {
            }
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        }
    }
}