# FilesCom.Models.SftpHostKey

## Example SftpHostKey Object

```
{
  "id": 1,
  "name": "",
  "fingerprint_md5": "",
  "fingerprint_sha256": ""
}
```

* `id` / `Id`  (Nullable<Int64>): Sftp Host Key ID
* `name` / `Name`  (string): The friendly name of this SFTP Host Key.
* `fingerprint_md5` / `FingerprintMd5`  (string): MD5 Fingerpint of the public key
* `fingerprint_sha256` / `FingerprintSha256`  (string): SHA256 Fingerpint of the public key
* `private_key` / `PrivateKey`  (string): The private key data.


---

## List Sftp Host Keys

```
Task<SftpHostKey[]> SftpHostKey.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via either the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).


---

## Show Sftp Host Key

```
Task<SftpHostKey> SftpHostKey.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Sftp Host Key ID.


---

## Create Sftp Host Key

```
Task<SftpHostKey> SftpHostKey.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `name` (string): The friendly name of this SFTP Host Key.
* `private_key` (string): The private key data.


---

## Update Sftp Host Key

```
Task<SftpHostKey> SftpHostKey.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Sftp Host Key ID.
* `name` (string): The friendly name of this SFTP Host Key.
* `private_key` (string): The private key data.


---

## Delete Sftp Host Key

```
Task<SftpHostKey> SftpHostKey.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Sftp Host Key ID.


---

## Update Sftp Host Key

```
var SftpHostKey = SftpHostKey.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("name", "");
parameters.Add("private_key", "");

SftpHostKey.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Sftp Host Key ID.
* `name` (string): The friendly name of this SFTP Host Key.
* `private_key` (string): The private key data.


---

## Delete Sftp Host Key

```
var SftpHostKey = SftpHostKey.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


SftpHostKey.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Sftp Host Key ID.
