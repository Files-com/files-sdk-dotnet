using FilesCom.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FilesCom
{
    /// <summary>
    /// Whether a client sends an operation on to the address Files.com names when it answers that the site is served
    /// from another region, with the error <see cref="LockoutRegionMismatchException"/>. Set it as
    /// <see cref="FilesConfiguration.RegionRelocation"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// With <see cref="Automatic"/>, the default, the client sends the request again, with the same credentials and
    /// body, to the site's address, when:
    /// </para>
    /// <list type="bullet">
    /// <item><description>
    /// the answer came from the address the request was sent to, the client's <see cref="FilesConfiguration.BaseUrl"/>,
    /// over HTTPS whose certificate the client's handler checked as strictly as .NET does by default, against the
    /// system's trusted roots or a custom trust store, with no validation callback and no chain policy that waives a
    /// check, such as <c>AllowUnknownCertificateAuthority</c>, at every request since the handler's first, as the
    /// client saw its settings when each request started and when its answer arrived; and not after a redirect to
    /// another origin. The handler is the client's own, or one that
    /// <see cref="HttpTransport.FromHandlerFactory"/> returned: the client cannot see how a shared handler checked
    /// the servers it connected to before, or for other code;
    /// </description></item>
    /// <item><description>
    /// the address it names is a DNS name or an HTTPS origin, with no path, on port 443, with a public DNS name, such
    /// as the site's own domain, and is not the address already used;
    /// </description></item>
    /// <item><description>
    /// the operation has not moved already, and has a retry left: moving is one of the operation's
    /// <see cref="FilesConfiguration.MaxNetworkRetries"/> retries, so with no retries the error is thrown.
    /// </description></item>
    /// </list>
    /// <para>
    /// The operation keeps the new address for its remaining requests, and so do the objects and lists it returns, for
    /// their own operations and next pages. The client's configuration is unchanged, and every other operation starts
    /// at BaseUrl. When the operation does not move, the error is thrown with the address Files.com named, as
    /// <see cref="LockoutRegionMismatchException.RequestedOrigin"/>, and the reason, as
    /// <see cref="LockoutRegionMismatchException.Refusal"/>.
    /// </para>
    /// </remarks>
    public sealed class RegionRelocationPolicy
    {
        private readonly bool automatic;
        private readonly IReadOnlyList<KeyValuePair<string, string>> routes;

        private RegionRelocationPolicy(bool automatic, IReadOnlyList<KeyValuePair<string, string>> routes)
        {
            this.automatic = automatic;
            this.routes = routes;
        }

        /// <summary>
        /// Moves an operation to its site's address when Files.com answers with it, as described for
        /// <see cref="RegionRelocationPolicy"/>. The default.
        /// </summary>
        public static RegionRelocationPolicy Automatic { get; } = new RegionRelocationPolicy(true, new KeyValuePair<string, string>[0]);

        /// <summary>
        /// Never moves an operation: the error is thrown, with the address Files.com named, for the application to
        /// act on, such as by changing its <see cref="FilesConfiguration.BaseUrl"/>.
        /// </summary>
        public static RegionRelocationPolicy Disabled { get; } = new RegionRelocationPolicy(false, new KeyValuePair<string, string>[0]);

        /// <summary>
        /// Moves an operation only from <paramref name="fromOrigin"/> to <paramref name="toOrigin"/>, and from the first
        /// to the second origin of each pair <see cref="OrBetween"/> adds. A pair moves one way: add the reverse with
        /// <see cref="OrBetween"/> to move back.
        /// </summary>
        /// <param name="fromOrigin">
        /// The origin the answer must come from, such as <c>https://files.example.com</c>: a client's
        /// <see cref="FilesConfiguration.BaseUrl"/>, or the address it has moved an object or list to.
        /// </param>
        /// <param name="toOrigin">The origin Files.com must name, such as <c>https://files.example.eu:8443</c>.</param>
        /// <returns>A policy that allows this move only.</returns>
        /// <remarks>
        /// A listed pair allows what <see cref="Automatic"/> never does: a port other than 443, a name that is not
        /// public, an IP address or plain HTTP, as a development site may need, and an answer over a handler whose
        /// certificate checks the client cannot see or that relaxes them, such as a shared handler, one of a type of
        /// your own, one with a validation callback, or one whose chain policy waives a check or did since its first
        /// request. Listing a pair states that both origins serve Files.com for the client's credentials: the answer
        /// must still come from <paramref name="fromOrigin"/>, and not after a redirect to another origin, and the
        /// move still takes a retry.
        /// </remarks>
        /// <exception cref="MissingParameterException">An origin is null.</exception>
        /// <exception cref="InvalidParameterException">
        /// An origin is not an HTTP or HTTPS origin: a scheme, a host and an optional port, with no user information,
        /// path, query or fragment.
        /// </exception>
        public static RegionRelocationPolicy OnlyBetween(string fromOrigin, string toOrigin)
        {
            return new RegionRelocationPolicy(false, new[] { Route(fromOrigin, toOrigin) });
        }

        /// <summary>
        /// This policy's pairs, and one more: a policy that also moves an operation from <paramref name="fromOrigin"/>
        /// to <paramref name="toOrigin"/>. Only a policy made with <see cref="OnlyBetween"/> has pairs.
        /// </summary>
        /// <param name="fromOrigin">The origin the answer must come from, as for <see cref="OnlyBetween"/>.</param>
        /// <param name="toOrigin">The origin Files.com must name, as for <see cref="OnlyBetween"/>.</param>
        /// <returns>A policy that allows this policy's moves and this one.</returns>
        /// <exception cref="MissingParameterException">An origin is null.</exception>
        /// <exception cref="InvalidParameterException">An origin is not an HTTP or HTTPS origin.</exception>
        /// <exception cref="InvalidOperationException">This policy is <see cref="Automatic"/> or <see cref="Disabled"/>.</exception>
        public RegionRelocationPolicy OrBetween(string fromOrigin, string toOrigin)
        {
            if (routes.Count == 0)
            {
                throw new InvalidOperationException("Pairs of origins can only be added to a policy made with RegionRelocationPolicy.OnlyBetween.");
            }
            return new RegionRelocationPolicy(false, routes.Concat(new[] { Route(fromOrigin, toOrigin) }).ToArray());
        }

        internal bool IsDisabled => !automatic && routes.Count == 0;

        internal bool IsAutomatic => automatic;

        // Whether a pair lists this move, between canonical origins.
        internal bool Lists(string fromOrigin, string toOrigin)
        {
            return routes.Any(route => route.Key == fromOrigin && route.Value == toOrigin);
        }

        /// <summary>The policy, such as <c>Automatic</c> or <c>OnlyBetween(https://a.example.com -&gt; https://b.example.com)</c>.</summary>
        public override string ToString()
        {
            return automatic ? "Automatic" : routes.Count == 0 ? "Disabled" : $"OnlyBetween({string.Join(", ", routes.Select(route => route.Key + " -> " + route.Value))})";
        }

        private static KeyValuePair<string, string> Route(string fromOrigin, string toOrigin)
        {
            return new KeyValuePair<string, string>(CheckedOrigin(fromOrigin, nameof(fromOrigin)), CheckedOrigin(toOrigin, nameof(toOrigin)));
        }

        private static string CheckedOrigin(string origin, string argument)
        {
            if (origin == null)
            {
                throw MissingParameterException.For(argument);
            }
            return Origins.Parse(origin, bareHost: false)
                ?? throw InvalidParameterException.For(argument, $"\"{origin}\" is not an origin, such as https://files.example.com or http://localhost:3000: a scheme, a host and an optional port, with no user information, path, query or fragment");
        }
    }

    /// <summary>Why a client did not move an operation to the address Files.com named for its site.</summary>
    public enum RegionRelocationRefusal
    {
        /// <summary><see cref="FilesConfiguration.RegionRelocation"/> is <see cref="RegionRelocationPolicy.Disabled"/>.</summary>
        Disabled,

        /// <summary>The operation had moved to another address once already.</summary>
        AlreadyRelocated,

        /// <summary>
        /// The answer cannot be trusted to name the site's address, and the error's message says why: it was not a
        /// 403, or it came from another address than the operation sent the request to, or after a redirect to another
        /// origin, none of which a listed pair changes; or it did not come over HTTPS whose certificate the client's
        /// handler has checked as strictly as .NET does by default since its first request, such as over plain HTTP or
        /// through a shared handler, and no pair of <see cref="RegionRelocationPolicy.OnlyBetween"/> lists the move.
        /// </summary>
        UntrustedIssuer,

        /// <summary>
        /// The answer named no address, or one that is not a DNS name or an origin with no path: such as one with user
        /// information, a query, a fragment or a scheme other than HTTP or HTTPS.
        /// </summary>
        InvalidHost,

        /// <summary>The answer named the address the operation already sent the request to.</summary>
        SameOrigin,

        /// <summary>
        /// The policy does not move there: <see cref="RegionRelocationPolicy.Automatic"/> moves only to HTTPS on port
        /// 443 with a public DNS name, and <see cref="RegionRelocationPolicy.OnlyBetween"/> only between the origins
        /// it lists.
        /// </summary>
        NotAllowed,

        /// <summary>
        /// The operation had no retry left, of its <see cref="FilesConfiguration.MaxNetworkRetries"/>, to move with.
        /// </summary>
        RetriesExhausted,
    }
}