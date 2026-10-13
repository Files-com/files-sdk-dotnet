using FilesCom;
using FilesCom.Transfers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FilesTests
{
    // Continuing a download whose stage, or staging directory, cannot be shown to be its own in a later run: on Linux
    // where the file system records no birth time, as FileIdentity.BirthForTests makes it wherever these tests run. Its
    // checkpoint then records the SHA-256 of each part written (format 4), and the later run never writes to, changes or
    // removes that stage: it copies the parts it still holds as recorded into a new stage of its own and continues there.
    public partial class DownloadTest
    {
        [TestMethod]
        [Timeout(60000)]
        [DataRow("stage")]
        [DataRow("directory")]
        [DataRow("both")]
        public async Task AStageThatCannotBeShownToBeItsOwnIsCopiedAsRecordedAndContinued(string weak)
        {
            SkipWithoutDescriptors();
            WithoutBirthTimeFor(weak);
            string checkpoint = Path.Combine(folder, "download.checkpoint");
            using (var server = new DownloadServer(Contents))
            {
                await InterruptAfterThreePartsAsync(server, checkpoint);
                string recorded = File.ReadAllText(checkpoint);
                StringAssert.Contains(recorded, "\"version\": 4");
                Assert.AreEqual(3, recorded.Split("\"sha256\"").Length - 1, "One digest for each part written: " + recorded);
                using (JsonDocument saved = JsonDocument.Parse(recorded))
                {
                    // Only what the row makes weak is: the other identity holds a birth time, and can name its object.
                    Assert.AreEqual(weak != "directory", !FileIdentity.Strong(saved.RootElement.GetProperty("stage_identity").GetString()), recorded);
                    Assert.AreEqual(weak != "stage", !FileIdentity.Strong(saved.RootElement.GetProperty("staging_identity").GetString()), recorded);
                }
                string earlier = LeftStage();
                EarlierStage before = EarlierStage.Of(earlier);
                int already = server.StorageRequests.Count;

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, SalvageOptions(checkpoint));

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual(3 * 16384, result.ResumedBytes, "The three parts the earlier stage holds as recorded are kept.");
                AssertContinued(server, already, 3, 4, 5, 6);
                Assert.IsFalse(File.Exists(checkpoint));
                before.AssertUnchanged("the earlier stage");
                Assert.AreEqual(1, Directory.GetDirectories(folder, ".files-download-*").Length, "Only the earlier stage's staging directory remains; the new stage was published.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("stage", "readable by its group")]
        [DataRow("directory", "readable by its group")]
        [DataRow("both", "readable by its group")]
        [DataRow("stage", "an access control list")]
        [DataRow("directory", "an access control list")]
        [DataRow("both", "an access control list")]
        public async Task AnEarlierStagingDirectoryThatIsNotPrivatelyThisUsersIsNotMadePrivateOrUsed(string weak, string change)
        {
            SkipWithoutDescriptors();
            if (change == "an access control list" && !OperatingSystem.IsMacOS())
            {
                Assert.Inconclusive("macOS access control lists; Linux POSIX ACLs are not run.");
            }
            WithoutBirthTimeFor(weak);
            string checkpoint = Path.Combine(folder, "download.checkpoint");
            using (var server = new DownloadServer(Contents))
            {
                await InterruptAfterThreePartsAsync(server, checkpoint);
                string earlier = LeftStage();
                string staging = Path.GetDirectoryName(earlier);
                if (change == "an access control list")
                {
                    Run("/bin/chmod", "+a", "everyone allow list,search", staging);
                }
                else if (!OperatingSystem.IsWindows())
                {
                    File.SetUnixFileMode(staging, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute | UnixFileMode.GroupRead | UnixFileMode.GroupExecute);
                }
                EarlierStage before = EarlierStage.Of(earlier);

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, SalvageOptions(checkpoint));

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual(0, result.ResumedBytes, "Nothing in it is used.");
                before.AssertUnchanged("the earlier stage and its staging directory, " + change + " and not made private");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task OnlyPartsTheEarlierStageStillHoldsAsRecordedAreUsed()
        {
            SkipWithoutDescriptors();
            WithoutBirthTimeFor("both");
            string checkpoint = Path.Combine(folder, "download.checkpoint");
            using (var server = new DownloadServer(Contents))
            {
                await InterruptAfterThreePartsAsync(server, checkpoint);
                string earlier = LeftStage();
                using (var changing = new FileStream(earlier, FileMode.Open, FileAccess.Write))
                {
                    changing.Position = 16384 + 5;
                    changing.WriteByte((byte)(Contents[16384 + 5] ^ 0xFF));
                }
                EarlierStage before = EarlierStage.Of(earlier);
                int already = server.StorageRequests.Count;

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, SalvageOptions(checkpoint));

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual(2 * 16384, result.ResumedBytes, "The first and third parts are as recorded; the second is not.");
                AssertContinued(server, already, 1, 3, 4, 5, 6);
                before.AssertUnchanged("the earlier stage, changed bytes and all");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("readable by others")]
        [DataRow("a second link")]
        [DataRow("a symbolic link")]
        public async Task AnEarlierStageThatIsNotPrivatelyThisUsersIsNotReadOrChanged(string change)
        {
            SkipWithoutDescriptors();
            WithoutBirthTimeFor("both");
            string checkpoint = Path.Combine(folder, "download.checkpoint");
            using (var server = new DownloadServer(Contents))
            {
                await InterruptAfterThreePartsAsync(server, checkpoint);
                string earlier = LeftStage();
                string other = Path.Combine(folder, "other.bin");
                if (change == "readable by others")
                {
                    if (!OperatingSystem.IsWindows())
                    {
                        File.SetUnixFileMode(earlier, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.GroupRead | UnixFileMode.OtherRead);
                    }
                }
                else if (change == "a second link")
                {
                    Assert.AreEqual(0, Link(earlier, other), "the test's hard link");
                }
                else
                {
                    File.Move(earlier, other);
                    File.CreateSymbolicLink(earlier, other);
                }
                EarlierStage before = EarlierStage.Of(earlier);

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, SalvageOptions(checkpoint));

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual(0, result.ResumedBytes, "Nothing in it is used.");
                before.AssertUnchanged("the earlier stage, as " + change);
                if (change != "readable by others")
                {
                    Assert.IsTrue(File.Exists(other), "What the test put beside it stays.");
                }
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("a digest that is not one")]
        [DataRow("digests that do not cover what is written")]
        [DataRow("no digests")]
        public async Task ACheckpointWhoseDigestsCannotBeReliedOnIsRefused(string damage)
        {
            SkipWithoutDescriptors();
            WithoutBirthTimeFor("both");
            string checkpoint = Path.Combine(folder, "download.checkpoint");
            using (var server = new DownloadServer(Contents))
            {
                await InterruptAfterThreePartsAsync(server, checkpoint);
                string recorded = File.ReadAllText(checkpoint);
                int digest = recorded.IndexOf("\"sha256\": \"", StringComparison.Ordinal) + 11;
                string damaged = damage == "a digest that is not one" ? recorded.Substring(0, digest) + "zz" + recorded.Substring(digest + 2)
                    : damage == "no digests" ? recorded.Replace("\"extents\"", "\"no_extents\"")
                    : recorded.Replace("\"end\": 32768", "\"end\": 30000");
                Assert.AreNotEqual(recorded, damaged, "the test's damage");
                File.WriteAllText(checkpoint, damaged);
                EarlierStage before = EarlierStage.Of(LeftStage());

                TransferCheckpointException refused = await Assert.ThrowsExactlyAsync<TransferCheckpointException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, SalvageOptions(checkpoint)));
                Assert.AreEqual(TransferCheckpointProblem.Unreadable, refused.Problem);
                Assert.AreEqual(damaged, File.ReadAllText(checkpoint), "The checkpoint stays as it is.");

                var startOver = SalvageOptions(checkpoint);
                startOver.CheckpointRestartPolicy = CheckpointRestartPolicy.StartOver;
                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, startOver);
                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual(0, result.ResumedBytes);
                before.AssertUnchanged("the earlier stage");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("fails")]
        [DataRow("is cancelled")]
        public async Task ACopyThatStopsLeavesTheEarlierStageAndCheckpointAsTheyWere(string stop)
        {
            SkipWithoutDescriptors();
            WithoutBirthTimeFor("both");
            string checkpoint = Path.Combine(folder, "download.checkpoint");
            using (var server = new DownloadServer(Contents))
            {
                await InterruptAfterThreePartsAsync(server, checkpoint);
                string recorded = File.ReadAllText(checkpoint);
                EarlierStage before = EarlierStage.Of(LeftStage());
                int already = server.StorageRequests.Count;
                var cancellation = new CancellationTokenSource();
                DownloadCoordinator.SalvageFaults.Value = end =>
                {
                    if (end == 2 * 16384)
                    {
                        if (stop == "fails")
                        {
                            throw new IOException("The test's copy failure.");
                        }
                        cancellation.Cancel();
                    }
                };
                try
                {
                    Exception stopped = await Assert.ThrowsAsync<Exception>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, SalvageOptions(checkpoint), cancellation.Token));
                    Assert.IsTrue(stop == "fails" ? stopped is IOException : stopped is OperationCanceledException, stopped.ToString());
                }
                finally
                {
                    DownloadCoordinator.SalvageFaults.Value = null;
                }

                Assert.AreEqual(recorded, File.ReadAllText(checkpoint), "The checkpoint stays as it was.");
                before.AssertUnchanged("the earlier stage");
                Assert.AreEqual(1, Directory.GetDirectories(folder, ".files-download-*").Length, "The new stage was removed.");
                Assert.AreEqual(already, server.StorageRequests.Count, "Nothing was fetched.");
                Assert.IsFalse(File.Exists(Destination));

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, SalvageOptions(checkpoint));
                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual(3 * 16384, result.ResumedBytes, "A later run still uses what the earlier stage holds.");
            }
        }

        // The checkpoint names the new stage in place of the earlier one only once the new stage's names, its file's entry
        // in its staging directory and that directory's entry in the folder, are on storage: where a directory cannot be
        // flushed, because flushing fails or the file system cannot do it, the checkpoint still names the earlier stage.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("fails")]
        [DataRow("cannot be done")]
        public async Task ANewStageWhoseNamesCannotBeFlushedLeavesTheEarlierStageAndCheckpoint(string flushing)
        {
            SkipWithoutDescriptors();
            WithoutBirthTimeFor("both");
            string checkpoint = Path.Combine(folder, "download.checkpoint");
            using (var server = new DownloadServer(Contents))
            {
                await InterruptAfterThreePartsAsync(server, checkpoint);
                string recorded = File.ReadAllText(checkpoint);
                EarlierStage before = EarlierStage.Of(LeftStage());
                int already = server.StorageRequests.Count;
                var asked = new List<Native.FlushCall>();
                Native.FlushFaults.Value = call =>
                {
                    lock (asked)
                    {
                        asked.Add(call);
                    }
                    bool directory = call == Native.FlushCall.FullSyncDirectory || call == Native.FlushCall.SyncDirectory;
                    // EIO, or EINVAL, which says a file system cannot flush a directory.
                    return !directory ? 0 : flushing == "fails" ? 5 : 22;
                };
                try
                {
                    await Assert.ThrowsAsync<IOException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, SalvageOptions(checkpoint)));
                }
                finally
                {
                    Native.FlushFaults.Value = null;
                }

                CollectionAssert.Contains(asked, OperatingSystem.IsMacOS() ? Native.FlushCall.FullSyncDirectory : Native.FlushCall.SyncDirectory, "The new staging directory was to be flushed.");
                Assert.AreEqual(recorded, File.ReadAllText(checkpoint), "The checkpoint still names the earlier stage.");
                before.AssertUnchanged("the earlier stage");
                Assert.AreEqual(1, Directory.GetDirectories(folder, ".files-download-*").Length, "The new stage was removed.");
                Assert.AreEqual(already, server.StorageRequests.Count, "Nothing was fetched.");
                Assert.IsFalse(File.Exists(Destination));

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, SalvageOptions(checkpoint));
                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual(3 * 16384, result.ResumedBytes, "A later run still uses what the earlier stage holds.");
            }
        }

        // The copy's buffer is held in the limiter's buffer space, as a part's is, and no larger than it; the copy takes no
        // request slot. While that space is taken the copy waits, before making a stage; a wait that is cancelled takes
        // nothing and changes nothing; once there is room, the download continues as recorded and every byte is given back.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("waits")]
        [DataRow("is cancelled while it waits")]
        public async Task ACopyWaitsForBufferSpaceAndTakesNoRequestSlot(string copy)
        {
            SkipWithoutDescriptors();
            WithoutBirthTimeFor("both");
            string checkpoint = Path.Combine(folder, "download.checkpoint");
            using (var server = new DownloadServer(Contents))
            {
                await InterruptAfterThreePartsAsync(server, checkpoint);
                string recorded = File.ReadAllText(checkpoint);
                EarlierStage before = EarlierStage.Of(LeftStage());
                int already = server.StorageRequests.Count;
                var limiter = new TransferLimiter(maxConcurrentRequests: 2, maxBufferedBytes: 4096);
                IDisposable taken = await limiter.ReserveBufferAsync(4096, CancellationToken.None);
                var copying = new List<(long Buffered, int Requests)>();
                DownloadCoordinator.SalvageFaults.Value = end => copying.Add((limiter.BufferedBytes, limiter.ActiveRequests));
                var cancellation = new CancellationTokenSource();
                DownloadOptions options = SalvageOptions(checkpoint);
                options.Limiter = limiter;
                try
                {
                    Task<DownloadResult> download = Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options, cancellation.Token);
                    await UntilAsync(() => limiter.QueuedBufferReservations == 1 || download.IsCompleted);
                    Assert.IsFalse(download.IsCompleted, "The copy waits for buffer space.");
                    Assert.AreEqual(1, Directory.GetDirectories(folder, ".files-download-*").Length, "No new stage while it waits.");
                    Assert.AreEqual(0, limiter.ActiveRequests, "No request slot while it waits.");
                    Assert.AreEqual(already, server.StorageRequests.Count);
                    if (copy == "waits")
                    {
                        taken.Dispose();
                        DownloadResult result = await download;
                        CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                        Assert.AreEqual(3 * 16384, result.ResumedBytes);
                        AssertContinued(server, already, 3, 4, 5, 6);
                        CollectionAssert.AreEqual(Enumerable.Repeat((4096L, 0), 3).ToList(), copying, "Each part was copied through the limiter's 4096 bytes, with no request slot.");
                    }
                    else
                    {
                        cancellation.Cancel();
                        await Assert.ThrowsAsync<OperationCanceledException>(() => download);
                        Assert.AreEqual(0, limiter.QueuedBufferReservations, "The cancelled wait is gone.");
                        Assert.AreEqual(4096, limiter.BufferedBytes, "Only the test's own space is taken.");
                        taken.Dispose();
                        Assert.AreEqual(0, copying.Count, "Nothing was copied.");
                        Assert.AreEqual(recorded, File.ReadAllText(checkpoint), "The checkpoint stays as it was.");
                        Assert.AreEqual(1, Directory.GetDirectories(folder, ".files-download-*").Length, "No new stage was made.");
                        Assert.AreEqual(already, server.StorageRequests.Count, "Nothing was fetched.");
                        DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options);
                        CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                        Assert.AreEqual(3 * 16384, result.ResumedBytes, "A later run still uses what the earlier stage holds.");
                    }
                    Assert.AreEqual(0, limiter.BufferedBytes, "All buffer space was given back.");
                    Assert.AreEqual(0, limiter.ActiveRequests);
                    before.AssertUnchanged("the earlier stage");
                }
                finally
                {
                    DownloadCoordinator.SalvageFaults.Value = null;
                    taken.Dispose();
                }
            }
        }

        // Each attempt at a part disposes of the hash it made however it ends: retried after its body ended early, failed
        // with more bytes than the part, cancelled while its body arrives, or committed whole.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("ends early and is retried")]
        [DataRow("has more bytes than the part")]
        [DataRow("is cancelled")]
        public async Task EveryAttemptAtAPartDisposesOfItsHash(string attempt)
        {
            SkipWithoutDescriptors();
            WithoutBirthTimeFor("both");
            int made = 0;
            int disposed = 0;
            var receiving = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            FileDownloadSink.HashesForTests.Value = change =>
            {
                if (change < 0)
                {
                    Interlocked.Increment(ref disposed);
                }
                else if (Interlocked.Increment(ref made) == 2)
                {
                    // The second part's attempt has its hash: its body is being received.
                    receiving.TrySetResult(true);
                }
            };
            string checkpoint = Path.Combine(folder, "download.checkpoint");
            using (var server = new DownloadServer(Contents))
            {
                int answered = 0;
                var cancellation = new CancellationTokenSource();
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range?.First != 16384 || Interlocked.Increment(ref answered) > 1)
                    {
                        return false;
                    }
                    string range = $"Content-Range: bytes 16384-32767/{Contents.Length}\r\nETag: {server.ETag}\r\n";
                    if (attempt == "has more bytes than the part")
                    {
                        // Chunked, so nothing but the SDK's own count stops the extra bytes.
                        await connection.SendAsync($"HTTP/1.1 206 OK\r\nTransfer-Encoding: chunked\r\n{range}\r\n");
                        await connection.SendAsync($"{16384 + 10:x}\r\n");
                        await connection.SendAsync(Contents.Skip(16384).Take(16384 + 10).ToArray());
                        await connection.SendAsync("\r\n0\r\n\r\n");
                        return true;
                    }
                    await connection.SendAsync($"HTTP/1.1 206 OK\r\nContent-Length: 16384\r\n{range}\r\n");
                    await connection.SendAsync(Contents.Skip(16384).Take(5000).ToArray());
                    if (attempt == "is cancelled")
                    {
                        await receiving.Task;
                        cancellation.Cancel();
                        await connection.HoldAsync();
                    }
                    else
                    {
                        connection.Dispose();
                    }
                    return true;
                };
                try
                {
                    Task<DownloadResult> download = Client(server, retries: 2).Transfers.DownloadFileAsync("remote.bin", Destination, SalvageOptions(checkpoint), cancellation.Token);
                    if (attempt == "ends early and is retried")
                    {
                        DownloadResult result = await download;
                        CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                        Assert.AreEqual(1, result.FinalProgress.Retries);
                        Assert.AreEqual(8, made, "One hash for each attempt: seven parts, one of them twice.");
                    }
                    else
                    {
                        Exception stopped = await Assert.ThrowsAsync<Exception>(() => download);
                        Assert.IsTrue(attempt == "is cancelled" ? stopped is OperationCanceledException : stopped is InvalidResponseException, stopped.ToString());
                        Assert.AreEqual(2, made, "The first part's attempt, and the second's.");
                    }
                    Assert.AreEqual(made, disposed, "Every attempt's hash was disposed.");
                }
                finally
                {
                    FileDownloadSink.HashesForTests.Value = null;
                }
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task AStageThatCanBeShownToBeItsOwnKeepsTheCheckpointFormatWithoutDigests()
        {
            string checkpoint = Path.Combine(folder, "download.checkpoint");
            using (var server = new DownloadServer(Contents))
            {
                await InterruptAfterThreePartsAsync(server, checkpoint);
                string recorded = File.ReadAllText(checkpoint);
                StringAssert.Contains(recorded, "\"version\": 3");
                Assert.IsFalse(recorded.Contains("extents"), recorded);
            }
        }

        // Stops a download with a checkpoint as its fourth part is asked for, so its checkpoint records three parts.
        private async Task InterruptAfterThreePartsAsync(DownloadServer server, string checkpoint)
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
            await Assert.ThrowsAsync<OperationCanceledException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, SalvageOptions(checkpoint), cancellation.Token));
            Assert.IsTrue(File.Exists(checkpoint));
            Assert.IsFalse(File.Exists(Destination));
        }

        // Waits, at most ten seconds, for condition to hold.
        private static async Task UntilAsync(Func<bool> condition)
        {
            for (DateTime until = DateTime.UtcNow.AddSeconds(10); !condition() && DateTime.UtcNow < until;)
            {
                await Task.Delay(10);
            }
        }

        private static DownloadOptions SalvageOptions(string checkpoint)
        {
            return new DownloadOptions { PartSize = 16 * 1024, MaxConcurrency = 1, CheckpointPath = checkpoint };
        }

        // After the version is confirmed (GET 0-0), only the parts given, each with the version's entity tag.
        private static void AssertContinued(DownloadServer server, int already, params int[] parts)
        {
            List<StorageRequest> continued = server.StorageRequests.Skip(already).ToList();
            Assert.AreEqual("GET 0-0", continued[0].ToString(), "The version is confirmed first.");
            CollectionAssert.AreEquivalent(parts.Select(i => $"GET {i * 16384}-{Math.Min(i * 16384 + 16383, Contents.Length - 1)}").ToList(),
                continued.Skip(1).Select(request => request.ToString()).ToList(), "Only the parts not held as recorded.");
            Assert.IsTrue(continued.All(request => request.IfMatch == server.ETag), "Every request carries the version's entity tag.");
        }

        // The stage file an interrupted run left, in its staging directory in folder.
        private string LeftStage()
        {
            string staging = Directory.GetDirectories(folder, ".files-download-*").Single();
            return Directory.GetFileSystemEntries(staging).Single();
        }

        // Within this test's flow, identities record no birth time for the stage, its staging directory, or both, and a
        // made-up one for anything else, held to Linux's rule wherever the test runs (FileIdentity.BirthForTests).
        private static void WithoutBirthTimeFor(string weak)
        {
            bool stage = weak == "stage" || weak == "both";
            bool directory = weak == "directory" || weak == "both";
            FileIdentity.BirthForTests.Value = status => (stage && status.IsRegular) || (directory && status.IsDirectory) ? null : "1791156357.529261620";
        }

        private static void SkipWithoutDescriptors()
        {
            if (!LocalDirectory.ByDescriptor)
            {
                Assert.Inconclusive("Stages are copied as recorded only where directories are held by descriptor.");
            }
        }

        [DllImport("libc", EntryPoint = "link", SetLastError = true)]
        private static extern int Link(string existing, string added);

        // What an earlier stage is, without following a link: its kind, permissions, length, bytes and modification time,
        // its staging directory's permissions, and, for both, the time their status last changed (to the nanosecond) and
        // their access control lists, to show that a later run left them as they were, permissions not even set again.
        private sealed class EarlierStage
        {
            private string path;
            private string described;

            internal static EarlierStage Of(string path)
            {
                return new EarlierStage { path = path, described = Describe(path) };
            }

            internal void AssertUnchanged(string what)
            {
                Assert.AreEqual(described, Describe(path), what + " is left as it was");
            }

            private static string Describe(string path)
            {
                var info = new FileInfo(path);
                string staging = Path.GetDirectoryName(path);
                string bytes = info.LinkTarget == null ? Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(path))) : "link to " + info.LinkTarget;
                string modes = OperatingSystem.IsWindows() ? "" : $"{File.GetUnixFileMode(path)} directory {File.GetUnixFileMode(staging)}";
                string changed = OperatingSystem.IsWindows() ? ""
                    : OperatingSystem.IsMacOS() ? Run("/usr/bin/stat", "-f", "%Fc", path, staging) + Run("/bin/ls", "-lde", path, staging)
                    : Run("/usr/bin/stat", "-c", "%z", path, staging) + Run("/bin/ls", "-ld", path, staging);
                return $"{info.Attributes} {modes} {(info.LinkTarget == null ? info.Length : 0)} {bytes} {info.LastWriteTimeUtc:O} {changed}";
            }
        }
    }
}