using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Transfers
{
    // A local directory that transfers read from and downloads write into, selected once by the path the caller gave and
    // from then on used as the directory it was, wherever it is later moved, and entered one name at a time without
    // following symbolic links (OpenChild, OpenFileToRead). A file is downloaded into a stage and made visible under its
    // name only when it is complete (Publish), replacing any file of that name, so an interrupted download never leaves a
    // partial file under the final name.
    //
    // Where the platform allows it (macOS, and Linux wherever the runtime can call the C library, Native), everything
    // below the selected directory goes through directory descriptors, as UnixDirectory describes, which another program
    // writing into the directory cannot redirect. On Windows download destinations (WindowsDirectory) are held open
    // from the moment they are selected, whatever the publication policy; upload sources are read by path. Elsewhere
    // (Linux on a runtime that cannot call the C library, such as .NET Core 2.x, or on a
    // processor other than x64, x86, Arm64 and 32-bit Arm, whose flag and structure layouts PlatformFlags does not
    // have; and other systems) directories and stages are used by path (PathDirectory), which a program renaming entries
    // in the directory at the same moment can redirect.
    internal abstract class LocalDirectory : IDisposable
    {
        // The directory as the caller named it, for messages.
        internal abstract string Path { get; }

        // Whether directories are held by descriptor here (UnixDirectory), rather than used by path.
#if NET6_0_OR_GREATER
        [System.Runtime.Versioning.UnsupportedOSPlatformGuard("windows")]
#endif
        internal static bool ByDescriptor => !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && UnixDirectory.Supported;

        // Opens an existing download destination, following symbolic links in path, which is the caller's choice:
        // on Windows it is held from now on, whatever the policy (WindowsDirectory).
        internal static LocalDirectory Open(string path, DownloadPublicationPolicy policy = DownloadPublicationPolicy.ReplaceExisting)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return WindowsDirectory.OpenPath(path);
            }
            if (ByDescriptor)
            {
                return UnixDirectory.OpenPath(path);
            }
            return PathDirectory.OpenPath(path);
        }

        // Opens an upload source. Windows reads it by path, as before: publication's root and ancestor holds are not
        // needed to read files, and would exclude readable sources on mounted volumes and restricted shares.
        internal static LocalDirectory OpenSource(string path)
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? PathDirectory.OpenPath(path) : Open(path);
        }

        // The directory called name inside this one, created if create and missing. name is one path component: never
        // empty, "." or "..", and never containing a separator. A symbolic link, or anything but a directory, is refused.
        internal abstract LocalDirectory OpenChild(string name, bool create);

        // This same directory, as a handle of its own to dispose.
        internal abstract LocalDirectory Duplicate();

        // The regular file called name in this directory, opened to read and shared only with readers. A symbolic link,
        // or anything but a regular file, is refused.
        internal abstract FileStream OpenFileToRead(string name);

        // A new, empty stage for the file finalName in this directory.
        internal abstract LocalStage CreateStage(string finalName);

        // The stage an earlier download left at location, for finalName, or null when it is gone, not this user's, or,
        // when identity is given, not the file with that identity (FileIdentity), whose staging directory, when
        // stagingIdentity is given, must have that one.
        internal abstract LocalStage ReopenStage(StageLocation location, string finalName, string identity = null, string stagingIdentity = null);

        // The stage an earlier download left at location, opened only to read, for a download whose checkpoint records
        // digests of what it wrote (DownloadCheckpoint.Extents), or null when it cannot be read safely: when it, or its
        // staging directory, is not this user's alone, is not what it should be (a regular file with one link, a
        // directory), is a link, or does not have the device and inode numbers recorded. Nothing about it is changed.
        // Where stages are not held by descriptor, none is opened.
        internal virtual SalvageSource OpenSalvage(StageLocation location, string identity, string stagingIdentity)
        {
            return null;
        }

        // What is at name in this directory, looked at without following a link, or null when nothing is.
        internal abstract LocalEntry Examine(string name);

        // Whether files can be put in place here as policy says, without a weaker way of doing it.
        internal abstract bool Supports(DownloadPublicationPolicy policy);

        // Completes the placing of a stage an earlier run put in place as name: its name in its staging directory, if it
        // still has one there as well (a link, not a rename, put it in place), is removed while it is the same file, and
        // then the staging directory while it is the one recorded and empty. Whatever else is there is left.
        internal abstract void FinishPlaced(StageLocation location, string identity, string stagingIdentity);

        // Flushes this directory's entries to storage (Durable), throwing when the flush fails; false where the file
        // system cannot flush a directory, or the platform is not asked to.
        internal abstract bool FlushEntries();

        // The directory's identity on storage (device and inode), or null where directories are used by path.
        internal virtual Native.FileStatus Identity => null;

        // Whether the path this directory was opened by still leads to it, so that a file found or put in it is where
        // that path says. Only a directory held by descriptor can have been moved away from its path while it was used
        // (UnixDirectory looks); one used by path is always where its path leads, and on Windows one is either used by
        // path or held from the root of its volume so that it cannot be renamed.
        internal virtual bool StillAtPath()
        {
            return true;
        }

        public abstract void Dispose();

        internal static void CheckName(string name)
        {
            if (string.IsNullOrEmpty(name) || name == "." || name == ".." || name.IndexOf('/') >= 0 || name.IndexOf('\0') >= 0
                || (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && name.IndexOfAny(new[] { '\\', ':' }) >= 0))
            {
                throw new ArgumentException($"\"{name}\" is not a name of a single file or directory.");
            }
        }
    }

    // Makes written data and directory entries reach storage, not only the system's cache, as far as the platform and
    // file system let this SDK do so. Where directories are held by descriptor (macOS and Linux), a file is flushed with
    // F_FULLFSYNC on macOS, or with fsync where the file system has no F_FULLFSYNC and on Linux, and a directory is flushed
    // after an entry is renamed into it; a flush that fails, rather than being unsupported, is an error. A file system can
    // report that it cannot flush a directory at all, and the rename then reaches storage when the system writes it:
    // Unsupported, which a caller that needs the rename to be durable must treat as a failure. So is a directory this
    // user may add files to but not list on Linux, which can be opened then only to name it (O_PATH), and is not flushed
    // through such a descriptor; macOS flushes one opened only to search it (O_SEARCH). Elsewhere (Windows), a file
    // is flushed (FlushFileBuffers) and this SDK has no way to flush a directory: NotAttempted. Whether storage itself keeps
    // what it acknowledged through a power loss is the hardware's promise, not this SDK's.
    internal static class Durable
    {
        internal enum DirectoryFlush
        {
            Flushed,
            Unsupported,
            NotAttempted,
        }

        internal static void Sync(FileStream file)
        {
            file.Flush(true);
            if (LocalDirectory.ByDescriptor)
            {
                Native.SyncFile(file.SafeFileHandle, $"The file {file.Name}");
            }
        }

        // Flushes the entries of the directory that holds path, such as path itself just renamed into it. Throws when the
        // flush fails.
        internal static DirectoryFlush SyncDirectoryOf(string path)
        {
            if (!LocalDirectory.ByDescriptor)
            {
                return DirectoryFlush.NotAttempted;
            }
            string directory = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path));
            int fd = Native.Open(directory, Native.Flags.ReadOnly | Native.Flags.Directory | Native.Flags.CloseOnExec);
            if (fd < 0)
            {
                throw Native.Failure(Marshal.GetLastWin32Error(), $"The directory {directory} cannot be opened to flush it");
            }
            try
            {
                return Native.SyncDirectory(fd, $"The directory {directory}") ? DirectoryFlush.Flushed : DirectoryFlush.Unsupported;
            }
            finally
            {
                Native.Close(fd);
            }
        }
    }

    // Where a stage is, relative to its destination directory, so a later download can find it again: the staging
    // directory and the stage's name in it. Names only, never a full path.
    internal sealed class StageLocation
    {
        internal StageLocation(string directory, string file, int? mode)
        {
            Directory = directory;
            File = file;
            Mode = mode;
        }

        internal string Directory { get; }

        internal string File { get; }

        // The permissions the published file gets, where the platform has them.
        internal int? Mode { get; }
    }

    // What is at a name in a directory, looked at without following a link.
    internal sealed class LocalEntry
    {
        internal LocalEntry(bool regular, string identity, long size)
        {
            Regular = regular;
            Identity = identity;
            Size = size;
        }

        // A regular file: not a directory, link, device or other special file.
        internal bool Regular { get; }

        // Its identity on storage (FileIdentity), or null where the platform has none to give.
        internal string Identity { get; }

        internal long Size { get; }
    }

    // A file's identity on storage, as text a checkpoint can record: its device and inode numbers on macOS and Linux, its
    // volume serial number and file index on Windows. It tells one file from another on the same storage while both
    // exist; it is not a hash of the contents. The number of a file since deleted can be given to a new one, as Linux
    // file systems do at once, so on Linux the identity also holds the time the file was created, where the file system
    // records it (Native.FileStatus.Birth): a file given a deleted one's number was created after it. Without that time,
    // as an earlier version of this SDK recorded identities there, and as they are read where the file system records
    // none or statx cannot be called, the numbers would fit any file later given them, so they identify nothing (Same).
    internal static class FileIdentity
    {
        // For tests: the birth time each object's identity records, or null for none, in place of the file system's, and
        // identities are then held to Linux's rule wherever the test runs (Strong).
        internal static readonly AsyncLocal<Func<Native.FileStatus, string>> BirthForTests = new AsyncLocal<Func<Native.FileStatus, string>>();

        internal static string Of(Native.FileStatus status)
        {
            if (status == null)
            {
                return null;
            }
            Func<Native.FileStatus, string> birth = BirthForTests.Value;
            string recorded = birth == null ? status.Birth : birth(status);
            return recorded == null ? $"{status.Device}:{status.Inode}" : $"{status.Device}:{status.Inode}:{recorded}";
        }

        internal static string Of(uint volumeSerial, uint indexHigh, uint indexLow)
        {
            return $"{volumeSerial:x8}:{indexHigh:x8}{indexLow:x8}";
        }

        // Whether identity, read now, names the file recorded as recorded: the same identity, which on Linux must hold the
        // time the file was created. Where either has none there, no file is shown to be the one recorded, so nothing
        // that relies on it is done: a stage is neither continued nor made private, a file in place is not taken for the
        // one an earlier run put there, and that run's names are not removed. The download goes on as for an object not
        // its own, as its publication and restart policies say, and the object is left as it is.
        internal static bool Same(string recorded, string identity)
        {
            if (recorded == null || identity == null || recorded != identity)
            {
                return false;
            }
            return Strong(recorded);
        }

        // Whether identity can name one object across runs (Same): on Linux, only with the time it was created.
        internal static bool Strong(string identity)
        {
            if (identity == null)
            {
                return false;
            }
            bool linux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || BirthForTests.Value != null;
            return !linux || identity.Split(':').Length == 3;
        }

        // Whether the object status describes has the device and inode numbers recorded: consistent with being the object
        // recorded, which, without the time it was created, it is not shown to be.
        internal static bool Consistent(string recorded, Native.FileStatus status)
        {
            if (recorded == null || status == null)
            {
                return false;
            }
            string[] parts = recorded.Split(':');
            return parts.Length >= 2 && parts[0] == status.Device.ToString(System.Globalization.CultureInfo.InvariantCulture) && parts[1] == status.Inode.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }

    // A stage an earlier download left, held open only to read its bytes (LocalDirectory.OpenSalvage): it has no way to
    // write, truncate, change, rename or remove the stage or its staging directory, which stay as they are.
    internal abstract class SalvageSource : IDisposable
    {
        // The stage, for messages.
        internal abstract string Path { get; }

        // Its length when it was opened.
        internal abstract long Length { get; }

        // Reads up to count bytes at offset into buffer; returns how many, 0 at the end.
        internal abstract int Read(long offset, byte[] buffer, int count);

        public abstract void Dispose();
    }

    // UnixDirectory's: the staging directory and the stage, held by the descriptors they were checked through.
    internal sealed class UnixSalvageSource : SalvageSource
    {
        private readonly int stagingFd;
        private readonly SafeFileHandle file;
        private readonly long length;
        private readonly string path;
        private int closed;
#if !NET6_0_OR_GREATER
        private readonly FileStream stream;
#endif

        internal UnixSalvageSource(int stagingFd, SafeFileHandle file, long length, string path)
        {
            this.stagingFd = stagingFd;
            this.file = file;
            this.length = length;
            this.path = path;
#if !NET6_0_OR_GREATER
            stream = new FileStream(file, FileAccess.Read, 1);
#endif
        }

        internal override string Path => path;

        internal override long Length => length;

        internal override int Read(long offset, byte[] buffer, int count)
        {
#if NET6_0_OR_GREATER
            return RandomAccess.Read(file, new Span<byte>(buffer, 0, count), offset);
#else
            stream.Position = offset;
            return stream.Read(buffer, 0, count);
#endif
        }

        public override void Dispose()
        {
            if (Interlocked.Exchange(ref closed, 1) == 0)
            {
#if !NET6_0_OR_GREATER
                stream.Dispose();
#endif
                file.Dispose();
                Native.Close(stagingFd);
            }
        }
    }

    // A file being downloaded, until it is published or discarded. Writes go to the stage's own open handle, wherever
    // its entry is. Disposing it closes it, and deletes it unless it was published, kept for a later resume, or lost.
    internal abstract class LocalStage : IDisposable
    {
        private int state; // 0 open, 1 published, 2 kept, 3 lost

        internal abstract StageLocation Location { get; }

        // The stage file's identity on storage (FileIdentity), and its staging directory's, or null where the platform
        // has none to give.
        internal abstract string Identity { get; }

        internal virtual string StagingIdentity => null;

        // Whether a later run could not show this stage, or its staging directory, to be this download's (FileIdentity
        // .Strong), so its checkpoint must record digests of what is written to it (DownloadCheckpoint.Extents).
        internal bool DigestsRequired => Identity != null && (!FileIdentity.Strong(Identity) || (StagingIdentity != null && !FileIdentity.Strong(StagingIdentity)));

        internal abstract long Length { get; }

        internal abstract Task WriteAsync(long offset, byte[] buffer, int count, CancellationToken cancellationToken);

        internal abstract void SetLength(long length);

        // Makes everything written so far durable, so a checkpoint may record it.
        internal abstract void Flush();

        // Makes the stage's own names durable: its file's entry in its staging directory and that directory's entry in the
        // download's directory, so that a checkpoint naming a new stage in place of another can replace that one's
        // checkpoint. Throws where either cannot be flushed to storage.
        internal virtual void FlushNames()
        {
            throw new IOException("The names of a stage on this platform cannot be flushed to storage.");
        }

        // Whether the stage has been put in place as the destination file, even if what followed failed.
        internal bool Placed => state == 1;

        // Whether the stage was lost to this download: its name in its staging directory was found to lead to another
        // file (UnixDirectory). It is then neither in place nor kept, and nothing at its name is removed.
        internal bool Lost => state == 3;

        // Flushes the stage and makes it visible as the destination file under the selected policy.
        // DownloadPublicationException describes policy-specific refusals and reported publication-stage failures;
        // ordinary filesystem I/O exceptions may also propagate. Placed records whether the stage was put in place.
        internal void Publish(DownloadPublicationPolicy policy = DownloadPublicationPolicy.ReplaceExisting)
        {
            PublishCore(policy);
            state = 1;
        }

        // After a failure, cancellation or refusal before the file was in place: makes the stage private again, as it
        // was before its access was changed to the destination's, and returns whether it is shown to be, so that it may
        // be kept for a later run; a stage that cannot be made private again, or never was, is not kept, except by the
        // path fallback (PathDirectory), which cannot make a stage private and keeps it as it is.
        internal abstract bool RestorePrivacy();

        // Called by PublishCore once the stage is the destination file, so that a failure after that, such as flushing
        // the directory, is not taken for an unpublished stage.
        protected void MarkPlaced()
        {
            state = 1;
        }

        // Called by PublishCore when the stage's name no longer leads to the stage (Lost).
        protected void MarkLost()
        {
            state = 3;
        }

        // Leaves the stage where it is when disposed, for a later download to continue.
        internal void Keep()
        {
            if (state == 0)
            {
                state = 2;
            }
        }

        public void Dispose()
        {
            Close(delete: state == 0, published: state == 1);
        }

        protected abstract void PublishCore(DownloadPublicationPolicy policy);

        // Closes the stage; deletes it when delete, and its emptied staging directory when delete or published.
        protected abstract void Close(bool delete, bool published);
    }

    // Windows upload sources, and the fallback on other systems where directories cannot be held by descriptor:
    // directories by path. Only the fallback publishes stages. A stage is a file in a staging directory of its own
    // inside the destination directory, held open without
    // sharing until it is published. Neither has an identity this fallback can give, nor can it make them private: they
    // get the permissions new files and directories get, and a stage kept for a later run is kept as it is.
    internal sealed class PathDirectory : LocalDirectory
    {
        private readonly string path;

        private PathDirectory(string path)
        {
            this.path = path;
        }

        internal override string Path => path;

        internal static PathDirectory OpenPath(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"The directory {path} does not exist.");
            }
            return new PathDirectory(System.IO.Path.GetFullPath(path));
        }

        internal override LocalDirectory OpenChild(string name, bool create)
        {
            CheckName(name);
            string child = System.IO.Path.Combine(path, name);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && WindowsPublication.IsAnotherEntrysShortName(child, name))
            {
                throw new IOException($"{child} is the short name of a local folder with another name, which the transfer does not use.");
            }
            var info = new DirectoryInfo(child);
            if (!info.Exists)
            {
                if (File.Exists(child) || !create)
                {
                    throw new IOException($"{child} is not a directory.");
                }
                info.Create();
                info.Refresh();
            }
            if ((info.Attributes & FileAttributes.ReparsePoint) != 0)
            {
                throw new IOException($"{child} is a symbolic link or other reparse point, which transfers do not follow.");
            }
            return new PathDirectory(child);
        }

        internal override LocalDirectory Duplicate()
        {
            return new PathDirectory(path);
        }

        internal override FileStream OpenFileToRead(string name)
        {
            CheckName(name);
            string child = System.IO.Path.Combine(path, name);
            if ((File.GetAttributes(child) & (FileAttributes.ReparsePoint | FileAttributes.Directory | FileAttributes.Device)) != 0)
            {
                throw new IOException($"{child} is not a regular file, which transfers read only.");
            }
            return new FileStream(child, FileMode.Open, FileAccess.Read, FileShare.Read, 1);
        }

        internal override LocalStage CreateStage(string finalName)
        {
            CheckName(finalName);
            string staging = System.IO.Path.Combine(path, StagingName());
            Directory.CreateDirectory(staging);
            try
            {
                return new PathStage(path, staging, "download", finalName, create: true);
            }
            catch
            {
                TryDeleteDirectory(staging);
                throw;
            }
        }

        internal override LocalStage ReopenStage(StageLocation location, string finalName, string identity = null, string stagingIdentity = null)
        {
            CheckName(finalName);
            CheckName(location.Directory);
            CheckName(location.File);
            string staging = System.IO.Path.Combine(path, location.Directory);
            if (!File.Exists(System.IO.Path.Combine(staging, location.File)) || (new DirectoryInfo(staging).Attributes & FileAttributes.ReparsePoint) != 0)
            {
                return null;
            }
            PathStage stage;
            try
            {
                stage = new PathStage(path, staging, location.File, finalName, create: false);
            }
            catch (IOException)
            {
                return null;
            }
            if (stage.Opened && (identity == null || FileIdentity.Same(identity, stage.Identity)))
            {
                return stage;
            }
            // Not the stage recorded: whatever it is stays.
            stage.Keep();
            stage.Dispose();
            return null;
        }

        internal override LocalEntry Examine(string name)
        {
            CheckName(name);
            string child = System.IO.Path.Combine(path, name);
            FileAttributes attributes;
            try
            {
                attributes = File.GetAttributes(child);
            }
            catch (Exception e) when (e is FileNotFoundException || e is DirectoryNotFoundException)
            {
                return null;
            }
            bool regular = (attributes & (FileAttributes.ReparsePoint | FileAttributes.Directory | FileAttributes.Device)) == 0;
            return new LocalEntry(regular, null, regular ? new FileInfo(child).Length : 0);
        }

        // Windows uses this only to read upload sources, never to publish. The fallback on other systems supports only
        // the default, which does not replace a file there at all.
        internal override bool Supports(DownloadPublicationPolicy policy)
        {
            return !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && policy == DownloadPublicationPolicy.ReplaceExisting;
        }

        internal override void FinishPlaced(StageLocation location, string identity, string stagingIdentity)
        {
            CheckName(location.Directory);
            TryDeleteDirectory(System.IO.Path.Combine(path, location.Directory));
        }

        internal override bool FlushEntries()
        {
            return false;
        }

        public override void Dispose()
        {
        }

        internal static string StagingName()
        {
            return ".files-download-" + Guid.NewGuid().ToString("N");
        }

        internal static void TryDeleteDirectory(string directory)
        {
            try
            {
                Directory.Delete(directory);
            }
            catch (Exception)
            {
                // Left behind, empty or not ours to remove.
            }
        }

        private sealed class PathStage : LocalStage
        {
            private readonly string directory;
            private readonly string staging;
            private readonly string name;
            private readonly string finalName;
            private readonly FileStream file;
            private readonly SemaphoreSlim writing = new SemaphoreSlim(1, 1);

            internal PathStage(string directory, string staging, string name, string finalName, bool create)
            {
                this.directory = directory;
                this.staging = staging;
                this.name = name;
                this.finalName = finalName;
                file = new FileStream(System.IO.Path.Combine(staging, name), create ? FileMode.CreateNew : FileMode.Open, FileAccess.ReadWrite, FileShare.None, 1);
            }

            // Whether the stage was there to open.
            internal bool Opened => file != null;

            internal override StageLocation Location => new StageLocation(System.IO.Path.GetFileName(staging), name, null);

            internal override string Identity => null;

            internal override long Length => file.Length;

            internal override async Task WriteAsync(long offset, byte[] buffer, int count, CancellationToken cancellationToken)
            {
#if NET6_0_OR_GREATER
                await RandomAccess.WriteAsync(file.SafeFileHandle, new ReadOnlyMemory<byte>(buffer, 0, count), offset, cancellationToken);
#else
                await writing.WaitAsync(cancellationToken);
                try
                {
                    file.Position = offset;
                    await file.WriteAsync(buffer, 0, count, cancellationToken);
                }
                finally
                {
                    writing.Release();
                }
#endif
            }

            internal override void SetLength(long length)
            {
                file.SetLength(length);
            }

            internal override void Flush()
            {
                file.Flush(true);
            }

            // Puts the stage in place. This fallback has no way to give the stage the access of the file it would replace,
            // which a rename would lose, so an existing file is not replaced: the download fails, and the file stays as
            // it is. Another policy is refused before anything is fetched (Supports).
            protected override void PublishCore(DownloadPublicationPolicy policy)
            {
                string stage = System.IO.Path.Combine(staging, name);
                string destination = System.IO.Path.Combine(directory, finalName);
                if (policy != DownloadPublicationPolicy.ReplaceExisting || File.Exists(destination))
                {
                    throw policy != DownloadPublicationPolicy.ReplaceExisting
                        ? new DownloadPublicationException(DownloadPublicationProblem.NotSupported, destination, $"On this platform the SDK cannot put {destination} in place as {policy} says, so it was not put in place.")
                        : new DownloadPublicationException(DownloadPublicationProblem.AccessNotKept, destination, $"The file {destination} exists, and on this platform the SDK cannot give the file replacing it the same access, so it was not replaced.");
                }
                file.Flush(true);
                file.Dispose();
#if NETCOREAPP3_0_OR_GREATER
                File.Move(stage, destination, overwrite: true);
#else
                if (File.Exists(destination))
                {
                    File.Replace(stage, destination, null);
                }
                else
                {
                    File.Move(stage, destination);
                }
#endif
                MarkPlaced();
                TryDeleteDirectory(staging);
            }

            // This fallback cannot make a stage private; it keeps it with the permissions it was created with.
            internal override bool RestorePrivacy()
            {
                return true;
            }

            protected override void Close(bool delete, bool published)
            {
                if (file == null)
                {
                    return;
                }
                file.Dispose();
                writing.Dispose();
                if (delete)
                {
                    try
                    {
                        File.Delete(System.IO.Path.Combine(staging, name));
                    }
                    catch (Exception)
                    {
                        // Reported by the download's own failure, which is what the caller sees.
                    }
                    TryDeleteDirectory(staging);
                }
            }
        }
    }

    // Directories held by descriptor, on macOS, and on Linux wherever the runtime can call the C library (Native). The
    // selected directory is opened by its path; every name below it is opened relative to its parent's descriptor with
    // O_NOFOLLOW, so a symbolic link placed in the tree is never followed, and a directory renamed or replaced after it
    // was opened does not change where the download writes. A directory is opened to read it, or, where this user may
    // add entries to it but not list it, only to search it (O_SEARCH on macOS, O_PATH on Linux): creating, examining,
    // renaming and removing entries by name need no more. The one thing such a descriptor cannot do on Linux is flush
    // the directory's entries (Durable); macOS flushes through it.
    //
    // A stage lives in a staging directory of its own, .files-download-<random>, created in the destination directory
    // and made private before anything is put in it: its permissions become 0700, which succeeds only for its owner and
    // so proves the directory opened is the one this process created, and any access control list it inherited from the
    // destination directory is removed, so that those permissions alone decide who can use it. Only its owner can then
    // add, rename or remove entries in it. The stage is created in it with mkstemp, which goes by path, so it counts only
    // once it is shown to be the very file held open (same device and inode) at that name in the staging directory's
    // descriptor; it too is made owner-only, without an access control list. Every object's identity is recorded when
    // it is established, and an entry is removed only while it is still that object: a staging directory someone moved
    // away stays where they put it, and whatever took its name is left alone.
    //
    // Publishing renames the stage from the staging directory's descriptor to the destination directory's, after the
    // stage is flushed to storage and given its access. A file it replaces keeps who may use it: its access control list,
    // group and permissions go to the stage, the permissions last, so that they decide. They are read from the file at
    // that name in the held destination directory, through a descriptor of that very file, never by path: opened to
    // read, or to write where it may only be written, or on Linux only to name it; a file this user can neither read nor
    // write on macOS cannot be examined that way, and is not replaced. The new file is given the old one's owner and
    // group too; when this user cannot give it either, as with a file of another user's, or of a group this user is not
    // in, the file keeps who may use it only by not being replaced: the download fails before the rename, and the file
    // stays as it was. A new file gets a new file's permissions (0666 less the umask) and no access control list. The destination directory is flushed after the rename, so the new entry, not only the file's bytes, reaches
    // storage (Durable).
    //
    // The file put in place is the one the download wrote, where its path says, or the download says otherwise. A
    // rename goes by name, and the staging directory is private to this user, not to this process: another program
    // running as this user can rename the stage away and put another file at its name. So, just before the rename, the
    // entry at the stage's name must be the stage the download holds open, and the path the download was given must
    // still lead to the directory it opened (a renamed or replaced directory would put the file somewhere that path does
    // not say); otherwise nothing is put in place (DownloadPublicationProblem.StageReplaced, DestinationMoved). Just after,
    // the entry at the file's name must be the stage, and the path must still lead to the directory; otherwise the
    // download fails saying what is where: another file put in place instead of the stage (StageReplaced), or the file in
    // place in a directory no longer at that path (DestinationMoved, placed). These are checks of names at the moment
    // each is made, not a lock: what they find after the rename is what the file's name led to then. An entry found to
    // be another file is left where it is. A file an earlier run put in place, recognized instead of downloaded again
    // (DownloadCoordinator.RecoverPlaced, DirectoryTransfer), counts only while the same path leads to the directory it
    // was found in (StillAtPath), looked at after anything the recognition waited for.
    //
    // The guarantee is against other users who can write into the destination tree; a program running as the same user,
    // or an administrator, can do anything this one can, and the checks above only report, as far as they can, what such
    // a program did. A process running as root can change the permissions of any directory, so for it the ownership
    // proof holds nothing. Checking an entry and removing it are two calls, not one atomic step: in the private staging
    // directory no other user can change entries between them (a program running as this user can), but the staging
    // directory itself is removed from the destination directory, where a user who may rename entries there could put
    // another empty directory at its name between the two, which would then be removed in its place.
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.UnsupportedOSPlatform("windows")]
#endif
    internal sealed class UnixDirectory : LocalDirectory
    {
        private const string StageTemplate = "download.XXXXXX";

        private readonly int fd;
        private readonly string path;
        // The directory's full path, as the download was given it (StillAtPath).
        private readonly string full;
        // Whether fd was opened only to search the directory, which this user may not list.
        private readonly bool searchOnly;
        private int closed;

        private UnixDirectory(int fd, string path, string full, bool searchOnly)
        {
            this.fd = fd;
            this.path = path;
            this.full = full;
            this.searchOnly = searchOnly;
        }

        internal static bool Supported => Native.Flags != null;

        internal override string Path => path;

        internal override Native.FileStatus Identity => Native.Stat(fd, $"The directory {path}");

        // Opens the directory at path, following the links in it, which are the caller's choice: to read it, or only to
        // search it where this user may not list it (see the class's remarks).
        internal static UnixDirectory OpenPath(string path)
        {
            string full = System.IO.Path.GetFullPath(path);
            bool searchOnly = false;
            int fd = Native.Open(full, Native.Flags.ReadOnly | Native.Flags.Directory | Native.Flags.CloseOnExec);
            if (fd < 0)
            {
                int errno = Marshal.GetLastWin32Error();
                if (errno == Native.EACCES)
                {
                    fd = Native.Open(full, Native.Flags.SearchOnly | Native.Flags.CloseOnExec);
                    searchOnly = fd >= 0;
                }
                if (fd < 0)
                {
                    throw Native.Failure(errno, $"The directory {path} cannot be opened");
                }
            }
            return new UnixDirectory(fd, path, full, searchOnly);
        }

        internal override LocalDirectory OpenChild(string name, bool create)
        {
            CheckName(name);
            string child = System.IO.Path.Combine(path, name);
            int childFd = OpenDestinationAt(fd, name, out bool childSearchOnly, out int errno);
            if (childFd < 0 && errno == Native.ENOENT && create)
            {
                if (Native.MkdirAt(fd, name, 0x1FF) != 0 && Marshal.GetLastWin32Error() != Native.EEXIST)
                {
                    throw Native.Failure(Marshal.GetLastWin32Error(), $"The directory {child} cannot be created");
                }
                childFd = OpenDestinationAt(fd, name, out childSearchOnly, out errno);
            }
            if (childFd < 0)
            {
                // macOS reports a symbolic link opened as a directory without following it as not a directory. The link
                // is looked at only to say so.
                if (errno == Native.ENOTDIR && (new FileInfo(child).Attributes & FileAttributes.ReparsePoint) != 0)
                {
                    throw new IOException($"The directory {child} cannot be opened: it is a symbolic link, which transfers do not follow.");
                }
                throw Native.Failure(errno, $"The directory {child} cannot be opened");
            }
            return new UnixDirectory(childFd, child, System.IO.Path.Combine(full, name), childSearchOnly);
        }

        internal override LocalDirectory Duplicate()
        {
            int copy = OpenDestinationAt(fd, ".", out bool copySearchOnly, out int errno);
            if (copy < 0)
            {
                throw Native.Failure(errno, $"The directory {path} cannot be opened again");
            }
            return new UnixDirectory(copy, path, full, copySearchOnly);
        }

        // Whether the path the download was given for this directory still leads to it (the same device and inode),
        // following the links in that path as the system does now. False when it leads to another directory, or to
        // nothing this user can reach, as when the directory was renamed, or replaced by another, after it was opened:
        // a file put in it, or found in it, would then not be where that path says.
        internal override bool StillAtPath()
        {
            Native.FileStatus now = Native.TryStatPath(full, out _);
            return now != null && now.Same(Native.Stat(fd, $"The directory {path}"));
        }

        internal override FileStream OpenFileToRead(string name)
        {
            CheckName(name);
            string child = System.IO.Path.Combine(path, name);
            // Without blocking, so a pipe put in the file's place cannot hold the transfer, and refused below.
            int fileFd = Native.OpenAt(fd, name, Native.Flags.ReadOnly | Native.Flags.NoFollow | Native.Flags.CloseOnExec | Native.Flags.NonBlock);
            if (fileFd < 0)
            {
                int errno = Marshal.GetLastWin32Error();
                if (errno == Native.ENOENT)
                {
                    throw new System.IO.FileNotFoundException($"The file {child} does not exist.", child);
                }
                throw Native.Failure(errno, $"The file {child} cannot be opened");
            }
            var handle = new SafeFileHandle((IntPtr)fileFd, ownsHandle: true);
            FileStream stream;
            try
            {
                stream = new FileStream(handle, FileAccess.Read, 1);
            }
            catch (UnauthorizedAccessException)
            {
                handle.Dispose();
                throw new IOException($"{child} is not a regular file, which transfers read only.");
            }
            catch
            {
                handle.Dispose();
                throw;
            }
            if (!stream.CanSeek)
            {
                stream.Dispose();
                throw new IOException($"{child} is not a regular file, which transfers read only.");
            }
            return stream;
        }

        internal override LocalStage CreateStage(string finalName)
        {
            CheckName(finalName);
            string stagingName = PathDirectory.StagingName();
            // Created open to the umask, so its permissions show the umask a new file would get, then made private at
            // once: nothing is inside it until then.
            if (Native.MkdirAt(fd, stagingName, 0x1FF) != 0)
            {
                throw Native.Failure(Marshal.GetLastWin32Error(), $"A staging directory cannot be created in {path}");
            }
            int stagingFd = OpenDirectoryAt(fd, stagingName);
            if (stagingFd < 0)
            {
                // Whatever is at that name now was not shown to be this download's, so it stays.
                throw Native.Failure(Marshal.GetLastWin32Error(), $"The staging directory created in {path} cannot be opened");
            }
            Native.FileStatus staging = null;
            try
            {
                int created = Native.Stat(stagingFd, $"The staging directory {stagingName} in {path}").Mode & 0x1FF;
                staging = MakeStagingPrivate(stagingFd, stagingName);
                int umask = ~created & 0x1FF;
                return UnixStage.Create(this, stagingName, stagingFd, staging, finalName, 0x1B6 & ~umask);
            }
            catch
            {
                Native.Close(stagingFd);
                RemoveOwned(fd, stagingName, staging, directory: true);
                throw;
            }
        }

        internal override LocalStage ReopenStage(StageLocation location, string finalName, string identity = null, string stagingIdentity = null)
        {
            CheckName(finalName);
            CheckName(location.Directory);
            CheckName(location.File);
            int stagingFd = OpenDirectoryAt(fd, location.Directory);
            if (stagingFd < 0)
            {
                return null;
            }
            Native.FileStatus staging;
            SafeFileHandle file = null;
            try
            {
                // Looked at before anything is changed: a staging directory other than the one recorded is left alone.
                if (stagingIdentity != null && !FileIdentity.Same(stagingIdentity, FileIdentity.Of(Native.Stat(stagingFd, $"The staging directory {location.Directory} in {path}"))))
                {
                    Native.Close(stagingFd);
                    return null;
                }
                staging = MakeStagingPrivate(stagingFd, location.Directory);
                int fileFd = Native.OpenAt(stagingFd, location.File, Native.Flags.ReadWrite | Native.Flags.NoFollow | Native.Flags.CloseOnExec);
                if (fileFd < 0)
                {
                    Native.Close(stagingFd);
                    return null;
                }
                file = new SafeFileHandle((IntPtr)fileFd, ownsHandle: true);
                Native.FileStatus opened = Native.Stat(file);
                if (!opened.IsRegular || opened.Links != 1 || opened.Uid != staging.Uid || (identity != null && !FileIdentity.Same(identity, FileIdentity.Of(opened))))
                {
                    file.Dispose();
                    Native.Close(stagingFd);
                    return null;
                }
                MakeStagePrivate(file, System.IO.Path.Combine(path, location.Directory, location.File));
                return new UnixStage(this, location.Directory, stagingFd, staging, location.File, file, opened, finalName, location.Mode ?? 0x1A4);
            }
            catch (IOException)
            {
                // Not this user's, or it cannot be made private: the download starts over in a new stage.
                file?.Dispose();
                Native.Close(stagingFd);
                return null;
            }
        }

        internal override LocalEntry Examine(string name)
        {
            CheckName(name);
            Native.FileStatus status = Native.TryStatAt(fd, name);
            return status == null ? null : new LocalEntry(status.IsRegular, FileIdentity.Of(status), status.Size);
        }

        // Opened without following a link and without waiting on a special file before its type is known, and checked
        // before anything is read: nothing about the staging directory or the stage is changed, here or later.
        internal override SalvageSource OpenSalvage(StageLocation location, string identity, string stagingIdentity)
        {
            CheckName(location.Directory);
            CheckName(location.File);
            uint user = Native.GetEuid();
            int stagingFd = Native.OpenAt(fd, location.Directory, Native.Flags.ReadOnly | Native.Flags.Directory | Native.Flags.NoFollow | Native.Flags.NonBlock | Native.Flags.CloseOnExec);
            if (stagingFd < 0)
            {
                return null;
            }
            SafeFileHandle file = null;
            try
            {
                string staged = $"The staging directory {location.Directory} in {path}";
                Native.FileStatus staging = Native.Stat(stagingFd, staged);
                if (!staging.IsDirectory || !Private(staging, user) || Native.AllowsByAccessControl(stagingFd, staged) || !FileIdentity.Consistent(stagingIdentity, staging))
                {
                    Native.Close(stagingFd);
                    return null;
                }
                int fileFd = Native.OpenAt(stagingFd, location.File, Native.Flags.ReadOnly | Native.Flags.NoFollow | Native.Flags.NonBlock | Native.Flags.CloseOnExec);
                if (fileFd < 0)
                {
                    Native.Close(stagingFd);
                    return null;
                }
                file = new SafeFileHandle((IntPtr)fileFd, ownsHandle: true);
                string what = System.IO.Path.Combine(path, location.Directory, location.File);
                Native.FileStatus opened = Native.Stat(file, $"The file {what}");
                if (!opened.IsRegular || opened.Links != 1 || !Private(opened, user) || Native.AllowsByAccessControl(file, $"The file {what}") || !FileIdentity.Consistent(identity, opened))
                {
                    file.Dispose();
                    Native.Close(stagingFd);
                    return null;
                }
                return new UnixSalvageSource(stagingFd, file, opened.Size, what);
            }
            catch (IOException)
            {
                file?.Dispose();
                Native.Close(stagingFd);
                return null;
            }
        }

        // Whether status describes an object this user owns and no one else may use: no permissions for its group or for
        // others.
        private static bool Private(Native.FileStatus status, uint user)
        {
            return CheckpointStorage.OwnerOf(status) == user && (status.Mode & 0x3F) == 0;
        }

        // Every policy: CreateNew renames without replacing where the file system can, or links then removes the stage's
        // own name where it cannot (UnixStage).
        internal override bool Supports(DownloadPublicationPolicy policy)
        {
            return true;
        }

        internal override void FinishPlaced(StageLocation location, string identity, string stagingIdentity)
        {
            CheckName(location.Directory);
            CheckName(location.File);
            int stagingFd = OpenDirectoryAt(fd, location.Directory);
            if (stagingFd < 0)
            {
                return;
            }
            try
            {
                Native.FileStatus entry = Native.TryStatAt(stagingFd, location.File);
                if (entry != null && FileIdentity.Same(identity, FileIdentity.Of(entry)))
                {
                    // The same file under its stage name too: that name alone is removed.
                    Native.UnlinkAt(stagingFd, location.File, 0);
                }
            }
            finally
            {
                Native.Close(stagingFd);
            }
            Native.FileStatus staging = Native.TryStatAt(fd, location.Directory);
            if (staging != null && staging.IsDirectory && FileIdentity.Same(stagingIdentity, FileIdentity.Of(staging)))
            {
                // Removed only while empty.
                Native.UnlinkAt(fd, location.Directory, Native.Flags.RemoveDirectory);
            }
        }

        internal override bool FlushEntries()
        {
            return Native.SyncDirectory(fd, $"The directory {path}", searchOnly);
        }

        // Makes the staging directory held by stagingFd private and returns its identity. Changing its permissions
        // succeeds only for its owner, which proves it is this process's; its access control list, inherited from the
        // destination directory or not, is removed, so its permissions alone decide who can use it.
        private Native.FileStatus MakeStagingPrivate(int stagingFd, string stagingName)
        {
            string what = $"The staging directory {stagingName} in {path}";
            if (Native.FChmod(stagingFd, 0x1C0) != 0)
            {
                throw new IOException($"{what} was replaced by one this user does not own, so nothing was written.");
            }
            Native.RemoveAccessControl(stagingFd, directory: true, what);
            Native.FileStatus status = Native.Stat(stagingFd, what);
            if (!status.IsDirectory || (status.Mode & 0xFFF) != 0x1C0)
            {
                throw new IOException($"{what} could not be made private, so nothing was written.");
            }
            return status;
        }

        // Makes a stage owner-only, without an access control list, before anything is written to it.
        private static void MakeStagePrivate(SafeFileHandle file, string what)
        {
            if (Native.FChmod(file, 0x180) != 0)
            {
                throw Native.Failure(Marshal.GetLastWin32Error(), $"The file {what} cannot be made private");
            }
            Native.RemoveAccessControl(file, $"The file {what}");
        }

        // Removes the entry name in the directory held by parentFd, only while it is still expected, an object this
        // download created and verified; anything that has taken its place is left alone. A directory is removed only
        // while it is empty. The check and the removal are two calls (see the class's remarks).
        private static void RemoveOwned(int parentFd, string name, Native.FileStatus expected, bool directory)
        {
            if (expected == null)
            {
                return;
            }
            Native.FileStatus entry = Native.TryStatAt(parentFd, name);
            if (entry == null || !entry.Same(expected))
            {
                return;
            }
            Native.UnlinkAt(parentFd, name, directory ? Native.Flags.RemoveDirectory : 0);
        }

        public override void Dispose()
        {
            if (Interlocked.Exchange(ref closed, 1) == 0)
            {
                Native.Close(fd);
            }
        }

        private static int OpenDirectoryAt(int parent, string name)
        {
            return Native.OpenAt(parent, name, Native.Flags.ReadOnly | Native.Flags.Directory | Native.Flags.NoFollow | Native.Flags.CloseOnExec);
        }

        // A directory files are downloaded into, called name in the directory parent holds, opened without following a
        // symbolic link: to read it, or only to search it where this user may not list it (searchOnly). -1 when it can be
        // opened neither way, with errno saying why it could not be read.
        private static int OpenDestinationAt(int parent, string name, out bool searchOnly, out int errno)
        {
            searchOnly = false;
            int opened = OpenDirectoryAt(parent, name);
            errno = opened < 0 ? Marshal.GetLastWin32Error() : 0;
            if (opened < 0 && errno == Native.EACCES)
            {
                opened = Native.OpenAt(parent, name, Native.Flags.SearchOnly | Native.Flags.NoFollow | Native.Flags.CloseOnExec);
                searchOnly = opened >= 0;
            }
            return opened;
        }

        private sealed class UnixStage : LocalStage
        {
            private readonly UnixDirectory directory;
            private readonly string stagingName;
            private readonly int stagingFd;
            private readonly Native.FileStatus staging;
            private readonly string name;
            private readonly SafeFileHandle file;
            private readonly Native.FileStatus identity;
            private readonly string finalName;
            private readonly int mode;
#if !NET6_0_OR_GREATER
            // Without RandomAccess, positioned writes go through a stream on the same descriptor, one at a time.
            private readonly FileStream stream;
            private readonly SemaphoreSlim writing = new SemaphoreSlim(1, 1);
#endif

            internal UnixStage(UnixDirectory directory, string stagingName, int stagingFd, Native.FileStatus staging, string name, SafeFileHandle file, Native.FileStatus identity, string finalName, int mode)
            {
                this.directory = directory;
                this.stagingName = stagingName;
                this.stagingFd = stagingFd;
                this.staging = staging;
                this.name = name;
                this.file = file;
                this.identity = identity;
                this.finalName = finalName;
                this.mode = mode;
#if !NET6_0_OR_GREATER
                stream = new FileStream(file, FileAccess.ReadWrite, 1);
#endif
            }

            // A new stage in the private staging directory held by stagingFd, called stagingName in directory.
            internal static UnixStage Create(UnixDirectory directory, string stagingName, int stagingFd, Native.FileStatus staging, string finalName, int mode)
            {
                byte[] template = System.Text.Encoding.UTF8.GetBytes(System.IO.Path.Combine(directory.path, stagingName, StageTemplate) + "\0");
                int fileFd = Native.MkStemp(template);
                if (fileFd < 0)
                {
                    throw Native.Failure(Marshal.GetLastWin32Error(), $"A stage cannot be created in {directory.path}");
                }
                string created = System.Text.Encoding.UTF8.GetString(template, 0, template.Length - 1);
                string name = System.IO.Path.GetFileName(created);
                var file = new SafeFileHandle((IntPtr)fileFd, ownsHandle: true);
                try
                {
                    // mkstemp went by path: the file counts only if it is the very file at that name in the staging
                    // directory this process holds, which nobody else can add an entry to. A file it created anywhere else
                    // is left there.
                    Native.FileStatus opened = Native.Stat(file);
                    Native.FileStatus entry = Native.TryStatAt(stagingFd, name);
                    if (entry == null || !entry.Same(opened) || !opened.IsRegular || opened.Links != 1)
                    {
                        throw new IOException($"The staging directory {stagingName} in {directory.path} was moved while the stage was created, so nothing was written.");
                    }
                    MakeStagePrivate(file, System.IO.Path.Combine(directory.path, stagingName, name));
                    return new UnixStage(directory, stagingName, stagingFd, staging, name, file, opened, finalName, mode);
                }
                catch
                {
                    file.Dispose();
                    throw;
                }
            }

            internal override StageLocation Location => new StageLocation(stagingName, name, mode);

            internal override string Identity => FileIdentity.Of(identity);

            internal override string StagingIdentity => FileIdentity.Of(staging);

#if NET6_0_OR_GREATER
            internal override long Length => RandomAccess.GetLength(file);

            internal override async Task WriteAsync(long offset, byte[] buffer, int count, CancellationToken cancellationToken)
            {
                await RandomAccess.WriteAsync(file, new ReadOnlyMemory<byte>(buffer, 0, count), offset, cancellationToken);
            }

            internal override void SetLength(long length)
            {
                RandomAccess.SetLength(file, length);
            }
#else
            internal override long Length => stream.Length;

            internal override async Task WriteAsync(long offset, byte[] buffer, int count, CancellationToken cancellationToken)
            {
                await writing.WaitAsync(cancellationToken);
                try
                {
                    stream.Position = offset;
                    await stream.WriteAsync(buffer, 0, count, cancellationToken);
                }
                finally
                {
                    writing.Release();
                }
            }

            internal override void SetLength(long length)
            {
                stream.SetLength(length);
            }
#endif

            internal override void Flush()
            {
                Native.SyncFile(file, $"The file being downloaded to {directory.path}");
            }

            // Through the descriptors this stage and its directory hold, not by path: the staging directory, which holds
            // the file's entry, then the download's directory, which holds the staging directory's. A file system that
            // cannot flush a directory, or a directory opened only to search it, fails this.
            internal override void FlushNames()
            {
                string staged = $"The staging directory {stagingName} in {directory.path}";
                if (!Native.SyncDirectory(stagingFd, staged) || !directory.FlushEntries())
                {
                    throw new IOException($"{staged}, or its entry in {directory.path}, cannot be flushed to storage.");
                }
            }

            // Puts the stage in place as policy says, once it is flushed to storage: ReplaceExisting gives it the access
            // of a file it replaces, or a new file's (GiveAccess), and renames it over the name; ReplaceRegularFile gives it
            // a replaced regular file's access as PowerShell does (TakeOverAccess) and renames it over the name, refusing
            // anything else there; CreateNew gives it a new file's permissions and puts it in place only if nothing is at
            // the name (MoveWithoutReplacing). Just before it is put in place, the stage's name must lead to the stage, and
            // the directory's path to the directory the download opened (CheckBeforePlacing); just after, the file's name
            // must lead to the stage, and the path to its directory (CheckPlaced), as the class's remarks describe. The
            // destination directory is then flushed.
            protected override void PublishCore(DownloadPublicationPolicy policy)
            {
                Flush();
                string destination = System.IO.Path.Combine(directory.path, finalName);
                if (policy == DownloadPublicationPolicy.CreateNew)
                {
                    SetMode(mode, destination);
                    CheckBeforePlacing(destination);
                    MoveWithoutReplacing(destination);
                }
                else
                {
                    if (policy == DownloadPublicationPolicy.ReplaceRegularFile)
                    {
                        TakeOverAccess(destination);
                    }
                    else
                    {
                        GiveAccess(destination);
                    }
                    CheckBeforePlacing(destination);
                    if (Native.RenameAt(stagingFd, name, directory.fd, finalName) != 0)
                    {
                        int errno = Marshal.GetLastWin32Error();
                        IOException failure = Native.Failure(errno, $"The file downloaded to {destination} cannot be put in place");
                        if (errno == Native.EISDIR)
                        {
                            throw new DownloadPublicationException(DownloadPublicationProblem.DestinationNotRegularFile, destination, $"{destination} is a directory, which a file does not replace, so nothing was replaced.", failure);
                        }
                        throw failure;
                    }
                    MarkPlaced();
                }
                CheckPlaced(destination);
                bool atPath = directory.StillAtPath();
                // The new entry reaches storage only with its directory. Where the file system cannot flush a directory,
                // or cannot through a directory this user may not list, it gets there when the system writes it; a flush
                // that fails leaves the file in place, but perhaps not through a system crash, which is what the failure
                // says.
                try
                {
                    directory.FlushEntries();
                }
                catch (IOException e)
                {
                    if (!atPath)
                    {
                        throw Moved(destination, placed: true, e);
                    }
                    throw new DownloadPublicationException(DownloadPublicationProblem.PlacedNotDurable, destination, $"The file downloaded to {destination} was put in place, but may not survive a system crash: {e.Message}", e);
                }
                if (!atPath)
                {
                    throw Moved(destination, placed: true, null);
                }
            }

            // Refuses, putting nothing in place, unless the entry at the stage's name in its staging directory is the
            // stage this download holds open and wrote, and the directory's path still leads to the directory the
            // download opened. Another program running as this user may have renamed the stage away and put another file
            // at its name: the stage is then lost to this download, its bytes are not kept, and what took its name is left
            // where it is. A directory renamed or replaced keeps the stage, which may be kept for a later run.
            private void CheckBeforePlacing(string destination)
            {
                Native.FileStatus entry = Native.TryStatAt(stagingFd, name);
                if (entry == null || !entry.Same(identity))
                {
                    MarkLost();
                    throw new DownloadPublicationException(DownloadPublicationProblem.StageReplaced, destination, $"The file being downloaded to {destination} was replaced, where it waited in its private staging folder, by another program running as this user, so it was not put in place: {destination} is as it was, and what took the stage's place is left where it is. The downloaded bytes are not kept.");
                }
                if (!directory.StillAtPath())
                {
                    throw Moved(destination, placed: false, null);
                }
                Native.BeforePlacingFaults.Value?.Invoke();
            }

            // Once the stage has been renamed or linked into place: fails, saying so, unless the entry at the file's name is
            // the stage. What is there then is another program's, put at the stage's name as the stage was put in place,
            // and is left as it is.
            private void CheckPlaced(string destination)
            {
                Native.FileStatus entry = Native.TryStatAt(directory.fd, finalName);
                if (entry == null || !entry.Same(identity))
                {
                    MarkLost();
                    throw new DownloadPublicationException(DownloadPublicationProblem.StageReplaced, destination, $"What was put in place at {destination} is not the file downloaded: another program running as this user replaced the file being downloaded, where it waited in its private staging folder, as it was being put in place. What that program put there took the place of any file at {destination}, and is left as it is. The downloaded bytes are not kept.");
                }
            }

            private DownloadPublicationException Moved(string destination, bool placed, IOException flush)
            {
                if (!placed)
                {
                    return new DownloadPublicationException(DownloadPublicationProblem.DestinationMoved, destination, $"The folder {directory.path} was renamed, or replaced by another, while the file was downloaded, so the file was not put in place: in the folder the download opened it would not be at {destination}. Nothing was put in place at {destination}.");
                }
                string durable = flush == null ? "" : $" It may not survive a system crash either: {flush.Message}";
                return new DownloadPublicationException(DownloadPublicationProblem.DestinationMoved, destination, $"The file downloaded to {destination} was put in place in the folder the download opened, but that folder was renamed, or replaced by another, as the file was put in place, so the file is not at {destination}: it is in that folder, wherever it now is, and is not removed.{durable}", flush, placed: true);
            }

            // Puts the stage at finalName only if nothing is there, in one step where the file system allows it (renameat2
            // with RENAME_NOREPLACE on Linux, renameatx_np with RENAME_EXCL on macOS); otherwise by linking finalName to
            // the stage, which fails too when anything is there, then removing the stage's own name while it is still
            // this file. Between those two steps the file has both names; a later run that finds both finishes the
            // second step (LocalDirectory.FinishPlaced). Nothing is ever replaced: any other failure fails the download.
            private void MoveWithoutReplacing(string destination)
            {
                int result = Native.RenameAtNoReplace(stagingFd, name, directory.fd, finalName, out bool supported, out int errno);
                if (supported)
                {
                    if (result != 0)
                    {
                        throw errno == Native.EEXIST ? Exists(destination) : Native.Failure(errno, $"The file downloaded to {destination} cannot be put in place");
                    }
                    MarkPlaced();
                    return;
                }
                if (Native.LinkAt(stagingFd, name, directory.fd, finalName, 0) != 0)
                {
                    errno = Marshal.GetLastWin32Error();
                    IOException failure = Native.Failure(errno, $"The file downloaded to {destination} cannot be put in place");
                    if (errno == Native.EEXIST)
                    {
                        throw Exists(destination);
                    }
                    if (errno == Native.EPERM || errno == Native.Flags.NotSupported || errno == Native.Flags.OperationNotSupported)
                    {
                        throw new DownloadPublicationException(DownloadPublicationProblem.NotSupported, destination, $"The file system of {directory.path} can neither rename a file without replacing what is at its new name nor link one, so {destination} was not put in place: {failure.Message}", failure);
                    }
                    throw failure;
                }
                MarkPlaced();
                RemoveOwned(stagingFd, name, identity, directory: false);
            }

            private static DownloadPublicationException Exists(string destination)
            {
                return new DownloadPublicationException(DownloadPublicationProblem.DestinationExists, destination, $"{destination} exists, and is left as it is: nothing was replaced.");
            }

            // Gives the stage the access of the file it replaces, or a new file's. A file being replaced keeps exactly who
            // may use it: its access control list, owner, group and permissions all go to the stage, the permissions
            // last, so that they decide whatever the list set before, and all are checked after. What cannot be read or
            // given fails the download before anything is replaced, leaving the file as it was: an owner or group this
            // user cannot give (another user, or a group this user is not in) cannot be kept, and permissions under
            // another owner or group would let other people use the file, so nothing narrower or wider is published in
            // its place. setuid, setgid and sticky, which are not access, are not given. Only metadata is read, never
            // contents, through a descriptor of the file itself (Native.CopyAccessControl), so a file this user may
            // write but not read is replaced too.
            private void GiveAccess(string destination)
            {
                Native.FileStatus existing = Native.TryStatAt(directory.fd, finalName);
                if (existing == null || !existing.IsRegular)
                {
                    SetMode(mode, destination);
                    return;
                }
                existing = Native.Observed(existing);
                string what = $"The file {destination}";
                try
                {
                    Native.CopyAccessControl(directory.fd, finalName, existing, file, what);
                    if (existing.Uid != identity.Uid && Native.FChown(file, existing.Uid, uint.MaxValue) != 0)
                    {
                        throw Native.Failure(Marshal.GetLastWin32Error(), $"{what} is another user's, who cannot be made the owner of the file replacing it, so it was not replaced");
                    }
                    if (existing.Gid != identity.Gid && Native.FChown(file, uint.MaxValue, existing.Gid) != 0)
                    {
                        throw Native.Failure(Marshal.GetLastWin32Error(), $"{what} belongs to a group the file replacing it cannot be given, so it was not replaced");
                    }
                    int given = existing.Mode & 0x1FF;
                    SetMode(given, destination);
                    Native.FileStatus result = Native.Stat(file, what);
                    if ((result.Mode & 0x1FF) != given || result.Uid != existing.Uid || result.Gid != existing.Gid)
                    {
                        throw new IOException($"{what} could not keep its access when replaced, so it was not replaced.");
                    }
                }
                catch (IOException e) when (!(e is DownloadPublicationException))
                {
                    throw new DownloadPublicationException(DownloadPublicationProblem.AccessNotKept, destination, e.Message, e);
                }
            }

            // Gives the stage the access of the regular file it replaces as PowerShell's -Force does, or a new file's: the
            // file stays this user's, and takes the replaced file's group, then its access control list, then its
            // permissions: all of them when the file was this user's; otherwise the owner permissions a new file gets,
            // with the replaced file's group and other permissions, so no one is given access the replaced file did not
            // give, but its user. A directory, link or other special file at the name is not replaced. What cannot be read
            // or given fails the download before anything is replaced.
            private void TakeOverAccess(string destination)
            {
                Native.FileStatus existing = Native.TryStatAt(directory.fd, finalName);
                if (existing == null)
                {
                    SetMode(mode, destination);
                    return;
                }
                if (!existing.IsRegular)
                {
                    throw new DownloadPublicationException(DownloadPublicationProblem.DestinationNotRegularFile, destination, $"{destination} is not a regular file, so it was not replaced.");
                }
                existing = Native.Observed(existing);
                string what = $"The file {destination}";
                try
                {
                    if (existing.Gid != identity.Gid && Native.FChown(file, uint.MaxValue, existing.Gid) != 0)
                    {
                        throw Native.Failure(Marshal.GetLastWin32Error(), $"{what} belongs to a group the file replacing it cannot be given, so it was not replaced");
                    }
                    Native.CopyAccessControl(directory.fd, finalName, existing, file, what);
                    int given = existing.Uid == identity.Uid ? existing.Mode & 0x1FF : (mode & 0x1C0) | (existing.Mode & 0x3F);
                    SetMode(given, destination);
                    Native.FileStatus result = Native.Stat(file, what);
                    if ((result.Mode & 0x1FF) != given || result.Uid != identity.Uid || result.Gid != existing.Gid)
                    {
                        throw new IOException($"{what} could not keep its access when replaced, so it was not replaced.");
                    }
                }
                catch (IOException e) when (!(e is DownloadPublicationException))
                {
                    throw new DownloadPublicationException(DownloadPublicationProblem.AccessNotKept, destination, e.Message, e);
                }
            }

            // Owner-only again, without an access control list, still this user's: then it may be kept. Publishing may
            // have given it a new file's permissions, or the access of the file it was to replace, before it was refused.
            internal override bool RestorePrivacy()
            {
                string what = $"The file being downloaded to {directory.path}";
                try
                {
                    if (Native.FChmod(file, 0x180) != 0)
                    {
                        return false;
                    }
                    Native.RemoveAccessControl(file, what);
                    Native.FileStatus now = Native.Stat(file, what);
                    return (now.Mode & 0xFFF) == 0x180 && now.Uid == identity.Uid;
                }
                catch (IOException)
                {
                    return false;
                }
            }

            private void SetMode(int permissions, string destination)
            {
                if (Native.FChmod(file, permissions) != 0)
                {
                    throw Native.Failure(Marshal.GetLastWin32Error(), $"The permissions of the file downloaded to {destination} cannot be set");
                }
            }

            protected override void Close(bool delete, bool published)
            {
#if !NET6_0_OR_GREATER
                stream.Dispose();
                writing.Dispose();
#endif
                file.Dispose();
                if (delete)
                {
                    RemoveOwned(stagingFd, name, identity, directory: false);
                }
                Native.Close(stagingFd);
                if (delete || published)
                {
                    RemoveOwned(directory.fd, stagingName, staging, directory: true);
                }
            }
        }
    }

    // The C library calls UnixDirectory makes, with the flag and structure layouts of the platform it runs on. On macOS
    // none of them is variadic with an argument that matters: open and openat are never asked to create, files are
    // created by mkstemp, and fcntl is given only commands that take no argument, so the calling conventions of variadic
    // functions (which differ on Apple silicon) never come into it; syscall, the one variadic call made with arguments,
    // is made only on Linux (StatAt). File identities come from fstat and fstatat on macOS (the 64-bit-inode layout) and
    // from statx on Linux, whose layout is the same on every architecture, or from newfstatat or fstatat64 where statx
    // fails. Access control lists are the extended ACLs of macOS (acl_*_np) and the POSIX ACLs of Linux, kept as the
    // system.posix_acl_* attributes.
    internal static class Native
    {
        internal const int EPERM = 1;
        internal const int ENOENT = 2;
        internal const int EBADF = 9;
        internal const int EACCES = 13;
        internal const int EEXIST = 17;
        internal const int ENOTDIR = 20;
        internal const int EISDIR = 21;
        internal const int EINVAL = 22;
        internal const int ENOTTY = 25;

        private const int AclTypeExtended = 0x100;
        private const int FullFsync = 51;
        private const string PosixAclAccess = "system.posix_acl_access";
        private const string PosixAclDefault = "system.posix_acl_default";

        private const string Library = "libc";

        private const int ENOSYS = 38;

        // The names the C library loads by on Linux, where "libc" alone does not load it: glibc's, then musl's.
        private static readonly string[] LinuxLibraries = { "libc.so.6", "libc.musl-x86_64.so.1", "libc.musl-aarch64.so.1", "libc.musl-armv7.so.1", "libc.musl-armhf.so.1" };

        // Where descriptors can be used: on macOS, and on Linux in every build once "libc" resolves to the C library,
        // glibc's or musl's (through NativeLibrary, which the netstandard2.0 build finds by reflection on .NET Core 3.0 and
        // later, or as Mono maps it itself). Null only where the C library cannot be called at all, as with .NET Core 2.x on
        // Linux, on a Linux processor PlatformFlags has no layouts for, and on other systems: directories and stages are
        // then used by path (PathDirectory), and checkpoints are refused (CheckpointStorage). Whether each call works is
        // not probed here: one that fails fails the operation it is part of (StatAt says how status calls fall back
        // first), and never makes the SDK use paths or trust a checkpoint instead.
        static Native()
        {
            PlatformFlags flags = PlatformFlags.ForCurrentPlatform();
            if (flags != null && !flags.Darwin)
            {
#if NETCOREAPP3_0_OR_GREATER
                try
                {
                    NativeLibrary.SetDllImportResolver(typeof(Native).Assembly, (name, assembly, search) =>
                        name == Library ? LoadLinuxLibrary(library => NativeLibrary.TryLoad(library, out IntPtr handle) ? handle : IntPtr.Zero) : IntPtr.Zero);
                }
                catch (InvalidOperationException)
                {
                    // A resolver is set already; the default resolution applies.
                }
#else
                TryInstallLibcResolverByReflection();
#endif
            }
            Flags = flags != null && CanLoad() ? flags : null;
        }

        // The C library, by the first of its Linux names that loads, or IntPtr.Zero for the default resolution.
        private static IntPtr LoadLinuxLibrary(Func<string, IntPtr> tryLoad)
        {
            foreach (string library in LinuxLibraries)
            {
                IntPtr handle = tryLoad(library);
                if (handle != IntPtr.Zero)
                {
                    return handle;
                }
            }
            return IntPtr.Zero;
        }

        internal static PlatformFlags Flags { get; }

        // Resolves "libc" to the C library on Linux through NativeLibrary, found by reflection: for the netstandard2.0
        // build, which cannot name it, on a runtime that has it (.NET Core 3.0 and later). False where the runtime has no
        // NativeLibrary, true once a resolver is set, by this or before it.
        internal static bool TryInstallLibcResolverByReflection()
        {
            Type nativeLibrary = Type.GetType("System.Runtime.InteropServices.NativeLibrary", throwOnError: false);
            Type resolverType = Type.GetType("System.Runtime.InteropServices.DllImportResolver", throwOnError: false);
            if (nativeLibrary == null || resolverType == null)
            {
                return false;
            }
            System.Reflection.MethodInfo set = nativeLibrary.GetMethod("SetDllImportResolver", new[] { typeof(System.Reflection.Assembly), resolverType });
            libraryTryLoad = nativeLibrary.GetMethod("TryLoad", new[] { typeof(string), typeof(IntPtr).MakeByRefType() });
            System.Reflection.MethodInfo resolve = typeof(Native).GetMethod(nameof(ResolveLibrary), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if (set == null || libraryTryLoad == null)
            {
                return false;
            }
            try
            {
                set.Invoke(null, new object[] { typeof(Native).Assembly, Delegate.CreateDelegate(resolverType, resolve) });
            }
            catch (System.Reflection.TargetInvocationException e) when (e.InnerException is InvalidOperationException)
            {
                // A resolver is set already; the default resolution applies.
            }
            return true;
        }

        private static System.Reflection.MethodInfo libraryTryLoad;

        // The resolver TryInstallLibcResolverByReflection sets: the C library for "libc", by the first of its Linux names
        // that loads; the default resolution for anything else.
        private static IntPtr ResolveLibrary(string name, System.Reflection.Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (name != Library || !RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return IntPtr.Zero;
            }
            return LoadLinuxLibrary(library =>
            {
                object[] arguments = { library, IntPtr.Zero };
                return (bool)libraryTryLoad.Invoke(null, arguments) ? (IntPtr)arguments[1] : IntPtr.Zero;
            });
        }

        // Whether the C library can be called at all: a call that cannot fail otherwise loads it.
        private static bool CanLoad()
        {
            try
            {
                GetEuid();
                return true;
            }
            catch (Exception e) when (e is DllNotFoundException || e is EntryPointNotFoundException || e is BadImageFormatException)
            {
                return false;
            }
        }

        // The flushes SyncFile and SyncDirectory make.
        internal enum FlushCall
        {
            FullSyncFile,
            SyncFile,
            FullSyncDirectory,
            SyncDirectory,
        }

        // For the SDK's own tests: the errno a flush of the given kind is to fail with instead of calling the system (0
        // to call it), for the flushes made within the flow of a test that sets it. Nothing else sets it, so everywhere
        // else every flush is the system's.
        internal static readonly AsyncLocal<Func<FlushCall, int>> FlushFaults = new AsyncLocal<Func<FlushCall, int>>();

        // For the SDK's own tests: the owner and group a file being replaced is to be taken to have, in place of those
        // it has, for the downloads within the flow of a test that sets it, so that giving them to the file replacing it
        // is refused by the system itself, as for a file of another user's. Nothing else sets it.
        internal static readonly AsyncLocal<Func<FileStatus, (uint Uid, uint Gid)>> ReplacedOwnerFaults = new AsyncLocal<Func<FileStatus, (uint Uid, uint Gid)>>();

        // For the SDK's own tests: what is to happen just after a stage is checked and before it is renamed or linked into
        // place, for the downloads within the flow of a test that sets it, so that a test can change names at the moment
        // another program could. Nothing else sets it.
        internal static readonly AsyncLocal<Action> BeforePlacingFaults = new AsyncLocal<Action>();

        // status, with the owner and group a test gave it (ReplacedOwnerFaults), or as it is.
        internal static FileStatus Observed(FileStatus status)
        {
            Func<FileStatus, (uint Uid, uint Gid)> fault = ReplacedOwnerFaults.Value;
            if (fault == null)
            {
                return status;
            }
            (uint uid, uint gid) = fault(status);
            return new FileStatus { Device = status.Device, Inode = status.Inode, Mode = status.Mode, Uid = uid, Gid = gid, Links = status.Links, Size = status.Size };
        }

        internal static IOException Failure(int errno, string what)
        {
            string why = errno == Flags?.Loop ? "a symbolic link is in the way, which transfers do not follow"
                : errno == ENOTDIR ? "something in the way is not a directory"
                : errno == ENOENT ? "it does not exist"
                : errno == EEXIST ? "it exists already"
                : errno == 13 || errno == 1 ? "permission is denied"
                : $"error {errno}";
            return new IOException($"{what}: {why}.");
        }

        [DllImport(Library, EntryPoint = "open", SetLastError = true)]
        internal static extern int Open(string path, int flags);

        [DllImport(Library, EntryPoint = "openat", SetLastError = true)]
        internal static extern int OpenAt(int directory, string path, int flags);

        [DllImport(Library, EntryPoint = "mkdirat", SetLastError = true)]
        internal static extern int MkdirAt(int directory, string path, int mode);

        [DllImport(Library, EntryPoint = "mkstemp", SetLastError = true)]
        internal static extern int MkStemp(byte[] template);

        [DllImport(Library, EntryPoint = "renameat", SetLastError = true)]
        internal static extern int RenameAt(int fromDirectory, string from, int toDirectory, string to);

        [DllImport(Library, EntryPoint = "linkat", SetLastError = true)]
        internal static extern int LinkAt(int fromDirectory, string from, int toDirectory, string to, int flags);

        [DllImport(Library, EntryPoint = "renameat2", SetLastError = true)]
        private static extern int RenameAt2(int fromDirectory, string from, int toDirectory, string to, uint flags);

        [DllImport(Library, EntryPoint = "renameatx_np", SetLastError = true)]
        private static extern int RenameAtXNp(int fromDirectory, string from, int toDirectory, string to, uint flags);

        // For the SDK's own tests: whether, within the flow of a test that sets it, renaming without replacing is taken to
        // be unsupported by the file system, so that the link fallback is used. Nothing else sets it.
        internal static readonly AsyncLocal<bool> NoReplaceRenameUnsupportedFaults = new AsyncLocal<bool>();

        // Renames from to to, failing with EEXIST rather than replacing anything there: renameatx_np with RENAME_EXCL on
        // macOS, renameat2 with RENAME_NOREPLACE on Linux. supported is false when the C library, kernel or file system
        // cannot (ENOTSUP on macOS; EINVAL or ENOSYS on Linux, or no renameat2 in the C library, as with musl): then
        // nothing was renamed. errno is the call's error.
        internal static int RenameAtNoReplace(int fromDirectory, string from, int toDirectory, string to, out bool supported, out int errno)
        {
            if (NoReplaceRenameUnsupportedFaults.Value)
            {
                supported = false;
                errno = 0;
                return -1;
            }
            try
            {
                int result = Flags.Darwin ? RenameAtXNp(fromDirectory, from, toDirectory, to, 0x4) : RenameAt2(fromDirectory, from, toDirectory, to, 1);
                errno = result == 0 ? 0 : Marshal.GetLastWin32Error();
                supported = result == 0 || (Flags.Darwin ? errno != Flags.NotSupported : errno != EINVAL && errno != ENOSYS);
                return result;
            }
            catch (EntryPointNotFoundException)
            {
                supported = false;
                errno = 0;
                return -1;
            }
        }

        [DllImport(Library, EntryPoint = "unlinkat", SetLastError = true)]
        internal static extern int UnlinkAt(int directory, string path, int flags);

        [DllImport(Library, EntryPoint = "fchmod", SetLastError = true)]
        internal static extern int FChmod(int fd, int mode);

        [DllImport(Library, EntryPoint = "fchmod", SetLastError = true)]
        internal static extern int FChmod(SafeFileHandle fd, int mode);

        [DllImport(Library, EntryPoint = "fsync", SetLastError = true)]
        internal static extern int FSync(SafeFileHandle fd);

        [DllImport(Library, EntryPoint = "close", SetLastError = true)]
        internal static extern int Close(int fd);

        [DllImport(Library, EntryPoint = "fchown", SetLastError = true)]
        internal static extern int FChown(SafeFileHandle fd, uint owner, uint group);

        [DllImport(Library, EntryPoint = "fsync", SetLastError = true)]
        private static extern int FSync(int fd);

        [DllImport(Library, EntryPoint = "fcntl", SetLastError = true)]
        private static extern int FControl(int fd, int command);

        [DllImport(Library, EntryPoint = "fcntl", SetLastError = true)]
        private static extern int FControl(SafeFileHandle fd, int command);

        [DllImport(Library, EntryPoint = "fstat", SetLastError = true)]
        private static extern int FStatDarwin(int fd, byte[] status);

        [DllImport(Library, EntryPoint = "fstat$INODE64", SetLastError = true)]
        private static extern int FStatDarwinX64(int fd, byte[] status);

        [DllImport(Library, EntryPoint = "fstatat", SetLastError = true)]
        private static extern int FStatAtDarwin(int directory, string path, byte[] status, int flags);

        [DllImport(Library, EntryPoint = "fstatat$INODE64", SetLastError = true)]
        private static extern int FStatAtDarwinX64(int directory, string path, byte[] status, int flags);

        [DllImport(Library, EntryPoint = "statx", SetLastError = true)]
        private static extern int StatX(int directory, string path, int flags, uint mask, byte[] status);

        // syscall(2), for statx, newfstatat and fstatat64 where the C library cannot give them (StatAt). It is variadic,
        // but Linux's calling conventions (x86-64 System V, i386 System V, AAPCS64 and AAPCS on Linux) pass variadic
        // integer and pointer arguments as fixed ones, and it is never called on macOS; every argument is passed at the
        // width of a pointer.
        [DllImport(Library, EntryPoint = "syscall", SetLastError = true)]
        private static extern IntPtr SysCallStatX(IntPtr number, IntPtr directory, string path, IntPtr flags, IntPtr mask, byte[] status);

        [DllImport(Library, EntryPoint = "syscall", SetLastError = true)]
        private static extern IntPtr SysCallFStatAt(IntPtr number, IntPtr directory, string path, byte[] status, IntPtr flags);

        [DllImport(Library, EntryPoint = "acl_init", SetLastError = true)]
        private static extern IntPtr AclInit(int count);

        [DllImport(Library, EntryPoint = "acl_free", SetLastError = true)]
        private static extern int AclFree(IntPtr acl);

        [DllImport(Library, EntryPoint = "acl_set_fd_np", SetLastError = true)]
        private static extern int AclSetFd(int fd, IntPtr acl, int type);

        [DllImport(Library, EntryPoint = "acl_get_fd_np", SetLastError = true)]
        private static extern IntPtr AclGetFd(int fd, int type);

        [DllImport(Library, EntryPoint = "acl_get_entry", SetLastError = true)]
        private static extern int AclGetEntry(IntPtr acl, int entryId, out IntPtr entry);

        [DllImport(Library, EntryPoint = "fgetxattr", SetLastError = true)]
        private static extern IntPtr FGetXattrLinux(int fd, string name, byte[] value, UIntPtr size);

        [DllImport(Library, EntryPoint = "getxattr", SetLastError = true)]
        private static extern IntPtr GetXattrLinux(string path, string name, byte[] value, UIntPtr size);

        [DllImport(Library, EntryPoint = "fsetxattr", SetLastError = true)]
        private static extern int FSetXattrLinux(int fd, string name, byte[] value, UIntPtr size, int flags);

        [DllImport(Library, EntryPoint = "fremovexattr", SetLastError = true)]
        private static extern int FRemoveXattrLinux(int fd, string name);

        [DllImport(Library, EntryPoint = "acl_get_tag_type", SetLastError = true)]
        private static extern int AclGetTagType(IntPtr entry, out int tag);

        [DllImport(Library, EntryPoint = "realpath", SetLastError = true)]
        private static extern IntPtr RealPath(string path, byte[] resolved);

        [DllImport(Library, EntryPoint = "geteuid")]
        internal static extern uint GetEuid();

        // path with every link and "." or ".." in it resolved, as the system resolves it now (realpath).
        internal static string RealPath(string path)
        {
            // PATH_MAX is 1024 on macOS and 4096 on Linux.
            byte[] resolved = new byte[4097];
            if (RealPath(path, resolved) == IntPtr.Zero)
            {
                int errno = Marshal.GetLastWin32Error();
                if (errno == ENOENT)
                {
                    throw new DirectoryNotFoundException($"The directory {path} does not exist.");
                }
                throw Failure(errno, $"The directory {path} cannot be found");
            }
            return System.Text.Encoding.UTF8.GetString(resolved, 0, Array.IndexOf(resolved, (byte)0));
        }

        // Whether the object fd is a descriptor of has an access control list entry that allows anyone anything (macOS),
        // which its permissions then do not show; entries that only deny access take nothing away from what they show.
        // False on Linux, where whatever a POSIX access control list allows beyond the owner is within the group
        // permissions (the list's mask).
        internal static bool AllowsByAccessControl(int fd, string what)
        {
            if (!Flags.Darwin)
            {
                return false;
            }
            IntPtr acl = AclGetFd(fd, AclTypeExtended);
            if (acl == IntPtr.Zero)
            {
                int errno = Marshal.GetLastWin32Error();
                if (errno == ENOENT || Unsupported(errno))
                {
                    return false;
                }
                throw Failure(errno, $"The access control list of {what} cannot be read");
            }
            try
            {
                for (int id = 0; AclGetEntry(acl, id, out IntPtr entry) == 0; id = -1)
                {
                    // ACL_EXTENDED_DENY is 2; ACL_EXTENDED_ALLOW, 1.
                    if (AclGetTagType(entry, out int tag) != 0 || tag != 2)
                    {
                        return true;
                    }
                }
                return false;
            }
            finally
            {
                AclFree(acl);
            }
        }

        internal static bool AllowsByAccessControl(SafeFileHandle file, string what)
        {
            bool added = false;
            try
            {
                file.DangerousAddRef(ref added);
                return AllowsByAccessControl((int)file.DangerousGetHandle(), what);
            }
            finally
            {
                if (added)
                {
                    file.DangerousRelease();
                }
            }
        }

        // Who owns an object and what it is: its device and inode, which name it wherever its entries are, its type and
        // permissions, owner and group, links and size.
        internal sealed class FileStatus
        {
            internal ulong Device { get; set; }

            internal ulong Inode { get; set; }

            internal int Mode { get; set; }

            internal uint Uid { get; set; }

            internal uint Gid { get; set; }

            internal long Links { get; set; }

            internal long Size { get; set; }

            // When the object was created, as seconds and nanoseconds, where Linux's file system records it and statx
            // gives it (stx_btime); null otherwise.
            internal string Birth { get; set; }

            internal bool IsRegular => (Mode & 0xF000) == 0x8000;

            internal bool IsDirectory => (Mode & 0xF000) == 0x4000;

            // Whether other is the same object.
            internal bool Same(FileStatus other)
            {
                return other != null && other.Device == Device && other.Inode == Inode;
            }
        }

        internal static FileStatus Stat(int fd, string what)
        {
            if (!Flags.Darwin)
            {
                return StatAt(fd, "", Flags.EmptyPath, out int errno) ?? throw Failure(errno, $"{what} cannot be examined");
            }
            byte[] status = new byte[256];
            int result = RuntimeInformation.ProcessArchitecture == Architecture.X64 ? FStatDarwinX64(fd, status) : FStatDarwin(fd, status);
            if (result != 0)
            {
                throw Failure(Marshal.GetLastWin32Error(), $"{what} cannot be examined");
            }
            return FromDarwin(status);
        }

        internal static FileStatus Stat(SafeFileHandle file, string what = "A file being downloaded")
        {
            bool added = false;
            try
            {
                file.DangerousAddRef(ref added);
                return Stat((int)file.DangerousGetHandle(), what);
            }
            finally
            {
                if (added)
                {
                    file.DangerousRelease();
                }
            }
        }

        // The object at name in the directory held by directory, without following a symbolic link, or null when
        // nothing is there.
        internal static FileStatus TryStatAt(int directory, string name)
        {
            int errno;
            FileStatus found;
            if (Flags.Darwin)
            {
                byte[] status = new byte[256];
                int result = RuntimeInformation.ProcessArchitecture == Architecture.X64 ? FStatAtDarwinX64(directory, name, status, Flags.SymlinkNoFollow) : FStatAtDarwin(directory, name, status, Flags.SymlinkNoFollow);
                errno = result == 0 ? 0 : Marshal.GetLastWin32Error();
                found = result == 0 ? FromDarwin(status) : null;
            }
            else
            {
                found = StatAt(directory, name, Flags.SymlinkNoFollow, out errno);
            }
            if (found == null)
            {
                if (errno == ENOENT)
                {
                    return null;
                }
                throw Failure(errno, $"The entry {name} cannot be examined");
            }
            return found;
        }

        // The object path leads to now, following the symbolic links in it as the system does, or null, with errno, when it
        // leads to nothing this user can reach.
        internal static FileStatus TryStatPath(string path, out int errno)
        {
            if (Flags.Darwin)
            {
                byte[] status = new byte[256];
                int result = RuntimeInformation.ProcessArchitecture == Architecture.X64 ? FStatAtDarwinX64(Flags.CurrentDirectory, path, status, 0) : FStatAtDarwin(Flags.CurrentDirectory, path, status, 0);
                errno = result == 0 ? 0 : Marshal.GetLastWin32Error();
                return result == 0 ? FromDarwin(status) : null;
            }
            return StatAt(Flags.CurrentDirectory, path, 0, out errno);
        }

        // How StatAt reads status on Linux: 0, statx through the C library; 1, statx as a system call, where the C library
        // has no statx (glibc before 2.28, musl before 1.2.5); 2, the kernel's older status call as a system call
        // (newfstatat on x64 and Arm64, fstatat64 on x86 and 32-bit Arm), where statx fails with ENOSYS or EPERM. Neither
        // errno says why: ENOSYS is also what a kernel without statx gives, and EPERM can come from a security module or
        // file system as well as from a sandbox that denies statx alone. So the older call is tried for that status, and
        // kept for later ones only once it has worked where statx did not; a failure of both is the older call's, and
        // statx is tried again next time. Every other failure is the call's own.
        private static int statMethod;

        // The status of path in the directory held by directory, with flags (AT_EMPTY_PATH or AT_SYMLINK_NOFOLLOW), on
        // Linux; or null, with errno set, when it cannot be read. The three methods give the same device, inode, type,
        // permissions, owner, group, links and size; statx also gives the time of creation where the file system records
        // it (STATX_BASIC_STATS | STATX_BTIME), which the older call cannot.
        private static FileStatus StatAt(int directory, string path, int flags, out int errno)
        {
            byte[] status = new byte[256];
            int method = Volatile.Read(ref statMethod);
            int replaced = -1;
            while (true)
            {
                bool failed;
                try
                {
                    failed = method == 0 ? StatX(directory, path, flags, 0xFFF, status) != 0
                        : method == 1 ? SysCallStatX((IntPtr)Flags.StatxCall, (IntPtr)directory, path, (IntPtr)flags, (IntPtr)0xFFF, status) != IntPtr.Zero
                        : SysCallFStatAt((IntPtr)Flags.FStatAtCall, (IntPtr)directory, path, status, (IntPtr)flags) != IntPtr.Zero;
                }
                catch (EntryPointNotFoundException) when (method == 0)
                {
                    Interlocked.CompareExchange(ref statMethod, 1, 0);
                    method = 1;
                    continue;
                }
                if (!failed)
                {
                    if (replaced >= 0)
                    {
                        Interlocked.CompareExchange(ref statMethod, 2, replaced);
                    }
                    errno = 0;
                    return method == 2 ? FromLinuxStat(status, RuntimeInformation.ProcessArchitecture) : FromStatx(status);
                }
                errno = Marshal.GetLastWin32Error();
                if (method < 2 && (errno == ENOSYS || errno == EPERM))
                {
                    replaced = method;
                    method = 2;
                    continue;
                }
                return null;
            }
        }

        // struct stat with 64-bit inodes: st_dev (int32) at 0, st_mode (uint16) at 4, st_nlink (uint16) at 6, st_ino at 8,
        // st_uid at 16, st_gid at 20, st_size at 96.
        private static FileStatus FromDarwin(byte[] status)
        {
            return new FileStatus
            {
                Device = (uint)BitConverter.ToInt32(status, 0),
                Mode = BitConverter.ToUInt16(status, 4),
                Links = BitConverter.ToUInt16(status, 6),
                Inode = BitConverter.ToUInt64(status, 8),
                Uid = BitConverter.ToUInt32(status, 16),
                Gid = BitConverter.ToUInt32(status, 20),
                Size = BitConverter.ToInt64(status, 96),
            };
        }

        // struct statx: stx_mask at 0, stx_nlink at 16, stx_uid at 20, stx_gid at 24, stx_mode (uint16) at 28, stx_ino at
        // 32, stx_size at 40, stx_btime (tv_sec, then tv_nsec as uint32) at 80, stx_dev_major at 136 and stx_dev_minor at
        // 140. stx_btime counts only where stx_mask has STATX_BTIME: the file system records it.
        private static FileStatus FromStatx(byte[] status)
        {
            const uint StatxBirthTime = 0x800;
            return new FileStatus
            {
                Links = BitConverter.ToUInt32(status, 16),
                Uid = BitConverter.ToUInt32(status, 20),
                Gid = BitConverter.ToUInt32(status, 24),
                Mode = BitConverter.ToUInt16(status, 28),
                Inode = BitConverter.ToUInt64(status, 32),
                Size = (long)BitConverter.ToUInt64(status, 40),
                Device = ((ulong)BitConverter.ToUInt32(status, 136) << 32) | BitConverter.ToUInt32(status, 140),
                Birth = (BitConverter.ToUInt32(status, 0) & StatxBirthTime) == 0
                    ? null
                    : BitConverter.ToInt64(status, 80).ToString(System.Globalization.CultureInfo.InvariantCulture) + "." + BitConverter.ToUInt32(status, 88).ToString("D9", System.Globalization.CultureInfo.InvariantCulture),
            };
        }

        // The status the kernel's older status call fills in on architecture, read as the kernel's headers lay it out
        // (arch/*/include/uapi/asm/stat.h). newfstatat's struct stat: on x64, st_dev at 0, st_ino at 8, st_nlink (64-bit)
        // at 16, st_mode at 24, st_uid at 28, st_gid at 32, st_size at 48; on Arm64 (the generic layout), st_dev at 0,
        // st_ino at 8, st_mode at 16, st_nlink (32-bit) at 20, st_uid at 24, st_gid at 28, st_size at 48. fstatat64's
        // struct stat64 on 32-bit Arm and x86: st_dev at 0, st_mode at 16, st_nlink at 20, st_uid at 24, st_gid at 28, and
        // st_size and the whole st_ino at 48 and 96 on Arm, whose EABI aligns 64-bit members to 8 bytes, or at 44 and 88 on
        // x86, which aligns them to 4 (the kernel's copies for the other ABIs, oldabi_stat64 on Arm and the ia32 stat64 on
        // x64, are packed to match those). st_dev is written with new_encode_dev, and is split into its major and minor
        // numbers as statx gives them, so that an identity read either way holds the same numbers. Read this way, it holds
        // no time of creation, so it identifies no file a checkpoint recorded (FileIdentity.Same).
        internal static FileStatus FromLinuxStat(byte[] status, Architecture architecture)
        {
            ulong device = BitConverter.ToUInt64(status, 0);
            ulong major = ((device >> 8) & 0xFFF) | ((device >> 32) & ~0xFFFUL);
            ulong minor = (device & 0xFF) | ((device >> 12) & ~0xFFUL & 0xFFFFFFFFUL);
            var found = new FileStatus { Device = (major << 32) | (minor & 0xFFFFFFFFUL) };
            switch (architecture)
            {
                case Architecture.X64:
                    found.Inode = BitConverter.ToUInt64(status, 8);
                    found.Links = (long)BitConverter.ToUInt64(status, 16);
                    found.Mode = (int)BitConverter.ToUInt32(status, 24);
                    found.Uid = BitConverter.ToUInt32(status, 28);
                    found.Gid = BitConverter.ToUInt32(status, 32);
                    found.Size = BitConverter.ToInt64(status, 48);
                    break;
                case Architecture.Arm64:
                    found.Inode = BitConverter.ToUInt64(status, 8);
                    found.Mode = (int)BitConverter.ToUInt32(status, 16);
                    found.Links = BitConverter.ToUInt32(status, 20);
                    found.Uid = BitConverter.ToUInt32(status, 24);
                    found.Gid = BitConverter.ToUInt32(status, 28);
                    found.Size = BitConverter.ToInt64(status, 48);
                    break;
                case Architecture.Arm:
                case Architecture.X86:
                    bool arm = architecture == Architecture.Arm;
                    found.Mode = (int)BitConverter.ToUInt32(status, 16);
                    found.Links = BitConverter.ToUInt32(status, 20);
                    found.Uid = BitConverter.ToUInt32(status, 24);
                    found.Gid = BitConverter.ToUInt32(status, 28);
                    found.Size = BitConverter.ToInt64(status, arm ? 48 : 44);
                    found.Inode = BitConverter.ToUInt64(status, arm ? 96 : 88);
                    break;
                default:
                    throw new PlatformNotSupportedException($"No status layout is known for {architecture}.");
            }
            return found;
        }

        // Removes the object's access control list, so its permissions alone decide who can use it; for a directory, also
        // the list its new entries would inherit. A file system without access control lists has none to remove. The
        // removal is checked: an object that still has a list fails.
        internal static void RemoveAccessControl(int fd, bool directory, string what)
        {
            if (Flags.Darwin)
            {
                IntPtr empty = AclInit(0);
                int result = AclSetFd(fd, empty, AclTypeExtended);
                int errno = Marshal.GetLastWin32Error();
                AclFree(empty);
                if (result != 0 && !Unsupported(errno))
                {
                    throw Failure(errno, $"{what} cannot be made private: its access control list cannot be removed");
                }
                if (AclEntries(AclGetFd(fd, AclTypeExtended)) > 0)
                {
                    throw new IOException($"{what} still has an access control list after it was removed, so nothing was written.");
                }
                return;
            }
            foreach (string attribute in directory ? new[] { PosixAclAccess, PosixAclDefault } : new[] { PosixAclAccess })
            {
                if (FRemoveXattrLinux(fd, attribute) != 0)
                {
                    int errno = Marshal.GetLastWin32Error();
                    if (errno != Flags.NoAttribute && !Unsupported(errno))
                    {
                        throw Failure(errno, $"{what} cannot be made private: its access control list cannot be removed");
                    }
                }
                if ((long)FGetXattrLinux(fd, attribute, null, UIntPtr.Zero) >= 0)
                {
                    throw new IOException($"{what} still has an access control list after it was removed, so nothing was written.");
                }
            }
        }

        internal static void RemoveAccessControl(SafeFileHandle file, string what)
        {
            bool added = false;
            try
            {
                file.DangerousAddRef(ref added);
                RemoveAccessControl((int)file.DangerousGetHandle(), directory: false, what);
            }
            finally
            {
                if (added)
                {
                    file.DangerousRelease();
                }
            }
        }

        // Gives stage the access control list of existing, the regular file at name in the directory held by directory,
        // if it has one. The list is read through a descriptor of that very file, opened relative to the held directory
        // without following a symbolic link and shown to be existing (same device and inode), never by path, which a
        // rename of a directory above it could make name another file. The descriptor is opened to read where the file
        // allows that, and otherwise to write on macOS, which a write-only file allows, or only to name the file on Linux
        // (O_PATH), which needs neither. On macOS a file this user may neither read nor write cannot be examined, so it is
        // not replaced. A list that exists but cannot be read or given fails. The stage has no list of its own yet.
        internal static void CopyAccessControl(int directory, string name, FileStatus existing, SafeFileHandle stage, string what)
        {
            int fd = OpenToExamine(directory, name, what, out bool nameOnly);
            try
            {
                FileStatus opened = Stat(fd, what);
                if (!opened.Same(existing) || !opened.IsRegular)
                {
                    throw new IOException($"{what} was replaced while the download was replacing it, so it was not replaced.");
                }
                bool added = false;
                try
                {
                    stage.DangerousAddRef(ref added);
                    int stageFd = (int)stage.DangerousGetHandle();
                    if (Flags.Darwin)
                    {
                        IntPtr acl = AclGetFd(fd, AclTypeExtended);
                        int errno = Marshal.GetLastWin32Error();
                        if (acl == IntPtr.Zero)
                        {
                            if (errno != ENOENT && !Unsupported(errno))
                            {
                                throw Failure(errno, $"The access control list of {what} cannot be read, so it was not replaced");
                            }
                        }
                        else
                        {
                            int result = AclSetFd(stageFd, acl, AclTypeExtended);
                            errno = Marshal.GetLastWin32Error();
                            AclFree(acl);
                            if (result != 0)
                            {
                                throw Failure(errno, $"The access control list of {what} cannot be given to the file replacing it, so it was not replaced");
                            }
                        }
                    }
                    else
                    {
                        byte[] value = ReadPosixAcl(fd, nameOnly, what);
                        if (value != null && FSetXattrLinux(stageFd, PosixAclAccess, value, (UIntPtr)value.Length, 0) != 0)
                        {
                            throw Failure(Marshal.GetLastWin32Error(), $"The access control list of {what} cannot be given to the file replacing it, so it was not replaced");
                        }
                    }
                }
                finally
                {
                    if (added)
                    {
                        stage.DangerousRelease();
                    }
                }
            }
            finally
            {
                Close(fd);
            }
        }

        // A descriptor of the file at name in the directory held by directory, to examine it: see CopyAccessControl.
        // nameOnly says it only names the file (O_PATH), so its attributes are read by its /proc path.
        private static int OpenToExamine(int directory, string name, string what, out bool nameOnly)
        {
            int flags = Flags.NoFollow | Flags.NonBlock | Flags.CloseOnExec;
            nameOnly = false;
            int fd = OpenAt(directory, name, Flags.ReadOnly | flags);
            if (fd >= 0)
            {
                return fd;
            }
            int errno = Marshal.GetLastWin32Error();
            if (errno != EACCES && errno != EPERM)
            {
                throw Failure(errno, $"{what} cannot be examined before it is replaced");
            }
            if (Flags.Darwin)
            {
                fd = OpenAt(directory, name, Flags.WriteOnly | flags);
                if (fd >= 0)
                {
                    return fd;
                }
                errno = Marshal.GetLastWin32Error();
                if (errno == EACCES || errno == EPERM)
                {
                    throw new IOException($"{what} can be neither read nor written by this user, so who may use it cannot be examined, and it was not replaced.");
                }
                throw Failure(errno, $"{what} cannot be examined before it is replaced");
            }
            fd = OpenAt(directory, name, Flags.PathOnly | Flags.NoFollow | Flags.CloseOnExec);
            if (fd < 0)
            {
                throw Failure(Marshal.GetLastWin32Error(), $"{what} cannot be examined before it is replaced");
            }
            nameOnly = true;
            return fd;
        }

        // The POSIX access ACL of the file fd is a descriptor of: through fd, or, where fd only names the file (O_PATH),
        // through its /proc path, which is that same file. Null when it has none.
        private static byte[] ReadPosixAcl(int fd, bool nameOnly, string what)
        {
            string procPath = nameOnly ? $"/proc/self/fd/{fd}" : null;
            long size = (long)(nameOnly ? GetXattrLinux(procPath, PosixAclAccess, null, UIntPtr.Zero) : FGetXattrLinux(fd, PosixAclAccess, null, UIntPtr.Zero));
            if (size < 0)
            {
                int errno = Marshal.GetLastWin32Error();
                if (errno == Flags.NoAttribute || Unsupported(errno))
                {
                    return null;
                }
                throw Failure(errno, $"The access control list of {what} cannot be read, so it was not replaced");
            }
            byte[] value = new byte[size];
            long read = (long)(nameOnly ? GetXattrLinux(procPath, PosixAclAccess, value, (UIntPtr)size) : FGetXattrLinux(fd, PosixAclAccess, value, (UIntPtr)size));
            if (read != size)
            {
                throw new IOException($"The access control list of {what} changed while it was read, so it was not replaced.");
            }
            return value;
        }

        // The number of entries in acl, which is freed; 0 for none.
        private static int AclEntries(IntPtr acl)
        {
            if (acl == IntPtr.Zero)
            {
                return 0;
            }
            int entries = 0;
            for (int id = 0; AclGetEntry(acl, id, out _) == 0; id = -1)
            {
                entries++;
            }
            AclFree(acl);
            return entries;
        }

        private static bool Unsupported(int errno)
        {
            return errno == Flags.NotSupported || errno == Flags.OperationNotSupported;
        }

        // Makes what has been written to file reach storage, not only the system's cache: on macOS with F_FULLFSYNC,
        // which fsync alone does not do there; with fsync on Linux, and on a macOS file system without F_FULLFSYNC, which
        // is all such a file system offers. Only F_FULLFSYNC's being unsupported falls back to fsync: any other failure of
        // either flush is thrown, and nothing is taken to have reached storage.
        internal static void SyncFile(SafeFileHandle file, string what)
        {
            int errno;
            if (Flags.Darwin)
            {
                errno = FlushErrno(FlushCall.FullSyncFile, () => FControl(file, FullFsync));
                if (errno == 0)
                {
                    return;
                }
                if (!FullSyncUnsupported(errno))
                {
                    throw Failure(errno, $"{what} cannot be flushed to storage");
                }
            }
            errno = FlushErrno(FlushCall.SyncFile, () => FSync(file));
            if (errno != 0)
            {
                throw Failure(errno, $"{what} cannot be flushed to storage");
            }
        }

        // The same for a directory's entries, such as a name just renamed into it. Returns false where the file system
        // reports that it cannot flush a directory (EINVAL, or not supported), or, for a descriptor opened only to search
        // a directory this user may not list (searchOnly), where the system does not flush through such a descriptor
        // (EBADF, as Linux's O_PATH gives): its entries then reach storage when the system writes them, and the caller
        // decides whether that will do. Any other failure is thrown.
        internal static bool SyncDirectory(int fd, string what, bool searchOnly = false)
        {
            int errno;
            if (Flags.Darwin)
            {
                errno = FlushErrno(FlushCall.FullSyncDirectory, () => FControl(fd, FullFsync));
                if (errno == 0)
                {
                    return true;
                }
                if (!FullSyncUnsupported(errno) && !(searchOnly && errno == EBADF))
                {
                    throw Failure(errno, $"{what} cannot be flushed to storage");
                }
            }
            errno = FlushErrno(FlushCall.SyncDirectory, () => FSync(fd));
            if (errno == 0)
            {
                return true;
            }
            if (errno == EINVAL || Unsupported(errno) || (searchOnly && errno == EBADF))
            {
                return false;
            }
            throw Failure(errno, $"{what} cannot be flushed to storage");
        }

        // Whether F_FULLFSYNC failed only because the file system does not have it.
        private static bool FullSyncUnsupported(int errno)
        {
            return errno == EINVAL || errno == ENOTTY || Unsupported(errno);
        }

        // The errno of one flush, 0 when it succeeded: the system's, unless a test set FlushFaults.
        private static int FlushErrno(FlushCall call, Func<int> flush)
        {
            int injected = FlushFaults.Value?.Invoke(call) ?? 0;
            if (injected != 0)
            {
                return injected;
            }
            return flush() == 0 ? 0 : Marshal.GetLastWin32Error();
        }

        // open(2) flag values and errno values that differ between platforms. Null where they are not known, and
        // descriptors are then not used.
        internal sealed class PlatformFlags
        {
            internal int ReadOnly => 0;

            internal int WriteOnly => 1;

            internal int ReadWrite => 2;

            internal int Directory { get; private set; }

            internal int NoFollow { get; private set; }

            internal int CloseOnExec { get; private set; }

            internal int NonBlock { get; private set; }

            internal int RemoveDirectory { get; private set; }

            internal int Loop { get; private set; }

            // macOS, whose structures and ACLs these are; Linux otherwise.
            internal bool Darwin { get; private set; }

            // AT_SYMLINK_NOFOLLOW and AT_EMPTY_PATH (statx on a descriptor itself).
            internal int SymlinkNoFollow { get; private set; }

            internal int EmptyPath { get; private set; }

            // O_PATH: a descriptor that only names a file (Linux).
            internal int PathOnly { get; private set; }

            // A descriptor of a directory that may only be searched, which is all naming its entries needs: O_SEARCH on
            // macOS (O_EXEC with O_DIRECTORY), O_PATH with O_DIRECTORY on Linux.
            internal int SearchOnly { get; private set; }

            // AT_FDCWD: the current directory, for a call relative to a directory given a path.
            internal int CurrentDirectory { get; private set; }

            // The errno values for a missing extended attribute and an unsupported operation.
            internal int NoAttribute { get; private set; }

            internal int NotSupported { get; private set; }

            internal int OperationNotSupported { get; private set; }

            // The system call numbers of statx and of the older status call StatAt falls back to on Linux: newfstatat on
            // x64 and Arm64, fstatat64 on x86 and 32-bit Arm (arch/x86/entry/syscalls/syscall_64.tbl and syscall_32.tbl,
            // include/uapi/asm-generic/unistd.h, arch/arm/tools/syscall.tbl).
            internal int StatxCall { get; private set; }

            internal int FStatAtCall { get; private set; }

            internal static PlatformFlags ForCurrentPlatform()
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return new PlatformFlags
                    {
                        Directory = 0x100000,
                        NoFollow = 0x100,
                        CloseOnExec = 0x1000000,
                        NonBlock = 0x4,
                        RemoveDirectory = 0x80,
                        Loop = 62,
                        Darwin = true,
                        SymlinkNoFollow = 0x20,
                        NoAttribute = 93,
                        NotSupported = 45,
                        OperationNotSupported = 102,
                        SearchOnly = 0x40000000 | 0x100000,
                        CurrentDirectory = -2,
                    };
                }
                // Linux, used only where the C library can then be called (Native's static constructor).
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    switch (RuntimeInformation.ProcessArchitecture)
                    {
                        case Architecture.X64:
                        case Architecture.X86:
                            return new PlatformFlags
                            {
                                Directory = 0x10000,
                                NoFollow = 0x20000,
                                CloseOnExec = 0x80000,
                                NonBlock = 0x800,
                                RemoveDirectory = 0x200,
                                Loop = 40,
                                SymlinkNoFollow = 0x100,
                                EmptyPath = 0x1000,
                                PathOnly = 0x200000,
                                NoAttribute = 61,
                                NotSupported = 95,
                                OperationNotSupported = 95,
                                SearchOnly = 0x200000 | 0x10000,
                                CurrentDirectory = -100,
                                StatxCall = RuntimeInformation.ProcessArchitecture == Architecture.X64 ? 332 : 383,
                                FStatAtCall = RuntimeInformation.ProcessArchitecture == Architecture.X64 ? 262 : 300,
                            };
                        case Architecture.Arm64:
                        case Architecture.Arm:
                            return new PlatformFlags
                            {
                                Directory = 0x4000,
                                NoFollow = 0x8000,
                                CloseOnExec = 0x80000,
                                NonBlock = 0x800,
                                RemoveDirectory = 0x200,
                                Loop = 40,
                                SymlinkNoFollow = 0x100,
                                EmptyPath = 0x1000,
                                PathOnly = 0x200000,
                                NoAttribute = 61,
                                NotSupported = 95,
                                OperationNotSupported = 95,
                                SearchOnly = 0x200000 | 0x4000,
                                CurrentDirectory = -100,
                                StatxCall = RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? 291 : 397,
                                FStatAtCall = RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? 79 : 327,
                            };
                    }
                }
                return null;
            }
        }
    }
}