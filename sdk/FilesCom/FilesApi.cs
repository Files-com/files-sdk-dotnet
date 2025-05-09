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

        private async Task HandleErrorResponse(HttpResponseMessage response)
        {
            ResponseError responseError;
            string body = await response.Content.ReadAsStringAsync();
            try
            {
                responseError = JsonSerializer.Deserialize<ResponseError>(body);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + body);
            }
            string message = $"HTTP request failed with code {(int)response.StatusCode}: {responseError.error} {responseError.type}";
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
                message = responseError.error;
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
                    jsonString = await Task.Run(() => JsonSerializer.Serialize<Dictionary<string, object>>(parameters));
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
                if (options.ContainsKey("session_id"))
                {
                    if (!(options["session_id"] is string))
                    {
                        throw new ArgumentException("Bad option: session_id must be of type Int64", "options[\"session_id\"]");
                    }

                    httpRequestMessage.Headers.Add("X-FilesApi-Auth", options["session_id"].ToString());
                }
                else if (options.ContainsKey("api_key"))
                {
                    if (!(options["api_key"] is string))
                    {
                        throw new ArgumentException("Bad option: api_key must be of type string", "options[\"api_key\"]");
                    }

                    httpRequestMessage.Headers.Add("X-FilesApi-Key", (string)options["api_key"]);
                }
                else if (filesClient.SessionId != null && filesClient.SessionId.Length > 0)
                {
                    httpRequestMessage.Headers.Add("X-FilesApi-Auth", filesClient.SessionId.ToString());
                }
                else if (filesClient.ApiKey != null && filesClient.ApiKey.Length > 0)
                {
                    httpRequestMessage.Headers.Add("X-FilesApi-Key", filesClient.ApiKey);
                }
                else
                {
                    throw new InvalidOperationException($"Authentication required for API request: {verb} {uri}");
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
                throw new ApiConnectionException(e.Message);
            }
            catch (Exception e) when (e is InvalidOperationException || e is UriFormatException)
            {
                throw new InvalidParameterException(e.Message);
            }
            using (response)
            {
                if (!response.IsSuccessStatusCode)
                {
                    await this.HandleErrorResponse(response);
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
            HttpClient httpClient = _clientFactory.CreateClient(FilesClient.HttpUpload);
            Uri uri = new Uri(uriString);
            byte[] buffer = new byte[readLength];
            HttpContent httpContent = new ByteArrayContent(buffer);

            // Note: Casting to Integer here (int) means each part has a 2GB max length.  This is largely ok in the Files.com API design.
            await readStream.ReadAsync(buffer, 0, (int)readLength);

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
            using (response)
            {
                if (!response.IsSuccessStatusCode)
                {
                    await this.HandleErrorResponse(response);
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