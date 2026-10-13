using FilesCom.Http;
using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom
{
    /// <summary>
    /// The requests a <see cref="FilesApiService"/> sends: Files.com API requests, downloads from storage URLs, and
    /// upload parts.
    /// </summary>
    public interface IFilesApiService
    {
        /// <summary>
        /// Sends a Files.com API request and returns its response, its body already read.
        /// </summary>
        /// <param name="path">The API path after <c>/api/rest/v1</c>, such as <c>/users/{id}</c>; each placeholder is filled from <paramref name="parameters"/>.</param>
        /// <param name="verb">The HTTP method.</param>
        /// <param name="parameters">The operation's parameters, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <returns>The response.</returns>
        Task<HttpResponseMessage> SendRequest(
            string path,
            HttpMethod verb,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        );

        /// <summary>
        /// Downloads the contents at <paramref name="uriString"/> into <paramref name="writeStream"/>.
        /// </summary>
        /// <param name="uriString">The URL to download from, such as a file's <c>DownloadUri</c>.</param>
        /// <param name="writeStream">The stream to write the contents to, from its current position.</param>
        Task StreamDownload(string uriString, Stream writeStream);

        /// <summary>
        /// Uploads one part: reads exactly <paramref name="readLength"/> bytes from <paramref name="readStream"/>,
        /// starting at its current position, and sends them to <paramref name="uriString"/>.
        /// </summary>
        /// <param name="verb">The HTTP method the upload URL takes, such as PUT.</param>
        /// <param name="uriString">The URL to upload the part to.</param>
        /// <param name="readStream">The stream to read the part from, starting at its current position.</param>
        /// <param name="readLength">The number of bytes in the part.</param>
        Task ChunkUpload(HttpMethod verb, string uriString, Stream readStream, Int64 readLength);
    }

    public class FilesApiService : IFilesApiService
    {
        private static readonly SdkLog log = SdkLog.For(typeof(FilesApiService));
        private readonly IHttpClientFactory _clientFactory;
        // Whether this service retries requests itself, by the rules of RetryPolicy, as a client's own service does.
        // A service made with an application's factory sends each request once: retrying, if any, is up to the
        // handlers that factory builds, as it always has been.
        private readonly bool retries;

        // The most of an error answer's body that is read, kept, decoded, parsed or logged; the rest is never read. A
        // storage server's (a transfer URL's, which a caller or a redirect can choose) is read to 64 KiB: storage errors
        // are short documents, and whether one says its URL has expired is judged by its first 4 KiB
        // (SignedUrl.RefusedAsExpired). A Files.com API error's JSON is read to 1 MiB, far above any error object the API
        // sends. An answer cut off there is still an error with its status and headers, which alone decide retrying and
        // waiting; its body is then not taken for a Files.com error object.
        internal const int StorageAnswerBytes = 64 * 1024;
        internal const int ApiErrorBytes = 1024 * 1024;

        /// <summary>
        /// Makes a service that sends requests with the named HTTP clients of <paramref name="clientFactory"/>:
        /// <see cref="FilesClient.HttpFilesApi"/> for API requests and downloads, and <see cref="FilesClient.HttpUpload"/>
        /// for upload parts. It uses the settings of the default client, and the base address of the factory's
        /// clients, when they have one, for API requests.
        /// </summary>
        /// <remarks>
        /// The service sends each request once, however the clients' handlers retry or redirect it; the factory and its
        /// clients remain the application's. A <see cref="FilesClient"/> has a service of its own, which retries by the
        /// client's settings; to change how a client's requests are sent, give it an <see cref="HttpTransport"/>.
        /// </remarks>
        /// <param name="clientFactory">The factory whose named HTTP clients send the requests.</param>
        public FilesApiService(IHttpClientFactory clientFactory) : this(clientFactory, retries: false)
        {
        }

        internal FilesApiService(IHttpClientFactory clientFactory, bool retries)
        {
            _clientFactory = clientFactory;
            this.retries = retries;
        }

        // The exception for an error response whose body has been read: an ApiException, of the class its error type
        // names, for a JSON error object, a LockoutRegionMismatchException for Files.com's former plain-text answer that
        // the site is served from another region (RegionRelocation.IsFormerMismatch), and otherwise an
        // InvalidResponseException. The body, and why it could not be read, are logged only at DEBUG: the exception's
        // message and inner exceptions never quote the response, whose
        // JSON property names can be a signed URL. An API error's message is its error text with the credentials of
        // any URL in it removed, while ApiException.error keeps the text as sent. A transfer's message has only its
        // status, and whether storage said its URL has expired or the request timed out. The body is decoded by
        // HttpContent itself, under the response's Content-Type, so its charset and
        // byte order mark rules apply unchanged. received is what ErrorResponseAsync read of the body; when it is only
        // the start of a longer body (truncated), it is logged as that start and never parsed.
        private static async Task<SdkException> ErrorFor(HttpResponseMessage response, byte[] received, bool isTransfer = false, bool truncated = false)
        {
            string body = await Decode(response, received);
            int status = (int)response.StatusCode;
            log.Debug(truncated ? $"Error response (HTTP {status}), its first {received.Length} bytes: {body}" : $"Error response (HTTP {status}): {body}");

            ResponseError responseError = null;
            if (!truncated)
            {
                try
                {
                    responseError = JsonSerializer.Deserialize<ResponseError>(body, JsonUtil.Options);
                }
                catch (JsonException e)
                {
                    log.Debug("The error response is not a Files.com error", e);
                }
            }
            if (responseError == null)
            {
                if (!isTransfer && !truncated && RegionRelocation.IsFormerMismatch(status, body, response.Headers))
                {
                    var former = new LockoutRegionMismatchException($"HTTP request failed with code {status}: Files.com answered that the site is served from another region.", status, null, response.Headers);
                    former.AnsweredTo(response.RequestMessage);
                    return former;
                }
                bool expired = isTransfer && SignedUrl.RefusedAsExpired(status, received);
                // Only a whole body can say so: the start of a longer one is never parsed.
                bool timedOut = isTransfer && !truncated && RetryPolicy.StorageTimedOut(status, body);
                string failure = isTransfer
                    ? $"Transfer request failed (HTTP {status}){(expired ? ": the storage server says the transfer URL has expired" : timedOut ? ": the storage server says the request timed out" : "")}"
                    : truncated
                        ? $"HTTP request failed with code {status}, and its response is longer than the {ApiErrorBytes} bytes an error response is read to"
                        : $"HTTP request failed with code {status}, and its response is not a Files.com error";
                return new InvalidResponseException(failure, status, response.Headers) { StorageSaysExpired = expired, StorageSaysTimedOut = timedOut };
            }

            string message = isTransfer
                ? $"Transfer request failed (HTTP {status})"
                : string.IsNullOrEmpty(responseError.error) ? $"HTTP request failed with code {status}" : DiagnosticText.WithoutUrlCredentials(responseError.error);
            ApiException error = ApiErrors.For(message, status, responseError, response.Headers);
            if (!isTransfer && error is LockoutRegionMismatchException mismatch)
            {
                mismatch.AnsweredTo(response.RequestMessage);
            }
            return error;
        }

        // The exception for an error response whose headers have arrived, as ErrorFor reads it once its body has been
        // read too. The status and headers alone decide whether the request is retried and how long to wait, so when the
        // body stops arriving or its connection fails, the answer is still an error with them: an
        // InvalidResponseException, without the part of the body that arrived, and with why the body failed logged at
        // DEBUG. Cancellation is thrown as it is. The body is read only to its bound (StorageAnswerBytes for a transfer,
        // ApiErrorBytes for the API), before anything of it is kept, decoded, parsed or logged.
        private static async Task<SdkException> ErrorResponseAsync(HttpResponseMessage response, NetworkWatch watch, bool isTransfer)
        {
            byte[] body;
            bool truncated;
            try
            {
                (body, truncated) = await ResponseBody.ReadPrefixAsync(response, watch, isTransfer ? StorageAnswerBytes : ApiErrorBytes);
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                int status = (int)response.StatusCode;
                log.Debug($"The body of an error response (HTTP {status}) could not be read", e);
                string failure = isTransfer
                    ? $"Transfer request failed (HTTP {status})"
                    : $"HTTP request failed with code {status}, and its response could not be read";
                return new InvalidResponseException(failure, status, response.Headers);
            }
            return await ErrorFor(response, body, isTransfer, truncated);
        }

        // The failure of an attempt that RetryPolicy runs, answered with an error response: as ErrorResponseAsync
        // reads it, or, when its body cannot be decoded, that failure with the response's status and headers, which
        // RetryPolicy retries by and then throws as it is.
        private static async Task<Exception> AttemptFailureAsync(HttpResponseMessage response, NetworkWatch watch, bool isTransfer)
        {
            try
            {
                return await ErrorResponseAsync(response, watch, isTransfer);
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                return new RetryPolicy.AnsweredFailure(response, e);
            }
        }

        // The IFilesApiService methods run with the default client's settings, looked up once per call. The SDK's
        // own requests use the overloads that take the settings their operation captured.
        /// <summary>
        /// Sends a Files.com API request with the default client's settings (<see cref="FilesClient.Instance"/>).
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
        /// <exception cref="SdkException">No client has been made, so there are no default client settings to send the request with.</exception>
        public async Task<HttpResponseMessage> SendRequest(
            string path,
            HttpMethod verb,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        )
        {
            OperationContext context = OperationContext.OfDefaultClient() ?? throw FilesClient.NoDefaultClient(options, SendsCredentials(path, verb));
            return await SendRequest(context, path, verb, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Downloads the contents at <paramref name="uriString"/> into <paramref name="writeStream"/>, with the default
        /// client's settings (<see cref="FilesClient.Instance"/>).
        /// </summary>
        /// <param name="uriString">The URL to download from, such as a file's <c>DownloadUri</c>.</param>
        /// <param name="writeStream">The stream to write the contents to, from its current position.</param>
        /// <exception cref="SdkException">No client has been made, so there are no default client settings to download with.</exception>
        public async Task StreamDownload(string uriString, Stream writeStream)
        {
            await StreamDownload(DefaultClientContext().Network, uriString, writeStream, null, CancellationToken.None);
        }

        /// <summary>
        /// Uploads one part: reads exactly <paramref name="readLength"/> bytes from <paramref name="readStream"/>,
        /// starting at its current position, and sends them to <paramref name="uriString"/>, with the default client's
        /// settings, or the default settings when no client has been made.
        /// </summary>
        /// <param name="verb">The HTTP method the upload URL takes, such as PUT.</param>
        /// <param name="uriString">The URL to upload the part to.</param>
        /// <param name="readStream">The stream to read the part from, starting at its current position.</param>
        /// <param name="readLength">The number of bytes in the part.</param>
        /// <exception cref="MissingParameterException"><paramref name="readStream"/> is null.</exception>
        /// <exception cref="InvalidParameterException"><paramref name="readLength"/> is negative or greater than Int32.MaxValue.</exception>
        public Task ChunkUpload(HttpMethod verb, string uriString, Stream readStream, Int64 readLength)
        {
            // Needs no default client, as it never has: without one, the default settings apply.
            OperationContext context = OperationContext.OfDefaultClient();
            NetworkSettings network = context != null ? context.Network : NetworkSettings.From(new FilesConfiguration());
            return ChunkUpload(network, verb, SignedUrl.Of(uriString), readStream, readLength, null, CancellationToken.None);
        }

        // The default client's context, for a download, which sends no credentials but needs the client's settings.
        private static OperationContext DefaultClientContext()
        {
            OperationContext context = OperationContext.OfDefaultClient();
            if (context == null)
            {
                throw FilesClient.NoDefaultClient();
            }
            return context;
        }

        // Whether a request to path sends the Files.com credentials: every one but the request that creates a session.
        internal static bool SendsCredentials(string path, HttpMethod verb)
        {
            return !(path == "/sessions" && verb == HttpMethod.Post);
        }

        internal async Task<HttpResponseMessage> SendRequest(
            OperationContext context,
            string path,
            HttpMethod verb,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            HttpClient httpClient = _clientFactory.CreateClient(FilesClient.HttpFilesApi);
            string parsedPath = ParsePathParameters(path, parameters);
            // A factory given to the public constructor may set its own base address; the SDK's clients have none. An
            // operation that has moved to its site's region sends there.
            string query = "";
            UriBuilder AddressAtOrigin()
            {
                return new UriBuilder((context.RelocatedOrigin != null ? new Uri(context.RelocatedOrigin) : httpClient.BaseAddress ?? new Uri(context.BaseUrl)).ToString())
                {
                    Path = $"api/rest/v1{parsedPath}",
                    Query = query,
                };
            }
            string jsonString = "";
            bool hasBody = false;
            RequestBody attachments = null;

            switch (verb.Method)
            {
                case "GET":
                case "HEAD":
                case "DELETE":
                    query = QueryOf(parameters);
                    break;
                default:
                    if (RequestBody.HasFiles(parameters))
                    {
                        attachments = await Task.Run(() => RequestBody.For(parameters));
                    }
                    else
                    {
                        jsonString = await Task.Run(() => JsonSerializer.Serialize(parameters, JsonUtil.Options));
                    }
                    hasBody = true;
                    break;
            }

            // A request that only asks for something, or one its operation says Files.com can receive twice, is sent again
            // after any failure that may be temporary. Any other request may have taken effect once it may have reached
            // Files.com, so it is sent again only after a failure that shows it was not carried out (RetryPolicy); when
            // the failure that ends it could have followed it taking effect, its exception says it may have been
            // processed, with or without retries.
            bool replayable = context.ReplaySafe || verb.Method == "GET" || verb.Method == "HEAD" || verb.Method == "OPTIONS";

            // Every attempt goes to this address, until the operation moves to its site's region.
            UriBuilder uri = AddressAtOrigin();
            Uri requestUri = uri.Uri;
            string endpoint = DiagnosticText.Endpoint(verb, parsedPath);
            string relocation = null;
            // The last attempt's error response, when its body could not be decoded.
            RetryPolicy.AnsweredFailure undecodable = null;
            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(HttpRequestHeader.Accept.ToString(), "application/json"),
            };

            bool requiresAuth = SendsCredentials(path, verb);

            if (requiresAuth)
            {
                object workspaceId = options.ContainsKey("workspace_id") ? options["workspace_id"] : context.WorkspaceId;

                if (options.ContainsKey("session_id"))
                {
                    if (!(options["session_id"] is string))
                    {
                        throw new InvalidParameterException("Bad option: session_id must be of type string");
                    }

                    headers.Add(new KeyValuePair<string, string>("X-FilesAPI-Auth", options["session_id"].ToString()));
                }
                else if (options.ContainsKey("api_key"))
                {
                    if (!(options["api_key"] is string))
                    {
                        throw new InvalidParameterException("Bad option: api_key must be of type string");
                    }

                    headers.Add(new KeyValuePair<string, string>("X-FilesAPI-Key", (string)options["api_key"]));
                }
                else if (context.SessionId != null && context.SessionId.Length > 0)
                {
                    headers.Add(new KeyValuePair<string, string>("X-FilesAPI-Auth", context.SessionId));
                }
                else if (context.ApiKey != null && context.ApiKey.Length > 0)
                {
                    headers.Add(new KeyValuePair<string, string>("X-FilesAPI-Key", context.ApiKey));
                }
                else
                {
                    throw new AuthenticationException($"Authentication required for {endpoint}: set an API key or session ID in the client's configuration, or api_key or session_id in the request's options.");
                }

                if (workspaceId != null && !string.IsNullOrWhiteSpace(workspaceId.ToString()))
                {
                    headers.Add(new KeyValuePair<string, string>("X-Files-Workspace-Id", workspaceId.ToString()));
                }
            }

            if (!string.IsNullOrWhiteSpace(context.Language))
            {
                headers.Add(new KeyValuePair<string, string>("Accept-Language", context.Language));
            }

            // Each attempt sends a new request with the same method, address, headers and body. A body with files is
            // timed as an upload's is, write by write, since it can be large.
            HttpRequestMessage NewRequest(NetworkWatch watch)
            {
                var request = new HttpRequestMessage(verb, requestUri);
                try
                {
                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                    if (attachments != null)
                    {
                        request.Content = new WatchedContent(attachments.CreateContent(watch.Token), watch);
                    }
                    else if (hasBody)
                    {
                        request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                    }
                    else if (!replayable)
                    {
                        // A request that can change something and has no body of its own, a DELETE, goes with an
                        // empty one (Content-Length: 0, as common HTTP clients send such a request), so that the
                        // handler does not send it again. SocketsHttpHandler sends a request without content again
                        // itself, up to three times and below every handler, when its connection closes before any
                        // answer: that is safe for a request that only asks for something, not for one that may have
                        // taken effect.
                        request.Content = new ByteArrayContent(Array.Empty<byte>());
                    }
                    return request;
                }
                catch
                {
                    request.Dispose();
                    throw;
                }
            }

            try
            {
                if (attachments != null)
                {
                    await attachments.OpenAsync(cancellationToken);
                }

                log.Info($"Sending {endpoint}");
                log.Debug($"Sending {verb} request: {requestUri}");
                // The body, with the values of its credential members, such as a sign-in's password, left out.
                log.Debug($"content: {attachments?.Description ?? DiagnosticText.WithoutSecretValues(jsonString)}");

                return await Retries(context.Network, storage: false).RunAsync(
                    endpoint,
                    AttemptAsync,
                    replayable ? (Func<Exception, TimeSpan, bool>)null : (failure, delay) => RetryPolicy.NotCarriedOut(failure),
                    cancellationToken);
            }
            // A file's source failed, as it was opened, checked or read: its own exception, as it is, once no retry,
            // relocation or network failure handling can have been applied to it.
            catch (RequestBody.SourceFailedException e)
            {
                ExceptionDispatchInfo.Capture(e.Failure).Throw();
                throw;
            }
            // An error response whose body cannot be decoded fails as HttpContent does, unless the request can change
            // something: then it is an InvalidResponseException with the response's status and headers, which can say
            // that it may have taken effect.
            catch (Exception e) when (undecodable != null && e == undecodable.InnerException && !replayable)
            {
                var unreadable = new InvalidResponseException($"HTTP request failed with code {undecodable.Status}, and its response could not be read", undecodable.Status, undecodable.Headers) { RequestMayHaveBeenProcessed = true };
                DiagnosticText.ErrorFailed(log, "API request failed", unreadable, $"The HTTP {undecodable.Status} response could not be read");
                log.Debug("Why the response could not be read", e);
                throw unreadable;
            }
            // The failure itself is logged at DEBUG, not kept as an inner exception: a message such as an invalid
            // response header line can quote the response, and a caller's error reporter records inner exceptions.
            catch (HttpRequestException e)
            {
                log.Debug("API request failed", e);
                bool mayHaveBeenProcessed = !replayable && !RetryPolicy.NotCarriedOut(e);
                throw new ApiConnectionException(DiagnosticText.WithoutUrlCredentials(e.Message) + (mayHaveBeenProcessed ? MayHaveBeenProcessed : ""), mayHaveBeenProcessed);
            }
            catch (NetworkTimeoutException e) when (!replayable && !RetryPolicy.NotCarriedOut(e))
            {
                log.Debug("API request timed out", e);
                throw new NetworkTimeoutException(e.Message + MayHaveBeenProcessed, requestMayHaveBeenProcessed: true);
            }
            // An error response keeps its type, status and headers, and says whether the request may have taken effect:
            // whatever it says, unless it shows the request was not carried out.
            catch (SdkException e) when (IsErrorResponse(e))
            {
                if (!replayable && !RetryPolicy.NotCarriedOut(e))
                {
                    e.RequestMayHaveBeenProcessed = true;
                }
                DiagnosticText.ErrorFailed(log, "API request failed", e);
                throw;
            }
            finally
            {
                attachments?.Dispose();
            }

            // One attempt, at the operation's origin. The attempt after a move the policy allowed goes to the new
            // origin, and so does every later request of the operation.
            async Task<HttpResponseMessage> AttemptAsync(CancellationToken token)
            {
                if (relocation != null)
                {
                    context.Relocate(relocation);
                    relocation = null;
                    requestUri = AddressAtOrigin().Uri;
                }
                undecodable = null;
                try
                {
                    return await SendApiAttemptAsync(httpClient, NewRequest, context.Network, createsSession: !requiresAuth, token);
                }
                catch (RetryPolicy.AnsweredFailure answered)
                {
                    undecodable = answered;
                    throw;
                }
                catch (LockoutRegionMismatchException mismatch) when (mismatch.Refusal == null)
                {
                    Exception outcome = RegionRelocation.Consider(mismatch, context);
                    relocation = (outcome as RegionRelocation.Requested)?.Origin;
                    throw outcome;
                }
            }
        }

        // What the message of a failed request that may have taken effect ends with.
        private const string MayHaveBeenProcessed = " The request may have reached Files.com and taken effect, so the SDK did not send it again: check whether it did before sending it again.";

        // One attempt: sends the request and reads the whole response, which it returns with its body in memory, as
        // the SDK's callers expect. An error response is thrown as the exception it is read as. A failure to read a
        // file the request sends is a SourceFailedException, however the HTTP stack reports it, and is not retried.
        private static async Task<HttpResponseMessage> SendApiAttemptAsync(HttpClient httpClient, Func<NetworkWatch, HttpRequestMessage> newRequest, NetworkSettings network, bool createsSession, CancellationToken cancellationToken)
        {
            using (var watch = new NetworkWatch(network, cancellationToken))
            using (HttpRequestMessage request = newRequest(watch))
            {
                var attachments = (request.Content as WatchedContent)?.Inner as RequestBody.AttachmentContent;
                watch.Begin(attachments != null ? NetworkWait.Sending : NetworkWait.ResponseHeaders);
                HttpResponseMessage response;
                try
                {
                    response = await SendAsync(httpClient, request, watch, e => new InvalidParameterException(DiagnosticText.WithoutUrlCredentials(e.Message)));
                }
                catch (Exception) when (attachments?.SourceFailure != null)
                {
                    throw new RequestBody.SourceFailedException(attachments.SourceFailure);
                }
                try
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw await AttemptFailureAsync(response, watch, isTransfer: false);
                    }
                    byte[] body = await ResponseBody.ReadAllAsync(response, watch);
                    Buffer(response, body);
                    // Every response, whatever the operation makes of it: an object, a page, or nothing, with the values
                    // of its credential members, such as a new API key or the session a sign-in created, left out.
                    if (log.IsDebugEnabled)
                    {
                        log.Debug($"Response (HTTP {(int)response.StatusCode}): {DiagnosticText.WithoutSecretValues(await Decode(response, body), createsSession)}");
                    }
                    return response;
                }
                catch
                {
                    response.Dispose();
                    throw;
                }
            }
        }

        internal async Task StreamDownload(NetworkSettings network, string uriString, Stream writeStream, Func<CancellationToken, Task<SignedUrl>> renewUrl, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            HttpClient httpClient = _clientFactory.CreateClient(FilesClient.HttpFilesApi);
            // An address that is not a URL fails as it is, before anything is sent.
            _ = new Uri(uriString);
            SignedUrl url = SignedUrl.Of(uriString);
            bool renewing = false;

            // What cannot be taken back is writing to the caller's stream. From the first write on, a failure is not
            // retried: a write that fails may already have kept some bytes, and the stream cannot be rewound. A
            // write's own failure is the caller's, whatever its type, and is thrown as it is.
            bool writing = false;
            Exception destinationFailure = null;
            async Task WriteAsync(byte[] buffer, int count)
            {
                writing = true;
                try
                {
                    await writeStream.WriteAsync(buffer, 0, count, cancellationToken);
                }
                catch (Exception e)
                {
                    destinationFailure = e;
                    throw;
                }
            }

            // A download storage refuses saying its URL has expired, before anything was written, gets a new URL and
            // starts again at once, at most SignedUrl.MaxExpiredRenewals times.
            bool renew = false;
            try
            {
                for (int renewedForExpiry = 0; ; renewedForExpiry++)
                {
                    try
                    {
                        await Retries(network, storage: true).RunAsync(
                            "the download",
                            async token =>
                            {
                                if (renewUrl != null && (renew || url.ExpiresWithin(SignedUrl.RenewalMargin)))
                                {
                                    renewing = true;
                                    url = await RenewAsync(renewUrl, token);
                                    renewing = false;
                                    renew = false;
                                }
                                await DownloadAttemptAsync(httpClient, url, WriteAsync, network, token);
                                return true;
                            },
                            // A URL that will have expired by the retry is worth retrying only when it can be renewed first,
                            // and a renewal that failed has already been retried as an API request.
                            (failure, delay) => !writing && !renewing && (renewUrl != null || !url.ExpiresWithin(delay)),
                            cancellationToken);
                        break;
                    }
                    catch (InvalidResponseException e) when (e.StorageSaysExpired && renewUrl != null && !writing && renewedForExpiry < SignedUrl.MaxExpiredRenewals)
                    {
                        log.Info("Renewing the URL of a download: the storage server says it has expired");
                        renew = true;
                    }
                }
            }
            catch (HttpRequestException e) when (e != destinationFailure)
            {
                log.Debug("Download transport error", e);
                throw new ApiConnectionException("Download request failed (HttpRequestException)");
            }
            catch (SdkException e) when (e != destinationFailure && IsErrorResponse(e))
            {
                DiagnosticText.ErrorFailed(log, "Download request failed", e);
                throw;
            }
            log.Debug($"Successfully downloaded {url.Text}");
        }

        private static async Task DownloadAttemptAsync(HttpClient httpClient, SignedUrl url, Func<byte[], int, Task> write, NetworkSettings network, CancellationToken cancellationToken)
        {
            using (var watch = new NetworkWatch(network, cancellationToken))
            using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(url.Text)))
            {
                watch.Begin(NetworkWait.ResponseHeaders);
                using (HttpResponseMessage response = await SendAsync(httpClient, request, watch, e => new InvalidParameterException("Invalid download request")))
                {
                    DownloadResponse.CheckNotFrontendApp(response);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw await AttemptFailureAsync(response, watch, isTransfer: true);
                    }
                    DownloadResponse.CheckWhole(response);
                    await ResponseBody.CopyToAsync(response, watch, write);
                }
            }
        }

        // Uploads one part to url, which renewUrl, when given, replaces with a new URL for the same part once it is
        // about to expire.
        internal async Task ChunkUpload(NetworkSettings network, HttpMethod verb, SignedUrl url, Stream readStream, Int64 readLength, Func<CancellationToken, Task<SignedUrl>> renewUrl, CancellationToken cancellationToken)
        {
            if (readStream == null)
            {
                throw MissingParameterException.For(nameof(readStream));
            }
            // The existing implementation buffers a part in a single byte array.
            if (readLength < 0 || readLength > int.MaxValue)
            {
                throw InvalidParameterException.OutOfRange(nameof(readLength), readLength, "An upload part must be from 0 to Int32.MaxValue bytes long");
            }
            cancellationToken.ThrowIfCancellationRequested();

            HttpClient httpClient = _clientFactory.CreateClient(FilesClient.HttpUpload);
            // An address that is not a URL fails as it is, before the part is read.
            _ = new Uri(url.Text);
            // Reading the whole part before sending means every attempt, even to a renewed URL, sends these same bytes.
            byte[] part = await ReadPart(readStream, (int)readLength, cancellationToken);
            bool renewing = false;

            log.Info($"Sending {verb} upload request");
            log.Debug($"Sending {verb} request: {url.Text}");
            log.Debug($"content: {readLength} bytes");

            // A part storage refuses saying its URL has expired gets a new URL for the same part and is sent again at once,
            // with the same bytes, at most SignedUrl.MaxExpiredRenewals times.
            bool renew = false;
            try
            {
                for (int renewedForExpiry = 0; ; renewedForExpiry++)
                {
                    try
                    {
                        await Retries(network, storage: true).RunAsync(
                            "the upload part",
                            async token =>
                            {
                                if (renewUrl != null && (renew || url.ExpiresWithin(SignedUrl.RenewalMargin)))
                                {
                                    renewing = true;
                                    url = await RenewAsync(renewUrl, token);
                                    renewing = false;
                                    renew = false;
                                }
                                await ChunkAttemptAsync(httpClient, verb, url, part, network, token);
                                return true;
                            },
                            (failure, delay) => !renewing && (renewUrl != null || !url.ExpiresWithin(delay)),
                            cancellationToken);
                        break;
                    }
                    catch (InvalidResponseException e) when (e.StorageSaysExpired && renewUrl != null && renewedForExpiry < SignedUrl.MaxExpiredRenewals)
                    {
                        log.Info("Renewing the URL of an upload part: the storage server says it has expired");
                        renew = true;
                    }
                }
            }
            catch (HttpRequestException e)
            {
                log.Debug("Upload transport error", e);
                throw new ApiConnectionException("Upload request failed (HttpRequestException)");
            }
            catch (SdkException e) when (IsErrorResponse(e))
            {
                DiagnosticText.ErrorFailed(log, "Upload request failed", e);
                throw;
            }
        }

        private static async Task ChunkAttemptAsync(HttpClient httpClient, HttpMethod verb, SignedUrl url, byte[] part, NetworkSettings network, CancellationToken cancellationToken)
        {
            using (var watch = new NetworkWatch(network, cancellationToken))
            using (var httpRequestMessage = new HttpRequestMessage
            {
                Method = verb,
                RequestUri = new Uri(url.Text),
                Headers = {
                    { HttpRequestHeader.Accept.ToString(), "application/octet-stream" },
                    { HttpRequestHeader.ContentLength.ToString(), part.Length.ToString() },
                },
                Content = new WatchedContent(new ByteArrayContent(part), watch),
            })
            {
                watch.Begin(NetworkWait.Sending);
                using (HttpResponseMessage response = await SendAsync(httpClient, httpRequestMessage, watch, e => new InvalidParameterException("Invalid upload request")))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw await AttemptFailureAsync(response, watch, isTransfer: true);
                    }
                    // The storage server's answer, logged to its bound as an error answer is.
                    log.Debug(await Decode(response, (await ResponseBody.ReadPrefixAsync(response, watch, StorageAnswerBytes)).Body));
                }
            }
        }

        // Sends one attempt of an upload part for the transfer engine, with the method and headers the server
        // negotiated for it, and returns the outcome rather than retrying: the engine decides whether to retry, and
        // measures each attempt once. An attempt is exactly one request, and the redirects StorageRedirectHandler
        // follows within it; nothing here sends the body again on its own. The request carries no Files.com
        // credentials. It disposes content. Cancellation is thrown, not returned as an outcome; a timeout is a
        // failure that may be temporary.
        internal async Task<UploadPartAttempt> SendUploadPart(NetworkSettings network, HttpMethod verb, string uriString, IEnumerable<KeyValuePair<string, string>> headers, HttpContent content, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            HttpClient httpClient = _clientFactory.CreateClient(FilesClient.HttpUpload);
            HttpResponseMessage response;
            Stopwatch sending = new Stopwatch();
            using (var watch = new NetworkWatch(network, cancellationToken))
            using (var httpRequestMessage = new HttpRequestMessage { Method = verb, Content = new WatchedContent(content, watch) })
            {
                try
                {
                    httpRequestMessage.RequestUri = new Uri(uriString);
                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        if (!httpRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value) && !httpRequestMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value))
                        {
                            throw new InvalidResponseException($"The server asked for an upload header named \"{header.Key}\", which cannot be sent.");
                        }
                    }

                    log.Info($"Sending {verb} upload part request");
                    log.Debug($"Sending {verb} request: {uriString}");
                    watch.Begin(NetworkWait.Sending);
                    sending.Start();
                    response = await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, watch.Token);
                }
                catch (OperationCanceledException e) when (watch.TimedOut(e))
                {
                    return UploadPartAttempt.Failed(watch.Timeout(), transient: true, status: null, duration: sending.Elapsed);
                }
                catch (HttpRequestException e)
                {
                    log.Debug("Upload transport error", e);
                    return UploadPartAttempt.Failed(new ApiConnectionException("Upload request failed (HttpRequestException)"), RetryPolicy.IsRetryable(e), status: null, duration: sending.Elapsed);
                }
                catch (Exception e) when (IsUnsendable(e) || e is UriFormatException)
                {
                    log.Debug("Upload parameter error", e);
                    return UploadPartAttempt.Failed(new InvalidParameterException("Invalid upload request"), transient: false, status: null, duration: sending.Elapsed);
                }

                using (response)
                {
                    int status = (int)response.StatusCode;
                    if (status >= 300 && status < 400)
                    {
                        // A redirect StorageRedirectHandler would not follow. Its Location is not repeated: it may be signed.
                        log.Debug($"Upload part request redirected (HTTP {status}) somewhere it cannot follow");
                        return UploadPartAttempt.Failed(new InvalidResponseException($"Transfer request failed (HTTP {status}): the storage server redirected the part in a way it cannot be sent."), transient: false, status: status, duration: sending.Elapsed);
                    }
                    if (response.IsSuccessStatusCode)
                    {
                        // The status and headers are the answer; a part's response has no body to wait for.
                        return UploadPartAttempt.Accepted(ETagOf(response), status, sending.Elapsed);
                    }
                    // Server errors, timeouts and throttling: the part was not accepted, and may be next time. Whether
                    // that is so comes from the status, however the rest of the answer arrives, or from storage's whole
                    // answer that it stopped waiting for the part (InvalidResponseException.StorageSaysTimedOut).
                    Exception failure;
                    try
                    {
                        failure = await ErrorResponseAsync(response, watch, isTransfer: true);
                        log.Error(failure.Message);
                    }
                    catch (Exception e) when (!(e is OperationCanceledException))
                    {
                        // A body in a charset HttpContent cannot decode, which it reports as it is.
                        failure = e;
                    }
                    return UploadPartAttempt.Failed(failure, RetryPolicy.IsRetryable(status) || RetryPolicy.IsRetryable(failure), status, sending.Elapsed, RetryPolicy.RetryAfterOf(response.Headers));
                }
            }
        }

        // The HTTP client storage transfers are sent with (FilesClient.HttpUpload): StorageRedirectHandler, then the
        // client's transport. It carries no Files.com credentials.
        internal HttpClient StorageClient()
        {
            return _clientFactory.CreateClient(FilesClient.HttpUpload);
        }

        // Sends one storage request for the transfer engine, waiting only for the response headers, within the watch's
        // timeouts, as the SDK's own requests are sent.
        internal static Task<HttpResponseMessage> SendStorageRequestAsync(HttpClient httpClient, HttpRequestMessage request, NetworkWatch watch)
        {
            return SendAsync(httpClient, request, watch, e => new InvalidParameterException("Invalid download request"));
        }

        // The failure an error answer to a storage request is read as, as RetryPolicy retries and throws it.
        internal static Task<Exception> StorageFailureAsync(HttpResponseMessage response, NetworkWatch watch)
        {
            return AttemptFailureAsync(response, watch, isTransfer: true);
        }

        // A new URL from renewUrl, which must not have expired already: renewing it again would get no further.
        private static async Task<SignedUrl> RenewAsync(Func<CancellationToken, Task<SignedUrl>> renewUrl, CancellationToken cancellationToken)
        {
            SignedUrl renewed = await renewUrl(cancellationToken);
            if (renewed.ExpiresWithin(TimeSpan.Zero))
            {
                throw new InvalidResponseException("Files.com replaced a transfer URL that was about to expire with one that has already expired.");
            }
            return renewed;
        }

        // Sends request for one attempt, waiting only for the response headers, within the watch's timeouts. A request
        // that cannot be sent as it is, such as one to a URL with an unsupported scheme, is thrown as unsendable makes it.
        private static async Task<HttpResponseMessage> SendAsync(HttpClient httpClient, HttpRequestMessage request, NetworkWatch watch, Func<Exception, SdkException> unsendable)
        {
            try
            {
                return await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, watch.Token);
            }
            catch (OperationCanceledException e) when (watch.TimedOut(e))
            {
                throw watch.Timeout();
            }
            catch (Exception e) when (IsUnsendable(e))
            {
                log.Debug("The request could not be sent", e);
                throw unsendable(e);
            }
        }

        // The exception for an error response, as ErrorFor reads it. A custom handler can throw the same types, so their
        // messages are not logged below DEBUG.
        private static bool IsErrorResponse(SdkException e)
        {
            return e is ApiException || (e is InvalidResponseException unreadable && unreadable.httpStatus > 0);
        }

        // An exception HttpClient throws for a request it cannot send, before sending it.
        private static bool IsUnsendable(Exception e)
        {
            return (e is InvalidOperationException && !(e is ObjectDisposedException)) || e is ArgumentException || e is NotSupportedException;
        }

        // Replaces the response's content with its body, now read, keeping its headers.
        private static void Buffer(HttpResponseMessage response, byte[] body)
        {
            HttpContent received = response.Content;
            var buffered = new ByteArrayContent(body);
            if (received != null)
            {
                foreach (var header in received.Headers)
                {
                    buffered.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
            response.Content = buffered;
            received?.Dispose();
        }

        private static async Task<string> Decode(HttpResponseMessage response, byte[] body)
        {
            using (ByteArrayContent received = new ByteArrayContent(body))
            {
                received.Headers.ContentType = response.Content?.Headers.ContentType;
                return await received.ReadAsStringAsync();
            }
        }

        private RetryPolicy Retries(NetworkSettings network, bool storage)
        {
            NetworkSettings settings = retries ? network : network.WithoutRetries();
            return storage ? RetryPolicy.ForStorage(settings) : RetryPolicy.ForApi(settings);
        }

        // The ETag exactly as the server sent it, quotes included, or null when there is none.
        private static string ETagOf(HttpResponseMessage response)
        {
            IEnumerable<string> values;
            if (!response.Headers.TryGetValues("ETag", out values))
            {
                return null;
            }
            foreach (string value in values)
            {
                string etag = value.Trim();
                return etag.Length > 0 ? etag : null;
            }
            return null;
        }

        // ReadAsync may return fewer bytes than requested, so keep reading until the part is full. Reading stops at
        // the part's length, which leaves the rest of the stream for the next part. A stream may ignore the token
        // during a read, but once it is cancelled no further read starts and a read that completes anyway is not used.
        private static async Task<byte[]> ReadPart(Stream readStream, int partLength, CancellationToken cancellationToken)
        {
            byte[] part = new byte[partLength];
            int bytesRead = 0;
            while (bytesRead < partLength)
            {
                cancellationToken.ThrowIfCancellationRequested();
                int count = await readStream.ReadAsync(part, bytesRead, partLength - bytesRead, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                if (count == 0)
                {
                    throw new EndOfStreamException($"The upload stream ended after {bytesRead} of the {partLength} bytes expected for this part.");
                }
                bytesRead += count;
            }
            return part;
        }

        // The query string of a request that sends its parameters in its URL (GET, HEAD and DELETE), form-encoded. A
        // parameter whose value is a dictionary, such as sort_by or a filter, sends one entry per key, as name[key]=value,
        // whatever dictionary type the caller built: the README shows Dictionary<string, string>, and a
        // Dictionary<string, object> is as natural when the parameters are one. A value of any other type that is not
        // text, a number, a boolean or an enum, such as an array, a nested dictionary or a model, cannot be sent in a
        // query string in a form Files.com reads, and is refused: before this, it was sent as its type's name, so that a
        // filter the caller passed as a Dictionary<string, object> silently selected nothing and the list came back
        // unfiltered.
        internal static string QueryOf(Dictionary<string, object> parameters)
        {
            var queryParams = new List<KeyValuePair<string, string>>();
            foreach (KeyValuePair<string, object> parameter in parameters)
            {
                if (parameter.Key.Length == 0)
                {
                    continue;
                }
                if (parameter.Value is System.Collections.IDictionary entries)
                {
                    foreach (System.Collections.DictionaryEntry entry in entries)
                    {
                        string name = parameter.Key + "[" + entry.Key + "]";
                        queryParams.Add(new KeyValuePair<string, string>(name, QueryText(name, entry.Value)));
                    }
                }
                else
                {
                    queryParams.Add(new KeyValuePair<string, string>(parameter.Key, QueryText(parameter.Key, parameter.Value)));
                }
            }
            return new FormUrlEncodedContent(queryParams).ReadAsStringAsync().Result;
        }

        // The text of one query value: null stays empty, as before; text, numbers, booleans and enums are written as
        // they always were (a boolean as True or False, which Files.com reads).
        private static string QueryText(string name, object value)
        {
            if (value == null || value is string || value is bool || value is Enum || value is DateTime || value is DateTimeOffset || value is Guid || value is char
                || value is byte || value is sbyte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong
                || value is float || value is double || value is decimal)
            {
                return value?.ToString();
            }
            throw new InvalidParameterException($"Bad parameter: {name} is a {value.GetType().Name}, which cannot be sent in a query string. Pass text, a number, a boolean, or a Dictionary of them.", name);
        }

        /// <summary>
        /// Fills each <c>{name}</c> placeholder of <paramref name="path"/> with the value of that parameter.
        /// </summary>
        /// <param name="path">The API path, such as <c>/users/{id}</c>.</param>
        /// <param name="parameters">The operation's parameters, keyed by their API names.</param>
        /// <returns>The path with its placeholders filled.</returns>
        /// <exception cref="MissingParameterException">A parameter that fills a placeholder is null.</exception>
        protected static string ParsePathParameters(string path, Dictionary<string, object> parameters)
        {
            Regex regex = new Regex(@"(?<param>\{\w+\})", RegexOptions.Compiled);
            MatchCollection matches = regex.Matches(path);

            string parsedPath = path;
            foreach (Match match in matches)
            {
                Group param = match.Groups["param"];
                string name = param.Value.Substring(1, param.Value.Length - 2);
                object value = parameters[name];

                if (value == null)
                {
                    throw MissingParameterException.For(name);
                }

                parsedPath = parsedPath.Replace(param.Value, value.ToString());
            }
            return parsedPath;
        }
    }

    // The outcome of one attempt to send an upload part: accepted, with the ETag the server returned (if any), or
    // failed, with the exception that describes the failure and whether trying again could succeed. It also says what
    // the storage server answered and how long the request took, from sending it to receiving the answer, so the
    // transfer engine can measure the network without counting its own waits.
    internal sealed class UploadPartAttempt
    {
        private UploadPartAttempt(string etag, Exception error, bool isTransient, int? statusCode, TimeSpan duration, TimeSpan? retryAfter)
        {
            ETag = etag;
            Error = error;
            IsTransient = isTransient;
            StatusCode = statusCode;
            Duration = duration;
            RetryAfter = retryAfter;
        }

        internal static UploadPartAttempt Accepted(string etag, int statusCode, TimeSpan duration)
        {
            return new UploadPartAttempt(etag, null, false, statusCode, duration, null);
        }

        internal static UploadPartAttempt Failed(Exception error, bool transient, int? status, TimeSpan duration, TimeSpan? retryAfter = null)
        {
            return new UploadPartAttempt(null, error, transient, status, duration, retryAfter);
        }

        internal bool Succeeded => Error == null;

        // Storage answered that the part's URL has expired (SignedUrl.RefusedAsExpired): a new URL for the same part can
        // be tried.
        internal bool UrlExpired => (Error as InvalidResponseException)?.StorageSaysExpired == true;

        internal string ETag { get; }

        internal Exception Error { get; }

        // A transport failure or timeout, a server error (5xx other than 501), a request timeout (408 or storage's
        // RetryPolicy.StorageTimedOut) or throttling (429): RetryPolicy's failures that may be temporary.
        internal bool IsTransient { get; }

        // The storage server's HTTP status, or null when no answer was received.
        internal int? StatusCode { get; }

        // A failure asking the client to slow down: 429 or 503, or any failure answered with Retry-After.
        internal bool IsThrottled => !Succeeded && (StatusCode == 429 || StatusCode == (int)HttpStatusCode.ServiceUnavailable || RetryAfter.HasValue);

        // From sending the request to receiving its answer, including redirects followed on the way.
        internal TimeSpan Duration { get; }

        // The server's Retry-After, when it sent one.
        internal TimeSpan? RetryAfter { get; }
    }
}