using FilesCom;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCli.Commands
{
    internal sealed class DownloadFileCommand : FilesCommand
    {
        private readonly Option<string> path = new Option<string>("--path")
        {
            Description = "The Files.com path of the file to download.",
            Required = true,
        };

        private readonly Option<string> localPath = new Option<string>("--local-path", "--localPath")
        {
            Description = "Where to save it. Defaults to the file's name in the current directory.",
        };

        public DownloadFileCommand() : base("download-file", "DownloadFile", "Downloads a file.")
        {
            Options.Add(path);
            Options.Add(localPath);
        }

        protected override async Task ExecuteAsync(ParseResult parseResult, FilesClient client, Dictionary<string, object> credentials, CancellationToken cancellationToken)
        {
            string source = parseResult.GetValue(path);
            string destination = parseResult.GetValue(localPath);

            // A cancelled or failed download leaves what it had written in the local file.
            await client.RemoteFiles.DownloadFileAsync(source, destination, credentials, cancellationToken);
            Console.WriteLine($"Downloaded {source} to {destination ?? Path.GetFileName(source)}");
        }
    }
}