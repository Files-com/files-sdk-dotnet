# FilesCom.Models.Style

## Example Style Object

```
{
  "id": 1,
  "path": "example",
  "logo": "https://mysite.files.com/...",
  "logo_click_href": "https://www.example.com",
  "thumbnail": {
    "name": "My logo",
    "uri": "https://mysite.files.com/.../my_image.png"
  }
}
```

* `id` / `Id`  (Nullable<Int64>): Style ID
* `path` / `Path`  (string): Folder path. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `logo` / `Logo`  (Image): Logo
* `logo_click_href` / `LogoClickHref`  (string): URL to open when a public visitor clicks the logo
* `thumbnail` / `Thumbnail`  (Image): Logo thumbnail
* `file` / `File`  (System.Net.Http.ByteArrayContent): Logo for custom branding. Required when creating a new style.


---

## Show Style

```
Task<Style> Style.Find(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `path` (string): Required - Style path.


---

## Update Style

```
Task<Style> Style.Update(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `path` (string): Required - Style path.
* `file` (System.Net.Http.ByteArrayContent): Logo for custom branding. Required when creating a new style.
* `logo_click_href` (string): URL to open when a public visitor clicks the logo.


---

## Delete Style

```
Task Style.Delete(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `path` (string): Required - Style path.


---

## Update Style

```
var Style = Style.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("logo_click_href", "https://www.example.com");

Style.Update(parameters);
```

### Parameters

* `path` (string): Required - Style path.
* `file` (System.Net.Http.ByteArrayContent): Logo for custom branding. Required when creating a new style.
* `logo_click_href` (string): URL to open when a public visitor clicks the logo.


---

## Delete Style

```
var Style = Style.Find(1);

var parameters = new Dictionary<string, object>();


Style.Delete
```

### Parameters

* `path` (string): Required - Style path.
