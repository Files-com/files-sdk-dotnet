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

    // The record contract of BundleRecipient, generated from the schema: each case sends one request through the
    // SDK's BundleRecipients operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class BundleRecipientRecordContractTest : ContractCase
    {
        private const string ListPath = "/bundle_recipients";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { ["bundle_id"] = (Int64)7 });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("BundleRecipient", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["bundle_id"] = (Int64)7, ["recipient"] = "contract-recipient", ["name"] = "contract-name", ["company"] = "contract-company", ["note"] = "contract-note" })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "BundleRecipient", ("company", "string"), ("name", "string"), ("note", "string"), ("recipient", "string"), ("sent_at", "date-time"), ("workspace_id", "int64"));
            }
            return records;
        }

        // Every property the record holds, as the BundleRecipient the SDK returns gives it.
        private static void AssertModel(JsonObject record, BundleRecipient model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no BundleRecipient");
            Simulator.Same(record, "company", model.Company, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "note", model.Note, what);
            Simulator.Same(record, "recipient", model.Recipient, what);
            Simulator.Same(record, "sent_at", model.SentAt, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("BundleRecipient", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["bundle_id"] = (Int64)7, ["recipient"] = "contract-recipient", ["name"] = "contract-name", ["company"] = "contract-company", ["note"] = "contract-note" };
            BundleRecipient created = await Simulator.Sdk(operation).BundleRecipients.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["bundle_id"] = "number", ["recipient"] = "string", ["name"] = "string", ["company"] = "string", ["note"] = "string" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, null, Scope(), candidate => true);
            await Simulator.KeptAsync(sent, record, "BundleRecipient.create", "recipient", "name", "company", "note");
            AssertModel(record, created, "BundleRecipient.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("BundleRecipient", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { ["bundle_id"] = (Int64)7 };
            BundleRecipient[] listed = (await Simulator.Sdk(operation).BundleRecipients.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["bundle_id"] = "string" });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"BundleRecipient.list[{i}]");
            }
        }
    }
}