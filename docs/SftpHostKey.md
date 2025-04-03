# FilesCom.Models.SftpHostKey

## Example SftpHostKey Object

```
{
  "id": 1,
  "name": "My Key",
  "fingerprint_md5": "12:7e:f8:61:78:a4:b2:c2:ee:12:51:92:25:a7:42:cc",
  "fingerprint_sha256": "SHA256:5ANRkDpXWA+PgOquzZAG9RtQ1Bt8KXYAH2hecr7LQk8"
}
```

* `id` / `Id`  (Nullable<Int64>): SFTP Host Key ID
* `name` / `Name`  (string): The friendly name of this SFTP Host Key.
* `fingerprint_md5` / `FingerprintMd5`  (string): MD5 Fingerprint of the public key
* `fingerprint_sha256` / `FingerprintSha256`  (string): SHA256 Fingerprint of the public key
* `private_key` / `PrivateKey`  (string): The private key data.


---

## List SFTP Host Keys

```
Task<FilesList<SftpHostKey>> SftpHostKey.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).


---

## Show SFTP Host Key

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

## Create SFTP Host Key

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

## Update SFTP Host Key

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

## Delete SFTP Host Key

```
Task SftpHostKey.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Sftp Host Key ID.


---

## Update SFTP Host Key

```
var SftpHostKey = SftpHostKey.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("name", "My Key");

SftpHostKey.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Sftp Host Key ID.
* `name` (string): The friendly name of this SFTP Host Key.
* `private_key` (string): The private key data.


---

## Delete SFTP Host Key

```
var SftpHostKey = SftpHostKey.Find(1);

var parameters = new Dictionary<string, object>();


SftpHostKey.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Sftp Host Key ID.
