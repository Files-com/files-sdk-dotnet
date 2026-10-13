using FilesCom;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WireMock;

namespace FilesTests
{
    // The upload, copy and move helpers for a remote server, snapshot or child site: each is its base operation, sent
    // with the destination under that one's underscore path, which a destination path cannot leave.
    [TestClass]
    public class ExternalDestinationTest
    {
        [TestMethod]
        [Timeout(60000)]
        [DataRow("RemoteServer", "RemoteServers")]
        [DataRow("Snapshot", "Snapshots")]
        [DataRow("ChildSite", "Sites")]
        public async Task HelpersSendTheBaseOperationToTheUnderscorePath(string helper, string root)
        {
            string folder = Path.Combine(Path.GetTempPath(), "destination-test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            try
            {
                using (var server = new UploadServer())
                {
                    server.Site.On("POST", "/file_actions/copy/*", request => LoopbackSite.Json("{\"status\":\"completed\"}"));
                    server.Site.On("POST", "/file_actions/move/*", request => LoopbackSite.Json("{\"status\":\"completed\"}"));
                    string local = Path.Combine(folder, "local.bin");
                    File.WriteAllBytes(local, new byte[20]);
                    var files = FilesClient.Create(server.Site.Configuration()).RemoteFiles;
                    const string messy = @"in\..\nested//dir/./x.bin";

                    switch (helper)
                    {
                        case "RemoteServer":
                            await files.UploadToRemoteServerAsync(local, 5);
                            await files.UploadToRemoteServerAsync(local, 5, messy);
                            await files.CopyToRemoteServerAsync("a.bin", 5, "copies/a.bin");
                            await files.MoveToRemoteServerAsync("b.bin", 5, "../../moved/b.bin");
                            break;
                        case "Snapshot":
                            await files.UploadToSnapshotAsync(local, 5);
                            await files.UploadToSnapshotAsync(local, 5, messy);
                            await files.CopyToSnapshotAsync("a.bin", 5, "copies/a.bin");
                            await files.MoveToSnapshotAsync("b.bin", 5, "../../moved/b.bin");
                            break;
                        default:
                            await files.UploadToChildSiteAsync(local, 5);
                            await files.UploadToChildSiteAsync(local, 5, messy);
                            await files.CopyToChildSiteAsync("a.bin", 5, "copies/a.bin");
                            await files.MoveToChildSiteAsync("b.bin", 5, "../../moved/b.bin");
                            break;
                    }

                    CollectionAssert.AreEqual(new[] { $"_/{root}/5/local.bin", $"_/{root}/5/in/nested/dir/x.bin" }, server.Finishes.Select(finish => finish.GetProperty("path").GetString()).ToArray(),
                        "An omitted destination is the local file's name; separators, empty, . and .. segments are normalized away.");
                    Assert.AreEqual($"_/{root}/5/copies/a.bin", Body(server, "/file_actions/copy/a.bin").GetProperty("destination").GetString());
                    Assert.AreEqual($"_/{root}/5/moved/b.bin", Body(server, "/file_actions/move/b.bin").GetProperty("destination").GetString(), ".. cannot leave the destination's root.");
                }
            }
            finally
            {
                Directory.Delete(folder, true);
            }
        }

        private static JsonElement Body(UploadServer server, string path)
        {
            return JsonDocument.Parse(server.Site.Requests.Single(request => request.Path.Split('?')[0] == path).Body).RootElement;
        }
    }
}