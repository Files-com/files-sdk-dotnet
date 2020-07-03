using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Files
{
    public interface IFilesApiService
    {
        Task<string> SendRequest(
            string path,
            HttpMethod verb,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        );

        Task StreamDownload(string uriString, string localPath);

        Task ChunkUpload(HttpMethod verb, string uriString, FileStream readStream, int readLength);
    }

    public class FilesApiService : IFilesApiService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(FilesApiService));
        private readonly IHttpClientFactory _clientFactory;

        public FilesApiService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<string> SendRequest(
            string path,
            HttpMethod verb,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        )
        {
            HttpClient httpClient = _clientFactory.CreateClient(FilesClient.HttpFilesApi);
            string parsedPath = ParsePathParameters(path, parameters);
            Uri uri = new Uri(httpClient.BaseAddress, $"api/rest/v1{parsedPath}");
            string jsonString = await Task.Run(() => JsonSerializer.Serialize<Dictionary<string, object>>(parameters));
            HttpContent httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = verb,
                RequestUri = uri,
                Headers = {
                    { HttpRequestHeader.Accept.ToString(), "application/json" },
                },
                Content = httpContent,
            };

            log.Info($"Sending {verb} request: {uri}");
            log.Debug($"content: {jsonString}");

            using (HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage))
            {
                if (!response.IsSuccessStatusCode)
                {
                    string message = await response.Content.ReadAsStringAsync();
                    log.Error($"HTTP request failed with code {response.StatusCode}: {message}");
                    response.EnsureSuccessStatusCode();
                }
                string responseJson = await response.Content.ReadAsStringAsync();

                log.Debug(responseJson);
                return responseJson;
            }
        }

        public async Task StreamDownload(string uriString, string localPath)
        {
            HttpClient httpClient = _clientFactory.CreateClient(FilesClient.HttpFilesApi);
            Uri uri = new Uri(uriString);

            using (HttpResponseMessage response = await httpClient.GetAsync(uri))
            {
                response.EnsureSuccessStatusCode();

                using (FileStream fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await response.Content.CopyToAsync(fileStream);
                }
                log.Debug($"Successfully downloaded {uri} to {localPath}");
            }
        }

        public async Task ChunkUpload(HttpMethod verb, string uriString, FileStream readStream, int readLength)
        {
            HttpClient httpClient = _clientFactory.CreateClient(FilesClient.HttpUpload);
            Uri uri = new Uri(uriString);
            byte[] buffer = new byte[readLength];
            HttpContent httpContent = new ByteArrayContent(buffer);

            // TODO: Remove int restriction on length to allow files > 2GB
            await readStream.ReadAsync(buffer, 0, readLength);

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = verb,
                RequestUri = uri,
                Headers = {
                    { HttpRequestHeader.Accept.ToString(), "application/octet-stream" },
                    { HttpRequestHeader.ContentLength.ToString(), readLength.ToString() },
                },
                Content = httpContent,
            };

            log.Info($"Sending {verb} request: {uri}");
            log.Debug($"content: {readLength} bytes");

            using (HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage))
            {
                if (!response.IsSuccessStatusCode)
                {
                    string message = await response.Content.ReadAsStringAsync();
                    log.Error($"HTTP request failed with code {response.StatusCode}: {message}");
                    response.EnsureSuccessStatusCode();
                }
                string responseJson = await response.Content.ReadAsStringAsync();

                log.Debug(responseJson);
            }
        }

        protected static string ParsePathParameters(string path, Dictionary<string, object> parameters)
        {
            Regex regex = new Regex(@"(?<param>\{\w+\})", RegexOptions.Compiled);
            MatchCollection matches = regex.Matches(path);

            string parsedPath = path;
            foreach (Match match in matches)
            {
                Group param = match.Groups["param"];
                string name = param.Value.Substring(1, param.Value.Length - 2);
                object value = parameters[name];

                if (value == null) {
                    throw new InvalidOperationException($"Expected {name} parameter for path: {path}");
                }

                parsedPath = parsedPath.Replace(param.Value, value.ToString());
            }
            return parsedPath;
        }
    }
}

