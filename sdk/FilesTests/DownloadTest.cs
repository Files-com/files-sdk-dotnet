using FilesCom;
using FilesCom.Transfers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace FilesTests
{
    // Downloads with client.Transfers: bound to a version by its storage's first answer, a file is fetched in exactly
    // checked parts, several at once, each conditioned on that answer's entity tag, and published only whole; not bound,
    // it comes in one request. Answers that are not exactly the bytes asked for, and signs that the version changed, stop
    // the download without publishing or splicing anything.
    [TestClass]
    public partial class DownloadTest
    {
        private static readonly byte[] Contents = Enumerable.Range(0, 100 * 1024).Select(i => (byte)(i * 31 + 7)).ToArray();
        private string folder;

        [TestInitialize]
        public void Start()
        {
            folder = Path.Combine(Path.GetTempPath(), "download-test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
        }

        [TestCleanup]
        public void Stop()
        {
            Directory.Delete(folder, true);
        }

        private string Destination => Path.Combine(folder, "file.bin");

        private static DownloadOptions Parts(int concurrency = 4) => new DownloadOptions { PartSize = 16 * 1024, MaxConcurrency = concurrency };

        [TestMethod]
        [Timeout(60000)]
        public async Task ABoundFileIsFetchedInExactPartsAndPublishedWhole()
        {
            using (var server = new DownloadServer(Contents))
            {
                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Parts());

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.IsTrue(result.VersionBound);
                Assert.AreEqual(7, result.Parts);
                Assert.AreEqual(Contents.Length, result.Size);
                Assert.AreEqual(DownloadServer.DefaultDownload, server.ApiRequests.Single(), "An ordinary Download, which asks for no version.");
                List<StorageRequest> requests = server.StorageRequests.ToList();
                CollectionAssert.AreEquivalent(Enumerable.Range(0, 7).Select(i => $"GET {i * 16384}-{Math.Min(i * 16384 + 16383, Contents.Length - 1)}").ToList(), requests.Select(request => request.ToString()).ToList());
                Assert.AreEqual("GET 0-16383", requests[0].ToString(), "The first part is asked for first, alone.");
                Assert.IsNull(requests[0].IfMatch, "The first part binds the download: it carries no condition.");
                Assert.IsTrue(requests.Skip(1).All(request => request.IfMatch == server.ETag), "Every other part carries the entity tag the first was answered with.");
                Assert.IsTrue(requests.All(request => request.Header("Accept-Encoding") == "identity"), "The bytes are asked for as stored.");
                CollectionAssert.AreEqual(new[] { "file.bin" }, Directory.GetFileSystemEntries(folder).Select(Path.GetFileName).ToArray(), "No stage is left behind.");
            }
        }

        // Parts arrive in any order; the file, or the stream, still holds the exact bytes in order.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("file")]
        [DataRow("stream")]
        public async Task PartsThatArriveOutOfOrderMakeTheExactFile(string destination)
        {
            using (var server = new DownloadServer(Contents))
            {
                var firstHeld = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                int lastParts = 0;
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range?.First == 16384)
                    {
                        // The second part waits until the last three have been served.
                        await firstHeld.Task;
                    }
                    else if (request.Range?.First >= 4 * 16384 && Interlocked.Increment(ref lastParts) == 3)
                    {
                        firstHeld.TrySetResult(true);
                    }
                    return false;
                };
                byte[] downloaded = await Download(server, destination, Parts(concurrency: 7));

                CollectionAssert.AreEqual(Contents, downloaded);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("file")]
        [DataRow("stream")]
        public async Task AFileWithoutAVersionComesInOneRequest(string destination)
        {
            using (var server = new DownloadServer(Contents) { Bound = false })
            {
                DownloadResult result = null;
                byte[] downloaded = await Download(server, destination, Parts(), done => result = done);

                CollectionAssert.AreEqual(Contents, downloaded);
                Assert.IsFalse(result.VersionBound);
                Assert.AreEqual(1, result.Parts);
                StorageRequest request = server.StorageRequests.Single();
                Assert.IsNull(request.Range);
                Assert.IsNull(request.IfMatch);
            }
        }

        // A request for the whole file answered with part of it is refused, not taken for the whole file.
        [TestMethod]
        [Timeout(60000)]
        public async Task AnUnsolicitedPartialAnswerIsNotTakenForTheWholeFile()
        {
            using (var server = new DownloadServer(Contents) { Bound = false })
            {
                server.Intercept = async (request, connection) =>
                {
                    await DownloadServer.SendAsync(connection, 206, Contents, 0, 1000, $"Content-Range: bytes 0-999/{Contents.Length}\r\n");
                    return true;
                };

                InvalidResponseException error = await Assert.ThrowsExactlyAsync<InvalidResponseException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Parts()));

                StringAssert.Contains(error.Message, "HTTP 206");
                AssertNothingPublished();
            }
        }

        // An answer from the Files.com web app is refused, whatever its status, before any byte is written.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(200, true)]
        [DataRow(200, false)]
        [DataRow(404, false)]
        public async Task TheWebAppsAnswerIsRefusedBeforeAnyByteIsWritten(int status, bool bound)
        {
            using (var server = new DownloadServer(Contents) { Bound = bound })
            {
                server.Intercept = async (request, connection) =>
                {
                    await connection.RespondAsync(status, "<html>Files.com</html>", "X-Files-Frontend-App: true\r\n");
                    return true;
                };
                var stream = new MemoryStream();

                InvalidResponseException error = await Assert.ThrowsExactlyAsync<InvalidResponseException>(() => Client(server).Transfers.DownloadStreamAsync("remote.bin", stream, Parts()));

                StringAssert.Contains(error.Message, "web app");
                Assert.AreEqual(0, stream.Length);
            }
        }

        // A part whose body ends early is fetched again, alone, with the same range; the others are fetched once.
        [TestMethod]
        [Timeout(60000)]
        public async Task APartThatEndsEarlyIsFetchedAgainAlone()
        {
            using (var server = new DownloadServer(Contents))
            {
                int truncated = 0;
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range?.First == 16384 && Interlocked.Increment(ref truncated) == 1)
                    {
                        await connection.SendAsync($"HTTP/1.1 206 OK\r\nContent-Length: 16384\r\nContent-Range: bytes 16384-32767/{Contents.Length}\r\nETag: {server.ETag}\r\n\r\n");
                        await connection.SendAsync(Contents.Skip(16384).Take(5000).ToArray());
                        connection.Dispose();
                        return true;
                    }
                    return false;
                };

                DownloadResult result = await Client(server, retries: 2).Transfers.DownloadFileAsync("remote.bin", Destination, Parts());

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual(1, result.FinalProgress.Retries);
                Assert.AreEqual(2, server.StorageRequests.Count(request => request.Range?.First == 16384));
                Assert.AreEqual(8, server.StorageRequests.Count);
                Assert.AreEqual(Contents.Length + 5000, result.ReceivedBytes);
            }
        }

        // A first answer without an entity tag, for a file no larger than the part, is the whole file only once all of it
        // has arrived. Cut short, it is fetched again, and decides nothing: when the second answer binds a larger version,
        // that version is fetched in all its parts and published, or written, whole.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("file")]
        [DataRow("stream")]
        public async Task AnUnboundFirstAnswerCutShortDecidesNothingAboutTheFile(string destination)
        {
            using (var server = new DownloadServer(Contents))
            {
                int first = 0;
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range?.First != 0 || Interlocked.Increment(ref first) != 1)
                    {
                        return false;
                    }
                    // A 1,000-byte file, without an entity tag, whose body ends after 500 bytes.
                    await connection.SendAsync("HTTP/1.1 206 OK\r\nContent-Length: 1000\r\nContent-Range: bytes 0-999/1000\r\n\r\n");
                    await connection.SendAsync(Contents.Take(500).ToArray());
                    connection.Dispose();
                    return true;
                };

                (DownloadResult result, byte[] downloaded) = await DownloadWithRetries(server, destination);

                CollectionAssert.AreEqual(Contents, downloaded, "The whole version the second answer bound.");
                Assert.IsTrue(result.VersionBound);
                Assert.AreEqual(Contents.Length, result.Size);
                Assert.AreEqual(7, result.Parts);
                Assert.AreEqual(8, server.StorageRequests.Count, "The cut-short answer, then the version's 7 parts.");
                Assert.IsTrue(server.StorageRequests.Skip(2).All(request => request.IfMatch == server.ETag), "Every part after the binding answer is conditioned on its entity tag.");
            }
        }

        // A file smaller than a part is bound by a first answer that names all of it. When that answer's body is cut short,
        // the part is fetched again as exactly the bytes it named, as the version.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("file")]
        [DataRow("stream")]
        public async Task ASmallFilesFirstAnswerCutShortIsFetchedAgainAsTheBytesItNamed(string destination)
        {
            byte[] contents = Contents.Take(1000).ToArray();
            using (var server = new DownloadServer(contents))
            {
                int first = 0;
                server.Intercept = async (request, connection) =>
                {
                    if (Interlocked.Increment(ref first) != 1)
                    {
                        return false;
                    }
                    await connection.SendAsync($"HTTP/1.1 206 OK\r\nContent-Length: 1000\r\nContent-Range: bytes 0-999/1000\r\nETag: {server.ETag}\r\n\r\n");
                    await connection.SendAsync(contents.Take(500).ToArray());
                    connection.Dispose();
                    return true;
                };

                (DownloadResult result, byte[] downloaded) = await DownloadWithRetries(server, destination);

                CollectionAssert.AreEqual(contents, downloaded);
                Assert.IsTrue(result.VersionBound);
                Assert.AreEqual(1, result.FinalProgress.Retries);
                Assert.AreEqual(2, server.StorageRequests.Count);
                Assert.AreEqual((0L, 999L), server.StorageRequests[1].Range, "Fetched again as exactly the bytes the first answer named.");
                Assert.AreEqual(server.ETag, server.StorageRequests[1].IfMatch);
            }
        }

        // An answer that is not exactly the part asked for, of the version bound, fails the download; nothing is published.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("another range", typeof(InvalidResponseException))]
        [DataRow("no content range", typeof(InvalidResponseException))]
        [DataRow("another content length", typeof(InvalidResponseException))]
        [DataRow("more bytes than the range", typeof(InvalidResponseException))]
        [DataRow("an encoded body", typeof(InvalidResponseException))]
        [DataRow("another total", typeof(DownloadSourceChangedException))]
        [DataRow("another entity tag", typeof(DownloadSourceChangedException))]
        [DataRow("a weak entity tag", typeof(DownloadSourceChangedException))]
        [DataRow("no entity tag", typeof(InvalidResponseException))]
        [DataRow("416", typeof(DownloadSourceChangedException))]
        public async Task AnswersThatAreNotExactlyThePartFail(string answer, Type expected)
        {
            using (var server = new DownloadServer(Contents))
            {
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range?.First != 16384)
                    {
                        return false;
                    }
                    string range = $"Content-Range: bytes 16384-32767/{Contents.Length}\r\n";
                    string etag = $"ETag: {server.ETag}\r\n";
                    switch (answer)
                    {
                        case "another range":
                            await DownloadServer.SendAsync(connection, 206, Contents, 16385, 16384, $"Content-Range: bytes 16385-32768/{Contents.Length}\r\n{etag}");
                            break;
                        case "no content range":
                            await DownloadServer.SendAsync(connection, 206, Contents, 16384, 16384, etag);
                            break;
                        case "another content length":
                            await DownloadServer.SendAsync(connection, 206, Contents, 16384, 16000, range + etag);
                            break;
                        case "more bytes than the range":
                            // Chunked, so nothing but the SDK's own count stops the extra bytes.
                            await connection.SendAsync($"HTTP/1.1 206 OK\r\nTransfer-Encoding: chunked\r\n{range}{etag}\r\n");
                            await connection.SendAsync($"{16384 + 10:x}\r\n");
                            await connection.SendAsync(Contents.Skip(16384).Take(16384 + 10).ToArray());
                            await connection.SendAsync("\r\n0\r\n\r\n");
                            break;
                        case "an encoded body":
                            await DownloadServer.SendAsync(connection, 206, Contents, 16384, 16384, range + etag + "Content-Encoding: gzip\r\n");
                            break;
                        case "another total":
                            await DownloadServer.SendAsync(connection, 206, Contents, 16384, 16384, $"Content-Range: bytes 16384-32767/{Contents.Length + 1}\r\n{etag}");
                            break;
                        case "another entity tag":
                            await DownloadServer.SendAsync(connection, 206, Contents, 16384, 16384, range + "ETag: \"another\"\r\n");
                            break;
                        case "a weak entity tag":
                            await DownloadServer.SendAsync(connection, 206, Contents, 16384, 16384, range + $"ETag: W/{server.ETag}\r\n");
                            break;
                        case "no entity tag":
                            await DownloadServer.SendAsync(connection, 206, Contents, 16384, 16384, range);
                            break;
                        default:
                            await connection.RespondAsync(416, "", $"Content-Range: bytes */{Contents.Length}\r\n");
                            break;
                    }
                    return true;
                };

                Exception error = await Assert.ThrowsAsync<Exception>(() => Client(server, retries: 2).Transfers.DownloadFileAsync("remote.bin", Destination, Parts()));

                Assert.AreEqual(expected, error.GetType(), error.ToString());
                Assert.AreEqual(1, server.StorageRequests.Count(request => request.Range?.First == 16384), "Not retried.");
                AssertNothingPublished();
            }
        }

        // When the file changes during the download, the storage server refuses the old version's parts: the download
        // stops, nothing is published, and a stream holds only whole parts of the old version.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("file")]
        [DataRow("stream")]
        public async Task AChangeDuringTheDownloadStopsItWithoutMixingVersions(string destination)
        {
            using (var server = new DownloadServer(Contents))
            {
                byte[] changed = Contents.Select(b => (byte)(b ^ 0xFF)).ToArray();
                server.Intercept = (request, connection) =>
                {
                    if (request.Range?.First == 16384 && server.Contents == Contents)
                    {
                        server.Change(changed);
                    }
                    return Task.FromResult(false);
                };
                var stream = new MemoryStream();
                Task download = destination == "file"
                    ? Client(server, retries: 3).Transfers.DownloadFileAsync("remote.bin", Destination, Parts(concurrency: 1))
                    : Client(server, retries: 3).Transfers.DownloadStreamAsync("remote.bin", stream, Parts(concurrency: 1));

                DownloadSourceChangedException error = await Assert.ThrowsExactlyAsync<DownloadSourceChangedException>(() => download);

                StringAssert.Contains(error.Message, "412");
                AssertNothingPublished();
                Assert.AreEqual(0, stream.Length % 16384, "Only whole parts.");
                CollectionAssert.AreEqual(Contents.Take((int)stream.Length).ToArray(), stream.ToArray(), "Only the first version's bytes.");
            }
        }

        // Storage that ignores If-Match, as one behind a server that drops the condition would, still cannot make the
        // download mix versions: the part that comes from the new version is answered with its entity tag, which is not
        // the version's, so it is refused, nothing is published, and a stream holds only whole parts of the old version.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("file")]
        [DataRow("stream")]
        public async Task StorageThatIgnoresIfMatchStillCannotMixVersions(string destination)
        {
            using (var server = new DownloadServer(Contents) { IgnoresIfMatch = true })
            {
                byte[] changed = Contents.Select(b => (byte)(b ^ 0x5A)).ToArray();
                server.Intercept = (request, connection) =>
                {
                    if (request.Range?.First == 2 * 16384 && server.Contents == Contents)
                    {
                        server.Change(changed);
                    }
                    return Task.FromResult(false);
                };
                var stream = new MemoryStream();
                Task download = destination == "file"
                    ? Client(server, retries: 3).Transfers.DownloadFileAsync("remote.bin", Destination, Parts(concurrency: 1))
                    : Client(server, retries: 3).Transfers.DownloadStreamAsync("remote.bin", stream, Parts(concurrency: 1));

                DownloadSourceChangedException error = await Assert.ThrowsExactlyAsync<DownloadSourceChangedException>(() => download);

                StringAssert.Contains(error.Message, "entity tag");
                Assert.AreEqual(1, server.StorageRequests.Count(request => request.Range?.First == 2 * 16384), "Not fetched again.");
                AssertNothingPublished();
                Assert.AreEqual(0, stream.Length % 16384, "Only whole parts.");
                CollectionAssert.AreEqual(Contents.Take((int)stream.Length).ToArray(), stream.ToArray(), "Only the first version's bytes.");
            }
        }

        // Every storage request goes to the URL exactly as Files.com gave it, its signed query in its order and with its
        // escapes, as a GET: no version selector or other parameter is added, and no other method is used.
        [TestMethod]
        [Timeout(60000)]
        public async Task StorageRequestsSendTheSignedUrlExactlyAsGiven()
        {
            string checkpoint = Path.Combine(folder, "download.checkpoint");
            const string signed = "&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIDEXAMPLE%2F20261001%2Fus-east-1%2Fs3%2Faws4_request"
                + "&X-Amz-SignedHeaders=host&response-content-disposition=attachment%3B%20filename%2A%3DUTF-8%27%27r%C3%A9sum%C3%A9%20%281%29.bin"
                + "&response-content-type=application%2Foctet-stream&X-Amz-Signature=0f9e8d7c";
            using (var server = new DownloadServer(Contents) { MoreQuery = signed })
            {
                var cancellation = new CancellationTokenSource();
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range?.First == 3 * 16384 && !cancellation.IsCancellationRequested)
                    {
                        cancellation.Cancel();
                        await connection.HoldAsync();
                        return true;
                    }
                    return false;
                };
                var options = new DownloadOptions { PartSize = 16 * 1024, MaxConcurrency = 2, CheckpointPath = checkpoint };
                await Assert.ThrowsAsync<OperationCanceledException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options, cancellation.Token));
                await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options);

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                string expected = "/storage/1/remote.bin?X-Files-Signature=stand-in" + signed;
                Assert.IsTrue(server.StorageRequests.Count > 7, "The first part, the parts, the confirmation and the parts continued.");
                foreach (StorageRequest request in server.StorageRequests)
                {
                    Assert.AreEqual($"GET {expected} HTTP/1.1", request.Line);
                }
            }
        }

        // A URL about to expire is renewed before it is used, with an ordinary Download, and the version is kept only
        // while it is the same storage object's: a new URL for another object stops the download before anything is
        // fetched from it, even when that object answers with the same entity tag; a new version at the same object is
        // refused by storage, as every renewed request still carries the version's entity tag.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("same version")]
        [DataRow("version gone")]
        [DataRow("another object, same entity tag")]
        public async Task AnExpiringUrlIsRenewedForTheSameVersionOnly(string renewal)
        {
            // Every URL expires within the renewal margin, so every part renews it first.
            using (var server = new DownloadServer(Contents) { UrlLifetime = 20 })
            {
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range?.First != 0)
                    {
                        return false;
                    }
                    // The first part is served as the version; then the file changes, or moves to another object.
                    await DownloadServer.SendAsync(connection, 206, Contents, 0, 16384, $"Content-Range: bytes 0-16383/{Contents.Length}\r\nETag: {server.ETag}\r\n");
                    if (renewal == "version gone")
                    {
                        server.Change(Contents.Reverse().ToArray());
                    }
                    else if (renewal == "another object, same entity tag")
                    {
                        server.Relocate(contents: Contents.Reverse().ToArray(), keepETag: true);
                    }
                    return true;
                };
                FilesClient client = Client(server);

                if (renewal == "same version")
                {
                    await client.Transfers.DownloadFileAsync("remote.bin", Destination, Parts(concurrency: 1));
                    CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                    Assert.AreEqual(8, server.ApiRequests.Count, "The first answer, then a renewal before each of the 7 parts.");
                    Assert.IsTrue(server.ApiRequests.All(request => request == DownloadServer.DefaultDownload), "Every renewal is an ordinary Download.");
                    Assert.IsTrue(server.StorageRequests.Skip(1).All(request => request.IfMatch == server.ETag), "Renewed URLs are still sent the version's entity tag.");
                    return;
                }
                DownloadSourceChangedException error = await Assert.ThrowsExactlyAsync<DownloadSourceChangedException>(() => client.Transfers.DownloadFileAsync("remote.bin", Destination, Parts(concurrency: 1)));
                if (renewal == "version gone")
                {
                    StringAssert.Contains(error.Message, "412");
                    Assert.AreEqual(2, server.StorageRequests.Count, "The renewed URL's first request was refused.");
                }
                else
                {
                    StringAssert.Contains(error.Message, "another storage resource");
                    Assert.AreEqual(1, server.StorageRequests.Count, "Nothing was fetched from the other object.");
                }
                AssertNothingPublished();
            }
        }

        // A storage server that ignores Range gets one request for the whole file: not bound when its first answer is
        // already the whole file, and as the version when it starts ignoring Range once the download is bound.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("file", "first answer")]
        [DataRow("stream", "first answer")]
        [DataRow("file", "later part")]
        public async Task AServerThatIgnoresRangeIsDownloadedWhole(string destination, string when)
        {
            using (var server = new DownloadServer(Contents) { IgnoresRange = when == "first answer" })
            {
                server.Intercept = (request, connection) =>
                {
                    if (request.Range?.First == 16384)
                    {
                        server.IgnoresRange = true;
                    }
                    return Task.FromResult(false);
                };
                DownloadResult result = null;
                byte[] downloaded = await Download(server, destination, Parts(concurrency: 1), done => result = done);

                CollectionAssert.AreEqual(Contents, downloaded);
                Assert.AreEqual(when == "later part", result.VersionBound);
                Assert.AreEqual(1, result.Parts);
                StorageRequest whole = server.StorageRequests.Last();
                Assert.IsNull(whole.Range);
                Assert.AreEqual(when == "later part" ? server.ETag : null, whole.IfMatch);
            }
        }

        // An empty file has no range to bind it: its first answer, 416, says so, and the file comes in one request.
        [TestMethod]
        [Timeout(60000)]
        public async Task AnEmptyFileComesInOneRequest()
        {
            using (var server = new DownloadServer(new byte[0]))
            {
                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Parts());

                Assert.AreEqual(0, new FileInfo(Destination).Length);
                Assert.IsFalse(result.VersionBound);
                Assert.AreEqual(0, result.Size);
                CollectionAssert.AreEqual(new[] { "GET 0-16383", "GET whole" }, server.StorageRequests.Select(request => request.ToString()).ToList());
            }
        }

        // A first answer that cannot bind the download, without a strong entity tag, leaves a larger file to come in one
        // request, unconditioned; a file the first part holds whole is that answer. A first answer whose framing is not
        // exactly the part fails the download before anything is published.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("no entity tag", null)]
        [DataRow("a weak entity tag", null)]
        [DataRow("an entity tag that is not quoted", null)]
        [DataRow("two entity tags", null)]
        [DataRow("no entity tag, a small file", null)]
        [DataRow("another range", "with bytes 1-16384")]
        [DataRow("no content range", "without a Content-Range")]
        [DataRow("another content length", "Content-Length of 16000")]
        [DataRow("an encoded body", "encoded as gzip")]
        public async Task FirstAnswersThatCannotBindTheDownload(string answer, string problem)
        {
            byte[] contents = answer == "no entity tag, a small file" ? Contents.Take(1000).ToArray() : Contents;
            using (var server = new DownloadServer(contents))
            {
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range?.First != 0)
                    {
                        return false;
                    }
                    long length = Math.Min(16384, contents.Length);
                    string range = $"Content-Range: bytes 0-{length - 1}/{contents.Length}\r\n";
                    switch (answer)
                    {
                        case "a weak entity tag":
                            await DownloadServer.SendAsync(connection, 206, contents, 0, length, range + $"ETag: W/{server.ETag}\r\n");
                            break;
                        case "an entity tag that is not quoted":
                            await DownloadServer.SendAsync(connection, 206, contents, 0, length, range + "ETag: etag-1\r\n");
                            break;
                        case "two entity tags":
                            await DownloadServer.SendAsync(connection, 206, contents, 0, length, range + $"ETag: {server.ETag}\r\nETag: \"other\"\r\n");
                            break;
                        case "another range":
                            await DownloadServer.SendAsync(connection, 206, contents, 1, length, $"Content-Range: bytes 1-{length}/{contents.Length}\r\nETag: {server.ETag}\r\n");
                            break;
                        case "no content range":
                            await DownloadServer.SendAsync(connection, 206, contents, 0, length, $"ETag: {server.ETag}\r\n");
                            break;
                        case "another content length":
                            await DownloadServer.SendAsync(connection, 206, contents, 0, 16000, range + $"ETag: {server.ETag}\r\n");
                            break;
                        case "an encoded body":
                            await DownloadServer.SendAsync(connection, 206, contents, 0, length, range + $"ETag: {server.ETag}\r\nContent-Encoding: gzip\r\n");
                            break;
                        default:
                            await DownloadServer.SendAsync(connection, 206, contents, 0, length, range);
                            break;
                    }
                    return true;
                };

                if (problem != null)
                {
                    InvalidResponseException error = await Assert.ThrowsExactlyAsync<InvalidResponseException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Parts()));
                    StringAssert.Contains(error.Message, problem);
                    Assert.AreEqual(1, server.StorageRequests.Count);
                    AssertNothingPublished();
                    return;
                }
                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Parts());

                CollectionAssert.AreEqual(contents, File.ReadAllBytes(Destination));
                Assert.IsFalse(result.VersionBound);
                Assert.AreEqual(1, result.Parts);
                if (answer == "no entity tag, a small file")
                {
                    Assert.AreEqual(1, server.StorageRequests.Count, "The first answer is the whole file.");
                    return;
                }
                CollectionAssert.AreEqual(new[] { "GET 0-16383", "GET whole" }, server.StorageRequests.Select(request => request.ToString()).ToList());
                Assert.IsNull(server.StorageRequests.Last().IfMatch);
            }
        }

        // A small file's first answer without a strong entity tag is the whole file, unbound, once all of it has arrived;
        // without a Content-Length it must also have the size Files.com stated, which its Content-Range does not stand in
        // for. A stated size lower or higher fails the download once that answer is all here, without another attempt,
        // publication or a byte given to a stream. A Content-Length or a strong entity tag comes first, and an answer cut
        // short of its Content-Range is still the connection's failure, which decides nothing and is fetched again.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("file", 500, "no length")]
        [DataRow("stream", 500, "no length")]
        [DataRow("file", 5000, "no length")]
        [DataRow("stream", 5000, "no length")]
        [DataRow("file", 1000, "no length")]
        [DataRow("stream", 1000, "no length")]
        [DataRow("file", 5000, "a Content-Length")]
        [DataRow("stream", 5000, "a strong entity tag")]
        [DataRow("file", 5000, "cut short")]
        public async Task ASmallFilesWholeFirstAnswerWithoutAContentLengthMustHaveTheStatedSize(string destination, int stated, string answer)
        {
            byte[] contents = Contents.Take(1000).ToArray();
            using (var server = new DownloadServer(contents))
            {
                server.ApiIntercept = (target, connection) => StateSizeAsync(server, connection, $"\"size\":{stated},");
                int answers = 0;
                server.Intercept = async (request, connection) =>
                {
                    if (Interlocked.Increment(ref answers) != 1)
                    {
                        return false;
                    }
                    // Bytes 0-999 of a 1,000-byte file, ended by closing the connection unless it declares its length.
                    string head = "HTTP/1.1 206 OK\r\nContent-Range: bytes 0-999/1000\r\nContent-Type: application/octet-stream\r\n";
                    head += answer == "a Content-Length" ? "Content-Length: 1000\r\n" : "";
                    head += answer == "a strong entity tag" ? $"ETag: {server.ETag}\r\n" : "";
                    await connection.SendAsync(head + "\r\n");
                    await connection.SendAsync(contents.Take(answer == "cut short" ? 500 : 1000).ToArray());
                    connection.Dispose();
                    return true;
                };
                FilesClient client = Client(server, retries: 2);
                var stream = new MemoryStream();
                Func<Task<DownloadResult>> download = () => destination == "file"
                    ? client.Transfers.DownloadFileAsync("remote.bin", Destination, Parts())
                    : client.Transfers.DownloadStreamAsync("remote.bin", stream, Parts());

                if (answer == "no length" && stated != contents.Length)
                {
                    InvalidResponseException error = await Assert.ThrowsExactlyAsync<InvalidResponseException>(() => download());
                    StringAssert.Contains(error.Message, $"{stated} bytes Files.com stated");
                    Assert.IsFalse(error.RequestMayHaveBeenProcessed);
                    Assert.AreEqual(1, server.StorageRequests.Count, "Not fetched again.");
                    Assert.AreEqual(1, server.ApiRequests.Count, "Not asked for again.");
                    if (destination == "file")
                    {
                        AssertNothingPublished();
                    }
                    else
                    {
                        Assert.AreEqual(0, stream.Length, "Nothing given to the stream.");
                    }
                    return;
                }
                DownloadResult result = await download();

                CollectionAssert.AreEqual(contents, destination == "file" ? File.ReadAllBytes(Destination) : stream.ToArray());
                Assert.AreEqual(answer == "a strong entity tag" || answer == "cut short", result.VersionBound);
                Assert.AreEqual(answer == "cut short" ? 2 : 1, server.StorageRequests.Count, "A cut-short first answer is fetched again; no other is.");
                Assert.AreEqual(1, server.ApiRequests.Count);
            }
        }

        // A request for a range, or bound to a version, follows no redirect, within its origin or to another, so its answer
        // is always from the object the version belongs to, and If-Match goes nowhere else. A redirected first answer
        // binds nothing: the file then comes in one request, which follows the redirect, as any download not bound does.
        // A bound part redirected fails the download, and nothing is sent where it points.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("first answer", true)]
        [DataRow("first answer", false)]
        [DataRow("bound part", true)]
        [DataRow("bound part", false)]
        public async Task RangesAndConditionsFollowNoRedirect(string redirected, bool sameOrigin)
        {
            using (var elsewhere = new DownloadServer(Contents))
            using (var server = new DownloadServer(Contents))
            {
                server.Intercept = async (request, connection) =>
                {
                    // A first answer's redirect applies to every request; a bound part's, only to the second part.
                    if (request.Line.Contains("moved=1") || (redirected == "bound part" && request.Range?.First != 16384))
                    {
                        return false;
                    }
                    string target = sameOrigin ? $"{server.StorageUrl}/storage/1/remote.bin?moved=1" : $"{elsewhere.StorageUrlAtPort}/storage/1/remote.bin?moved=1";
                    await connection.SendAsync($"HTTP/1.1 307 Moved\r\nLocation: {target}\r\nContent-Length: 0\r\n\r\n");
                    return true;
                };
                List<StorageRequest> Followed() => server.StorageRequests.Concat(elsewhere.StorageRequests).Where(request => request.Line.Contains("moved=1")).ToList();

                if (redirected == "first answer")
                {
                    DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Parts(concurrency: 1));
                    CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                    Assert.IsFalse(result.VersionBound);
                    Assert.AreEqual("GET whole", Followed().Single().ToString(), "Only the request for the whole file follows the redirect.");
                    Assert.IsNull(Followed().Single().IfMatch);
                    return;
                }
                InvalidResponseException error = await Assert.ThrowsExactlyAsync<InvalidResponseException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Parts(concurrency: 1)));
                StringAssert.Contains(error.Message, "redirected");
                Assert.AreEqual(0, Followed().Count, "Nothing was sent where the redirect points.");
                AssertNothingPublished();
            }
        }

        // Cancelling stops every part, including while the first part's answer, which binds the download, is awaited and
        // no other part may start; nothing is published.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(0)]
        [DataRow(3)]
        public async Task CancellingStopsEveryPartAndPublishesNothing(int part)
        {
            using (var server = new DownloadServer(Contents))
            {
                var cancellation = new CancellationTokenSource();
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range?.First == part * 16384)
                    {
                        cancellation.Cancel();
                        await connection.HoldAsync();
                        return true;
                    }
                    return false;
                };

                await Assert.ThrowsAsync<OperationCanceledException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Parts(concurrency: 2), cancellation.Token));

                AssertNothingPublished();
                if (part == 0)
                {
                    Assert.AreEqual(1, server.StorageRequests.Count, "No other part starts before the first one's answer.");
                }
            }
        }

        // The destination stream's own failure is thrown as it is, and never retried, whatever its type, and whatever the
        // answer it was writing, even one without a length that is shorter than the size Files.com stated.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(true, true)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public async Task ADestinationsFailureIsThrownAsItIs(bool bound, bool declaresLength)
        {
            using (var server = new DownloadServer(Contents) { Bound = bound })
            {
                if (!declaresLength)
                {
                    server.Intercept = async (request, connection) =>
                    {
                        await SendWithoutLengthAsync(connection, Contents.Take(1000).ToArray());
                        return true;
                    };
                }
                var failure = new HttpRequestException("The destination's own connection failed.");
                var stream = new FailingStream(failure);

                Exception error = await Assert.ThrowsAsync<Exception>(() => Client(server, retries: 3).Transfers.DownloadStreamAsync("remote.bin", stream, Parts()));

                Assert.AreSame(failure, error);
                Assert.AreEqual(1, stream.Writes);
            }
        }

        // A connection that fails while the file arrives, and still fails when retried, is an ApiConnectionException, as
        // for every other request of the SDK, and one that stalls is the NetworkTimeoutException it is; neither may have
        // changed anything. Nothing is published, and a stream keeps only what it was given, never written twice: a
        // bound file's first parts, whole, or what arrived of a file coming whole, which is not fetched again.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("file", true, "ends early")]
        [DataRow("stream", true, "ends early")]
        [DataRow("file", false, "ends early")]
        [DataRow("stream", false, "ends early")]
        [DataRow("stream", false, "stalls")]
        public async Task AConnectionThatFailsAsTheFileArrivesIsAConnectionFailure(string destination, bool bound, string failure)
        {
            using (var server = new DownloadServer(Contents) { Bound = bound })
            {
                server.Intercept = async (request, connection) =>
                {
                    if (bound && request.Range?.First != 16384)
                    {
                        return false;
                    }
                    string head = bound
                        ? $"HTTP/1.1 206 OK\r\nContent-Length: 16384\r\nContent-Range: bytes 16384-32767/{Contents.Length}\r\nETag: {server.ETag}\r\n"
                        : $"HTTP/1.1 200 OK\r\nContent-Length: {Contents.Length}\r\n";
                    await connection.SendAsync(head + "Content-Type: application/octet-stream\r\n\r\n");
                    await connection.SendAsync(Contents.Skip(bound ? 16384 : 0).Take(5000).ToArray());
                    if (failure == "stalls")
                    {
                        await connection.HoldAsync();
                    }
                    else
                    {
                        connection.Dispose();
                    }
                    return true;
                };
                FilesConfiguration configuration = server.Configuration(maxNetworkRetries: 1);
                configuration.ReadTimeout = 1;
                FilesClient client = server.Client(configuration);
                var stream = new MemoryStream();

                Exception error = await Assert.ThrowsAsync<Exception>(() => destination == "file"
                    ? client.Transfers.DownloadFileAsync("remote.bin", Destination, Parts(concurrency: 1))
                    : client.Transfers.DownloadStreamAsync("remote.bin", stream, Parts(concurrency: 1)));

                Assert.AreEqual(failure == "stalls" ? typeof(NetworkTimeoutException) : typeof(ApiConnectionException), error.GetType(), error.ToString());
                Assert.IsFalse(((SdkException)error).RequestMayHaveBeenProcessed);
                bool written = destination == "stream" && !bound;
                Assert.AreEqual(written ? 1 : 2, server.StorageRequests.Count(request => !bound || request.Range?.First == 16384), written ? "Not fetched again once written." : "Retried, then given up.");
                if (destination == "file")
                {
                    AssertNothingPublished();
                }
                else if (bound)
                {
                    Assert.IsTrue(stream.Length % 16384 == 0 && stream.Length <= 16384, $"Whole parts only, not {stream.Length} bytes.");
                    CollectionAssert.AreEqual(Contents.Take((int)stream.Length).ToArray(), stream.ToArray());
                }
                else
                {
                    CollectionAssert.AreEqual(Contents.Take(5000).ToArray(), stream.ToArray(), "What arrived, once.");
                }
            }
        }

        // A file that comes in one answer that declares no length, ended by closing its connection, must have the size
        // Files.com stated for it. A shorter or longer body is inconsistent with that size, which shows neither a body cut
        // short nor a changed file: the download fails, and is not tried again. Nothing is published, and a stream holds
        // what was written before the difference showed, once: all of a shorter body, nothing past the stated size.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("file", -10)]
        [DataRow("stream", -10)]
        [DataRow("file", 0)]
        [DataRow("stream", 0)]
        [DataRow("file", 10)]
        [DataRow("stream", 10)]
        public async Task AnAnswerWithoutALengthMustHaveTheSizeFilesComStated(string destination, int difference)
        {
            byte[] body = Contents.Concat(Contents).Take(Contents.Length + difference).ToArray();
            using (var server = new DownloadServer(Contents) { Bound = false })
            {
                server.Intercept = async (request, connection) =>
                {
                    await SendWithoutLengthAsync(connection, body);
                    return true;
                };
                FilesClient client = Client(server, retries: 2);
                var stream = new MemoryStream();
                Func<Task<DownloadResult>> download = () => destination == "file"
                    ? client.Transfers.DownloadFileAsync("remote.bin", Destination, Parts())
                    : client.Transfers.DownloadStreamAsync("remote.bin", stream, Parts());

                if (difference == 0)
                {
                    DownloadResult result = await download();
                    CollectionAssert.AreEqual(Contents, destination == "file" ? File.ReadAllBytes(Destination) : stream.ToArray());
                    Assert.AreEqual(Contents.Length, result.Size);
                    return;
                }
                InvalidResponseException error = await Assert.ThrowsExactlyAsync<InvalidResponseException>(() => download());

                StringAssert.Contains(error.Message, $"{Contents.Length} bytes Files.com stated");
                Assert.IsFalse(error.RequestMayHaveBeenProcessed);
                Assert.AreEqual(1, server.StorageRequests.Count, "Not fetched again.");
                Assert.AreEqual(1, server.ApiRequests.Count, "Not asked for again.");
                if (destination == "file")
                {
                    AssertNothingPublished();
                }
                else if (difference < 0)
                {
                    CollectionAssert.AreEqual(body, stream.ToArray(), "All that arrived, once.");
                }
                else
                {
                    Assert.IsTrue(stream.Length <= Contents.Length, $"Nothing past the stated size, not {stream.Length} bytes.");
                    CollectionAssert.AreEqual(Contents.Take((int)stream.Length).ToArray(), stream.ToArray());
                }
            }
        }

        // Only a size Files.com states, and that can be one, is checked: without it, or with a negative one, an answer
        // without a length is taken as it arrives. 0 is a size: a body of 0 bytes has it, and a body of any byte fails
        // before that byte is written.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("", 1000, true)]
        [DataRow("\"size\":null,", 1000, true)]
        [DataRow("\"size\":-1,", 1000, true)]
        [DataRow("\"size\":0,", 0, true)]
        [DataRow("\"size\":0,", 1000, false)]
        public async Task OnlyASizeFilesComStatesIsChecked(string size, int length, bool succeeds)
        {
            byte[] body = Contents.Take(length).ToArray();
            using (var server = new DownloadServer(Contents) { Bound = false })
            {
                server.ApiIntercept = (target, connection) => StateSizeAsync(server, connection, size);
                server.Intercept = async (request, connection) =>
                {
                    await SendWithoutLengthAsync(connection, body);
                    return true;
                };
                var stream = new MemoryStream();
                Task download = Client(server).Transfers.DownloadStreamAsync("remote.bin", stream, Parts());

                if (succeeds)
                {
                    await download;
                    CollectionAssert.AreEqual(body, stream.ToArray());
                    return;
                }
                await Assert.ThrowsExactlyAsync<InvalidResponseException>(() => download);
                Assert.AreEqual(0, stream.Length, "Nothing past the stated size is written.");
            }
        }

        // A length the answer declares comes before the size Files.com stated: the whole of a file that changed since is
        // that file, and an answer cut short of its declared length is a connection's failure, fetched again, even when
        // what arrived has the stated size.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("whole")]
        [DataRow("cut short")]
        public async Task ADeclaredLengthComesBeforeTheStatedSize(string answer)
        {
            using (var server = new DownloadServer(Contents) { Bound = false })
            {
                server.ApiIntercept = (target, connection) => StateSizeAsync(server, connection, "\"size\":5000,");
                server.Intercept = async (request, connection) =>
                {
                    if (answer == "whole")
                    {
                        return false;
                    }
                    await connection.SendAsync($"HTTP/1.1 200 OK\r\nContent-Length: {Contents.Length}\r\nContent-Type: application/octet-stream\r\n\r\n");
                    await connection.SendAsync(Contents.Take(5000).ToArray());
                    connection.Dispose();
                    return true;
                };
                FilesClient client = Client(server, retries: 1);

                if (answer == "whole")
                {
                    await client.Transfers.DownloadFileAsync("remote.bin", Destination, Parts());
                    CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                    return;
                }
                Exception error = await Assert.ThrowsAsync<Exception>(() => client.Transfers.DownloadFileAsync("remote.bin", Destination, Parts()));
                Assert.AreEqual(typeof(ApiConnectionException), error.GetType(), error.ToString());
                Assert.AreEqual(2, server.StorageRequests.Count, "Fetched again, then given up.");
                AssertNothingPublished();
            }
        }

        // The size Files.com stated goes with the URL it came with: when storage refuses that URL as expired, the size
        // stated with the new one replaces it, and when none is stated with the new one, no size is checked.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("replaced")]
        [DataRow("cleared")]
        public async Task ARenewedUrlsStatedSizeReplacesTheOldOne(string renewal)
        {
            using (var server = new DownloadServer(Contents) { Bound = false })
            {
                int answers = 0;
                server.ApiIntercept = (target, connection) => StateSizeAsync(server, connection,
                    Interlocked.Increment(ref answers) == 1 ? "\"size\":5000," : renewal == "replaced" ? $"\"size\":{Contents.Length}," : "");
                bool refused = false;
                server.Intercept = async (request, connection) =>
                {
                    if (!refused)
                    {
                        refused = true;
                        await connection.RespondAsync(403, "<Error><Code>AuthenticationFailed</Code><Message>Signature not valid in the specified time frame</Message></Error>");
                        return true;
                    }
                    await SendWithoutLengthAsync(connection, Contents);
                    return true;
                };
                var stream = new MemoryStream();

                await Client(server).Transfers.DownloadStreamAsync("remote.bin", stream, Parts());

                CollectionAssert.AreEqual(Contents, stream.ToArray());
                Assert.AreEqual(2, server.ApiRequests.Count, "The URL was renewed once.");
                Assert.AreEqual(2, server.StorageRequests.Count);
            }
        }

        // The stage is private until the file is published with the permissions a new file gets; a symbolic link at the
        // destination is replaced, and its target left alone.
        [TestMethod]
        [Timeout(60000)]
        public async Task TheStageIsPrivateAndPublishingReplacesTheNameItself()
        {
            if (OperatingSystem.IsWindows())
            {
                return;
            }
            string outside = Path.Combine(folder, "outside");
            Directory.CreateDirectory(outside);
            string target = Path.Combine(outside, "target.bin");
            File.WriteAllText(target, "untouched");
            File.CreateSymbolicLink(Destination, target);
            using (var server = new DownloadServer(Contents))
            {
                UnixFileMode? stagingMode = null;
                UnixFileMode? stageMode = null;
                server.Intercept = (request, connection) =>
                {
                    if (!OperatingSystem.IsWindows())
                    {
                        string staging = Directory.GetDirectories(folder, ".files-download-*").Single();
                        stagingMode = File.GetUnixFileMode(staging);
                        stageMode = File.GetUnixFileMode(Directory.GetFiles(staging).Single());
                    }
                    return Task.FromResult(false);
                };

                await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Parts(concurrency: 1));

                Assert.AreEqual(UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute, stagingMode);
                Assert.AreEqual(UnixFileMode.UserRead | UnixFileMode.UserWrite, stageMode);
                Assert.IsNull(new FileInfo(Destination).LinkTarget, "The link was replaced by the file.");
                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual("untouched", File.ReadAllText(target));
                Assert.AreEqual(NewFileMode(), File.GetUnixFileMode(Destination));
            }
        }

        // Another program renaming entries in the destination directory while the download runs cannot redirect it:
        // with the staging directory and the destination directory both swapped for links to elsewhere, the download
        // still writes its own stage, and nothing reaches elsewhere. The path it was given no longer leads to the
        // directory it opened, so it puts the file nowhere, rather than where that path does not say.
        [TestMethod]
        [Timeout(60000)]
        public async Task RenamesDuringTheDownloadCannotRedirectIt()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && !RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return;
            }
            string chosen = Path.Combine(folder, "chosen");
            string moved = Path.Combine(folder, "moved");
            string elsewhere = Path.Combine(folder, "elsewhere");
            Directory.CreateDirectory(chosen);
            Directory.CreateDirectory(elsewhere);
            using (var server = new DownloadServer(Contents))
            {
                int swapped = 0;
                server.Intercept = (request, connection) =>
                {
                    if (request.Range?.First == 3 * 16384 && Interlocked.Increment(ref swapped) == 1)
                    {
                        string staging = Directory.GetDirectories(chosen, ".files-download-*").Single();
                        string stagingName = Path.GetFileName(staging);
                        // The staging directory is renamed away and replaced by a link to elsewhere; then the chosen
                        // directory itself is, too.
                        Directory.Move(staging, Path.Combine(chosen, "renamed-staging"));
                        Directory.CreateSymbolicLink(staging, elsewhere);
                        Directory.Move(chosen, moved);
                        Directory.CreateSymbolicLink(chosen, elsewhere);
                    }
                    return Task.FromResult(false);
                };

                DownloadPublicationException error = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Path.Combine(chosen, "file.bin"), Parts(concurrency: 1)));

                Assert.AreEqual(DownloadPublicationProblem.DestinationMoved, error.Problem);
                Assert.IsFalse(error.Placed);
                Assert.IsFalse(File.Exists(Path.Combine(moved, "file.bin")), "Nothing is published into the directory the download opened, which that path no longer leads to.");
                Assert.AreEqual(0, Directory.GetFileSystemEntries(elsewhere).Length, "Nothing was written elsewhere.");
                Assert.AreEqual(0, Directory.GetFileSystemEntries(Path.Combine(moved, "renamed-staging")).Length, "The stage was removed, through the staging directory the download holds.");
            }
        }

        // Cancelling a download with a checkpoint pauses it: its stage and the parts it wrote are kept, and running it
        // again, once storage has confirmed the version's entity tag for the file's first byte, fetches only the parts it
        // did not write, as the same version, then publishes and forgets the checkpoint.
        [TestMethod]
        [Timeout(60000)]
        public async Task ACancelledDownloadContinuesFromItsCheckpoint()
        {
            string checkpoint = Path.Combine(folder, "download.checkpoint");
            using (var server = new DownloadServer(Contents))
            {
                var cancellation = new CancellationTokenSource();
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range?.First == 3 * 16384 && !cancellation.IsCancellationRequested)
                    {
                        cancellation.Cancel();
                        await connection.HoldAsync();
                        return true;
                    }
                    return false;
                };
                var options = new DownloadOptions { PartSize = 16 * 1024, MaxConcurrency = 1, CheckpointPath = checkpoint };

                await Assert.ThrowsAsync<OperationCanceledException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options, cancellation.Token));

                Assert.IsFalse(File.Exists(Destination));
                Assert.IsTrue(File.Exists(checkpoint));
                string recorded = File.ReadAllText(checkpoint);
                Assert.IsFalse(recorded.Contains("download-key") || recorded.Contains("Signature") || recorded.Contains("/storage/") || recorded.Contains(DownloadServer.StorageHost), "No credentials or signed URLs: " + recorded);
                StringAssert.Contains(recorded, "etag-1");
                int before = server.StorageRequests.Count;

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options);

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual(3 * 16384, result.ResumedBytes);
                List<StorageRequest> continued = server.StorageRequests.Skip(before).ToList();
                Assert.AreEqual("GET 0-0", continued[0].ToString(), "The version is confirmed first.");
                CollectionAssert.AreEquivalent(Enumerable.Range(3, 4).Select(i => $"GET {i * 16384}-{Math.Min(i * 16384 + 16383, Contents.Length - 1)}").ToList(),
                    continued.Skip(1).Select(request => request.ToString()).ToList(), "Only the parts not yet written.");
                Assert.IsTrue(continued.All(request => request.IfMatch == server.ETag), "Every request carries the version's entity tag.");
                Assert.IsFalse(File.Exists(checkpoint));
                AssertNoStage();
            }
        }

        // A download whose process is killed while it waits for a part keeps what its checkpoint records: run again, by
        // another process, it confirms the version, fetches only the parts the checkpoint does not record, and publishes
        // the file whole. A running download records its progress at most every two seconds (CheckpointInterval), so
        // the third part is answered later than that: the checkpoint then records the first three parts before the
        // fourth is asked for. The killed process is this test's own child, still running, killed through its own handle.
        [TestMethod]
        [Timeout(60000)]
        public async Task ADownloadWhoseProcessIsKilledContinuesFromItsCheckpoint()
        {
            string checkpoint = Path.Combine(folder, "download.checkpoint");
            using (var server = new DownloadServer(Contents))
            {
                var waiting = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                int thirdAnswered = 0;
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range?.First == 2 * 16384 && Interlocked.Exchange(ref thirdAnswered, 1) == 0)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(2.5));
                        return false;
                    }
                    if (request.Range?.First == 3 * 16384 && waiting.TrySetResult(true))
                    {
                        await connection.HoldAsync();
                        return true;
                    }
                    return false;
                };
                FilesConfiguration site = server.Configuration();
                var start = new ProcessStartInfo(Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? Environment.ProcessPath)
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };
                foreach (string argument in new[] { "exec", typeof(WriterProcess).Assembly.Location, "download-writer", site.BaseUrl, site.ApiKey, server.Port.ToString(), Destination, checkpoint, (16 * 1024).ToString() })
                {
                    start.ArgumentList.Add(argument);
                }
                Process writer = Process.Start(start);
                Task<string> output = writer.StandardOutput.ReadToEndAsync();
                Task<string> errors = writer.StandardError.ReadToEndAsync();
                string ended;
                try
                {
                    Console.WriteLine($"Started writer process {writer.Id}");
                    // The writer asks for its fourth part within seconds; the test waits for that, or for the writer to
                    // exit, no longer than WriterDeadline.
                    Task exited = writer.WaitForExitAsync();
                    Task first = await Task.WhenAny(waiting.Task, exited, Task.Delay(WriterDeadline));
                    if (first == exited)
                    {
                        Assert.Fail($"The writer exited with {writer.ExitCode} before it asked for its fourth part.");
                    }
                    if (first != waiting.Task)
                    {
                        Assert.Fail($"The writer neither asked for its fourth part nor exited within {WriterDeadline.TotalSeconds} s.");
                    }
                    Assert.IsFalse(writer.HasExited, "The writer is still waiting for its fourth part.");
                    writer.Kill();
                }
                finally
                {
                    ended = await EndWriterAsync(writer, output, errors);
                    Console.WriteLine(ended);
                }
                Assert.IsTrue(ended.StartsWith("Ended", StringComparison.Ordinal), ended);

                Assert.IsFalse(File.Exists(Destination), "The killed writer published nothing.");
                Assert.IsTrue(File.Exists(checkpoint), "The killed writer's checkpoint remains.");
                StringAssert.Contains(File.ReadAllText(checkpoint), "etag-1");
                int before = server.StorageRequests.Count;

                var options = new DownloadOptions { PartSize = 16 * 1024, MaxConcurrency = 1, CheckpointPath = checkpoint };
                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options);

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual(3 * 16384, result.ResumedBytes, "The three parts the killed writer's checkpoint records are kept.");
                List<StorageRequest> continued = server.StorageRequests.Skip(before).ToList();
                Assert.AreEqual("GET 0-0", continued[0].ToString(), "The version is confirmed first.");
                CollectionAssert.AreEquivalent(Enumerable.Range(3, 4).Select(i => $"GET {i * 16384}-{Math.Min(i * 16384 + 16383, Contents.Length - 1)}").ToList(),
                    continued.Skip(1).Select(request => request.ToString()).ToList(), "Only the parts the checkpoint does not record.");
                Assert.IsTrue(continued.All(request => request.IfMatch == server.ETag), "Every request carries the version's entity tag.");
                Assert.IsFalse(File.Exists(checkpoint));
                AssertNoStage();
            }
        }

        // How long a test waits for its writer process to reach a point, or to end once killed.
        private static readonly TimeSpan WriterDeadline = TimeSpan.FromSeconds(20);

        // Ends a writer process a test started, through the handle Process.Start returned and no other: kills it if it is
        // still running, waits at most WriterDeadline for it to exit and its output to end, and disposes the handle.
        // Returns what happened, starting "Ended" if the process exited, and never throws, so that a test's own failure
        // is what it reports.
        private static async Task<string> EndWriterAsync(Process writer, Task<string> output, Task<string> errors)
        {
            int id = -1;
            try
            {
                id = writer.Id;
                try
                {
                    if (!writer.HasExited)
                    {
                        writer.Kill();
                    }
                }
                catch (InvalidOperationException)
                {
                    // It exited in the meantime.
                }
                Task settled = Task.WhenAll(writer.WaitForExitAsync(), output, errors);
                if (await Task.WhenAny(settled, Task.Delay(WriterDeadline)) != settled)
                {
                    return $"Writer process {id} did not exit and close its output within {WriterDeadline.TotalSeconds} s.";
                }
                return $"Ended writer process {id} with exit code {writer.ExitCode}. Output: {output.Result} {errors.Result}";
            }
            catch (Exception e)
            {
                return $"Ending writer process {id} failed: {e}";
            }
            finally
            {
                writer.Dispose();
            }
        }

        // Continuing a download from its checkpoint first gets a new URL from Files.com, then has storage confirm the
        // version it records, for the file's first byte. Stopped before that answer, by cancellation, a connection that
        // fails, an error answer, or a refusal of access, from Files.com or from storage, the download has learned
        // nothing of the version: its checkpoint and stage stay exactly as they were, nothing else is fetched, and the
        // next run confirms the version and fetches only the parts not yet written.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("cancelled")]
        [DataRow("connection fails")]
        [DataRow("error answer")]
        [DataRow("access refused")]
        [DataRow("site error answer")]
        public async Task AContinuedDownloadStoppedAsItsVersionIsConfirmedKeepsWhatItHad(string stop)
        {
            string checkpoint = Path.Combine(folder, "download.checkpoint");
            using (var server = new DownloadServer(Contents))
            {
                var first = new CancellationTokenSource();
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range?.First == 3 * 16384 && !first.IsCancellationRequested)
                    {
                        first.Cancel();
                        await connection.HoldAsync();
                        return true;
                    }
                    return false;
                };
                var options = new DownloadOptions { PartSize = 16 * 1024, MaxConcurrency = 1, CheckpointPath = checkpoint };
                await Assert.ThrowsAsync<OperationCanceledException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options, first.Token));
                string recorded = File.ReadAllText(checkpoint);
                string stage = Directory.GetDirectories(folder, ".files-download-*").SelectMany(Directory.GetFiles).Single();
                byte[] staged = File.ReadAllBytes(stage);
                Assert.AreEqual(3 * 16384, staged.Length, "Three parts were written.");
                int storageBefore = server.StorageRequests.Count;
                var second = new CancellationTokenSource();
                async Task<bool> StopAsync(ScriptedConnection connection)
                {
                    if (stop == "cancelled")
                    {
                        second.Cancel();
                        await connection.HoldAsync();
                    }
                    else if (stop == "connection fails")
                    {
                        connection.Dispose();
                    }
                    else if (stop == "access refused")
                    {
                        await connection.RespondAsync(403, "<Error><Code>AccessDenied</Code><Message>Access Denied</Message></Error>");
                    }
                    else if (stop == "site error answer")
                    {
                        await connection.RespondAsync(503, "{\"error\":\"Service Unavailable\",\"http-code\":503}");
                    }
                    else
                    {
                        await connection.RespondAsync(503, "<Error><Code>SlowDown</Code></Error>");
                    }
                    return true;
                }
                if (stop == "site error answer")
                {
                    server.ApiIntercept = (target, connection) => StopAsync(connection);
                }
                else
                {
                    server.Intercept = (request, connection) => request.Range?.First == 0 && request.Range?.Last == 0 ? StopAsync(connection) : Task.FromResult(false);
                }

                Exception error = await Assert.ThrowsAsync<Exception>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options, second.Token));

                Assert.IsFalse(error is DownloadSourceChangedException, error.ToString());
                Assert.IsTrue(stop == "cancelled" ? error is OperationCanceledException : stop == "connection fails" ? error is ApiConnectionException : error is SdkException, error.ToString());
                Assert.AreEqual(recorded, File.ReadAllText(checkpoint), "The checkpoint is left as it was.");
                CollectionAssert.AreEqual(staged, File.ReadAllBytes(stage), "The stage is left as it was, where it was.");
                Assert.IsTrue(server.StorageRequests.Skip(storageBefore).All(request => request.ToString() == "GET 0-0"), "Nothing but the confirmation is asked for under a version not confirmed.");
                Assert.IsFalse(File.Exists(Destination));

                server.ApiIntercept = null;
                server.Intercept = null;
                int before = server.StorageRequests.Count;
                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options);

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual(3 * 16384, result.ResumedBytes);
                CollectionAssert.AreEquivalent(new[] { "GET 0-0" }.Concat(Enumerable.Range(3, 4).Select(i => $"GET {i * 16384}-{Math.Min(i * 16384 + 16383, Contents.Length - 1)}")).ToList(),
                    server.StorageRequests.Skip(before).Select(request => request.ToString()).ToList(), "The confirmation, then only the parts not yet written.");
                Assert.IsFalse(File.Exists(checkpoint));
                AssertNoStage();
            }
        }

        // Storage that ignores Range since a download was stopped still confirms its version, by answering with the whole
        // file under the version's entity tag and size; the download then fetches the file whole, as the version, from
        // its first byte, and never writes an answer to a request for a range where that range belongs.
        [TestMethod]
        [Timeout(60000)]
        public async Task AContinuedDownloadOfStorageThatNowIgnoresRangeComesWholeAsTheVersion()
        {
            string checkpoint = Path.Combine(folder, "download.checkpoint");
            using (var server = new DownloadServer(Contents))
            {
                var cancellation = new CancellationTokenSource();
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range?.First == 3 * 16384 && !cancellation.IsCancellationRequested)
                    {
                        cancellation.Cancel();
                        await connection.HoldAsync();
                        return true;
                    }
                    return false;
                };
                var options = new DownloadOptions { PartSize = 16 * 1024, MaxConcurrency = 1, CheckpointPath = checkpoint };
                await Assert.ThrowsAsync<OperationCanceledException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options, cancellation.Token));
                server.Intercept = null;
                server.IgnoresRange = true;
                int before = server.StorageRequests.Count;

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options);

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.IsTrue(result.VersionBound);
                List<StorageRequest> continued = server.StorageRequests.Skip(before).ToList();
                CollectionAssert.AreEqual(new[] { "GET 0-0", $"GET {3 * 16384}-{4 * 16384 - 1}", "GET whole" }, continued.Select(request => request.ToString()).ToList(), "Confirmed, then a part answered whole, then the whole file.");
                Assert.IsTrue(continued.All(request => request.IfMatch == server.ETag));
                Assert.IsFalse(File.Exists(checkpoint));
                AssertNoStage();
            }
        }

        // Continuing from a checkpoint, a confirmation that storage refuses as expired, though its URL is new, is sent
        // again with a renewed URL; once confirmed, only the parts not yet written are fetched.
        [TestMethod]
        [Timeout(60000)]
        public async Task AConfirmationRefusedAsExpiredIsSentAgainWithARenewedUrl()
        {
            string checkpoint = Path.Combine(folder, "download.checkpoint");
            using (var server = new DownloadServer(Contents))
            {
                var cancellation = new CancellationTokenSource();
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range?.First == 3 * 16384 && !cancellation.IsCancellationRequested)
                    {
                        cancellation.Cancel();
                        await connection.HoldAsync();
                        return true;
                    }
                    return false;
                };
                var options = new DownloadOptions { PartSize = 16 * 1024, MaxConcurrency = 1, CheckpointPath = checkpoint };
                await Assert.ThrowsAsync<OperationCanceledException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options, cancellation.Token));
                bool refused = false;
                server.Intercept = async (request, connection) =>
                {
                    if (refused || request.Range != (0, 0))
                    {
                        return false;
                    }
                    refused = true;
                    await connection.RespondAsync(403, "<Error><Code>AccessDenied</Code><Message>Request has expired</Message></Error>");
                    return true;
                };
                int apiBefore = server.ApiRequests.Count;
                int before = server.StorageRequests.Count;

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options);

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual(3 * 16384, result.ResumedBytes);
                Assert.AreEqual(2, server.ApiRequests.Count - apiBefore, "A URL for the run, then one renewal.");
                CollectionAssert.AreEqual(new[] { "GET 0-0", "GET 0-0" }.Concat(Enumerable.Range(3, 4).Select(i => $"GET {i * 16384}-{Math.Min(i * 16384 + 16383, Contents.Length - 1)}")).ToList(),
                    server.StorageRequests.Skip(before).Select(request => request.ToString()).ToList());
            }
        }

        // When the version a checkpoint records is gone, as storage refuses its entity tag (412), or Files.com now gives
        // its URL for another object, even one with the same entity tag and size, the download starts over as a download
        // of the file as it is now, in its own stage, and never puts the old version's parts together with the new one's.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("version gone")]
        [DataRow("version gone, If-Match ignored")]
        [DataRow("another object, same entity tag")]
        [DataRow("stage gone")]
        public async Task ADownloadWhoseCheckpointCannotBeContinuedStartsOver(string lost)
        {
            string checkpoint = Path.Combine(folder, "download.checkpoint");
            using (var server = new DownloadServer(Contents))
            {
                var cancellation = new CancellationTokenSource();
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range?.First == 3 * 16384 && !cancellation.IsCancellationRequested)
                    {
                        cancellation.Cancel();
                        await connection.HoldAsync();
                        return true;
                    }
                    return false;
                };
                var options = new DownloadOptions { PartSize = 16 * 1024, MaxConcurrency = 1, CheckpointPath = checkpoint };
                await Assert.ThrowsAsync<OperationCanceledException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options, cancellation.Token));
                byte[] changed = Contents.Select(b => (byte)(b + 1)).ToArray();
                if (lost.StartsWith("version gone", StringComparison.Ordinal))
                {
                    server.Change(changed);
                    server.IgnoresIfMatch = lost.EndsWith("ignored", StringComparison.Ordinal);
                }
                else if (lost == "another object, same entity tag")
                {
                    server.Relocate(contents: changed, keepETag: true);
                }
                else
                {
                    Directory.Delete(Directory.GetDirectories(folder, ".files-download-*").Single(), true);
                }
                int before = server.StorageRequests.Count;

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options);

                CollectionAssert.AreEqual(lost == "stage gone" ? Contents : changed, File.ReadAllBytes(Destination));
                Assert.AreEqual(0, result.ResumedBytes);
                List<string> again = server.StorageRequests.Skip(before).Select(request => request.ToString()).ToList();
                CollectionAssert.AreEqual(lost.StartsWith("version gone", StringComparison.Ordinal) ? new[] { "GET 0-0" } : new string[0], again.Take(again.Count - 7).ToArray(), "Only a refused confirmation before the parts.");
                Assert.AreEqual(lost == "stage gone" ? 0 : 1, result.FinalProgress.Restarts);
                AssertNoStage();
            }
        }

        // A checkpoint is used only by the transfer it records.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("another remote path")]
        [DataRow("another local path")]
        [DataRow("another workspace")]
        [DataRow("not a checkpoint")]
        public async Task ACheckpointOfAnotherTransferIsRefused(string other)
        {
            string checkpoint = Path.Combine(folder, "download.checkpoint");
            using (var server = new DownloadServer(Contents))
            {
                var cancellation = new CancellationTokenSource();
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range?.First == 16384 && !cancellation.IsCancellationRequested)
                    {
                        cancellation.Cancel();
                        await connection.HoldAsync();
                        return true;
                    }
                    return false;
                };
                var options = new DownloadOptions { PartSize = 16 * 1024, MaxConcurrency = 1, CheckpointPath = checkpoint };
                await Assert.ThrowsAsync<OperationCanceledException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options, cancellation.Token));
                if (other == "not a checkpoint")
                {
                    File.WriteAllText(checkpoint, "{\"format\":\"something else\"}");
                }
                FilesConfiguration configuration = server.Configuration();
                if (other == "another workspace")
                {
                    configuration.WorkspaceId = "7";
                }
                int apiBefore = server.ApiRequests.Count;

                TransferCheckpointException error = await Assert.ThrowsExactlyAsync<TransferCheckpointException>(() => server.Client(configuration).Transfers.DownloadFileAsync(
                    other == "another remote path" ? "other.bin" : "remote.bin",
                    other == "another local path" ? Path.Combine(folder, "other.bin") : Destination,
                    options));

                Assert.AreEqual(checkpoint, error.CheckpointPath);
                Assert.AreEqual(apiBefore, server.ApiRequests.Count, "Nothing was sent.");
                Assert.IsFalse(File.Exists(Destination) || File.Exists(Path.Combine(folder, "other.bin")));
            }
            InvalidParameterException refused = await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => FilesClient.Create(new FilesConfiguration { ApiKey = "k" }).Transfers.DownloadStreamAsync("remote.bin", new MemoryStream(), new DownloadOptions { CheckpointPath = checkpoint }));
            Assert.AreEqual(nameof(DownloadOptions.CheckpointPath), refused.ParamName);
        }

        private void AssertNoStage()
        {
            CollectionAssert.AreEqual(new string[0], Directory.GetDirectories(folder, ".files-download-*"), "No stage is left behind.");
        }

        private void AssertNothingPublished()
        {
            Assert.IsFalse(File.Exists(Destination), "Nothing was published.");
            CollectionAssert.AreEqual(new string[0], Directory.GetDirectories(folder, ".files-download-*"), "The stage was removed.");
        }

        private async Task<byte[]> Download(DownloadServer server, string destination, DownloadOptions options, Action<DownloadResult> result = null)
        {
            if (destination == "file")
            {
                DownloadResult done = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options);
                result?.Invoke(done);
                return File.ReadAllBytes(Destination);
            }
            var stream = new MemoryStream();
            DownloadResult streamed = await Client(server).Transfers.DownloadStreamAsync("remote.bin", stream, options);
            result?.Invoke(streamed);
            return stream.ToArray();
        }

        // Downloads remote.bin to Destination or to a stream, in parts of 16 KiB, with two retries, and returns what arrived.
        private async Task<(DownloadResult Result, byte[] Downloaded)> DownloadWithRetries(DownloadServer server, string destination)
        {
            FilesClient client = Client(server, retries: 2);
            if (destination == "file")
            {
                DownloadResult result = await client.Transfers.DownloadFileAsync("remote.bin", Destination, Parts());
                return (result, File.ReadAllBytes(Destination));
            }
            var stream = new MemoryStream();
            DownloadResult streamed = await client.Transfers.DownloadStreamAsync("remote.bin", stream, Parts());
            return (streamed, stream.ToArray());
        }

        private static FilesClient Client(DownloadServer server, int retries = 0)
        {
            return server.Client(retries);
        }

        // Answers with body as 200, declaring no length, and ends it by closing the connection.
        private static async Task SendWithoutLengthAsync(ScriptedConnection connection, byte[] body)
        {
            await connection.SendAsync("HTTP/1.1 200 OK\r\nContent-Type: application/octet-stream\r\n\r\n");
            await connection.SendAsync(body);
            connection.Dispose();
        }

        // Answers a download's details as Files.com does, with a URL of server's storage, stating the file's size as size
        // says: a JSON member and its comma, or nothing.
        private static async Task<bool> StateSizeAsync(DownloadServer server, ScriptedConnection connection, string size)
        {
            await connection.RespondAsync(200, $"{{\"path\":\"remote.bin\",\"type\":\"file\",{size}\"download_uri\":\"{server.StorageUrl}/storage/1/remote.bin?X-Files-Signature=stand-in\"}}");
            return true;
        }

        // The permissions a new file gets here: 0666 less the umask.
        [System.Runtime.Versioning.UnsupportedOSPlatform("windows")]
        private UnixFileMode NewFileMode()
        {
            string probe = Path.Combine(folder, "probe");
            File.WriteAllText(probe, "");
            UnixFileMode mode = File.GetUnixFileMode(probe);
            File.Delete(probe);
            return mode;
        }

        private sealed class FailingStream : Stream
        {
            private readonly Exception failure;

            internal FailingStream(Exception failure)
            {
                this.failure = failure;
            }

            public int Writes { get; private set; }

            public override bool CanRead => false;

            public override bool CanSeek => false;

            public override bool CanWrite => true;

            public override long Length => throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                Writes++;
                throw failure;
            }

            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                Writes++;
                throw failure;
            }

            public override void Flush()
            {
            }

            public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

            public override void SetLength(long value) => throw new NotSupportedException();
        }
    }
}