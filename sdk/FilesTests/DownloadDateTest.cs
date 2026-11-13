using FilesCom.Transfers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FilesTests
{
    public partial class DownloadTest
    {
        private const string BaselineDate = "Wed, 21 Oct 2015 07:28:00 GMT";
        private const string NewerDate = "Wed, 21 Oct 2015 07:28:01 GMT";

        [TestMethod]
        [Timeout(60000)]
        public async Task ADateSurvivesTheWholeResponseFallbackAndUrlRenewal()
        {
            using (var server = new DownloadServer(Contents))
            {
                int wholes = 0;
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range != null)
                    {
                        await DownloadServer.SendAsync(connection, 206, Contents, 0, 16384,
                            $"Content-Range: bytes 0-16383/{Contents.Length}\r\nLast-Modified: {BaselineDate}\r\n");
                    }
                    else if (++wholes == 1)
                    {
                        await connection.RespondAsync(403, "<Error><Code>ExpiredToken</Code></Error>");
                    }
                    else
                    {
                        await DownloadServer.SendAsync(connection, 200, Contents, 0, Contents.Length, $"Last-Modified: {BaselineDate}\r\n");
                    }
                    return true;
                };

                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Parts());

                Assert.IsFalse(result.VersionBound);
                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual(3, server.StorageRequests.Count);
                Assert.AreEqual(2, server.ApiRequests.Count);
                foreach (StorageRequest request in server.StorageRequests.Skip(1))
                {
                    Assert.AreEqual(BaselineDate, request.Header("If-Unmodified-Since"));
                    Assert.IsNull(request.IfMatch);
                    Assert.IsNull(request.Range, "A date does not authorize multipart downloads.");
                }
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("stable")]
        [DataRow("precondition")]
        [DataRow("ignored condition")]
        public async Task ADateSurvivesAFirstResponseThatIgnoresRange(string answer)
        {
            using (var server = new DownloadServer(Contents))
            {
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range != null)
                    {
                        // HTTP 200 is declined as a range probe, but its valid date still guards the fallback.
                        await DownloadServer.SendAsync(connection, 200, Contents, 0, Contents.Length,
                            $"Last-Modified: {BaselineDate}\r\n");
                    }
                    else if (answer == "precondition" && request.Header("If-Unmodified-Since") == BaselineDate)
                    {
                        await connection.RespondAsync(412, "");
                    }
                    else
                    {
                        byte[] bytes = answer == "stable" ? Contents : Contents.Reverse().ToArray();
                        string date = answer == "stable" ? BaselineDate : NewerDate;
                        await DownloadServer.SendAsync(connection, 200, bytes, 0, bytes.Length, $"Last-Modified: {date}\r\n");
                    }
                    return true;
                };
                if (answer == "stable")
                {
                    DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Parts());
                    Assert.IsFalse(result.VersionBound);
                    CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                }
                else
                {
                    await Assert.ThrowsExactlyAsync<DownloadSourceChangedException>(() => Client(server, retries: 2).Transfers.DownloadFileAsync("remote.bin", Destination, Parts()));
                    AssertNothingPublished();
                }
                Assert.AreEqual(2, server.StorageRequests.Count);
                Assert.IsNotNull(server.StorageRequests[0].Range);
                Assert.IsNull(server.StorageRequests[1].Range);
                Assert.IsNull(server.StorageRequests[1].IfMatch);
                Assert.AreEqual(BaselineDate, server.StorageRequests[1].Header("If-Unmodified-Since"));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(true)]
        [DataRow(false)]
        public async Task ANewerSameSizeFileIsNeverPublishedEvenWhenTheConditionIsIgnored(bool obeys)
        {
            using (var server = new DownloadServer(Contents))
            {
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range != null)
                    {
                        await DownloadServer.SendAsync(connection, 206, Contents, 0, 16384,
                            $"Content-Range: bytes 0-16383/{Contents.Length}\r\nLast-Modified: {BaselineDate}\r\n");
                    }
                    else if (obeys && request.Header("If-Unmodified-Since") == BaselineDate)
                    {
                        await connection.RespondAsync(412, "");
                    }
                    else
                    {
                        byte[] changed = Contents.Reverse().ToArray();
                        await DownloadServer.SendAsync(connection, 200, changed, 0, changed.Length, $"Last-Modified: {NewerDate}\r\n");
                    }
                    return true;
                };
                await Assert.ThrowsExactlyAsync<DownloadSourceChangedException>(() => Client(server, retries: 2).Transfers.DownloadFileAsync("remote.bin", Destination, Parts()));
                AssertNothingPublished();
                Assert.AreEqual(2, server.StorageRequests.Count, "Source change is terminal, not a retry with a new baseline.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(null)]
        [DataRow("not an HTTP date")]
        [DataRow(BaselineDate + " garbage")]
        [DataRow("Wed, 32 Oct 2015 07:28:00 GMT")]
        [DataRow(BaselineDate + "\r\nLast-Modified: " + BaselineDate)]
        public async Task AbsentInvalidOrDuplicateDatesKeepWholeResponseCompatibility(string date)
        {
            using (var server = new DownloadServer(Contents))
            {
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range != null)
                    {
                        string modified = date == null ? "" : $"Last-Modified: {date}\r\n";
                        await DownloadServer.SendAsync(connection, 206, Contents, 0, 16384,
                            $"Content-Range: bytes 0-16383/{Contents.Length}\r\n{modified}");
                    }
                    else
                    {
                        await DownloadServer.SendAsync(connection, 200, Contents, 0, Contents.Length, $"Last-Modified: {BaselineDate}\r\n");
                    }
                    return true;
                };
                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Parts());
                Assert.IsFalse(result.VersionBound);
                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.IsNull(server.StorageRequests[1].Header("If-Unmodified-Since"));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task AWholeResponseRetryKeepsTheDateFromItsInterruptedBody()
        {
            using (var server = new DownloadServer(Contents) { Bound = false })
            {
                int requests = 0;
                server.Intercept = async (request, connection) =>
                {
                    if (++requests == 1)
                    {
                        await connection.SendAsync($"HTTP/1.1 200 OK\r\nContent-Length: {Contents.Length}\r\nLast-Modified: {BaselineDate}\r\n\r\n");
                        await connection.SendAsync(Contents.Take(100).ToArray());
                        connection.Dispose();
                    }
                    else if (request.Header("If-Unmodified-Since") == BaselineDate)
                    {
                        await connection.RespondAsync(412, "");
                    }
                    else
                    {
                        await DownloadServer.SendAsync(connection, 200, Contents, 0, Contents.Length, "");
                    }
                    return true;
                };
                await Assert.ThrowsExactlyAsync<DownloadSourceChangedException>(() => Client(server, retries: 2).Transfers.DownloadFileAsync("remote.bin", Destination, Parts()));
                AssertNothingPublished();
                Assert.AreEqual(2, server.StorageRequests.Count);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(false)]
        [DataRow(true)]
        public async Task StreamDownloadsKeepTheirDateWithoutReplayingWrittenBytes(bool legacy)
        {
            using (var server = new DownloadServer(Contents) { Bound = false })
            using (var destination = new MemoryStream())
            {
                int requests = 0;
                server.Intercept = async (request, connection) =>
                {
                    if (++requests == 1)
                    {
                        // A successful head whose body fails before any byte was emitted may be retried.
                        await connection.SendAsync($"HTTP/1.1 200 OK\r\nContent-Length: {Contents.Length}\r\nLast-Modified: {BaselineDate}\r\n\r\n");
                        connection.Dispose();
                    }
                    else if (request.Header("If-Unmodified-Since") == BaselineDate)
                    {
                        await connection.RespondAsync(412, "");
                    }
                    else
                    {
                        await DownloadServer.SendAsync(connection, 200, Contents, 0, Contents.Length, "");
                    }
                    return true;
                };
                var client = Client(server, retries: 2);
                DownloadSourceChangedException error = await Assert.ThrowsExactlyAsync<DownloadSourceChangedException>(() => legacy
                    ? client.RemoteFiles.DownloadFileAsync("remote.bin", destination)
                    : client.Transfers.DownloadStreamAsync("remote.bin", destination, Parts()));
                Assert.AreEqual("remote.bin", error.Path);
                Assert.AreEqual(0, destination.Length);
                Assert.IsTrue(destination.CanWrite);
                Assert.AreEqual(2, server.StorageRequests.Count);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task AnEarlierRetryDateDoesNotReplaceTheOriginalBaseline()
        {
            using (var server = new DownloadServer(Contents) { Bound = false })
            {
                int requests = 0;
                server.Intercept = async (request, connection) =>
                {
                    if (++requests <= 2)
                    {
                        string date = requests == 1 ? BaselineDate : "Wed, 21 Oct 2015 07:27:59 GMT";
                        await connection.SendAsync($"HTTP/1.1 200 OK\r\nContent-Length: {Contents.Length}\r\nLast-Modified: {date}\r\n\r\n");
                        await connection.SendAsync(Contents.Take(100).ToArray());
                        connection.Dispose();
                    }
                    else
                    {
                        await DownloadServer.SendAsync(connection, 200, Contents, 0, Contents.Length, $"Last-Modified: {BaselineDate}\r\n");
                    }
                    return true;
                };
                await Client(server, retries: 2).Transfers.DownloadFileAsync("remote.bin", Destination, Parts());
                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                Assert.AreEqual(3, server.StorageRequests.Count);
                foreach (StorageRequest request in server.StorageRequests.Skip(1))
                {
                    Assert.AreEqual(BaselineDate, request.Header("If-Unmodified-Since"));
                }
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(false, null)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        public async Task DateChecksCanBeDisabledForAClientAndOverriddenForOneDownload(bool clientSetting, bool? overrideSetting)
        {
            using (var server = new DownloadServer(Contents))
            {
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range != null)
                    {
                        await DownloadServer.SendAsync(connection, 206, Contents, 0, 16384,
                            $"Content-Range: bytes 0-16383/{Contents.Length}\r\nLast-Modified: {BaselineDate}\r\n");
                    }
                    else
                    {
                        byte[] changed = Contents.Reverse().ToArray();
                        await DownloadServer.SendAsync(connection, 200, changed, 0, changed.Length, $"Last-Modified: {NewerDate}\r\n");
                    }
                    return true;
                };
                var configuration = server.Configuration();
                configuration.CheckDownloadModificationTime = clientSetting;
                var client = server.Client(configuration);
                var options = Parts();
                options.CheckDownloadModificationTime = overrideSetting;
                bool enabled = overrideSetting ?? clientSetting;
                if (enabled)
                {
                    await Assert.ThrowsExactlyAsync<DownloadSourceChangedException>(() => client.Transfers.DownloadFileAsync("remote.bin", Destination, options));
                    AssertNothingPublished();
                }
                else
                {
                    await client.Transfers.DownloadFileAsync("remote.bin", Destination, options);
                    CollectionAssert.AreEqual(Contents.Reverse().ToArray(), File.ReadAllBytes(Destination));
                }
                Assert.AreEqual(enabled ? BaselineDate : null, server.StorageRequests[1].Header("If-Unmodified-Since"));
                Assert.IsTrue(server.ApiRequests.All(request => !request.Contains("check_download_modification_time")));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(false, null)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        public async Task LegacyDateChecksKeepTheConfigurationAndOptionsAtOperationStart(bool clientSetting, bool? overrideSetting)
        {
            using (var server = new DownloadServer(Contents) { Bound = false })
            using (var destination = new MemoryStream())
            {
                int requests = 0;
                server.Intercept = async (request, connection) =>
                {
                    if (++requests == 1)
                    {
                        await connection.SendAsync($"HTTP/1.1 200 OK\r\nContent-Length: {Contents.Length}\r\nLast-Modified: {BaselineDate}\r\n\r\n");
                        connection.Dispose();
                    }
                    else
                    {
                        await DownloadServer.SendAsync(connection, 200, Contents, 0, Contents.Length, $"Last-Modified: {NewerDate}\r\n");
                    }
                    return true;
                };
                var configuration = server.Configuration(maxNetworkRetries: 2);
                configuration.CheckDownloadModificationTime = clientSetting;
                var options = new System.Collections.Generic.Dictionary<string, object>();
                if (overrideSetting.HasValue)
                {
                    options["check_download_modification_time"] = overrideSetting.Value;
                }
                server.OnApiRequest = request =>
                {
                    configuration.CheckDownloadModificationTime = !clientSetting;
                    options["check_download_modification_time"] = !(overrideSetting ?? clientSetting);
                };
                var client = new FilesCom.FilesClient(configuration, server.Transport);
                bool enabled = overrideSetting ?? clientSetting;
                if (enabled)
                {
                    await Assert.ThrowsExactlyAsync<DownloadSourceChangedException>(() => client.RemoteFiles.DownloadFileAsync("remote.bin", destination, options));
                    Assert.AreEqual(0, destination.Length);
                }
                else
                {
                    await client.RemoteFiles.DownloadFileAsync("remote.bin", destination, options);
                    CollectionAssert.AreEqual(Contents, destination.ToArray());
                }
                Assert.AreEqual(enabled ? BaselineDate : null, server.StorageRequests[1].Header("If-Unmodified-Since"));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task DirectoryDateOverridesAreKeptForEveryFile()
        {
            using (var server = new DownloadServer(Contents))
            {
                server.AddFile("folder/a", Contents);
                server.AddFile("folder/b", Contents);
                server.Intercept = async (request, connection) =>
                {
                    if (request.Range != null)
                    {
                        await DownloadServer.SendAsync(connection, 206, Contents, 0, 16384,
                            $"Content-Range: bytes 0-16383/{Contents.Length}\r\nLast-Modified: {BaselineDate}\r\n");
                    }
                    else
                    {
                        byte[] changed = Contents.Reverse().ToArray();
                        await DownloadServer.SendAsync(connection, 200, changed, 0, changed.Length, $"Last-Modified: {NewerDate}\r\n");
                    }
                    return true;
                };
                var fileOptions = Parts();
                fileOptions.CheckDownloadModificationTime = false;
                string destination = Path.Combine(folder, "tree");
                Directory.CreateDirectory(destination);
                Task<DirectoryTransferResult> transfer = Client(server).Transfers.DownloadDirectoryAsync("folder", destination,
                    new DirectoryDownloadOptions { MaxConcurrentFiles = 1, FileOptions = fileOptions });
                fileOptions.CheckDownloadModificationTime = true;
                await transfer;
                CollectionAssert.AreEqual(Contents.Reverse().ToArray(), File.ReadAllBytes(Path.Combine(destination, "a")));
                CollectionAssert.AreEqual(Contents.Reverse().ToArray(), File.ReadAllBytes(Path.Combine(destination, "b")));
                Assert.IsTrue(server.StorageRequests.All(request => request.Header("If-Unmodified-Since") == null));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task DisablingDateChecksKeepsStrongVersionProtection()
        {
            using (var server = new DownloadServer(Contents))
            {
                server.Intercept = async (request, connection) =>
                {
                    var range = request.Range.Value;
                    long last = Math.Min(range.Last, Contents.Length - 1);
                    string etag = range.First == 0 ? server.ETag : "\"replacement\"";
                    await DownloadServer.SendAsync(connection, 206, Contents, range.First, last - range.First + 1,
                        $"Content-Range: bytes {range.First}-{last}/{Contents.Length}\r\nETag: {etag}\r\nLast-Modified: {NewerDate}\r\n");
                    return true;
                };
                var configuration = server.Configuration();
                configuration.CheckDownloadModificationTime = false;
                await Assert.ThrowsExactlyAsync<DownloadSourceChangedException>(() => server.Client(configuration).Transfers.DownloadFileAsync("remote.bin", Destination, Parts(concurrency: 1)));
                AssertNothingPublished();
                Assert.IsTrue(server.StorageRequests.Skip(1).All(request => request.IfMatch == server.ETag));
                Assert.IsTrue(server.StorageRequests.All(request => request.Header("If-Unmodified-Since") == null));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task StrongEntityTagsTakePrecedenceOverDates()
        {
            using (var server = new DownloadServer(Contents))
            {
                server.Intercept = async (request, connection) =>
                {
                    var range = request.Range.Value;
                    long last = Math.Min(range.Last, Contents.Length - 1);
                    string date = range.First == 0 ? BaselineDate : NewerDate;
                    await DownloadServer.SendAsync(connection, 206, Contents, range.First, last - range.First + 1,
                        $"Content-Range: bytes {range.First}-{last}/{Contents.Length}\r\nETag: {server.ETag}\r\nLast-Modified: {date}\r\n");
                    return true;
                };
                DownloadResult result = await Client(server).Transfers.DownloadFileAsync("remote.bin", Destination, Parts());
                Assert.IsTrue(result.VersionBound);
                CollectionAssert.AreEqual(Contents, File.ReadAllBytes(Destination));
                foreach (StorageRequest request in server.StorageRequests.Skip(1))
                {
                    Assert.AreEqual(server.ETag, request.IfMatch);
                    Assert.IsNull(request.Header("If-Unmodified-Since"));
                }
            }
        }
    }
}