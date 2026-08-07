# FilesCom.Models.AgentNodeConnection

## Example AgentNodeConnection Object

```
{
  "mode": "example",
  "status": "example",
  "last_seen_at": "2000-01-01T01:00:00Z"
}
```

* `mode` / `Mode`  (string): How the Agent process uses this proxy connection
* `status` / `Status`  (string): Whether this connection was observed recently and has not disconnected
* `last_seen_at` / `LastSeenAt`  (Nullable<DateTime>): Most recent successful observation for this connection
