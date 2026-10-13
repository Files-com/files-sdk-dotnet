using FilesCom;
using FilesCom.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FilesTests
{
    // Every failure the SDK reports is an SdkException: an error response is an ApiException of the class its error
    // type names, whatever its status; a response that cannot be read is an InvalidResponseException; incorrect use
    // is found before anything is sent. Messages never include a response body.
    [TestClass]
    public class SdkErrorsTest
    {
        private const string Secret = "https://storage.example.test/file?signature=secret-signature";
        private const string SecretWithUser = "https://user:secret-password@storage.example.test/file?signature=secret-signature#secret-fragment";
        // Apostrophes are legal in a URL's user information and path.
        private const string ApostropheInPassword = "https://user:private'secret@bucket.example/key.txt?token=PRIVATE_QUERY#PRIVATE_FRAGMENT";
        private const string ApostropheInPath = "https://bucket.example/O'Brien/key.txt?token=PRIVATE_QUERY#PRIVATE_FRAGMENT";
        // Each URL above as an error message shows it: host and path, without user information, query or fragment.
        private static readonly (string Url, string Shown)[] ShownUrls =
        {
            (Secret, "https://storage.example.test/file?[redacted]"),
            (SecretWithUser, "https://[redacted]@storage.example.test/file?[redacted]#[redacted]"),
            (ApostropheInPassword, "https://[redacted]@bucket.example/key.txt?[redacted]#[redacted]"),
            (ApostropheInPath, "https://bucket.example/O'Brien/key.txt?[redacted]#[redacted]"),
        };
        private LoopbackSite site;

        [TestInitialize]
        public void StartSite()
        {
            site = new LoopbackSite("key-a", "11");
        }

        [TestCleanup]
        public void StopSite()
        {
            site.Dispose();
        }

        [TestMethod]
        [Timeout(60000)]
        public async Task ErrorResponsesAreThrownByTheirTypeNeverByTheirStatus()
        {
            var responses = new (int Status, string Body, Type Expected)[]
            {
                (404, @"{""type"":""not-found"",""error"":""Not Found""}", typeof(NotFoundException)),
                (403, @"{""type"":""not-found"",""error"":""Not Found""}", typeof(NotFoundException)),
                (500, @"{""type"":""not-found"",""error"":""Not Found""}", typeof(NotFoundException)),
                (403, @"{""type"":""not-authorized/folder-admin-permission-required"",""error"":""Denied""}", typeof(FolderAdminPermissionRequiredException)),
                (404, @"{""type"":""not-found/a-type-added-later"",""error"":""Gone""}", typeof(NotFoundException)),
                (400, @"{""type"":""no-such-type"",""error"":""Unknown""}", typeof(ApiException)),
                (400, @"{""type"":"""",""error"":""Blank""}", typeof(ApiException)),
                (400, @"{""type"":""-"",""error"":""Dash""}", typeof(ApiException)),
                (400, @"{""type"":""/not-found"",""error"":""No group""}", typeof(ApiException)),
                (400, @"{""type"":""not found"",""error"":""Space""}", typeof(ApiException)),
                (500, @"{""error"":""No type""}", typeof(ApiException)),
                (502, "null", typeof(InvalidResponseException)),
                (502, "", typeof(InvalidResponseException)),
                (502, "[]", typeof(InvalidResponseException)),
                (502, @"{""type"":7,""error"":""Not a string""}", typeof(InvalidResponseException)),
                (502, "<html>Bad gateway for " + Secret + "</html>", typeof(InvalidResponseException)),
                // An ordinary error that quotes a signed URL, with a type and without one.
                (404, @"{""type"":""not-found"",""error"":""Nothing at " + Secret + @"""}", typeof(NotFoundException)),
                (500, @"{""error"":""Could not reach " + SecretWithUser + @"""}", typeof(ApiException)),
                (404, @"{""type"":""not-found"",""error"":""No access for " + ApostropheInPassword + @"""}", typeof(NotFoundException)),
                (500, @"{""error"":""Could not read '" + ApostropheInPath + @"'.""}", typeof(ApiException)),
                // A long word, for text that is read once however long it is, before a URL.
                (500, @"{""error"":""" + string.Concat(Enumerable.Repeat("a.", 100000)) + " at " + Secret + @"""}", typeof(ApiException)),
                // The JSON property path of the field that cannot be read names the secret.
                (400, @"{""error"":""Bad"",""model_errors"":{""" + Secret + @""":""not-an-array""}}", typeof(InvalidResponseException)),
            };
            FilesClient client = FilesClient.Create(site.Configuration());

            for (int id = 0; id < responses.Length; id++)
            {
                (int status, string body, Type expected) = responses[id];
                site.On("GET", "/users/" + id, request =>
                {
                    var response = LoopbackSite.Json(body, status: status);
                    response.AddHeader("X-Request-Id", "request-" + request.Id);
                    return response;
                });
                string row = $"HTTP {status} {body}";

                SdkException error = await CatchAsync(() => client.Users.FindAsync(id));

                Assert.AreEqual(expected, error.GetType(), row);
                Assert.IsFalse(Diagnostics(error).Contains("secret") || Diagnostics(error).Contains("PRIVATE"), row);
                if (error is ApiException apiError)
                {
                    Assert.AreEqual(status, apiError.httpStatus, row);
                    Assert.AreEqual("request-" + id, apiError.httpHeaders.GetValues("X-Request-Id").Single(), row);
                    // The error keeps the text as it came; the message keeps each URL's host and path, not its credentials.
                    Assert.AreEqual(JsonDocument.Parse(body).RootElement.GetProperty("error").GetString(), apiError.error, row);
                    Assert.AreEqual(ShownUrls.Aggregate(apiError.error, (text, url) => text.Replace(url.Url, url.Shown)), apiError.Message, row);
                }
                else
                {
                    var invalid = (InvalidResponseException)error;
                    Assert.IsInstanceOfType(invalid, typeof(ApiConnectionException), row);
                    Assert.AreEqual(status, invalid.httpStatus, row);
                    Assert.AreEqual("request-" + id, invalid.httpHeaders.GetValues("X-Request-Id").Single(), row);
                }
            }

            Assert.AreEqual(responses.Length, site.Requests.Count, "Failed responses are not retried when retries are off.");
        }

        [TestMethod]
        public async Task SuccessfulResponsesThatCannotBeReadAreInvalidResponses()
        {
            site.On("GET", "/users/1", request => LoopbackSite.Json("{\"id\":1,\"ftp_permission\":\" True \"}"));
            site.On("GET", "/users/2", request => LoopbackSite.Json("{\"id\":2,\"ftp_permission\":\"" + Secret + "\"}"));
            site.On("GET", "/users/3", request => LoopbackSite.Json("{\"id\":3,\"ftp_permission\":1}"));
            site.On("GET", "/metadata_categories/1", request => LoopbackSite.Json("{\"id\":1,\"definitions\":{\"" + Secret + "\":\"not-an-array\"}}"));
            site.On("GET", "/remote_mount_backends/1", request => LoopbackSite.Json("{\"id\":1,\"min_free_cpu\":\"79228162514264337593543950336\"}"));
            // No object where the operation returns one, or an array of them.
            site.On("GET", "/users/4", request => LoopbackSite.Json("null"));
            site.On("GET", "/users/5", request => LoopbackSite.Json("{}"));
            site.On("POST", "/workspaces", request => LoopbackSite.Json("null"));
            site.On("POST", "/file_actions/begin_upload/a.txt", request => LoopbackSite.Json("[{\"ref\":\"upload-ref\"},null]"));
            site.On("POST", "/file_actions/begin_upload/b.txt", request => LoopbackSite.Json("null"));
            FilesClient client = FilesClient.Create(site.Configuration());
            var unreadable = new (string Case, Func<Task> Find)[]
            {
                ("a boolean that is other text", () => client.Users.FindAsync(2)),
                ("a boolean that is a number", () => client.Users.FindAsync(3)),
                ("a key naming the secret", () => client.MetadataCategories.FindAsync(1)),
                ("a decimal too large, as text", () => client.RemoteMountBackends.FindAsync(1)),
                ("null for a model", () => client.Users.FindAsync(4)),
                ("null for a created model", () => client.Workspaces.CreateAsync(new Dictionary<string, object> { { "name", "created" } })),
                ("an array of models holding a null", () => client.RemoteFiles.BeginUploadAsync("a.txt")),
                ("null for an array of models", () => client.RemoteFiles.BeginUploadAsync("b.txt")),
            };

            Assert.IsTrue((await client.Users.FindAsync(1)).FtpPermission, "A boolean sent as text is read as before.");
            Assert.IsNull((await client.Users.FindAsync(5)).Id, "An empty object is a model, with no attributes set.");
            foreach ((string name, Func<Task> find) in unreadable)
            {
                SdkException error = await CatchAsync(find);

                Assert.AreEqual(typeof(InvalidResponseException), error.GetType(), name);
                Assert.AreEqual(0, ((InvalidResponseException)error).httpStatus, name);
                Assert.IsFalse(Diagnostics(error).Contains("secret"), name);
            }

            // Saving keeps the object's own values when Files.com answers with no object, so saving again sends them.
            Workspace draft = client.Workspaces.New(new Dictionary<string, object> { { "name", "draft" } });
            await Assert.ThrowsExactlyAsync<InvalidResponseException>(() => draft.SaveAsync());
            await Assert.ThrowsExactlyAsync<InvalidResponseException>(() => draft.SaveAsync());
            Assert.AreEqual("draft", draft.Name);
            Assert.IsNull(draft.Id);
            CollectionAssert.AreEqual(
                new[] { "{\"name\":\"created\"}", "{\"name\":\"draft\"}", "{\"name\":\"draft\"}" },
                site.Requests.Where(request => request.Path == "/workspaces").Select(request => request.Body).ToArray());
        }

        [TestMethod]
        public async Task IncorrectUseIsReportedBeforeAnyRequest()
        {
            FilesClient client = FilesClient.Create(site.Configuration());
            FilesClient withoutCredentials = FilesClient.Create(new FilesConfiguration { BaseUrl = site.Url, MaxNetworkRetries = 0 });
            User found = client.Users.New(new Dictionary<string, object> { { "id", 1L } });
            found.Notes = null;
            var uses = new (Type Expected, string Context, Func<Task> Use)[]
            {
                (typeof(AuthenticationException), "GET /users", () => withoutCredentials.Users.FindAsync(1)),
                (typeof(MissingParameterException), "id", () => client.Users.FindAsync(null)),
                (typeof(MissingParameterException), "id", () => client.Users.New().UpdateAsync()),
                (typeof(InvalidParameterException), "username must be of type string", () => client.Users.CreateAsync(new Dictionary<string, object> { { "username", 5 } })),
                (typeof(InvalidParameterException), "session_id must be of type string", () => client.Users.FindAsync(1, null, new Dictionary<string, object> { { "session_id", 5L } })),
                (typeof(InvalidParameterException), "notes is null, which cannot be sent", () => found.SaveAsync()),
            };

            foreach ((Type expected, string context, Func<Task> use) in uses)
            {
                SdkException error = await CatchAsync(use);

                Assert.AreEqual(expected, error.GetType(), context);
                StringAssert.Contains(error.Message, context);
                Assert.IsFalse(error.Message.Contains(site.ApiKey), context);
            }

            // An argument, option or setting the SDK cannot use names itself, whichever entry point refuses it.
            var named = new (Type Expected, string Parameter, Func<Task> Use)[]
            {
                (typeof(MissingParameterException), "createHandler", () => { HttpTransport.FromHandlerFactory(null); return Task.CompletedTask; }),
                (typeof(MissingParameterException), "handler", () => { HttpTransport.FromSharedHandler(null); return Task.CompletedTask; }),
                (typeof(InvalidParameterException), "handler", () => { HttpTransport.FromSharedHandler(new System.Net.Http.HttpClientHandler()); return Task.CompletedTask; }),
                (typeof(InvalidParameterException), "createHandler", () => FilesClient.Create(site.Configuration(), HttpTransport.FromHandlerFactory(() => null)).Users.FindAsync(1)),
                (typeof(InvalidParameterException), nameof(FilesConfiguration.MaxNetworkRetries), () => { FilesClient.Create(new FilesConfiguration { MaxNetworkRetries = -1 }); return Task.CompletedTask; }),
                (typeof(MissingParameterException), "localPath", () => client.RemoteFiles.UploadFileAsync(null)),
                (typeof(MissingParameterException), "localPath", () => RemoteFile.UploadFile("")),
            };
            foreach ((Type expected, string parameter, Func<Task> use) in named)
            {
                SdkException error = await CatchAsync(use);

                Assert.AreEqual(expected, error.GetType(), parameter);
                Assert.AreEqual(parameter, (error as MissingParameterException)?.ParamName ?? (error as InvalidParameterException)?.ParamName);
            }

            // Before any FilesClient has been made there is no default client. A static API request that sends credentials,
            // with none in its options, has no credentials at all; everything else lacks only the client, and says so,
            // without echoing credentials its options carry.
            const string optionKey = "option-key-not-echoed";
            var noClient = new (Type Expected, string Context, Func<Task> Use)[]
            {
                (typeof(AuthenticationException), "no FilesClient has been made", () => User.Find(1)),
                (typeof(SdkException), "No FilesClient has been made", () => User.Find(1, null, new Dictionary<string, object> { { "api_key", optionKey } })),
                (typeof(SdkException), "No FilesClient has been made", () => Session.Create()),
                (typeof(SdkException), "No FilesClient has been made", () => new User(new Dictionary<string, object> { { "id", 1L } }, null).DeleteAsync()),
                (typeof(SdkException), "No FilesClient has been made", () => FilesClient.StreamDownload(site.Url + "/download", new System.IO.MemoryStream())),
                (typeof(SdkException), "No FilesClient has been made", () => FilesClient.ChunkUpload(System.Net.Http.HttpMethod.Put, site.Url + "/part", new System.IO.MemoryStream(new byte[] { 1 }), 1)),
            };
            System.Reflection.PropertyInfo instance = typeof(FilesClient).GetProperty(nameof(FilesClient.Instance));
            FilesClient defaultClient = FilesClient.Instance;
            instance.SetValue(null, null);
            try
            {
                foreach ((Type expected, string context, Func<Task> use) in noClient)
                {
                    SdkException error = await CatchAsync(use);

                    Assert.AreEqual(expected, error.GetType(), context);
                    StringAssert.Contains(error.Message, context);
                    Assert.IsFalse(error.Message.Contains(optionKey), context);
                }
            }
            finally
            {
                instance.SetValue(null, defaultClient);
            }
            site.AssertReceived();
        }

        // The types and messages of an exception and of all its inner exceptions, as a structured error reporter records them.
        private static string Diagnostics(Exception error)
        {
            var diagnostics = new List<string>();
            for (Exception e = error; e != null; e = e.InnerException)
            {
                diagnostics.Add(e.GetType().FullName + ": " + e.Message);
            }
            return string.Join("\n", diagnostics);
        }

        private static async Task<SdkException> CatchAsync(Func<Task> operation)
        {
            try
            {
                await operation();
            }
            catch (SdkException e)
            {
                return e;
            }
            Assert.Fail("The operation succeeded instead of throwing an SdkException.");
            return null;
        }
    }
}