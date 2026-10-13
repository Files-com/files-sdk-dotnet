using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FilesCom;
using FilesCom.Transfers;

namespace TransferUploads
{
    // A stream upload with progress and a time limit. The upload borrows the stream: it reads it forward and never
    // closes it, so the stream is still yours afterward, whatever happened.
    internal static class CancelWithProgress
    {
        internal static async Task<int> UploadAsync(FilesClient client, string localPath, string remotePath, TimeSpan timeLimit, CancellationToken cancellationToken)
        {
            using (var withinTimeLimit = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            using (FileStream source = File.OpenRead(localPath))
            {
                withinTimeLimit.CancelAfter(timeLimit);
                var options = new UploadOptions { MkdirParents = true, Progress = new ConsoleProgress() };
                try
                {
                    UploadResult result = await client.Transfers.UploadStreamAsync(source, source.Length, remotePath, options, withinTimeLimit.Token);
                    Console.WriteLine($"Uploaded {result.Path} within {timeLimit.TotalSeconds} s.");
                    return 0;
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    // Every part in flight has stopped and the upload has released what it held. If the time ran out
                    // while the request completing the upload was on its way, Files.com may still have completed it.
                    Console.WriteLine($"Stopped after {timeLimit.TotalSeconds} s. {remotePath} is not complete unless the final request had already reached Files.com; check it before uploading again. The stream is still open: {source.CanRead}.");
                    return 3;
                }
            }
        }
    }
}
