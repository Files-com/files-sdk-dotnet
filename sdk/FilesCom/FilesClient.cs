using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
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
    /// <see cref="Create"/> makes an independent client, which you can use alongside any number of others. The
    /// constructor makes the default client, <see cref="Instance"/>, which the static model methods such as
    /// <c>User.Find(id)</c> use. Create a client once and reuse it; each client has its own connection pool.
    /// </remarks>
    public partial class FilesClient
    {
        public const string HttpFilesApi = "HttpFilesAPI";
        public const string HttpUpload = "HttpUpload";

        private static string UserAgent = "Files.com DOTNET SDK v" + typeof(FilesClient).Assembly.GetName().Version.ToString(3);
        private const string ConfigManagerSectionName = "files.com/filesConfiguration";

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(FilesConfiguration));

        private readonly FilesConfiguration config;
        private readonly IHost host;
        private readonly FilesApiService api;

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
        /// apply to operations started afterward. If null, the <c>files.com/filesConfiguration</c> section of the
        /// application configuration file is used, or the default settings if there is none.
        /// </param>
        public FilesClient(FilesConfiguration config = null) : this(config ?? LoadConfiguration(), registerAsDefault: true)
        {
        }

        private FilesClient(FilesConfiguration config, bool registerAsDefault)
        {
            this.config = config;

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

            var builder = new HostBuilder()
                .ConfigureServices((HostExecutionContext, services) =>
                {
                    services.AddTransient<FilesRedirectHandler>();

                    TimeSpan[] retries = new TimeSpan[this.config.MaxNetworkRetries];
                    Random rand = new Random();
                    for (int i = 0; i < retries.Length; i++)
                    {
                        double delay;

                        if (i == 0)
                        {
                            delay = this.config.InitialNetworkRequestDelay;
                        }
                        else if (i == retries.Length - 1)
                        {
                            delay = this.config.MaxNetworkRetryDelay;
                        }
                        else
                        {
                            delay = Math.Min(this.config.InitialNetworkRequestDelay, rand.NextDouble() * this.config.MaxNetworkRetryDelay);
                        }

                        retries[i] = TimeSpan.FromSeconds(delay);
                    }

                    // No BaseAddress: each request is addressed to the base URL its operation captured.
                    services.AddHttpClient(HttpFilesApi, client =>
                    {
                        client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
                        client.Timeout = Timeout.InfiniteTimeSpan;
                    })
#if NETCOREAPP2_1_OR_GREATER
                    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
                    {
                        AllowAutoRedirect = false,
                        ConnectTimeout = TimeSpan.FromSeconds(this.config.ConnectTimeout)
                    })
#else
                    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                    {
                        AllowAutoRedirect = false
                    })
#endif
                    .AddTransientHttpErrorPolicy(newBuilder => newBuilder.WaitAndRetryAsync(retries))
                    .AddHttpMessageHandler<FilesRedirectHandler>();

                    services.AddHttpClient(HttpUpload, client =>
                    {
                        client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
                        client.Timeout = Timeout.InfiniteTimeSpan;
                    })
#if NETCOREAPP2_1_OR_GREATER
                    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
                    {
                        ConnectTimeout = TimeSpan.FromSeconds(this.config.ConnectTimeout)
                    })
#endif
                    .AddTransientHttpErrorPolicy(newBuilder => newBuilder.WaitAndRetryAsync(retries));
                }).UseConsoleLifetime();

            host = builder.Build();
            api = new FilesApiService(host.Services.GetRequiredService<IHttpClientFactory>());

            if (registerAsDefault)
            {
                if (Instance != null)
                {
                    log.Info("Files.com Client instance already exists, replacing instance with new one");
                }
                Instance = this;
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
        public static FilesClient Create(FilesConfiguration configuration = null)
        {
            return new FilesClient((configuration ?? LoadConfiguration()).Copy(), registerAsDefault: false);
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

        public string BaseUrl
        {
            get { return config.BaseUrl; }
        }

        public string ApiKey
        {
            get { return config.ApiKey; }
        }

        public string Language
        {
            get { return config.Language; }
        }

        public string SessionId
        {
            get { return config.SessionId; }
        }

        public string WorkspaceId
        {
            get { return config.WorkspaceId; }
        }

        public int ReadTimeout
        {
            get { return config.ReadTimeout; }
        }

        public static Task<HttpResponseMessage> SendRequest(
            string path,
            HttpMethod verb,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        )
        {
            return SendRequest(OperationContext.OfDefaultClient(), path, verb, parameters, options, CancellationToken.None);
        }

        public static Task<string> SendStringRequest(
            string path,
            HttpMethod verb,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        )
        {
            return SendStringRequest(OperationContext.OfDefaultClient(), path, verb, parameters, options, CancellationToken.None);
        }

        public static Task StreamDownload(string uri, Stream writeStream)
        {
            return StreamDownload(OperationContext.OfDefaultClient(), uri, writeStream, CancellationToken.None);
        }

        /// <summary>
        /// Uploads one part: reads exactly <paramref name="readLength"/> bytes from <paramref name="readStream"/>,
        /// starting at its current position, and sends them to <paramref name="uri"/>.
        /// </summary>
        /// <remarks>
        /// The part is buffered in memory, so a retried request resends the same bytes. The stream is left open,
        /// positioned just after the part.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="readStream"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="readLength"/> is negative or greater than Int32.MaxValue.</exception>
        /// <exception cref="EndOfStreamException">The stream ended before <paramref name="readLength"/> bytes were read. The part was not sent.</exception>
        public static Task ChunkUpload(HttpMethod verb, string uri, Stream readStream, Int64 readLength)
        {
            return ChunkUpload(OperationContext.OfDefaultClient(), verb, uri, readStream, readLength, CancellationToken.None);
        }

        // Returns the client an object belongs to. An object made without a client (with its constructor, or by
        // deserializing it yourself) belongs to the default client from its first request on, whether that request
        // succeeds or not, and never changes client afterward. Concurrent first requests agree on one client.
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
                throw new InvalidOperationException("Instance must be created before sending API request");
            }
            return Interlocked.CompareExchange(ref owner, defaultClient, null) ?? defaultClient;
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
                throw new InvalidOperationException("Instance must be created before sending API request");
            }

            try
            {
                return await context.Client.api.SendRequest(context, path, verb, parameters, options, cancellationToken);
            }
            catch (Exception ex) when (!IsRequestedCancellation(ex, cancellationToken))
            {
                log.Error($"Failed to send Files API Request to path: {path}", ex);
                throw;
            }
        }

        internal static async Task<string> SendStringRequest(
            OperationContext context,
            string path,
            HttpMethod verb,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            using (HttpResponseMessage response = await SendRequest(context, path, verb, parameters, options, cancellationToken))
            {
                // SendRequest has already buffered the body, honoring the token.
                string responseJson = await response.Content.ReadAsStringAsync();
                log.Debug(responseJson);
                return responseJson;
            }
        }

        internal static async Task StreamDownload(OperationContext context, string uri, Stream writeStream, CancellationToken cancellationToken)
        {
            if (context == null)
            {
                throw new InvalidOperationException("Instance must be created before streaming download");
            }

            try
            {
                await context.Client.api.StreamDownload(context, uri, writeStream, cancellationToken);
            }
            catch (Exception ex) when (!IsRequestedCancellation(ex, cancellationToken))
            {
                log.Error("Failed to stream download");
                log.Debug($"Failed to stream download from {uri}", ex);
                throw;
            }
        }

        internal static async Task ChunkUpload(OperationContext context, HttpMethod verb, string uri, Stream readStream, Int64 readLength, CancellationToken cancellationToken)
        {
            if (context == null)
            {
                throw new InvalidOperationException("Instance must be created before uploading chunk");
            }

            try
            {
                await context.Client.api.ChunkUpload(verb, uri, readStream, readLength, cancellationToken);
            }
            catch (Exception ex) when (!IsRequestedCancellation(ex, cancellationToken))
            {
                log.Error("Failed to upload chunk");
                log.Debug($"Failed to upload chunk to {uri}", ex);
                throw;
            }
        }

        // Cancellation the caller asked for is not a failure, so it is not logged as one.
        internal static bool IsRequestedCancellation(Exception exception, CancellationToken cancellationToken)
        {
            return exception is OperationCanceledException && cancellationToken.IsCancellationRequested;
        }
    }
}