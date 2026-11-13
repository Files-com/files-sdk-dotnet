using FilesCom.Transfers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace FilesCom.Http
{
    // Best-effort, in-memory protection for requests not carrying a stronger If-Match condition.
    // A date grants no multipart or checkpoint identity, and is never reset by URL renewal or retries.
    internal sealed class DownloadDateGuard
    {
        private readonly string path;
        private readonly bool enabled;
        private DateTimeOffset? baseline;

        internal DownloadDateGuard(string path, bool enabled)
        {
            this.path = path;
            this.enabled = enabled;
        }

        internal static bool Enabled(bool clientSetting, Dictionary<string, object> options)
        {
            if (options == null || !options.TryGetValue("check_download_modification_time", out object value) || value == null)
            {
                return clientSetting;
            }
            if (!(value is bool enabled))
            {
                throw InvalidParameterException.For("check_download_modification_time", "The download setting must be a Boolean");
            }
            return enabled;
        }

        internal void Apply(HttpRequestMessage request)
        {
            if (enabled && baseline.HasValue)
            {
                request.Headers.IfUnmodifiedSince = baseline.Value;
            }
        }

        // Capture before reading the body so that an interrupted response guards its retry too.
        // The platform parses HTTP dates; missing, malformed or duplicate headers supply no baseline.
        internal void Check(HttpResponseMessage response)
        {
            int status = (int)response.StatusCode;
            if (status == 412)
            {
                throw new DownloadSourceChangedException(path, "the storage server refused its precondition (HTTP 412)");
            }
            if (!enabled || (status != 200 && status != 206))
            {
                return;
            }
            if (response.Content == null || !response.Content.Headers.TryGetValues("Last-Modified", out var values) || values.Take(2).Count() != 1)
            {
                return;
            }
            DateTimeOffset? answered = response.Content.Headers.LastModified;
            if (answered.HasValue)
            {
                if (baseline.HasValue && answered.Value > baseline.Value)
                {
                    throw new DownloadSourceChangedException(path, "the storage server answered with a newer Last-Modified date");
                }
                baseline = baseline ?? answered;
            }
        }
    }
}