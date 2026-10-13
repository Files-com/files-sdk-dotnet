using System;
using System.Threading;
using System.Threading.Tasks;
using FilesCom;
using FilesCom.Transfers;

namespace TransferDownloads
{
    // A whole remote folder, four files at a time, all within one TransferLimiter of six part requests. With a
    // checkpoint directory, running the same command again after Ctrl+C skips the files already downloaded, while
    // neither they nor their remote files have changed, and continues the ones in progress.
    internal static class Folder
    {
        internal static async Task DownloadAsync(FilesClient client, string remoteFolder, string localDirectory, string checkpointDirectory, DownloadPublicationPolicy policy, CancellationToken cancellationToken)
        {
            var options = new DirectoryDownloadOptions
            {
                MaxConcurrentFiles = 4,
                FileOptions = new DownloadOptions { PublicationPolicy = policy },
                Limiter = new TransferLimiter(maxConcurrentRequests: 6),
                CheckpointDirectory = checkpointDirectory,
                Progress = new FolderProgress(),
            };

            DirectoryTransferResult result = await client.Transfers.DownloadDirectoryAsync(remoteFolder, localDirectory, options, cancellationToken);

            Console.WriteLine($"Downloaded {result.Files.Count} files ({result.Bytes} bytes) to {result.LocalDirectory} in {result.Elapsed.TotalSeconds:0.0} s; " +
                $"{result.FinalProgress.FilesCompletedEarlier} were downloaded by an earlier run, {result.Directories.Count} empty folders created.");
        }

        private sealed class FolderProgress : IProgress<DirectoryTransferProgress>
        {
            public void Report(DirectoryTransferProgress progress)
            {
                Console.WriteLine($"  {progress.FilesDone} of {progress.FilesTotal} files, {progress.BytesDone} of {progress.BytesTotal} bytes, " +
                    $"{progress.FilesActive} in progress, {progress.BytesPerSecond / (1024 * 1024):0.0} MiB/s");
            }
        }
    }
}
