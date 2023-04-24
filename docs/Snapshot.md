# FilesCom.Models.Snapshot

* `id` / `Id`  (Nullable<Int64>): Snapshot ID.


---

## List Snapshots

```
Task<Snapshot> Snapshot.List(
    
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


---

## Delete Snapshot

```
Task<Snapshot> Snapshot.Delete(
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
var Snapshot = Snapshot.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


Snapshot.Update
```

### Parameters

* `id` (Nullable<Int64>): Required - Snapshot ID.


---

## Delete Snapshot

```
var Snapshot = Snapshot.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


Snapshot.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Snapshot ID.
