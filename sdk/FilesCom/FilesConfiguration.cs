using Microsoft.Extensions.Logging;
using System;
using System.Configuration;
namespace FilesCom
{
    /// <summary>
    /// The settings of a <see cref="FilesClient"/>: its credentials, address, language, timeouts and policies. A client
    /// made without one reads them from the application configuration's files.com/filesConfiguration section, or
    /// uses the defaults when there is none.
    /// </summary>
    /// <remarks>
    /// A client made with <c>new FilesClient(configuration)</c> keeps using this object, so later changes to it can
    /// affect that client; <see cref="FilesClient.Create(FilesConfiguration)"/> copies the settings instead, so later
    /// changes do not. Some settings are fixed when the client is made: its connections keep the
    /// <see cref="ConnectTimeout"/> it was made with.
    /// </remarks>
    public class FilesConfiguration : ConfigurationSection
    {
        /// <summary>
        /// The API key that requests send as X-FilesAPI-Key when neither their options nor <see cref="SessionId"/>
        /// give a credential.
        /// </summary>
        [ConfigurationProperty("ApiKey")]
        public string ApiKey
        {
            get { return (string)this["ApiKey"]; }
            set { this["ApiKey"] = value; }
        }

        /// <summary>
        /// The session ID that requests send as X-FilesAPI-Auth when their options give neither session_id nor
        /// api_key. It takes precedence over <see cref="ApiKey"/>.
        /// </summary>
        [ConfigurationProperty("SessionId")]
        public string SessionId
        {
            get { return (string)this["SessionId"]; }
            set { this["SessionId"] = value; }
        }

        /// <summary>
        /// The workspace ID that requests use when their options give no workspace_id.
        /// </summary>
        [ConfigurationProperty("WorkspaceId")]
        public string WorkspaceId
        {
            get { return (string)this["WorkspaceId"]; }
            set { this["WorkspaceId"] = value; }
        }

        /// <summary>
        /// The base URL requests go to, unless Files.com has moved the site to another region
        /// (<see cref="RegionRelocationPolicy"/>); the default is <c>https://app.files.com/</c>.
        /// </summary>
        [ConfigurationProperty("BaseUrl", DefaultValue = "https://app.files.com/")]
        public string BaseUrl
        {
            get { return (string)this["BaseUrl"]; }
            set { this["BaseUrl"] = value; }
        }

        /// <summary>
        /// The language requests send as their Accept-Language header. None is sent when it is blank.
        /// </summary>
        [ConfigurationProperty("Language")]
        public string Language
        {
            get { return (string)this["Language"]; }
            set { this["Language"] = value; }
        }

        /// <summary>
        /// How long, in seconds, to wait to connect to a server; the default is 30. On .NET 8 and later the client
        /// applies it to the handlers it makes itself, and to a <c>SocketsHttpHandler</c> from
        /// <see cref="HttpTransport.FromHandlerFactory"/> whose own <c>ConnectTimeout</c> is not set, and keeps the
        /// value it was made with. A connection that it ends was never made, so nothing was sent, and the request is
        /// retried, within <see cref="MaxNetworkRetries"/>, even when it can change something.
        /// </summary>
        /// <remarks>
        /// <para>
        /// It is not applied to any other handler: a shared handler (<see cref="HttpTransport.FromSharedHandler"/>),
        /// whose settings the SDK never changes, or a factory's handler of another type or whose own
        /// <c>ConnectTimeout</c> is already set. Such a handler connects as it is configured: its own connection
        /// settings stay in effect. On .NET Framework, or any other runtime that uses the SDK's .NET Standard 2.0 build,
        /// the handlers the client makes itself have no separate connect timeout.
        /// </para>
        /// <para>
        /// Whatever the handler, connecting also counts toward the wait that follows it: <see cref="ReadTimeout"/> for
        /// an API request or a download, and <see cref="WriteTimeout"/> for an upload part or a request with file
        /// attachments. Whichever limit runs out first ends the attempt. When that wait runs out before a connection is
        /// made, as it does where no shorter connect timeout applies, the request fails with
        /// <see cref="NetworkTimeoutException"/> for that timeout, and a request that can change something is not sent
        /// again after it, since the SDK cannot tell that nothing was sent. None of the timeouts limits how long a whole
        /// request, transfer or connection takes.
        /// </para>
        /// </remarks>
        [ConfigurationProperty("ConnectTimeout", DefaultValue = 30)]
        public int ConnectTimeout
        {
            get { return (int)this["ConnectTimeout"]; }
            set { this["ConnectTimeout"] = value; }
        }

        /// <summary>
        /// How long, in seconds, a request may wait for its response once it has been sent, and then between the
        /// parts of the response as they arrive; the default is 80. It limits a stalled connection, not how long a
        /// response takes: a download that keeps receiving data continues however long it lasts.
        /// </summary>
        [ConfigurationProperty("ReadTimeout", DefaultValue = 80)]
        public int ReadTimeout
        {
            get { return (int)this["ReadTimeout"]; }
            set { this["ReadTimeout"] = value; }
        }

        /// <summary>
        /// How long, in seconds, an upload part may go without sending any data: to connect and start, and then each
        /// time it sends more; the default is 80. It limits a stalled connection, not how long an upload takes. Time
        /// spent reading your file or stream is not counted.
        /// </summary>
        [ConfigurationProperty("WriteTimeout", DefaultValue = 80)]
        public int WriteTimeout
        {
            get { return (int)this["WriteTimeout"]; }
            set { this["WriteTimeout"] = value; }
        }

        /// <summary>
        /// The wait, in seconds, before the first retry of a Files.com API request; each later retry waits twice as
        /// long as the one before, up to <see cref="MaxNetworkRetryDelay"/>. The default is 0.1. Requests to the
        /// storage URLs of uploads and downloads wait as their servers ask instead.
        /// </summary>
        [ConfigurationProperty("InitialNetworkRequestDelay", DefaultValue = 0.1D)]
        public double InitialNetworkRequestDelay
        {
            get { return (double)this["InitialNetworkRequestDelay"]; }
            set { this["InitialNetworkRequestDelay"] = value; }
        }

        /// <summary>
        /// How many times the SDK retries a request that failed in a way that may be temporary, such as a connection
        /// failure, a timeout or a server error; the default is 3, and 0 turns the SDK's retries off.
        /// </summary>
        /// <remarks>
        /// These are the SDK's own attempts, each logged with its reason, number and wait. Below them, the HTTP handler
        /// may itself send a request that only asks for something, such as a GET, again when its connection closes before
        /// any answer, within one attempt, which neither counts here nor is logged. A request that can change something,
        /// such as a DELETE or the request that completes an upload, is sent once per attempt by the SDK's handlers, and
        /// again within it only where Files.com redirects it (a 307 or 308 keeps its method and body); after a lost or
        /// failed answer it is attempted again only when it certainly was not carried out, whatever this says. A handler
        /// of your own (<see cref="HttpTransport"/>) must not send it again either.
        /// </remarks>
        [ConfigurationProperty("MaxNetworkRetries", DefaultValue = 3)]
        public int MaxNetworkRetries
        {
            get { return (int)this["MaxNetworkRetries"]; }
            set { this["MaxNetworkRetries"] = value; }
        }

        /// <summary>
        /// The longest wait, in seconds, before a retry of a Files.com API request; the default is 2.
        /// </summary>
        [ConfigurationProperty("MaxNetworkRetryDelay", DefaultValue = 2.0D)]
        public double MaxNetworkRetryDelay
        {
            get { return (double)this["MaxNetworkRetryDelay"]; }
            set { this["MaxNetworkRetryDelay"] = value; }
        }

        private RegionRelocationPolicy regionRelocation = RegionRelocationPolicy.Automatic;

        /// <summary>
        /// Whether an operation moves on to the address Files.com names when it answers that the site is served from
        /// another region; <see cref="RegionRelocationPolicy.Automatic"/> by default. See
        /// <see cref="RegionRelocationPolicy"/>.
        /// </summary>
        /// <exception cref="MissingParameterException">The value is null.</exception>
        public RegionRelocationPolicy RegionRelocation
        {
            get { return regionRelocation; }
            set { regionRelocation = value ?? throw MissingParameterException.For(nameof(RegionRelocation)); }
        }

        /// <summary>
        /// Where a client made with this configuration logs: an application's logger factory, such as the one a host's
        /// dependency injection provides, under categories named for the SDK's classes (all in the <c>FilesCom</c>
        /// namespace). Null, the default, keeps log4net, which the application configures, as before. Each client logs
        /// through its own factory, so clients of one process can log to different places; nothing global is configured.
        /// The client keeps the value it was made with.
        /// </summary>
        /// <remarks>
        /// The SDK logs at Debug, Information, Warning and Error. Messages at Information and above never include
        /// credentials, request or response bodies, paths or signed URLs. Debug adds details such as request and
        /// response bodies, signed URLs and the paths of failed operations, so enable it only where those may be
        /// recorded; the values of a body's credential members, such as a sign-in's password, a new API key or the
        /// session a sign-in creates, are left out of it.
        /// </remarks>
        public ILoggerFactory LoggerFactory { get; set; }

        /// <summary>
        /// The least severe level of message a client made with this configuration logs, whatever its logger enables:
        /// for example <see cref="LogLevel.Warning"/> to keep one client quiet in an application that logs Information.
        /// Null, the default, logs whatever the logger (<see cref="LoggerFactory"/>, or log4net) enables. The client keeps
        /// the value it was made with.
        /// </summary>
        public LogLevel? MinimumLogLevel { get; set; }

        /// <summary>
        /// The name of the application or module using the SDK, which a client made with this configuration sends,
        /// with <see cref="ProductVersion"/>, as the first product of its <c>User-Agent</c> header, before the SDK's own
        /// (<c>Files.com DOTNET SDK</c> and its version), which it keeps. It must be an HTTP token: letters, digits and
        /// <c>!#$%&amp;'*+-.^_`|~</c>, without spaces. Null, the default, sends the SDK's product alone. The client keeps
        /// the value it was made with.
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// The version of <see cref="ProductName"/>, an HTTP token too; null for none. It is sent only with a
        /// <see cref="ProductName"/>.
        /// </summary>
        public string ProductVersion { get; set; }

        // Copies every setting, including ones added later, so that a client given the copy keeps these values
        // however this configuration changes afterward.
        internal FilesConfiguration Copy()
        {
            FilesConfiguration copy = new FilesConfiguration();
            foreach (ConfigurationProperty property in Properties)
            {
                copy[property] = this[property];
            }
            // Not configuration file settings: policies and logger factories cannot be written there.
            copy.regionRelocation = regionRelocation;
            copy.LoggerFactory = LoggerFactory;
            copy.MinimumLogLevel = MinimumLogLevel;
            copy.ProductName = ProductName;
            copy.ProductVersion = ProductVersion;
            return copy;
        }
    }
}