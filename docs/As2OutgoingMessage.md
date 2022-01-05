# FilesCom.Models.As2OutgoingMessage

## Example As2OutgoingMessage Object

```
{
  "id": 1,
  "as2_partner_id": 1,
  "uuid": "",
  "http_headers": "",
  "activity_log": "",
  "processing_result": "",
  "mic": "",
  "message_id": "",
  "body_size": "",
  "attachment_filename": "",
  "created_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): Id of the AS2 Partner.
* `as2_partner_id` / `As2PartnerId`  (Nullable<Int64>): Id of the AS2 Partner associated with this message.
* `uuid` / `Uuid`  (string): UUID assigned to this message.
* `http_headers` / `HttpHeaders`  (object): HTTP Headers sent with this message.
* `activity_log` / `ActivityLog`  (string): JSON Structure of the activity log.
* `processing_result` / `ProcessingResult`  (string): Result of processing. Valid values: `send_failed`, `send_success`
* `mic` / `Mic`  (string): AS2 Message Integrity Check
* `message_id` / `MessageId`  (string): AS2 Message Id
* `body_size` / `BodySize`  (string): Encrypted Payload Body Size
* `attachment_filename` / `AttachmentFilename`  (string): Filename of the file being sent.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Message creation date/time


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
