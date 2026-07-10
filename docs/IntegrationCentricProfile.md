# FilesCom.Models.IntegrationCentricProfile

## Example IntegrationCentricProfile Object

```
{
  "id": 1,
  "name": "Business Systems Onboarding",
  "workspace_id": 1,
  "use_for_all_users": true,
  "expected_remote_servers": [
    "example"
  ]
}
```

* `id` / `Id`  (Nullable<Int64>): Integration Centric Profile ID
* `name` / `Name`  (string): Profile name
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace ID
* `use_for_all_users` / `UseForAllUsers`  (bool): Whether this profile applies to all users in the Workspace by default
* `expected_remote_servers` / `ExpectedRemoteServers`  (object[]): Remote Server integrations the user is expected to add and connect. Each entry requires `server_type` and may include a display `name`.


---

## List Integration Centric Profiles

```
Task<FilesList<IntegrationCentricProfile>> IntegrationCentricProfile.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id` and `name`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id`.


---

## Show Integration Centric Profile

```
Task<IntegrationCentricProfile> IntegrationCentricProfile.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Integration Centric Profile ID.


---

## Create Integration Centric Profile

```
Task<IntegrationCentricProfile> IntegrationCentricProfile.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `name` (string): Required - Profile name
* `expected_remote_servers` (object[]): Required - Remote Server integrations the user is expected to add and connect. Each entry requires `server_type` and may include a display `name`.
* `workspace_id` (Nullable<Int64>): Workspace ID
* `use_for_all_users` (bool): Whether this profile applies to all users in the Workspace by default


---

## Update Integration Centric Profile

```
Task<IntegrationCentricProfile> IntegrationCentricProfile.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Integration Centric Profile ID.
* `name` (string): Profile name
* `workspace_id` (Nullable<Int64>): Workspace ID
* `expected_remote_servers` (object[]): Remote Server integrations the user is expected to add and connect. Each entry requires `server_type` and may include a display `name`.
* `use_for_all_users` (bool): Whether this profile applies to all users in the Workspace by default


---

## Delete Integration Centric Profile

```
Task IntegrationCentricProfile.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Integration Centric Profile ID.


---

## Update Integration Centric Profile

```
var IntegrationCentricProfile = IntegrationCentricProfile.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("name", "Business Systems Onboarding");
parameters.Add("workspace_id", 1);
parameters.Add("expected_remote_servers", ["example"]);
parameters.Add("use_for_all_users", false);

IntegrationCentricProfile.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Integration Centric Profile ID.
* `name` (string): Profile name
* `workspace_id` (Nullable<Int64>): Workspace ID
* `expected_remote_servers` (object[]): Remote Server integrations the user is expected to add and connect. Each entry requires `server_type` and may include a display `name`.
* `use_for_all_users` (bool): Whether this profile applies to all users in the Workspace by default


---

## Delete Integration Centric Profile

```
var IntegrationCentricProfile = IntegrationCentricProfile.Find(1);

var parameters = new Dictionary<string, object>();


IntegrationCentricProfile.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Integration Centric Profile ID.
