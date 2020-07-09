# Files.Models.Clickwrap

## Example Clickwrap Object

```
{
  "name": "Example Site NDA for Files.com Use",
  "body": "[Legal body text]",
  "use_with_users": "",
  "use_with_bundles": "",
  "use_with_inboxes": ""
}
```

* `name` / `Name`  (string): Name of the Clickwrap agreement (used when selecting from multiple Clickwrap agreements.)
* `body` / `Body`  (string): Body text of Clickwrap (supports Markdown formatting).
* `use_with_users` / `UseWithUsers`  (string): Use this Clickwrap for User Registrations?  Note: This only applies to User Registrations where the User is invited to your Files.com site using an E-Mail invitation process where they then set their own password.
* `use_with_bundles` / `UseWithBundles`  (string): Use this Clickwrap for Bundles?
* `use_with_inboxes` / `UseWithInboxes`  (string): Use this Clickwrap for Inboxes?
* `id` / `Id`  (Nullable<Int64>): Clickwrap ID.


---

## List Clickwraps

```
Task<Clickwrap[]> Clickwrap.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `page` (Nullable<Int64>): Current page number.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): Deprecated: If set to `count` returns a count of matching records rather than the records themselves.


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
Task<Clickwrap> Clickwrap.Delete(
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
var Clickwrap = Clickwrap.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("name", "Example Site NDA for Files.com Use");
parameters.Add("body", "[Legal body text]");

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
var Clickwrap = Clickwrap.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


Clickwrap.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Clickwrap ID.
