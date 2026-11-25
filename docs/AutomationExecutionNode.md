# FilesCom.Models.AutomationExecutionNode

## Example AutomationExecutionNode Object

```
{
  "node_id": "example",
  "node_type": "example",
  "status": "example",
  "run_stage": "example",
  "reused": true,
  "successful_operations": 1,
  "failed_operations": 1,
  "started_at": "2000-01-01T01:00:00Z",
  "completed_at": "2000-01-01T01:00:00Z",
  "duration_ms": 1,
  "inputs": [
    "example"
  ],
  "outputs": "example",
  "input_items": "example",
  "output_items": "example",
  "item_refs": [
    "example"
  ],
  "item_values": [
    "example"
  ]
}
```

* `node_id` / `NodeId`  (string): Node ID from the pinned Automation definition.
* `node_type` / `NodeType`  (string): Node type.
* `status` / `Status`  (string): Node status.
* `run_stage` / `RunStage`  (string): Current node execution stage.
* `reused` / `Reused`  (bool): Whether this node reused persisted output from an earlier run.
* `successful_operations` / `SuccessfulOperations`  (Nullable<Int64>): Count of successful operations in this node.
* `failed_operations` / `FailedOperations`  (Nullable<Int64>): Count of failed operations in this node.
* `started_at` / `StartedAt`  (Nullable<DateTime>): When this node started.
* `completed_at` / `CompletedAt`  (Nullable<DateTime>): When this node completed.
* `duration_ms` / `DurationMs`  (Nullable<Int64>): Node runtime in milliseconds.
* `inputs` / `Inputs`  (object[]): Ordered inbound edge references.
* `outputs` / `Outputs`  (object): Output counts, item kinds, and typed-error summaries by outlet.
* `input_items` / `InputItems`  (object): Available input items grouped by inlet. Resolve large values with item_refs and item_values.
* `output_items` / `OutputItems`  (object): Output items grouped by outlet. Resolve large values with item_refs and item_values.
* `item_refs` / `ItemRefs`  (object[]): References with a path from the response root and a digest. Replace the null at each path with the value for that digest. Path segments are literal object keys or zero-based array indexes.
* `item_values` / `ItemValues`  (object[]): Ordered entries with a digest, a value, and optional item_refs relative to that value. References inside entries point to earlier entries in this array.
