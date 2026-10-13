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

    // The record contract of Session, generated from the schema: each case sends one request through the
    // SDK's Sessions operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class SessionRecordContractTest : ContractCase
    {
        private const string ListPath = "/sessions";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("Session", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "Session", ("id", "string"), ("language", "string"), ("read_only", "boolean"), ("sftp_insecure_ciphers", "boolean"));
            }
            return records;
        }

        // Every property the record holds, as the Session the SDK returns gives it.
        private static void AssertModel(JsonObject record, Session model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no Session");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "language", model.Language, what);
            Simulator.Same(record, "read_only", model.ReadOnly, what);
            Simulator.Same(record, "sftp_insecure_ciphers", model.SftpInsecureCiphers, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("Session", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Session created = await Simulator.Sdk(operation).Sessions.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { });
        }
    }
}