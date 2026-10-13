using System;
using System.Threading;
using System.Threading.Tasks;
using FilesCom;
using FilesCom.Transfers;

namespace TransferUploads
{
    // One file, sent in parallel parts when Files.com allows it, with a line of progress per accepted part. The upload
    // chooses how many parts to send at once from what it measures, never more than six.
    internal static class OneFile
    {
        internal static async Task UploadAsync(FilesClient client, string localPath, string remotePath, CancellationToken cancellationToken)
        {
            var options = new UploadOptions
            {
                MaxConcurrency = 6,
                MkdirParents = true,
                Progress = new ConsoleProgress(),
            };

            UploadResult result = await client.Transfers.UploadFileAsync(localPath, remotePath, options, cancellationToken);

            string how = result.ParallelParts ? "in parallel" : "one at a time";
            UploadConcurrency concurrency = result.FinalProgress.Concurrency;
            Console.WriteLine($"Uploaded {result.Path}: {result.Size} bytes in {result.Parts.Count} parts, sent {how}; " +
                $"{result.SentBytes} bytes written to the network in {result.Elapsed.TotalSeconds:0.0} s.");
            Console.WriteLine($"Concurrency: ended at {concurrency.Target} parts at once, at most {concurrency.PeakActiveAttempts} in flight; " +
                $"grew {concurrency.Grows} times, lowered {concurrency.Shrinks} times, throttled {concurrency.Throttles} times; " +
                $"{concurrency.Profile} defaults, from {(concurrency.ProfileSource == UploadProfileSource.Server ? "the destination its part URLs showed" : "the options")}.");
            UploadPartSizing sizing = result.FinalProgress.PartSizing;
            Console.WriteLine(sizing.Source == PartSizeSource.ServerPlanned
                ? $"Part sizes: planned by Files.com ({sizing.ServerPlannedReason})."
                : $"Part sizes: chosen, largest {sizing.LargestPartSize} bytes; grew {sizing.Grows} times, {sizing.ProbesRejected} larger sizes given up.");
        }
    }
}
