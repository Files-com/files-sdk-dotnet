using System;
using System.Threading;
using System.Threading.Tasks;
using FilesCom;
using FilesCom.Transfers;

namespace TransferUploads
{
    // A whole local directory, four files at a time, all within one TransferLimiter of six part requests. With a
    // checkpoint directory, running the same command again after Ctrl+C skips the files already uploaded and continues
    // the ones in progress.
    internal static class Folder
    {
        internal static async Task UploadAsync(FilesClient client, string localDirectory, string remoteFolder, string checkpointDirectory, CancellationToken cancellationToken)
        {
            var options = new DirectoryUploadOptions
            {
                MaxConcurrentFiles = 4,
                Limiter = new TransferLimiter(maxConcurrentRequests: 6),
                CheckpointDirectory = checkpointDirectory,
                Progress = new DirectoryProgress(),
            };

            DirectoryTransferResult result = await client.Transfers.UploadDirectoryAsync(localDirectory, remoteFolder, options, cancellationToken);

            Console.WriteLine($"Uploaded {result.Files.Count} files ({result.Bytes} bytes) to {result.RemoteDirectory} in {result.Elapsed.TotalSeconds:0.0} s; " +
                $"{result.FinalProgress.FilesCompletedEarlier} were uploaded by an earlier run, {result.Directories.Count} empty folders created.");
            foreach (string excluded in result.Excluded)
            {
                Console.WriteLine($"  left out {excluded}: not a regular file or folder");
            }
        }
    }

    // Prints the whole transfer's progress, at most a few times a second.
    internal sealed class DirectoryProgress : IProgress<DirectoryTransferProgress>
    {
        public void Report(DirectoryTransferProgress progress)
        {
            Console.WriteLine($"  {progress.FilesDone} of {progress.FilesTotal} files, {progress.BytesDone} of {progress.BytesTotal} bytes, " +
                $"{progress.FilesActive} in progress, {progress.BytesPerSecond / (1024 * 1024):0.0} MiB/s");
        }
    }
}
