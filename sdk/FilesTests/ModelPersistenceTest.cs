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
    // An object's SaveAsync and UpdateAsync change that object: when Files.com accepts the request, the object takes
    // the attributes Files.com returns, with one request per operation and no request to read it again. When the
    // request fails, the object keeps its own values. SaveAsync sends only the attributes set on the object.
    [TestClass]
    public class ModelPersistenceTest
    {
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
        public async Task SaveAndUpdateRefreshTheObjectWithWhatFilesComReturns()
        {
            // Files.com returns its canonical form of each name.
            site.On("POST", "/workspaces", request => LoopbackSite.Json("{\"id\":7,\"name\":\"Projects\"}"));
            var patches = new Queue<(int Status, string Json)>(new[]
            {
                (200, "{\"id\":7,\"name\":\"Renamed\"}"),
                (200, "{\"id\":7,\"name\":\"Saved\"}"),
                (422, "{\"type\":\"bad-request/request-params-invalid\",\"error\":\"Name is invalid\"}"),
                (422, "{\"type\":\"bad-request/request-params-invalid\",\"error\":\"Name is invalid\"}"),
            });
            site.On("PATCH", "/workspaces/7", request =>
            {
                (int status, string json) = patches.Dequeue();
                return LoopbackSite.Json(json, status: status);
            });
            site.On("DELETE", "/workspaces/7", request => LoopbackSite.Status(204));
            FilesClient client = FilesClient.Create(site.Configuration());

            Workspace workspace = client.Workspaces.New(new Dictionary<string, object> { { "name", " projects " } });
            await workspace.SaveAsync();
            Assert.AreEqual(7, workspace.Id);
            Assert.AreEqual("Projects", workspace.Name);

            Workspace returned = await workspace.UpdateAsync(new Dictionary<string, object> { { "name", "renamed" } });
            Assert.AreNotSame(workspace, returned);
            Assert.AreEqual("Renamed", workspace.Name);
            Assert.AreEqual("Renamed", returned.Name);

            workspace.Name = "saved";
            await workspace.SaveAsync();
            Assert.AreEqual("Saved", workspace.Name);

            workspace.Name = "!";
            await Assert.ThrowsExactlyAsync<RequestParamsInvalidException>(() => workspace.SaveAsync());
            await Assert.ThrowsExactlyAsync<RequestParamsInvalidException>(() => workspace.UpdateAsync(new Dictionary<string, object> { { "name", "?" } }));
            Assert.AreEqual(7, workspace.Id);
            Assert.AreEqual("!", workspace.Name, "A failed request leaves the object's values as they were.");

            await workspace.DestroyAsync();

            site.AssertReceived(
                "POST /workspaces key=key-a workspace=11",
                "PATCH /workspaces/7 key=key-a workspace=11",
                "PATCH /workspaces/7 key=key-a workspace=11",
                "PATCH /workspaces/7 key=key-a workspace=11",
                "PATCH /workspaces/7 key=key-a workspace=11",
                "DELETE /workspaces/7?id=7 key=key-a workspace=11");
            CollectionAssert.AreEqual(
                new[] { "name= projects ", "id=7 name=renamed", "id=7 name=saved", "id=7 name=!", "id=7 name=?" },
                site.Requests.Take(5).Select(request => Fields(request.Body)).ToArray());
        }

        [TestMethod]
        public async Task SaveSendsOnlyTheAttributesSetOnTheObject()
        {
            site.On("POST", "/users", request => LoopbackSite.Json("{\"id\":8,\"username\":\"new-user\",\"ftp_permission\":true}"));
            site.On("GET", "/users/9", request => LoopbackSite.Json(
                "{\"id\":9,\"username\":\"found\",\"notes\":null,\"authenticate_until\":\"2026-10-01T00:00:00Z\",\"group_ids\":\"1,2\"}"));
            site.On("PATCH", "/users/9", request => LoopbackSite.Json("{\"id\":9,\"username\":\"found\",\"name\":\"Found User\"}"));
            FilesClient client = FilesClient.Create(site.Configuration());

            // Only the required username: none of the defaults of the other attributes are sent.
            User required = client.Users.New(new Dictionary<string, object> { { "username", "new-user" } });
            await required.SaveAsync();
            Assert.AreEqual(8, required.Id);
            Assert.IsTrue(required.FtpPermission);

            // False, zero and empty values that are set are sent.
            User explicitValues = client.Users.New();
            explicitValues.Username = "explicit";
            explicitValues.Notes = "";
            explicitValues.FtpPermission = false;
            explicitValues.PasswordValidityDays = 0;
            await explicitValues.SaveAsync();

            // A found user sends back only what was changed, not the values Files.com gave it.
            User found = await client.Users.FindAsync(9);
            found.Name = "Found User";
            await found.SaveAsync();
            Assert.AreEqual("Found User", found.Name);

            // A null that is set is rejected, as a null parameter is, before any request is sent.
            found.Notes = null;
            InvalidParameterException rejected = await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => found.SaveAsync());
            Assert.AreEqual("Bad parameter: notes is null, which cannot be sent", rejected.Message);

            site.AssertReceived(
                "POST /users key=key-a workspace=11",
                "POST /users key=key-a workspace=11",
                "GET /users/9?id=9 key=key-a workspace=11",
                "PATCH /users/9 key=key-a workspace=11");
            Assert.AreEqual("username=new-user", Fields(site.Requests[0].Body));
            Assert.AreEqual("ftp_permission=false notes= password_validity_days=0 username=explicit", Fields(site.Requests[1].Body));
            Assert.AreEqual("id=9 name=Found User", Fields(site.Requests[3].Body));
        }

        [TestMethod]
        public async Task SaveSendsValuesChangedInPlace()
        {
            // The site keeps what it was last sent, and answers with it.
            string paths = "[\"a\",\"b\"]";
            string formFields = "[{\"id\":1,\"label\":\"Color\",\"options_for_select\":[\"red\",\"blue\"]}]";
            site.On("GET", "/bundles/7", request => LoopbackSite.Json($"{{\"id\":7,\"paths\":{paths},\"expires_at\":\"2026-10-01T00:00:00Z\"}}"));
            site.On("PATCH", "/bundles/7", request =>
            {
                paths = Field(request.Body, "paths") ?? paths;
                return LoopbackSite.Json($"{{\"id\":7,\"paths\":{paths}}}");
            });
            site.On("GET", "/form_field_sets/9", request => LoopbackSite.Json($"{{\"id\":9,\"form_fields\":{formFields}}}"));
            site.On("PATCH", "/form_field_sets/9", request =>
            {
                formFields = Field(request.Body, "form_fields") ?? formFields;
                return LoopbackSite.Json($"{{\"id\":9,\"form_fields\":{formFields}}}");
            });
            FilesClient client = FilesClient.Create(site.Configuration());
            Bundle bundle = await client.Bundles.FindAsync(7);

            // Reading, through a property or getAttributes, changes nothing, so nothing but the ID is sent.
            Assert.AreEqual(2, bundle.Paths.Length);
            Assert.AreEqual(2, ((string[])bundle.getAttributes()["paths"]).Length);
            await bundle.SaveAsync();
            bundle.Paths[0] = "property-edit";
            await bundle.SaveAsync();
            ((string[])bundle.getAttributes()["paths"])[1] = "attributes-edit";
            await bundle.SaveAsync();
            // The object returned by an update is separate: changing it leaves this one alone.
            Bundle returned = await bundle.UpdateAsync();
            string[] ownPaths = bundle.Paths;
            returned.Paths[0] = "returned-edit";
            Assert.AreEqual("property-edit", ownPaths[0]);
            await bundle.SaveAsync();

            // A change inside a model in an array counts too.
            FormFieldSet form = await client.FormFieldSets.FindAsync(9);
            form.FormFields[0].OptionsForSelect[0] = "nested-edit";
            await form.SaveAsync();

            CollectionAssert.AreEqual(new[] { "property-edit", "attributes-edit" }, bundle.Paths);
            CollectionAssert.AreEqual(new[] { "nested-edit", "blue" }, form.FormFields[0].OptionsForSelect);
            CollectionAssert.AreEqual(
                new[]
                {
                    "PATCH /bundles/7 id=7",
                    "PATCH /bundles/7 id=7 paths=[\"property-edit\",\"b\"]",
                    "PATCH /bundles/7 id=7 paths=[\"property-edit\",\"attributes-edit\"]",
                    "PATCH /bundles/7 id=7",
                    "PATCH /bundles/7 id=7",
                    "PATCH /form_field_sets/9 options_for_select=[\"nested-edit\",\"blue\"]",
                },
                site.Requests.Where(request => request.Method == "PATCH").Select(request => request.Method + " " + request.Path + " " +
                    (request.Path.StartsWith("/form_field_sets") ? "options_for_select=" + OptionsForSelect(request.Body) : Fields(request.Body))).ToArray());
        }

        [TestMethod]
        public async Task SaveSendsTypedPropertiesInTheFormTheirParametersTake()
        {
            site.On("PATCH", "/users/7", request => LoopbackSite.Json("{\"id\":7,\"authenticate_until\":\"2027-01-01T08:30:00Z\"}"));
            site.On("GET", "/automations/3", request => LoopbackSite.Json("{\"id\":3,\"group_ids\":[1,2]}"));
            site.On("PATCH", "/automations/3", request => LoopbackSite.Json("{\"id\":3,\"group_ids\":[1,5]}"));
            site.On("PATCH", "/remote_mount_backends/4", request => LoopbackSite.Json($"{{\"id\":4,\"min_free_cpu\":{Field(request.Body, "min_free_cpu")}}}"));
            FilesClient client = FilesClient.Create(site.Configuration());

            // A date-time goes as the same ISO 8601 text whether it is given to an update directly or set as a DateTime.
            User user = client.Users.New(new Dictionary<string, object> { { "id", 7L } });
            await user.UpdateAsync(new Dictionary<string, object> { { "authenticate_until", "2027-01-01T08:30:00Z" } });
            user.AuthenticateUntil = new DateTime(2027, 1, 1, 8, 30, 0, DateTimeKind.Utc);
            await user.SaveAsync();
            Assert.AreEqual(new DateTime(2027, 1, 1, 8, 30, 0, DateTimeKind.Utc), user.AuthenticateUntil.Value.ToUniversalTime());

            // IDs the API takes as comma-delimited text, here changed in place.
            Automation automation = await client.Automations.FindAsync(3);
            automation.GroupIds[1] = 5;
            await automation.SaveAsync();
            CollectionAssert.AreEqual(new long?[] { 1, 5 }, automation.GroupIds);

            // A decimal goes as its exact digits, as text, which the API reads as a decimal; a double given directly, as
            // callers always could, still goes as a JSON number.
            RemoteMountBackend backend = client.RemoteMountBackends.New(new Dictionary<string, object> { { "id", 4L } });
            await backend.UpdateAsync(new Dictionary<string, object> { { "min_free_cpu", 12.5 } });
            backend.MinFreeCpu = 1.0049999999999999999999999999m;
            await backend.SaveAsync();
            Assert.AreEqual(1.0049999999999999999999999999m, backend.MinFreeCpu);

            CollectionAssert.AreEqual(
                new[]
                {
                    "PATCH authenticate_until=2027-01-01T08:30:00Z id=7",
                    "PATCH authenticate_until=2027-01-01T08:30:00Z id=7",
                    "GET ",
                    "PATCH group_ids=1,5 id=3",
                    "PATCH id=4 min_free_cpu=12.5",
                    "PATCH id=4 min_free_cpu=1.0049999999999999999999999999",
                },
                site.Requests.Select(request => request.Method + " " + (request.Method == "GET" ? "" : Fields(request.Body))).ToArray());
            CollectionAssert.AreEqual(
                new[] { "12.5", "\"1.0049999999999999999999999999\"" },
                site.Requests.Where(request => request.Path.StartsWith("/remote_mount_backends")).Select(request => Field(request.Body, "min_free_cpu")).ToArray());
        }

        // A decimal attribute that was never set, or that Files.com returns as null, reads as null, as a number
        // attribute does, rather than failing or reading as 0.
        [TestMethod]
        [Timeout(60000)]
        public async Task DecimalAttributesWithoutAValueAreNull()
        {
            site.On("GET", "/remote_mount_backends/4", request => LoopbackSite.Json("{\"id\":4,\"min_free_cpu\":null,\"min_free_mem\":\"2.50\"}"));
            FilesClient client = FilesClient.Create(site.Configuration());

            RemoteMountBackend unset = client.RemoteMountBackends.New(new Dictionary<string, object> { { "id", 4L } });
            RemoteMountBackend found = await client.RemoteMountBackends.FindAsync(4);

            Assert.IsNull(unset.MinFreeCpu);
            Assert.IsNull(found.MinFreeCpu);
            Assert.AreEqual(2.50m, found.MinFreeMem);
        }

        // The JSON of one field of a JSON object, or null when it has none.
        private static string Field(string json, string name)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                return document.RootElement.TryGetProperty(name, out JsonElement field) ? field.GetRawText() : null;
            }
        }

        // The options_for_select sent for the first form field.
        private static string OptionsForSelect(string json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                return document.RootElement.GetProperty("form_fields")[0].GetProperty("options_for_select").GetRawText();
            }
        }

        // A JSON object's fields as "name=value" in name order, such as "id=7 name=Projects".
        private static string Fields(string json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                return string.Join(" ", document.RootElement.EnumerateObject()
                    .OrderBy(property => property.Name, System.StringComparer.Ordinal)
                    .Select(property => property.Name + "=" + (property.Value.ValueKind == JsonValueKind.String ? property.Value.GetString() : property.Value.GetRawText())));
            }
        }
    }
}