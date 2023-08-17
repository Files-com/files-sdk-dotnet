using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FilesCom
{
    public class FilesListEnumerator<T> : IEnumerator<T>, IEnumerable<T>
    {
        private FilesList<T> filesList;
        private int index = -1;
        private T current;
        private int page = 0;

        public FilesListEnumerator(FilesList<T> filesList)
        {
            this.filesList = filesList;
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
            page = 0;
            index = -1;
        }

        public bool MoveNext()
        {
            index++;
            if (index >= filesList.data.Count)
            {
                if (filesList.HasNextPage || page == 0)
                {
                    Task.Run(() => filesList.LoadNextPage()).Wait();
                    index = 0;
                    page++;
                }
                else
                {
                    return false;
                }
            }

            current = filesList.data[index];
            return true;
        }
    }
}