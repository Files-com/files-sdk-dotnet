using FilesCom;
using FilesCom.Transfers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FilesTests
{
    // What a download owns and gives up: its stage's access and cleanup, the access of a file it replaces, the moment it
    // publishes, the limiter it borrows, the URL it follows when it cannot bind to a version, the answers it accepts
    // whole, and how it plans its parts.
    public partial class DownloadTest
    {
        [TestMethod]
        [Timeout(60000)]
        public async Task StageIsPrivateUnderAnInheritedAccessControlList()
        {
            if (!OperatingSystem.IsMacOS())
            {
                Assert.Inconclusive("Inherited access control lists are exercised natively on macOS here; Linux POSIX ACLs are not run.");
                return;
            }
            string shared = Path.Combine(folder, "shared");
            Directory.CreateDirectory(shared);
            Run("/bin/chmod", "+a", "everyone allow read,write,execute,delete,append,list,search,add_file,add_subdirectory,delete_child,readsecurity,file_inherit,directory_inherit", shared);
            try
            {
                using (var server = new DownloadServer(Contents))
                {
                    string observed = null;
                    server.Intercept = (request, connection) =>
                    {
                        string staging = Directory.GetDirectories(shared, ".files-download-*").Single();
                        observed ??= Run("/bin/ls", "-lde", staging, Directory.GetFiles(staging).Single());
                        return Task.FromResult(false);
                    };
                    string destination = Path.Combine(shared, "file.bin");

                    await Client(server).Transfers.DownloadFileAsync("remote.bin", destination, Parts(concurrency: 1));

                    StringAssert.Contains(Run("/bin/ls", "-lde", shared), "everyone", "The shared folder keeps its own access.");
                    Assert.IsFalse(observed.Contains("everyone"), "Neither the staging directory nor the stage keeps an inherited entry: " + observed);
                    Assert.IsFalse(Run("/bin/ls", "-le", destination).Contains("everyone"), "A new file gets a new file's permissions and no list.");
                    CollectionAssert.AreEqual(Contents, File.ReadAllBytes(destination));
                }
            }
            finally
            {
                Run("/bin/chmod", "-RN", shared);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task CleanupLeavesWhatTookTheStagingDirectorysPlace()
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("Staging directories are held by descriptor on macOS and Linux only.");
                return;
            }
            using (var server = new DownloadServer(Contents))
            {
                string replaced = null;
                server.Intercept = (request, connection) =>
                {
                    if (replaced == null)
                    {
                        replaced = Directory.GetDirectories(folder, ".files-download-*").Single();
                        Directory.Move(replaced, Path.Combine(folder, "moved-stage"));
                        Directory.CreateDirectory(replaced);
                    }
                    return Task.FromResult(false);
                };

                await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Parts(concurrency: 1));

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.IsTrue(Directory.Exists(replaced), "The directory that took the staging directory's name is not the download's to remove.");
                Assert.IsTrue(Directory.Exists(Path.Combine(folder, "moved-stage")), "The moved staging directory stays where it was put.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("private", DisplayName = "Owner only")]
        [DataRow("group", DisplayName = "Another group, which may read")]
        [DataRow("acl", DisplayName = "An access control list")]
        [DataRow("write-only", DisplayName = "Writable, not readable")]
        public async Task ReplacedFileKeepsItsAccess(string access)
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("Unix permissions only.");
                return;
            }
            UnixFileMode mode = access == "group" ? UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.GroupRead
                : access == "acl" ? UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.GroupRead | UnixFileMode.OtherRead
                : access == "write-only" ? UnixFileMode.UserWrite
                : UnixFileMode.UserRead | UnixFileMode.UserWrite;
            File.WriteAllText(Destination, "OLD");
            if (access == "group")
            {
                // A group this user belongs to, other than the one new files get.
                Run("/usr/bin/chgrp", OperatingSystem.IsMacOS() ? "everyone" : Run("/usr/bin/id", "-gn").Trim(), Destination);
            }
            if (access == "acl")
            {
                if (!OperatingSystem.IsMacOS())
                {
                    Assert.Inconclusive("macOS access control lists; Linux POSIX ACLs are not run.");
                }
                Run("/bin/chmod", "+a", "group:everyone deny write", Destination);
            }
            File.SetUnixFileMode(Destination, mode);
            string group = Run("/usr/bin/stat", OperatingSystem.IsMacOS() ? "-f" : "-c", OperatingSystem.IsMacOS() ? "%Sg" : "%G", Destination).Trim();
            using (var server = new DownloadServer(Contents))
            {
                await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Parts(concurrency: 1));

                Assert.AreEqual(mode, File.GetUnixFileMode(Destination), "The permissions of the file it replaced.");
                Assert.AreEqual(group, Run("/usr/bin/stat", OperatingSystem.IsMacOS() ? "-f" : "-c", OperatingSystem.IsMacOS() ? "%Sg" : "%G", Destination).Trim(), "Its group.");
                if (access == "acl")
                {
                    StringAssert.Contains(Run("/bin/ls", "-le", Destination), "group:everyone deny write", "Its access control list.");
                }
                File.SetUnixFileMode(Destination, UnixFileMode.UserRead | UnixFileMode.UserWrite);
                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("file-empty", DisplayName = "An empty file, as its size becomes known")]
        [DataRow("file-final", DisplayName = "A file, as its last part is written")]
        [DataRow("stream-final", DisplayName = "A stream, as its last part is written")]
        public async Task CancellingFromTheLastProgressReportPublishesNothing(string when)
        {
            byte[] contents = when == "file-empty" ? new byte[0] : Contents;
            File.WriteAllText(Destination, "ORIGINAL");
            using (var server = new DownloadServer(contents))
            using (var cancellation = new CancellationTokenSource())
            {
                var options = Parts(concurrency: 1);
                options.Progress = new SyncProgress<DownloadProgress>(progress =>
                {
                    if (when == "file-empty" ? progress.TotalBytesKnown : progress.WrittenBytes == contents.Length)
                    {
                        cancellation.Cancel();
                    }
                });
                var stream = new MemoryStream();

                Task download = when.StartsWith("stream", StringComparison.Ordinal)
                    ? Client(server).Transfers.DownloadStreamAsync("remote.bin", stream, options, cancellation.Token)
                    : Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options, cancellation.Token);
                await Assert.ThrowsAsync<OperationCanceledException>(() => download);

                Assert.AreEqual("ORIGINAL", File.ReadAllText(Destination), "The file already there stays.");
                CollectionAssert.AreEqual(new string[0], Directory.GetDirectories(folder, ".files-download-*"), "The stage was removed.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task AnOverlongPartWritesNothingOfItselfOrItsNeighbours()
        {
            using (var server = new DownloadServer(System.Text.Encoding.ASCII.GetBytes("abcdef")))
            {
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range?.First != 3)
                    {
                        return false;
                    }
                    // Four bytes for a three-byte part, in one chunk.
                    await connection.SendAsync("HTTP/1.1 206 OK\r\nContent-Type: application/octet-stream\r\nContent-Range: bytes 3-5/6\r\nETag: \"etag-1\"\r\nTransfer-Encoding: chunked\r\n\r\n4\r\nWXYZ\r\n0\r\n\r\n");
                    return true;
                };
                string checkpoint = Path.Combine(folder, "checkpoint");

                await Assert.ThrowsExactlyAsync<InvalidResponseException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, new DownloadOptions { PartSize = 3, MaxConcurrency = 1, CheckpointPath = checkpoint }));

                byte[] stage = File.ReadAllBytes(Directory.GetFiles(Directory.GetDirectories(folder, ".files-download-*").Single()).Single());
                Assert.AreEqual("abc", System.Text.Encoding.ASCII.GetString(stage, 0, 3), "The part before stays as it was written.");
                Assert.IsTrue(stage.Skip(3).All(b => b == 0), "Nothing of the overlong part was written: " + System.Text.Encoding.ASCII.GetString(stage));
                Assert.IsFalse(File.Exists(Destination));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task ATinyLimiterStaysUsableAcrossWholeAndPartDownloads()
        {
            var limiter = new TransferLimiter(maxConcurrentRequests: 1, maxBufferedBytes: 1);
            long mostBuffered = 0;
            using (var whole = new DownloadServer(new byte[] { 1, 2, 3 }) { Bound = false })
            using (var bound = new DownloadServer(new byte[] { 4, 5, 6 }))
            {
                bound.Intercept = (request, connection) =>
                {
                    mostBuffered = Math.Max(mostBuffered, limiter.BufferedBytes);
                    return Task.FromResult(false);
                };
                var wholeBytes = new MemoryStream();
                var streamed = new MemoryStream();

                await Client(whole).Transfers.DownloadStreamAsync("remote.bin", wholeBytes, new DownloadOptions { Limiter = limiter });
                Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes, "The whole download gave back its slot and its buffer.");
                await Client(bound).Transfers.DownloadStreamAsync("remote.bin", streamed, new DownloadOptions { Limiter = limiter, PartSize = 1 });
                await Client(bound).Transfers.DownloadFileAsync("remote.bin", Destination, new DownloadOptions { Limiter = limiter, PartSize = 2 });

                CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, wholeBytes.ToArray(), "Read through a one-byte buffer.");
                CollectionAssert.AreEqual(new byte[] { 4, 5, 6 }, streamed.ToArray());
                CollectionAssert.AreEqual(new byte[] { 4, 5, 6 }, File.ReadAllBytes(Destination));
                Assert.IsTrue(mostBuffered <= 1, $"Never more than the limiter's buffer space: {mostBuffered}.");
                Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes + limiter.QueuedRequests + limiter.QueuedBufferReservations);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(204)]
        [DataRow(203)]
        public async Task OnlyHttp200IsAWholeFile(int status)
        {
            using (var server = new DownloadServer(new byte[] { 1, 2, 3 }) { Bound = false })
            {
                server.Intercept = async (request, connection) =>
                {
                    await connection.RespondAsync(status, status == 204 ? "" : "abc");
                    return true;
                };
                var stream = new MemoryStream();

                InvalidResponseException error = await Assert.ThrowsExactlyAsync<InvalidResponseException>(() => Client(server).Transfers.DownloadStreamAsync("remote.bin", stream));

                StringAssert.Contains(error.Message, $"HTTP {status}");
                Assert.AreEqual(0, stream.Length);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task AWholeDownloadsExpiringUrlIsRenewedBeforeItsRetry()
        {
            using (var server = new DownloadServer(Contents) { Bound = false, UrlLifetime = 1 })
            {
                bool failed = false;
                server.Intercept = async (request, connection) =>
                {
                    if (failed)
                    {
                        return false;
                    }
                    failed = true;
                    await connection.RespondAsync(503, "{}");
                    return true;
                };
                var stream = new MemoryStream();

                await Client(server, retries: 1).Transfers.DownloadStreamAsync("remote.bin", stream);

                CollectionAssert.AreEqual(Contents, stream.ToArray());
                Assert.AreEqual(2, server.ApiRequests.Count, "The expiring URL was renewed before the retry: " + string.Join(" | ", server.ApiRequests) + " storage: " + string.Join(" | ", server.StorageRequests.Select(r => r.Line)));
                Assert.AreEqual(DownloadServer.DefaultDownload, server.ApiRequests[1], "Renewed with an ordinary Download.");
            }
        }

        // Downloads are bound only where the URL is HTTPS, on the default port and without user information, to an object
        // at an Amazon S3 general-purpose endpoint, which checks If-Match against the object the URL names: the forms
        // Files.com's storage URLs take. Any other storage comes in one request: a host that merely looks like S3's, such
        // as a load balancer whose customer-chosen name starts with s3-, S3 itself over HTTP, which authenticates no
        // server, and S3's other endpoints, which this SDK does not qualify.
        [TestMethod]
        [Timeout(10000)]
        [DataRow("https://files-bucket.s3.amazonaws.com/metadata/1/u", true)]
        [DataRow("https://s3.amazonaws.com/files-bucket/metadata/1/u", true)]
        [DataRow("https://s3.us-west-2.amazonaws.com/files-bucket/metadata/1/u", true)]
        [DataRow("https://s3-us-west-2.amazonaws.com/files-bucket/metadata/1/u", true)]
        [DataRow("https://files-bucket.s3.eu-central-1.amazonaws.com/u", true)]
        [DataRow("https://files-bucket.s3.dualstack.us-east-1.amazonaws.com/u", true)]
        [DataRow("https://files-bucket.s3.cn-north-1.amazonaws.com.cn/u", true)]
        [DataRow("https://FILES-BUCKET.S3.US-EAST-1.AMAZONAWS.COM/u", true)]
        [DataRow("https://files-bucket.s3.amazonaws.com:443/u", true)]
        [DataRow("https://s3-proxy-1234567890abcdef.elb.us-east-2.amazonaws.com/file", false)]
        [DataRow("http://s3-proxy-1234567890abcdef.elb.us-east-2.amazonaws.com/file", false)]
        [DataRow("https://s3-proxy-1234567890.us-east-2.elb.amazonaws.com/file", false)]
        [DataRow("http://files-bucket.s3.amazonaws.com/u", false)]
        [DataRow("https://files-bucket.s3.amazonaws.com:8443/u", false)]
        [DataRow("https://user@files-bucket.s3.amazonaws.com/u", false)]
        [DataRow("https://s3.amazonaws.com/", false)]
        [DataRow("https://FILES-BUCKET.S3.AMAZONAWS.COM./u", false)]
        [DataRow("https://files-bucket.s3-fips.us-east-1.amazonaws.com/u", false)]
        [DataRow("https://files-bucket.s3-accelerate.amazonaws.com/u", false)]
        [DataRow("https://ap-123456789012.s3-accesspoint.us-east-1.amazonaws.com/u", false)]
        [DataRow("https://files-bucket.s3-object-lambda.us-east-1.amazonaws.com/u", false)]
        [DataRow("http://files-bucket.s3-website-us-east-1.amazonaws.com/u", false)]
        [DataRow("https://execute-api.us-east-1.amazonaws.com/u", false)]
        [DataRow("https://s3.amazonaws.com.example.com/u", false)]
        [DataRow("https://amazonaws.com/u", false)]
        [DataRow("https://d111111abcdef8.cloudfront.net/u", false)]
        [DataRow("https://storage.googleapis.com/bucket/u", false)]
        [DataRow("https://acme.files.com/api/rest/v1/download/u", false)]
        [DataRow("ftp://files-bucket.s3.amazonaws.com/u", false)]
        public void OnlyHttpsUrlsOfAmazonS3GeneralPurposeEndpointsCanBindDownloads(string url, bool binds)
        {
            Assert.AreEqual(binds, DownloadVersion.CanBind(FilesCom.Http.SignedUrl.Of(url + "?X-Amz-Signature=stand-in")));
        }

        // What the storage URL Files.com gives decides how a download comes, though every storage here answers ranges and
        // entity tags as S3 does: bound, in conditioned parts, only from an HTTPS URL of an S3 general-purpose endpoint;
        // from anything else, such as a load balancer whose name starts with s3-, or S3 over HTTP, in one request without
        // a condition, whatever it answers.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("https://files-test-bucket.s3.us-east-1.amazonaws.com", true)]
        [DataRow("https://s3.us-west-2.amazonaws.com", true)]
        [DataRow("https://s3-us-west-2.amazonaws.com", true)]
        [DataRow("https://files-test-bucket.s3.dualstack.us-east-1.amazonaws.com", true)]
        [DataRow("https://s3-proxy-1234567890abcdef.elb.us-east-2.amazonaws.com", false)]
        [DataRow("http://s3-proxy-1234567890abcdef.elb.us-east-2.amazonaws.com", false)]
        [DataRow("http://files-test-bucket.s3.us-east-1.amazonaws.com", false)]
        public async Task OnlyStorageAnHttpsAmazonS3UrlNamesIsBoundToAVersion(string origin, bool binds)
        {
            using (var server = new DownloadServer(Contents) { StorageOrigin = origin })
            {
                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Parts());

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual(binds, result.VersionBound);
                if (binds)
                {
                    Assert.AreEqual(7, server.StorageRequests.Count);
                    Assert.IsTrue(server.StorageRequests.Skip(1).All(request => request.IfMatch == server.ETag && request.Range.HasValue));
                }
                else
                {
                    Assert.AreEqual("GET whole", server.StorageRequests.Single().ToString(), "One request, for the whole file.");
                    Assert.IsNull(server.StorageRequests.Single().IfMatch);
                }
            }
        }

        // A download is bound only by an answer from storage its URL, as renewed, can bind. When Files.com's URL names S3 but
        // its renewal, made before the first answer, names other storage, the file comes from that storage in one request,
        // whatever ranges and entity tags it serves: renewed because the URL is about to expire, or because S3 refused it
        // as expired.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("about to expire")]
        [DataRow("refused as expired")]
        public async Task ARenewedUrlOfStorageThatCannotBindTheDownloadIsFetchedInOneRequest(string renewal)
        {
            using (var server = new DownloadServer(Contents) { UrlLifetime = renewal == "about to expire" ? 20 : (int?)null })
            {
                // Every URL after the first names this server's own address, as storage other than S3 does.
                server.OnApiRequest = target => server.Bound = server.ApiRequests.Count < 2;
                bool refused = false;
                if (renewal == "refused as expired")
                {
                    server.Intercept = async (request, connection) =>
                    {
                        if (refused)
                        {
                            return false;
                        }
                        refused = true;
                        await connection.RespondAsync(403, "<Error><Code>AccessDenied</Code><Message>Request has expired</Message></Error>");
                        return true;
                    };
                }

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Parts());

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.IsFalse(result.VersionBound, "Not bound to what storage the renewed URL names answered.");
                Assert.AreEqual(2, server.ApiRequests.Count, "The first URL, then its renewal.");
                Assert.AreEqual(renewal == "refused as expired" ? 2 : 1, server.StorageRequests.Count);
                Assert.AreEqual("GET whole", server.StorageRequests.Last().ToString(), "One request for the whole file, from the renewed URL.");
                Assert.IsTrue(server.StorageRequests.All(request => request.IfMatch == null));
            }
        }

        // A version's storage resource is its URL without the query, whose signature and expiry change with every URL
        // for the same object; another path or host is another resource. It is recorded as a digest, not as the URL.
        [TestMethod]
        [Timeout(10000)]
        public void AVersionsResourceIsItsUrlWithoutTheQuery()
        {
            string first = DownloadVersion.SourceOf(FilesCom.Http.SignedUrl.Of("https://b.s3.amazonaws.com/metadata/1/u?X-Amz-Date=20261001T000000Z&X-Amz-Signature=1"));
            Assert.AreEqual(first, DownloadVersion.SourceOf(FilesCom.Http.SignedUrl.Of("https://b.s3.amazonaws.com/metadata/1/u?X-Amz-Date=20261001T010000Z&X-Amz-Signature=2#f")));
            Assert.AreNotEqual(first, DownloadVersion.SourceOf(FilesCom.Http.SignedUrl.Of("https://b.s3.amazonaws.com/metadata/1/v?X-Amz-Signature=1")));
            Assert.AreNotEqual(first, DownloadVersion.SourceOf(FilesCom.Http.SignedUrl.Of("https://c.s3.amazonaws.com/metadata/1/u?X-Amz-Signature=1")));
            StringAssert.StartsWith(first, "sha256:");
            Assert.IsFalse(first.Contains("amazonaws") || first.Contains("metadata"), first);
        }

        [TestMethod]
        [Timeout(10000)]
        public void PartsArePlannedLazilyWithinTheFileWhateverItsSize()
        {
            // At most three are taken: a plan that ran past the size would give more, not run on.
            ByteRange[] two = DownloadCoordinator.PlanRanges(long.MaxValue, long.MaxValue - 1, null, CancellationToken.None).Take(3).ToArray();
            CollectionAssert.AreEqual(new[] { "0 0 9223372036854775806", "1 9223372036854775806 1" }, two.Select(part => $"{part.Index} {part.Start} {part.Length}").ToArray(), "The last part is what remains: no position passes the size.");

            ByteRange[] first = DownloadCoordinator.PlanRanges(long.MaxValue, 1, null, CancellationToken.None).Take(3).ToArray();
            CollectionAssert.AreEqual(new long[] { 0, 1, 2 }, first.Select(part => part.Start).ToArray(), "Parts are made as they are reached.");

            var written = new IntervalSet();
            written.Add(0, 1000L * 1000 * 1000);
            ByteRange next = DownloadCoordinator.PlanRanges(long.MaxValue, 1, written, CancellationToken.None).First();
            Assert.AreEqual(1000L * 1000 * 1000, next.Start, "A run of written parts is passed over at once.");

            using (var cancellation = new CancellationTokenSource())
            {
                using (var parts = DownloadCoordinator.PlanRanges(long.MaxValue, 1, null, cancellation.Token).GetEnumerator())
                {
                    Assert.IsTrue(parts.MoveNext());
                    cancellation.Cancel();
                    Assert.ThrowsExactly<OperationCanceledException>(() => parts.MoveNext());
                }
            }
        }

        // A write-only file's folder moves during the download, and a file anyone may read takes its old path: neither is
        // replaced, since the path given no longer leads to the folder the download holds, and the file in that folder
        // gets nothing from the one at its old path. ReplacedFileKeepsItsAccess reads a write-only file's own access.
        [TestMethod]
        [Timeout(60000)]
        public async Task AWriteOnlyFileWhoseFolderMovesIsNotReplacedNorIsTheFileAtItsOldPath()
        {
            if (!OperatingSystem.IsMacOS())
            {
                Assert.Inconclusive("macOS access control lists; Linux reads a write-only file's through O_PATH, which is not run.");
                return;
            }
            string held = Path.Combine(folder, "held");
            string moved = Path.Combine(folder, "moved");
            Directory.CreateDirectory(held);
            string destination = Path.Combine(held, "file.bin");
            File.WriteAllText(destination, "OLD");
            File.SetUnixFileMode(destination, UnixFileMode.UserWrite);
            string replaced = Path.Combine(moved, "file.bin");
            try
            {
                using (var server = new DownloadServer(Contents))
                {
                    server.Intercept = (request, connection) =>
                    {
                        if (!Directory.Exists(moved))
                        {
                            // The folder the download holds moves away, and a file anyone may read takes the old path.
                            Directory.Move(held, moved);
                            Directory.CreateDirectory(held);
                            File.WriteAllText(destination, "DECOY");
                            Run("/bin/chmod", "+a", "everyone allow read", destination);
                        }
                        return Task.FromResult(false);
                    };

                    DownloadPublicationException error = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", destination, Parts(concurrency: 1)));

                    Assert.AreEqual(DownloadPublicationProblem.DestinationMoved, error.Problem);
                    Assert.IsFalse(error.Placed);
                    Assert.AreEqual(UnixFileMode.UserWrite, File.GetUnixFileMode(replaced), "The file in the folder the download holds is not replaced, and keeps its permissions.");
                    Assert.IsFalse(Run("/bin/ls", "-le", replaced).Contains("everyone"), "It gets no access control list from the file now at its old path.");
                    Assert.AreEqual("DECOY", File.ReadAllText(destination), "That file is left alone.");
                    CollectionAssert.AreEqual(new string[0], Directory.GetDirectories(moved, ".files-download-*"), "The stage was removed.");
                }
            }
            finally
            {
                Run("/bin/chmod", "-RN", folder);
                if (File.Exists(replaced))
                {
                    File.SetUnixFileMode(replaced, UnixFileMode.UserRead | UnixFileMode.UserWrite);
                }
            }
            Assert.AreEqual("OLD", File.ReadAllText(replaced), "The file in the folder the download holds stays as it was.");
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task AFileThisUserCanNeitherReadNorWriteIsNotReplaced()
        {
            if (!OperatingSystem.IsMacOS())
            {
                Assert.Inconclusive("macOS; on Linux such a file is examined through O_PATH, which is not run.");
                return;
            }
            File.WriteAllText(Destination, "OLD");
            File.SetUnixFileMode(Destination, UnixFileMode.None);
            try
            {
                using (var server = new DownloadServer(Contents))
                {
                    DownloadPublicationException error = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Parts(concurrency: 1)));

                    Assert.AreEqual(DownloadPublicationProblem.AccessNotKept, error.Problem);
                    StringAssert.Contains(error.Message, "neither read nor written");
                    Assert.AreEqual(UnixFileMode.None, File.GetUnixFileMode(Destination));
                    CollectionAssert.AreEqual(new string[0], Directory.GetDirectories(folder, ".files-download-*"), "The stage was removed.");
                }
            }
            finally
            {
                File.SetUnixFileMode(Destination, UnixFileMode.UserRead | UnixFileMode.UserWrite);
            }
            Assert.AreEqual("OLD", File.ReadAllText(Destination), "The file stays as it was.");
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("group", DisplayName = "0604 of a group the new file cannot be given, whose members could not read it")]
        [DataRow("owner", DisplayName = "0640 of another user's")]
        public async Task AFileWhoseOwnerOrGroupCannotBeKeptIsNotReplaced(string lost)
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("Unix owners and groups only.");
                return;
            }
            if (Run("/usr/bin/id", "-u").Trim() == "0")
            {
                Assert.Inconclusive("As root, any owner or group can be given, so nothing is refused.");
                return;
            }
            UnixFileMode mode = lost == "group"
                ? UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.OtherRead
                : UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.GroupRead;
            File.WriteAllText(Destination, "OLD");
            File.SetUnixFileMode(Destination, mode);
            string before = Run("/bin/ls", "-ln", Destination);
            using (var server = new DownloadServer(Contents))
            {
                // The file is taken to be of a group this user is not in, or of another user: giving either to the file
                // replacing it is left to the system, which refuses.
                Native.ReplacedOwnerFaults.Value = status => lost == "group" ? (status.Uid, 424242u) : (status.Uid + 424242u, status.Gid);
                DownloadPublicationException error;
                try
                {
                    error = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Parts(concurrency: 1)));
                }
                finally
                {
                    Native.ReplacedOwnerFaults.Value = null;
                }

                StringAssert.Contains(error.Message, "was not replaced");
                StringAssert.Contains(error.Message, lost == "group" ? "belongs to a group" : "is another user's");
                Assert.AreEqual(DownloadPublicationProblem.AccessNotKept, error.Problem);
            }
            Assert.AreEqual("OLD", File.ReadAllText(Destination), "The file stays as it was.");
            Assert.AreEqual(before, Run("/bin/ls", "-ln", Destination), "With its owner, group and permissions.");
            CollectionAssert.AreEqual(new string[0], Directory.GetDirectories(folder, ".files-download-*"), "The stage was removed.");
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task TheFallbackReplacesNoFileWhoseAccessItCannotKeep()
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("On Windows the fallback replaces a file as the platform does.");
                return;
            }
            File.WriteAllText(Destination, "OLD");
            File.SetUnixFileMode(Destination, UnixFileMode.UserRead | UnixFileMode.UserWrite);
            using (LocalDirectory directory = PathDirectory.OpenPath(folder))
            {
                using (LocalStage stage = directory.CreateStage(Path.GetFileName(Destination)))
                {
                    await stage.WriteAsync(0, Contents, Contents.Length, CancellationToken.None);

                    DownloadPublicationException error = Assert.ThrowsExactly<DownloadPublicationException>(() => stage.Publish());

                    Assert.AreEqual(DownloadPublicationProblem.AccessNotKept, error.Problem);
                    StringAssert.Contains(error.Message, "was not replaced");
                }
                using (LocalStage stage = directory.CreateStage("new.bin"))
                {
                    await stage.WriteAsync(0, Contents, Contents.Length, CancellationToken.None);
                    stage.Publish();
                }
            }

            Assert.AreEqual("OLD", File.ReadAllText(Destination), "The file stays as it was.");
            Assert.AreEqual(UnixFileMode.UserRead | UnixFileMode.UserWrite, File.GetUnixFileMode(Destination));
            CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Path.Combine(folder, "new.bin")), "A new file is still published.");
            CollectionAssert.AreEqual(new string[0], Directory.GetDirectories(folder, ".files-download-*"), "Both stages are gone.");
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task APartOfTheLargestSizeIsAdmittedWithinAOneByteLimiter()
        {
            var limiter = new TransferLimiter(maxConcurrentRequests: 1, maxBufferedBytes: 1);
            using (var server = new DownloadServer(new byte[] { 1 }))
            using (var cancellation = new CancellationTokenSource())
            {
                server.Intercept = (request, connection) =>
                {
                    // The part was admitted and asked for, which is all this is about: nothing is sent back.
                    cancellation.Cancel();
                    connection.Dispose();
                    return Task.FromResult(true);
                };

                await Assert.ThrowsAsync<OperationCanceledException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, new DownloadOptions { PartSize = long.MaxValue, Limiter = limiter }, cancellation.Token));

                Assert.AreEqual((0L, long.MaxValue - 1), server.StorageRequests.Single().Range, "The part of 2^63 - 1 bytes was asked for.");
                Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes + limiter.QueuedRequests + limiter.QueuedBufferReservations);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task AnOverlongPartOfAStreamIsRefusedWithinItsOwnBuffer()
        {
            var limiter = new TransferLimiter(maxConcurrentRequests: 1, maxBufferedBytes: 3);
            long mostBuffered = 0;
            using (var server = new DownloadServer(System.Text.Encoding.ASCII.GetBytes("abcdef")))
            {
                server.Intercept = async (request, connection) =>
                {
                    mostBuffered = Math.Max(mostBuffered, limiter.BufferedBytes);
                    if (request.Range?.First != 3)
                    {
                        return false;
                    }
                    await connection.SendAsync("HTTP/1.1 206 OK\r\nContent-Type: application/octet-stream\r\nContent-Range: bytes 3-5/6\r\nETag: \"etag-1\"\r\nTransfer-Encoding: chunked\r\n\r\n4\r\nWXYZ\r\n0\r\n\r\n");
                    return true;
                };
                var stream = new MemoryStream();

                await Assert.ThrowsExactlyAsync<InvalidResponseException>(() => Client(server).Transfers.DownloadStreamAsync("remote.bin", stream, new DownloadOptions { PartSize = 3, MaxConcurrency = 1, Limiter = limiter }));

                Assert.AreEqual("abc", System.Text.Encoding.ASCII.GetString(stream.ToArray()), "Only the part before reached the stream, as it was sent.");
                Assert.AreEqual(3, mostBuffered, "Each part held exactly its own three bytes.");
                Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(5, DisplayName = "The directory's flush fails (EIO)")]
        [DataRow(22, DisplayName = "The file system cannot flush a directory (EINVAL)")]
        public async Task APublishedFilesDirectoryFlushIsReportedAsItWent(int errno)
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("Directories are flushed on macOS and Linux only.");
                return;
            }
            File.WriteAllText(Destination, "OLD");
            using (var server = new DownloadServer(Contents))
            {
                Native.FlushFaults.Value = call => call == Native.FlushCall.FullSyncDirectory || call == Native.FlushCall.SyncDirectory ? errno : 0;
                try
                {
                    Task download = Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Parts(concurrency: 1));
                    if (errno == 5)
                    {
                        DownloadPublicationException error = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => download);
                        Assert.AreEqual(DownloadPublicationProblem.PlacedNotDurable, error.Problem);
                        Assert.IsTrue(error.Placed);
                        StringAssert.Contains(error.Message, "was put in place, but may not survive a system crash");
                    }
                    else
                    {
                        // Nothing depends on the new name being on storage: it gets there when the system writes it.
                        await download;
                    }
                }
                finally
                {
                    Native.FlushFaults.Value = null;
                }

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination), "The file is in place either way.");
                CollectionAssert.AreEqual(new string[0], Directory.GetDirectories(folder, ".files-download-*"), "Nothing is left to continue.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(5, false, DisplayName = "F_FULLFSYNC fails (EIO): nothing is published")]
        [DataRow(45, true, DisplayName = "F_FULLFSYNC is not supported (ENOTSUP): fsync")]
        public async Task OnlyAnUnsupportedFullSyncFallsBackToFsync(int errno, bool published)
        {
            if (!OperatingSystem.IsMacOS())
            {
                Assert.Inconclusive("F_FULLFSYNC is macOS's.");
                return;
            }
            File.WriteAllText(Destination, "OLD");
            var calls = new System.Collections.Concurrent.ConcurrentQueue<Native.FlushCall>();
            using (var server = new DownloadServer(Contents))
            {
                Native.FlushFaults.Value = call =>
                {
                    calls.Enqueue(call);
                    return call == Native.FlushCall.FullSyncFile ? errno : 0;
                };
                try
                {
                    Task download = Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Parts(concurrency: 1));
                    if (published)
                    {
                        await download;
                    }
                    else
                    {
                        IOException error = await Assert.ThrowsExactlyAsync<IOException>(() => download);
                        StringAssert.Contains(error.Message, "cannot be flushed to storage");
                    }
                }
                finally
                {
                    Native.FlushFaults.Value = null;
                }
            }

            Assert.AreEqual(published, calls.Contains(Native.FlushCall.SyncFile), "fsync only in place of an unsupported F_FULLFSYNC: " + string.Join(", ", calls));
            if (published)
            {
                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
            }
            else
            {
                Assert.AreEqual("OLD", File.ReadAllText(Destination), "A file not flushed is not put in place.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(true, "<Error><Code>AccessDenied</Code><Message>Request has expired</Message></Error>", true, DisplayName = "A part, which Amazon S3 refuses as expired: the URL is renewed")]
        [DataRow(false, "<Error><Code>AuthenticationFailed</Code><Message>Signature not valid in the specified time frame</Message></Error>", true, DisplayName = "A whole file, which Azure refuses as expired: the URL is renewed")]
        [DataRow(true, "<Error><Code>AccessDenied</Code><Message>Access Denied</Message></Error>", false, DisplayName = "A part refused by an ordinary 403: the download's failure")]
        public async Task ARequestStorageRefusesAsExpiredIsSentAgainWithARenewedUrl(bool bound, string answer, bool renewed)
        {
            var limiter = new TransferLimiter();
            using (var server = new DownloadServer(Contents) { Bound = bound })
            {
                bool refused = false;
                server.Intercept = async (request, connection) =>
                {
                    if (refused)
                    {
                        return false;
                    }
                    refused = true;
                    await connection.RespondAsync(403, answer);
                    return true;
                };
                var stream = new MemoryStream();
                Task download = Client(server).Transfers.DownloadStreamAsync("remote.bin", stream, new DownloadOptions { Limiter = limiter, PartSize = bound ? 1 : (long?)null });

                if (renewed)
                {
                    await download;
                    CollectionAssert.AreEqual(Contents, stream.ToArray());
                    Assert.AreEqual(2, server.ApiRequests.Count, "One renewal: " + string.Join(" | ", server.ApiRequests));
                    Assert.AreEqual(DownloadServer.DefaultDownload, server.ApiRequests[1], "Renewed with an ordinary Download.");
                }
                else
                {
                    InvalidResponseException error = await Assert.ThrowsExactlyAsync<InvalidResponseException>(() => download);
                    Assert.IsFalse(error.Message.Contains("expired") || error.Message.Contains("AccessDenied"), error.Message);
                    Assert.AreEqual(1, server.ApiRequests.Count, "No renewal.");
                }
                Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes);
            }
        }

        // Runs a program and returns what it wrote to its standard output; fails the test if it fails.
        private static string Run(string program, params string[] arguments)
        {
            var start = new ProcessStartInfo(program) { RedirectStandardOutput = true, RedirectStandardError = true };
            foreach (string argument in arguments)
            {
                start.ArgumentList.Add(argument);
            }
            using (Process process = Process.Start(start))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                Assert.AreEqual(0, process.ExitCode, $"{program} {string.Join(" ", arguments)}: {error}");
                return output;
            }
        }

        private sealed class SyncProgress<T> : IProgress<T>
        {
            private readonly Action<T> report;

            internal SyncProgress(Action<T> report)
            {
                this.report = report;
            }

            public void Report(T value)
            {
                report(value);
            }
        }
    }
}