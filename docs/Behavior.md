# FilesCom.Models.Behavior

## Example Behavior Object

```
{
  "id": 1,
  "path": "example",
  "attachment_url": "example",
  "behavior": "webhook",
  "name": "example",
  "description": "example",
  "value": {
    "key": "example value"
  },
  "disable_parent_folder_behavior": true
}
```

* `id` / `Id`  (Nullable<Int64>): Folder behavior ID
* `path` / `Path`  (string): Folder path.  Note that Behavior paths cannot be updated once initially set.  You will need to remove and re-create the behavior on the new path. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `attachment_url` / `AttachmentUrl`  (string): URL for attached file
* `behavior` / `BehaviorType`  (string): Behavior type.
* `name` / `Name`  (string): Name for this behavior.
* `description` / `Description`  (string): Description for this behavior.
* `value` / `Value`  (object): Settings for this behavior.  See the section above for an example value to provide here.  Formatting is different for each Behavior type.  May be sent as nested JSON or a single JSON-encoded string.  If using XML encoding for the API call, this data must be sent as a JSON-encoded string.
* `disable_parent_folder_behavior` / `DisableParentFolderBehavior`  (bool): If true, the parent folder's behavior will be disabled for this folder.
* `attachment_file` / `AttachmentFile`  (System.Net.Http.ByteArrayContent): Certain behaviors may require a file, for instance, the "watermark" behavior requires a watermark image
* `attachment_delete` / `AttachmentDelete`  (bool): If true, will delete the file stored in attachment


---

## List Behaviors

```
Task<FilesList<Behavior>> Behavior.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[behavior]=desc`). Valid fields are `behavior`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `behavior`.
* `filter_prefix` (object): If set, return records where the specified field is prefixed by the supplied value. Valid fields are `behavior`.


---

## Show Behavior

```
Task<Behavior> Behavior.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Behavior ID.


---

## List Behaviors by path

```
Task<FilesList<Behavior>> Behavior.ListFor(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[behavior]=desc`). Valid fields are `behavior`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `behavior`.
* `filter_prefix` (object): If set, return records where the specified field is prefixed by the supplied value. Valid fields are `behavior`.
* `path` (string): Required - Path to operate on.
* `recursive` (string): Show behaviors above this path?
* `behavior` (string): DEPRECATED: If set only shows folder behaviors matching this behavior type. Use `filter[behavior]` instead.


---

## Create Behavior

```
Task<Behavior> Behavior.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `value` (string): The value of the folder behavior.  Can be an integer, array, or hash depending on the type of folder behavior. See The Behavior Types section for example values for each type of behavior.
* `attachment_file` (System.Net.Http.ByteArrayContent): Certain behaviors may require a file, for instance, the "watermark" behavior requires a watermark image
* `name` (string): Name for this behavior.
* `description` (string): Description for this behavior.
* `path` (string): Required - Folder behaviors path.
* `behavior` (string): Required - Behavior type.


---

## Test webhook

```
Task Behavior.WebhookTest(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `url` (string): Required - URL for testing the webhook.
* `method` (string): HTTP method(GET or POST).
* `encoding` (string): HTTP encoding method.  Can be JSON, XML, or RAW (form data).
* `headers` (object): Additional request headers.
* `body` (object): Additional body parameters.
* `action` (string): action for test body


---

## Update Behavior

```
Task<Behavior> Behavior.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Behavior ID.
* `value` (string): The value of the folder behavior.  Can be an integer, array, or hash depending on the type of folder behavior. See The Behavior Types section for example values for each type of behavior.
* `attachment_file` (System.Net.Http.ByteArrayContent): Certain behaviors may require a file, for instance, the "watermark" behavior requires a watermark image
* `name` (string): Name for this behavior.
* `description` (string): Description for this behavior.
* `behavior` (string): Behavior type.
* `path` (string): Folder behaviors path.
* `attachment_delete` (bool): If true, will delete the file stored in attachment


---

## Delete Behavior

```
Task Behavior.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Behavior ID.


---

## Update Behavior

```
var Behavior = Behavior.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("value", "{\"method\": \"GET\"}");
parameters.Add("name", "example");
parameters.Add("description", "example");
parameters.Add("behavior", "webhook");
parameters.Add("path", "example");
parameters.Add("attachment_delete", true);

Behavior.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Behavior ID.
* `value` (string): The value of the folder behavior.  Can be an integer, array, or hash depending on the type of folder behavior. See The Behavior Types section for example values for each type of behavior.
* `attachment_file` (System.Net.Http.ByteArrayContent): Certain behaviors may require a file, for instance, the "watermark" behavior requires a watermark image
* `name` (string): Name for this behavior.
* `description` (string): Description for this behavior.
* `behavior` (string): Behavior type.
* `path` (string): Folder behaviors path.
* `attachment_delete` (bool): If true, will delete the file stored in attachment


---

## Delete Behavior

```
var Behavior = Behavior.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


Behavior.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Behavior ID.
