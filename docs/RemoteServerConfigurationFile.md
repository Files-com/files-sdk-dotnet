# FilesCom.Models.RemoteServerConfigurationFile

## Example RemoteServerConfigurationFile Object

```
{
  "id": 1,
  "permission_set": "full",
  "private_key": "example",
  "subdomain": "example",
  "root": "example",
  "api_token": "example",
  "port": 1,
  "hostname": "example",
  "public_key": "example",
  "status": "example",
  "server_host_key": "example",
  "config_version": "example"
}
```

* `id` / `Id`  (Nullable<Int64>): Agent ID
* `permission_set` / `PermissionSet`  (string): 
* `private_key` / `PrivateKey`  (string): private key
* `subdomain` / `Subdomain`  (string): 
* `root` / `Root`  (string): Agent local root path
* `api_token` / `ApiToken`  (string): Files Agent API Token
* `port` / `Port`  (Nullable<Int64>): Incoming port for files agent connections
* `hostname` / `Hostname`  (string): 
* `public_key` / `PublicKey`  (string): public key
* `status` / `Status`  (string): either running or shutdown
* `server_host_key` / `ServerHostKey`  (string): 
* `config_version` / `ConfigVersion`  (string): agent config version
