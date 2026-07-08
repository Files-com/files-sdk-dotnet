# FilesCom.Models.Secret

## Example Secret Object

```
{
  "id": 1,
  "workspace_id": 1,
  "name": "Production API token",
  "description": "Used by production API integrations.",
  "secret_type": "token",
  "metadata": {
    "key": "example value"
  },
  "value_field_names": [
    "example"
  ],
  "created_at": "2000-01-01T01:00:00Z",
  "updated_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): Secret ID.
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace ID. 0 means the default workspace.
* `name` / `Name`  (string): Secret name.
* `description` / `Description`  (string): Internal description for your reference.
* `secret_type` / `SecretType`  (string): Secret type.
* `metadata` / `Metadata`  (object): Non-secret metadata for the Secret type.
* `value_field_names` / `ValueFieldNames`  (string[]): Names of configured secret value fields. Secret values are never returned.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Secret create date/time.
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): Secret update date/time.


---

## List Secrets

```
Task<FilesList<Secret>> Secret.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `name` or `secret_type`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id`, `name` or `secret_type`. Valid field combinations are `[ workspace_id, name ]`, `[ workspace_id, secret_type ]`, `[ secret_type, name ]` or `[ workspace_id, secret_type, name ]`.
* `filter_prefix` (object): If set, return records where the specified field is prefixed by the supplied value. Valid fields are `name`.


---

## Show Secret

```
Task<Secret> Secret.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Secret ID.


---

## Create Secret

```
Task<Secret> Secret.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `name` (string): Required - Secret name.
* `description` (string): Internal description for your reference.
* `secret_type` (string): Required - Secret type.
* `metadata` (object): Non-secret metadata for the Secret type.
* `workspace_id` (Nullable<Int64>): Workspace ID. 0 means the default workspace.


---

## Update Secret

```
Task<Secret> Secret.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Secret ID.
* `name` (string): Secret name.
* `description` (string): Internal description for your reference.
* `secret_type` (string): Secret type.
* `metadata` (object): Non-secret metadata for the Secret type.


---

## Delete Secret

```
Task Secret.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Secret ID.


---

## Update Secret

```
var Secret = Secret.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("name", "Production API token");
parameters.Add("description", "Used by production API integrations.");
parameters.Add("secret_type", "token");
parameters.Add("metadata", {"key":"example value"});

Secret.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Secret ID.
* `name` (string): Secret name.
* `description` (string): Internal description for your reference.
* `secret_type` (string): Secret type.
* `metadata` (object): Non-secret metadata for the Secret type.


---

## Delete Secret

```
var Secret = Secret.Find(1);

var parameters = new Dictionary<string, object>();


Secret.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Secret ID.
