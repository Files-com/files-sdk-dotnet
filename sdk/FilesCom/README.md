# Files.com .NET Client

The Files.com .NET client library provides convenient access to the Files.com API from applications using the .NET framework.

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

new FilesClient(config);

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

// You may also specify the API key on a per-request basis in the final parameter to static methods.
var options = new Dictionary<string, object>();
options.Add("api_key", "YOUR_API_KEY");

try
{
    User.Find(id, params, options);
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
FilesClient client = new FilesClient(filesConfig);
paramsDict.Add("username", "username");
paramsDict.Add("password", "password");

try
{
    Session session = await Session.Create(paramsDict);
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

// You may set the returned session to be used by default for subsequent requests.
FilesConfiguration filesConfig = new FilesConfiguration();
filesConfig.SessionId = session.Id;

// Alternatively, you can specify the session ID on a per-object basis
// in the second parameter to a model constructor.

user = new User(params, requestOptions);
Dictionary<string, object> paramsDict = new Dictionary<string, object>();
Dictionary<string, object> optionsDict = new Dictionary<string, object>();
optionsDict.Add("session_id", session.Id);
User user = new User(paramsDict, optionsDict);

// You may also specify the session ID on a per-request basis in the final parameter to static methods.
Dictionary<string, object> paramsDict = new Dictionary<string, object>();
Dictionary<string, object> optionsDict = new Dictionary<string, object>();
optionsDict.Add("session_id", session.Id);

try
{
    await Folder.ListFor("/", paramsDict, optionsDict).All();
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

### Configuration Options

#### Base URL

Setting the base URL for the API is required if your site is configured to disable global acceleration.
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
config.InitialNetworkRetryDelay = 1;
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

Working with paths in Files.com involves several important considerations. Understanding how path comparisons are applied helps developers ensure consistency and accuracy across all interactions with the platform.
<div></div>

### Capitalization

Files.com compares paths in a **case-insensitive** manner. This means path segments are treated as equivalent regardless of letter casing.

For example, all of the following resolve to the same internal path:

| Path Variant                          | Interpreted As              |
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

All path parameters in Files.com (API, SDKs, CLI, automations, integrations) must **omit leading and trailing slashes**. Paths are always treated as **absolute and slash-delimited**, so only internal `/` separators are used and never at the start or end of the string.

####  Path Slash Examples
| Path                              | Valid? | Notes                         |
|-----------------------------------|--------|-------------------------------|
| `folder/subfolder/file.txt`       |   ✅   | Correct, internal separators only |
| `/folder/subfolder/file.txt`      |   ❌   | Leading slash not allowed     |
| `folder/subfolder/file.txt/`      |   ❌   | Trailing slash not allowed    |
| `//folder//file.txt`              |   ❌   | Duplicate separators not allowed |

<div></div>

### Unicode Normalization

Files.com normalizes all paths using [Unicode NFC (Normalization Form C)](https://www.unicode.org/reports/tr15/#Norm_Forms) before comparison. This ensures consistency across different representations of the same characters.

For example, the following two paths are treated as equivalent after NFC normalization:

| Input                                  | Normalized Form       |
|----------------------------------------|------------------------|
| `uploads/\u0065\u0301.txt`             | `uploads/é.txt`        |
| `docs/Café/Report.txt`                 | `docs/Café/Report.txt` |

- All input must be UTF‑8 encoded.
- Precomposed and decomposed characters are unified.
- This affects search, deduplication, and comparisons across SDKs.

<div></div>

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
|`DoesNotSupportSortingException`|  `BadRequestException` |
|`FolderMustNotBeAFileException`|  `BadRequestException` |
|`FoldersNotAllowedException`|  `BadRequestException` |
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
|`InvalidSortFieldException`|  `BadRequestException` |
|`InvalidSortFilterCombinationException`|  `BadRequestException` |
|`InvalidUploadOffsetException`|  `BadRequestException` |
|`InvalidUploadPartGapException`|  `BadRequestException` |
|`InvalidUploadPartSizeException`|  `BadRequestException` |
|`MethodNotAllowedException`|  `BadRequestException` |
|`MultipleSortParamsNotAllowedException`|  `BadRequestException` |
|`NoValidInputParamsException`|  `BadRequestException` |
|`PartNumberTooLargeException`|  `BadRequestException` |
|`PathCannotHaveTrailingWhitespaceException`|  `BadRequestException` |
|`ReauthenticationNeededFieldsException`|  `BadRequestException` |
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
|`FilesAgentTokenFailedException`|  `NotAuthenticatedException` |
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
|`ApiKeyOnlyForMobileAppException`|  `NotAuthorizedException` |
|`ApiKeyOnlyForOfficeIntegrationException`|  `NotAuthorizedException` |
|`BillingOrSiteAdminPermissionRequiredException`|  `NotAuthorizedException` |
|`BillingPermissionRequiredException`|  `NotAuthorizedException` |
|`BundleMaximumUsesReachedException`|  `NotAuthorizedException` |
|`BundlePermissionRequiredException`|  `NotAuthorizedException` |
|`CannotLoginWhileUsingKeyException`|  `NotAuthorizedException` |
|`CantActForOtherUserException`|  `NotAuthorizedException` |
|`ContactAdminForPasswordChangeHelpException`|  `NotAuthorizedException` |
|`FilesAgentFailedAuthorizationException`|  `NotAuthorizedException` |
|`FolderAdminOrBillingPermissionRequiredException`|  `NotAuthorizedException` |
|`FolderAdminPermissionRequiredException`|  `NotAuthorizedException` |
|`FullPermissionRequiredException`|  `NotAuthorizedException` |
|`HistoryPermissionRequiredException`|  `NotAuthorizedException` |
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
|`ReadOnlySessionException`|  `NotAuthorizedException` |
|`ReadPermissionRequiredException`|  `NotAuthorizedException` |
|`ReauthenticationFailedException`|  `NotAuthorizedException` |
|`ReauthenticationFailedFinalException`|  `NotAuthorizedException` |
|`ReauthenticationNeededActionException`|  `NotAuthorizedException` |
|`RecaptchaFailedException`|  `NotAuthorizedException` |
|`SelfManagedRequiredException`|  `NotAuthorizedException` |
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
|`AgentUnavailableException`|  `ProcessingFailureException` |
|`AlreadyCompletedException`|  `ProcessingFailureException` |
|`AutomationCannotBeRunManuallyException`|  `ProcessingFailureException` |
|`BehaviorNotAllowedOnRemoteServerException`|  `ProcessingFailureException` |
|`BundleOnlyAllowsPreviewsException`|  `ProcessingFailureException` |
|`BundleOperationRequiresSubfolderException`|  `ProcessingFailureException` |
|`CouldNotCreateParentException`|  `ProcessingFailureException` |
|`DestinationExistsException`|  `ProcessingFailureException` |
|`DestinationFolderLimitedException`|  `ProcessingFailureException` |
|`DestinationParentConflictException`|  `ProcessingFailureException` |
|`DestinationParentDoesNotExistException`|  `ProcessingFailureException` |
|`ExceededRuntimeLimitException`|  `ProcessingFailureException` |
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
|`MetadataNotSupportedOnRemotesException`|  `ProcessingFailureException` |
|`ModelSaveErrorException`|  `ProcessingFailureException` |
|`MultipleProcessingErrorsException`|  `ProcessingFailureException` |
|`PathTooLongException`|  `ProcessingFailureException` |
|`RecipientAlreadySharedException`|  `ProcessingFailureException` |
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

### Automatic Pagination

The `ListAutoPaging` method automatically paginates and loads each page into memory.

```csharp title="Example Request"
using FilesCom.Models;

try
{
    foreach (var file in Folder.ListFor("/").ListAutoPaging())
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

The `LoadNextPage/HasNextPage` methods allow for manual pagination and loading of each page into memory.

```csharp title="Example Request"
using FilesCom.Models;

try
{
    FilesList<RemoteFile> listing = Folder.ListFor("/");
    do
    {
        foreach (var file in await listing.LoadNextPage())
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

The `All` method loads all items into memory.

```csharp title="Example Request"
using FilesCom.Models;

try
{
    var files = await Folder.ListFor("/").All();
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
