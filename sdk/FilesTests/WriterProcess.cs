using FilesCom;
using FilesCom.Transfers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FilesTests
{
    // FilesTests.dll run as a program (dotnet exec FilesTests.dll ...) rather than by the test host: the processes a test,
    // or a check run as real operating system users, starts to write a download and then kills, as a crash or a killed
    // application stops one. Run with other arguments, it does nothing.
    //
    //   download-writer BASE_URL API_KEY PORT DESTINATION CHECKPOINT PART_SIZE
    //
    // downloads remote.bin from the stand-in site at BASE_URL with that API key, its storage requests sent to PORT on
    // this computer as a DownloadServer's client sends them, one part at a time with the checkpoint.
    //
    //   principal-owner FOLDER SIGNALS [WAIT_SECONDS [WRITER]]
    //
    // as the user who owns FOLDER, an ordinary directory (not a link to one; otherwise nothing is started): serves
    // remote.bin from a DownloadServer in this process, starts download-writer (of this assembly, or of the assembly
    // WRITER, such as one built with an earlier SDK, which takes the same arguments) into FOLDER (file.bin,
    // download.checkpoint) and, once its checkpoint records three parts and it asks for the fourth, kills it through the
    // handle it started it with. It then writes SIGNALS/killed and waits, at most WAIT_SECONDS (600 by default), for
    // SIGNALS/continue. Only if that signal came and FOLDER and everything in it are as they were (type, permissions,
    // owner, group, links, size, identity, what a link points to and, for files, bytes; links are not followed) does it
    // continue the download; otherwise it stops without touching anything. It prints one JSON line of what it saw, the
    // checkpoint and FOLDER as the continuation's first storage request found them included, and exits 0 only if the
    // continuation kept the three recorded parts, fetched only the rest and published exact bytes.
    //
    //   principal-other CHECKPOINT DESTINATION
    //
    // as another user: tries to continue the download CHECKPOINT records into DESTINATION, against a DownloadServer of
    // its own that records every request. It prints one JSON line and exits 0 only if the download failed before any
    // request reached that server.
    //
    //   principal-publish DESTINATION POLICY
    //
    // downloads remote.bin from a DownloadServer of its own to DESTINATION, in parts and without a checkpoint, as the
    // publication policy POLICY (ReplaceExisting, CreateNew or ReplaceRegularFile) says. It prints one JSON line: how the
    // download ended, every request, and DESTINATION's directory as Snapshot sees it, without following links, before
    // and after; it exits 0 whenever the download ran, however it ended, for the caller to judge.
    public static class WriterProcess
    {
        // remote.bin's bytes for the principal modes: 100 KiB, seven 16 KiB parts.
        private static readonly byte[] Contents = Enumerable.Range(0, 100 * 1024).Select(i => (byte)(i * 31 + 7)).ToArray();
        private const int PartSize = 16 * 1024;
        private static readonly TimeSpan Deadline = TimeSpan.FromSeconds(60);

        public static int Main(string[] args)
        {
            if (args.Length == 7 && args[0] == "download-writer")
            {
                return Write(args);
            }
            if (args.Length >= 3 && args.Length <= 5 && args[0] == "principal-owner")
            {
                TimeSpan wait = TimeSpan.FromSeconds(args.Length >= 4 ? int.Parse(args[3]) : 600);
                string writer = args.Length == 5 ? args[4] : typeof(WriterProcess).Assembly.Location;
                return OwnerAsync(args[1], args[2], wait, writer).GetAwaiter().GetResult();
            }
            if (args.Length == 3 && args[0] == "principal-other")
            {
                return OtherAsync(args[1], args[2]).GetAwaiter().GetResult();
            }
            if (args.Length == 3 && args[0] == "principal-publish")
            {
                return PublishAsync(args[1], (DownloadPublicationPolicy)Enum.Parse(typeof(DownloadPublicationPolicy), args[2])).GetAwaiter().GetResult();
            }
            return 0;
        }

        private static int Write(string[] args)
        {
            int port = int.Parse(args[3]);
            FilesConfiguration configuration = new FilesConfiguration
            {
                BaseUrl = args[1],
                ApiKey = args[2],
                MaxNetworkRetries = 0,
                InitialNetworkRequestDelay = 0,
                MaxNetworkRetryDelay = 0,
            };
            HttpTransport transport = HttpTransport.FromHandlerFactory(() => new ToLoopback(port) { InnerHandler = new SocketsHttpHandler { UseProxy = false } });
            using (FilesClient client = FilesClient.Create(configuration, transport))
            {
                DownloadOptions options = new DownloadOptions { PartSize = long.Parse(args[6]), MaxConcurrency = 1, CheckpointPath = args[5] };
                client.Transfers.DownloadFileAsync(DownloadServer.DefaultFile, args[4], options).GetAwaiter().GetResult();
            }
            return 0;
        }

        private static async Task<int> OwnerAsync(string folder, string signals, TimeSpan wait, string writerAssembly)
        {
            var report = new Dictionary<string, object> { ["mode"] = "principal-owner", ["folder"] = folder, ["writer_assembly"] = writerAssembly };
            try
            {
                return await OwnAsync(folder, signals, wait, writerAssembly, report);
            }
            catch (Exception e)
            {
                report["error"] = Chain(e);
                return Finish(report, false);
            }
        }

        private static async Task<int> OwnAsync(string folder, string signals, TimeSpan wait, string writerAssembly, Dictionary<string, object> report)
        {
            string destination = Path.Combine(folder, "file.bin");
            string checkpoint = Path.Combine(folder, "download.checkpoint");
            report["folder_at_start"] = Describe(folder, out bool ordinary);
            if (!ordinary)
            {
                // Nothing is started or written: Snapshot lists only an ordinary directory, so it could not tell what changed.
                return Finish(report, false);
            }
            using (var server = new DownloadServer(Contents))
            {
                // As ADownloadWhoseProcessIsKilledContinuesFromItsCheckpoint: the third part is answered after the two
                // seconds between checkpoint records, so three parts are recorded when the fourth is asked for and held.
                var waiting = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                int thirdAnswered = 0;
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range?.First == 2 * PartSize && Interlocked.Exchange(ref thirdAnswered, 1) == 0)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(2.5));
                        return false;
                    }
                    if (request.Range?.First == 3 * PartSize && waiting.TrySetResult(true))
                    {
                        await connection.HoldAsync();
                        return true;
                    }
                    return false;
                };
                FilesConfiguration site = server.Configuration();
                var start = new ProcessStartInfo(Environment.ProcessPath) { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true };
                foreach (string argument in new[] { "exec", writerAssembly, "download-writer", site.BaseUrl, site.ApiKey, server.Port.ToString(), destination, checkpoint, PartSize.ToString() })
                {
                    start.ArgumentList.Add(argument);
                }
                Process writer = Process.Start(start);
                Task<string> output = writer.StandardOutput.ReadToEndAsync();
                Task<string> errors = writer.StandardError.ReadToEndAsync();
                try
                {
                    report["writer_pid"] = writer.Id;
                    Task first = await Task.WhenAny(waiting.Task, writer.WaitForExitAsync(), Task.Delay(Deadline));
                    report["writer_reached_fourth_part"] = first == waiting.Task;
                }
                finally
                {
                    // Whatever happened, the writer this process started is ended through that handle before going on.
                    report["writer_end"] = await EndAsync(writer, output, errors);
                }
                if (!(bool)report["writer_reached_fourth_part"] || !((string)report["writer_end"]).StartsWith("Ended", StringComparison.Ordinal))
                {
                    return Finish(report, false);
                }

                Dictionary<string, string> left = Snapshot(folder);
                report["left_after_kill"] = left;
                int requestsAtKill = server.ApiRequests.Count + server.StorageRequests.Count;
                File.WriteAllText(Path.Combine(signals, "killed"), JsonSerializer.Serialize(left));
                DateTime until = DateTime.UtcNow + wait;
                while (!File.Exists(Path.Combine(signals, "continue")) && DateTime.UtcNow < until)
                {
                    await Task.Delay(200);
                }
                report["continue_signalled"] = File.Exists(Path.Combine(signals, "continue"));
                Dictionary<string, string> before = Snapshot(folder);
                List<string> changed = before.Keys.Union(left.Keys).Where(key => !before.TryGetValue(key, out string now) || !left.TryGetValue(key, out string then) || now != then)
                    .OrderBy(key => key, StringComparer.Ordinal).ToList();
                report["changed_until_continue"] = changed;
                if (!(bool)report["continue_signalled"] || changed.Count > 0)
                {
                    // Nothing is continued, removed or changed: the folder stays as the other user left it.
                    report["continued"] = false;
                    report["requests_after_kill"] = server.ApiRequests.Count + server.StorageRequests.Count - requestsAtKill;
                    return Finish(report, false);
                }

                report["continued"] = true;
                int already = server.StorageRequests.Count;
                // What the continuation's first storage request finds: by then it has chosen how to continue, and a stage it
                // could not show to be its own has been copied, and the checkpoint moved to the copy.
                int found = 0;
                server.Intercept = (request, connection) =>
                {
                    if (Interlocked.Exchange(ref found, 1) == 0)
                    {
                        report["checkpoint_at_first_request"] = File.Exists(checkpoint) ? File.ReadAllText(checkpoint) : null;
                        try
                        {
                            report["folder_at_first_request"] = Snapshot(folder, observing: true);
                        }
                        catch (Exception e)
                        {
                            // Only an observation: it must not stop the request it was made from.
                            report["folder_at_first_request"] = Chain(e);
                        }
                    }
                    return Task.FromResult(false);
                };
                var options = new DownloadOptions { PartSize = PartSize, MaxConcurrency = 1, CheckpointPath = checkpoint };
                DownloadResult result = await server.Client().Transfers.DownloadFileAsync(DownloadServer.DefaultFile, destination, options);
                List<string> continued = server.StorageRequests.Skip(already).Select(request => request.ToString()).ToList();
                List<string> rest = new[] { "GET 0-0" }.Concat(Enumerable.Range(3, 4).Select(i => $"GET {i * PartSize}-{Math.Min(i * PartSize + PartSize - 1, Contents.Length - 1)}")).ToList();
                report["resumed_bytes"] = result.ResumedBytes;
                report["continued_requests"] = continued;
                report["exact_bytes"] = File.ReadAllBytes(destination).SequenceEqual(Contents);
                report["checkpoint_removed"] = !File.Exists(checkpoint);
                bool passed = result.ResumedBytes == 3 * PartSize
                    && continued.Count == rest.Count && continued[0] == rest[0] && continued.Skip(1).OrderBy(r => r).SequenceEqual(rest.Skip(1).OrderBy(r => r))
                    && (bool)report["exact_bytes"] && (bool)report["checkpoint_removed"];
                return Finish(report, passed);
            }
        }

        // Ends a writer process this one started, through the handle Process.Start returned and no other: kills it if it is
        // still running, waits at most Deadline for it to exit and its output to end, and disposes the handle. Returns
        // what happened, starting "Ended" if the process exited, and never throws.
        private static async Task<string> EndAsync(Process writer, Task<string> output, Task<string> errors)
        {
            int id = -1;
            try
            {
                id = writer.Id;
                bool killed = false;
                try
                {
                    if (!writer.HasExited)
                    {
                        writer.Kill();
                        killed = true;
                    }
                }
                catch (InvalidOperationException)
                {
                    // It exited in the meantime.
                }
                Task settled = Task.WhenAll(writer.WaitForExitAsync(), output, errors);
                if (await Task.WhenAny(settled, Task.Delay(Deadline)) != settled)
                {
                    return $"Writer process {id} did not exit and close its output within {Deadline.TotalSeconds} s.";
                }
                return $"Ended writer process {id}{(killed ? ", killed," : "")} with exit code {writer.ExitCode}. Output: {output.Result} {errors.Result}";
            }
            catch (Exception e)
            {
                return $"Ending writer process {id} failed: {string.Join(" <- ", Chain(e))}";
            }
            finally
            {
                writer.Dispose();
            }
        }

        private static async Task<int> OtherAsync(string checkpoint, string destination)
        {
            var report = new Dictionary<string, object> { ["mode"] = "principal-other", ["checkpoint"] = checkpoint, ["destination"] = destination };
            using (var server = new DownloadServer(Contents))
            {
                try
                {
                    var options = new DownloadOptions { PartSize = PartSize, MaxConcurrency = 1, CheckpointPath = checkpoint };
                    await server.Client().Transfers.DownloadFileAsync(DownloadServer.DefaultFile, destination, options);
                    report["outcome"] = "completed";
                }
                catch (Exception e)
                {
                    report["outcome"] = "failed";
                    report["exception"] = Chain(e);
                }
                report["api_requests"] = server.ApiRequests.ToList();
                report["storage_requests"] = server.StorageRequests.Select(request => request.ToString()).ToList();
                bool refused = (string)report["outcome"] == "failed" && server.ApiRequests.Count == 0 && server.StorageRequests.Count == 0;
                return Finish(report, refused);
            }
        }

        private static async Task<int> PublishAsync(string destination, DownloadPublicationPolicy policy)
        {
            string folder = Path.GetDirectoryName(destination);
            var report = new Dictionary<string, object> { ["mode"] = "principal-publish", ["destination"] = destination, ["policy"] = policy.ToString(), ["folder_before"] = Snapshot(folder, observing: true) };
            using (var server = new DownloadServer(Contents))
            {
                try
                {
                    var options = new DownloadOptions { PartSize = PartSize, MaxConcurrency = 1, PublicationPolicy = policy };
                    await server.Client().Transfers.DownloadFileAsync(DownloadServer.DefaultFile, destination, options);
                    report["outcome"] = "completed";
                }
                catch (Exception e)
                {
                    report["outcome"] = "failed";
                    report["exception"] = Chain(e);
                }
                report["api_requests"] = server.ApiRequests.ToList();
                report["storage_requests"] = server.StorageRequests.Select(request => request.ToString()).ToList();
            }
            report["folder_after"] = Snapshot(folder, observing: true);
            return Finish(report, true);
        }

        // folder and every entry under it, by its path below folder (folder itself as "."), without following links: its
        // type, permissions, owner, group, link count, size and identity (device, inode and, where recorded, birth time),
        // what a link points to, and a regular file's SHA-256. Only ordinary directories are listed: a link, to a directory
        // or not, is recorded as a link, and nothing it leads to is listed or read. Paths are resolved by name, so this
        // relies on folder being the owner's alone, as principal-owner's FOLDER is, and on its not changing while read.
        // Observing, a file that cannot be read, such as one another process holds open without sharing it on Windows, is
        // described as unreadable; otherwise that fails the snapshot, so the comparisons principal-owner relies on fail.
        private static Dictionary<string, string> Snapshot(string folder, bool observing = false)
        {
            var entries = new Dictionary<string, string>(StringComparer.Ordinal) { ["."] = Describe(folder, out bool ordinary, observing) };
            var directories = new Stack<string>();
            if (ordinary)
            {
                directories.Push(folder);
            }
            var options = new EnumerationOptions { RecurseSubdirectories = false, AttributesToSkip = 0, IgnoreInaccessible = false };
            while (directories.Count > 0)
            {
                foreach (string path in Directory.EnumerateFileSystemEntries(directories.Pop(), "*", options))
                {
                    entries[Path.GetRelativePath(folder, path)] = Describe(path, out bool directory, observing);
                    if (directory)
                    {
                        directories.Push(path);
                    }
                }
            }
            return entries;
        }

        // What Snapshot records of path; directory is whether it is an ordinary directory, not a link to one or a junction.
        private static string Describe(string path, out bool directory, bool observing = false)
        {
            if (!OperatingSystem.IsLinux() && !OperatingSystem.IsMacOS())
            {
                FileSystemInfo info = File.Exists(path) ? new FileInfo(path) : new DirectoryInfo(path);
                directory = info is DirectoryInfo && info.Exists && !info.Attributes.HasFlag(FileAttributes.ReparsePoint);
                return $"{info.Attributes} {(info is FileInfo file ? file.Length + " " + Hash(path, observing) : "")}";
            }
            Native.FileStatus status = Native.TryStatAt(Native.Flags.CurrentDirectory, path);
            directory = status != null && (status.Mode & 0xF000) == 0x4000;
            if (status == null)
            {
                return "absent";
            }
            int type = status.Mode & 0xF000;
            string kind = type == 0x8000 ? "file" : type == 0x4000 ? "directory" : type == 0xA000 ? "link" : $"type{type:x}";
            string described = $"{kind} mode {Convert.ToString(status.Mode & 0xFFF, 8)} uid {status.Uid} gid {status.Gid} links {status.Links} size {status.Size} identity {FileIdentity.Of(status)}";
            if (type == 0xA000)
            {
                described += " to " + new FileInfo(path).LinkTarget;
            }
            if (type == 0x8000)
            {
                described += " sha256 " + Hash(path, observing);
            }
            return described;
        }

        private static string Hash(string path, bool observing)
        {
            try
            {
                return Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();
            }
            catch (Exception e) when (observing && (e is IOException || e is UnauthorizedAccessException))
            {
                return $"unreadable ({e.GetType().Name})";
            }
        }

        private static List<string> Chain(Exception e)
        {
            var chain = new List<string>();
            for (Exception inner = e; inner != null; inner = inner.InnerException)
            {
                chain.Add(inner.GetType().FullName + ": " + inner.Message);
            }
            return chain;
        }

        private static int Finish(Dictionary<string, object> report, bool passed)
        {
            report["passed"] = passed;
            Console.WriteLine(JsonSerializer.Serialize(report));
            return passed ? 0 : 1;
        }
    }
}