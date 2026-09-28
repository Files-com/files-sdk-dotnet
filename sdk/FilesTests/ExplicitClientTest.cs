using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FilesCom;
using FilesCom.Models;
using FilesCom.Util;

namespace FilesTests
{
    // Clients made with FilesClient.Create, and the objects their operations return, send every request to their
    // own site with their own credentials, whatever other clients exist or become the default.
    [TestClass]
    public class ExplicitClientTest
    {
        private static readonly DateTime MTime = new DateTime(2026, 9, 24, 0, 0, 0, DateTimeKind.Utc);
        private LoopbackSite siteA;
        private LoopbackSite siteB;

        [TestInitialize]
        public void StartSites()
        {
            siteA = new LoopbackSite("key-a", "11");
            siteB = new LoopbackSite("key-b", "22");
            siteA.ServeUsers();
            siteB.ServeUsers();
        }

        [TestCleanup]
        public void StopSites()
        {
            siteA.Dispose();
            siteB.Dispose();
        }

        [TestMethod]
        public async Task ClientsForDifferentSitesWorkConcurrently()
        {
            FilesClient clientA = FilesClient.Create(siteA.Configuration());
            FilesClient clientB = FilesClient.Create(siteB.Configuration());

            await Task.WhenAll(Enumerable.Range(0, 20).Select(async i =>
            {
                User user = await (i % 2 == 0 ? clientA : clientB).Users.FindAsync(1);
                await user.UpdateAsync(Changes());
            }));

            foreach (LoopbackSite site in new[] { siteA, siteB })
            {
                Assert.AreEqual(20, site.Requests.Count);
                foreach (ReceivedRequest request in site.Requests)
                {
                    Assert.AreEqual(site.ApiKey + " " + site.WorkspaceId, request.ApiKey + " " + request.WorkspaceId, request.ToString());
                }
            }
        }

        [TestMethod]
        public async Task CreateKeepsItsOwnCopyOfTheConfigurationAndLeavesTheDefaultAlone()
        {
            FilesClient defaultClient = new FilesClient(siteB.Configuration());
            FilesConfiguration configuration = siteA.Configuration();
            configuration.SessionId = "session-a";
            configuration.Language = "de";
            configuration.ReadTimeout = 45;
            FilesClient client = FilesClient.Create(configuration);

            configuration.BaseUrl = siteB.Url;
            configuration.ApiKey = "changed";
            configuration.SessionId = "changed";
            configuration.WorkspaceId = "22";
            configuration.Language = "fr";
            configuration.ReadTimeout = 1;
            await client.Users.FindAsync(1);

            Assert.AreSame(defaultClient, FilesClient.Instance);
            CollectionAssert.AreEqual(
                new object[] { siteA.Url, "key-a", "session-a", "11", "de", 45 },
                new object[] { client.BaseUrl, client.ApiKey, client.SessionId, client.WorkspaceId, client.Language, client.ReadTimeout });
            siteA.AssertReceived("GET /users/1?id=1 session=session-a workspace=11");
            Assert.AreEqual("de", siteA.Requests[0].Language);
            siteB.AssertReceived();
            Assert.AreEqual("https://app.files.com/", FilesClient.Create().BaseUrl);
        }

        [TestMethod]
        public async Task ConstructorClientsCanUseTheirOwnOperationsAfterTheDefaultChanges()
        {
            FilesClient legacyClient = new FilesClient(siteA.Configuration());
            new FilesClient(siteB.Configuration());

            await legacyClient.Users.FindAsync(1);

            siteA.AssertReceived("GET /users/1?id=1 key=key-a workspace=11");
            siteB.AssertReceived();
        }

        [TestMethod]
        public async Task NewModelsSaveAndUpdateWithTheirClient()
        {
            foreach (LoopbackSite site in new[] { siteA, siteB })
            {
                site.On("POST", "/workspaces", request => LoopbackSite.Json("{\"id\":7,\"name\":\"Projects\"}"));
                site.On("PATCH", "/workspaces/7", request => LoopbackSite.Json("{\"id\":7,\"name\":\"Renamed\"}"));
            }
            FilesClient clientA = FilesClient.Create(siteA.Configuration());
            new FilesClient(siteB.Configuration());

            Workspace workspace = clientA.Workspaces.New(new Dictionary<string, object> { { "name", "Projects" } });
            await workspace.SaveAsync();
            workspace.Name = "Renamed";
            await workspace.SaveAsync();

            Assert.AreEqual(7, workspace.Id);
            siteA.AssertReceived("POST /workspaces key=key-a workspace=11", "PATCH /workspaces/7 key=key-a workspace=11");
            siteB.AssertReceived();
            // Which client a model belongs to is never part of its data.
            foreach (string json in new[] { siteA.Requests[0].Body, siteA.Requests[1].Body, JsonSerializer.Serialize(workspace, JsonUtil.Options) })
            {
                using (JsonDocument document = JsonDocument.Parse(json))
                {
                    CollectionAssert.AreEquivalent(new[] { "id", "name" }, document.RootElement.EnumerateObject().Select(property => property.Name).ToArray(), json);
                }
            }
        }

        [TestMethod]
        [Timeout(30000)]
        public async Task MemberOperationKeepsTheOptionsItStartedWith()
        {
            var arrived = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var respond = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            int updates = 0;
            siteA.On("PATCH", "/users/1", async request =>
            {
                if (Interlocked.Increment(ref updates) == 1)
                {
                    arrived.TrySetResult(true);
                    await respond.Task;
                }
                return LoopbackSite.Json("{\"id\":1,\"username\":\"changed\"}");
            }, priority: 1);
            FilesConfiguration configuration = siteA.Configuration();
            configuration.ApiKey = "configured-key";
            configuration.WorkspaceId = "99";
            User user = FilesClient.Create(configuration).Users.New(
                new Dictionary<string, object> { { "id", 1L } },
                new Dictionary<string, object> { { "api_key", "key-a" }, { "workspace_id", "11" } });

            Task<User> updating = user.UpdateAsync(Changes());
            await arrived.Task;
            user.SetOption("api_key", "key-b");
            user.SetOption("workspace_id", "22");
            respond.SetResult(true);
            User updated = await updating;
            await updated.UpdateAsync(Changes());
            await user.UpdateAsync(Changes());

            // The first update and the User it returned use the options it started with; the original User's next
            // update uses the options changed meanwhile.
            siteA.AssertReceived(
                "PATCH /users/1 key=key-a workspace=11",
                "PATCH /users/1 key=key-a workspace=11",
                "PATCH /users/1 key=key-b workspace=22");
        }

        [TestMethod]
        public async Task NewLeavesModelsPassedInItsAttributesWithTheirOwnClient()
        {
            siteB.On("GET", "/form_field_sets/3", request => LoopbackSite.Json("{\"id\":3,\"title\":\"Form\"}"));
            siteB.On("PATCH", "/form_field_sets/3", request => LoopbackSite.Json("{\"id\":3,\"title\":\"Changed\"}"));
            FilesClient clientA = FilesClient.Create(siteA.Configuration());
            FilesClient clientB = FilesClient.Create(siteB.Configuration());
            FormFieldSet form = await clientB.FormFieldSets.FindAsync(3);

            Bundle bundle = clientA.Bundles.New(new Dictionary<string, object> { { "form_field_set", form } });
            await form.UpdateAsync(new Dictionary<string, object> { { "title", "Changed" } });

            Assert.AreSame(form, bundle.FormFieldSet);
            siteA.AssertReceived();
            siteB.AssertReceived("GET /form_field_sets/3?id=3 key=key-b workspace=22", "PATCH /form_field_sets/3 key=key-b workspace=22");
        }

        [TestMethod]
        public async Task ListsLoadEveryPageWithTheirClient()
        {
            FilesClient clientA = FilesClient.Create(siteA.Configuration());
            FilesList<User> users = clientA.Users.List();
            await users.LoadNextPageAsync();

            new FilesClient(siteB.Configuration());
            await users.LoadNextPageAsync();
            users.Reset();
            await users.LoadNextPageAsync();
            List<User> all = await clientA.Users.List().AllAsync();
            List<long?> enumerated = clientA.Users.List().ListAutoPaging(CancellationToken.None).Select(user => user.Id).ToList();
            await all[1].UpdateAsync(Changes());

            CollectionAssert.AreEqual(new long?[] { 1, 2 }, all.Select(user => user.Id).ToArray());
            CollectionAssert.AreEqual(new long?[] { 1, 2 }, enumerated);
            string firstPage = "GET /users key=key-a workspace=11";
            string secondPage = "GET /users?cursor=page-2 key=key-a workspace=11";
            siteA.AssertReceived(firstPage, secondPage, firstPage, firstPage, secondPage, firstPage, secondPage, "PATCH /users/2 key=key-a workspace=11");
            siteB.AssertReceived();
        }

        [TestMethod]
        public async Task TransfersRunEveryStageWithTheirClient()
        {
            byte[] contents = Enumerable.Range(1, 10).Select(value => (byte)value).ToArray();
            siteA.ServeUpload(partsize: 4);
            siteA.ServeDownload(contents);
            FilesClient clientA = FilesClient.Create(siteA.Configuration());
            new FilesClient(siteB.Configuration());
            var source = new MemoryStream(contents);
            var destination = new MemoryStream();

            Assert.IsTrue(await clientA.RemoteFiles.UploadFileAsync("remote.bin", source, contents.Length, MTime));
            await clientA.RemoteFiles.DownloadFileAsync("remote.bin", destination);
            RemoteFile file = clientA.RemoteFiles.New(new Dictionary<string, object> { { "path", "remote.bin" } });
            string downloadUri = await file.GetDownloadUriWithLoadAsync();

            string begin = "POST /file_actions/begin_upload/remote.bin key=key-a workspace=11";
            string details = "GET /files/remote.bin?path=remote.bin key=key-a workspace=11";
            siteA.AssertReceived(begin, "PUT /part", begin, "PUT /part", begin, "PUT /part", "POST /files/remote.bin key=key-a workspace=11", details, "GET /download", details);
            siteB.AssertReceived();
            CollectionAssert.AreEqual(
                new[] { "01-02-03-04", "05-06-07-08", "09-0A" },
                siteA.Requests.Where(request => request.Method == "PUT").Select(request => BitConverter.ToString(request.BodyBytes)).ToArray());
            CollectionAssert.AreEqual(contents, destination.ToArray());
            Assert.IsFalse(source.CanRead, "The upload disposes the stream it is given.");
            Assert.IsTrue(destination.CanWrite, "The download leaves the stream it is given open.");
            Assert.AreEqual(siteA.Url + "/download", downloadUri);
        }

        [TestMethod]
        public async Task ClientRequestsFollowTheAuthenticationRules()
        {
            siteA.On("POST", "/sessions", request => LoopbackSite.Json("{\"id\":\"created-session\"}"));
            FilesClient client = FilesClient.Create(siteA.Configuration());
            FilesConfiguration sessionConfiguration = siteA.Configuration();
            sessionConfiguration.SessionId = "configured-session";

            await client.Users.FindAsync(1, null, new Dictionary<string, object> { { "session_id", "explicit-session" }, { "api_key", "explicit-key" } });
            await client.Users.FindAsync(1, null, new Dictionary<string, object> { { "api_key", "explicit-key" } });
            await client.Users.FindAsync(1, null, new Dictionary<string, object> { { "workspace_id", null } });
            await client.Users.FindAsync(1, null, new Dictionary<string, object> { { "workspace_id", 0 } });
            Session session = await client.Sessions.CreateAsync(new Dictionary<string, object> { { "username", "user" }, { "password", "password" } });
            await FilesClient.Create(sessionConfiguration).Users.FindAsync(1);

            Assert.AreEqual("created-session", session.Id);
            siteA.AssertReceived(
                "GET /users/1?id=1 session=explicit-session workspace=11",
                "GET /users/1?id=1 key=explicit-key workspace=11",
                "GET /users/1?id=1 key=key-a",
                "GET /users/1?id=1 key=key-a workspace=0",
                "POST /sessions",
                "GET /users/1?id=1 session=configured-session workspace=11");
        }

        [TestMethod]
        public async Task ClientMethodsLeaveTheCallersDictionariesUnchanged()
        {
            FilesClient client = FilesClient.Create(siteA.Configuration());
            var parameters = new Dictionary<string, object>();
            var options = new Dictionary<string, object> { { "api_key", "explicit-key" } };

            User user = await client.Users.FindAsync(1, parameters, options);
            await user.UpdateAsync(parameters);
            await client.Users.List(parameters, options).AllAsync();

            Assert.AreEqual(0, parameters.Count);
            CollectionAssert.AreEqual(new[] { "api_key" }, options.Keys.ToArray());
            Assert.AreEqual(4, siteA.Requests.Count(request => request.ApiKey == "explicit-key"));
        }

        private static Dictionary<string, object> Changes()
        {
            return new Dictionary<string, object> { { "username", "changed" } };
        }
    }
}
