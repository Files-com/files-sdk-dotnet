# FilesCom.Models.Style

## Example Style Object

```
{
  "id": 1,
  "path": "example",
  "logo": "https://mysite.files.com/...",
  "thumbnail": "example"
}
```

* `id` / `Id`  (Nullable<Int64>): Style ID
* `path` / `Path`  (string): Folder path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `logo` / `Logo`  (Image): Logo
* `thumbnail` / `Thumbnail`  (Image): Logo thumbnail
* `file` / `File`  (System.Net.Http.ByteArrayContent): Logo for custom branding.


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
* `file` (System.Net.Http.ByteArrayContent): Required - Logo for custom branding.


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

parameters.Add("file", "file");

Style.Update(parameters);
```

### Parameters

* `path` (string): Required - Style path.
* `file` (System.Net.Http.ByteArrayContent): Required - Logo for custom branding.


---

## Delete Style

```
var Style = Style.Find(1);

var parameters = new Dictionary<string, object>();


Style.Delete
```

### Parameters

* `path` (string): Required - Style path.
