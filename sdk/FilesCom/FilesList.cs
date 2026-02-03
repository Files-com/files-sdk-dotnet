using FilesCom.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace FilesCom
{
    public class FilesList<T> : IEnumerable<T>
    {
        private string path;
        private HttpMethod method;
        private Dictionary<string, object> parameters;
        private Dictionary<string, object> options;
        private string cursor;
        public List<T> data = new List<T>();

        public FilesList(string path, HttpMethod method, Dictionary<string, object> parameters, Dictionary<string, object> options)
        {
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

        public async Task<FilesList<T>> LoadNextPage()
        {
            if (cursor != null)
            {
                parameters["cursor"] = cursor;
            }
            using (HttpResponseMessage response = await FilesClient.SendRequest(path, method, parameters, options))
            {
                string body = await response.Content.ReadAsStringAsync();
                try
                {
                    data = JsonSerializer.Deserialize<List<T>>(body, JsonUtil.Options);
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
            return this;
        }

        public FilesListEnumerator<T> ListAutoPaging()
        {
            return new FilesListEnumerator<T>(this);
        }

        public async Task<List<T>> All()
        {
            List<T> allData = new List<T>();

            // Force starting from the beginning
            cursor = null;

            do
            {
                await LoadNextPage();
                allData.AddRange(data);
            } while (cursor != null);

            return allData;
        }
    }
}