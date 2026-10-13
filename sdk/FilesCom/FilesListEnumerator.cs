using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom
{
    /// <summary>
    /// Enumerates every item of a <see cref="FilesList{T}"/>, from its first page on, blocking while each page
    /// loads. <see cref="FilesList{T}.ListAutoPagingAsync"/> does the same without blocking.
    /// </summary>
    public class FilesListEnumerator<T> : IEnumerator<T>, IEnumerable<T>
    {
        private readonly FilesList<T> filesList;
        private readonly CancellationToken cancellationToken;
        private readonly bool wrapsFailures;
        private FilesList<T>.Paging paging;
        private FilesList<T>.Page page;
        private int index = -1;
        private T current;

        /// <summary>
        /// Loads the first page. Failures are thrown wrapped in <see cref="AggregateException"/>.
        /// </summary>
        /// <param name="filesList">The list to enumerate, from its first page.</param>
        public FilesListEnumerator(FilesList<T> filesList) : this(filesList, CancellationToken.None, wrapsFailures: true)
        {
        }

        internal FilesListEnumerator(FilesList<T> filesList, CancellationToken cancellationToken) : this(filesList, cancellationToken, wrapsFailures: false)
        {
        }

        private FilesListEnumerator(FilesList<T> filesList, CancellationToken cancellationToken, bool wrapsFailures)
        {
            this.filesList = filesList;
            this.cancellationToken = cancellationToken;
            this.wrapsFailures = wrapsFailures;
            LoadFirstPage();
        }

        /// <summary>
        /// The item the last call of <see cref="MoveNext"/> that returned true moved to, or the default value of
        /// <typeparamref name="T"/> before any has. It stays that item when MoveNext returns false or fails, and after
        /// <see cref="Reset"/>. Reading it loads nothing.
        /// </summary>
        public T Current { get { return current; } }

        object IEnumerator.Current { get { return current; } }

        /// <summary>
        /// Does nothing: the enumerator holds nothing open between pages, since each page's response is disposed once
        /// the page has been read. The enumerator can still be used afterwards.
        /// </summary>
        public void Dispose()
        {
            // no resources to release
        }

        /// <summary>
        /// Returns this enumerator itself, not a new one, and loads nothing. Enumerating it again, such as with a second
        /// <c>foreach</c>, continues from where the last enumeration stopped rather than starting over, so once one has
        /// reached the end, the next has no items; <see cref="Reset"/> starts over from the first page.
        /// </summary>
        /// <returns>This enumerator.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        /// <summary>
        /// Starts again from the first page, which it loads now.
        /// </summary>
        public void Reset()
        {
            LoadFirstPage();
        }

        /// <summary>
        /// Moves to the next item. When the page loaded last has no more items, it first loads the next page, blocking
        /// while it does, and passes over pages that have no items.
        /// </summary>
        /// <remarks>
        /// A failure to load a page is thrown wrapped in <see cref="AggregateException"/> when this enumerator was made
        /// without a cancellation token, by its constructor or <see cref="FilesList{T}.ListAutoPaging()"/>, and as it
        /// is when it was made by <see cref="FilesList{T}.ListAutoPaging(CancellationToken)"/>. A failed load keeps
        /// <see cref="Current"/> and the cursor of the page to load next, so the next call of MoveNext requests that
        /// same page.
        /// </remarks>
        /// <returns>True when <see cref="Current"/> is the next item; false when the list has no more items, and on
        /// every call after that until <see cref="Reset"/>.</returns>
        public bool MoveNext()
        {
            index++;
            // Pages can be empty, so keep loading until one has an item or there are no more.
            while (index >= page.Items.Count)
            {
                string nextCursor = page.NextCursor;
                if (nextCursor == null)
                {
                    return false;
                }
                page = WaitForPage(() => filesList.LoadPage(paging, nextCursor, cancellationToken));
                index = 0;
            }

            current = page.Items[index];
            return true;
        }

        private void LoadFirstPage()
        {
            page = WaitForPage(() =>
            {
                paging = filesList.StartPaging();
                return filesList.LoadPage(paging, null, cancellationToken);
            });
            index = -1;
        }

        // Loads on the thread pool, so that a caller's synchronization context cannot deadlock the wait.
        private FilesList<T>.Page WaitForPage(Func<Task<FilesList<T>.Page>> loadPage)
        {
            Task<FilesList<T>.Page> load = Task.Run(loadPage);
            if (wrapsFailures)
            {
                load.Wait();
                return load.Result;
            }
            return load.GetAwaiter().GetResult();
        }
    }
}