# FilesCom.Models.Project

## Example Project Object

```
{
  "id": 1,
  "global_access": "none"
}
```

* `id` / `Id`  (Nullable<Int64>): Project ID
* `global_access` / `GlobalAccess`  (string): Global access settings


---

## List Projects

```
Task<Project[]> Project.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).


---

## Show Project

```
Task<Project> Project.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Project ID.


---

## Create Project

```
Task<Project> Project.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `global_access` (string): Required - Global permissions.  Can be: `none`, `anyone_with_read`, `anyone_with_full`.


---

## Update Project

```
Task<Project> Project.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Project ID.
* `global_access` (string): Required - Global permissions.  Can be: `none`, `anyone_with_read`, `anyone_with_full`.


---

## Delete Project

```
Task Project.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Project ID.


---

## Update Project

```
var Project = Project.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("global_access", "global_access");

Project.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Project ID.
* `global_access` (string): Required - Global permissions.  Can be: `none`, `anyone_with_read`, `anyone_with_full`.


---

## Delete Project

```
var Project = Project.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


Project.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Project ID.
