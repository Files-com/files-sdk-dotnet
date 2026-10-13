using FilesCom.Http;
using System;
using System.Threading;

namespace FilesCom
{
    // The client that one logical operation runs with, and that client's settings as they were when the operation
    // started. Every request of the operation, including retries, later pages and the stages of a transfer, uses
    // these same settings, so the operation never mixes endpoints or credentials, even if the configuration of a
    // client made with the FilesClient constructor changes while it runs. The models it returns belong to Client.
    //
    // Its API requests go to Origin: BaseUrl, unless the object or list the operation started from was moved to its
    // site's region by an earlier operation, or this operation moves there itself (RegionRelocation), once at most.
    internal sealed class OperationContext
    {
        private readonly Activity activity;
        private readonly Region region;

        // origin, when not null, is where an earlier operation moved the object or list this one starts from.
        internal OperationContext(FilesClient client, string origin = null)
        {
            Client = client;
            BaseUrl = client.BaseUrl;
            ApiKey = client.ApiKey;
            SessionId = client.SessionId;
            WorkspaceId = client.WorkspaceId;
            Language = client.Language;
            ReadTimeout = client.ReadTimeout;
            RegionRelocation = client.RegionRelocation;
            Network = client.CurrentNetworkSettings();
            Logging = client.Logging;
            activity = new Activity(client.Lifetime);
            region = new Region(origin);
        }

        private OperationContext(OperationContext context, NetworkSettings network, bool replaySafe)
        {
            ReplaySafe = replaySafe;
            Client = context.Client;
            BaseUrl = context.BaseUrl;
            ApiKey = context.ApiKey;
            SessionId = context.SessionId;
            WorkspaceId = context.WorkspaceId;
            Language = context.Language;
            ReadTimeout = context.ReadTimeout;
            RegionRelocation = context.RegionRelocation;
            Network = network;
            Logging = context.Logging;
            activity = context.activity;
            region = context.region;
        }

        internal FilesClient Client { get; }

        internal string BaseUrl { get; }

        internal RegionRelocationPolicy RegionRelocation { get; }

        // Where the operation's API requests go now.
        internal string Origin => region.Origin ?? BaseUrl;

        // Origin, when it is not BaseUrl, for the objects and lists the operation returns: null when it is.
        internal string RelocatedOrigin => region.Origin;

        // Whether this operation has moved to another origin.
        internal bool HasRelocated => region.Moved;

        // Moves the operation's remaining API requests to origin, a canonical origin.
        internal void Relocate(string origin)
        {
            region.MoveTo(origin);
        }

        internal string ApiKey { get; }

        internal string SessionId { get; }

        internal string WorkspaceId { get; }

        internal string Language { get; }

        internal int ReadTimeout { get; }

        // Retries and timeouts.
        internal NetworkSettings Network { get; }

        // The same settings, for a request that must not be retried automatically because whether it took effect
        // cannot be told from a lost or failed response. It is part of the same operation.
        internal OperationContext WithoutRetries()
        {
            return new OperationContext(this, Network.WithoutRetries(), replaySafe: false);
        }

        // Whether this context's requests may be sent again once they may have reached Files.com, whatever their method:
        // true only for requests Files.com can receive twice without harm (ForReplaySafeRequests). Otherwise only a
        // request that just asks for something (GET, HEAD, OPTIONS) is sent again after a failure it may have followed.
        internal bool ReplaySafe { get; }

        // The same settings, for a request Files.com can receive twice without harm, such as asking for an upload part's
        // URL, or creating a folder that may exist already: like a GET, it is retried after its answer was lost. It is
        // part of the same operation.
        internal OperationContext ForReplaySafeRequests()
        {
            return new OperationContext(this, Network, replaySafe: true);
        }

        // Holds the client open while network work of the operation runs, and returns the token that work uses:
        // cancelled by cancellationToken or when the client closes. An operation that is not running yet cannot begin
        // once its client has been disposed, and throws ObjectDisposedException; a stage of one already running, such
        // as a part of an upload, begins with the token it was given, and ends by that token's cancellation instead.
        internal OperationScope Begin(CancellationToken cancellationToken)
        {
            OperationScope scope = activity.Enter(cancellationToken);
            // What runs within the scope, its tasks included, logs where the operation's client says.
            scope.Logging = SdkLog.Use(Logging);
            return scope;
        }

        // The same, without making the client's logging current here: for an operation that begins on its caller, in a
        // method that is not async, and runs in a task of its own, which uses the logging itself (SdkLog.Use). Begin
        // would leave the logging current on the caller, whose code runs on outside the operation.
        internal OperationScope BeginElsewhere(CancellationToken cancellationToken)
        {
            return activity.Enter(cancellationToken);
        }

        // Where the operation's messages are logged: its client's logging.
        internal ClientLogging Logging { get; }

        // The context of the default client, or null when no client has been constructed yet. Operations validate
        // their arguments before sending, so a null context fails only when the request would be sent.
        internal static OperationContext OfDefaultClient()
        {
            FilesClient client = FilesClient.Instance;
            return client == null ? null : new OperationContext(client);
        }

        // Where the operation sends API requests, when not to BaseUrl, shared by every copy of its context.
        private sealed class Region
        {
            private volatile string origin;
            private volatile bool moved;

            internal Region(string origin)
            {
                this.origin = origin;
            }

            internal string Origin => origin;

            internal bool Moved => moved;

            internal void MoveTo(string destination)
            {
                origin = destination;
                moved = true;
            }
        }

        // How many stages of the operation are running. The first to begin admits the operation to its client, and
        // the last to end lets the client close.
        internal sealed class Activity
        {
            private readonly object gate = new object();
            private readonly ClientLifetime lifetime;
            private int stages;

            internal Activity(ClientLifetime lifetime)
            {
                this.lifetime = lifetime;
            }

            internal OperationScope Enter(CancellationToken cancellationToken)
            {
                bool first;
                lock (gate)
                {
                    first = stages == 0;
                    if (first)
                    {
                        lifetime.Begin();
                    }
                    stages++;
                }
                if (!first)
                {
                    return new OperationScope(this, cancellationToken);
                }
                try
                {
                    return new OperationScope(this, CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, lifetime.Closing));
                }
                catch
                {
                    Exit();
                    throw;
                }
            }

            internal void Exit()
            {
                lock (gate)
                {
                    if (--stages == 0)
                    {
                        lifetime.End();
                    }
                }
            }
        }

        // A stage of an operation that holds its client open until disposed.
        internal sealed class OperationScope : IDisposable
        {
            private readonly Activity activity;
            private readonly CancellationTokenSource cancellation;
            private int disposed;

            internal OperationScope(Activity activity, CancellationTokenSource cancellation)
            {
                this.activity = activity;
                this.cancellation = cancellation;
                Token = cancellation.Token;
            }

            internal OperationScope(Activity activity, CancellationToken token)
            {
                this.activity = activity;
                Token = token;
            }

            internal CancellationToken Token { get; }

            internal IDisposable Logging { get; set; }

            public void Dispose()
            {
                if (Interlocked.Exchange(ref disposed, 1) == 0)
                {
                    Logging?.Dispose();
                    cancellation?.Dispose();
                    activity.Exit();
                }
            }
        }
    }
}