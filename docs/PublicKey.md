# FilesCom.Models.PublicKey

## Example PublicKey Object

```
{
  "id": 1,
  "workspace_id": 1,
  "title": "My public key",
  "created_at": "2000-01-01T01:00:00Z",
  "fingerprint": "43:51:43:a1:b5:fc:8b:b7:0a:3a:a9:b1:0f:66:73:a8",
  "fingerprint_sha256": "V5Q5t/ghT3R8Tol5GX9385bzmpygWVRnLuI9EXNrjCX",
  "status": "complete",
  "last_login_at": "2000-01-01T01:00:00Z",
  "generated_private_key": "example",
  "generated_public_key": "example",
  "username": "User",
  "user_id": 1
}
```

* `id` / `Id`  (Nullable<Int64>): Public key ID
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace ID (0 for default workspace).
* `title` / `Title`  (string): Public key title
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Public key created at date/time
* `fingerprint` / `Fingerprint`  (string): Public key fingerprint (MD5)
* `fingerprint_sha256` / `FingerprintSha256`  (string): Public key fingerprint (SHA256)
* `status` / `Status`  (string): Only returned when generating keys. Can be invalid, not_generated, generating, complete
* `last_login_at` / `LastLoginAt`  (Nullable<DateTime>): Key's most recent login time via SFTP
* `generated_private_key` / `GeneratedPrivateKey`  (string): Only returned when generating keys. Private key generated for the user.
* `generated_public_key` / `GeneratedPublicKey`  (string): Only returned when generating keys. Public key generated for the user.
* `username` / `Username`  (string): Username of the user this public key is associated with
* `user_id` / `UserId`  (Nullable<Int64>): User ID this public key is associated with
* `public_key` / `PublicKeyType`  (string): Actual contents of SSH key.
* `generate_keypair` / `GenerateKeypair`  (bool): If true, generate a new SSH key pair. Can not be used with `public_key`
* `generate_private_key_password` / `GeneratePrivateKeyPassword`  (string): Password for the private key. Used for the generation of the key. Will be ignored if `generate_keypair` is false.
* `generate_algorithm` / `GenerateAlgorithm`  (string): Type of key to generate.  One of rsa, dsa, ecdsa, ed25519. Used for the generation of the key. Will be ignored if `generate_keypair` is false.
* `generate_length` / `GenerateLength`  (Nullable<Int64>): Length of key to generate. If algorithm is ecdsa, this is the signature size. Used for the generation of the key. Will be ignored if `generate_keypair` is false.


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
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `user_id`, `title` or `created_at`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `created_at` and `workspace_id`.
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
* `public_key` (string): Actual contents of SSH key.
* `generate_keypair` (bool): If true, generate a new SSH key pair. Can not be used with `public_key`
* `generate_private_key_password` (string): Password for the private key. Used for the generation of the key. Will be ignored if `generate_keypair` is false.
* `generate_algorithm` (string): Type of key to generate.  One of rsa, dsa, ecdsa, ed25519. Used for the generation of the key. Will be ignored if `generate_keypair` is false.
* `generate_length` (Nullable<Int64>): Length of key to generate. If algorithm is ecdsa, this is the signature size. Used for the generation of the key. Will be ignored if `generate_keypair` is false.


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
