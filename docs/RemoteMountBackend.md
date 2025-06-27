# FilesCom.Models.RemoteMountBackend

## Example RemoteMountBackend Object

```
{
  "canary_file_path": "backend1.txt",
  "enabled": true,
  "fall": 1,
  "health_check_enabled": true,
  "health_check_type": "active",
  "id": 1,
  "interval": 60,
  "min_free_cpu": 1.0,
  "min_free_mem": 1.0,
  "priority": 1,
  "remote_path": "/path/on/remote",
  "remote_server_id": 1,
  "remote_server_mount_id": 1,
  "rise": 1,
  "status": "healthy",
  "undergoing_maintenance": true
}
```

* `canary_file_path` / `CanaryFilePath`  (string): Path to the canary file used for health checks.
* `enabled` / `Enabled`  (bool): True if this backend is enabled.
* `fall` / `Fall`  (Nullable<Int64>): Number of consecutive failures before considering the backend unhealthy.
* `health_check_enabled` / `HealthCheckEnabled`  (bool): True if health checks are enabled for this backend.
* `health_check_type` / `HealthCheckType`  (string): Type of health check to perform.
* `id` / `Id`  (Nullable<Int64>): Unique identifier for this backend.
* `interval` / `Interval`  (Nullable<Int64>): Interval in seconds between health checks.
* `min_free_cpu` / `MinFreeCpu`  (double): Minimum free CPU percentage required for this backend to be considered healthy.
* `min_free_mem` / `MinFreeMem`  (double): Minimum free memory percentage required for this backend to be considered healthy.
* `priority` / `Priority`  (Nullable<Int64>): Priority of this backend.
* `remote_path` / `RemotePath`  (string): Path on the remote server to treat as the root of this mount.
* `remote_server_id` / `RemoteServerId`  (Nullable<Int64>): The remote server that this backend is associated with.
* `remote_server_mount_id` / `RemoteServerMountId`  (Nullable<Int64>): The mount ID of the Remote Server Mount that this backend is associated with.
* `rise` / `Rise`  (Nullable<Int64>): Number of consecutive successes before considering the backend healthy.
* `status` / `Status`  (string): Status of this backend.
* `undergoing_maintenance` / `UndergoingMaintenance`  (bool): True if this backend is undergoing maintenance.


---

## List Remote Mount Backends

```
Task<FilesList<RemoteMountBackend>> RemoteMountBackend.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `remote_server_mount_id`.


---

## Show Remote Mount Backend

```
Task<RemoteMountBackend> RemoteMountBackend.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Mount Backend ID.


---

## Create Remote Mount Backend

```
Task<RemoteMountBackend> RemoteMountBackend.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `enabled` (bool): True if this backend is enabled.
* `fall` (Nullable<Int64>): Number of consecutive failures before considering the backend unhealthy.
* `health_check_enabled` (bool): True if health checks are enabled for this backend.
* `health_check_type` (string): Type of health check to perform.
* `interval` (Nullable<Int64>): Interval in seconds between health checks.
* `min_free_cpu` (double): Minimum free CPU percentage required for this backend to be considered healthy.
* `min_free_mem` (double): Minimum free memory percentage required for this backend to be considered healthy.
* `priority` (Nullable<Int64>): Priority of this backend.
* `remote_path` (string): Path on the remote server to treat as the root of this mount.
* `rise` (Nullable<Int64>): Number of consecutive successes before considering the backend healthy.
* `canary_file_path` (string): Required - Path to the canary file used for health checks.
* `remote_server_mount_id` (Nullable<Int64>): Required - The mount ID of the Remote Server Mount that this backend is associated with.
* `remote_server_id` (Nullable<Int64>): Required - The remote server that this backend is associated with.


---

## Reset backend status to healthy

```
Task RemoteMountBackend.ResetStatus(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Mount Backend ID.


---

## Update Remote Mount Backend

```
Task<RemoteMountBackend> RemoteMountBackend.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Mount Backend ID.
* `enabled` (bool): True if this backend is enabled.
* `fall` (Nullable<Int64>): Number of consecutive failures before considering the backend unhealthy.
* `health_check_enabled` (bool): True if health checks are enabled for this backend.
* `health_check_type` (string): Type of health check to perform.
* `interval` (Nullable<Int64>): Interval in seconds between health checks.
* `min_free_cpu` (double): Minimum free CPU percentage required for this backend to be considered healthy.
* `min_free_mem` (double): Minimum free memory percentage required for this backend to be considered healthy.
* `priority` (Nullable<Int64>): Priority of this backend.
* `remote_path` (string): Path on the remote server to treat as the root of this mount.
* `rise` (Nullable<Int64>): Number of consecutive successes before considering the backend healthy.
* `canary_file_path` (string): Path to the canary file used for health checks.
* `remote_server_id` (Nullable<Int64>): The remote server that this backend is associated with.


---

## Delete Remote Mount Backend

```
Task RemoteMountBackend.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Mount Backend ID.


---

## Reset backend status to healthy

```
var RemoteMountBackend = RemoteMountBackend.Find(1);

var parameters = new Dictionary<string, object>();


RemoteMountBackend.ResetStatus
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Mount Backend ID.


---

## Update Remote Mount Backend

```
var RemoteMountBackend = RemoteMountBackend.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("enabled", true);
parameters.Add("fall", 1);
parameters.Add("health_check_enabled", true);
parameters.Add("health_check_type", "active");
parameters.Add("interval", 60);
parameters.Add("min_free_cpu", 1.0);
parameters.Add("min_free_mem", 1.0);
parameters.Add("priority", 1);
parameters.Add("remote_path", "/path/on/remote");
parameters.Add("rise", 1);
parameters.Add("canary_file_path", "backend1.txt");
parameters.Add("remote_server_id", 1);

RemoteMountBackend.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Mount Backend ID.
* `enabled` (bool): True if this backend is enabled.
* `fall` (Nullable<Int64>): Number of consecutive failures before considering the backend unhealthy.
* `health_check_enabled` (bool): True if health checks are enabled for this backend.
* `health_check_type` (string): Type of health check to perform.
* `interval` (Nullable<Int64>): Interval in seconds between health checks.
* `min_free_cpu` (double): Minimum free CPU percentage required for this backend to be considered healthy.
* `min_free_mem` (double): Minimum free memory percentage required for this backend to be considered healthy.
* `priority` (Nullable<Int64>): Priority of this backend.
* `remote_path` (string): Path on the remote server to treat as the root of this mount.
* `rise` (Nullable<Int64>): Number of consecutive successes before considering the backend healthy.
* `canary_file_path` (string): Path to the canary file used for health checks.
* `remote_server_id` (Nullable<Int64>): The remote server that this backend is associated with.


---

## Delete Remote Mount Backend

```
var RemoteMountBackend = RemoteMountBackend.Find(1);

var parameters = new Dictionary<string, object>();


RemoteMountBackend.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Mount Backend ID.
