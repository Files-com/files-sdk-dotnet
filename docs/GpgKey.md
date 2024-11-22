# FilesCom.Models.GpgKey

## Example GpgKey Object

```
{
  "id": 1,
  "expires_at": "2000-01-01T01:00:00Z",
  "name": "key name",
  "user_id": 1,
  "public_key": "7f8bc1210b09b9ddf469e6b6b8920e76",
  "private_key": "ab236cfe4a195f0226bc2e674afdd6b0",
  "private_key_password": "[your GPG private key password]"
}
```

* `id` / `Id`  (Nullable<Int64>): Your GPG key ID.
* `expires_at` / `ExpiresAt`  (Nullable<DateTime>): Your GPG key expiration date.
* `name` / `Name`  (string): Your GPG key name.
* `user_id` / `UserId`  (Nullable<Int64>): GPG owner's user id
* `public_key` / `PublicKey`  (string): Your GPG public key
* `private_key` / `PrivateKey`  (string): Your GPG private key.
* `private_key_password` / `PrivateKeyPassword`  (string): Your GPG private key password. Only required for password protected keys.


---

## List GPG Keys

```
Task<FilesList<GpgKey>> GpgKey.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `name` and `expires_at`.


---

## Show GPG Key

```
Task<GpgKey> GpgKey.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Gpg Key ID.


---

## Create GPG Key

```
Task<GpgKey> GpgKey.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `public_key` (string): Your GPG public key
* `private_key` (string): Your GPG private key.
* `private_key_password` (string): Your GPG private key password. Only required for password protected keys.
* `name` (string): Required - Your GPG key name.


---

## Create an export CSV of GPG Key resources

```
Task<Export> GpgKey.CreateExport(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `name` and `expires_at`.


---

## Update GPG Key

```
Task<GpgKey> GpgKey.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Gpg Key ID.
* `public_key` (string): Your GPG public key
* `private_key` (string): Your GPG private key.
* `private_key_password` (string): Your GPG private key password. Only required for password protected keys.
* `name` (string): Your GPG key name.


---

## Delete GPG Key

```
Task GpgKey.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Gpg Key ID.


---

## Update GPG Key

```
var GpgKey = GpgKey.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("public_key", "7f8bc1210b09b9ddf469e6b6b8920e76");
parameters.Add("private_key", "ab236cfe4a195f0226bc2e674afdd6b0");
parameters.Add("private_key_password", "[your GPG private key password]");
parameters.Add("name", "key name");

GpgKey.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Gpg Key ID.
* `public_key` (string): Your GPG public key
* `private_key` (string): Your GPG private key.
* `private_key_password` (string): Your GPG private key password. Only required for password protected keys.
* `name` (string): Your GPG key name.


---

## Delete GPG Key

```
var GpgKey = GpgKey.Find(1);

var parameters = new Dictionary<string, object>();


GpgKey.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Gpg Key ID.
