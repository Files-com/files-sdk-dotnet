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
Task<FilesList<MetadataCategory>> MetadataCategory.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
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

### Parameters

* `id` (Nullable<Int64>): Required - Metadata Category ID.


---

## Create Metadata Category

```
Task<MetadataCategory> MetadataCategory.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
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

### Parameters

* `id` (Nullable<Int64>): Required - Metadata Category ID.


---

## Update Metadata Category

```
var MetadataCategory = MetadataCategory.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("name", "Approval Workflow");
parameters.Add("default_columns", ["Approval Status"]);

MetadataCategory.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Metadata Category ID.
* `name` (string): Name of the metadata category.
* `default_columns` (string[]): Metadata keys that should appear as columns in the UI by default.


---

## Delete Metadata Category

```
var MetadataCategory = MetadataCategory.Find(1);

var parameters = new Dictionary<string, object>();


MetadataCategory.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Metadata Category ID.
