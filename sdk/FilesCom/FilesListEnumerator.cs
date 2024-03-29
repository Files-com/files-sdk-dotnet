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

        public FilesListEnumerator(FilesList<T> filesList)
        {
            this.filesList = filesList;
            Task.Run(() => filesList.LoadNextPage()).Wait();
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
            Task.Run(() => filesList.LoadNextPage()).Wait();
            index = -1;
        }

        public bool MoveNext()
        {
            index++;
            if (index >= filesList.data.Count)
            {
                if (filesList.HasNextPage)
                {
                    Task.Run(() => filesList.LoadNextPage()).Wait();
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
    }
}