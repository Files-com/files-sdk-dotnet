﻿# Files.com .NET Client

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
````

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

### Authenticate with an API Key

Authenticating with an API key is the recommended authentication method for most scenarios, and is
the method used in the examples on this site.

To use the API or SDKs with an API Key, first generate an API key from the [web
interface](https://www.files.com/docs/sdk-and-apis/api-keys) or [via the API or an
SDK](/net/resources/developers/api-keys).

Note that when using a user-specific API key, if the user is an administrator, you will have full
access to the entire API. If the user is not an administrator, you will only be able to access files
that user can access, and no access will be granted to site administration functions in the API.

```csharp title="Example Request"
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
var options = Dictionary<string, object>();
options.Add("api_key", "YOUR_API_KEY");
User.Find(id, params, options);
```

Don't forget to replace the placeholder, `YOUR_API_KEY`, with your actual API key.

### Authenticate with a Session

You can also authenticate to the REST API or SDKs by creating a user session using the username and
password of an active user. If the user is an administrator, the session will have full access to
the entire API. Sessions created from regular user accounts will only be able to access files that
user can access, and no access will be granted to site administration functions.

API sessions use the exact same session timeout settings as web interface sessions. When an API
session times out, simply create a new session and resume where you left off. This process is not
automatically handled by SDKs because we do not want to store password information in memory without
your explicit consent.

#### Logging In

To create a session, the `create` method is called on the `Session` object with the user's username and
password.

This returns a session object that can be used to authenticate SDK method calls.
To create a session, the `create` method is called on the `Session` object with the user's username and
password.

```csharp title="Example Request"
Dictionary<string, object> paramsDict = new Dictionary<string, object>();
FilesClient client = new FilesClient(filesConfig);
paramsDict.Add("username", "username");
paramsDict.Add("password", "password");
Session session = await Session.Create(paramsDict);
```

#### Using a Session

Once a session has been created, you can store the session globally, use the session per object, or use the session per request to authenticate SDK operations.

```csharp title="Example Request"

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
await Folder.ListFor("/", paramsDict, optionsDict).All();

```

#### Logging Out

User sessions can be ended calling the `destroy` method on the `session` object.

```csharp title="Example Request"
await Session.Destroy();
```

## Configuration

### Configuration Options

#### Base URL

Setting the base URL for the API is required if your site is configured to disable global acceleration.
This can also be set to use a mock server in development or CI.

```csharp title="Example setting"
config.BaseUrl = "https://MY-SUBDOMAIN.files.com"
```

#### Open Timeout

Open timeout in seconds. The default value is 30.

```csharp title="Example setting"
config.OpenTimeout = 60
```

#### Read Timeout

Read timeout in seconds. The default value is 80.

```csharp title="Example setting"
config.ReadTimeout = 90
```

#### Initial Network Retry Delay

Initial retry delay in seconds. The default value is 0.5.

```csharp title="Example setting"
config.InitialNetworkRetryDelay = 1
```

#### Maximum Network Retries

Maximum number of retries. The default value is 3.

```csharp title="Example setting"
config.MaxNetworkRetries = 5
```

#### Maximum Retry Delay

Maximum network retry delay in seconds. The default value is 2.

```csharp title="Example setting"
config.MaxNetworkRetryDelay = 5
```

## Sort and Filter

Several of the Files.com API resources have list operations that return multiple instances of the resource.  The List operations
can be sorted and filtered.

### Sorting

The returned data can be sorted by passing in the ```sort_by``` method argument.

Each resource has a set of valid fields for sorting and can be sorted by one field at a time.

The argument value is a C# ```Dictionary<string, object>``` object that has a property of the resource field name sort on and a value of either ```"asc"``` or ```"desc"``` to specify the sort order.

### Filters

Filters apply selection criteria to the underlying query that returns the results. Filters can be applied individually to select resource fields
and/or in a combination with each other.  The results of applying filters and filter combinations can be sorted by a single field.

The passed in argument value is a C# ```Dictionary<string, object>``` object that has a property of the resource field name to filter on and a passed in value to use in the filter comparison.

Each resource has their own set of valid filters and fields, valid combinations of filters, and sortable fields.

#### Types of Filters

##### Exact Filter

`filter` - find resources that have an exact field value match to a passed in value. (i.e., FIELD_VALUE = PASS_IN_VALUE).

#### Range Filters

`filter_gt` - find resources that have a field value that is greater than the passed in value.  (i.e., FIELD_VALUE > PASS_IN_VALUE).

`filter_gte` - find resources that have a field value that is greater than or equal to the passed in value.  (i.e., FIELD_VALUE >=  PASS_IN_VALUE).

`filter_lt` - find resources that have a field value that is less than the passed in value.  (i.e., FIELD_VALUE < PASS_IN_VALUE).

`filter_lte` - find resources that have a field value that is less than or equal to the passed in value.  (i.e., FIELD_VALUE \<= PASS_IN_VALUE).

##### Pattern Filter

`filter_prefix` - find resources where the specified field is prefixed by the supplied value. This is applicable to values that are strings.

```csharp title="Sort Example"
// users sorted by username
config.ApiKey = "my-key";
new FilesClient(config);
var args = new Dictionary<string, object>();
var sortArgs = new Dictionary<string, object>();
sortArgs.Add("username", "asc");
args.Add("sort_by", sortArgs);

var userIterator = User.List(args);
foreach (User user in userIterator.ListAutoPaging()) {
  // Operate on user
}

```

```csharp title="Exact Filter Example"
// non admin users
config.ApiKey = "my-key";
new FilesClient(config);
var args = new Dictionary<string, object>();
var filterArgs = new Dictionary<string, object>();
var sortArgs = new Dictionary<string, object>();
filterArgs.Add("not_site_admin", true);
sortArgs.Add("username", "asc");
args.Add("filter", filterArgs);
args.Add("sort_by", sortArgs);

var userIterator = User.List(args);
foreach (User user in userIterator.ListAutoPaging()) {
  // Operate on user
}

```

```csharp  title="Range Filter Example"
// users who haven't logged in since 2024-01-01
config.ApiKey = "my-key";
new FilesClient(config);
var args = new Dictionary<string, object>();
var filterArgs = new Dictionary<string, object>();
var sortArgs = new Dictionary<string, object>();
filterArgs.Add("last_login_at","2024-01-01");
sortArgs.Add("last_login_at", "asc");
args.Add("filter_gte", filterArgs);
args.Add("sort_by", sortArgs);

var userIterator = User.List(args);
foreach (User user in userIterator.ListAutoPaging()) {
  // Operate on user
}
```

```csharp  title="Pattern Filter Example"
// users who usernames start with 'test'
config.ApiKey = "my-key";
new FilesClient(config);
var args = new Dictionary<string, object>();
var filterArgs = new Dictionary<string, object>();
var sortArgs = new Dictionary<string, object>();
filterArgs.Add("username","test");
sortArgs.Add("last_login_at", "asc");
args.Add("filter_prefix", filterArgs);
args.Add("sort_by", sortArgs);

var userIterator = User.List(args);
foreach (User user in userIterator.ListAutoPaging()) {
  // Operate on user
}
```

```csharp s title="Combined Filter Example"
// users who usernames start with 'test' and are not admins
config.ApiKey = "my-key";
new FilesClient(config);
var args = new Dictionary<string, object>();
var filterPrefixArgs = new Dictionary<string, object>();
var filterArgs = new Dictionary<string, object>();
var sortArgs = new Dictionary<string, object>();
filterPrefixArgs.Add("username","test");
filterArgs.Add("not_site_admin", true);
sortArgs.Add("last_login_at", "asc");
args.Add("filter_prefix", filterPrefixArgs);
args.Add("filter", filterArgs);
args.Add("sort_by", sortArgs);

var userIterator = User.List(args);
foreach (User user in userIterator.ListAutoPaging()) {
  // Operate on user
}
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
    Console.WriteLine($"Authentication Error occurred({e.GetType().Name}): " + e.Message);
}
catch (FilesCom.SdkException e)
{
    Console.WriteLine($"Unknown Error occurred({e.GetType().Name}): " + e.Message);
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
|`FolderMustNotBeAFileException`|  `BadRequestException` |
|`InvalidBodyException`|  `BadRequestException` |
|`InvalidCursorException`|  `BadRequestException` |
|`InvalidCursorTypeForSortException`|  `BadRequestException` |
|`InvalidEtagsException`|  `BadRequestException` |
|`InvalidFilterAliasCombinationException`|  `BadRequestException` |
|`InvalidFilterCombinationException`|  `BadRequestException` |
|`InvalidFilterFieldException`|  `BadRequestException` |
|`InvalidFilterParamException`|  `BadRequestException` |
|`InvalidFilterParamValueException`|  `BadRequestException` |
|`InvalidInputEncodingException`|  `BadRequestException` |
|`InvalidInterfaceException`|  `BadRequestException` |
|`InvalidOauthProviderException`|  `BadRequestException` |
|`InvalidPathException`|  `BadRequestException` |
|`InvalidReturnToUrlException`|  `BadRequestException` |
|`InvalidUploadOffsetException`|  `BadRequestException` |
|`InvalidUploadPartGapException`|  `BadRequestException` |
|`InvalidUploadPartSizeException`|  `BadRequestException` |
|`MethodNotAllowedException`|  `BadRequestException` |
|`NoValidInputParamsException`|  `BadRequestException` |
|`PartNumberTooLargeException`|  `BadRequestException` |
|`PathCannotHaveTrailingWhitespaceException`|  `BadRequestException` |
|`ReauthenticationNeededFieldsException`|  `BadRequestException` |
|`RequestParamsContainInvalidCharacterException`|  `BadRequestException` |
|`RequestParamsInvalidException`|  `BadRequestException` |
|`RequestParamsRequiredException`|  `BadRequestException` |
|`SearchAllOnChildPathException`|  `BadRequestException` |
|`UnsupportedCurrencyException`|  `BadRequestException` |
|`UnsupportedHttpResponseFormatException`|  `BadRequestException` |
|`UnsupportedMediaTypeException`|  `BadRequestException` |
|`UserIdInvalidException`|  `BadRequestException` |
|`UserIdOnUserEndpointException`|  `BadRequestException` |
|`UserRequiredException`|  `BadRequestException` |
|`AdditionalAuthenticationRequiredException`|  `NotAuthenticatedException` |
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
|`MustAuthenticateWithApiKeyException`|  `NotAuthorizedException` |
|`NeedAdminPermissionForInboxException`|  `NotAuthorizedException` |
|`NonAdminsMustQueryByFolderOrPathException`|  `NotAuthorizedException` |
|`NotAllowedToCreateBundleException`|  `NotAuthorizedException` |
|`PasswordChangeNotRequiredException`|  `NotAuthorizedException` |
|`PasswordChangeRequiredException`|  `NotAuthorizedException` |
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
|`ZipDownloadIpMismatchException`|  `NotAuthorizedException` |
|`ApiKeyNotFoundException`|  `NotFoundException` |
|`BundlePathNotFoundException`|  `NotFoundException` |
|`BundleRegistrationNotFoundException`|  `NotFoundException` |
|`CodeNotFoundException`|  `NotFoundException` |
|`FileNotFoundException`|  `NotFoundException` |
|`FileUploadNotFoundException`|  `NotFoundException` |
|`FolderNotFoundException`|  `NotFoundException` |
|`GroupNotFoundException`|  `NotFoundException` |
|`InboxNotFoundException`|  `NotFoundException` |
|`NestedNotFoundException`|  `NotFoundException` |
|`PlanNotFoundException`|  `NotFoundException` |
|`SiteNotFoundException`|  `NotFoundException` |
|`UserNotFoundException`|  `NotFoundException` |
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
|`ModelSaveErrorException`|  `ProcessingFailureException` |
|`MultipleProcessingErrorsException`|  `ProcessingFailureException` |
|`PathTooLongException`|  `ProcessingFailureException` |
|`RecipientAlreadySharedException`|  `ProcessingFailureException` |
|`RemoteServerErrorException`|  `ProcessingFailureException` |
|`ResourceLockedException`|  `ProcessingFailureException` |
|`SubfolderLockedException`|  `ProcessingFailureException` |
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
|`AgentUnavailableException`|  `ServiceUnavailableException` |
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

## Examples

### Writing a File

```csharp
// Will upload a file called "test.txt"

using FilesCom;
using FilesCom.Models;

namespace Example
{
    class Program
    {
        static async Task Main(string[] args)
        {
            FilesConfiguration filesConfig = new FilesConfiguration();
            filesConfig.ApiKey = "YOUR-API-KEY";
            FilesClient client = new FilesClient(filesConfig);

            Console.WriteLine("Upload Attempt:");
            var localFilePath = System.IO.Directory.GetCurrentDirectory();
            var uploadRes = await RemoteFile.UploadFile(localFilePath + "/test.txt", "/test.txt");
            if (uploadRes) {
                Console.WriteLine("-- Upload Result: {0}", uploadRes.ToString());
            } else {
                Console.WriteLine("-- Upload Result: Failed");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }
    }
}
```

### Uploading a File from a Stream

```csharp
using(MemoryStream stream = new System.IO.MemoryStream())
{
    byte[] firstString = System.Text.UTF8Encoding.UTF8.GetBytes("Example memory stream contents.");
    stream.Write(firstString, 0, firstString.Length);
    stream.Position = 0; // Reset stream pointer
    var modifiedTime = DateTime.UtcNow;
    Console.WriteLine("-- Uploaded File Size: {0}", stream.Length);
    await RemoteFile.UploadFile("dotnet_upload_stream.txt", stream, stream.Length, modifiedTime, options);
}
```

### Reading a File's Text as a Stream

```csharp
using(MemoryStream stream = new System.IO.MemoryStream())
{
    RemoteFile f = await RemoteFile.Find("/remote_streamio.txt", null, null);
    Console.WriteLine("-- Downloaded File Path: {0}", f.Path);
    Console.WriteLine("-- Downloaded File Size: {0}", f.Size);
    await f.DownloadFile(stream);
    Console.WriteLine("-- Downloaded File Contents: {0}", Encoding.UTF8.GetString(stream.ToArray()));
}
```

### Reading a File and Writing it to Your Local Drive

```csharp
var downloadResponse = await RemoteFile.DownloadFile("Remote-Path/file.txt", "Local-Path/file.txt");
if (downloadResponse) {
    Console.WriteLine("-- Download result: {0}" + downloadResponse+.ToString());
} else {
    Console.WriteLine("-- Download result: Failed");
}
```

### List Root Folder (loads all pages into memory)

```csharp
var files = Folder.ListFor("/", null, null);
foreach (var file in await files.All())
{
    Console.WriteLine("- Path: {0}", file.Path);
}
```

### List Root Folder with Auto Pagination (loads each page into memory)

```csharp
foreach (var file in Folder.ListFor("/").ListAutoPaging())
{
    Console.WriteLine("- Path: {0}", file.Path);
}
```

### List Root Folder with Manual Pagination (loads each page into memory)

```csharp
FilesList<RemoteFile> listing = Folder.ListFor("/");
do
{
    foreach (var file in await listing.LoadNextPage())
    {
        Console.WriteLine("- Path: {0}", file.Path);
    }
} while (listing.HasNextPage);
```

### List Folder with a Filter

```csharp
Dictionary<string, object> parameters = new Dictionary<string, object>(){
    { "filter", "*.png"},
};
var files = Folder.ListFor("/", parameters, null);
foreach (var file in files.ListAutoPaging())
{
    Console.WriteLine("- Path: {0}", file.Path);
}
```

### Comparing Case-Insensitive Files and Paths

For related documentation see [Case Sensitivity Documentation](https://www.files.com/docs/files-and-folders/file-system-semantics/case-sensitivity).
```csharp
if(PathUtil.same("Fïłèńämê.Txt", "filename.txt")) {
    Console.WriteLine("Paths are the same");
}
```

### Logging

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
