using FilesCom;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCli.Commands
{
    // A command that signs in to Files.com, does its work and turns the outcome into an exit code.
    internal abstract class FilesCommand : Command
    {
        protected FilesCommand(string name, string legacyName, string description) : base(name, description)
        {
            // The name the command had before version 2.0 still works.
            Aliases.Add(legacyName);
            SetAction(RunAsync);
        }

        // Does the command's work. Every request includes credentials.
        protected abstract Task ExecuteAsync(ParseResult parseResult, FilesClient client, Dictionary<string, object> credentials, CancellationToken cancellationToken);

        private async Task<int> RunAsync(ParseResult parseResult, CancellationToken cancellationToken)
        {
            FilesClient client = null;
            Dictionary<string, object> credentials = null;
            try
            {
                // Uses the files.com/filesConfiguration section of files-cli.dll.config, or the default settings.
                client = FilesClient.Create();
                credentials = await SignIn.CredentialsAsync(client, parseResult.GetValue(SignIn.User), cancellationToken);
                await ExecuteAsync(parseResult, client, credentials, cancellationToken);
                return ExitCode.Success;
            }
            catch (SignInException e)
            {
                Console.Error.WriteLine(e.Message);
                return ExitCode.Usage;
            }
            catch (ConfigurationErrorsException e)
            {
                Console.Error.WriteLine($"files-cli.dll.config is not valid: {e.Message}");
                return ExitCode.Usage;
            }
            catch (OperationCanceledException)
            {
                Console.Error.WriteLine("Cancelled before finishing.");
                return ExitCode.Cancelled;
            }
            catch (SdkException e)
            {
                Console.Error.WriteLine($"Files.com request failed ({e.GetType().Name}): {e.Message}");
                return ExitCode.Failure;
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                Console.Error.WriteLine($"Local file error: {e.Message}");
                return ExitCode.Failure;
            }
            finally
            {
                if (credentials != null && credentials.ContainsKey("session_id"))
                {
                    await SignOutAsync(client, credentials);
                }
                // Closes the client's connections once its requests, including signing out, have ended.
                if (client != null)
                {
                    await client.DisposeAsync();
                }
            }
        }

        // Ends the session this run created, even after a failure or Ctrl+C, but only briefly and without replacing
        // the command's own outcome.
        private static async Task SignOutAsync(FilesClient client, Dictionary<string, object> credentials)
        {
            using var limit = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            try
            {
                await client.Sessions.DeleteAsync(null, credentials, limit.Token);
            }
            catch (Exception e) when (e is SdkException || e is OperationCanceledException)
            {
                Console.Error.WriteLine("Could not sign out. The session expires on its own.");
            }
        }
    }
}