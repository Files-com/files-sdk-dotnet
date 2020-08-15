# Files.Models.Project

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

* `page` (Nullable<Int64>): Current page number.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
* `cursor` (string): Send cursor to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.


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
Task<Project> Project.Delete(
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
