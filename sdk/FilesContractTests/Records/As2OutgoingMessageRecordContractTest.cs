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

    // The record contract of As2OutgoingMessage, generated from the schema: each case sends one request through the
    // SDK's As2OutgoingMessages operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class As2OutgoingMessageRecordContractTest : ContractCase
    {
        private const string ListPath = "/as2_outgoing_messages";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["workspace_id"] = 7, ["as2_partner_id"] = 7, ["as2_station_id"] = 7, ["uuid"] = "contract-uuid", ["processing_result"] = "not_started", ["processing_result_description"] = "contract-processing_result_description", ["mic"] = "contract-mic", ["mic_sha_256"] = "contract-mic_sha_256", ["as2_to"] = "contract-as2_to", ["as2_from"] = "contract-as2_from", ["as2_subject"] = "contract-as2_subject", ["date"] = "contract-date", ["message_id"] = "contract-message_id", ["body_size"] = "contract-body_size", ["attachment_filename"] = "contract-attachment_filename", ["created_at"] = "2030-01-02T03:04:05Z", ["http_response_code"] = "contract-http_response_code", ["http_transmission_duration"] = 1.5, ["mdn_received"] = true, ["mdn_valid"] = true, ["mdn_signature_verified"] = true, ["mdn_message_id_matched"] = true, ["mdn_mic_matched"] = true, ["mdn_processing_success"] = true, ["raw_uri"] = "contract-raw_uri", ["smime_uri"] = "contract-smime_uri", ["smime_signed_uri"] = "contract-smime_signed_uri", ["encrypted_uri"] = "contract-encrypted_uri", ["mdn_response_uri"] = "contract-mdn_response_uri" };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "As2OutgoingMessage", ("id", "int64"), ("workspace_id", "int64"), ("as2_partner_id", "int64"), ("as2_station_id", "int64"), ("uuid", "string"), ("processing_result", "string"), ("processing_result_description", "string"), ("mic", "string"), ("mic_sha_256", "string"), ("as2_to", "string"), ("as2_from", "string"), ("as2_subject", "string"), ("date", "string"), ("message_id", "string"), ("body_size", "string"), ("attachment_filename", "string"), ("created_at", "date-time"), ("http_response_code", "string"), ("http_transmission_duration", "double"), ("mdn_received", "boolean"), ("mdn_valid", "boolean"), ("mdn_signature_verified", "boolean"), ("mdn_message_id_matched", "boolean"), ("mdn_mic_matched", "boolean"), ("mdn_processing_success", "boolean"), ("raw_uri", "string"), ("smime_uri", "string"), ("smime_signed_uri", "string"), ("encrypted_uri", "string"), ("mdn_response_uri", "string"));
            }
            return records;
        }

        // Every property the record holds, as the As2OutgoingMessage the SDK returns gives it.
        private static void AssertModel(JsonObject record, As2OutgoingMessage model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no As2OutgoingMessage");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "as2_partner_id", model.As2PartnerId, what);
            Simulator.Same(record, "as2_station_id", model.As2StationId, what);
            Simulator.Same(record, "uuid", model.Uuid, what);
            Simulator.Same(record, "processing_result", model.ProcessingResult, what);
            Simulator.Same(record, "processing_result_description", model.ProcessingResultDescription, what);
            Simulator.Same(record, "mic", model.Mic, what);
            Simulator.Same(record, "mic_sha_256", model.MicSha256, what);
            Simulator.Same(record, "as2_to", model.As2To, what);
            Simulator.Same(record, "as2_from", model.As2From, what);
            Simulator.Same(record, "as2_subject", model.As2Subject, what);
            Simulator.Same(record, "date", model.Date, what);
            Simulator.Same(record, "message_id", model.MessageId, what);
            Simulator.Same(record, "body_size", model.BodySize, what);
            Simulator.Same(record, "attachment_filename", model.AttachmentFilename, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "http_response_code", model.HttpResponseCode, what);
            Simulator.Same(record, "http_transmission_duration", model.HttpTransmissionDuration, what);
            Simulator.Same(record, "mdn_received", model.MdnReceived, what);
            Simulator.Same(record, "mdn_valid", model.MdnValid, what);
            Simulator.Same(record, "mdn_signature_verified", model.MdnSignatureVerified, what);
            Simulator.Same(record, "mdn_message_id_matched", model.MdnMessageIdMatched, what);
            Simulator.Same(record, "mdn_mic_matched", model.MdnMicMatched, what);
            Simulator.Same(record, "mdn_processing_success", model.MdnProcessingSuccess, what);
            Simulator.Same(record, "raw_uri", model.RawUri, what);
            Simulator.Same(record, "smime_uri", model.SmimeUri, what);
            Simulator.Same(record, "smime_signed_uri", model.SmimeSignedUri, what);
            Simulator.Same(record, "encrypted_uri", model.EncryptedUri, what);
            Simulator.Same(record, "mdn_response_uri", model.MdnResponseUri, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("As2OutgoingMessage", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            As2OutgoingMessage[] listed = (await Simulator.Sdk(operation).As2OutgoingMessages.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"As2OutgoingMessage.list[{i}]");
            }
        }
    }
}