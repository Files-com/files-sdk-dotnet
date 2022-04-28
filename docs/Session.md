# FilesCom.Models.Session

## Example Session Object

```
{
  "id": "60525f92e859c4c3d74cb02fd176b1525901b525",
  "language": "en",
  "read_only": true,
  "sftp_insecure_ciphers": true
}
```

* `id` / `Id`  (string): Session ID
* `language` / `Language`  (string): Session language
* `read_only` / `ReadOnly`  (bool): Is this session read only?
* `sftp_insecure_ciphers` / `SftpInsecureCiphers`  (bool): Are insecure SFTP ciphers allowed for this user? (If this is set to true, the site administrator has signaled that it is ok to use less secure SSH ciphers for this user.)
* `username` / `Username`  (string): Username to sign in as
* `password` / `Password`  (string): Password for sign in
* `otp` / `Otp`  (string): If this user has a 2FA device, provide its OTP or code here.
* `partial_session_id` / `PartialSessionId`  (string): Identifier for a partially-completed login


---

## Create user session (log in)

```
Task<Session> Session.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `username` (string): Username to sign in as
* `password` (string): Password for sign in
* `otp` (string): If this user has a 2FA device, provide its OTP or code here.
* `partial_session_id` (string): Identifier for a partially-completed login


---

## Delete user session (log out)

```
Task<Session> Session.Delete(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```
