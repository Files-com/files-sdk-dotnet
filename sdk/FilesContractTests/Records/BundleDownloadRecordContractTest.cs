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

    // The record contract of BundleDownload, generated from the schema: each case sends one request through the
    // SDK's BundleDownloads operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class BundleDownloadRecordContractTest : ContractCase
    {
        private const string ListPath = "/bundle_downloads";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["download_method"] = "file", ["path"] = "contract-path", ["workspace_id"] = 7, ["created_at"] = "2030-01-02T03:04:05Z" };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "BundleDownload", ("download_method", "string"), ("path", "string"), ("workspace_id", "int64"), ("created_at", "date-time"));
            }
            return records;
        }

        // Every property the record holds, as the BundleDownload the SDK returns gives it.
        private static void AssertModel(JsonObject record, BundleDownload model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no BundleDownload");
            Simulator.Same(record, "download_method", model.DownloadMethod, what);
            Simulator.Same(record, "path", model.Path, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("BundleDownload", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            BundleDownload[] listed = (await Simulator.Sdk(operation).BundleDownloads.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"BundleDownload.list[{i}]");
            }
        }
    }
}