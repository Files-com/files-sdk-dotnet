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

    // The record contract of As2IncomingMessage, generated from the schema: each case sends one request through the
    // SDK's As2IncomingMessages operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class As2IncomingMessageRecordContractTest : ContractCase
    {
        private const string ListPath = "/as2_incoming_messages";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["workspace_id"] = 7, ["as2_partner_id"] = 7, ["as2_station_id"] = 7, ["uuid"] = "contract-uuid", ["content_type"] = "contract-content_type", ["processing_result"] = "not_started", ["processing_result_description"] = "contract-processing_result_description", ["mic"] = "contract-mic", ["mic_algo"] = "contract-mic_algo", ["as2_to"] = "contract-as2_to", ["as2_from"] = "contract-as2_from", ["message_id"] = "contract-message_id", ["subject"] = "contract-subject", ["date"] = "contract-date", ["body_size"] = "contract-body_size", ["attachment_filename"] = "contract-attachment_filename", ["ip"] = "contract-ip", ["created_at"] = "2030-01-02T03:04:05Z", ["http_response_code"] = "contract-http_response_code", ["recipient_serial"] = "contract-recipient_serial", ["hex_recipient_serial"] = "contract-hex_recipient_serial", ["recipient_issuer"] = "contract-recipient_issuer", ["message_received"] = true, ["message_decrypted"] = true, ["message_signature_verified"] = true, ["message_processing_success"] = true, ["message_mdn_returned"] = true, ["encrypted_uri"] = "contract-encrypted_uri", ["smime_signed_uri"] = "contract-smime_signed_uri", ["smime_uri"] = "contract-smime_uri", ["raw_uri"] = "contract-raw_uri", ["mdn_response_uri"] = "contract-mdn_response_uri" };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "As2IncomingMessage", ("id", "int64"), ("workspace_id", "int64"), ("as2_partner_id", "int64"), ("as2_station_id", "int64"), ("uuid", "string"), ("content_type", "string"), ("processing_result", "string"), ("processing_result_description", "string"), ("mic", "string"), ("mic_algo", "string"), ("as2_to", "string"), ("as2_from", "string"), ("message_id", "string"), ("subject", "string"), ("date", "string"), ("body_size", "string"), ("attachment_filename", "string"), ("ip", "string"), ("created_at", "date-time"), ("http_response_code", "string"), ("recipient_serial", "string"), ("hex_recipient_serial", "string"), ("recipient_issuer", "string"), ("message_received", "boolean"), ("message_decrypted", "boolean"), ("message_signature_verified", "boolean"), ("message_processing_success", "boolean"), ("message_mdn_returned", "boolean"), ("encrypted_uri", "string"), ("smime_signed_uri", "string"), ("smime_uri", "string"), ("raw_uri", "string"), ("mdn_response_uri", "string"));
            }
            return records;
        }

        // Every property the record holds, as the As2IncomingMessage the SDK returns gives it.
        private static void AssertModel(JsonObject record, As2IncomingMessage model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no As2IncomingMessage");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "as2_partner_id", model.As2PartnerId, what);
            Simulator.Same(record, "as2_station_id", model.As2StationId, what);
            Simulator.Same(record, "uuid", model.Uuid, what);
            Simulator.Same(record, "content_type", model.ContentType, what);
            Simulator.Same(record, "processing_result", model.ProcessingResult, what);
            Simulator.Same(record, "processing_result_description", model.ProcessingResultDescription, what);
            Simulator.Same(record, "mic", model.Mic, what);
            Simulator.Same(record, "mic_algo", model.MicAlgo, what);
            Simulator.Same(record, "as2_to", model.As2To, what);
            Simulator.Same(record, "as2_from", model.As2From, what);
            Simulator.Same(record, "message_id", model.MessageId, what);
            Simulator.Same(record, "subject", model.Subject, what);
            Simulator.Same(record, "date", model.Date, what);
            Simulator.Same(record, "body_size", model.BodySize, what);
            Simulator.Same(record, "attachment_filename", model.AttachmentFilename, what);
            Simulator.Same(record, "ip", model.Ip, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "http_response_code", model.HttpResponseCode, what);
            Simulator.Same(record, "recipient_serial", model.RecipientSerial, what);
            Simulator.Same(record, "hex_recipient_serial", model.HexRecipientSerial, what);
            Simulator.Same(record, "recipient_issuer", model.RecipientIssuer, what);
            Simulator.Same(record, "message_received", model.MessageReceived, what);
            Simulator.Same(record, "message_decrypted", model.MessageDecrypted, what);
            Simulator.Same(record, "message_signature_verified", model.MessageSignatureVerified, what);
            Simulator.Same(record, "message_processing_success", model.MessageProcessingSuccess, what);
            Simulator.Same(record, "message_mdn_returned", model.MessageMdnReturned, what);
            Simulator.Same(record, "encrypted_uri", model.EncryptedUri, what);
            Simulator.Same(record, "smime_signed_uri", model.SmimeSignedUri, what);
            Simulator.Same(record, "smime_uri", model.SmimeUri, what);
            Simulator.Same(record, "raw_uri", model.RawUri, what);
            Simulator.Same(record, "mdn_response_uri", model.MdnResponseUri, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("As2IncomingMessage", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            As2IncomingMessage[] listed = (await Simulator.Sdk(operation).As2IncomingMessages.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"As2IncomingMessage.list[{i}]");
            }
        }
    }
}