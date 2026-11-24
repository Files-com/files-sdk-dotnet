# FilesCom.Models.DatabaseDestination

## Example DatabaseDestination Object

```
{
  "id": 1,
  "workspace_id": 1,
  "name": "Orders",
  "provider": "snowflake",
  "config": "example",
  "secret_id": 1,
  "created_at": "2000-01-01T01:00:00Z",
  "updated_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): Database Destination ID.
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace ID. 0 means the default workspace.
* `name` / `Name`  (string): Name of the destination.
* `provider` / `Provider`  (string): Database provider.
* `config` / `Config`  (object): Provider settings for the destination. Use the schema matching provider. The connection address and table are fixed after creation.
* `secret_id` / `SecretId`  (Nullable<Int64>): ID of a Secret in the same Workspace. Use a Token Secret for Snowflake, a service account Key/Value Secret for BigQuery, or a Username/Password Secret for a database login.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): When the destination was created.
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): When the destination was last updated.


---

## List Database Destinations

```
FilesList<DatabaseDestination> DatabaseDestination.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials:

```
FilesList<DatabaseDestination> client.DatabaseDestinations.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id` and `name`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id` and `name`. Valid field combinations are `[ workspace_id, name ]`.
* `filter_prefix` (object): If set, return records where the specified field is prefixed by the supplied value. Valid fields are `name`.


---

## Show Database Destination

```
Task<DatabaseDestination> DatabaseDestination.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<DatabaseDestination> client.DatabaseDestinations.FindAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Database Destination ID.


---

## Create Database Destination

```
Task<DatabaseDestination> DatabaseDestination.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<DatabaseDestination> client.DatabaseDestinations.CreateAsync(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `name` (string): Required - Name of the destination.
* `provider` (string): Required - Database provider.
* `config` (object): Required - Provider settings for the destination. Use the schema matching provider. The connection address and table are fixed after creation.
* `secret_id` (Nullable<Int64>): Required - ID of a Secret in the same Workspace. Use a Token Secret for Snowflake, a service account Key/Value Secret for BigQuery, or a Username/Password Secret for a database login.
* `workspace_id` (Nullable<Int64>): Workspace ID. 0 means the default workspace.


---

## Update Database Destination

```
Task<DatabaseDestination> DatabaseDestination.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<DatabaseDestination> client.DatabaseDestinations.UpdateAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Database Destination ID.
* `name` (string): Name of the destination.
* `secret_id` (Nullable<Int64>): ID of a Secret in the same Workspace. Use a Token Secret for Snowflake, a service account Key/Value Secret for BigQuery, or a Username/Password Secret for a database login.
* `config` (object): Provider settings for the destination. Use the schema matching provider. The connection address and table are fixed after creation.


---

## Delete Database Destination

```
Task DatabaseDestination.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task client.DatabaseDestinations.DeleteAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Database Destination ID.


---

## Update Database Destination

```
var databaseDestination = await DatabaseDestination.Find(1L);

var parameters = new Dictionary<string, object>();

parameters.Add("name", "Orders");
parameters.Add("secret_id", 1L);
parameters.Add("config", new Dictionary<string, object> { { "account_identifier", "value" }, { "database_name", "value" }, { "schema_name", "value" }, { "table_name", "value" } });

await databaseDestination.Update(parameters);
```

`UpdateAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.
When it succeeds, the object takes the attributes Files.com returns. The `DatabaseDestination` it returns is a separate object with those attributes.

### Parameters

* `id` (Nullable<Int64>): Required - Database Destination ID.
* `name` (string): Name of the destination.
* `secret_id` (Nullable<Int64>): ID of a Secret in the same Workspace. Use a Token Secret for Snowflake, a service account Key/Value Secret for BigQuery, or a Username/Password Secret for a database login.
* `config` (object): Provider settings for the destination. Use the schema matching provider. The connection address and table are fixed after creation.


---

## Delete Database Destination

```
var databaseDestination = await DatabaseDestination.Find(1L);

var parameters = new Dictionary<string, object>();

await databaseDestination.Delete(parameters);
```

`DeleteAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.

### Parameters

* `id` (Nullable<Int64>): Required - Database Destination ID.


---

## Save

```
await databaseDestination.SaveAsync(cancellationToken);
```

Creates the object on Files.com, or updates it if it has an ID. It sends the attributes set on the object, in its constructor or with its properties, or changed in place, such as an element of an array, since it was made or last came from Files.com, and only those the operation takes, each in the form its parameter takes. When it succeeds, the object takes the attributes Files.com returns. `Save()` does the same without a cancellation token.
