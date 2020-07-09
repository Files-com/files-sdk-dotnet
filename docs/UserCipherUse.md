# Files.Models.UserCipherUse

## Example UserCipherUse Object

```
{
  "id": 1,
  "protocol_cipher": "TLSv1.2; ECDHE-RSA-AES256-GCM-SHA384",
  "created_at": "2000-01-01T01:00:00Z",
  "interface": "restapi",
  "updated_at": "2000-01-01T01:00:00Z",
  "user_id": 1
}
```

* `id` / `Id`  (Nullable<Int64>): UserCipherUse ID
* `protocol_cipher` / `ProtocolCipher`  (string): The protocol and cipher employed
* `created_at` / `CreatedAt`  (Nullable<DateTime>): The earliest recorded use of this combination of interface and protocol and cipher (for this user)
* `interface` / `Interface`  (string): The interface accessed
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): The most recent use of this combination of interface and protocol and cipher (for this user)
* `user_id` / `UserId`  (Nullable<Int64>): ID of the user who performed this access


---

## List User Cipher Uses

```
Task<UserCipherUse[]> UserCipherUse.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `page` (Nullable<Int64>): Current page number.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
