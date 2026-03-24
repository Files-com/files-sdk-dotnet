# FilesCom.Models.ExpectationIncident

## Example ExpectationIncident Object

```
{
  "id": 1,
  "workspace_id": 1,
  "expectation_id": 1,
  "status": "open",
  "opened_at": "2000-01-01T01:00:00Z",
  "last_failed_at": "2000-01-01T01:00:00Z",
  "acknowledged_at": "2000-01-01T01:00:00Z",
  "snoozed_until": "2000-01-01T01:00:00Z",
  "resolved_at": "2000-01-01T01:00:00Z",
  "opened_by_evaluation_id": 1,
  "last_evaluation_id": 2,
  "resolved_by_evaluation_id": 3,
  "summary": null,
  "created_at": "2000-01-01T01:00:00Z",
  "updated_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): Expectation Incident ID
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace ID. `0` means the default workspace.
* `expectation_id` / `ExpectationId`  (Nullable<Int64>): Expectation ID.
* `status` / `Status`  (string): Incident status.
* `opened_at` / `OpenedAt`  (Nullable<DateTime>): When the incident was opened.
* `last_failed_at` / `LastFailedAt`  (Nullable<DateTime>): When the most recent failing evaluation contributing to the incident occurred.
* `acknowledged_at` / `AcknowledgedAt`  (Nullable<DateTime>): When the incident was acknowledged.
* `snoozed_until` / `SnoozedUntil`  (Nullable<DateTime>): When the current snooze expires.
* `resolved_at` / `ResolvedAt`  (Nullable<DateTime>): When the incident was resolved.
* `opened_by_evaluation_id` / `OpenedByEvaluationId`  (Nullable<Int64>): Evaluation that first opened the incident.
* `last_evaluation_id` / `LastEvaluationId`  (Nullable<Int64>): Most recent evaluation linked to the incident.
* `resolved_by_evaluation_id` / `ResolvedByEvaluationId`  (Nullable<Int64>): Evaluation that resolved the incident.
* `summary` / `Summary`  (object): Compact incident summary payload.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Creation time.
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): Last update time.


---

## List Expectation Incidents

```
Task<FilesList<ExpectationIncident>> ExpectationIncident.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `created_at` or `expectation_id`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `expectation_id` and `workspace_id`. Valid field combinations are `[ workspace_id, expectation_id ]`.


---

## Show Expectation Incident

```
Task<ExpectationIncident> ExpectationIncident.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Expectation Incident ID.


---

## Resolve an expectation incident

```
Task<ExpectationIncident> ExpectationIncident.Resolve(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Expectation Incident ID.


---

## Snooze an expectation incident until a specified time

```
Task<ExpectationIncident> ExpectationIncident.Snooze(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Expectation Incident ID.
* `snoozed_until` (string): Required - Time until which the incident should remain snoozed.


---

## Acknowledge an expectation incident

```
Task<ExpectationIncident> ExpectationIncident.Acknowledge(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Expectation Incident ID.


---

## Resolve an expectation incident

```
var ExpectationIncident = ExpectationIncident.Find(1);

var parameters = new Dictionary<string, object>();


ExpectationIncident.Resolve
```

### Parameters

* `id` (Nullable<Int64>): Required - Expectation Incident ID.


---

## Snooze an expectation incident until a specified time

```
var ExpectationIncident = ExpectationIncident.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("snoozed_until", "snoozed_until");

ExpectationIncident.Snooze(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Expectation Incident ID.
* `snoozed_until` (string): Required - Time until which the incident should remain snoozed.


---

## Acknowledge an expectation incident

```
var ExpectationIncident = ExpectationIncident.Find(1);

var parameters = new Dictionary<string, object>();


ExpectationIncident.Acknowledge
```

### Parameters

* `id` (Nullable<Int64>): Required - Expectation Incident ID.
