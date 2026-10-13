using FilesCom;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCli.Commands
{
    internal sealed class UploadFileCommand : FilesCommand
    {
        private readonly Option<FileInfo> localPath = new Option<FileInfo>("--local-path", "--localPath")
        {
            Description = "The file to upload.",
            Required = true,
        }.AcceptExistingOnly();

        private readonly Option<string> path = new Option<string>("--path")
        {
            Description = "Where to put it in Files.com, such as reports/report.pdf.",
            Required = true,
        };

        public UploadFileCommand() : base("upload-file", "UploadFile", "Uploads a file.")
        {
            Options.Add(localPath);
            Options.Add(path);
        }

        protected override async Task ExecuteAsync(ParseResult parseResult, FilesClient client, Dictionary<string, object> credentials, CancellationToken cancellationToken)
        {
            FileInfo file = parseResult.GetValue(localPath);
            string destination = parseResult.GetValue(path);

            await client.RemoteFiles.UploadFileAsync(file.FullName, destination, credentials, cancellationToken: cancellationToken);
            Console.WriteLine($"Uploaded {file.FullName} to {destination}");
        }
    }
}