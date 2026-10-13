# FilesCom.Models.ChildSiteManagementPolicy

## Example ChildSiteManagementPolicy Object

```
{
  "id": 1,
  "policy_type": "settings",
  "name": "example",
  "description": "example",
  "value": "{ \"color2_left\": \"#000000\", \"folder_behaviors\": [{ \"behavior\": \"file_expiration\", \"value\": { \"days_to_retain\": 30 } }] }",
  "applied_child_site_ids": [
    1,
    2
  ],
  "skip_child_site_ids": [
    1,
    2
  ],
  "child_site_ids": [
    1,
    2
  ],
  "default_policy": true,
  "created_at": "2000-01-01T01:00:00Z",
  "updated_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): Policy ID.
* `policy_type` / `PolicyType`  (string): Type of policy.  Valid values: `settings`.
* `name` / `Name`  (string): Name for this policy.
* `description` / `Description`  (string): Description for this policy.
* `value` / `Value`  (object): Policy configuration data. On a Child Site, contains only managed setting names with null values. Settings policies accept site settings plus an optional `folder_behaviors` array for parent-managed root behaviors on child sites. For more information, refer to the Value Hash section of the developer documentation.
* `applied_child_site_ids` / `AppliedChildSiteIds`  (Nullable<Int64>[]): IDs of child sites that this policy has been applied to. This field is read-only.
* `skip_child_site_ids` / `SkipChildSiteIds`  (Nullable<Int64>[]): IDs of child sites excluded from this default policy.
* `child_site_ids` / `ChildSiteIds`  (Nullable<Int64>[]): IDs of child sites explicitly assigned to this non-default policy.
* `default_policy` / `DefaultPolicy`  (bool): Whether this policy applies to child sites not explicitly assigned to another policy.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): When this policy was created.
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): When this policy was last updated.


---

## List Child Site Management Policies. On a Child Site, returns only the policy covering that site. Its value contains managed setting names with null values

```
FilesList<ChildSiteManagementPolicy> ChildSiteManagementPolicy.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials:

```
FilesList<ChildSiteManagementPolicy> client.ChildSiteManagementPolicies.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).


---

## Show Child Site Management Policy

```
Task<ChildSiteManagementPolicy> ChildSiteManagementPolicy.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<ChildSiteManagementPolicy> client.ChildSiteManagementPolicies.FindAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Child Site Management Policy ID.


---

## Create Child Site Management Policy

```
Task<ChildSiteManagementPolicy> ChildSiteManagementPolicy.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<ChildSiteManagementPolicy> client.ChildSiteManagementPolicies.CreateAsync(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `value` (object): Policy configuration data. Attributes differ by policy type. For more information, refer to the Value Hash section of the developer documentation.
* `skip_child_site_ids` (Nullable<Int64>[]): IDs of child sites excluded from this default policy.
* `child_site_ids` (Nullable<Int64>[]): IDs of child sites explicitly assigned to this non-default policy.
* `default_policy` (bool): Whether this policy applies to child sites not explicitly assigned to another policy.
* `policy_type` (string): Required - Type of policy.  Valid values: `settings`.
* `name` (string): Name for this policy.
* `description` (string): Description for this policy.


---

## Update Child Site Management Policy

```
Task<ChildSiteManagementPolicy> ChildSiteManagementPolicy.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<ChildSiteManagementPolicy> client.ChildSiteManagementPolicies.UpdateAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Child Site Management Policy ID.
* `value` (object): Policy configuration data. Attributes differ by policy type. For more information, refer to the Value Hash section of the developer documentation.
* `skip_child_site_ids` (Nullable<Int64>[]): IDs of child sites excluded from this default policy.
* `child_site_ids` (Nullable<Int64>[]): IDs of child sites explicitly assigned to this non-default policy.
* `default_policy` (bool): Whether this policy applies to child sites not explicitly assigned to another policy.
* `policy_type` (string): Type of policy.  Valid values: `settings`.
* `name` (string): Name for this policy.
* `description` (string): Description for this policy.


---

## Delete Child Site Management Policy

```
Task ChildSiteManagementPolicy.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task client.ChildSiteManagementPolicies.DeleteAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Child Site Management Policy ID.


---

## Update Child Site Management Policy

```
var childSiteManagementPolicy = await ChildSiteManagementPolicy.Find(1L);

var parameters = new Dictionary<string, object>();

parameters.Add("value", new Dictionary<string, object> { { "color2_left", "#000000" } });
parameters.Add("skip_child_site_ids", new Nullable<Int64>[] { 1, 2 });
parameters.Add("child_site_ids", new Nullable<Int64>[] { 1, 2 });
parameters.Add("default_policy", true);
parameters.Add("policy_type", "settings");
parameters.Add("name", "example");
parameters.Add("description", "example");

await childSiteManagementPolicy.Update(parameters);
```

`UpdateAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.
When it succeeds, the object takes the attributes Files.com returns. The `ChildSiteManagementPolicy` it returns is a separate object with those attributes.

### Parameters

* `id` (Nullable<Int64>): Required - Child Site Management Policy ID.
* `value` (object): Policy configuration data. Attributes differ by policy type. For more information, refer to the Value Hash section of the developer documentation.
* `skip_child_site_ids` (Nullable<Int64>[]): IDs of child sites excluded from this default policy.
* `child_site_ids` (Nullable<Int64>[]): IDs of child sites explicitly assigned to this non-default policy.
* `default_policy` (bool): Whether this policy applies to child sites not explicitly assigned to another policy.
* `policy_type` (string): Type of policy.  Valid values: `settings`.
* `name` (string): Name for this policy.
* `description` (string): Description for this policy.


---

## Delete Child Site Management Policy

```
var childSiteManagementPolicy = await ChildSiteManagementPolicy.Find(1L);

var parameters = new Dictionary<string, object>();

await childSiteManagementPolicy.Delete(parameters);
```

`DeleteAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.

### Parameters

* `id` (Nullable<Int64>): Required - Child Site Management Policy ID.


---

## Save

```
await childSiteManagementPolicy.SaveAsync(cancellationToken);
```

Creates the object on Files.com, or updates it if it has an ID. It sends the attributes set on the object, in its constructor or with its properties, or changed in place, such as an element of an array, since it was made or last came from Files.com, and only those the operation takes, each in the form its parameter takes. When it succeeds, the object takes the attributes Files.com returns. `Save()` does the same without a cancellation token.
