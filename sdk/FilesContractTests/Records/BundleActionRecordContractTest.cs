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

    // The record contract of BundleAction, generated from the schema: each case sends one request through the
    // SDK's BundleActions operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class BundleActionRecordContractTest : ContractCase
    {
        private const string ListPath = "/bundle_actions";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["action"] = "create", ["created_at"] = "2030-01-02T03:04:05Z", ["destination"] = "contract-destination", ["path"] = "contract-path", ["source"] = "contract-source", ["bundle_registration_id"] = 7, ["workspace_id"] = 7, ["bundle_registration_name"] = "contract-bundle_registration_name", ["bundle_registration_email"] = "contract-bundle_registration_email", ["bundle_registration_ip"] = "contract-bundle_registration_ip", ["form_field_set_id"] = 7 };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "BundleAction", ("action", "string"), ("created_at", "date-time"), ("destination", "string"), ("path", "string"), ("source", "string"), ("bundle_registration_id", "int64"), ("workspace_id", "int64"), ("bundle_registration_name", "string"), ("bundle_registration_email", "string"), ("bundle_registration_ip", "string"), ("form_field_set_id", "int64"));
            }
            return records;
        }

        // Every property the record holds, as the BundleAction the SDK returns gives it.
        private static void AssertModel(JsonObject record, BundleAction model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no BundleAction");
            Simulator.Same(record, "action", model.Action, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "destination", model.Destination, what);
            Simulator.Same(record, "path", model.Path, what);
            Simulator.Same(record, "source", model.Source, what);
            Simulator.Same(record, "bundle_registration_id", model.BundleRegistrationId, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "bundle_registration_name", model.BundleRegistrationName, what);
            Simulator.Same(record, "bundle_registration_email", model.BundleRegistrationEmail, what);
            Simulator.Same(record, "bundle_registration_ip", model.BundleRegistrationIp, what);
            Simulator.Same(record, "form_field_set_id", model.FormFieldSetId, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("BundleAction", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            BundleAction[] listed = (await Simulator.Sdk(operation).BundleActions.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"BundleAction.list[{i}]");
            }
        }
    }
}