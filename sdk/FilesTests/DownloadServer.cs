using FilesCom;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FilesTests
{
    // A Files.com site and the Amazon S3 storage it gives download URLs for, on one loopback port, for downloads
    // (client.Transfers). The site holds remote.bin, and any files and folders added. It answers GET /files/<path> as
    // Files.com's ordinary Download does, with the file's storage URL and nothing that describes a version, and lists
    // folders (GET /folders/<path>), ListPageSize entries a page. Storage URLs are HTTPS URLs of an S3 host (StorageHost),
    // or of whatever origin StorageOrigin names, which a client made with Client reaches through handlers that send every
    // request for a host that is not on this computer to this server, over HTTP, with its path and query as sent
    // (Transport), so the name is never looked up; with Bound false they name the loopback address itself, as storage
    // other than S3 does. The storage server serves each file's object as S3 does: its current bytes, whole or as the Range asked for,
    // with Content-Range and the object's entity tag, which every new version changes; 416 for a range past its end; 412
    // to a request whose If-Match is not the object's entity tag; and 404 for an object no longer there. Change replaces
    // a file's bytes in place, at the same object, as an overwrite does; Relocate moves the file to a new object, as a
    // new upload does. Intercept can answer any storage request itself instead, to send what a real server should not.
    public sealed class DownloadServer : IDisposable
    {
        public const string DefaultFile = "remote.bin";

        // A host name of the form Amazon S3's virtual-hosted, regional endpoints have.
        public const string StorageHost = "files-test-bucket.s3.us-east-1.amazonaws.com";

        private readonly ScriptedServer server;
        private readonly List<string> apiRequests = new List<string>();
        private readonly List<StorageRequest> storageRequests = new List<StorageRequest>();
        private readonly List<string> storageSends = new List<string>();
        private readonly Dictionary<string, ServedFile> files = new Dictionary<string, ServedFile>(StringComparer.Ordinal);
        private readonly HashSet<string> folders = new HashSet<string>(StringComparer.Ordinal);

        public DownloadServer(byte[] contents)
        {
            files[DefaultFile] = new ServedFile(DefaultFile, contents);
            server = new ScriptedServer(ServeAsync);
        }

        public int Port => server.Port;

        public string Url => $"http://127.0.0.1:{Port}";

        // Where storage URLs point: StorageOrigin, or with Bound false this server's own address.
        public string StorageUrl => Bound ? StorageOrigin : Url;

        // The scheme, host and port of storage URLs while Bound: by default HTTPS to the S3 host, on its default port.
        public string StorageOrigin { get; set; } = $"https://{StorageHost}";

        // This server's storage at the S3 host and this server's own port, which any DownloadServer's client reaches: for
        // a URL that must lead here from another server's.
        public string StorageUrlAtPort => $"https://{StorageHost}:{Port}";

        // What a client sends for remote.bin's ordinary Download: nothing but the file's path.
        public const string DefaultDownload = "/api/rest/v1/files/remote.bin?path=remote.bin";

        // The current version's bytes of remote.bin, and its object's entity tag. Change replaces them with a new version.
        public byte[] Contents => files[DefaultFile].Contents;

        public string ETag => files[DefaultFile].Current.ETag;

        // The number of entries a folder listing returns a page; null for all at once.
        public int? ListPageSize { get; set; }

        // Whether storage URLs name Amazon S3, which the SDK binds downloads to; false for storage it cannot bind.
        public bool Bound { get; set; } = true;

        // Whether the storage server ignores If-Match, as storage behind a server that drops the condition would, and
        // serves the object's current bytes whatever it says.
        public bool IgnoresIfMatch { get; set; }

        // Whether the storage server ignores Range and always sends the whole file.
        public bool IgnoresRange { get; set; }

        // How long each storage URL lasts, in seconds; null for a URL whose expiry cannot be told.
        public int? UrlLifetime { get; set; }

        // More of a signed query, appended as it is to every storage URL's, after its stand-in signature.
        public string MoreQuery { get; set; }

        // Answers a storage request itself, returning true, or leaves it to the server by returning false.
        public Func<StorageRequest, ScriptedConnection, Task<bool>> Intercept { get; set; }

        // Called with the target of each API request, a listing or a download's details, before it is answered, so that a
        // test can change local files while the client waits for the answer.
        public Action<string> OnApiRequest { get; set; }

        // Answers an API request itself, or fails it, returning true, or leaves it to the server by returning false; called
        // with its target after OnApiRequest.
        public Func<string, ScriptedConnection, Task<bool>> ApiIntercept { get; set; }

        public IReadOnlyList<string> ApiRequests
        {
            get
            {
                lock (apiRequests)
                {
                    return apiRequests.ToList();
                }
            }
        }

        public IReadOnlyList<StorageRequest> StorageRequests
        {
            get
            {
                lock (storageRequests)
                {
                    return storageRequests.ToList();
                }
            }
        }

        // Each storage request a client of Transport sent, as its method and range, in the order the client's own handlers
        // passed them on, before any of it reached the server: what a client sends after a moment shows here as sent after
        // it, however late the server comes to read it (StorageRequests).
        public IReadOnlyList<string> StorageSends
        {
            get
            {
                lock (storageSends)
                {
                    return storageSends.ToList();
                }
            }
        }

        public FilesConfiguration Configuration(int maxNetworkRetries = 0)
        {
            return new FilesConfiguration { BaseUrl = Url, ApiKey = "download-key", MaxNetworkRetries = maxNetworkRetries, InitialNetworkRequestDelay = 0, MaxNetworkRetryDelay = 0 };
        }

        // Handlers that send a request for any host not on this computer to this computer's loopback address, over HTTP,
        // with its path and query as sent: at the port its URL names, or at this server's when it names its scheme's
        // default, so that storage URLs naming an S3 host reach this server, or another DownloadServer at another port,
        // without the name being looked up, and no request leaves this computer. Each request still names its own URL
        // above these handlers, where the SDK follows redirects.
        public HttpTransport Transport => HttpTransport.FromHandlerFactory(() => new ToLoopback(Port, Sending) { InnerHandler = new SocketsHttpHandler { UseProxy = false } });

        // A client of this site, through Transport.
        public FilesClient Client(int maxNetworkRetries = 0)
        {
            return FilesClient.Create(Configuration(maxNetworkRetries), Transport);
        }

        public FilesClient Client(FilesConfiguration configuration)
        {
            return FilesClient.Create(configuration, Transport);
        }

        // Makes remote.bin a new version with contents.
        public void Change(byte[] contents)
        {
            ChangeFile(DefaultFile, contents);
        }

        // Adds the file at path, whose folders exist from then on.
        public void AddFile(string path, byte[] contents)
        {
            lock (files)
            {
                files[path] = new ServedFile(path, contents);
            }
        }

        // Makes the file at path a new version with contents, at the same object, modified later unless keepModified, as
        // a storage that keeps modification times across versions does.
        public void ChangeFile(string path, byte[] contents, bool keepModified = false)
        {
            lock (files)
            {
                files[path].Change(contents, keepModified);
            }
        }

        // Moves the file at path to a new object, with contents, or its own bytes when contents is null; its old object
        // is gone. With keepETag the new object answers with the entity tag the old one had, as another object can.
        public void Relocate(string path = DefaultFile, byte[] contents = null, bool keepETag = false)
        {
            lock (files)
            {
                files[path].Relocate(contents, keepETag);
            }
        }

        public void AddFolder(string path)
        {
            lock (files)
            {
                folders.Add(path);
            }
        }

        public byte[] ContentsOf(string path)
        {
            lock (files)
            {
                return files[path].Contents;
            }
        }

        private void Sending(HttpRequestMessage request)
        {
            if (!request.RequestUri.AbsolutePath.StartsWith("/api/rest/v1/", StringComparison.Ordinal))
            {
                lock (storageSends)
                {
                    storageSends.Add($"{request.Method} {request.Headers.Range}");
                }
            }
        }

        // Completes once the server has read and answered every request a client has sent it (ScriptedServer.SettledAsync).
        public Task SettledAsync()
        {
            return server.SettledAsync();
        }

        public void Dispose()
        {
            server.Dispose();
        }

        private async Task ServeAsync(ScriptedConnection connection)
        {
            while (true)
            {
                string line;
                try
                {
                    line = await connection.ReadRequestHeadAsync();
                }
                catch (System.IO.EndOfStreamException)
                {
                    return;
                }
                string target = line.Split(' ')[1];
                if (target.StartsWith("/api/rest/v1/", StringComparison.Ordinal))
                {
                    lock (apiRequests)
                    {
                        apiRequests.Add(target);
                    }
                    OnApiRequest?.Invoke(target);
                    if (ApiIntercept != null && await ApiIntercept(target, connection))
                    {
                        continue;
                    }
                    if (target.StartsWith("/api/rest/v1/folders/", StringComparison.Ordinal))
                    {
                        await AnswerListingAsync(connection, target);
                    }
                    else
                    {
                        await AnswerDownloadAsync(connection, target);
                    }
                    continue;
                }
                var request = new StorageRequest(line, connection.HeaderLines);
                lock (storageRequests)
                {
                    storageRequests.Add(request);
                }
                if (Intercept != null && await Intercept(request, connection))
                {
                    continue;
                }
                await AnswerStorageAsync(connection, request);
            }
        }

        private async Task AnswerDownloadAsync(ScriptedConnection connection, string target)
        {
            string path = Uri.UnescapeDataString(target.Split('?')[0].Substring("/api/rest/v1/files/".Length));
            ServedFile file;
            lock (files)
            {
                files.TryGetValue(path, out file);
            }
            if (file == null)
            {
                await connection.RespondAsync(404, "{\"error\":\"Not Found\",\"http-code\":404,\"type\":\"not-found\"}");
                return;
            }
            (byte[] contents, string _, int @object) = file.Current;
            string uri = $"{StorageUrl}/storage/{@object}/{Uri.EscapeDataString(path)}?X-Files-Signature=stand-in";
            if (UrlLifetime.HasValue)
            {
                uri += $"&X-Files-Date={DateTime.UtcNow:yyyyMMdd'T'HHmmss'Z'}&X-Files-Expires={UrlLifetime.Value}";
            }
            uri += MoreQuery;
            string json = $"{{\"path\":{Json(path)},\"type\":\"file\",\"size\":{contents.Length},\"download_uri\":{Json(uri)}}}";
            await connection.RespondAsync(200, json);
        }

        // The entries directly inside a folder, in name order, a page at a time: the cursor is the next entry's index.
        private async Task AnswerListingAsync(ScriptedConnection connection, string target)
        {
            Dictionary<string, string> query = Query(target);
            string folder = Uri.UnescapeDataString(target.Split('?')[0].Substring("/api/rest/v1/folders/".Length)).Trim('/');
            var entries = new SortedDictionary<string, string>(StringComparer.Ordinal);
            lock (files)
            {
                string prefix = folder.Length == 0 ? "" : folder + "/";
                foreach (string path in files.Keys.Concat(folders).Where(path => path.StartsWith(prefix, StringComparison.Ordinal) && path.Length > prefix.Length))
                {
                    string rest = path.Substring(prefix.Length);
                    int slash = rest.IndexOf('/');
                    string child = prefix + (slash < 0 ? rest : rest.Substring(0, slash));
                    bool file = slash < 0 && files.ContainsKey(path);
                    entries[child] = file
                        ? $"{{\"path\":{Json(child)},\"type\":\"file\",\"size\":{files[child].Contents.Length},\"mtime\":{Json(files[child].Modified.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"))}}}"
                        : $"{{\"path\":{Json(child)},\"type\":\"directory\"}}";
                }
            }
            int start = query.TryGetValue("cursor", out string cursor) ? int.Parse(cursor, CultureInfo.InvariantCulture) : 0;
            int count = Math.Min(ListPageSize ?? int.MaxValue, entries.Count - start);
            string next = start + count < entries.Count ? $"X-Files-Cursor: {start + count}\r\n" : "";
            await connection.RespondAsync(200, "[" + string.Join(",", entries.Values.Skip(start).Take(count)) + "]", next);
        }

        // As S3 answers a GET of /storage/<object>/<path>: the object's current bytes, unless the object is gone (404) or
        // If-Match names another entity tag (412).
        private async Task AnswerStorageAsync(ScriptedConnection connection, StorageRequest request)
        {
            string[] segments = request.Line.Split(' ')[1].Split('?')[0].Split('/');
            ServedFile file = null;
            if (segments.Length == 4 && int.TryParse(segments[2], NumberStyles.None, CultureInfo.InvariantCulture, out int @object))
            {
                lock (files)
                {
                    files.TryGetValue(Uri.UnescapeDataString(segments[3]), out file);
                }
                if (file != null && file.Current.Object != @object)
                {
                    file = null;
                }
            }
            if (file == null)
            {
                await connection.RespondAsync(404, "<Error><Code>NoSuchKey</Code></Error>");
                return;
            }
            (byte[] contents, string etag, int _) = file.Current;
            if (request.IfMatch != null && request.IfMatch != etag && !IgnoresIfMatch)
            {
                await connection.RespondAsync(412, "<Error><Code>PreconditionFailed</Code></Error>");
                return;
            }
            if (request.Range == null || IgnoresRange)
            {
                await SendAsync(connection, 200, contents, 0, contents.Length, $"ETag: {etag}\r\n");
                return;
            }
            (long first, long last) = request.Range.Value;
            if (first >= contents.Length)
            {
                await connection.RespondAsync(416, "", $"Content-Range: bytes */{contents.Length}\r\n");
                return;
            }
            last = Math.Min(last, contents.Length - 1);
            await SendAsync(connection, 206, contents, first, last - first + 1, $"Content-Range: bytes {first}-{last}/{contents.Length}\r\nETag: {etag}\r\n");
        }

        public static async Task SendAsync(ScriptedConnection connection, int status, byte[] contents, long offset, long length, string headers)
        {
            await connection.SendAsync($"HTTP/1.1 {status} OK\r\nContent-Length: {length}\r\nContent-Type: application/octet-stream\r\n{headers}\r\n");
            byte[] body = new byte[length];
            Array.Copy(contents, offset, body, 0, length);
            await connection.SendAsync(body);
        }

        private static Dictionary<string, string> Query(string target)
        {
            var query = new Dictionary<string, string>();
            int start = target.IndexOf('?');
            if (start < 0)
            {
                return query;
            }
            foreach (string pair in target.Substring(start + 1).Split('&'))
            {
                int equals = pair.IndexOf('=');
                if (equals > 0)
                {
                    query[Uri.UnescapeDataString(pair.Substring(0, equals))] = Uri.UnescapeDataString(pair.Substring(equals + 1).Replace('+', ' '));
                }
            }
            return query;
        }

        private static string Json(string text)
        {
            return System.Text.Json.JsonSerializer.Serialize(text);
        }

        // One file the site holds: the object it is stored as, numbered from 1, and its versions, numbered from 1, whose
        // entity tags differ. remote.bin keeps the entity tags it always had.
        private sealed class ServedFile
        {
            private readonly string key;
            private int version = 1;
            private int @object = 1;
            private int tagged = 1;

            internal ServedFile(string path, byte[] contents)
            {
                key = path == DefaultFile ? "" : Uri.EscapeDataString(path) + ".";
                Contents = contents;
            }

            internal byte[] Contents { get; private set; }

            private int modifiedAt = 1;

            internal DateTime Modified => new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc).AddMinutes(modifiedAt);

            internal (byte[] Contents, string ETag, int Object) Current
            {
                get
                {
                    lock (this)
                    {
                        return (Contents, $"\"etag-{key}{tagged}\"", @object);
                    }
                }
            }

            internal void Change(byte[] contents, bool keepModified)
            {
                lock (this)
                {
                    Contents = contents;
                    version++;
                    tagged = version;
                    if (!keepModified)
                    {
                        modifiedAt = version;
                    }
                }
            }

            internal void Relocate(byte[] contents, bool keepETag)
            {
                lock (this)
                {
                    @object++;
                    Contents = contents ?? Contents;
                    if (!keepETag)
                    {
                        version++;
                        tagged = version;
                    }
                }
            }
        }
    }

    // Sends a request for a host that is not on this computer to the loopback address instead, over HTTP, keeping its path
    // and query exactly, then gives the request its own URL back. Each request is shown to sending, when given, as it is
    // passed on, with its own URL.
    internal sealed class ToLoopback : DelegatingHandler
    {
        private readonly int port;
        private readonly Action<HttpRequestMessage> sending;

        internal ToLoopback(int port, Action<HttpRequestMessage> sending = null)
        {
            this.port = port;
            this.sending = sending;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            sending?.Invoke(request);
            Uri asked = request.RequestUri;
            if (asked.IsLoopback)
            {
                return await base.SendAsync(request, cancellationToken);
            }
            request.RequestUri = new Uri($"http://127.0.0.1:{(asked.IsDefaultPort ? port : asked.Port)}{asked.PathAndQuery}");
            try
            {
                return await base.SendAsync(request, cancellationToken);
            }
            finally
            {
                request.RequestUri = asked;
            }
        }
    }

    // One request to the storage server.
    public sealed class StorageRequest
    {
        internal StorageRequest(string line, string[] headers)
        {
            Line = line;
            Headers = headers;
            string range = Header("Range");
            if (range != null && range.StartsWith("bytes=", StringComparison.Ordinal) && range.IndexOf(',') < 0)
            {
                string[] bounds = range.Substring(6).Split('-');
                Range = (long.Parse(bounds[0], CultureInfo.InvariantCulture), long.Parse(bounds[1], CultureInfo.InvariantCulture));
            }
            IfMatch = Header("If-Match");
        }

        public string Line { get; }

        public string[] Headers { get; }

        public (long First, long Last)? Range { get; }

        public string IfMatch { get; }

        public string Header(string name)
        {
            string prefix = name + ":";
            string line = Headers.FirstOrDefault(header => header.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            return line?.Substring(prefix.Length).Trim();
        }

        public override string ToString()
        {
            return Range.HasValue ? $"GET {Range.Value.First}-{Range.Value.Last}" : "GET whole";
        }
    }
}