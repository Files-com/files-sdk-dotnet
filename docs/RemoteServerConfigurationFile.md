# FilesCom.Models.RemoteServerConfigurationFile

## Example RemoteServerConfigurationFile Object

```
{
  "id": 1,
  "permission_set": "full",
  "api_token": "example",
  "root": "example",
  "port": 1,
  "hostname": "example",
  "public_key": "example",
  "private_key": "example",
  "status": "example",
  "config_version": "example",
  "server_host_key": "example",
  "subdomain": "example"
}
```

* `id` / `Id`  (Nullable<Int64>): Agent ID
* `permission_set` / `PermissionSet`  (string): 
* `api_token` / `ApiToken`  (string): Files Agent API Token
* `root` / `Root`  (string): Agent local root path
* `port` / `Port`  (Nullable<Int64>): Incoming port for files agent connections
* `hostname` / `Hostname`  (string): 
* `public_key` / `PublicKey`  (string): public key
* `private_key` / `PrivateKey`  (string): private key
* `status` / `Status`  (string): either running or shutdown
* `config_version` / `ConfigVersion`  (string): agent config version
* `server_host_key` / `ServerHostKey`  (string): 
* `subdomain` / `Subdomain`  (string): 
