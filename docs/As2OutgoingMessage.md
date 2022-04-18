# FilesCom.Models.As2OutgoingMessage

## Example As2OutgoingMessage Object

```
{
  "id": 1,
  "as2_partner_id": 1,
  "as2_station_id": 1,
  "uuid": "",
  "http_headers": "",
  "activity_log": "",
  "processing_result": "",
  "mic": "",
  "mic_sha_256": "",
  "as2_to": "",
  "as2_from": "",
  "date": "",
  "message_id": "",
  "body_size": "",
  "attachment_filename": "",
  "created_at": "2000-01-01T01:00:00Z",
  "http_response_code": "",
  "http_response_headers": "",
  "mdn_received": true,
  "mdn_valid": true,
  "mdn_signature_verified": true,
  "mdn_message_id_matched": true,
  "mdn_mic_matched": true,
  "mdn_processing_success": true,
  "raw_uri": "",
  "smime_uri": "",
  "smime_signed_uri": "",
  "encrypted_uri": "",
  "mdn_response_uri": ""
}
```

* `id` / `Id`  (Nullable<Int64>): Id of the AS2 Partner.
* `as2_partner_id` / `As2PartnerId`  (Nullable<Int64>): Id of the AS2 Partner associated with this message.
* `as2_station_id` / `As2StationId`  (Nullable<Int64>): Id of the AS2 Station associated with this message.
* `uuid` / `Uuid`  (string): UUID assigned to this message.
* `http_headers` / `HttpHeaders`  (object): HTTP Headers sent with this message.
* `activity_log` / `ActivityLog`  (string): JSON Structure of the activity log.
* `processing_result` / `ProcessingResult`  (string): Result of processing.
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
Task<As2OutgoingMessage[]> As2OutgoingMessage.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via either the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `as2_partner_id` (Nullable<Int64>): As2 Partner ID.  If provided, will return message specific to that partner.
