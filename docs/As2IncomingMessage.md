# FilesCom.Models.As2IncomingMessage

## Example As2IncomingMessage Object

```
{
  "id": 1,
  "as2_partner_id": 1,
  "as2_station_id": 1,
  "uuid": "example",
  "content_type": "example",
  "http_headers": {
    "key": "example value"
  },
  "processing_result": "example",
  "processing_result_description": "example",
  "mic": "example",
  "mic_algo": "example",
  "as2_to": "example",
  "as2_from": "example",
  "message_id": "example",
  "subject": "example",
  "date": "example",
  "body_size": "example",
  "attachment_filename": "example",
  "ip": "example",
  "created_at": "2000-01-01T01:00:00Z",
  "http_response_code": "example",
  "http_response_headers": {
    "key": "example value"
  },
  "recipient_serial": "example",
  "hex_recipient_serial": "A5:EB:C1:95:DC:D8:2B:E7",
  "recipient_issuer": "example",
  "message_received": true,
  "message_decrypted": true,
  "message_signature_verified": true,
  "message_processing_success": true,
  "message_mdn_returned": true,
  "encrypted_uri": "example",
  "smime_signed_uri": "example",
  "smime_uri": "example",
  "raw_uri": "example",
  "mdn_response_uri": "example"
}
```

* `id` / `Id`  (Nullable<Int64>): Id of the AS2 Partner.
* `as2_partner_id` / `As2PartnerId`  (Nullable<Int64>): Id of the AS2 Partner associated with this message.
* `as2_station_id` / `As2StationId`  (Nullable<Int64>): Id of the AS2 Station associated with this message.
* `uuid` / `Uuid`  (string): UUID assigned to this message.
* `content_type` / `ContentType`  (string): Content Type header of the incoming message.
* `http_headers` / `HttpHeaders`  (object): HTTP Headers sent with this message.
* `processing_result` / `ProcessingResult`  (string): Result of processing.
* `processing_result_description` / `ProcessingResultDescription`  (string): Result of processing description.
* `mic` / `Mic`  (string): AS2 Message Integrity Check
* `mic_algo` / `MicAlgo`  (string): AS2 Message Integrity Check Algorithm Used
* `as2_to` / `As2To`  (string): AS2 TO header of message
* `as2_from` / `As2From`  (string): AS2 FROM header of message
* `message_id` / `MessageId`  (string): AS2 Message Id
* `subject` / `Subject`  (string): AS2 Subject Header
* `date` / `Date`  (string): Date Header
* `body_size` / `BodySize`  (string): Encrypted Payload Body Size
* `attachment_filename` / `AttachmentFilename`  (string): Filename of the file being received.
* `ip` / `Ip`  (string): IP Address of the Sender
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Message creation date/time
* `http_response_code` / `HttpResponseCode`  (string): HTTP Response Code sent for this message
* `http_response_headers` / `HttpResponseHeaders`  (object): HTTP Headers sent for this message.
* `recipient_serial` / `RecipientSerial`  (string): Incoming Message Recipient(the Client Cert used to encrypt this message)'s serial
* `hex_recipient_serial` / `HexRecipientSerial`  (string): Incoming Message Recipient(the Client Cert used to encrypt this message)'s serial in hex format.
* `recipient_issuer` / `RecipientIssuer`  (string): Incoming Message Recipient(the Client Cert used to encrypt this message)'s issuer
* `message_received` / `MessageReceived`  (bool): Message body received?
* `message_decrypted` / `MessageDecrypted`  (bool): Message decrypted successfully?
* `message_signature_verified` / `MessageSignatureVerified`  (bool): Message signature verified?
* `message_processing_success` / `MessageProcessingSuccess`  (bool): Message processed successfully?
* `message_mdn_returned` / `MessageMdnReturned`  (bool): MDN returned?
* `encrypted_uri` / `EncryptedUri`  (string): URL to download the encrypted signed smime that is to sent as AS2 body
* `smime_signed_uri` / `SmimeSignedUri`  (string): URL to download the file contents as smime with signature
* `smime_uri` / `SmimeUri`  (string): URL to download the file contents encoded as smime
* `raw_uri` / `RawUri`  (string): URL to download the original file contents
* `mdn_response_uri` / `MdnResponseUri`  (string): URL to download the http response body


---

## List AS2 Incoming Messages

```
Task<FilesList<As2IncomingMessage>> As2IncomingMessage.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `created_at` and `as2_partner_id`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `created_at`.
* `filter_gt` (object): If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at`.
* `filter_gteq` (object): If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at`.
* `filter_lt` (object): If set, return records where the specified field is less than the supplied value. Valid fields are `created_at`.
* `filter_lteq` (object): If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at`.
* `as2_partner_id` (Nullable<Int64>): As2 Partner ID.  If provided, will return message specific to that partner.
