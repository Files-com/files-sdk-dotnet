using FilesCom;
using FilesCom.Transfers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace FilesTests
{
    // A checkpoint is relied on only where no other user can have changed it: a transfer refuses one kept where another
    // user could write it, or put something else at its name, before it sends anything or reads what the checkpoint says,
    // and leaves the stage and destination as they were. Other users are stood in for by permissions, access control
    // entries and links this test sets, and by an owner the SDK is told to observe; no second account is used. The macOS
    // and Linux cases run there; the Windows case runs only on Windows.
    [TestClass]
    public class CheckpointIntegrityTest
    {
        private const int Part = 16 * 1024;

        private static readonly byte[] Contents = Enumerable.Range(0, 100 * 1024).Select(i => (byte)(i * 31 + 7)).ToArray();

        private string folder;

        [TestInitialize]
        public void Start()
        {
            folder = Path.Combine(Path.GetTempPath(), "checkpoint-integrity-test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
        }

        [TestCleanup]
        public void Stop()
        {
            if (!OperatingSystem.IsWindows())
            {
                foreach (string directory in Directory.GetDirectories(folder, "*", SearchOption.AllDirectories).Prepend(folder))
                {
                    if ((File.GetAttributes(directory) & FileAttributes.ReparsePoint) == 0)
                    {
                        File.SetUnixFileMode(directory, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
                    }
                }
            }
            Directory.Delete(folder, true);
        }

        private string Destination => Path.Combine(folder, "file.bin");

        // A download with a checkpoint in state, interrupted after its first three parts are written, then run again
        // after the change: refused before anything is sent or used, the stage and its parts kept; once the change is
        // undone, the same download continues from those parts.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("its directory writable by its group")]
        [DataRow("its directory writable by everyone")]
        [DataRow("a directory above writable by everyone")]
        [DataRow("the checkpoint writable by its group")]
        [DataRow("the checkpoint a link to another file")]
        [DataRow("the checkpoint another user's")]
        public async Task ADownloadRefusesACheckpointOthersCouldHaveChanged(string change)
        {
            if (OperatingSystem.IsWindows())
            {
                return;
            }
            string shared = Path.Combine(folder, "shared");
            string state = Path.Combine(shared, "state");
            Directory.CreateDirectory(state);
            string checkpoint = Path.Combine(state, "download.checkpoint");
            using (var server = new DownloadServer(Contents))
            {
                var options = new DownloadOptions { PartSize = Part, MaxConcurrency = 1, CheckpointPath = checkpoint };
                await Interrupt(server, options);
                Assert.AreEqual(UnixFileMode.UserRead | UnixFileMode.UserWrite, File.GetUnixFileMode(checkpoint), "Written readable and writable by its owner only.");
                CollectionAssert.AreEqual(new[] { checkpoint }, Directory.GetFileSystemEntries(state), "No temporary file is left.");
                string stage = Directory.GetDirectories(folder, ".files-download-*").Single();

                Action undo = Change(change, shared, state, checkpoint);
                int apiBefore = server.ApiRequests.Count;
                int storageBefore = server.StorageRequests.Count;

                TransferCheckpointException error;
                try
                {
                    error = await Assert.ThrowsExactlyAsync<TransferCheckpointException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options));
                }
                finally
                {
                    FilesCom.Transfers.CheckpointStorage.ObservedOwnerFaults.Value = null;
                }

                Assert.AreEqual(TransferCheckpointProblem.Untrusted, error.Problem, error.Message);
                Assert.AreEqual(checkpoint, error.CheckpointPath);
                Assert.AreEqual(apiBefore, server.ApiRequests.Count, "Nothing was sent.");
                Assert.AreEqual(storageBefore, server.StorageRequests.Count);
                Assert.IsFalse(File.Exists(Destination), "Nothing was published.");
                Assert.IsTrue(Directory.Exists(stage) && Directory.GetFiles(stage).Length == 1, "The stage is kept.");

                undo();
                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options);

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual(3 * Part, result.ResumedBytes, "The kept parts are used once the checkpoint can be relied on.");
                Assert.IsFalse(File.Exists(checkpoint));
            }
        }

        // On Windows: an entry of the checkpoint directory's access control list that lets Users write in it makes the
        // download refuse the checkpoint before it sends anything, keeping the stage; once the entry is removed, the
        // same download continues. The checkpoint the SDK writes has a private access control list of its own. The
        // checkpoint directory is first made private, so that the entry the test adds is the only one letting others in,
        // whatever this user's temporary directory allows.
        [TestMethod]
        [Timeout(60000)]
        [OSCondition(OperatingSystems.Windows)]
        public async Task ADownloadRefusesACheckpointOthersCouldHaveChangedOnWindows()
        {
            if (!OperatingSystem.IsWindows())
            {
                return;
            }
            string state = Path.Combine(folder, "state");
            Directory.CreateDirectory(state);
            WindowsTestAccess.MakePrivate(state);
            string checkpoint = Path.Combine(state, "download.checkpoint");
            using (var server = new DownloadServer(Contents))
            {
                var options = new DownloadOptions { PartSize = Part, MaxConcurrency = 1, CheckpointPath = checkpoint };
                await Interrupt(server, options);
                Assert.IsTrue(new FileInfo(checkpoint).GetAccessControl().AreAccessRulesProtected, "The checkpoint's list is its own.");
                var users = new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null), FileSystemRights.Write, AccessControlType.Allow);
                var directory = new DirectoryInfo(state);
                DirectorySecurity security = directory.GetAccessControl();
                security.AddAccessRule(users);
                directory.SetAccessControl(security);
                int apiBefore = server.ApiRequests.Count;

                TransferCheckpointException error = await Assert.ThrowsExactlyAsync<TransferCheckpointException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options));

                Assert.AreEqual(TransferCheckpointProblem.Untrusted, error.Problem, error.Message);
                Assert.AreEqual(apiBefore, server.ApiRequests.Count, "Nothing was sent.");
                Assert.IsFalse(File.Exists(Destination), "Nothing was published.");

                security = directory.GetAccessControl();
                security.RemoveAccessRule(users);
                directory.SetAccessControl(security);
                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options);

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual(3 * Part, result.ResumedBytes);
            }
        }

        // The netstandard2.0 build reaches the C library on Linux through NativeLibrary found by reflection; on this runtime
        // (.NET 8 or 10) the same code finds it and sets its resolver, and native calls still work after. (Whether Linux then
        // loads libc.so.6 through it is for a Linux run.)
        [TestMethod]
        public void TheCLibraryResolverTheNetstandardBuildSetsIsFoundByReflection()
        {
            if (OperatingSystem.IsWindows())
            {
                return;
            }
            Assert.IsTrue(Native.TryInstallLibcResolverByReflection());
            Assert.IsTrue(LocalDirectory.ByDescriptor, "The C library is still called.");
            Assert.AreEqual(Native.GetEuid(), Native.GetEuid());
        }

        // Where statx fails, Linux status comes from the kernel's older call, whose structure differs by architecture:
        // newfstatat's struct stat on x64 and Arm64, fstatat64's struct stat64 on 32-bit Arm and x86. Each is laid out
        // here as the kernel's headers declare it (arch/*/include/uapi/asm/stat.h; 64-bit members 8-byte aligned by
        // Arm's EABI and 4-byte aligned on x86), around bytes that are none of the fields, and must read as the same
        // identity statx gives: device (major and minor from new_encode_dev), inode, mode, owner, group, links and size.
        // (Whether a Linux kernel fills them so is for a Linux run on each architecture.)
        [TestMethod]
        public void LinuxStatusIsReadAsEachArchitecturesKernelLaysItOut()
        {
            const ulong major = 259;
            const ulong minor = 0x12345;
            ulong encoded = (minor & 0xFF) | (major << 8) | ((minor & ~0xFFUL) << 12);
            var layouts = new (System.Runtime.InteropServices.Architecture Architecture, int Inode, int Links, bool WideLinks, int Mode, int Uid, int Gid, int Size)[]
            {
                (System.Runtime.InteropServices.Architecture.X64, 8, 16, true, 24, 28, 32, 48),
                (System.Runtime.InteropServices.Architecture.Arm64, 8, 20, false, 16, 24, 28, 48),
                (System.Runtime.InteropServices.Architecture.Arm, 96, 20, false, 16, 24, 28, 48),
                (System.Runtime.InteropServices.Architecture.X86, 88, 20, false, 16, 24, 28, 44),
            };
            foreach (var layout in layouts)
            {
                byte[] status = Enumerable.Repeat((byte)0xEE, 256).ToArray();
                BitConverter.GetBytes(encoded).CopyTo(status, 0);
                BitConverter.GetBytes(0x100000002UL).CopyTo(status, layout.Inode);
                (layout.WideLinks ? BitConverter.GetBytes(3UL) : BitConverter.GetBytes(3U)).CopyTo(status, layout.Links);
                BitConverter.GetBytes(0x81A4U).CopyTo(status, layout.Mode);
                BitConverter.GetBytes(1000U).CopyTo(status, layout.Uid);
                BitConverter.GetBytes(1001U).CopyTo(status, layout.Gid);
                BitConverter.GetBytes(0x100000005L).CopyTo(status, layout.Size);

                Native.FileStatus read = Native.FromLinuxStat(status, layout.Architecture);

                string shown = layout.Architecture.ToString();
                Assert.AreEqual((major << 32) | minor, read.Device, shown + " device");
                Assert.AreEqual(0x100000002UL, read.Inode, shown + " inode");
                Assert.AreEqual(3L, read.Links, shown + " links");
                Assert.AreEqual(0x81A4, read.Mode, shown + " mode");
                Assert.AreEqual(1000U, read.Uid, shown + " owner");
                Assert.AreEqual(1001U, read.Gid, shown + " group");
                Assert.AreEqual(0x100000005L, read.Size, shown + " size");
            }
        }

        // Where the runtime cannot check who else can change a checkpoint (it cannot call the C library, as .NET Core 2.x on
        // Linux cannot; stood in for here by telling the SDK so), a checkpoint is refused, never used unchecked: before
        // anything is sent or read, leaving an existing checkpoint and its stage as they are, and writing none for a new
        // download. Where checks can be made again, the same download continues.
        [TestMethod]
        [Timeout(60000)]
        public async Task ACheckpointIsRefusedWhereTheRuntimeCannotCheckIt()
        {
            if (OperatingSystem.IsWindows())
            {
                return;
            }
            string checkpoint = Path.Combine(folder, "download.checkpoint");
            string fresh = Path.Combine(folder, "fresh.checkpoint");
            using (var server = new DownloadServer(Contents))
            {
                var options = new DownloadOptions { PartSize = Part, MaxConcurrency = 1, CheckpointPath = checkpoint };
                await Interrupt(server, options);
                byte[] recorded = File.ReadAllBytes(checkpoint);
                int apiBefore = server.ApiRequests.Count;

                TransferCheckpointException existing;
                TransferCheckpointException added;
                FilesCom.Transfers.CheckpointStorage.NativeUnavailableFaults.Value = true;
                try
                {
                    existing = await Assert.ThrowsExactlyAsync<TransferCheckpointException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options));
                    added = await Assert.ThrowsExactlyAsync<TransferCheckpointException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Path.Combine(folder, "other.bin"), new DownloadOptions { PartSize = Part, CheckpointPath = fresh }));
                }
                finally
                {
                    FilesCom.Transfers.CheckpointStorage.NativeUnavailableFaults.Value = false;
                }

                foreach (TransferCheckpointException error in new[] { existing, added })
                {
                    Assert.AreEqual(TransferCheckpointProblem.Untrusted, error.Problem, error.Message);
                    StringAssert.Contains(error.Message, "cannot call the C library");
                }
                Assert.AreEqual(apiBefore, server.ApiRequests.Count, "Nothing was sent.");
                CollectionAssert.AreEqual(recorded, File.ReadAllBytes(checkpoint), "The checkpoint is left as it was.");
                Assert.IsFalse(File.Exists(fresh), "No checkpoint was written.");
                Assert.IsFalse(File.Exists(Destination) || File.Exists(Path.Combine(folder, "other.bin")), "Nothing was published.");
                Assert.AreEqual(1, Directory.GetDirectories(folder, ".files-download-*").Length, "The stage is kept, and no other made.");

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options);

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual(3 * Part, result.ResumedBytes);
            }
        }

        // A download that has no checkpoint yet refuses to keep one where others could change it, before it sends
        // anything, and writes nothing there.
        [TestMethod]
        [Timeout(60000)]
        public async Task ANewDownloadKeepsNoCheckpointWhereOthersCouldChangeIt()
        {
            if (OperatingSystem.IsWindows())
            {
                return;
            }
            string state = Path.Combine(folder, "state");
            Directory.CreateDirectory(state);
            File.SetUnixFileMode(state, File.GetUnixFileMode(state) | UnixFileMode.GroupWrite);
            using (var server = new DownloadServer(Contents))
            {
                var options = new DownloadOptions { PartSize = Part, CheckpointPath = Path.Combine(state, "download.checkpoint") };

                TransferCheckpointException error = await Assert.ThrowsExactlyAsync<TransferCheckpointException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options));

                Assert.AreEqual(TransferCheckpointProblem.Untrusted, error.Problem);
                Assert.AreEqual(0, server.ApiRequests.Count, "Nothing was sent.");
                Assert.AreEqual(0, Directory.GetFileSystemEntries(state).Length);
                Assert.AreEqual(0, Directory.GetDirectories(folder, ".files-download-*").Length, "No stage was made.");
            }
        }

        // Defense in depth, for a checkpoint that can be relied on but records what its stage cannot back: more written
        // bytes than the stage holds, or permissions beyond those of a new file. It is refused before anything is sent,
        // and nothing is published. (Only this user can write such a checkpoint, as this test does.)
        [TestMethod]
        [Timeout(60000)]
        [DataRow("coverage past the stage")]
        [DataRow("permissions beyond a new file's")]
        public async Task ACheckpointClaimingWhatItsStageCannotBackIsRefused(string claim)
        {
            if (OperatingSystem.IsWindows())
            {
                return;
            }
            string checkpoint = Path.Combine(folder, "download.checkpoint");
            using (var server = new DownloadServer(Contents))
            {
                var options = new DownloadOptions { PartSize = Part, MaxConcurrency = 1, CheckpointPath = checkpoint };
                await Interrupt(server, options);
                JsonNode record = JsonNode.Parse(File.ReadAllText(checkpoint));
                if (claim == "coverage past the stage")
                {
                    record["written"] = new JsonArray(new JsonArray(0, Contents.Length));
                }
                else
                {
                    record["stage_mode"] = 0x1FF;
                }
                File.WriteAllText(checkpoint, record.ToJsonString());
                File.SetUnixFileMode(checkpoint, UnixFileMode.UserRead | UnixFileMode.UserWrite);
                int apiBefore = server.ApiRequests.Count;

                TransferCheckpointException error = await Assert.ThrowsExactlyAsync<TransferCheckpointException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options));

                Assert.AreEqual(TransferCheckpointProblem.Unreadable, error.Problem, error.Message);
                Assert.AreEqual(apiBefore, server.ApiRequests.Count, "Nothing was sent.");
                Assert.IsFalse(File.Exists(Destination), "Nothing was published.");
                Assert.AreEqual(1, Directory.GetDirectories(folder, ".files-download-*").Length, "The stage is kept.");
            }
        }

        // A continued download replacing an existing file gives the file that replaces it the existing file's access,
        // whatever permissions the checkpoint records for a new file.
        [TestMethod]
        [Timeout(60000)]
        public async Task AContinuedDownloadGivesTheFileItReplacesItsOwnAccess()
        {
            if (OperatingSystem.IsWindows())
            {
                return;
            }
            File.WriteAllText(Destination, "old");
            File.SetUnixFileMode(Destination, UnixFileMode.UserRead | UnixFileMode.UserWrite);
            string checkpoint = Path.Combine(folder, "download.checkpoint");
            using (var server = new DownloadServer(Contents))
            {
                var options = new DownloadOptions { PartSize = Part, MaxConcurrency = 1, CheckpointPath = checkpoint };
                await Interrupt(server, options);
                Assert.AreEqual("old", File.ReadAllText(Destination), "An interrupted download leaves the file as it was.");

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options);

                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual(3 * Part, result.ResumedBytes);
                Assert.AreEqual(UnixFileMode.UserRead | UnixFileMode.UserWrite, File.GetUnixFileMode(Destination));
            }
        }

        // An upload refuses a checkpoint kept where others could change it before it sends anything: whether it would
        // start a new upload, or its checkpoint records that the final request was sent.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(false, DisplayName = "A new upload")]
        [DataRow(true, DisplayName = "An upload whose final request is recorded as sent")]
        public async Task AnUploadRefusesACheckpointOthersCouldChange(bool finalRequestSent)
        {
            if (OperatingSystem.IsWindows())
            {
                return;
            }
            string state = Path.Combine(folder, "state");
            Directory.CreateDirectory(state);
            string checkpoint = Path.Combine(state, "upload.checkpoint");
            string source = Path.Combine(folder, "source.bin");
            File.WriteAllBytes(source, Contents.Take(3 * 1024).ToArray());
            using (var server = new UploadServer { PartSize = number => 1024 })
            {
                var options = new UploadOptions { CheckpointPath = checkpoint };
                if (finalRequestSent)
                {
                    server.RespondToFinish = body => Task.FromResult(LoopbackSite.Status(503));
                    await Assert.ThrowsExactlyAsync<UploadCompletionUnknownException>(() => FilesClient.Create(server.Site.Configuration()).Transfers.UploadFileAsync(source, "remote.bin", options));
                    Assert.IsTrue(JsonDocument.Parse(File.ReadAllText(checkpoint)).RootElement.GetProperty("final_request_sent").GetBoolean());
                }
                File.SetUnixFileMode(state, File.GetUnixFileMode(state) | UnixFileMode.GroupWrite);
                int before = server.Site.Requests.Count;

                TransferCheckpointException error = await Assert.ThrowsExactlyAsync<TransferCheckpointException>(() => FilesClient.Create(server.Site.Configuration()).Transfers.UploadFileAsync(source, "remote.bin", options));

                Assert.AreEqual(TransferCheckpointProblem.Untrusted, error.Problem, error.Message);
                Assert.AreEqual(before, server.Site.Requests.Count, "Nothing was sent.");
                Assert.AreEqual(finalRequestSent, File.Exists(checkpoint), "The checkpoint is left as it was.");
            }
        }

        // A directory transfer creates its checkpoint directory private, with any missing directory above it, and refuses
        // one others could change before it sends anything.
        [TestMethod]
        [Timeout(60000)]
        public async Task ADirectoryTransferKeepsItsCheckpointsPrivate()
        {
            if (OperatingSystem.IsWindows())
            {
                return;
            }
            string local = Path.Combine(folder, "local");
            Directory.CreateDirectory(local);
            File.WriteAllBytes(Path.Combine(local, "a.bin"), Contents.Take(10).ToArray());
            string checkpoints = Path.Combine(folder, "state", "checkpoints");
            using (var server = new UploadServer())
            {
                await FilesClient.Create(server.Site.Configuration()).Transfers.UploadDirectoryAsync(local, "backup", new DirectoryUploadOptions { CheckpointDirectory = checkpoints });

                Assert.AreEqual(UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute, File.GetUnixFileMode(checkpoints), "Created private.");
                Assert.AreEqual(UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute, File.GetUnixFileMode(Path.GetDirectoryName(checkpoints)), "So is the directory created above it.");

                File.SetUnixFileMode(checkpoints, File.GetUnixFileMode(checkpoints) | UnixFileMode.GroupWrite | UnixFileMode.GroupExecute);
                int before = server.Site.Requests.Count;

                TransferCheckpointException error = await Assert.ThrowsExactlyAsync<TransferCheckpointException>(() => FilesClient.Create(server.Site.Configuration()).Transfers.UploadDirectoryAsync(local, "backup", new DirectoryUploadOptions { CheckpointDirectory = checkpoints }));

                Assert.AreEqual(TransferCheckpointProblem.Untrusted, error.Problem, error.Message);
                Assert.AreEqual(before, server.Site.Requests.Count, "Nothing was sent.");
            }
        }

        // A checkpoint directory named outside the local directory, through a link, but physically inside it is refused
        // before anything is sent or created, as one named inside it is.
        [TestMethod]
        [Timeout(60000)]
        public async Task ACheckpointDirectoryInsideTheTreeThroughALinkIsRefused()
        {
            if (OperatingSystem.IsWindows())
            {
                return;
            }
            string local = Path.Combine(folder, "local");
            Directory.CreateDirectory(Path.Combine(local, "sub"));
            File.WriteAllBytes(Path.Combine(local, "a.bin"), Contents.Take(10).ToArray());
            string alias = Path.Combine(folder, "alias");
            Directory.CreateSymbolicLink(alias, Path.Combine(local, "sub"));
            using (var server = new UploadServer())
            {
                InvalidParameterException error = await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => FilesClient.Create(server.Site.Configuration()).Transfers.UploadDirectoryAsync(local, "backup", new DirectoryUploadOptions { CheckpointDirectory = Path.Combine(alias, "checkpoints") }));

                StringAssert.Contains(error.Message, "inside the local directory");
                Assert.AreEqual(0, server.Site.Requests.Count, "Nothing was sent.");
                Assert.IsFalse(Directory.Exists(Path.Combine(local, "sub", "checkpoints")), "Nothing was created in the tree.");
            }
        }

        // Makes the change the test names, and returns what undoes it.
        [System.Runtime.Versioning.UnsupportedOSPlatform("windows")]
        private Action Change(string change, string shared, string state, string checkpoint)
        {
            UnixFileMode stateMode = File.GetUnixFileMode(state);
            UnixFileMode sharedMode = File.GetUnixFileMode(shared);
            switch (change)
            {
                case "its directory writable by its group":
                    File.SetUnixFileMode(state, stateMode | UnixFileMode.GroupWrite);
                    return () => File.SetUnixFileMode(state, stateMode);
                case "its directory writable by everyone":
                    File.SetUnixFileMode(state, stateMode | UnixFileMode.OtherWrite);
                    return () => File.SetUnixFileMode(state, stateMode);
                case "a directory above writable by everyone":
                    // Anyone could put another directory, or a link to one, at state's name.
                    File.SetUnixFileMode(shared, sharedMode | UnixFileMode.OtherWrite);
                    return () => File.SetUnixFileMode(shared, sharedMode);
                case "the checkpoint writable by its group":
                    File.SetUnixFileMode(checkpoint, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.GroupWrite);
                    return () => File.SetUnixFileMode(checkpoint, UnixFileMode.UserRead | UnixFileMode.UserWrite);
                case "the checkpoint a link to another file":
                    string elsewhere = Path.Combine(folder, "elsewhere.checkpoint");
                    File.Move(checkpoint, elsewhere);
                    File.CreateSymbolicLink(checkpoint, elsewhere);
                    return () =>
                    {
                        File.Delete(checkpoint);
                        File.Move(elsewhere, checkpoint);
                    };
                default:
                    // The owner of the checkpoint file is observed as another user's: a file this test cannot give away.
                    ulong inode = Inode(checkpoint);
                    FilesCom.Transfers.CheckpointStorage.ObservedOwnerFaults.Value = (status, owner) => status.Inode == inode ? owner + 1 : owner;
                    return () => { };
            }
        }

        [System.Runtime.Versioning.UnsupportedOSPlatform("windows")]
        private static ulong Inode(string path)
        {
            using (FileStream file = File.OpenRead(path))
            {
                return FilesCom.Transfers.Native.Stat(file.SafeFileHandle, path).Inode;
            }
        }

        // Runs the download until its first three parts are written, then cancels it: its checkpoint and stage stay.
        private async Task Interrupt(DownloadServer server, DownloadOptions options)
        {
            var cancellation = new CancellationTokenSource();
            server.Intercept = async (request, connection) =>
            {
                if (request.Range?.First == 3 * Part && !cancellation.IsCancellationRequested)
                {
                    cancellation.Cancel();
                    await connection.HoldAsync();
                    return true;
                }
                return false;
            };
            await Assert.ThrowsAsync<OperationCanceledException>(() => Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, options, cancellation.Token));
            server.Intercept = null;
            Assert.IsTrue(File.Exists(options.CheckpointPath));
        }

        private static FilesClient Client(DownloadServer server)
        {
            return server.Client();
        }
    }
}