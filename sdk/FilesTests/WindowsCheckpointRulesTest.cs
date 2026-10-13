using FilesCom.Transfers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FilesTests
{
    // The rules Windows checkpoint storage decides by (WindowsSecurity), given access control lists in the binary form
    // Windows keeps them in and the paths GetFinalPathNameByHandle gives. They are plain data, so they run on every
    // system; what the Windows calls themselves return is for a Windows run of CheckpointIntegrityTest.
    [TestClass]
    public class WindowsCheckpointRulesTest
    {
        private const string User = "S-1-5-21-1004336348-1177238915-682003330-1001";
        private const string OtherUser = "S-1-5-21-1004336348-1177238915-682003330-1002";
        private const string Users = "S-1-5-32-545";
        private const string AuthenticatedUsers = "S-1-5-11";
        private const string Everyone = "S-1-1-0";
        private const string Staff = "S-1-5-21-1004336348-1177238915-682003330-1105";
        private const string System = "S-1-5-18";
        private const string Administrators = "S-1-5-32-544";
        private const string CreatorOwner = "S-1-3-0";
        private const string OwnerRights = "S-1-3-4";
        private const uint FullControl = 0x1F01FF;
        private const uint Modify = 0x1301BF;
        private const uint ReadAndExecute = 0x1200A9;
        private const uint WriteData = 0x2;
        private const uint GenericAll = 0x10000000;
        private const byte Allowed = 0;
        private const byte Denied = 1;
        private const byte AllowedObject = 5;
        private const byte InheritOnly = 0x08;
        private const byte Inherited = 0x10;
        private const byte ObjectAndContainerInherit = 0x03;

        private static readonly TokenAccounts Accounts = new TokenAccounts(User);

        private static byte[] Private => Acl(Ace(Allowed, 0, FullControl, User), Ace(Allowed, 0, FullControl, System), Ace(Allowed, 0, FullControl, Administrators));

        [TestMethod]
        public void OnlyThisUserSystemAndAdministratorsMayChangeWhatIsInACheckpointDirectory()
        {
            Assert.IsNull(WindowsSecurity.UntrustedGrant(Private, Accounts), "A private list.");
            Assert.IsNull(WindowsSecurity.UntrustedGrant(Acl(Ace(Allowed, 0, FullControl, User), Ace(Allowed, 0, ReadAndExecute, Everyone)), Accounts), "Others may read and list.");
            Assert.IsNull(WindowsSecurity.UntrustedGrant(Acl(Ace(Denied, 0, WriteData, Everyone), Ace(Allowed, 0, FullControl, User)), Accounts), "An entry that denies grants nothing.");
            Assert.IsNull(WindowsSecurity.UntrustedGrant(Acl(Ace(Allowed, InheritOnly | ObjectAndContainerInherit, FullControl, Users), Ace(Allowed, 0, FullControl, User)), Accounts), "An entry only children inherit does not apply to the directory.");
            Assert.IsNull(WindowsSecurity.UntrustedGrant(Acl(Ace(Allowed, InheritOnly | ObjectAndContainerInherit, FullControl, CreatorOwner), Ace(Allowed, 0, FullControl, OwnerRights), Ace(Allowed, 0, FullControl, User)), Accounts), "CREATOR OWNER and OWNER RIGHTS name the owner.");
            Assert.IsNull(WindowsSecurity.UntrustedGrant(Acl(Ace(Allowed, 0, FullControl, Administrators)), Accounts), "Administrators, whose group an elevated administrator's new objects belong to by default.");
            Assert.AreEqual(Staff, WindowsSecurity.UntrustedGrant(Acl(Ace(Allowed, 0, FullControl, User), Ace(Allowed, 0, Modify, Staff)), Accounts), "A group, even one a token might name as its default owner, is other users.");

            Assert.AreEqual(Users, WindowsSecurity.UntrustedGrant(Acl(Ace(Allowed, 0, FullControl, User), Ace(Allowed, 0, WriteData, Users)), Accounts), "Users may add files.");
            Assert.AreEqual(AuthenticatedUsers, WindowsSecurity.UntrustedGrant(Acl(Ace(Allowed, Inherited, Modify, AuthenticatedUsers)), Accounts), "An inherited entry applies as any other.");
            Assert.AreEqual(Everyone, WindowsSecurity.UntrustedGrant(Acl(Ace(Allowed, 0, GenericAll, Everyone)), Accounts), "GENERIC_ALL.");
            Assert.AreEqual(OtherUser, WindowsSecurity.UntrustedGrant(Acl(Ace(Allowed, 0, 0x10000, OtherUser)), Accounts), "DELETE lets another user remove or rename the directory.");
            Assert.AreEqual(OtherUser, WindowsSecurity.UntrustedGrant(Acl(Ace(Allowed, 0, 0x40000, OtherUser)), Accounts), "WRITE_DAC lets another user give themselves anything.");
            Assert.AreEqual(Users, WindowsSecurity.UntrustedGrant(Acl(Ace(AllowedObject, 0, WriteData, Users, objectType: true)), Accounts), "An object entry, its SID after its GUID.");
            Assert.AreEqual("an access control entry of type 4", WindowsSecurity.UntrustedGrant(Acl(Ace(4, 0, WriteData, Users)), Accounts), "An allowing entry of a kind not known is refused.");
            StringAssert.StartsWith(WindowsSecurity.UntrustedGrant(null, Accounts), "everyone", "No list at all allows everyone everything.");
        }

        // A download's stage is private only when no one else may read it either: what a checkpoint directory allows others,
        // reading and listing, a stage does not.
        [TestMethod]
        public void OnlyThisUserSystemAndAdministratorsMayReadOrChangeAStage()
        {
            Assert.IsNull(WindowsSecurity.UntrustedGrant(Private, Accounts, WindowsSecurity.PrivateRights), "A private list.");
            Assert.AreEqual(Everyone, WindowsSecurity.UntrustedGrant(Acl(Ace(Allowed, 0, FullControl, User), Ace(Allowed, 0, ReadAndExecute, Everyone)), Accounts, WindowsSecurity.PrivateRights), "Others may read it.");
            Assert.AreEqual(Users, WindowsSecurity.UntrustedGrant(Acl(Ace(Allowed, 0, FullControl, User), Ace(Allowed, Inherited, 0x80000000, Users)), Accounts, WindowsSecurity.PrivateRights), "GENERIC_READ, inherited, as from a destination directory.");
            Assert.AreEqual(OtherUser, WindowsSecurity.UntrustedGrant(Acl(Ace(Allowed, 0, FullControl, User), Ace(Allowed, 0, WriteData, OtherUser)), Accounts, WindowsSecurity.PrivateRights), "Others may change it.");
            Assert.IsNull(WindowsSecurity.UntrustedGrant(Acl(Ace(Allowed, 0, FullControl, User), Ace(Allowed, 0, 0x80, Everyone)), Accounts, WindowsSecurity.PrivateRights), "Reading its attributes reads none of its bytes.");
        }

        // A stage is created with a private list. Only reading it back as that list shows it private, and so lets it be kept
        // for a later run; a list that cannot be read back, on a volume that cannot say or says it keeps lists, may still be
        // in place, so the file's final access is still set as it is put in place. Only a file system shown to keep no
        // lists, read back as having none or a volume that says so, needs none.
        [TestMethod]
        public void AStagesListThatCannotBeReadBackIsNotPrivateButIsStillReplaced()
        {
            WindowsSecurity.StageList read = WindowsSecurity.StageListFrom(read: true, hasList: true, volumeKeepsLists: null);
            Assert.IsTrue(WindowsSecurity.NeedsFinalAccess(read));
            Assert.IsTrue(WindowsSecurity.ShownPrivate(read, User, Private, Accounts), "Read back as its private list.");
            Assert.IsFalse(WindowsSecurity.ShownPrivate(read, Staff, Private, Accounts), "Of an owner not trusted.");

            foreach (bool? volume in new bool?[] { null, true })
            {
                WindowsSecurity.StageList unread = WindowsSecurity.StageListFrom(read: false, hasList: false, volumeKeepsLists: volume);
                Assert.IsTrue(WindowsSecurity.NeedsFinalAccess(unread), $"Unread, the volume saying {volume?.ToString() ?? "nothing"}: its private list may be in place.");
                Assert.IsFalse(WindowsSecurity.ShownPrivate(unread, User, Private, Accounts), "Nothing unread is shown private.");
            }

            WindowsSecurity.StageList none = WindowsSecurity.StageListFrom(read: true, hasList: false, volumeKeepsLists: null);
            WindowsSecurity.StageList fat = WindowsSecurity.StageListFrom(read: false, hasList: false, volumeKeepsLists: false);
            foreach (WindowsSecurity.StageList kept in new[] { none, fat })
            {
                Assert.IsFalse(WindowsSecurity.NeedsFinalAccess(kept), "A file system that keeps no list has none to replace.");
                Assert.IsFalse(WindowsSecurity.ShownPrivate(kept, User, null, Accounts), "Nor can it keep a stage private.");
            }
        }

        [TestMethod]
        public void AListGivenToAStageMustReadBackWithItsEntriesProtectionAndInheritance()
        {
            const ushort Protected = 0x1000;
            const ushort AutoInherited = 0x0400;
            const ushort AutoInheritRequest = 0x0100;
            const ushort Present = 0x0004;
            const ushort SelfRelative = 0x8000;
            const ushort SaclAutoInherited = 0x0800;
            byte[] inherited = Acl(Ace(Allowed, Inherited, Modify, Users), Ace(Denied, Inherited | 0x04, WriteData, Staff));
            byte[] copy = inherited.ToArray();

            Assert.IsTrue(WindowsSecurity.SameList(inherited, AutoInherited, copy, (ushort)(AutoInherited | Present | SelfRelative | SaclAutoInherited)), "How the descriptor is held, and the SACL's control, are not compared.");
            Assert.IsFalse(WindowsSecurity.SameList(inherited, AutoInherited, copy, (ushort)(AutoInherited | Protected)), "Protected where the list given inherits.");
            Assert.IsFalse(WindowsSecurity.SameList(inherited, (ushort)(AutoInherited | Protected), copy, AutoInherited), "Inheriting where the list given is protected.");
            Assert.IsFalse(WindowsSecurity.SameList(inherited, AutoInherited, copy, 0), "No longer marked auto-inherited.");
            Assert.IsFalse(WindowsSecurity.SameList(inherited, AutoInherited, Acl(Ace(Allowed, 0, Modify, Users), Ace(Denied, 0x04, WriteData, Staff)), AutoInherited), "The same entries no longer flagged inherited.");
            Assert.IsFalse(WindowsSecurity.SameList(inherited, AutoInherited, Acl(Ace(Allowed, Inherited, Modify, Users)), AutoInherited), "An entry lost.");

            Assert.IsTrue(WindowsSecurity.SameList(null, 0, null, Present), "No list stays no list.");
            Assert.IsFalse(WindowsSecurity.SameList(null, 0, Acl(), 0), "An empty list, which allows no one, is not no list, which allows everyone.");
            Assert.IsFalse(WindowsSecurity.SameList(Acl(), 0, null, 0), "Nor the other way.");
            Assert.IsTrue(WindowsSecurity.SameList(Acl(), Protected, Acl(), Protected), "An empty list stays empty.");

            Assert.AreEqual((ushort)(AutoInherited | AutoInheritRequest), WindowsSecurity.RequestedControl((ushort)(AutoInherited | Present | SelfRelative)), "Auto-inheritance is asked for as well as stated.");
            Assert.AreEqual(Protected, WindowsSecurity.RequestedControl((ushort)(Protected | Present | SaclAutoInherited)), "Protection alone; nothing of the SACL's.");
            Assert.AreEqual((ushort)0, WindowsSecurity.RequestedControl(Present), "A list neither protected nor auto-inherited, as a token's default gives.");
        }

        [TestMethod]
        public void AStagesPathIsAskedForAsOpenedWhenItsShareWillNotNormalizeIt()
        {
            CollectionAssert.AreEqual(new uint[] { 0x0, 0x8, 0x1, 0x9 }, WindowsSecurity.HeldPathForms, "Normalized, as opened, then both by volume GUID.");
            Assert.IsTrue(WindowsSecurity.AnotherPathForm(5), "ERROR_ACCESS_DENIED: an SMB share will not let this user query a directory above the file.");
            Assert.IsTrue(WindowsSecurity.AnotherPathForm(3), "ERROR_PATH_NOT_FOUND: no drive letter.");
            Assert.IsFalse(WindowsSecurity.AnotherPathForm(6), "ERROR_INVALID_HANDLE is the file's own failure.");
        }

        [TestMethod]
        public void AListThatDoesNotHoldTogetherIsNotRead()
        {
            byte[] acl = Private;
            Assert.ThrowsExactly<InvalidDataException>(() => WindowsSecurity.UntrustedGrant(acl.Take(20).ToArray(), Accounts));
            acl[10] = 0xFF;
            acl[11] = 0x7F;
            Assert.ThrowsExactly<InvalidDataException>(() => WindowsSecurity.UntrustedGrant(acl, Accounts));
        }

        [TestMethod]
        public void OwnersAreThisUserSystemOrAdministrators()
        {
            Assert.IsTrue(WindowsSecurity.TrustedOwner(User, Accounts));
            Assert.IsTrue(WindowsSecurity.TrustedOwner(System, Accounts));
            Assert.IsTrue(WindowsSecurity.TrustedOwner(Administrators, Accounts));
            Assert.IsFalse(WindowsSecurity.TrustedOwner(OtherUser, Accounts));
            Assert.IsFalse(WindowsSecurity.TrustedOwner(Staff, Accounts), "A group a token may name as its default owner is not this user.");
            Assert.IsFalse(WindowsSecurity.TrustedOwner(null, Accounts));
        }

        [TestMethod]
        public void SidsAreWrittenAsWindowsWritesThem()
        {
            byte[] sid = Sid(User);
            Assert.AreEqual(User, WindowsSecurity.SidToString(sid, 0, sid.Length));
            Assert.IsNull(WindowsSecurity.SidToString(sid, 0, sid.Length - 1), "A SID that does not fit.");
            byte[] large = { 1, 1, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 7, 0, 0, 0 };
            Assert.AreEqual("S-1-0x010203040506-7", WindowsSecurity.SidToString(large, 0, large.Length), "An authority of 2^32 or more, in hexadecimal.");
        }

        [TestMethod]
        public void ResolvedPathsAreWalkedFromTheirVolumeOrShare()
        {
            Assert.AreEqual(@"\\?\C:\", WindowsSecurity.Root(@"\\?\C:\Users\me\state", out List<string> names));
            CollectionAssert.AreEqual(new[] { "Users", "me", "state" }, names);
            Assert.AreEqual(@"\\?\C:\", WindowsSecurity.Root(@"\\?\C:\", out names));
            Assert.AreEqual(0, names.Count);
            Assert.AreEqual(@"\\?\UNC\server\share\", WindowsSecurity.Root(@"\\?\UNC\server\share\transfers\state", out names));
            CollectionAssert.AreEqual(new[] { "transfers", "state" }, names);
            Assert.ThrowsExactly<UntrustedStateException>(() => WindowsSecurity.Root(@"\\?\Volume{0f2c1d2e-1111-2222-3333-444455556666}\state", out _), "A volume without a drive letter.");
            Assert.ThrowsExactly<UntrustedStateException>(() => WindowsSecurity.Root(@"\\?\UNC\server", out _));

            Assert.AreEqual(@"C:\Users\me", WindowsSecurity.Shown(@"\\?\C:\Users\me"));
            Assert.AreEqual(@"\\server\share\x", WindowsSecurity.Shown(@"\\?\UNC\server\share\x"));

            Assert.IsTrue(WindowsSecurity.Within(@"\\?\C:\Data\Tree\sub", @"\\?\c:\data\tree"));
            Assert.IsTrue(WindowsSecurity.Within(@"\\?\C:\Data\Tree", @"\\?\C:\Data\Tree\"));
            Assert.IsFalse(WindowsSecurity.Within(@"\\?\C:\Data\Treetop", @"\\?\C:\Data\Tree"));
        }

        // An ACL: AclRevision 2, Sbz1, AclSize, AceCount, Sbz2, then the entries.
        private static byte[] Acl(params byte[][] aces)
        {
            int size = 8 + aces.Sum(ace => ace.Length);
            var acl = new List<byte> { 2, 0 };
            acl.AddRange(BitConverter.GetBytes((ushort)size));
            acl.AddRange(BitConverter.GetBytes((ushort)aces.Length));
            acl.AddRange(new byte[2]);
            foreach (byte[] ace in aces)
            {
                acl.AddRange(ace);
            }
            return acl.ToArray();
        }

        // An entry: ACE_HEADER (AceType, AceFlags, AceSize), ACCESS_MASK, for an object entry its flags and GUID, then the
        // SID.
        private static byte[] Ace(byte type, int flags, uint mask, string sid, bool objectType = false)
        {
            var body = new List<byte>(BitConverter.GetBytes(mask));
            if (type == AllowedObject)
            {
                body.AddRange(BitConverter.GetBytes(objectType ? 1u : 0u));
                if (objectType)
                {
                    body.AddRange(Guid.NewGuid().ToByteArray());
                }
            }
            body.AddRange(Sid(sid));
            var ace = new List<byte> { type, (byte)flags };
            ace.AddRange(BitConverter.GetBytes((ushort)(4 + body.Count)));
            ace.AddRange(body);
            return ace.ToArray();
        }

        // A SID's binary form: Revision, SubAuthorityCount, the 48-bit authority big-endian, the subauthorities.
        private static byte[] Sid(string sid)
        {
            string[] parts = sid.Split('-');
            ulong authority = ulong.Parse(parts[2]);
            var bytes = new List<byte> { byte.Parse(parts[1]), (byte)(parts.Length - 3) };
            for (int shift = 40; shift >= 0; shift -= 8)
            {
                bytes.Add((byte)(authority >> shift));
            }
            foreach (string sub in parts.Skip(3))
            {
                bytes.AddRange(BitConverter.GetBytes(uint.Parse(sub)));
            }
            return bytes.ToArray();
        }
    }
}