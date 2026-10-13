using FilesCom;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WireMock;

namespace FilesTests
{
    // Where each client logs (FilesConfiguration.LoggerFactory and MinimumLogLevel), and the product an application
    // names in its client's User-Agent (ProductName and ProductVersion).
    [TestClass]
    public class ClientConfigurationTest
    {
        [TestMethod]
        [Timeout(60000)]
        public async Task EachClientLogsWhereItsConfigurationSays()
        {
            using (var siteA = new LoopbackSite("key-a", "11"))
            using (var siteB = new LoopbackSite("key-b", "22"))
            using (var log4Net = new LogCapture())
            {
                ServeUserAfterAThrottle(siteA);
                ServeUserAfterAThrottle(siteB);
                var loggedA = new CapturingLoggerFactory(LogLevel.Debug);
                var loggedB = new CapturingLoggerFactory(LogLevel.Debug);
                FilesClient clientA = FilesClient.Create(Configured(siteA, loggedA));
                FilesClient clientB = FilesClient.Create(Configured(siteB, loggedB));

                await clientA.Users.FindAsync(1);

                Assert.IsTrue(loggedA.Entries.Any(entry => entry.Category == "FilesCom.Http.RetryPolicy" && entry.Level == LogLevel.Information && entry.Message.StartsWith("Retrying")), "Client A's retry is logged through its factory: " + string.Join(" | ", loggedA.Entries.Select(e => e.Message)));
                Assert.IsTrue(loggedA.Entries.Any(entry => entry.Message == "Files.com Client created with API Key"), "So is its creation.");
                Assert.IsFalse(loggedB.Entries.Any(entry => entry.Message.StartsWith("Retrying")), "Nothing of client A's reaches client B's factory.");
                Assert.IsFalse(log4Net.Normal.Contains("Retrying") || log4Net.Debug.Contains("Retrying"), "Nor log4net, which nothing configured globally.");

                await clientB.Users.FindAsync(1);

                Assert.IsTrue(loggedB.Entries.Any(entry => entry.Message.StartsWith("Retrying")));
                Assert.IsTrue(loggedB.Entries.Any(entry => entry.Message.Contains("\"username\":\"22\"")), "Client B's answer is logged through its own factory, at DEBUG.");
                // Client B's answer names its workspace as the user: "22" alone could be part of a loopback port.
                Assert.IsFalse(loggedA.Entries.Any(entry => entry.Message.Contains("\"username\":\"22\"")), "Client B's operation is not logged through client A's factory.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task MinimumLogLevelKeepsOneClientQuiet()
        {
            using (var site = new LoopbackSite("key-a", "11"))
            {
                ServeUserAfterAThrottle(site);
                var logged = new CapturingLoggerFactory(LogLevel.Debug);
                FilesConfiguration configuration = Configured(site, logged);
                configuration.MinimumLogLevel = LogLevel.Warning;

                await FilesClient.Create(configuration).Users.FindAsync(1);

                Assert.IsFalse(logged.Entries.Any(entry => entry.Level < LogLevel.Warning), "Only warnings and errors: " + string.Join(" | ", logged.Entries.Select(e => e.Message)));
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task DebugDetailsAreLoggedOnlyWhereDebugIsEnabled()
        {
            using (var site = new LoopbackSite("key-a", "11"))
            {
                site.On("GET", "/users/*", request => LoopbackSite.Json("{\"error\":\"Not Found\",\"http-code\":404,\"type\":\"not-found\",\"detail\":\"SYNTHETIC_DETAIL\"}", status: 404));
                var information = new CapturingLoggerFactory(LogLevel.Information);
                var debug = new CapturingLoggerFactory(LogLevel.Debug);

                await Assert.ThrowsAsync<NotFoundException>(() => FilesClient.Create(Configured(site, information)).Users.FindAsync(1));
                await Assert.ThrowsAsync<NotFoundException>(() => FilesClient.Create(Configured(site, debug)).Users.FindAsync(1));

                Assert.IsFalse(information.Entries.Any(entry => entry.Message.Contains("SYNTHETIC_DETAIL")), "A response body is a Debug detail.");
                Assert.IsTrue(debug.Entries.Any(entry => entry.Level == LogLevel.Debug && entry.Message.Contains("SYNTHETIC_DETAIL")), "Debug has it when enabled.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task WithoutALoggerFactoryLog4NetLogsAsBefore()
        {
            using (var site = new LoopbackSite("key-a", "11"))
            using (var log4Net = new LogCapture())
            {
                ServeUserAfterAThrottle(site);

                await FilesClient.Create(site.Configuration(maxNetworkRetries: 1)).Users.FindAsync(1);

                StringAssert.Contains(log4Net.Normal, "Retrying");
            }
        }

        // Debug records request and response bodies, without the values of their credential members: a sign-in's
        // password and one-time password, the session it creates, and the key a new API key is made with.
        [TestMethod]
        [Timeout(60000)]
        public async Task DebugLeavesOutTheCredentialsInRequestAndResponseBodies()
        {
            using (var site = new LoopbackSite("key-a", "11"))
            {
                site.On("POST", "/sessions", request => LoopbackSite.Json("{\"id\":\"SESSION_SENTINEL\",\"language\":\"en\"}"));
                site.On("POST", "/api_keys", request => LoopbackSite.Json("{\"id\":7,\"name\":\"automation\",\"key\":\"KEY_SENTINEL\"}"));
                var debug = new CapturingLoggerFactory(LogLevel.Debug);
                FilesClient client = FilesClient.Create(Configured(site, debug));

                await client.Sessions.CreateAsync(new Dictionary<string, object> { { "username", "USER_SENTINEL" }, { "password", "PASSWORD_SENTINEL" }, { "otp", "OTP_SENTINEL" } });
                await client.ApiKeys.CreateAsync(new Dictionary<string, object> { { "name", "automation" } });

                string logged = string.Join(" | ", debug.Entries.Where(entry => entry.Level == LogLevel.Debug).Select(entry => entry.Message));
                StringAssert.Contains(logged, "USER_SENTINEL", "The request body is still recorded at Debug.");
                StringAssert.Contains(logged, "\"password\":\"[redacted]\"", "With its credential members' values left out.");
                StringAssert.Contains(logged, "\"name\":\"automation\"", "The response body is still recorded.");
                foreach (string secret in new[] { "PASSWORD_SENTINEL", "OTP_SENTINEL", "SESSION_SENTINEL", "KEY_SENTINEL" })
                {
                    Assert.IsFalse(logged.Contains(secret), $"{secret} is never logged: {logged}");
                }
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task ALoggerThatAlwaysThrowsChangesNoOperationsOutcome()
        {
            using (var site = new LoopbackSite("key-a", "11"))
            {
                ServeUserAfterAThrottle(site);
                var throwing = new CapturingLoggerFactory(LogLevel.Debug) { Throws = true };

                FilesCom.Models.User user = await FilesClient.Create(Configured(site, throwing)).Users.FindAsync(1);

                Assert.AreEqual(1L, user.Id, "The retried request succeeds though every message the SDK logged failed.");
                Assert.IsTrue(throwing.Attempts > 0, "The SDK did log, through the failing logger.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task OpeningARemoteFileToWriteLeavesTheCallersLoggingAlone()
        {
            using (var server = new UploadServer())
            using (var log4Net = new LogCapture())
            using (var cancellation = new CancellationTokenSource())
            {
                var logged = new CapturingLoggerFactory(LogLevel.Debug);
                FilesConfiguration configuration = server.Site.Configuration();
                configuration.LoggerFactory = logged;
                FilesClient client = FilesClient.Create(configuration);
                SdkLog outside = SdkLog.For(typeof(ClientConfigurationTest));

                System.IO.Stream stream = await client.FileSystem.OpenWriteAsync("remote.bin", cancellationToken: cancellation.Token);
                outside.Warn("CALLER-AFTER-OPEN");
                await stream.WriteAsync(new byte[] { 1, 2, 3 });
                cancellation.Cancel();
                stream.Dispose();
                outside.Warn("CALLER-AFTER-DISPOSE");

                Assert.IsFalse(logged.Entries.Any(entry => entry.Message.StartsWith("CALLER-", StringComparison.Ordinal)), "The caller's own messages are not logged through the client's factory: " + string.Join(" | ", logged.Entries.Select(e => e.Message)));
                StringAssert.Contains(log4Net.Normal, "CALLER-AFTER-OPEN", "Outside any operation, messages go to log4net.");
                StringAssert.Contains(log4Net.Normal, "CALLER-AFTER-DISPOSE");
                Assert.AreEqual(0, server.Finishes.Count, "The abandoned write is not completed.");
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task TheApplicationsProductComesBeforeTheSdksOwn()
        {
            using (var site = new LoopbackSite("key-a", "11"))
            {
                site.On("GET", "/users/*", request => LoopbackSite.Json("{\"id\":1}"));
                FilesConfiguration configuration = site.Configuration();
                configuration.ProductName = "FilesPowerShell";
                configuration.ProductVersion = "3.1.0-beta.2";
                FilesClient client = FilesClient.Create(configuration);
                configuration.ProductName = "Changed";

                await client.Users.FindAsync(1);
                await FilesClient.Create(site.Configuration()).Users.FindAsync(1);

                string[] agents = site.Requests.Select(request => request.Headers["User-Agent"]).ToArray();
                StringAssert.StartsWith(agents[0], "FilesPowerShell/3.1.0-beta.2 Files.com DOTNET SDK v", "The client keeps the product it was made with, then the SDK's own.");
                StringAssert.StartsWith(agents[1], "Files.com DOTNET SDK v", "Without a product, the SDK's alone.");
            }
        }

        [TestMethod]
        [DataRow("Files PowerShell", "1.0", DisplayName = "A name with a space")]
        [DataRow("FilesPowerShell", "1.0\r\nX-Injected: yes", DisplayName = "A version with a line break")]
        [DataRow("Files/PowerShell", null, DisplayName = "A name with a slash")]
        [DataRow(null, "1.0", DisplayName = "A version without a name")]
        public void AProductThatIsNotAnHttpTokenIsRefused(string name, string version)
        {
            var configuration = new FilesConfiguration { ApiKey = "key", ProductName = name, ProductVersion = version };

            Assert.ThrowsExactly<InvalidParameterException>(() => FilesClient.Create(configuration));
        }

        private static FilesConfiguration Configured(LoopbackSite site, ILoggerFactory factory)
        {
            FilesConfiguration configuration = site.Configuration(maxNetworkRetries: 1);
            configuration.LoggerFactory = factory;
            return configuration;
        }

        // GET /users/1: throttled once, then answered.
        private static void ServeUserAfterAThrottle(LoopbackSite site)
        {
            int requests = 0;
            site.On("GET", "/users/*", request => Interlocked.Increment(ref requests) == 1
                ? LoopbackSite.Json("{\"error\":\"Slow down\",\"http-code\":429,\"type\":\"too-many-requests\"}", status: 429)
                : LoopbackSite.Json($"{{\"id\":1,\"username\":\"{site.WorkspaceId}\"}}"));
        }

        private sealed class CapturingLoggerFactory : ILoggerFactory
        {
            private readonly LogLevel enabled;
            private readonly List<(string Category, LogLevel Level, string Message)> entries = new List<(string, LogLevel, string)>();
            private int attempts;

            // Whether every Log call throws, as a failing logging provider's would.
            internal bool Throws { get; set; }

            internal int Attempts => Volatile.Read(ref attempts);

            internal CapturingLoggerFactory(LogLevel enabled)
            {
                this.enabled = enabled;
            }

            internal IReadOnlyList<(string Category, LogLevel Level, string Message)> Entries
            {
                get
                {
                    lock (entries)
                    {
                        return entries.ToList();
                    }
                }
            }

            public ILogger CreateLogger(string categoryName)
            {
                return new Logger(this, categoryName);
            }

            public void AddProvider(ILoggerProvider provider)
            {
            }

            public void Dispose()
            {
            }

            private sealed class Logger : ILogger
            {
                private readonly CapturingLoggerFactory factory;
                private readonly string category;

                internal Logger(CapturingLoggerFactory factory, string category)
                {
                    this.factory = factory;
                    this.category = category;
                }

                public IDisposable BeginScope<TState>(TState state) where TState : notnull
                {
                    return null;
                }

                public bool IsEnabled(LogLevel logLevel)
                {
                    return logLevel >= factory.enabled;
                }

                public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
                {
                    Interlocked.Increment(ref factory.attempts);
                    if (factory.Throws)
                    {
                        throw new InvalidOperationException("The logging provider failed.");
                    }
                    lock (factory.entries)
                    {
                        factory.entries.Add((category, logLevel, formatter(state, exception)));
                    }
                }
            }
        }
    }
}