using System;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;

namespace FilesCom.Http
{
    // Whether an operation moves to the address a LockoutRegionMismatchException names, by the operation's
    // RegionRelocationPolicy. Files.com answers 403 with the type not-authenticated/lockout-region-mismatch when a
    // site is served from another region's address, and names it in data.host: the site's domain, or its full URL.
    //
    // Only the address the request went to may send the operation elsewhere, since the operation already trusts it with
    // its credentials: the answer must come from the operation's current origin, not after a redirect to another origin,
    // and over HTTPS whose certificate the client's handler checked, unless a listed pair vouches for the move. A
    // move is a retry of the operation, with no wait: it needs a retry left and uses one, and happens once at most.
    internal static class RegionRelocation
    {
        // The failure an attempt ends with for a mismatch: a Requested move, which RetryPolicy retries at once, or the
        // mismatch refused, with why.
        internal static Exception Consider(LockoutRegionMismatchException mismatch, OperationContext context)
        {
            string origin = Origins.Parse(HostOf(mismatch), bareHost: true);
            RegionRelocationPolicy policy = context.RegionRelocation;
            string current = Origins.Of(new Uri(context.Origin));
            RegionRelocationRefusal? refusal = null;
            UntrustedAnswer? untrusted = null;
            if (policy.IsDisabled)
            {
                refusal = RegionRelocationRefusal.Disabled;
            }
            else if (context.HasRelocated)
            {
                refusal = RegionRelocationRefusal.AlreadyRelocated;
            }
            else if ((untrusted = Untrusted(mismatch, current, origin, policy)) != null)
            {
                refusal = RegionRelocationRefusal.UntrustedIssuer;
            }
            else if (origin == null)
            {
                refusal = RegionRelocationRefusal.InvalidHost;
            }
            else if (origin == current)
            {
                refusal = RegionRelocationRefusal.SameOrigin;
            }
            else if (!(policy.IsAutomatic ? Origins.IsPublicHttps(origin) : policy.Lists(current, origin)))
            {
                refusal = RegionRelocationRefusal.NotAllowed;
            }
            return refusal.HasValue ? mismatch.Refused(refusal.Value, origin, untrusted) : (Exception)new Requested(mismatch, origin);
        }

        // Why the answer cannot be trusted to name the site's address, or null when it can. It must be Files.com's 403,
        // from the operation's current origin and not after a redirect to another origin, whatever pairs the policy
        // lists; and over HTTPS whose certificate the client's handler checks, unless a listed pair vouches for the move.
        private static UntrustedAnswer? Untrusted(LockoutRegionMismatchException mismatch, string current, string origin, RegionRelocationPolicy policy)
        {
            if (mismatch.httpStatus != 403)
            {
                return UntrustedAnswer.NotForbidden;
            }
            if (mismatch.AnsweredFrom == null)
            {
                return UntrustedAnswer.UnknownSender;
            }
            if (mismatch.AnsweredAfterCrossingOrigins)
            {
                return UntrustedAnswer.AfterRedirect;
            }
            if (Origins.Of(mismatch.AnsweredFrom) != current)
            {
                return UntrustedAnswer.OtherOrigin;
            }
            if (Checked(mismatch) || (origin != null && policy.Lists(current, origin)))
            {
                return null;
            }
            return IsHttps(mismatch.AnsweredFrom) ? UntrustedAnswer.UncheckedHandler : UntrustedAnswer.PlainHttp;
        }

        // An answer over HTTPS, whose certificate the handler that received it checks.
        private static bool Checked(LockoutRegionMismatchException mismatch)
        {
            return mismatch.AnsweredOverCheckedTls && IsHttps(mismatch.AnsweredFrom);
        }

        private static bool IsHttps(Uri address)
        {
            return string.Equals(address.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase);
        }

        private static string HostOf(LockoutRegionMismatchException mismatch)
        {
            object host = null;
            if (mismatch.data?.TryGetValue("host", out host) != true)
            {
                return null;
            }
            switch (host)
            {
                case JsonElement element when element.ValueKind == JsonValueKind.String:
                    return element.GetString();
                case string text:
                    return text;
                default:
                    return null;
            }
        }

        // Why an answer cannot be trusted to name the site's address (RegionRelocationRefusal.UntrustedIssuer): the first
        // of these that holds, in the order Untrusted checks them.
        internal enum UntrustedAnswer
        {
            // Another status than the 403 Files.com names a site's address in.
            NotForbidden,

            // A response that does not say which request it answers, and so where it came from.
            UnknownSender,

            // After a redirect to another origin.
            AfterRedirect,

            // From another origin than the one the request was sent to.
            OtherOrigin,

            // Over plain HTTP, whose server the client cannot verify, and no listed pair vouches for the move.
            PlainHttp,

            // Over HTTPS through a handler the client does not see check certificates, and no listed pair vouches for
            // the move.
            UncheckedHandler,
        }

        // A move the policy allows: the operation's next attempt goes to Origin, if it has a retry left.
        internal sealed class Requested : Exception
        {
            internal Requested(LockoutRegionMismatchException mismatch, string origin)
                : base($"Files.com answered that the site is served from {origin}")
            {
                Mismatch = mismatch;
                Origin = origin;
            }

            internal LockoutRegionMismatchException Mismatch { get; }

            internal string Origin { get; }
        }
    }

    // Origins, as a scheme, a host and a port, compared as text in one canonical form: lowercase, the host in ASCII
    // (punycode), and the port only when it is not the scheme's default.
    internal static class Origins
    {
        // Names that are never a public site's: RFC 6761 and 6762 special-use names and private-use top-level names.
        private static readonly string[] NonPublicSuffixes = { "localhost", "local", "test", "invalid", "example", "internal", "home.arpa", "onion" };

        internal static string Of(Uri uri)
        {
            string scheme = uri.Scheme.ToLowerInvariant();
            string host = uri.HostNameType == UriHostNameType.IPv6 ? uri.Host.ToLowerInvariant() : uri.IdnHost.ToLowerInvariant();
            return uri.IsDefaultPort ? $"{scheme}://{host}" : $"{scheme}://{host}:{uri.Port.ToString(CultureInfo.InvariantCulture)}";
        }

        // The canonical origin of text: an HTTP or HTTPS origin with no user information, query or fragment and a path
        // of at most "/", or, when bareHost, also a host with nothing else, which is HTTPS on port 443. Null for
        // anything else, including a name that is not valid IDNA, a host that ends in a dot, whitespace, control
        // characters and backslashes. Text is parsed once, as it is: nothing is decoded or guessed.
        internal static string Parse(string text, bool bareHost)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            foreach (char c in text)
            {
                if (char.IsWhiteSpace(c) || char.IsControl(c) || c == '\\' || c == '@' || c == '?' || c == '#' || c == '%')
                {
                    return null;
                }
            }
            string authority;
            string scheme;
            int separator = text.IndexOf("://", StringComparison.Ordinal);
            if (separator < 0)
            {
                if (!bareHost || text.IndexOfAny(new[] { ':', '/', '[' }) >= 0)
                {
                    return null;
                }
                scheme = Uri.UriSchemeHttps;
                authority = text;
            }
            else
            {
                scheme = text.Substring(0, separator).ToLowerInvariant();
                if (scheme != Uri.UriSchemeHttps && scheme != Uri.UriSchemeHttp)
                {
                    return null;
                }
                string rest = text.Substring(separator + 3);
                int slash = rest.IndexOf('/');
                if (slash >= 0 && slash != rest.Length - 1)
                {
                    return null;
                }
                authority = slash >= 0 ? rest.Substring(0, slash) : rest;
            }
            if (authority.Length == 0 || authority.EndsWith(".", StringComparison.Ordinal) || authority.Contains(".:")
                || !Uri.TryCreate(scheme + "://" + authority + "/", UriKind.Absolute, out Uri uri)
                || uri.HostNameType == UriHostNameType.Unknown || uri.HostNameType == UriHostNameType.Basic)
            {
                return null;
            }
            if (uri.HostNameType == UriHostNameType.Dns)
            {
                try
                {
                    new IdnMapping().GetAscii(uri.Host);
                }
                catch (ArgumentException)
                {
                    return null;
                }
            }
            return Of(uri);
        }

        // An origin an automatic move may go to: HTTPS on port 443, with a DNS name of more than one label that is not
        // special-use or private.
        internal static bool IsPublicHttps(string origin)
        {
            var uri = new Uri(origin);
            if (uri.Scheme != Uri.UriSchemeHttps || uri.Port != 443 || uri.HostNameType != UriHostNameType.Dns)
            {
                return false;
            }
            string host = uri.IdnHost.ToLowerInvariant();
            if (host.IndexOf('.') < 0)
            {
                return false;
            }
            foreach (string suffix in NonPublicSuffixes)
            {
                if (host == suffix || host.EndsWith("." + suffix, StringComparison.Ordinal))
                {
                    return false;
                }
            }
            return true;
        }
    }

    // What the SDK's own pipeline records on a request as it sends it, for the answer to be judged by: whether a
    // redirect took it to another origin, and whether the handler that sent it checks server certificates.
    internal static class RequestMarks
    {
        internal const string CrossedOrigins = "FilesCom.CrossedOrigins";
        internal const string ChecksCertificates = "FilesCom.ChecksCertificates";

        // A storage request whose answer must come from the very resource it was sent to, such as a download's request
        // carrying If-Match, which names that resource's version, or for a range of it: its redirects are not followed
        // (StorageRedirectHandler).
        internal const string StaysAtResource = "FilesCom.StaysAtResource";

        internal static void Set(HttpRequestMessage request, string mark)
        {
#if NET5_0_OR_GREATER
            request.Options.Set(new HttpRequestOptionsKey<bool>(mark), true);
#else
            request.Properties[mark] = true;
#endif
        }

        internal static bool Has(HttpRequestMessage request, string mark)
        {
            if (request == null)
            {
                return false;
            }
#if NET5_0_OR_GREATER
            return request.Options.TryGetValue(new HttpRequestOptionsKey<bool>(mark), out bool set) && set;
#else
            return request.Properties.TryGetValue(mark, out object set) && set is bool marked && marked;
#endif
        }
    }
}