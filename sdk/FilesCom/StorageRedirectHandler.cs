using FilesCom.Http;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom
{
    // Follows the redirects of upload part requests (client.Transfers). A redirect is followed only when the part can
    // go on unchanged: the same method and the same bytes, which 307 and 308 keep, as do 301 and 302 of a request that
    // is not a POST. A redirect that would turn the part into a GET (303, or 301 or 302 of a POST), leaves http and
    // https, or goes from HTTPS to HTTP is not followed; its response is returned and the part fails.
    //
    // The headers the server negotiated for the part go with it within the same origin. To another origin, the
    // credential headers do not, nor does a Host header, which names the old origin. Every hop resends the whole part,
    // within the same attempt: a redirect is not a failed attempt, so it does not use up the retry limit, and it is
    // followed whether or not the server allows retrying parts.
    //
    // Download requests of client.Transfers go through here too, and follow redirects the same way. A request marked
    // StaysAtResource, such as a download's request bound to a version by its If-Match, or for a range, must be answered
    // by the resource it was sent to, whose version it names or binds: its redirect, to any URL, is not followed, and its
    // response is returned instead.
    internal sealed class StorageRedirectHandler : DelegatingHandler
    {
        internal const int MaxRedirects = 5;

        private static readonly string[] OriginHeaders = { "Authorization", "Proxy-Authorization", "Cookie", "Host" };

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpRequestMessage hop = FilesRedirectHandler.CloneRequest(request);
            HttpResponseMessage response = await base.SendAsync(hop, cancellationToken);
            for (int redirects = 0; redirects < MaxRedirects && KeepsThePart(hop.Method, response); redirects++)
            {
                Uri from = hop.RequestUri;
                Uri to = FilesRedirectHandler.ResolveRedirectUri(from, response.Headers.Location);
                if (!MaySendTo(from, to) || RequestMarks.Has(hop, RequestMarks.StaysAtResource))
                {
                    break;
                }
                if (!FilesRedirectHandler.IsSameOrigin(from, to))
                {
                    foreach (string header in OriginHeaders)
                    {
                        hop.Headers.Remove(header);
                    }
                }
                hop.RequestUri = to;
                response.Dispose();
                response = await base.SendAsync(hop, cancellationToken);
            }
            return response;
        }

        private static bool KeepsThePart(HttpMethod method, HttpResponseMessage response)
        {
            int status = (int)response.StatusCode;
            if (response.Headers.Location == null)
            {
                return false;
            }
            return status == 307 || status == 308 || ((status == 301 || status == 302) && method != HttpMethod.Post);
        }

        private static bool MaySendTo(Uri from, Uri to)
        {
            bool toHttps = string.Equals(to.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase);
            bool toHttp = string.Equals(to.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase);
            bool fromHttps = string.Equals(from.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase);
            return toHttps || (toHttp && !fromHttps);
        }
    }
}