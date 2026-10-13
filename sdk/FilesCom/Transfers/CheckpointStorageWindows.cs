using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace FilesCom.Transfers
{
    // Checkpoints on Windows, trusted before anything read from them is relied on, as CheckpointStorage describes, with
    // Windows' own means:
    //
    //   - The directory's path is resolved once (GetFinalPathNameByHandle), following the links, junctions and mount
    //     points the caller's path goes through. Then every directory on the resolved path, from its volume's or share's
    //     root, is opened without following a reparse point that names another path (a symbolic link, junction or mount
    //     point: a name surrogate), which is refused, and held open, without sharing the right to delete it, until the
    //     checkpoint has been read, written or removed. While they are held no one can rename, move or remove them, so the
    //     path names the directories checked.
    //   - The checkpoint directory must belong to this user, to SYSTEM or to the Administrators group, and its access
    //     control list must allow no one else to add, remove or rename entries in it, remove it, or change its access or
    //     owner (WindowsSecurity.ChangingRights). One without an access control list allows everyone everything, and is
    //     refused. Only entries that apply to the directory itself count: entries that only deny, and entries that only
    //     its children inherit, do not.
    //   - A checkpoint must be a regular file, not a reparse point that names another path, with one name, belonging to
    //     this user, to SYSTEM or to Administrators, whose access control list allows no one else to write it, remove it,
    //     or change its access or owner.
    //   - A checkpoint is written as a new file, created belonging to this user, with an access control list of its own,
    //     protected from inheritance, that allows only this user, SYSTEM and Administrators; flushed; and moved over the
    //     checkpoint (MOVEFILE_REPLACE_EXISTING, MOVEFILE_WRITE_THROUGH).
    //
    // This user is the user of the token the calling thread acts with: the one it impersonates, if it does, or the
    // process's. The owner the token gives new objects by default is not trusted as such: a token may name a group there,
    // which other users can be in; so the SDK names this user as the owner of what it creates. Whether Windows accepts
    // that owner for every token the SDK may run with (impersonation at each level, elevated or not, and threads that
    // act as different users) is what the platform's documentation leads to expect, not something tested here.
    //
    // The directories above the checkpoint directory need not be private. Holding them keeps them in place while the
    // checkpoint is used (it keeps their names, not their access control lists), and a replacement found by a later use
    // belongs to another user, and is refused. That someone who controls one of them cannot change who may use the
    // checkpoint directory or the checkpoint rests on this reading of Windows' documentation, not yet confirmed on
    // Windows with two users: access to an object is decided by that object's own security descriptor; inherited
    // entries are copies written into it when it is created, or when a program sets a parent's list and Windows
    // propagates it to the children, which rewrites each child's list; and rewriting a child's list takes the right to
    // change it (WRITE_DAC) or its ownership, which only this user, SYSTEM and Administrators have here. Whatever its
    // directory's list became meanwhile, the checkpoint's owner and list are checked again each time it is read. As
    // elsewhere, the guarantee is against other users; a program running as this user or as an administrator can do
    // anything this one can. The directory's entry is not flushed (Durable.DirectoryFlush.NotAttempted), as before.
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    internal sealed class WindowsStateDirectory : StateDirectory
    {
        private readonly List<SafeFileHandle> held;
        private readonly string path;
        private readonly TokenAccounts accounts;

        private WindowsStateDirectory(List<SafeFileHandle> held, string path, TokenAccounts accounts)
        {
            this.held = held;
            this.path = path;
            this.accounts = accounts;
        }

        // The directory, as resolved, without the \\?\ prefix, for messages.
        internal override string Path => WindowsSecurity.Shown(path);

        internal static WindowsStateDirectory Open(string full)
        {
            TokenAccounts accounts = WindowsNative.CurrentAccounts();
            string resolved = WindowsNative.FinalPathOf(full);
            string root = WindowsSecurity.Root(resolved, out List<string> names);
            var held = new List<SafeFileHandle>();
            try
            {
                string current = root;
                held.Add(WindowsNative.OpenDirectoryToHold(current, WindowsSecurity.Shown(current), examine: names.Count == 0));
                for (int i = 0; i < names.Count; i++)
                {
                    current = current.TrimEnd('\\') + "\\" + names[i];
                    held.Add(WindowsNative.OpenDirectoryToHold(current, WindowsSecurity.Shown(current), examine: i == names.Count - 1));
                }
                SafeFileHandle directory = held[held.Count - 1];
                string shown = WindowsSecurity.Shown(current);
                WindowsNative.Security security = WindowsNative.SecurityOf(directory, $"The directory {shown}");
                if (!WindowsSecurity.TrustedOwner(security.Owner, accounts))
                {
                    throw new UntrustedStateException($"the directory {shown} belongs to another user ({security.Owner})");
                }
                string grant = WindowsSecurity.UntrustedGrant(security.Dacl, accounts);
                if (grant != null)
                {
                    throw new UntrustedStateException($"the directory {shown} lets another user ({grant}) change what is in it");
                }
                return new WindowsStateDirectory(held, current, accounts);
            }
            catch
            {
                Close(held);
                throw;
            }
        }

        internal override byte[] Read(string name)
        {
            LocalDirectory.CheckName(name);
            string file = path.TrimEnd('\\') + "\\" + name;
            string shown = WindowsSecurity.Shown(file);
            SafeFileHandle handle = WindowsNative.OpenFileToRead(file, shown);
            if (handle == null)
            {
                return null;
            }
            try
            {
                WindowsNative.FileInformation information = WindowsNative.InformationOf(handle, $"The checkpoint {shown}");
                if ((information.Attributes & WindowsNative.DirectoryAttribute) != 0)
                {
                    throw new UntrustedStateException($"{shown} is not a regular file");
                }
                if (information.Links != 1)
                {
                    throw new UntrustedStateException($"{shown} has another name too");
                }
                WindowsNative.Security security = WindowsNative.SecurityOf(handle, $"The checkpoint {shown}");
                if (!WindowsSecurity.TrustedOwner(security.Owner, accounts))
                {
                    throw new UntrustedStateException($"{shown} belongs to another user ({security.Owner})");
                }
                string grant = WindowsSecurity.UntrustedGrant(security.Dacl, accounts);
                if (grant != null)
                {
                    throw new UntrustedStateException($"{shown} lets another user ({grant}) write it");
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
            string file = path.TrimEnd('\\') + "\\" + name;
            string temporary = path.TrimEnd('\\') + "\\." + name + "." + Guid.NewGuid().ToString("N") + ".tmp";
            bool created = false;
            try
            {
                using (SafeFileHandle handle = WindowsNative.CreatePrivateFile(temporary, WindowsSecurity.Shown(file), accounts))
                {
                    created = true;
                    using (var stream = new FileStream(handle, FileAccess.Write, 1))
                    {
                        stream.Write(contents, 0, contents.Length);
                        stream.Flush(true);
                    }
                }
                WindowsNative.MoveOver(temporary, file, WindowsSecurity.Shown(file));
                created = false;
            }
            finally
            {
                if (created)
                {
                    WindowsNative.DeleteFile(temporary);
                }
            }
        }

        // Windows has no directory flush here: the move itself is written through.
        internal override Durable.DirectoryFlush Flush()
        {
            return Durable.DirectoryFlush.NotAttempted;
        }

        internal override void Delete(string name)
        {
            LocalDirectory.CheckName(name);
            WindowsNative.DeleteFile(path.TrimEnd('\\') + "\\" + name);
        }

        public override void Dispose()
        {
            Close(held);
        }

        private static void Close(List<SafeFileHandle> handles)
        {
            for (int i = handles.Count - 1; i >= 0; i--)
            {
                handles[i].Dispose();
            }
            handles.Clear();
        }
    }

    // What decides whether Windows checkpoint storage can be trusted, from the security descriptors and paths the system
    // gives (WindowsStateDirectory): plain data, so that the rules are the same code wherever they are tested.
    internal static class WindowsSecurity
    {
        // Rights that let their holder change a directory's entries (FILE_ADD_FILE, FILE_ADD_SUBDIRECTORY,
        // FILE_DELETE_CHILD), write a file (FILE_WRITE_DATA and FILE_APPEND_DATA, the same bits), remove the object
        // (DELETE), change its access or owner (WRITE_DAC, WRITE_OWNER), or do any of these (GENERIC_ALL, GENERIC_WRITE).
        internal const uint ChangingRights = 0x2 | 0x4 | 0x40 | 0x10000 | 0x40000 | 0x80000 | 0x10000000 | 0x40000000;

        // Rights that let their holder change a file as ChangingRights do, or read its bytes (FILE_READ_DATA,
        // FILE_EXECUTE, GENERIC_READ, GENERIC_EXECUTE): what no one else may have on a private stage.
        internal const uint PrivateRights = ChangingRights | 0x1 | 0x20 | 0x80000000 | 0x20000000;

        internal const string LocalSystem = "S-1-5-18";
        internal const string Administrators = "S-1-5-32-544";

        // OWNER RIGHTS, which grants to whoever owns the object, and CREATOR OWNER, which only children inherit.
        private const string OwnerRights = "S-1-3-4";
        private const string CreatorOwner = "S-1-3-0";

        // What reading back the access control list of a stage created with a private one shows (StageListFrom).
        internal enum StageList
        {
            // It is in place: the file's access must be set as it is put in place.
            Kept,
            // The file system keeps none: read back as no list at all, or the volume says it keeps none.
            NotKept,
            // It could not be read, on a volume not shown to keep none: it may be in place.
            Unknown,
        }

        // What the stage's list is, from whether it could be read, whether a list was there, and what the volume says of
        // keeping lists, when it could say (null when its volume cannot be asked, as on some SMB shares). Only a volume
        // shown to keep none makes an unreadable list NotKept.
        internal static StageList StageListFrom(bool read, bool hasList, bool? volumeKeepsLists)
        {
            if (read)
            {
                return hasList ? StageList.Kept : StageList.NotKept;
            }
            return volumeKeepsLists == false ? StageList.NotKept : StageList.Unknown;
        }

        // Whether the file's access must be set as it is put in place: whenever the private list may be in place, so that
        // a list that could not be read back is never published as the file's.
        internal static bool NeedsFinalAccess(StageList list)
        {
            return list != StageList.NotKept;
        }

        // Whether a stage is shown to be private, and so may be kept for a later run: its list was read back, is there,
        // belongs to a trusted owner and lets no one else read or change it. Nothing unread is shown private.
        internal static bool ShownPrivate(StageList list, string owner, byte[] acl, TokenAccounts accounts)
        {
            return list == StageList.Kept && TrustedOwner(owner, accounts) && UntrustedGrant(acl, accounts, PrivateRights) == null;
        }

        // Who may own a checkpoint directory or a checkpoint: this user, SYSTEM or Administrators.
        internal static bool TrustedOwner(string owner, TokenAccounts accounts)
        {
            return owner != null && (Same(owner, accounts.User) || Same(owner, LocalSystem) || Same(owner, Administrators));
        }

        // The first account acl, an ACL as the system keeps it, gives any of rights (by default ChangingRights, which let
        // it change the object) that is not this user, SYSTEM, Administrators or the object's owner itself (OWNER RIGHTS,
        // CREATOR OWNER); null when there is none. A null acl, no access control list at all, allows everyone
        // everything. Entries that deny, and entries that only children inherit, are passed over; an allowing entry of a
        // kind this does not know is refused.
        internal static string UntrustedGrant(byte[] acl, TokenAccounts accounts, uint rights = ChangingRights)
        {
            if (acl == null)
            {
                return "everyone, since it has no access control list";
            }
            // ACL: AclRevision and Sbz1 (bytes), AclSize, AceCount and Sbz2 (ushorts); then the entries, each starting
            // with ACE_HEADER: AceType and AceFlags (bytes) and AceSize (ushort), followed by its ACCESS_MASK.
            if (acl.Length < 8)
            {
                throw new InvalidDataException("The access control list is too short.");
            }
            int count = BitConverter.ToUInt16(acl, 4);
            int offset = 8;
            for (int i = 0; i < count; i++)
            {
                if (offset + 8 > acl.Length)
                {
                    throw new InvalidDataException("The access control list ends inside an entry.");
                }
                byte type = acl[offset];
                byte flags = acl[offset + 1];
                int size = BitConverter.ToUInt16(acl, offset + 2);
                if (size < 8 || offset + size > acl.Length)
                {
                    throw new InvalidDataException("An access control entry has an impossible size.");
                }
                uint mask = BitConverter.ToUInt32(acl, offset + 4);
                bool inheritOnly = (flags & 0x08) != 0;
                if (!inheritOnly && (mask & rights) != 0 && !Denies(type))
                {
                    int sid = SidOffset(acl, offset, type, size);
                    string account = sid < 0 ? null : SidToString(acl, sid, offset + size);
                    if (account == null)
                    {
                        return $"an access control entry of type {type}";
                    }
                    if (!Trusted(account, accounts))
                    {
                        return account;
                    }
                }
                offset += size;
            }
            return null;
        }

        // A SID as a string, S-1-..., from its binary form at offset in bytes, which must end by end; null when it does not
        // fit.
        internal static string SidToString(byte[] bytes, int offset, int end)
        {
            if (offset + 8 > end)
            {
                return null;
            }
            int subAuthorities = bytes[offset + 1];
            if (offset + 8 + (4 * subAuthorities) > end)
            {
                return null;
            }
            ulong authority = 0;
            for (int i = 2; i < 8; i++)
            {
                authority = (authority << 8) | bytes[offset + i];
            }
            var text = new StringBuilder("S-").Append(bytes[offset]).Append('-');
            text.Append(authority < 0x100000000UL ? authority.ToString(System.Globalization.CultureInfo.InvariantCulture) : "0x" + authority.ToString("X12", System.Globalization.CultureInfo.InvariantCulture));
            for (int i = 0; i < subAuthorities; i++)
            {
                text.Append('-').Append(BitConverter.ToUInt32(bytes, offset + 8 + (4 * i)).ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            return text.ToString();
        }

        // The root of a path GetFinalPathNameByHandle gives, \\?\C:\ or \\?\UNC\server\share\, with the names of the
        // directories below it in names. Any other form, such as a volume without a drive letter, cannot be walked.
        internal static string Root(string resolved, out List<string> names)
        {
            const string Unc = @"\\?\UNC\";
            const string Local = @"\\?\";
            string root;
            string rest;
            if (resolved.StartsWith(Unc, StringComparison.OrdinalIgnoreCase))
            {
                string[] parts = resolved.Substring(Unc.Length).Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2)
                {
                    throw new UntrustedStateException($"{Shown(resolved)} is not a path whose directories can be checked");
                }
                root = Unc + parts[0] + "\\" + parts[1] + "\\";
                rest = string.Join("\\", parts, 2, parts.Length - 2);
            }
            else if (resolved.StartsWith(Local, StringComparison.Ordinal) && resolved.Length >= Local.Length + 2 && resolved[Local.Length + 1] == ':' && char.IsLetter(resolved[Local.Length]))
            {
                root = resolved.Substring(0, Local.Length + 2) + "\\";
                rest = resolved.Substring(Local.Length + 2);
            }
            else
            {
                throw new UntrustedStateException($"{Shown(resolved)} is not a path whose directories can be checked");
            }
            names = new List<string>(rest.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries));
            if (names.Exists(name => name == "." || name == ".."))
            {
                throw new UntrustedStateException($"{Shown(resolved)} is not a path whose directories can be checked");
            }
            return root;
        }

        // A resolved path as people write it: C:\... or \\server\share\....
        internal static string Shown(string resolved)
        {
            if (resolved.StartsWith(@"\\?\UNC\", StringComparison.OrdinalIgnoreCase))
            {
                return @"\\" + resolved.Substring(8);
            }
            return resolved.StartsWith(@"\\?\", StringComparison.Ordinal) ? resolved.Substring(4) : resolved;
        }

        // Whether the resolved path inner is the resolved path outer or inside it.
        internal static bool Within(string inner, string outer)
        {
            string prefix = outer.TrimEnd('\\');
            return string.Equals(inner.TrimEnd('\\'), prefix, StringComparison.OrdinalIgnoreCase)
                || inner.StartsWith(prefix + "\\", StringComparison.OrdinalIgnoreCase);
        }

        // The control bits that decide how a DACL inherits: SE_DACL_PROTECTED keeps the entries its directory passes on
        // out of it, and SE_DACL_AUTO_INHERITED marks a list whose inherited entries are flagged as such, so that a later
        // change to what the directory passes on replaces them. SE_DACL_AUTO_INHERIT_REQ asks for the latter when a list
        // is set. The other bits (present, defaulted, self-relative, the SACL's) say how a descriptor is held, not who
        // may use the file.
        internal const ushort DaclProtected = 0x1000;
        internal const ushort DaclAutoInherited = 0x0400;
        internal const ushort DaclAutoInheritRequest = 0x0100;

        // The forms GetFinalPathNameByHandle is asked for, in turn, for the path of a held file a setter goes by:
        // normalized, with its drive letter (FILE_NAME_NORMALIZED | VOLUME_NAME_DOS); as opened (FILE_NAME_OPENED),
        // which an SMB share gives without querying each directory above the file; then both with the volume's GUID
        // (VOLUME_NAME_GUID), for a volume without a drive letter.
        internal static readonly uint[] HeldPathForms = { 0x0, 0x8, 0x1, 0x9 };

        // Whether GetFinalPathNameByHandle failing with error leaves the next form to try: access denied, as when an SMB
        // share will not let this user query a directory above the file, or path not found, as when there is no drive
        // letter. Any other error is the file's.
        internal static bool AnotherPathForm(int error)
        {
            return error == 5 || error == 3;
        }

        // The control asked for when a list is written as a copy of one whose control is copied: its protection and its
        // auto-inheritance, which is asked for (SE_DACL_AUTO_INHERIT_REQ) as well as stated.
        internal static ushort RequestedControl(ushort copied)
        {
            ushort control = (ushort)(copied & (DaclProtected | DaclAutoInherited));
            return (control & DaclAutoInherited) != 0 ? (ushort)(control | DaclAutoInheritRequest) : control;
        }

        // Whether a file whose DACL reads back as acl, with control, has the list given as givenAcl, with givenControl,
        // both as GetSecurityInfo shows them: the same entries, byte for byte, their flags included; no list for no list
        // and an empty one for an empty one; and the same protection and auto-inheritance.
        internal static bool SameList(byte[] givenAcl, ushort givenControl, byte[] acl, ushort control)
        {
            const ushort Inheritance = DaclProtected | DaclAutoInherited;
            bool entries = givenAcl == null ? acl == null : acl != null && System.Linq.Enumerable.SequenceEqual(givenAcl, acl);
            return entries && (givenControl & Inheritance) == (control & Inheritance);
        }

        private static bool Trusted(string account, TokenAccounts accounts)
        {
            return Same(account, accounts.User) || Same(account, LocalSystem) || Same(account, Administrators)
                || Same(account, OwnerRights) || Same(account, CreatorOwner);
        }

        // ACCESS_DENIED_ACE_TYPE, ACCESS_DENIED_OBJECT_ACE_TYPE, ACCESS_DENIED_CALLBACK_ACE_TYPE and
        // ACCESS_DENIED_CALLBACK_OBJECT_ACE_TYPE.
        private static bool Denies(byte type)
        {
            return type == 1 || type == 6 || type == 10 || type == 12;
        }

        // Where the SID of the allowing entry at offset starts, or -1 for a kind of entry this does not know. The basic and
        // callback kinds (ACCESS_ALLOWED_ACE_TYPE, ACCESS_ALLOWED_CALLBACK_ACE_TYPE) have it after the mask; the object
        // kinds (ACCESS_ALLOWED_OBJECT_ACE_TYPE, ACCESS_ALLOWED_CALLBACK_OBJECT_ACE_TYPE) after a flags field and the
        // GUIDs it says are present.
        private static int SidOffset(byte[] acl, int offset, byte type, int size)
        {
            switch (type)
            {
                case 0:
                case 9:
                    return offset + 8;
                case 5:
                case 11:
                    if (size < 12)
                    {
                        return -1;
                    }
                    uint present = BitConverter.ToUInt32(acl, offset + 8);
                    return offset + 12 + ((present & 1) != 0 ? 16 : 0) + ((present & 2) != 0 ? 16 : 0);
                default:
                    return -1;
            }
        }

        private static bool Same(string account, string other)
        {
            return other != null && string.Equals(account, other, StringComparison.OrdinalIgnoreCase);
        }
    }

    // The account checkpoint storage trusts besides SYSTEM and Administrators: the user of the token the calling thread
    // acts with (WindowsNative.CurrentAccounts).
    internal sealed class TokenAccounts
    {
        internal TokenAccounts(string user)
        {
            User = user;
        }

        internal string User { get; }
    }

    // The Windows calls WindowsStateDirectory makes.
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    internal static class WindowsNative
    {
        internal const uint DirectoryAttribute = 0x10;

        private const uint ReparsePointAttribute = 0x400;
        private const uint NameSurrogate = 0x20000000;

        private const uint ReadAttributes = 0x80;
        private const uint ReadControl = 0x20000;
        private const uint GenericRead = 0x80000000;
        private const uint GenericWrite = 0x40000000;
        private const uint ShareRead = 1;
        private const uint ShareWrite = 2;
        private const uint ShareDelete = 4;
        private const uint CreateNew = 1;
        private const uint OpenExisting = 3;
        private const uint BackupSemantics = 0x02000000;
        private const uint OpenReparsePoint = 0x00200000;
        private const uint NormalAttribute = 0x80;
        // How a stage is opened, created or reopened. With RandomAccess (.NET 6 and later), for overlapped I/O
        // (FILE_FLAG_OVERLAPPED): RandomAccess writes a synchronous handle on a thread pool thread that it holds until
        // Windows has done the write, from the same pool a download receives its parts on; it writes an overlapped
        // handle without holding one. Without RandomAccess, synchronous, as the stage's FileStream writes it
        // (WindowsStage.WriteAsync).
#if NET6_0_OR_GREATER
        internal const uint StageIo = 0x40000000;
#else
        internal const uint StageIo = 0;
#endif
        private const uint ReplaceExisting = 1;
        private const uint WriteThrough = 8;
        private const int FileAttributeTagInfo = 9;
        private const int FileObject = 1;
        private const uint OwnerInformation = 1;
        private const uint DaclInformation = 4;
        private const uint TokenQuery = 8;
        private const int TokenUser = 1;
        // FILE_GENERIC_READ, FILE_GENERIC_WRITE, FILE_GENERIC_EXECUTE and FILE_ALL_ACCESS: what generic rights mean for files.
        private const uint FileGenericRead = 0x120089;
        private const uint FileGenericWrite = 0x120116;
        private const uint FileGenericExecute = 0x1200A0;
        private const uint FileAllAccess = 0x1F01FF;
        // SEF_DACL_AUTO_INHERIT, SEF_AVOID_PRIVILEGE_CHECK and SEF_AVOID_OWNER_CHECK.
        private const uint DaclAutoInherit = 0x1;
        private const uint AvoidPrivilegeCheck = 0x8;
        private const uint AvoidOwnerCheck = 0x10;
        private const int NoToken = 1008;
        private const int NotFound = 2;
        private const int PathNotFound = 3;
        private const int AlreadyExists = 183;
        private const uint PersistentAcls = 0x8;
        private const uint Delete = 0x10000;
        private const uint Synchronize = 0x100000;
        private const uint FileTraverse = 0x20;
        private const uint FileAddSubdirectory = 0x4;
        private const uint FileAddFile = 0x2;
        private const uint HiddenAttribute = 0x2;
        private const uint NonDirectoryFile = 0x40;
        private const uint DeleteOnCloseOption = 0x1000;
        private const int AccessDenied = 5;
        private const uint CaseInsensitive = 0x40;
        private const uint FileCreateDisposition = 2;
        private const uint DirectoryFile = 0x1;
        private const uint SynchronousIoNonAlert = 0x20;
        private const uint OpenReparsePointOption = 0x200000;
        private const int StatusAccessDenied = unchecked((int)0xC0000022);


        // BY_HANDLE_FILE_INFORMATION, its FILETIMEs as pairs of DWORDs.
        [StructLayout(LayoutKind.Sequential)]
        internal struct FileInformation
        {
            internal uint Attributes;
            internal uint CreationLow;
            internal uint CreationHigh;
            internal uint AccessLow;
            internal uint AccessHigh;
            internal uint WriteLow;
            internal uint WriteHigh;
            internal uint VolumeSerial;
            internal uint SizeHigh;
            internal uint SizeLow;
            internal uint Links;
            internal uint IndexHigh;
            internal uint IndexLow;
        }

        // FILE_ATTRIBUTE_TAG_INFO.
        [StructLayout(LayoutKind.Sequential)]
        private struct AttributeTag
        {
            internal uint Attributes;
            internal uint ReparseTag;
        }

        // GENERIC_MAPPING.
        [StructLayout(LayoutKind.Sequential)]
        private struct GenericMapping
        {
            internal uint GenericRead;
            internal uint GenericWrite;
            internal uint GenericExecute;
            internal uint GenericAll;
        }

        // UNICODE_STRING, lengths in bytes.
        [StructLayout(LayoutKind.Sequential)]
        private struct UnicodeString
        {
            internal ushort Length;
            internal ushort MaximumLength;
            internal IntPtr Buffer;
        }

        // OBJECT_ATTRIBUTES.
        [StructLayout(LayoutKind.Sequential)]
        private struct ObjectAttributes
        {
            internal int Length;
            internal IntPtr RootDirectory;
            internal IntPtr ObjectName;
            internal uint Attributes;
            internal IntPtr SecurityDescriptor;
            internal IntPtr SecurityQualityOfService;
        }

        // IO_STATUS_BLOCK.
        [StructLayout(LayoutKind.Sequential)]
        private struct IoStatusBlock
        {
            internal IntPtr Status;
            internal IntPtr Information;
        }

        // SECURITY_ATTRIBUTES.
        [StructLayout(LayoutKind.Sequential)]
        private struct SecurityAttributes
        {
            internal int Length;
            internal IntPtr Descriptor;
            internal int InheritHandle;
        }

        // An object's owner, as a SID string, its DACL, as the system keeps it, or null when it has none, and the control
        // of its security descriptor.
        internal sealed class Security
        {
            internal string Owner { get; set; }

            internal byte[] Dacl { get; set; }

            internal ushort Control { get; set; }
        }

        [DllImport("kernel32.dll", EntryPoint = "CreateFileW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern SafeFileHandle CreateFile(string name, uint access, uint share, IntPtr security, uint disposition, uint flags, IntPtr template);

        [DllImport("kernel32.dll", EntryPoint = "CreateFileW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern SafeFileHandle CreateFile(string name, uint access, uint share, ref SecurityAttributes security, uint disposition, uint flags, IntPtr template);

        [DllImport("kernel32.dll", EntryPoint = "CreateDirectoryW", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CreateDirectory(string name, ref SecurityAttributes security);

        [DllImport("kernel32.dll", EntryPoint = "GetFinalPathNameByHandleW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern uint GetFinalPathNameByHandle(SafeFileHandle file, [Out] char[] path, uint length, uint flags);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetFileInformationByHandle(SafeFileHandle file, out FileInformation information);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetFileInformationByHandleEx(SafeFileHandle file, int informationClass, out AttributeTag information, uint size);

        [DllImport("kernel32.dll", EntryPoint = "MoveFileExW", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool MoveFileEx(string from, string to, uint flags);

        [DllImport("kernel32.dll", EntryPoint = "DeleteFileW", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteFileNative(string name);

        [DllImport("kernel32.dll", EntryPoint = "GetVolumeInformationByHandleW", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetVolumeInformationByHandle(SafeFileHandle file, char[] volumeName, uint volumeNameSize, IntPtr serialNumber, IntPtr maximumComponentLength, out uint fileSystemFlags, char[] fileSystemName, uint fileSystemNameSize);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CreatePrivateObjectSecurityEx(IntPtr parentDescriptor, IntPtr creatorDescriptor, out IntPtr newDescriptor, IntPtr objectType, [MarshalAs(UnmanagedType.Bool)] bool isContainerObject, uint autoInheritFlags, IntPtr token, ref GenericMapping genericMapping);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyPrivateObjectSecurity(ref IntPtr objectDescriptor);

        [DllImport("advapi32.dll", EntryPoint = "SetFileSecurityW", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetFileSecurity(string fileName, uint securityInformation, IntPtr securityDescriptor);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool InitializeSecurityDescriptor(IntPtr descriptor, uint revision);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetSecurityDescriptorDacl(IntPtr descriptor, [MarshalAs(UnmanagedType.Bool)] bool present, IntPtr dacl, [MarshalAs(UnmanagedType.Bool)] bool defaulted);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetSecurityDescriptorControl(IntPtr descriptor, ushort bitsOfInterest, ushort bitsToSet);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetSecurityDescriptorControl(IntPtr descriptor, out ushort control, out uint revision);

        [DllImport("ntdll.dll")]
        private static extern int NtCreateFile(out SafeFileHandle handle, uint access, ref ObjectAttributes attributes, out IoStatusBlock status, IntPtr allocationSize, uint fileAttributes, uint share, uint disposition, uint options, IntPtr eaBuffer, uint eaLength);

        [DllImport("ntdll.dll")]
        private static extern int RtlNtStatusToDosError(int status);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll")]
        private static extern IntPtr LocalFree(IntPtr memory);

        [DllImport("advapi32.dll")]
        private static extern uint GetSecurityInfo(SafeFileHandle handle, int objectType, uint information, out IntPtr owner, out IntPtr group, out IntPtr dacl, out IntPtr sacl, out IntPtr descriptor);

        [DllImport("advapi32.dll", EntryPoint = "ConvertStringSecurityDescriptorToSecurityDescriptorW", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ConvertStringSecurityDescriptor(string descriptor, uint revision, out IntPtr converted, IntPtr size);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenProcessToken(IntPtr process, uint access, out IntPtr token);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenThreadToken(IntPtr thread, uint access, [MarshalAs(UnmanagedType.Bool)] bool openAsSelf, out IntPtr token);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentThread();

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetTokenInformation(IntPtr token, int informationClass, IntPtr information, int length, out int returned);

        [DllImport("advapi32.dll")]
        private static extern int GetLengthSid(IntPtr sid);

        // The final path of the directory at path, following every link and junction in path, which is the caller's.
        internal static string FinalPathOf(string path)
        {
            using (SafeFileHandle handle = CreateFile(path, ReadAttributes, ShareRead | ShareWrite | ShareDelete, IntPtr.Zero, OpenExisting, BackupSemantics, IntPtr.Zero))
            {
                if (handle.IsInvalid)
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error == NotFound || error == PathNotFound)
                    {
                        throw new DirectoryNotFoundException($"The directory {path} does not exist.");
                    }
                    throw Failure(error, $"The directory {path} cannot be opened");
                }
                return FinalPathOf(handle, $"The directory {path}");
            }
        }

        // The final path of the object handle names, as GetFinalPathNameByHandle gives it (\\?\C:\... or \\?\UNC\...).
        internal static string FinalPathOf(SafeFileHandle handle, string what)
        {
            return FinalPathOf(handle, 0, out int error) ?? throw Failure(error, $"{what} cannot be resolved");
        }

        // The path of the object handle names in the form flags asks GetFinalPathNameByHandle for; null, with its error,
        // when it gives none.
        private static string FinalPathOf(SafeFileHandle handle, uint flags, out int error)
        {
            var buffer = new char[1024];
            while (true)
            {
                uint length = GetFinalPathNameByHandle(handle, buffer, (uint)buffer.Length, flags);
                if (length == 0)
                {
                    error = Marshal.GetLastWin32Error();
                    return null;
                }
                if (length < buffer.Length)
                {
                    error = 0;
                    return new string(buffer, 0, (int)length);
                }
                buffer = new char[length + 1];
            }
        }

        // The path of the file handle holds, for a setter that goes by path: the first form GetFinalPathNameByHandle gives
        // (WindowsSecurity.HeldPathForms) whose path, opened without following a reparse point, is shown to reach this
        // very file (the same volume and file index). Where the normalized form is refused, as on an SMB share that will
        // not let this user query a directory above the file, or on a volume without a drive letter, the name as opened
        // and the volume GUID forms are asked for too; whether any of them is given, on a share or a volume of any kind,
        // is not known until tried, and when none is, this fails. A path that reaches another file is refused; one that
        // cannot be opened leaves the next form. The check comes before the setter opens the path itself: an ancestor
        // this SDK does not hold (the default policy's) could change between the two, which the caller's reading back of
        // the held file shows but cannot undo; the held policies hold every ancestor.
        internal static string PathOfHeld(SafeFileHandle file, string what)
        {
            FileInformation held = InformationOf(file, what);
            int error = 0;
            foreach (uint form in WindowsSecurity.HeldPathForms)
            {
                string path = FinalPathOf(file, form, out error);
                if (path == null)
                {
                    if (WindowsSecurity.AnotherPathForm(error))
                    {
                        continue;
                    }
                    break;
                }
                using (SafeFileHandle reached = CreateFile(path, ReadAttributes, ShareRead | ShareWrite | ShareDelete, IntPtr.Zero, OpenExisting, BackupSemantics | OpenReparsePoint, IntPtr.Zero))
                {
                    if (reached.IsInvalid)
                    {
                        error = Marshal.GetLastWin32Error();
                        continue;
                    }
                    FileInformation there = InformationOf(reached, what);
                    if (there.VolumeSerial != held.VolumeSerial || there.IndexHigh != held.IndexHigh || there.IndexLow != held.IndexLow)
                    {
                        throw new IOException($"{what} is not the file its path, {path}, now leads to.");
                    }
                    return path;
                }
            }
            throw Failure(error, $"{what} cannot be resolved");
        }

        // The directory at path, opened without following a reparse point and held so that no one can rename, move or
        // remove it (no FILE_SHARE_DELETE); one that is a name surrogate (link, junction, mount point) is refused. Opened
        // to read its owner and access control list too when examine.
        internal static SafeFileHandle OpenDirectoryToHold(string path, string shown, bool examine)
        {
            SafeFileHandle handle = CreateFile(path, ReadAttributes | (examine ? ReadControl : 0), ShareRead | ShareWrite, IntPtr.Zero, OpenExisting, BackupSemantics | OpenReparsePoint, IntPtr.Zero);
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
                AttributeTag tag = TagOf(handle, $"The directory {shown}");
                if ((tag.Attributes & DirectoryAttribute) == 0 || ((tag.Attributes & ReparsePointAttribute) != 0 && (tag.ReparseTag & NameSurrogate) != 0))
                {
                    throw new UntrustedStateException($"{shown} was replaced by a link or something other than a directory while it was checked");
                }
                return handle;
            }
            catch
            {
                handle.Dispose();
                throw;
            }
        }

        // The file at path opened to read, or null when there is none. A name surrogate (link) is refused; another kind of
        // reparse point, such as a file kept in the cloud, is opened again as the file, which must be the same.
        internal static SafeFileHandle OpenFileToRead(string path, string shown)
        {
            SafeFileHandle handle = CreateFile(path, GenericRead, ShareRead, IntPtr.Zero, OpenExisting, OpenReparsePoint, IntPtr.Zero);
            if (handle.IsInvalid)
            {
                int error = Marshal.GetLastWin32Error();
                handle.Dispose();
                if (error == NotFound || error == PathNotFound)
                {
                    return null;
                }
                throw Failure(error, $"The checkpoint {shown} cannot be opened");
            }
            try
            {
                AttributeTag tag = TagOf(handle, $"The checkpoint {shown}");
                if ((tag.Attributes & ReparsePointAttribute) == 0)
                {
                    return handle;
                }
                if ((tag.ReparseTag & NameSurrogate) != 0)
                {
                    throw new UntrustedStateException($"{shown} is a link");
                }
                FileInformation first = InformationOf(handle, $"The checkpoint {shown}");
                SafeFileHandle file = CreateFile(path, GenericRead, ShareRead, IntPtr.Zero, OpenExisting, 0, IntPtr.Zero);
                if (file.IsInvalid)
                {
                    int error = Marshal.GetLastWin32Error();
                    file.Dispose();
                    throw Failure(error, $"The checkpoint {shown} cannot be opened");
                }
                // The file opened again stays this method's until it is returned: any failure closes it, and is thrown.
                try
                {
                    FileInformation second = InformationOf(file, $"The checkpoint {shown}");
                    if (first.VolumeSerial != second.VolumeSerial || first.IndexHigh != second.IndexHigh || first.IndexLow != second.IndexLow)
                    {
                        throw new UntrustedStateException($"{shown} changed while it was opened");
                    }
                }
                catch
                {
                    file.Dispose();
                    throw;
                }
                handle.Dispose();
                return file;
            }
            catch
            {
                handle.Dispose();
                throw;
            }
        }

        internal static FileInformation InformationOf(SafeFileHandle handle, string what)
        {
            if (!GetFileInformationByHandle(handle, out FileInformation information))
            {
                throw Failure(Marshal.GetLastWin32Error(), $"{what} cannot be examined");
            }
            return information;
        }

        // Whether the volume of the object handle names keeps access control lists (FILE_PERSISTENT_ACLS), as NTFS and ReFS
        // do and FAT and exFAT do not; null when the volume cannot be asked, as some file systems and SMB servers do not
        // answer GetVolumeInformationByHandleW. Only a hint: what a file's own list reads back as decides.
        internal static bool? KeepsAccessControl(SafeFileHandle handle)
        {
            if (!GetVolumeInformationByHandle(handle, null, 0, IntPtr.Zero, IntPtr.Zero, out uint flags, null, 0))
            {
                return null;
            }
            return (flags & PersistentAcls) != 0;
        }

        // Whether the object handle names is a reparse point that names another path: a symbolic link, junction or mount
        // point (a name surrogate).
        internal static bool NamesAnotherPath(SafeFileHandle handle, string what)
        {
            AttributeTag tag = TagOf(handle, what);
            return (tag.Attributes & ReparsePointAttribute) != 0 && (tag.ReparseTag & NameSurrogate) != 0;
        }

        // The owner and DACL of the object handle names.
        internal static Security SecurityOf(SafeFileHandle handle, string what)
        {
            uint error = GetSecurityInfo(handle, FileObject, OwnerInformation | DaclInformation, out IntPtr owner, out _, out IntPtr dacl, out _, out IntPtr descriptor);
            if (error != 0)
            {
                throw Failure((int)error, $"Who may use {what} cannot be read");
            }
            try
            {
                byte[] list = null;
                if (dacl != IntPtr.Zero)
                {
                    // ACL: AclSize is the ushort at offset 2.
                    list = new byte[(ushort)Marshal.ReadInt16(dacl, 2)];
                    Marshal.Copy(dacl, list, 0, list.Length);
                }
                return new Security { Owner = SidString(owner), Dacl = list, Control = ControlOf(descriptor, $"Who may use {what}") };
            }
            finally
            {
                LocalFree(descriptor);
            }
        }

        // A new file at path, created with an access control list of its own, protected from inheritance, that allows
        // only this user, SYSTEM and Administrators. Fails when anything is at path already.
        internal static SafeFileHandle CreatePrivateFile(string path, string shown, TokenAccounts accounts)
        {
            IntPtr descriptor = PrivateDescriptor(inheritable: false, accounts);
            try
            {
                var attributes = new SecurityAttributes { Length = Marshal.SizeOf<SecurityAttributes>(), Descriptor = descriptor };
                SafeFileHandle handle = CreateFile(path, GenericWrite, 0, ref attributes, CreateNew, NormalAttribute, IntPtr.Zero);
                if (handle.IsInvalid)
                {
                    int error = Marshal.GetLastWin32Error();
                    handle.Dispose();
                    throw Failure(error, $"A new checkpoint cannot be created beside {shown}");
                }
                return handle;
            }
            finally
            {
                LocalFree(descriptor);
            }
        }

        // A new stage at path, opened with access and shared with no one, created with an access control list of its own,
        // protected from inheritance, that allows only this user, SYSTEM and Administrators. Its owner is the one this
        // user's token gives new objects, as any file this user creates gets, so that the file put in place keeps the
        // owner it has always had. Fails when anything is at path already.
        internal static SafeFileHandle CreatePrivateStage(string path, string shown, uint access, TokenAccounts accounts)
        {
            IntPtr descriptor = PrivateDescriptor(inheritable: false, accounts, withOwner: false);
            try
            {
                var attributes = new SecurityAttributes { Length = Marshal.SizeOf<SecurityAttributes>(), Descriptor = descriptor };
                SafeFileHandle handle = CreateFile(path, access, 0, ref attributes, CreateNew, NormalAttribute | StageIo, IntPtr.Zero);
                if (handle.IsInvalid)
                {
                    int error = Marshal.GetLastWin32Error();
                    handle.Dispose();
                    throw Failure(error, $"The file being downloaded, {shown}, cannot be created");
                }
                return handle;
            }
            finally
            {
                LocalFree(descriptor);
            }
        }

        // The destination directory at path, opened following the links in path as creating a file there does, so that
        // what is made through this handle is made in the very directory a file put at that path lands in: with the right
        // to add files and directories, and to read its security descriptor where that is granted (without it, a directory
        // a user may write into but not examine, it is opened all the same).
        internal static SafeFileHandle OpenDestination(string path, string shown)
        {
            SafeFileHandle handle = null;
            foreach (uint access in new[] { ReadControl, 0u })
            {
                handle?.Dispose();
                handle = CreateFile(path, access | FileTraverse | FileAddFile | FileAddSubdirectory | Synchronize, ShareRead | ShareWrite | ShareDelete, IntPtr.Zero, OpenExisting, BackupSemantics, IntPtr.Zero);
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
                    throw new DirectoryNotFoundException($"The directory {shown} does not exist.");
                }
                throw Failure(error, $"The directory {shown} cannot be opened");
            }
            return handle;
        }

        // A new directory called name in the directory parent holds, created and opened in one call (NtCreateFile with
        // FILE_CREATE), so that the handle is the very directory this call created and nothing put at its name can be
        // taken for it: belonging to this user, with the access control list its parent passes on, held without delete
        // sharing, with the right to delete it where that is granted (without it, it is left behind when done, as a
        // directory this user may not delete is). Fails when anything is at that name.
        internal static SafeFileHandle CreateDirectoryHeld(SafeFileHandle parent, string name, string shown, TokenAccounts accounts)
        {
            if (!ConvertStringSecurityDescriptor($"O:{accounts.User}", 1, out IntPtr descriptor, IntPtr.Zero))
            {
                throw Failure(Marshal.GetLastWin32Error(), $"The directory {shown} cannot be created");
            }
            try
            {
                uint[] tries = { Delete | ReadAttributes | Synchronize, ReadAttributes | Synchronize };
                for (int i = 0; ; i++)
                {
                    int status = CreateRelative(parent, name, tries[i], 0, ShareRead | ShareWrite, DirectoryFile | SynchronousIoNonAlert | OpenReparsePointOption, descriptor, out SafeFileHandle created);
                    if (status >= 0)
                    {
                        return created;
                    }
                    created.Dispose();
                    if (status != StatusAccessDenied || i == tries.Length - 1)
                    {
                        throw Failure(RtlNtStatusToDosError(status), $"The directory {shown} cannot be created");
                    }
                }
            }
            finally
            {
                LocalFree(descriptor);
            }
        }

        // An empty file created in the directory parent holds, under a name of its own, with no security descriptor of the
        // SDK's, so that the file system gives it the one it gives any file this user creates directly in that directory,
        // which reading it back shows (for this user, without reading the directory's own descriptor). Hidden, shared with
        // no one, and deleted when its handle closes, whether by the SDK or, should the process end, by the system; it never
        // holds any of a download's bytes. Null when a file created there would not let this user read its security
        // descriptor or delete it, which is then not created.
        internal static SafeFileHandle CreateAccessProbe(SafeFileHandle parent, string shown)
        {
            string name = ".files-download-access-" + Guid.NewGuid().ToString("N");
            int status = CreateRelative(parent, name, ReadControl | Delete | Synchronize, HiddenAttribute, 0, NonDirectoryFile | SynchronousIoNonAlert | DeleteOnCloseOption, IntPtr.Zero, out SafeFileHandle created);
            if (status >= 0)
            {
                return created;
            }
            created.Dispose();
            if (status == StatusAccessDenied)
            {
                return null;
            }
            throw Failure(RtlNtStatusToDosError(status), $"A file cannot be created in the directory of {shown} to learn the access a new file there gets");
        }

        // NtCreateFile of name relative to the directory parent holds, creating it (FILE_CREATE).
        private static int CreateRelative(SafeFileHandle parent, string name, uint access, uint fileAttributes, uint share, uint options, IntPtr descriptor, out SafeFileHandle created)
        {
            IntPtr buffer = Marshal.StringToHGlobalUni(name);
            IntPtr objectName = Marshal.AllocHGlobal(Marshal.SizeOf<UnicodeString>());
            bool referenced = false;
            try
            {
                Marshal.StructureToPtr(new UnicodeString { Length = (ushort)(name.Length * 2), MaximumLength = (ushort)((name.Length * 2) + 2), Buffer = buffer }, objectName, false);
                parent.DangerousAddRef(ref referenced);
                var attributes = new ObjectAttributes
                {
                    Length = Marshal.SizeOf<ObjectAttributes>(),
                    RootDirectory = parent.DangerousGetHandle(),
                    ObjectName = objectName,
                    Attributes = CaseInsensitive,
                    SecurityDescriptor = descriptor,
                };
                return NtCreateFile(out created, access, ref attributes, out IoStatusBlock _, IntPtr.Zero, fileAttributes, share, FileCreateDisposition, options, IntPtr.Zero, 0);
            }
            finally
            {
                if (referenced)
                {
                    parent.DangerousRelease();
                }
                Marshal.FreeHGlobal(objectName);
                Marshal.FreeHGlobal(buffer);
            }
        }

        // Creates the directory at path, if missing, with an access control list of its own, protected from inheritance
        // and inherited by what is created in it, that allows only this user, SYSTEM and Administrators.
        internal static void CreatePrivateDirectory(string path)
        {
            IntPtr descriptor = PrivateDescriptor(inheritable: true, CurrentAccounts());
            try
            {
                var attributes = new SecurityAttributes { Length = Marshal.SizeOf<SecurityAttributes>(), Descriptor = descriptor };
                if (!CreateDirectory(path, ref attributes))
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error != AlreadyExists)
                    {
                        throw Failure(error, $"The directory {path} cannot be created");
                    }
                }
            }
            finally
            {
                LocalFree(descriptor);
            }
        }

        // Moves from over to, replacing it, written through to storage.
        internal static void MoveOver(string from, string to, string shown)
        {
            if (!MoveFileEx(from, to, ReplaceExisting | WriteThrough))
            {
                throw Failure(Marshal.GetLastWin32Error(), $"The checkpoint {shown} cannot be replaced");
            }
        }

        // Removes the file at path, if it can.
        internal static void DeleteFile(string path)
        {
            DeleteFileNative(path);
        }

        private static AttributeTag TagOf(SafeFileHandle handle, string what)
        {
            if (!GetFileInformationByHandleEx(handle, FileAttributeTagInfo, out AttributeTag tag, (uint)Marshal.SizeOf<AttributeTag>()))
            {
                throw Failure(Marshal.GetLastWin32Error(), $"{what} cannot be examined");
            }
            return tag;
        }

        // A descriptor that allows only this user, SYSTEM and Administrators, protected from inheritance; inherited by what
        // is created in it when inheritable. It makes this user the owner when withOwner; without, what it is given to
        // keeps its own owner, or, when created with it, gets the one this user's token gives new objects.
        internal static IntPtr PrivateDescriptor(bool inheritable, TokenAccounts accounts, bool withOwner = true)
        {
            string inherit = inheritable ? "OICI" : "";
            string descriptor = $"{(withOwner ? "O:" + accounts.User : "")}D:P(A;{inherit};FA;;;{accounts.User})(A;{inherit};FA;;;SY)(A;{inherit};FA;;;BA)";
            if (!ConvertStringSecurityDescriptor(descriptor, 1, out IntPtr converted, IntPtr.Zero))
            {
                throw Failure(Marshal.GetLastWin32Error(), "A private access control list cannot be made");
            }
            return converted;
        }

        // The user of the token the calling thread acts with: the token it impersonates with, if it does (opened as the
        // process, so that an identification-level token still answers), or else the process's. Read for each use, since
        // a thread may impersonate another user for one transfer and not the next.
        internal static TokenAccounts CurrentAccounts()
        {
            if (!OpenThreadToken(GetCurrentThread(), TokenQuery, true, out IntPtr token))
            {
                int error = Marshal.GetLastWin32Error();
                if (error != NoToken)
                {
                    throw Failure(error, "The user this thread acts as cannot be read");
                }
                if (!OpenProcessToken(GetCurrentProcess(), TokenQuery, out token))
                {
                    throw Failure(Marshal.GetLastWin32Error(), "The process's user cannot be read");
                }
            }
            try
            {
                return new TokenAccounts(TokenSid(token, TokenUser));
            }
            finally
            {
                CloseHandle(token);
            }
        }

        // Calls use with the token the calling thread acts with, opened to query it: the one it impersonates with, if it
        // does (opened as the process, so that an identification-level token still answers), or else the process's.
        internal static void WithToken(Action<IntPtr> use)
        {
            if (!OpenThreadToken(GetCurrentThread(), TokenQuery, true, out IntPtr token))
            {
                int error = Marshal.GetLastWin32Error();
                if (error != NoToken)
                {
                    throw Failure(error, "The user this thread acts as cannot be read");
                }
                if (!OpenProcessToken(GetCurrentProcess(), TokenQuery, out token))
                {
                    throw Failure(Marshal.GetLastWin32Error(), "The process's user cannot be read");
                }
            }
            try
            {
                use(token);
            }
            finally
            {
                CloseHandle(token);
            }
        }

        // Calls use with the security descriptor of the object handle names, its parts information says, self-relative,
        // as GetSecurityInfo gives it; freed after.
        internal static void WithSecurityDescriptor(SafeFileHandle handle, uint information, string what, Action<IntPtr> use)
        {
            uint error = GetSecurityInfo(handle, FileObject, information, out _, out _, out _, out _, out IntPtr descriptor);
            if (error != 0)
            {
                throw Failure((int)error, $"Who may use {what} cannot be read");
            }
            try
            {
                use(descriptor);
            }
            finally
            {
                LocalFree(descriptor);
            }
        }

        // Calls use with the security descriptor Windows gives a new file created directly in a directory whose descriptor
        // is parent, for the user of the token the calling thread acts with, worked out as the file system works it out
        // when it creates one (CreatePrivateObjectSecurityEx, with DACL auto-inheritance and the generic rights of files):
        // the entries the directory passes on to its files, those that go no further than its own children
        // (NO_PROPAGATE_INHERIT) included, their generic rights mapped and CREATOR OWNER resolved; where it passes none on,
        // the token's default list, even an empty one; where the token has none, no list at all. Freed after.
        internal static void WithNewFileDescriptor(IntPtr parent, string what, Action<IntPtr> use)
        {
            WithToken(token =>
            {
                var files = new GenericMapping { GenericRead = FileGenericRead, GenericWrite = FileGenericWrite, GenericExecute = FileGenericExecute, GenericAll = FileAllAccess };
                if (!CreatePrivateObjectSecurityEx(parent, IntPtr.Zero, out IntPtr created, IntPtr.Zero, false, DaclAutoInherit | AvoidPrivilegeCheck | AvoidOwnerCheck, token, ref files))
                {
                    throw Failure(Marshal.GetLastWin32Error(), $"The access a new file in {what} gets cannot be worked out");
                }
                try
                {
                    use(created);
                }
                finally
                {
                    DestroyPrivateObjectSecurity(ref created);
                }
            });
        }

        // The control bits of descriptor.
        internal static ushort ControlOf(IntPtr descriptor, string what)
        {
            if (!GetSecurityDescriptorControl(descriptor, out ushort control, out uint _))
            {
                throw Failure(Marshal.GetLastWin32Error(), $"{what} cannot be read");
            }
            return control;
        }

        // Asks that the file handle holds be given dacl (IntPtr.Zero for no list) as a copy of a list whose control is
        // control: a new descriptor holding only that DACL, with its protection and auto-inheritance requested
        // (WindowsSecurity.RequestedControl), through SetFileSecurity, the file-system setter documented to set a file's
        // DACL from a descriptor. What the file system then stores is not documented for SetFileSecurity itself. The
        // rules Windows documents for SetPrivateObjectSecurityEx when it is asked to auto-inherit (SEF_DACL_AUTO_INHERIT)
        // keep an object's own inherited entries when neither its list nor the one given is protected, and clear the
        // inherited flags of a protected list given; the caller protects the file's list first on the assumption that a
        // file system may apply them, which is not established for any file system here. So the caller reads the file's
        // list back and refuses it unless the entries, their flags and the protection and auto-inheritance are exactly
        // those given. It goes by a path shown to reach this very file (PathOfHeld), which may fail: the file's handle
        // holds it without sharing the right to delete or rename it, in a staging directory held the same way.
        // SetFileSecurity is obsolete, not withdrawn: the setters Windows recommends instead (SetSecurityInfo,
        // SetNamedSecurityInfo) work an unprotected list out from the file's current directory, which for a stage is the
        // staging directory, and need to read that directory's descriptor.
        internal static void SetFileDacl(SafeFileHandle file, IntPtr dacl, ushort control, string what)
        {
            string path = PathOfHeld(file, what);
            // SECURITY_DESCRIPTOR_MIN_LENGTH: 20 bytes, 40 with 64-bit pointers.
            IntPtr descriptor = Marshal.AllocHGlobal(64);
            try
            {
                const ushort Inheritance = WindowsSecurity.DaclProtected | WindowsSecurity.DaclAutoInherited | WindowsSecurity.DaclAutoInheritRequest;
                if (!InitializeSecurityDescriptor(descriptor, 1) || !SetSecurityDescriptorDacl(descriptor, true, dacl, false)
                    || !SetSecurityDescriptorControl(descriptor, Inheritance, WindowsSecurity.RequestedControl(control)))
                {
                    throw Failure(Marshal.GetLastWin32Error(), $"The list for {what} cannot be made");
                }
                if (!SetFileSecurity(path, DaclInformation, descriptor))
                {
                    throw Failure(Marshal.GetLastWin32Error(), $"Who may use {what} cannot be set");
                }
            }
            finally
            {
                Marshal.FreeHGlobal(descriptor);
            }
        }

        // The SID TOKEN_USER holds: it starts with the pointer to it.
        private static string TokenSid(IntPtr token, int informationClass)
        {
            GetTokenInformation(token, informationClass, IntPtr.Zero, 0, out int length);
            if (length <= 0)
            {
                throw Failure(Marshal.GetLastWin32Error(), "The process's user cannot be read");
            }
            IntPtr buffer = Marshal.AllocHGlobal(length);
            try
            {
                if (!GetTokenInformation(token, informationClass, buffer, length, out _))
                {
                    throw Failure(Marshal.GetLastWin32Error(), "The process's user cannot be read");
                }
                return SidString(Marshal.ReadIntPtr(buffer));
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        private static string SidString(IntPtr sid)
        {
            if (sid == IntPtr.Zero)
            {
                return null;
            }
            var bytes = new byte[GetLengthSid(sid)];
            Marshal.Copy(sid, bytes, 0, bytes.Length);
            return WindowsSecurity.SidToString(bytes, 0, bytes.Length);
        }

        private static IOException Failure(int error, string what)
        {
            return new IOException($"{what}: {new Win32Exception(error).Message}");
        }
    }
}