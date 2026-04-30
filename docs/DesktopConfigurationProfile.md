# FilesCom.Models.DesktopConfigurationProfile

## Example DesktopConfigurationProfile Object

```
{
  "id": 1,
  "name": "North America Desktop Profile",
  "workspace_id": 1,
  "use_for_all_users": true,
  "disable_drive_mounting": true,
  "mount_mappings": {
    "key": "example value"
  }
}
```

* `id` / `Id`  (Nullable<Int64>): Desktop Configuration Profile ID
* `name` / `Name`  (string): Profile name
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace ID
* `use_for_all_users` / `UseForAllUsers`  (bool): Whether this profile applies to all users in the Workspace by default
* `disable_drive_mounting` / `DisableDriveMounting`  (bool): Whether the desktop app should hide drive mounting, prevent new drive mounts, and unmount active drive mounts for users with this profile
* `mount_mappings` / `MountMappings`  (object): Mount point mappings for the desktop app. Keys must be a single uppercase Windows drive letter other than A, B, or C, and values are Files.com paths to mount there.


---

## List Desktop Configuration Profiles

```
Task<FilesList<DesktopConfigurationProfile>> DesktopConfigurationProfile.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id` and `name`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id`.


---

## Show Desktop Configuration Profile

```
Task<DesktopConfigurationProfile> DesktopConfigurationProfile.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Desktop Configuration Profile ID.


---

## Create Desktop Configuration Profile

```
Task<DesktopConfigurationProfile> DesktopConfigurationProfile.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `name` (string): Required - Profile name
* `mount_mappings` (object): Required - Mount point mappings for the desktop app. Keys must be a single uppercase Windows drive letter other than A, B, or C, and values are Files.com paths to mount there.
* `workspace_id` (Nullable<Int64>): Workspace ID
* `use_for_all_users` (bool): Whether this profile applies to all users in the Workspace by default
* `disable_drive_mounting` (bool): Whether the desktop app should hide drive mounting, prevent new drive mounts, and unmount active drive mounts for users with this profile


---

## Update Desktop Configuration Profile

```
Task<DesktopConfigurationProfile> DesktopConfigurationProfile.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Desktop Configuration Profile ID.
* `name` (string): Profile name
* `workspace_id` (Nullable<Int64>): Workspace ID
* `mount_mappings` (object): Mount point mappings for the desktop app. Keys must be a single uppercase Windows drive letter other than A, B, or C, and values are Files.com paths to mount there.
* `use_for_all_users` (bool): Whether this profile applies to all users in the Workspace by default
* `disable_drive_mounting` (bool): Whether the desktop app should hide drive mounting, prevent new drive mounts, and unmount active drive mounts for users with this profile


---

## Delete Desktop Configuration Profile

```
Task DesktopConfigurationProfile.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Desktop Configuration Profile ID.


---

## Update Desktop Configuration Profile

```
var DesktopConfigurationProfile = DesktopConfigurationProfile.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("name", "North America Desktop Profile");
parameters.Add("workspace_id", 1);
parameters.Add("mount_mappings", {"key":"example value"});
parameters.Add("use_for_all_users", false);
parameters.Add("disable_drive_mounting", false);

DesktopConfigurationProfile.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Desktop Configuration Profile ID.
* `name` (string): Profile name
* `workspace_id` (Nullable<Int64>): Workspace ID
* `mount_mappings` (object): Mount point mappings for the desktop app. Keys must be a single uppercase Windows drive letter other than A, B, or C, and values are Files.com paths to mount there.
* `use_for_all_users` (bool): Whether this profile applies to all users in the Workspace by default
* `disable_drive_mounting` (bool): Whether the desktop app should hide drive mounting, prevent new drive mounts, and unmount active drive mounts for users with this profile


---

## Delete Desktop Configuration Profile

```
var DesktopConfigurationProfile = DesktopConfigurationProfile.Find(1);

var parameters = new Dictionary<string, object>();


DesktopConfigurationProfile.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Desktop Configuration Profile ID.
