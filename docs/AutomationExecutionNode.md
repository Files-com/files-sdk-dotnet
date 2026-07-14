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
  "output_items": "example"
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
* `input_items` / `InputItems`  (object): Materialized input items currently available, grouped by inlet.
* `output_items` / `OutputItems`  (object): Materialized output items grouped by outlet.
