# FilesCom.Models.Clickwrap

## Example Clickwrap Object

```
{
  "id": 1,
  "name": "Example Site NDA for Files.com Use",
  "body": "[Legal body text]",
  "use_with_users": "example",
  "use_with_bundles": "example",
  "use_with_inboxes": "example"
}
```

* `id` / `Id`  (Nullable<Int64>): Clickwrap ID
* `name` / `Name`  (string): Name of the Clickwrap agreement (used when selecting from multiple Clickwrap agreements.)
* `body` / `Body`  (string): Body text of Clickwrap (supports Markdown formatting).
* `use_with_users` / `UseWithUsers`  (string): Use this Clickwrap for User Registrations?  Note: This only applies to User Registrations where the User is invited to your Files.com site using an E-Mail invitation process where they then set their own password.
* `use_with_bundles` / `UseWithBundles`  (string): Use this Clickwrap for Bundles?
* `use_with_inboxes` / `UseWithInboxes`  (string): Use this Clickwrap for Inboxes?


---

## List Clickwraps

```
Task<FilesList<Clickwrap>> Clickwrap.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are .


---

## Show Clickwrap

```
Task<Clickwrap> Clickwrap.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Clickwrap ID.


---

## Create Clickwrap

```
Task<Clickwrap> Clickwrap.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `name` (string): Name of the Clickwrap agreement (used when selecting from multiple Clickwrap agreements.)
* `body` (string): Body text of Clickwrap (supports Markdown formatting).
* `use_with_bundles` (string): Use this Clickwrap for Bundles?
* `use_with_inboxes` (string): Use this Clickwrap for Inboxes?
* `use_with_users` (string): Use this Clickwrap for User Registrations?  Note: This only applies to User Registrations where the User is invited to your Files.com site using an E-Mail invitation process where they then set their own password.


---

## Update Clickwrap

```
Task<Clickwrap> Clickwrap.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Clickwrap ID.
* `name` (string): Name of the Clickwrap agreement (used when selecting from multiple Clickwrap agreements.)
* `body` (string): Body text of Clickwrap (supports Markdown formatting).
* `use_with_bundles` (string): Use this Clickwrap for Bundles?
* `use_with_inboxes` (string): Use this Clickwrap for Inboxes?
* `use_with_users` (string): Use this Clickwrap for User Registrations?  Note: This only applies to User Registrations where the User is invited to your Files.com site using an E-Mail invitation process where they then set their own password.


---

## Delete Clickwrap

```
Task Clickwrap.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Clickwrap ID.


---

## Update Clickwrap

```
var Clickwrap = Clickwrap.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("name", "Example Site NDA for Files.com Use");
parameters.Add("body", "[Legal body text]");
parameters.Add("use_with_bundles", "example");
parameters.Add("use_with_inboxes", "example");
parameters.Add("use_with_users", "example");

Clickwrap.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Clickwrap ID.
* `name` (string): Name of the Clickwrap agreement (used when selecting from multiple Clickwrap agreements.)
* `body` (string): Body text of Clickwrap (supports Markdown formatting).
* `use_with_bundles` (string): Use this Clickwrap for Bundles?
* `use_with_inboxes` (string): Use this Clickwrap for Inboxes?
* `use_with_users` (string): Use this Clickwrap for User Registrations?  Note: This only applies to User Registrations where the User is invited to your Files.com site using an E-Mail invitation process where they then set their own password.


---

## Delete Clickwrap

```
var Clickwrap = Clickwrap.Find(1);

var parameters = new Dictionary<string, object>();


Clickwrap.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Clickwrap ID.
