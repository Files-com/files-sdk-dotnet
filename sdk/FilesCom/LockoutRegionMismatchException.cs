using FilesCom.Http;
using System;
using System.Net.Http;

namespace FilesCom
{
    /// <summary>
    /// Files.com answered that the site is served from another region's address, which it names. A client moves
    /// the operation there itself when its <see cref="FilesConfiguration.RegionRelocation"/> allows; this error is
    /// thrown when it does not, with the address and why.
    /// </summary>
    public partial class LockoutRegionMismatchException
    {
        /// <summary>
        /// The origin Files.com named for the site, such as <c>https://files.example.com</c>, or null when it named none
        /// that is an origin.
        /// </summary>
        public string RequestedOrigin { get; private set; }

        /// <summary>
        /// Why the client did not move the operation to <see cref="RequestedOrigin"/>, or null when this error was not
        /// the answer to a Files.com API request of a client's own, such as an error of a transfer to storage.
        /// </summary>
        public RegionRelocationRefusal? Refusal { get; private set; }

        // Where the answer came from: the address of the request that received it, and whether the SDK's pipeline
        // recorded that the request crossed origins in a redirect, or went through a handler that checks certificates.
        internal Uri AnsweredFrom { get; private set; }

        internal bool AnsweredAfterCrossingOrigins { get; private set; }

        internal bool AnsweredOverCheckedTls { get; private set; }

        internal void AnsweredTo(HttpRequestMessage request)
        {
            AnsweredFrom = request?.RequestUri;
            AnsweredAfterCrossingOrigins = RequestMarks.Has(request, RequestMarks.CrossedOrigins);
            AnsweredOverCheckedTls = RequestMarks.Has(request, RequestMarks.ChecksCertificates);
        }

        // The same error, refused a move to origin, which is null when the answer named no origin: its message says
        // why, after the error text as Files.com sent it. For UntrustedIssuer, untrusted says which condition failed.
        internal LockoutRegionMismatchException Refused(RegionRelocationRefusal refusal, string origin, RegionRelocation.UntrustedAnswer? untrusted = null)
        {
            string target = origin != null ? $"to {origin}" : "elsewhere";
            string why;
            switch (refusal)
            {
                case RegionRelocationRefusal.Disabled:
                    why = $"The request was not sent {target}: FilesConfiguration.RegionRelocation is Disabled.";
                    break;
                case RegionRelocationRefusal.AlreadyRelocated:
                    why = $"The request was not sent {target}: its operation had moved to another address already.";
                    break;
                case RegionRelocationRefusal.UntrustedIssuer:
                    why = $"The request was not sent {target}: {Untrusted(untrusted.Value, origin)}";
                    break;
                case RegionRelocationRefusal.InvalidHost:
                    why = "The request was not sent again: the answer named no address that is a DNS name or an origin without a path.";
                    break;
                case RegionRelocationRefusal.SameOrigin:
                    why = "The request was not sent again: the answer named the address the request was sent to.";
                    break;
                case RegionRelocationRefusal.NotAllowed:
                    why = $"The request was not sent {target}: FilesConfiguration.RegionRelocation does not allow it. Automatic moves go only to HTTPS on port 443 with a public DNS name; RegionRelocationPolicy.OnlyBetween allows exact pairs of origins.";
                    break;
                default:
                    why = $"The request was not sent {target}: moving takes one of the operation's retries, and it had none left.";
                    break;
            }
            string message = string.IsNullOrEmpty(Message) ? why : Message.TrimEnd() + " " + why;
            return new LockoutRegionMismatchException(message, httpStatus, responseError, httpHeaders)
            {
                RequestedOrigin = origin,
                Refusal = refusal,
            };
        }

        // Why the answer could not be trusted to name the site's address. A pair of RegionRelocationPolicy.OnlyBetween
        // vouches only for an answer the client cannot verify, from the address the request was sent to, so only then
        // does the message say what a pair would do.
        private string Untrusted(RegionRelocation.UntrustedAnswer why, string origin)
        {
            switch (why)
            {
                case RegionRelocation.UntrustedAnswer.NotForbidden:
                    return $"the answer was HTTP {httpStatus}, and Files.com names a site's address only in a 403.";
                case RegionRelocation.UntrustedAnswer.UnknownSender:
                    return "the response did not say which request it answered, so the client cannot tell which address sent it.";
                case RegionRelocation.UntrustedAnswer.AfterRedirect:
                    return "the answer came after a redirect to another origin, not straight from the address the request was sent to.";
                case RegionRelocation.UntrustedAnswer.OtherOrigin:
                    return $"the answer came from {Origins.Of(AnsweredFrom)}, not from the address the request was sent to.";
                case RegionRelocation.UntrustedAnswer.PlainHttp:
                    return "the answer came over plain HTTP, whose server the client cannot verify." + ListedPair(origin);
                default:
                    return "the answer came over HTTPS through a handler the client does not see check certificates as .NET does: a shared handler, one with a validation callback or a chain policy that waives a check, one of a type the client cannot inspect, or one that has sent a request with such settings." + ListedPair(origin);
            }
        }

        // What listing a pair would do for an unverified answer from the address the request was sent to.
        private string ListedPair(string origin)
        {
            string from = Origins.Of(AnsweredFrom);
            if (origin == null)
            {
                return " It also named no address that is a DNS name or an origin without a path, so there is nowhere to move.";
            }
            if (origin == from)
            {
                return " It also named the address the request was sent to, so there is nowhere to move.";
            }
            return $" If both addresses serve Files.com for the client's credentials, RegionRelocationPolicy.OnlyBetween(\"{from}\", \"{origin}\") allows this move.";
        }
    }
}