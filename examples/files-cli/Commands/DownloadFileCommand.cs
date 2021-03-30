using FilesCom.Models;
using System;
using System.Threading.Tasks;

namespace files_cli.Commands
{
    public class DownloadFileCommand : BaseCommand
    {
        string Path { get; set; }
        string LocalPath { get; set; }

        public DownloadFileCommand()
        {
            IsCommand("DownloadFile", "Downloads a file");

            HasLongDescription("Downloads a file by a given path.");

            HasRequiredOption("path=", "The path of the file to download.", path => Path = path);
            HasOption("localPath=", "The local path and file name to save the file.", localPath => LocalPath = localPath);

            LocalPath = System.IO.Directory.GetCurrentDirectory();
        }

        public override async Task<int> RunAsync(string[] remainingArguments)
        {
            var result = await RemoteFile.DownloadFile(Path, LocalPath, await GetOptions());
            Console.WriteLine(result);

            return 0;
        }
    }
}
