# FilesCom.Models.PublicKey

## Example PublicKey Object

```
{
  "id": 1,
  "title": "My public key",
  "created_at": "2000-01-01T01:00:00Z",
  "fingerprint": "43:51:43:a1:b5:fc:8b:b7:0a:3a:a9:b1:0f:66:73:a8"
}
```

* `id` / `Id`  (Nullable<Int64>): Public key ID
* `title` / `Title`  (string): Public key title
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Public key created at date/time
* `fingerprint` / `Fingerprint`  (string): Public key fingerprint
* `user_id` / `UserId`  (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `public_key` / `PublicKeyType`  (string): Actual contents of SSH key.


---

## List Public Keys

```
Task<PublicKey[]> PublicKey.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via either the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).


---

## Show Public Key

```
Task<PublicKey> PublicKey.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Public Key ID.


---

## Create Public Key

```
Task<PublicKey> PublicKey.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `title` (string): Required - Internal reference for key.
* `public_key` (string): Required - Actual contents of SSH key.


---

## Update Public Key

```
Task<PublicKey> PublicKey.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Public Key ID.
* `title` (string): Required - Internal reference for key.


---

## Delete Public Key

```
Task<PublicKey> PublicKey.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Public Key ID.


---

## Update Public Key

```
var PublicKey = PublicKey.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("title", "My Main Key");

PublicKey.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Public Key ID.
* `title` (string): Required - Internal reference for key.


---

## Delete Public Key

```
var PublicKey = PublicKey.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


PublicKey.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Public Key ID.
