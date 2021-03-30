using FilesCom.Models;
using System;
using System.Threading.Tasks;

namespace files_cli.Commands
{
    public class UploadFileCommand : BaseCommand
    {
        string Path { get; set; }
        string LocalPath { get; set; }

        public UploadFileCommand()
        {
            IsCommand("UploadFile", "Uploads a file");

            HasLongDescription("Uploads a file to a given path.");

            HasRequiredOption("path=", "The path of the file to be uploaded.", path => Path = path);
            HasOption("localPath=", "The path of the file to be uploaded.", localPath => LocalPath = localPath);

            LocalPath = System.IO.Directory.GetCurrentDirectory();
        }

        public override async Task<int> RunAsync(string[] remainingArguments)
        {
            var result = await RemoteFile.UploadFile(LocalPath, Path, await GetOptions());
            Console.WriteLine(result);

            return 0;
        }
    }
}
