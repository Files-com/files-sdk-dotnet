# FilesCom.Models.SharedWorkflow

## Example SharedWorkflow Object

```
{
  "id": 1,
  "name": "example",
  "description": "example",
  "submission_revision": "example",
  "form_fields": [
    {
      "id": 1,
      "label": "Sample Label",
      "required": true,
      "help_text": "Help Text",
      "field_type": "text",
      "options_for_select": [
        "red",
        "green",
        "blue"
      ],
      "default_option": "red",
      "form_field_set_id": 1
    }
  ]
}
```

* `id` / `Id`  (Nullable<Int64>): Shared Workflow ID.
* `name` / `Name`  (string): Name displayed to recipients.
* `description` / `Description`  (string): Description displayed to recipients.
* `submission_revision` / `SubmissionRevision`  (string): Revision required when submitting a Shared Workflow run.
* `form_fields` / `FormFields`  (FormField[]): Current custom questions in display order.


---

## List Shared Workflows

```
FilesList<SharedWorkflow> SharedWorkflow.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials:

```
FilesList<SharedWorkflow> client.SharedWorkflows.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are .


---

## Show Shared Workflow

```
Task<SharedWorkflow> SharedWorkflow.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<SharedWorkflow> client.SharedWorkflows.FindAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Shared Workflow ID.


---

## Show a Shared Workflow run started by the recipient

```
Task<AutomationRun> SharedWorkflow.FindRun(
    Nullable<Int64> id, Nullable<Int64> run_id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<AutomationRun> client.SharedWorkflows.FindRunAsync(
    Nullable<Int64> id, Nullable<Int64> run_id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Shared Workflow ID.
* `run_id` (Nullable<Int64>): Required - ID of the run returned by the Shared Workflow submission.


---

## Run a Shared Workflow

```
Task<AutomationRun> SharedWorkflow.RunWorkflow(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<AutomationRun> client.SharedWorkflows.RunWorkflowAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Shared Workflow ID.
* `submission_revision` (string): Required - Revision returned when opening the Shared Workflow.
* `submission_id` (string): Required - Client-generated UUID used to retry the same submission without starting another run.
* `file_path` (string): Required - Path of a file the recipient can read, including a file uploaded to a Snapshot created for this workflow.


---

## Run a Shared Workflow

```
var sharedWorkflow = await SharedWorkflow.Find(1L);

var parameters = new Dictionary<string, object>();

parameters.Add("submission_revision", "submission_revision");
parameters.Add("submission_id", "submission_id");
parameters.Add("file_path", "file_path");

await sharedWorkflow.RunWorkflow(parameters);
```

`RunWorkflowAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.

### Parameters

* `id` (Nullable<Int64>): Required - Shared Workflow ID.
* `submission_revision` (string): Required - Revision returned when opening the Shared Workflow.
* `submission_id` (string): Required - Client-generated UUID used to retry the same submission without starting another run.
* `file_path` (string): Required - Path of a file the recipient can read, including a file uploaded to a Snapshot created for this workflow.
