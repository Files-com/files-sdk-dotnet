# FilesCom.Models.RemoteServerConfigurationFile

## Example RemoteServerConfigurationFile Object

```
{
  "id": 1,
  "permission_set": "example",
  "private_key": "example",
  "subdomain": "example",
  "root": "C:\\Users\\",
  "follow_links": true,
  "prefer_protocol": "example",
  "dns": "example",
  "proxy_all_outbound": true,
  "endpoint_override": "example",
  "log_file": "example",
  "log_level": "example",
  "log_rotate_num": 1,
  "log_rotate_size": 1,
  "override_max_concurrent_jobs": 1,
  "graceful_shutdown_timeout": 1,
  "transfer_rate_limit": "example",
  "auto_update_policy": "example",
  "api_token": "example",
  "port": 1,
  "hostname": "example",
  "public_key": "example",
  "status": "example",
  "server_host_key": "example",
  "config_version": "example"
}
```

* `id` / `Id`  (Nullable<Int64>): The remote server ID of the agent
* `permission_set` / `PermissionSet`  (string): The permission set for the agent ['read_write', 'read_only', 'write_only']
* `private_key` / `PrivateKey`  (string): The private key for the agent
* `subdomain` / `Subdomain`  (string): Files.com subdomain site name
* `root` / `Root`  (string): The root directory for the agent
* `follow_links` / `FollowLinks`  (bool): Follow symlinks when traversing directories
* `prefer_protocol` / `PreferProtocol`  (string): Preferred network protocol ['udp', 'tcp'] (default udp)
* `dns` / `Dns`  (string): DNS lookup method ['auto','doh','system'] (default auto)
* `proxy_all_outbound` / `ProxyAllOutbound`  (bool): Proxy all outbound traffic through files.com proxy server
* `endpoint_override` / `EndpointOverride`  (string): Custom site endpoint URL
* `log_file` / `LogFile`  (string): Log file name and location
* `log_level` / `LogLevel`  (string): Log level for the agent logs ['debug', 'info', 'warn', 'error', 'fatal'] (default info)
* `log_rotate_num` / `LogRotateNum`  (Nullable<Int64>): Log route for agent logs. (default 5)
* `log_rotate_size` / `LogRotateSize`  (Nullable<Int64>): Log route size in MB for agent logs. (default 20)
* `override_max_concurrent_jobs` / `OverrideMaxConcurrentJobs`  (Nullable<Int64>): Maximum number of concurrent jobs (default 500)
* `graceful_shutdown_timeout` / `GracefulShutdownTimeout`  (Nullable<Int64>): Graceful shutdown timeout in seconds (default 15)
* `transfer_rate_limit` / `TransferRateLimit`  (string): File transfer (upload/download) rate limit
 `<limit>-<period>`, with the given periods:
* 'S': second
* 'M': minute
* 'H': hour
* 'D': day
Examples:
* 5 requests/second: '5-S'
* 10 requests/minute: '10-M'
* 1000 requests/hour: '1000-H'
* 2000 requests/day: '2000-D'
* `auto_update_policy` / `AutoUpdatePolicy`  (string): Auto update policy ['manual_trigger', 'critical_only', 'always', 'never'] (default critical_only)
* `api_token` / `ApiToken`  (string): Files Agent API Token
* `port` / `Port`  (Nullable<Int64>): Incoming port for files agent connections
* `hostname` / `Hostname`  (string): 
* `public_key` / `PublicKey`  (string): public key
* `status` / `Status`  (string): either running or shutdown
* `server_host_key` / `ServerHostKey`  (string): 
* `config_version` / `ConfigVersion`  (string): agent config version
