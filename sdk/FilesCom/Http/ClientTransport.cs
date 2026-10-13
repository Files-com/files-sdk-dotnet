using FilesCom.Util;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Http
{
    // One client's HTTP pipelines, made by an IHttpClientFactory of its own:
    //
    //   FilesClient.HttpFilesApi   API requests and downloads   FilesRedirectHandler, then a transport handler
    //   FilesClient.HttpUpload     upload parts                 StorageRedirectHandler, then a transport handler
    //
    // Neither retries or times out: the SDK does both above them. The factory replaces a pipeline's handlers every
    // two minutes, its default, so that new connections pick up DNS changes, and disposes the old ones once no
    // request uses them. Handlers of a shared transport belong to the application and are never replaced or disposed.
    //
    // The client owns the service provider and every other handler, and DisposeAsync, which it calls once its
    // operations have ended, disposes them all, including those the factory still holds, so their connections close
    // then rather than whenever the factory would have let them go.
    internal sealed class ClientTransport
    {
        private static readonly SdkLog log = SdkLog.For(typeof(ClientTransport));

        private readonly ServiceProvider services;
        private readonly HashSet<OwnedHandler> owned = new HashSet<OwnedHandler>();

        private ClientTransport(HttpTransport transport, TimeSpan connectTimeout, string userAgent)
        {
            var collection = new ServiceCollection();
            collection.AddTransient<FilesRedirectHandler>();
            collection.AddTransient<StorageRedirectHandler>();
            Pipeline(collection, FilesClient.HttpFilesApi, transport, connectTimeout, userAgent).AddHttpMessageHandler<FilesRedirectHandler>();
            Pipeline(collection, FilesClient.HttpUpload, transport, connectTimeout, userAgent).AddHttpMessageHandler<StorageRedirectHandler>();
            services = collection.BuildServiceProvider();
            Factory = services.GetRequiredService<IHttpClientFactory>();
        }

        internal IHttpClientFactory Factory { get; }

        // transport is null for the SDK's own handlers.
        internal static ClientTransport Create(HttpTransport transport, TimeSpan connectTimeout, string userAgent)
        {
            return new ClientTransport(transport, connectTimeout, userAgent);
        }

        internal async Task DisposeAsync()
        {
            await services.DisposeAsync();
            OwnedHandler[] remaining;
            lock (owned)
            {
                remaining = owned.ToArray();
            }
            foreach (OwnedHandler handler in remaining)
            {
                try
                {
                    handler.Dispose();
                }
                catch (Exception e)
                {
                    DiagnosticText.WarnCleanupFailed(log, "Closing a client: disposing one of its HTTP handlers failed", e);
                }
            }
        }

        private IHttpClientBuilder Pipeline(IServiceCollection collection, string name, HttpTransport transport, TimeSpan connectTimeout, string userAgent)
        {
            IHttpClientBuilder pipeline = collection.AddHttpClient(name, client =>
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
                    client.Timeout = Timeout.InfiniteTimeSpan;
                })
                // The SDK logs its requests itself, without the signed query of a storage URL.
                .RemoveAllLoggers();
            if (transport != null && transport.IsShared)
            {
                return pipeline
                    .ConfigurePrimaryHttpMessageHandler(() => new SharedHandler(transport.CreateHandler(connectTimeout)))
                    .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
            }
            return pipeline.ConfigurePrimaryHttpMessageHandler(() =>
                new OwnedHandler(transport != null ? transport.CreateHandler(connectTimeout) : HttpTransport.CreateDefaultHandler(connectTimeout), this));
        }

        // A handler this client owns: disposed once, by the factory when it replaces it or by DisposeAsync. Its answer
        // to a request counts as over checked TLS only when it has always checked certificates, as the request starts
        // and again once the answer arrives: the application that made it may still hold it and change it meanwhile.
        private sealed class OwnedHandler : DelegatingHandler
        {
            private readonly ClientTransport transport;
            private int disposed;

            internal OwnedHandler(HttpMessageHandler handler, ClientTransport transport) : base(handler)
            {
                this.transport = transport;
                lock (transport.owned)
                {
                    transport.owned.Add(this);
                }
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                bool checks = HttpTransport.HasAlwaysCheckedCertificates(InnerHandler);
                HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
                if (checks && HttpTransport.HasAlwaysCheckedCertificates(InnerHandler))
                {
                    RequestMarks.Set(request, RequestMarks.ChecksCertificates);
                }
                return response;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing && Interlocked.Exchange(ref disposed, 1) == 0)
                {
                    lock (transport.owned)
                    {
                        transport.owned.Remove(this);
                    }
                    base.Dispose(disposing);
                }
            }
        }

        // The application's handler, which the client sends through but never disposes. How it checked the servers it
        // connected to before the client had it, or for other code, cannot be seen, so its answers never count as over
        // checked TLS.
        private sealed class SharedHandler : DelegatingHandler
        {
            internal SharedHandler(HttpMessageHandler handler) : base(handler)
            {
            }

            protected override void Dispose(bool disposing)
            {
            }
        }
    }
}