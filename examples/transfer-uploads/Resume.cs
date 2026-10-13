using System;
using System.Threading;
using System.Threading.Tasks;
using FilesCom;
using FilesCom.Transfers;

namespace TransferUploads
{
    // One file with a checkpoint: stop it with Ctrl+C, or let it fail, and run the same command again to continue it.
    // Parts Files.com accepted are not sent again; the checkpoint is deleted once the file is completed. An upload that
    // cannot be continued, because the file changed or Files.com no longer knows it, fails with TransferCheckpointException
    // (the default CheckpointRestartPolicy): starting over is left to the person running the command.
    internal static class Resume
    {
        internal static async Task UploadAsync(FilesClient client, string localPath, string remotePath, string checkpointPath, CancellationToken cancellationToken)
        {
            var options = new UploadOptions { MkdirParents = true, CheckpointPath = checkpointPath, Progress = new ConsoleProgress() };

            UploadResult result = await client.Transfers.UploadFileAsync(localPath, remotePath, options, cancellationToken);

            Console.WriteLine(result.ResumedParts > 0
                ? $"Uploaded {result.Path}: continued with {result.ResumedParts} parts ({result.ResumedBytes} bytes) an earlier run had sent."
                : $"Uploaded {result.Path}: {result.Size} bytes in {result.Parts.Count} parts.");
        }
    }
}
