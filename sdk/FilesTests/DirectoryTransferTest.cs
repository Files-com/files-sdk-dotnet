using FilesCom;
using FilesCom.Transfers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using WireMock;

namespace FilesTests
{
    // Directory uploads and downloads with client.Transfers against loopback sites: what arrives where, how many files
    // and requests at once, what a failure or cancellation leaves, and what running a transfer again reuses.
    [TestClass]
    public class DirectoryTransferTest
    {
        private string folder;

        [TestInitialize]
        public void Start()
        {
            folder = Path.Combine(Path.GetTempPath(), "directory-test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
        }

        [TestCleanup]
        public void Stop()
        {
            Directory.Delete(folder, true);
        }

        private string Local => Path.Combine(folder, "local");

        private string Checkpoints => Path.Combine(folder, "checkpoints");

        // An upload reads its source by path on Windows, without reserving its ancestors against deletion for download
        // publication. Another program's open handle with delete access is compatible with those reads.
        [TestMethod]
        [Timeout(60000)]
        [OSCondition(OperatingSystems.Windows)]
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public async Task OnWindowsDirectoryUploadDoesNotReserveItsAncestorsForPublication()
        {
            if (!OperatingSystem.IsWindows())
            {
                return;
            }
            byte[] contents = Pattern(20, 1);
            WriteLocal("sub/file.bin", contents);
            using (var ancestor = OpenDirectoryForDelete(folder, 0x00010000, 7, IntPtr.Zero, 3, 0x02000000, IntPtr.Zero))
            using (var server = new UploadServer())
            {
                Assert.IsFalse(ancestor.IsInvalid, $"The source ancestor is open with delete access: error {Marshal.GetLastWin32Error()}.");

                DirectoryTransferResult result = await Client(server).Transfers.UploadDirectoryAsync(Local, "backup");

                CollectionAssert.AreEqual(contents, server.AcceptedBytes("backup/sub/file.bin"));
                Assert.AreEqual(1, result.Files.Count);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task DirectoryUploadSendsEveryFileToItsPathAndCreatesEmptyFolders()
        {
            using (var server = new UploadServer())
            {
                byte[] a = Pattern(20, 1);
                byte[] b = Pattern(12, 2);
                WriteLocal("a.bin", a);
                WriteLocal("sub/b.bin", b);
                WriteLocal("sub/deeper/c.bin", new byte[0]);
                Directory.CreateDirectory(Path.Combine(Local, "empty"));
                byte[] secret = Pattern(9, 3);
                string outside = Path.Combine(folder, "secret.bin");
                File.WriteAllBytes(outside, secret);
                bool links = !OperatingSystem.IsWindows();
                if (links)
                {
                    File.CreateSymbolicLink(Path.Combine(Local, "link.bin"), outside);
                    Directory.CreateSymbolicLink(Path.Combine(Local, "linked"), folder);
                }
                var progress = new DirectoryProgressLog();

                DirectoryTransferResult result = await Client(server).Transfers.UploadDirectoryAsync(Local, "backup", new DirectoryUploadOptions { MaxConcurrentFiles = 2, Progress = progress });

                CollectionAssert.AreEqual(a, server.AcceptedBytes("backup/a.bin"));
                CollectionAssert.AreEqual(b, server.AcceptedBytes("backup/sub/b.bin"));
                CollectionAssert.AreEquivalent(new[] { "backup/a.bin", "backup/sub/b.bin", "backup/sub/deeper/c.bin" }, server.Finishes.Select(finish => finish.GetProperty("path").GetString()).ToArray());
                Assert.IsTrue(server.Negotiations.Where(n => !n.TryGetProperty("ref", out _)).All(n => n.GetProperty("mkdir_parents").GetBoolean()), "Missing folders are created with each file.");
                CollectionAssert.AreEqual(new[] { "backup/empty" }, server.Folders.ToArray(), "Only the folder with nothing to upload is created by itself.");
                CollectionAssert.AreEqual(new[] { "a.bin", "sub/b.bin", "sub/deeper/c.bin" }, result.Files.Select(file => file.RelativePath).ToArray());
                CollectionAssert.AreEqual(new[] { "backup/a.bin", "backup/sub/b.bin", "backup/sub/deeper/c.bin" }, result.Files.Select(file => file.RemotePath).ToArray());
                CollectionAssert.AreEqual(new[] { Path.Combine(Local, "sub", "b.bin") }, result.Files.Where(file => file.Size == 12).Select(file => file.LocalPath).ToArray());
                CollectionAssert.AreEqual(new[] { "empty" }, result.Directories.ToArray());
                CollectionAssert.AreEqual(links ? new[] { "link.bin", "linked" } : new string[0], result.Excluded.ToArray(), "Symbolic links are not followed.");
                Assert.IsFalse(server.Parts.Any(part => part.Bytes.SequenceEqual(secret)), "Nothing outside the directory is read.");
                Assert.AreEqual(32, result.Bytes);
                DirectoryTransferProgress last = progress.Reports.Last();
                Assert.AreEqual(3, last.FilesTotal);
                Assert.AreEqual(3, last.FilesDone);
                Assert.AreEqual(0, last.FilesActive);
                Assert.AreEqual(32, last.BytesTotal);
                Assert.AreEqual(32, last.BytesDone);
                Assert.AreEqual(32, last.NetworkBytes);
                Assert.IsTrue(progress.Reports.All(report => report.FilesActive <= 2), "At most MaxConcurrentFiles files at once.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task FilesOfADirectoryStayWithinTheirFileAndRequestLimits()
        {
            using (var server = new UploadServer())
            {
                var inFlight = new Dictionary<string, int>();
                int mostFiles = 0;
                server.RespondToPart = async request =>
                {
                    lock (inFlight)
                    {
                        inFlight[request.File] = inFlight.TryGetValue(request.File, out int count) ? count + 1 : 1;
                        mostFiles = Math.Max(mostFiles, inFlight.Count);
                    }
                    await Task.Delay(20);
                    lock (inFlight)
                    {
                        if (--inFlight[request.File] == 0)
                        {
                            inFlight.Remove(request.File);
                        }
                    }
                    return UploadServer.Accept(request);
                };
                for (int i = 0; i < 6; i++)
                {
                    WriteLocal($"file{i}.bin", Pattern(24, i));
                }
                var limiter = new TransferLimiter(maxConcurrentRequests: 3);

                DirectoryTransferResult result = await Client(server).Transfers.UploadDirectoryAsync(Local, "backup", new DirectoryUploadOptions { MaxConcurrentFiles = 2, Limiter = limiter });

                Assert.AreEqual(6, server.Finishes.Count);
                Assert.IsTrue(mostFiles <= 2, $"{mostFiles} files had parts in flight at once.");
                Assert.IsTrue(server.MaxPartsInFlight <= 3, $"{server.MaxPartsInFlight} part requests at once.");
                Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes + limiter.QueuedRequests + limiter.QueuedBufferReservations);
                Assert.AreEqual(6 * 24, result.Bytes);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task FilesSharingALimiterTakeTurnsSoALargeOneCannotHoldItAll()
        {
            using (var server = new UploadServer())
            {
                server.RespondToPart = async request =>
                {
                    await Task.Delay(10);
                    return UploadServer.Accept(request);
                };
                WriteLocal("a-large.bin", Pattern(8 * 10, 1));
                WriteLocal("b-small.bin", Pattern(8 * 2, 2));

                await Client(server).Transfers.UploadDirectoryAsync(Local, "backup", new DirectoryUploadOptions { MaxConcurrentFiles = 2, Limiter = new TransferLimiter(maxConcurrentRequests: 1) });

                List<string> events = server.Events.ToList();
                int smallDone = events.IndexOf("end backup/b-small.bin");
                int largeLastPart = events.LastIndexOf("put backup/a-large.bin/10");
                Assert.IsTrue(smallDone >= 0 && smallDone < largeLastPart, "The small file completes while the large one still sends: " + string.Join(", ", events));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task FirstFailureStopsTheDirectoryAndIsThrownUnchanged()
        {
            using (var server = new UploadServer())
            {
                WriteLocal("a.bin", Pattern(16, 1));
                WriteLocal("b.bin", Pattern(16, 2));
                WriteLocal("c.bin", Pattern(16, 3));
                var failure = new InvalidOperationException("The handler for b.bin failed.");
                var options = new DirectoryUploadOptions
                {
                    MaxConcurrentFiles = 1,
                    FileOptions = new UploadOptions { Progress = new FailingProgress(report => report.Path == "backup/b.bin" ? failure : null) },
                };

                InvalidOperationException error = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => Client(server).Transfers.UploadDirectoryAsync(Local, "backup", options));

                Assert.AreSame(failure, error);
                CollectionAssert.AreEqual(new[] { "backup/a.bin" }, server.Finishes.Select(finish => finish.GetProperty("path").GetString()).ToArray(), "The file before stays completed.");
                Assert.IsFalse(server.Events.Any(e => e.Contains("c.bin")), "No file starts after the failure.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("", DisplayName = "Completed files are skipped")]
        [DataRow("changed", DisplayName = "A completed file that changed is uploaded again")]
        [DataRow("same-metadata", DisplayName = "A completed file changed keeping its length and modification time is uploaded again")]
        [DataRow("touched", DisplayName = "A completed file only touched is still skipped")]
        public async Task CancelledDirectoryUploadContinuesFromItsCheckpointDirectory(string change)
        {
            bool changeFirst = change == "changed" || change == "same-metadata";
            using (var server = new UploadServer { ParallelParts = "false" })
            using (var cancellation = new CancellationTokenSource())
            {
                byte[] a = Pattern(16, 1);
                byte[] b = Pattern(24, 2);
                byte[] c = Pattern(8, 3);
                WriteLocal("a.bin", a);
                WriteLocal("b.bin", b);
                WriteLocal("c.bin", c);
                server.RespondToPart = async request =>
                {
                    if (request.File == "backup/b.bin" && request.Number == 2 && request.Attempt == 1)
                    {
                        cancellation.Cancel();
                        await Task.Delay(100);
                        return LoopbackSite.Status(503);
                    }
                    return UploadServer.Accept(request);
                };
                var options = new DirectoryUploadOptions { MaxConcurrentFiles = 1, CheckpointDirectory = Checkpoints };
                await Assert.ThrowsAsync<OperationCanceledException>(() => Client(server).Transfers.UploadDirectoryAsync(Local, "backup", options, cancellation.Token));
                Assert.AreEqual(2, Directory.GetFiles(Checkpoints).Length, "The transfer's record and the checkpoint of the file in progress.");
                server.RespondToPart = request => Task.FromResult(UploadServer.Accept(request));
                string first = Path.Combine(Local, "a.bin");
                DateTime modified = File.GetLastWriteTimeUtc(first);
                if (changeFirst)
                {
                    a = Pattern(16, 9);
                    File.WriteAllBytes(first, a);
                    File.SetLastWriteTimeUtc(first, change == "same-metadata" ? modified : DateTime.UtcNow.AddMinutes(1));
                }
                if (change == "touched")
                {
                    File.SetLastWriteTimeUtc(first, DateTime.UtcNow.AddMinutes(1));
                }
                int sentBefore = server.Parts.Count;

                DirectoryTransferResult result = await Client(server).Transfers.UploadDirectoryAsync(Local, "backup", options);

                string[] sent = server.Parts.Skip(sentBefore).Select(part => $"{part.File}/{part.Number}").ToArray();
                CollectionAssert.AreEqual((changeFirst ? new[] { "backup/a.bin/1", "backup/a.bin/2" } : new string[0]).Concat(new[] { "backup/b.bin/2", "backup/b.bin/3", "backup/c.bin/1" }).ToArray(), sent);
                CollectionAssert.AreEqual(new[] { !changeFirst, false, false }, result.Files.Select(file => file.CompletedEarlier).ToArray());
                Assert.AreEqual(changeFirst ? 0 : 1, result.FinalProgress.FilesCompletedEarlier);
                CollectionAssert.AreEqual(a, server.Parts.Where(part => part.File == "backup/a.bin" && part.Accepted).GroupBy(part => part.Number).OrderBy(g => g.Key).SelectMany(g => g.Last().Bytes).ToArray());
                CollectionAssert.AreEqual(b, server.AcceptedBytes("backup/b.bin"));
                Assert.AreEqual("ref-backup/b.bin", server.Finishes.Last(finish => finish.GetProperty("path").GetString() == "backup/b.bin").GetProperty("ref").GetString(), "The file in progress is continued, not begun again.");
                Assert.AreEqual(0, Directory.GetFiles(Checkpoints).Length, "Nothing is left once the transfer is complete.");
            }
        }

        [TestMethod]
        [Timeout(20000)]
        public async Task AHandlerThatThrowsAsAFileEndsEndsTheTransferWithItsFailure()
        {
            using (var server = new DownloadServer(new byte[1]))
            {
                server.AddFile("folder/a", new byte[] { 1 });
                var failure = new IOException("The caller's progress handler failed.");
                var options = new DirectoryDownloadOptions
                {
                    MaxConcurrentFiles = 1,
                    Progress = new SyncProgress<DirectoryTransferProgress>(progress =>
                    {
                        if (progress.FilesDone == 1 && progress.FilesActive == 0)
                        {
                            throw failure;
                        }
                    }),
                };
                FilesClient client = Client(server);

                IOException error = await Assert.ThrowsAsync<IOException>(() => client.Transfers.DownloadDirectoryAsync("folder", Local, options));

                Assert.AreSame(failure, error, "The handler's failure is the transfer's.");
                Task closing = client.DisposeAsync().AsTask();
                Assert.AreSame(closing, await Task.WhenAny(closing, Task.Delay(TimeSpan.FromSeconds(10))), "The file gave back its slot, so nothing holds the client open.");
                CollectionAssert.AreEqual(new byte[] { 1 }, File.ReadAllBytes(Path.Combine(Local, "a")));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task ACompletedFileIsReadAgainWithinTheLimitersBufferSpace()
        {
            using (var server = new UploadServer { ParallelParts = "false" })
            using (var cancellation = new CancellationTokenSource())
            {
                WriteLocal("a.bin", Pattern(16, 1));
                WriteLocal("b.bin", Pattern(24, 2));
                server.RespondToPart = async request =>
                {
                    if (request.File == "backup/b.bin" && request.Attempt == 1)
                    {
                        cancellation.Cancel();
                        await Task.Delay(100);
                        return LoopbackSite.Status(503);
                    }
                    return UploadServer.Accept(request);
                };
                var limiter = new TransferLimiter(maxConcurrentRequests: 2, maxBufferedBytes: 8);
                var options = new DirectoryUploadOptions { MaxConcurrentFiles = 1, CheckpointDirectory = Checkpoints, Limiter = limiter };
                await Assert.ThrowsAsync<OperationCanceledException>(() => Client(server).Transfers.UploadDirectoryAsync(Local, "backup", options, cancellation.Token));
                server.RespondToPart = request => Task.FromResult(UploadServer.Accept(request));
                int requestsBefore = server.Site.Requests.Count;
                IDisposable held = await limiter.ReserveBufferAsync(8, CancellationToken.None);

                Task<DirectoryTransferResult> transfer = Client(server).Transfers.UploadDirectoryAsync(Local, "backup", options);
                while (limiter.QueuedBufferReservations == 0)
                {
                    Assert.IsFalse(transfer.IsCompleted, "The completed file was read again without buffer space from the limiter: " + transfer.Status);
                    await Task.Delay(5);
                }

                Assert.AreEqual(requestsBefore, server.Site.Requests.Count, "The transfer waits for buffer space to read the completed file again, before anything is sent.");
                held.Dispose();
                DirectoryTransferResult result = await transfer;
                CollectionAssert.AreEqual(new[] { true, false }, result.Files.Select(file => file.CompletedEarlier).ToArray());
                Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes + limiter.QueuedBufferReservations);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task DirectoryOptionsAreTakenWhenTheTransferStarts()
        {
            using (var server = new DownloadServer(new byte[1]))
            {
                server.AddFile("folder/a", new byte[] { 1 });
                server.AddFile("folder/b", new byte[] { 2 });
                var fileOptions = new DownloadOptions { PartSize = 1024 };
                var options = new DirectoryDownloadOptions { MaxConcurrentFiles = 1, FileOptions = fileOptions };
                int reports = 0;
                options.Progress = new DirectoryProgressLogThen(() =>
                {
                    reports++;
                    // Changed while the transfer runs: it keeps what it started with.
                    options.MaxConcurrentFiles = 0;
                    options.FileOptions = null;
                    fileOptions.PartSize = 0;
                    fileOptions.MaxConcurrency = 0;
                });

                DirectoryTransferResult result = await Client(server).Transfers.DownloadDirectoryAsync("folder", Local, options);

                Assert.AreEqual(2, result.Files.Count);
                Assert.IsTrue(reports > 0);
                CollectionAssert.AreEqual(new byte[] { 2 }, File.ReadAllBytes(Path.Combine(Local, "b")));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task DirectoryTransfersRefuseSettingsThatCannotApply()
        {
            using (var server = new UploadServer())
            {
                WriteLocal("a.bin", Pattern(8, 1));
                FilesClient client = Client(server);

                Assert.AreEqual(nameof(DirectoryTransferOptions.CheckpointDirectory), (await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => client.Transfers.UploadDirectoryAsync(Local, "backup", new DirectoryUploadOptions { CheckpointDirectory = Path.Combine(Local, "checkpoints") }))).ParamName);
                Assert.AreEqual(nameof(DirectoryTransferOptions.CheckpointDirectory), (await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => client.Transfers.DownloadDirectoryAsync("backup", Local, new DirectoryDownloadOptions { CheckpointDirectory = Local }))).ParamName);
                Assert.AreEqual(nameof(DirectoryUploadOptions.FileOptions), (await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => client.Transfers.UploadDirectoryAsync(Local, "backup", new DirectoryUploadOptions { FileOptions = new UploadOptions { Limiter = new TransferLimiter() } }))).ParamName);
                await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => client.Transfers.UploadDirectoryAsync(Local, "backup", new DirectoryUploadOptions { FileOptions = new UploadOptions { ModifiedTime = DateTime.UtcNow } }));
                await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => client.Transfers.DownloadDirectoryAsync("backup", Local, new DirectoryDownloadOptions { FileOptions = new DownloadOptions { CheckpointPath = "x" } }));
                Assert.AreEqual(nameof(DirectoryTransferOptions.MaxConcurrentFiles), (await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => client.Transfers.UploadDirectoryAsync(Local, "backup", new DirectoryUploadOptions { MaxConcurrentFiles = 0 }))).ParamName);

                Assert.AreEqual(0, server.Site.Requests.Count, "Nothing is sent.");
                Assert.IsFalse(Directory.Exists(Path.Combine(Local, "checkpoints")));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task DirectoryDownloadWritesEveryFileAndEmptyFolderUnderTheLocalDirectory()
        {
            using (var server = new DownloadServer(new byte[1]) { ListPageSize = 1 })
            {
                byte[] a = Pattern(5000, 1);
                byte[] b = Pattern(700, 2);
                server.AddFile("backup/a.bin", a);
                server.AddFile("backup/sub/b.bin", b);
                server.AddFile("backup/sub/deeper/c.bin", new byte[0]);
                server.AddFolder("backup/empty");
                server.AddFile("other/d.bin", Pattern(10, 4));
                var progress = new DirectoryProgressLog();

                DirectoryTransferResult result = await Client(server).Transfers.DownloadDirectoryAsync("backup", Local, new DirectoryDownloadOptions
                {
                    MaxConcurrentFiles = 2,
                    Progress = progress,
                    FileOptions = new DownloadOptions { PartSize = 1024, MaxConcurrency = 2 },
                });

                CollectionAssert.AreEqual(a, File.ReadAllBytes(Path.Combine(Local, "a.bin")));
                CollectionAssert.AreEqual(b, File.ReadAllBytes(Path.Combine(Local, "sub", "b.bin")));
                Assert.AreEqual(0, new FileInfo(Path.Combine(Local, "sub", "deeper", "c.bin")).Length);
                Assert.IsTrue(Directory.Exists(Path.Combine(Local, "empty")));
                CollectionAssert.AreEquivalent(new[] { "a.bin", "c.bin", "b.bin" }, Directory.GetFiles(Local, "*", SearchOption.AllDirectories).Select(Path.GetFileName).ToArray(), "Only the folder's files, and no stage, are left.");
                CollectionAssert.AreEqual(new[] { "a.bin", "sub/b.bin", "sub/deeper/c.bin" }, result.Files.Select(file => file.RelativePath).ToArray());
                CollectionAssert.AreEqual(new[] { "empty" }, result.Directories.ToArray());
                Assert.IsTrue(server.ApiRequests.Count(request => request.StartsWith("/api/rest/v1/folders/backup?", StringComparison.Ordinal) || request == "/api/rest/v1/folders/backup") > 1, "The listing is read a page at a time.");
                Assert.IsTrue(server.StorageRequests.Where(request => request.Line.Contains("a.bin")).All(request => request.Range.HasValue), "Each file is downloaded as a file is, in parts.");
                DirectoryTransferProgress last = progress.Reports.Last();
                Assert.AreEqual(3, last.FilesDone);
                Assert.AreEqual(5700, last.BytesDone);
                Assert.AreEqual(5700, last.BytesTotal);
                Assert.AreEqual(5700, last.NetworkBytes);
                Assert.AreEqual(5700, result.Bytes);
            }
        }

        // On macOS and Linux a local directory this user may add files to and search, but not list (0300), gets a folder's
        // files all the same, in it and in folders the download creates there.
        [TestMethod]
        [Timeout(60000)]
        public async Task DirectoryDownloadWritesIntoALocalDirectoryThatMayNotBeListed()
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("Unix permissions only.");
                return;
            }
            if (Native.GetEuid() == 0)
            {
                Assert.Inconclusive("The superuser may list any folder.");
                return;
            }
            Directory.CreateDirectory(Local);
            using (var server = new DownloadServer(new byte[1]))
            {
                byte[] a = Pattern(5000, 1);
                byte[] b = Pattern(700, 2);
                server.AddFile("backup/a.bin", a);
                server.AddFile("backup/sub/b.bin", b);
                File.SetUnixFileMode(Local, UnixFileMode.UserWrite | UnixFileMode.UserExecute);
                DirectoryTransferResult result;
                try
                {
                    Assert.ThrowsExactly<UnauthorizedAccessException>(() => Directory.GetFileSystemEntries(Local), "The directory cannot be listed.");
                    result = await Client(server).Transfers.DownloadDirectoryAsync("backup", Local, new DirectoryDownloadOptions { FileOptions = new DownloadOptions { PartSize = 1024 } });
                }
                finally
                {
                    File.SetUnixFileMode(Local, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
                }

                CollectionAssert.AreEqual(a, File.ReadAllBytes(Path.Combine(Local, "a.bin")));
                CollectionAssert.AreEqual(b, File.ReadAllBytes(Path.Combine(Local, "sub", "b.bin")));
                CollectionAssert.AreEquivalent(new[] { "a.bin", "sub" }, Directory.GetFileSystemEntries(Local).Select(Path.GetFileName).ToArray(), "No stage is left behind.");
                CollectionAssert.AreEqual(new[] { "a.bin", "sub/b.bin" }, result.Files.Select(file => file.RelativePath).ToArray());
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("..", DisplayName = "A name that climbs out")]
        [DataRow(".", DisplayName = "A name that stays put")]
        public async Task RemoteNamesThatCannotBeLocalNamesFailBeforeAnythingIsWritten(string name)
        {
            using (var server = new DownloadServer(new byte[1]))
            {
                server.AddFile("backup/a.bin", Pattern(10, 1));
                server.AddFile("backup/" + name, Pattern(10, 2));

                IOException error = await Assert.ThrowsExactlyAsync<IOException>(() => Client(server).Transfers.DownloadDirectoryAsync("backup", Local));

                StringAssert.Contains(error.Message, $"\"{name}\"");
                CollectionAssert.AreEqual(new string[0], Directory.GetFileSystemEntries(Local), "Nothing is written.");
                Assert.AreEqual(0, server.StorageRequests.Count);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task SymbolicLinkWhereAFolderBelongsIsNotFollowed()
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("Creating symbolic links needs a privilege on Windows.");
            }
            using (var server = new DownloadServer(new byte[1]))
            {
                server.AddFile("backup/sub/b.bin", Pattern(10, 2));
                string outside = Path.Combine(folder, "outside");
                Directory.CreateDirectory(outside);
                Directory.CreateDirectory(Local);
                Directory.CreateSymbolicLink(Path.Combine(Local, "sub"), outside);

                IOException error = await Assert.ThrowsExactlyAsync<IOException>(() => Client(server).Transfers.DownloadDirectoryAsync("backup", Local));

                StringAssert.Contains(error.Message, "symbolic link");
                CollectionAssert.AreEqual(new string[0], Directory.GetFileSystemEntries(outside), "Nothing is written through the link.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("", DisplayName = "Completed files are skipped")]
        [DataRow("remote", DisplayName = "A completed file changed remotely is downloaded again")]
        [DataRow("remote-same-metadata", DisplayName = "A completed file changed remotely, keeping its size and modification time, is downloaded again")]
        [DataRow("unbound", DisplayName = "A completed file downloaded without a version is downloaded again")]
        [DataRow("unbindable", DisplayName = "A completed file Files.com now gives from storage that cannot be bound is downloaded again")]
        [DataRow("local", DisplayName = "A completed file changed locally is downloaded again")]
        [DataRow("legacy", DisplayName = "A completed file an earlier SDK version recorded, by its length and modification time, is skipped by default")]
        public async Task CancelledDirectoryDownloadContinuesFromItsCheckpointDirectory(string change)
        {
            using (var server = new DownloadServer(new byte[1]))
            using (var cancellation = new CancellationTokenSource())
            {
                byte[] a = Pattern(2048, 1);
                byte[] b = Pattern(4096, 2);
                byte[] c = Pattern(100, 3);
                server.AddFile("backup/a.bin", a);
                server.AddFile("backup/b.bin", b);
                server.AddFile("backup/c.bin", c);
                // Without versions, files come whole: b is stopped in its one request instead.
                server.Bound = change != "unbound";
                server.Intercept = (request, connection) =>
                {
                    bool stopHere = change == "unbound" ? request.Range == null : request.Range?.First == 2048;
                    if (FileOf(request) == "backup/b.bin" && stopHere && !cancellation.IsCancellationRequested)
                    {
                        // Held unanswered until the download gives up on it.
                        cancellation.Cancel();
                        return Task.FromResult(true);
                    }
                    return Task.FromResult(false);
                };
                var options = new DirectoryDownloadOptions { MaxConcurrentFiles = 1, CheckpointDirectory = Checkpoints, FileOptions = new DownloadOptions { PartSize = 1024, MaxConcurrency = 1 } };
                await Assert.ThrowsAsync<OperationCanceledException>(() => Client(server).Transfers.DownloadDirectoryAsync("backup", Local, options, cancellation.Token));
                Assert.IsFalse(File.Exists(Path.Combine(Local, "b.bin")), "The file in progress is not published.");
                switch (change)
                {
                    case "remote":
                    case "remote-same-metadata":
                        a = Pattern(2048, 7);
                        server.ChangeFile("backup/a.bin", a, keepModified: change == "remote-same-metadata");
                        break;
                    case "unbound":
                        server.Bound = true;
                        break;
                    case "unbindable":
                        // Its URLs now name storage the SDK cannot bind, not the resource the versions came from.
                        server.Bound = false;
                        break;
                    case "local":
                        File.WriteAllBytes(Path.Combine(Local, "a.bin"), Pattern(10, 8));
                        break;
                    case "legacy":
                        RecordAsAnEarlierVersion();
                        break;
                }
                int storageBefore = server.StorageRequests.Count;

                DirectoryTransferResult result = await Client(server).Transfers.DownloadDirectoryAsync("backup", Local, options);

                string[] fetched = Fetched(server, storageBefore);
                // A completed file is confirmed by storage, for its first byte, unless it has no version to confirm, or its
                // size or local file already show it changed; a changed one is fetched again.
                string[] confirmed = change == "" || change == "legacy" || change.StartsWith("remote", StringComparison.Ordinal) ? new[] { "backup/a.bin 0-0" } : new string[0];
                string[] again = change.Length > 0 && change != "legacy" ? new[] { "backup/a.bin 0-1023", "backup/a.bin 1024-2047" } : new string[0];
                // A file stopped while it came whole has nothing to continue; one stopped in parts is confirmed, then continued.
                string[] rest = change == "unbound" ? new[] { "backup/b.bin 0-1023", "backup/b.bin 1024-2047", "backup/b.bin 2048-3071", "backup/b.bin 3072-4095" } : new[] { "backup/b.bin 0-0", "backup/b.bin 2048-3071", "backup/b.bin 3072-4095" };
                string[] expected = confirmed.Concat(again).Concat(rest).Concat(new[] { "backup/c.bin 0-1023" }).ToArray();
                if (change == "unbindable")
                {
                    // Not bound, every file comes whole: a again, and b, whose URL now names other storage, from its start.
                    expected = new[] { "backup/a.bin whole", "backup/b.bin whole", "backup/c.bin whole" };
                }
                CollectionAssert.AreEqual(expected, fetched, "Only what was not done, or cannot be shown unchanged, is fetched.");
                CollectionAssert.AreEqual(a, File.ReadAllBytes(Path.Combine(Local, "a.bin")));
                CollectionAssert.AreEqual(b, File.ReadAllBytes(Path.Combine(Local, "b.bin")));
                CollectionAssert.AreEqual(c, File.ReadAllBytes(Path.Combine(Local, "c.bin")));
                CollectionAssert.AreEqual(new[] { change.Length == 0 || change == "legacy", false, false }, result.Files.Select(file => file.CompletedEarlier).ToArray());
                Assert.AreEqual(0, Directory.GetFiles(Checkpoints).Length);
            }
        }

        // With CreateNew and a checkpoint directory, running a stopped download again reports the files it completed as
        // completed earlier, and continues the one in progress; a completed file replaced by another, even of the same
        // length and modification time, is not taken for it: CreateNew refuses it, and the default downloads it again.
        // A record an earlier SDK version wrote, which names no file's identity, shows no file to be this transfer's own
        // either: CreateNew refuses the file there, and ReplaceRegularFile downloads it again. Nor, on Linux, does one
        // whose identities hold no time of creation, as an earlier version recorded them there: another file put in a
        // completed file's place once it was removed, which may be given its number, is refused, then replaced by default.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("own", DisplayName = "Its own completed files are completed earlier")]
        [DataRow("foreign", DisplayName = "Another file in a completed file's place is refused, then replaced by default")]
        [DataRow("legacy", DisplayName = "Another file in the place of a file an earlier SDK version recorded is refused, then replaced by ReplaceRegularFile")]
        [DataRow("numbers", DisplayName = "Another file in the place of a file recorded without its time of creation is refused, then replaced by default")]
        public async Task ADirectoryDownloadWithCreateNewCompletesItsOwnFilesOnly(string state)
        {
            if (state == "numbers" && !OperatingSystem.IsLinux())
            {
                Assert.Inconclusive("Only Linux identities hold the time a file was created.");
                return;
            }
            using (var server = new DownloadServer(new byte[1]))
            using (var cancellation = new CancellationTokenSource())
            {
                byte[] a = Pattern(2048, 1);
                byte[] b = Pattern(4096, 2);
                byte[] c = Pattern(100, 3);
                server.AddFile("backup/a.bin", a);
                server.AddFile("backup/b.bin", b);
                server.AddFile("backup/c.bin", c);
                server.Intercept = (request, connection) =>
                {
                    if (FileOf(request) == "backup/b.bin" && request.Range?.First == 2048 && !cancellation.IsCancellationRequested)
                    {
                        cancellation.Cancel();
                        return Task.FromResult(true);
                    }
                    return Task.FromResult(false);
                };
                var options = new DirectoryDownloadOptions { MaxConcurrentFiles = 1, CheckpointDirectory = Checkpoints, FileOptions = new DownloadOptions { PartSize = 1024, MaxConcurrency = 1, PublicationPolicy = DownloadPublicationPolicy.CreateNew } };
                await Assert.ThrowsAsync<OperationCanceledException>(() => Client(server).Transfers.DownloadDirectoryAsync("backup", Local, options, cancellation.Token));
                string localA = Path.Combine(Local, "a.bin");
                byte[] foreign = Pattern(2048, 9);
                if (state == "legacy")
                {
                    RecordAsAnEarlierVersion();
                }
                if (state == "numbers")
                {
                    RecordWithoutCreationTimes(Path.Combine(Checkpoints, "transfer.json"));
                    // a removed first, so that a new file may be given its number; with a's time.
                    DateTime modified = File.GetLastWriteTimeUtc(localA);
                    File.Delete(localA);
                    File.WriteAllBytes(localA, foreign);
                    File.SetLastWriteTimeUtc(localA, modified);
                }
                else if (state != "own")
                {
                    // A new file, while a still exists, so it cannot be given a's number; with a's time.
                    string other = Path.Combine(folder, "other.bin");
                    File.WriteAllBytes(other, foreign);
                    File.SetLastWriteTimeUtc(other, File.GetLastWriteTimeUtc(localA));
                    File.Move(other, localA, overwrite: true);
                }
                int storageBefore = server.StorageRequests.Count;

                if (state != "own")
                {
                    DownloadPublicationException error = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadDirectoryAsync("backup", Local, options));
                    Assert.AreEqual(DownloadPublicationProblem.DestinationExists, error.Problem);
                    Assert.AreEqual(localA, error.LocalPath);
                    CollectionAssert.AreEqual(foreign, File.ReadAllBytes(localA), "The other file is left as it is.");
                    options.FileOptions.PublicationPolicy = state == "legacy" ? DownloadPublicationPolicy.ReplaceRegularFile : DownloadPublicationPolicy.ReplaceExisting;
                    storageBefore = server.StorageRequests.Count;
                }
                DirectoryTransferResult result = await Client(server).Transfers.DownloadDirectoryAsync("backup", Local, options);

                string[] fetched = Fetched(server, storageBefore);
                string[] again = state != "own" ? new[] { "backup/a.bin 0-1023", "backup/a.bin 1024-2047" } : new[] { "backup/a.bin 0-0" };
                CollectionAssert.AreEqual(again.Concat(new[] { "backup/b.bin 0-0", "backup/b.bin 2048-3071", "backup/b.bin 3072-4095", "backup/c.bin 0-1023" }).ToArray(), fetched);
                CollectionAssert.AreEqual(a, File.ReadAllBytes(localA));
                CollectionAssert.AreEqual(b, File.ReadAllBytes(Path.Combine(Local, "b.bin")));
                CollectionAssert.AreEqual(c, File.ReadAllBytes(Path.Combine(Local, "c.bin")));
                CollectionAssert.AreEqual(new[] { state == "own", false, false }, result.Files.Select(file => file.CompletedEarlier).ToArray());
                Assert.AreEqual(0, Directory.GetFiles(Checkpoints).Length);
            }
        }

        // A file put in place whose completion the transfer could not record keeps its own checkpoint, from which the next
        // run recognizes it as this transfer's file, without fetching it; it is recorded and reported completed earlier
        // only once its storage confirms it is still the file's current version. One that changed since, even to the same
        // size, or that was downloaded without a version, is downloaded again as the policy says: CreateNew refuses to
        // replace it, and leaves it as it is; ReplaceRegularFile and the default replace it with the file as it is now.
        // Finishing what the earlier run left removes only its own staging directory: another put at its name stays. On
        // Linux, a checkpoint whose identities hold no time of creation, as an earlier version of this SDK recorded them
        // there, shows neither the file nor the staging directory to be the earlier run's (another directory at its name
        // may have its number): CreateNew refuses the file, fetching nothing of it and leaving its checkpoint as it was,
        // and ReplaceRegularFile downloads it again; neither removes the directory at the staging directory's name.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("", DownloadPublicationPolicy.CreateNew, false, DisplayName = "Its own unchanged file is completed earlier")]
        [DataRow("same size", DownloadPublicationPolicy.CreateNew, false, DisplayName = "A file changed to the same size is refused by CreateNew")]
        [DataRow("other size", DownloadPublicationPolicy.CreateNew, false, DisplayName = "A file changed to another size is refused by CreateNew")]
        [DataRow("unbound", DownloadPublicationPolicy.CreateNew, false, DisplayName = "A file downloaded without a version is refused by CreateNew")]
        [DataRow("same size", DownloadPublicationPolicy.ReplaceRegularFile, false, DisplayName = "A file changed to the same size is downloaded again by ReplaceRegularFile")]
        [DataRow("unbound", DownloadPublicationPolicy.ReplaceExisting, false, DisplayName = "A file downloaded without a version is downloaded again by default")]
        [DataRow("", DownloadPublicationPolicy.CreateNew, true, DisplayName = "Its own unchanged file, recorded without times of creation, is refused by CreateNew")]
        [DataRow("same size", DownloadPublicationPolicy.ReplaceRegularFile, true, DisplayName = "A file recorded without times of creation, changed to the same size, is downloaded again by ReplaceRegularFile")]
        public async Task AFilePutInPlaceButNotRecordedIsRecognizedByItsCheckpoint(string change, DownloadPublicationPolicy policy, bool withoutCreationTimes)
        {
            if (withoutCreationTimes && !OperatingSystem.IsLinux())
            {
                Assert.Inconclusive("Only Linux identities hold the time a file was created.");
                return;
            }
            using (var server = new DownloadServer(new byte[1]) { Bound = change != "unbound" })
            {
                byte[] a = Pattern(2048, 1);
                byte[] b = Pattern(100, 2);
                server.AddFile("backup/a.bin", a);
                server.AddFile("backup/b.bin", b);
                var options = new DirectoryDownloadOptions { MaxConcurrentFiles = 1, CheckpointDirectory = Checkpoints, FileOptions = new DownloadOptions { PartSize = 1024, MaxConcurrency = 1, PublicationPolicy = policy } };
                DirectoryTransfer.RecordFaults.Value = path => path == "a.bin" ? new IOException("Recording failed.") : null;
                DownloadPublicationException error;
                try
                {
                    error = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadDirectoryAsync("backup", Local, options));
                }
                finally
                {
                    DirectoryTransfer.RecordFaults.Value = null;
                }
                string localA = Path.Combine(Local, "a.bin");
                Assert.AreEqual(DownloadPublicationProblem.PlacedNotRecorded, error.Problem);
                Assert.IsTrue(error.Placed);
                Assert.AreEqual(localA, error.LocalPath);
                CollectionAssert.AreEqual(a, File.ReadAllBytes(localA), "The file stays in place.");
                Assert.IsTrue(File.Exists(error.CheckpointPath), "Its own checkpoint stays.");
                string staging = Path.Combine(Local, JsonNode.Parse(File.ReadAllText(error.CheckpointPath))["stage_directory"].GetValue<string>());
                Assert.IsFalse(Directory.Exists(staging), "The staging directory went with the stage.");
                if (withoutCreationTimes)
                {
                    RecordWithoutCreationTimes(error.CheckpointPath);
                }
                string recorded = File.ReadAllText(error.CheckpointPath);
                Directory.CreateDirectory(staging);
                byte[] current = change == "same size" ? Pattern(2048, 5) : change == "other size" ? Pattern(3000, 5) : a;
                if (current != a)
                {
                    server.ChangeFile("backup/a.bin", current);
                }
                int storageBefore = server.StorageRequests.Count;

                if ((change.Length > 0 || withoutCreationTimes) && policy == DownloadPublicationPolicy.CreateNew)
                {
                    DownloadPublicationException refused = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadDirectoryAsync("backup", Local, options));

                    Assert.AreEqual(DownloadPublicationProblem.DestinationExists, refused.Problem, refused.Message);
                    Assert.AreEqual(localA, refused.LocalPath);
                    CollectionAssert.AreEqual(a, File.ReadAllBytes(localA), "The file it cannot count as current is left as it is.");
                    Assert.IsTrue(Fetched(server, storageBefore).Where(request => request.StartsWith("backup/a.bin ", StringComparison.Ordinal)).All(request => request == "backup/a.bin 0-0"), "Nothing of it is fetched: storage is at most asked to confirm it.");
                    if (withoutCreationTimes)
                    {
                        Assert.AreEqual(recorded, File.ReadAllText(error.CheckpointPath), "Nothing shows the file to be the earlier run's, so its checkpoint is left as it was.");
                    }
                    else
                    {
                        Assert.IsFalse(File.Exists(error.CheckpointPath), "The earlier run is done with.");
                    }
                    Assert.IsTrue(Directory.Exists(staging), "The directory put at the staging directory's name is not the one the earlier run created.");
                    return;
                }
                DirectoryTransferResult result = await Client(server).Transfers.DownloadDirectoryAsync("backup", Local, options);

                string[] fetched = Fetched(server, storageBefore);
                // Recognized, a is first confirmed; not shown to be the earlier run's, it is only fetched again.
                string[] again = change.Length == 0 ? new[] { "backup/a.bin 0-0" } : change == "unbound" ? new[] { "backup/a.bin whole" }
                    : withoutCreationTimes ? new[] { "backup/a.bin 0-1023", "backup/a.bin 1024-2047" } : new[] { "backup/a.bin 0-0", "backup/a.bin 0-1023", "backup/a.bin 1024-2047" };
                string[] rest = change == "unbound" ? new[] { "backup/b.bin whole" } : new[] { "backup/b.bin 0-1023" };
                CollectionAssert.AreEqual(again.Concat(rest).ToArray(), fetched, change.Length == 0 ? "a is not fetched again." : "a is fetched again as it is now.");
                CollectionAssert.AreEqual(new[] { change.Length == 0, false }, result.Files.Select(file => file.CompletedEarlier).ToArray());
                CollectionAssert.AreEqual(current, File.ReadAllBytes(localA));
                CollectionAssert.AreEqual(b, File.ReadAllBytes(Path.Combine(Local, "b.bin")));
                Assert.AreEqual(0, Directory.GetFiles(Checkpoints).Length);
                Assert.IsTrue(Directory.Exists(staging), "The directory put at the staging directory's name is not the one the earlier run created.");
            }
        }

        // On macOS and Linux a checkpointed run opens its local directory before it lists the remote folder. Renamed while
        // the listing is read, and replaced by another at its path, with every file completed earlier, the directory it
        // opened is not taken for the one at the path: nothing is done in either, the files stay where they are, and the
        // record is kept, so that once the directory is back at its path the transfer completes without fetching them.
        [TestMethod]
        [Timeout(60000)]
        public async Task ACompletedDownloadWhoseFolderMovesWhileItIsListedIsNotReportedWhereItIsNot()
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("On Windows a folder download's folder is used by its path, or held so that it cannot be renamed.");
                return;
            }
            using (var server = new DownloadServer(new byte[1]))
            {
                byte[] a = Pattern(2048, 1);
                byte[] b = Pattern(100, 2);
                server.AddFile("backup/a.bin", a);
                server.AddFile("backup/sub/b.bin", b);
                await CompleteEveryFileAndKeepTheRecord(server, 2);
                string moved = Local + "-moved";
                int storageBefore = server.StorageRequests.Count;
                int apiBefore = server.ApiRequests.Count;
                server.OnApiRequest = target =>
                {
                    if (target.StartsWith("/api/rest/v1/folders/backup", StringComparison.Ordinal) && !Directory.Exists(moved))
                    {
                        Directory.Move(Local, moved);
                        Directory.CreateDirectory(Local);
                    }
                };

                DownloadPublicationException error = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadDirectoryAsync("backup", Local, WithCheckpoints()));

                Assert.AreEqual(DownloadPublicationProblem.DestinationMoved, error.Problem);
                Assert.IsFalse(error.Placed, "Nothing is done in the directory the transfer opened.");
                Assert.AreEqual(Path.GetFullPath(Local), error.LocalPath);
                CollectionAssert.AreEqual(new string[0], Directory.GetFileSystemEntries(Local), "Nothing is put in the directory now at the path.");
                CollectionAssert.AreEqual(a, File.ReadAllBytes(Path.Combine(moved, "a.bin")));
                CollectionAssert.AreEqual(b, File.ReadAllBytes(Path.Combine(moved, "sub", "b.bin")));
                Assert.AreEqual(storageBefore, server.StorageRequests.Count, "Nothing is fetched, nor any version confirmed.");
                Assert.IsFalse(server.ApiRequests.Skip(apiBefore).Any(request => request.StartsWith("/api/rest/v1/files/", StringComparison.Ordinal)), "No completed file is looked at.");
                CollectionAssert.AreEqual(new[] { "a.bin", "sub/b.bin" }, RecordedFiles(), "The record is kept.");

                server.OnApiRequest = null;
                Directory.Delete(Local);
                Directory.Move(moved, Local);
                DirectoryTransferResult result = await Client(server).Transfers.DownloadDirectoryAsync("backup", Local, WithCheckpoints());

                CollectionAssert.AreEqual(new[] { true, true }, result.Files.Select(file => file.CompletedEarlier).ToArray());
                CollectionAssert.AreEqual(new[] { "backup/a.bin 0-0", "backup/sub/b.bin 0-0" }, Fetched(server, storageBefore), "Nothing is fetched again: storage only confirms each version.");
                Assert.AreEqual(0, Directory.GetFiles(Checkpoints).Length);
            }
        }

        // The same directory renamed and replaced after the listing was read, while the version of a completed file is
        // being confirmed: that file, found in the directory the transfer opened, is in place there, not where the transfer
        // would report it, and the transfer fails saying so, leaving the files and the record as they are.
        [TestMethod]
        [Timeout(60000)]
        public async Task ACompletedFileWhoseFolderMovesWhileItsVersionIsConfirmedIsNotReportedWhereItIsNot()
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("On Windows a folder download's folder is used by its path, or held so that it cannot be renamed.");
                return;
            }
            using (var server = new DownloadServer(new byte[1]))
            {
                byte[] a = Pattern(2048, 1);
                byte[] b = Pattern(100, 2);
                server.AddFile("backup/a.bin", a);
                server.AddFile("backup/sub/b.bin", b);
                await CompleteEveryFileAndKeepTheRecord(server, 2);
                string moved = Local + "-moved";
                int storageBefore = server.StorageRequests.Count;
                server.OnApiRequest = target =>
                {
                    if (target.StartsWith("/api/rest/v1/files/", StringComparison.Ordinal) && !Directory.Exists(moved))
                    {
                        Directory.Move(Local, moved);
                        Directory.CreateDirectory(Local);
                    }
                };

                DownloadPublicationException error = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadDirectoryAsync("backup", Local, WithCheckpoints()));

                Assert.AreEqual(DownloadPublicationProblem.DestinationMoved, error.Problem);
                Assert.IsTrue(error.Placed, "The file is in place, in the directory the transfer opened.");
                Assert.AreEqual(Path.Combine(Path.GetFullPath(Local), "a.bin"), error.LocalPath);
                StringAssert.Contains(error.Message, $"so the file is not at {error.LocalPath}");
                CollectionAssert.AreEqual(new string[0], Directory.GetFileSystemEntries(Local), "Nothing is put in the directory now at the path.");
                CollectionAssert.AreEqual(a, File.ReadAllBytes(Path.Combine(moved, "a.bin")));
                CollectionAssert.AreEqual(b, File.ReadAllBytes(Path.Combine(moved, "sub", "b.bin")));
                CollectionAssert.AreEqual(new[] { "backup/a.bin 0-0" }, Fetched(server, storageBefore), "Nothing is fetched: storage only confirms a's version.");
                CollectionAssert.AreEqual(new[] { "a.bin", "sub/b.bin" }, RecordedFiles(), "The record is kept.");
            }
        }

        // A folder download continued from its checkpoint directory, with one file completed earlier and another stopped
        // partway: the completed file fails the transfer (its folder moves while its version is confirmed) while the
        // other file's version is being confirmed by storage. That file has learned nothing of its version: its checkpoint and stage
        // stay as they were, and once the folder is back, the transfer fetches only the parts that file had not written.
        [TestMethod]
        [Timeout(60000)]
        public async Task AFileWhoseVersionIsBeingConfirmedWhenAnotherFailsTheTransferKeepsWhatItHad()
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("On Windows a folder download's folder is used by its path, or held so that it cannot be renamed.");
                return;
            }
            using (var server = new DownloadServer(new byte[1]))
            using (var cancellation = new CancellationTokenSource())
            {
                byte[] a = Pattern(2048, 1);
                byte[] b = Pattern(4096, 2);
                server.AddFile("backup/a.bin", a);
                server.AddFile("backup/b.bin", b);
                server.Intercept = (request, connection) =>
                {
                    if (FileOf(request) == "backup/b.bin" && request.Range?.First == 2048 && !cancellation.IsCancellationRequested)
                    {
                        // Held unanswered until the download gives up on it.
                        cancellation.Cancel();
                        return Task.FromResult(true);
                    }
                    return Task.FromResult(false);
                };
                await Assert.ThrowsAsync<OperationCanceledException>(() => Client(server).Transfers.DownloadDirectoryAsync("backup", Local, WithCheckpoints(), cancellation.Token));
                server.Intercept = null;
                CollectionAssert.AreEqual(new[] { "a.bin" }, RecordedFiles(), "a is completed; b is not.");
                string checkpointOfB = Directory.GetFiles(Checkpoints, "*.download.json").Single();
                string recorded = File.ReadAllText(checkpointOfB);
                int storageBefore = server.StorageRequests.Count;
                string moved = Local + "-moved";
                var confirmingB = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                server.Intercept = async (request, connection) =>
                {
                    if (FileOf(request) != "backup/b.bin" || request.Range != (0, 0))
                    {
                        return false;
                    }
                    // b's version is being confirmed, and is not answered until the transfer gives up on it.
                    confirmingB.TrySetResult(true);
                    await connection.HoldAsync();
                    return true;
                };
                server.ApiIntercept = async (target, connection) =>
                {
                    if (!target.StartsWith("/api/rest/v1/files/", StringComparison.Ordinal) || !target.Contains("a.bin"))
                    {
                        return false;
                    }
                    // Once b's version is being confirmed, a's folder moves, and a is answered as usual.
                    await Task.WhenAny(confirmingB.Task, Task.Delay(10000));
                    Directory.Move(Local, moved);
                    Directory.CreateDirectory(Local);
                    return false;
                };
                var twoAtOnce = new DirectoryDownloadOptions { MaxConcurrentFiles = 2, CheckpointDirectory = Checkpoints, FileOptions = new DownloadOptions { PartSize = 1024, MaxConcurrency = 1 } };

                DownloadPublicationException error = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadDirectoryAsync("backup", Local, twoAtOnce));

                Assert.AreEqual(DownloadPublicationProblem.DestinationMoved, error.Problem);
                Assert.AreEqual(Path.Combine(Path.GetFullPath(Local), "a.bin"), error.LocalPath);
                Assert.IsTrue(confirmingB.Task.IsCompleted, "b's version was being confirmed.");
                Assert.AreEqual(recorded, File.ReadAllText(checkpointOfB), "b's checkpoint is left as it was.");
                string[] staged = Directory.GetDirectories(moved, ".files-download-*").SelectMany(Directory.GetFiles).ToArray();
                Assert.AreEqual(1, staged.Length, "b's stage is left where it was, in the folder the transfer opened.");
                CollectionAssert.AreEqual(b.Take(2048).ToArray(), File.ReadAllBytes(staged[0]));
                CollectionAssert.AreEquivalent(new[] { "backup/a.bin 0-0", "backup/b.bin 0-0" }, Fetched(server, storageBefore), "Nothing is fetched: storage is only asked to confirm each version.");

                server.Intercept = null;
                server.ApiIntercept = null;
                storageBefore = server.StorageRequests.Count;
                Directory.Delete(Local);
                Directory.Move(moved, Local);
                DirectoryTransferResult result = await Client(server).Transfers.DownloadDirectoryAsync("backup", Local, WithCheckpoints());

                CollectionAssert.AreEqual(new[] { "backup/a.bin 0-0", "backup/b.bin 0-0", "backup/b.bin 2048-3071", "backup/b.bin 3072-4095" }, Fetched(server, storageBefore), "a's and b's versions confirmed, then only the parts of b not yet written.");
                CollectionAssert.AreEqual(a, File.ReadAllBytes(Path.Combine(Local, "a.bin")));
                CollectionAssert.AreEqual(b, File.ReadAllBytes(Path.Combine(Local, "b.bin")));
                CollectionAssert.AreEqual(new[] { true, false }, result.Files.Select(file => file.CompletedEarlier).ToArray());
                Assert.AreEqual(0, Directory.GetFiles(Checkpoints).Length);
            }
        }

        // A local file where a remote folder belongs fails the files below it and is left as it is; files already
        // completed stay, and an empty folder already there is used as it is.
        [TestMethod]
        [Timeout(60000)]
        public async Task ALocalFileWhereAFolderBelongsIsLeftAndCompletedFilesStay()
        {
            using (var server = new DownloadServer(new byte[1]))
            {
                byte[] a = Pattern(100, 1);
                server.AddFile("backup/a.bin", a);
                server.AddFolder("backup/empty");
                server.AddFile("backup/sub/b.bin", Pattern(100, 2));
                Directory.CreateDirectory(Path.Combine(Local, "empty"));
                WriteLocal("sub", Pattern(5, 3));
                var options = new DirectoryDownloadOptions { MaxConcurrentFiles = 1, FileOptions = new DownloadOptions { PublicationPolicy = DownloadPublicationPolicy.CreateNew } };

                await Assert.ThrowsAsync<IOException>(() => Client(server).Transfers.DownloadDirectoryAsync("backup", Local, options));

                CollectionAssert.AreEqual(Pattern(5, 3), File.ReadAllBytes(Path.Combine(Local, "sub")), "The file is left as it is.");
                CollectionAssert.AreEqual(a, File.ReadAllBytes(Path.Combine(Local, "a.bin")), "The file completed first stays.");
                Assert.IsTrue(Directory.Exists(Path.Combine(Local, "empty")));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task CheckpointDirectoryOfAnotherTransferIsRefusedBeforeAnythingIsDone()
        {
            using (var server = new DownloadServer(new byte[1]))
            {
                server.AddFile("backup/a.bin", Pattern(10, 1));
                server.AddFile("other/a.bin", Pattern(10, 2));
                Directory.CreateDirectory(Checkpoints);
                var options = new DirectoryDownloadOptions { CheckpointDirectory = Checkpoints };
                string record = Path.Combine(Checkpoints, "transfer.json");
                File.WriteAllText(record, "{\"format\":\"files-sdk-dotnet-transfer-checkpoint\",\"version\":1,\"kind\":\"download-directory\",\"site\":\"" + server.Url + "\",\"workspace\":null,\"remote_path\":\"other\",\"local_path\":" + JsonSerializer.Serialize(Path.GetFullPath(Local)) + ",\"completed\":[]}");

                TransferCheckpointException error = await Assert.ThrowsExactlyAsync<TransferCheckpointException>(() => Client(server).Transfers.DownloadDirectoryAsync("backup", Local, options));

                Assert.AreEqual(record, error.CheckpointPath);
                Assert.AreEqual(0, server.StorageRequests.Count);
                Assert.IsFalse(File.Exists(Path.Combine(Local, "a.bin")));
            }
        }

        // Rewrites the identities a checkpoint or the transfer's record at path holds as an earlier version of this SDK
        // recorded them on Linux: device and inode numbers alone, without the time each was created.
        private static void RecordWithoutCreationTimes(string path)
        {
            JsonObject recorded = JsonNode.Parse(File.ReadAllText(path)).AsObject();
            var holders = new List<JsonObject> { recorded };
            if (recorded["completed"] is JsonArray completed)
            {
                holders.AddRange(completed.Select(done => done.AsObject()));
            }
            foreach (JsonObject holder in holders)
            {
                foreach (string key in new[] { "stage_identity", "staging_identity", "local_identity" })
                {
                    string identity = holder[key]?.GetValue<string>();
                    if (identity != null)
                    {
                        Assert.AreEqual(3, identity.Split(':').Length, $"This version records the time of creation in {key}.");
                        holder[key] = string.Join(":", identity.Split(':').Take(2));
                    }
                }
            }
            // Written in place, so the file keeps the access the SDK gave it.
            File.WriteAllText(path, recorded.ToJsonString());
        }

        // Rewrites the transfer's record as an earlier SDK version wrote it: format 1, completed files without their identity.
        private void RecordAsAnEarlierVersion()
        {
            string path = Path.Combine(Checkpoints, "transfer.json");
            var record = JsonNode.Parse(File.ReadAllText(path)).AsObject();
            record["version"] = 1;
            foreach (var done in record["completed"].AsArray())
            {
                done.AsObject().Remove("local_identity");
            }
            // Written in place, so the file keeps the access the SDK gave it.
            File.WriteAllText(path, record.ToJsonString());
        }

        // A checkpointed download of the backup folder into Local, one file at a time.
        private DirectoryDownloadOptions WithCheckpoints(IProgress<DirectoryTransferProgress> progress = null)
        {
            return new DirectoryDownloadOptions { MaxConcurrentFiles = 1, CheckpointDirectory = Checkpoints, Progress = progress, FileOptions = new DownloadOptions { PartSize = 1024, MaxConcurrency = 1 } };
        }

        // Downloads every file of the server's backup folder into Local, then fails the transfer as it reports that the
        // last of them ended, so that its record says every file is completed, as a run stopped just then leaves it.
        private async Task CompleteEveryFileAndKeepTheRecord(DownloadServer server, int files)
        {
            var failure = new IOException("Stopped once every file was completed.");
            var progress = new SyncProgress<DirectoryTransferProgress>(report =>
            {
                if (report.FilesDone == files && report.FilesActive == 0)
                {
                    throw failure;
                }
            });
            IOException error = await Assert.ThrowsAsync<IOException>(() => Client(server).Transfers.DownloadDirectoryAsync("backup", Local, WithCheckpoints(progress)));
            Assert.AreSame(failure, error);
            Assert.AreEqual(files, RecordedFiles().Length, "Every file is recorded as completed.");
        }

        // The relative paths the transfer's record says are completed.
        private string[] RecordedFiles()
        {
            return JsonNode.Parse(File.ReadAllText(Path.Combine(Checkpoints, "transfer.json")))["completed"].AsArray().Select(done => done["relative_path"].GetValue<string>()).ToArray();
        }

        // Many files of one part each: their parts teach one target, which file admission follows with room to grow, so
        // more files than the old 64 are in progress at once, while the storage never holds more requests than the one
        // shared target.
        [TestMethod]
        [Timeout(60000)]
        public async Task OnePartFilesRampFileAdmissionPast64OnOneSharedTarget()
        {
            using (var server = new DownloadServer(new byte[1]))
            {
                for (int i = 0; i < 1000; i++)
                {
                    server.AddFile($"many/f{i:D4}.bin", Pattern(100, i));
                }
                // Long enough that each file's own work, its API request and putting it in place, is small beside its part.
                var storage = new HeldCount();
                server.Intercept = (request, connection) => storage.HoldAsync(Task.Delay(600));
                var progress = new DirectoryProgressLog();
                DownloadConcurrency shared = null;
                var options = new DirectoryDownloadOptions
                {
                    Progress = progress,
                    FileOptions = new DownloadOptions
                    {
                        AutomaticConcurrency = SharedTuning(start: 16, max: 64, step: 24),
                        Progress = new SyncProgress<DownloadProgress>(report => shared = report.Concurrency ?? shared),
                    },
                };

                DirectoryTransferResult result = await Client(server).Transfers.DownloadDirectoryAsync("many", Local, options);

                Assert.AreEqual(1000, result.Files.Count);
                CollectionAssert.AreEqual(Pattern(100, 0), File.ReadAllBytes(Path.Combine(Local, "f0000.bin")));
                CollectionAssert.AreEqual(Pattern(100, 999), File.ReadAllBytes(Path.Combine(Local, "f0999.bin")));
                int mostFiles = progress.Reports.Max(r => r.FilesActive);
                int mostLimit = progress.Reports.Max(r => r.FileLimit);
                Assert.IsTrue(progress.Reports.First().FileLimit <= 6, $"File admission starts low: {progress.Reports.First().FileLimit}.");
                string scope = shared == null ? "no scope" : $"shared {shared.Shared}, target {shared.Target}/{shared.MaxTarget}, peak {shared.PeakActiveRequests}, grows {shared.Grows}, shrinks {shared.Shrinks}, last {shared.LastDecision}, at target {shared.TimeAtTarget.TotalMilliseconds:F0} ms, limiter {shared.TimeWaitingForLimiter.TotalMilliseconds:F0} ms, no part {shared.TimeWithoutReadyPart.TotalMilliseconds:F0} ms, writing {shared.TimeWritingDestination.TotalMilliseconds:F0} ms";
                Assert.IsTrue(shared != null && shared.Shared, scope);
                Assert.IsTrue(mostLimit > 64 && mostFiles > 64, $"Past the old 64 files at once: limit {mostLimit}, files {mostFiles}; {scope}.");
                Assert.IsTrue(storage.Peak <= 64, $"One shared target, never one per file: {storage.Peak} requests at once.");
            }
        }

        // Files of many parts share one target: the storage holds at most that many of their requests at once, however
        // many files are in progress. Static file options keep each file's own number, as before.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(true, DisplayName = "Automatic: one shared budget")]
        [DataRow(false, DisplayName = "Static: each file its own number")]
        public async Task MultipartFilesShareOneBudget(bool automatic)
        {
            using (var server = new DownloadServer(new byte[1]))
            {
                for (int i = 0; i < 4; i++)
                {
                    server.AddFile($"big/f{i}.bin", Pattern(32 * 1024, i));
                }
                var storage = new HeldCount();
                server.Intercept = (request, connection) => storage.HoldAsync(Task.Delay(30));
                var fileOptions = automatic
                    ? new DownloadOptions { PartSize = 1024, AutomaticConcurrency = SharedTuning(start: 6, max: 6, step: 1) }
                    : new DownloadOptions { PartSize = 1024, ConcurrencyMode = DownloadConcurrencyMode.Static, MaxConcurrency = 6 };
                var options = new DirectoryDownloadOptions { FileOptions = fileOptions, Limiter = new TransferLimiter(maxConcurrentRequests: 64) };
                if (!automatic)
                {
                    options.MaxConcurrentFiles = 4;
                }

                DirectoryTransferResult result = await Client(server).Transfers.DownloadDirectoryAsync("big", Local, options);

                Assert.AreEqual(4, result.Files.Count);
                for (int i = 0; i < 4; i++)
                {
                    CollectionAssert.AreEqual(Pattern(32 * 1024, i), File.ReadAllBytes(Path.Combine(Local, $"f{i}.bin")));
                }
                if (automatic)
                {
                    Assert.AreEqual(6, storage.Peak, "Four files of many parts share six requests at once.");
                }
                else
                {
                    Assert.IsTrue(storage.Peak > 6, $"Each static file has its own six: {storage.Peak} at once.");
                }
            }
        }

        // The int property keeps its signature and value; once set, it is a hard cap on automatic file admission too.
        [TestMethod]
        [Timeout(60000)]
        public async Task MaxConcurrentFilesKeepsItsIntContractAndCapsAutomaticFiles()
        {
            var options = new DirectoryDownloadOptions();
            int before = options.MaxConcurrentFiles;
            Assert.AreEqual(DirectoryTransferOptions.DefaultMaxConcurrentFiles, before);
            options.MaxConcurrentFiles = 3;
            int after = options.MaxConcurrentFiles;
            Assert.AreEqual(3, after);
            using (var server = new DownloadServer(new byte[1]))
            {
                for (int i = 0; i < 40; i++)
                {
                    server.AddFile($"few/f{i:D2}.bin", Pattern(64, i));
                }
                server.Intercept = async (request, connection) =>
                {
                    await Task.Delay(20);
                    return false;
                };
                var progress = new DirectoryProgressLog();
                options.Progress = progress;
                options.FileOptions = new DownloadOptions { AutomaticConcurrency = SharedTuning(start: 16, max: 32, step: 16) };

                DirectoryTransferResult result = await Client(server).Transfers.DownloadDirectoryAsync("few", Local, options);

                Assert.AreEqual(40, result.Files.Count);
                Assert.IsTrue(progress.Reports.All(r => r.FilesActive <= 3 && r.FileLimit <= 3), "The caller's cap holds for automatic files.");
            }
        }

        // A directory upload of many one-part files: their parts teach one target too, which file admission follows, so
        // more parts are in flight than the old fixed four files allowed, and never more than the one shared target.
        [TestMethod]
        [Timeout(60000)]
        public async Task UploadDirectoryFilesLearnOneSharedTarget()
        {
            using (var server = new UploadServer())
            {
                for (int i = 0; i < 300; i++)
                {
                    WriteLocal($"f{i:D3}.bin", Pattern(8, i));
                }
                server.RespondToPart = async request =>
                {
                    await Task.Delay(40);
                    return UploadServer.Accept(request);
                };
                var progress = new DirectoryProgressLog();
                var tuning = AutomaticConcurrencyOptions.For(UploadPerformanceProfile.Generic);
                tuning.MinTarget = 1;
                tuning.StartTarget = 2;
                tuning.SoftCeiling = 12;
                tuning.MaxTarget = 12;
                tuning.PlateauTarget = 12;
                tuning.WindowAttempts = 4;
                tuning.GrowthIntervalWindows = 1;
                tuning.GrowthStep = 2;
                tuning.ProbeGainPercent = 3;
                tuning.ProbeWindows = 2;
                tuning.HoldWindows = 1;
                tuning.LatencyGrowthQueueLimit = 0;
                tuning.LatencyBackoffQueueLimit = 0;

                DirectoryTransferResult result = await Client(server).Transfers.UploadDirectoryAsync(Local, "up", new DirectoryUploadOptions { Progress = progress, FileOptions = new UploadOptions { AutomaticConcurrency = tuning } });

                Assert.AreEqual(300, result.Files.Count);
                CollectionAssert.AreEqual(Pattern(8, 0), server.AcceptedBytes("up/f000.bin"));
                CollectionAssert.AreEqual(Pattern(8, 299), server.AcceptedBytes("up/f299.bin"));
                Assert.IsTrue(server.MaxPartsInFlight > DirectoryTransferOptions.DefaultMaxConcurrentFiles, $"More than the old fixed files at once: {server.MaxPartsInFlight}.");
                Assert.IsTrue(server.MaxPartsInFlight <= 12, $"Never more than the one shared target: {server.MaxPartsInFlight}.");
                Assert.IsTrue(progress.Reports.Max(r => r.FileLimit) > 6, "File admission followed the target up.");
            }
        }

        // A directory upload straight to Amazon S3 with the scaled S3 policy given as tuning, or chosen for its files and
        // started with UploadStartup.Aggressive, which starts their one shared target at 150 (a default directory upload
        // warm-starts instead: ADefaultDirectoryUploadToS3SendsOnePartAtATimeUntilStorageAnswers):
        // the directory's own limiter, made before any file negotiates, has a slot for every part that target may reach,
        // rather than the 64 it used to have, so more parts than 64 are in flight at once and never more than the target.
        // The storage holds the parts after each file's first until it has held more than 64 at once, so the count is of
        // what is admitted, not of how fast the host turns requests around; a limiter of 64 slots would hold at 64 until
        // the wait gives up. With default file options, a caller's limiter, or the files' MaxConcurrency, is the hard cap
        // it was, with the scaled S3 policy chosen for the destination.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("scaled", DisplayName = "The scaled S3 policy given as tuning: past the old 64 at once, never more than the target")]
        [DataRow("aggressive", DisplayName = "Startup Aggressive: past the old 64 at once, never more than the shared target")]
        [DataRow("limiter", DisplayName = "A caller's limiter caps the parts at its slots")]
        [DataRow("max-concurrency", DisplayName = "The files' MaxConcurrency caps the parts")]
        public async Task ADirectoryUploadToS3IsNotCappedByItsOwnLimiter(string cap)
        {
            using (var server = new UploadServer { Aws = true })
            {
                // Amazon S3 limits that let an upload choose 8-byte parts, as the part sizing tests use them, and S3 part URLs
                // whose object name has no slash of its own, which the loopback route reads a part number after.
                UploadGrant.AwsLimitsForTests.Value = new PartSizeLimits(1, 8, 100, 4096);
                server.UploadUri = (file, number, negotiation) => UploadServer.AwsPartUrl(file.Replace('/', '~'), number, negotiation);
                for (int i = 0; i < 10; i++)
                {
                    WriteLocal($"f{i}.bin", Pattern(8 * 32, i));
                }
                var storage = new HeldCount();
                var drain = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                bool barrier = cap == "scaled" || cap == "aggressive";
                server.RespondToPart = async request =>
                {
                    // Each file's first part goes through, as its other parts are negotiated after it; the rest are held,
                    // in the default case until released below, otherwise for a moment.
                    if (!barrier || request.Number > 1)
                    {
                        await storage.HoldAsync(barrier ? drain.Task : Task.WhenAny(drain.Task, Task.Delay(40)));
                    }
                    return UploadServer.Accept(request);
                };
                var seen = new List<UploadConcurrency>();
                var options = new DirectoryUploadOptions
                {
                    FileOptions = new UploadOptions
                    {
                        Progress = new SyncProgress<TransferProgress>(report =>
                        {
                            if (report.Concurrency != null)
                            {
                                lock (seen)
                                {
                                    seen.Add(report.Concurrency);
                                }
                            }
                        }),
                    },
                };
                TransferLimiter limiter = null;
                if (cap == "scaled")
                {
                    options.FileOptions.AutomaticConcurrency = AutomaticConcurrencyOptions.ScaledS3();
                }
                else if (cap == "aggressive")
                {
                    options.FileOptions.Startup = UploadStartup.Aggressive;
                }
                else if (cap == "limiter")
                {
                    limiter = new TransferLimiter(maxConcurrentRequests: 3, maxBufferedBytes: 1024 * 1024);
                    options.Limiter = limiter;
                }
                else if (cap == "max-concurrency")
                {
                    options.FileOptions.MaxConcurrency = 5;
                }

                // The server's own client sends the S3 part requests to the loopback site, as the part sizing tests do.
                Task<DirectoryTransferResult> upload = server.Client().Transfers.UploadDirectoryAsync(Local, "up", options);
                try
                {
                    DateTime until = DateTime.UtcNow.AddSeconds(30);
                    while (barrier && storage.Peak <= 64 && !upload.IsCompleted && DateTime.UtcNow < until)
                    {
                        await Task.Delay(10);
                    }
                }
                finally
                {
                    // Released whatever happened, so that held requests never outlive the test.
                    drain.TrySetResult(true);
                }
                DirectoryTransferResult result = await upload;

                Assert.AreEqual(10, result.Files.Count);
                CollectionAssert.AreEqual(Pattern(8 * 32, 9), server.AcceptedBytes("up/f9.bin"));
                List<UploadConcurrency> reported;
                lock (seen)
                {
                    reported = seen.ToList();
                }
                int maxTarget = cap == "max-concurrency" ? 5 : 1024;
                string shared = string.Join(",", reported.Select(c => $"{c.Shared}/{c.Profile}/{c.ProfileSource}/{c.Target}/{c.MaxTarget}").Distinct());
                UploadProfileSource source = cap == "scaled" ? UploadProfileSource.Options : UploadProfileSource.Server;
                Assert.IsTrue(reported.Count > 0 && reported.All(c => c.Shared && c.Profile == UploadPerformanceProfile.S3 && c.ProfileSource == source && c.MaxTarget == maxTarget),
                    "Every file reports the shared scaled S3 policy, given or chosen for the destination: " + shared);
                switch (cap)
                {
                    case "limiter":
                        Assert.IsTrue(storage.Peak <= 3, $"The caller's limiter holds: {storage.Peak} parts at once; {shared}.");
                        Assert.AreEqual(3, limiter.MaxConcurrentRequests);
                        Assert.AreEqual(1024 * 1024, limiter.MaxBufferedBytes);
                        Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes + limiter.QueuedRequests + limiter.QueuedBufferReservations);
                        break;
                    case "max-concurrency":
                        Assert.IsTrue(storage.Peak <= 5, $"The files' MaxConcurrency holds: {storage.Peak} parts at once; {shared}.");
                        break;
                    default:
                        Assert.IsTrue(storage.Peak > 64, $"Past the old 64 request slots: {storage.Peak} parts at once; {shared}.");
                        Assert.IsTrue(storage.Peak <= 150, $"Never more than the shared target: {storage.Peak} parts at once; {shared}.");
                        break;
                }
            }
        }

        // A directory upload with default file options straight to Amazon S3 warm-starts as one upload does
        // (ParallelUploadTest.ADefaultUploadToS3SendsOnePartAtATimeUntilStorageAnswers), once, in the scope its files share:
        // with every answer held, one part at a time of all the files, though several files have begun and have their
        // parts ready, rather than one each; every byte arrives once the answers come.
        [TestMethod]
        [Timeout(60000)]
        public async Task ADefaultDirectoryUploadToS3SendsOnePartAtATimeUntilStorageAnswers()
        {
            using (var server = new UploadServer { Aws = true, HoldParts = true })
            {
                UploadGrant.AwsLimitsForTests.Value = new PartSizeLimits(1, 8, 100, 4096);
                server.UploadUri = (file, number, negotiation) => UploadServer.AwsPartUrl(file.Replace('/', '~'), number, negotiation);
                for (int i = 0; i < 6; i++)
                {
                    WriteLocal($"f{i}.bin", Pattern(8 * 4, i));
                }
                var seen = new List<UploadConcurrency>();
                var options = new DirectoryUploadOptions
                {
                    FileOptions = new UploadOptions
                    {
                        Progress = new SyncProgress<TransferProgress>(report =>
                        {
                            lock (seen)
                            {
                                if (report.Concurrency != null)
                                {
                                    seen.Add(report.Concurrency);
                                }
                            }
                        }),
                    },
                };

                Task<DirectoryTransferResult> upload = server.Client().Transfers.UploadDirectoryAsync(Local, "up", options);
                TimeSpan held;
                try
                {
                    await server.WaitUntil(s => s.PartsInFlight >= 1 && s.Negotiations.Count >= 4, "a part is held and four files have begun");
                    await Task.Delay(250);
                    held = UploadServer.Clock;
                }
                finally
                {
                    server.ReleaseHeldParts();
                }
                DirectoryTransferResult result = await upload;

                Assert.AreEqual(1, server.MaxPartsInFlightBetween(TimeSpan.Zero, held), "One part at a time, of all the files, before any was answered.");
                Assert.AreEqual(6, result.Files.Count);
                for (int i = 0; i < 6; i++)
                {
                    CollectionAssert.AreEqual(Pattern(8 * 4, i), server.AcceptedBytes($"up/f{i}.bin"));
                }
                List<UploadConcurrency> reported;
                lock (seen)
                {
                    reported = seen.ToList();
                }
                Assert.IsTrue(reported.Count > 0 && reported.All(c => c.Shared && c.Profile == UploadPerformanceProfile.S3 && c.ProfileSource == UploadProfileSource.Server),
                    "Every file reports the shared scaled S3 policy chosen for the destination.");
            }
        }

        // A directory download's files share a scope too, but it never warm-starts: from storage the SDK binds downloads
        // to, its files start together at the scaled S3 policy's 150, with more than one part at once before any is
        // answered, as before.
        [TestMethod]
        [Timeout(60000)]
        public async Task ADefaultDirectoryDownloadFromS3StartsAtTheScaledPolicysStart()
        {
            using (var server = new DownloadServer(new byte[1]))
            {
                for (int i = 0; i < 4; i++)
                {
                    server.AddFile($"big/f{i}.bin", Pattern(32 * 1024, i));
                }
                var storage = new HeldCount();
                var release = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                server.Intercept = (request, connection) => storage.HoldAsync(release.Task);
                DownloadConcurrency first = null;
                var options = new DirectoryDownloadOptions
                {
                    FileOptions = new DownloadOptions
                    {
                        PartSize = 1024,
                        Progress = new SyncProgress<DownloadProgress>(report => Interlocked.CompareExchange(ref first, report.Concurrency, null)),
                    },
                };

                Task<DirectoryTransferResult> download = Client(server).Transfers.DownloadDirectoryAsync("big", Local, options);
                try
                {
                    DateTime until = DateTime.UtcNow.AddSeconds(10);
                    while (storage.Peak < 2 && !download.IsCompleted && DateTime.UtcNow < until)
                    {
                        await Task.Delay(10);
                    }
                }
                finally
                {
                    release.TrySetResult(true);
                }
                DirectoryTransferResult result = await download;

                Assert.IsTrue(storage.Peak > 1, $"More than one part at once before any was answered: {storage.Peak}.");
                Assert.IsTrue(first != null && first.Shared && first.Profile == UploadPerformanceProfile.S3 && first.Target == 150,
                    first == null ? "No concurrency was reported." : $"The files' first shared target: {first.Target}, {first.Profile}, shared {first.Shared}.");
                Assert.AreEqual(4, result.Files.Count);
                for (int i = 0; i < 4; i++)
                {
                    CollectionAssert.AreEqual(Pattern(32 * 1024, i), File.ReadAllBytes(Path.Combine(Local, $"f{i}.bin")));
                }
            }
        }

        // Files of one part each, uploaded straight to S3, spend most of their time beginning (begin_upload) or
        // finishing (the final request) wherever Files.com takes longer to answer those than storage takes to accept a
        // part. Here every one of those answers is held, so the files the target's own limit allows can never have a part
        // ready to send. The upload still begins more files, past that limit, to feed the target, but one at a time, a
        // file each time it looks again, never all at once, and never more than MaxConcurrentFiles; the caller's limiter
        // still holds the parts to its slots, and is left with nothing in use. Every byte arrives once the answers come.
        // The files' shared target is held at 4, by tuning of their own, which has no warm start, so that only this pacing
        // begins files past its limit: a warm start's raises may begin several at once instead, as
        // TheLastRaiseOfASharedWarmStartBeginsFilesBeforeAnyFinishes shows.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("begin", DisplayName = "Beginning held: more files begin, one at a time")]
        [DataRow("finish", DisplayName = "Finishing held: more files begin, one at a time")]
        public async Task OnePartFilesBeginOneAtATimePastTheTargetsLimitUntilTheirPartsAreReady(string held)
        {
            const int most = 24;
            using (var server = new UploadServer { Aws = true })
            {
                // Parts no smaller than the files: each is sent as one part.
                UploadGrant.AwsLimitsForTests.Value = new PartSizeLimits(8, 8, 100, 4096);
                server.UploadUri = (file, number, negotiation) => UploadServer.AwsPartUrl(file.Replace('/', '~'), number, negotiation);
                for (int i = 0; i < 60; i++)
                {
                    WriteLocal($"f{i:D2}.bin", Pattern(8, i));
                }
                var release = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                var arrivals = new List<TimeSpan>();
                Func<string, Task> hold = file =>
                {
                    lock (arrivals)
                    {
                        arrivals.Add(UploadServer.Clock);
                    }
                    return release.Task;
                };
                if (held == "begin")
                {
                    server.HoldNegotiation = hold;
                }
                else
                {
                    server.HoldFinish = hold;
                }
                var progress = new DirectoryProgressLog();
                var limiter = new TransferLimiter(maxConcurrentRequests: 2, maxBufferedBytes: 1024 * 1024);
                AutomaticConcurrencyOptions fixedTarget = SharedTuning(start: 4, max: 4, step: 1);
                fixedTarget.MinTarget = 4;
                var options = new DirectoryUploadOptions { MaxConcurrentFiles = most, Limiter = limiter, Progress = progress, FileOptions = new UploadOptions { AutomaticConcurrency = fixedTarget } };

                Task<DirectoryTransferResult> upload = server.Client().Transfers.UploadDirectoryAsync(Local, "up", options);
                List<TimeSpan> seen;
                IReadOnlyList<DirectoryTransferProgress> whileHeld;
                try
                {
                    await server.WaitUntil(_ => Count(arrivals) >= most || upload.IsCompleted, $"{most} files' requests are held", () => $"{Count(arrivals)} held");
                    await Task.Delay(300);
                    lock (arrivals)
                    {
                        seen = arrivals.ToList();
                    }
                    whileHeld = progress.Reports;
                }
                finally
                {
                    release.TrySetResult(true);
                }
                DirectoryTransferResult result = await upload;

                Assert.AreEqual(most, seen.Count, "Files begun to feed the target, up to MaxConcurrentFiles and no more.");
                // Each file reports that it began before its transfer starts, and no file ends while the answers are held, so
                // the first report of each number of files in progress is when that file began, not when its request reached
                // Files.com. Past the files the target's limit allows (6, with the target held at 4), files begin one at a
                // time: the upload looks again every 100 ms while no file ends.
                Assert.IsTrue(whileHeld.All(r => r.FilesDone == 0), "No file ended while the answers were held.");
                List<TimeSpan?> began = Enumerable.Range(7, most - 6).Select(files => whileHeld.FirstOrDefault(r => r.FilesActive == files)?.Elapsed).ToList();
                Assert.IsTrue(began.All(at => at.HasValue), $"A report as each of files 7 to {most} began, among {string.Join(", ", whileHeld.Select(r => r.FilesActive).Distinct())} in progress.");
                List<TimeSpan> later = began.Select(at => at.Value).ToList();
                double gap = (later.Last() - later.First()).TotalMilliseconds / (later.Count - 1);
                Assert.IsTrue(gap >= 50, $"Files past the target's limit began one at a time, {gap:0} ms apart on average, not all at once.");
                Assert.IsTrue(progress.Reports.All(r => r.FilesActive <= most && r.FileLimit <= most), "Never more than MaxConcurrentFiles in progress.");
                Assert.IsTrue(server.MaxPartsInFlight <= 2, $"The caller's limiter holds the parts to its slots: {server.MaxPartsInFlight} at once.");
                Assert.AreEqual(0, limiter.ActiveRequests + limiter.BufferedBytes + limiter.QueuedRequests + limiter.QueuedBufferReservations);
                Assert.AreEqual(60, result.Files.Count);
                for (int i = 0; i < 60; i++)
                {
                    CollectionAssert.AreEqual(Pattern(8, i), server.AcceptedBytes($"up/f{i:D2}.bin"));
                }
            }
        }

        private static int Count(List<TimeSpan> arrivals)
        {
            lock (arrivals)
            {
                return arrivals.Count;
            }
        }

        // Static upload files keep the fixed number of files at once.
        [TestMethod]
        [Timeout(60000)]
        public async Task UploadDirectoryWithStaticFilesKeepsItsFixedFiles()
        {
            using (var server = new UploadServer())
            {
                for (int i = 0; i < 30; i++)
                {
                    WriteLocal($"f{i:D2}.bin", Pattern(8, i));
                }
                server.RespondToPart = async request =>
                {
                    await Task.Delay(20);
                    return UploadServer.Accept(request);
                };
                var progress = new DirectoryProgressLog();

                DirectoryTransferResult result = await Client(server).Transfers.UploadDirectoryAsync(Local, "up", new DirectoryUploadOptions { Progress = progress, FileOptions = new UploadOptions { ConcurrencyMode = UploadConcurrencyMode.Static, MaxConcurrency = 1 } });

                Assert.AreEqual(30, result.Files.Count);
                Assert.IsTrue(progress.Reports.All(r => r.FilesActive <= DirectoryTransferOptions.DefaultMaxConcurrentFiles && r.FileLimit == DirectoryTransferOptions.DefaultMaxConcurrentFiles));
                Assert.IsTrue(server.MaxPartsInFlight <= DirectoryTransferOptions.DefaultMaxConcurrentFiles);
            }
        }

        private static AutomaticConcurrencyOptions SharedTuning(int start, int max, int step)
        {
            AutomaticConcurrencyOptions tuning = AutomaticConcurrencyOptions.For(UploadPerformanceProfile.S3);
            tuning.MinTarget = 1;
            tuning.StartTarget = start;
            tuning.SoftCeiling = max;
            tuning.MaxTarget = max;
            tuning.PlateauTarget = max;
            tuning.WindowAttempts = 4;
            tuning.GrowthIntervalWindows = 1;
            tuning.GrowthStep = step;
            tuning.ProbeGainPercent = 3;
            tuning.ProbeWindows = 2;
            tuning.HoldWindows = 1;
            tuning.LatencyGrowthQueueLimit = 0;
            tuning.LatencyBackoffQueueLimit = 0;
            return tuning;
        }

        // Counts the storage requests a loopback server holds at once.
        private sealed class HeldCount
        {
            private int held;
            private int peak;

            public int Peak => Volatile.Read(ref peak);

            public async Task<bool> HoldAsync(Task wait)
            {
                int now = Interlocked.Increment(ref held);
                int seen;
                while (now > (seen = Volatile.Read(ref peak)) && Interlocked.CompareExchange(ref peak, now, seen) != seen)
                {
                }
                try
                {
                    await wait;
                }
                finally
                {
                    Interlocked.Decrement(ref held);
                }
                return false;
            }
        }

        private void WriteLocal(string relativePath, byte[] contents)
        {
            string path = Path.Combine(new[] { Local }.Concat(relativePath.Split('/')).ToArray());
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, contents);
        }

        // The storage requests after the first before, each as the file asked for and its range, or whole.
        private static string[] Fetched(DownloadServer server, int before)
        {
            return server.StorageRequests.Skip(before).Select(request => $"{FileOf(request)} " + (request.Range.HasValue ? $"{request.Range.Value.First}-{request.Range.Value.Last}" : "whole")).ToArray();
        }

        private static string FileOf(StorageRequest request)
        {
            string[] segments = request.Line.Split(' ')[1].Split('?')[0].Split('/');
            return segments.Length > 3 ? Uri.UnescapeDataString(segments[3]) : DownloadServer.DefaultFile;
        }

        private static byte[] Pattern(int length, int seed)
        {
            return Enumerable.Range(0, length).Select(i => (byte)((i * 7) + seed)).ToArray();
        }

        private static FilesClient Client(UploadServer server)
        {
            return FilesClient.Create(server.Site.Configuration());
        }

        private static FilesClient Client(DownloadServer server)
        {
            return server.Client();
        }

        private sealed class DirectoryProgressLog : IProgress<DirectoryTransferProgress>
        {
            private readonly List<DirectoryTransferProgress> reports = new List<DirectoryTransferProgress>();

            public IReadOnlyList<DirectoryTransferProgress> Reports
            {
                get
                {
                    lock (reports)
                    {
                        return reports.ToList();
                    }
                }
            }

            public void Report(DirectoryTransferProgress value)
            {
                lock (reports)
                {
                    reports.Add(value);
                }
            }
        }

        private sealed class DirectoryProgressLogThen : IProgress<DirectoryTransferProgress>
        {
            private readonly Action then;

            internal DirectoryProgressLogThen(Action then)
            {
                this.then = then;
            }

            public void Report(DirectoryTransferProgress value)
            {
                then();
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

        [DllImport("kernel32.dll", EntryPoint = "CreateFileW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern Microsoft.Win32.SafeHandles.SafeFileHandle OpenDirectoryForDelete(string path, uint access, uint share, IntPtr security, uint creation, uint flags, IntPtr template);

        private sealed class FailingProgress : IProgress<TransferProgress>
        {
            private readonly Func<TransferProgress, Exception> failure;

            internal FailingProgress(Func<TransferProgress, Exception> failure)
            {
                this.failure = failure;
            }

            public void Report(TransferProgress value)
            {
                Exception e = failure(value);
                if (e != null)
                {
                    throw e;
                }
            }
        }
    }
}