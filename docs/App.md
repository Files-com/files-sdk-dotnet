# FilesCom.Models.App

## Example App Object

```
{
  "name": "example",
  "extended_description": "example",
  "short_description": "example",
  "documentation_links": {
    "Important Info": "http://files.test/learn-more"
  },
  "icon_url": "example",
  "logo_url": "example",
  "screenshot_list_urls": [
    "example"
  ],
  "logo_thumbnail_url": "example",
  "sso_strategy_type": "example",
  "remote_server_type": "example",
  "folder_behavior_type": "example",
  "external_homepage_url": "example",
  "marketing_youtube_url": "example",
  "tutorial_youtube_url": "example",
  "app_type": "example",
  "featured": true
}
```

* `name` / `Name`  (string): Name of the App
* `extended_description` / `ExtendedDescription`  (string): Long form description of the App
* `short_description` / `ShortDescription`  (string): Short description of the App
* `documentation_links` / `DocumentationLinks`  (object): Collection of named links to documentation
* `icon_url` / `IconUrl`  (string): App icon
* `logo_url` / `LogoUrl`  (string): Full size logo for the App
* `screenshot_list_urls` / `ScreenshotListUrls`  (string[]): Screenshots of the App
* `logo_thumbnail_url` / `LogoThumbnailUrl`  (string): Logo thumbnail for the App
* `sso_strategy_type` / `SsoStrategyType`  (string): Associated SSO Strategy type, if any
* `remote_server_type` / `RemoteServerType`  (string): Associated Remote Server type, if any
* `folder_behavior_type` / `FolderBehaviorType`  (string): Associated Folder Behavior type, if any
* `external_homepage_url` / `ExternalHomepageUrl`  (string): Link to external homepage
* `marketing_youtube_url` / `MarketingYoutubeUrl`  (string): Marketing video page
* `tutorial_youtube_url` / `TutorialYoutubeUrl`  (string): Tutorial video page
* `app_type` / `AppType`  (string): The type of the App
* `featured` / `Featured`  (bool): Is featured on the App listing?


---

## List Apps

```
Task<FilesList<App>> App.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[name]=desc`). Valid fields are `name` and `app_type`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `name` and `app_type`. Valid field combinations are `[ name, app_type ]` and `[ app_type, name ]`.
* `filter_prefix` (object): If set, return records where the specified field is prefixed by the supplied value. Valid fields are `name`.
