using System;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom
{
    public class FilesRedirectHandler : DelegatingHandler
    {
        private const int MaxRedirects = 50;

        private static readonly string[] FilesAuthHeaders =
        {
            "X-FilesAPI-Key",
            "X-FilesAPI-Auth",
            "X-Files-Workspace-Id"
        };

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            HttpRequestMessage redirectRequest = CloneRequest(request);
            int redirectCount = 0;
            HttpResponseMessage response = await base.SendAsync(redirectRequest, cancellationToken);

            while (IsRedirect(response.StatusCode) && response.Headers.Location != null)
            {
                if (redirectCount >= MaxRedirects)
                {
                    return response;
                }

                Uri requestUri = redirectRequest.RequestUri;
                Uri redirectUri = ResolveRedirectUri(requestUri, response.Headers.Location);
                if (!ShouldFollowRedirect(requestUri, redirectUri))
                {
                    return response;
                }

                redirectRequest.Headers.Remove("Authorization");
                if (!IsSameOrigin(requestUri, redirectUri))
                {
                    foreach (string header in FilesAuthHeaders)
                    {
                        redirectRequest.Headers.Remove(header);
                    }
                }

                int statusCode = (int)response.StatusCode;
                bool redirectToGet = (statusCode == 301 || statusCode == 302) && redirectRequest.Method == HttpMethod.Post;
                redirectToGet = redirectToGet || (statusCode == 303 && redirectRequest.Method != HttpMethod.Get && redirectRequest.Method != HttpMethod.Head);
                if (redirectToGet)
                {
                    redirectRequest.Method = HttpMethod.Get;
                    redirectRequest.Content = null;
                    redirectRequest.Headers.TransferEncodingChunked = false;
                }

                redirectRequest.RequestUri = redirectUri;
                redirectCount++;
                response.Dispose();
                response = await base.SendAsync(redirectRequest, cancellationToken);
            }

            return response;
        }

        private static HttpRequestMessage CloneRequest(HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri)
            {
                Content = request.Content,
                Version = request.Version
            };
#if NET5_0_OR_GREATER
            clone.VersionPolicy = request.VersionPolicy;
#endif
            foreach (var header in request.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
#if NET5_0_OR_GREATER
            foreach (var option in request.Options)
            {
                clone.Options.Set(new HttpRequestOptionsKey<object>(option.Key), option.Value);
            }
#else
            foreach (var property in request.Properties)
            {
                clone.Properties.Add(property);
            }
#endif
            return clone;
        }

        private static bool IsRedirect(HttpStatusCode statusCode)
        {
            int code = (int)statusCode;
            return code == 300 || code == 301 || code == 302 || code == 303 || code == 307 || code == 308;
        }

        private static bool IsSameOrigin(Uri first, Uri second)
        {
            return string.Equals(first.Scheme, second.Scheme, StringComparison.OrdinalIgnoreCase)
                && string.Equals(first.IdnHost, second.IdnHost, StringComparison.OrdinalIgnoreCase)
                && first.Port == second.Port;
        }

        private static Uri ResolveRedirectUri(Uri requestUri, Uri location)
        {
            Uri redirectUri = location.IsAbsoluteUri ? location : new Uri(requestUri, location);
            if (string.IsNullOrEmpty(redirectUri.Fragment) && !string.IsNullOrEmpty(requestUri.Fragment))
            {
                var redirectUriBuilder = new UriBuilder(redirectUri)
                {
                    Fragment = requestUri.Fragment.Substring(1)
                };
                redirectUri = redirectUriBuilder.Uri;
            }
            return redirectUri;
        }

        private static bool ShouldFollowRedirect(Uri requestUri, Uri redirectUri)
        {
            bool isHttp = string.Equals(redirectUri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase);
            bool isHttps = string.Equals(redirectUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase);
            if (!isHttp && !isHttps)
            {
                return false;
            }

            bool isHttpsDowngrade = string.Equals(requestUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
                && isHttp;

            return !isHttpsDowngrade
                || RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase);
        }
    }
}