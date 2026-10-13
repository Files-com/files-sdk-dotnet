using FilesCom.Util;
using System;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace FilesCom
{
    /// <summary>
    /// The HTTP handlers that send a client's requests, for an application that needs its own proxy, TLS settings,
    /// certificate checks or instrumentation. Give one to <see cref="FilesClient.Create(FilesConfiguration, HttpTransport)"/>
    /// or the <see cref="FilesClient(FilesConfiguration, HttpTransport)"/> constructor.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The handlers are the bottom of the client's HTTP pipeline. Everything the SDK decides stays above them and
    /// applies to all of the client's requests, to the Files.com API and to the storage URLs of uploads and downloads
    /// alike: the endpoint and credentials of each request, which redirects to follow and which headers go with
    /// them, retries, timeouts and cancellation.
    /// </para>
    /// <para>
    /// The SDK follows redirects itself, so that credentials never go to another origin, and needs handlers that
    /// do not. It turns off automatic redirects on the handlers <see cref="FromHandlerFactory"/> creates, and
    /// refuses a shared <see cref="HttpClientHandler"/> or <c>SocketsHttpHandler</c> that follows them. A handler of
    /// another type must not follow redirects or retry requests itself: the SDK sends a request that can change
    /// something, such as a DELETE or the one that completes an upload, once per attempt, and again only where
    /// Files.com redirects it, and cannot know that a handler sent one again. It must also end a request when the
    /// request's cancellation token is cancelled.
    /// </para>
    /// <para>
    /// <see cref="FilesConfiguration.ConnectTimeout"/> applies to the handlers the SDK creates by default on .NET 8
    /// and later, and to a <c>SocketsHttpHandler</c> from <see cref="FromHandlerFactory"/> whose own
    /// <c>ConnectTimeout</c> is not set. It is not applied to other handlers, nor to a shared handler, whose settings
    /// the SDK never changes: they connect as they are configured, with their own connection settings in effect.
    /// With any handler, connecting also counts toward the client's <see cref="FilesConfiguration.ReadTimeout"/> or
    /// <see cref="FilesConfiguration.WriteTimeout"/> wait that follows it, and whichever limit runs out first ends
    /// the attempt.
    /// </para>
    /// </remarks>
    public sealed class HttpTransport
    {
        private static readonly SdkLog log = SdkLog.For(typeof(HttpTransport));
        // The handlers seen not to check certificates as a client sent through them (HasAlwaysCheckedCertificates).
        private static readonly ConditionalWeakTable<HttpMessageHandler, object> relaxed = new ConditionalWeakTable<HttpMessageHandler, object>();

        private readonly Func<HttpMessageHandler> createHandler;
        private readonly HttpMessageHandler sharedHandler;
        // The handlers createHandler has returned, so that one returned twice is refused rather than disposed twice.
        private readonly ConditionalWeakTable<HttpMessageHandler, object> created = new ConditionalWeakTable<HttpMessageHandler, object>();

        private HttpTransport(Func<HttpMessageHandler> createHandler, HttpMessageHandler sharedHandler)
        {
            this.createHandler = createHandler;
            this.sharedHandler = sharedHandler;
        }

        /// <summary>
        /// Handlers that the client creates with <paramref name="createHandler"/> and owns.
        /// </summary>
        /// <param name="createHandler">
        /// Returns a new handler each time it is called, such as an <see cref="HttpClientHandler"/> with a
        /// <c>Proxy</c>, or a <see cref="DelegatingHandler"/> chain that ends in one. The client calls it when it needs
        /// a handler, including when it replaces its handlers from time to time, as it does to pick up DNS changes.
        /// </param>
        /// <returns>A transport that can be given to any number of clients; each creates handlers of its own.</returns>
        /// <remarks>
        /// The client configures each handler before its first request: it turns off automatic redirects and, on a
        /// <c>SocketsHttpHandler</c> whose <c>ConnectTimeout</c> is not set, applies <see cref="FilesConfiguration.ConnectTimeout"/>;
        /// a <c>ConnectTimeout</c> the handler already has is kept.
        /// It disposes each handler when it no longer uses it, and every handler it still has when it is disposed,
        /// once its operations have ended. Configure a handler fully before returning it: once it has sent a request
        /// with settings that do not check certificates as .NET does, its answers never move an operation to another
        /// region automatically (<see cref="RegionRelocationPolicy"/>), whatever its settings are changed to. A factory
        /// that returns null, a handler it returned before, or a <see cref="DelegatingHandler"/> chain without an inner
        /// handler fails the operation that needed the handler with <see cref="InvalidParameterException"/> naming
        /// <paramref name="createHandler"/>, before its request is sent.
        /// </remarks>
        /// <exception cref="MissingParameterException"><paramref name="createHandler"/> is null.</exception>
        public static HttpTransport FromHandlerFactory(Func<HttpMessageHandler> createHandler)
        {
            if (createHandler == null)
            {
                throw MissingParameterException.For(nameof(createHandler));
            }
            return new HttpTransport(createHandler, null);
        }

        /// <summary>
        /// A handler that the application owns and shares, with other clients or its own code.
        /// </summary>
        /// <param name="handler">
        /// The handler, or a <see cref="DelegatingHandler"/> chain, to send every request through. An
        /// <see cref="HttpClientHandler"/> or <c>SocketsHttpHandler</c> in it must have <c>AllowAutoRedirect</c> set to
        /// false.
        /// </param>
        /// <returns>A transport that can be given to any number of clients.</returns>
        /// <remarks>
        /// Clients never change or dispose the handler, even when they are disposed. Dispose it yourself once every
        /// client that uses it has been disposed. Its own settings, including how long it takes to connect, apply as
        /// they are. A client cannot see how the handler checked the servers it connected to before the client had
        /// it, or for other code, so its answers move an operation to another region only between a pair of
        /// <see cref="RegionRelocationPolicy.OnlyBetween"/>.
        /// </remarks>
        /// <exception cref="MissingParameterException"><paramref name="handler"/> is null.</exception>
        /// <exception cref="InvalidParameterException">
        /// The handler follows redirects itself, or a <see cref="DelegatingHandler"/> in it has no inner handler.
        /// </exception>
        public static HttpTransport FromSharedHandler(HttpMessageHandler handler)
        {
            if (handler == null)
            {
                throw MissingParameterException.For(nameof(handler));
            }
            HttpMessageHandler primary = PrimaryOf(handler) ?? throw InvalidParameterException.For(nameof(handler), "A DelegatingHandler in the handler chain has no InnerHandler");
            if (FollowsRedirects(primary))
            {
                throw InvalidParameterException.For(nameof(handler), $"The shared {primary.GetType().Name} follows redirects itself, which would send Files.com credentials to other origins. Set its AllowAutoRedirect to false: the SDK follows redirects itself");
            }
            return new HttpTransport(null, handler);
        }

        // Whether the handlers belong to the application, which disposes them.
        internal bool IsShared => sharedHandler != null;

        // The handler for one of a client's HTTP pipelines. For a factory, a new handler the client owns, configured
        // for it; for a shared handler, the handler itself.
        internal HttpMessageHandler CreateHandler(TimeSpan connectTimeout)
        {
            if (sharedHandler != null)
            {
                return sharedHandler;
            }
            // What the factory returns is the application's argument, so a handler the client cannot use is reported as
            // createHandler's, before any request is sent through it.
            HttpMessageHandler handler = createHandler() ?? throw InvalidParameterException.For(nameof(createHandler), "The HttpTransport handler factory returned null");
            lock (created)
            {
                // Already owned, by the pipeline it was returned to first, which disposes it.
                if (created.TryGetValue(handler, out _))
                {
                    throw InvalidParameterException.For(nameof(createHandler), "The HttpTransport handler factory returned a handler it had returned before. It must return a new handler each time: the client disposes the handlers it creates");
                }
                created.Add(handler, null);
            }
            // From here the handler is the client's, so one it cannot use is disposed, not left to the application.
            try
            {
                HttpMessageHandler primary = PrimaryOf(handler) ?? throw InvalidParameterException.For(nameof(createHandler), "A DelegatingHandler in the chain the HttpTransport handler factory returned has no InnerHandler");
                Configure(primary, connectTimeout);
                return handler;
            }
            catch
            {
                Release(handler);
                throw;
            }
        }

        // Disposes a handler the client could not use. A failure to dispose it is logged; the reason it could not be
        // used is what the caller sees.
        private static void Release(HttpMessageHandler handler)
        {
            try
            {
                handler.Dispose();
            }
            catch (Exception e)
            {
                DiagnosticText.WarnCleanupFailed(log, "Setting up an HTTP handler: disposing a handler that could not be used failed", e);
            }
        }

        // The handlers the SDK sends with when it is given none: its own, on .NET 8 and later with ConnectTimeout.
        internal static HttpMessageHandler CreateDefaultHandler(TimeSpan connectTimeout)
        {
#if NETCOREAPP2_1_OR_GREATER
            return new SocketsHttpHandler { AllowAutoRedirect = false, ConnectTimeout = connectTimeout };
#else
            return new HttpClientHandler { AllowAutoRedirect = false };
#endif
        }

        private static void Configure(HttpMessageHandler primary, TimeSpan connectTimeout)
        {
            switch (primary)
            {
                case HttpClientHandler handler:
                    handler.AllowAutoRedirect = false;
                    break;
#if NETCOREAPP2_1_OR_GREATER
                case SocketsHttpHandler handler:
                    handler.AllowAutoRedirect = false;
                    if (handler.ConnectTimeout == System.Threading.Timeout.InfiniteTimeSpan)
                    {
                        handler.ConnectTimeout = connectTimeout;
                    }
                    break;
#endif
            }
        }

        private static bool FollowsRedirects(HttpMessageHandler primary)
        {
            switch (primary)
            {
                case HttpClientHandler handler:
                    return handler.AllowAutoRedirect;
#if NETCOREAPP2_1_OR_GREATER
                case SocketsHttpHandler handler:
                    return handler.AllowAutoRedirect;
#endif
                default:
                    return false;
            }
        }

        // Whether handler, which a client owns, has checked the certificates of HTTPS servers as .NET does
        // (ChecksCertificates) at every request the client has sent through it, from its first, which the client sees
        // since it configures the handler before then: configuring one that has already sent a request fails. A
        // handler's settings can be changed after its first request without changing the connections it has made, as
        // an application that kept the handler, or its chain policy, can; so a handler once seen not to check never
        // counts as checking again. Kept for the handler at the bottom of the chain, whichever chains it is in.
        internal static bool HasAlwaysCheckedCertificates(HttpMessageHandler handler)
        {
            HttpMessageHandler primary = PrimaryOf(handler);
            if (primary == null || relaxed.TryGetValue(primary, out _))
            {
                return false;
            }
            if (ChecksCertificates(primary))
            {
                return true;
            }
            relaxed.GetValue(primary, _ => null);
            return false;
        }

        // Whether the handler at the bottom of handler's chain checks the certificates of HTTPS servers as .NET does,
        // against the system's trusted roots or a custom trust store, with no callback that could accept others and no
        // setting that relaxes the check or the encryption (ChecksAsTheSystemDoes). A handler of another type, such as
        // one of an application's own, cannot be seen to.
        private static bool ChecksCertificates(HttpMessageHandler handler)
        {
            try
            {
                switch (PrimaryOf(handler))
                {
                    case HttpClientHandler clientHandler:
#if NETCOREAPP2_1_OR_GREATER
                        return clientHandler.ServerCertificateCustomValidationCallback == null;
#else
                        // On .NET Framework, the process-wide callback decides too.
                        return clientHandler.ServerCertificateCustomValidationCallback == null && System.Net.ServicePointManager.ServerCertificateValidationCallback == null;
#endif
#if NETCOREAPP2_1_OR_GREATER
                    case SocketsHttpHandler socketsHandler:
                        return socketsHandler.SslOptions.RemoteCertificateValidationCallback == null && ChecksAsTheSystemDoes(socketsHandler.SslOptions);
#endif
                    default:
                        return false;
                }
            }
            catch (PlatformNotSupportedException)
            {
                return false;
            }
        }

#if NETCOREAPP2_1_OR_GREATER
        // The flags that let a chain be accepted when its revocation status cannot be found. They relax nothing the
        // system's own default checks: SocketsHttpHandler does not check revocation unless told to.
        private const System.Security.Cryptography.X509Certificates.X509VerificationFlags RevocationUnknown =
            System.Security.Cryptography.X509Certificates.X509VerificationFlags.IgnoreEndRevocationUnknown |
            System.Security.Cryptography.X509Certificates.X509VerificationFlags.IgnoreCtlSignerRevocationUnknown |
            System.Security.Cryptography.X509Certificates.X509VerificationFlags.IgnoreCertificateAuthorityRevocationUnknown |
            System.Security.Cryptography.X509Certificates.X509VerificationFlags.IgnoreRootRevocationUnknown;

        // Whether TLS options check a server as strictly as the system does: they require encryption and, when they
        // have a chain policy of their own, it trusts only its roots (the system's, or a custom trust store) and waives
        // no other check, such as an unknown authority, an expired or not yet valid certificate, a wrong usage or an
        // invalid name constraint, nor verifies at a fixed time instead of now. Only the revocation flags above are
        // allowed. The protocol versions and cipher suites allowed change how the connection is encrypted, not who the
        // server is, and are not considered.
        private static bool ChecksAsTheSystemDoes(System.Net.Security.SslClientAuthenticationOptions options)
        {
#pragma warning disable SYSLIB0040 // EncryptionPolicy: the policies that allow no encryption are what is looked for.
            if (options.EncryptionPolicy != System.Net.Security.EncryptionPolicy.RequireEncryption)
            {
                return false;
            }
#pragma warning restore SYSLIB0040
#if NET7_0_OR_GREATER
            System.Security.Cryptography.X509Certificates.X509ChainPolicy policy = options.CertificateChainPolicy;
            if (policy != null && ((policy.VerificationFlags & ~RevocationUnknown) != 0 || !policy.VerificationTimeIgnored))
            {
                return false;
            }
#endif
            return true;
        }
#endif

        // The handler at the bottom of a chain of delegating handlers, or null when the chain has no bottom.
        private static HttpMessageHandler PrimaryOf(HttpMessageHandler handler)
        {
            while (handler is DelegatingHandler delegating)
            {
                handler = delegating.InnerHandler;
            }
            return handler;
        }
    }
}