# FilesCom.Models.AgentNode

## Example AgentNode Object

```
{
  "node_id": "example",
  "name": "example",
  "hostname": "example",
  "availability_role": "example",
  "connection_status": "example",
  "is_default": true,
  "agent_version": "example",
  "direct_transfer_available": true,
  "last_seen_at": "2000-01-01T01:00:00Z"
}
```

* `node_id` / `NodeId`  (string): Stable Agent installation ID
* `name` / `Name`  (string): Customer-configured Agent node name
* `hostname` / `Hostname`  (string): Hostname reported by the Agent
* `availability_role` / `AvailabilityRole`  (string): Configured traffic preference
* `connection_status` / `ConnectionStatus`  (string): Whether this node is currently available for traffic
* `is_default` / `IsDefault`  (bool): Whether this node is the current default route for new unscoped work
* `agent_version` / `AgentVersion`  (string): Agent version reported by this node
* `direct_transfer_available` / `DirectTransferAvailable`  (bool): Whether the proxy recently validated a direct connection to this Agent node. False means direct transfers are enabled but not currently available; null means disabled or unsupported.
* `last_seen_at` / `LastSeenAt`  (Nullable<DateTime>): Most recent successful node observation
