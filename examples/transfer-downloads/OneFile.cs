using System;
using System.Threading;
using System.Threading.Tasks;
using FilesCom;
using FilesCom.Transfers;

namespace TransferDownloads
{
    // One file, in parts fetched four at a time when Files.com binds the download to the file's version, published
    // under its name only once complete, as policy says. With a checkpoint, a download stopped by Ctrl+C or a failure
    // continues from the parts already written when the same command runs again, and one whose file an earlier run had
    // already put in place completes without fetching it again.
    internal static class OneFile
    {
        internal static async Task DownloadAsync(FilesClient client, string remotePath, string localPath, string checkpointPath, DownloadPublicationPolicy policy, CancellationToken cancellationToken)
        {
            var options = new DownloadOptions
            {
                MaxConcurrency = 4,
                CheckpointPath = checkpointPath,
                PublicationPolicy = policy,
                Progress = new ConsoleProgress(),
            };

            DownloadResult result = await client.Transfers.DownloadFileAsync(remotePath, localPath, options, cancellationToken);
            if (result.CompletedEarlier)
            {
                Console.WriteLine($"{result.LocalPath} had been put in place by an earlier run; nothing was fetched.");
                return;
            }

            string how = result.VersionBound ? $"in {result.Parts} parts, all of one version" : "in one request, as its storage could not bind it to one version";
            Console.WriteLine($"Downloaded {result.Path} to {result.LocalPath}: {result.Size} bytes {how}; " +
                $"{result.ResumedBytes} bytes from an earlier run, {result.ReceivedBytes} received in {result.Elapsed.TotalSeconds:0.0} s.");
        }
    }

    // Prints each report on one line. The SDK calls Report one call at a time, on its own threads.
    internal sealed class ConsoleProgress : IProgress<DownloadProgress>
    {
        public void Report(DownloadProgress progress)
        {
            string total = progress.TotalBytesKnown ? $"{progress.TotalBytes}" : "?";
            Console.WriteLine($"  {progress.Path}: {progress.WrittenBytes} of {total} bytes written, {progress.ActiveParts} parts in flight, {progress.Retries} retried");
        }
    }
}
