using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace FilesCom.Transfers
{
    // The Windows calls WindowsDirectory makes to hold directories and stages and put stages in place. A stage is
    // opened, never shared, with the right to delete it, so it is renamed and deleted through its own handle, and no one
    // else can open, rename, replace or delete it while it is open. Every policy looks at the destination's entry
    // without following a reparse point, and renames the stage into the held destination directory through the
    // stage's handle (FILE_RENAME_INFORMATION with the directory as its RootDirectory, given to NtSetInformationFile:
    // SetFileInformationByHandle refuses a name relative to a RootDirectory with ERROR_INVALID_PARAMETER): without
    // replacing anything for CreateNew, which the system refuses with STATUS_OBJECT_NAME_COLLISION
    // (ERROR_ALREADY_EXISTS); replacing a regular file for ReplaceRegularFile; or replacing a regular file or a reparse
    // point, as an entry, never followed, for ReplaceExisting. A replaced entry first gives the stage its access control
    // list through the stage's handle, as PowerShell does. A folder at the name is never replaced, nor is a reparse
    // point for ReplaceRegularFile.
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    internal static class WindowsPublication
    {
        private const uint GenericRead = 0x80000000;
        private const uint GenericWrite = 0x40000000;
        private const uint Delete = 0x00010000;
        private const uint ReadControl = 0x00020000;
        private const uint WriteDac = 0x00040000;
        private const uint Synchronize = 0x00100000;
        private const uint ReadAttributes = 0x00000080;
        private const uint ShareRead = 1;
        private const uint ShareWrite = 2;
        private const uint ShareDelete = 4;
        private const uint OpenExisting = 3;
        private const uint BackupSemantics = 0x02000000;
        private const uint OpenReparsePoint = 0x00200000;
        private const uint DirectoryAttribute = 0x10;
        private const uint ReparsePointAttribute = 0x400;
        private const uint HiddenAttribute = 0x2;
        private const uint NormalAttribute = 0x80;
        private const int FileBasicInfo = 0;
        private const int FileDispositionInfo = 4;
        private const int FileRenameInformation = 10;
        private const int FileObject = 1;
        private const uint OwnerInformation = 1;
        private const uint GroupInformation = 2;
        private const uint DaclInformation = 4;
        private const uint ProtectedDacl = 0x80000000;
        private const int NotFound = 2;
        private const int PathNotFound = 3;
        private const int AccessDenied = 5;
        private const int SharingViolation = 32;
        private const int FileExists = 80;
        private const int AlreadyExists = 183;

        // What a stage is opened with, all of it granted by the stage's own access control list: to read and write it,
        // to rename and delete it, and to read and set its access control list.
        private const uint StageRights = GenericRead | GenericWrite | Delete | ReadControl | WriteDac | ReadAttributes;

        // A new stage at path, created with an access control list of its own, protected from inheritance, that allows
        // only this user, SYSTEM and Administrators (WindowsNative.CreatePrivateStage), held open shared with no one.
        internal static SafeFileHandle CreatePrivateStage(string path, string shown)
        {
            return WindowsNative.CreatePrivateStage(path, shown, StageRights, WindowsNative.CurrentAccounts());
        }

        // The stage an earlier run kept at path, held open as a new one is, shared with no one; null when it is gone, or
        // cannot be opened so, as when it is open elsewhere or its list does not grant those rights, in which case the
        // download starts over. A reparse point or directory is refused.
        internal static SafeFileHandle OpenStage(string path, string shown)
        {
            SafeFileHandle handle = CreateFileW(path, StageRights, 0, IntPtr.Zero, OpenExisting, OpenReparsePoint | WindowsNative.StageIo, IntPtr.Zero);
            if (handle.IsInvalid)
            {
                int error = Marshal.GetLastWin32Error();
                handle.Dispose();
                if (error == NotFound || error == PathNotFound || error == AccessDenied || error == SharingViolation)
                {
                    return null;
                }
                throw Failure(error, $"The file being downloaded, {shown}, cannot be opened");
            }
            try
            {
                if ((WindowsNative.InformationOf(handle, $"The file being downloaded, {shown},").Attributes & (ReparsePointAttribute | DirectoryAttribute)) != 0)
                {
                    throw new IOException($"The file being downloaded, {shown}, was replaced by a link or directory, so nothing was written.");
                }
                return handle;
            }
            catch
            {
                handle.Dispose();
                throw;
            }
        }

        // What the stage's own access control list reads back as (WindowsSecurity.StageListFrom), and whether that shows
        // the stage private (WindowsSecurity.ShownPrivate). A list that cannot be read back is not shown private, whatever
        // the volume; the volume is asked only then, as a hint that it keeps no lists at all.
        internal static WindowsSecurity.StageList ReadStageList(SafeFileHandle handle, string what, out bool shownPrivate)
        {
            shownPrivate = false;
            WindowsNative.Security security;
            try
            {
                security = WindowsNative.SecurityOf(handle, what);
            }
            catch (IOException)
            {
                return WindowsSecurity.StageListFrom(read: false, hasList: false, WindowsNative.KeepsAccessControl(handle));
            }
            WindowsSecurity.StageList list = WindowsSecurity.StageListFrom(read: true, hasList: security.Dacl != null, volumeKeepsLists: null);
            try
            {
                shownPrivate = WindowsSecurity.ShownPrivate(list, security.Owner, security.Dacl, WindowsNative.CurrentAccounts());
            }
            catch (IOException)
            {
                // This user cannot be read: nothing is shown private.
            }
            return list;
        }

        // Whether the stage handle holds is shown private (ReadStageList).
        internal static bool IsPrivate(SafeFileHandle handle, string what)
        {
            ReadStageList(handle, what, out bool shownPrivate);
            return shownPrivate;
        }

        // Whether the file handle holds has no other name.
        internal static bool OneName(SafeFileHandle handle, string what)
        {
            return WindowsNative.InformationOf(handle, what).Links == 1;
        }

        // Makes the stage private again: its own list, protected, allowing only this user, SYSTEM and Administrators.
        internal static void MakePrivate(SafeFileHandle stage, string what)
        {
            IntPtr descriptor = WindowsNative.PrivateDescriptor(inheritable: false, WindowsNative.CurrentAccounts(), withOwner: false);
            try
            {
                if (!GetSecurityDescriptorDacl(descriptor, out bool present, out IntPtr dacl, out bool _) || !present)
                {
                    throw Failure(Marshal.GetLastWin32Error(), $"{what} cannot be made private");
                }
                int result = SetSecurityInfo(stage, FileObject, DaclInformation | ProtectedDacl, IntPtr.Zero, IntPtr.Zero, dacl, IntPtr.Zero);
                if (result != 0)
                {
                    throw Failure(result, $"{what} cannot be made private");
                }
            }
            finally
            {
                LocalFree(descriptor);
            }
        }

        // Gives the stage the access control list a file created directly in the destination directory gets, destination
        // being the handle the stage's staging directory was created through, so the very directory the file is put in,
        // wherever the caller's path led. First as the file system itself gives it: an empty file is created there, of this
        // SDK's own and holding nothing (WindowsNative.CreateAccessProbe), and its list is read back, which needs no right
        // to read the directory's own descriptor, only this user's rights on a file it creates there. Where a file created
        // there would not let this user read its list or delete it, the list is worked out from the directory's own
        // descriptor instead, as the file system works it out (WindowsNative.WithNewFileDescriptor), when this user may
        // read it. Either way it is what the directory passes on to its files, entries that reach only its own children
        // included; else the default list of this user's token, even an empty one; else no list. It is written to the stage
        // as it is (GiveList). Refused, before anything is put in place, when neither way can learn it or it cannot be given.
        internal static void GiveNewFileAccess(SafeFileHandle stage, SafeFileHandle destination, string shown)
        {
            string file = $"the file downloaded to {shown}";
            try
            {
                using (SafeFileHandle probe = WindowsNative.CreateAccessProbe(destination, shown))
                {
                    if (probe != null)
                    {
                        WindowsNative.WithSecurityDescriptor(probe, DaclInformation, $"a new file in the directory of {shown}", created => GiveList(stage, created, file));
                        return;
                    }
                }
                WindowsNative.WithSecurityDescriptor(destination, OwnerInformation | GroupInformation | DaclInformation, $"the directory of {shown}", destinationDescriptor =>
                    WindowsNative.WithNewFileDescriptor(destinationDescriptor, $"the directory of {shown}", created => GiveList(stage, created, file)));
            }
            catch (IOException e) when (!(e is DownloadPublicationException))
            {
                throw NotGiven(shown, e.Message);
            }
        }

        // Gives the stage the DACL of descriptor exactly: its entries, their flags included, no list for none, and its
        // protection and auto-inheritance. The stage's own list is protected first, when it is not already (a stage is
        // created with a protected one), so that the list is taken as given rather than merged with entries the stage
        // inherited; it is then written with that control asked for (WindowsNative.SetFileDacl) and read back through the
        // stage's handle, as the list given was read from its own file. Unless it reads back as the list given, with the
        // same protection and auto-inheritance (WindowsSecurity.SameList), it did not reach this stage as given, and
        // nothing is put in place.
        private static void GiveList(SafeFileHandle stage, IntPtr descriptor, string what)
        {
            if (!GetSecurityDescriptorDacl(descriptor, out bool present, out IntPtr dacl, out bool _))
            {
                throw Failure(Marshal.GetLastWin32Error(), $"The list for {what} cannot be read");
            }
            if (!present)
            {
                dacl = IntPtr.Zero;
            }
            byte[] given = null;
            if (dacl != IntPtr.Zero)
            {
                // ACL: AclSize is the ushort at offset 2.
                given = new byte[(ushort)Marshal.ReadInt16(dacl, 2)];
                Marshal.Copy(dacl, given, 0, given.Length);
            }
            ushort control = WindowsNative.ControlOf(descriptor, $"The list for {what}");
            if ((WindowsNative.SecurityOf(stage, what).Control & WindowsSecurity.DaclProtected) == 0)
            {
                MakePrivate(stage, what);
            }
            WindowsNative.SetFileDacl(stage, dacl, control, what);
            WindowsNative.Security now = WindowsNative.SecurityOf(stage, what);
            if (!WindowsSecurity.SameList(given, control, now.Dacl, now.Control))
            {
                throw new IOException($"The list given to {what} is not the one it now has, with its protection and inheritance.");
            }
        }

        // Removes the file at path, without following a reparse point, only while it is the file with identity.
        internal static void DeleteIfSame(string path, string identity)
        {
            if (identity == null)
            {
                return;
            }
            using (SafeFileHandle handle = CreateFileW(path, Delete | ReadAttributes, ShareRead | ShareWrite | ShareDelete, IntPtr.Zero, OpenExisting, OpenReparsePoint, IntPtr.Zero))
            {
                if (!handle.IsInvalid && IdentityOf(handle, path) == identity)
                {
                    DeleteOnClose(handle);
                }
            }
        }

        // The directory at path, opened without following a reparse point and held so that no one can rename, move or
        // remove it (no FILE_SHARE_DELETE). One that is a name surrogate (link, junction, mount point) is refused; another
        // kind of reparse point, such as a folder kept in the cloud, is the directory itself.
        internal static SafeFileHandle HoldDirectory(string path, string shown)
        {
            SafeFileHandle handle = CreateFileW(path, ReadAttributes | Synchronize, ShareRead | ShareWrite, IntPtr.Zero, OpenExisting, BackupSemantics | OpenReparsePoint, IntPtr.Zero);
            if (handle.IsInvalid)
            {
                int error = Marshal.GetLastWin32Error();
                handle.Dispose();
                if (error == NotFound || error == PathNotFound)
                {
                    throw new DirectoryNotFoundException($"The directory {shown} does not exist.");
                }
                throw Failure(error, $"The directory {shown} cannot be opened");
            }
            try
            {
                if ((WindowsNative.InformationOf(handle, $"The directory {shown}").Attributes & DirectoryAttribute) == 0)
                {
                    throw new IOException($"{shown} is not a directory.");
                }
                if (WindowsNative.NamesAnotherPath(handle, $"The directory {shown}"))
                {
                    throw new IOException($"{shown} is a symbolic link or other reparse point, which transfers do not follow.");
                }
                return handle;
            }
            catch
            {
                handle.Dispose();
                throw;
            }
        }

        // Creates the directory at path, if missing, inheriting from its parent.
        internal static void CreateDirectory(string path, string shown)
        {
            if (!CreateDirectoryW(path, IntPtr.Zero))
            {
                int error = Marshal.GetLastWin32Error();
                if (error != AlreadyExists)
                {
                    throw Failure(error, $"The directory {shown} cannot be created");
                }
            }
        }

        // Creates the staging directory name in the directory destination holds, and holds it, as this download's, through
        // the handle that created it (WindowsNative.CreateDirectoryHeld); fails when anything is at that name.
        internal static SafeFileHandle CreateStagingDirectory(SafeFileHandle parent, string name, string shown)
        {
            return WindowsNative.CreateDirectoryHeld(parent, name, shown, WindowsNative.CurrentAccounts());
        }

        // A staging directory at path, opened without following a reparse point, held without delete sharing, with the
        // right to delete it where that is granted (without it, it is left behind when done, as a directory this user may
        // not delete is); null when nothing, a link or something other than a directory is there.
        internal static SafeFileHandle OpenStagingDirectory(string path, string shown)
        {
            // With the right to delete it where that is granted, else without.
            SafeFileHandle handle = null;
            foreach (uint access in new[] { Delete, 0u })
            {
                handle?.Dispose();
                handle = CreateFileW(path, access | ReadAttributes | Synchronize, ShareRead | ShareWrite, IntPtr.Zero, OpenExisting, BackupSemantics | OpenReparsePoint, IntPtr.Zero);
                if (!handle.IsInvalid || Marshal.GetLastWin32Error() != AccessDenied)
                {
                    break;
                }
            }
            if (handle.IsInvalid)
            {
                int error = Marshal.GetLastWin32Error();
                handle.Dispose();
                if (error == NotFound || error == PathNotFound)
                {
                    return null;
                }
                throw Failure(error, $"The staging directory {shown} cannot be opened");
            }
            try
            {
                if ((WindowsNative.InformationOf(handle, $"The staging directory {shown}").Attributes & DirectoryAttribute) == 0 || WindowsNative.NamesAnotherPath(handle, $"The staging directory {shown}"))
                {
                    handle.Dispose();
                    return null;
                }
                return handle;
            }
            catch
            {
                handle.Dispose();
                throw;
            }
        }

        // The identity of the file handle holds (FileIdentity).
        internal static string IdentityOf(SafeFileHandle handle, string what)
        {
            WindowsNative.FileInformation information = WindowsNative.InformationOf(handle, what);
            return FileIdentity.Of(information.VolumeSerial, information.IndexHigh, information.IndexLow);
        }

        // Whether the entry at path, whose last component is name, is reached through a short (8.3) name of another
        // entry: NTFS keeps a generated short name such as QUARTE~1.XLS for Quarterly Report.xlsx, and a path naming
        // the short name opens, replaces or enters the long-named entry. A remote file or folder named like a short name
        // would then replace or enter a local entry of a different name. The entry's own name is asked for
        // (GetLongPathNameW) and compared with the name given: false when nothing is at path, or when the names agree
        // apart from case.
        internal static bool IsAnotherEntrysShortName(string path, string name)
        {
            var buffer = new System.Text.StringBuilder(path.Length + 260);
            uint length = GetLongPathNameW(path, buffer, (uint)buffer.Capacity);
            if (length == 0)
            {
                return false;
            }
            if (length > buffer.Capacity)
            {
                buffer.EnsureCapacity((int)length);
                length = GetLongPathNameW(path, buffer, (uint)buffer.Capacity);
                if (length == 0)
                {
                    return false;
                }
            }
            string resolved = buffer.ToString();
            int separator = resolved.LastIndexOf('\\');
            string actual = separator < 0 ? resolved : resolved.Substring(separator + 1);
            return !string.Equals(actual, name, StringComparison.OrdinalIgnoreCase);
        }

        // What is at path, without following a reparse point, or null when nothing is.
        internal static LocalEntry Examine(string path)
        {
            using (SafeFileHandle handle = CreateFileW(path, ReadAttributes, ShareRead | ShareWrite | ShareDelete, IntPtr.Zero, OpenExisting, BackupSemantics | OpenReparsePoint, IntPtr.Zero))
            {
                if (handle.IsInvalid)
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error == NotFound || error == PathNotFound)
                    {
                        return null;
                    }
                    throw Failure(error, $"{path} cannot be examined");
                }
                WindowsNative.FileInformation information = WindowsNative.InformationOf(handle, path);
                bool regular = (information.Attributes & (ReparsePointAttribute | DirectoryAttribute)) == 0;
                long size = ((long)information.SizeHigh << 32) | information.SizeLow;
                return new LocalEntry(regular, FileIdentity.Of(information.VolumeSerial, information.IndexHigh, information.IndexLow), size);
            }
        }

        // Puts the stage held by stage in place as finalName in the held directory at where (shown as directory), as
        // policy says. destinationDirectory is the handle the staging directory was created or reopened through; it
        // already grants FILE_ADD_FILE and stays open until the stage closes. The stage first takes its final access: a replaced entry's access control list, or, with nothing
        // there, newFileAccess, which gives it a new file's; accessTaken is set once it has either, which a stage kept for
        // a later run must not keep. Throws DownloadPublicationException when the policy refuses the destination, and
        // leaves the stage where it is.
        internal static void Publish(SafeFileHandle stage, SafeFileHandle destinationDirectory, string where, string directory, string finalName, DownloadPublicationPolicy policy, Action newFileAccess, ref bool accessTaken)
        {
            string destination = Path.Combine(directory, finalName);
            string target = where.TrimEnd('\\') + "\\" + finalName;
            bool hidden = false;
            using (SafeFileHandle existing = CreateFileW(target, ReadControl | ReadAttributes, ShareRead | ShareWrite | ShareDelete, IntPtr.Zero, OpenExisting, BackupSemantics | OpenReparsePoint, IntPtr.Zero))
            {
                if (!existing.IsInvalid)
                {
                    uint attributes = WindowsNative.InformationOf(existing, destination).Attributes;
                    if (policy == DownloadPublicationPolicy.CreateNew)
                    {
                        throw Exists(destination);
                    }
                    if ((attributes & DirectoryAttribute) != 0)
                    {
                        throw new DownloadPublicationException(DownloadPublicationProblem.DestinationNotRegularFile, destination, $"{destination} is a directory, which is not replaced, so nothing was replaced.");
                    }
                    if ((attributes & ReparsePointAttribute) != 0 && policy == DownloadPublicationPolicy.ReplaceRegularFile)
                    {
                        throw new DownloadPublicationException(DownloadPublicationProblem.DestinationNotRegularFile, destination, $"{destination} is a reparse point, which is not replaced, so nothing was replaced.");
                    }
                    hidden = (attributes & HiddenAttribute) != 0;
                    accessTaken = true;
                    TakeOverAccessRules(existing, stage, destination);
                }
                else
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error != NotFound && error != PathNotFound)
                    {
                        throw Failure(error, $"{destination} cannot be examined");
                    }
                    if (newFileAccess != null)
                    {
                        accessTaken = true;
                        newFileAccess();
                    }
                }
            }
            if (hidden)
            {
                // A file that replaces a hidden one stays hidden, as PowerShell's replacement does.
                SetAttributes(stage, HiddenAttribute, destination);
            }
            byte[] rename = RenameInformation(destinationDirectory, finalName, replace: policy != DownloadPublicationPolicy.CreateNew);
            int status = NtSetInformationFile(stage, out IoStatusBlock _, rename, (uint)rename.Length, FileRenameInformation);
            GC.KeepAlive(destinationDirectory);
            if (status < 0)
            {
                int error = RtlNtStatusToDosError(status);
                if (error == FileExists || error == AlreadyExists)
                {
                    throw Exists(destination);
                }
                throw Failure(error, $"The file downloaded to {destination} cannot be put in place");
            }
        }

        // Deletes the file handle holds when the handle closes.
        internal static void DeleteOnClose(SafeFileHandle handle)
        {
            SetFileInformationByHandle(handle, FileDispositionInfo, new byte[] { 1 }, 1);
        }

        private static DownloadPublicationException Exists(string destination)
        {
            return new DownloadPublicationException(DownloadPublicationProblem.DestinationExists, destination, $"{destination} exists, and is left as it is: nothing was replaced.");
        }

        // Gives the stage the access control list of the file it replaces, read through a handle of that file that does
        // not follow a reparse point, and written to the stage as it is, with its control (GiveList): a protected list stays
        // protected, and an inherited one keeps the entries the replaced file inherited from its directory, rather than
        // being worked out again from the staging directory. A protected list whose entries are still flagged inherited,
        // which Windows' rules clear when such a list is set, does not read back as given, and that file is not replaced.
        private static void TakeOverAccessRules(SafeFileHandle replaced, SafeFileHandle stage, string destination)
        {
            try
            {
                WindowsNative.WithSecurityDescriptor(replaced, DaclInformation, destination, descriptor =>
                    GiveList(stage, descriptor, $"the file replacing {destination}"));
            }
            catch (IOException e) when (!(e is DownloadPublicationException))
            {
                throw NotKept(destination, e.Message);
            }
        }

        private static DownloadPublicationException NotGiven(string destination, string reason)
        {
            return new DownloadPublicationException(DownloadPublicationProblem.AccessNotKept, destination, $"The file downloaded to {destination} could not be given the access a new file there gets: {reason}. Nothing was put in place.");
        }

        private static DownloadPublicationException NotKept(string destination, string reason)
        {
            return new DownloadPublicationException(DownloadPublicationProblem.AccessNotKept, destination, $"The access rules of {destination} could not be given to the file replacing it: {reason}. Nothing was replaced; the file is as it was.");
        }

        // Adds attributes to the file handle holds. FILE_BASIC_INFO: four times, left as zero so they do not change,
        // then the attributes.
        private static void SetAttributes(SafeFileHandle handle, uint add, string destination)
        {
            var information = new byte[40];
            if (!GetFileInformationByHandleEx(handle, FileBasicInfo, information, (uint)information.Length))
            {
                throw Failure(Marshal.GetLastWin32Error(), $"The attributes of the file downloaded to {destination} cannot be read");
            }
            uint attributes = (BitConverter.ToUInt32(information, 32) & ~NormalAttribute) | add;
            var change = new byte[40];
            Array.Copy(BitConverter.GetBytes(attributes), 0, change, 32, 4);
            if (!SetFileInformationByHandle(handle, FileBasicInfo, change, (uint)change.Length))
            {
                throw Failure(Marshal.GetLastWin32Error(), $"The attributes of the file downloaded to {destination} cannot be set");
            }
        }

        // FILE_RENAME_INFORMATION naming the destination relative to the held directory: ReplaceIfExists, padded to
        // pointer size, RootDirectory, FileNameLength in bytes, then the name.
        private static byte[] RenameInformation(SafeFileHandle directory, string name, bool replace)
        {
            byte[] encoded = Encoding.Unicode.GetBytes(name);
            int handleOffset = IntPtr.Size;
            int lengthOffset = handleOffset + IntPtr.Size;
            int nameOffset = lengthOffset + 4;
            var buffer = new byte[nameOffset + encoded.Length + 2];
            buffer[0] = replace ? (byte)1 : (byte)0;
            IntPtr root = directory.DangerousGetHandle();
            byte[] rootBytes = IntPtr.Size == 8 ? BitConverter.GetBytes(root.ToInt64()) : BitConverter.GetBytes(root.ToInt32());
            Array.Copy(rootBytes, 0, buffer, handleOffset, rootBytes.Length);
            Array.Copy(BitConverter.GetBytes(encoded.Length), 0, buffer, lengthOffset, 4);
            Array.Copy(encoded, 0, buffer, nameOffset, encoded.Length);
            return buffer;
        }

        private static IOException Failure(int error, string what)
        {
            return new IOException($"{what}: {new Win32Exception(error).Message}");
        }

        // IO_STATUS_BLOCK.
        [StructLayout(LayoutKind.Sequential)]
        private struct IoStatusBlock
        {
            internal IntPtr Status;
            internal IntPtr Information;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern SafeFileHandle CreateFileW(string name, uint access, uint share, IntPtr security, uint creation, uint flags, IntPtr template);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern uint GetLongPathNameW(string shortPath, System.Text.StringBuilder longPath, uint bufferLength);

        [DllImport("ntdll.dll")]
        private static extern int NtSetInformationFile(SafeFileHandle handle, out IoStatusBlock status, byte[] information, uint length, int informationClass);

        [DllImport("ntdll.dll")]
        private static extern int RtlNtStatusToDosError(int status);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetFileInformationByHandle(SafeFileHandle handle, int informationClass, byte[] information, uint size);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetFileInformationByHandleEx(SafeFileHandle handle, int informationClass, byte[] information, uint size);

        [DllImport("advapi32.dll")]
        private static extern int SetSecurityInfo(SafeFileHandle handle, int objectType, uint securityInformation, IntPtr owner, IntPtr group, IntPtr dacl, IntPtr sacl);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetSecurityDescriptorDacl(IntPtr securityDescriptor, [MarshalAs(UnmanagedType.Bool)] out bool present, out IntPtr dacl, [MarshalAs(UnmanagedType.Bool)] out bool defaulted);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CreateDirectoryW(string name, IntPtr security);

        [DllImport("kernel32.dll")]
        private static extern IntPtr LocalFree(IntPtr memory);
    }
}