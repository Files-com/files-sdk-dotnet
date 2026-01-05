# FilesCom.Models.GpgKey

## Example GpgKey Object

```
{
  "id": 1,
  "workspace_id": 1,
  "expires_at": "2000-01-01T01:00:00Z",
  "name": "key name",
  "partner_id": 1,
  "partner_name": "example",
  "user_id": 1,
  "public_key_md5": "7f8bc1210b09b9ddf469e6b6b8920e76",
  "private_key_md5": "ab236cfe4a195f0226bc2e674afdd6b0",
  "generated_public_key": "7f8bc1210b09b9ddf469e6b6b8920e76",
  "generated_private_key": "ab236cfe4a195f0226bc2e674afdd6b0",
  "private_key_password_md5": "[the GPG private key password]"
}
```

* `id` / `Id`  (Nullable<Int64>): GPG key ID.
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace ID (0 for default workspace).
* `expires_at` / `ExpiresAt`  (Nullable<DateTime>): GPG key expiration date.
* `name` / `Name`  (string): GPG key name.
* `partner_id` / `PartnerId`  (Nullable<Int64>): Partner ID who owns this GPG Key, if applicable.
* `partner_name` / `PartnerName`  (string): Name of the Partner who owns this GPG Key, if applicable.
* `user_id` / `UserId`  (Nullable<Int64>): User ID who owns this GPG Key, if applicable.
* `public_key_md5` / `PublicKeyMd5`  (string): MD5 hash of the GPG public key
* `private_key_md5` / `PrivateKeyMd5`  (string): MD5 hash of the GPG private key.
* `generated_public_key` / `GeneratedPublicKey`  (string): GPG public key
* `generated_private_key` / `GeneratedPrivateKey`  (string): GPG private key.
* `private_key_password_md5` / `PrivateKeyPasswordMd5`  (string): GPG private key password. Only required for password protected keys.
* `public_key` / `PublicKey`  (string): The GPG public key
* `private_key` / `PrivateKey`  (string): The GPG private key
* `private_key_password` / `PrivateKeyPassword`  (string): The GPG private key password
* `generate_expires_at` / `GenerateExpiresAt`  (string): Expiration date of the key. Used for the generation of the key. Will be ignored if `generate_keypair` is false.
* `generate_keypair` / `GenerateKeypair`  (bool): If true, generate a new GPG key pair. Can not be used with `public_key`/`private_key`
* `generate_full_name` / `GenerateFullName`  (string): Full name of the key owner. Used for the generation of the key. Will be ignored if `generate_keypair` is false.
* `generate_email` / `GenerateEmail`  (string): Email address of the key owner. Used for the generation of the key. Will be ignored if `generate_keypair` is false.


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
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `name` or `expires_at`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id`, `partner_id` or `expires_at`. Valid field combinations are `[ workspace_id, expires_at ]`.
* `filter_gt` (object): If set, return records where the specified field is greater than the supplied value. Valid fields are `expires_at`.
* `filter_gteq` (object): If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `expires_at`.
* `filter_lt` (object): If set, return records where the specified field is less than the supplied value. Valid fields are `expires_at`.
* `filter_lteq` (object): If set, return records where the specified field is less than or equal the supplied value. Valid fields are `expires_at`.


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
* `partner_id` (Nullable<Int64>): Partner ID who owns this GPG Key, if applicable.
* `public_key` (string): The GPG public key
* `private_key` (string): The GPG private key
* `private_key_password` (string): The GPG private key password
* `name` (string): Required - GPG key name.
* `workspace_id` (Nullable<Int64>): Workspace ID (0 for default workspace).
* `generate_expires_at` (string): Expiration date of the key. Used for the generation of the key. Will be ignored if `generate_keypair` is false.
* `generate_keypair` (bool): If true, generate a new GPG key pair. Can not be used with `public_key`/`private_key`
* `generate_full_name` (string): Full name of the key owner. Used for the generation of the key. Will be ignored if `generate_keypair` is false.
* `generate_email` (string): Email address of the key owner. Used for the generation of the key. Will be ignored if `generate_keypair` is false.


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
* `partner_id` (Nullable<Int64>): Partner ID who owns this GPG Key, if applicable.
* `public_key` (string): The GPG public key
* `private_key` (string): The GPG private key
* `private_key_password` (string): The GPG private key password
* `name` (string): GPG key name.


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

parameters.Add("partner_id", 1);
parameters.Add("name", "key name");

GpgKey.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Gpg Key ID.
* `partner_id` (Nullable<Int64>): Partner ID who owns this GPG Key, if applicable.
* `public_key` (string): The GPG public key
* `private_key` (string): The GPG private key
* `private_key_password` (string): The GPG private key password
* `name` (string): GPG key name.


---

## Delete GPG Key

```
var GpgKey = GpgKey.Find(1);

var parameters = new Dictionary<string, object>();


GpgKey.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Gpg Key ID.
