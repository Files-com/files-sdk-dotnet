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

    // The record contract of IpAddress, generated from the schema: each case sends one request through the
    // SDK's IpAddresses operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class IpAddressRecordContractTest : ContractCase
    {
        private const string ListPath = "/ip_addresses";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            // Each record holds its own string id, which only a fixture supplies.
            JsonObject[] records = Enumerable.Range(1, count).Select(n =>
            {
                JsonObject record = new JsonObject { ["associated_with"] = "contract-associated_with", ["group_id"] = 7, ["ip_addresses"] = new JsonArray("a") };
                record["id"] = $"contract-id-{n}";
                return record;
            }).ToArray();
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), records);
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "IpAddress", ("id", "string"), ("associated_with", "string"), ("group_id", "int64"), ("ip_addresses", "array(string)"));
            }
            return records;
        }

        // Every property the record holds, as the IpAddress the SDK returns gives it.
        private static void AssertModel(JsonObject record, IpAddress model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no IpAddress");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "associated_with", model.AssociatedWith, what);
            Simulator.Same(record, "group_id", model.GroupId, what);
            Simulator.Same(record, "ip_addresses", model.IpAddresses, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("IpAddress", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            IpAddress[] listed = (await Simulator.Sdk(operation).IpAddresses.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"IpAddress.list[{i}]");
            }
        }
    }
}