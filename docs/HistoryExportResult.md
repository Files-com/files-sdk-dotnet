# FilesCom.Models.HistoryExportResult

## Example HistoryExportResult Object

```
{
  "id": 1,
  "created_at": 1,
  "created_at_iso8601": 1,
  "user_id": 1,
  "file_id": 1,
  "parent_id": 1,
  "path": "MyFile.txt",
  "folder": "Folder",
  "src": "SrcFolder",
  "destination": "DestFolder",
  "ip": "127.0.0.1",
  "username": "jerry",
  "action": "read",
  "failure_type": "bad_password",
  "interface": "ftp",
  "target_id": 1,
  "target_name": "full",
  "target_permission": "full",
  "target_recursive": true,
  "target_expires_at": 1,
  "target_permission_set": "desktop_app",
  "target_platform": "windows",
  "target_username": "jerry",
  "target_user_id": 1
}
```

* `id` / `Id`  (Nullable<Int64>): Action ID
* `created_at` / `CreatedAt`  (Nullable<Int64>): When the action happened
* `created_at_iso8601` / `CreatedAtIso8601`  (Nullable<Int64>): When the action happened, in ISO8601 format.
* `user_id` / `UserId`  (Nullable<Int64>): User ID
* `file_id` / `FileId`  (Nullable<Int64>): File ID related to the action
* `parent_id` / `ParentId`  (Nullable<Int64>): ID of the parent folder
* `path` / `Path`  (string): Path of the related action This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `folder` / `Folder`  (string): Folder in which the action occurred
* `src` / `Src`  (string): File move originated from this path
* `destination` / `Destination`  (string): File moved to this destination folder
* `ip` / `Ip`  (string): Client IP that performed the action
* `username` / `Username`  (string): Username of the user that performed the action
* `action` / `Action`  (string): What action was taken. Valid values: `create`, `read`, `update`, `destroy`, `move`, `login`, `failedlogin`, `copy`, `user_create`, `user_update`, `user_destroy`, `group_create`, `group_update`, `group_destroy`, `permission_create`, `permission_destroy`, `api_key_create`, `api_key_update`, `api_key_destroy`
* `failure_type` / `FailureType`  (string): The type of login failure, if applicable.  Valid values: `expired_trial`, `account_overdue`, `locked_out`, `ip_mismatch`, `password_mismatch`, `site_mismatch`, `username_not_found`, `none`, `no_ftp_permission`, `no_web_permission`, `no_directory`, `errno_enoent`, `no_sftp_permission`, `no_dav_permission`, `no_restapi_permission`, `key_mismatch`, `region_mismatch`, `expired_access`, `desktop_ip_mismatch`, `desktop_api_key_not_used_quickly_enough`, `disabled`, `country_mismatch`
* `interface` / `Interface`  (string): Inteface through which the action was taken. Valid values: `web`, `ftp`, `robot`, `jsapi`, `webdesktopapi`, `sftp`, `dav`, `desktop`, `restapi`, `scim`, `office`, `mobile`, `as2`, `inbound_email`, `remote`
* `target_id` / `TargetId`  (Nullable<Int64>): ID of the object (such as Users, or API Keys) on which the action was taken
* `target_name` / `TargetName`  (string): Name of the User, Group or other object with a name related to this action
* `target_permission` / `TargetPermission`  (string): Permission level of the action
* `target_recursive` / `TargetRecursive`  (bool): Whether or not the action was recursive
* `target_expires_at` / `TargetExpiresAt`  (Nullable<Int64>): If searching for Histories about API keys, this is when the API key will expire
* `target_permission_set` / `TargetPermissionSet`  (string): If searching for Histories about API keys, this represents the permission set of the associated  API key
* `target_platform` / `TargetPlatform`  (string): If searching for Histories about API keys, this is the platform on which the action was taken
* `target_username` / `TargetUsername`  (string): If searching for Histories about API keys, this is the username on which the action was taken
* `target_user_id` / `TargetUserId`  (Nullable<Int64>): If searching for Histories about API keys, this is the User ID on which the action was taken


---

## List History Export Results

```
Task<HistoryExportResult[]> HistoryExportResult.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via either the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `history_export_id` (Nullable<Int64>): Required - ID of the associated history export.
