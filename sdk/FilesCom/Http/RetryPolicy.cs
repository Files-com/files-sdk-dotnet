using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FilesCom.Http
{
    // How the requests of one operation are retried after a failure that may be temporary, up to MaxNetworkRetries
    // times: these are the SDK's attempts, each a request handed to the HTTP handler, and with no retries there is one.
    // The handler may send a request without content again within an attempt, when its connection closes before any
    // answer (SocketsHttpHandler does, for a GET), which this policy neither counts nor logs; a request that can change
    // something always has content, even an empty one, so that the handler does not send it again (FilesApiService);
    // FilesRedirectHandler may send it again within the attempt, where a redirect says, which is routing, not a retry.
    //
    // A failure may be temporary when no answer arrived (a connection failure or a timeout), when the answer's status
    // is 408, 429 or 5xx other than 501, or when a storage server answers a transfer's request with its whole error
    // saying that it stopped waiting for the request (StorageTimedOut). Not retried: cancellation, requests that cannot
    // be sent (such as a URL with an unsupported scheme), TLS certificate failures, redirects that were not followed,
    // any other status, and failures of the caller's own streams.
    //
    // Sending a request again is safe only when it did not take effect, or when taking effect twice is harmless. So a
    // Files.com API request that can change something (any method but GET, HEAD and OPTIONS, unless its operation says
    // it is safe to repeat: OperationContext.ReplaySafe) is retried only after a failure that shows it was not carried
    // out (NotCarriedOut): a failure before any of it was sent, or Files.com's answer, trusted, that the site is served
    // from another region, which comes before the request reaches the application. No other answer shows it, whatever
    // its status, error type or body: Files.com gives the same errors, even its rate limit, both before an operation
    // starts and after it has begun, and a proxy can answer after passing the request on. After any other failure the
    // request is not sent again, and FilesApiService says it may have taken effect with
    // SdkException.RequestMayHaveBeenProcessed. Transfers send storage requests again by these rules alone: a part or
    // range is the same bytes to the same place.
    //
    // The wait before a retry depends on where the request goes:
    //
    //   Files.com API   InitialNetworkRequestDelay, doubled for each later retry, up to MaxNetworkRetryDelay.
    //   Storage         after 429 or 503, the whole time its Retry-After asks, however long, or no wait at all
    //                   without one; after any other failure, 100 ms.
    //
    // A wait longer than the SDK can schedule (CanWait), which only a storage server's Retry-After can ask for, is not
    // cut short: that failure is not retried, and is thrown as it is when no retry is left.
    //
    // RunAsync retries one operation's attempts with a Polly retry strategy configured by these rules alone. Upload
    // parts are not sent through it: the transfer engine retries them itself, with these rules, between admissions.
    internal sealed class RetryPolicy
    {
        // The longest wait the SDK schedules, before a retry or as a pause of new requests: what Task.Delay and the
        // timers behind Polly's waits accept on the running platform, uint.MaxValue - 1 milliseconds on .NET 8 and
        // later, and Int32.MaxValue milliseconds on .NET Framework, which the netstandard2.0 build may run on, in whole
        // seconds, as Retry-After counts them, so that a timer's own rounding stays within that range.
#if NET8_0_OR_GREATER
        internal static readonly TimeSpan MaxWait = TimeSpan.FromSeconds((uint.MaxValue - 1L) / 1000);
#else
        internal static readonly TimeSpan MaxWait = TimeSpan.FromSeconds(int.MaxValue / 1000);
#endif

        private static readonly TimeSpan StorageRetryDelay = TimeSpan.FromMilliseconds(100);

        private static readonly SdkLog log = SdkLog.For(typeof(RetryPolicy));

        private readonly NetworkSettings settings;
        private readonly bool storage;

#if DEBUG
        // Debug builds only: the TimeProvider for the retry waits of operations started in the current async flow, for
        // tests that observe those waits at the timer. Null, its value unless a test sets it, leaves Polly's own
        // default, TimeProvider.System. Release builds have no such state and always use that default.
        internal static readonly AsyncLocal<TimeProvider> WaitClock = new AsyncLocal<TimeProvider>();
#endif

        private RetryPolicy(NetworkSettings settings, bool storage)
        {
            this.settings = settings;
            this.storage = storage;
        }

        internal static RetryPolicy ForApi(NetworkSettings settings)
        {
            return new RetryPolicy(settings, storage: false);
        }

        internal static RetryPolicy ForStorage(NetworkSettings settings)
        {
            return new RetryPolicy(settings, storage: true);
        }

        internal int MaxRetries => settings.MaxNetworkRetries;

        internal static bool IsRetryable(int status)
        {
            return status == 408 || status == 429 || (status >= 500 && status != 501);
        }

        // Whether an attempt's failure may be temporary. Attempts report a failure to connect, send or receive as
        // HttpRequestException, a timeout as NetworkTimeoutException and an error response as the exception it is
        // read as, or as an AnsweredFailure when it cannot be; anything else, such as the IOException of a caller's
        // stream, is not retried.
        internal static bool IsRetryable(Exception failure)
        {
            switch (failure)
            {
                case NetworkTimeoutException _:
                case RegionRelocation.Requested _:
                    return true;
                case HttpRequestException transport:
                    return !IsCertificateOrProtocolFailure(transport);
                case InvalidResponseException storage when storage.StorageSaysTimedOut:
                    return true;
                default:
                    int? status = StatusOf(failure);
                    return status.HasValue && IsRetryable(status.Value);
            }
        }

        // Whether a storage server's whole error answer, its status and decoded body, says that it stopped waiting for the
        // rest of the request: the 400 of Amazon S3, and of storage compatible with it, whose XML Error has the Code
        // RequestTimeout, which AWS's own SDKs send again. Only one whole XML document, read without a DTD, can say so;
        // no other status, code or body does.
        internal static bool StorageTimedOut(int status, string body)
        {
            if (status != 400)
            {
                return false;
            }
            try
            {
                using (XmlReader reader = XmlReader.Create(new StringReader(body), new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit }))
                {
                    XElement error = XDocument.Load(reader).Root;
                    return error.Name == "Error" && (string)error.Element("Code") == "RequestTimeout";
                }
            }
            catch (XmlException)
            {
                return false;
            }
        }

        // Whether failure shows that the request was not carried out, so that sending it again cannot do anything twice,
        // and a request that can change something did not take effect: none of it was sent (BeforeSending), or Files.com
        // answered, in a way RegionRelocation trusts, that the site is served from another region, which it does before
        // the request reaches the application, and the operation moves there (Requested) or has no retry left to
        // (RetriesExhausted, which only a trusted move becomes). Any other failure of a request that may have been sent
        // leaves open whether it took effect.
        internal static bool NotCarriedOut(Exception failure)
        {
            return failure is RegionRelocation.Requested
                || (failure is LockoutRegionMismatchException mismatch && mismatch.Refusal == RegionRelocationRefusal.RetriesExhausted)
                || BeforeSending(failure);
        }

        // Whether failure happened before any of the request was sent: the connection could not be made, because the
        // host name did not resolve, the connection was refused or unreachable, the TLS handshake failed, or the
        // handler's ConnectTimeout ran out. Any other failure to connect, send or receive may have followed the whole
        // request reaching the server.
        internal static bool BeforeSending(Exception failure)
        {
            if (failure is NetworkTimeoutException timeout)
            {
                return timeout.BeforeSending;
            }
            if (!(failure is HttpRequestException))
            {
                return false;
            }
#if NET8_0_OR_GREATER
            HttpRequestError error = ((HttpRequestException)failure).HttpRequestError;
            if (error == HttpRequestError.NameResolutionError || error == HttpRequestError.ConnectionError || error == HttpRequestError.SecureConnectionError)
            {
                return true;
            }
#endif
            for (Exception cause = failure.InnerException; cause != null; cause = cause.InnerException)
            {
                if (cause is SocketException socket)
                {
                    switch (socket.SocketErrorCode)
                    {
                        case SocketError.ConnectionRefused:
                        case SocketError.HostNotFound:
                        case SocketError.HostUnreachable:
                        case SocketError.NetworkUnreachable:
                        case SocketError.NoData:
                        case SocketError.TryAgain:
                        case SocketError.AddressNotAvailable:
                            return true;
                    }
                    return false;
                }
                if (cause is WebException web)
                {
                    // .NET Framework's HttpClientHandler.
                    return web.Status == WebExceptionStatus.NameResolutionFailure || web.Status == WebExceptionStatus.ConnectFailure || web.Status == WebExceptionStatus.ProxyNameResolutionFailure;
                }
            }
            return false;
        }

        // The wait before retry number retry, the first being 1, after a failure answered with status, or with no
        // status when none arrived, and with the server's Retry-After, if it sent one.
        internal TimeSpan DelayBefore(int retry, int? status, TimeSpan? retryAfter)
        {
            if (storage)
            {
                if (status == 429 || status == 503)
                {
                    return retryAfter ?? TimeSpan.Zero;
                }
                return StorageRetryDelay;
            }
            if (settings.InitialRetryDelay == TimeSpan.Zero)
            {
                return TimeSpan.Zero;
            }
            double ticks = settings.InitialRetryDelay.Ticks * Math.Pow(2, retry - 1);
            return ticks < settings.MaxRetryDelay.Ticks ? TimeSpan.FromTicks((long)ticks) : settings.MaxRetryDelay;
        }

        // Runs attempt until it succeeds, fails in a way that is not retried, or has been retried MaxRetries times,
        // and returns its result or throws its last failure as it was thrown, the failure of an AnsweredFailure
        // included. mayRetry, when given, can refuse a retry that the rules allow, given the failure and the wait the
        // retry would begin with, which is never longer than CanWait allows: a retry that could not wait so long is
        // refused first. Each retry is logged, with its reason and wait; cancellation ends the wait at once and starts
        // no further attempt.
        internal Task<T> RunAsync<T>(string operation, Func<CancellationToken, Task<T>> attempt, Func<Exception, TimeSpan, bool> mayRetry, CancellationToken cancellationToken)
        {
            if (MaxRetries == 0)
            {
                return Unwrapped(attempt(cancellationToken));
            }
            var builder = new ResiliencePipelineBuilder<T>();
#if DEBUG
            TimeProvider waitClock = WaitClock.Value;
            if (waitClock != null)
            {
                builder.TimeProvider = waitClock;
            }
#endif
            ResiliencePipeline<T> pipeline = builder
                .AddRetry(new RetryStrategyOptions<T>
                {
                    MaxRetryAttempts = MaxRetries,
                    ShouldHandle = arguments =>
                    {
                        // AttemptNumber counts from 0, so a retry is left while it is below MaxRetries.
                        Exception failure = arguments.Outcome.Exception;
                        bool retry = failure != null && IsRetryable(failure) && arguments.AttemptNumber < MaxRetries;
                        if (retry)
                        {
                            TimeSpan delay = DelayAfter(failure, arguments.AttemptNumber);
                            retry = CanWait(operation, delay, failure) && (mayRetry == null || mayRetry(failure, delay));
                        }
                        return new ValueTask<bool>(retry);
                    },
                    DelayGenerator = arguments => new ValueTask<TimeSpan?>(DelayAfter(arguments.Outcome.Exception, arguments.AttemptNumber)),
                    OnRetry = arguments =>
                    {
                        LogRetry(operation, arguments.AttemptNumber + 1, arguments.RetryDelay, arguments.Outcome.Exception);
                        return default;
                    },
                    // Every wait comes from DelayGenerator: no backoff, jitter or cap of Polly's own.
                    BackoffType = DelayBackoffType.Constant,
                    Delay = TimeSpan.Zero,
                    UseJitter = false,
                })
                .Build();
            return Unwrapped(pipeline.ExecuteAsync(token => new ValueTask<T>(attempt(token)), cancellationToken).AsTask());
        }

        private static async Task<T> Unwrapped<T>(Task<T> run)
        {
            try
            {
                return await run;
            }
            catch (AnsweredFailure answered)
            {
                ExceptionDispatchInfo.Capture(answered.InnerException).Throw();
                throw;
            }
            catch (RegionRelocation.Requested move)
            {
                throw move.Mismatch.Refused(RegionRelocationRefusal.RetriesExhausted, move.Origin);
            }
        }

        // The HTTP status of a failure that is an error response, or null.
        internal static int? StatusOf(Exception failure)
        {
            switch (failure)
            {
                case ApiException error:
                    return error.httpStatus;
                case InvalidResponseException unreadable when unreadable.httpStatus > 0:
                    return unreadable.httpStatus;
                case AnsweredFailure answered:
                    return answered.Status;
                default:
                    return null;
            }
        }

        // The wait a response's Retry-After asks for, or null when it has none that can be read: a number of seconds,
        // or an HTTP date, which is a wait of zero once it has passed.
        internal static TimeSpan? RetryAfterOf(HttpHeaders headers)
        {
            IEnumerable<string> values;
            if (headers == null || !headers.TryGetValues("Retry-After", out values))
            {
                return null;
            }
            foreach (string value in values)
            {
                string text = value.Trim();
                if (text.Length > 0 && IsDigits(text))
                {
                    // A number of seconds too large for a TimeSpan still asks for longer than the SDK can wait.
                    return double.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out double seconds) && seconds < TimeSpan.MaxValue.TotalSeconds
                        ? TimeSpan.FromSeconds(seconds)
                        : TimeSpan.MaxValue;
                }
                if (RetryConditionHeaderValue.TryParse(text, out RetryConditionHeaderValue parsed) && parsed.Date.HasValue)
                {
                    TimeSpan untilDate = parsed.Date.Value - DateTimeOffset.UtcNow;
                    return untilDate > TimeSpan.Zero ? untilDate : TimeSpan.Zero;
                }
                return null;
            }
            return null;
        }

        // The wait the Retry-After of a failure that is an error response asks for, or null.
        internal static TimeSpan? RetryAfterOf(Exception failure)
        {
            return RetryAfterOf(HeadersOf(failure));
        }

        // A move to the site's region is retried at once: nothing failed that time could fix.
        private TimeSpan DelayAfter(Exception failure, int attemptNumber)
        {
            return failure is RegionRelocation.Requested ? TimeSpan.Zero : DelayBefore(attemptNumber + 1, StatusOf(failure), RetryAfterOf(HeadersOf(failure)));
        }

        // Logs a retry of operation, number retry of MaxRetries, after failure, which the server answered with status and
        // Retry-After when it answered: why and how long it waits, and whether a storage wait is the server's
        // Retry-After. Nothing a response or URL could carry is logged below DEBUG, nor the failure's message, which a
        // custom handler may have written, even for a NetworkTimeoutException.
        internal void LogRetry(string operation, int retry, TimeSpan delay, int? status, TimeSpan? retryAfter, Exception failure)
        {
            string reason = failure is RegionRelocation.Requested move ? $"Files.com answered that the site is served from {move.Origin}, at that address"
                : status.HasValue ? $"HTTP {status.Value}" : failure is NetworkTimeoutException ? "a timeout" : $"a connection failure ({failure.GetType().Name})";
            log.Info($"Retrying {operation} after {reason}: retry {retry} of {MaxRetries}, in {Seconds(delay)}{RetryAfterNote(status, retryAfter)}");
            log.Debug($"The failure before retry {retry} of {operation}", failure);
        }

        internal void LogRetry(string operation, int retry, TimeSpan delay, Exception failure)
        {
            LogRetry(operation, retry, delay, StatusOf(failure), RetryAfterOf(HeadersOf(failure)), failure);
        }

        // The wait before retry number retry after failure, for an engine that retries attempts itself: from the
        // failure's status and Retry-After, when it is an error response.
        internal TimeSpan DelayAfterFailure(Exception failure, int retry)
        {
            return DelayBefore(retry, StatusOf(failure), RetryAfterOf(HeadersOf(failure)));
        }

        // Whether a retry of operation can wait for delay, the wait it begins with after a failure answered with status
        // and Retry-After: at most MaxWait. Only a storage server's Retry-After asks for longer, and the wait is not cut
        // short: the retry is not made, which is logged, and the caller throws the failure as it does with no retry left.
        internal bool CanWait(string operation, TimeSpan delay, int? status, TimeSpan? retryAfter)
        {
            if (delay <= MaxWait)
            {
                return true;
            }
            string asked = retryAfter.HasValue && retryAfter.Value != TimeSpan.MaxValue ? $"asked for {Seconds(retryAfter.Value)}, longer" : "asked for longer";
            log.Info($"Not retrying {operation} after HTTP {status}: the server's Retry-After {asked} than this platform can wait");
            return false;
        }

        internal bool CanWait(string operation, TimeSpan delay, Exception failure)
        {
            return CanWait(operation, delay, StatusOf(failure), RetryAfterOf(HeadersOf(failure)));
        }

        // The failure to throw once an attempt's failure is not retried: an AnsweredFailure is its own inner failure.
        internal static Exception Unwrap(Exception failure)
        {
            return failure is AnsweredFailure answered ? answered.InnerException : failure;
        }

        // How a storage retry's wait relates to the server's Retry-After.
        private string RetryAfterNote(int? status, TimeSpan? retryAfter)
        {
            if (!storage || (status != 429 && status != 503))
            {
                return "";
            }
            return retryAfter.HasValue ? ", as the server's Retry-After asked" : ", without a Retry-After to wait for";
        }

        private static HttpHeaders HeadersOf(Exception failure)
        {
            switch (failure)
            {
                case ApiException error:
                    return error.httpHeaders;
                case InvalidResponseException unreadable:
                    return unreadable.httpHeaders;
                case AnsweredFailure answered:
                    return answered.Headers;
                default:
                    return null;
            }
        }

        // TLS failures that are not the connection breaking, such as an untrusted certificate: trying again gets the
        // same answer.
        private static bool IsCertificateOrProtocolFailure(Exception failure)
        {
            for (Exception cause = failure; cause != null; cause = cause.InnerException)
            {
                // Not FilesCom.AuthenticationException, which is about the SDK's own credentials.
                if (cause is System.Security.Authentication.AuthenticationException)
                {
                    return true;
                }
            }
            return false;
        }

        // An attempt's failure to read an error response that does not itself carry the response's status, such as a
        // body in a charset that cannot be decoded. The status and headers decide the retry and its wait as they do
        // for any error response, and RunAsync throws the failure itself once no retry follows.
        internal sealed class AnsweredFailure : Exception
        {
            internal AnsweredFailure(HttpResponseMessage response, Exception failure)
                : base($"The HTTP {(int)response.StatusCode} response could not be read ({failure.GetType().Name})", failure)
            {
                Status = (int)response.StatusCode;
                Headers = response.Headers;
            }

            internal int Status { get; }

            internal HttpHeaders Headers { get; }
        }

        private static string Seconds(TimeSpan time)
        {
            return time.TotalSeconds.ToString("0.###", CultureInfo.InvariantCulture) + " seconds";
        }

        private static bool IsDigits(string text)
        {
            foreach (char c in text)
            {
                if (c < '0' || c > '9')
                {
                    return false;
                }
            }
            return true;
        }
    }
}