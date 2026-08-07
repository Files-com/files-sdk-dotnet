# FilesCom.Models.AgentNode

## Example AgentNode Object

```
{
  "node_id": "example",
  "name": "example",
  "hostname": "example",
  "availability_role": "example",
  "status": "example",
  "is_default": true,
  "direct_transfer_available": true,
  "last_seen_at": "2000-01-01T01:00:00Z",
  "instances": [
    {
      "instance_id": "example",
      "process_state": "example",
      "status": "example",
      "is_default": true,
      "agent_version": "example",
      "last_seen_at": "2000-01-01T01:00:00Z",
      "connections": [
        {
          "mode": "example",
          "status": "example",
          "last_seen_at": "2000-01-01T01:00:00Z"
        }
      ]
    }
  ]
}
```

* `node_id` / `NodeId`  (string): Stable Agent installation ID
* `name` / `Name`  (string): Customer-configured Agent node name
* `hostname` / `Hostname`  (string): Hostname reported by the Agent
* `availability_role` / `AvailabilityRole`  (string): Configured traffic preference
* `status` / `Status`  (string): Whether this node currently has an available Agent instance
* `is_default` / `IsDefault`  (bool): Whether this node is the current default route for new unscoped work
* `direct_transfer_available` / `DirectTransferAvailable`  (bool): Whether the proxy recently validated a direct connection to this Agent node. False means direct transfers are enabled but not currently available; null means disabled or unsupported.
* `last_seen_at` / `LastSeenAt`  (Nullable<DateTime>): Most recent successful node observation
* `instances` / `Instances`  (AgentNodeInstance[]): Current Agent processes for this node
