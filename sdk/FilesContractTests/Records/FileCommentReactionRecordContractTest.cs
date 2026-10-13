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

    // The record contract of FileCommentReaction, generated from the schema: each case sends one request through the
    // SDK's FileCommentReactions operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class FileCommentReactionRecordContractTest : ContractCase
    {
        private const string ListPath = "/file_comment_reactions";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("FileCommentReaction", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["file_comment_id"] = (Int64)7, ["emoji"] = "contract-emoji" })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "FileCommentReaction", ("id", "int64"), ("emoji", "string"));
            }
            return records;
        }

        // Every property the record holds, as the FileCommentReaction the SDK returns gives it.
        private static void AssertModel(JsonObject record, FileCommentReaction model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no FileCommentReaction");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "emoji", model.Emoji, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("FileCommentReaction", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["file_comment_id"] = (Int64)7, ["emoji"] = "contract-emoji" };
            FileCommentReaction created = await Simulator.Sdk(operation).FileCommentReactions.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["file_comment_id"] = "number", ["emoji"] = "string" });
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("FileCommentReaction", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).FileCommentReactions.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
        }
    }
}