# FilesCom.Models.Snapshot

## Example Snapshot Object

```
{
  "id": 1,
  "expires_at": "2000-01-01T01:00:00Z",
  "finalized_at": "2000-01-01T01:00:00Z",
  "name": "My Snapshot",
  "user_id": 1,
  "bundle_id": 1,
  "workspace_id": 1
}
```

* `id` / `Id`  (Nullable<Int64>): The snapshot's unique ID.
* `expires_at` / `ExpiresAt`  (Nullable<DateTime>): When the snapshot expires.
* `finalized_at` / `FinalizedAt`  (Nullable<DateTime>): When the snapshot was finalized.
* `name` / `Name`  (string): A name for the snapshot.
* `user_id` / `UserId`  (Nullable<Int64>): The user that created this snapshot, if applicable.
* `bundle_id` / `BundleId`  (Nullable<Int64>): The bundle using this snapshot, if applicable.
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace ID. `0` means the default workspace.
* `paths` / `Paths`  (string[]): An array of paths to add to the snapshot.


---

## List Snapshots

```
FilesList<Snapshot> Snapshot.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials:

```
FilesList<Snapshot> client.Snapshots.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).


---

## Show Snapshot

```
Task<Snapshot> Snapshot.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<Snapshot> client.Snapshots.FindAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Snapshot ID.


---

## Create Snapshot

```
Task<Snapshot> Snapshot.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<Snapshot> client.Snapshots.CreateAsync(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `expires_at` (string): When the snapshot expires.
* `name` (string): A name for the snapshot.
* `paths` (string[]): An array of paths to add to the snapshot.
* `workspace_id` (Nullable<Int64>): Workspace ID. `0` means the default workspace.


---

## Finalize Snapshot

```
Task Snapshot.Finalize(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task client.Snapshots.FinalizeAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Snapshot ID.


---

## Update Snapshot

```
Task<Snapshot> Snapshot.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<Snapshot> client.Snapshots.UpdateAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Snapshot ID.
* `expires_at` (string): When the snapshot expires.
* `name` (string): A name for the snapshot.
* `paths` (string[]): An array of paths to add to the snapshot.


---

## Delete Snapshot

```
Task Snapshot.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task client.Snapshots.DeleteAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Snapshot ID.


---

## Finalize Snapshot

```
var snapshot = await Snapshot.Find(1L);

var parameters = new Dictionary<string, object>();

await snapshot.Finalize(parameters);
```

`FinalizeAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.

### Parameters

* `id` (Nullable<Int64>): Required - Snapshot ID.


---

## Update Snapshot

```
var snapshot = await Snapshot.Find(1L);

var parameters = new Dictionary<string, object>();

parameters.Add("expires_at", "2000-01-01T01:00:00Z");
parameters.Add("name", "My Snapshot");

await snapshot.Update(parameters);
```

`UpdateAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.
When it succeeds, the object takes the attributes Files.com returns. The `Snapshot` it returns is a separate object with those attributes.

### Parameters

* `id` (Nullable<Int64>): Required - Snapshot ID.
* `expires_at` (string): When the snapshot expires.
* `name` (string): A name for the snapshot.
* `paths` (string[]): An array of paths to add to the snapshot.


---

## Delete Snapshot

```
var snapshot = await Snapshot.Find(1L);

var parameters = new Dictionary<string, object>();

await snapshot.Delete(parameters);
```

`DeleteAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.

### Parameters

* `id` (Nullable<Int64>): Required - Snapshot ID.


---

## Save

```
await snapshot.SaveAsync(cancellationToken);
```

Creates the object on Files.com, or updates it if it has an ID. It sends the attributes set on the object, in its constructor or with its properties, or changed in place, such as an element of an array, since it was made or last came from Files.com, and only those the operation takes, each in the form its parameter takes. When it succeeds, the object takes the attributes Files.com returns. `Save()` does the same without a cancellation token.
