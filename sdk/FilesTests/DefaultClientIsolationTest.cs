using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FilesCom;
using FilesCom.Models;

namespace FilesTests
{
    // The static model methods run with the default client. Whatever they return, and every later request those
    // objects make, stays with the client that was the default when the object got its client, even after another
    // client becomes the default. These tests use only APIs that predate explicit clients.
    [TestClass]
    public class DefaultClientIsolationTest
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
        public async Task ModelKeepsTheDefaultClientItWasFoundWith()
        {
            new FilesClient(siteA.Configuration());
            User explicitUser = await User.Find(1, null, new Dictionary<string, object> { { "api_key", "explicit-a" }, { "workspace_id", "11" } });
            User configuredUser = await User.Find(2);

            new FilesClient(siteB.Configuration());
            await explicitUser.Update(Changes());
            await configuredUser.Update(Changes());

            siteA.AssertReceived(
                "GET /users/1?id=1 key=explicit-a workspace=11",
                "GET /users/2?id=2 key=key-a workspace=11",
                "PATCH /users/1 key=explicit-a workspace=11",
                "PATCH /users/2 key=key-a workspace=11");
            siteB.AssertReceived();
        }

        [TestMethod]
        [Timeout(30000)]
        public async Task ResultKeepsItsClientWhenTheDefaultChangesDuringTheRequest()
        {
            var arrived = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var respond = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            siteA.On("GET", "/users/5", async request =>
            {
                arrived.TrySetResult(true);
                await respond.Task;
                return LoopbackSite.Json("{\"id\":5}");
            });
            new FilesClient(siteA.Configuration());

            Task<User> finding = User.Find(5);
            await arrived.Task;
            new FilesClient(siteB.Configuration());
            respond.SetResult(true);
            User user = await finding;
            await user.Update(Changes());

            siteA.AssertReceived("GET /users/5?id=5 key=key-a workspace=11", "PATCH /users/5 key=key-a workspace=11");
            siteB.AssertReceived();
        }

        [TestMethod]
        public async Task NestedModelsKeepTheirParentsClientAndOptions()
        {
            siteA.On("GET", "/site", request => LoopbackSite.Json("{\"id\":9,\"user\":{\"id\":1,\"username\":\"nested\"}}"));
            siteA.On("GET", "/syncs", request => LoopbackSite.Json(@"[{""id"":3803,""name"":""Batchsync"",""latest_sync_run"":{""id"":41970631,""completed_at"":null,""log_url"":null,""runtime"":null,""status"":""in_progress"",""successful_files"":0}}]"));
            var options = new Dictionary<string, object> { { "api_key", "explicit-a" } };
            new FilesClient(siteA.Configuration());
            Site site = await Site.Get(null, options);
            List<Sync> syncs = await Sync.List(null, options).All();

            new FilesClient(siteB.Configuration());
            await site.User.Update(Changes());

            Assert.AreEqual("explicit-a", syncs[0].LatestSyncRun.GetOption("api_key"));
            Assert.IsNull(syncs[0].LatestSyncRun.Runtime);
            siteA.AssertReceived("GET /site key=explicit-a workspace=11", "GET /syncs key=explicit-a workspace=11", "PATCH /users/1 key=explicit-a workspace=11");
            siteB.AssertReceived();
        }

        [TestMethod]
        public async Task ListKeepsItsClientForLaterPagesAndReset()
        {
            new FilesClient(siteA.Configuration());
            FilesList<User> users = User.List();
            var constructed = new FilesList<User>("/users", HttpMethod.Get, new Dictionary<string, object>(), new Dictionary<string, object>());
            await users.LoadNextPage();

            new FilesClient(siteB.Configuration());
            await users.LoadNextPage();
            users.Reset();
            await users.LoadNextPage();
            await constructed.LoadNextPage();

            new FilesClient(siteA.Configuration());
            await constructed.LoadNextPage();

            Assert.AreEqual(2, constructed.data[0].Id);
            siteA.AssertReceived("GET /users key=key-a workspace=11", "GET /users?cursor=page-2 key=key-a workspace=11", "GET /users key=key-a workspace=11");
            siteB.AssertReceived("GET /users key=key-b workspace=22", "GET /users?cursor=page-2 key=key-b workspace=22");
        }

        [TestMethod]
        public async Task UploadKeepsItsClientWhenTheDefaultChangesMidTransfer()
        {
            bool switched = false;
            siteA.ServeUpload(partsize: 4, onPart: () =>
            {
                if (!switched)
                {
                    switched = true;
                    new FilesClient(siteB.Configuration());
                }
            });
            siteB.ServeUpload(partsize: 4);
            new FilesClient(siteA.Configuration());

            Assert.IsTrue(await RemoteFile.UploadFile("remote.bin", new MemoryStream(new byte[10]), 10, MTime));

            string begin = "POST /file_actions/begin_upload/remote.bin key=key-a workspace=11";
            siteA.AssertReceived(begin, "PUT /part", begin, "PUT /part", begin, "PUT /part", "POST /files/remote.bin key=key-a workspace=11");
            siteB.AssertReceived();
        }

        [TestMethod]
        public async Task ModelMadeWithoutAClientKeepsTheDefaultOfItsFirstRequestEvenIfItFailed()
        {
            int attempts = 0;
            siteA.On("PATCH", "/users/3", request => ++attempts == 1 ? LoopbackSite.Status(500) : LoopbackSite.Json("{\"id\":3}"));
            new FilesClient(siteA.Configuration());
            var user = new User(new Dictionary<string, object> { { "id", 3L } }, null);

            await Assert.ThrowsExceptionAsync<ApiException>(() => user.Update(Changes()));
            new FilesClient(siteB.Configuration());
            await user.Update(Changes());

            siteA.AssertReceived("PATCH /users/3 key=key-a workspace=11", "PATCH /users/3 key=key-a workspace=11");
            siteB.AssertReceived();
        }

        [TestMethod]
        public async Task SavedModelKeepsTheClientAndOptionsItWasCreatedWith()
        {
            foreach (LoopbackSite site in new[] { siteA, siteB })
            {
                site.On("POST", "/workspaces", request => LoopbackSite.Json("{\"id\":7,\"name\":\"Projects\"}"));
                site.On("PATCH", "/workspaces/7", request => LoopbackSite.Json("{\"id\":7,\"name\":\"Renamed\"}"));
            }
            new FilesClient(siteA.Configuration());
            var workspace = new Workspace(new Dictionary<string, object> { { "name", "Projects" } }, new Dictionary<string, object> { { "api_key", "explicit-a" } });

            await workspace.Save();
            new FilesClient(siteB.Configuration());
            workspace.Name = "Renamed";
            await workspace.Save();

            siteA.AssertReceived("POST /workspaces key=explicit-a workspace=11", "PATCH /workspaces/7 key=explicit-a workspace=11");
            siteB.AssertReceived();
        }

        [TestMethod]
        public async Task ConcurrentFirstRequestsOfAModelAgreeOnOneClient()
        {
            FilesConfiguration a = siteA.Configuration();
            FilesConfiguration b = siteB.Configuration();
            new FilesClient(a);
            List<User> users = Enumerable.Range(1, 40).Select(id => new User(new Dictionary<string, object> { { "id", (long)id } }, null)).ToList();

            using (var stop = new CancellationTokenSource())
            {
                Task switching = Task.Run(() =>
                {
                    for (int i = 0; !stop.IsCancellationRequested; i++)
                    {
                        new FilesClient(i % 2 == 0 ? b : a);
                    }
                });
                try
                {
                    await Task.WhenAll(users.Select(user => Task.WhenAll(user.Update(Changes()), user.Update(Changes()))));
                }
                finally
                {
                    stop.Cancel();
                    await switching;
                }
            }

            List<ReceivedRequest> updates = siteA.Requests.Concat(siteB.Requests).ToList();
            Assert.AreEqual(80, updates.Count);
            foreach (IGrouping<string, ReceivedRequest> requestsForUser in updates.GroupBy(request => request.Path))
            {
                Assert.AreEqual(1, requestsForUser.Select(request => request.Site).Distinct().Count(), requestsForUser.Key + " was sent to both sites");
            }
        }

        [TestMethod]
        public async Task ConstructorClientUsesConfigurationChangesInLaterOperations()
        {
            FilesConfiguration configuration = siteA.Configuration();
            new FilesClient(configuration);
            await User.Find(1);

            configuration.BaseUrl = siteB.Url;
            configuration.ApiKey = "rotated";
            configuration.WorkspaceId = null;
            configuration.Language = "fr";
            await User.Find(1);

            siteA.AssertReceived("GET /users/1?id=1 key=key-a workspace=11");
            siteB.AssertReceived("GET /users/1?id=1 key=rotated");
            Assert.AreEqual("fr", siteB.Requests[0].Language);
        }

        [TestMethod]
        public async Task OperationKeepsTheSettingsItStartedWithWhileTheConfigurationChanges()
        {
            FilesConfiguration configuration = siteA.Configuration();
            System.Action moveToSiteB = () =>
            {
                configuration.BaseUrl = siteB.Url;
                configuration.ApiKey = "rotated";
            };
            siteA.On("GET", "/users", request =>
            {
                if (request.Path.Contains("cursor=page-2"))
                {
                    return LoopbackSite.Json("[{\"id\":2}]");
                }
                moveToSiteB();
                return LoopbackSite.Json("[{\"id\":1}]", cursor: "page-2");
            }, priority: 1);
            siteA.ServeUpload(partsize: 4, onPart: moveToSiteB);
            new FilesClient(configuration);

            List<User> users = await User.List().All();
            configuration.BaseUrl = siteA.Url;
            configuration.ApiKey = "key-a";
            await RemoteFile.UploadFile("remote.bin", new MemoryStream(new byte[6]), 6, MTime);
            await User.Find(1);

            Assert.AreEqual(2, users.Count);
            string begin = "POST /file_actions/begin_upload/remote.bin key=key-a workspace=11";
            siteA.AssertReceived(
                "GET /users key=key-a workspace=11",
                "GET /users?cursor=page-2 key=key-a workspace=11",
                begin, "PUT /part", begin, "PUT /part",
                "POST /files/remote.bin key=key-a workspace=11");
            siteB.AssertReceived("GET /users/1?id=1 key=rotated workspace=11");
        }

        private static Dictionary<string, object> Changes()
        {
            return new Dictionary<string, object> { { "username", "changed" } };
        }
    }
}
