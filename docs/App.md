# Files.Models.App

## Example App Object

```
{
  "name": "",
  "extended_description": "",
  "documentation_links": "Important Info => http://files.test/learn-more",
  "sso_strategy_type": "",
  "remote_server_type": "",
  "folder_behavior_type": "",
  "external_homepage_url": "",
  "app_type": "",
  "featured": true
}
```

* `name` / `Name`  (string): Name of the App
* `extended_description` / `ExtendedDescription`  (string): Long form description of the App
* `documentation_links` / `DocumentationLinks`  (string): Collection of named links to documentation
* `sso_strategy_type` / `SsoStrategyType`  (string): Associated SSO Strategy type, if any
* `remote_server_type` / `RemoteServerType`  (string): Associated Remote Server type, if any
* `folder_behavior_type` / `FolderBehaviorType`  (string): Associated Folder Behavior type, if any
* `external_homepage_url` / `ExternalHomepageUrl`  (string): Link to external homepage
* `app_type` / `AppType`  (string): The type of the App
* `featured` / `Featured`  (bool): Is featured on the App listing?


---

## List Apps

```
Task<App[]> App.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `page` (Nullable<Int64>): Current page number.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
