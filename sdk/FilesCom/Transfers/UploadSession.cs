using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Transfers
{
    using FilesCom.Models;

    // One upload's requests to the Files.com API: its begin_upload negotiations and the final request that completes
    // it. Every request uses the client context and request options the upload started with, and the destination path
    // exactly as the caller spelled it. The part requests themselves go to the negotiated URLs, not through here.
    internal sealed class UploadSession
    {
        private readonly OperationContext context;
        private readonly string path;
        private readonly Dictionary<string, object> requestOptions;
        private readonly bool? mkdirParents;

        internal UploadSession(OperationContext context, string path, Dictionary<string, object> requestOptions, bool? mkdirParents)
        {
            this.context = context;
            this.path = path;
            this.requestOptions = requestOptions;
            this.mkdirParents = mkdirParents;
        }

        internal string Path => path;

        // The upload's reference, which every later request carries. Set by the first negotiation.
        internal string Ref { get; private set; }

        // Starts the upload and negotiates its first part. size is left out for a stream of unknown length, which
        // Files.com then plans part by part.
        internal async Task<FileUploadPart[]> BeginAsync(long? size, CancellationToken cancellationToken)
        {
            Dictionary<string, object> parameters = Parameters();
            if (size.HasValue)
            {
                parameters["size"] = size.Value;
            }
            // Sent again after a lost answer, like every negotiation: an upload Files.com began but this SDK never heard
            // of is never completed, and a part's URL can be issued again.
            FileUploadPart[] parts = await RemoteFile.BeginUploadCore(context.ForReplaySafeRequests(), path, parameters, requestOptions, cancellationToken);
            if (parts == null || parts.Length == 0 || string.IsNullOrEmpty(parts[0].Ref))
            {
                throw new InvalidResponseException("The server did not start the upload: its response has no upload reference.");
            }
            Ref = parts[0].Ref;
            return parts;
        }

        // Continues an upload an earlier run started, with the reference Files.com gave it.
        internal void Resume(string reference)
        {
            Ref = reference;
        }

        // Negotiates count parts from firstPart on, or a new URL for one part already planned.
        internal Task<FileUploadPart[]> ContinueAsync(long firstPart, long count, CancellationToken cancellationToken)
        {
            Dictionary<string, object> parameters = Parameters();
            parameters["ref"] = Ref;
            parameters["part"] = firstPart;
            if (count > 1)
            {
                parameters["parts"] = count;
            }
            return RemoteFile.BeginUploadCore(context.ForReplaySafeRequests(), path, parameters, requestOptions, cancellationToken);
        }

        // Completes the upload. The part list must be every part, in order. ETags are sent when every part returned
        // one and left out when none did, which destinations that accept parts one at a time without ETags allow; a
        // mixture cannot describe the upload consistently, so it is refused before anything is sent.
        //
        // The request is sent without the client's automatic retries (a redirect Files.com answers it with is still
        // followed, as for any request, within the attempt): Files.com discards an upload's reference
        // when it completes it, so a repeat after a lost answer would report a completed upload as not found. Only an
        // answer that shows it was not carried out, a trusted move to the site's region (RetryPolicy.NotCarriedOut),
        // means nothing was completed; any other failure after sending, even an error such as a 422, which Files.com
        // also answers after completing the file in storage, leaves the outcome unknown, reported as
        // UploadCompletionUnknownException with the failure inside.
        internal async Task<RemoteFile> FinishAsync(long size, IReadOnlyList<UploadedPart> parts, DateTime? modifiedTime, CancellationToken cancellationToken)
        {
            var parameters = new Dictionary<string, object>
            {
                { "action", "end" },
                { "ref", Ref },
                { "size", size },
            };
            if (modifiedTime.HasValue)
            {
                parameters["provided_mtime"] = modifiedTime.Value.ToUniversalTime().ToString("u", CultureInfo.InvariantCulture);
            }

            List<UploadedPart> withoutETag = parts.Where(part => part.ETag == null).ToList();
            if (withoutETag.Count == 0)
            {
                parameters["etags"] = parts.Select(part => new Dictionary<string, object> { { "etag", part.ETag }, { "part", part.Number } }).ToList();
            }
            else if (withoutETag.Count < parts.Count)
            {
                throw new InvalidResponseException($"The server accepted {withoutETag.Count} of the {parts.Count} parts (the first is part {withoutETag[0].Number}) without an ETag, and the rest with one, so the upload cannot be completed.");
            }

            try
            {
                return await RemoteFile.CreateCore(context.WithoutRetries(), path, parameters, requestOptions, cancellationToken);
            }
            catch (Exception e) when (!(e is OperationCanceledException) && !NotSent(e) && !WasRefused(e))
            {
                throw new UploadCompletionUnknownException(path, size, parts, e);
            }
        }

        // Incorrect use, found before the final request is sent.
        private static bool NotSent(Exception exception)
        {
            return exception is ArgumentException || exception is InvalidParameterException || exception is MissingParameterException || exception is AuthenticationException;
        }

        // Files.com answered in a way that shows it did not complete the upload.
        private static bool WasRefused(Exception exception)
        {
            return exception is ApiException error && !error.RequestMayHaveBeenProcessed;
        }

        private Dictionary<string, object> Parameters()
        {
            var parameters = new Dictionary<string, object>();
            if (mkdirParents.HasValue)
            {
                parameters["mkdir_parents"] = mkdirParents.Value;
            }
            // Asked for with every negotiation, as the Go SDK asks: Files.com includes it for a destination it can be sent
            // to directly, a Files.com Agent or Integration Worker, and the upload's default tuning is chosen by it
            // (UploadGrant.Profile). The parts are still sent to the URLs negotiated.
            parameters["with_direct_connection_info"] = true;
            return parameters;
        }
    }
}