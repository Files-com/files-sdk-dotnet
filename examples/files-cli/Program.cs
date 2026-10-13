using FilesCli.Commands;
using System;
using System.CommandLine;
using System.Threading.Tasks;

namespace FilesCli
{
    // A small command-line tool built on the Files.com .NET SDK:
    //
    //   dotnet run -- list-users
    //   dotnet run -- show-user --id 1
    //   dotnet run -- upload-file --local-path report.pdf --path reports/report.pdf
    //   dotnet run -- download-file --path reports/report.pdf --local-path copy.pdf
    //
    // It signs in with FILES_API_KEY, or with --user and FILES_PASSWORD, or with the API key in its app.config.
    internal static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            var root = new RootCommand("Works with Files.com users and files through the Files.com .NET SDK.")
            {
                SignIn.User,
                SignIn.RemovedPassword,
                SignIn.RemovedOtp,
                new ListUsersCommand(),
                new ShowUserCommand(),
                new UploadFileCommand(),
                new DownloadFileCommand(),
            };

            // A Files.com path can start with "@", so arguments are never read from response files.
            ParseResult parseResult = root.Parse(args, new ParserConfiguration { ResponseFileTokenReplacer = null });
            if (parseResult.UnmatchedTokens.Count > 0)
            {
                // The unrecognized words are not repeated: after a mistyped option, the next word can be a secret.
                Console.Error.WriteLine("Unrecognized command, option or argument. Run files-cli --help to see the commands and options.");
                return ExitCode.Usage;
            }

            return await parseResult.InvokeAsync(new InvocationConfiguration
            {
                // Ctrl+C cancels the running command. Give it time to release its files before the process exits,
                // instead of the default two seconds.
                ProcessTerminationTimeout = TimeSpan.FromSeconds(10),
                // Commands report the failures they expect; anything else is a bug and keeps its stack trace.
                EnableDefaultExceptionHandler = false,
            });
        }
    }
}