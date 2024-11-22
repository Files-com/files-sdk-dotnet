# FilesCom.Models.App

## Example App Object

```
{
  "app_type": "example",
  "documentation_links": {
    "Important Info": "http://files.test/learn-more"
  },
  "extended_description": "example",
  "extended_description_for_marketing_site": "example",
  "external_homepage_url": "example",
  "featured": true,
  "folder_behavior_type": "example",
  "icon_url": "example",
  "logo_thumbnail_url": "example",
  "logo_url": "example",
  "marketing_intro": "example",
  "marketing_youtube_url": "example",
  "name": "example",
  "package_manager_install_command": "example",
  "remote_server_type": "example",
  "screenshot_list_urls": [
    "example"
  ],
  "sdk_installation_instructions_link": "example",
  "short_description": "example",
  "sso_strategy_type": "example",
  "siem_type": "example",
  "tutorial_youtube_url": "example"
}
```

* `app_type` / `AppType`  (string): The type of the App
* `documentation_links` / `DocumentationLinks`  (object): Collection of named links to documentation
* `extended_description` / `ExtendedDescription`  (string): Long description for the in-App landing page
* `extended_description_for_marketing_site` / `ExtendedDescriptionForMarketingSite`  (string): Long form description of the App
* `external_homepage_url` / `ExternalHomepageUrl`  (string): Link to external homepage
* `featured` / `Featured`  (bool): Is featured on the App listing?
* `folder_behavior_type` / `FolderBehaviorType`  (string): Associated Folder Behavior type, if any
* `icon_url` / `IconUrl`  (string): App icon
* `logo_thumbnail_url` / `LogoThumbnailUrl`  (string): Logo thumbnail for the App
* `logo_url` / `LogoUrl`  (string): Full size logo for the App
* `marketing_intro` / `MarketingIntro`  (string): Marketing introdution of the App
* `marketing_youtube_url` / `MarketingYoutubeUrl`  (string): Marketing video page
* `name` / `Name`  (string): Name of the App
* `package_manager_install_command` / `PackageManagerInstallCommand`  (string): Package manager install command
* `remote_server_type` / `RemoteServerType`  (string): Associated Remote Server type, if any
* `screenshot_list_urls` / `ScreenshotListUrls`  (string[]): Screenshots of the App
* `sdk_installation_instructions_link` / `SdkInstallationInstructionsLink`  (string): Link to SDK installation instructions
* `short_description` / `ShortDescription`  (string): Short description of the App
* `sso_strategy_type` / `SsoStrategyType`  (string): Associated SSO Strategy type, if any
* `siem_type` / `SiemType`  (string): Associated SIEM type, if any
* `tutorial_youtube_url` / `TutorialYoutubeUrl`  (string): Tutorial video page


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
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `name` and `app_type`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `name` and `app_type`. Valid field combinations are `[ name, app_type ]`.
* `filter_prefix` (object): If set, return records where the specified field is prefixed by the supplied value. Valid fields are `name`.
