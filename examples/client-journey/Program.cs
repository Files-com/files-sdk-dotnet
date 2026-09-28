using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FilesCom;
using FilesCom.Models;

namespace ClientJourney
{
    // Works with Files.com through one explicit client: signs in, creates and updates a file, pages through a
    // folder and downloads the file. Every step can be cancelled with Ctrl+C and stops at the time limit.
    //
    //   dotnet run -- https://MY-SUBDOMAIN.files.com [time-limit-seconds]
    //
    // Signs in with FILES_API_KEY, or with a session for FILES_USERNAME and FILES_PASSWORD. The example works in a
    // new folder named files-sdk-example-<time> and deletes it at the end.
    internal static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Usage: client-journey <base-url> [time-limit-seconds]");
                return 2;
            }
            string apiKey = Environment.GetEnvironmentVariable("FILES_API_KEY");
            string username = Environment.GetEnvironmentVariable("FILES_USERNAME");
            string password = Environment.GetEnvironmentVariable("FILES_PASSWORD");
            if (string.IsNullOrEmpty(apiKey) && (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)))
            {
                Console.Error.WriteLine("Set FILES_API_KEY, or both FILES_USERNAME and FILES_PASSWORD, to sign in.");
                return 2;
            }
            TimeSpan timeLimit = TimeSpan.FromSeconds(args.Length > 1 ? double.Parse(args[1], CultureInfo.InvariantCulture) : 300);

            using (var cancellation = new CancellationTokenSource(timeLimit))
            {
                Console.CancelKeyPress += (sender, e) =>
                {
                    e.Cancel = true;
                    cancellation.Cancel();
                };
                try
                {
                    FilesClient client = await SignInAsync(args[0], apiKey, username, password, cancellation.Token);
                    await RunAsync(client, cancellation.Token);
                    return 0;
                }
                catch (OperationCanceledException)
                {
                    // Requests Files.com had already received are not undone.
                    Console.Error.WriteLine("Stopped before finishing: cancelled or out of time.");
                    return 3;
                }
                catch (SdkException e)
                {
                    Console.Error.WriteLine($"Files.com request failed ({e.GetType().Name}): {e.Message}");
                    return 1;
                }
            }
        }

        private static async Task RunAsync(FilesClient client, CancellationToken cancellationToken)
        {
            string folder = "files-sdk-example-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            string path = folder + "/hello.txt";

            // Create a folder, then a file from a stream. The upload disposes the stream when it ends.
            await client.Folders.CreateAsync(folder, new Dictionary<string, object> { { "mkdir_parents", true } }, cancellationToken: cancellationToken);
            byte[] contents = Encoding.UTF8.GetBytes("Hello from the Files.com .NET SDK.\n");
            await client.RemoteFiles.UploadFileAsync(path, new MemoryStream(contents), contents.Length, DateTime.UtcNow, cancellationToken: cancellationToken);
            Console.WriteLine($"Uploaded {path}");

            // Update it. An object from a client sends its own requests with that client.
            RemoteFile file = await client.RemoteFiles.FindAsync(path, cancellationToken: cancellationToken);
            await file.UpdateAsync(new Dictionary<string, object> { { "priority_color", "blue" } }, cancellationToken);
            Console.WriteLine($"Updated {file.Path}");

            // Page through the folder, one item per page to show paging. Each page comes from the same client.
            var onePerPage = new Dictionary<string, object> { { "per_page", 1L } };
            foreach (RemoteFile entry in client.Folders.ListFor(folder, onePerPage).ListAutoPaging(cancellationToken))
            {
                Console.WriteLine($"Listed {entry.Path}");
            }

            // Download into a stream you own; the download leaves it open.
            using (var downloaded = new MemoryStream())
            {
                await client.RemoteFiles.DownloadFileAsync(path, downloaded, cancellationToken: cancellationToken);
                Console.WriteLine($"Downloaded {downloaded.Length} bytes: {Encoding.UTF8.GetString(downloaded.ToArray()).Trim()}");
            }

            await client.RemoteFiles.DeleteAsync(folder, new Dictionary<string, object> { { "recursive", true } }, cancellationToken: cancellationToken);
            Console.WriteLine($"Deleted {folder}");

            if (!string.IsNullOrEmpty(client.SessionId))
            {
                await client.Sessions.DeleteAsync(cancellationToken: cancellationToken);
                Console.WriteLine("Signed out");
            }
        }

        // A client that uses the API key, or a session made by signing in with the username and password.
        private static async Task<FilesClient> SignInAsync(string baseUrl, string apiKey, string username, string password, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(apiKey))
            {
                return FilesClient.Create(new FilesConfiguration { BaseUrl = baseUrl, ApiKey = apiKey });
            }

            // Signing in needs no credentials, so this client has none of its own.
            FilesClient site = FilesClient.Create(new FilesConfiguration { BaseUrl = baseUrl });
            var login = new Dictionary<string, object>
            {
                { "username", username },
                { "password", password },
            };
            Session session = await site.Sessions.CreateAsync(login, cancellationToken: cancellationToken);

            // Options give credentials to a single request.
            var asSignedInUser = new Dictionary<string, object> { { "session_id", session.Id } };
            Site details = await site.Sites.GetAsync(null, asSignedInUser, cancellationToken);
            Console.WriteLine($"Signed in to {details.Name}");

            // A client made with the session uses it for every request.
            return FilesClient.Create(new FilesConfiguration { BaseUrl = baseUrl, SessionId = session.Id });
        }
    }
}
