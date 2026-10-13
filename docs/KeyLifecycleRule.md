# FilesCom.Models.KeyLifecycleRule

## Example KeyLifecycleRule Object

```
{
  "id": 1,
  "key_type": "gpg",
  "inactivity_days": 12,
  "expiration_days": 365,
  "apply_to_all_workspaces": true,
  "name": "inactive gpg keys",
  "workspace_id": 12
}
```

* `id` / `Id`  (Nullable<Int64>): Key Lifecycle Rule ID
* `key_type` / `KeyType`  (string): Key type for which the rule will apply (gpg, ssh, or api).
* `inactivity_days` / `InactivityDays`  (Nullable<Int64>): Number of days of inactivity before the rule applies.
* `expiration_days` / `ExpirationDays`  (Nullable<Int64>): Number of days after creation before an SSH key expires. Applies only to SSH keys.
* `apply_to_all_workspaces` / `ApplyToAllWorkspaces`  (bool): If true, a default-workspace rule also applies to keys in all workspaces.
* `name` / `Name`  (string): Key Lifecycle Rule name
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace ID. `0` means the default workspace.


---

## List Key Lifecycle Rules

```
FilesList<KeyLifecycleRule> KeyLifecycleRule.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials:

```
FilesList<KeyLifecycleRule> client.KeyLifecycleRules.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id` and `key_type`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `key_type` and `workspace_id`. Valid field combinations are `[ workspace_id, key_type ]`.


---

## Show Key Lifecycle Rule

```
Task<KeyLifecycleRule> KeyLifecycleRule.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<KeyLifecycleRule> client.KeyLifecycleRules.FindAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Key Lifecycle Rule ID.


---

## Create Key Lifecycle Rule

```
Task<KeyLifecycleRule> KeyLifecycleRule.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<KeyLifecycleRule> client.KeyLifecycleRules.CreateAsync(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `apply_to_all_workspaces` (bool): If true, a default-workspace rule also applies to keys in all workspaces.
* `expiration_days` (Nullable<Int64>): Number of days after creation before an SSH key expires. Applies only to SSH keys.
* `key_type` (string): Key type for which the rule will apply (gpg, ssh, or api).
* `inactivity_days` (Nullable<Int64>): Number of days of inactivity before the rule applies.
* `name` (string): Key Lifecycle Rule name
* `workspace_id` (Nullable<Int64>): Workspace ID. `0` means the default workspace.


---

## Update Key Lifecycle Rule

```
Task<KeyLifecycleRule> KeyLifecycleRule.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<KeyLifecycleRule> client.KeyLifecycleRules.UpdateAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Key Lifecycle Rule ID.
* `apply_to_all_workspaces` (bool): If true, a default-workspace rule also applies to keys in all workspaces.
* `expiration_days` (Nullable<Int64>): Number of days after creation before an SSH key expires. Applies only to SSH keys.
* `key_type` (string): Key type for which the rule will apply (gpg, ssh, or api).
* `inactivity_days` (Nullable<Int64>): Number of days of inactivity before the rule applies.
* `name` (string): Key Lifecycle Rule name
* `workspace_id` (Nullable<Int64>): Workspace ID. `0` means the default workspace.


---

## Delete Key Lifecycle Rule

```
Task KeyLifecycleRule.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task client.KeyLifecycleRules.DeleteAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Key Lifecycle Rule ID.


---

## Update Key Lifecycle Rule

```
var keyLifecycleRule = await KeyLifecycleRule.Find(1L);

var parameters = new Dictionary<string, object>();

parameters.Add("apply_to_all_workspaces", true);
parameters.Add("expiration_days", 365L);
parameters.Add("key_type", "gpg");
parameters.Add("inactivity_days", 12L);
parameters.Add("name", "inactive gpg keys");
parameters.Add("workspace_id", 12L);

await keyLifecycleRule.Update(parameters);
```

`UpdateAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.
When it succeeds, the object takes the attributes Files.com returns. The `KeyLifecycleRule` it returns is a separate object with those attributes.

### Parameters

* `id` (Nullable<Int64>): Required - Key Lifecycle Rule ID.
* `apply_to_all_workspaces` (bool): If true, a default-workspace rule also applies to keys in all workspaces.
* `expiration_days` (Nullable<Int64>): Number of days after creation before an SSH key expires. Applies only to SSH keys.
* `key_type` (string): Key type for which the rule will apply (gpg, ssh, or api).
* `inactivity_days` (Nullable<Int64>): Number of days of inactivity before the rule applies.
* `name` (string): Key Lifecycle Rule name
* `workspace_id` (Nullable<Int64>): Workspace ID. `0` means the default workspace.


---

## Delete Key Lifecycle Rule

```
var keyLifecycleRule = await KeyLifecycleRule.Find(1L);

var parameters = new Dictionary<string, object>();

await keyLifecycleRule.Delete(parameters);
```

`DeleteAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.

### Parameters

* `id` (Nullable<Int64>): Required - Key Lifecycle Rule ID.


---

## Save

```
await keyLifecycleRule.SaveAsync(cancellationToken);
```

Creates the object on Files.com, or updates it if it has an ID. It sends the attributes set on the object, in its constructor or with its properties, or changed in place, such as an element of an array, since it was made or last came from Files.com, and only those the operation takes, each in the form its parameter takes. When it succeeds, the object takes the attributes Files.com returns. `Save()` does the same without a cancellation token.
