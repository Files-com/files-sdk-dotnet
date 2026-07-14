# FilesCom.Models.AutomationRun

## Example AutomationRun Object

```
{
  "id": 1,
  "automation_id": 1,
  "automation_version_id": 1,
  "workspace_id": 1,
  "cancel_requested_at": "2000-01-01T01:00:00Z",
  "completed_at": "2000-01-01T01:00:00Z",
  "created_at": "2000-01-01T01:00:00Z",
  "retry_at": "2000-01-01T01:00:00Z",
  "retried_at": "2000-01-01T01:00:00Z",
  "retried_in_run_id": 1,
  "retry_of_run_id": 1,
  "rerun_of_run_id": 1,
  "rerun_from_node_id": "example",
  "runtime": 1.0,
  "status": "success",
  "successful_operations": 1,
  "failed_operations": 1,
  "definition": "example",
  "node_states": "example",
  "execution_nodes": [
    {
      "node_id": "example",
      "node_type": "example",
      "status": "example",
      "run_stage": "example",
      "reused": true,
      "successful_operations": 1,
      "failed_operations": 1,
      "started_at": "2000-01-01T01:00:00Z",
      "completed_at": "2000-01-01T01:00:00Z",
      "duration_ms": 1,
      "inputs": [
        "example"
      ],
      "outputs": "example",
      "input_items": "example",
      "output_items": "example"
    }
  ],
  "journal_url": "example",
  "status_messages_url": "https://www.example.com/log_file.txt"
}
```

* `id` / `Id`  (Nullable<Int64>): ID.
* `automation_id` / `AutomationId`  (Nullable<Int64>): ID of the associated Automation.
* `automation_version_id` / `AutomationVersionId`  (Nullable<Int64>): ID of the immutable Automation version pinned by this run.
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace ID.
* `cancel_requested_at` / `CancelRequestedAt`  (Nullable<DateTime>): Date/time at which cancellation was requested.
* `completed_at` / `CompletedAt`  (Nullable<DateTime>): Automation run completion/failure date/time.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Automation run start date/time.
* `retry_at` / `RetryAt`  (Nullable<DateTime>): If set, this automation will be retried at this date/time due to `failure` or `partial_failure`.
* `retried_at` / `RetriedAt`  (Nullable<DateTime>): If set, this Automation run was retried due to `failure` or `partial_failure`.
* `retried_in_run_id` / `RetriedInRunId`  (Nullable<Int64>): ID of the run that is or will be retrying this run.
* `retry_of_run_id` / `RetryOfRunId`  (Nullable<Int64>): ID of the original run that this run is retrying.
* `rerun_of_run_id` / `RerunOfRunId`  (Nullable<Int64>): ID of the run whose persisted node outputs this run reused.
* `rerun_from_node_id` / `RerunFromNodeId`  (string): Node at which this run resumed execution.
* `runtime` / `Runtime`  (Nullable<double>): Automation run runtime.
* `status` / `Status`  (string): The status of the AutomationRun. One of `queued`, `running`, `success`, `partial_failure`, `failure`, `skipped`, or `canceled`.
* `successful_operations` / `SuccessfulOperations`  (Nullable<Int64>): Count of successful operations.
* `failed_operations` / `FailedOperations`  (Nullable<Int64>): Count of failed operations.
* `definition` / `Definition`  (object): Automation definition snapshot pinned by this run. For performance reasons, this is not provided when listing Automation runs.
* `node_states` / `NodeStates`  (object): Status and execution stage for each node in this run. For performance reasons, this is not provided when listing Automation runs.
* `execution_nodes` / `ExecutionNodes`  (AutomationExecutionNode[]): Execution status, timing, and bounded output summaries for each node. For performance reasons, this is not provided when listing Automation runs.
* `journal_url` / `JournalUrl`  (string): Link to the run journal artifact.
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

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `automation_id`, `created_at` or `status`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `status`, `workspace_id` or `automation_id`. Valid field combinations are `[ workspace_id, status ]`, `[ automation_id, status ]`, `[ workspace_id, automation_id ]` or `[ workspace_id, automation_id, status ]`.
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


---

## Show Automation Run Node

```
Task<AutomationExecutionNode> AutomationRun.FindNode(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Automation Run ID.
* `node_id` (string): Required - Node ID from the pinned Automation definition.


---

## Cancel Automation Run

```
Task<AutomationRun> AutomationRun.Cancel(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Automation Run ID.


---

## Re-run Automation from Node

```
Task<AutomationRun> AutomationRun.Rerun(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Automation Run ID.
* `node_id` (string): Required - Node ID at which execution should resume.


---

## Cancel Automation Run

```
var AutomationRun = AutomationRun.Find(1);

var parameters = new Dictionary<string, object>();


AutomationRun.Cancel
```

### Parameters

* `id` (Nullable<Int64>): Required - Automation Run ID.


---

## Re-run Automation from Node

```
var AutomationRun = AutomationRun.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("node_id", "node_id");

AutomationRun.Rerun(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Automation Run ID.
* `node_id` (string): Required - Node ID at which execution should resume.
