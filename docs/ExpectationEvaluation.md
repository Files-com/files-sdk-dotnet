# FilesCom.Models.ExpectationEvaluation

## Example ExpectationEvaluation Object

```
{
  "id": 1,
  "workspace_id": 1,
  "expectation_id": 1,
  "status": "open",
  "opened_via": "manual",
  "opened_at": "2000-01-01T01:00:00Z",
  "window_start_at": "2000-01-01T01:00:00Z",
  "window_end_at": "2000-01-01T01:00:00Z",
  "deadline_at": "2000-01-01T01:00:00Z",
  "late_acceptance_cutoff_at": "2000-01-01T01:00:00Z",
  "hard_close_at": "2000-01-01T01:00:00Z",
  "closed_at": "2000-01-01T01:00:00Z",
  "matched_files": [

  ],
  "missing_files": [

  ],
  "criteria_errors": [

  ],
  "summary": null,
  "created_at": "2000-01-01T01:00:00Z",
  "updated_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): ExpectationEvaluation ID
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace ID. `0` means the default workspace.
* `expectation_id` / `ExpectationId`  (Nullable<Int64>): Expectation ID.
* `status` / `Status`  (string): Evaluation status.
* `opened_via` / `OpenedVia`  (string): How the evaluation window was opened.
* `opened_at` / `OpenedAt`  (Nullable<DateTime>): When the evaluation row was opened.
* `window_start_at` / `WindowStartAt`  (Nullable<DateTime>): Logical start of the candidate window.
* `window_end_at` / `WindowEndAt`  (Nullable<DateTime>): Actual candidate cutoff boundary for the window.
* `deadline_at` / `DeadlineAt`  (Nullable<DateTime>): Logical due boundary for schedule-driven windows.
* `late_acceptance_cutoff_at` / `LateAcceptanceCutoffAt`  (Nullable<DateTime>): Logical cutoff for late acceptance, when present.
* `hard_close_at` / `HardCloseAt`  (Nullable<DateTime>): Hard stop after which the window may not remain open.
* `closed_at` / `ClosedAt`  (Nullable<DateTime>): When the evaluation row was finalized.
* `matched_files` / `MatchedFiles`  (object[]): Captured evidence for files that matched the window.
* `missing_files` / `MissingFiles`  (object[]): Captured evidence for required files that were missing.
* `criteria_errors` / `CriteriaErrors`  (object[]): Captured criteria failures for the window.
* `summary` / `Summary`  (object): Compact evaluator summary payload.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Creation time.
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): Last update time.


---

## List Expectation Evaluations

```
Task<FilesList<ExpectationEvaluation>> ExpectationEvaluation.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `created_at` or `expectation_id`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `expectation_id` and `workspace_id`. Valid field combinations are `[ workspace_id, expectation_id ]`.


---

## Show Expectation Evaluation

```
Task<ExpectationEvaluation> ExpectationEvaluation.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Expectation Evaluation ID.
