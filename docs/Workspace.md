# FilesCom.Models.Workspace

## Example Workspace Object

```
{
  "id": 1,
  "name": "Project Alpha"
}
```

* `id` / `Id`  (Nullable<Int64>): Workspace ID
* `name` / `Name`  (string): Workspace name


---

## List Workspaces

```
Task<FilesList<Workspace>> Workspace.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `name`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `name`.
* `filter_prefix` (object): If set, return records where the specified field is prefixed by the supplied value. Valid fields are `name`.


---

## Show Workspace

```
Task<Workspace> Workspace.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Workspace ID.


---

## Create Workspace

```
Task<Workspace> Workspace.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `name` (string): Workspace name


---

## Update Workspace

```
Task<Workspace> Workspace.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Workspace ID.
* `name` (string): Workspace name


---

## Delete Workspace

```
Task Workspace.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Workspace ID.


---

## Update Workspace

```
var Workspace = Workspace.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("name", "Project Alpha");

Workspace.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Workspace ID.
* `name` (string): Workspace name


---

## Delete Workspace

```
var Workspace = Workspace.Find(1);

var parameters = new Dictionary<string, object>();


Workspace.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Workspace ID.
