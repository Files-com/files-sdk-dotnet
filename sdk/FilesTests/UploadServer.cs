using FilesCom;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WireMock;

namespace FilesTests
{
    // One Files.com site that accepts uploads the way the service negotiates them, on a loopback port. begin_upload
    // answers ordinary writes (action put/write) with parts sized by PartSize, each sent to /part/<file>/<number> with a
    // query signed like a storage URL on this site by default, or with Aws, to the object /part/<file> at an Amazon S3
    // general-purpose endpoint, as a SigV4 presigned UploadPart URL for one upload and the part's number, as Files.com
    // gives native uploads. A client made with Client
    // reaches that endpoint through handlers that send its requests to this site, over HTTP, with their path and query
    // as sent (Transport): the S3 host's name is never looked up. Every part request is recorded with its bytes and
    // headers, and can be held, failed or answered with an ETag; the final request is recorded too. The server only
    // answers: which parts arrive, in what order, how many at once and how often comes from the SDK.
    public sealed class UploadServer : IDisposable
    {
        public const string SignedQuery = "X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIA%2F20260924%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Signature=5ig%2bnature%3D&uploadId=up~1";

        // A virtual-hosted Amazon S3 general-purpose endpoint, for Aws.
        public const string AwsHost = "files-test-bucket.s3.us-east-1.amazonaws.com";

        private static readonly TimeSpan Patience = TimeSpan.FromSeconds(10);

        private readonly object gate = new object();
        private readonly List<PartRequest> parts = new List<PartRequest>();
        private readonly List<string> events = new List<string>();
        private readonly List<JsonElement> negotiations = new List<JsonElement>();
        private readonly List<string> negotiationRuns = new List<string>();
        private readonly List<JsonElement> finishes = new List<JsonElement>();
        private readonly List<string> folders = new List<string>();
        private readonly Dictionary<string, int> timesNegotiated = new Dictionary<string, int>();
        private readonly Dictionary<string, int> uploadsBegun = new Dictionary<string, int>();
        private readonly Dictionary<string, string> negotiatedUris = new Dictionary<string, string>();
        private readonly Dictionary<string, KeyValuePair<string, long>> partsByTarget = new Dictionary<string, KeyValuePair<string, long>>();
        private readonly TaskCompletionSource<bool> releaseHeldParts = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        // The part requests being handled, and the most at once: both changed and read only under gate, so that a request
        // answered while another arrives is never counted as still in flight.
        private int partsInFlight;
        private int maxPartsInFlight;

        public UploadServer(string apiKey = "key-a", string workspaceId = "11")
        {
            Site = new LoopbackSite(apiKey, workspaceId);
            Site.On("POST", "/file_actions/begin_upload/*", NegotiateAsync);
            Site.On("PUT", "//part/*", ReceivePartAsync);
            // A path-style S3 URL names the bucket first.
            Site.On("PUT", "//files-test-bucket/part/*", ReceivePartAsync);
            Site.On("POST", "/files/*", FinishAsync);
            Site.On("POST", "/folders/*", CreateFolder);
        }

        public LoopbackSite Site { get; }

        // The partsize the server plans for each part number.
        public Func<long, long> PartSize { get; set; } = number => 8;

        // The partsize for a part number and its negotiation count (1 for the first); by default PartSize, whose value for
        // the next number is the next_partsize.
        public Func<long, int, long> PartSizeFor { get; set; }

        // The parallel_parts and retry_parts values sent, as JSON literals; null leaves the field out.
        public string ParallelParts { get; set; } = "true";

        public string RetryParts { get; set; } = "true";

        // The parallel_parts value for a part number and its negotiation count (1 for the first); by default
        // ParallelParts.
        public Func<long, int, string> ParallelPartsFor { get; set; }

        // The parallel_parts value for a file, part number and negotiation count, when set: before ParallelPartsFor.
        public Func<string, long, int, string> ParallelPartsForFile { get; set; }

        // Where the server says to send a part: by default PartUrl(file, number, negotiation), or AwsPartUrl with Aws.
        public Func<string, long, int, string> UploadUri { get; set; }

        // Whether parts go to the Amazon S3 endpoint (AwsPartUrl) by default.
        public bool Aws { get; set; }

        // The action of every negotiated part.
        public string Action { get; set; } = "put/write";

        // More JSON fields for every negotiated part, such as ,"headers":{...}.
        public string ExtraPartFields { get; set; } = "";

        // More JSON fields for a part number and its negotiation count (1 for the first); by default ExtraPartFields.
        public Func<long, int, string> ExtraPartFieldsFor { get; set; }

        // The expires value for a part number and its negotiation count (1 for the first), or null for none.
        public Func<long, int, string> Expires { get; set; } = (number, negotiation) => null;

        // Answers a part request; the default accepts it with an ETag naming its file, number and attempt.
        public Func<PartRequest, Task<ResponseMessage>> RespondToPart { get; set; } = request => Task.FromResult(Accept(request));

        // Answers the final request; the default completes the file. The body it was given is recorded either way.
        public Func<JsonElement, Task<ResponseMessage>> RespondToFinish { get; set; }

        // Answers a begin_upload request instead of the server, when it returns a response.
        public Func<JsonElement, ResponseMessage> RefuseNegotiation { get; set; }

        // When set, a begin_upload request, or a final request, for the file it names waits for the task it returns before
        // being answered.
        public Func<string, Task> HoldNegotiation { get; set; }

        public Func<string, Task> HoldFinish { get; set; }

        // When set, part requests wait for ReleaseHeldParts before being answered.
        public bool HoldParts { get; set; }

        public int PartsInFlight
        {
            get
            {
                lock (gate)
                {
                    return partsInFlight;
                }
            }
        }

        public int MaxPartsInFlight
        {
            get
            {
                lock (gate)
                {
                    return maxPartsInFlight;
                }
            }
        }

        // The time now, on the monotonic Stopwatch clock that PartRequest.ArrivedAt and AnsweredAt use.
        public static TimeSpan Clock => TimeSpan.FromSeconds(Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency);

        public IReadOnlyList<PartRequest> Parts
        {
            get
            {
                lock (gate)
                {
                    return parts.ToList();
                }
            }
        }

        // "begin <file> part=<n> parts=<n>", "put <file>/<n>" (on arrival) and "end <file>", in order.
        public IReadOnlyList<string> Events
        {
            get
            {
                lock (gate)
                {
                    return events.ToList();
                }
            }
        }

        public IReadOnlyList<JsonElement> Negotiations
        {
            get
            {
                lock (gate)
                {
                    return negotiations.ToList();
                }
            }
        }

        // The negotiations a client made with Client(run:) asked for, in the order they arrived: what one run of an upload
        // asked, and nothing an earlier run, cancelled, still had on its way, which the site may record after it stopped.
        public IReadOnlyList<JsonElement> NegotiationsOf(string run)
        {
            lock (gate)
            {
                return negotiations.Where((negotiation, index) => negotiationRuns[index] == run).ToList();
            }
        }

        public IReadOnlyList<JsonElement> Finishes
        {
            get
            {
                lock (gate)
                {
                    return finishes.ToList();
                }
            }
        }

        // The folders created with POST /folders, in order.
        public IReadOnlyList<string> Folders
        {
            get
            {
                lock (gate)
                {
                    return folders.ToList();
                }
            }
        }

        // The most part requests the server was handling at once at any time from `from` to `to`.
        public int MaxPartsInFlightBetween(TimeSpan from, TimeSpan to)
        {
            IReadOnlyList<PartRequest> all = Parts;
            IEnumerable<TimeSpan> moments = new[] { from }.Concat(all.Select(part => part.ArrivedAt).Where(arrived => arrived >= from && arrived <= to));
            return moments.Max(moment => all.Count(part => part.ArrivedAt <= moment && (part.AnsweredAt == null || part.AnsweredAt > moment)));
        }

        // Where part number of file is sent; negotiation counts the times the part was negotiated, from 1.
        public string PartUrl(string file, long number, int negotiation)
        {
            return $"{Site.Url}/part/{Uri.EscapeDataString(file)}/{number}?{SignedQuery}&negotiation={negotiation}";
        }

        // Where part number of file is sent at the Amazon S3 endpoint: a presigned UploadPart URL, as Files.com signs
        // one, with only its host signed, valid for 15 minutes from now, for upload ID up~1 of the object /part/<file>,
        // plus extra after negotiation.
        public static string AwsPartUrl(string file, long number, int negotiation, string uploadId = "up~1", string signedHeaders = "host", string host = AwsHost, string extra = "")
        {
            string now = DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'", System.Globalization.CultureInfo.InvariantCulture);
            return $"https://{host}/part/{Uri.EscapeDataString(file)}?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIA%2F20261001%2Fus-east-1%2Fs3%2Faws4_request"
                + $"&X-Amz-Date={now}&X-Amz-Expires=900&X-Amz-SignedHeaders={Uri.EscapeDataString(signedHeaders)}&X-Amz-Signature=5ig%2bnature%3D&partNumber={number}&uploadId={uploadId}&negotiation={negotiation}{extra}";
        }

        // Handlers that send requests for any host that is not on this machine to this site, over HTTP, with their path and
        // query as the SDK sent them, such as those to the Amazon S3 endpoint or to a custom one a test names. Requests to
        // other loopback servers, such as redirect targets, go to them.
        public HttpTransport Transport => TransportFor(null);

        // A client of this site, through Transport; one made with run marks each request it sends Files.com with it
        // (ToSite.RunHeader), so that every negotiation is known by the run that asked for it (NegotiationsOf).
        public FilesClient Client(int maxNetworkRetries = 0, string run = null)
        {
            return FilesClient.Create(Site.Configuration(maxNetworkRetries), TransportFor(run));
        }

        public FilesClient Client(FilesConfiguration configuration)
        {
            return FilesClient.Create(configuration, Transport);
        }

        private HttpTransport TransportFor(string run)
        {
            return HttpTransport.FromHandlerFactory(() => new ToSite(new Uri(Site.Url), run) { InnerHandler = new SocketsHttpHandler { UseProxy = false } });
        }

        // The upload_uri the server gave part number of file the negotiation-th time it was negotiated.
        public string NegotiatedUri(string file, long number, int negotiation)
        {
            lock (gate)
            {
                return negotiatedUris[$"{file}/{number}/{negotiation}"];
            }
        }

        public void ReleaseHeldParts()
        {
            releaseHeldParts.TrySetResult(true);
        }

        public static ResponseMessage Accept(PartRequest request)
        {
            ResponseMessage response = LoopbackSite.Status(200);
            response.AddHeader("ETag", $"\"etag-{request.File}-{request.Number}-{request.Attempt}\"");
            return response;
        }

        public static ResponseMessage AcceptWithoutETag()
        {
            return LoopbackSite.Status(200);
        }

        // The bytes of every accepted part of file, placed at the offsets their part numbers imply.
        public byte[] AcceptedBytes(string file)
        {
            return Parts.Where(part => part.File == file && part.Accepted).OrderBy(part => part.Number).SelectMany(part => part.Bytes).ToArray();
        }

        // state, if given, describes more of the upload's stage than the site's events when the wait times out.
        public async Task WaitUntil(Func<UploadServer, bool> condition, string description, Func<string> state = null)
        {
            Stopwatch waited = Stopwatch.StartNew();
            while (!condition(this))
            {
                if (waited.Elapsed > Patience)
                {
                    Assert.Fail($"Timed out waiting until {description}. " + (state != null ? state() : $"Events: {string.Join(", ", Events)}"));
                }
                await Task.Delay(10);
            }
        }

        public void Dispose()
        {
            ReleaseHeldParts();
            Site.Dispose();
        }

        // The remote path a request names after prefix, such as "/files/", whether or not its slashes were escaped.
        private static string PathAfter(ReceivedRequest request, string prefix)
        {
            return Uri.UnescapeDataString(request.Path.Split('?')[0].Substring(prefix.Length));
        }

        private ResponseMessage CreateFolder(ReceivedRequest request)
        {
            string folder = PathAfter(request, "/folders/");
            lock (gate)
            {
                folders.Add(folder);
                events.Add($"mkdir {folder}");
            }
            return LoopbackSite.Json($"{{\"path\":{JsonSerializer.Serialize(folder)},\"type\":\"directory\"}}");
        }

        private async Task<ResponseMessage> NegotiateAsync(ReceivedRequest request)
        {
            if (HoldNegotiation != null)
            {
                await HoldNegotiation(PathAfter(request, "/file_actions/begin_upload/"));
            }
            return Negotiate(request);
        }

        private ResponseMessage Negotiate(ReceivedRequest request)
        {
            string file = PathAfter(request, "/file_actions/begin_upload/");
            JsonElement body = JsonDocument.Parse(request.Body).RootElement.Clone();
            long first = body.TryGetProperty("part", out JsonElement part) ? part.GetInt64() : 1;
            long count = body.TryGetProperty("parts", out JsonElement partsWanted) ? partsWanted.GetInt64() : 1;
            var answer = new List<string>();
            lock (gate)
            {
                negotiations.Add(body);
                negotiationRuns.Add(request.Headers.TryGetValue(ToSite.RunHeader, out string run) ? run : null);
                events.Add($"begin {file} part={first} parts={count}");
                ResponseMessage refusal = RefuseNegotiation?.Invoke(body);
                if (refusal != null)
                {
                    return refusal;
                }
                // A request without a ref begins a new upload: the first of each file is ref-<file>, later ones
                // ref-<file>-<n>.
                string reference = body.TryGetProperty("ref", out JsonElement given) ? given.GetString() : null;
                if (reference == null)
                {
                    uploadsBegun.TryGetValue(file, out int begun);
                    uploadsBegun[file] = ++begun;
                    reference = begun == 1 ? $"ref-{file}" : $"ref-{file}-{begun}";
                }
                for (long number = first; number < first + count; number++)
                {
                    string key = file + "/" + number;
                    timesNegotiated.TryGetValue(key, out int negotiation);
                    timesNegotiated[key] = ++negotiation;
                    string expires = Expires(number, negotiation);
                    string uri = UploadUri != null ? UploadUri(file, number, negotiation) : Aws ? AwsPartUrl(file, number, negotiation) : PartUrl(file, number, negotiation);
                    negotiatedUris[$"{file}/{number}/{negotiation}"] = uri;
                    partsByTarget[Target(PathAndQuery(uri))] = new KeyValuePair<string, long>(file, number);
                    answer.Add("{" +
                        $"\"upload_uri\":\"{uri}\",\"http_method\":\"PUT\",\"ref\":\"{reference}\",\"part_number\":{number}," +
                        (Action != null ? $"\"action\":\"{Action}\"," : "") +
                        $"\"partsize\":{(PartSizeFor != null ? PartSizeFor(number, negotiation) : PartSize(number))},\"next_partsize\":{(PartSizeFor != null ? PartSizeFor(number + 1, negotiation) : PartSize(number + 1))}," +
                        $"\"send\":{{\"partsize\":\"required-header Content-Length\",\"partdata\":\"body\"}}" +
                        (ParallelPart(file, number, negotiation) != null ? $",\"parallel_parts\":{ParallelPart(file, number, negotiation)}" : "") +
                        (RetryParts != null ? $",\"retry_parts\":{RetryParts}" : "") +
                        (expires != null ? $",\"expires\":\"{expires}\"" : "") +
                        (ExtraPartFieldsFor != null ? ExtraPartFieldsFor(number, negotiation) : ExtraPartFields) + "}");
                }
            }
            return LoopbackSite.Json("[" + string.Join(",", answer) + "]");
        }

        // The path and query of an absolute URL, as its request names them.
        private static string PathAndQuery(string uri)
        {
            return uri.Substring(uri.IndexOf('/', uri.IndexOf("//", StringComparison.Ordinal) + 2));
        }

        // A request's path, unescaped as the site receives it, and its query as sent: an S3 URL's escaped object name
        // may name a file in a folder.
        private static string Target(string pathAndQuery)
        {
            int query = pathAndQuery.IndexOf('?');
            return query < 0 ? Uri.UnescapeDataString(pathAndQuery) : Uri.UnescapeDataString(pathAndQuery.Substring(0, query)) + pathAndQuery.Substring(query);
        }

        private string ParallelPart(string file, long number, int negotiation)
        {
            return ParallelPartsForFile != null ? ParallelPartsForFile(file, number, negotiation) : ParallelPartsFor != null ? ParallelPartsFor(number, negotiation) : ParallelParts;
        }

        private async Task<ResponseMessage> ReceivePartAsync(ReceivedRequest received)
        {
            string path = received.Path.Split('?')[0];
            string target = path.Substring(path.IndexOf("/part/", StringComparison.Ordinal) + "/part/".Length);
            int slash = target.LastIndexOf('/');
            PartRequest request;
            lock (gate)
            {
                // An S3 URL names only the object, so its part is the one whose negotiated URL this is.
                KeyValuePair<string, long> part = slash >= 0 && long.TryParse(target.Substring(slash + 1), out long number)
                    ? new KeyValuePair<string, long>(Uri.UnescapeDataString(target.Substring(0, slash)), number)
                    : partsByTarget[Target(received.Path)];
                request = new PartRequest(received, part.Key, part.Value);
                request.Attempt = parts.Count(part => part.File == request.File && part.Number == request.Number) + 1;
                parts.Add(request);
                events.Add($"put {request.File}/{request.Number}");
                partsInFlight++;
                maxPartsInFlight = Math.Max(maxPartsInFlight, partsInFlight);
            }
            try
            {
                if (HoldParts)
                {
                    await releaseHeldParts.Task;
                }
                ResponseMessage response = await RespondToPart(request);
                request.Accepted = response.StatusCode is int status && status < 300;
                return response;
            }
            finally
            {
                request.AnsweredAt = Clock;
                lock (gate)
                {
                    partsInFlight--;
                }
            }
        }

        private async Task<ResponseMessage> FinishAsync(ReceivedRequest request)
        {
            string file = PathAfter(request, "/files/");
            JsonElement body = JsonDocument.Parse(request.Body).RootElement.Clone();
            lock (gate)
            {
                finishes.Add(body);
                events.Add($"end {file}");
            }
            if (HoldFinish != null)
            {
                await HoldFinish(file);
            }
            if (RespondToFinish != null)
            {
                return await RespondToFinish(body);
            }
            return LoopbackSite.Json($"{{\"path\":{JsonSerializer.Serialize(file)},\"type\":\"file\",\"size\":{body.GetProperty("size").GetInt64()}}}");
        }
    }

    // Sends requests for any host that is not a loopback address to the site instead, over HTTP, keeping their path and
    // query exactly.
    internal sealed class ToSite : DelegatingHandler
    {
        // What a client of one run marks its requests to Files.com with; storage requests stay as the SDK sends them.
        internal const string RunHeader = "X-Test-Run";

        private readonly Uri site;
        private readonly string run;

        internal ToSite(Uri site, string run = null)
        {
            this.site = site;
            this.run = run;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.RequestUri.IsLoopback)
            {
                request.RequestUri = new Uri(site, request.RequestUri.PathAndQuery);
            }
            if (run != null && request.RequestUri.AbsolutePath.StartsWith("/api/rest/v1/", StringComparison.Ordinal))
            {
                request.Headers.TryAddWithoutValidation(RunHeader, run);
            }
            return base.SendAsync(request, cancellationToken);
        }
    }

    public sealed class PartRequest
    {
        private readonly List<KeyValuePair<string, TimeSpan>> marks = new List<KeyValuePair<string, TimeSpan>>();

        internal PartRequest(ReceivedRequest request, string file, long number)
        {
            Request = request;
            File = file;
            Number = number;
        }

        public ReceivedRequest Request { get; }

        public string File { get; }

        public long Number { get; }

        public int Attempt { get; internal set; }

        public byte[] Bytes => Request.BodyBytes;

        public bool Accepted { get; internal set; }

        // When the request arrived, on the monotonic Stopwatch clock.
        public TimeSpan ArrivedAt { get; } = UploadServer.Clock;

        // When it was answered, or null while it is not.
        public TimeSpan? AnsweredAt { get; internal set; }

        // The steps of answering it that a test's handler marked, each with when it was reached on the Stopwatch clock, in
        // order: what the destination actually did, for the evidence of tests whose outcome rests on its timing.
        public IReadOnlyList<KeyValuePair<string, TimeSpan>> Marks
        {
            get
            {
                lock (marks)
                {
                    return marks.ToList();
                }
            }
        }

        // Marks that the handler answering this request has reached step now.
        public void Mark(string step)
        {
            lock (marks)
            {
                marks.Add(new KeyValuePair<string, TimeSpan>(step, UploadServer.Clock));
            }
        }
    }

    // The time a modeled destination takes to answer, kept on the Stopwatch clock instead of by a timer. Timers can end
    // far later than asked on some hosts: on the Windows ARM64 guest, Task.Delay(60) took 51 to 135 ms and its lateness
    // changed from one moment to the next, while a Stopwatch spin ended within 0.2 ms. A destination modeled with timers
    // there changes speed between the windows an adaptive upload measures, which is not the destination a test means.
    // One dedicated thread, not one of the thread pool's, completes each wait at its time: it spins, yielding, only while
    // a wait is pending, and otherwise sleeps until one is asked for.
    public static class DestinationClock
    {
        private static readonly object gate = new object();
        private static readonly List<KeyValuePair<TimeSpan, TaskCompletionSource<bool>>> pending = new List<KeyValuePair<TimeSpan, TaskCompletionSource<bool>>>();
        private static Thread thread;

        // Ends duration after now, on the Stopwatch clock.
        public static Task Wait(TimeSpan duration)
        {
            var done = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            lock (gate)
            {
                pending.Add(new KeyValuePair<TimeSpan, TaskCompletionSource<bool>>(UploadServer.Clock + duration, done));
                if (thread == null)
                {
                    thread = new Thread(Run) { IsBackground = true, Name = "Destination clock" };
                    thread.Start();
                }
                Monitor.Pulse(gate);
            }
            return done.Task;
        }

        private static void Run()
        {
            while (true)
            {
                var due = new List<TaskCompletionSource<bool>>();
                lock (gate)
                {
                    while (pending.Count == 0)
                    {
                        Monitor.Wait(gate);
                    }
                    TimeSpan now = UploadServer.Clock;
                    for (int i = pending.Count - 1; i >= 0; i--)
                    {
                        if (pending[i].Key <= now)
                        {
                            due.Add(pending[i].Value);
                            pending.RemoveAt(i);
                        }
                    }
                }
                foreach (TaskCompletionSource<bool> done in due)
                {
                    done.TrySetResult(true);
                }
                Thread.Yield();
            }
        }
    }

    // A forward-only stream whose first read waits for ReleaseFirstRead, ignoring its token, like a stream that cannot
    // be interrupted, and then returns one byte. Later reads return what is asked for at once.
    public sealed class StalledFirstReadStream : System.IO.Stream
    {
        private readonly TaskCompletionSource<bool> firstReadStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource<bool> releaseFirstRead = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        private int reads;
        private bool disposed;

        public Task FirstReadStarted => firstReadStarted.Task;

        public int Reads => Volatile.Read(ref reads);

        public void ReleaseFirstRead()
        {
            releaseFirstRead.TrySetResult(true);
        }

        public override bool CanRead => !disposed;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (Interlocked.Increment(ref reads) == 1)
            {
                firstReadStarted.TrySetResult(true);
                await releaseFirstRead.Task;
                return 1;
            }
            return count;
        }

        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override void Flush()
        {
        }
        public override long Seek(long offset, System.IO.SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            disposed = true;
            base.Dispose(disposing);
        }
    }
}