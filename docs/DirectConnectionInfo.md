# FilesCom.Models.DirectConnectionInfo

## Example DirectConnectionInfo Object

```
{
  "version": 1,
  "server_name": "example",
  "addresses": [
    "example"
  ],
  "direct_uri": "example",
  "ca_pem": "example"
}
```

* `version` / `Version`  (Nullable<Int64>): Direct connection information schema version.
* `server_name` / `ServerName`  (string): TLS server name (SNI and Host header) for the Agent's direct transfer listener.
* `addresses` / `Addresses`  (string[]): Validated ip:port candidates that may be dialed over TCP+TLS for this transfer.
* `direct_uri` / `DirectUri`  (string): Signed HTTPS URI for direct Agent transfer traffic.
* `ca_pem` / `CaPem`  (string): PEM-encoded CA certificate used to verify the Agent's direct transfer TLS certificate.
