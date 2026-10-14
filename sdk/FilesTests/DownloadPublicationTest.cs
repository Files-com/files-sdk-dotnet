using FilesCom;
using FilesCom.Transfers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace FilesTests
{
    // How a download to a local file is put in place under each publication policy, what a refusal keeps, and how a later
    // run with the same checkpoint finishes what an earlier one did not, against a loopback storage server and the
    // local file system. Process crashes are stood in for by the failures that leave the same state behind.
    [TestClass]
    public class DownloadPublicationTest
    {
        private const int Part = 16 * 1024;
        private static readonly byte[] Contents = Enumerable.Range(0, 64 * 1024).Select(i => (byte)(i * 13 + 5)).ToArray();
        private string folder;

        // On Windows the test's folder is first made private, so that no other account can change the checkpoints kept in
        // it whatever this user's temporary directory allows.
        [TestInitialize]
        public void Start()
        {
            folder = Path.Combine(Path.GetTempPath(), "publication-test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            if (OperatingSystem.IsWindows())
            {
                WindowsTestAccess.MakePrivate(folder);
            }
        }

        [TestCleanup]
        public void Stop()
        {
            Native.FlushFaults.Value = null;
            Native.ReplacedOwnerFaults.Value = null;
            Native.NoReplaceRenameUnsupportedFaults.Value = false;
            Native.BeforePlacingFaults.Value = null;
            DownloadCoordinator.BeforeRecoveringFaults.Value = null;
            Directory.Delete(folder, true);
        }

        private string Destination => Path.Combine(folder, "file.bin");

        private string Checkpoint => Path.Combine(folder, "download.checkpoint");

        private DownloadOptions Options(DownloadPublicationPolicy policy, bool checkpoint = false) => new DownloadOptions
        {
            PartSize = Part,
            MaxConcurrency = 1,
            PublicationPolicy = policy,
            CheckpointPath = checkpoint ? Checkpoint : null,
        };

        [TestMethod]
        [Timeout(60000)]
        [DataRow(false, DisplayName = "Renamed without replacing")]
        [DataRow(true, DisplayName = "Linked, then its stage name removed, where renaming without replacing is not supported")]
        public async Task CreateNewPutsTheFileInPlaceAsANewFile(bool linked)
        {
            Native.NoReplaceRenameUnsupportedFaults.Value = linked;
            using (var server = new DownloadServer(Contents))
            {
                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Options(DownloadPublicationPolicy.CreateNew));

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.IsFalse(result.CompletedEarlier);
                AssertNoStage();
                if (!OperatingSystem.IsWindows())
                {
                    Assert.AreEqual(NewFileMode(), File.GetUnixFileMode(Destination));
                    Assert.AreEqual(1L, Links(Destination), "The file has one name, its own.");
                }
            }
        }

        // An entry already at the name is refused before anything is fetched, and left exactly as it is: a folder, a
        // link, even one to nothing, is never replaced or followed.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("file", DownloadPublicationPolicy.CreateNew, DownloadPublicationProblem.DestinationExists)]
        [DataRow("directory", DownloadPublicationPolicy.CreateNew, DownloadPublicationProblem.DestinationExists)]
        [DataRow("dangling link", DownloadPublicationPolicy.CreateNew, DownloadPublicationProblem.DestinationExists)]
        [DataRow("directory", DownloadPublicationPolicy.ReplaceRegularFile, DownloadPublicationProblem.DestinationNotRegularFile)]
        [DataRow("link", DownloadPublicationPolicy.ReplaceRegularFile, DownloadPublicationProblem.DestinationNotRegularFile)]
        [DataRow("dangling link", DownloadPublicationPolicy.ReplaceRegularFile, DownloadPublicationProblem.DestinationNotRegularFile)]
        public async Task AnEntryAlreadyThereIsRefusedBeforeAnythingIsFetched(string entry, DownloadPublicationPolicy policy, DownloadPublicationProblem problem)
        {
            if (entry.Contains("link") && OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("Creating symbolic links needs a privilege on Windows.");
                return;
            }
            string target = Path.Combine(folder, "target.bin");
            Put(entry, Destination, target);
            using (var server = new DownloadServer(Contents))
            {
                DownloadPublicationException error = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Options(policy)));

                Assert.AreEqual(problem, error.Problem);
                Assert.AreEqual(Destination, error.LocalPath);
                Assert.IsFalse(error.Placed);
                Assert.IsFalse(error.Retained);
                Assert.AreEqual(0, server.StorageRequests.Count, "Nothing is fetched.");
                AssertUnchanged(entry, Destination, target);
                AssertNoStage();
            }
        }

        // An entry that appears while the file is downloaded is refused as the file is put in place, and left exactly as
        // it is; with a checkpoint, the complete download is kept, so running it again once the name is free puts it in
        // place without fetching it again.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(DownloadPublicationPolicy.CreateNew, "file", false, DownloadPublicationProblem.DestinationExists)]
        [DataRow(DownloadPublicationPolicy.CreateNew, "file", true, DownloadPublicationProblem.DestinationExists)]
        [DataRow(DownloadPublicationPolicy.CreateNew, "dangling link", false, DownloadPublicationProblem.DestinationExists)]
        [DataRow(DownloadPublicationPolicy.ReplaceRegularFile, "directory", false, DownloadPublicationProblem.DestinationNotRegularFile)]
        [DataRow(DownloadPublicationPolicy.ReplaceRegularFile, "link", false, DownloadPublicationProblem.DestinationNotRegularFile)]
        public async Task AnEntryThatArrivesLateIsNeverReplaced(DownloadPublicationPolicy policy, string entry, bool linked, DownloadPublicationProblem problem)
        {
            if (entry.Contains("link") && OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("Creating symbolic links needs a privilege on Windows.");
                return;
            }
            Native.NoReplaceRenameUnsupportedFaults.Value = linked;
            string target = Path.Combine(folder, "target.bin");
            using (var server = new DownloadServer(Contents))
            {
                server.Intercept = (request, connection) =>
                {
                    if (request.Range?.First == 3 * Part && !File.Exists(Destination) && !Directory.Exists(Destination))
                    {
                        Put(entry, Destination, target);
                    }
                    return Task.FromResult(false);
                };

                DownloadPublicationException error = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Options(policy, checkpoint: true)));

                Assert.AreEqual(problem, error.Problem);
                Assert.IsFalse(error.Placed);
                Assert.IsTrue(error.Retained, "The complete download is kept.");
                Assert.AreEqual(Checkpoint, error.CheckpointPath);
                AssertUnchanged(entry, Destination, target);
                Assert.AreEqual("complete", JsonNode.Parse(File.ReadAllText(Checkpoint))["phase"].GetValue<string>());
                Assert.AreEqual(1, Stages().Length, "The stage is kept.");
                AssertStageIsPrivate(Stages()[0], "The stage kept is private.");

                Remove(entry, Destination);
                server.Intercept = null;
                int storageBefore = server.StorageRequests.Count;
                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Options(policy, checkpoint: true));

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                StorageRequest confirmed = server.StorageRequests.Skip(storageBefore).Single();
                Assert.AreEqual("GET 0-0", confirmed.ToString(), "Nothing is fetched again: storage only confirms the version kept, first.");
                Assert.AreEqual(server.ETag, confirmed.IfMatch);
                Assert.AreEqual(Contents.Length, result.ResumedBytes);
                Assert.IsFalse(result.CompletedEarlier);
                Assert.IsFalse(File.Exists(Checkpoint));
                AssertNoStage();
            }
        }

        // A policy stricter than the one a checkpoint was written under decides: what the earlier run kept stays, and is
        // used once the policy allows it.
        [TestMethod]
        [Timeout(60000)]
        public async Task ALaterRunsStricterPolicyDecides()
        {
            using (var server = new DownloadServer(Contents))
            {
                var cancellation = new CancellationTokenSource();
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range?.First == 2 * Part && !cancellation.IsCancellationRequested)
                    {
                        cancellation.Cancel();
                        await connection.HoldAsync();
                        return true;
                    }
                    return false;
                };
                await Assert.ThrowsAsync<OperationCanceledException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Options(DownloadPublicationPolicy.ReplaceRegularFile, checkpoint: true), cancellation.Token));
                File.WriteAllText(Destination, "OLD");
                string recorded = File.ReadAllText(Checkpoint);
                int storageBefore = server.StorageRequests.Count;

                DownloadPublicationException error = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Options(DownloadPublicationPolicy.CreateNew, checkpoint: true)));

                Assert.AreEqual(DownloadPublicationProblem.DestinationExists, error.Problem);
                Assert.IsTrue(error.Retained, "The earlier run's stage is kept as it is.");
                Assert.AreEqual("OLD", File.ReadAllText(Destination));
                Assert.AreEqual(recorded, File.ReadAllText(Checkpoint), "The checkpoint is left as it was.");
                Assert.AreEqual(storageBefore, server.StorageRequests.Count);
                Assert.AreEqual(1, Stages().Length);

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Options(DownloadPublicationPolicy.ReplaceRegularFile, checkpoint: true));

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual(2 * Part, result.ResumedBytes);
                CollectionAssert.AreEqual(new[] { "GET 0-0", $"GET {2 * Part}-{3 * Part - 1}", $"GET {3 * Part}-{4 * Part - 1}" }, server.StorageRequests.Skip(storageBefore).Select(request => request.ToString()).ToList(), "The version confirmed, then the parts not yet written.");
            }
        }

        // PowerShell's -Force: a regular file of this user's keeps all its permissions; one of another user's is replaced
        // by one of this user's, with the owner permissions a new file gets and the replaced file's group and other
        // permissions. The default instead keeps the owner, or does not replace the file.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(false, DisplayName = "This user's file")]
        [DataRow(true, DisplayName = "Another user's file")]
        public async Task ReplaceRegularFileKeepsTheReplacedFilesGroupAndOthersForThisUser(bool anotherUsers)
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("Unix permissions.");
                return;
            }
            File.WriteAllText(Destination, "OLD");
            UnixFileMode before = UnixFileMode.UserRead | UnixFileMode.OtherRead;
            File.SetUnixFileMode(Destination, before);
            using (var server = new DownloadServer(Contents))
            {
                // Taken to be another user's, whose files this one cannot give away: the default refuses it.
                Native.ReplacedOwnerFaults.Value = status => anotherUsers ? (status.Uid + 424242u, status.Gid) : (status.Uid, status.Gid);
                if (anotherUsers)
                {
                    DownloadPublicationException refused = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Options(DownloadPublicationPolicy.ReplaceExisting)));
                    Assert.AreEqual(DownloadPublicationProblem.AccessNotKept, refused.Problem);
                    Assert.AreEqual("OLD", File.ReadAllText(Destination));
                }

                await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Options(DownloadPublicationPolicy.ReplaceRegularFile));

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                UnixFileMode expected = anotherUsers ? (NewFileMode() & (UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute)) | UnixFileMode.OtherRead : before;
                Assert.AreEqual(expected, File.GetUnixFileMode(Destination));
                StringAssert.Contains(Run("/usr/bin/stat", StatOwnerFormat(), Destination), Run("/usr/bin/id", "-u").Trim(), "The file is this user's.");
                AssertNoStage();
            }
        }

        // A replaced file whose group the new file cannot be given is not replaced; with a checkpoint the download is kept,
        // private, and put in place without fetching again once the group can be given.
        [TestMethod]
        [Timeout(60000)]
        public async Task AReplacementThatCannotKeepAccessKeepsTheDownloadPrivate()
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("Unix groups and permissions.");
                return;
            }
            File.WriteAllText(Destination, "OLD");
            File.SetUnixFileMode(Destination, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.GroupRead);
            bool acl = OperatingSystem.IsMacOS();
            if (acl)
            {
                // Its access control list goes to the stage before its group is found not to.
                Run("/bin/chmod", "+a", "everyone allow read", Destination);
            }
            using (var server = new DownloadServer(Contents))
            {
                Native.ReplacedOwnerFaults.Value = status => (status.Uid, 424242u);
                DownloadPublicationException error = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Options(DownloadPublicationPolicy.ReplaceExisting, checkpoint: true)));
                Native.ReplacedOwnerFaults.Value = null;

                Assert.AreEqual(DownloadPublicationProblem.AccessNotKept, error.Problem);
                Assert.IsTrue(error.Retained);
                Assert.AreEqual("OLD", File.ReadAllText(Destination));
                string stage = Stages().Single();
                Assert.AreEqual("-rw-------", Mode(stage), "The stage kept is owner-only again.");
                if (acl)
                {
                    Assert.IsFalse(Run("/bin/ls", "-le", stage).Contains(" 0: "), "The stage kept has no access control list: " + Run("/bin/ls", "-le", stage));
                }
                int storageBefore = server.StorageRequests.Count;

                await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Options(DownloadPublicationPolicy.ReplaceExisting, checkpoint: true));

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual("GET 0-0", server.StorageRequests.Skip(storageBefore).Single().ToString(), "Nothing is fetched again: storage only confirms the version kept.");
            }
        }

        // A run that ends after the file is in place, but before it is recorded durable, leaves the file and its
        // checkpoint; the next run recognizes that very file and completes without fetching anything.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(true, DisplayName = "Bound to a version")]
        [DataRow(false, DisplayName = "Not bound to a version")]
        public async Task AFileAnEarlierRunPutInPlaceIsCompletedWithoutFetchingIt(bool bound)
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("Directories are flushed on macOS and Linux only.");
                return;
            }
            using (var server = new DownloadServer(Contents) { Bound = bound })
            {
                await PlaceWithoutDurability(server, DownloadPublicationPolicy.CreateNew);
                int storageBefore = server.StorageRequests.Count;
                int apiBefore = server.ApiRequests.Count;

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Options(DownloadPublicationPolicy.CreateNew, checkpoint: true));

                Assert.IsTrue(result.CompletedEarlier);
                Assert.AreEqual(Contents.Length, result.Size);
                Assert.AreEqual(bound, result.VersionBound);
                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual(storageBefore, server.StorageRequests.Count, "Nothing is fetched.");
                Assert.AreEqual(apiBefore, server.ApiRequests.Count, "Nothing is asked of Files.com.");
                Assert.IsFalse(File.Exists(Checkpoint));
                AssertNoStage();
            }
        }

        // A file put in place by someone else where an earlier run's file was, even with the same bytes, is not taken for
        // that run's: CreateNew refuses it, and the default replaces it with a new download.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(DownloadPublicationPolicy.CreateNew)]
        [DataRow(DownloadPublicationPolicy.ReplaceExisting)]
        public async Task AnotherFileWhereTheEarlierRunsFileWasIsNotTakenForIt(DownloadPublicationPolicy policy)
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("Directories are flushed on macOS and Linux only.");
                return;
            }
            using (var server = new DownloadServer(Contents))
            {
                await PlaceWithoutDurability(server, DownloadPublicationPolicy.CreateNew);
                // A new file, while the first still exists, so it cannot be given the first one's number.
                string foreign = Path.Combine(folder, "foreign.bin");
                File.WriteAllBytes(foreign, Contents);
                File.SetLastWriteTimeUtc(foreign, File.GetLastWriteTimeUtc(Destination));
                File.Move(foreign, Destination, overwrite: true);
                int storageBefore = server.StorageRequests.Count;

                if (policy == DownloadPublicationPolicy.CreateNew)
                {
                    DownloadPublicationException error = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Options(policy, checkpoint: true)));
                    Assert.AreEqual(DownloadPublicationProblem.DestinationExists, error.Problem);
                    Assert.AreEqual(storageBefore, server.StorageRequests.Count);
                }
                else
                {
                    DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Options(policy, checkpoint: true));
                    Assert.IsFalse(result.CompletedEarlier);
                    Assert.AreEqual(storageBefore + 4, server.StorageRequests.Count, "The file is downloaded again.");
                }
                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
            }
        }

        // Linux gives a deleted file's number to the next file at once, so a checkpoint whose identities hold no time of
        // creation, as an earlier version of this SDK recorded them there, shows nothing to be its run's: neither the file
        // in place, the earlier run's own or another put there once it was removed, which may have its number; nor a
        // directory put at the staging directory's name; nor, in it, a file at a kept stage's name. Nothing is taken for
        // the earlier run's, completed earlier, continued, removed or made private: CreateNew refuses the file there,
        // fetching nothing, and leaves it and the checkpoint as they are; the default downloads the file again, whole, in
        // a new stage, and puts it in place.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("own", DownloadPublicationPolicy.CreateNew, DisplayName = "The earlier run's own file is refused by CreateNew")]
        [DataRow("own", DownloadPublicationPolicy.ReplaceExisting, DisplayName = "The earlier run's own file is downloaded again by default")]
        [DataRow("another", DownloadPublicationPolicy.CreateNew, DisplayName = "Another file put in its place is refused by CreateNew")]
        [DataRow("another", DownloadPublicationPolicy.ReplaceExisting, DisplayName = "Another file put in its place is downloaded again by default")]
        [DataRow("kept stage", DownloadPublicationPolicy.ReplaceExisting, DisplayName = "What was put in place of a kept stage is left, and the file downloaded again")]
        public async Task OnLinuxACheckpointWithoutTimesOfCreationShowsNothingToBeItsRuns(string state, DownloadPublicationPolicy policy)
        {
            if (!OperatingSystem.IsLinux())
            {
                Assert.Inconclusive("Only Linux identities hold the time a file was created.");
                return;
            }
            using (var server = new DownloadServer(Contents))
            {
                if (state == "kept stage")
                {
                    await CancelAtPart(server, 2, policy);
                }
                else
                {
                    await PlaceWithoutDurability(server, DownloadPublicationPolicy.CreateNew);
                }
                JsonNode written = JsonNode.Parse(File.ReadAllText(Checkpoint));
                Assert.AreEqual(3, written["staging_identity"].GetValue<string>().Split(':').Length, "This version records the time the staging directory was created.");
                RecordWithoutCreationTimes();
                string recorded = File.ReadAllText(Checkpoint);
                // What the earlier run made is removed, and others put at its names, which may be given its numbers.
                string staging = Path.Combine(folder, written["stage_directory"].GetValue<string>());
                string stageName = Path.Combine(staging, written["stage_file"].GetValue<string>());
                if (Directory.Exists(staging))
                {
                    Directory.Delete(staging, recursive: true);
                }
                Directory.CreateDirectory(staging);
                if (state == "kept stage")
                {
                    File.WriteAllBytes(stageName, Contents.Take(2 * Part).ToArray());
                }
                if (state == "another")
                {
                    File.Delete(Destination);
                    File.WriteAllBytes(Destination, Enumerable.Range(0, Contents.Length).Select(i => (byte)(i * 7 + 1)).ToArray());
                }
                byte[] there = File.Exists(Destination) ? File.ReadAllBytes(Destination) : null;
                string stagingMode = Run("/bin/ls", "-ld", staging).Substring(0, 10);
                int storageBefore = server.StorageRequests.Count;

                if (policy == DownloadPublicationPolicy.CreateNew)
                {
                    DownloadPublicationException error = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Options(policy, checkpoint: true)));
                    Assert.AreEqual(DownloadPublicationProblem.DestinationExists, error.Problem, error.Message);
                    Assert.AreEqual(storageBefore, server.StorageRequests.Count, "Nothing is fetched.");
                    CollectionAssert.AreEqual(there, File.ReadAllBytes(Destination), "The file there is left as it is.");
                    Assert.AreEqual(recorded, File.ReadAllText(Checkpoint), "The checkpoint is left as it was.");
                }
                else
                {
                    DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Options(policy, checkpoint: true));
                    Assert.IsFalse(result.CompletedEarlier, "Nothing is taken for the earlier run's file.");
                    Assert.AreEqual(0, result.ResumedBytes, "Nothing the earlier run wrote is used.");
                    CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                    Assert.AreEqual(storageBefore + Contents.Length / Part, server.StorageRequests.Count, "The file is downloaded again, whole.");
                    Assert.IsFalse(File.Exists(Checkpoint));
                }
                Assert.IsTrue(Directory.Exists(staging), "The directory at the staging directory's name is not the earlier run's to remove.");
                Assert.AreEqual(stagingMode, Run("/bin/ls", "-ld", staging).Substring(0, 10), "Nor is it made private.");
                if (state == "kept stage")
                {
                    CollectionAssert.AreEqual(Contents.Take(2 * Part).ToArray(), File.ReadAllBytes(stageName), "The file at the stage's name is not continued.");
                }
            }
        }

        // Where a link, not a rename, put the file in place, a run that ended before removing the stage's own name leaves
        // the file with two names; the next run removes the stage's name, and leaves anything else that is there.
        [TestMethod]
        [Timeout(60000)]
        public async Task AFileLeftWithItsStageNameTooIsFinished()
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("Hard links and directory flushes on macOS and Linux.");
                return;
            }
            Native.NoReplaceRenameUnsupportedFaults.Value = true;
            using (var server = new DownloadServer(Contents))
            {
                await PlaceWithoutDurability(server, DownloadPublicationPolicy.CreateNew);
                JsonNode recorded = JsonNode.Parse(File.ReadAllText(Checkpoint));
                string staging = Path.Combine(folder, recorded["stage_directory"].GetValue<string>());
                string stageName = Path.Combine(staging, recorded["stage_file"].GetValue<string>());
                Directory.CreateDirectory(staging);
                Run("/bin/ln", Destination, stageName);
                Assert.AreEqual(2L, Links(Destination));

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Options(DownloadPublicationPolicy.CreateNew, checkpoint: true));

                Assert.IsTrue(result.CompletedEarlier);
                Assert.IsFalse(File.Exists(stageName), "The stage's name is removed.");
                Assert.AreEqual(1L, Links(Destination));
                Assert.IsTrue(Directory.Exists(staging), "A staging directory other than the one recorded is left.");
                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
            }
        }

        // A download not bound to a version cannot be confirmed later, so a refusal keeps nothing of it.
        [TestMethod]
        [Timeout(60000)]
        public async Task AnUnboundDownloadThatIsRefusedKeepsNothing()
        {
            using (var server = new DownloadServer(Contents) { Bound = false })
            {
                server.Intercept = (request, connection) =>
                {
                    if (!File.Exists(Destination))
                    {
                        File.WriteAllText(Destination, "OLD");
                    }
                    return Task.FromResult(false);
                };

                DownloadPublicationException error = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Options(DownloadPublicationPolicy.CreateNew, checkpoint: true)));

                Assert.AreEqual(DownloadPublicationProblem.DestinationExists, error.Problem);
                Assert.IsFalse(error.Retained);
                Assert.AreEqual("OLD", File.ReadAllText(Destination));
                Assert.IsFalse(File.Exists(Checkpoint));
                AssertNoStage();
            }
        }

        // A checkpoint whose stage holds less than it records fails by default, leaving both as they are; with StartOver,
        // that stage, shown to be the one recorded, is emptied and the file downloaded from its first byte.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(CheckpointRestartPolicy.Fail)]
        [DataRow(CheckpointRestartPolicy.StartOver)]
        public async Task AStageThatDoesNotHoldWhatItsCheckpointRecordsFollowsTheRestartPolicy(CheckpointRestartPolicy restart)
        {
            using (var server = new DownloadServer(Contents))
            {
                await CancelAtPart(server, 3, DownloadPublicationPolicy.ReplaceExisting);
                string stage = Stages().Single();
                using (var file = new FileStream(stage, FileMode.Open, FileAccess.Write))
                {
                    file.SetLength(100);
                }
                string recorded = File.ReadAllText(Checkpoint);
                int storageBefore = server.StorageRequests.Count;
                DownloadOptions options = Options(DownloadPublicationPolicy.ReplaceExisting, checkpoint: true);
                options.CheckpointRestartPolicy = restart;

                if (restart == CheckpointRestartPolicy.Fail)
                {
                    TransferCheckpointException error = await Assert.ThrowsExactlyAsync<TransferCheckpointException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options));
                    Assert.AreEqual(TransferCheckpointProblem.Unreadable, error.Problem);
                    StringAssert.Contains(error.Message, "StartOver");
                    Assert.AreEqual(recorded, File.ReadAllText(Checkpoint));
                    Assert.AreEqual(100L, new FileInfo(stage).Length, "The stage is left as it is.");
                    Assert.AreEqual(storageBefore, server.StorageRequests.Count);
                    return;
                }
                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options);

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual(0L, result.ResumedBytes);
                Assert.AreEqual($"GET 0-{Part - 1}", server.StorageRequests[storageBefore].ToString(), "From the first byte.");
                Assert.IsFalse(File.Exists(Checkpoint));
                AssertNoStage();
            }
        }

        // Checkpoints of downloads are written in format 3, which binds a download to its storage's entity tag and the
        // resource it came from, and which an SDK that reads only formats 1 and 2 refuses. A format 2 checkpoint of a 2.0
        // preview, bound to a Files.com download identity instead, records a partial download that cannot be confirmed: it
        // is refused and left as it is, unless the restart policy is StartOver, which downloads the file from its first
        // byte. A later format is refused and left as it is.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("preview", DisplayName = "A 2.0 preview's format 2 checkpoint is refused and left")]
        [DataRow("preview, start over", DisplayName = "A 2.0 preview's format 2 checkpoint is started over with StartOver")]
        [DataRow("later", DisplayName = "A later format is refused and left")]
        public async Task CheckpointFormatsAreReadAsTheyWereWritten(string format)
        {
            using (var server = new DownloadServer(Contents))
            {
                await CancelAtPart(server, 3, DownloadPublicationPolicy.ReplaceExisting);
                JsonNode written = JsonNode.Parse(File.ReadAllText(Checkpoint));
                Assert.AreEqual(3, written["version"].GetValue<int>());
                Assert.AreEqual("partial", written["phase"].GetValue<string>());
                Assert.IsNotNull(written["stage_identity"]?.GetValue<string>());
                Assert.AreEqual(server.ETag, written["etag"].GetValue<string>());
                StringAssert.StartsWith(written["source"].GetValue<string>(), "sha256:");
                JsonObject rewritten = written.AsObject();
                if (format.StartsWith("preview", StringComparison.Ordinal))
                {
                    rewritten["version"] = 2;
                    rewritten.Remove("etag");
                    rewritten.Remove("source");
                    rewritten["token"] = "fdi1.version-1";
                    rewritten["if_match"] = server.ETag;
                }
                else
                {
                    rewritten["version"] = 4;
                }
                File.WriteAllText(Checkpoint, rewritten.ToJsonString());
                string recorded = File.ReadAllText(Checkpoint);
                int storageBefore = server.StorageRequests.Count;
                DownloadOptions options = Options(DownloadPublicationPolicy.ReplaceExisting, checkpoint: true);

                if (format != "preview, start over")
                {
                    TransferCheckpointException error = await Assert.ThrowsExactlyAsync<TransferCheckpointException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options));
                    Assert.AreEqual(TransferCheckpointProblem.Unreadable, error.Problem);
                    if (format == "preview")
                    {
                        StringAssert.Contains(error.Message, "cannot be confirmed");
                    }
                    Assert.AreEqual(recorded, File.ReadAllText(Checkpoint), "It is left as it is.");
                    Assert.AreEqual(1, Stages().Length);
                    Assert.AreEqual(storageBefore, server.StorageRequests.Count);
                    return;
                }
                options.CheckpointRestartPolicy = CheckpointRestartPolicy.StartOver;
                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options);

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual(0, result.ResumedBytes);
                Assert.IsTrue(server.StorageRequests.Skip(storageBefore).All(request => request.ToString() != "GET 0-0"), "Nothing it records is confirmed or used: the file comes from its first byte.");
            }
        }

        // On macOS and Linux a folder this user may add files to and search, but not list (0300), gets the downloaded file
        // all the same, new or replacing one, with or without a checkpoint: it is opened only to search it.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(DownloadPublicationPolicy.CreateNew, false, false)]
        [DataRow(DownloadPublicationPolicy.CreateNew, false, true)]
        [DataRow(DownloadPublicationPolicy.ReplaceExisting, true, false)]
        [DataRow(DownloadPublicationPolicy.ReplaceRegularFile, true, true)]
        public async Task AFolderThatMayNotBeListedStillGetsTheFile(DownloadPublicationPolicy policy, bool existing, bool checkpoint)
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
            string addOnly = Path.Combine(folder, "add-only");
            Directory.CreateDirectory(addOnly);
            string destination = Path.Combine(addOnly, "file.bin");
            UnixFileMode kept = UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.GroupRead;
            if (existing)
            {
                File.WriteAllText(destination, "OLD");
                File.SetUnixFileMode(destination, kept);
            }
            UnixFileMode newFile = NewFileMode();
            DownloadResult result;
            File.SetUnixFileMode(addOnly, UnixFileMode.UserWrite | UnixFileMode.UserExecute);
            try
            {
                Assert.ThrowsExactly<UnauthorizedAccessException>(() => Directory.GetFileSystemEntries(addOnly), "The folder cannot be listed.");
                using (var server = new DownloadServer(Contents))
                {
                    result = await Client(server).Transfers.DownloadFileAsync("remote.bin", destination, Options(policy, checkpoint));
                }
            }
            finally
            {
                File.SetUnixFileMode(addOnly, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
            }

            CollectionAssert.AreEqual(Contents, File.ReadAllBytes(destination));
            Assert.AreEqual(destination, result.LocalPath);
            Assert.AreEqual(existing ? kept : newFile, File.GetUnixFileMode(destination), existing ? "The replaced file's permissions are kept." : "A new file's permissions.");
            CollectionAssert.AreEqual(new[] { "file.bin" }, Directory.GetFileSystemEntries(addOnly).Select(Path.GetFileName).ToArray(), "No stage is left behind.");
            Assert.IsFalse(File.Exists(Checkpoint));
        }

        // On macOS and Linux another program of this user renames the stage away, in its private staging folder, and puts
        // another file at its name: that file is not put in place as the download, the file at the destination name stays
        // as it was, and neither file is removed. The stage moved away holds every byte downloaded, and is made private
        // again; nothing is kept for a later run.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(DownloadPublicationPolicy.CreateNew, false)]
        [DataRow(DownloadPublicationPolicy.ReplaceExisting, false)]
        [DataRow(DownloadPublicationPolicy.ReplaceRegularFile, true)]
        public async Task AStageAnotherProgramReplacesIsNotPutInPlace(DownloadPublicationPolicy policy, bool checkpoint)
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("On Windows a stage is held open without sharing the right to rename it.");
                return;
            }
            if (policy != DownloadPublicationPolicy.CreateNew)
            {
                File.WriteAllText(Destination, "OLD");
            }
            string stage = null;
            using (var server = new DownloadServer(Contents))
            {
                server.Intercept = (request, connection) =>
                {
                    if (request.Range?.First == 3 * Part && stage == null)
                    {
                        stage = Stages().Single();
                        File.Move(stage, stage + ".original");
                        File.WriteAllText(stage, "SUBSTITUTE");
                    }
                    return Task.FromResult(false);
                };

                DownloadPublicationException error = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Options(policy, checkpoint)));

                Assert.AreEqual(DownloadPublicationProblem.StageReplaced, error.Problem);
                Assert.IsFalse(error.Placed);
                Assert.IsFalse(error.Retained);
                Assert.AreEqual(checkpoint ? Checkpoint : null, error.CheckpointPath);
                StringAssert.Contains(error.Message, "was not put in place");
            }
            if (policy == DownloadPublicationPolicy.CreateNew)
            {
                Assert.IsFalse(File.Exists(Destination), "Nothing is put in place.");
            }
            else
            {
                Assert.AreEqual("OLD", File.ReadAllText(Destination), "The file at that name stays as it was.");
            }
            Assert.AreEqual("SUBSTITUTE", File.ReadAllText(stage), "What took the stage's name is left where it is.");
            CollectionAssert.AreEqual(Contents, File.ReadAllBytes(stage + ".original"), "The stage moved away holds every byte downloaded.");
            Assert.AreEqual("-rw-------", Mode(stage + ".original"), "It is private again.");
            Assert.IsFalse(File.Exists(Checkpoint), "Nothing is kept for a later run.");
        }

        // The same done at the very moment the stage is put in place, just after it was checked: what that program put at
        // the stage's name is what the rename or link put in place, and the download says so rather than succeed.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(DownloadPublicationPolicy.ReplaceExisting, false)]
        [DataRow(DownloadPublicationPolicy.CreateNew, false)]
        [DataRow(DownloadPublicationPolicy.CreateNew, true)]
        public async Task AStageReplacedAsItIsPutInPlaceIsReportedNotTakenForTheDownload(DownloadPublicationPolicy policy, bool linked)
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("On Windows a stage is held open without sharing the right to rename it.");
                return;
            }
            Native.NoReplaceRenameUnsupportedFaults.Value = linked;
            if (policy == DownloadPublicationPolicy.ReplaceExisting)
            {
                File.WriteAllText(Destination, "OLD");
            }
            string stage = null;
            Native.BeforePlacingFaults.Value = () =>
            {
                stage = Stages().Single();
                File.Move(stage, stage + ".original");
                File.WriteAllText(stage, "SUBSTITUTE");
            };
            DownloadPublicationException error;
            try
            {
                using (var server = new DownloadServer(Contents))
                {
                    error = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Options(policy)));
                }
            }
            finally
            {
                Native.BeforePlacingFaults.Value = null;
            }

            Assert.AreEqual(DownloadPublicationProblem.StageReplaced, error.Problem);
            Assert.IsFalse(error.Placed, "The downloaded file is not in place.");
            Assert.IsFalse(error.Retained);
            StringAssert.Contains(error.Message, "is not the file downloaded");
            Assert.AreEqual("SUBSTITUTE", File.ReadAllText(Destination), "What that program put at the stage's name is what was put in place, and is left as it is.");
            CollectionAssert.AreEqual(Contents, File.ReadAllBytes(stage + ".original"), "The stage moved away holds every byte downloaded.");
        }

        // On macOS and Linux the destination folder is renamed during the download, and another folder takes its name:
        // nothing is put in either, since the path given no longer leads to the folder the download opened. With a
        // checkpoint the downloaded bytes are kept, private, in the folder it opened, and once that folder is back at its
        // path, running the download again puts the file in place without fetching it again; without one, nothing is
        // left in either folder.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(DownloadPublicationPolicy.CreateNew, false)]
        [DataRow(DownloadPublicationPolicy.CreateNew, true)]
        [DataRow(DownloadPublicationPolicy.ReplaceExisting, false)]
        [DataRow(DownloadPublicationPolicy.ReplaceRegularFile, true)]
        public async Task AFolderRenamedDuringTheDownloadGetsNothingNorDoesItsReplacement(DownloadPublicationPolicy policy, bool checkpoint)
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("On Windows the folders are held for every policy, so they cannot be renamed.");
                return;
            }
            string into = Path.Combine(folder, "into");
            string original = Path.Combine(folder, "into-original");
            Directory.CreateDirectory(into);
            string destination = Path.Combine(into, "file.bin");
            if (policy != DownloadPublicationPolicy.CreateNew)
            {
                File.WriteAllText(destination, "OLD");
            }
            using (var server = new DownloadServer(Contents))
            {
                server.Intercept = (request, connection) =>
                {
                    if (request.Range?.First == 3 * Part && !Directory.Exists(original))
                    {
                        Directory.Move(into, original);
                        Directory.CreateDirectory(into);
                    }
                    return Task.FromResult(false);
                };

                DownloadPublicationException error = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", destination, Options(policy, checkpoint)));

                Assert.AreEqual(DownloadPublicationProblem.DestinationMoved, error.Problem);
                Assert.AreEqual(destination, error.LocalPath);
                Assert.IsFalse(error.Placed);
                Assert.AreEqual(checkpoint, error.Retained);
                CollectionAssert.AreEqual(new string[0], Directory.GetFileSystemEntries(into), "Nothing is put in the folder now at the path.");
                if (policy == DownloadPublicationPolicy.CreateNew)
                {
                    Assert.IsFalse(File.Exists(Path.Combine(original, "file.bin")), "Nor in the folder the download opened.");
                }
                else
                {
                    Assert.AreEqual("OLD", File.ReadAllText(Path.Combine(original, "file.bin")), "Nor is the file in the folder the download opened replaced.");
                }
                if (!checkpoint)
                {
                    CollectionAssert.AreEqual(new string[0], Directory.GetDirectories(original, ".files-download-*"), "Nothing is left in the folder the download opened.");
                    return;
                }
                string[] kept = Directory.GetDirectories(original, ".files-download-*").SelectMany(Directory.GetFiles).ToArray();
                Assert.AreEqual(1, kept.Length, "The stage is kept in the folder the download opened.");
                Assert.AreEqual("-rw-------", Mode(kept[0]), "The stage kept is private.");

                Directory.Delete(into);
                Directory.Move(original, into);
                server.Intercept = null;
                int storageBefore = server.StorageRequests.Count;
                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", destination, Options(policy, checkpoint: true));

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(destination));
                Assert.AreEqual("GET 0-0", server.StorageRequests.Skip(storageBefore).Single().ToString(), "Nothing is fetched again: storage only confirms the version kept.");
                Assert.AreEqual(Contents.Length, result.ResumedBytes);
                Assert.IsFalse(File.Exists(Checkpoint));
                CollectionAssert.AreEqual(new string[0], Directory.GetDirectories(into, ".files-download-*"), "No stage is left behind.");
            }
        }

        // The folder is renamed at the very moment the file is put in place, just after the path was checked: the file is
        // in place in the folder the download opened, and the download says so, rather than that it is at the path given.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(DownloadPublicationPolicy.CreateNew)]
        [DataRow(DownloadPublicationPolicy.ReplaceRegularFile)]
        public async Task AFolderRenamedAsTheFileIsPutInPlaceIsReported(DownloadPublicationPolicy policy)
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("On Windows the folders are held for every policy, so they cannot be renamed.");
                return;
            }
            string into = Path.Combine(folder, "into");
            string original = Path.Combine(folder, "into-original");
            Directory.CreateDirectory(into);
            string destination = Path.Combine(into, "file.bin");
            if (policy != DownloadPublicationPolicy.CreateNew)
            {
                File.WriteAllText(destination, "OLD");
            }
            Native.BeforePlacingFaults.Value = () =>
            {
                Directory.Move(into, original);
                Directory.CreateDirectory(into);
            };
            DownloadPublicationException error;
            try
            {
                using (var server = new DownloadServer(Contents))
                {
                    error = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", destination, Options(policy)));
                }
            }
            finally
            {
                Native.BeforePlacingFaults.Value = null;
            }

            Assert.AreEqual(DownloadPublicationProblem.DestinationMoved, error.Problem);
            Assert.IsTrue(error.Placed, "The file is in place, in the folder the download opened.");
            Assert.IsFalse(error.Retained);
            StringAssert.Contains(error.Message, $"is not at {destination}");
            CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Path.Combine(original, "file.bin")));
            CollectionAssert.AreEqual(new string[0], Directory.GetFileSystemEntries(into), "Nothing is put in the folder now at the path.");
            CollectionAssert.AreEqual(new string[0], Directory.GetDirectories(original, ".files-download-*"), "No stage is left behind.");
        }

        // On macOS and Linux a run with the same checkpoint recognizes the file an earlier run put in place, and completes
        // it without fetching it again. Recognized in a directory no longer at the path given, renamed and replaced by
        // another just as it is recognized, it is not reported completed earlier: it is in place there, and it and its
        // checkpoint stay as they are, so that once the directory is back at its path a run completes it.
        [TestMethod]
        [Timeout(60000)]
        public async Task AFileAnEarlierRunPutInPlaceIsNotReportedWhereItsFolderNoLongerIs()
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("On Windows the folders are held for every policy, so they cannot be renamed.");
                return;
            }
            string into = Path.Combine(folder, "into");
            string original = Path.Combine(folder, "into-original");
            Directory.CreateDirectory(into);
            string destination = Path.Combine(into, "file.bin");
            using (var server = new DownloadServer(Contents))
            {
                Native.FlushFaults.Value = call => (call == Native.FlushCall.FullSyncDirectory || call == Native.FlushCall.SyncDirectory) && File.Exists(destination) ? 5 : 0;
                try
                {
                    DownloadPublicationException placed = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", destination, Options(DownloadPublicationPolicy.CreateNew, checkpoint: true)));
                    Assert.AreEqual(DownloadPublicationProblem.PlacedNotDurable, placed.Problem, "An earlier run that ended once the file was in place.");
                }
                finally
                {
                    Native.FlushFaults.Value = null;
                }
                string recorded = File.ReadAllText(Checkpoint);
                int storageBefore = server.StorageRequests.Count;
                DownloadCoordinator.BeforeRecoveringFaults.Value = () =>
                {
                    Directory.Move(into, original);
                    Directory.CreateDirectory(into);
                };
                DownloadPublicationException error;
                try
                {
                    error = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", destination, Options(DownloadPublicationPolicy.CreateNew, checkpoint: true)));
                }
                finally
                {
                    DownloadCoordinator.BeforeRecoveringFaults.Value = null;
                }

                Assert.AreEqual(DownloadPublicationProblem.DestinationMoved, error.Problem);
                Assert.IsTrue(error.Placed, "The file is in place, in the folder it was found in.");
                Assert.IsFalse(error.Retained);
                Assert.AreEqual(destination, error.LocalPath);
                Assert.AreEqual(Checkpoint, error.CheckpointPath);
                StringAssert.Contains(error.Message, $"so the file is not at {destination}");
                Assert.AreEqual(recorded, File.ReadAllText(Checkpoint), "Its checkpoint is left as it was.");
                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Path.Combine(original, "file.bin")));
                CollectionAssert.AreEqual(new string[0], Directory.GetFileSystemEntries(into), "Nothing is put in the folder now at the path.");
                Assert.AreEqual(storageBefore, server.StorageRequests.Count, "Nothing is fetched.");

                Directory.Delete(into);
                Directory.Move(original, into);
                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", destination, Options(DownloadPublicationPolicy.CreateNew, checkpoint: true));

                Assert.IsTrue(result.CompletedEarlier);
                Assert.AreEqual(destination, result.LocalPath);
                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(destination));
                Assert.AreEqual(storageBefore, server.StorageRequests.Count, "Nothing is fetched again.");
                Assert.IsFalse(File.Exists(Checkpoint));
                CollectionAssert.AreEqual(new string[0], Directory.GetDirectories(into, ".files-download-*"), "No stage is left behind.");
            }
        }

        // On Windows the file put in place has exactly the access control list of a file created directly in its directory,
        // as Windows works it out, its control (auto-inherited, not protected) included: with the entries the directory
        // passes on to its own files only (NO_PROPAGATE_INHERIT), an allow and a deny, which a file would not inherit
        // through a directory in between, beside one that propagates. It goes on inheriting as such a file does: a change
        // to what the directory passes on reaches both alike.
        [TestMethod]
        [Timeout(60000)]
        [OSCondition(OperatingSystems.Windows)]
        [DataRow(DownloadPublicationPolicy.ReplaceExisting)]
        [DataRow(DownloadPublicationPolicy.CreateNew)]
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public async Task OnWindowsThePlacedFileHasTheAccessOfAFileCreatedInItsDirectory(DownloadPublicationPolicy policy)
        {
            if (!OperatingSystem.IsWindows())
            {
                return;
            }
            string destination = Path.Combine(folder, "destination");
            var directory = new DirectoryInfo(destination);
            directory.Create();
            var users = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            var guests = new SecurityIdentifier(WellKnownSidType.BuiltinGuestsSid, null);
            var everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            DirectorySecurity security = directory.GetAccessControl();
            security.AddAccessRule(new FileSystemAccessRule(users, FileSystemRights.ReadAndExecute, InheritanceFlags.ObjectInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            security.AddAccessRule(new FileSystemAccessRule(guests, FileSystemRights.Write, InheritanceFlags.ObjectInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Deny));
            security.AddAccessRule(new FileSystemAccessRule(everyone, FileSystemRights.ReadAttributes, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow));
            directory.SetAccessControl(security);
            string direct = Path.Combine(destination, "direct.bin");
            File.WriteAllBytes(direct, new byte[1]);
            string expected = new FileInfo(direct).GetAccessControl().GetSecurityDescriptorSddlForm(AccessControlSections.Access);
            using (var server = new DownloadServer(Contents))
            {
                string placed = Path.Combine(destination, "file.bin");

                await Client(server).Transfers.DownloadFileAsync("remote.bin", placed, Options(policy));

                FileSecurity access = new FileInfo(placed).GetAccessControl();
                Assert.AreEqual(expected, access.GetSecurityDescriptorSddlForm(AccessControlSections.Access), "The list of a file created there.");
                FileSystemAccessRule[] inherited = access.GetAccessRules(false, true, typeof(SecurityIdentifier)).Cast<FileSystemAccessRule>().ToArray();
                Assert.IsTrue(inherited.Any(rule => rule.IdentityReference.Equals(users) && rule.AccessControlType == AccessControlType.Allow), "The allow for the directory's own files.");
                Assert.IsTrue(inherited.Any(rule => rule.IdentityReference.Equals(guests) && rule.AccessControlType == AccessControlType.Deny), "The deny for the directory's own files.");
                Assert.IsTrue(inherited.Any(rule => rule.IdentityReference.Equals(everyone)), "The entry that propagates.");
                Assert.IsFalse(access.AreAccessRulesProtected, "It inherits.");
                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(placed));

                var later = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
                security = directory.GetAccessControl();
                security.AddAccessRule(new FileSystemAccessRule(later, FileSystemRights.ReadData, InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                directory.SetAccessControl(security);
                string changed = new FileInfo(direct).GetAccessControl().GetSecurityDescriptorSddlForm(AccessControlSections.Access);
                Assert.AreNotEqual(expected, changed, "The directory now passes on another entry.");
                Assert.AreEqual(changed, new FileInfo(placed).GetAccessControl().GetSecurityDescriptorSddlForm(AccessControlSections.Access), "It reached the file put in place as it reached the one created there.");
            }
        }

        // On Windows a file replaced by ReplaceRegularFile, or by the default, leaves its access control list to the file
        // replacing it exactly, control included: a protected one stays protected, and later changes to its directory's
        // inheritable entries do not reach it; an inheriting one keeps its own and its inherited entries, and goes on
        // inheriting.
        [TestMethod]
        [Timeout(60000)]
        [OSCondition(OperatingSystems.Windows)]
        [DataRow(true, DownloadPublicationPolicy.ReplaceRegularFile, DisplayName = "Protected, ReplaceRegularFile")]
        [DataRow(false, DownloadPublicationPolicy.ReplaceRegularFile, DisplayName = "Inheriting, ReplaceRegularFile")]
        [DataRow(true, DownloadPublicationPolicy.ReplaceExisting, DisplayName = "Protected, by default")]
        [DataRow(false, DownloadPublicationPolicy.ReplaceExisting, DisplayName = "Inheriting, by default")]
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public async Task OnWindowsAReplacedFilesListIsKeptWithItsProtectionOrInheritance(bool isProtected, DownloadPublicationPolicy policy)
        {
            if (!OperatingSystem.IsWindows())
            {
                return;
            }
            string destination = Path.Combine(folder, "replaced");
            var directory = new DirectoryInfo(destination);
            directory.Create();
            string placed = Path.Combine(destination, "file.bin");
            File.WriteAllBytes(placed, new byte[1]);
            var users = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            FileSecurity own = new FileInfo(placed).GetAccessControl();
            own.AddAccessRule(new FileSystemAccessRule(users, FileSystemRights.Read, AccessControlType.Allow));
            if (isProtected)
            {
                own.SetAccessRuleProtection(isProtected: true, preserveInheritance: true);
            }
            new FileInfo(placed).SetAccessControl(own);
            string before = new FileInfo(placed).GetAccessControl().GetSecurityDescriptorSddlForm(AccessControlSections.Access);
            using (var server = new DownloadServer(Contents))
            {
                await Client(server).Transfers.DownloadFileAsync("remote.bin", placed, Options(policy));

                FileSecurity access = new FileInfo(placed).GetAccessControl();
                Assert.AreEqual(before, access.GetSecurityDescriptorSddlForm(AccessControlSections.Access), "The replaced file's list, control included.");
                Assert.AreEqual(isProtected, access.AreAccessRulesProtected);
                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(placed));

                var later = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
                DirectorySecurity security = directory.GetAccessControl();
                security.AddAccessRule(new FileSystemAccessRule(later, FileSystemRights.ReadData, InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                directory.SetAccessControl(security);
                bool reached = new FileInfo(placed).GetAccessControl().GetAccessRules(false, true, typeof(SecurityIdentifier)).Cast<FileSystemAccessRule>().Any(rule => rule.IdentityReference.Equals(later));
                Assert.AreEqual(!isProtected, reached, "A later entry of the directory reaches only an inheriting list.");
            }
        }

        // On Windows the destination directory is held, without sharing the right to rename or remove it, from the moment a
        // download starts, by default as for the other policies: another program cannot rename it away and put a link in
        // its place while the download runs, so the file is put in place in the very directory the download opened.
        [TestMethod]
        [Timeout(60000)]
        [OSCondition(OperatingSystems.Windows)]
        [DataRow(DownloadPublicationPolicy.ReplaceExisting)]
        [DataRow(DownloadPublicationPolicy.CreateNew)]
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public async Task OnWindowsTheDestinationDirectoryIsHeldWhateverThePolicy(DownloadPublicationPolicy policy)
        {
            if (!OperatingSystem.IsWindows())
            {
                return;
            }
            string chosen = Path.Combine(folder, "chosen");
            string moved = Path.Combine(folder, "moved");
            Directory.CreateDirectory(chosen);
            using (var server = new DownloadServer(Contents))
            {
                Exception refused = null;
                int attempts = 0;
                server.Intercept = (request, connection) =>
                {
                    if (Interlocked.Increment(ref attempts) == 1)
                    {
                        try
                        {
                            Directory.Move(chosen, moved);
                        }
                        catch (IOException e)
                        {
                            refused = e;
                        }
                    }
                    return Task.FromResult(false);
                };
                string placed = Path.Combine(chosen, "file.bin");

                await Client(server).Transfers.DownloadFileAsync("remote.bin", placed, Options(policy));

                Assert.IsNotNull(refused, "Renaming the held directory during the download is refused.");
                Assert.IsFalse(Directory.Exists(moved), "The directory stayed where it was.");
                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(placed));
                CollectionAssert.AreEqual(new string[0], Directory.GetDirectories(chosen, ".files-download-*"), "The staging directory was removed.");
            }
        }

        // On Windows a download into a folder on an SMB share, below a folder this user may not query, which Windows cannot
        // then give a normalized path for, is put in place all the same, with the access control list of a file created
        // directly there. It needs such a share: FILES_SDK_TEST_SMB_RESTRICTED_DESTINATION names a folder on one, which this
        // user may add files to and whose ancestors on the share it may pass through but not all query; without it the
        // test is inconclusive.
        [TestMethod]
        [Timeout(60000)]
        [OSCondition(OperatingSystems.Windows)]
        [DataRow(DownloadPublicationPolicy.ReplaceExisting)]
        [DataRow(DownloadPublicationPolicy.CreateNew)]
        public async Task OnWindowsAShareFolderBelowOneThatCannotBeQueriedStillGetsTheFileWithItsAccess(DownloadPublicationPolicy policy)
        {
            if (!OperatingSystem.IsWindows())
            {
                return;
            }
            string destination = Environment.GetEnvironmentVariable("FILES_SDK_TEST_SMB_RESTRICTED_DESTINATION");
            if (string.IsNullOrEmpty(destination))
            {
                Assert.Inconclusive("FILES_SDK_TEST_SMB_RESTRICTED_DESTINATION names no folder on an SMB share below one this user may not query.");
            }
            string run = Path.Combine(destination, Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(run);
            try
            {
                string direct = Path.Combine(run, "direct.bin");
                File.WriteAllBytes(direct, new byte[1]);
                string expected = new FileInfo(direct).GetAccessControl().GetSecurityDescriptorSddlForm(AccessControlSections.Access);
                using (var server = new DownloadServer(Contents))
                {
                    string placed = Path.Combine(run, "file.bin");

                    await Client(server).Transfers.DownloadFileAsync("remote.bin", placed, Options(policy));

                    Assert.AreEqual(expected, new FileInfo(placed).GetAccessControl().GetSecurityDescriptorSddlForm(AccessControlSections.Access), "The list of a file created there.");
                    CollectionAssert.AreEqual(Contents, File.ReadAllBytes(placed));
                    CollectionAssert.AreEqual(new[] { "direct.bin", "file.bin" }, Directory.GetFileSystemEntries(run).Select(Path.GetFileName).OrderBy(n => n, StringComparer.Ordinal).ToArray(), "Nothing else is left there.");
                }
            }
            finally
            {
                Directory.Delete(run, recursive: true);
            }
        }

        // On Windows a download into a folder this user may add files to, but whose own permissions it may not read (an
        // OWNER RIGHTS entry for the folder alone leaves its owner without READ_CONTROL), is put in place all the same,
        // with the access control list of a file created directly there, entries for the folder's own files included. It
        // runs as this user without the privileges that read any file's permissions whatever its list says, which an
        // administrator's token may hold enabled, so that the folder's list does refuse it.
        [TestMethod]
        [Timeout(60000)]
        [OSCondition(OperatingSystems.Windows)]
        [DataRow(DownloadPublicationPolicy.ReplaceExisting)]
        [DataRow(DownloadPublicationPolicy.CreateNew)]
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public async Task OnWindowsAFolderWhosePermissionsCannotBeReadStillGetsTheFileWithItsAccess(DownloadPublicationPolicy policy)
        {
            if (!OperatingSystem.IsWindows())
            {
                return;
            }
            string destination = Path.Combine(folder, "drop");
            var directory = new DirectoryInfo(destination);
            directory.Create();
            var users = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            var security = new DirectorySecurity();
            security.SetAccessRuleProtection(isProtected: true, preserveInheritance: false);
            // The folder itself: its owner may list, add and remove entries, and read their attributes, but not read or
            // change its permissions.
            security.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier("S-1-3-4"), FileSystemRights.ListDirectory | FileSystemRights.CreateFiles | FileSystemRights.CreateDirectories | FileSystemRights.DeleteSubdirectoriesAndFiles | FileSystemRights.ReadAttributes | FileSystemRights.Traverse | FileSystemRights.Delete | FileSystemRights.Synchronize, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow));
            // What it creates there is its own, and its own files may be read by Users.
            security.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.CreatorOwnerSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.InheritOnly, AccessControlType.Allow));
            security.AddAccessRule(new FileSystemAccessRule(users, FileSystemRights.ReadAndExecute, InheritanceFlags.ObjectInherit, PropagationFlags.NoPropagateInherit | PropagationFlags.InheritOnly, AccessControlType.Allow));
            directory.SetAccessControl(security);
            Console.WriteLine($"Unreadable test folder: {destination}, list {security.GetSecurityDescriptorSddlForm(AccessControlSections.Access)}");
            await WindowsTestAccess.WithoutBackupOrRestoreAsync(async () =>
            {
                Assert.ThrowsExactly<UnauthorizedAccessException>(() => directory.GetAccessControl(), "The folder's own permissions cannot be read.");
                string direct = Path.Combine(destination, "direct.bin");
                File.WriteAllBytes(direct, new byte[1]);
                string expected = new FileInfo(direct).GetAccessControl().GetSecurityDescriptorSddlForm(AccessControlSections.Access);
                using (var server = new DownloadServer(Contents))
                {
                    string placed = Path.Combine(destination, "file.bin");

                    await Client(server).Transfers.DownloadFileAsync("remote.bin", placed, Options(policy));

                    Assert.AreEqual(expected, new FileInfo(placed).GetAccessControl().GetSecurityDescriptorSddlForm(AccessControlSections.Access), "The list of a file created there.");
                    CollectionAssert.AreEqual(Contents, File.ReadAllBytes(placed));
                    CollectionAssert.AreEqual(new[] { "direct.bin", "file.bin" }, Directory.GetFileSystemEntries(destination).Select(Path.GetFileName).OrderBy(n => n, StringComparer.Ordinal).ToArray(), "Nothing else is left there.");
                }
                Assert.ThrowsExactly<UnauthorizedAccessException>(() => directory.GetAccessControl(), "The folder's own permissions still cannot be read once the file is there.");
            });
        }

        // On Windows the default policy follows a directory link the caller names, as it always has: the file lands in the
        // link's target, and gets the access control list of a file created directly in that target, not one worked out
        // from the link's own permissions, which differ.
        [TestMethod]
        [Timeout(60000)]
        [OSCondition(OperatingSystems.Windows)]
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public async Task OnWindowsAFileDownloadedThroughAFolderLinkGetsTheAccessOfItsTarget()
        {
            if (!OperatingSystem.IsWindows())
            {
                return;
            }
            string target = Path.Combine(folder, "target");
            var directory = new DirectoryInfo(target);
            directory.Create();
            var users = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            DirectorySecurity security = directory.GetAccessControl();
            security.AddAccessRule(new FileSystemAccessRule(users, FileSystemRights.ReadAndExecute, InheritanceFlags.ObjectInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            directory.SetAccessControl(security);
            string link = Path.Combine(folder, "link");
            try
            {
                Directory.CreateSymbolicLink(link, target);
            }
            catch (IOException)
            {
                Assert.Inconclusive("Creating symbolic links needs a privilege on Windows.");
            }
            catch (UnauthorizedAccessException)
            {
                Assert.Inconclusive("Creating symbolic links needs a privilege on Windows.");
            }
            string direct = Path.Combine(target, "direct.bin");
            File.WriteAllBytes(direct, new byte[1]);
            string expected = new FileInfo(direct).GetAccessControl().GetSecurityDescriptorSddlForm(AccessControlSections.Access);
            using (var server = new DownloadServer(Contents))
            {
                await Client(server).Transfers.DownloadFileAsync("remote.bin", Path.Combine(link, "file.bin"), Options(DownloadPublicationPolicy.ReplaceExisting));

                string placed = Path.Combine(target, "file.bin");
                Assert.AreEqual(expected, new FileInfo(placed).GetAccessControl().GetSecurityDescriptorSddlForm(AccessControlSections.Access), "The list of a file created in the target.");
                Assert.IsTrue(new FileInfo(placed).GetAccessControl().GetAccessRules(false, true, typeof(SecurityIdentifier)).Cast<FileSystemAccessRule>().Any(rule => rule.IdentityReference.Equals(users)), "The target's entry for its own files.");
                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(placed));
            }
        }

        // On Windows a stage is private, with an access control list of its own that allows only this user, SYSTEM and
        // Administrators, while it is written and once it is kept for a later run; the file put in place gets a new
        // file's access, inherited from its directory. The checkpoint is kept in the test's folder, which is first made
        // private, so that no other account can change it whatever this user's temporary directory allows.
        [TestMethod]
        [Timeout(60000)]
        [OSCondition(OperatingSystems.Windows)]
        [DataRow(DownloadPublicationPolicy.ReplaceExisting)]
        [DataRow(DownloadPublicationPolicy.CreateNew)]
        public async Task OnWindowsAKeptStageIsPrivateAndThePlacedFileInheritsItsAccess(DownloadPublicationPolicy policy)
        {
            if (!OperatingSystem.IsWindows())
            {
                return;
            }
            WindowsTestAccess.MakePrivate(folder);
            using (var server = new DownloadServer(Contents))
            {
                await CancelAtPart(server, 2, policy);
                AssertStageIsPrivate(Stages().Single(), "The stage kept is private.");

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Options(policy, checkpoint: true));

                Assert.AreEqual(2L * Part, result.ResumedBytes, "The kept stage is continued.");
                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                FileSecurity placed = new FileInfo(Destination).GetAccessControl();
                Assert.IsFalse(placed.AreAccessRulesProtected, "The file inherits from its directory, as a new file there does.");
                Assert.AreNotEqual(0, placed.GetAccessRules(false, true, typeof(SecurityIdentifier)).Count, "It has the entries its directory passes on.");
                AssertNoStage();
            }
        }

        // A policy is checked before anything is sent, and one that cannot apply is refused rather than ignored.
        [TestMethod]
        [Timeout(60000)]
        public async Task PoliciesThatCannotApplyAreRefusedBeforeAnythingIsSent()
        {
            using (var server = new DownloadServer(Contents))
            {
                FilesClient client = Client(server);
                Assert.AreEqual(nameof(DownloadOptions.PublicationPolicy), Assert.ThrowsExactly<InvalidParameterException>(() => client.Transfers.DownloadStreamAsync("remote.bin", new MemoryStream(), new DownloadOptions { PublicationPolicy = DownloadPublicationPolicy.CreateNew })).ParamName);
                await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => client.FileSystem.OpenReadAsync("remote.bin", new DownloadOptions { PublicationPolicy = DownloadPublicationPolicy.ReplaceRegularFile }));
                Assert.AreEqual(nameof(DownloadOptions.PublicationPolicy), Assert.ThrowsExactly<InvalidParameterException>(() => client.Transfers.DownloadFileAsync("remote.bin", Destination, new DownloadOptions { PublicationPolicy = (DownloadPublicationPolicy)7 })).ParamName);
                Assert.AreEqual(nameof(DownloadOptions.CheckpointRestartPolicy), Assert.ThrowsExactly<InvalidParameterException>(() => client.Transfers.DownloadFileAsync("remote.bin", Destination, new DownloadOptions { CheckpointRestartPolicy = (CheckpointRestartPolicy)7 })).ParamName);
                Assert.ThrowsExactly<InvalidParameterException>(() => client.Transfers.DownloadDirectoryAsync("", Path.Combine(folder, "tree"), new DirectoryDownloadOptions { FileOptions = new DownloadOptions { PublicationPolicy = (DownloadPublicationPolicy)7 } }));

                Assert.AreEqual(0, server.ApiRequests.Count);
                Assert.AreEqual(0, server.StorageRequests.Count);
                Assert.IsFalse(File.Exists(Destination));
            }
        }

        // Downloads the file with a checkpoint and policy, failing the flush of its directory once the file is in place,
        // as a run that ended right after putting it there would leave it.
        private async Task PlaceWithoutDurability(DownloadServer server, DownloadPublicationPolicy policy)
        {
            string destination = Destination;
            Native.FlushFaults.Value = call => (call == Native.FlushCall.FullSyncDirectory || call == Native.FlushCall.SyncDirectory) && File.Exists(destination) ? 5 : 0;
            try
            {
                DownloadPublicationException error = await Assert.ThrowsExactlyAsync<DownloadPublicationException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Options(policy, checkpoint: true)));
                Assert.AreEqual(DownloadPublicationProblem.PlacedNotDurable, error.Problem);
                Assert.IsTrue(error.Placed);
                Assert.IsFalse(error.Retained);
                StringAssert.Contains(error.Message, "was put in place, but may not survive a system crash");
            }
            finally
            {
                Native.FlushFaults.Value = null;
            }
            CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination), "The file stays in place.");
            Assert.AreEqual("complete", JsonNode.Parse(File.ReadAllText(Checkpoint))["phase"].GetValue<string>(), "Its checkpoint records it complete.");
        }

        // Cancels a download with a checkpoint as it asks for the part at index, leaving the parts before it recorded.
        private async Task CancelAtPart(DownloadServer server, int index, DownloadPublicationPolicy policy)
        {
            var cancellation = new CancellationTokenSource();
            server.Intercept = async (request, connection) =>
            {
                if (request.Range?.First == index * Part && !cancellation.IsCancellationRequested)
                {
                    cancellation.Cancel();
                    await connection.HoldAsync();
                    return true;
                }
                return false;
            };
            await Assert.ThrowsAsync<OperationCanceledException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Options(policy, checkpoint: true), cancellation.Token));
            server.Intercept = null;
        }

        // Rewrites the checkpoint's identities of the stage and its staging directory as an earlier version of this SDK
        // recorded them on Linux: their device and inode numbers alone, without the time each was created.
        private void RecordWithoutCreationTimes()
        {
            JsonObject recorded = JsonNode.Parse(File.ReadAllText(Checkpoint)).AsObject();
            foreach (string key in new[] { "stage_identity", "staging_identity" })
            {
                string identity = recorded[key]?.GetValue<string>();
                if (identity != null)
                {
                    recorded[key] = string.Join(":", identity.Split(':').Take(2));
                }
            }
            File.WriteAllText(Checkpoint, recorded.ToJsonString());
        }

        // Puts entry at path: a file, a directory, a symbolic link to target, which exists, or one to nothing.
        private static void Put(string entry, string path, string target)
        {
            switch (entry)
            {
                case "file":
                    File.WriteAllText(path, "OLD");
                    break;
                case "directory":
                    Directory.CreateDirectory(path);
                    File.WriteAllText(Path.Combine(path, "inside.txt"), "INSIDE");
                    break;
                case "link":
                    File.WriteAllText(target, "TARGET");
                    File.CreateSymbolicLink(path, target);
                    break;
                case "dangling link":
                    File.CreateSymbolicLink(path, target);
                    break;
            }
        }

        private static void AssertUnchanged(string entry, string path, string target)
        {
            switch (entry)
            {
                case "file":
                    Assert.AreEqual("OLD", File.ReadAllText(path));
                    break;
                case "directory":
                    Assert.AreEqual("INSIDE", File.ReadAllText(Path.Combine(path, "inside.txt")));
                    break;
                case "link":
                    Assert.AreEqual(target, new FileInfo(path).LinkTarget);
                    Assert.AreEqual("TARGET", File.ReadAllText(target), "The link is not followed.");
                    break;
                case "dangling link":
                    Assert.AreEqual(target, new FileInfo(path).LinkTarget);
                    Assert.IsFalse(File.Exists(target), "Nothing is written through the link.");
                    break;
            }
        }

        private static void Remove(string entry, string path)
        {
            if (entry == "directory")
            {
                Directory.Delete(path, true);
            }
            else
            {
                File.Delete(path);
            }
        }

        private string[] Stages()
        {
            return Directory.GetDirectories(folder, ".files-download-*").SelectMany(Directory.GetFiles).ToArray();
        }

        private void AssertNoStage()
        {
            CollectionAssert.AreEqual(new string[0], Directory.GetDirectories(folder, ".files-download-*"), "No stage is left behind.");
        }

        // That no one else may use stage: on Windows, its access control list is its own and allows only this user,
        // SYSTEM and Administrators; elsewhere, only its owner may read and write it.
        private static void AssertStageIsPrivate(string stage, string message)
        {
            if (OperatingSystem.IsWindows())
            {
                FileSecurity kept = new FileInfo(stage).GetAccessControl();
                string list = kept.GetSecurityDescriptorSddlForm(AccessControlSections.Access);
                Assert.IsTrue(kept.AreAccessRulesProtected, $"{message} The stage's list is its own: {list}");
                string[] trusted = { WindowsIdentity.GetCurrent().User.Value, "S-1-5-18", "S-1-5-32-544" };
                foreach (FileSystemAccessRule rule in kept.GetAccessRules(true, true, typeof(SecurityIdentifier)))
                {
                    Assert.IsTrue(rule.AccessControlType == AccessControlType.Deny || trusted.Contains(rule.IdentityReference.Value), $"{message} {rule.IdentityReference} may use the stage: {list}");
                }
                return;
            }
            Assert.AreEqual("-rw-------", Mode(stage), message);
        }

        private static FilesClient Client(DownloadServer server)
        {
            return server.Client();
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

        [System.Runtime.Versioning.UnsupportedOSPlatform("windows")]
        private static long Links(string path)
        {
            using (Microsoft.Win32.SafeHandles.SafeFileHandle handle = File.OpenHandle(path))
            {
                return Native.Stat(handle).Links;
            }
        }

        [System.Runtime.Versioning.UnsupportedOSPlatform("windows")]
        private static string Mode(string path)
        {
            return Run("/bin/ls", "-l", path).Substring(0, 10);
        }

        private static string StatOwnerFormat()
        {
            return OperatingSystem.IsMacOS() ? "-f%u" : "-c%u";
        }

        private static string Run(string program, params string[] arguments)
        {
            var start = new ProcessStartInfo(program) { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
            foreach (string argument in arguments)
            {
                start.ArgumentList.Add(argument);
            }
            using (Process process = Process.Start(start))
            {
                string output = process.StandardOutput.ReadToEnd();
                string errors = process.StandardError.ReadToEnd();
                if (!process.WaitForExit(10000))
                {
                    process.Kill();
                    throw new TimeoutException($"{program} did not end.");
                }
                Assert.AreEqual(0, process.ExitCode, $"{program} {string.Join(" ", arguments)}: {errors}");
                return output;
            }
        }
    }
}