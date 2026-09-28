using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom
{
    public class FilesListEnumerator<T> : IEnumerator<T>, IEnumerable<T>
    {
        private FilesList<T> filesList;
        private readonly CancellationToken cancellationToken;
        private readonly bool wrapsFailures;
        private FilesList<T>.Paging paging;
        private int index = -1;
        private T current;

        /// <summary>
        /// Loads the first page. Failures are thrown wrapped in <see cref="AggregateException"/>.
        /// </summary>
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
            WaitForPage(() =>
            {
                paging = filesList.StartPaging();
                return filesList.LoadNextPage(paging, cancellationToken);
            });
        }

        public T Current { get { return current; } }

        object IEnumerator.Current { get { return current; } }

        public void Dispose()
        {
            // no resources to release
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        public void Reset()
        {
            filesList.Reset();
            WaitForPage(() => filesList.LoadNextPage(paging, cancellationToken));
            index = -1;
        }

        public bool MoveNext()
        {
            index++;
            if (index >= filesList.data.Count)
            {
                if (filesList.HasNextPage)
                {
                    WaitForPage(() => filesList.LoadNextPage(paging, cancellationToken));
                    index = 0;
                }
                else
                {
                    return false;
                }
            }

            if (filesList.data.Count == 0)
            {
                return false;
            }

            current = filesList.data[index];
            return true;
        }

        // Loads on the thread pool, so that a caller's synchronization context cannot deadlock the wait.
        private void WaitForPage(Func<Task> loadPage)
        {
            Task load = Task.Run(loadPage);
            if (wrapsFailures)
            {
                load.Wait();
            }
            else
            {
                load.GetAwaiter().GetResult();
            }
        }
    }
}