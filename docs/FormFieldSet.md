# FilesCom.Models.FormFieldSet

## Example FormFieldSet Object

```
{
  "id": 1,
  "title": "Sample Form Title",
  "require_complete_metadata_capture": false,
  "form_layout": [
    1,
    2,
    3,
    4
  ],
  "form_fields": [
    {
      "id": 1,
      "label": "Sample Label",
      "required": true,
      "help_text": "Help Text",
      "field_type": "text",
      "options_for_select": [
        "red",
        "green",
        "blue"
      ],
      "default_option": "red",
      "form_field_set_id": 1
    }
  ],
  "skip_name": true,
  "skip_email": true,
  "skip_company": true,
  "in_use": true
}
```

* `id` / `Id`  (Nullable<Int64>): Form field set id
* `title` / `Title`  (string): Title to be displayed
* `require_complete_metadata_capture` / `RequireCompleteMetadataCapture`  (bool): Reject answers over 1024 characters or more than 32 answered custom fields to ensure complete file metadata capture
* `form_layout` / `FormLayout`  (Nullable<Int64>[]): Layout of the form
* `form_fields` / `FormFields`  (FormField[]): Associated form field definitions; authenticated form field set responses include historical definitions, while form_layout identifies current fields
* `skip_name` / `SkipName`  (bool): Any associated InboxRegistrations or BundleRegistrations can be saved without providing name
* `skip_email` / `SkipEmail`  (bool): Any associated InboxRegistrations or BundleRegistrations can be saved without providing email
* `skip_company` / `SkipCompany`  (bool): Any associated InboxRegistrations or BundleRegistrations can be saved without providing company
* `in_use` / `InUse`  (bool): Form Field Set is in use by an active Inbox / Bundle / Inbox Registration / Bundle Registration
* `user_id` / `UserId`  (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace ID


---

## List Form Field Sets

```
FilesList<FormFieldSet> FormFieldSet.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials:

```
FilesList<FormFieldSet> client.FormFieldSets.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).


---

## Show Form Field Set

```
Task<FormFieldSet> FormFieldSet.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<FormFieldSet> client.FormFieldSets.FindAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Form Field Set ID.


---

## Create Form Field Set

```
Task<FormFieldSet> FormFieldSet.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<FormFieldSet> client.FormFieldSets.CreateAsync(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `title` (string): Title to be displayed
* `require_complete_metadata_capture` (bool): Reject answers over 1024 characters or more than 32 answered custom fields to ensure complete file metadata capture
* `workspace_id` (Nullable<Int64>): Workspace ID
* `skip_email` (bool): Skip validating form email
* `skip_name` (bool): Skip validating form name
* `skip_company` (bool): Skip validating company
* `form_fields` (object[]): 


---

## Update Form Field Set

```
Task<FormFieldSet> FormFieldSet.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<FormFieldSet> client.FormFieldSets.UpdateAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Form Field Set ID.
* `title` (string): Title to be displayed
* `require_complete_metadata_capture` (bool): Reject answers over 1024 characters or more than 32 answered custom fields to ensure complete file metadata capture
* `workspace_id` (Nullable<Int64>): Workspace ID
* `skip_email` (bool): Skip validating form email
* `skip_name` (bool): Skip validating form name
* `skip_company` (bool): Skip validating company
* `form_fields` (object[]): 


---

## Delete Form Field Set

```
Task FormFieldSet.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task client.FormFieldSets.DeleteAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Form Field Set ID.


---

## Update Form Field Set

```
var formFieldSet = await FormFieldSet.Find(1L);

var parameters = new Dictionary<string, object>();

parameters.Add("title", "Sample Form Title");
parameters.Add("require_complete_metadata_capture", false);
parameters.Add("workspace_id", 1L);
parameters.Add("skip_email", false);
parameters.Add("skip_name", false);
parameters.Add("skip_company", false);
parameters.Add("form_fields", new object[] { new Dictionary<string, object> { { "id", 1 }, { "label", "Sample Label" }, { "required", true }, { "help_text", "Help Text" }, { "field_type", "text" }, { "options_for_select", new object[] { "red", "green", "blue" } }, { "default_option", "red" }, { "form_field_set_id", 1 } } });

await formFieldSet.Update(parameters);
```

`UpdateAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.
When it succeeds, the object takes the attributes Files.com returns. The `FormFieldSet` it returns is a separate object with those attributes.

### Parameters

* `id` (Nullable<Int64>): Required - Form Field Set ID.
* `title` (string): Title to be displayed
* `require_complete_metadata_capture` (bool): Reject answers over 1024 characters or more than 32 answered custom fields to ensure complete file metadata capture
* `workspace_id` (Nullable<Int64>): Workspace ID
* `skip_email` (bool): Skip validating form email
* `skip_name` (bool): Skip validating form name
* `skip_company` (bool): Skip validating company
* `form_fields` (object[]): 


---

## Delete Form Field Set

```
var formFieldSet = await FormFieldSet.Find(1L);

var parameters = new Dictionary<string, object>();

await formFieldSet.Delete(parameters);
```

`DeleteAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.

### Parameters

* `id` (Nullable<Int64>): Required - Form Field Set ID.


---

## Save

```
await formFieldSet.SaveAsync(cancellationToken);
```

Creates the object on Files.com, or updates it if it has an ID. It sends the attributes set on the object, in its constructor or with its properties, or changed in place, such as an element of an array, since it was made or last came from Files.com, and only those the operation takes, each in the form its parameter takes. When it succeeds, the object takes the attributes Files.com returns. `Save()` does the same without a cancellation token.
