# FilesCom.Models.MetadataCategory

## Example MetadataCategory Object

```
{
  "id": 1,
  "name": "Approval Workflow",
  "definitions": {
    "Approval Status": [
      "Under Review",
      "Approved",
      "Rejected"
    ],
    "Reviewer": [

    ]
  },
  "default_columns": [
    "Approval Status"
  ]
}
```

* `id` / `Id`  (Nullable<Int64>): Metadata Category ID
* `name` / `Name`  (string): Name of the metadata category.
* `definitions` / `Definitions`  (Dictionary<string, string[]>): Map of key names to arrays of allowed values. An empty array means free-form text.
* `default_columns` / `DefaultColumns`  (string[]): Metadata keys that should appear as columns in the UI by default.


---

## List Metadata Categories

```
FilesList<MetadataCategory> MetadataCategory.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials:

```
FilesList<MetadataCategory> client.MetadataCategories.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are .


---

## Show Metadata Category

```
Task<MetadataCategory> MetadataCategory.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<MetadataCategory> client.MetadataCategories.FindAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Metadata Category ID.


---

## List Metadata Categories by Path

```
FilesList<MetadataCategory> MetadataCategory.ListFor(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials:

```
FilesList<MetadataCategory> client.MetadataCategories.ListFor(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `path` (string): Required - Path to operate on.


---

## Create Metadata Category

```
Task<MetadataCategory> MetadataCategory.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<MetadataCategory> client.MetadataCategories.CreateAsync(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `name` (string): Required - Name of the metadata category.
* `default_columns` (string[]): Metadata keys that should appear as columns in the UI by default.


---

## Update Metadata Category

```
Task<MetadataCategory> MetadataCategory.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<MetadataCategory> client.MetadataCategories.UpdateAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Metadata Category ID.
* `name` (string): Name of the metadata category.
* `default_columns` (string[]): Metadata keys that should appear as columns in the UI by default.


---

## Delete Metadata Category

```
Task MetadataCategory.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task client.MetadataCategories.DeleteAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Metadata Category ID.


---

## Update Metadata Category

```
var metadataCategory = await MetadataCategory.Find(1L);

var parameters = new Dictionary<string, object>();

parameters.Add("name", "Approval Workflow");
parameters.Add("default_columns", new string[] { "Approval Status" });

await metadataCategory.Update(parameters);
```

`UpdateAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.
When it succeeds, the object takes the attributes Files.com returns. The `MetadataCategory` it returns is a separate object with those attributes.

### Parameters

* `id` (Nullable<Int64>): Required - Metadata Category ID.
* `name` (string): Name of the metadata category.
* `default_columns` (string[]): Metadata keys that should appear as columns in the UI by default.


---

## Delete Metadata Category

```
var metadataCategory = await MetadataCategory.Find(1L);

var parameters = new Dictionary<string, object>();

await metadataCategory.Delete(parameters);
```

`DeleteAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.

### Parameters

* `id` (Nullable<Int64>): Required - Metadata Category ID.


---

## Save

```
await metadataCategory.SaveAsync(cancellationToken);
```

Creates the object on Files.com, or updates it if it has an ID. It sends the attributes set on the object, in its constructor or with its properties, or changed in place, such as an element of an array, since it was made or last came from Files.com, and only those the operation takes, each in the form its parameter takes. When it succeeds, the object takes the attributes Files.com returns. `Save()` does the same without a cancellation token.
