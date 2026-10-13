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

    // The record contract of Invoice, generated from the schema: each case sends one request through the
    // SDK's Invoices operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class InvoiceRecordContractTest : ContractCase
    {
        private const string ListPath = "/invoices";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["amount"] = "12.50", ["balance"] = "12.50", ["created_at"] = "2030-01-02T03:04:05Z", ["currency"] = "contract-currency", ["download_uri"] = "contract-download_uri", ["method"] = "contract-method", ["payment_reversed_at"] = "2030-01-02T03:04:05Z", ["payment_type"] = "contract-payment_type", ["site_name"] = "contract-site_name", ["type"] = "contract-type" };
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
                Simulator.Conforms(record, "Invoice", ("id", "int64"), ("amount", "decimal"), ("balance", "decimal"), ("created_at", "date-time"), ("currency", "string"), ("download_uri", "string"), ("method", "string"), ("payment_reversed_at", "date-time"), ("payment_type", "string"), ("site_name", "string"), ("type", "string"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the AccountLineItem the SDK returns gives it.
        private static void AssertModel(JsonObject record, AccountLineItem model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no AccountLineItem");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "amount", model.Amount, what);
            Simulator.Same(record, "balance", model.Balance, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "currency", model.Currency, what);
            Simulator.Same(record, "download_uri", model.DownloadUri, what);
            Simulator.Same(record, "method", model.Method, what);
            Simulator.Same(record, "payment_reversed_at", model.PaymentReversedAt, what);
            Simulator.Same(record, "payment_type", model.PaymentType, what);
            Simulator.Same(record, "site_name", model.SiteName, what);
            Simulator.Same(record, "type", model.Type, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("Invoice", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            AccountLineItem[] listed = (await Simulator.Sdk(operation).Invoices.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"Invoice.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("Invoice", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            AccountLineItem found = await Simulator.Sdk(operation).Invoices.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "Invoice.find");
        }
    }
}