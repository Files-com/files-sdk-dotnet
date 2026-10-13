using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FilesCom;
using FilesCom.Transfers;

namespace TransferUploads
{
    // Standard input, whose length is not known until it ends, such as the output of another program. Each part is kept
    // in a temporary file of its own until Files.com accepts it, so parts can be as large as the limits allow without
    // being held in memory. Where the parts go straight to Amazon S3, the upload chooses their sizes, which grow while
    // larger parts raise throughput, and parts go several at a time; otherwise Files.com plans each part, and they go
    // one at a time.
    internal static class UnknownLength
    {
        internal static async Task UploadAsync(FilesClient client, string remotePath, long? maxBytes, CancellationToken cancellationToken)
        {
            var options = new UploadOptions
            {
                MkdirParents = true,
                Progress = new ConsoleProgress(),
                StreamPartStorage = StreamPartStorage.TemporaryFiles,
                // A declared bound lets parts start and stay smaller: every part must leave room, in the parts Files.com
                // still allows, for the most the stream may hold. A longer stream fails the upload, which is not completed.
                MaxStreamLength = maxBytes,
            };

            using (Stream input = Console.OpenStandardInput())
            {
                UploadResult result = await client.Transfers.UploadStreamAsync(input, remotePath, options, cancellationToken);

                UploadPartSizing sizing = result.FinalProgress.PartSizing;
                Console.WriteLine($"Uploaded {result.Path}: {result.Size} bytes read from standard input, in {result.Parts.Count} parts.");
                Console.WriteLine(sizing.Source == PartSizeSource.ServerPlanned
                    ? $"Part sizes: planned by Files.com ({sizing.ServerPlannedReason}), sent {(result.ParallelParts ? "in parallel" : "one at a time")}."
                    : $"Part sizes: chosen within at most {sizing.MaxStreamLength} bytes; largest {sizing.LargestPartSize} bytes, grown {sizing.Grows} times.");
            }
        }
    }
}
