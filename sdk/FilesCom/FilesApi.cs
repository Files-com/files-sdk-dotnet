using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom
{
    public interface IFilesApiService
    {
        Task<HttpResponseMessage> SendRequest(
            string path,
            HttpMethod verb,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        );

        Task StreamDownload(string uriString, Stream writeStream);

        Task ChunkUpload(HttpMethod verb, string uriString, Stream readStream, Int64 readLength);
    }

    public class FilesApiService : IFilesApiService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(FilesApiService));
        private readonly IHttpClientFactory _clientFactory;

        public FilesApiService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        private async Task HandleErrorResponse(HttpResponseMessage response, bool isTransfer = false)
        {
            ResponseError responseError;
            string body = await response.Content.ReadAsStringAsync();
            if (isTransfer)
            {
                log.Debug($"Transfer response: {body}");
            }
            try
            {
                responseError = JsonSerializer.Deserialize<ResponseError>(body, JsonUtil.Options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException(isTransfer
                    ? $"Transfer request failed (HTTP {(int)response.StatusCode})"
                    : "Unexpected data received from server: " + body);
            }
            string message = isTransfer
                ? $"Transfer request failed (HTTP {(int)response.StatusCode})"
                : $"HTTP request failed with code {(int)response.StatusCode}: {responseError.error} {responseError.type}";
            log.Error(message);
            if (responseError.type == null)
            {
                throw new ApiException(message, (int)response.StatusCode, responseError, response.Headers);
            }
            else
            {
                string[] errorParts = responseError.type.Split('/');
                string errorType = errorParts[errorParts.Length - 1];
                string errorClassName = String.Join("", Array.ConvertAll(errorType.Split('-'), part => part[0].ToString().ToUpper() + part.Substring(1))) + "Exception";

                Type type = Type.GetType("FilesCom." + errorClassName);
                if (!isTransfer)
                {
                    message = responseError.error;
                }
                throw (ApiException)Activator.CreateInstance(type, new object[] { message, (int)response.StatusCode, responseError, response.Headers });
            }
        }

        public async Task<HttpResponseMessage> SendRequest(
            string path,
            HttpMethod verb,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        )
        {
            FilesClient filesClient = FilesClient.Instance;

            if (filesClient == null)
            {
                throw new InvalidOperationException("FilesClient instance must be created before sending API requests.");
            }

            HttpClient httpClient = _clientFactory.CreateClient(FilesClient.HttpFilesApi);
            string parsedPath = ParsePathParameters(path, parameters);
            UriBuilder uri = new UriBuilder(httpClient.BaseAddress.ToString());
            uri.Path = $"api/rest/v1{parsedPath}";
            string jsonString = "";
            HttpContent httpContent = null;

            switch (verb.Method)
            {
                case "GET":
                case "HEAD":
                case "DELETE":
                    Dictionary<string, string> queryParams = new Dictionary<string, string>();
                    foreach (var k in parameters.Keys)
                    {
                        if (k.Length == 0)
                        {
                            continue;
                        }
                        if (parameters[k]?.GetType() == typeof(Dictionary<string, string>))
                        {
                            Dictionary<string, string> parameter = (Dictionary<string, string>)parameters[k];
                            foreach (var k2 in parameter.Keys)
                            {
                                queryParams.Add(k + "[" + k2 + "]", parameter[k2]?.ToString());
                            }
                        }
                        else
                        {
                            queryParams.Add(k, parameters[k]?.ToString());
                        }
                    }
                    uri.Query = new FormUrlEncodedContent(queryParams).ReadAsStringAsync().Result;
                    break;
                default:
                    jsonString = await Task.Run(() => JsonSerializer.Serialize(parameters, JsonUtil.Options));
                    httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
                    break;
            }

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = verb,
                RequestUri = uri.Uri,
                Headers = {
                    { HttpRequestHeader.Accept.ToString(), "application/json" },
                },
            };

            if (httpContent != null)
            {
                httpRequestMessage.Content = httpContent;
            }

            bool requiresAuth = !(path == "/sessions" && verb == HttpMethod.Post);

            if (requiresAuth)
            {
                object workspaceId = options.ContainsKey("workspace_id") ? options["workspace_id"] : filesClient.WorkspaceId;

                if (options.ContainsKey("session_id"))
                {
                    if (!(options["session_id"] is string))
                    {
                        throw new ArgumentException("Bad option: session_id must be of type Int64", "options[\"session_id\"]");
                    }

                    httpRequestMessage.Headers.Add("X-FilesAPI-Auth", options["session_id"].ToString());
                }
                else if (options.ContainsKey("api_key"))
                {
                    if (!(options["api_key"] is string))
                    {
                        throw new ArgumentException("Bad option: api_key must be of type string", "options[\"api_key\"]");
                    }

                    httpRequestMessage.Headers.Add("X-FilesAPI-Key", (string)options["api_key"]);
                }
                else if (filesClient.SessionId != null && filesClient.SessionId.Length > 0)
                {
                    httpRequestMessage.Headers.Add("X-FilesAPI-Auth", filesClient.SessionId.ToString());
                }
                else if (filesClient.ApiKey != null && filesClient.ApiKey.Length > 0)
                {
                    httpRequestMessage.Headers.Add("X-FilesAPI-Key", filesClient.ApiKey);
                }
                else
                {
                    throw new InvalidOperationException($"Authentication required for API request: {verb} {uri}");
                }

                if (workspaceId != null && !string.IsNullOrWhiteSpace(workspaceId.ToString()))
                {
                    httpRequestMessage.Headers.Add("X-Files-Workspace-Id", workspaceId.ToString());
                }
            }

            if (!string.IsNullOrWhiteSpace(filesClient.Language))
            {
                httpRequestMessage.Headers.Add("Accept-Language", filesClient.Language);
            }

            log.Info($"Sending {verb} request: {uri}");
            log.Debug($"content: {jsonString}");

            HttpResponseMessage response;
            try
            {
                response = await httpClient.SendAsync(httpRequestMessage);
            }
            catch (HttpRequestException e)
            {
                throw new ApiConnectionException(e.Message);
            }
            catch (Exception e) when (e is InvalidOperationException || e is ArgumentNullException)
            {
                throw new InvalidParameterException(e.Message);
            }
            if (!response.IsSuccessStatusCode)
            {
                await this.HandleErrorResponse(response);
            }
            return response;
        }

        public async Task StreamDownload(string uriString, Stream writeStream)
        {
            FilesClient filesClient = FilesClient.Instance;
            HttpClient httpClient = _clientFactory.CreateClient(FilesClient.HttpFilesApi);
            Uri uri = new Uri(uriString);
            HttpResponseMessage response;

            try
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(filesClient.ReadTimeout));

                response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cts.Token);
            }
            catch (HttpRequestException e)
            {
                log.Debug("Download transport error", e);
                throw new ApiConnectionException("Download request failed (HttpRequestException)");
            }
            catch (Exception e) when (e is InvalidOperationException || e is UriFormatException)
            {
                log.Debug("Download parameter error", e);
                throw new InvalidParameterException("Invalid download request");
            }
            using (response)
            {
                if (!response.IsSuccessStatusCode)
                {
                    await this.HandleErrorResponse(response, isTransfer: true);
                }

                using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                {
                    byte[] buffer = new byte[2 * 1024 * 1024]; // 2MB
                    int bytesRead;

                    while ((bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await writeStream.WriteAsync(buffer, 0, bytesRead);
                    }
                }
                log.Debug($"Successfully downloaded {uri}");
            }
        }

        public async Task ChunkUpload(HttpMethod verb, string uriString, Stream readStream, Int64 readLength)
        {
            if (readStream == null)
            {
                throw new ArgumentNullException(nameof(readStream));
            }
            // The existing implementation buffers a part in a single byte array.
            if (readLength < 0 || readLength > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(readLength), readLength, "An upload part must be from 0 to Int32.MaxValue bytes long.");
            }

            HttpClient httpClient = _clientFactory.CreateClient(FilesClient.HttpUpload);
            Uri uri = new Uri(uriString);
            // Reading the whole part before sending means a retried request resends these same bytes.
            byte[] part = await ReadPart(readStream, (int)readLength);
            HttpContent httpContent = new ByteArrayContent(part);

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

            log.Info($"Sending {verb} upload request");
            log.Debug($"Sending {verb} request: {uri}");
            log.Debug($"content: {readLength} bytes");

            HttpResponseMessage response;
            try
            {
                response = await httpClient.SendAsync(httpRequestMessage);
            }
            catch (HttpRequestException e)
            {
                log.Debug("Upload transport error", e);
                throw new ApiConnectionException("Upload request failed (HttpRequestException)");
            }
            catch (Exception e) when (e is InvalidOperationException || e is ArgumentNullException)
            {
                log.Debug("Upload parameter error", e);
                throw new InvalidParameterException("Invalid upload request");
            }
            using (response)
            {
                if (!response.IsSuccessStatusCode)
                {
                    await this.HandleErrorResponse(response, isTransfer: true);
                }
                string responseJson = await response.Content.ReadAsStringAsync();

                log.Debug(responseJson);
            }
        }

        // ReadAsync may return fewer bytes than requested, so keep reading until the part is full. Reading stops at
        // the part's length, which leaves the rest of the stream for the next part.
        private static async Task<byte[]> ReadPart(Stream readStream, int partLength)
        {
            byte[] part = new byte[partLength];
            int bytesRead = 0;
            while (bytesRead < partLength)
            {
                int count = await readStream.ReadAsync(part, bytesRead, partLength - bytesRead);
                if (count == 0)
                {
                    throw new EndOfStreamException($"The upload stream ended after {bytesRead} of the {partLength} bytes expected for this part.");
                }
                bytesRead += count;
            }
            return part;
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

                if (value == null)
                {
                    throw new InvalidOperationException($"Expected {name} parameter for path: {path}");
                }

                parsedPath = parsedPath.Replace(param.Value, value.ToString());
            }
            return parsedPath;
        }
    }
}