using FilesCom;
using FilesCom.Models;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCli.Commands
{
    internal sealed class ShowUserCommand : FilesCommand
    {
        private readonly Option<long> id = new Option<long>("--id")
        {
            Description = "The ID of the user to show.",
            Required = true,
        };

        public ShowUserCommand() : base("show-user", "ShowUser", "Shows a user's details.")
        {
            Options.Add(id);
        }

        protected override async Task ExecuteAsync(ParseResult parseResult, FilesClient client, Dictionary<string, object> credentials, CancellationToken cancellationToken)
        {
            User user = await client.Users.FindAsync(parseResult.GetValue(id), null, credentials, cancellationToken);

            Console.WriteLine($"Id: {user.Id}");
            Console.WriteLine($"Username: {user.Username}");
            Console.WriteLine($"AdminGroupIds: {string.Join(", ", user.AdminGroupIds ?? Array.Empty<long?>())}");
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
            Console.WriteLine($"ExternallyManaged: {user.ExternallyManaged}");
            Console.WriteLine($"TimeZone: {user.TimeZone}");
            Console.WriteLine($"TypeOf2fa: {user.TypeOf2fa}");
            Console.WriteLine($"UserRoot: {user.UserRoot}");
        }
    }
}