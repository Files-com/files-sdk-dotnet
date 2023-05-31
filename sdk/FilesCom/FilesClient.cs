using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FilesCom
{
    public class FilesClient
    {
        public const string HttpFilesApi = "HttpFilesAPI";
        public const string HttpUpload = "HttpUpload";

        private const string UserAgent = "Files-.NET-SDK";
        private const string ConfigManagerSectionName = "files.com/filesConfiguration";

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(FilesConfiguration));

        private readonly FilesConfiguration config;
        private readonly IHost host;

        public static FilesClient Instance { get; private set; }

        public FilesClient(FilesConfiguration config = null)
        {
            if (Instance != null)
            {
                log.Info("Files.com Client instance already exists, replacing instance with new one");
            }

            Instance = this;
            this.config = config;

            if (this.config == null)
            {
                log.Info("FilesConfiguration found in app.config");
                this.config = (FilesConfiguration)ConfigurationManager.GetSection(ConfigManagerSectionName);
            }
            if (this.config == null)
            {
                log.Info("No FilesConfiguration found, using defaults");
                this.config = new FilesConfiguration();
            }

            if (this.SessionId != null && this.SessionId.Length > 0)
            {
                log.Info("Files.com Client created with Session Id");
            }
            else if (this.ApiKey != null && this.ApiKey.Length > 0)
            {
                log.Info("Files.com Client created with API Key");
            }
            else
            {
                log.Info("Files.com Client created with no preconfigured auth");
            }

            var builder = new HostBuilder()
                .ConfigureServices((HostExecutionContext, services) =>
                {
                    TimeSpan[] retries = new TimeSpan[this.config.MaxNetworkRetries];
                    Random rand = new Random();
                    for (int i = 0; i < retries.Length; i++)
                    {
                        double delay;

                        if (i == 0)
                        {
                            delay = this.config.InitialNetworkRequestDelay;
                        }
                        else if (i == retries.Length - 1)
                        {
                            delay = this.config.MaxNetworkRetryDelay;
                        }
                        else
                        {
                            delay = Math.Min(this.config.InitialNetworkRequestDelay, rand.NextDouble() * this.config.MaxNetworkRetryDelay);
                        }

                        retries[i] = TimeSpan.FromSeconds(delay);
                    }

                    services.AddHttpClient(HttpFilesApi, client =>
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Add(HttpRequestHeader.UserAgent.ToString(), UserAgent);
                    })
                    .AddTransientHttpErrorPolicy(newBuilder => newBuilder.WaitAndRetryAsync(retries));

                    services.AddHttpClient(HttpUpload, client =>
                    {
                        client.DefaultRequestHeaders.Add(HttpRequestHeader.UserAgent.ToString(), UserAgent);
                    })
                    .AddTransientHttpErrorPolicy(newBuilder => newBuilder.WaitAndRetryAsync(retries));

                    services.AddTransient<IFilesApiService, FilesApiService>();
                }).UseConsoleLifetime();

            host = builder.Build();
        }

        public string BaseUrl
        {
            get { return config.BaseUrl; }
        }

        public string ApiKey
        {
            get { return config.ApiKey; }
        }

        public string SessionId
        {
            get { return config.SessionId; }
        }

        public static async Task<string> SendRequest(
            string path,
            HttpMethod verb,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        )
        {
            if (Instance == null)
            {
                throw new InvalidOperationException("Instance must be created before sending API request");
            }

            using (var serviceScope = Instance.host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    var filesApi = services.GetRequiredService<IFilesApiService>();
                    return await filesApi.SendRequest(path, verb, parameters, options);
                }
                catch (Exception ex)
                {
                    log.Error($"Failed to send Files API Request to path: {path}", ex);
                    throw;
                }
            }
        }

        public static async Task StreamDownload(string uri, Stream writeStream)
        {
            if (Instance == null)
            {
                throw new InvalidOperationException("Instance must be created before streaming download");
            }

            using (var serviceScope = Instance.host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    var filesApi = services.GetRequiredService<IFilesApiService>();
                    await filesApi.StreamDownload(uri, writeStream);
                }
                catch (Exception ex)
                {
                    log.Error($"Failed to stream download from {uri}", ex);
                    throw;
                }
            }
        }

        public static async Task ChunkUpload(HttpMethod verb, string uri, Stream readStream, Int64 readLength)
        {
            if (Instance == null)
            {
                throw new InvalidOperationException("Instance must be created before uploading chunk");
            }

            using (var serviceScope = Instance.host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    var filesApi = services.GetRequiredService<IFilesApiService>();
                    await filesApi.ChunkUpload(verb, uri, readStream, readLength);
                }
                catch (Exception ex)
                {
                    log.Error($"Failed to upload chunk to {uri}", ex);
                    throw;
                }
            }
        }
    }
}