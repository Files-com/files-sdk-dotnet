# FilesCom.Models.Action

## Example Action Object

```
{
  "id": 1,
  "path": "",
  "when": "2000-01-01T01:00:00Z",
  "destination": "/to_path",
  "display": "Actual text of the action here.",
  "ip": "192.283.128.182",
  "source": "/from_path",
  "targets": null,
  "user_id": 1,
  "username": "user",
  "user_is_from_parent_site": true,
  "action": "create",
  "failure_type": "none",
  "interface": "web"
}
```

* `id` / `Id`  (Nullable<Int64>): Action ID
* `path` / `Path`  (string): Path. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `when` / `When`  (Nullable<DateTime>): Action occurrence date/time
* `destination` / `Destination`  (string): The destination path for this action, if applicable
* `display` / `Display`  (string): Friendly displayed output
* `ip` / `Ip`  (string): IP Address that performed this action
* `source` / `Source`  (string): The source path for this action, if applicable
* `targets` / `Targets`  (object): Targets
* `user_id` / `UserId`  (Nullable<Int64>): User ID
* `username` / `Username`  (string): Username
* `user_is_from_parent_site` / `UserIsFromParentSite`  (bool): true if this change was performed by a user on a parent site.
* `action` / `ActionType`  (string): Type of action
* `failure_type` / `FailureType`  (string): Failure type.  If action was a user login or session failure, why did it fail?
* `interface` / `Interface`  (string): Interface on which this action occurred.
