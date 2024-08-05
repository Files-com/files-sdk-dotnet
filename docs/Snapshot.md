# FilesCom.Models.Snapshot

## Example Snapshot Object

```
{
  "id": 1,
  "expires_at": "2000-01-01T01:00:00Z",
  "finalized_at": "2000-01-01T01:00:00Z",
  "name": "My Snapshot",
  "user_id": 1,
  "bundle_id": 1
}
```

* `id` / `Id`  (Nullable<Int64>): The snapshot's unique ID.
* `expires_at` / `ExpiresAt`  (Nullable<DateTime>): When the snapshot expires.
* `finalized_at` / `FinalizedAt`  (Nullable<DateTime>): When the snapshot was finalized.
* `name` / `Name`  (string): A name for the snapshot.
* `user_id` / `UserId`  (Nullable<Int64>): The user that created this snapshot, if applicable.
* `bundle_id` / `BundleId`  (Nullable<Int64>): The bundle using this snapshot, if applicable.
* `paths` / `Paths`  (string[]): An array of paths to add to the snapshot.


---

## List Snapshots

```
Task<FilesList<Snapshot>> Snapshot.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).


---

## Show Snapshot

```
Task<Snapshot> Snapshot.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
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

### Parameters

* `expires_at` (string): When the snapshot expires.
* `name` (string): A name for the snapshot.
* `paths` (string[]): An array of paths to add to the snapshot.


---

## Finalize Snapshot

```
Task Snapshot.Finalize(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
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

### Parameters

* `id` (Nullable<Int64>): Required - Snapshot ID.


---

## Finalize Snapshot

```
var Snapshot = Snapshot.Find(1);

var parameters = new Dictionary<string, object>();


Snapshot.Finalize
```

### Parameters

* `id` (Nullable<Int64>): Required - Snapshot ID.


---

## Update Snapshot

```
var Snapshot = Snapshot.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("expires_at", "2000-01-01T01:00:00Z");
parameters.Add("name", "My Snapshot");

Snapshot.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Snapshot ID.
* `expires_at` (string): When the snapshot expires.
* `name` (string): A name for the snapshot.
* `paths` (string[]): An array of paths to add to the snapshot.


---

## Delete Snapshot

```
var Snapshot = Snapshot.Find(1);

var parameters = new Dictionary<string, object>();


Snapshot.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Snapshot ID.
