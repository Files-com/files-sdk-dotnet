using FilesCom.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace FilesCom.Transfers
{
    // The version of a file that a download is bound to, as its storage answered for it: the strong entity tag the
    // storage server sent with the first part, which every later request carries in If-Match; the file's size, which
    // that answer's Content-Range stated; and the storage resource that answer came from (Source), which the tag
    // belongs to. It is the SDK's own record of what storage answered, not anything Files.com's API describes, and it
    // grants nothing by itself: no signature or credential is part of it.
    //
    // A download is bound only where the URL its first answer came from names storage known to check If-Match against
    // the entity tag of the very object it names, and to answer 412 when that object no longer has it (CanBind): Amazon
    // S3, where a GET of one object is answered from one version of it, reached over HTTPS, which authenticates the
    // server its host names. Elsewhere, such as a server that reads the file for Files.com, nothing shows that every
    // part came from one version, so the file comes in one request. The entity tag need not be a digest of the bytes: it
    // is compared as storage sent it, and only for the resource it came from, since the same text from another resource
    // proves nothing about this one.
    internal sealed class DownloadVersion
    {
        internal const string IfMatchHeader = "If-Match";

        private const string ETagHeader = "ETag";

        private const string SourcePrefix = "sha256:";

        internal DownloadVersion(string etag, long size, string source)
        {
            ETag = etag;
            Size = size;
            Source = source;
        }

        // The strong entity tag ("…", not W/"…"), exactly as storage sent it, which every request carries in If-Match.
        internal string ETag { get; }

        // The file's size in bytes, at least 1: an empty file has no part to bind it.
        internal long Size { get; }

        // The storage resource the entity tag belongs to (SourceOf).
        internal string Source { get; }

        internal IReadOnlyList<KeyValuePair<string, string>> Headers => new[] { new KeyValuePair<string, string>(IfMatchHeader, ETag) };

        // Whether a response's entity tag is this version's: compared strongly, so a weak tag never matches.
        internal bool Matches(string etag)
        {
            return etag != null && !etag.StartsWith("W/", StringComparison.Ordinal) && string.Equals(etag, ETag, StringComparison.Ordinal);
        }

        // Whether url is for the storage resource this version came from.
        internal bool IsSourceOf(SignedUrl url)
        {
            return string.Equals(SourceOf(url), Source, StringComparison.Ordinal);
        }

        // Whether downloads from url can be bound to a version: whether it is an HTTPS URL, on the default port and
        // without user information, of an object at an Amazon S3 general-purpose endpoint, as its host is structured
        // (AwsMultipartRoute.IsGeneralPurposeEndpoint: s3, s3.<region>, s3-<region> or s3.dualstack.<region>, under
        // amazonaws.com or amazonaws.com.cn, alone with the bucket in the path, or after it), the forms Files.com's
        // storage URLs take. A host that only contains an s3 label, such as a load balancer whose customer-chosen name
        // starts with s3-, is not S3, and HTTP authenticates no server. S3's other endpoints (FIPS, accelerate, access
        // points, Object Lambda, websites and others) are not qualified here, and come in one request, as other storage
        // does. Asked of every URL a download is to be bound from, as renewed.
        internal static bool CanBind(SignedUrl url)
        {
            return Uri.TryCreate(url.Text, UriKind.Absolute, out Uri uri)
                && uri.Scheme == Uri.UriSchemeHttps && uri.IsDefaultPort && uri.UserInfo.Length == 0 && uri.AbsolutePath.Length > 1
                && AwsMultipartRoute.IsGeneralPurposeEndpoint(uri.Host);
        }

        // The storage resource url names: its text before the query and fragment, which hold the signature and its
        // expiry, and change with every URL Files.com gives for the same object. Kept as its SHA-256, so that no part of
        // a URL, which may hold a credential in its path, is recorded.
        internal static string SourceOf(SignedUrl url)
        {
            string text = url.Text;
            int end = text.IndexOfAny(new[] { '?', '#' });
            string resource = end < 0 ? text : text.Substring(0, end);
            using (SHA256 sha = SHA256.Create())
            {
                byte[] digest = sha.ComputeHash(Encoding.UTF8.GetBytes(resource));
                return SourcePrefix + string.Concat(digest.Select(b => b.ToString("x2", System.Globalization.CultureInfo.InvariantCulture)));
            }
        }

        // The response's entity tag, as sent, or null when it has none, or more than one.
        internal static string EntityTagOf(HttpResponseMessage response)
        {
            if (!response.Headers.TryGetValues(ETagHeader, out IEnumerable<string> values))
            {
                return null;
            }
            List<string> tags = values.ToList();
            return tags.Count == 1 ? tags[0] : null;
        }

        // entity-tag = opaque-tag = DQUOTE *etagc DQUOTE, etagc = %x21 / %x23-7E / obs-text (RFC 9110), without the
        // weak prefix. obs-text is not accepted: a header value is sent as it is given, and only ASCII is certain to be.
        internal static bool IsStrongEntityTag(string value)
        {
            if (value == null || value.Length < 2 || value[0] != '"' || value[value.Length - 1] != '"')
            {
                return false;
            }
            for (int i = 1; i < value.Length - 1; i++)
            {
                char c = value[i];
                if (c != 0x21 && (c < 0x23 || c > 0x7E))
                {
                    return false;
                }
            }
            return true;
        }
    }
}