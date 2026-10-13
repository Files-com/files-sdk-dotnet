using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FilesCom;
using FilesCom.Transfers;

namespace TransferUploads
{
    // Several files at once, all within one TransferLimiter: at most six part requests and 32 MiB of buffers
    // between them, however many files there are. Each file's result is reported on its own, so one failure does not
    // hide the others.
    internal static class SharedLimit
    {
        internal static async Task<int> UploadAsync(FilesClient client, string remoteFolder, string[] localPaths, CancellationToken cancellationToken)
        {
            var limiter = new TransferLimiter(maxConcurrentRequests: 6, maxBufferedBytes: 32 * 1024 * 1024);
            var options = new UploadOptions { Limiter = limiter, MkdirParents = true };

            Task<string>[] uploads = localPaths
                .Select(localPath => UploadOneAsync(client, localPath, remoteFolder + "/" + Path.GetFileName(localPath), options, cancellationToken))
                .ToArray();
            Task all = Task.WhenAll(uploads);
            while (await Task.WhenAny(all, Task.Delay(500)) != all)
            {
                Console.WriteLine($"  limiter: {limiter.ActiveRequests} requests in flight, {limiter.QueuedRequests} parts waiting, {limiter.BufferedBytes} bytes buffered");
            }

            foreach (Task<string> upload in uploads)
            {
                Console.WriteLine(upload.Result);
            }
            cancellationToken.ThrowIfCancellationRequested();
            return uploads.All(upload => upload.Result.StartsWith("Uploaded")) ? 0 : 1;
        }

        private static async Task<string> UploadOneAsync(FilesClient client, string localPath, string remotePath, UploadOptions options, CancellationToken cancellationToken)
        {
            try
            {
                UploadResult result = await client.Transfers.UploadFileAsync(localPath, remotePath, options, cancellationToken);
                return $"Uploaded {result.Path}: {result.Size} bytes in {result.Parts.Count} parts";
            }
            catch (Exception e) when (e is SdkException || e is IOException)
            {
                return $"Failed {remotePath}: {e.Message}";
            }
            catch (OperationCanceledException)
            {
                return $"Cancelled {remotePath}";
            }
        }
    }
}
