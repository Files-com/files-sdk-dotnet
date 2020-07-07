using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;

namespace Files
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

        public FilesClient() : this((FilesConfiguration)ConfigurationManager.GetSection(ConfigManagerSectionName)) { }

        public FilesClient(FilesConfiguration config)
        {
            if (Instance != null) {
                log.Info("Files.com Client instance already exists, replacing instance with new one");
            }

            Instance = this;
            this.config = config;

            // TODO: Add user/password support?
            if (this.config.ApiKey != null)
            {
                log.Info("Files.com Client created with API Key");
            }
            else
            {
                log.Error("Files.com Client Settings must have ApiKey");
                throw new InvalidOperationException("Incomplete settings for Files.com Client");
            }

            var builder = new HostBuilder()
                .ConfigureServices((HostExecutionContext, services) =>
                {
                    TimeSpan[] retries = new TimeSpan[config.MaxNetworkRetries];
                    Random rand = new Random();
                    for (int i = 0; i < retries.Length; i++)
                    {
                        double delay;

                        if (i == 0)
                        {
                            delay = config.InitialNetworkRequestDelay;
                        }
                        else if (i == retries.Length - 1)
                        {
                            delay = config.MaxNetworkRetryDelay;
                        }
                        else
                        { 
                            delay = Math.Min(config.InitialNetworkRequestDelay, rand.NextDouble() * config.MaxNetworkRetryDelay);
                        }

                        retries[i] = TimeSpan.FromSeconds(delay);
                    }

                    services.AddHttpClient(HttpFilesApi, client =>
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Add(HttpRequestHeader.UserAgent.ToString(), UserAgent);
                        client.DefaultRequestHeaders.Add("X-FilesApi-Key", ApiKey);
                    })
                    .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(retries));

                    services.AddHttpClient(HttpUpload, client =>
                    {
                        client.DefaultRequestHeaders.Add(HttpRequestHeader.UserAgent.ToString(), UserAgent);
                    })
                    .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(retries));

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

        public static async Task<string> SendRequest(
            string path,
            HttpMethod verb,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        )
        {
            if (Instance == null) {
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
                    throw ex;
                }
            }
        }

        public static async Task StreamDownload(string uri, string localPath)
        { 
            if (Instance == null) {
                throw new InvalidOperationException("Instance must be created before streaming download");
            }

            using (var serviceScope = Instance.host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    var filesApi = services.GetRequiredService<IFilesApiService>();
                    await filesApi.StreamDownload(uri, localPath);
                }
                catch (Exception ex)
                {
                    log.Error($"Failed to stream download from {uri}", ex);
                    throw ex;
                }
            }
        }

        public static async Task ChunkUpload(HttpMethod verb, string uri, FileStream readStream, int readLength)
        {
            if (Instance == null) {
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
                    throw ex;
                }
            }
        }
    }
}

