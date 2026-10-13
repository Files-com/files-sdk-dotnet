# FilesCom.Models.SftpHostKey

## Example SftpHostKey Object

```
{
  "active": true,
  "custom_domain_id": 1,
  "id": 1,
  "name": "My Key",
  "key_type": "ssh-rsa",
  "fingerprint_md5": "12:7e:f8:61:78:a4:b2:c2:ee:12:51:92:25:a7:42:cc",
  "fingerprint_sha256": "SHA256:5ANRkDpXWA+PgOquzZAG9RtQ1Bt8KXYAH2hecr7LQk8"
}
```

* `active` / `Active`  (bool): If true, use this SFTP Host Key.
* `custom_domain_id` / `CustomDomainId`  (Nullable<Int64>): Custom Domain ID. If set, this key is used only for that Custom Domain.
* `id` / `Id`  (Nullable<Int64>): SFTP Host Key ID
* `name` / `Name`  (string): The friendly name of this SFTP Host Key.
* `key_type` / `KeyType`  (string): SSH key type
* `fingerprint_md5` / `FingerprintMd5`  (string): MD5 Fingerprint of the public key
* `fingerprint_sha256` / `FingerprintSha256`  (string): SHA256 Fingerprint of the public key
* `private_key` / `PrivateKey`  (string): The private key data.


---

## List SFTP Host Keys

```
FilesList<SftpHostKey> SftpHostKey.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials:

```
FilesList<SftpHostKey> client.SftpHostKeys.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).


---

## Show SFTP Host Key

```
Task<SftpHostKey> SftpHostKey.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<SftpHostKey> client.SftpHostKeys.FindAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
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

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<SftpHostKey> client.SftpHostKeys.CreateAsync(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `active` (bool): If true, use this SFTP Host Key.
* `custom_domain_id` (Nullable<Int64>): Custom Domain ID. If set, this key is used only for that Custom Domain.
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

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<SftpHostKey> client.SftpHostKeys.UpdateAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Sftp Host Key ID.
* `active` (bool): If true, use this SFTP Host Key.
* `custom_domain_id` (Nullable<Int64>): Custom Domain ID. If set, this key is used only for that Custom Domain.
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

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task client.SftpHostKeys.DeleteAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Sftp Host Key ID.


---

## Update SFTP Host Key

```
var sftpHostKey = await SftpHostKey.Find(1L);

var parameters = new Dictionary<string, object>();

parameters.Add("active", true);
parameters.Add("custom_domain_id", 1L);
parameters.Add("name", "My Key");

await sftpHostKey.Update(parameters);
```

`UpdateAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.
When it succeeds, the object takes the attributes Files.com returns. The `SftpHostKey` it returns is a separate object with those attributes.

### Parameters

* `id` (Nullable<Int64>): Required - Sftp Host Key ID.
* `active` (bool): If true, use this SFTP Host Key.
* `custom_domain_id` (Nullable<Int64>): Custom Domain ID. If set, this key is used only for that Custom Domain.
* `name` (string): The friendly name of this SFTP Host Key.
* `private_key` (string): The private key data.


---

## Delete SFTP Host Key

```
var sftpHostKey = await SftpHostKey.Find(1L);

var parameters = new Dictionary<string, object>();

await sftpHostKey.Delete(parameters);
```

`DeleteAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.

### Parameters

* `id` (Nullable<Int64>): Required - Sftp Host Key ID.


---

## Save

```
await sftpHostKey.SaveAsync(cancellationToken);
```

Creates the object on Files.com, or updates it if it has an ID. It sends the attributes set on the object, in its constructor or with its properties, or changed in place, such as an element of an array, since it was made or last came from Files.com, and only those the operation takes, each in the form its parameter takes. When it succeeds, the object takes the attributes Files.com returns. `Save()` does the same without a cancellation token.
