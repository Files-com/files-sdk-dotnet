# Files.com .NET Client

The Files.com .NET client library provides a direct, high performance integration to Files.com from applications using the .NET framework.

Files.com is the cloud-native, next-gen MFT, SFTP, and secure file-sharing platform that replaces brittle legacy servers with one always-on, secure fabric. Automate mission-critical file flows—across any cloud, protocol, or partner—while supporting human collaboration and eliminating manual work.

With universal SFTP, AS2, HTTPS, and 50+ native connectors backed by military-grade encryption, Files.com unifies governance, visibility, and compliance in a single pane of glass.

The content included here should be enough to get started, but please visit our
[Developer Documentation Website](https://developers.files.com/net/) for the complete documentation.

## Introduction

The Files.com .NET client library provides convenient access to all aspects of Files.com from applications using the .NET framework.

Files.com customers use our .NET client library for directly working with files and folders as well as performing management tasks such as adding/removing users, onboarding counterparties, retrieving information about automations and more.

Every function in the Files.com application is available via .NET.  Nothing is excluded.

The .NET client library uses the Files.com RESTful APIs via the HTTPS protocol (port 443) to securely communicate and transfer files so no firewall changes should be required in order to allow connectivity to Files.com.

### Files.com is Committed to .NET

.NET is our second most popular integration language for custom development and is supported by our highest level of support.

### Frameworks Supported

- .NET 8
- .NET 6
- .NET 5
- .NET Core 2.1, 2.2, 3.0, 3.1
- .NET Standard 2.0
- .NET Framework 4.6.1, 4.6.2, 4.7, 4.7.1, 4.7.2, 4.8

.NET Framework versions below 4.6.1 are officially in end-of-life status according to Microsoft.  As a policy, Files.com does not support integrations which are considered end-of-life by their vendor.

If you need support for a newer framework version, please ask, and we'll be happy to add it.  It will most likely work out of the box anyway.

### Installation

#### Command Line Installation

The Files.com client library can be installed on the command line using the ```dotnet``` command.  This method will retrieve a NuGet package
from the https://www.nuget.org/ repository and install it into your project.

To install the package:

```bash
dotnet add package FilesCom
```

Fetch the dependencies:

```bash
dotnet restore
```

#### Visual Studio Installation

The Files.com client library can also be installed using Visual Studio and the NuGet package manager.  In Visual Studio, click on "Manage Nuget Packages"
under the "Project" menu.  On the "Browse" tab search for "FilesCom", and click on the FilesCom NuGet package.  In the right panel, click the "Install"
button next to the version dropdown.

##### Visual Studio Requirements

Installation of the FilesCom Nuget package is fully supported by Visual Studio 2022, 2019, and 2017 version 15.3 or later.

For Visual Studio 2015, the following must be installed:

* [Nuget Package manager for Visual Studio 2015 version 3.6 or higher](https://www.nuget.org/downloads)
* [.NET Standard Support for Visual Studio 2015](https://aka.ms/netstandard-build-support-netfx)

Explore the [files-sdk-dotnet](https://github.com/Files-com/files-sdk-dotnet) code on GitHub.

### Getting Support

The Files.com Support team provides official support for all of our official Files.com integration tools.

To initiate a support conversation, you can send an [Authenticated Support Request](https://www.files.com/docs/overview/requesting-support) or simply send an E-Mail to support@files.com.

## Authentication

There are two ways to authenticate: API Key authentication and Session-based authentication.

### Authenticate with an API Key

Authenticating with an API key is the recommended authentication method for most scenarios, and is
the method used in the examples on this site.

To use an API Key, first generate an API key from the [web
interface](https://www.files.com/docs/sdk-and-apis/api-keys) or [via the API or an
SDK](/net/resources/developers/api-keys).

Note that when using a user-specific API key, if the user is an administrator, you will have full
access to the entire API. If the user is not an administrator, you will only be able to access files
that user can access, and no access will be granted to site administration functions in the API.

```csharp title="Example Request"
using FilesCom;
using FilesCom.Models;

// Using manual configuration
var config = new FilesConfiguration();
config.ApiKey = "YOUR_API_KEY";

// ...as the default client, which static methods such as User.Find use
new FilesClient(config);

// ...or as an independent client, used through its properties such as client.Users
FilesClient client = FilesClient.Create(config);

// In app.config
<configSections>
    <sectionGroup name="files.com">
        <section
            name="filesConfiguration"
            type="Files.FilesConfiguration, Files.com"
        />
    </sectionGroup>
</configSections>
<files.com>
    <filesConfiguration ApiKey="YOUR_API_KEY" />
</files.com>

new FilesClient();

// You may also specify the API key on a per-request basis in the options parameter.
var options = new Dictionary<string, object>();
options.Add("api_key", "YOUR_API_KEY");

try
{
    User user = await User.Find(id, null, options);
    User sameUser = await client.Users.FindAsync(id, null, options);
}
catch (FilesCom.NotAuthenticatedException e)
{
    Console.WriteLine($"Authentication Error Occurred ({e.GetType().Name}): " + e.Message);
}
catch (FilesCom.SdkException e)
{
    Console.WriteLine($"Unknown Error Occurred ({e.GetType().Name}): " + e.Message);
}
```

Don't forget to replace the placeholder, `YOUR_API_KEY`, with your actual API key.

### Authenticate with a Session

You can also authenticate by creating a user session using the username and
password of an active user. If the user is an administrator, the session will have full access to
all capabilities of Files.com. Sessions created from regular user accounts will only be able to access files that
user can access, and no access will be granted to site administration functions.

Sessions use the exact same session timeout settings as web interface sessions. When a
session times out, simply create a new session and resume where you left off. This process is not
automatically handled by our SDKs because we do not want to store password information in memory without
your explicit consent.

#### Logging In

To create a session, the `create` method is called on the `Session` object with the user's username and
password.

This returns a session object that can be used to authenticate SDK method calls.
To create a session, the `create` method is called on the `Session` object with the user's username and
password.

```csharp title="Example Request"
using FilesCom;
using FilesCom.Models;

Dictionary<string, object> paramsDict = new Dictionary<string, object>();
FilesClient client = FilesClient.Create(filesConfig);
paramsDict.Add("username", "username");
paramsDict.Add("password", "password");

try
{
    Session session = await client.Sessions.CreateAsync(paramsDict);
}
catch (FilesCom.NotAuthenticatedException e)
{
    Console.WriteLine($"Authentication Error Occurred ({e.GetType().Name}): " + e.Message);
}
catch (FilesCom.SdkException e)
{
    Console.WriteLine($"Unknown Error Occurred ({e.GetType().Name}): " + e.Message);
}
```

#### Using a Session

Once a session has been created, you can store the session globally, use the session per object, or use the session per request to authenticate SDK operations.

```csharp title="Example Request"
using FilesCom;
using FilesCom.Models;

// You may make a client that uses the returned session for all of its requests,
// or pass the configuration to new FilesClient to make it the default for static methods.
FilesConfiguration sessionConfig = new FilesConfiguration();
sessionConfig.SessionId = session.Id;
FilesClient sessionClient = FilesClient.Create(sessionConfig);

// Alternatively, you can specify the session ID on a per-object basis
// in the second parameter to a model constructor.
Dictionary<string, object> optionsDict = new Dictionary<string, object>();
optionsDict.Add("session_id", session.Id);
User user = new User(new Dictionary<string, object>(), optionsDict);

// You may also specify the session ID on a per-request basis in the options parameter.
try
{
    await sessionClient.Folders.ListFor("/").AllAsync();
    await client.Folders.ListFor("/", null, optionsDict).AllAsync();
}
catch (FilesCom.NotAuthenticatedException e)
{
    Console.WriteLine($"Authentication Error Occurred ({e.GetType().Name}): " + e.Message);
}
catch (FilesCom.SdkException e)
{
    Console.WriteLine($"Unknown Error Occurred ({e.GetType().Name}): " + e.Message);
}
```

#### Logging Out

User sessions can be ended calling the `destroy` method on the `session` object.

```csharp title="Example Request"
using FilesCom.Models;

try
{
    await Session.Destroy();
}
catch (FilesCom.NotAuthenticatedException e)
{
    Console.WriteLine($"Authentication Error Occurred ({e.GetType().Name}): " + e.Message);
}
catch (FilesCom.SdkException e)
{
    Console.WriteLine($"Unknown Error Occurred ({e.GetType().Name}): " + e.Message);
}
```

## Configuration

### Clients

A `FilesClient` connects to one Files.com site with one set of credentials. `FilesClient.Create(config)` makes an
independent client. Its properties, such as `client.Users` or `client.RemoteFiles`, run operations with that
client, and the objects and lists they return keep using it. Make one client for each site or user you work
with; clients can be used at the same time.

`Create` keeps a copy of the configuration, so later changes to `config` do not affect the client. Create a
client once and reuse it: each client has its own connection pool.

`new FilesClient(config)` makes the default client, which static methods such as `User.Find(id)` use. Making
another replaces the default for later static calls, but an object keeps the client it came from. A client made
this way reads `config` as each operation starts, so changes you make apply to operations started afterward.

```csharp title="Example Request"
using FilesCom;
using FilesCom.Models;

var config = new FilesConfiguration();
config.BaseUrl = "https://MY-SUBDOMAIN.files.com";
config.ApiKey = "YOUR_API_KEY";

FilesClient client = FilesClient.Create(config);

try
{
    User user = await client.Users.FindAsync(id);
    await user.UpdateAsync(new Dictionary<string, object> { { "name", "New Name" } });
}
catch (FilesCom.NotAuthenticatedException e)
{
    Console.WriteLine($"Authentication Error Occurred ({e.GetType().Name}): " + e.Message);
}
catch (FilesCom.SdkException e)
{
    Console.WriteLine($"Unknown Error Occurred ({e.GetType().Name}): " + e.Message);
}
```

### Configuration Options

#### Base URL

Set this to the full https:// URL of your Files.com subdomain (e.g. `https://MY-SUBDOMAIN.files.com`).
This is not required in most cases, but one benefit of setting it is that it ensures that authentication failures will be logged to your site's API logs.  Without setting this, we won't know which site to associate the authentication failure with, and it won't be logged to your site's API logs.
This is always required if your site is configured to disable global acceleration.
This can also be set to use a mock server in development or CI.

```csharp title="Example setting"
using FilesCom;

var config = new FilesConfiguration();
config.BaseUrl = "https://MY-SUBDOMAIN.files.com";
```

#### Connect Timeout

Connect timeout in seconds. The default value is 30.

```csharp title="Example setting"
using FilesCom;

var config = new FilesConfiguration();
config.ConnectTimeout = 60;
```

#### Read Timeout

Read timeout in seconds. The default value is 60.

```csharp title="Example setting"
using FilesCom;

var config = new FilesConfiguration();
config.ReadTimeout = 90;
```

#### Initial Network Retry Delay

Initial retry delay in seconds. The default value is 0.5.

```csharp title="Example setting"
using FilesCom;

var config = new FilesConfiguration();
config.InitialNetworkRequestDelay = 1;
```

#### Maximum Network Retries

Maximum number of retries. The default value is 3.

```csharp title="Example setting"
using FilesCom;

var config = new FilesConfiguration();
config.MaxNetworkRetries = 5;
```

#### Maximum Retry Delay

Maximum network retry delay in seconds. The default value is 2.

```csharp title="Example setting"
using FilesCom;

var config = new FilesConfiguration();
config.MaxNetworkRetryDelay = 5;
```

## Sort and Filter

Several of the Files.com API resources have list operations that return multiple instances of the
resource. The List operations can be sorted and filtered.

### Sorting

To sort the returned data, pass in the ```sort_by``` method argument.

Each resource supports a unique set of valid sort fields and can only be sorted by one field at a
time.

The argument value is a C# ```Dictionary<string, string>``` object that has a property of the
resource field name sort on and a value of either ```"asc"``` or ```"desc"``` to specify the sort
order.

#### Special note about the List Folder Endpoint

For historical reasons, and to maintain compatibility
with a variety of other cloud-based MFT and EFSS services, Folders will always be listed before Files
when listing a Folder.  This applies regardless of the sorting parameters you provide.  These *will* be
used, after the initial sort application of Folders before Files.

```csharp title="Sort Example"
using FilesCom;
using FilesCom.Models;

// users sorted by username
var args = new Dictionary<string, object>();
var sortArgs = new Dictionary<string, string>();
sortArgs.Add("username", "asc");
args.Add("sort_by", sortArgs);

try
{
    var userIterator = User.List(args);
    foreach (User user in userIterator.ListAutoPaging()) {
        // Operate on user
    }
}
catch (FilesCom.NotAuthenticatedException e)
{
    Console.WriteLine($"Authentication Error Occurred ({e.GetType().Name}): " + e.Message);
}
catch (FilesCom.SdkException e)
{
    Console.WriteLine($"Unknown Error Occurred ({e.GetType().Name}): " + e.Message);
}
```

### Filtering

Filters apply selection criteria to the underlying query that returns the results. They can be
applied individually or combined with other filters, and the resulting data can be sorted by a
single field.

Each resource supports a unique set of valid filter fields, filter combinations, and combinations of
filters and sort fields.

The passed in argument value is a C# ```Dictionary<string, string>``` object that has a property of
the resource field name to filter on and a passed in value to use in the filter comparison.

#### Filter Types

| Filter | Type | Description |
| --------- | --------- | --------- |
| `filter` | Exact | Find resources that have an exact field value match to a passed in value. (i.e., FIELD_VALUE = PASS_IN_VALUE). |
| `filter_prefix` | Pattern | Find resources where the specified field is prefixed by the supplied value. This is applicable to values that are strings. |
| `filter_gt` | Range | Find resources that have a field value that is greater than the passed in value.  (i.e., FIELD_VALUE > PASS_IN_VALUE). |
| `filter_gteq` | Range | Find resources that have a field value that is greater than or equal to the passed in value.  (i.e., FIELD_VALUE >=  PASS_IN_VALUE). |
| `filter_lt` | Range | Find resources that have a field value that is less than the passed in value.  (i.e., FIELD_VALUE < PASS_IN_VALUE). |
| `filter_lteq` | Range | Find resources that have a field value that is less than or equal to the passed in value.  (i.e., FIELD_VALUE \<= PASS_IN_VALUE). |

```csharp title="Exact Filter Example"
using FilesCom.Models;

// non admin users
var args = new Dictionary<string, object>();
var filterArgs = new Dictionary<string, string>();
filterArgs.Add("not_site_admin", "true");
args.Add("filter", filterArgs);

try
{
    var userIterator = User.List(args);
    foreach (User user in userIterator.ListAutoPaging()) {
        // Operate on user
    }
}
catch (FilesCom.NotAuthenticatedException e)
{
    Console.WriteLine($"Authentication Error Occurred ({e.GetType().Name}): " + e.Message);
}
catch (FilesCom.SdkException e)
{
    Console.WriteLine($"Unknown Error Occurred ({e.GetType().Name}): " + e.Message);
}
```

```csharp  title="Range Filter Example"
using FilesCom.Models;

// users who haven't logged in since 2024-01-01
var args = new Dictionary<string, object>();
var filterArgs = new Dictionary<string, string>();
filterArgs.Add("last_login_at","2024-01-01");
args.Add("filter_gteq", filterArgs);

try
{
    var userIterator = User.List(args);
    foreach (User user in userIterator.ListAutoPaging()) {
        // Operate on user
    }
}
catch (FilesCom.NotAuthenticatedException e)
{
    Console.WriteLine($"Authentication Error Occurred ({e.GetType().Name}): " + e.Message);
}
catch (FilesCom.SdkException e)
{
    Console.WriteLine($"Unknown Error Occurred ({e.GetType().Name}): " + e.Message);
}
```

```csharp  title="Pattern Filter Example"
using FilesCom.Models;

// users whose usernames start with 'test'
var args = new Dictionary<string, object>();
var filterArgs = new Dictionary<string, string>();
filterArgs.Add("username","test");
args.Add("filter_prefix", filterArgs);

try
{
    var userIterator = User.List(args);
    foreach (User user in userIterator.ListAutoPaging()) {
        // Operate on user
    }
}
catch (FilesCom.NotAuthenticatedException e)
{
    Console.WriteLine($"Authentication Error Occurred ({e.GetType().Name}): " + e.Message);
}
catch (FilesCom.SdkException e)
{
    Console.WriteLine($"Unknown Error Occurred ({e.GetType().Name}): " + e.Message);
}
```

```csharp s title="Combination Filter with Sort Example"
using FilesCom.Models;

// users whose usernames start with 'test' and are not admins
var args = new Dictionary<string, object>();
var filterPrefixArgs = new Dictionary<string, string>();
var filterArgs = new Dictionary<string, string>();
var sortArgs = new Dictionary<string, string>();
filterPrefixArgs.Add("username","test");
filterArgs.Add("not_site_admin", "true");
sortArgs.Add("username", "asc");
args.Add("filter_prefix", filterPrefixArgs);
args.Add("filter", filterArgs);
args.Add("sort_by", sortArgs);

try
{
    var userIterator = User.List(args);
    foreach (User user in userIterator.ListAutoPaging()) {
        // Operate on user
    }
}
catch (FilesCom.NotAuthenticatedException e)
{
    Console.WriteLine($"Authentication Error Occurred ({e.GetType().Name}): " + e.Message);
}
catch (FilesCom.SdkException e)
{
    Console.WriteLine($"Unknown Error Occurred ({e.GetType().Name}): " + e.Message);
}
```

## Paths

Files.com preserves the spelling of file and folder paths while comparing them using shared case and Unicode rules. Use the SDK comparison helpers when matching paths locally.
<div></div>

### Capitalization

Files.com uses case-insensitive path matching based on its fixed Unicode comparison map.

For example, the following paths have the same comparison key:

| Path Variant                          | Comparison Key              |
|---------------------------------------|------------------------------|
| `Documents/Reports/Q1.pdf`            | `documents/reports/q1.pdf`  |
| `documents/reports/q1.PDF`            | `documents/reports/q1.pdf`  |
| `DOCUMENTS/REPORTS/Q1.PDF`            | `documents/reports/q1.pdf`  |

This behavior applies across:
- API requests
- Folder and file lookup operations
- Automations and workflows

See also: [Case Sensitivity Documentation](https://www.files.com/docs/files-and-folders/case-sensitivity/)

The `PathUtil.same` function in the Files.com SDK is designed to help you determine if two paths on
your native file system would be considered the same on Files.com. This is particularly important
when handling errors related to duplicate file names and when developing tools for folder
synchronization.

```csharp title="Compare Case-Insensitive Files and Paths"
using FilesCom.Util;

if(PathUtil.same("Fïłèńämê.Txt", "filename.txt")) {
    Console.WriteLine("Paths are the same");
}
```

### Slashes

Use `/` between folder and file names, without leading or trailing slashes. SDK normalization helpers convert backslashes to `/`, remove duplicate separators, and discard exact `.` and `..` components. Discarding `..` leaves the preceding folder name intact.

| Input | Normalized path |
|-------|-----------------|
| `folder/subfolder/file.txt` | `folder/subfolder/file.txt` |
| `/folder/subfolder/file.txt` | `folder/subfolder/file.txt` |
| `folder/subfolder/file.txt/` | `folder/subfolder/file.txt` |
| `//folder//file.txt` | `folder/file.txt` |
| `folder/../file.txt` | `folder/file.txt` |

<div></div>

### Unicode and Path Comparison

Files.com compares paths using a fixed mapping shared by the server and SDKs. It treats case and many accent differences as equivalent: `Résumé.txt` and `resume.txt` identify the same file, as do `q` followed by a combining acute accent and `q`. The mapping also handles other equivalences, such as Hiragana and Katakana. Lowercasing or applying a standard Unicode normalization form alone does not reproduce these rules.

SDK comparison helpers normalize path separators and dot segments, then apply the bundled [versioned comparison map](https://github.com/Files-com/files-sdk-javascript/blob/master/shared/path_comparison.json). The [shared examples](https://github.com/Files-com/files-sdk-javascript/blob/master/shared/comparison_examples.json) give exact comparison results for integrations that implement their own matching. The map uses hexadecimal Unicode scalar values as keys: a missing entry preserves the character, an empty replacement removes it, and other replacements may contain several characters. Apply each replacement once without normalizing or lowercasing the result again.

Use comparison results only for matching. Send the original path spelling in API requests and preserve it for display and local filenames; comparison results can have a different spelling or length.

Trailing whitespace is significant for comparison. `report.txt` and `report.txt ` are different file paths, and SDK helpers preserve spaces, tabs, and newlines. Folder names cannot end in whitespace. See [Unicode Normalization](https://www.files.com/docs/files-and-folders/file-system-semantics/unicode-normalization) for the complete path rules.

<div></div>

## Workspaces

A Workspace groups files, users, groups, Partners, integrations, and workflows within a Files.com Site. An integration can provision a Workspace for a department or project and delegate its operation to a team without making that team Site Administrators. Every Site has a Default Workspace, with ID `0`; additional Workspaces have their own IDs and root folders.

Account membership, request context, and permission grants serve different purposes. Creating an account in a Workspace determines where it belongs. Selecting a Workspace determines which resources a request operates on. A permission grant determines what the caller can do there. Selecting a Workspace never grants access to it.

### Accounts and Administrative Access

A user's or group's `workspace_id` identifies the Workspace the account belongs to. Accounts belonging to a Custom Workspace stay within it. Default Workspace users and groups can receive permissions in one or more Custom Workspaces while keeping their existing accounts in Workspace `0`.

| Account | Workspace Administrator assignment | Scope |
| --- | --- | --- |
| User belonging to a Custom Workspace | Set the user's `workspace_admin` to `true`. | That user's own Custom Workspace. |
| Default Workspace user | Create an `admin` Permission for the user on a Custom Workspace's root folder. | Each Custom Workspace with a root grant. |
| Default Workspace group | Create an `admin` Permission for the group on a Custom Workspace's root folder. | Every member inherits administration of each Workspace with a root grant. |

`workspace_admin` is not a summary of a user's effective administrative access. A Default Workspace user can administer a Custom Workspace through a direct or group root grant while their `workspace_admin` remains `false`. Groups have no `workspace_admin` field. See [Users](/net/resources/user-accounts/users) and [Groups](/net/resources/user-accounts/groups) for account fields.

An `admin` grant on the **Custom Workspace root** provides full Workspace Administrator authority over its files, users, groups, Partners, workflows, and integrations. An `admin` grant on a subfolder provides Folder Admin authority over that folder and its descendants; it does not provide Workspace administration. Other permission levels provide their corresponding folder access without Workspace administration. [Permissions](/net/resources/user-accounts/permissions) defines the levels.

Site Administrators manage cross-Workspace assignments to Default Workspace accounts. Workspace Administrators manage accounts and permissions within their own scope. Site Administrators retain access to every Workspace; adding a Workspace grant does not narrow Site Administrator authority. The [product documentation](https://www.files.com/docs/workspaces/workspace-administrators) explains the role's operational scope and site-wide controls.

### Request Context and API Keys

The REST header `X-Files-Workspace-Id` selects a Workspace for a request. SDK request options and CLI configuration send that same selection. Workspace-scoped resources are listed, created, and changed within the selected context, and ordinary paths are relative to its root.

A resource's `workspace_id` request field describes the resource's Workspace membership. It is separate from the SDK's Workspace request option or REST header. Creating a Workspace-scoped resource in a Custom Workspace defaults its `workspace_id` to the selected Workspace; a mismatching membership value is rejected with `not-authorized/insufficient-permission-for-params`.

Selecting another Workspace with an API key requires a **Full Access key created in the Default Workspace**. A user key follows that user's current access, including group permissions. A site-wide Full Access key created in the Default Workspace has Site Administrator authority in every Workspace. A Files Only key stays in its creation Workspace, even if its user has cross-Workspace access. Any key created in a Custom Workspace stays within that Workspace. Selecting another context with these confined keys is rejected with `bad-request/invalid-workspace-id-header`.

An account belonging to a Custom Workspace is scoped there when it authenticates normally. For a Default Workspace user, explicitly select the intended Workspace for an integration rather than relying on an interactive login preference. [API Keys](/net/resources/developers/api-keys) and [Authentication](/net/overview/authentication) cover credentials.

The Files.com .NET SDK supports workspace scoping by using the `WorkspaceId` attribute on the `FilesConfiguration` object. Scope a single request by passing `workspace_id` in the request options.

The adjacent scoping example uses a credential authorized for the selected Workspace. A group member uses their own Full Access user key from the Default Workspace; the Site Administrator credential used to assign the grant is not needed for their day-to-day work.

```csharp title="Example Request"
using FilesCom;
using FilesCom.Models;
using System.Collections.Generic;

var config = new FilesConfiguration();
config.ApiKey = "YOUR_API_KEY";
config.WorkspaceId = "123";
new FilesClient(config);

Folder.ListFor("/", null, new Dictionary<string, object>
{
    { "workspace_id", 456 }
});
```

### Delegating a Workspace to an Existing Group

An operations team already represented by a Default Workspace group can administer a Custom Workspace through one root Permission. The group and its members stay in the Default Workspace, so the same team can receive different access in other Workspaces.

First retrieve the target [Workspace](/net/resources/settings/workspaces) and [Group](/net/resources/user-accounts/groups) IDs as a Site Administrator in Workspace `0`. The examples use Workspace `123`, group `456`, and member user `789`; replace them with your own IDs. Confirm that the group belongs to Workspace `0` and that the intended user is a member.

Create the Permission using a Default Workspace Full Access site-wide key or a Full Access user key belonging to a Site Administrator. Keep the request context at `0` and use the qualified root path `_/Workspaces/123`. Set `group_id` to the group's ID, `permission` to `admin`, and `recursive` to `true`. Save the returned Permission `id` for later removal. For an individual Default Workspace user, use `user_id` instead of `group_id`.

For a Default Workspace group, a Site Administrator can also select Workspace `123` and use an empty `path` to grant access to its root. The qualified path in Workspace `0` works for both Default Workspace users and groups and keeps the account scope and target Workspace explicit. Appending a subfolder to the path would grant Folder Admin access instead of Workspace Administrator authority.

After the grant, run the request-context example above with the member's own credential and Workspace `123` selected. That member can work with the Workspace's files and perform Workspace Administrator operations, such as managing its users, Partners, and integrations. A Site Administrator's successful request does not establish that the member has the intended access.

```csharp title="Grant group administration"
using FilesCom;
using System.Collections.Generic;

var admin = FilesClient.Create(new FilesConfiguration {
    ApiKey = "YOUR_SITE_ADMIN_API_KEY",
    WorkspaceId = "0"
});
var grant = await admin.Permissions.CreateAsync(new Dictionary<string, object> {
    { "path", "_/Workspaces/123" },
    { "group_id", 456L },
    { "permission", "admin" },
    { "recursive", true }
});
System.Console.WriteLine(grant.Id);
```

### Permission Inspection and Removal

List the member's Permissions with `user_id` and `include_groups=true` to include grants inherited through group membership. Listing only direct user grants can miss the Permission that provides Workspace administration. In Workspace `0`, the Custom Workspace root appears as `_/Workspaces/123`; in Workspace `123`, paths are relative to that root. Inspect the root path and `permission=admin`, rather than treating the user's `workspace_admin` field as their effective role.

Permission lists show grants, rather than a single effective-role boolean. Membership in several groups combines their access. A Permission using `group_ids` instead of `group_id` requires membership in all the specified groups; it is not a shorthand for assigning the same grant to several independent groups.

Removing a member ends access received through that group. Deleting the root Permission ends the group's Workspace Administrator grant for every member. These changes leave independent direct and other group grants in place, so review all applicable grants when withdrawing access. Default Workspace user API keys follow those permission changes without being recreated.

Group membership maintained through SCIM follows the same rule. A Group Admin allowed to add members can give those users the group's existing Workspace Administrator access. Choose who manages the group with that authority in mind.

Delete the Permission by its returned `id` as the Site Administrator in Workspace `0`. The removal examples use Permission ID `9001`; replace it with the ID returned by your create request. Permissions are created and deleted, rather than updated in place. If narrower folder access is still needed, assign it explicitly; deleting a broad grant does not restore narrower grants it previously replaced.

```csharp title="Inspect member grants and remove the group grant"
var grants = await admin.Permissions.List(new Dictionary<string, object> {
    { "user_id", "789" }, { "include_groups", true }
}).AllAsync();
foreach (var item in grants) {
    System.Console.WriteLine($"{item.Path}: {item.PermissionType}");
}
await admin.Permissions.DeleteAsync(9001L);
```

## Foreign Language Support

The Files.com .Net SDK supports localized responses by using the `Language` attribute on the `FilesConfiguration` object.
When configured, this guides the API in selecting a preferred language for applicable response content.

Language support currently applies to select human-facing fields only, such as notification messages
and error descriptions.

If the specified language is not supported or the value is omitted, the API defaults to English.

```shell title="Example Request"
using FilesCom;

var config = new FilesConfiguration();
config.Language = "es";
```

## Errors

The Files.com DotNet SDK will return errors by raising exceptions. There are many exception classes defined in the Files SDK that correspond
to specific errors.

The raised exceptions come from two categories:

1.  SDK Exceptions - errors that originate within the SDK
2.  API Exceptions - errors that occur due to the response from the Files.com API.  These errors are grouped into common error types.

There are several types of exceptions within each category.  Exception classes indicate different types of errors and are named in a
fashion that describe the general premise of the originating error.  More details can be found in the exception object message using the
`Message` attribute.

Use standard DotNet exception handling to detect and deal with errors.  It is generally recommended to catch specific errors first, then
catch the general `SdkException` exception as a catch-all.

```csharp title="Example Error Handling"
using FilesCom.Models;

Dictionary<string, object> paramsDict = new Dictionary<string, object>();
paramsDict.Add("username", "USERNAME");
paramsDict.Add("password", "BADPASSWORD");
Session session;

try
{
    session = await Session.Create(paramsDict);
}
catch (FilesCom.NotAuthenticatedException e)
{
    Console.WriteLine($"Authentication Error Occurred ({e.GetType().Name}): " + e.Message);
}
catch (FilesCom.SdkException e)
{
    Console.WriteLine($"Unknown Error Occurred ({e.GetType().Name}): " + e.Message);
}
```

### Error Types

#### SDK Errors

SDK errors are general errors that occur within the SDK code.  These errors generate exceptions.  Each of these
exception classes inherit from a standard `SdkException` base class.

```shell title="Example SDK Exception Class Inheritance Structure"
FilesCom.ApiConnectException ->
FilesCom.SdkException ->
Exception
```
##### SDK Exception Classes

| Exception Class Name| Description |
| --------------- | ------------ |
| `ApiConnectionException`| The Files.com API cannot be reached |
| `InvalidParameterException`| A passed in parameter is invalid |
| `NotImplementedException`| The called method has not be implemented by the SDK |
| `InvalidResponseException`| A bad formed response came back from the API |

#### API Errors

API errors are errors returned by the Files.com API.  Each exception class inherits from an error group base class.
The error group base class indicates a particular type of error.

```shell title="Example API Exception Class Inheritance Structure"
FilesCom.FolderAdminPermissionRequiredException ->
FilesCom.NotAuthorizedException ->
FilesCom.ApiException ->
FilesCom.SdkException ->
Exception
```
##### API Exception Classes

| Exception Class Name | Error Group |
| --------- | --------- |
|`AgentUpgradeRequiredException`|  `BadRequestException` |
|`AttachmentTooLargeException`|  `BadRequestException` |
|`CannotDownloadDirectoryException`|  `BadRequestException` |
|`CantMoveWithMultipleLocationsException`|  `BadRequestException` |
|`DatetimeParseException`|  `BadRequestException` |
|`DestinationSameException`|  `BadRequestException` |
|`DestinationSiteMismatchException`|  `BadRequestException` |
|`DoesNotSupportSortingException`|  `BadRequestException` |
|`FolderMustNotBeAFileException`|  `BadRequestException` |
|`FoldersNotAllowedException`|  `BadRequestException` |
|`InternalGeneralErrorException`|  `BadRequestException` |
|`InvalidBodyException`|  `BadRequestException` |
|`InvalidCursorException`|  `BadRequestException` |
|`InvalidCursorTypeForSortException`|  `BadRequestException` |
|`InvalidEtagsException`|  `BadRequestException` |
|`InvalidFilterAliasCombinationException`|  `BadRequestException` |
|`InvalidFilterFieldException`|  `BadRequestException` |
|`InvalidFilterParamException`|  `BadRequestException` |
|`InvalidFilterParamFormatException`|  `BadRequestException` |
|`InvalidFilterParamValueException`|  `BadRequestException` |
|`InvalidInputEncodingException`|  `BadRequestException` |
|`InvalidInterfaceException`|  `BadRequestException` |
|`InvalidOauthProviderException`|  `BadRequestException` |
|`InvalidPathException`|  `BadRequestException` |
|`InvalidReturnToUrlException`|  `BadRequestException` |
|`InvalidSearchQueryException`|  `BadRequestException` |
|`InvalidSortFieldException`|  `BadRequestException` |
|`InvalidSortFilterCombinationException`|  `BadRequestException` |
|`InvalidUploadOffsetException`|  `BadRequestException` |
|`InvalidUploadPartGapException`|  `BadRequestException` |
|`InvalidUploadPartSizeException`|  `BadRequestException` |
|`InvalidWorkspaceIdHeaderException`|  `BadRequestException` |
|`MethodNotAllowedException`|  `BadRequestException` |
|`MultipleSortParamsNotAllowedException`|  `BadRequestException` |
|`NoValidInputParamsException`|  `BadRequestException` |
|`OffsetUploadNotAllowedWithMalwareScanningException`|  `BadRequestException` |
|`PartNumberTooLargeException`|  `BadRequestException` |
|`PathCannotHaveTrailingWhitespaceException`|  `BadRequestException` |
|`ReauthenticationNeededFieldsException`|  `BadRequestException` |
|`RequestBodyTooLargeException`|  `BadRequestException` |
|`RequestParamsContainInvalidCharacterException`|  `BadRequestException` |
|`RequestParamsInvalidException`|  `BadRequestException` |
|`RequestParamsRequiredException`|  `BadRequestException` |
|`SearchAllOnChildPathException`|  `BadRequestException` |
|`UnrecognizedSortIndexException`|  `BadRequestException` |
|`UnsupportedCurrencyException`|  `BadRequestException` |
|`UnsupportedHttpResponseFormatException`|  `BadRequestException` |
|`UnsupportedMediaTypeException`|  `BadRequestException` |
|`UserIdInvalidException`|  `BadRequestException` |
|`UserIdOnUserEndpointException`|  `BadRequestException` |
|`UserRequiredException`|  `BadRequestException` |
|`AdditionalAuthenticationRequiredException`|  `NotAuthenticatedException` |
|`ApiKeySessionsNotSupportedException`|  `NotAuthenticatedException` |
|`AuthenticationRequiredException`|  `NotAuthenticatedException` |
|`BundleRegistrationCodeFailedException`|  `NotAuthenticatedException` |
|`InboxRegistrationCodeFailedException`|  `NotAuthenticatedException` |
|`InvalidCredentialsException`|  `NotAuthenticatedException` |
|`InvalidOauthException`|  `NotAuthenticatedException` |
|`InvalidOrExpiredCodeException`|  `NotAuthenticatedException` |
|`InvalidSessionException`|  `NotAuthenticatedException` |
|`InvalidUsernameOrPasswordException`|  `NotAuthenticatedException` |
|`LockedOutException`|  `NotAuthenticatedException` |
|`LockoutRegionMismatchException`|  `NotAuthenticatedException` |
|`OneTimePasswordIncorrectException`|  `NotAuthenticatedException` |
|`TwoFactorAuthenticationErrorException`|  `NotAuthenticatedException` |
|`TwoFactorAuthenticationSetupExpiredException`|  `NotAuthenticatedException` |
|`ApiKeyIsDisabledException`|  `NotAuthorizedException` |
|`ApiKeyIsPathRestrictedException`|  `NotAuthorizedException` |
|`ApiKeyOnlyForDesktopAppException`|  `NotAuthorizedException` |
|`ApiKeyOnlyForFileOperationsException`|  `NotAuthorizedException` |
|`ApiKeyOnlyForMobileAppException`|  `NotAuthorizedException` |
|`ApiKeyOnlyForOfficeIntegrationException`|  `NotAuthorizedException` |
|`BillingInformationHiddenException`|  `NotAuthorizedException` |
|`BillingPermissionRequiredException`|  `NotAuthorizedException` |
|`BundleMaximumUsesReachedException`|  `NotAuthorizedException` |
|`BundlePermissionRequiredException`|  `NotAuthorizedException` |
|`CannotAdministerHigherLevelUserException`|  `NotAuthorizedException` |
|`CannotLoginWhileUsingKeyException`|  `NotAuthorizedException` |
|`CantActForOtherUserException`|  `NotAuthorizedException` |
|`ContactAdminForPasswordChangeHelpException`|  `NotAuthorizedException` |
|`FilesAgentFailedAuthorizationException`|  `NotAuthorizedException` |
|`FolderAdminOrBillingPermissionRequiredException`|  `NotAuthorizedException` |
|`FolderAdminPermissionRequiredException`|  `NotAuthorizedException` |
|`FullPermissionRequiredException`|  `NotAuthorizedException` |
|`HistoryPermissionRequiredException`|  `NotAuthorizedException` |
|`InAppAiAssistantUnavailableException`|  `NotAuthorizedException` |
|`InsufficientPermissionForParamsException`|  `NotAuthorizedException` |
|`InsufficientPermissionForSiteException`|  `NotAuthorizedException` |
|`MoverAccessDeniedException`|  `NotAuthorizedException` |
|`MoverPackageRequiredException`|  `NotAuthorizedException` |
|`MustAuthenticateWithApiKeyException`|  `NotAuthorizedException` |
|`NeedAdminPermissionForInboxException`|  `NotAuthorizedException` |
|`NonAdminsMustQueryByFolderOrPathException`|  `NotAuthorizedException` |
|`NotAllowedToCreateBundleException`|  `NotAuthorizedException` |
|`NotEnqueuableSyncException`|  `NotAuthorizedException` |
|`PasswordChangeNotRequiredException`|  `NotAuthorizedException` |
|`PasswordChangeRequiredException`|  `NotAuthorizedException` |
|`PaymentMethodErrorException`|  `NotAuthorizedException` |
|`PreviewOnlyPermissionCannotDownloadException`|  `NotAuthorizedException` |
|`ReadOnlySessionException`|  `NotAuthorizedException` |
|`ReadPermissionRequiredException`|  `NotAuthorizedException` |
|`ReauthenticationFailedException`|  `NotAuthorizedException` |
|`ReauthenticationFailedFinalException`|  `NotAuthorizedException` |
|`ReauthenticationNeededActionException`|  `NotAuthorizedException` |
|`RecaptchaFailedException`|  `NotAuthorizedException` |
|`RemoteDesktopDebugLoggingDisabledException`|  `NotAuthorizedException` |
|`RootFolderBehaviorSiteAdminRequiredException`|  `NotAuthorizedException` |
|`RootFolderBehaviorSkipSiteAdminRequiredException`|  `NotAuthorizedException` |
|`SelfManagedRequiredException`|  `NotAuthorizedException` |
|`SiteAdminOrPartnerAdminPermissionRequiredException`|  `NotAuthorizedException` |
|`SiteAdminOrWorkspaceAdminOrFolderAdminPermissionRequiredException`|  `NotAuthorizedException` |
|`SiteAdminOrWorkspaceAdminOrPartnerAdminOrFolderAdminPermissionRequiredException`|  `NotAuthorizedException` |
|`SiteAdminOrWorkspaceAdminOrPartnerAdminPermissionRequiredException`|  `NotAuthorizedException` |
|`SiteAdminOrWorkspaceAdminPermissionRequiredException`|  `NotAuthorizedException` |
|`SiteAdminRequiredException`|  `NotAuthorizedException` |
|`SiteFilesAreImmutableException`|  `NotAuthorizedException` |
|`TwoFactorAuthenticationRequiredException`|  `NotAuthorizedException` |
|`UserIdWithoutSiteAdminException`|  `NotAuthorizedException` |
|`WriteAndBundlePermissionRequiredException`|  `NotAuthorizedException` |
|`WritePermissionRequiredException`|  `NotAuthorizedException` |
|`ApiKeyNotFoundException`|  `NotFoundException` |
|`BundlePathNotFoundException`|  `NotFoundException` |
|`BundleRegistrationNotFoundException`|  `NotFoundException` |
|`CodeNotFoundException`|  `NotFoundException` |
|`FileNotFoundException`|  `NotFoundException` |
|`FileUploadNotFoundException`|  `NotFoundException` |
|`GroupNotFoundException`|  `NotFoundException` |
|`InboxNotFoundException`|  `NotFoundException` |
|`NestedNotFoundException`|  `NotFoundException` |
|`PlanNotFoundException`|  `NotFoundException` |
|`SiteNotFoundException`|  `NotFoundException` |
|`UserNotFoundException`|  `NotFoundException` |
|`AgentPushUpdateBlockedException`|  `ProcessingFailureException` |
|`AgentUnavailableException`|  `ProcessingFailureException` |
|`AiTaskCannotBeRunManuallyException`|  `ProcessingFailureException` |
|`AlreadyCompletedException`|  `ProcessingFailureException` |
|`AutomationCannotBeRunManuallyException`|  `ProcessingFailureException` |
|`BehaviorNotAllowedOnRemoteServerException`|  `ProcessingFailureException` |
|`BufferedUploadDisabledForThisDestinationException`|  `ProcessingFailureException` |
|`BundleOnlyAllowsPreviewsException`|  `ProcessingFailureException` |
|`BundleOperationRequiresSubfolderException`|  `ProcessingFailureException` |
|`ConfigurationLockedPathException`|  `ProcessingFailureException` |
|`CouldNotCreateParentException`|  `ProcessingFailureException` |
|`DestinationExistsException`|  `ProcessingFailureException` |
|`DestinationFolderLimitedException`|  `ProcessingFailureException` |
|`DestinationParentConflictException`|  `ProcessingFailureException` |
|`DestinationParentDoesNotExistException`|  `ProcessingFailureException` |
|`ExceededRuntimeLimitException`|  `ProcessingFailureException` |
|`ExpectationAlreadyHasOpenWindowException`|  `ProcessingFailureException` |
|`ExpectationNotManualTriggerException`|  `ProcessingFailureException` |
|`ExpiredPrivateKeyException`|  `ProcessingFailureException` |
|`ExpiredPublicKeyException`|  `ProcessingFailureException` |
|`ExportFailureException`|  `ProcessingFailureException` |
|`ExportNotReadyException`|  `ProcessingFailureException` |
|`FailedToChangePasswordException`|  `ProcessingFailureException` |
|`FileLockedException`|  `ProcessingFailureException` |
|`FileNotUploadedException`|  `ProcessingFailureException` |
|`FilePendingProcessingException`|  `ProcessingFailureException` |
|`FileProcessingErrorException`|  `ProcessingFailureException` |
|`FileTooBigToDecryptException`|  `ProcessingFailureException` |
|`FileTooBigToEncryptException`|  `ProcessingFailureException` |
|`FileUploadedToWrongRegionException`|  `ProcessingFailureException` |
|`FilenameTooLongException`|  `ProcessingFailureException` |
|`FolderLockedException`|  `ProcessingFailureException` |
|`FolderNotEmptyException`|  `ProcessingFailureException` |
|`HistoryUnavailableException`|  `ProcessingFailureException` |
|`InvalidBundleCodeException`|  `ProcessingFailureException` |
|`InvalidFileTypeException`|  `ProcessingFailureException` |
|`InvalidFilenameException`|  `ProcessingFailureException` |
|`InvalidPriorityColorException`|  `ProcessingFailureException` |
|`InvalidRangeException`|  `ProcessingFailureException` |
|`InvalidSiteException`|  `ProcessingFailureException` |
|`InvalidZipFileException`|  `ProcessingFailureException` |
|`MetadataNotSupportedOnRemotesException`|  `ProcessingFailureException` |
|`ModelSaveErrorException`|  `ProcessingFailureException` |
|`MultipleProcessingErrorsException`|  `ProcessingFailureException` |
|`PathTooLongException`|  `ProcessingFailureException` |
|`RecipientAlreadySharedException`|  `ProcessingFailureException` |
|`RemoteEntryReadOnlyException`|  `ProcessingFailureException` |
|`RemoteServerErrorException`|  `ProcessingFailureException` |
|`ResourceBelongsToParentSiteException`|  `ProcessingFailureException` |
|`ResourceLockedException`|  `ProcessingFailureException` |
|`SubfolderLockedException`|  `ProcessingFailureException` |
|`SyncInProgressException`|  `ProcessingFailureException` |
|`TwoFactorAuthenticationCodeAlreadySentException`|  `ProcessingFailureException` |
|`TwoFactorAuthenticationCountryBlacklistedException`|  `ProcessingFailureException` |
|`TwoFactorAuthenticationGeneralErrorException`|  `ProcessingFailureException` |
|`TwoFactorAuthenticationMethodUnsupportedErrorException`|  `ProcessingFailureException` |
|`TwoFactorAuthenticationUnsubscribedRecipientException`|  `ProcessingFailureException` |
|`UpdatesNotAllowedForRemotesException`|  `ProcessingFailureException` |
|`DuplicateShareRecipientException`|  `RateLimitedException` |
|`ReauthenticationRateLimitedException`|  `RateLimitedException` |
|`TooManyConcurrentLoginsException`|  `RateLimitedException` |
|`TooManyConcurrentRequestsException`|  `RateLimitedException` |
|`TooManyLoginAttemptsException`|  `RateLimitedException` |
|`TooManyRequestsException`|  `RateLimitedException` |
|`TooManySharesException`|  `RateLimitedException` |
|`AutomationsUnavailableException`|  `ServiceUnavailableException` |
|`MigrationInProgressException`|  `ServiceUnavailableException` |
|`SearchUnavailableException`|  `ServiceUnavailableException` |
|`SiteDisabledException`|  `ServiceUnavailableException` |
|`UploadsUnavailableException`|  `ServiceUnavailableException` |
|`AccountAlreadyExistsException`|  `SiteConfigurationException` |
|`AccountOverdueException`|  `SiteConfigurationException` |
|`NoAccountForSiteException`|  `SiteConfigurationException` |
|`SiteWasRemovedException`|  `SiteConfigurationException` |
|`TrialExpiredException`|  `SiteConfigurationException` |
|`TrialLockedException`|  `SiteConfigurationException` |
|`UserRequestsEnabledRequiredException`|  `SiteConfigurationException` |

## Pagination

Certain API operations return lists of objects. When the number of objects in the list is large,
the API will paginate the results.

The Files.com DotNet SDK provides multiple ways to paginate through lists of objects.

Creating a list, such as `client.Folders.ListFor("/")`, sends no request; pages are requested as you
load them. Every page comes from the client the list was made with. Lists made with static methods,
such as `Folder.ListFor("/")`, use the default client.

### Automatic Pagination

The `ListAutoPaging` method automatically paginates and loads each page into memory.

Pass a `CancellationToken` to stop before the next page is loaded. With a token, failures are thrown as they
are, such as `NotFoundException` or `OperationCanceledException`. Without one, they are wrapped in an
`AggregateException`.

```csharp title="Example Request"
using FilesCom.Models;

try
{
    foreach (var file in client.Folders.ListFor("/").ListAutoPaging(cancellationToken))
    {
        Console.WriteLine("- Path: {0}", file.Path);
    }
}
catch (FilesCom.NotAuthenticatedException e)
{
    Console.WriteLine($"Authentication Error Occurred ({e.GetType().Name}): " + e.Message);
}
catch (FilesCom.SdkException e)
{
    Console.WriteLine($"Unknown Error Occurred ({e.GetType().Name}): " + e.Message);
}
```

### Manual Pagination

The `LoadNextPageAsync/HasNextPage` methods allow for manual pagination and loading of each page into memory.

```csharp title="Example Request"
using FilesCom.Models;

try
{
    FilesList<RemoteFile> listing = client.Folders.ListFor("/");
    do
    {
        foreach (var file in await listing.LoadNextPageAsync(cancellationToken))
        {
            Console.WriteLine("- Path: {0}", file.Path);
        }
    } while (listing.HasNextPage);
}
catch (FilesCom.NotAuthenticatedException e)
{
    Console.WriteLine($"Authentication Error Occurred ({e.GetType().Name}): " + e.Message);
}
catch (FilesCom.SdkException e)
{
    Console.WriteLine($"Unknown Error Occurred ({e.GetType().Name}): " + e.Message);
}
```

### Load All Items

The `AllAsync` method loads all items into memory.

```csharp title="Example Request"
using FilesCom.Models;

try
{
    var files = await client.Folders.ListFor("/").AllAsync(cancellationToken);
    foreach (var file in files)
    {
        Console.WriteLine("- Path: {0}", file.Path);
    }
}
catch (FilesCom.NotAuthenticatedException e)
{
    Console.WriteLine($"Authentication Error Occurred ({e.GetType().Name}): " + e.Message);
}
catch (FilesCom.SdkException e)
{
    Console.WriteLine($"Unknown Error Occurred ({e.GetType().Name}): " + e.Message);
}
```

## Cancellation

Every method that sends a request has an `Async` form that takes a `CancellationToken`: client operations such
as `client.Users.FindAsync`, object methods such as `user.UpdateAsync` and `SaveAsync`, the list methods
`LoadNextPageAsync`, `AllAsync` and `ListAutoPaging(cancellationToken)`, and the file transfers
`client.RemoteFiles.UploadFileAsync` and `DownloadFileAsync`.

Cancelling the token stops the operation wherever it is: waiting for a response, waiting to retry, reading or
writing file contents, or between pages and upload parts. The operation then ends with an
`OperationCanceledException` (or a `TaskCanceledException`, which derives from it), and it starts no further
request, retry, page or part.

The methods without a token, such as `User.Find` and `LoadNextPage`, work as before and cannot be cancelled.

```csharp title="Example Request"
using FilesCom;
using FilesCom.Models;

using (var cancellation = new CancellationTokenSource(TimeSpan.FromMinutes(10)))
{
    try
    {
        await client.RemoteFiles.UploadFileAsync(localPath, destinationPath, cancellationToken: cancellation.Token);
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("The upload was cancelled.");
    }
}
```

### What Cancellation Leaves Behind

Cancellation stops the SDK; it does not undo requests Files.com has already received. If the last request of
an upload was sent before you cancelled, the file may still be completed.

The SDK passes the token to the streams you give it. A stream that ignores its token finishes its current read
or write before the operation stops.

- `UploadFileAsync(destinationPath, stream, ...)` disposes the stream when it ends, including when it is cancelled.
- `DownloadFileAsync(path, stream)` leaves your stream open.
- `DownloadFileAsync(path, localPath)` closes the local file when it ends. After a failure or cancellation, the
  file may be partly written.

The `ReadTimeout` setting limits only the wait for a download's response headers. To limit a whole operation,
use a token that cancels itself, as in the example.

## Logs

To enable logging, create a file named `log4net.config` in the same directory as
the application with the contents as shown.

Then, in the application, use that file to configure `log4net`:

```csharp
log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("./log4net.config"));
```

```xml title="log4net.config"
<?xml version="1.0" encoding="UTF-8" ?>
<log4net>
  <root>
    <level value="ALL" />
    <appender-ref ref="console" />
  </root>
  <appender name="console" type="log4net.Appender.ConsoleAppender">
    <layout type="log4net.Layout.PatternLayout">
      <conversionPattern value="%date [%thread] %-5level %logger - %message%newline" />
    </layout>
  </appender>
</log4net>
```

## Mock Server

Files.com publishes a Files.com API server, which is useful for testing your use of the Files.com
SDKs and other direct integrations against the Files.com API in an integration test environment.

It is a Ruby app that operates as a minimal server for the purpose of testing basic network
operations and JSON encoding for your SDK or API client. It does not maintain state and it does not
deeply inspect your submissions for correctness.

Eventually we will add more features intended for integration testing, such as the ability to
intentionally provoke errors.

Download the server as a Docker image via [Docker Hub](https://hub.docker.com/r/filescom/files-mock-server).

The Source Code is also available on [GitHub](https://github.com/Files-com/files-mock-server).

A README is available on the GitHub link.
