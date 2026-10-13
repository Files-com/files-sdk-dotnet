using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Transfers
{
    // Directories and stages on Windows. The directory the caller names is opened by the path the caller gave, following
    // the links in it, which are the caller's choice; a directory below it is entered one name at a time, and one that
    // is a reparse point is refused.
    //
    // With CreateNew or ReplaceRegularFile (held), the directory is held from the moment it is selected, before anything
    // is listed or fetched, until the transfer is done with it: its path is resolved once, following those links, and
    // every directory on the resolved path, from its volume's or share's root, is opened without following a reparse
    // point that names another path (a symbolic link, junction or mount point), which is refused, and held open without
    // sharing the right to delete it, as CheckpointStorageWindows holds a checkpoint's directories; so is every directory
    // entered below it. While they are held no one can rename, move or remove any of them, so every path the transfer
    // uses, which it builds on the resolved path, names the directories it selected and checked. A path that cannot be
    // walked from a root that way, such as one on a volume with no drive letter, is refused for those policies
    // (DownloadPublicationProblem.NotSupported). With the default policy, as before, directories are used by the path the
    // caller gave, which a program renaming entries at the same moment can redirect.
    //
    // A stage is a file in a staging directory of its own, .files-download-<random>, created in the destination directory
    // and inheriting from it. The staging directory is created and opened in one call, so the handle held (without delete
    // sharing) is the very directory this download created, whatever its owner reads as, and its identity (volume serial
    // number and file index) is recorded: it is removed only through that handle, or by a later run only while it is the
    // directory with that identity, and only while empty; anything else at a staging name is left alone. The stage is
    // created with an access control list of its own, protected from inheritance, that allows only this user, SYSTEM and
    // Administrators, and held open shared with no one, so its bytes are private while it is written and after it is
    // closed, kept for a later run or left behind by a crash. Its list is read back: only a list read back as that private
    // one, of a trusted owner, shows the stage private, and only a stage shown private is kept for a later run. A list
    // that cannot be read back may still be in place, so it is replaced as the file is put in place all the same; only a
    // file system shown to keep no lists (read back as none, or a volume that says so) needs none. A stage an earlier run
    // kept is continued only while it is that file, still has one name and is still shown private; otherwise the download
    // starts over, and the stage, when it is shown to be this download's, is removed. Every right the stage is opened with
    // is granted by its own list, so a destination's list never refuses it.
    //
    // Only as it is put in place does the stage take its final access: the access control list of the regular file it
    // replaces, for ReplaceRegularFile, exactly as that file has it (WindowsPublication); otherwise the one a file created
    // directly in the destination directory gets. That directory is the one the staging directory was created through,
    // opened as the caller's path leads (following its links, as putting the file there does), so the list is that of the
    // very directory the file lands in. The file system shows the list itself: an empty file of the SDK's own is created
    // there, holding nothing, deleted when its handle closes, and its list read back, which needs no right to read the
    // directory's own descriptor (WindowsNative.CreateAccessProbe); where that file could not be read or deleted, the list
    // is worked out from the directory's descriptor, if this user may read it (WindowsNative.WithNewFileDescriptor). It is
    // what the destination passes on to its files, entries that reach only its own children included, which inheriting
    // through the staging directory would lose; else the token's default list, even an empty one; else no list. It is
    // written to the stage as it is, its protection and auto-inheritance included, with SetFileSecurity by a path shown
    // to reach the stage (WindowsNative.SetFileDacl), and read back through the stage's handle, control and all; a rename
    // keeps it. When it cannot be learned or given, the file is not put in place
    // (DownloadPublicationProblem.AccessNotKept). The stage is still open and shared with no one then, so no one else can
    // open it meanwhile; if it is then not put in place, it is made private again before it is kept.
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    internal sealed class WindowsDirectory : LocalDirectory
    {
        private static readonly SdkLog log = SdkLog.For(typeof(WindowsDirectory));

        // The directory as the caller named it, for messages and for the local paths a checkpoint records.
        private readonly string path;
        // The path every operation uses: the resolved one when held, else the caller's.
        private readonly string where;
        // The directories held, this one's last, shared by every handle to it and to the directories below it.
        private readonly Held held;
        private int closed;

        private WindowsDirectory(string path, string where, Held held)
        {
            this.path = path;
            this.where = where;
            this.held = held;
        }

        internal override string Path => path;

        // Opens the directory at path; held, for CreateNew and ReplaceRegularFile.
        internal static WindowsDirectory OpenPath(string path, bool hold)
        {
            string full = System.IO.Path.GetFullPath(path);
            if (!hold)
            {
                if (!Directory.Exists(full))
                {
                    throw new DirectoryNotFoundException($"The directory {path} does not exist.");
                }
                return new WindowsDirectory(full, full, null);
            }
            string resolved = WindowsNative.FinalPathOf(full);
            string root;
            List<string> names;
            try
            {
                root = WindowsSecurity.Root(resolved, out names);
            }
            catch (UntrustedStateException)
            {
                throw new DownloadPublicationException(DownloadPublicationProblem.NotSupported, full, $"The directory {path} is at {WindowsSecurity.Shown(resolved)}, whose directories cannot be held from the root of its volume or share, so files cannot be put in place there as the publication policy says, and nothing was downloaded.");
            }
            Held current = null;
            try
            {
                string at = root;
                current = new Held(WindowsPublication.HoldDirectory(at, WindowsSecurity.Shown(at)), null);
                foreach (string name in names)
                {
                    at = at.TrimEnd('\\') + "\\" + name;
                    var below = new Held(WindowsPublication.HoldDirectory(at, WindowsSecurity.Shown(at)), current);
                    // The directory above is now held by the one below it only.
                    current.Release();
                    current = below;
                }
                return new WindowsDirectory(full, at, current);
            }
            catch
            {
                current?.Release();
                throw;
            }
        }

        internal override LocalDirectory OpenChild(string name, bool create)
        {
            CheckName(name);
            string shown = System.IO.Path.Combine(path, name);
            string child = Join(where, name);
            // A name that reaches another folder through its short (8.3) name would enter that folder instead.
            if (WindowsPublication.IsAnotherEntrysShortName(child, name))
            {
                throw ShortNameOfAnother(shown, "folder");
            }
            if (held == null)
            {
                var info = new DirectoryInfo(child);
                if (!info.Exists)
                {
                    if (File.Exists(child) || !create)
                    {
                        throw new IOException($"{shown} is not a directory.");
                    }
                    info.Create();
                    info.Refresh();
                }
                if ((info.Attributes & FileAttributes.ReparsePoint) != 0)
                {
                    throw new IOException($"{shown} is a symbolic link or other reparse point, which transfers do not follow.");
                }
                return new WindowsDirectory(shown, child, null);
            }
            if (create)
            {
                WindowsPublication.CreateDirectory(child, shown);
            }
            return new WindowsDirectory(shown, child, new Held(WindowsPublication.HoldDirectory(child, shown), held));
        }

        // The failure of a remote name that is the short (8.3) name of a local entry of another name: Windows would
        // open, enter or replace that entry, so the transfer does not use the name.
        private static IOException ShortNameOfAnother(string shown, string kind)
        {
            return new IOException($"{shown} is the short name of a local {kind} with another name, which the transfer does not replace or use. Rename the local {kind}, or download to another folder.");
        }

        internal override LocalDirectory Duplicate()
        {
            held?.AddReference();
            return new WindowsDirectory(path, where, held);
        }

        internal override FileStream OpenFileToRead(string name)
        {
            CheckName(name);
            string child = Join(where, name);
            if ((File.GetAttributes(child) & (FileAttributes.ReparsePoint | FileAttributes.Directory | FileAttributes.Device)) != 0)
            {
                throw new IOException($"{System.IO.Path.Combine(path, name)} is not a regular file, which transfers read only.");
            }
            return new FileStream(child, FileMode.Open, FileAccess.Read, FileShare.Read, 1);
        }

        internal override LocalStage CreateStage(string finalName)
        {
            CheckName(finalName);
            string stagingName = PathDirectory.StagingName();
            string shown = System.IO.Path.Combine(path, stagingName);
            StagingDirectory holder = StagingDirectory.Create(where, path, stagingName, shown);
            try
            {
                return WindowsStage.Create(this, stagingName, holder, finalName);
            }
            catch
            {
                holder.Remove();
                throw;
            }
        }

        internal override LocalStage ReopenStage(StageLocation location, string finalName, string identity = null, string stagingIdentity = null)
        {
            CheckName(finalName);
            CheckName(location.Directory);
            CheckName(location.File);
            StagingDirectory holder = StagingDirectory.Open(Join(where, location.Directory), System.IO.Path.Combine(path, location.Directory), where, path);
            if (holder == null)
            {
                return null;
            }
            bool recorded = stagingIdentity != null && holder.Identity == stagingIdentity;
            if (stagingIdentity != null && !recorded)
            {
                // Not the staging directory recorded: it stays as it is.
                holder.Dispose();
                return null;
            }
            WindowsStage stage;
            try
            {
                stage = WindowsStage.Reopen(this, location.Directory, holder, location.File, finalName, identity);
            }
            catch
            {
                holder.Dispose();
                throw;
            }
            if (stage == null)
            {
                // The staging directory recorded goes too once it is empty, as when the stage removed was this
                // download's; anything else in it keeps it.
                if (recorded)
                {
                    holder.Remove();
                }
                else
                {
                    holder.Dispose();
                }
            }
            return stage;
        }

        internal override LocalEntry Examine(string name)
        {
            CheckName(name);
            return WindowsPublication.Examine(Join(where, name));
        }

        // Held, every policy (WindowsPublication); by path, only the default, which does not need the directory held.
        internal override bool Supports(DownloadPublicationPolicy policy)
        {
            return held != null || policy == DownloadPublicationPolicy.ReplaceExisting;
        }

        // A stage is renamed into place, so it has no second name to remove: only its staging directory is, while it is
        // the one recorded, and empty. Without a recorded identity, as an earlier SDK version recorded none, it stays.
        internal override void FinishPlaced(StageLocation location, string identity, string stagingIdentity)
        {
            CheckName(location.Directory);
            if (stagingIdentity == null)
            {
                return;
            }
            StagingDirectory holder = StagingDirectory.Open(Join(where, location.Directory), System.IO.Path.Combine(path, location.Directory));
            if (holder == null)
            {
                return;
            }
            if (holder.Identity == stagingIdentity)
            {
                holder.Remove();
            }
            else
            {
                holder.Dispose();
            }
        }

        // Windows has no directory flush here (Durable.DirectoryFlush.NotAttempted).
        internal override bool FlushEntries()
        {
            return false;
        }

        public override void Dispose()
        {
            if (Interlocked.Exchange(ref closed, 1) == 0)
            {
                held?.Release();
            }
        }

        private static string Join(string directory, string name)
        {
            return directory.TrimEnd('\\') + "\\" + name;
        }

        // A held directory, with the one above it held for as long as it is: closed once every handle to it, and to every
        // directory below it, is disposed.
        private sealed class Held
        {
            private readonly SafeFileHandle handle;
            private readonly Held parent;
            private int references = 1;

            internal Held(SafeFileHandle handle, Held parent)
            {
                this.handle = handle;
                this.parent = parent;
                parent?.AddReference();
            }

            internal void AddReference()
            {
                Interlocked.Increment(ref references);
            }

            internal void Release()
            {
                if (Interlocked.Decrement(ref references) == 0)
                {
                    handle.Dispose();
                    parent?.Release();
                }
            }
        }

        // A staging directory, held without delete sharing while its stage is. One this download makes is created and opened
        // in the same call, so its handle is the directory this download created, whose identity the checkpoint records;
        // one an earlier run made is opened without following a link, and is that run's only while its identity is the one
        // recorded (ReopenStage, FinishPlaced), whoever its owner reads as. It is removed only through its handle, so only
        // while it is that very directory, and only if it is empty; anything else at a staging name is left alone.
        private sealed class StagingDirectory : IDisposable
        {
            private readonly SafeFileHandle handle;

            private StagingDirectory(SafeFileHandle handle, string identity, SafeFileHandle destination)
            {
                this.handle = handle;
                Identity = identity;
                Destination = destination;
            }

            internal string Identity { get; }

            // The destination directory, the very directory the staging directory is in and a stage is put in, as opened
            // following the caller's path when the staging directory was created or opened (WindowsNative.OpenDestination);
            // null when it was opened only to be removed.
            internal SafeFileHandle Destination { get; }

            // A new staging directory called name in the destination directory at where (shown as path), created through
            // the destination's handle (WindowsPublication.CreateStagingDirectory), which is kept.
            internal static StagingDirectory Create(string where, string path, string name, string shown)
            {
                SafeFileHandle destination = WindowsNative.OpenDestination(where, path);
                SafeFileHandle handle = null;
                try
                {
                    handle = WindowsPublication.CreateStagingDirectory(destination, name, shown);
                    return new StagingDirectory(handle, WindowsPublication.IdentityOf(handle, $"The staging directory {shown}"), destination);
                }
                catch
                {
                    handle?.Dispose();
                    destination.Dispose();
                    throw;
                }
            }

            // The directory at path, or null when there is none, or it is a link or not a directory; with the destination
            // directory at where (shown as destinationShown) opened too, when it is to hold a stage.
            internal static StagingDirectory Open(string path, string shown, string where = null, string destinationShown = null)
            {
                SafeFileHandle handle = WindowsPublication.OpenStagingDirectory(path, shown);
                if (handle == null)
                {
                    return null;
                }
                SafeFileHandle destination = null;
                try
                {
                    destination = where == null ? null : WindowsNative.OpenDestination(where, destinationShown);
                    return new StagingDirectory(handle, WindowsPublication.IdentityOf(handle, $"The staging directory {shown}"), destination);
                }
                catch
                {
                    destination?.Dispose();
                    handle.Dispose();
                    throw;
                }
            }

            // Removes the directory if it is empty, and closes it.
            internal void Remove()
            {
                WindowsPublication.DeleteOnClose(handle);
                handle.Dispose();
                Destination?.Dispose();
            }

            public void Dispose()
            {
                handle.Dispose();
                Destination?.Dispose();
            }
        }

        private sealed class WindowsStage : LocalStage
        {
            private readonly WindowsDirectory directory;
            private readonly string stagingName;
            private readonly StagingDirectory staging;
            private readonly string name;
            private readonly string finalName;
            private readonly SafeFileHandle handle;
            private readonly FileStream file;
            private readonly string identity;
            // Whether its private list may be in place, so that its access must be set as it is put in place
            // (WindowsSecurity.NeedsFinalAccess): true unless the file system is shown to keep no lists.
            private readonly bool needsFinalAccess;
            // Whether it was shown to be private when it was created or reopened (WindowsSecurity.ShownPrivate), which alone
            // lets it be kept for a later run.
            private readonly bool privateStage;
#if !NET6_0_OR_GREATER
            private readonly SemaphoreSlim writing = new SemaphoreSlim(1, 1);
#endif
            // Whether the stage has been given its final access, which a stage kept for a later run must not keep.
            private bool accessTaken;

            private WindowsStage(WindowsDirectory directory, string stagingName, StagingDirectory staging, string name, string finalName, SafeFileHandle handle, string identity, bool needsFinalAccess, bool privateStage)
            {
                this.needsFinalAccess = needsFinalAccess;
                this.directory = directory;
                this.stagingName = stagingName;
                this.staging = staging;
                this.name = name;
                this.finalName = finalName;
                this.identity = identity;
                this.privateStage = privateStage;
                this.handle = handle;
                // As the handle was opened (WindowsNative.StageIo): on .NET 6 and later the stream is asynchronous too.
                file = new FileStream(handle, FileAccess.ReadWrite, 1);
            }

            private string StagePath => Join(Join(directory.where, stagingName), name);

            internal static WindowsStage Create(WindowsDirectory directory, string stagingName, StagingDirectory staging, string finalName)
            {
                const string name = "download";
                string shown = System.IO.Path.Combine(directory.path, stagingName, name);
                SafeFileHandle handle = WindowsPublication.CreatePrivateStage(Join(Join(directory.where, stagingName), name), shown);
                try
                {
                    string what = $"The file being downloaded, {shown},";
                    // A list that cannot be read back is used, but not shown private, so not kept for a later run; it is
                    // still replaced as the file is put in place.
                    WindowsSecurity.StageList list = WindowsPublication.ReadStageList(handle, what, out bool shownPrivate);
                    return new WindowsStage(directory, stagingName, staging, name, finalName, handle, WindowsPublication.IdentityOf(handle, what), WindowsSecurity.NeedsFinalAccess(list), shownPrivate);
                }
                catch
                {
                    WindowsPublication.DeleteOnClose(handle);
                    handle.Dispose();
                    throw;
                }
            }

            // The stage an earlier run kept, if it is still the file recorded, with one name, and private; null when it is
            // not continued. One shown to be that file but no longer private, whose bytes someone else could have
            // changed, is removed.
            internal static WindowsStage Reopen(WindowsDirectory directory, string stagingName, StagingDirectory staging, string name, string finalName, string identity)
            {
                string shown = System.IO.Path.Combine(directory.path, stagingName, name);
                string what = $"The file being downloaded, {shown},";
                SafeFileHandle handle = WindowsPublication.OpenStage(Join(Join(directory.where, stagingName), name), shown);
                if (handle == null)
                {
                    return null;
                }
                try
                {
                    string opened = WindowsPublication.IdentityOf(handle, what);
                    if (identity != null && opened != identity)
                    {
                        // Not the stage recorded: whatever it is stays.
                        handle.Dispose();
                        return null;
                    }
                    if (!WindowsPublication.OneName(handle, what) || !WindowsPublication.IsPrivate(handle, what))
                    {
                        log.Info("Starting a download over: its stage is not private, so its bytes cannot be relied on");
                        if (identity != null)
                        {
                            WindowsPublication.DeleteOnClose(handle);
                        }
                        handle.Dispose();
                        return null;
                    }
                    return new WindowsStage(directory, stagingName, staging, name, finalName, handle, opened, needsFinalAccess: true, privateStage: true);
                }
                catch (IOException)
                {
                    handle.Dispose();
                    return null;
                }
            }

            internal override StageLocation Location => new StageLocation(stagingName, name, null);

            internal override string Identity => identity;

            internal override string StagingIdentity => staging.Identity;

            internal override long Length => file.Length;

            internal override async Task WriteAsync(long offset, byte[] buffer, int count, CancellationToken cancellationToken)
            {
#if NET6_0_OR_GREATER
                await RandomAccess.WriteAsync(handle, new ReadOnlyMemory<byte>(buffer, 0, count), offset, cancellationToken);
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

            // Puts the stage in place, flushed, with its final access. The default replaces a file of that name as Windows
            // does it, by path; CreateNew and ReplaceRegularFile rename the stage through its own handle into the held
            // directory (WindowsPublication).
            protected override void PublishCore(DownloadPublicationPolicy policy)
            {
                file.Flush(true);
                // A final name that reaches another file through its short (8.3) name would replace that file, whichever
                // policy replaces; the stage stays where it is, as it does for any refused destination.
                if (WindowsPublication.IsAnotherEntrysShortName(Join(directory.where, finalName), finalName))
                {
                    throw ShortNameOfAnother(System.IO.Path.Combine(directory.path, finalName), "file");
                }
                if (policy != DownloadPublicationPolicy.ReplaceExisting)
                {
                    WindowsPublication.Publish(handle, directory.where, directory.path, finalName, policy, needsFinalAccess ? GiveNewFileAccess : (Action)null, ref accessTaken);
                    MarkPlaced();
                    return;
                }
                if (needsFinalAccess)
                {
                    accessTaken = true;
                    GiveNewFileAccess();
                }
                file.Dispose();
                string target = Join(directory.where, finalName);
#if NETCOREAPP3_0_OR_GREATER
                File.Move(StagePath, target, overwrite: true);
#else
                if (File.Exists(target))
                {
                    File.Replace(StagePath, target, null);
                }
                else
                {
                    File.Move(StagePath, target);
                }
#endif
                MarkPlaced();
            }

            // The access control list a file created directly in the destination directory gets: that of the directory the
            // staging directory was created in, which is the one the file is put in.
            private void GiveNewFileAccess()
            {
                WindowsPublication.GiveNewFileAccess(handle, staging.Destination, System.IO.Path.Combine(directory.path, finalName));
            }

            // Private again, and shown to be, after putting it in place gave it its final access and was then refused; a
            // stage never shown to be private, or closed, cannot be kept private.
            internal override bool RestorePrivacy()
            {
                if (!privateStage || handle.IsClosed)
                {
                    return false;
                }
                string what = $"The file being downloaded to {directory.path}";
                try
                {
                    if (accessTaken)
                    {
                        WindowsPublication.MakePrivate(handle, what);
                        accessTaken = false;
                    }
                    return WindowsPublication.IsPrivate(handle, what);
                }
                catch (IOException)
                {
                    return false;
                }
            }

            protected override void Close(bool delete, bool published)
            {
                if (delete && !handle.IsClosed)
                {
                    // Deleted through its own handle, when it closes: whatever may have taken its name stays.
                    WindowsPublication.DeleteOnClose(handle);
                }
                file.Dispose();
#if !NET6_0_OR_GREATER
                writing.Dispose();
#endif
                if (delete && !published)
                {
                    // Closed before a publication that then failed: removed while it is still the stage.
                    WindowsPublication.DeleteIfSame(StagePath, identity);
                }
                if (delete || published)
                {
                    staging.Remove();
                }
                else
                {
                    staging.Dispose();
                }
            }
        }
    }
}