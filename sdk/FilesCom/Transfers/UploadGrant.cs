using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace FilesCom.Transfers
{
    using FilesCom.Models;

    // What one upload may do, as the SDK reads it from the first begin_upload answer: whether it may choose unequal part
    // sizes, within which limits, and which performance defaults suit its destination. Nothing in the answer grants this
    // by name. The upload chooses its part sizes only where its parts go straight to Amazon S3 as an ordinary multipart
    // upload (AwsMultipartRoute), whose numbered parts may differ in size within S3's own rules and whose signed URLs bind
    // neither a part's length nor its bytes. There partsize and next_partsize are recommendations, and every part is sent
    // with a Content-Length of exactly its bytes. Everywhere else the upload sends the parts Files.com sizes. The limits
    // are the SDK's own planning limits for S3 (AwsLimits), within S3's: they are no promise of Files.com's, and grant
    // nothing beyond what Files.com and the destination enforce. The first answer is the grant for the whole upload: while
    // the upload chooses sizes, a later answer, for more parts or a renewed URL, must be for the same S3 upload, or the
    // upload is stopped before the part is sent.
    internal sealed class UploadGrant
    {
        // The SDK's planning limits for an ordinary multipart upload to Amazon S3: parts other than the last of at least
        // S3's 5 MiB; parts of at most 1 GiB, a fifth of S3's 5 GiB, which bounds what one part holds for its retries; at
        // most S3's 10,000 parts, or fewer when available_parts says so; and files of at most 5 TiB, the largest Files.com
        // plans uploads for. 10,000 parts of 1 GiB hold more than 5 TiB, so every size up to it has a plan.
        internal static readonly PartSizeLimits AwsLimits = new PartSizeLimits(5L * 1024 * 1024, 1L << 30, 10000, 5L << 40);

        // For the SDK's own tests: the limits an upload to Amazon S3 plans within instead of AwsLimits, for the uploads
        // within the flow of a test that sets it, so that a test can use small parts. Nothing else sets it.
        internal static readonly AsyncLocal<PartSizeLimits> AwsLimitsForTests = new AsyncLocal<PartSizeLimits>();

        private UploadGrant(PartSizeLimits limits, UploadPerformanceProfile profile, AwsMultipartRoute route)
        {
            Limits = limits;
            Profile = profile;
            Route = route;
        }

        // The limits for choosing part sizes, or null when Files.com sizes the parts.
        internal PartSizeLimits Limits { get; }

        // The destination the parts show, which chooses an automatic upload's default tuning: Agent when Files.com supplies
        // direct connection information for the upload (a Files.com Agent or Integration Worker destination), which comes
        // first, as in the Go SDK; S3 for an ordinary multipart upload to Amazon S3; Generic otherwise. Grants nothing.
        internal UploadPerformanceProfile Profile { get; }

        // The S3 upload the parts go to, or null.
        internal AwsMultipartRoute Route { get; }

        // The grant of one answer, whose first part is number firstNumber: chosen sizes only when every part of it is for
        // the same ordinary S3 multipart upload.
        internal static UploadGrant Of(FileUploadPart[] parts, long firstNumber)
        {
            AwsMultipartRoute route = AwsMultipartRoute.Of(parts[0], firstNumber);
            for (int i = 1; i < parts.Length && route != null; i++)
            {
                if (!route.SameUpload(AwsMultipartRoute.Of(parts[i], firstNumber + i)))
                {
                    route = null;
                }
            }
            if (parts.Any(HasDirectConnection))
            {
                // A direct destination, whatever its URLs look like: Files.com sizes the parts, and the Agent profile applies.
                return new UploadGrant(null, UploadPerformanceProfile.Agent, null);
            }
            return route == null
                ? new UploadGrant(null, UploadPerformanceProfile.Generic, null)
                : new UploadGrant(AwsLimitsForTests.Value ?? AwsLimits, UploadPerformanceProfile.S3, route);
        }

        // Whether Files.com supplied the part with complete direct connection information for a direct Agent transfer, as
        // the Go SDK judges it present: version 1, a server name, at least one address, a direct URI and a CA certificate.
        // Anything less is not a direct destination. This SDK does not send the part directly; the information only
        // identifies the destination.
        internal static bool HasDirectConnection(FileUploadPart part)
        {
            DirectConnectionInfo info = part.DirectConnectionInfo;
            return info != null && info.Version == 1 && !string.IsNullOrEmpty(info.ServerName) && info.Addresses != null && info.Addresses.Length > 0
                && !string.IsNullOrEmpty(info.DirectUri) && !string.IsNullOrEmpty(info.CaPem);
        }

        // Checks a later answer for a part against what the upload relies on: while it chooses part sizes, the part must
        // still go to the same S3 upload, with nothing that binds its length or bytes. With Files.com's sizes, nothing
        // more is relied on than each answer says itself.
        internal void CheckStillAllowed(FileUploadPart part, long number, bool choosingSizes)
        {
            if (choosingSizes && !Route.SameUpload(AwsMultipartRoute.Of(part, number)))
            {
                throw new InvalidResponseException($"For part {number}, the server gave a URL that is not for the same Amazon S3 multipart upload as the upload's other parts, or that fixes the part's length or bytes, which the upload chooses, so the upload was stopped before sending the part.");
            }
        }
    }

    // An ordinary multipart upload straight to Amazon S3, as one part's begin_upload answer shows it, or nothing. Every
    // condition is read from the answer as Files.com sent it, and all must hold; anything else, such as a host that only
    // resembles S3's, a custom or S3-compatible endpoint, a Files.com or Agent URL, an append or restart, or a parallel
    // flag on its own, is not this route:
    //
    //   action                  put/write: an ordinary write, not an append or restart, whose parts Files.com may map
    //                           to others
    //   http_method             PUT
    //   upload_uri              https, on the default port, without user information or fragment, to an S3
    //                           general-purpose endpoint (s3, s3.<region>, s3-<region> or s3.dualstack.<region>, under
    //                           amazonaws.com or amazonaws.com.cn, alone with the bucket in the path, or after it), and a
    //                           SigV4 presigned UploadPart query: partNumber once, naming this part's number; uploadId
    //                           once; X-Amz-Algorithm AWS4-HMAC-SHA256; X-Amz-Credential scoped to s3; X-Amz-SignedHeaders
    //                           host alone, so that neither length nor bytes are signed; X-Amz-Signature; and no checksum,
    //                           digest or length among its parameters
    //   headers                 none required with the part
    //   send                    exactly the part's size as its Content-Length, and its bytes as the body
    //   parameters              none
    //   direct_connection_info  none
    //
    // The URL is only read here, and sent exactly as given.
    internal sealed class AwsMultipartRoute
    {
        private static readonly string[] BindingQueryNames = { "x-amz-content-sha256", "x-amz-sdk-checksum-algorithm", "x-amz-decoded-content-length", "content-md5", "content-length" };

        private AwsMultipartRoute(string resource, string uploadId)
        {
            Resource = resource;
            UploadId = uploadId;
        }

        // The endpoint and object of the upload: the URL's text before its query.
        internal string Resource { get; }

        internal string UploadId { get; }

        // What a checkpoint records of the route: the SHA-256 of its endpoint, object and upload ID, not the URL.
        internal string Digest
        {
            get
            {
                using (SHA256 sha = SHA256.Create())
                {
                    var text = new StringBuilder("sha256:");
                    foreach (byte b in sha.ComputeHash(Encoding.UTF8.GetBytes(Resource + "\n" + UploadId)))
                    {
                        text.Append(b.ToString("x2", CultureInfo.InvariantCulture));
                    }
                    return text.ToString();
                }
            }
        }

        internal bool SameUpload(AwsMultipartRoute other)
        {
            return other != null && string.Equals(Resource, other.Resource, StringComparison.Ordinal) && string.Equals(UploadId, other.UploadId, StringComparison.Ordinal);
        }

        // The route of part number, as negotiated, or null when it is not this route.
        internal static AwsMultipartRoute Of(FileUploadPart part, long number)
        {
            if (part.Action != "put/write" || part.HttpMethod != "PUT" || part.DirectConnectionInfo != null || (part.PartNumber.HasValue && part.PartNumber.Value != number)
                || !IsEmpty(part.Headers) || !IsEmpty(part.Parameters) || !IsOrdinarySend(part.Send))
            {
                return null;
            }
            string text = part.UploadUri;
            if (string.IsNullOrEmpty(text) || text.IndexOf('#') >= 0 || !Uri.TryCreate(text, UriKind.Absolute, out Uri uri)
                || uri.Scheme != Uri.UriSchemeHttps || !uri.IsDefaultPort || uri.UserInfo.Length > 0 || !IsGeneralPurposeEndpoint(uri.Host) || uri.AbsolutePath.Length <= 1)
            {
                return null;
            }
            int query = text.IndexOf('?');
            if (query < 0)
            {
                return null;
            }
            var parameters = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            foreach (string pair in text.Substring(query + 1).Split('&'))
            {
                int equals = pair.IndexOf('=');
                if (equals <= 0)
                {
                    return null;
                }
                string name = Uri.UnescapeDataString(pair.Substring(0, equals));
                string lower = name.ToLowerInvariant();
                if (lower.StartsWith("x-amz-checksum", StringComparison.Ordinal) || Array.IndexOf(BindingQueryNames, lower) >= 0)
                {
                    return null;
                }
                if (!parameters.TryGetValue(name, out List<string> values))
                {
                    parameters[name] = values = new List<string>();
                }
                values.Add(Uri.UnescapeDataString(pair.Substring(equals + 1)));
            }
            string uploadId = Single(parameters, "uploadId");
            string credential = Single(parameters, "X-Amz-Credential");
            if (Single(parameters, "partNumber") != number.ToString(CultureInfo.InvariantCulture) || string.IsNullOrEmpty(uploadId)
                || Single(parameters, "X-Amz-Algorithm") != "AWS4-HMAC-SHA256" || credential == null || !credential.EndsWith("/s3/aws4_request", StringComparison.Ordinal)
                || Single(parameters, "X-Amz-SignedHeaders") != "host" || string.IsNullOrEmpty(Single(parameters, "X-Amz-Signature")))
            {
                return null;
            }
            return new AwsMultipartRoute(text.Substring(0, query), uploadId);
        }

        // s3, s3.<region>, s3-<region> or s3.dualstack.<region>, under amazonaws.com or amazonaws.com.cn, alone or after a
        // bucket of one or more labels. Not S3's accelerate, FIPS, website, access point, Object Lambda, Outposts, Express
        // or control endpoints, which this SDK does not plan uploads for.
        internal static bool IsGeneralPurposeEndpoint(string host)
        {
            host = host.ToLowerInvariant();
            string within = host.EndsWith(".amazonaws.com", StringComparison.Ordinal) ? host.Substring(0, host.Length - ".amazonaws.com".Length)
                : host.EndsWith(".amazonaws.com.cn", StringComparison.Ordinal) ? host.Substring(0, host.Length - ".amazonaws.com.cn".Length)
                : null;
            if (string.IsNullOrEmpty(within))
            {
                return false;
            }
            string[] labels = within.Split('.');
            int endpoint = Array.LastIndexOf(labels, "s3");
            if (endpoint >= 0)
            {
                int after = labels.Length - endpoint - 1;
                bool known = after == 0 || (after == 1 && IsRegion(labels[endpoint + 1])) || (after == 2 && labels[endpoint + 1] == "dualstack" && IsRegion(labels[endpoint + 2]));
                if (!known)
                {
                    return false;
                }
            }
            else
            {
                endpoint = labels.Length - 1;
                if (!labels[endpoint].StartsWith("s3-", StringComparison.Ordinal) || !IsRegion(labels[endpoint].Substring(3)))
                {
                    return false;
                }
            }
            for (int i = 0; i < endpoint; i++)
            {
                if (labels[i].Length == 0)
                {
                    return false;
                }
            }
            return true;
        }

        // A region such as us-east-1, eu-central-2 or us-gov-west-1: two letters, words of letters, then a number.
        private static bool IsRegion(string label)
        {
            string[] words = label.Split('-');
            if (words.Length < 3 || words[0].Length != 2)
            {
                return false;
            }
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length == 0)
                {
                    return false;
                }
                foreach (char c in words[i])
                {
                    bool expected = i == words.Length - 1 ? c >= '0' && c <= '9' : c >= 'a' && c <= 'z';
                    if (!expected)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static string Single(Dictionary<string, List<string>> parameters, string name)
        {
            return parameters.TryGetValue(name, out List<string> values) && values.Count == 1 ? values[0] : null;
        }

        private static bool IsEmpty(object field)
        {
            return field == null || (field is JsonElement json && (json.ValueKind == JsonValueKind.Null || (json.ValueKind == JsonValueKind.Object && !json.EnumerateObject().MoveNext())));
        }

        // Exactly the part's size as its Content-Length and its bytes as the body.
        private static bool IsOrdinarySend(object send)
        {
            if (!(send is JsonElement json) || json.ValueKind != JsonValueKind.Object)
            {
                return false;
            }
            int count = 0;
            foreach (JsonProperty instruction in json.EnumerateObject())
            {
                count++;
                bool ordinary = instruction.Value.ValueKind == JsonValueKind.String
                    && ((instruction.Name == "partsize" && string.Equals(instruction.Value.GetString(), "required-header Content-Length", StringComparison.OrdinalIgnoreCase))
                        || (instruction.Name == "partdata" && instruction.Value.GetString() == "body"));
                if (!ordinary)
                {
                    return false;
                }
            }
            return count == 2;
        }
    }
}