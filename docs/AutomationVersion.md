# FilesCom.Models.AutomationVersion

## Example AutomationVersion Object

```
{
  "id": 1,
  "automation_id": 1,
  "workspace_id": 1,
  "version": 1,
  "created_at": "2000-01-01T01:00:00Z",
  "user_id": 1,
  "username": "example",
  "api_key_id": 1,
  "definition": {
    "schema_version": 1,
    "nodes": [
      {
        "id": "trigger",
        "type": "trigger_manual"
      },
      {
        "id": "create_reports",
        "type": "create_folder",
        "config": {
          "destinations": [
            "reports/"
          ]
        }
      }
    ],
    "edges": [
      {
        "from": "trigger",
        "to": "create_reports"
      }
    ]
  }
}
```

* `id` / `Id`  (Nullable<Int64>): Automation revision ID.
* `automation_id` / `AutomationId`  (Nullable<Int64>): ID of the Automation that owns this revision.
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace that owns the Automation. Zero is the Default Workspace.
* `version` / `Version`  (Nullable<Int64>): Revision number within the Automation.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Time the revision was saved.
* `user_id` / `UserId`  (Nullable<Int64>): ID of the user who saved the revision, when available.
* `username` / `Username`  (string): Username recorded when the revision was saved, when available.
* `api_key_id` / `ApiKeyId`  (Nullable<Int64>): ID of the API key used to save the revision, when available.
* `definition` / `Definition`  (object): Saved Automation v2 graph definition. Included when retrieving a single revision.


---

## List Automation Versions

```
FilesList<AutomationVersion> AutomationVersion.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials:

```
FilesList<AutomationVersion> client.AutomationVersions.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are .
* `automation_id` (Nullable<Int64>): Required - ID of the Automation that owns this revision.


---

## Show Automation Version

```
Task<AutomationVersion> AutomationVersion.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<AutomationVersion> client.AutomationVersions.FindAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Automation Version ID.


---

## Restore an Automation v2 definition as a new current version

```
Task<Automation> AutomationVersion.Restore(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<Automation> client.AutomationVersions.RestoreAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Automation Version ID.


---

## Restore an Automation v2 definition as a new current version

```
var automationVersion = await AutomationVersion.Find(1L);

var parameters = new Dictionary<string, object>();

await automationVersion.Restore(parameters);
```

`RestoreAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.

### Parameters

* `id` (Nullable<Int64>): Required - Automation Version ID.
