# FilesCom.Models.IntegrationCentricProfile

## Example IntegrationCentricProfile Object

```
{
  "id": 1,
  "name": "Business Systems Onboarding",
  "workspace_id": 1,
  "use_for_all_users": false,
  "expected_remote_servers": [
    {
      "server_type": "dropbox",
      "name": "Dropbox"
    }
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
FilesList<IntegrationCentricProfile> IntegrationCentricProfile.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials:

```
FilesList<IntegrationCentricProfile> client.IntegrationCentricProfiles.List(
    
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

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<IntegrationCentricProfile> client.IntegrationCentricProfiles.FindAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
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

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<IntegrationCentricProfile> client.IntegrationCentricProfiles.CreateAsync(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
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

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<IntegrationCentricProfile> client.IntegrationCentricProfiles.UpdateAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
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

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task client.IntegrationCentricProfiles.DeleteAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Integration Centric Profile ID.


---

## Update Integration Centric Profile

```
var integrationCentricProfile = await IntegrationCentricProfile.Find(1L);

var parameters = new Dictionary<string, object>();

parameters.Add("name", "Business Systems Onboarding");
parameters.Add("workspace_id", 1L);
parameters.Add("expected_remote_servers", new object[] { new Dictionary<string, object> { { "server_type", "dropbox" }, { "name", "Dropbox" } } });
parameters.Add("use_for_all_users", false);

await integrationCentricProfile.Update(parameters);
```

`UpdateAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.
When it succeeds, the object takes the attributes Files.com returns. The `IntegrationCentricProfile` it returns is a separate object with those attributes.

### Parameters

* `id` (Nullable<Int64>): Required - Integration Centric Profile ID.
* `name` (string): Profile name
* `workspace_id` (Nullable<Int64>): Workspace ID
* `expected_remote_servers` (object[]): Remote Server integrations the user is expected to add and connect. Each entry requires `server_type` and may include a display `name`.
* `use_for_all_users` (bool): Whether this profile applies to all users in the Workspace by default


---

## Delete Integration Centric Profile

```
var integrationCentricProfile = await IntegrationCentricProfile.Find(1L);

var parameters = new Dictionary<string, object>();

await integrationCentricProfile.Delete(parameters);
```

`DeleteAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.

### Parameters

* `id` (Nullable<Int64>): Required - Integration Centric Profile ID.


---

## Save

```
await integrationCentricProfile.SaveAsync(cancellationToken);
```

Creates the object on Files.com, or updates it if it has an ID. It sends the attributes set on the object, in its constructor or with its properties, or changed in place, such as an element of an array, since it was made or last came from Files.com, and only those the operation takes, each in the form its parameter takes. When it succeeds, the object takes the attributes Files.com returns. `Save()` does the same without a cancellation token.
