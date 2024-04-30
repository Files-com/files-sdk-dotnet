# FilesCom.Models.AutomationRun

## Example AutomationRun Object

```
{
  "id": 1,
  "automation_id": 1,
  "completed_at": "2000-01-01T01:00:00Z",
  "created_at": "2000-01-01T01:00:00Z",
  "runtime": 1.0,
  "status": "success",
  "run_stage": "planning",
  "successful_operations": 1,
  "failed_operations": 1,
  "status_messages_url": "https://www.example.com/log_file.txt"
}
```

* `id` / `Id`  (Nullable<Int64>): ID.
* `automation_id` / `AutomationId`  (Nullable<Int64>): ID of the associated Automation.
* `completed_at` / `CompletedAt`  (Nullable<DateTime>): Automation run completion/failure date/time.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Automation run start date/time.
* `runtime` / `Runtime`  (double): Automation run runtime.
* `status` / `Status`  (string): The success status of the AutomationRun. One of `running`, `success`, `partial_failure`, or `failure`.
* `run_stage` / `RunStage`  (string): The stage currently being executed in the execution environment.  One of `queued_for_planning`, `planning`, `queued_for_execution`, `executing`, or `finished`.
* `successful_operations` / `SuccessfulOperations`  (Nullable<Int64>): Count of successful operations.
* `failed_operations` / `FailedOperations`  (Nullable<Int64>): Count of failed operations.
* `status_messages_url` / `StatusMessagesUrl`  (string): Link to status messages log file.


---

## List Automation Runs

```
Task<FilesList<AutomationRun>> AutomationRun.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[automation_id]=desc`). Valid fields are `automation_id`, `created_at` or `status`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `status` and `automation_id`. Valid field combinations are `[ automation_id, status ]`.
* `automation_id` (Nullable<Int64>): Required - ID of the associated Automation.


---

## Show Automation Run

```
Task<AutomationRun> AutomationRun.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Automation Run ID.
