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

    // The record contract of FormFieldSet, generated from the schema: each case sends one request through the
    // SDK's FormFieldSets operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class FormFieldSetRecordContractTest : ContractCase
    {
        private const string ListPath = "/form_field_sets";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("FormFieldSet", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["title"] = "contract-title", ["require_complete_metadata_capture"] = true, ["skip_email"] = true, ["skip_name"] = true, ["skip_company"] = true })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "FormFieldSet", ("id", "int64"), ("title", "string"), ("require_complete_metadata_capture", "boolean"), ("form_layout", "array(int64)"), ("skip_name", "boolean"), ("skip_email", "boolean"), ("skip_company", "boolean"), ("in_use", "boolean"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the FormFieldSet the SDK returns gives it.
        private static void AssertModel(JsonObject record, FormFieldSet model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no FormFieldSet");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "title", model.Title, what);
            Simulator.Same(record, "require_complete_metadata_capture", model.RequireCompleteMetadataCapture, what);
            Simulator.Same(record, "form_layout", model.FormLayout, what);
            Simulator.Same(record, "skip_name", model.SkipName, what);
            Simulator.Same(record, "skip_email", model.SkipEmail, what);
            Simulator.Same(record, "skip_company", model.SkipCompany, what);
            Simulator.Same(record, "in_use", model.InUse, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("FormFieldSet", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["title"] = "contract-title", ["require_complete_metadata_capture"] = true, ["skip_email"] = true, ["skip_name"] = true, ["skip_company"] = true };
            FormFieldSet created = await Simulator.Sdk(operation).FormFieldSets.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["title"] = "string", ["require_complete_metadata_capture"] = "boolean", ["skip_email"] = "boolean", ["skip_name"] = "boolean", ["skip_company"] = "boolean" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "FormFieldSet.create", "title", "require_complete_metadata_capture", "skip_email", "skip_name", "skip_company");
            AssertModel(record, created, "FormFieldSet.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("FormFieldSet", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            FormFieldSet[] listed = (await Simulator.Sdk(operation).FormFieldSets.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"FormFieldSet.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("FormFieldSet", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            FormFieldSet found = await Simulator.Sdk(operation).FormFieldSets.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "FormFieldSet.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("FormFieldSet", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["title"] = "contract-title-2";
            sent["require_complete_metadata_capture"] = false;
            sent["skip_email"] = false;
            sent["skip_name"] = false;
            sent["skip_company"] = false;
            FormFieldSet updated = await Simulator.Sdk(operation).FormFieldSets.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["title"] = "string", ["require_complete_metadata_capture"] = "boolean", ["skip_email"] = "boolean", ["skip_name"] = "boolean", ["skip_company"] = "boolean" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "FormFieldSet.update", "title", "require_complete_metadata_capture", "skip_email", "skip_name", "skip_company");
            AssertModel(now, updated, "FormFieldSet.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("FormFieldSet", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).FormFieldSets.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"FormFieldSet.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}