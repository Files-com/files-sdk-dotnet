# FilesCom.Models.As2OutgoingMessage

## Example As2OutgoingMessage Object

```
{
  "id": 1,
  "as2_partner_id": 1,
  "as2_station_id": 1,
  "uuid": "example",
  "http_headers": {
    "key": "example value"
  },
  "activity_log": "example",
  "processing_result": "example",
  "processing_result_description": "example",
  "mic": "example",
  "mic_sha_256": "example",
  "as2_to": "example",
  "as2_from": "example",
  "date": "example",
  "message_id": "example",
  "body_size": "example",
  "attachment_filename": "example",
  "created_at": "2000-01-01T01:00:00Z",
  "http_response_code": "example",
  "http_response_headers": {
    "key": "example value"
  },
  "http_transmission_duration": 1.0,
  "mdn_received": true,
  "mdn_valid": true,
  "mdn_signature_verified": true,
  "mdn_message_id_matched": true,
  "mdn_mic_matched": true,
  "mdn_processing_success": true,
  "raw_uri": "example",
  "smime_uri": "example",
  "smime_signed_uri": "example",
  "encrypted_uri": "example",
  "mdn_response_uri": "example"
}
```

* `id` / `Id`  (Nullable<Int64>): Id of the AS2 Partner.
* `as2_partner_id` / `As2PartnerId`  (Nullable<Int64>): Id of the AS2 Partner associated with this message.
* `as2_station_id` / `As2StationId`  (Nullable<Int64>): Id of the AS2 Station associated with this message.
* `uuid` / `Uuid`  (string): UUID assigned to this message.
* `http_headers` / `HttpHeaders`  (object): HTTP Headers sent with this message.
* `activity_log` / `ActivityLog`  (string): JSON Structure of the activity log.
* `processing_result` / `ProcessingResult`  (string): Result of processing.
* `processing_result_description` / `ProcessingResultDescription`  (string): Result of processing description.
* `mic` / `Mic`  (string): AS2 Message Integrity Check SHA1
* `mic_sha_256` / `MicSha256`  (string): AS2 Message Integrity Check SHA256
* `as2_to` / `As2To`  (string): AS2 TO
* `as2_from` / `As2From`  (string): AS2 FROM
* `date` / `Date`  (string): Date Header
* `message_id` / `MessageId`  (string): AS2 Message Id
* `body_size` / `BodySize`  (string): Encrypted Payload Body Size
* `attachment_filename` / `AttachmentFilename`  (string): Filename of the file being sent.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Message creation date/time
* `http_response_code` / `HttpResponseCode`  (string): HTTP Response Code received for this message
* `http_response_headers` / `HttpResponseHeaders`  (object): HTTP Headers received for this message.
* `http_transmission_duration` / `HttpTransmissionDuration`  (double): HTTP transmission duration in seceonds
* `mdn_received` / `MdnReceived`  (bool): Did the partner give a response body?
* `mdn_valid` / `MdnValid`  (bool): Is the response in MDN format?
* `mdn_signature_verified` / `MdnSignatureVerified`  (bool): MDN signature verified?
* `mdn_message_id_matched` / `MdnMessageIdMatched`  (bool): MDN message id matched?
* `mdn_mic_matched` / `MdnMicMatched`  (bool): MDN MIC matched?
* `mdn_processing_success` / `MdnProcessingSuccess`  (bool): MDN disposition indicate a successful processing?
* `raw_uri` / `RawUri`  (string): URL to download the original file contents
* `smime_uri` / `SmimeUri`  (string): URL to download the file contents encoded as smime
* `smime_signed_uri` / `SmimeSignedUri`  (string): URL to download the file contents as smime with signature
* `encrypted_uri` / `EncryptedUri`  (string): URL to download the encrypted signed smime that is to sent as AS2 body
* `mdn_response_uri` / `MdnResponseUri`  (string): URL to download the http response body


---

## List As2 Outgoing Messages

```
Task<FilesList<As2OutgoingMessage>> As2OutgoingMessage.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[created_at]=desc`). Valid fields are `created_at` and `as2_partner_id`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `created_at`.
* `filter_gt` (object): If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at`.
* `filter_gteq` (object): If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at`.
* `filter_lt` (object): If set, return records where the specified field is less than the supplied value. Valid fields are `created_at`.
* `filter_lteq` (object): If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at`.
* `as2_partner_id` (Nullable<Int64>): As2 Partner ID.  If provided, will return message specific to that partner.
