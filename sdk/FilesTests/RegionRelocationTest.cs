using FilesCom;
using FilesCom.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FilesTests
{
    // Regional relocation over real TLS: Files.com sites on loopback, reached by their DNS names through the client's
    // own handler, with certificates from an authority of the test's own that only that handler trusts. An operation
    // moves, once, to the address its configured origin names in a 403 lockout-region-mismatch, when the answer can
    // be trusted and the address is allowed, as one of its retries; what it returns keeps the new address.
    [TestClass]
    public class RegionRelocationTest
    {
        private const string App = "app.relocation-test.net";
        private const string Eu = "eu.relocation-test.net";
        private const string Other = "other.relocation-test.net";
        private const string Impostor = "impostor.relocation-test.net";

        private static TestCertificateAuthority authority;
        private static TestCertificateAuthority stranger;
        private static X509Certificate2 trusted;
        private static X509Certificate2 untrusted;
        private Sites sites;

        [ClassInitialize]
        public static void MakeCertificates(TestContext context)
        {
            authority = new TestCertificateAuthority("Relocation Test Authority");
            stranger = new TestCertificateAuthority("Stranger Authority");
            trusted = authority.Issue(App, Eu, Other, "localhost");
            untrusted = stranger.Issue(Impostor);
        }

        [ClassCleanup]
        public static void DisposeCertificates()
        {
            trusted.Dispose();
            untrusted.Dispose();
            authority.Dispose();
            stranger.Dispose();
        }

        [TestInitialize]
        public void Start()
        {
            sites = new Sites(name => name == Impostor ? untrusted : trusted);
        }

        [TestCleanup]
        public void Stop()
        {
            sites.Dispose();
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow(Eu)]
        [DataRow("https://eu.relocation-test.net")]
        [DataRow("https://EU.relocation-test.net:443/")]
        public async Task TheOperationMovesAndWhatItReturnsStaysThere(string host)
        {
            sites.On(App, "PATCH /users/1", request => Mismatch(host));
            sites.On(Eu, "PATCH /users/1", request => Answer.Json("{\"id\":1,\"name\":\"Moved\"}"));
            sites.On(Eu, "POST /users/1/unlock", request => Answer.Json("{}"));
            sites.On(App, "GET /users/2", request => Answer.Json("{\"id\":2}"));
            FilesClient client = Client();

            User user;
            using (var logs = new LogCapture())
            {
                user = await client.Users.UpdateAsync(1, new Dictionary<string, object> { { "name", "Moved" } });

                CollectionAssert.Contains(logs.Lines("Retrying"), "Retrying PATCH /users after Files.com answered that the site is served from https://eu.relocation-test.net, at that address: retry 1 of 3, in 0 seconds");
            }
            await user.UnlockAsync();
            await client.Users.FindAsync(2);

            Assert.AreEqual("Moved", user.Name);
            CollectionAssert.AreEqual(
                new[] { $"{App} PATCH /users/1", $"{Eu} PATCH /users/1", $"{Eu} POST /users/1/unlock", $"{App} GET /users/2?id=2" },
                sites.Received.Select(request => request.ToString()).ToArray(),
                string.Join(" | ", sites.Received));
            Received[] patches = sites.Received.Take(2).ToArray();
            CollectionAssert.AreEqual(patches[0].Body, patches[1].Body, "The same body goes to the new address.");
            Assert.AreEqual("key-a", patches[1].Headers["X-FilesAPI-Key"]);
            Assert.AreEqual(Eu, patches[1].ServerName, "The new address was reached over TLS by its own name.");
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task PagesAndTheirItemsStayWhereTheListMoved()
        {
            sites.On(App, "GET /users", request => Mismatch(Eu));
            sites.On(Eu, "GET /users", request => request.Path.Contains("cursor=next")
                ? Answer.Json("[{\"id\":3}]")
                : Answer.Json("[{\"id\":2}]", "X-Files-Cursor-Next: next\r\n"));
            sites.On(Eu, "POST /users/3/unlock", request => Answer.Json("{}"));
            FilesClient client = Client();

            List<User> users = await client.Users.List().AllAsync();
            await users[1].UnlockAsync();
            // Another operation of the client starts at its BaseUrl, which does not know the user.
            await Assert.ThrowsExactlyAsync<NotFoundException>(() => client.Users.FindAsync(3));
            // Each page loaded on its own is an operation of its own, from where the list's last page came from.
            FilesList<User> list = client.Users.List();
            await list.LoadNextPageAsync();
            await list.LoadNextPageAsync();

            CollectionAssert.AreEqual(new long?[] { 2, 3 }, users.Select(user => user.Id).ToArray());
            Assert.AreEqual(3, list.data.Single().Id);
            CollectionAssert.AreEqual(
                new[]
                {
                    $"{App} GET /users", $"{Eu} GET /users", $"{Eu} GET /users?cursor=next", $"{Eu} POST /users/3/unlock", $"{App} GET /users/3?id=3",
                    $"{App} GET /users", $"{Eu} GET /users", $"{Eu} GET /users?cursor=next",
                },
                sites.Received.Select(request => request.ToString()).ToArray(),
                string.Join(" | ", sites.Received));
        }

        // A change answered with the site's region was not carried out when the answer is trusted, even with no retry
        // left to move it. An answer whose trust was not checked, as with relocation disabled, shows nothing: the change
        // may have taken effect.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(false, DisplayName = "Trusted, with no retry left")]
        [DataRow(true, DisplayName = "Not checked, with relocation disabled")]
        public async Task AChangeAnsweredWithTheSitesRegionSaysWhetherItMayHaveTakenEffect(bool disabled)
        {
            sites.On(App, "PATCH /users/1", request => Mismatch(Eu));
            FilesClient client = Client(retries: 0, policy: disabled ? RegionRelocationPolicy.Disabled : null);

            LockoutRegionMismatchException error = await Assert.ThrowsExactlyAsync<LockoutRegionMismatchException>(() => client.Users.UpdateAsync(1, new Dictionary<string, object> { { "name", "Moved" } }));

            Assert.AreEqual(disabled ? RegionRelocationRefusal.Disabled : RegionRelocationRefusal.RetriesExhausted, error.Refusal);
            Assert.AreEqual(disabled, error.RequestMayHaveBeenProcessed);
            Assert.AreEqual(1, sites.Received.Count);
        }

        // Moving is one of the operation's retries: with none left, the mismatch is thrown, and nothing more is sent.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(0, false, 1, DisplayName = "No retries: no move")]
        [DataRow(1, true, 2, DisplayName = "A 503 took the only retry: no move")]
        [DataRow(2, true, 3, DisplayName = "A 503 and the move each take a retry")]
        [DataRow(1, false, 2, DisplayName = "The move takes the only retry")]
        public async Task MovingTakesOneOfTheOperationsRetries(int retries, bool unavailableFirst, int requests)
        {
            int appRequests = 0;
            sites.On(App, "GET /users/1", request => ++appRequests == 1 && unavailableFirst ? Answer.Status(503) : Mismatch(Eu));
            sites.On(Eu, "GET /users/1", request => Answer.Json("{\"id\":1}"));
            FilesClient client = Client(retries);

            bool moves = retries > (unavailableFirst ? 1 : 0);
            if (moves)
            {
                User user = await client.Users.FindAsync(1);
                Assert.AreEqual(1, user.Id);
            }
            else
            {
                LockoutRegionMismatchException error = await Assert.ThrowsExactlyAsync<LockoutRegionMismatchException>(() => client.Users.FindAsync(1));
                Assert.AreEqual(RegionRelocationRefusal.RetriesExhausted, error.Refusal);
                Assert.AreEqual("https://eu.relocation-test.net", error.RequestedOrigin);
                StringAssert.EndsWith(error.Message, "The request was not sent to https://eu.relocation-test.net: moving takes one of the operation's retries, and it had none left.");
            }
            Assert.AreEqual(requests, sites.Received.Count);
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task AnOperationMovesOnceAtMost()
        {
            sites.On(App, "GET /users/1", request => Mismatch(Eu));
            sites.On(Eu, "GET /users/1", request => Mismatch(Other));
            FilesClient client = Client();

            LockoutRegionMismatchException error = await Assert.ThrowsExactlyAsync<LockoutRegionMismatchException>(() => client.Users.FindAsync(1));

            Assert.AreEqual(RegionRelocationRefusal.AlreadyRelocated, error.Refusal);
            Assert.AreEqual("https://other.relocation-test.net", error.RequestedOrigin);
            Assert.AreEqual(2, sites.Received.Count);
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("", null, RegionRelocationRefusal.InvalidHost)]
        [DataRow("eu.relocation-test.net/api", null, RegionRelocationRefusal.InvalidHost)]
        [DataRow("https://eu.relocation-test.net/api", null, RegionRelocationRefusal.InvalidHost)]
        [DataRow("https://user@eu.relocation-test.net", null, RegionRelocationRefusal.InvalidHost)]
        [DataRow("https://eu.relocation-test.net?next=1", null, RegionRelocationRefusal.InvalidHost)]
        [DataRow("https://eu.relocation-test.net#top", null, RegionRelocationRefusal.InvalidHost)]
        [DataRow("//eu.relocation-test.net", null, RegionRelocationRefusal.InvalidHost)]
        [DataRow("eu.relocation-test.net:8443", null, RegionRelocationRefusal.InvalidHost)]
        [DataRow("ftp://eu.relocation-test.net", null, RegionRelocationRefusal.InvalidHost)]
        [DataRow("eu.relocation-test.net\\evil", null, RegionRelocationRefusal.InvalidHost)]
        [DataRow("eu.relocation-test.net.", null, RegionRelocationRefusal.InvalidHost)]
        [DataRow("eu%2erelocation-test.net", null, RegionRelocationRefusal.InvalidHost)]
        [DataRow(" eu.relocation-test.net", null, RegionRelocationRefusal.InvalidHost)]
        [DataRow("https://eu.relocation-test.net:8443", "https://eu.relocation-test.net:8443", RegionRelocationRefusal.NotAllowed)]
        [DataRow("http://eu.relocation-test.net", "http://eu.relocation-test.net", RegionRelocationRefusal.NotAllowed)]
        [DataRow("localhost", "https://localhost", RegionRelocationRefusal.NotAllowed)]
        [DataRow("127.0.0.1", "https://127.0.0.1", RegionRelocationRefusal.NotAllowed)]
        [DataRow("files.test", "https://files.test", RegionRelocationRefusal.NotAllowed)]
        [DataRow("intranet", "https://intranet", RegionRelocationRefusal.NotAllowed)]
        [DataRow("app.relocation-test.net", "https://app.relocation-test.net", RegionRelocationRefusal.SameOrigin)]
        [DataRow("https://APP.relocation-test.net:443/", "https://app.relocation-test.net", RegionRelocationRefusal.SameOrigin)]
        public async Task AddressesThatAreNotAllowedAreRefused(string host, string origin, RegionRelocationRefusal refusal)
        {
            sites.On(App, "GET /users/1", request => Mismatch(host));
            FilesClient client = Client();

            LockoutRegionMismatchException error = await Assert.ThrowsExactlyAsync<LockoutRegionMismatchException>(() => client.Users.FindAsync(1));

            Assert.AreEqual(refusal, error.Refusal);
            Assert.AreEqual(origin, error.RequestedOrigin);
            Assert.AreEqual(1, sites.Received.Count);
            StringAssert.StartsWith(error.Message, "You are attempting to access a site in a different region.");
        }

        [TestMethod]
        [Timeout(60000)]
        [DataRow("{\"host\":42}")]
        [DataRow("{\"host\":null}")]
        [DataRow("{}")]
        [DataRow((string)null)]
        public async Task AnAnswerWithoutAHostIsRefused(string data)
        {
            sites.On(App, "GET /users/1", request => Answer.Json(MismatchJson(data), status: 403));
            FilesClient client = Client();

            LockoutRegionMismatchException error = await Assert.ThrowsExactlyAsync<LockoutRegionMismatchException>(() => client.Users.FindAsync(1));

            Assert.AreEqual(RegionRelocationRefusal.InvalidHost, error.Refusal);
            Assert.IsNull(error.RequestedOrigin);
            Assert.AreEqual(1, sites.Received.Count);
        }

        // Only the address the operation trusts with its credentials may send it elsewhere, whatever pair the
        // application lists: not an answer after a redirect to another origin, one from another address, as where a
        // handler of the application's own sent the request elsewhere, or one that does not say which request it
        // answers. The error says which, and offers no pair.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("after a redirect", false)]
        [DataRow("after a redirect", true)]
        [DataRow("from another address", true)]
        [DataRow("without its request", true)]
        public async Task AnAnswerFromAnotherOriginIsNotTrustedWhateverPairIsListed(string answer, bool listedPair)
        {
            sites.On(App, "GET /users/1", request => Answer.Redirect(307, $"https://{Other}/api/rest/v1/users/1"));
            sites.On(Other, "GET /users/1", request => answer == "after a redirect" ? Answer.Redirect(307, $"https://{App}/api/rest/v1/users/back") : Mismatch(Eu));
            sites.On(App, "GET /users/back", request => Mismatch(Eu));
            RegionRelocationPolicy policy = listedPair ? RegionRelocationPolicy.OnlyBetween($"https://{App}", $"https://{Eu}") : null;
            FilesClient client = Client(policy: policy, wrap: answer == "after a redirect" ? null : handler => new ApplicationHandler(answer == "from another address" ? Other : null, handler));

            LockoutRegionMismatchException error = await Assert.ThrowsExactlyAsync<LockoutRegionMismatchException>(() => client.Users.FindAsync(1));

            Assert.AreEqual(RegionRelocationRefusal.UntrustedIssuer, error.Refusal);
            Assert.AreEqual("https://eu.relocation-test.net", error.RequestedOrigin);
            string why = answer == "after a redirect" ? "the answer came after a redirect to another origin, not straight from the address the request was sent to."
                : answer == "from another address" ? "the answer came from https://other.relocation-test.net, not from the address the request was sent to."
                : "the response did not say which request it answered, so the client cannot tell which address sent it.";
            StringAssert.EndsWith(error.Message, "The request was not sent to https://eu.relocation-test.net: " + why);
            Assert.IsFalse(sites.Received.Any(request => request.Host == Eu), "Nothing went to the address the answer named.");
            if (answer == "after a redirect")
            {
                Assert.AreEqual(3, sites.Received.Count);
                Assert.IsFalse(sites.Received[2].Headers.ContainsKey("X-FilesAPI-Key"), "Credentials stay off every hop after the origin changed.");
            }
        }

        // Only Files.com's 403 names a site's address, whatever pair the application lists; the error keeps the answer.
        [TestMethod]
        [Timeout(60000)]
        [DataRow(false)]
        [DataRow(true)]
        public async Task AnAnswerWithAnotherStatusIsNotTrusted(bool listedPair)
        {
            sites.On(App, "GET /users/1", request => Answer.Json(MismatchJson($"{{\"host\":\"{Eu}\"}}"), status: 401));
            FilesClient client = Client(policy: listedPair ? RegionRelocationPolicy.OnlyBetween($"https://{App}", $"https://{Eu}") : null);

            LockoutRegionMismatchException error = await Assert.ThrowsExactlyAsync<LockoutRegionMismatchException>(() => client.Users.FindAsync(1));

            Assert.AreEqual(RegionRelocationRefusal.UntrustedIssuer, error.Refusal);
            Assert.AreEqual(401, error.httpStatus);
            Assert.AreEqual(Eu, error.data["host"].ToString());
            StringAssert.EndsWith(error.Message, "The request was not sent to https://eu.relocation-test.net: the answer was HTTP 401, and Files.com names a site's address only in a 403.");
            Assert.AreEqual(1, sites.Received.Count);
        }

        // A handler that decides for itself which certificates to accept cannot be seen to check them: only a pair of
        // origins the application lists moves the operation.
        [TestMethod]
        [Timeout(60000)]
        public async Task WithoutCertificateChecksTheClientCanSeeOnlyAListedPairMoves()
        {
            sites.On(App, "GET /users/1", request => Mismatch(Eu));
            sites.On(Eu, "GET /users/1", request => Answer.Json("{\"id\":1}"));
            Action<SocketsHttpHandler> ownChecks = handler => handler.SslOptions.RemoteCertificateValidationCallback = (sender, certificate, chain, errors) => errors == System.Net.Security.SslPolicyErrors.None;

            LockoutRegionMismatchException error = await Assert.ThrowsExactlyAsync<LockoutRegionMismatchException>(() => Client(configure: ownChecks).Users.FindAsync(1));
            User user = await Client(policy: RegionRelocationPolicy.OnlyBetween($"https://{App}", $"https://{Eu}"), configure: ownChecks).Users.FindAsync(1);

            Assert.AreEqual(RegionRelocationRefusal.UntrustedIssuer, error.Refusal);
            Assert.AreEqual(403, error.httpStatus);
            Assert.AreEqual(Eu, error.httpHeaders.GetValues("X-Files-Host").Single(), "The error keeps the answer's headers.");
            StringAssert.EndsWith(error.Message, "The request was not sent to https://eu.relocation-test.net: the answer came over HTTPS through a handler the client does not see check certificates as .NET does: "
                + "a shared handler, one with a validation callback or a chain policy that waives a check, one of a type the client cannot inspect, or one that has sent a request with such settings. "
                + "If both addresses serve Files.com for the client's credentials, RegionRelocationPolicy.OnlyBetween(\"https://app.relocation-test.net\", \"https://eu.relocation-test.net\") allows this move.");
            Assert.AreEqual(1, user.Id);
            CollectionAssert.AreEqual(new[] { $"{App} GET /users/1?id=1", $"{App} GET /users/1?id=1", $"{Eu} GET /users/1?id=1" }, sites.Received.Select(request => request.ToString()).ToArray(),
                string.Join(" | ", sites.Received));
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task DisabledThrowsWithTheAddressForTheApplication()
        {
            sites.On(App, "GET /users/1", request => Mismatch(Eu));
            FilesClient client = Client(policy: RegionRelocationPolicy.Disabled);

            LockoutRegionMismatchException error = await Assert.ThrowsExactlyAsync<LockoutRegionMismatchException>(() => client.Users.FindAsync(1));

            Assert.AreEqual(RegionRelocationRefusal.Disabled, error.Refusal);
            Assert.AreEqual("https://eu.relocation-test.net", error.RequestedOrigin);
            Assert.AreEqual(Eu, error.data["host"].ToString(), "The error keeps what Files.com sent.");
            StringAssert.EndsWith(error.Message, "The request was not sent to https://eu.relocation-test.net: FilesConfiguration.RegionRelocation is Disabled.");
            Assert.AreEqual(1, sites.Received.Count);
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task OnlyBetweenMovesBetweenItsPairsAlone()
        {
            sites.On(App, "GET /users/1", request => Mismatch("https://eu.relocation-test.net:8443"));
            sites.On(App, "GET /users/2", request => Mismatch(Other));
            sites.On("eu.relocation-test.net:8443", "GET /users/1", request => Answer.Json("{\"id\":1}"));
            RegionRelocationPolicy policy = RegionRelocationPolicy.OnlyBetween($"https://{App}", "https://eu.relocation-test.net:8443")
                .OrBetween($"https://{Other}", $"https://{App}");
            FilesClient client = Client(policy: policy);

            User user = await client.Users.FindAsync(1);
            LockoutRegionMismatchException error = await Assert.ThrowsExactlyAsync<LockoutRegionMismatchException>(() => client.Users.FindAsync(2));

            Assert.AreEqual(1, user.Id);
            Assert.AreEqual(RegionRelocationRefusal.NotAllowed, error.Refusal);
            Assert.AreEqual("OnlyBetween(https://app.relocation-test.net -> https://eu.relocation-test.net:8443, https://other.relocation-test.net -> https://app.relocation-test.net)", policy.ToString());
            Assert.AreEqual("fromOrigin", Assert.ThrowsExactly<InvalidParameterException>(() => RegionRelocationPolicy.OnlyBetween("https://app.relocation-test.net/api", "https://eu.relocation-test.net")).ParamName);
            Assert.ThrowsExactly<InvalidOperationException>(() => RegionRelocationPolicy.Automatic.OrBetween($"https://{App}", $"https://{Eu}"));
        }

        // The TLS of the new address is checked before the request, with its credentials, is sent there.
        [TestMethod]
        [Timeout(60000)]
        public async Task ANewAddressWithoutATrustedCertificateGetsNothing()
        {
            sites.On(App, "GET /users/1", request => Mismatch(Impostor));
            FilesClient client = Client();

            await Assert.ThrowsExactlyAsync<ApiConnectionException>(() => client.Users.FindAsync(1));

            Assert.AreEqual(1, sites.Received.Count, "Only the first address received a request.");
            ScriptedConnection impostor = sites.Connections.Single(connection => connection.ServerName == Impostor);
            await impostor.Closed;
            Assert.AreEqual(0, impostor.Requests.Count, "The client closed the connection to the new address without sending anything over it.");
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task AFileGoesToTheNewAddressWithTheSameBytes()
        {
            sites.On(App, "PATCH /users/1", request => Mismatch(Eu));
            sites.On(Eu, "PATCH /users/1", request => Answer.Json("{\"id\":1}"));
            FilesClient client = Client();
            byte[] contents = Enumerable.Range(0, 100000).Select(i => (byte)i).ToArray();

            await client.Users.UpdateAsync(1, new Dictionary<string, object>
            {
                { "avatar_file", FileAttachment.FromStream(new MemoryStream(contents), "avatar.png") },
                { "notes", "" },
            });

            Assert.AreEqual(2, sites.Received.Count);
            CollectionAssert.AreEqual(sites.Received[0].Body, sites.Received[1].Body);
            Assert.IsTrue(sites.Received[1].Body.Length > contents.Length);
        }

        // A development site over plain HTTP moves only between the exact pair the application lists: the pair the
        // error names. An answer that names no other address has nowhere to move, and the error names no pair.
        [TestMethod]
        [Timeout(60000)]
        public async Task PlainHttpMovesOnlyBetweenAListedPair()
        {
            using (var plain = new Sites(null))
            {
                string local = $"http://127.0.0.1:{plain.Port}";
                string other = $"http://localhost:{plain.Port}";
                plain.On($"127.0.0.1:{plain.Port}", "GET /users/1", request => Mismatch(other));
                plain.On($"127.0.0.1:{plain.Port}", "GET /users/2", request => Mismatch(""));
                plain.On($"127.0.0.1:{plain.Port}", "GET /users/3", request => Mismatch(local));
                plain.On($"localhost:{plain.Port}", "GET /users/1", request => Answer.Json("{\"id\":1}"));
                var configuration = new FilesConfiguration { BaseUrl = local, ApiKey = "key-a", MaxNetworkRetries = 3, InitialNetworkRequestDelay = 0 };

                LockoutRegionMismatchException error = await Assert.ThrowsExactlyAsync<LockoutRegionMismatchException>(() => FilesClient.Create(configuration).Users.FindAsync(1));
                LockoutRegionMismatchException unnamed = await Assert.ThrowsExactlyAsync<LockoutRegionMismatchException>(() => FilesClient.Create(configuration).Users.FindAsync(2));
                LockoutRegionMismatchException same = await Assert.ThrowsExactlyAsync<LockoutRegionMismatchException>(() => FilesClient.Create(configuration).Users.FindAsync(3));
                configuration.RegionRelocation = RegionRelocationPolicy.OnlyBetween(local, other);
                User user = await FilesClient.Create(configuration).Users.FindAsync(1);

                string plainHttp = "the answer came over plain HTTP, whose server the client cannot verify.";
                Assert.AreEqual(RegionRelocationRefusal.UntrustedIssuer, error.Refusal);
                StringAssert.EndsWith(error.Message, $"The request was not sent to {other}: {plainHttp} If both addresses serve Files.com for the client's credentials, RegionRelocationPolicy.OnlyBetween(\"{local}\", \"{other}\") allows this move.");
                Assert.AreEqual(RegionRelocationRefusal.UntrustedIssuer, unnamed.Refusal);
                Assert.IsNull(unnamed.RequestedOrigin);
                StringAssert.EndsWith(unnamed.Message, $"The request was not sent elsewhere: {plainHttp} It also named no address that is a DNS name or an origin without a path, so there is nowhere to move.");
                Assert.AreEqual(RegionRelocationRefusal.UntrustedIssuer, same.Refusal);
                StringAssert.EndsWith(same.Message, $"The request was not sent to {local}: {plainHttp} It also named the address the request was sent to, so there is nowhere to move.");
                Assert.AreEqual(1, user.Id);
                Assert.AreEqual(5, plain.Received.Count);
            }
        }

        // An error of a transfer to storage never moves anything, whatever it says.
        [TestMethod]
        [Timeout(60000)]
        public async Task StorageErrorsNeverMoveAnything()
        {
            sites.On(App, "GET /files/remote.bin", request => Answer.Json($"{{\"path\":\"remote.bin\",\"download_uri\":\"https://{Other}/storage/remote.bin?X-Files-Signature=stand-in\"}}"));
            sites.On(Other, "GET /storage/remote.bin", request => Mismatch(Eu), rawPath: true);
            FilesClient client = Client();

            LockoutRegionMismatchException error = await Assert.ThrowsExactlyAsync<LockoutRegionMismatchException>(() => client.RemoteFiles.DownloadFileAsync("remote.bin", new MemoryStream()));

            Assert.IsNull(error.Refusal);
            Assert.AreEqual(2, sites.Received.Count);
        }

        // A chain policy that waives a check leaves a connection unverified, whatever else it trusts: its answer does not
        // move an operation automatically, though the waiver let the connection be made, here with a certificate that
        // would otherwise fail. A pair the application lists still moves it. Waiving a revocation status that cannot be
        // found relaxes nothing the default checks, and moves it.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("unknown authority", false, false)]
        [DataRow("not time valid", false, false)]
        [DataRow("fixed verification time", false, false)]
        [DataRow("unknown authority", true, true)]
        [DataRow("revocation unknown", false, true)]
        public async Task ChainPoliciesThatWaiveACheckDoNotMoveAnOperationAutomatically(string waiver, bool listedPair, bool moves)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            using (X509Certificate2 certificate = waiver == "unknown authority" ? stranger.Issue(App, Eu)
                : waiver == "revocation unknown" ? authority.Issue(App, Eu)
                : authority.Issue(now.AddHours(-20), now.AddHours(-1), App, Eu))
            using (var lax = new Sites(name => certificate))
            {
                lax.On(App, "GET /users/1", request => Mismatch(Eu));
                lax.On(Eu, "GET /users/1", request => Answer.Json("{\"id\":1}"));
                RegionRelocationPolicy policy = listedPair ? RegionRelocationPolicy.OnlyBetween($"https://{App}", $"https://{Eu}") : null;
                FilesClient client = Client(policy: policy, at: lax, configure: handler =>
                {
                    X509ChainPolicy chain = handler.SslOptions.CertificateChainPolicy;
                    switch (waiver)
                    {
                        case "unknown authority":
                            chain.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                            break;
                        case "not time valid":
                            chain.VerificationFlags = X509VerificationFlags.IgnoreNotTimeValid;
                            break;
                        case "fixed verification time":
                            chain.VerificationTime = now.AddHours(-10).UtcDateTime;
                            break;
                        default:
                            chain.VerificationFlags = X509VerificationFlags.IgnoreEndRevocationUnknown;
                            break;
                    }
                });

                if (moves)
                {
                    Assert.AreEqual(1, (await client.Users.FindAsync(1)).Id);
                    CollectionAssert.AreEqual(new[] { $"{App} GET /users/1?id=1", $"{Eu} GET /users/1?id=1" }, lax.Received.Select(request => request.ToString()).ToArray());
                }
                else
                {
                    LockoutRegionMismatchException error = await Assert.ThrowsExactlyAsync<LockoutRegionMismatchException>(() => client.Users.FindAsync(1));
                    Assert.AreEqual(RegionRelocationRefusal.UntrustedIssuer, error.Refusal);
                    Assert.AreEqual(1, lax.Received.Count, "The waiver let the connection be made; only the move was refused.");
                }
            }
        }

        // A connection made while a chain policy accepts an unknown authority stays in its handler when the policy is
        // tightened afterwards, which the handler's settings then no longer show: here one made by the handler's first
        // request, a client's own or, for a shared handler, the application's before it shared the handler. An answer
        // over it does not move an operation automatically, though the new address has a trusted certificate; a pair
        // the application lists still moves it.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("owned", false)]
        [DataRow("shared", false)]
        [DataRow("shared", true)]
        public async Task AHandlerThatConnectedWithAWaiverDoesNotMoveAnOperationOnceTightened(string transport, bool listedPair)
        {
            using (X509Certificate2 unknown = stranger.Issue(App))
            using (var lax = new Sites(name => name == App ? unknown : trusted))
            {
                lax.On(App, "GET /users/1", request => Answer.Json("{\"id\":1}"));
                lax.On(App, "GET /users/2", request => Mismatch(Eu));
                lax.On(Eu, "GET /users/2", request => Answer.Json("{\"id\":2}"));
                SocketsHttpHandler handler = null;
                HttpMessageHandler Waiving()
                {
                    handler = new SocketsHttpHandler { ConnectCallback = lax.ConnectAsync, AllowAutoRedirect = false };
                    handler.SslOptions.CertificateChainPolicy = authority.TrustPolicy();
                    handler.SslOptions.CertificateChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                    return handler;
                }
                var configuration = new FilesConfiguration { BaseUrl = $"https://{App}", ApiKey = "key-a", MaxNetworkRetries = 3, InitialNetworkRequestDelay = 0 };
                if (listedPair)
                {
                    configuration.RegionRelocation = RegionRelocationPolicy.OnlyBetween($"https://{App}", $"https://{Eu}");
                }
                FilesClient client;
                if (transport == "shared")
                {
                    using (var own = new HttpClient(Waiving(), disposeHandler: false))
                    {
                        (await own.GetAsync($"https://{App}/api/rest/v1/users/1")).Dispose();
                    }
                    client = FilesClient.Create(configuration, HttpTransport.FromSharedHandler(handler));
                }
                else
                {
                    client = FilesClient.Create(configuration, HttpTransport.FromHandlerFactory(Waiving));
                    await client.Users.FindAsync(1);
                }
                handler.SslOptions.CertificateChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;

                try
                {
                    if (listedPair)
                    {
                        Assert.AreEqual(2, (await client.Users.FindAsync(2)).Id);
                    }
                    else
                    {
                        LockoutRegionMismatchException error = await Assert.ThrowsExactlyAsync<LockoutRegionMismatchException>(() => client.Users.FindAsync(2));
                        Assert.AreEqual(RegionRelocationRefusal.UntrustedIssuer, error.Refusal);
                    }
                }
                finally
                {
                    await client.DisposeAsync();
                    handler.Dispose();
                }

                var expected = new List<string> { $"{App} GET /users/1", $"{App} GET /users/2?id=2" };
                if (transport == "owned")
                {
                    expected[0] += "?id=1";
                }
                if (listedPair)
                {
                    expected.Add($"{Eu} GET /users/2?id=2");
                }
                CollectionAssert.AreEqual(expected, lax.Received.Select(request => request.ToString()).ToList(), string.Join(" | ", lax.Received));
                Assert.AreEqual(1, lax.Connections.Count(connection => connection.ServerName == App), "Every request to the first address went over the connection made with the waiver.");
            }
        }

        // The same for a handler whose chain policy is relaxed while it connects, after the request started with it
        // strict: the answer over that connection does not move the operation.
        [TestMethod]
        [Timeout(60000)]
        public async Task AHandlerRelaxedWhileItConnectsDoesNotMoveThatOperation()
        {
            using (X509Certificate2 unknown = stranger.Issue(App))
            using (var lax = new Sites(name => name == App ? unknown : trusted))
            {
                lax.On(App, "GET /users/2", request => Mismatch(Eu));
                lax.On(Eu, "GET /users/2", request => Answer.Json("{\"id\":2}"));
                FilesClient client = Client(at: lax, configure: handler => handler.ConnectCallback = async (context, cancellationToken) =>
                {
                    handler.SslOptions.CertificateChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                    return await lax.ConnectAsync(context, cancellationToken);
                });

                LockoutRegionMismatchException error = await Assert.ThrowsExactlyAsync<LockoutRegionMismatchException>(() => client.Users.FindAsync(2));

                Assert.AreEqual(RegionRelocationRefusal.UntrustedIssuer, error.Refusal);
                Assert.AreEqual(1, lax.Received.Count, "The waiver let the connection be made; only the move was refused.");
            }
        }

        // A file whose download moves to the site's region keeps that address for its later operations, whether the
        // download is the generated operation or a helper that refreshes the file; another operation of the client
        // still starts at BaseUrl, where its 503 is not sent again, since the delete may have taken effect.
        [TestMethod]
        [Timeout(60000)]
        [DataRow("DownloadAsync")]
        [DataRow("GetDownloadUriWithLoadAsync")]
        [DataRow("DownloadFileAsync")]
        public async Task AFileKeepsTheAddressItsDownloadMovedTo(string download)
        {
            sites.On(App, "GET /files/item.bin", request => Mismatch(Eu));
            sites.On(App, "DELETE /files/item.bin", request => Answer.Status(503));
            sites.On(Eu, "GET /files/item.bin", request => Answer.Json($"{{\"path\":\"item.bin\",\"download_uri\":\"https://{Eu}/storage/item.bin?X-Files-Signature=stand-in\"}}"));
            sites.On(Eu, "GET /storage/item.bin", request => Answer.Json("contents"), rawPath: true);
            sites.On(Eu, "DELETE /files/item.bin", request => Answer.Json("{}"));
            FilesClient client = Client(retries: 1);
            RemoteFile file = client.RemoteFiles.New(new Dictionary<string, object> { { "path", "item.bin" } });

            switch (download)
            {
                case "DownloadAsync":
                    await file.DownloadAsync();
                    break;
                case "GetDownloadUriWithLoadAsync":
                    await file.GetDownloadUriWithLoadAsync();
                    break;
                default:
                    await file.DownloadFileAsync(new MemoryStream());
                    break;
            }
            await file.DeleteAsync();
            ApiException unavailable = await Assert.ThrowsExactlyAsync<ApiException>(() => client.RemoteFiles.DeleteAsync("item.bin"));
            Assert.IsTrue(unavailable.RequestMayHaveBeenProcessed);

            var expected = new List<string> { $"{App} GET /files/item.bin?path=item.bin", $"{Eu} GET /files/item.bin?path=item.bin" };
            if (download == "DownloadFileAsync")
            {
                expected.Add($"{Eu} GET /storage/item.bin?X-Files-Signature=stand-in");
            }
            expected.AddRange(new[] { $"{Eu} DELETE /files/item.bin?path=item.bin", $"{App} DELETE /files/item.bin?path=item.bin" });
            CollectionAssert.AreEqual(expected, sites.Received.Select(request => request.ToString()).ToList(), string.Join(" | ", sites.Received));
        }

        private FilesClient Client(int retries = 3, RegionRelocationPolicy policy = null, Action<SocketsHttpHandler> configure = null, Sites at = null, Func<HttpMessageHandler, HttpMessageHandler> wrap = null)
        {
            var configuration = new FilesConfiguration { BaseUrl = $"https://{App}", ApiKey = "key-a", MaxNetworkRetries = retries, InitialNetworkRequestDelay = 0, MaxNetworkRetryDelay = 0 };
            if (policy != null)
            {
                configuration.RegionRelocation = policy;
            }
            return FilesClient.Create(configuration, HttpTransport.FromHandlerFactory(() =>
            {
                var handler = new SocketsHttpHandler { ConnectCallback = (at ?? sites).ConnectAsync };
                handler.SslOptions.CertificateChainPolicy = authority.TrustPolicy();
                configure?.Invoke(handler);
                return wrap != null ? wrap(handler) : handler;
            }));
        }

        // A handler of the application's own in the transport's chain: it sends each request to host instead, as a
        // misconfigured proxy might, or, with no host, answers each itself, as a stub might, with a mismatch that does
        // not say which request it answers.
        private sealed class ApplicationHandler : DelegatingHandler
        {
            private readonly string host;

            internal ApplicationHandler(string host, HttpMessageHandler inner) : base(inner)
            {
                this.host = host;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (host == null)
                {
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden) { Content = new StringContent(MismatchJson($"{{\"host\":\"{Eu}\"}}"), Encoding.UTF8, "application/json") });
                }
                request.RequestUri = new UriBuilder(request.RequestUri) { Host = host }.Uri;
                return base.SendAsync(request, cancellationToken);
            }
        }

        private static Answer Mismatch(string host)
        {
            return Answer.Json(MismatchJson($"{{\"host\":{System.Text.Json.JsonSerializer.Serialize(host)}}}"), $"X-Files-Host: {host}\r\n", 403);
        }

        private static string MismatchJson(string data)
        {
            return "{\"error\":\"You are attempting to access a site in a different region.\",\"http-code\":403,\"title\":\"Lockout Region Mismatch\",\"type\":\"not-authenticated/lockout-region-mismatch\""
                + (data != null ? ",\"data\":" + data : "") + "}";
        }

        // A response of one of the test's sites.
        private sealed class Answer
        {
            internal int Code;
            internal string Body = "";
            internal string Headers = "";

            internal static Answer Json(string json, string headers = "", int status = 200)
            {
                return new Answer { Code = status, Body = json, Headers = headers };
            }

            internal static Answer Status(int status)
            {
                return new Answer { Code = status, Body = "{}" };
            }

            internal static Answer Redirect(int status, string location)
            {
                return new Answer { Code = status, Headers = $"Location: {location}\r\n" };
            }
        }

        private sealed class Received
        {
            internal string Host;
            internal string Method;
            internal string Path;
            internal string ServerName;
            internal Dictionary<string, string> Headers;
            internal byte[] Body;

            public override string ToString()
            {
                return $"{Host} {Method} {Path.Replace("/api/rest/v1", "")}";
            }
        }

        // Files.com sites on one loopback port, told apart by the Host of each request: over HTTPS, with the
        // certificate certificateFor gives for each name, or over plain HTTP when it is null.
        private sealed class Sites : IDisposable
        {
            private readonly ScriptedServer server;
            private readonly Dictionary<string, Func<Received, Answer>> routes = new Dictionary<string, Func<Received, Answer>>();
            private readonly List<Received> received = new List<Received>();

            internal Sites(Func<string, X509Certificate2> certificateFor)
            {
                server = new ScriptedServer(ServeAsync, certificateFor);
            }

            internal int Port => server.Port;

            internal IReadOnlyList<ScriptedConnection> Connections => server.Connections;

            internal List<Received> Received
            {
                get
                {
                    lock (received)
                    {
                        return received.ToList();
                    }
                }
            }

            // Answers requests to host (with its port, when not the default) for route, a method and a path relative
            // to /api/rest/v1, or, when rawPath, the path as it is.
            internal void On(string host, string route, Func<Received, Answer> answer, bool rawPath = false)
            {
                string[] parts = route.Split(' ');
                routes[$"{host} {parts[0]} {(rawPath ? "" : "/api/rest/v1")}{parts[1]}"] = answer;
            }

            // Connects to these sites whatever name and port the client asks for.
            internal async ValueTask<Stream> ConnectAsync(SocketsHttpConnectionContext context, CancellationToken cancellationToken)
            {
                var socket = new Socket(SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };
                try
                {
                    await socket.ConnectAsync(new IPEndPoint(IPAddress.Loopback, server.Port), cancellationToken);
                    return new NetworkStream(socket, ownsSocket: true);
                }
                catch
                {
                    socket.Dispose();
                    throw;
                }
            }

            public void Dispose()
            {
                server.Dispose();
            }

            private async Task ServeAsync(ScriptedConnection connection)
            {
                while (true)
                {
                    string line;
                    try
                    {
                        line = await connection.ReadRequestHeadAsync();
                    }
                    catch (EndOfStreamException)
                    {
                        return;
                    }
                    string[] parts = line.Split(' ');
                    var headers = connection.HeaderLines
                        .Select(header => header.Split(new[] { ':' }, 2))
                        .ToDictionary(header => header[0].Trim(), header => header[1].Trim(), StringComparer.OrdinalIgnoreCase);
                    var request = new Received
                    {
                        Host = headers["Host"],
                        Method = parts[0],
                        Path = parts[1],
                        ServerName = connection.ServerName,
                        Headers = headers,
                        Body = await connection.ReadBodyAsync(),
                    };
                    lock (received)
                    {
                        received.Add(request);
                    }
                    Answer answer = routes.TryGetValue($"{request.Host} {request.Method} {request.Path.Split('?')[0]}", out Func<Received, Answer> route)
                        ? route(request)
                        : Answer.Json("{\"error\":\"Not Found\",\"http-code\":404,\"type\":\"not-found\"}", status: 404);
                    await connection.RespondAsync(answer.Code, answer.Body, answer.Headers);
                }
            }
        }
    }
}