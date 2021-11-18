# FilesCom.Models.As2Key

## Example As2Key Object

```
{
  "id": 1,
  "as2_partnership_name": "Test",
  "created_at": "2000-01-01T01:00:00Z",
  "fingerprint": "cf:cb:d3:26:52:6c:55:88:83:17:13:cf:e7:70:eb:1b:32:37:38:c0"
}
```

* `id` / `Id`  (Nullable<Int64>): AS2 Key ID
* `as2_partnership_name` / `As2PartnershipName`  (string): AS2 Partnership Name
* `created_at` / `CreatedAt`  (Nullable<DateTime>): AS2 Key created at date/time
* `fingerprint` / `Fingerprint`  (string): Public key fingerprint
* `user_id` / `UserId`  (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `public_key` / `PublicKey`  (string): Actual contents of Public key.


---

## List As2 Keys

```
Task<As2Key[]> As2Key.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via either the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).


---

## Show As2 Key

```
Task<As2Key> As2Key.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - As2 Key ID.


---

## Create As2 Key

```
Task<As2Key> As2Key.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `as2_partnership_name` (string): Required - AS2 Partnership Name
* `public_key` (string): Required - Actual contents of Public key.


---

## Update As2 Key

```
Task<As2Key> As2Key.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - As2 Key ID.
* `as2_partnership_name` (string): Required - AS2 Partnership Name


---

## Delete As2 Key

```
Task<As2Key> As2Key.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - As2 Key ID.


---

## Update As2 Key

```
var As2Key = As2Key.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("as2_partnership_name", "Test");

As2Key.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - As2 Key ID.
* `as2_partnership_name` (string): Required - AS2 Partnership Name


---

## Delete As2 Key

```
var As2Key = As2Key.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


As2Key.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - As2 Key ID.
