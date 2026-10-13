using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace FilesContractTests.Records
{
    // Imported inside the namespace so that model names, such as Lock, take precedence over those of the namespaces
    // above, such as System.Threading.Lock.
    using FilesCom.Models;

    // The record contract of ApiKey, generated from the schema: each case sends one request through the
    // SDK's ApiKeys operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class ApiKeyRecordContractTest : ContractCase
    {
        private const string ListPath = "/api_keys";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("ApiKey", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["user_id"] = (Int64)7, ["description"] = "contract-description", ["expires_at"] = "2030-01-02T03:04:05Z", ["name"] = "contract-name", ["aws_style_credentials"] = true, ["permission_set"] = "none", ["workspace_id"] = (Int64)7 })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "ApiKey", ("id", "int64"), ("descriptive_label", "string"), ("description", "string"), ("created_at", "date-time"), ("expires_at", "date-time"), ("key", "string"), ("aws_style_credentials", "boolean"), ("aws_access_key_id", "string"), ("aws_secret_key", "string"), ("last_use_at", "date-time"), ("name", "string"), ("permission_set", "string"), ("platform", "string"), ("site_id", "int64"), ("site_name", "string"), ("url", "string"), ("user_id", "int64"), ("workspace_id", "int64"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the ApiKey the SDK returns gives it.
        private static void AssertModel(JsonObject record, ApiKey model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no ApiKey");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "descriptive_label", model.DescriptiveLabel, what);
            Simulator.Same(record, "description", model.Description, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "expires_at", model.ExpiresAt, what);
            Simulator.Same(record, "key", model.Key, what);
            Simulator.Same(record, "aws_style_credentials", model.AwsStyleCredentials, what);
            Simulator.Same(record, "aws_access_key_id", model.AwsAccessKeyId, what);
            Simulator.Same(record, "aws_secret_key", model.AwsSecretKey, what);
            Simulator.Same(record, "last_use_at", model.LastUseAt, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "permission_set", model.PermissionSet, what);
            Simulator.Same(record, "platform", model.Platform, what);
            Simulator.Same(record, "site_id", model.SiteId, what);
            Simulator.Same(record, "site_name", model.SiteName, what);
            Simulator.Same(record, "url", model.Url, what);
            Simulator.Same(record, "user_id", model.UserId, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("ApiKey", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["user_id"] = (Int64)7, ["description"] = "contract-description", ["expires_at"] = "2030-01-02T03:04:05Z", ["name"] = "contract-name", ["aws_style_credentials"] = true, ["permission_set"] = "none", ["workspace_id"] = (Int64)7 };
            ApiKey created = await Simulator.Sdk(operation).ApiKeys.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["user_id"] = "number", ["description"] = "string", ["expires_at"] = "string", ["name"] = "string", ["aws_style_credentials"] = "boolean", ["permission_set"] = "string", ["workspace_id"] = "number" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "ApiKey.create", "user_id", "description", "expires_at", "name", "aws_style_credentials", "permission_set", "workspace_id");
            AssertModel(record, created, "ApiKey.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("ApiKey", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            ApiKey[] listed = (await Simulator.Sdk(operation).ApiKeys.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"ApiKey.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("ApiKey", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            ApiKey found = await Simulator.Sdk(operation).ApiKeys.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "ApiKey.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("ApiKey", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["description"] = "contract-description-2";
            sent["expires_at"] = "2031-02-03T04:05:06Z";
            sent["name"] = "contract-name-2";
            ApiKey updated = await Simulator.Sdk(operation).ApiKeys.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["description"] = "string", ["expires_at"] = "string", ["name"] = "string" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "ApiKey.update", "description", "expires_at", "name");
            AssertModel(now, updated, "ApiKey.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("ApiKey", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).ApiKeys.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"ApiKey.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}