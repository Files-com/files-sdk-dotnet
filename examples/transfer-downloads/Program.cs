using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FilesCom;
using FilesCom.Transfers;

namespace TransferDownloads
{
    // Downloads files with client.Transfers and client.FileSystem:
    //
    //   dotnet run -- <base-url> one <remote-path> <local-file> [<checkpoint>]              one file, continued when run again with a checkpoint
    //   dotnet run -- <base-url> folder <remote-folder> <local-directory> [<checkpoints>]   a folder, four files at a time
    //   dotnet run -- <base-url> cat <remote-path>                                          a file's text, read as a stream
    //
    // one and folder take --no-replace last to leave every local file already there as it is. Signs in with
    // FILES_API_KEY. Ctrl+C cancels any of them.
    internal static class Program
    {
        private const string Usage = "Usage: transfer-downloads <base-url> one <remote-path> <local-file> [<checkpoint>] [--no-replace]\n" +
            "       transfer-downloads <base-url> folder <remote-folder> <local-directory> [<checkpoint-directory>] [--no-replace]\n" +
            "       transfer-downloads <base-url> cat <remote-path>";

        private static async Task<int> Main(string[] args)
        {
            bool noReplace = args.Length > 0 && args[args.Length - 1] == "--no-replace";
            if (noReplace)
            {
                args = args[..^1];
            }
            DownloadPublicationPolicy policy = noReplace ? DownloadPublicationPolicy.CreateNew : DownloadPublicationPolicy.ReplaceExisting;
            string command = args.Length > 1 ? args[1] : null;
            bool valid = ((command == "one" || command == "folder") && (args.Length == 4 || args.Length == 5)) || (command == "cat" && args.Length == 3);
            if (!valid)
            {
                Console.Error.WriteLine(Usage);
                return 2;
            }
            string apiKey = Environment.GetEnvironmentVariable("FILES_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.Error.WriteLine("Set FILES_API_KEY to sign in.");
                return 2;
            }
            await using FilesClient client = FilesClient.Create(new FilesConfiguration { BaseUrl = args[0], ApiKey = apiKey });

            using (var cancellation = new CancellationTokenSource())
            {
                Console.CancelKeyPress += (sender, e) =>
                {
                    e.Cancel = true;
                    cancellation.Cancel();
                };
                try
                {
                    switch (command)
                    {
                        case "one":
                            await OneFile.DownloadAsync(client, args[2], args[3], args.Length == 5 ? args[4] : null, policy, cancellation.Token);
                            return 0;
                        case "folder":
                            await Folder.DownloadAsync(client, args[2], args[3], args.Length == 5 ? args[4] : null, policy, cancellation.Token);
                            return 0;
                        default:
                            await Cat.PrintAsync(client, args[2], cancellation.Token);
                            return 0;
                    }
                }
                catch (OperationCanceledException)
                {
                    // Nothing partial is left under a file's name: an unfinished file stays in its staging folder, and
                    // with a checkpoint the same command continues it.
                    Console.Error.WriteLine("Cancelled. With a checkpoint, run the same command again to continue.");
                    return 3;
                }
                catch (DownloadSourceChangedException e)
                {
                    Console.Error.WriteLine($"{e.Message} Run the command again to download the file as it is now.");
                    return 1;
                }
                catch (DownloadPublicationException e)
                {
                    // Not put in place as the policy says (or put in place, but uncertain): Problem says which.
                    string kept = e.Retained ? " What was downloaded is kept: run the same command again once the name is free." : "";
                    Console.Error.WriteLine($"{e.LocalPath} ({e.Problem}, in place: {e.Placed}): {e.Message}{kept}");
                    return 1;
                }
                catch (SdkException e)
                {
                    Console.Error.WriteLine($"Files.com request failed ({e.GetType().Name}): {e.Message}");
                    return 1;
                }
                catch (IOException e)
                {
                    Console.Error.WriteLine($"A local file or folder could not be written: {e.Message}");
                    return 1;
                }
            }
        }
    }
}
