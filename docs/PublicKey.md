# FilesCom.Models.PublicKey

## Example PublicKey Object

```
{
  "id": 1,
  "title": "My public key",
  "created_at": "2000-01-01T01:00:00Z",
  "fingerprint": "43:51:43:a1:b5:fc:8b:b7:0a:3a:a9:b1:0f:66:73:a8",
  "fingerprint_sha256": "V5Q5t/ghT3R8Tol5GX9385bzmpygWVRnLuI9EXNrjCX",
  "last_login_at": "2000-01-01T01:00:00Z",
  "username": "User",
  "user_id": 1
}
```

* `id` / `Id`  (Nullable<Int64>): Public key ID
* `title` / `Title`  (string): Public key title
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Public key created at date/time
* `fingerprint` / `Fingerprint`  (string): Public key fingerprint (MD5)
* `fingerprint_sha256` / `FingerprintSha256`  (string): Public key fingerprint (SHA256)
* `last_login_at` / `LastLoginAt`  (Nullable<DateTime>): Key's most recent login time via SFTP
* `username` / `Username`  (string): Username of the user this public key is associated with
* `user_id` / `UserId`  (Nullable<Int64>): User ID this public key is associated with
* `public_key` / `PublicKeyType`  (string): Actual contents of SSH key.


---

## List Public Keys

```
Task<FilesList<PublicKey>> PublicKey.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `created_at`.
* `filter_gt` (object): If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at`.
* `filter_gteq` (object): If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at`.
* `filter_lt` (object): If set, return records where the specified field is less than the supplied value. Valid fields are `created_at`.
* `filter_lteq` (object): If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at`.


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
Task PublicKey.Delete(
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
var PublicKey = PublicKey.Find(1);

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
var PublicKey = PublicKey.Find(1);

var parameters = new Dictionary<string, object>();


PublicKey.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Public Key ID.
