using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FilesCom;

namespace TransferDownloads
{
    // A remote text file, read line by line through a stream, as a local one would be. The download behind the stream
    // stays a little ahead of the reader; disposing the stream early stops it.
    internal static class Cat
    {
        internal static async Task PrintAsync(FilesClient client, string remotePath, CancellationToken cancellationToken)
        {
            using (Stream remote = await client.FileSystem.OpenReadAsync(remotePath, cancellationToken: cancellationToken))
            using (var reader = new StreamReader(remote))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    Console.WriteLine(line);
                }
            }
        }
    }
}
