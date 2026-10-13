using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace FilesCom.Transfers
{
    // Where transfers keep their checkpoints, trusted before anything read from it is relied on (CheckpointFile). A
    // checkpoint says which bytes of a download's stage are written, what access its published file gets, which parts of
    // an upload Files.com accepted and whether its final request was sent; another user able to change it could make a
    // transfer publish a partial file, give a new file wider access, or act on an upload's state falsely, whatever the
    // stage's own protection. So a checkpoint directory is used only when no other user can change what is in it, and a
    // checkpoint only when it is this user's own file. On Windows, WindowsStateDirectory checks that with Windows'
    // owners and access control lists. On macOS, and on Linux wherever the runtime can call the C library (where
    // UnixDirectory is used, Native), as follows:
    //
    //   - The directory's path is resolved once (realpath), following the links the caller's path goes through, and then
    //     opened again from / one name at a time without following symbolic links, so the directory opened is the one
    //     checked, and a link put anywhere in the way meanwhile is refused.
    //   - Every directory on that path, the checkpoint directory included, must belong to this user or to root, and be
    //     writable by neither its group nor other users, unless it is sticky and the entry below it on the path belongs to
    //     this user or root (as /tmp): then others cannot rename or remove that entry. On macOS it must also have no
    //     access control list entry that allows anything, since those grant access beyond its permissions; entries that
    //     only deny are fine. On Linux, whatever a POSIX access control list allows other users is within its group
    //     permissions (the list's mask), so the permissions alone decide.
    //   - A checkpoint is opened relative to the directory held, without following a symbolic link, and must be a regular
    //     file of this user's, with one link, writable by neither its group nor other users (and on macOS no allowing
    //     access control list entry).
    //   - A checkpoint is written as a new file made readable and writable only by this user, with no access control
    //     list, before anything is written to it; flushed; renamed over the checkpoint in the directory held; and the
    //     directory is flushed.
    //
    // Anything else fails before any byte of the checkpoint is read or written (UntrustedStateException): the checkpoint,
    // and what it records, are left as they are. A check that cannot be made, because a call fails, fails the same way or
    // as that call's own failure; it never lets a checkpoint be used unchecked. What a transfer did before it opened its
    // checkpoint stays done: a directory download lists the remote folder first, and directories the transfer was asked
    // to create, the checkpoint directory (CreateDirectory) or the local directory, may remain, empty. The guarantee is
    // against other users; a program running as this user or as root can do anything this one can, as with
    // LocalDirectory.
    //
    // Where none of this can be checked, on Linux where the runtime cannot call the C library at all (such as .NET Core
    // 2.x) or runs on a processor whose system layouts the SDK does not have (other than x64, x86, Arm64 and 32-bit Arm,
    // such as ppc64le or s390x), and on systems other than Windows, macOS and Linux, a checkpoint is refused the same way,
    // before any of it is read or written: the transfer runs only without one.
    internal static class CheckpointStorage
    {
        // For the SDK's own tests: the owner an object checked here is to be taken to have, given the status observed and
        // the owner it has, for the checks made within the flow of a test that sets it, so that an object of another
        // user's can be stood in for. Nothing else sets it.
        internal static readonly AsyncLocal<Func<Native.FileStatus, uint, uint>> ObservedOwnerFaults = new AsyncLocal<Func<Native.FileStatus, uint, uint>>();

        // For the SDK's own tests: whether the checks made within the flow of a test that sets it are to be taken as ones
        // this runtime cannot make (the C library cannot be called), as on .NET Core 2.x on Linux. Nothing else sets it.
        internal static readonly AsyncLocal<bool> NativeUnavailableFaults = new AsyncLocal<bool>();

        // The permissions a directory this SDK creates for checkpoints gets: this user's only.
        private const int PrivateDirectoryMode = 0x1C0;

        // Opens the directory holding checkpoints, trusted as the class describes.
        internal static StateDirectory Open(string directory)
        {
            string full = Path.GetFullPath(directory);
            if (IsWindows)
            {
                return WindowsStateDirectory.Open(full);
            }
            if (LocalDirectory.ByDescriptor && !NativeUnavailableFaults.Value)
            {
                return UnixStateDirectory.Open(full);
            }
            throw new UntrustedStateException("this runtime cannot check who else can change it, since it cannot call the C library (as with .NET Core 2.x on Linux), runs Linux on a processor the SDK has no system layouts for (other than x64, x86, Arm64 and 32-bit Arm, such as ppc64le or s390x), or runs on a system other than Windows, macOS and Linux; run the transfer without a checkpoint (on .NET Core 2.x, or on .NET 8 or later)", whole: true);
        }

#if NET5_0_OR_GREATER
        [System.Runtime.Versioning.SupportedOSPlatformGuard("windows")]
#endif
        private static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        // Creates directory, if missing, as a directory only this user can use, and so each missing directory above it.
        // Directories that exist are left as they are, and checked, like every other, when the directory is opened.
        internal static void CreateDirectory(string directory)
        {
            string full = Path.GetFullPath(directory);
            if (!IsWindows && (!LocalDirectory.ByDescriptor || NativeUnavailableFaults.Value))
            {
                // Nothing to create for: Open refuses every checkpoint here.
                return;
            }
            var missing = new Stack<string>();
            string existing = full;
            while (existing != null && !Directory.Exists(existing))
            {
                missing.Push(existing);
                existing = Path.GetDirectoryName(existing);
            }
            foreach (string created in missing)
            {
                if (IsWindows)
                {
                    WindowsNative.CreatePrivateDirectory(created);
                    continue;
                }
                string parent = Path.GetDirectoryName(created);
                int parentFd = Native.Open(parent, Native.Flags.ReadOnly | Native.Flags.Directory | Native.Flags.CloseOnExec);
                if (parentFd < 0)
                {
                    throw Native.Failure(Marshal.GetLastWin32Error(), $"The directory {parent} cannot be opened");
                }
                try
                {
                    if (Native.MkdirAt(parentFd, Path.GetFileName(created), PrivateDirectoryMode) != 0 && Marshal.GetLastWin32Error() != Native.EEXIST)
                    {
                        throw Native.Failure(Marshal.GetLastWin32Error(), $"The directory {created} cannot be created");
                    }
                }
                finally
                {
                    Native.Close(parentFd);
                }
            }
        }

        // Whether the directory path, or the nearest directory above it that exists, is tree or inside it as it is on
        // storage, whatever links path goes through: by identity (device and inode) where directories are held by
        // descriptor, and by final path on Windows (GetFinalPathNameByHandle, which resolves links, junctions and mount
        // points). False elsewhere, where only names are compared.
        internal static bool IsWithin(string path, LocalDirectory tree)
        {
            string existing = Path.GetFullPath(path);
            while (!Directory.Exists(existing))
            {
                existing = Path.GetDirectoryName(existing);
                if (existing == null)
                {
                    return false;
                }
            }
            if (IsWindows)
            {
                return WindowsSecurity.Within(WindowsNative.FinalPathOf(existing), WindowsNative.FinalPathOf(tree.Path));
            }
            if (!LocalDirectory.ByDescriptor)
            {
                return false;
            }
            Native.FileStatus root = tree.Identity;
            if (root == null)
            {
                return false;
            }
            int fd = Native.Open(existing, Native.Flags.ReadOnly | Native.Flags.Directory | Native.Flags.CloseOnExec);
            if (fd < 0)
            {
                throw Native.Failure(Marshal.GetLastWin32Error(), $"The directory {existing} cannot be opened");
            }
            try
            {
                Native.FileStatus current = Native.Stat(fd, $"The directory {existing}");
                while (true)
                {
                    if (current.Same(root))
                    {
                        return true;
                    }
                    int parent = UnixStateDirectory.OpenToExamine(fd, "..", noFollow: false);
                    if (parent < 0)
                    {
                        throw Native.Failure(Marshal.GetLastWin32Error(), $"A directory above {existing} cannot be opened");
                    }
                    Native.Close(fd);
                    fd = parent;
                    Native.FileStatus above = Native.Stat(fd, $"A directory above {existing}");
                    if (above.Same(current))
                    {
                        return false;
                    }
                    current = above;
                }
            }
            finally
            {
                Native.Close(fd);
            }
        }

        // The owner status has, as a test may have it observed (ObservedOwnerFaults).
        internal static uint OwnerOf(Native.FileStatus status)
        {
            Func<Native.FileStatus, uint, uint> fault = ObservedOwnerFaults.Value;
            return fault == null ? status.Uid : fault(status, status.Uid);
        }
    }

    // A directory's refusal to be relied on for checkpoints, or a checkpoint's (CheckpointStorage). Reason says why: what
    // another user could change, or, when Whole, everything a message needs to say.
    internal sealed class UntrustedStateException : IOException
    {
        internal UntrustedStateException(string reason, bool whole = false)
            : base(reason)
        {
            Reason = reason;
            Whole = whole;
        }

        internal string Reason { get; }

        internal bool Whole { get; }
    }

    // A directory holding checkpoints, open until disposed.
    internal abstract class StateDirectory : IDisposable
    {
        // The directory, for messages.
        internal abstract string Path { get; }

        // The contents of the checkpoint called name, or null when there is none.
        internal abstract byte[] Read(string name);

        // Replaces the checkpoint called name with contents, the new file flushed to storage before it is renamed over the
        // old one. A failure before the rename leaves the old checkpoint.
        internal abstract void Replace(string name, byte[] contents);

        // Flushes the directory's entries, such as a checkpoint just replaced, to storage (Durable). Throws when the
        // flush fails.
        internal abstract Durable.DirectoryFlush Flush();

        // Removes the checkpoint called name, if it can.
        internal abstract void Delete(string name);

        public abstract void Dispose();
    }

    // Checkpoints in a directory held by descriptor and checked as CheckpointStorage describes.
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.UnsupportedOSPlatform("windows")]
#endif
    internal sealed class UnixStateDirectory : StateDirectory
    {
        private const int OthersWrite = 0x12;
        private const int Sticky = 0x200;
        private const int PrivateFileMode = 0x180;

        private readonly int fd;
        private readonly string path;
        private readonly uint user;
        private int closed;

        private UnixStateDirectory(int fd, string path, uint user)
        {
            this.fd = fd;
            this.path = path;
            this.user = user;
        }

        internal override string Path => path;

        internal static UnixStateDirectory Open(string full)
        {
            string real = Native.RealPath(full);
            uint user = Native.GetEuid();
            int current = Native.Open("/", Native.Flags.ReadOnly | Native.Flags.Directory | Native.Flags.CloseOnExec);
            if (current < 0)
            {
                throw Native.Failure(Marshal.GetLastWin32Error(), "The directory / cannot be opened");
            }
            try
            {
                string currentPath = "/";
                Native.FileStatus status = Native.Stat(current, "The directory /");
                foreach (string name in real.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string childPath = currentPath.TrimEnd('/') + "/" + name;
                    int child = OpenToExamine(current, name, noFollow: true);
                    if (child < 0)
                    {
                        int errno = Marshal.GetLastWin32Error();
                        if (errno == Native.ENOENT)
                        {
                            throw new DirectoryNotFoundException($"The directory {childPath} does not exist.");
                        }
                        if (errno == Native.Flags.Loop || errno == Native.ENOTDIR)
                        {
                            throw new UntrustedStateException($"{childPath} was replaced by something other than a directory while it was checked");
                        }
                        throw Native.Failure(errno, $"The directory {childPath} cannot be opened");
                    }
                    Native.FileStatus childStatus;
                    try
                    {
                        childStatus = Native.Stat(child, $"The directory {childPath}");
                        CheckHolds(current, status, currentPath, CheckpointStorage.OwnerOf(childStatus), user);
                    }
                    catch
                    {
                        Native.Close(child);
                        throw;
                    }
                    Native.Close(current);
                    current = child;
                    status = childStatus;
                    currentPath = childPath;
                }
                // The checkpoint directory itself: a checkpoint in it is checked to be this user's before it is read.
                CheckHolds(current, status, currentPath, user, user);
                return new UnixStateDirectory(current, currentPath, user);
            }
            catch
            {
                Native.Close(current);
                throw;
            }
        }

        // A descriptor of the directory name in the directory held by parent, to examine it and open names in it: opened
        // to read, or on Linux, where this user may only search it, only to name it (O_PATH), which serves both there.
        // A symbolic link is not followed when noFollow. -1, with errno set, when it cannot be opened.
        internal static int OpenToExamine(int parent, string name, bool noFollow)
        {
            int flags = Native.Flags.Directory | Native.Flags.CloseOnExec | (noFollow ? Native.Flags.NoFollow : 0);
            int fd = Native.OpenAt(parent, name, Native.Flags.ReadOnly | flags);
            if (fd < 0 && !Native.Flags.Darwin && Marshal.GetLastWin32Error() == Native.EACCES)
            {
                fd = Native.OpenAt(parent, name, Native.Flags.PathOnly | flags);
            }
            return fd;
        }

        // Checks that no other user can change the entry, belonging to entryOwner, that the directory held by directoryFd,
        // with status, has on the path: see CheckpointStorage.
        private static void CheckHolds(int directoryFd, Native.FileStatus status, string directoryPath, uint entryOwner, uint user)
        {
            uint owner = CheckpointStorage.OwnerOf(status);
            if (owner != user && owner != 0)
            {
                throw new UntrustedStateException($"the directory {directoryPath} belongs to another user");
            }
            if ((status.Mode & OthersWrite) != 0 && ((status.Mode & Sticky) == 0 || (entryOwner != user && entryOwner != 0)))
            {
                throw new UntrustedStateException($"the directory {directoryPath} can be written by other users");
            }
            if (Native.AllowsByAccessControl(directoryFd, $"The directory {directoryPath}"))
            {
                throw new UntrustedStateException($"the directory {directoryPath} has an access control list that allows other users access");
            }
        }

        internal override byte[] Read(string name)
        {
            LocalDirectory.CheckName(name);
            string file = path.TrimEnd('/') + "/" + name;
            int fileFd = Native.OpenAt(fd, name, Native.Flags.ReadOnly | Native.Flags.NoFollow | Native.Flags.NonBlock | Native.Flags.CloseOnExec);
            if (fileFd < 0)
            {
                int errno = Marshal.GetLastWin32Error();
                if (errno == Native.ENOENT)
                {
                    return null;
                }
                if (errno == Native.Flags.Loop)
                {
                    throw new UntrustedStateException($"{file} is a symbolic link");
                }
                throw Native.Failure(errno, $"The checkpoint {file} cannot be opened");
            }
            var handle = new SafeFileHandle((IntPtr)fileFd, ownsHandle: true);
            try
            {
                Native.FileStatus status = Native.Stat(handle, $"The checkpoint {file}");
                if (!status.IsRegular)
                {
                    throw new UntrustedStateException($"{file} is not a regular file");
                }
                if (CheckpointStorage.OwnerOf(status) != user)
                {
                    throw new UntrustedStateException($"{file} belongs to another user");
                }
                if (status.Links != 1)
                {
                    throw new UntrustedStateException($"{file} has another name too");
                }
                if ((status.Mode & OthersWrite) != 0)
                {
                    throw new UntrustedStateException($"{file} can be written by other users");
                }
                if (Native.AllowsByAccessControl(handle, $"The checkpoint {file}"))
                {
                    throw new UntrustedStateException($"{file} has an access control list that allows other users access");
                }
                using (var stream = new FileStream(handle, FileAccess.Read, 1))
                {
                    handle = null;
                    var contents = new MemoryStream();
                    stream.CopyTo(contents);
                    return contents.ToArray();
                }
            }
            finally
            {
                handle?.Dispose();
            }
        }

        internal override void Replace(string name, byte[] contents)
        {
            LocalDirectory.CheckName(name);
            string file = path.TrimEnd('/') + "/" + name;
            // mkstemp goes by path: the file counts only as the very file at that name in the directory held.
            byte[] template = Encoding.UTF8.GetBytes(path.TrimEnd('/') + "/." + name + ".XXXXXX\0");
            int temporaryFd = Native.MkStemp(template);
            if (temporaryFd < 0)
            {
                throw Native.Failure(Marshal.GetLastWin32Error(), $"A new checkpoint cannot be created in {path}");
            }
            string temporary = System.IO.Path.GetFileName(Encoding.UTF8.GetString(template, 0, template.Length - 1));
            var handle = new SafeFileHandle((IntPtr)temporaryFd, ownsHandle: true);
            Native.FileStatus created = null;
            try
            {
                created = Native.Stat(handle, $"The new checkpoint {temporary} in {path}");
                Native.FileStatus entry = Native.TryStatAt(fd, temporary);
                if (entry == null || !entry.Same(created) || !created.IsRegular || created.Links != 1)
                {
                    created = null;
                    throw new UntrustedStateException($"the directory {path} changed while a checkpoint was written in it");
                }
                if (Native.FChmod(handle, PrivateFileMode) != 0)
                {
                    throw Native.Failure(Marshal.GetLastWin32Error(), $"The new checkpoint {temporary} in {path} cannot be made private");
                }
                Native.RemoveAccessControl(handle, $"The new checkpoint {temporary} in {path}");
                using (var stream = new FileStream(handle, FileAccess.Write, 1))
                {
                    stream.Write(contents, 0, contents.Length);
                    stream.Flush();
                    Native.SyncFile(handle, $"The checkpoint {file}");
                }
                if (Native.RenameAt(fd, temporary, fd, name) != 0)
                {
                    throw Native.Failure(Marshal.GetLastWin32Error(), $"The checkpoint {file} cannot be replaced");
                }
                created = null;
            }
            finally
            {
                handle.Dispose();
                if (created != null)
                {
                    Native.FileStatus left = Native.TryStatAt(fd, temporary);
                    if (left != null && left.Same(created))
                    {
                        Native.UnlinkAt(fd, temporary, 0);
                    }
                }
            }
        }

        internal override Durable.DirectoryFlush Flush()
        {
            return Native.SyncDirectory(fd, $"The directory {path}") ? Durable.DirectoryFlush.Flushed : Durable.DirectoryFlush.Unsupported;
        }

        internal override void Delete(string name)
        {
            LocalDirectory.CheckName(name);
            if (Native.UnlinkAt(fd, name, 0) != 0 && Marshal.GetLastWin32Error() != Native.ENOENT)
            {
                throw Native.Failure(Marshal.GetLastWin32Error(), $"The checkpoint {path.TrimEnd('/')}/{name} cannot be removed");
            }
        }

        public override void Dispose()
        {
            if (Interlocked.Exchange(ref closed, 1) == 0)
            {
                Native.Close(fd);
            }
        }
    }
}