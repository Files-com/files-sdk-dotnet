using FilesCom;
using FilesCom.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WireMock;

namespace FilesTests
{
    // Files sent as file parameters (FileAttachment): every attempt and every redirect that repeats the request sends
    // the same bytes, of the length the request states; a file that cannot be read, or has changed, fails the operation
    // as it is, without another attempt; what the operation opened is released when it ends, and a caller's stream is
    // left open. What the API decodes from these bodies is checked against its own parser by the attachment journeys.
    [TestClass]
    public class AttachmentTest
    {
        private static readonly byte[] Contents = Enumerable.Range(0, 70000).Select(i => (byte)(i * 7)).ToArray();
        private LoopbackSite site;
        private string folder;

        [TestInitialize]
        public void Start()
        {
            site = new LoopbackSite("key-a", "11");
            folder = Path.Combine(Path.GetTempPath(), "attachment-test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
        }

        [TestCleanup]
        public void Stop()
        {
            site.Dispose();
            Directory.Delete(folder, true);
        }

        // Each redirect sends the whole body again, read again from its file or stream. A move to the site's region,
        // the retry a change with files can have after it was sent, does too: RegionRelocationTest.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("bytes", "avatar.png", "multipart/form-data")]
        [DataRow("file", "avatar.png", "multipart/form-data")]
        [DataRow("stream", "avatar.png", "multipart/form-data")]
        [DataRow("forward-only stream", "avatar.png", "multipart/form-data")]
        [DataRow("bytes", "100%.png", "application/json")]
        [DataRow("file", "100%.png", "application/json")]
        [DataRow("stream", "100%.png", "application/json")]
        [DataRow("forward-only stream", "100%.png", "application/json")]
        public async Task RedirectsSendTheSameBytes(string kind, string fileName, string contentType)
        {
            site.On("PATCH", "/users/1", request => Redirect(307, "/users/moved"));
            site.On("PATCH", "/users/moved", request => Redirect(308, "/users/final"));
            site.On("PATCH", "/users/final", request => LoopbackSite.Json("{\"id\":1}"));
            FilesClient client = FilesClient.Create(site.Configuration(maxNetworkRetries: 1));

            await client.Users.UpdateAsync(1, new Dictionary<string, object>
            {
                { "avatar_file", Attachment(kind, fileName) },
                { "notes", "" },
                { "disabled", false },
            });

            ReceivedRequest[] patches = site.Requests.Where(request => request.Method == "PATCH").ToArray();
            CollectionAssert.AreEqual(new[] { "/users/1", "/users/moved", "/users/final" }, patches.Select(request => request.Path).ToArray());
            foreach (ReceivedRequest patch in patches)
            {
                CollectionAssert.AreEqual(patches[0].BodyBytes, patch.BodyBytes, $"The body sent to {patch.Path} differs.");
                Assert.AreEqual(patch.BodyBytes.Length.ToString(), patch.Headers["Content-Length"]);
                StringAssert.StartsWith(patch.Headers["Content-Type"], contentType);
                Assert.AreEqual("key-a", patch.ApiKey);
            }
            CollectionAssert.AreEqual(Contents.Skip(kind == "stream" ? 3 : 0).ToArray(), FileIn(patches[0]));
        }

        // A stream that can seek is read as the request is sent, and fails halfway through it, in a form or in JSON; one
        // that cannot is copied first, and fails before anything is sent. Either way its exception is its own, whatever
        // its type: an InvalidOperationException is not taken for a request that cannot be sent, nor for a limit, and an
        // HttpRequestException, or an OperationCanceledException for a timeout, as a stream read from the network may
        // throw, is not taken for a connection failure or timeout of the request, which would be retried.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("avatar.png", true, "io")]
        [DataRow("100%.png", true, "io")]
        [DataRow("avatar.png", false, "io")]
        [DataRow("avatar.png", true, "invalid operation")]
        [DataRow("100%.png", false, "invalid operation")]
        [DataRow("avatar.png", true, "http request")]
        [DataRow("100%.png", true, "http request")]
        [DataRow("avatar.png", false, "http request")]
        [DataRow("avatar.png", true, "source timeout")]
        [DataRow("100%.png", true, "source timeout")]
        [DataRow("avatar.png", false, "source timeout")]
        public async Task AFileThatCannotBeReadFailsAsItIsWithoutAnotherAttempt(string fileName, bool seekable, string failure)
        {
            site.On("PATCH", "/users/1", request => LoopbackSite.Json("{\"id\":1}"));
            FilesClient client = FilesClient.Create(site.Configuration(maxNetworkRetries: 3));
            Exception unreadable = SourceFailure(failure);
            var stream = new FailingAfter(Contents, 40000, unreadable, seekable);

            Exception error = null;
            try
            {
                await client.Users.UpdateAsync(1, new Dictionary<string, object>
                {
                    { "avatar_file", FileAttachment.FromStream(stream, fileName) },
                });
            }
            catch (Exception e)
            {
                error = e;
            }

            Assert.AreSame(unreadable, error);
            Assert.AreEqual(1, stream.Passes, "The request was not sent again.");
            Assert.IsFalse(stream.Disposed, "The stream is the caller's, and stays open.");
            Assert.AreEqual(0, site.Requests.Count, "No request was received whole.");
        }

        // A stream that fails when it is measured, as the operation starts or as the retry after a first attempt that
        // could not connect checks it is unchanged: its exception, as it is, and no further attempt.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("opening", 0, 0)]
        [DataRow("checking before a retry", 2, 1)]
        public async Task AStreamThatCannotBeMeasuredFailsAsItIsWithoutAnotherAttempt(string when, int measuredBefore, int attempts)
        {
            site.On("PATCH", "/users/1", request => LoopbackSite.Json("{\"id\":1}"));
            int sent = 0;
            // Every attempt fails to connect, before anything is sent.
            HttpTransport transport = HttpTransport.FromHandlerFactory(() => new RetryTest.FirstConnectionRefused(() => Interlocked.Increment(ref sent) > 0, asConnectionError: true));
            FilesClient client = FilesClient.Create(site.Configuration(maxNetworkRetries: 3), transport);
            Exception unmeasurable = SourceFailure("http request");
            var stream = new FailingAfter(Contents, int.MaxValue, null, seekable: true) { LengthFailure = unmeasurable, LengthReadsBeforeFailure = measuredBefore };

            Exception error = null;
            try
            {
                await client.Users.UpdateAsync(1, new Dictionary<string, object> { { "avatar_file", FileAttachment.FromStream(stream, "avatar.png") } });
            }
            catch (Exception e)
            {
                error = e;
            }

            Assert.AreSame(unmeasurable, error, when);
            Assert.AreEqual(attempts, Volatile.Read(ref sent), when);
            Assert.AreEqual(0, site.Requests.Count, when);
        }

        // The control: cancelling the operation while a file is read as the request is sent stops it as cancellation,
        // without another attempt, and leaves the caller's stream open.
        [TestMethod]
        [Timeout(60000)]
        public async Task CancellingWhileAFileIsReadStopsTheOperation()
        {
            site.On("PATCH", "/users/1", request => LoopbackSite.Json("{\"id\":1}"));
            FilesClient client = FilesClient.Create(site.Configuration(maxNetworkRetries: 3));
            var stream = new FailingAfter(Contents, 40000, null, seekable: true) { Stalled = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously) };
            var cancellation = new CancellationTokenSource();

            Task update = client.Users.UpdateAsync(1, new Dictionary<string, object> { { "avatar_file", FileAttachment.FromStream(stream, "avatar.png") } }, cancellationToken: cancellation.Token);
            await stream.Stalled.Task;
            cancellation.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(() => update);
            Assert.AreEqual(1, stream.Passes, "The request was not sent again.");
            Assert.IsFalse(stream.Disposed, "The stream is the caller's, and stays open.");
            Assert.AreEqual(0, site.Requests.Count, "No request was received whole.");
        }

        // A request whose whole body arrived, and that was answered 503, or whose answer was lost, may have taken effect,
        // so it is sent once, with retries left, and says so; its file is read once. One shown not carried out, by a move
        // to the site's region, is sent again with the same bytes: RegionRelocationTest.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(true, DisplayName = "Answered 503: sent once, and it may have taken effect")]
        [DataRow(false, DisplayName = "Its answer lost: sent once, and it may have taken effect")]
        public async Task AnAttachmentThatMayHaveTakenEffectIsSentOnce(bool answered)
        {
            var bodies = new List<byte[]>();
            using (var server = new ScriptedServer(async connection =>
            {
                await connection.ReadRequestHeadAsync();
                byte[] body = await connection.ReadBodyAsync();
                int number;
                lock (bodies)
                {
                    bodies.Add(body);
                    number = bodies.Count;
                }
                if (number == 1 && answered)
                {
                    // This handler answers one request: a retry would come on a new connection.
                    await connection.RespondAsync(503, "{\"error\":\"Unavailable\",\"http-code\":503}", "Connection: close\r\n");
                    connection.Dispose();
                    return;
                }
                if (number == 1)
                {
                    // Closed without an answer.
                    connection.Dispose();
                    return;
                }
                await connection.RespondAsync(200, "{\"id\":1}");
            }))
            {
                var configuration = new FilesConfiguration { BaseUrl = $"http://127.0.0.1:{server.Port}", ApiKey = "key-a", MaxNetworkRetries = 3, InitialNetworkRequestDelay = 0 };
                var stream = new FailingAfter(Contents, int.MaxValue, null, seekable: true);
                Task<User> update = FilesClient.Create(configuration).Users.UpdateAsync(1, new Dictionary<string, object>
                {
                    { "avatar_file", FileAttachment.FromStream(stream, "avatar.png") },
                });

                SdkException error = answered
                    ? await Assert.ThrowsExactlyAsync<ApiException>(() => update)
                    : await Assert.ThrowsExactlyAsync<ApiConnectionException>(() => update);
                Assert.IsTrue(error.RequestMayHaveBeenProcessed);
                Assert.AreEqual(1, bodies.Count, "The request was sent once.");
                Assert.AreEqual(1, stream.Passes);
            }
        }

        // A stream that changes length before a retry, here after a first attempt that could not connect, is not sent.
        [TestMethod]
        [Timeout(60000)]
        public async Task AStreamThatChangesLengthIsNotSentAgain()
        {
            var stream = new MemoryStream();
            stream.Write(Contents, 0, 1000);
            stream.Position = 0;
            site.On("PATCH", "/users/1", request => LoopbackSite.Json("{\"id\":1}"));
            int attempts = 0;
            HttpTransport transport = HttpTransport.FromHandlerFactory(() => new RetryTest.FirstConnectionRefused(() =>
            {
                if (Interlocked.Increment(ref attempts) > 1)
                {
                    return false;
                }
                stream.SetLength(2000);
                return true;
            }, asConnectionError: true));
            FilesClient client = FilesClient.Create(site.Configuration(maxNetworkRetries: 3), transport);

            IOException error = await Assert.ThrowsExactlyAsync<IOException>(() => client.Users.UpdateAsync(1, new Dictionary<string, object>
            {
                { "avatar_file", FileAttachment.FromStream(stream, "avatar.png") },
            }));

            StringAssert.Contains(error.Message, "avatar_file changed length");
            Assert.AreEqual(1, Volatile.Read(ref attempts), "The retry was not sent.");
            Assert.AreEqual(0, site.Requests.Count);
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(200)]
        [DataRow(422)]
        public async Task WhatTheOperationOpenedIsReleasedWhenItEnds(int status)
        {
            string path = Path.Combine(folder, "avatar.png");
            File.WriteAllBytes(path, Contents);
            string[] spoolsDuringRequest = null;
            string[] spoolsBefore = Spools();
            site.On("PATCH", "/users/1", request =>
            {
                spoolsDuringRequest = Spools().Except(spoolsBefore).ToArray();
                return status == 200 ? LoopbackSite.Json("{\"id\":1}") : LoopbackSite.Json("{\"error\":\"Invalid\",\"http-code\":422,\"type\":\"processing-failure/model-save-error\"}", status: 422);
            });
            FilesClient client = FilesClient.Create(site.Configuration());
            var borrowed = new MemoryStream(Contents);

            Task update = client.Users.UpdateAsync(1, new Dictionary<string, object>
            {
                { "avatar_file", FileAttachment.FromFile(path) },
                { "bundle_watermark_attachment_file", FileAttachment.FromStream(new FailingAfter(Contents, int.MaxValue, null), "spooled.png") },
                { "logo_file", FileAttachment.FromStream(borrowed, "borrowed.png") },
            });
            if (status == 200)
            {
                await update;
            }
            else
            {
                await Assert.ThrowsExactlyAsync<ModelSaveErrorException>(() => update);
            }

            Assert.AreEqual(1, spoolsDuringRequest.Length, "The forward-only stream was copied to one temporary file.");
            Assert.IsFalse(File.Exists(spoolsDuringRequest[0]), "The temporary file was deleted.");
            using (new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                // The operation's handle is closed: nothing shares the file now.
            }
            Assert.IsTrue(borrowed.CanRead, "The caller's stream stays open.");
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task AFileThatCannotBeOpenedReleasesTheOnesOpenedBeforeIt()
        {
            string path = Path.Combine(folder, "icon.png");
            File.WriteAllBytes(path, Contents);
            FilesClient client = FilesClient.Create(site.Configuration());

            await Assert.ThrowsExactlyAsync<System.IO.FileNotFoundException>(() => client.Sites.UpdateAsync(new Dictionary<string, object>
            {
                { "icon16_file", FileAttachment.FromFile(path) },
                { "icon32_file", FileAttachment.FromFile(Path.Combine(folder, "missing.png")) },
            }));

            using (new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
            }
            Assert.AreEqual(0, site.Requests.Count);
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task AStreamLongerThanItsLimitSendsNothing()
        {
            FilesClient client = FilesClient.Create(site.Configuration());
            string[] spoolsBefore = Spools();

            InvalidParameterException error = await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => client.Users.UpdateAsync(1, new Dictionary<string, object>
            {
                { "avatar_file", FileAttachment.FromStream(new FailingAfter(Contents, int.MaxValue, null), "avatar.png", maxBytes: Contents.Length - 1) },
            }));

            StringAssert.Contains(error.Message, "avatar_file holds more than the 69999 bytes");
            Assert.AreEqual(0, site.Requests.Count);
            CollectionAssert.AreEquivalent(spoolsBefore, Spools(), "The partial copy was deleted.");
        }

        // A forward-only stream is copied to a temporary file that only this user can open from the moment it exists,
        // before a byte of the stream is in it, whatever the process's umask: on .NET 8 and 10 as FileStream creates it
        // (mode 0600), and as the netstandard2.0 build creates it on a system other than Windows (mkstemp,
        // TemporaryPartFile.CreatePrivate, taken here through its test switch). When the copy first reads the stream, the
        // file is there, empty and with mode 0600; the request sends the stream's bytes, and the file is deleted once the
        // operation ends.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(false, DisplayName = "As .NET 8 and 10 create it")]
        [DataRow(true, DisplayName = "As the netstandard2.0 build creates it")]
        [UnsupportedOSPlatform("windows")]
        public async Task AStreamCopiedForAFileParameterIsPrivateBeforeAnyOfItIsWritten(bool netstandard)
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("Files are made private by their Unix mode on systems other than Windows.");
            }
            site.On("PATCH", "/users/1", request => LoopbackSite.Json("{\"id\":1}"));
            FilesClient client = FilesClient.Create(site.Configuration());
            string[] spoolsBefore = Spools();
            var seen = new List<(string Path, long Length, UnixFileMode Mode)>();
            var stream = new ObservedStream(Contents, () =>
            {
                foreach (string spool in Spools().Except(spoolsBefore))
                {
                    seen.Add((spool, new FileInfo(spool).Length, File.GetUnixFileMode(spool)));
                }
            });

            FilesCom.Transfers.TemporaryPartFile.NetstandardCreationForTests.Value = netstandard;
            try
            {
                // Its media type is given, as FileIn finds the file part in the request by it.
                await client.Users.UpdateAsync(1, new Dictionary<string, object>
                {
                    { "avatar_file", FileAttachment.FromStream(stream, "avatar.png", contentType: "image/png") },
                });
            }
            finally
            {
                FilesCom.Transfers.TemporaryPartFile.NetstandardCreationForTests.Value = false;
            }

            Assert.AreEqual(1, seen.Count, "The stream was copied to one temporary file, there when the copy began.");
            Assert.AreEqual(0, seen[0].Length, "Nothing was written to it yet.");
            Assert.AreEqual(UnixFileMode.UserRead | UnixFileMode.UserWrite, seen[0].Mode, $"{seen[0].Path} is this user's alone from the start.");
            StringAssert.StartsWith(Path.GetFileName(seen[0].Path), "files-upload-");
            CollectionAssert.AreEqual(Contents, FileIn(site.Requests.Single(request => request.Method == "PATCH")));
            Assert.IsFalse(File.Exists(seen[0].Path), "The temporary file was deleted.");
        }

        // Where the netstandard2.0 build cannot make a temporary file that only this user can read, because the runtime
        // cannot call the C library, it makes none: the operation fails before the stream is read or anything is sent,
        // and the umask is never changed to make one.
        [TestMethod]
        [Timeout(60000)]
        public async Task AStreamIsNotCopiedWhereNoPrivateTemporaryFileCanBeMade()
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("The netstandard2.0 build's private creation is for systems other than Windows.");
            }
            FilesClient client = FilesClient.Create(site.Configuration());
            string[] spoolsBefore = Spools();
            bool read = false;
            var stream = new ObservedStream(Contents, () => read = true);

            FilesCom.Transfers.TemporaryPartFile.NetstandardCreationForTests.Value = true;
            FilesCom.Transfers.TemporaryPartFile.NoCLibraryForTests.Value = true;
            IOException error;
            try
            {
                error = await Assert.ThrowsExactlyAsync<IOException>(() => client.Users.UpdateAsync(1, new Dictionary<string, object>
                {
                    { "avatar_file", FileAttachment.FromStream(stream, "avatar.png") },
                }));
            }
            finally
            {
                FilesCom.Transfers.TemporaryPartFile.NetstandardCreationForTests.Value = false;
                FilesCom.Transfers.TemporaryPartFile.NoCLibraryForTests.Value = false;
            }

            StringAssert.Contains(error.Message, "that only this user can read");
            Assert.IsFalse(read, "The stream was not read.");
            Assert.AreEqual(0, site.Requests.Count, "Nothing was sent.");
            CollectionAssert.AreEquivalent(spoolsBefore, Spools(), "No temporary file was made.");
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task FileParametersTakeFileAttachmentsOrFileObjects()
        {
            site.On("PATCH", "/users/1", request => LoopbackSite.Json("{\"id\":1}"));
            FilesClient client = FilesClient.Create(site.Configuration());

            InvalidParameterException legacy = await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => client.Users.UpdateAsync(1, new Dictionary<string, object>
            {
                { "avatar_file", new ByteArrayContent(Contents) },
            }));
            InvalidParameterException nested = await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => client.Bundles.UpdateAsync(1, new Dictionary<string, object>
            {
                { "watermark_value", new Dictionary<string, object> { { "image", FileAttachment.FromBytes(Contents, "a.png") } } },
            }));
            await client.Users.UpdateAsync(1, new Dictionary<string, object>
            {
                { "avatar_file", new Dictionary<string, object> { { "encoded_content", "AAEC" }, { "filename", "object.png" }, { "type", "image/png" } } },
            });

            Assert.AreEqual("Bad parameter: avatar_file must be of type FileAttachment or IDictionary<string, object>", legacy.Message);
            StringAssert.Contains(nested.Message, "can only be the value of a file parameter");
            Assert.AreEqual(1, site.Requests.Count);
            Assert.AreEqual("{\"avatar_file\":{\"encoded_content\":\"AAEC\",\"filename\":\"object.png\",\"type\":\"image/png\"},\"id\":1}", site.Requests[0].Body);
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task AttachmentBodiesAreLoggedWithoutTheirBytes()
        {
            site.On("PATCH", "/users/1", request => LoopbackSite.Json("{\"id\":1}"));
            FilesClient client = FilesClient.Create(site.Configuration());
            byte[] secret = Encoding.UTF8.GetBytes("sentinel-file-contents");

            using (var logs = new LogCapture())
            {
                await client.Users.UpdateAsync(1, new Dictionary<string, object>
                {
                    { "avatar_file", FileAttachment.FromBytes(secret, "100%.png", "image/png") },
                    { "notes", "a note" },
                });

                StringAssert.Contains(logs.Debug, "content: JSON {\"avatar_file\":{\"encoded_content\":\"(22 bytes as Base64)\",\"filename\":\"100%.png\",\"type\":\"image/png\"},\"notes\":\"a note\",\"id\":1}");
                Assert.IsFalse((logs.Debug + logs.Normal).Contains(Convert.ToBase64String(secret)), "The file's bytes are never logged.");
                Assert.IsFalse((logs.Debug + logs.Normal).Contains("sentinel-file-contents"));
            }
        }

        private static Exception SourceFailure(string kind)
        {
            switch (kind)
            {
                case "io":
                    return new IOException("The disk holding the avatar failed.");
                case "invalid operation":
                    return new InvalidOperationException("The avatar's source cannot be read in its state.");
                case "source timeout":
                    return new OperationCanceledException("The avatar's own source timed out.", new TimeoutException("Where it reads from did not answer in time."));
                default:
                    return new HttpRequestException("The avatar's own source, read from elsewhere, lost its connection.");
            }
        }

        private FileAttachment Attachment(string kind, string fileName)
        {
            switch (kind)
            {
                case "bytes":
                    return FileAttachment.FromBytes(Contents, fileName, "image/png");
                case "file":
                    string path = Path.Combine(folder, "source.bin");
                    File.WriteAllBytes(path, Contents);
                    return FileAttachment.FromFile(path, fileName, "image/png");
                case "stream":
                    return FileAttachment.FromStream(new MemoryStream(Contents) { Position = 3 }, fileName, "image/png");
                default:
                    return FileAttachment.FromStream(new FailingAfter(Contents, int.MaxValue, null), fileName, "image/png");
            }
        }

        private ResponseMessage Redirect(int status, string path)
        {
            ResponseMessage response = LoopbackSite.Status(status);
            response.AddHeader("Location", site.Url + "/api/rest/v1" + path);
            return response;
        }

        // The file's bytes in a request: the Base64 of a JSON body's file object, or a form's file part.
        private static byte[] FileIn(ReceivedRequest request)
        {
            if (request.Headers["Content-Type"].StartsWith("application/json"))
            {
                using (JsonDocument body = JsonDocument.Parse(request.BodyBytes))
                {
                    return Convert.FromBase64String(body.RootElement.GetProperty("avatar_file").GetProperty("encoded_content").GetString());
                }
            }
            byte[] bytes = request.BodyBytes;
            string boundary = "--" + request.Headers["Content-Type"].Split("boundary=")[1];
            int start = IndexOf(bytes, Encoding.ASCII.GetBytes("Content-Type: image/png\r\n\r\n"), 0) + 27;
            int end = IndexOf(bytes, Encoding.ASCII.GetBytes("\r\n" + boundary), start);
            return bytes.Skip(start).Take(end - start).ToArray();
        }

        private static int IndexOf(byte[] bytes, byte[] pattern, int from)
        {
            for (int i = from; i <= bytes.Length - pattern.Length; i++)
            {
                if (bytes.AsSpan(i, pattern.Length).SequenceEqual(pattern))
                {
                    return i;
                }
            }
            return -1;
        }

        // The temporary files of operations, by either name they are given: files-upload-<id>.tmp as .NET 8 and 10 create
        // them, and files-upload-<id>-<random> as the netstandard2.0 build does (TemporaryPartFile).
        private static string[] Spools()
        {
            return Directory.GetFiles(Path.GetTempPath(), "files-upload-*");
        }

        // A forward-only stream of contents that calls atFirstRead once, when it is first read, before giving any bytes.
        private sealed class ObservedStream : Stream
        {
            private readonly byte[] contents;
            private readonly Action atFirstRead;
            private int position;
            private bool observed;

            internal ObservedStream(byte[] contents, Action atFirstRead)
            {
                this.contents = contents;
                this.atFirstRead = atFirstRead;
            }

            public override bool CanRead => true;

            public override bool CanSeek => false;

            public override bool CanWrite => false;

            public override long Length => throw new NotSupportedException();

            public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (!observed)
                {
                    observed = true;
                    atFirstRead();
                }
                int n = Math.Min(count, contents.Length - position);
                Array.Copy(contents, position, buffer, offset, n);
                position += n;
                return n;
            }

            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                return Task.FromResult(Read(buffer, offset, count));
            }

            public override void Flush()
            {
            }

            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

            public override void SetLength(long value) => throw new NotSupportedException();

            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        }

        // A stream of contents that fails with error, if any, once failAt bytes have been read, and counts the passes
        // made over it. It can seek only when seekable.
        private sealed class FailingAfter : Stream
        {
            private readonly byte[] contents;
            private readonly int failAt;
            private readonly Exception error;
            private readonly bool seekable;
            private int position;

            internal FailingAfter(byte[] contents, int failAt, Exception error, bool seekable = false)
            {
                this.contents = contents;
                this.failAt = failAt;
                this.error = error;
                this.seekable = seekable;
            }

            public int Passes { get; private set; }

            public bool Disposed { get; private set; }

            // When set, a read once failAt bytes have been read waits until it is cancelled, as a stalled source's does,
            // and completes this first.
            public TaskCompletionSource<bool> Stalled { get; set; }

            // Thrown by Length once it has been read LengthReadsBeforeFailure times.
            public Exception LengthFailure { get; set; }

            public int LengthReadsBeforeFailure { get; set; }

            public override bool CanRead => true;

            public override bool CanSeek => seekable;

            public override bool CanWrite => false;

            public override long Length
            {
                get
                {
                    if (LengthFailure != null && LengthReadsBeforeFailure-- <= 0)
                    {
                        throw LengthFailure;
                    }
                    return seekable ? contents.Length : throw new NotSupportedException();
                }
            }

            public override long Position
            {
                get => seekable ? position : throw new NotSupportedException();
                set => position = seekable ? (int)value : throw new NotSupportedException();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (position == 0)
                {
                    Passes++;
                }
                if (position >= failAt && error != null)
                {
                    throw error;
                }
                int n = Math.Min(Math.Min(count, 4096), contents.Length - position);
                Buffer.BlockCopy(contents, position, buffer, offset, n);
                position += n;
                return n;
            }

            public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                if (Stalled != null && position >= failAt)
                {
                    Stalled.TrySetResult(true);
                    await Task.Delay(Timeout.Infinite, cancellationToken);
                }
                return Read(buffer, offset, count);
            }

            protected override void Dispose(bool disposing)
            {
                Disposed = true;
                base.Dispose(disposing);
            }

            public override void Flush()
            {
            }

            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

            public override void SetLength(long value) => throw new NotSupportedException();

            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        }
    }
}