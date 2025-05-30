# FilesCom.Models.FormFieldSet

## Example FormFieldSet Object

```
{
  "id": 1,
  "title": "Sample Form Title",
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
* `form_layout` / `FormLayout`  (Nullable<Int64>[]): Layout of the form
* `form_fields` / `FormFields`  (object[]): Associated form fields
* `skip_name` / `SkipName`  (bool): Any associated InboxRegistrations or BundleRegistrations can be saved without providing name
* `skip_email` / `SkipEmail`  (bool): Any associated InboxRegistrations or BundleRegistrations can be saved without providing email
* `skip_company` / `SkipCompany`  (bool): Any associated InboxRegistrations or BundleRegistrations can be saved without providing company
* `in_use` / `InUse`  (bool): Form Field Set is in use by an active Inbox / Bundle / Inbox Registration / Bundle Registration
* `user_id` / `UserId`  (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.


---

## List Form Field Sets

```
Task<FilesList<FormFieldSet>> FormFieldSet.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).


---

## Show Form Field Set

```
Task<FormFieldSet> FormFieldSet.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
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

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `title` (string): Title to be displayed
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

### Parameters

* `id` (Nullable<Int64>): Required - Form Field Set ID.
* `title` (string): Title to be displayed
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

### Parameters

* `id` (Nullable<Int64>): Required - Form Field Set ID.


---

## Update Form Field Set

```
var FormFieldSet = FormFieldSet.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("title", "Sample Form Title");
parameters.Add("skip_email", false);
parameters.Add("skip_name", false);
parameters.Add("skip_company", false);
parameters.Add("form_fields", [{"id":1,"label":"Sample Label","required":true,"help_text":"Help Text","field_type":"text","options_for_select":["red","green","blue"],"default_option":"red","form_field_set_id":1}]);

FormFieldSet.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Form Field Set ID.
* `title` (string): Title to be displayed
* `skip_email` (bool): Skip validating form email
* `skip_name` (bool): Skip validating form name
* `skip_company` (bool): Skip validating company
* `form_fields` (object[]): 


---

## Delete Form Field Set

```
var FormFieldSet = FormFieldSet.Find(1);

var parameters = new Dictionary<string, object>();


FormFieldSet.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Form Field Set ID.
