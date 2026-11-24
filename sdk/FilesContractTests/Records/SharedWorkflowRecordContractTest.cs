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

    // The record contract of SharedWorkflow, generated from the schema: each case sends one request through the
    // SDK's SharedWorkflows operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class SharedWorkflowRecordContractTest : ContractCase
    {
        private const string ListPath = "/shared_workflows";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["name"] = "contract-name", ["description"] = "contract-description", ["submission_revision"] = "contract-submission_revision" };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            // Fixture records get IDs from 1 in the order given.
            List<JsonObject> held = new List<JsonObject>();
            for (int id = 1; id <= count; id++)
            {
                held.Add(await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{id}", null, null));
            }
            return Conform(held.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "SharedWorkflow", ("id", "int64"), ("name", "string"), ("description", "string"), ("submission_revision", "string"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the SharedWorkflow the SDK returns gives it.
        private static void AssertModel(JsonObject record, SharedWorkflow model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no SharedWorkflow");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "description", model.Description, what);
            Simulator.Same(record, "submission_revision", model.SubmissionRevision, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("SharedWorkflow", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            SharedWorkflow[] listed = (await Simulator.Sdk(operation).SharedWorkflows.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"SharedWorkflow.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("SharedWorkflow", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            SharedWorkflow found = await Simulator.Sdk(operation).SharedWorkflows.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "SharedWorkflow.find");
        }
    }
}