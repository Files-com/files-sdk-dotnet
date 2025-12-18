# FilesCom.Models.KeyLifecycleRule

## Example KeyLifecycleRule Object

```
{
  "id": 1,
  "key_type": "gpg",
  "inactivity_days": 12,
  "name": "inactive gpg keys"
}
```

* `id` / `Id`  (Nullable<Int64>): Key Lifecycle Rule ID
* `key_type` / `KeyType`  (string): Key type for which the rule will apply (gpg or ssh).
* `inactivity_days` / `InactivityDays`  (Nullable<Int64>): Number of days of inactivity before the rule applies.
* `name` / `Name`  (string): Key Lifecycle Rule name


---

## List Key Lifecycle Rules

```
Task<FilesList<KeyLifecycleRule>> KeyLifecycleRule.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are .


---

## Show Key Lifecycle Rule

```
Task<KeyLifecycleRule> KeyLifecycleRule.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Key Lifecycle Rule ID.


---

## Create Key Lifecycle Rule

```
Task<KeyLifecycleRule> KeyLifecycleRule.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `key_type` (string): Key type for which the rule will apply (gpg or ssh).
* `inactivity_days` (Nullable<Int64>): Number of days of inactivity before the rule applies.
* `name` (string): Key Lifecycle Rule name


---

## Update Key Lifecycle Rule

```
Task<KeyLifecycleRule> KeyLifecycleRule.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Key Lifecycle Rule ID.
* `key_type` (string): Key type for which the rule will apply (gpg or ssh).
* `inactivity_days` (Nullable<Int64>): Number of days of inactivity before the rule applies.
* `name` (string): Key Lifecycle Rule name


---

## Delete Key Lifecycle Rule

```
Task KeyLifecycleRule.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Key Lifecycle Rule ID.


---

## Update Key Lifecycle Rule

```
var KeyLifecycleRule = KeyLifecycleRule.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("key_type", "gpg");
parameters.Add("inactivity_days", 12);
parameters.Add("name", "inactive gpg keys");

KeyLifecycleRule.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Key Lifecycle Rule ID.
* `key_type` (string): Key type for which the rule will apply (gpg or ssh).
* `inactivity_days` (Nullable<Int64>): Number of days of inactivity before the rule applies.
* `name` (string): Key Lifecycle Rule name


---

## Delete Key Lifecycle Rule

```
var KeyLifecycleRule = KeyLifecycleRule.Find(1);

var parameters = new Dictionary<string, object>();


KeyLifecycleRule.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Key Lifecycle Rule ID.
