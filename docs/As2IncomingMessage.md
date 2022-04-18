# FilesCom.Models.As2IncomingMessage

## Example As2IncomingMessage Object

```
{
  "id": 1,
  "as2_partner_id": 1,
  "as2_station_id": 1,
  "uuid": "",
  "content_type": "",
  "http_headers": "",
  "activity_log": "",
  "processing_result": "",
  "mic": "",
  "mic_algo": "",
  "as2_to": "",
  "as2_from": "",
  "message_id": "",
  "subject": "",
  "date": "",
  "body_size": "",
  "attachment_filename": "",
  "ip": "",
  "created_at": "2000-01-01T01:00:00Z",
  "http_response_code": "",
  "http_response_headers": "",
  "recipient_serial": "",
  "hex_recipient_serial": "A5:EB:C1:95:DC:D8:2B:E7",
  "recipient_issuer": "",
  "message_received": true,
  "message_decrypted": true,
  "message_signature_verified": true,
  "message_processing_success": true,
  "message_mdn_returned": true,
  "encrypted_uri": "",
  "smime_signed_uri": "",
  "smime_uri": "",
  "raw_uri": "",
  "mdn_response_uri": ""
}
```

* `id` / `Id`  (Nullable<Int64>): Id of the AS2 Partner.
* `as2_partner_id` / `As2PartnerId`  (Nullable<Int64>): Id of the AS2 Partner associated with this message.
* `as2_station_id` / `As2StationId`  (Nullable<Int64>): Id of the AS2 Station associated with this message.
* `uuid` / `Uuid`  (string): UUID assigned to this message.
* `content_type` / `ContentType`  (string): Content Type header of the incoming message.
* `http_headers` / `HttpHeaders`  (object): HTTP Headers sent with this message.
* `activity_log` / `ActivityLog`  (string): JSON Structure of the activity log.
* `processing_result` / `ProcessingResult`  (string): Result of processing.
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

## List As2 Incoming Messages

```
Task<As2IncomingMessage[]> As2IncomingMessage.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via either the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `as2_partner_id` (Nullable<Int64>): As2 Partner ID.  If provided, will return message specific to that partner.
