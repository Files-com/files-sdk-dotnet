using FilesCom.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom
{
    /// <summary>
    /// A list whose items are loaded a page at a time. Creating it sends no request; each page is requested when
    /// you load it. Every page, including pages loaded after <see cref="Reset"/>, comes from the list's client.
    /// </summary>
    public class FilesList<T> : IEnumerable<T>
    {
        private string path;
        private HttpMethod method;
        private Dictionary<string, object> parameters;
        private Dictionary<string, object> options;
        private FilesClient client;
        private string cursor;
        public List<T> data = new List<T>();

        /// <summary>
        /// Makes a list that belongs to the default client from its first page on.
        /// </summary>
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
            this.parameters = parameters;
            this.options = options;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        public bool HasNextPage { get { return cursor != null; } }

        public void Reset()
        {
            cursor = null;
            parameters.Remove("cursor");
            data.Clear();
        }

        public Task<FilesList<T>> LoadNextPage()
        {
            return LoadNextPageAsync(CancellationToken.None);
        }

        /// <summary>
        /// Loads the next page into <see cref="data"/>, replacing the previous page.
        /// </summary>
        public async Task<FilesList<T>> LoadNextPageAsync(CancellationToken cancellationToken = default)
        {
            await LoadNextPage(StartPaging(), cancellationToken);
            return this;
        }

        internal async Task LoadNextPage(Paging paging, CancellationToken cancellationToken)
        {
            if (cursor != null)
            {
                parameters["cursor"] = cursor;
            }
            using (HttpResponseMessage response = await FilesClient.SendRequest(paging.Context, path, method, parameters, paging.Options, cancellationToken))
            {
                string body = await response.Content.ReadAsStringAsync();
                try
                {
                    data = JsonUtil.DeserializeWithOptions<List<T>>(body, paging.Context.Client, paging.Options);
                }
                catch (JsonException)
                {
                    throw new InvalidResponseException("Unexpected data received from uri: " + body);
                }
                if (response.Headers.Contains("X-Files-Cursor"))
                {
                    cursor = new List<string>(response.Headers.GetValues("X-Files-Cursor"))[0];
                }
                else
                {
                    cursor = null;
                }
            }
        }

        public FilesListEnumerator<T> ListAutoPaging()
        {
            return new FilesListEnumerator<T>(this);
        }

        /// <summary>
        /// Enumerates every item, loading each page as it is reached. All pages use the client settings in effect
        /// when the first page is loaded.
        /// </summary>
        /// <remarks>
        /// Failures, including cancellation as <see cref="OperationCanceledException"/>, are thrown as they are,
        /// as if awaited. The overload without a token wraps them in <see cref="AggregateException"/>.
        /// </remarks>
        public FilesListEnumerator<T> ListAutoPaging(CancellationToken cancellationToken)
        {
            return new FilesListEnumerator<T>(this, cancellationToken);
        }

        public Task<List<T>> All()
        {
            return AllAsync(CancellationToken.None);
        }

        /// <summary>
        /// Loads every page from the first one and returns all their items. All pages use the client settings in
        /// effect when this starts.
        /// </summary>
        public async Task<List<T>> AllAsync(CancellationToken cancellationToken = default)
        {
            Paging paging = StartPaging();
            List<T> allData = new List<T>();

            // Force starting from the beginning
            cursor = null;

            do
            {
                await LoadNextPage(paging, cancellationToken);
                allData.AddRange(data);
            } while (cursor != null);

            return allData;
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
            return new Paging(new OperationContext(FilesClient.Bind(ref client)), DictionaryUtil.Copy(options));
        }
    }
}