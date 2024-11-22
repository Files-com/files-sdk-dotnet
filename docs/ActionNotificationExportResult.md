# FilesCom.Models.ActionNotificationExportResult

## Example ActionNotificationExportResult Object

```
{
  "id": 1,
  "created_at": 1,
  "status": 200,
  "message": "Success",
  "success": true,
  "request_headers": "{\"User-Agent\":\"Files.com Webhook\"}",
  "request_method": "GET",
  "request_url": "www.example.com/webhook_receiver",
  "path": "MyFolder/MyFile.txt",
  "folder": "MyFolder"
}
```

* `id` / `Id`  (Nullable<Int64>): Notification ID
* `created_at` / `CreatedAt`  (Nullable<Int64>): When the notification was sent.
* `status` / `Status`  (Nullable<Int64>): HTTP status code returned in the webhook response.
* `message` / `Message`  (string): A message indicating the overall status of the webhook notification.
* `success` / `Success`  (bool): `true` if the webhook succeeded by receiving a 200 or 204 response.
* `request_headers` / `RequestHeaders`  (string): A JSON-encoded string with headers that were sent with the webhook.
* `request_method` / `RequestMethod`  (string): The HTTP verb used to perform the webhook.
* `request_url` / `RequestUrl`  (string): The webhook request URL.
* `path` / `Path`  (string): The path to the actual file that triggered this notification. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `folder` / `Folder`  (string): The folder associated with the triggering action for this notification.


---

## List Action Notification Export Results

```
Task<FilesList<ActionNotificationExportResult>> ActionNotificationExportResult.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action_notification_export_id` (Nullable<Int64>): Required - ID of the associated action notification export.
