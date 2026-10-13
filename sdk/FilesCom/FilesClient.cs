using FilesCom.Http;
using FilesCom.Transfers;
using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom
{
    /// <summary>
    /// A connection to Files.com: an endpoint, credentials and HTTP connections. Run operations with a client
    /// through its resource properties, such as <c>client.Users.FindAsync(id)</c>. The objects and lists those
    /// operations return keep using the same client.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Create(FilesConfiguration)"/> makes an independent client, which you can use alongside any number
    /// of others. The constructor makes the default client, <see cref="Instance"/>, which the static model methods
    /// such as <c>User.Find(id)</c> use. Create a client once and reuse it; each client has its own connection pool.
    /// To send its requests through handlers of your own, for a proxy or instrumentation, give it an
    /// <see cref="HttpTransport"/>.
    /// </para>
    /// <para>
    /// Dispose a client when you no longer need it. From then on, the client and the objects, lists and transfers it
    /// returned throw <see cref="ObjectDisposedException"/> for any new operation. Disposing cancels the operations it
    /// is running, and closes its connections once the SDK has finished with what they use: their requests, responses
    /// and temporary files, and a stream an operation owns. <see cref="DisposeAsync"/> completes when that is done;
    /// <see cref="Dispose"/> does the same without waiting. A list or enumerator that is not loading a page does not
    /// delay closing.
    /// </para>
    /// <para>
    /// <see cref="DisposeAsync"/> does not wait for the operations' own tasks: await each one for its outcome. A running
    /// operation usually ends with <see cref="OperationCanceledException"/>, but one whose response had already arrived
    /// can still succeed, and an upload whose final request had been sent may have completed the file. Its task can
    /// complete after DisposeAsync does, for example while it turns the response it received into its result.
    /// </para>
    /// <para>
    /// The SDK's work for an operation stops promptly when cancelled unless a stream you gave it does not: an upload or
    /// download waits for a read or write of your stream that ignores cancellation, and so does
    /// <see cref="DisposeAsync"/>. The SDK never closes your stream to stop it. Do not await DisposeAsync from a
    /// progress or trace handler of the client's own transfer, which the transfer waits for; <see cref="Dispose"/> from
    /// one returns at once.
    /// </para>
    /// <para>
    /// Making a new default client does not dispose the previous one, whose objects keep using it, and disposing the
    /// default client leaves <see cref="Instance"/> as it is.
    /// </para>
    /// </remarks>
    public partial class FilesClient : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// The name of the HTTP client, from the <see cref="System.Net.Http.IHttpClientFactory"/> a
        /// <see cref="FilesApiService"/> is made with, that it sends API requests and downloads with.
        /// </summary>
        public const string HttpFilesApi = "HttpFilesAPI";
        /// <summary>
        /// The name of the HTTP client, from the <see cref="System.Net.Http.IHttpClientFactory"/> a
        /// <see cref="FilesApiService"/> is made with, that it sends upload parts with.
        /// </summary>
        public const string HttpUpload = "HttpUpload";

        private static readonly string SdkProduct = "Files.com DOTNET SDK v" + PackageVersion();
        private const string ConfigManagerSectionName = "files.com/filesConfiguration";

        private static readonly SdkLog log = SdkLog.For(typeof(FilesConfiguration));

        private readonly FilesConfiguration config;
        private readonly ClientTransport transport;
        private readonly FilesApiService api;
        private readonly ClientLifetime lifetime = new ClientLifetime();
        private readonly ClientLogging logging;

        /// <summary>
        /// The default client: the one most recently made with the <see cref="FilesClient"/> constructor. Static
        /// model methods, such as <c>User.Find(id)</c>, run with it.
        /// </summary>
        public static FilesClient Instance { get; private set; }

        /// <summary>
        /// Makes a client and, once it is ready, makes it the default client (<see cref="Instance"/>) in place of any
        /// previous one.
        /// </summary>
        /// <param name="config">
        /// The settings to use. The client reads them as each operation starts, so changes you make to this object
        /// apply to operations started afterward; the client's connections keep the <see cref="FilesConfiguration.ConnectTimeout"/>
        /// it was made with. If null, the <c>files.com/filesConfiguration</c> section of the application configuration
        /// file is used, or the default settings if there is none.
        /// </param>
        /// <exception cref="InvalidParameterException">
        /// A timeout, retry or delay setting is out of range, or <see cref="FilesConfiguration.ProductName"/> or
        /// <see cref="FilesConfiguration.ProductVersion"/> is not valid.
        /// </exception>
        public FilesClient(FilesConfiguration config = null) : this(config ?? LoadConfiguration(), null, registerAsDefault: true)
        {
        }

        /// <summary>
        /// Makes a client that sends its requests through <paramref name="transport"/> and, once it is ready, makes it
        /// the default client (<see cref="Instance"/>) in place of any previous one.
        /// </summary>
        /// <param name="config">The settings to use, read as the other constructor reads them.</param>
        /// <param name="transport">The handlers to send requests through. If null, the SDK's own.</param>
        /// <exception cref="InvalidParameterException">
        /// A timeout, retry or delay setting is out of range, or <see cref="FilesConfiguration.ProductName"/> or
        /// <see cref="FilesConfiguration.ProductVersion"/> is not valid.
        /// </exception>
        public FilesClient(FilesConfiguration config, HttpTransport transport) : this(config ?? LoadConfiguration(), transport, registerAsDefault: true)
        {
        }

        private FilesClient(FilesConfiguration config, HttpTransport transport, bool registerAsDefault)
        {
            this.config = config;
            // Checked before anything is made, so a client that fails here has nothing to release.
            TimeSpan connectTimeout = NetworkSettings.Timeout(nameof(FilesConfiguration.ConnectTimeout), config.ConnectTimeout);
            NetworkSettings.From(config);
            string userAgent = UserAgentFor(config);
            logging = new ClientLogging(config.LoggerFactory, config.MinimumLogLevel);
            using (SdkLog.Use(logging))
            {
                Announce();
            }

            this.transport = ClientTransport.Create(transport, connectTimeout, userAgent);
            api = new FilesApiService(this.transport.Factory, retries: true);

            if (registerAsDefault)
            {
                if (Instance != null)
                {
                    using (SdkLog.Use(logging))
                    {
                        log.Info("Files.com Client instance already exists, replacing instance with new one");
                    }
                }
                Instance = this;
            }
        }

        // Logs how the client authenticates.
        private void Announce()
        {
            if (this.SessionId != null && this.SessionId.Length > 0)
            {
                log.Info("Files.com Client created with Session Id");
            }
            else if (this.ApiKey != null && this.ApiKey.Length > 0)
            {
                log.Info("Files.com Client created with API Key");
            }
            else
            {
                log.Info("Files.com Client created with no pre-configured auth");
            }
        }

        // The products of the client's User-Agent header: the application's, when the configuration names one, then the
        // SDK's own, which is always sent.
        private static string UserAgentFor(FilesConfiguration config)
        {
            if (config.ProductName == null)
            {
                if (config.ProductVersion != null)
                {
                    throw InvalidParameterException.For(nameof(FilesConfiguration.ProductVersion), "A ProductVersion is sent only with a ProductName");
                }
                return SdkProduct;
            }
            try
            {
                return new System.Net.Http.Headers.ProductHeaderValue(config.ProductName, config.ProductVersion) + " " + SdkProduct;
            }
            catch (Exception e) when (e is FormatException || e is ArgumentException)
            {
                throw InvalidParameterException.For(nameof(FilesConfiguration.ProductName), "ProductName and ProductVersion must each be an HTTP token: letters, digits and !#$%&'*+-.^_`|~, without spaces");
            }
        }

        /// <summary>
        /// Makes an independent client. It does not change the default client (<see cref="Instance"/>), so any
        /// number of clients, for different sites or users, can be used at the same time.
        /// </summary>
        /// <param name="configuration">
        /// The settings to use. The client keeps a copy, so later changes to this object do not affect it. If null,
        /// the <c>files.com/filesConfiguration</c> section of the application configuration file is used, or the
        /// default settings if there is none.
        /// </param>
        /// <exception cref="InvalidParameterException">
        /// A timeout, retry or delay setting is out of range, or <see cref="FilesConfiguration.ProductName"/> or
        /// <see cref="FilesConfiguration.ProductVersion"/> is not valid.
        /// </exception>
        public static FilesClient Create(FilesConfiguration configuration = null)
        {
            return new FilesClient((configuration ?? LoadConfiguration()).Copy(), null, registerAsDefault: false);
        }

        /// <summary>
        /// Makes an independent client that sends its requests through <paramref name="transport"/>, such as handlers
        /// with a proxy. It does not change the default client (<see cref="Instance"/>).
        /// </summary>
        /// <param name="configuration">The settings to use, copied as <see cref="Create(FilesConfiguration)"/> copies them.</param>
        /// <param name="transport">The handlers to send requests through. If null, the SDK's own.</param>
        /// <exception cref="InvalidParameterException">
        /// A timeout, retry or delay setting is out of range, or <see cref="FilesConfiguration.ProductName"/> or
        /// <see cref="FilesConfiguration.ProductVersion"/> is not valid.
        /// </exception>
        public static FilesClient Create(FilesConfiguration configuration, HttpTransport transport)
        {
            return new FilesClient((configuration ?? LoadConfiguration()).Copy(), transport, registerAsDefault: false);
        }

        /// <summary>
        /// Closes the client without waiting: it refuses new operations, cancels the running ones and, once the SDK has
        /// finished with what they use, closes its connections. Calling it again, or after <see cref="DisposeAsync"/>,
        /// does nothing more.
        /// </summary>
        public void Dispose()
        {
            using (SdkLog.Use(logging))
            {
                _ = lifetime.CloseAsync(transport.DisposeAsync);
            }
        }

        /// <summary>
        /// Closes the client: it refuses new operations, cancels the running ones, waits until the SDK has finished with
        /// what they use, then closes its connections. It does not wait for the operations' own tasks. Every call,
        /// including one after <see cref="Dispose"/>, waits for the same closing.
        /// </summary>
        public ValueTask DisposeAsync()
        {
            using (SdkLog.Use(logging))
            {
                return new ValueTask(lifetime.CloseAsync(transport.DisposeAsync));
            }
        }

        private static FilesConfiguration LoadConfiguration()
        {
            FilesConfiguration configuration = (FilesConfiguration)ConfigurationManager.GetSection(ConfigManagerSectionName);
            if (configuration != null)
            {
                log.Info("FilesConfiguration found in app.config");
                return configuration;
            }
            log.Info("No FilesConfiguration found, using defaults");
            return new FilesConfiguration();
        }

        /// <summary>
        /// File transfers that run with this client: uploads and downloads of files, streams and directories, in parts
        /// sent or fetched in parallel where Files.com allows it, with progress, shared resource limits, checkpoints to
        /// continue them, and cancellation.
        /// </summary>
        public TransferOperations Transfers => new TransferOperations(this);

        /// <summary>
        /// Remote files and folders used the way local ones are, with streams to read and write them, listing, and
        /// deleting, copying and moving that return once Files.com has finished, all with this client.
        /// </summary>
        public RemoteFileSystem FileSystem => new RemoteFileSystem(this);

        internal ClientLifetime Lifetime => lifetime;

        // Where the client's messages are logged, as configured when it was made.
        internal ClientLogging Logging => logging;

        // The retry and timeout settings for an operation starting now.
        internal NetworkSettings CurrentNetworkSettings()
        {
            return NetworkSettings.From(config);
        }

        internal RegionRelocationPolicy RegionRelocation => config.RegionRelocation;

        // The version of the package this assembly came from, such as 2.0.1 or 2.0.0-beta.1, without build metadata.
        private static string PackageVersion()
        {
            Assembly assembly = typeof(FilesClient).Assembly;
            string version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            if (string.IsNullOrEmpty(version))
            {
                return assembly.GetName().Version.ToString(3);
            }
            int metadata = version.IndexOf('+');
            return metadata < 0 ? version : version.Substring(0, metadata);
        }

        /// <summary>
        /// The base URL from this client's configuration, where its requests go unless Files.com has moved its site to
        /// another region (<see cref="RegionRelocationPolicy"/>).
        /// </summary>
        public string BaseUrl
        {
            get { return config.BaseUrl; }
        }

        /// <summary>
        /// The API key, from this client's configuration, that its requests send as X-FilesAPI-Key when neither their
        /// options nor <see cref="SessionId"/> give a credential.
        /// </summary>
        public string ApiKey
        {
            get { return config.ApiKey; }
        }

        /// <summary>
        /// The language, from this client's configuration, that its requests send as their Accept-Language header,
        /// unless it is blank.
        /// </summary>
        public string Language
        {
            get { return config.Language; }
        }

        /// <summary>
        /// The session ID, from this client's configuration, that its requests send as X-FilesAPI-Auth when their
        /// options give neither session_id nor api_key. It takes precedence over <see cref="ApiKey"/>.
        /// </summary>
        public string SessionId
        {
            get { return config.SessionId; }
        }

        /// <summary>
        /// The workspace ID, from this client's configuration, that its requests use when their options give no
        /// workspace_id.
        /// </summary>
        public string WorkspaceId
        {
            get { return config.WorkspaceId; }
        }

        /// <summary>
        /// <see cref="FilesConfiguration.ReadTimeout"/> of this client's configuration, in seconds.
        /// </summary>
        public int ReadTimeout
        {
            get { return config.ReadTimeout; }
        }

        /// <summary>
        /// Sends a Files.com API request with the default client (<see cref="Instance"/>), retried as its requests are.
        /// </summary>
        /// <param name="path">The API path after <c>/api/rest/v1</c>, such as <c>/users/{id}</c>; each placeholder is filled from <paramref name="parameters"/>.</param>
        /// <param name="verb">The HTTP method.</param>
        /// <param name="parameters">The operation's parameters, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <returns>The response, its body already read.</returns>
        /// <exception cref="AuthenticationException">
        /// No client has been made, the request sends credentials, and <paramref name="options"/> has no api_key or
        /// session_id, so it has none.
        /// </exception>
        /// <exception cref="SdkException">No client has been made, so there is no default client to send the request with.</exception>
        public static Task<HttpResponseMessage> SendRequest(
            string path,
            HttpMethod verb,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        )
        {
            return SendRequest(OperationContext.OfDefaultClient(), path, verb, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Sends a Files.com API request with the default client (<see cref="Instance"/>), as
        /// <see cref="SendRequest(string, HttpMethod, Dictionary{string, object}, Dictionary{string, object})"/> does,
        /// and returns the response's body.
        /// </summary>
        /// <param name="path">The API path after <c>/api/rest/v1</c>, such as <c>/users/{id}</c>; each placeholder is filled from <paramref name="parameters"/>.</param>
        /// <param name="verb">The HTTP method.</param>
        /// <param name="parameters">The operation's parameters, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <returns>The response's body, as text.</returns>
        /// <exception cref="AuthenticationException">
        /// No client has been made, the request sends credentials, and <paramref name="options"/> has no api_key or
        /// session_id, so it has none.
        /// </exception>
        /// <exception cref="SdkException">No client has been made, so there is no default client to send the request with.</exception>
        public static Task<string> SendStringRequest(
            string path,
            HttpMethod verb,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        )
        {
            return SendStringRequest(OperationContext.OfDefaultClient(), path, verb, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Downloads the contents at <paramref name="uri"/>, such as a file's <c>DownloadUri</c>, into
        /// <paramref name="writeStream"/>, with the default client.
        /// </summary>
        /// <remarks>
        /// A failure that may be temporary is retried until the first byte is written. This method cannot request a
        /// new URL, so a signed URL that has expired is not retried; <c>RemoteFile.DownloadFileAsync</c> can. The
        /// stream is left open.
        /// </remarks>
        /// <param name="uri">The URL to download from, such as a file's <c>DownloadUri</c>.</param>
        /// <param name="writeStream">The stream to write the contents to, from its current position.</param>
        /// <exception cref="SdkException">No client has been made, so there is no default client to download with.</exception>
        public static Task StreamDownload(string uri, Stream writeStream)
        {
            return StreamDownload(OperationContext.OfDefaultClient(), uri, writeStream, null, CancellationToken.None);
        }

        /// <summary>
        /// Uploads one part: reads exactly <paramref name="readLength"/> bytes from <paramref name="readStream"/>,
        /// starting at its current position, and sends them to <paramref name="uri"/>.
        /// </summary>
        /// <remarks>
        /// The part is buffered in memory, so a retried request resends the same bytes. This method cannot request a
        /// new URL, so a signed URL that has expired is not retried. The stream is left open, positioned just after
        /// the part.
        /// </remarks>
        /// <param name="verb">The HTTP method the upload URL takes, such as PUT.</param>
        /// <param name="uri">The URL to upload the part to.</param>
        /// <param name="readStream">The stream to read the part from, starting at its current position.</param>
        /// <param name="readLength">The number of bytes in the part.</param>
        /// <exception cref="MissingParameterException"><paramref name="readStream"/> is null.</exception>
        /// <exception cref="InvalidParameterException"><paramref name="readLength"/> is negative or greater than Int32.MaxValue.</exception>
        /// <exception cref="EndOfStreamException">The stream ended before <paramref name="readLength"/> bytes were read. The part was not sent.</exception>
        /// <exception cref="SdkException">No client has been made, so there is no default client to upload with.</exception>
        public static Task ChunkUpload(HttpMethod verb, string uri, Stream readStream, Int64 readLength)
        {
            return ChunkUpload(OperationContext.OfDefaultClient(), verb, SignedUrl.Of(uri), readStream, readLength, null, CancellationToken.None);
        }

        // Returns the client an object belongs to. An object made without a client (with its constructor, or by
        // deserializing it yourself) belongs to the default client from its first request on, whether that request
        // succeeds or not, and never changes client afterward. Concurrent first requests agree on one client. A
        // disposed default client runs nothing, so an object is not bound to it. Without a default client the object
        // has nothing to send with: that is reported as such (NoDefaultClient), not as missing credentials, which only
        // the request it would send could show.
        internal static FilesClient Bind(ref FilesClient owner)
        {
            FilesClient bound = owner;
            if (bound != null)
            {
                return bound;
            }

            FilesClient defaultClient = Instance;
            if (defaultClient == null)
            {
                throw NoDefaultClient();
            }
            if (defaultClient.lifetime.IsClosing)
            {
                throw ClientLifetime.Disposed();
            }
            return Interlocked.CompareExchange(ref owner, defaultClient, null) ?? defaultClient;
        }

        // The failure of an operation that needs the default client when none has been made: there is nothing to send
        // with, whatever its options carry.
        internal static SdkException NoDefaultClient()
        {
            return new SdkException("No FilesClient has been made. Static methods, such as User.Find, and objects made without a client run with the default client, which the FilesClient constructor makes: make a FilesClient first, or use the operations of one made with FilesClient.Create.");
        }

        // The failure of an API request that needs the default client when none has been made. Without a client there is
        // no configuration, so a request that sends credentials (sendsCredentials, as FilesApiService.SendsCredentials
        // decides for every request) has them only when its options carry a non-empty api_key or session_id; without them
        // it fails as a request without credentials does. Any other request, with credentials in its options or sending
        // none, lacks only the client.
        internal static SdkException NoDefaultClient(Dictionary<string, object> options, bool sendsCredentials)
        {
            if (sendsCredentials && !HasCredentialOption(options, "api_key") && !HasCredentialOption(options, "session_id"))
            {
                return new AuthenticationException("Authentication required: no FilesClient has been made, so the request has no credentials. Make a FilesClient with an API key or session ID first.");
            }
            return NoDefaultClient();
        }

        private static bool HasCredentialOption(Dictionary<string, object> options, string name)
        {
            return options != null && options.TryGetValue(name, out object value) && value is string text && text.Length > 0;
        }

        internal static async Task<HttpResponseMessage> SendRequest(
            OperationContext context,
            string path,
            HttpMethod verb,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            if (context == null)
            {
                throw NoDefaultClient(options, FilesApiService.SendsCredentials(path, verb));
            }

            using (OperationContext.OperationScope operation = context.Begin(cancellationToken))
            {
                try
                {
                    return await context.Client.api.SendRequest(context, path, verb, parameters, options, operation.Token);
                }
                catch (Exception ex) when (!IsRequestedCancellation(ex, operation.Token))
                {
                    DiagnosticText.ErrorFailed(log, "Failed to send Files API Request", ex, $"Failed to send Files API Request to path: {path}");
                    throw;
                }
            }
        }

        // The body of the answer, as text. The answer is read and disposed while the operation holds its client open, as
        // every response the SDK itself uses is, so that closing the client waits for that too.
        internal static async Task<string> SendStringRequest(
            OperationContext context,
            string path,
            HttpMethod verb,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            if (context == null)
            {
                throw NoDefaultClient(options, FilesApiService.SendsCredentials(path, verb));
            }

            using (OperationContext.OperationScope operation = context.Begin(cancellationToken))
            using (HttpResponseMessage response = await SendRequest(context, path, verb, parameters, options, operation.Token))
            {
                // SendRequest has already buffered and logged the body, honoring the token.
                return await response.Content.ReadAsStringAsync();
            }
        }

        // Sends a request whose answer the operation does not use, and disposes the answer while the operation holds its
        // client open.
        internal static async Task SendRequestAndDiscard(
            OperationContext context,
            string path,
            HttpMethod verb,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            if (context == null)
            {
                throw NoDefaultClient(options, FilesApiService.SendsCredentials(path, verb));
            }

            using (OperationContext.OperationScope operation = context.Begin(cancellationToken))
            {
                (await SendRequest(context, path, verb, parameters, options, operation.Token)).Dispose();
            }
        }

        // Downloads uri into writeStream. renewUrl, when given, supplies a new URL for the same file once uri is about
        // to expire.
        internal static async Task StreamDownload(OperationContext context, string uri, Stream writeStream, Func<CancellationToken, Task<SignedUrl>> renewUrl, CancellationToken cancellationToken)
        {
            if (context == null)
            {
                throw NoDefaultClient();
            }

            using (OperationContext.OperationScope operation = context.Begin(cancellationToken))
            {
                try
                {
                    await context.Client.api.StreamDownload(context.Network, uri, writeStream, renewUrl, operation.Token);
                }
                catch (Exception ex) when (!IsRequestedCancellation(ex, operation.Token))
                {
                    log.Error("Failed to stream download");
                    log.Debug($"Failed to stream download from {uri}", ex);
                    throw;
                }
            }
        }

        // Uploads one part to url. renewUrl, when given, supplies a new URL for the same part once url is about to
        // expire.
        internal static async Task ChunkUpload(OperationContext context, HttpMethod verb, SignedUrl url, Stream readStream, Int64 readLength, Func<CancellationToken, Task<SignedUrl>> renewUrl, CancellationToken cancellationToken)
        {
            if (context == null)
            {
                throw NoDefaultClient();
            }

            using (OperationContext.OperationScope operation = context.Begin(cancellationToken))
            {
                try
                {
                    await context.Client.api.ChunkUpload(context.Network, verb, url, readStream, readLength, renewUrl, operation.Token);
                }
                catch (Exception ex) when (!IsRequestedCancellation(ex, operation.Token))
                {
                    log.Error("Failed to upload chunk");
                    log.Debug($"Failed to upload chunk to {url.Text}", ex);
                    throw;
                }
            }
        }

        // The storage HTTP client of the client context belongs to, for the transfer engine.
        internal static HttpClient StorageClient(OperationContext context)
        {
            return context.Client.api.StorageClient();
        }

        // One attempt of an upload part, for the transfer engine, which holds the client open for the whole upload.
        internal static Task<UploadPartAttempt> SendUploadPart(OperationContext context, HttpMethod verb, string uri, IEnumerable<KeyValuePair<string, string>> headers, HttpContent content, CancellationToken cancellationToken)
        {
            return context.Client.api.SendUploadPart(context.Network, verb, uri, headers, content, cancellationToken);
        }

        // Cancellation the caller asked for, or the client's closing, is not a failure, so it is not logged as one.
        internal static bool IsRequestedCancellation(Exception exception, CancellationToken cancellationToken)
        {
            return exception is OperationCanceledException && cancellationToken.IsCancellationRequested;
        }
    }
}