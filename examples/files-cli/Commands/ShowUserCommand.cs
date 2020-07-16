using Files.Models;
using System;
using System.Threading.Tasks;

namespace files_cli.Commands
{
    public class ShowUserCommand : BaseCommand
    {
        public int UserId { get; set; }

        public ShowUserCommand()
        {
            IsCommand("ShowUser", "Prints user info");

            HasLongDescription("Shows a specific user's information.");

            HasRequiredOption("id=", "The id of the user to show.", id => UserId = int.Parse(id));
        }

        public override async Task<int> RunAsync(string[] remainingArguments)
        {
            User user = await User.Get(UserId, null, await GetOptions());

            Console.WriteLine($"Id: {user.Id}");
            Console.WriteLine($"Username: {user.Username}");
            Console.WriteLine($"AdminGroupIds: {user.AdminGroupIds}");
            Console.WriteLine($"AllowedIps: {user.AllowedIps}");
            Console.WriteLine($"AttachmentsPermission: {user.AttachmentsPermission}");
            Console.WriteLine($"ApiKeysCount: {user.ApiKeysCount}");
            Console.WriteLine($"AuthenticateUntil: {user.AuthenticateUntil}");
            Console.WriteLine($"AuthenticationMethod: {user.AuthenticationMethod}");
            Console.WriteLine($"AvatarUrl: {user.AvatarUrl}");
            Console.WriteLine($"BillingPermission: {user.BillingPermission}");
            Console.WriteLine($"BypassSiteAllowedIps: {user.BypassSiteAllowedIps}");
            Console.WriteLine($"CreatedAt: {user.CreatedAt}");
            Console.WriteLine($"DavPermission: {user.DavPermission}");
            Console.WriteLine($"Disabled: {user.Disabled}");
            Console.WriteLine($"LastLoginAt: {user.LastLoginAt}");
            Console.WriteLine($"LastProtocolCipher: {user.LastProtocolCipher}");
            Console.WriteLine($"LockoutExpires: {user.LockoutExpires}");
            Console.WriteLine($"Name: {user.Name}");
            Console.WriteLine($"Notes: {user.Notes}");
            Console.WriteLine($"NotificationDailySendTime: {user.NotificationDailySendTime}");
            Console.WriteLine($"PasswordSetAt: {user.PasswordSetAt}");
            Console.WriteLine($"PasswordValidityDays: {user.PasswordValidityDays}");
            Console.WriteLine($"PublicKeysCount: {user.PublicKeysCount}");
            Console.WriteLine($"ReceiveAdminAlerts: {user.ReceiveAdminAlerts}");
            Console.WriteLine($"Require2fa: {user.Require2fa}");
            Console.WriteLine($"RequirePasswordChange: {user.RequirePasswordChange}");
            Console.WriteLine($"RestapiPermission: {user.RestapiPermission}");
            Console.WriteLine($"SelfManaged: {user.SelfManaged}");
            Console.WriteLine($"SftpPermission: {user.SftpPermission}");
            Console.WriteLine($"SiteAdmin: {user.SiteAdmin}");
            Console.WriteLine($"SkipWelcomeScreen: {user.SkipWelcomeScreen}");
            Console.WriteLine($"SslRequired: {user.SslRequired}");
            Console.WriteLine($"SsoStrategyId: {user.SsoStrategyId}");
            Console.WriteLine($"SubscribeToNewsletter: {user.SubscribeToNewsletter}");
            Console.WriteLine($"ExternallyManaged: {user.ExternallyManaged}");
            Console.WriteLine($"TimeZone: {user.TimeZone}");
            Console.WriteLine($"TypeOf2fa: {user.TypeOf2fa}");
            Console.WriteLine($"UserRoot: {user.UserRoot}");
            return Success;
        }
    }
}
