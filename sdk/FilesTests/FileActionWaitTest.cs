using FilesCom;
using FilesCom.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WireMock;

namespace FilesTests
{
    // Waiting for what Files.com finishes after it answers: a copy or move's migration (FileAction.WaitAsync), and a
    // path that a deletion removes (RemoteFiles.WaitUntilGoneAsync).
    [TestClass]
    public class FileActionWaitTest
    {
        private static readonly TimeSpan Quickly = TimeSpan.FromMilliseconds(10);

        [TestMethod]
        [Timeout(60000)]
        public async Task PendingCopyIsWaitedForUntilItsMigrationCompletes()
        {
            using (var site = new LoopbackSite("key-a", "11"))
            {
                site.On("POST", "/file_actions/copy/*", request => LoopbackSite.Json("{\"status\":\"pending\",\"file_migration_id\":7}"));
                ServeMigration(site, "pending", "processing", "completed");
                FilesClient client = FilesClient.Create(site.Configuration());
                var progress = new List<string>();

                FileAction action = await client.RemoteFiles.CopyAsync("a.bin", new Dictionary<string, object> { { "destination", "b.bin" } });
                FileMigration migration = await action.WaitAsync(Quickly, new SyncProgress<FileMigration>(m => progress.Add($"{m.Status} {m.FilesMoved}")));

                Assert.AreEqual("completed", migration.Status);
                CollectionAssert.AreEqual(new[] { "pending 0", "processing 1", "completed 2" }, progress);
                ReceivedRequest[] checks = site.Requests.Where(request => request.Path.StartsWith("/file_migrations/")).ToArray();
                Assert.AreEqual(3, checks.Length);
                Assert.IsTrue(checks.All(request => request.Path.Split('?')[0] == "/file_migrations/7" && request.ApiKey == "key-a" && request.WorkspaceId == "11"), "Checked with the client the action came from.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("failed", "File migration 7 (move) failed.")]
        [DataRow("cancelled", "File migration 7 (move) was cancelled.")]
        public async Task MigrationThatFailsOrIsCancelledEndsTheWaitWithIt(string status, string said)
        {
            using (var site = new LoopbackSite("key-a", "11"))
            using (var logs = new LogCapture())
            {
                const string failure = "storage https://store.invalid/x?X-Amz-Signature=SYNTHETIC_SECRET\nAuthorization: Bearer SYNTHETIC_TOKEN";
                site.On("POST", "/file_actions/move/*", request => LoopbackSite.Json("{\"status\":\"pending\",\"file_migration_id\":7}"));
                ServeMigrationFailing(site, failure, "processing", status);
                FilesClient client = FilesClient.Create(site.Configuration());
                FileAction action = await client.RemoteFiles.MoveAsync("a.bin", new Dictionary<string, object> { { "destination", "b.bin" } });

                FileMigrationFailedException error = await Assert.ThrowsExactlyAsync<FileMigrationFailedException>(() => action.WaitAsync(Quickly));

                Assert.AreEqual(said + " Its failure message is in Migration.FailureMessage.", error.Message, "The message is bounded: which migration, which operation, how it ended.");
                Assert.AreEqual(failure, error.Migration.FailureMessage, "The server's own text stays on the model.");
                Assert.AreEqual(status, error.Migration.Status);
                Assert.AreSame(action, error.Action);
                Assert.IsFalse(logs.Normal.Contains("SYNTHETIC"), "Ordinary logs do not repeat it.");
                StringAssert.Contains(logs.Debug, "SYNTHETIC_SECRET", "DEBUG keeps it, as asked for.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("{\"status\":\"completed\"}", null, DisplayName = "Complete when answered")]
        [DataRow("{\"status\":\"failed\"}", typeof(FileMigrationFailedException), DisplayName = "Failed when answered")]
        [DataRow("{\"status\":\"pending\"}", typeof(InvalidResponseException), DisplayName = "Pending without a migration")]
        public async Task ActionWithoutAMigrationIsNotPolled(string answer, Type failure)
        {
            using (var site = new LoopbackSite("key-a", "11"))
            {
                site.On("POST", "/file_actions/copy/*", request => LoopbackSite.Json(answer));
                FilesClient client = FilesClient.Create(site.Configuration());
                FileAction action = await client.RemoteFiles.CopyAsync("a.bin", new Dictionary<string, object> { { "destination", "b.bin" } });

                Exception error = null;
                FileMigration migration = null;
                try
                {
                    migration = await action.WaitAsync(Quickly);
                }
                catch (Exception e)
                {
                    error = e;
                }

                Assert.AreEqual(failure, error?.GetType());
                Assert.IsNull(migration);
                Assert.AreEqual(1, site.Requests.Count, "Nothing is checked.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task WaitStopsWhenCancelledOrItsClientCloses()
        {
            using (var site = new LoopbackSite("key-a", "11"))
            {
                site.On("POST", "/file_actions/copy/*", request => LoopbackSite.Json("{\"status\":\"pending\",\"file_migration_id\":7}"));
                ServeMigration(site, Enumerable.Repeat("processing", 100).ToArray());
                FilesClient client = FilesClient.Create(site.Configuration());
                FileAction action = await client.RemoteFiles.CopyAsync("a.bin", new Dictionary<string, object> { { "destination", "b.bin" } });
                Stopwatch waited = Stopwatch.StartNew();

                using (var cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(200)))
                {
                    await Assert.ThrowsAsync<OperationCanceledException>(() => action.WaitAsync(TimeSpan.FromMinutes(10), cancellationToken: cancellation.Token));
                }
                Task<FileMigration> waiting = action.WaitAsync(TimeSpan.FromMinutes(10));
                await Task.Delay(100);
                await client.DisposeAsync();
                await Assert.ThrowsAsync<OperationCanceledException>(() => waiting);
                await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => action.WaitAsync(Quickly));

                Assert.IsTrue(waited.Elapsed < TimeSpan.FromSeconds(10), $"Both waits stopped promptly: {waited.Elapsed}.");
                await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => action.WaitAsync(TimeSpan.Zero));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task WaitUntilGoneReturnsOnceNothingIsAtThePath()
        {
            using (var site = new LoopbackSite("key-a", "11"))
            {
                int checks = 0;
                site.On("GET", "/file_actions/metadata/*", request => Interlocked.Increment(ref checks) < 3
                    ? LoopbackSite.Json("{\"path\":\"folder\",\"type\":\"directory\"}")
                    : LoopbackSite.Json("{\"error\":\"Not Found\",\"http-code\":404,\"type\":\"not-found/file-not-found\"}", status: 404));
                FilesClient client = FilesClient.Create(site.Configuration());

                await client.RemoteFiles.WaitUntilGoneAsync("folder", Quickly);

                Assert.AreEqual(3, checks);
                Assert.IsTrue(site.Requests.All(request => request.Path.Split('?')[0] == "/file_actions/metadata/folder" && request.ApiKey == "key-a"));
                await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => client.RemoteFiles.WaitUntilGoneAsync("folder", TimeSpan.FromMilliseconds(-1)));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task WaitsKeepTheOptionsTheyStartedWith()
        {
            using (var site = new LoopbackSite("key-a", "11"))
            {
                var options = new Dictionary<string, object> { { "api_key", "key-options" } };
                int checks = 0;
                site.On("GET", "/file_actions/metadata/*", request =>
                {
                    if (Interlocked.Increment(ref checks) == 1)
                    {
                        // Changed while the wait runs.
                        options["api_key"] = "key-changed";
                        return LoopbackSite.Json("{\"path\":\"folder\",\"type\":\"directory\"}");
                    }
                    return LoopbackSite.Json("{\"error\":\"Not Found\",\"http-code\":404,\"type\":\"not-found/file-not-found\"}", status: 404);
                });
                site.On("POST", "/file_actions/copy/*", request => LoopbackSite.Json("{\"status\":\"pending\",\"file_migration_id\":7}"));
                int polls = 0;
                FileAction action = null;
                site.On("GET", "/file_migrations/*", request =>
                {
                    if (Interlocked.Increment(ref polls) == 1)
                    {
                        action.SetOption("api_key", "key-changed");
                        return LoopbackSite.Json("{\"id\":7,\"status\":\"processing\"}");
                    }
                    return LoopbackSite.Json("{\"id\":7,\"status\":\"completed\"}");
                });
                FilesClient client = FilesClient.Create(site.Configuration());

                await client.RemoteFiles.WaitUntilGoneAsync("folder", Quickly, options);
                action = await client.RemoteFiles.CopyAsync("a", new Dictionary<string, object> { { "destination", "b" } }, new Dictionary<string, object> { { "api_key", "key-action" } });
                await action.WaitAsync(Quickly);

                CollectionAssert.AreEqual(new[] { "key-options", "key-options" }, site.Requests.Where(r => r.Path.StartsWith("/file_actions/metadata/")).Select(r => r.ApiKey).ToArray());
                CollectionAssert.AreEqual(new[] { "key-action", "key-action" }, site.Requests.Where(r => r.Path.StartsWith("/file_migrations/")).Select(r => r.ApiKey).ToArray());
            }
        }

        // Answers GET /file_migrations/7 with each status in turn, then the last one, with files_moved counting checks.
        private static void ServeMigration(LoopbackSite site, params string[] statuses)
        {
            ServeMigrationFailing(site, "The destination is full", statuses);
        }

        private static void ServeMigrationFailing(LoopbackSite site, string failureMessage, params string[] statuses)
        {
            int checks = 0;
            site.On("GET", "/file_migrations/*", request =>
            {
                int check = Interlocked.Increment(ref checks) - 1;
                string status = statuses[Math.Min(check, statuses.Length - 1)];
                return LoopbackSite.Json($"{{\"id\":7,\"path\":\"a.bin\",\"dest_path\":\"b.bin\",\"operation\":\"move\",\"status\":\"{status}\",\"files_moved\":{check},\"files_total\":2,\"failure_message\":{System.Text.Json.JsonSerializer.Serialize(failureMessage)}}}");
            });
        }

        // Reports on the calling thread, unlike Progress<T>, which posts to the thread pool.
        private sealed class SyncProgress<T> : IProgress<T>
        {
            private readonly Action<T> report;

            internal SyncProgress(Action<T> report)
            {
                this.report = report;
            }

            public void Report(T value)
            {
                report(value);
            }
        }
    }
}