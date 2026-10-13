using FilesCom;
using FilesCom.Models;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCli.Commands
{
    // How a command authenticates, in order of preference: a session for --user, whose password is in
    // FILES_PASSWORD (and one-time password, when the account needs one, in FILES_OTP); the API key in FILES_API_KEY;
    // or the ApiKey or SessionId in files-cli.dll.config.
    internal static class SignIn
    {
        public static readonly Option<string> User = new Option<string>("--user", "-u")
        {
            Description = "Sign in as this user, with the password in FILES_PASSWORD and any one-time password in FILES_OTP.",
            Recursive = true,
        };

        public static readonly Option<string> RemovedPassword = Removed("--password", "-p", "FILES_PASSWORD");

        public static readonly Option<string> RemovedOtp = Removed("--otp", "-o", "FILES_OTP");

        // A secret on the command line ends up in shell history and process listings, so these options only say where
        // the value goes now. The message never repeats what was typed.
        private static Option<string> Removed(string name, string alias, string variable)
        {
            var option = new Option<string>(name, alias) { Hidden = true, Recursive = true };
            option.Validators.Add(result => result.AddError($"{name} is no longer supported. Set {variable} instead."));
            return option;
        }

        // Returns the request options that authenticate this run. They hold a new session's ID when signing in as
        // --user, and are empty when the client's own configuration authenticates.
        public static async Task<Dictionary<string, object>> CredentialsAsync(FilesClient client, string user, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(user))
            {
                var login = new Dictionary<string, object>
                {
                    { "username", user },
                    { "password", RequiredVariable("FILES_PASSWORD", "Set FILES_PASSWORD to sign in as --user.") },
                };
                string otp = Environment.GetEnvironmentVariable("FILES_OTP");
                if (!string.IsNullOrEmpty(otp))
                {
                    login.Add("otp", otp);
                }
                Session session = await client.Sessions.CreateAsync(login, cancellationToken: cancellationToken);
                return new Dictionary<string, object> { { "session_id", session.Id } };
            }

            string apiKey = Environment.GetEnvironmentVariable("FILES_API_KEY");
            if (!string.IsNullOrEmpty(apiKey))
            {
                return new Dictionary<string, object> { { "api_key", apiKey } };
            }

            if (string.IsNullOrEmpty(client.ApiKey) && string.IsNullOrEmpty(client.SessionId))
            {
                throw new SignInException("Set FILES_API_KEY, use --user with FILES_PASSWORD set, or set ApiKey in files-cli.dll.config.");
            }
            return new Dictionary<string, object>();
        }

        private static string RequiredVariable(string name, string message)
        {
            string value = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrEmpty(value))
            {
                throw new SignInException(message);
            }
            return value;
        }
    }

    // The sign-in settings are missing or incomplete, so no request was sent.
    internal sealed class SignInException : Exception
    {
        public SignInException(string message) : base(message)
        {
        }
    }
}