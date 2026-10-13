using FilesCom;
using FilesCom.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using WireMock;

namespace FilesTests
{
    // A list continues while Files.com gives a next cursor, however many pages are empty. Every way of going through
    // a list starts from its query's first page; the asynchronous one loads a page only when it is reached, holds
    // one page at a time and stops sending as soon as its caller stops.
    [TestClass]
    public class PagingTest
    {
        private readonly List<TaskCompletionSource<bool>> heldResponses = new List<TaskCompletionSource<bool>>();
        private LoopbackSite site;

        [TestInitialize]
        public void StartSite()
        {
            site = new LoopbackSite("key-a", "11");
        }

        [TestCleanup]
        public void StopSite()
        {
            foreach (TaskCompletionSource<bool> response in heldResponses)
            {
                response.TrySetResult(true);
            }
            site.Dispose();
        }

        [TestMethod]
        public async Task EveryWayOfPagingContinuesThroughEmptyPagesWhileThereIsACursor()
        {
            // Two empty pages in the middle, then an item, then an empty last page. One page gives its cursor only in
            // X-Files-Cursor-Next, as Files.com also sends it; the others only in the older X-Files-Cursor.
            site.On("GET", "/users", request =>
            {
                switch (Cursor(request))
                {
                    case null: return LoopbackSite.Json("[{\"id\":1},{\"id\":2}]", cursor: "c2");
                    case "c2": return LoopbackSite.Json("[]", cursor: "c3");
                    case "c3": return WithHeader(LoopbackSite.Json("[]"), "X-Files-Cursor-Next", "c4");
                    case "c4": return LoopbackSite.Json("[{\"id\":4}]", cursor: "c5");
                    default: return LoopbackSite.Json("[]");
                }
            });
            FilesClient client = FilesClient.Create(site.Configuration());

            var enumerated = new List<long?>();
            await foreach (User user in client.Users.List().ListAutoPagingAsync())
            {
                enumerated.Add(user.Id);
            }
            List<long?> blocking = client.Users.List().ListAutoPaging(CancellationToken.None).Select(user => user.Id).ToList();
            List<long?> all = (await client.Users.List().AllAsync()).Select(user => user.Id).ToList();
            var manual = new List<long?>();
            FilesList<User> pages = client.Users.List();
            do
            {
                manual.AddRange((await pages.LoadNextPageAsync()).Select(user => user.Id));
            } while (pages.HasNextPage);

            foreach (List<long?> ids in new[] { enumerated, blocking, all, manual })
            {
                CollectionAssert.AreEqual(new long?[] { 1, 2, 4 }, ids);
            }
            string[] journey = { "GET /users", "GET /users?cursor=c2", "GET /users?cursor=c3", "GET /users?cursor=c4", "GET /users?cursor=c5" };
            CollectionAssert.AreEqual(
                Enumerable.Repeat(journey, 4).SelectMany(requests => requests).ToArray(),
                site.Requests.Select(request => request.Method + " " + request.Path).ToArray());
        }

        [TestMethod]
        [Timeout(30000)]
        public async Task AsyncPagingWaitsForAPageWithoutBlockingAndHoldsOnlyThePageInUse()
        {
            var secondPageRequested = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            TaskCompletionSource<bool> sendSecondPage = Held();
            site.On("GET", "/users", async request =>
            {
                if (Cursor(request) == null)
                {
                    return LoopbackSite.Json("[{\"id\":1}]", cursor: "c2");
                }
                secondPageRequested.TrySetResult(true);
                await sendSecondPage.Task;
                return LoopbackSite.Json("[{\"id\":2}]");
            });
            FilesClient client = FilesClient.Create(site.Configuration());

            IAsyncEnumerator<User> users = client.Users.List().ListAutoPagingAsync().GetAsyncEnumerator();
            try
            {
                Assert.AreEqual(0, site.Requests.Count, "Nothing is sent before enumerating.");
                WeakReference firstUser = await NextUser(users);

                ValueTask<bool> movingToSecondPage = users.MoveNextAsync();
                await secondPageRequested.Task;
                Assert.IsFalse(movingToSecondPage.IsCompleted, "Waiting for the page returns to the caller instead of blocking.");
                sendSecondPage.SetResult(true);
                Assert.IsTrue(await movingToSecondPage);
                Assert.AreEqual(2, users.Current.Id);

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                Assert.IsFalse(firstUser.IsAlive, "The first page is released once the second is in use.");
            }
            finally
            {
                await users.DisposeAsync();
            }
        }

        [TestMethod]
        [Timeout(30000)]
        public async Task AsyncPagingStopsWhenItsCallerStopsFailsOrCancels()
        {
            site.On("GET", "/users", request => Cursor(request) == "c2"
                ? LoopbackSite.Json(@"{""type"":""not-found"",""error"":""Not Found""}", status: 404)
                : LoopbackSite.Json("[{\"id\":1},{\"id\":2}]", cursor: "c2"));
            FilesClient client = FilesClient.Create(site.Configuration());

            await foreach (User user in client.Users.List().ListAutoPagingAsync())
            {
                break;
            }
            await Task.Delay(300);
            site.AssertReceived("GET /users key=key-a workspace=11");

            var reached = new List<long?>();
            await Assert.ThrowsExactlyAsync<NotFoundException>(async () =>
            {
                await foreach (User user in client.Users.List().ListAutoPagingAsync())
                {
                    reached.Add(user.Id);
                }
            });
            CollectionAssert.AreEqual(new long?[] { 1, 2 }, reached, "Items before the failed page are enumerated.");

            // A token given with WithCancellation, or to ListAutoPagingAsync, stops before the next page.
            using (var cancellation = new CancellationTokenSource())
            {
                await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                {
                    await foreach (User user in client.Users.List().ListAutoPagingAsync().WithCancellation(cancellation.Token))
                    {
                        cancellation.Cancel();
                    }
                });
            }
            using (var cancellation = new CancellationTokenSource())
            {
                await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                {
                    await foreach (User user in client.Users.List().ListAutoPagingAsync(cancellation.Token))
                    {
                        cancellation.Cancel();
                    }
                });
            }

            site.AssertReceived(
                "GET /users key=key-a workspace=11",
                "GET /users key=key-a workspace=11",
                "GET /users?cursor=c2 key=key-a workspace=11",
                "GET /users key=key-a workspace=11",
                "GET /users key=key-a workspace=11");
        }

        [TestMethod]
        public async Task ListsStartFromTheirQueryAndKeepTheirPageWhenLoadingFails()
        {
            int secondPageLoads = 0;
            site.On("GET", "/users", request =>
            {
                if (Cursor(request) != "c2")
                {
                    return LoopbackSite.Json("[{\"id\":1}]", cursor: "c2");
                }
                return ++secondPageLoads == 1
                    ? LoopbackSite.Json(@"{""type"":""not-found"",""error"":""Not Found""}", status: 404)
                    : LoopbackSite.Json("[{\"id\":2}]");
            });
            new FilesClient(site.Configuration());
            var query = new Dictionary<string, object> { { "per_page", 1L } };
            FilesList<User> users = User.List(query);

            await users.LoadNextPageAsync();
            await Assert.ThrowsExactlyAsync<NotFoundException>(() => users.LoadNextPageAsync());
            Assert.AreEqual(1, users.data.Single().Id, "A failed page leaves the loaded page as it was.");
            Assert.IsTrue(users.HasNextPage);
            await users.LoadNextPageAsync();
            Assert.AreEqual(2, users.data.Single().Id);
            Assert.IsFalse(users.HasNextPage);

            List<User> all = await users.AllAsync();
            users.Reset();
            await users.LoadNextPageAsync();

            CollectionAssert.AreEqual(new long?[] { 1, 2 }, all.Select(user => user.Id).ToArray());
            Assert.AreEqual(1, users.data.Single().Id);
            CollectionAssert.AreEqual(new[] { "per_page" }, query.Keys.ToArray(), "The caller's query is not changed.");
            site.AssertReceived(
                "GET /users?per_page=1 key=key-a workspace=11",
                "GET /users?per_page=1&cursor=c2 key=key-a workspace=11",
                "GET /users?per_page=1&cursor=c2 key=key-a workspace=11",
                "GET /users?per_page=1 key=key-a workspace=11",
                "GET /users?per_page=1&cursor=c2 key=key-a workspace=11",
                "GET /users?per_page=1 key=key-a workspace=11");
        }

        [TestMethod]
        [DataRow("null", DisplayName = "JSON null")]
        [DataRow("{\"id\":1}", DisplayName = "an object")]
        [DataRow("\"users\"", DisplayName = "a string")]
        public async Task APageThatIsNotAnArrayIsAnInvalidResponse(string page)
        {
            site.On("GET", "/users", request => LoopbackSite.Json(page, cursor: "c2"));
            FilesClient client = FilesClient.Create(site.Configuration());
            FilesList<User> users = client.Users.List();

            await Assert.ThrowsExactlyAsync<InvalidResponseException>(() => users.LoadNextPageAsync());
            await Assert.ThrowsExactlyAsync<InvalidResponseException>(async () =>
            {
                await foreach (User user in client.Users.List().ListAutoPagingAsync())
                {
                }
            });

            Assert.AreEqual(0, users.data.Count);
            Assert.IsFalse(users.HasNextPage);
        }

        [TestMethod]
        [Timeout(30000)]
        public async Task AnEmptyPageThatGivesBackItsOwnCursorIsAnInvalidResponse()
        {
            // An empty first page names c1, and c1 is empty again and names c1: the list makes no progress.
            site.On("GET", "/users", request => LoopbackSite.Json("[]", cursor: "c1"));
            FilesClient client = FilesClient.Create(site.Configuration());

            InvalidResponseException error = await Assert.ThrowsExactlyAsync<InvalidResponseException>(async () =>
            {
                await foreach (User user in client.Users.List().ListAutoPagingAsync())
                {
                }
            });
            await Assert.ThrowsExactlyAsync<InvalidResponseException>(() => client.Users.List().AllAsync());
            FilesList<User> pages = client.Users.List();
            await pages.LoadNextPageAsync();
            await Assert.ThrowsExactlyAsync<InvalidResponseException>(() => pages.LoadNextPageAsync());

            StringAssert.Contains(error.Message, "no progress");
            Assert.IsTrue(pages.HasNextPage, "The page that failed can be loaded again: the position is kept.");
            Assert.AreEqual(0, pages.data.Count);
            Assert.AreEqual(6, site.Requests.Count, "Each way of paging asked for the first page and c1 once.");
        }

        // The cursor a page request asked for, or null for a first page.
        private static string Cursor(ReceivedRequest request)
        {
            string cursor = request.Path.Split('?', '&').FirstOrDefault(part => part.StartsWith("cursor="));
            return cursor?.Substring("cursor=".Length);
        }

        private static ResponseMessage WithHeader(ResponseMessage response, string name, string value)
        {
            response.AddHeader(name, value);
            return response;
        }

        // A response source that the test cleanup sets, so no held response outlives its test.
        private TaskCompletionSource<bool> Held()
        {
            var held = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            heldResponses.Add(held);
            return held;
        }

        // Moves to the next user and returns a weak reference to it, so that nothing here keeps it alive.
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static async Task<WeakReference> NextUser(IAsyncEnumerator<User> users)
        {
            Assert.IsTrue(await users.MoveNextAsync());
            return new WeakReference(users.Current);
        }
    }
}