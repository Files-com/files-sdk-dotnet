# FilesCom.Models.ActionNotificationExport

## Example ActionNotificationExport Object

```
{
  "id": 1,
  "export_version": "example",
  "start_at": "2000-01-01T01:00:00Z",
  "end_at": "2000-01-01T01:00:00Z",
  "status": "ready",
  "query_path": "MyFile.txt",
  "query_folder": "MyFolder",
  "query_message": "Connection Refused",
  "query_request_method": "GET",
  "query_request_url": "http://example.com/webhook",
  "query_status": "200",
  "query_success": true,
  "results_url": "https://files.com/action_notification_results.csv"
}
```

* `id` / `Id`  (Nullable<Int64>): History Export ID
* `export_version` / `ExportVersion`  (string): Version of the underlying records for the export.
* `start_at` / `StartAt`  (Nullable<DateTime>): Start date/time of export range.
* `end_at` / `EndAt`  (Nullable<DateTime>): End date/time of export range.
* `status` / `Status`  (string): Status of export.  Valid values: `building`, `ready`, or `failed`
* `query_path` / `QueryPath`  (string): Return notifications that were triggered by actions on this specific path.
* `query_folder` / `QueryFolder`  (string): Return notifications that were triggered by actions in this folder.
* `query_message` / `QueryMessage`  (string): Error message associated with the request, if any.
* `query_request_method` / `QueryRequestMethod`  (string): The HTTP request method used by the webhook.
* `query_request_url` / `QueryRequestUrl`  (string): The target webhook URL.
* `query_status` / `QueryStatus`  (string): The HTTP status returned from the server in response to the webhook request.
* `query_success` / `QuerySuccess`  (bool): true if the webhook request succeeded (i.e. returned a 200 or 204 response status). false otherwise.
* `results_url` / `ResultsUrl`  (string): If `status` is `ready`, this will be a URL where all the results can be downloaded at once as a CSV.
* `user_id` / `UserId`  (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.


---

## Show Action Notification Export

```
Task<ActionNotificationExport> ActionNotificationExport.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Action Notification Export ID.


---

## Create Action Notification Export

```
Task<ActionNotificationExport> ActionNotificationExport.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `start_at` (string): Start date/time of export range.
* `end_at` (string): End date/time of export range.
* `query_message` (string): Error message associated with the request, if any.
* `query_request_method` (string): The HTTP request method used by the webhook.
* `query_request_url` (string): The target webhook URL.
* `query_status` (string): The HTTP status returned from the server in response to the webhook request.
* `query_success` (bool): true if the webhook request succeeded (i.e. returned a 200 or 204 response status). false otherwise.
* `query_path` (string): Return notifications that were triggered by actions on this specific path.
* `query_folder` (string): Return notifications that were triggered by actions in this folder.
