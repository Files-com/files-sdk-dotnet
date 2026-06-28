# FilesCom.Models.AiAssistantPersonality

## Example AiAssistantPersonality Object

```
{
  "id": 1,
  "workspace_id": 1,
  "system_prompt": "Respond as a concise operations assistant.",
  "use_by_default": true,
  "apply_to_all_workspaces": true,
  "created_at": "2000-01-01T01:00:00Z",
  "updated_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): AI Assistant Personality ID.
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace ID. `0` means the default workspace.
* `system_prompt` / `SystemPrompt`  (string): System prompt injected into the in-app AI Assistant.
* `use_by_default` / `UseByDefault`  (bool): Whether this personality is the default personality for the Workspace.
* `apply_to_all_workspaces` / `ApplyToAllWorkspaces`  (bool): If true, this default-workspace personality can apply to users in all workspaces.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Creation time.
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): Last update time.


---

## List Ai Assistant Personalities

```
Task<FilesList<AiAssistantPersonality>> AiAssistantPersonality.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id` and `id`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id`.


---

## Show Ai Assistant Personality

```
Task<AiAssistantPersonality> AiAssistantPersonality.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Ai Assistant Personality ID.


---

## Create Ai Assistant Personality

```
Task<AiAssistantPersonality> AiAssistantPersonality.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `apply_to_all_workspaces` (bool): If true, this default-workspace personality can apply to users in all workspaces.
* `system_prompt` (string): Required - System prompt injected into the in-app AI Assistant.
* `use_by_default` (bool): Whether this personality is the default personality for the Workspace.
* `workspace_id` (Nullable<Int64>): Workspace ID. `0` means the default workspace.


---

## Update Ai Assistant Personality

```
Task<AiAssistantPersonality> AiAssistantPersonality.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Ai Assistant Personality ID.
* `apply_to_all_workspaces` (bool): If true, this default-workspace personality can apply to users in all workspaces.
* `system_prompt` (string): System prompt injected into the in-app AI Assistant.
* `use_by_default` (bool): Whether this personality is the default personality for the Workspace.
* `workspace_id` (Nullable<Int64>): Workspace ID. `0` means the default workspace.


---

## Delete Ai Assistant Personality

```
Task AiAssistantPersonality.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Ai Assistant Personality ID.


---

## Update Ai Assistant Personality

```
var AiAssistantPersonality = AiAssistantPersonality.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("apply_to_all_workspaces", false);
parameters.Add("system_prompt", "Respond as a concise operations assistant.");
parameters.Add("use_by_default", false);
parameters.Add("workspace_id", 0);

AiAssistantPersonality.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Ai Assistant Personality ID.
* `apply_to_all_workspaces` (bool): If true, this default-workspace personality can apply to users in all workspaces.
* `system_prompt` (string): System prompt injected into the in-app AI Assistant.
* `use_by_default` (bool): Whether this personality is the default personality for the Workspace.
* `workspace_id` (Nullable<Int64>): Workspace ID. `0` means the default workspace.


---

## Delete Ai Assistant Personality

```
var AiAssistantPersonality = AiAssistantPersonality.Find(1);

var parameters = new Dictionary<string, object>();


AiAssistantPersonality.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Ai Assistant Personality ID.
