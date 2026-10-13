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

    // The record contract of BundleRegistration, generated from the schema: each case sends one request through the
    // SDK's BundleRegistrations operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class BundleRegistrationRecordContractTest : ContractCase
    {
        private const string ListPath = "/bundle_registrations";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["code"] = "contract-code", ["name"] = "contract-name", ["company"] = "contract-company", ["email"] = "contract-email", ["ip"] = "contract-ip", ["inbox_code"] = "contract-inbox_code", ["clickwrap_body"] = "contract-clickwrap_body", ["form_field_set_id"] = 7, ["bundle_code"] = "contract-bundle_code", ["bundle_id"] = 7, ["bundle_recipient_id"] = 7, ["workspace_id"] = 7, ["created_at"] = "2030-01-02T03:04:05Z" };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "BundleRegistration", ("code", "string"), ("name", "string"), ("company", "string"), ("email", "string"), ("ip", "string"), ("inbox_code", "string"), ("clickwrap_body", "string"), ("form_field_set_id", "int64"), ("bundle_code", "string"), ("bundle_id", "int64"), ("bundle_recipient_id", "int64"), ("workspace_id", "int64"), ("created_at", "date-time"));
            }
            return records;
        }

        // Every property the record holds, as the BundleRegistration the SDK returns gives it.
        private static void AssertModel(JsonObject record, BundleRegistration model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no BundleRegistration");
            Simulator.Same(record, "code", model.Code, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "company", model.Company, what);
            Simulator.Same(record, "email", model.Email, what);
            Simulator.Same(record, "ip", model.Ip, what);
            Simulator.Same(record, "inbox_code", model.InboxCode, what);
            Simulator.Same(record, "clickwrap_body", model.ClickwrapBody, what);
            Simulator.Same(record, "form_field_set_id", model.FormFieldSetId, what);
            Simulator.Same(record, "bundle_code", model.BundleCode, what);
            Simulator.Same(record, "bundle_id", model.BundleId, what);
            Simulator.Same(record, "bundle_recipient_id", model.BundleRecipientId, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("BundleRegistration", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            BundleRegistration[] listed = (await Simulator.Sdk(operation).BundleRegistrations.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"BundleRegistration.list[{i}]");
            }
        }
    }
}