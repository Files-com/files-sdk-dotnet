using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace FilesCom.Http
{
    // What a storage server's answer to a download request must be before any of its body is used.
    internal static class DownloadResponse
    {
        // Set by the Files.com web app, which answers some URLs, such as an expired or unknown download link, with a page
        // of its own rather than the file.
        internal const string FrontendAppHeader = "X-Files-Frontend-App";

        // An answer from the Files.com web app, whatever its status or type, is never a file's contents.
        internal static void CheckNotFrontendApp(HttpResponseMessage response)
        {
            if (response.Headers.Contains(FrontendAppHeader) || (response.Content != null && response.Content.Headers.Contains(FrontendAppHeader)))
            {
                throw new InvalidResponseException($"The download was answered by the Files.com web app (HTTP {(int)response.StatusCode}), not with the file's contents, so nothing was written.");
            }
        }

        // A request for the whole file must be answered with all of it, which only 200 is: part of the file (206), which a
        // request without a Range header never asks for, and an answer that has no content (204) or is otherwise not the
        // resource itself cannot be taken for the whole file, however long its body.
        internal static void CheckWhole(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.PartialContent)
            {
                throw new InvalidResponseException("The download was answered with part of the file (HTTP 206) although the whole file was asked for, so nothing was written.");
            }
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new InvalidResponseException($"The download was answered with HTTP {(int)response.StatusCode}, not with the whole file (HTTP 200), so nothing was written.");
            }
            CheckIdentityEncoding(response);
        }

        // A body whose bytes are encoded, such as compressed, does not hold the file's bytes at their positions.
        internal static void CheckIdentityEncoding(HttpResponseMessage response)
        {
            ICollection<string> encodings = response.Content?.Headers.ContentEncoding;
            if (encodings != null && encodings.Any(encoding => !string.Equals(encoding, "identity", StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidResponseException($"The download was answered with its body encoded as {string.Join(", ", encodings)}, not as the file's bytes, so nothing was written.");
            }
        }

        // The length the answer declares for its body, if any.
        internal static long? ContentLength(HttpResponseMessage response)
        {
            return response.Content?.Headers.ContentLength;
        }
    }
}