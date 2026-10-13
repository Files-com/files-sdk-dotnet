using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FilesCom;
using FilesCom.Transfers;

namespace TransferUploads
{
    // Uploads files with client.Transfers:
    //
    //   dotnet run -- <base-url> one <local-file> <remote-path>          one file, with progress
    //   dotnet run -- <base-url> many <remote-folder> <local-file>...     several files sharing one limiter
    //   dotnet run -- <base-url> cancel <local-file> <remote-path> <s>    a stream upload stopped after <s> seconds
    //   dotnet run -- <base-url> stdin <remote-path> [<max-bytes>]        standard input, of unknown length
    //   dotnet run -- <base-url> resume <local-file> <remote-path> <checkpoint>          one file, continued when run again
    //   dotnet run -- <base-url> folder <local-directory> <remote-folder> [<checkpoints>] a directory, four files at a time
    //
    // Signs in with FILES_API_KEY. Ctrl+C cancels any of them.
    internal static class Program
    {
        private const string Usage = "Usage: transfer-uploads <base-url> one <local-file> <remote-path>\n" +
            "       transfer-uploads <base-url> many <remote-folder> <local-file>...\n" +
            "       transfer-uploads <base-url> cancel <local-file> <remote-path> <seconds>\n" +
            "       transfer-uploads <base-url> stdin <remote-path> [<max-bytes>]\n" +
            "       transfer-uploads <base-url> resume <local-file> <remote-path> <checkpoint>\n" +
            "       transfer-uploads <base-url> folder <local-directory> <remote-folder> [<checkpoint-directory>]";

        private static async Task<int> Main(string[] args)
        {
            string command = args.Length > 1 ? args[1] : null;
            bool valid = (command == "one" && args.Length == 4) || (command == "many" && args.Length >= 4) || (command == "cancel" && args.Length == 5)
                || (command == "stdin" && (args.Length == 3 || (args.Length == 4 && long.TryParse(args[3], NumberStyles.None, CultureInfo.InvariantCulture, out _))))
                || (command == "resume" && args.Length == 5) || (command == "folder" && (args.Length == 4 || args.Length == 5));
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
                            await OneFile.UploadAsync(client, args[2], args[3], cancellation.Token);
                            return 0;
                        case "many":
                            return await SharedLimit.UploadAsync(client, args[2], args.Skip(3).ToArray(), cancellation.Token);
                        case "stdin":
                            await UnknownLength.UploadAsync(client, args[2], args.Length == 4 ? long.Parse(args[3], CultureInfo.InvariantCulture) : (long?)null, cancellation.Token);
                            return 0;
                        case "resume":
                            await Resume.UploadAsync(client, args[2], args[3], args[4], cancellation.Token);
                            return 0;
                        case "folder":
                            await Folder.UploadAsync(client, args[2], args[3], args.Length == 5 ? args[4] : null, cancellation.Token);
                            return 0;
                        default:
                            return await CancelWithProgress.UploadAsync(client, args[2], args[3], TimeSpan.FromSeconds(double.Parse(args[4], CultureInfo.InvariantCulture)), cancellation.Token);
                    }
                }
                catch (OperationCanceledException)
                {
                    // Parts already sent are not undone. Cancelled while its final request was on its way, an upload may
                    // still have been completed.
                    Console.Error.WriteLine("Cancelled. An upload whose final request had already been sent may still have been completed. " +
                        "With a checkpoint, run the same command again to continue.");
                    return 3;
                }
                catch (UploadCompletionUnknownException e)
                {
                    // Without an inner exception when a checkpoint recorded that an earlier run sent the final request.
                    Console.Error.WriteLine(e.InnerException != null ? $"{e.Message} ({e.InnerException.Message})" : e.Message);
                    return 1;
                }
                catch (SdkException e)
                {
                    Console.Error.WriteLine($"Files.com request failed ({e.GetType().Name}): {e.Message}");
                    return 1;
                }
                catch (IOException e)
                {
                    Console.Error.WriteLine($"A local file could not be read: {e.Message}");
                    return 1;
                }
            }
        }
    }

    // Prints each report on one line. The SDK calls Report one call at a time, on its own threads.
    internal sealed class ConsoleProgress : IProgress<TransferProgress>
    {
        public void Report(TransferProgress progress)
        {
            double megabytesPerSecond = progress.Elapsed.TotalSeconds > 0 ? progress.AcknowledgedBytes / progress.Elapsed.TotalSeconds / (1024 * 1024) : 0;
            string cooldown = progress.Concurrency.CooldownRemaining > TimeSpan.Zero ? $", paused {progress.Concurrency.CooldownRemaining.TotalSeconds:0.0} s after throttling" : "";
            // For a stream of unknown length, the total is what has been read so far.
            string total = progress.TotalBytesKnown ? $"{progress.TotalBytes}" : $"{progress.TotalBytes} read so far";
            Console.WriteLine($"  {progress.Path}: {progress.AcknowledgedBytes} of {total} bytes accepted, " +
                $"{progress.Concurrency.ActiveAttempts} of up to {progress.ConcurrencyLimit} parts in flight, {progress.PartRetries} retried, {megabytesPerSecond:0.0} MiB/s{cooldown}");
        }
    }
}
