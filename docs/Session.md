# FilesCom.Models.Session

## Example Session Object

```
{
  "id": "60525f92e859c4c3d74cb02fd176b1525901b525",
  "language": "en",
  "login_token": "@tok-randomcode",
  "login_token_domain": "https://mysite.files.com",
  "max_dir_listing_size": 1,
  "multiple_regions": true,
  "read_only": true,
  "root_path": "",
  "sftp_insecure_ciphers": true,
  "site_id": 1,
  "ssl_required": true,
  "tls_disabled": true,
  "two_factor_setup_needed": true,
  "allowed_2fa_method_sms": true,
  "allowed_2fa_method_totp": true,
  "allowed_2fa_method_u2f": true,
  "allowed_2fa_method_yubi": true,
  "use_provided_modified_at": true,
  "windows_mode_ftp": true
}
```

* `id` / `Id`  (string): Session ID
* `language` / `Language`  (string): Session language
* `login_token` / `LoginToken`  (string): Login token. If set, this token will allow your user to log in via browser at the domain in `login_token_domain`.
* `login_token_domain` / `LoginTokenDomain`  (string): Domain to use with `login_token`.
* `max_dir_listing_size` / `MaxDirListingSize`  (Nullable<Int64>): Maximum number of files to retrieve per folder for a directory listing.  This is based on the user's plan.
* `multiple_regions` / `MultipleRegions`  (bool): Can access multiple regions?
* `read_only` / `ReadOnly`  (bool): Is this session read only?
* `root_path` / `RootPath`  (string): Initial root path to start the user's session in.
* `sftp_insecure_ciphers` / `SftpInsecureCiphers`  (bool): Are insecure SFTP ciphers allowed for this user? (If this is set to true, the site administrator has signaled that it is ok to use less secure SSH ciphers for this user.)
* `site_id` / `SiteId`  (Nullable<Int64>): Site ID
* `ssl_required` / `SslRequired`  (bool): Is SSL required for this user?  (If so, ensure all your communications with this user use SSL.)
* `tls_disabled` / `TlsDisabled`  (bool): Aee insecure TLS versions allowed for this user? (If this is set to true, the site administrator has signaled that it is ok to use less secure TLS versions for this user.)
* `two_factor_setup_needed` / `TwoFactorSetupNeeded`  (bool): If true, this user needs to add a Two Factor Authentication method before performing any further actions.
* `allowed_2fa_method_sms` / `Allowed2faMethodSms`  (bool): Sent only if 2FA setup is needed. Is SMS two factor authentication allowed?
* `allowed_2fa_method_totp` / `Allowed2faMethodTotp`  (bool): Sent only if 2FA setup is needed. Is TOTP two factor authentication allowed?
* `allowed_2fa_method_u2f` / `Allowed2faMethodU2f`  (bool): Sent only if 2FA setup is needed. Is U2F two factor authentication allowed?
* `allowed_2fa_method_yubi` / `Allowed2faMethodYubi`  (bool): Sent only if 2FA setup is needed. Is Yubikey two factor authentication allowed?
* `use_provided_modified_at` / `UseProvidedModifiedAt`  (bool): Allow the user to provide file/folder modified at dates?  If false, the server will always use the current date/time.
* `windows_mode_ftp` / `WindowsModeFtp`  (bool): Does this user want to use Windows line-ending emulation?  (CR vs CRLF)
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
