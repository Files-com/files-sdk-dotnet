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
    "header_name": "Authorization"
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
FilesList<Secret> Secret.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials:

```
FilesList<Secret> client.Secrets.List(
    
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

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<Secret> client.Secrets.FindAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
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

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<Secret> client.Secrets.CreateAsync(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
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

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<Secret> client.Secrets.UpdateAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
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

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task client.Secrets.DeleteAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Secret ID.


---

## Update Secret

```
var secret = await Secret.Find(1L);

var parameters = new Dictionary<string, object>();

parameters.Add("name", "Production API token");
parameters.Add("description", "Used by production API integrations.");
parameters.Add("secret_type", "token");
parameters.Add("metadata", new Dictionary<string, object> { { "header_name", "Authorization" } });

await secret.Update(parameters);
```

`UpdateAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.
When it succeeds, the object takes the attributes Files.com returns. The `Secret` it returns is a separate object with those attributes.

### Parameters

* `id` (Nullable<Int64>): Required - Secret ID.
* `name` (string): Secret name.
* `description` (string): Internal description for your reference.
* `secret_type` (string): Secret type.
* `metadata` (object): Non-secret metadata for the Secret type.


---

## Delete Secret

```
var secret = await Secret.Find(1L);

var parameters = new Dictionary<string, object>();

await secret.Delete(parameters);
```

`DeleteAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.

### Parameters

* `id` (Nullable<Int64>): Required - Secret ID.


---

## Save

```
await secret.SaveAsync(cancellationToken);
```

Creates the object on Files.com, or updates it if it has an ID. It sends the attributes set on the object, in its constructor or with its properties, or changed in place, such as an element of an array, since it was made or last came from Files.com, and only those the operation takes, each in the form its parameter takes. When it succeeds, the object takes the attributes Files.com returns. `Save()` does the same without a cancellation token.
