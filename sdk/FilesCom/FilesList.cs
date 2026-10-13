using FilesCom.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom
{
    /// <summary>
    /// A list whose items are loaded a page at a time. Creating it sends no request; each page is requested when
    /// you load it. Every page, including pages loaded after <see cref="Reset"/>, comes from the list's client.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="ListAutoPagingAsync"/> goes through all the items, loading one page at a time as you reach it, and
    /// <see cref="ListAutoPaging(CancellationToken)"/> does the same while blocking. <see cref="LoadNextPageAsync"/>
    /// loads one page into <see cref="data"/>, and <see cref="HasNextPage"/> tells you whether there is another.
    /// <see cref="AllAsync"/> loads every page and returns all their items together.
    /// </para>
    /// <para>
    /// A list's query is the parameters it was made with, copied then. ListAutoPagingAsync, ListAutoPaging and
    /// AllAsync each run it from its first page, with their own position, and leave <see cref="data"/> and
    /// <see cref="HasNextPage"/> as they were.
    /// </para>
    /// <para>
    /// A page can be empty and still be followed by more. A page with no items that gives back the cursor it was
    /// loaded with makes no progress: loading it throws <see cref="InvalidResponseException"/>, rather than ending the
    /// list as if it were complete.
    /// </para>
    /// </remarks>
    public class FilesList<T> : IEnumerable<T>
    {
        private readonly string path;
        private readonly HttpMethod method;
        private readonly Dictionary<string, object> parameters;
        private readonly Dictionary<string, object> options;
        private FilesClient client;
        // Where the list's pages come from when not from its client's BaseUrl: the address of its site's region, where
        // loading a page moved. Null for BaseUrl.
        private volatile string origin;
        private string cursor;

        /// <summary>
        /// The items of the page <see cref="LoadNextPageAsync"/> loaded last.
        /// </summary>
        public List<T> data = new List<T>();

        /// <summary>
        /// Makes a list that belongs to the default client from its first page on.
        /// </summary>
        /// <param name="path">The API path to list, such as <c>/users</c>.</param>
        /// <param name="method">The HTTP method of each page's request.</param>
        /// <param name="parameters">The list's query, keyed by API names; copied now.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public FilesList(string path, HttpMethod method, Dictionary<string, object> parameters, Dictionary<string, object> options)
            : this(null, path, method, parameters, options)
        {
        }

        // A list of client's, or, when client is null, of the default client at the time of its first page.
        internal FilesList(FilesClient client, string path, HttpMethod method, Dictionary<string, object> parameters, Dictionary<string, object> options)
        {
            this.client = client;
            this.path = path;
            this.method = method;
            this.parameters = DictionaryUtil.Copy(parameters) ?? new Dictionary<string, object>();
            this.options = options;
        }

        /// <summary>
        /// Enumerates the items of the page loaded last, in <see cref="data"/>. It loads no page.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        /// <summary>
        /// Whether <see cref="LoadNextPageAsync"/> has another page to load.
        /// </summary>
        public bool HasNextPage { get { return cursor != null; } }

        /// <summary>
        /// Makes <see cref="LoadNextPageAsync"/> load the first page again, and clears <see cref="data"/>.
        /// </summary>
        public void Reset()
        {
            cursor = null;
            data.Clear();
        }

        /// <summary>
        /// Same as <see cref="LoadNextPageAsync"/>, without a cancellation token.
        /// </summary>
        public Task<FilesList<T>> LoadNextPage()
        {
            return LoadNextPageAsync(CancellationToken.None);
        }

        /// <summary>
        /// Loads the next page into <see cref="data"/>, replacing the previous page. The first call, and the first
        /// after <see cref="Reset"/>, loads the first page.
        /// </summary>
        /// <remarks>
        /// Once <see cref="HasNextPage"/> is false, the next call starts again from the first page. If loading a page
        /// fails, <see cref="data"/> and <see cref="HasNextPage"/> stay as they were, so the same page can be tried
        /// again.
        /// </remarks>
        /// <param name="cancellationToken">Stops loading the page.</param>
        public async Task<FilesList<T>> LoadNextPageAsync(CancellationToken cancellationToken = default)
        {
            Page page = await LoadPage(StartPaging(), cursor, cancellationToken);
            data = page.Items;
            cursor = page.NextCursor;
            return this;
        }

        /// <summary>
        /// Same as <see cref="ListAutoPaging(CancellationToken)"/> without a token, except that failures are thrown
        /// wrapped in <see cref="AggregateException"/>.
        /// </summary>
        public FilesListEnumerator<T> ListAutoPaging()
        {
            return new FilesListEnumerator<T>(this);
        }

        /// <summary>
        /// Enumerates every item, from the first page on, blocking while it loads each page. All pages use the client
        /// settings in effect when the first page is loaded, which is before this returns.
        /// </summary>
        /// <remarks>
        /// Prefer <see cref="ListAutoPagingAsync"/>, which does not block. Failures, including cancellation as
        /// <see cref="OperationCanceledException"/>, are thrown as they are, as if awaited. The overload without a
        /// token wraps them in <see cref="AggregateException"/>.
        /// </remarks>
        /// <param name="cancellationToken">Stops loading pages.</param>
        public FilesListEnumerator<T> ListAutoPaging(CancellationToken cancellationToken)
        {
            return new FilesListEnumerator<T>(this, cancellationToken);
        }

        /// <summary>
        /// Enumerates every item asynchronously, from the first page on, with <c>await foreach</c>. It holds one
        /// page at a time and loads the next when you reach its end, so stopping early, with <c>break</c>, loads no
        /// further page.
        /// </summary>
        /// <remarks>
        /// Nothing is sent until you start enumerating. All pages use the client settings in effect when the first
        /// page is loaded. Each enumeration starts again from the first page. Failures and cancellation are thrown
        /// from the <c>await foreach</c> as they are. The token given here and one given with
        /// <c>WithCancellation</c> both cancel it.
        /// </remarks>
        /// <param name="cancellationToken">Stops loading pages.</param>
        public async IAsyncEnumerable<T> ListAutoPagingAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            Paging paging = StartPaging();
            Page page = null;
            do
            {
                page = await LoadPage(paging, page?.NextCursor, cancellationToken);
                foreach (T item in page.Items)
                {
                    yield return item;
                }
            } while (page.NextCursor != null);
        }

        // Enumerates every item of the list as part of a compound operation: every page uses context, its client
        // settings, origin and lifetime, and options, the operation's own snapshot, whatever this list was made with.
        internal async IAsyncEnumerable<T> ListWithinAsync(OperationContext context, Dictionary<string, object> options, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var paging = new Paging(context, DictionaryUtil.Copy(options) ?? new Dictionary<string, object>());
            Page page = null;
            do
            {
                page = await LoadPage(paging, page?.NextCursor, cancellationToken);
                foreach (T item in page.Items)
                {
                    yield return item;
                }
            } while (page.NextCursor != null);
        }

        /// <summary>
        /// Same as <see cref="AllAsync"/>, without a cancellation token.
        /// </summary>
        public Task<List<T>> All()
        {
            return AllAsync(CancellationToken.None);
        }

        /// <summary>
        /// Loads every page from the first one and returns all their items, which are all in memory at once. All
        /// pages use the client settings in effect when this starts.
        /// </summary>
        /// <remarks>
        /// To go through a long list without holding all of it, use <see cref="ListAutoPagingAsync"/>.
        /// </remarks>
        /// <param name="cancellationToken">Stops loading pages.</param>
        public async Task<List<T>> AllAsync(CancellationToken cancellationToken = default)
        {
            List<T> all = new List<T>();
            await foreach (T item in ListAutoPagingAsync(cancellationToken))
            {
                all.Add(item);
            }
            return all;
        }

        // The client settings and options every page of one paging journey uses, so that a journey never mixes
        // endpoints or credentials.
        internal sealed class Paging
        {
            internal Paging(OperationContext context, Dictionary<string, object> options)
            {
                Context = context;
                Options = options;
            }

            internal OperationContext Context { get; }

            internal Dictionary<string, object> Options { get; }
        }

        internal Paging StartPaging()
        {
            return new Paging(new OperationContext(FilesClient.Bind(ref client), origin), DictionaryUtil.Copy(options));
        }

        // One page's items, and the cursor of the page after it, or null when it is the last.
        internal sealed class Page
        {
            internal Page(List<T> items, string nextCursor)
            {
                Items = items;
                NextCursor = nextCursor;
            }

            internal List<T> Items { get; }

            internal string NextCursor { get; }
        }

        // Loads the page at pageCursor, or the query's first page when pageCursor is null. The cursor, not the
        // number of items, says whether another page follows, so an empty page can come before more items, and an
        // empty page without a next cursor is the last. An empty page that gives back the cursor it was loaded with
        // makes no progress: loading it again would repeat forever, and taking it for the end would report a list as
        // complete that is not known to be. It fails with InvalidResponseException, before anything of the page is
        // kept, so a caller's position is where it was.
        internal async Task<Page> LoadPage(Paging paging, string pageCursor, CancellationToken cancellationToken)
        {
            Dictionary<string, object> query = new Dictionary<string, object>(parameters);
            if (pageCursor != null)
            {
                query["cursor"] = pageCursor;
            }
            // The page is read, and its response disposed, while the list holds its client open.
            using (OperationContext.OperationScope loading = paging.Context.Begin(cancellationToken))
            using (HttpResponseMessage response = await FilesClient.SendRequest(paging.Context, path, method, query, paging.Options, loading.Token))
            {
                // SendRequest has already buffered the body, honoring the token.
                string body = await response.Content.ReadAsStringAsync();
                List<T> items = JsonUtil.DeserializeWithOptions<List<T>>(body, paging.Context, paging.Options);
                if (items == null)
                {
                    throw new InvalidResponseException("Unexpected data received from server: a page of results was null instead of an array");
                }
                // Later pages, and later journeys through the list, come from where this page came from.
                origin = paging.Context.RelocatedOrigin;
                string nextCursor = NextCursor(response);
                if (items.Count == 0 && nextCursor != null && nextCursor == pageCursor)
                {
                    throw new InvalidResponseException("Files.com answered a page of the list with no items and the same cursor it was asked for, so the list makes no progress and cannot be listed further.");
                }
                return new Page(items, nextCursor);
            }
        }

        // Files.com sends the next page's cursor in X-Files-Cursor-Next, and also in the older X-Files-Cursor, which
        // earlier versions of the API sent alone.
        private static string NextCursor(HttpResponseMessage response)
        {
            foreach (string header in new[] { "X-Files-Cursor-Next", "X-Files-Cursor" })
            {
                if (response.Headers.TryGetValues(header, out IEnumerable<string> values))
                {
                    foreach (string value in values)
                    {
                        return string.IsNullOrEmpty(value) ? null : value;
                    }
                }
            }
            return null;
        }
    }
}