using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading.Tasks;

namespace FilesTests
{
    // How the Windows tests give their folders and their token the access their assertions take as given, rather than
    // what the machine they run on happens to pass on: a temporary folder inherits the list of this user's temporary
    // directory, which may let other accounts change what is in it, and a test run as an administrator may hold
    // privileges that read and change any file's permissions whatever its list says. What each test then has is written
    // to its output, before the transfer it is given to.
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    internal static class WindowsTestAccess
    {
        private const uint PrivilegeRemoved = 0x4;
        private const uint PrivilegeEnabled = 0x2;
        private const int NotAllAssigned = 1300;
        private const int TokenPrivileges = 3;
        private const uint MaximumAllowed = 0x02000000;
        private const int SecurityImpersonation = 2;
        private const int TokenImpersonation = 2;

        // Gives directory a list of its own, protected from inheritance, that lets only this user, SYSTEM and
        // Administrators use it and what is created in it, as a private folder of this user's has; writes this user, the
        // owner the token gives new objects, and the directory's owner and list as they now read back, and fails unless
        // they show it private: its own list, an owner the SDK trusts, and no one else allowed.
        internal static void MakePrivate(string directory)
        {
            var info = new DirectoryInfo(directory);
            SecurityIdentifier[] trusted = TrustedAccounts();
            var security = new DirectorySecurity();
            security.SetAccessRuleProtection(isProtected: true, preserveInheritance: false);
            foreach (SecurityIdentifier account in trusted)
            {
                security.AddAccessRule(new FileSystemAccessRule(account, FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow));
            }
            info.SetAccessControl(security);

            DirectorySecurity now = info.GetAccessControl(AccessControlSections.Owner | AccessControlSections.Access);
            var owner = (SecurityIdentifier)now.GetOwner(typeof(SecurityIdentifier));
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            string shown = $"user {current.User}, owner of new objects {current.Owner}; {directory}: owner {owner}, list {now.GetSecurityDescriptorSddlForm(AccessControlSections.Access)}";
            Console.WriteLine($"Private test folder: {shown}");
            Assert.IsTrue(now.AreAccessRulesProtected, $"The folder's list is its own: {shown}");
            Assert.IsTrue(trusted.Contains(owner), $"The folder belongs to this user, SYSTEM or Administrators: {shown}");
            foreach (FileSystemAccessRule rule in now.GetAccessRules(true, true, typeof(SecurityIdentifier)))
            {
                Assert.IsTrue(rule.AccessControlType == AccessControlType.Deny || trusted.Contains((SecurityIdentifier)rule.IdentityReference), $"Only this user, SYSTEM and Administrators may use the folder: {shown}");
            }
        }

        // Runs action as this user, impersonating a copy of the process's token from which the privileges that let an
        // administrator read and change any file's permissions whatever its list says (SeBackupPrivilege and
        // SeRestorePrivilege) are removed, so that a list refusing this user refuses it. The impersonation goes with
        // action through everything it awaits. Writes what each token holds of those privileges.
        internal static async Task WithoutBackupOrRestoreAsync(Func<Task> action)
        {
            string[] privileges = { "SeBackupPrivilege", "SeRestorePrivilege" };
            using (WindowsIdentity process = WindowsIdentity.GetCurrent(TokenAccessLevels.Duplicate | TokenAccessLevels.Query))
            using (SafeAccessTokenHandle token = Without(process.AccessToken, privileges))
            {
                Console.WriteLine($"Restricted test token: user {process.User}; process token {Held(process.AccessToken, privileges)}; test token {Held(token, privileges)}");
                await WindowsIdentity.RunImpersonatedAsync(token, action);
            }
        }

        private static SecurityIdentifier[] TrustedAccounts()
        {
            return new[]
            {
                WindowsIdentity.GetCurrent().User,
                new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null),
                new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null),
            };
        }

        // An impersonation token copied from token with each of privileges removed, so that nothing can enable them again.
        // A privilege token does not hold is left out as it is.
        private static SafeAccessTokenHandle Without(SafeAccessTokenHandle token, string[] privileges)
        {
            if (!DuplicateTokenEx(token, MaximumAllowed, IntPtr.Zero, SecurityImpersonation, TokenImpersonation, out IntPtr duplicate))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            var copy = new SafeAccessTokenHandle(duplicate);
            // TOKEN_PRIVILEGES: the count, then LUID_AND_ATTRIBUTES, a LUID and its attributes, for each.
            var state = new byte[4 + 12 * privileges.Length];
            BitConverter.GetBytes(privileges.Length).CopyTo(state, 0);
            for (int i = 0; i < privileges.Length; i++)
            {
                BitConverter.GetBytes(Luid(privileges[i])).CopyTo(state, 4 + 12 * i);
                BitConverter.GetBytes(PrivilegeRemoved).CopyTo(state, 4 + 12 * i + 8);
            }
            if (!AdjustTokenPrivileges(copy, false, state, state.Length, IntPtr.Zero, IntPtr.Zero))
            {
                int error = Marshal.GetLastWin32Error();
                copy.Dispose();
                throw new Win32Exception(error);
            }
            int result = Marshal.GetLastWin32Error();
            if (result != 0 && result != NotAllAssigned)
            {
                copy.Dispose();
                throw new Win32Exception(result);
            }
            return copy;
        }

        // Whether token holds each of privileges, and whether it is enabled.
        private static string Held(SafeAccessTokenHandle token, string[] privileges)
        {
            GetTokenInformation(token, TokenPrivileges, IntPtr.Zero, 0, out int length);
            IntPtr buffer = Marshal.AllocHGlobal(length);
            try
            {
                if (!GetTokenInformation(token, TokenPrivileges, buffer, length, out length))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                int count = Marshal.ReadInt32(buffer);
                return string.Join(", ", privileges.Select(name =>
                {
                    long luid = Luid(name);
                    for (int i = 0; i < count; i++)
                    {
                        if (Marshal.ReadInt64(buffer, 4 + 12 * i) == luid)
                        {
                            return name + ((Marshal.ReadInt32(buffer, 4 + 12 * i + 8) & PrivilegeEnabled) != 0 ? " enabled" : " disabled");
                        }
                    }
                    return name + " not held";
                }));
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        private static long Luid(string privilege)
        {
            if (!LookupPrivilegeValue(null, privilege, out long luid))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return luid;
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DuplicateTokenEx(SafeAccessTokenHandle token, uint access, IntPtr attributes, int level, int type, out IntPtr copy);

        [DllImport("advapi32.dll", EntryPoint = "LookupPrivilegeValueW", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool LookupPrivilegeValue(string system, string name, out long luid);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AdjustTokenPrivileges(SafeAccessTokenHandle token, [MarshalAs(UnmanagedType.Bool)] bool disableAll, byte[] state, int length, IntPtr previous, IntPtr returned);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetTokenInformation(SafeAccessTokenHandle token, int informationClass, IntPtr information, int length, out int returned);
    }
}