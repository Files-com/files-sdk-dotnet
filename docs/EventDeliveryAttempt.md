# FilesCom.Models.EventDeliveryAttempt

## Example EventDeliveryAttempt Object

```
{
  "id": 1,
  "event_record_id": 1,
  "event_subscription_id": 1,
  "event_target_id": 1,
  "workspace_id": 1,
  "status": "example",
  "attempt_number": 1,
  "response_code": 1,
  "error_message": "example",
  "response_body": "example",
  "latency_ms": 1,
  "delivered_at": "2000-01-01T01:00:00Z",
  "last_attempted_at": "2000-01-01T01:00:00Z",
  "next_attempt_at": "2000-01-01T01:00:00Z",
  "created_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): Event Delivery Attempt ID
* `event_record_id` / `EventRecordId`  (Nullable<Int64>): Event Record ID
* `event_subscription_id` / `EventSubscriptionId`  (Nullable<Int64>): Event Subscription ID
* `event_target_id` / `EventTargetId`  (Nullable<Int64>): Event Target ID
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace ID. 0 means the default workspace or site-wide.
* `status` / `Status`  (string): Delivery status.
* `attempt_number` / `AttemptNumber`  (Nullable<Int64>): Number of delivery attempts made.
* `response_code` / `ResponseCode`  (Nullable<Int64>): HTTP response code, if applicable.
* `error_message` / `ErrorMessage`  (string): Delivery error message, if applicable.
* `response_body` / `ResponseBody`  (string): Delivery response body, if applicable.
* `latency_ms` / `LatencyMs`  (Nullable<Int64>): Delivery latency in milliseconds.
* `delivered_at` / `DeliveredAt`  (Nullable<DateTime>): Successful delivery date/time.
* `last_attempted_at` / `LastAttemptedAt`  (Nullable<DateTime>): Most recent attempt date/time.
* `next_attempt_at` / `NextAttemptAt`  (Nullable<DateTime>): Next scheduled attempt date/time.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Delivery Attempt create date/time.


---

## List Event Delivery Attempts

```
Task<FilesList<EventDeliveryAttempt>> EventDeliveryAttempt.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `status`, `event_record_id`, `event_target_id` or `workspace_id`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `status`, `workspace_id`, `event_record_id` or `event_target_id`. Valid field combinations are `[ workspace_id, status ]`, `[ workspace_id, event_record_id ]` or `[ workspace_id, event_target_id ]`.


---

## Show Event Delivery Attempt

```
Task<EventDeliveryAttempt> EventDeliveryAttempt.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Event Delivery Attempt ID.
