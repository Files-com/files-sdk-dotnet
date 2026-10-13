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

    // The record contract of InboxRegistration, generated from the schema: each case sends one request through the
    // SDK's InboxRegistrations operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class InboxRegistrationRecordContractTest : ContractCase
    {
        private const string ListPath = "/inbox_registrations";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["code"] = "contract-code", ["name"] = "contract-name", ["company"] = "contract-company", ["email"] = "contract-email", ["ip"] = "contract-ip", ["clickwrap_body"] = "contract-clickwrap_body", ["form_field_set_id"] = 7, ["inbox_id"] = 7, ["inbox_recipient_id"] = 7, ["inbox_title"] = "contract-inbox_title", ["created_at"] = "2030-01-02T03:04:05Z" };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "InboxRegistration", ("code", "string"), ("name", "string"), ("company", "string"), ("email", "string"), ("ip", "string"), ("clickwrap_body", "string"), ("form_field_set_id", "int64"), ("inbox_id", "int64"), ("inbox_recipient_id", "int64"), ("inbox_title", "string"), ("created_at", "date-time"));
            }
            return records;
        }

        // Every property the record holds, as the InboxRegistration the SDK returns gives it.
        private static void AssertModel(JsonObject record, InboxRegistration model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no InboxRegistration");
            Simulator.Same(record, "code", model.Code, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "company", model.Company, what);
            Simulator.Same(record, "email", model.Email, what);
            Simulator.Same(record, "ip", model.Ip, what);
            Simulator.Same(record, "clickwrap_body", model.ClickwrapBody, what);
            Simulator.Same(record, "form_field_set_id", model.FormFieldSetId, what);
            Simulator.Same(record, "inbox_id", model.InboxId, what);
            Simulator.Same(record, "inbox_recipient_id", model.InboxRecipientId, what);
            Simulator.Same(record, "inbox_title", model.InboxTitle, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("InboxRegistration", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            InboxRegistration[] listed = (await Simulator.Sdk(operation).InboxRegistrations.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"InboxRegistration.list[{i}]");
            }
        }
    }
}