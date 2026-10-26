# Files.com .NET Client

The Files.com .NET client library provides a direct, high performance integration to Files.com from applications using the .NET framework.

Files.com is the cloud-native, next-gen MFT, SFTP, and secure file-sharing platform that replaces brittle legacy servers with one always-on, secure fabric. Automate mission-critical file flows—across any cloud, protocol, or partner—while supporting human collaboration and eliminating manual work.

With universal SFTP, AS2, HTTPS, and 50+ native connectors backed by military-grade encryption, Files.com unifies governance, visibility, and compliance in a single pane of glass.

The content included here should be enough to get started, but please visit our
[Developer Documentation Website](https://developers.files.com/net/) for the complete documentation.

Upgrading from FilesCom 1.x? See [Upgrading from FilesCom 1.x to 2.0](UPGRADING.md).

## PowerShell

FilesCom.PowerShell is a binary PowerShell module for Files.com, built on this SDK and distributed through NuGet.org
rather than the PowerShell Gallery; if [its NuGet.org page](https://www.nuget.org/packages/FilesCom.PowerShell) lists
no version, none has been published yet. It runs on PowerShell 7.4 or later (not Windows PowerShell 5.1) and bundles
this SDK with every other .NET assembly it needs. Its version is its own, independent of this SDK's version.

```powershell
Register-PSResourceRepository -Name NuGetGallery -Uri https://api.nuget.org/v3/index.json
Install-PSResource -Name FilesCom.PowerShell -Repository NuGetGallery -Version <version> -Scope CurrentUser
Import-Module FilesCom.PowerShell -RequiredVersion <version>
Get-Command -Module FilesCom.PowerShell
```

More:

- [PowerShell guide](POWERSHELL.md): installing, updating, connecting, transfers and troubleshooting
- [Installation](https://developers.files.com/powershell/overview/installation),
  [introduction](https://developers.files.com/powershell/overview/introduction),
  [authentication](https://developers.files.com/powershell/overview/authentication) and
  [transfers](https://developers.files.com/powershell/overview/transfers) on developers.files.com
- [Command reference](https://developers.files.com/powershell/commands)
- [FilesCom.PowerShell on NuGet.org](https://www.nuget.org/packages/FilesCom.PowerShell)

## Introduction

For integration choices and credential scope, start with [SDKs & APIs](https://www.files.com/docs/clients-and-protocols/sdks-and-apis). [Scripts, APIs & Application Workflows](https://www.files.com/docs/automated-workflows/scripts-apis-and-application-workflows) explains how to coordinate Files.com operations with the work your application owns.

The Files.com .NET client library calls the Files.com API from .NET applications.

Files.com customers use our .NET client library for directly working with files and folders as well as performing management tasks such as adding/removing users, onboarding counterparties, retrieving information about automations and more.

The .NET client library uses the Files.com REST API over HTTPS (port 443) for API requests and file transfers.

### Frameworks Supported

FilesCom 2.0 supports applications that target these frameworks. NuGet installs the build of the FilesCom package
that matches your project's target framework:

- .NET 10, with the package's `net10.0` build
- .NET 8, with the package's `net8.0` build, until Microsoft's support for .NET 8 ends on November 10, 2026
- .NET Framework 4.8 and 4.8.1, with the package's .NET Standard 2.0 (`netstandard2.0`) build

NuGet also installs the .NET Standard 2.0 build for other runtimes that implement .NET Standard 2.0, but Files.com
does not test or support them.

As a policy, Files.com does not support integrations which are considered end-of-life by their vendor. Version 2.0
removed support for .NET Core, .NET 5, .NET 6 and .NET Framework 4.6.1 through 4.7.2. See
[Upgrading from FilesCom 1.x to 2.0](UPGRADING.md) for what changed.

<div></div>

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

Use a version of Visual Studio that supports your project's target framework.

Explore the [files-sdk-dotnet](https://github.com/Files-com/files-sdk-dotnet) code on GitHub.

### Getting Support

The Files.com Support team provides official support for all of our official Files.com integration tools.

To initiate a support conversation, you can send an [Authenticated Support Request](https://www.files.com/docs/support-and-services/requesting-support) or simply send an E-Mail to support@files.com.

## Authentication

There are two ways to authenticate: API Key authentication and Session-based authentication.

### Authenticate with an API Key

Authenticating with an API key is the recommended authentication method for most scenarios, and is
the method used in the examples on this site.

To use an API Key, first generate an API key from the [web
interface](https://www.files.com/docs/security/authentication-and-credentials/api-keys) or [via the API or an
SDK](https://developers.files.com/net/resources/developers/api-keys).

A user key follows its account's permissions, narrowed by the key's permission scope, Workspace, and any path restriction on file operations. It can perform administrative operations only when both the account and key permit them. [API Keys](https://www.files.com/docs/security/authentication-and-credentials/api-keys) explains credential scope and lifecycle; [Workspaces](https://developers.files.com/net/overview/workspaces) explains request context and delegated administration.

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
```

```xml title="app.config"
<configSections>
    <sectionGroup name="files.com">
        <section
            name="filesConfiguration"
            type="FilesCom.FilesConfiguration, FilesCom"
        />
    </sectionGroup>
</configSections>
<files.com>
    <filesConfiguration ApiKey="YOUR_API_KEY" />
</files.com>
```

```csharp title="Example Request Using app.config"
// Using the configuration in app.config
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

Create a session with the username and password of an active user. The session uses that user's permissions, including any delegated administrative access. [Workspaces](https://developers.files.com/net/overview/workspaces) describes account scope and administrative access.

Sessions use the exact same session timeout settings as web interface sessions. When a
session times out, simply create a new session and resume where you left off. This process is not
automatically handled by our SDKs because we do not want to store password information in memory without
your explicit consent.

#### Logging In

To create a session, the `create` method is called on the `Session` object with the user's username and
password.

This returns a session object that can be used to authenticate SDK method calls.

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

`Create` copies the configuration, so later changes to `config` do not affect the client. Reuse the client because each client has its own connection pool.

`new FilesClient(config)` makes the default client, which static methods such as `User.Find(id)` use. Making
another replaces the default for later static calls, but an object keeps the client it came from. A client made
this way reads `config` as each operation starts, so changes you make apply to operations started afterward;
its connections keep the `ConnectTimeout` it was made with.

```csharp title="Example Request"
using FilesCom;
using FilesCom.Models;

var config = new FilesConfiguration();
config.BaseUrl = "https://MY-SUBDOMAIN.files.com";
config.ApiKey = "YOUR_API_KEY";

await using FilesClient client = FilesClient.Create(config);

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

### Closing a Client

Dispose a client when you no longer need it; `await using` or `using` does it for you. From then on, the client,
and the objects, lists and transfers it returned, throw `ObjectDisposedException` for any new operation. Disposing
cancels the operations the client is running, and closes its connections once the SDK has finished with what they
use: their requests, responses and temporary files, and a stream an operation owns, such as the one
`UploadFileAsync(destinationPath, stream, ...)` disposes. `DisposeAsync` completes when that is done and the
connections are closed; `Dispose` does the same without waiting. A list or enumerator that is not loading a page does
not delay closing.

`DisposeAsync` does not wait for the operations' own tasks: await each one for its outcome. A running operation
usually ends with `OperationCanceledException`, as when its token is cancelled, but one whose response had already
arrived can still succeed, and an upload whose final request had been sent may have completed the file. Its task can
complete after `DisposeAsync` does, for example while it turns the response it received into its result.

An operation reading or writing a stream you gave it waits for that stream's current read or write, which the
SDK never forces closed, so a stream that ignores cancellation also delays `DisposeAsync`. Do not await
`DisposeAsync` inside a progress handler of the client's own transfer, which waits for the handler; `Dispose`
returns at once there.

Disposing the default client leaves it as the default, so later static calls throw `ObjectDisposedException`
until you make a new one. Making a new default client does not dispose the previous one.

### HTTP Transport

To send a client's requests through your own proxy, TLS settings, certificate checks or instrumentation, give it
an `HttpTransport`. The SDK still decides everything above the handlers, for API requests and file transfers
alike: the endpoint and credentials of each request, which redirects to follow and which headers go with them,
retries, timeouts and cancellation.

- `HttpTransport.FromHandlerFactory(() => new HttpClientHandler { ... })` gives the client handlers to own. It
  calls your function whenever it needs a new handler, including when it replaces its handlers from time to time to
  pick up DNS changes, and disposes each handler when it is done with it, and every remaining one when the client is
  disposed. Return a new handler each time.
- `HttpTransport.FromSharedHandler(handler)` sends through a handler you own and share. Clients never change or dispose the shared handler. Dispose it after disposing every client that uses it.

The SDK follows redirects itself, so that credentials never go to another origin. It turns off redirects on the
handlers your factory returns, and refuses a shared `HttpClientHandler` or `SocketsHttpHandler` whose
`AllowAutoRedirect` is true. A handler of another kind must not follow redirects or retry requests itself: the SDK
sends a request that can change something, such as a `DELETE` or the one that completes an upload, once per attempt,
and again only where Files.com redirects it, and it cannot know that a handler sent one again.

```csharp title="Example Request"
using System.Net;
using System.Net.Http;
using FilesCom;

var transport = HttpTransport.FromHandlerFactory(() => new HttpClientHandler
{
    Proxy = new WebProxy("http://proxy.example.com:8080"),
    UseProxy = true,
});

await using FilesClient client = FilesClient.Create(config, transport);
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

How long, in seconds, to wait to connect to a server. The default value is 30.

On .NET 8 and later the client applies it to the handlers it makes itself, and to a `SocketsHttpHandler` from an
`HttpTransport` factory whose own `ConnectTimeout` is not set. A connection that it ends was never made, so nothing
was sent, and the request is retried, within Maximum Network Retries, even when it can change something.

It is not applied to any other handler: a shared handler, whose settings the SDK never changes, or a factory's handler
of another type or whose own `ConnectTimeout` is already set. Such a handler connects as it is configured, with its
own connection settings in effect. On .NET Framework, or any other runtime that uses the SDK's .NET Standard 2.0
build, the handlers the client makes itself have no separate connect timeout.

Whatever the handler, connecting also counts toward the wait that follows it: the Read Timeout for an API request or
a download, and the Write Timeout for an upload part or a request with file attachments. Whichever limit runs out
first ends the attempt. When that wait runs out before a connection is made, as it does where no shorter connect
timeout applies, the request fails with `NetworkTimeoutException` for that timeout, and a request that can change
something is not sent again after it, since the SDK cannot tell that nothing was sent.

None of the timeouts limits how long a whole request, transfer or connection takes: each limits one wait on the
network.

```csharp title="Example setting"
using FilesCom;

var config = new FilesConfiguration();
config.ConnectTimeout = 60;
```

#### Read Timeout

How long, in seconds, a request may wait for its response once it has been sent, and then between the parts of
the response as they arrive. The default value is 80.

It limits a stalled connection, not how long a response takes: a download that keeps receiving data continues
however long it lasts. A request that times out fails with `NetworkTimeoutException`, after the retries a
connection failure gets. An error response whose body stops arriving fails with its HTTP status, as an
`InvalidResponseException`, and is retried as that status is.

```csharp title="Example setting"
using FilesCom;

var config = new FilesConfiguration();
config.ReadTimeout = 90;
```

#### Write Timeout

How long, in seconds, an upload part may go without sending any data: to connect and start, and then each time it
sends more. The default value is 80.

It limits a stalled connection, not how long an upload takes, and time spent reading your file or stream is not
counted. A part that times out is retried like one whose connection failed.

```csharp title="Example setting"
using FilesCom;

var config = new FilesConfiguration();
config.WriteTimeout = 120;
```

#### Maximum Network Retries

`MaxNetworkRetries` limits the SDK's retries after temporary failures. The default is **3**; **0** disables
SDK retries. Eligible failures are connection failures, timeouts, HTTP 408, HTTP 429, and 5xx other than 501.
Each SDK retry is logged with its reason, number and wait.

| Request | When the SDK retries |
| --- | --- |
| Reads, such as `GET` | After an eligible temporary failure. |
| Requests that can change data, such as creating a user | Only when the request certainly did not take effect: nothing was sent, or a trusted regional-relocation response allows it to move. |
| Transfer requests explicitly safe to repeat | Like reads: for example, obtaining an upload part's address or creating a folder that may already exist. |
| Upload completion | Never automatically retried. Check the remote file if completion is uncertain. |

For a potentially mutating request, a lost answer, timeout or error response can occur after the change took
effect. The SDK does not replay it, even with retries left. Its exception has `RequestMayHaveBeenProcessed` set:
**check the outcome before sending the request again.** No error status or body, including a rate-limit response,
proves that the change did not begin.

A false flag alone does not guarantee safe replay. For example, an `InvalidResponseException` with `httpStatus = 0`
can mean a successful response was unusable by `CreateAsync`: the change took effect, and no error response is attached.

Transfer parts have their own retry rules. New file downloads can retry into staging; downloads into your stream,
and earlier whole-file download methods, cannot retry after writing bytes. See [Transfers](/net/overview/transfers/).

##### HTTP Handler and Redirect Caveats

This setting counts SDK retries, not every network send. Within an attempt, .NET's handler can resend a read after
a connection closes without an answer; even 0 does not guarantee a single send. The SDK gives mutating requests
content, including an empty body for a `DELETE`, to prevent the handler's automatic replay of bodyless requests.

SDK-managed redirects are still followed within an attempt: `307` and `308` resend the method and body to the new
address. This is routing, and neither redirects nor retry settings guarantee exactly-once processing. A custom
`HttpTransport` handler must not perform its own retries or follow redirects, since the SDK could not track them
or reliably report whether a change may have taken effect.

Certificate failures, refused redirects and failures of caller-owned streams are not retried.

```csharp title="Example setting"
using FilesCom;

var config = new FilesConfiguration();
config.MaxNetworkRetries = 5;
```

#### Initial Network Retry Delay

The wait, in seconds, before the first retry of a Files.com API request. Each later retry waits twice as long as
the one before, up to the Maximum Retry Delay. The default value is 0.1, so the default retries wait 0.1, 0.2 and
0.4 seconds.

File-transfer retry behavior is described in [Transfers](/net/overview/transfers/).

```csharp title="Example setting"
using FilesCom;

var config = new FilesConfiguration();
config.InitialNetworkRequestDelay = 1;
```

#### Maximum Retry Delay

The longest wait, in seconds, before a retry of a Files.com API request. The default value is 2.

```csharp title="Example setting"
using FilesCom;

var config = new FilesConfiguration();
config.MaxNetworkRetryDelay = 5;
```

#### Regional Relocation

When a site is served from another region's address, Files.com answers a request sent elsewhere with
`LockoutRegionMismatchException` and the site's address, its domain or its URL. With the default,
`RegionRelocationPolicy.Automatic`, the client then sends the request again to that address, with the same
credentials and body, when:

- the answer came from the address the request was sent to, over HTTPS whose certificate the client's handler
  checked as strictly as .NET does by default, against the system's trusted roots or a custom trust store, at every
  request since the handler's first (as the client saw its settings when each request started and when its answer
  arrived), and not after a redirect to another origin. The handler is the client's own or
  one from `HttpTransport.FromHandlerFactory`: a client cannot see how a shared handler checked the servers it
  connected to before, or for other code;
- the address is an HTTPS origin on port 443 with a public DNS name, such as the site's own domain, and not the one
  already used;
- the operation has not moved already and has a retry left: moving is one of its Maximum Network Retries, so with 0
  the error is thrown instead.

The operation sends its remaining requests there, and so do the objects and lists it returns, for their own
operations and next pages. The client's configuration is unchanged, and other operations start at the Base URL.

`RegionRelocationPolicy.Disabled` never moves: the error is thrown for your application to act on, such as by changing
its Base URL. `RegionRelocationPolicy.OnlyBetween(from, to)`, with more pairs added by `OrBetween`, moves only from the
first origin of a pair you list to its second; a pair moves one way, so list the way back as another pair. The origins
may use another port, a local name or plain HTTP, as a development site may need, or answer through a handler whose
certificate checks the client cannot see or that relaxes them, such as a shared handler, a
`RemoteCertificateValidationCallback` of your own, or a `CertificateChainPolicy` with
`AllowUnknownCertificateAuthority`, even one tightened after the handler's first request.

When the operation does not move, the exception's `RequestedOrigin` is the address Files.com named, and `Refusal` says
why, such as `RegionRelocationRefusal.Disabled` or `RetriesExhausted`. For `UntrustedIssuer`, the message says which
condition the answer failed, and names the pair that would allow the move only when a pair can.

```csharp title="Example setting"
using FilesCom;

var config = new FilesConfiguration();
config.RegionRelocation = RegionRelocationPolicy.Disabled;

// Or only between exact origins:
config.RegionRelocation = RegionRelocationPolicy.OnlyBetween(
    "https://files.example.com", "https://files.example.eu");
```

#### Logging

Set `LoggerFactory` to your application's `ILoggerFactory` to receive SDK messages under categories named for SDK classes in the `FilesCom` namespace. Each client uses its own factory, so clients in one process can use different log destinations without global configuration. A client without a factory uses log4net, as described in [Logs](/net/overview/logs/).

`MinimumLogLevel` filters out a client's messages below that level, even if its logger enables them. Setting `LogLevel.Warning` excludes that client's `Information` messages. Each client retains the logger factory and minimum level set when it was created.

```csharp title="Example setting"
using FilesCom;
using Microsoft.Extensions.Logging;

var config = new FilesConfiguration();
config.LoggerFactory = loggerFactory; // for example, from the host's services
config.MinimumLogLevel = LogLevel.Warning;
```

#### Product Identity

`ProductName` and `ProductVersion` name the application or module using the SDK. A client sends them as the first
product of its `User-Agent` header, before the SDK's own, `Files.com DOTNET SDK` and its version, which it always
sends. Each must be an HTTP token, without spaces; a client with any other value is refused as it is made, with
`ArgumentException`. A client keeps the values it was made with.

```csharp title="Example setting"
using FilesCom;

var config = new FilesConfiguration();
config.ProductName = "MyBackupTool";
config.ProductVersion = "1.4.0";
// User-Agent: MyBackupTool/1.4.0 Files.com DOTNET SDK v2.0.0
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
    await foreach (User user in userIterator.ListAutoPagingAsync()) {
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
    await foreach (User user in userIterator.ListAutoPagingAsync()) {
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

// users who have logged in on or after 2024-01-01
var args = new Dictionary<string, object>();
var filterArgs = new Dictionary<string, string>();
filterArgs.Add("last_login_at","2024-01-01");
args.Add("filter_gteq", filterArgs);

try
{
    var userIterator = User.List(args);
    await foreach (User user in userIterator.ListAutoPagingAsync()) {
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
    await foreach (User user in userIterator.ListAutoPagingAsync()) {
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

```csharp title="Combination Filter with Sort Example"
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
    await foreach (User user in userIterator.ListAutoPagingAsync()) {
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

See also: [Case Sensitivity Documentation](https://www.files.com/docs/files-and-folders/file-system-semantics/case-sensitivity)

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

`workspace_admin` is not a summary of a user's effective administrative access. A Default Workspace user can administer a Custom Workspace through a direct or group root grant while their `workspace_admin` remains `false`. Groups have no `workspace_admin` field. See [Users](https://developers.files.com/net/resources/user-accounts/users) and [Groups](https://developers.files.com/net/resources/user-accounts/groups) for account fields.

An `admin` grant on the **Custom Workspace root** provides full Workspace Administrator authority over its files, users, groups, Partners, workflows, and integrations. An `admin` grant on a subfolder provides Folder Admin authority over that folder and its descendants; it does not provide Workspace administration. Other permission levels provide their corresponding folder access without Workspace administration. [Permissions](https://developers.files.com/net/resources/user-accounts/permissions) defines the levels.

Site Administrators manage cross-Workspace assignments to Default Workspace accounts. Workspace Administrators manage accounts and permissions within their own scope. Site Administrators retain access to every Workspace; adding a Workspace grant does not narrow Site Administrator authority. The [product documentation](https://www.files.com/docs/workspaces/workspace-administrators) explains the administrator's operational scope and site-wide controls.

### Request Context and API Keys

You can include the `X-Files-Workspace-Id` REST header to select a Workspace for a request. SDK request options and CLI configuration send that same selection. When a Workspace is selected, Workspace-scoped resources are listed, created, and changed within that context, and ordinary paths are relative to its root.

A resource's `workspace_id` request field describes the resource's Workspace membership. It is separate from the SDK's Workspace request option or REST header. Creating a Workspace-scoped resource in a Custom Workspace defaults its `workspace_id` to the selected Workspace; a mismatching membership value is rejected with `not-authorized/insufficient-permission-for-params`.

Selecting another Workspace with an API key requires a **Full Access key created in the Default Workspace**. A user key follows that user's current access, including group permissions. A site-wide Full Access key created in the Default Workspace has Site Administrator authority in every Workspace. A Files Only key stays in its creation Workspace, even if its user has cross-Workspace access. Any key created in a Custom Workspace stays within that Workspace. Selecting another context with these confined keys is rejected with `bad-request/invalid-workspace-id-header`.

An account belonging to a Custom Workspace is scoped there when it authenticates normally. For a Default Workspace user, explicitly select the intended Workspace for an integration rather than relying on an interactive login preference. [API Keys](https://developers.files.com/net/resources/developers/api-keys) and [Authentication](https://developers.files.com/net/overview/authentication) cover credentials.

Set `FilesConfiguration.WorkspaceId` to select a Workspace for the client. To select a Workspace for one request, pass `workspace_id` in the request options.

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

First retrieve the target [Workspace](https://developers.files.com/net/resources/settings/workspaces) and [Group](https://developers.files.com/net/resources/user-accounts/groups) IDs as a Site Administrator in Workspace `0`. The examples use Workspace `123`, group `456`, and member user `789`; replace them with your own IDs. Confirm that the group belongs to Workspace `0` and that the intended user is a member.

Create the Permission using a Default Workspace Full Access site-wide key or a Full Access user key belonging to a Site Administrator. Keep the request context at `0` and use the qualified root path `_/Workspaces/123`. Set `group_id` to the group's ID, `permission` to `admin`, and `recursive` to `true`. Save the returned Permission `id` for later removal. For an individual Default Workspace user, use `user_id` instead of `group_id`.

For a Default Workspace group, a Site Administrator can also select Workspace `123` and use an empty `path` to grant access to its root. The qualified path in Workspace `0` works for both Default Workspace users and groups and keeps the account scope and target Workspace explicit. Appending a subfolder to the path would grant Folder Admin access instead of Workspace Administrator authority.

After the grant, make a request as the member, using their own credential with Workspace `123` selected, such as listing that Workspace's root folder. That member can work with the Workspace's files and perform Workspace Administrator operations, such as managing its users, Partners, and integrations. A Site Administrator's successful request does not establish that the member has the intended access.

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

List the member's Permissions with `user_id` and `include_groups=true` to include grants inherited through group membership. Listing only direct user grants can miss the Permission that provides Workspace administration. In Workspace `0`, the Custom Workspace root appears as `_/Workspaces/123`; in Workspace `123`, paths are relative to that root. Inspect the root path and `permission=admin`, rather than treating the user's `workspace_admin` field as their effective administrative access.

Permission lists show individual grants, rather than a single flag for effective administrative access. Membership in several groups combines their access. A Permission using `group_ids` instead of `group_id` requires membership in all the specified groups; it is not a shorthand for assigning the same grant to several independent groups.

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

Set the `Language` property on `FilesConfiguration` to request localized response content.

```shell title="Example Request"
using FilesCom;

var config = new FilesConfiguration();
config.Language = "es";
```

## Errors

The Files.com .NET SDK reports errors by throwing exceptions. There are many exception classes defined in the Files SDK that correspond
to specific errors.

The raised exceptions come from two categories:

1.  SDK Exceptions - errors that originate within the SDK
2.  API Exceptions - errors that occur due to the response from the Files.com API.  These errors are grouped into common error types.

Exception class names identify the error type. The exception's `Message` property provides error details.

Use .NET exception handling to handle errors. Catch specific exception types before `SdkException`.

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

SDK exceptions inherit from `SdkException`. Invalid SDK usage throws an exception before any request is sent.

Cancellation throws `OperationCanceledException`. Errors from streams you provide retain the stream's exception type.

```shell title="Example SDK Exception Class Inheritance Structure"
FilesCom.InvalidResponseException ->
FilesCom.ApiConnectionException ->
FilesCom.SdkException ->
Exception
```
##### SDK Exception Classes

| Exception Class Name| Description |
| --------------- | ------------ |
| `ApiConnectionException`| The Files.com API cannot be reached, or gave no reliable response |
| `InvalidResponseException`| An `ApiConnectionException` for a response that could not be read, with its `httpStatus` and `httpHeaders` when it was an error response |
| `AuthenticationException`| There is no API key or session ID for a request that needs one |
| `MissingParameterException`| A required parameter, or the ID of the object called, is missing |
| `InvalidParameterException`| A passed in parameter or option is invalid, such as a value of the wrong type |
| `NotImplementedException`| The called method has not been implemented by the SDK |

#### API Errors

API errors are errors returned by the Files.com API.  Each exception class inherits from an error group base class.
The error group base class indicates a particular type of error.

The class comes from the error type in the response, never from its HTTP status. If the SDK has no class for the returned error type, it throws the error group's exception class, including `NotFoundException`, or `ApiException` if the group is unknown. Every API exception
has the response's `httpStatus`, `httpHeaders` and error details, such as `error` and `type`. Its `Message` removes URL user information, query strings and fragments from the error text because these can contain signed URL credentials. The `error` property retains the original text.

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
|`LockOperationBusyException`|  `ServiceUnavailableException` |
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

## Cancellation

Every method that sends a request has an `Async` form that takes a `CancellationToken`: client operations such
as `client.Users.FindAsync`, object methods such as `user.UpdateAsync` and `SaveAsync`, the list methods
`LoadNextPageAsync`, `AllAsync`, `ListAutoPagingAsync` and `ListAutoPaging(cancellationToken)`, and the file
transfers `client.RemoteFiles.UploadFileAsync` and `DownloadFileAsync`, and the uploads of `client.Transfers`,
described in File Transfers.

Cancelling the token stops the operation wherever it is: waiting for a response, waiting to retry, reading or
writing file contents, or between pages and upload parts. The operation then ends with an
`OperationCanceledException` (or a `TaskCanceledException`, which derives from it), and it starts no further
request, retry, page or part.

Methods without a token, including `User.Find` and `LoadNextPage`, do not support token-based cancellation.

[Disposing the client](/net/overview/configuration/#closing-a-client) also cancels its running operations.

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

Cancellation stops the SDK operation. It does not undo requests Files.com has already received. If the last request of
an upload was sent before you cancelled, the file may still be completed.

The SDK passes the token to the streams you give it. A stream that ignores its token finishes its current read
or write before the operation stops.

- `UploadFileAsync(destinationPath, stream, ...)` disposes the stream when it ends, including when it is cancelled.
- `DownloadFileAsync(path, stream)` leaves your stream open.
- `DownloadFileAsync(path, localPath)` closes the local file when it ends. After a failure or cancellation, the
  file may be partly written.

### Timeouts

The `ReadTimeout` and `WriteTimeout` settings end a request whose connection stalls: one that waits too long for its
response or for more of it, or an upload part that cannot send. It then fails with `NetworkTimeoutException`, not
`OperationCanceledException`, after the retries a connection failure gets. An error response whose body stops
arriving fails with its HTTP status instead, as an `InvalidResponseException`. They do not limit a transfer that keeps
moving, however long it takes. To limit a whole operation, use a token that cancels itself, as in the example.

## Pagination

Certain API operations return lists of objects. When the number of objects in the list is large,
the API will paginate the results.

The Files.com .NET SDK provides several ways to load paginated lists.

Creating a list with `client.Folders.ListFor("/")` sends no request. Pages are requested when you load them. Every page comes from the client the list was made with. Lists made with static methods,
such as `Folder.ListFor("/")`, use the default client.

A page can be empty even when more items follow it. Each of these ways continues for as long as Files.com says
there is another page, so none of them stops early at an empty page.

### Automatic Pagination

`ListAutoPagingAsync` goes through every item with `await foreach`. It loads a page when you reach it and holds
only that page, so a long list does not fill memory, and it does not block while a page loads. Leaving the loop
early, with `break`, loads no further page.

Pass a `CancellationToken`, or use `WithCancellation`, to stop before the next page is loaded. Failures, such as
`NotFoundException` or `OperationCanceledException`, are thrown from the `await foreach`.

`await foreach` needs C# 8 or later. .NET 8 and .NET 10 projects already use a later C# version. In a .NET Framework project, set `<LangVersion>8.0</LangVersion>` or later, or call `GetAsyncEnumerator` and `MoveNextAsync` directly.

```csharp title="Example Request"
using FilesCom.Models;

try
{
    await foreach (var file in client.Folders.ListFor("/").ListAutoPagingAsync(cancellationToken))
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

### Automatic Pagination in Synchronous Code

`ListAutoPaging` does the same with `foreach`, blocking while each page loads.

With a `CancellationToken`, failures are thrown as they are, such as `NotFoundException` or
`OperationCanceledException`. Without one, they are wrapped in an `AggregateException`.

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

If loading a page fails, the list keeps the page it had and `HasNextPage` is unchanged, so you can try again.
`Reset` starts the list again from its first page.

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

The `AllAsync` method loads every page, then returns all their items together, so they are all in memory at once.

`ListAutoPagingAsync`, `ListAutoPaging` and `AllAsync` each start from the list's first page, whatever pages
`LoadNextPageAsync` has loaded.

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

## File Attachments

Some operations take a file as a parameter: a user's `avatar_file`, a behavior's `attachment_file`, a bundle's
`watermark_attachment_file`, a site's `logo_file`, its icons and its login page background, and a style's `file`.
Give such a parameter a `FileAttachment`:

- `FileAttachment.FromFile(path)` sends a local file, under its own name unless you give another.
- `FileAttachment.FromBytes(bytes, fileName)` sends bytes in memory. The array is not copied. Leave it unchanged until the operation ends.
- `FileAttachment.FromStream(stream, fileName)` sends the rest of a stream of yours, from its position when the
  operation starts.

Each also takes the file's media type, such as `image/png`; without one, it is sent as `application/octet-stream`.
The file name is sent exactly as given. Files.com still validates the file and its name as it does for any upload.

A model's file property, such as `User.AvatarFile`, takes a `FileAttachment` too, which `SaveAsync` sends with the
object's other changes.

```csharp title="Example Request"
using FilesCom;
using FilesCom.Models;

await using FilesClient client = FilesClient.Create(config);

User user = await client.Users.UpdateAsync(userId, new Dictionary<string, object>
{
    { "avatar_file", FileAttachment.FromFile("avatar.png", contentType: "image/png") },
    { "notes", "New picture" },
});

Site site = await client.Sites.UpdateAsync(new Dictionary<string, object>
{
    { "logo_file", FileAttachment.FromBytes(logoBytes, "logo.png", "image/png") },
    { "icon16_file", FileAttachment.FromStream(iconStream, "icon16.png", "image/png") },
});
```

### How Files Are Sent

The files go in the same request as the operation's other parameters, and every other parameter keeps the exact
value it would have without a file: `false`, `0`, empty text, `null` inside an object, empty lists and objects,
and numbers or booleans inside an object alike.

- When form fields can represent every value exactly, and the file names unchanged, the request is
  `multipart/form-data`, with each file as a part of its own.
- Otherwise the request is JSON, in which each file is the API's file object, with its bytes as Base64 in
  `encoded_content`, its name in `filename` and its media type in `type`. The request is then about a third larger
  than the files. That is the case for an empty list or object, `null` or a number or boolean inside an object,
  a list of objects, and a file name with `%`, `\`, `/` or `"`, which a form would change.

Either way, files are read in chunks as the request is sent, and never held whole in memory for it, and the request
states its exact length. A large request is timed as an upload is: `WriteTimeout` limits how long it may go without
sending any data, however long it takes in all.

A file parameter also takes the API's file object itself, a `Dictionary<string, object>` with `encoded_content`,
`filename` and `type`, which is sent as it is. A `FileAttachment` outside a file parameter, including one nested in another parameter's dictionary, throws `InvalidParameterException` before the request is sent.

```csharp title="Exact Values With A File"
// Sent as JSON, since a form cannot carry an empty list or a number inside an object.
await client.Bundles.CreateAsync(new Dictionary<string, object>
{
    { "paths", new string[0] },
    { "watermark_value", new Dictionary<string, object> { { "transparency", 25 } } },
    { "watermark_attachment_file", FileAttachment.FromFile("watermark.png") },
});
```

### Retries, Redirects and Failures

A request that is sent again, after a failure that may be temporary, a redirect that repeats it, or a move to the
site's region, sends the same bytes. A request with files that can change something, such as an update, is sent
again only when it certainly was not carried out, such as after a move to the site's region: after a lost answer, or
an error such as 503, it is not, and says it may have taken effect (see Maximum Network Retries under
Configuration). So files and streams must keep their contents and length until the operation ends:

- A local file is opened when the operation starts and read through that one handle for every attempt. A file whose
  length changes fails the operation with an `IOException` before it is sent again.
- A stream that can seek is read in place, from its starting position, for each attempt, and must keep its length.
- A stream that cannot seek is first copied, up to `maxBytes` (64 MiB unless you choose otherwise), to a temporary
  file of the operation's own, readable only by you where the system allows. A longer stream fails the operation with
  `InvalidParameterException` before anything is sent.

A failure to read a file or stream, such as an `IOException`, is thrown as it is, and the request is not sent again,
even when retries remain. A file that cannot be opened fails the same way, before anything is sent, with .NET's own
exception, such as `System.IO.FileNotFoundException`. `FilesCom.FileNotFoundException` represents the API error. If both namespaces are in scope, use the fully qualified exception name in your `catch` clause, or catch `IOException` for local file failures.

The operation closes files it opened and deletes its temporary files when it ends, whether it succeeds or fails. A deletion failure is logged and does not replace the operation's result or exception. Streams you provide remain open. Their final position is determined by the SDK.

The SDK's DEBUG log shows the parameters of a request with files, with each file's length in place of its bytes,
which are never logged.

## File Transfers

Use `client.Transfers` to upload and download files, folders and streams. The SDK adjusts parallel transfers where
supported, retries temporary failures within a limit, and reports progress. Start with the paths; tuning is optional.

### Upload a File

`UploadFileAsync(localPath, destinationPath)` uploads a local file. Set `MkdirParents` to create missing remote
folders. The returned `UploadResult` describes the completed file, its parts and the bytes sent over the network.

Keep the source unchanged until the task finishes. The SDK opens it once and reads through that handle on every
attempt, so replacing or renaming the path does not change the source. Windows prevents other programs from changing the file while the handle is open.

A successful task means Files.com completed the upload. If the final request's outcome is unknown, check the remote
file before uploading again; see [Cancellation and Failures](#cancellation-and-failures).

```csharp title="Uploading a File"
using FilesCom;
using FilesCom.Transfers;

FilesClient client = FilesClient.Create(new FilesConfiguration { ApiKey = "YOUR_API_KEY" });

UploadResult result = await client.Transfers.UploadFileAsync(
    @"C:\exports\report.csv",
    "reports/report.csv",
    new UploadOptions { MkdirParents = true });

Console.WriteLine($"Uploaded {result.Size} bytes in {result.Parts.Count} parts");
```

### Downloads

`DownloadFileAsync(remotePath, localPath)` downloads into a staging file beside the destination. It puts the file
in place only after all bytes have arrived and passed validation. **By default, it replaces an existing local
file.** Choose `PublicationPolicy = DownloadPublicationPolicy.CreateNew` to keep anything already at that path.
See [Putting Downloaded Files in Place](#putting-downloaded-files-in-place) for the policies and platform limits.

The SDK uses parallel parts where storage supports both ranges and reliable version checks, and fetches the whole
file in one response when needed. A version-bound download fails with `DownloadSourceChangedException` if the source
changes, instead of combining different versions. `DownloadResult.VersionBound` reports version protection, not
whether parts were used: a whole-response fallback can remain bound to a version. Unbound downloads cannot reuse
partial bytes on a later run.

Temporary failures are retried within `MaxNetworkRetries`. A whole-response download into a local file can restart
from its first byte after a failed attempt. Cancellation or a failure before publication leaves the destination
file unchanged. A report of the last byte written is not completion: await the task.

Use `DownloadStreamAsync(remotePath, stream)` to write into your own stream, from its current position, in order.
The SDK leaves it open and never writes the same bytes into it twice, so it can restart only before writing any
bytes. Parts fetched ahead are buffered within the transfer limiter's memory bound.

```csharp title="Downloading a File"
DownloadResult result = await client.Transfers.DownloadFileAsync(
    "reports/report.csv",
    "report.csv",
    new DownloadOptions
    {
        PublicationPolicy = DownloadPublicationPolicy.CreateNew,
    });

Console.WriteLine($"Downloaded {result.Size} bytes; bound to a version: {result.VersionBound}");
```

```csharp title="Downloading Into a Stream"
using (Stream destination = System.IO.File.Create("report.csv"))
{
    await client.Transfers.DownloadStreamAsync("reports/report.csv", destination);
}
```

The stream example opens the local file itself, so it truncates an existing file immediately. Use
`DownloadFileAsync` when you need staging and overwrite protection.

### Folders

`UploadDirectoryAsync(localDirectory, remoteDirectory)` and `DownloadDirectoryAsync(remoteDirectory, localDirectory)`
transfer whole folder trees, preserving relative paths and empty folders. Both list the source first, then start
files in path order. `FileOptions` supplies each file's settings.

Automatic mode shares a concurrency target among the folder's files, with separate targets when upload destinations
need different tuning. It adjusts both active files and parts, up to `MaxConcurrentFiles` if set, or 128 otherwise.
In static mode, `MaxConcurrentFiles` defaults to 4, and each file has its own part concurrency.
`DirectoryTransferProgress.FileLimit` shows the current limit. All files share the folder transfer's limiter.

Uploads exclude symbolic links and other entries that are neither regular files nor directories, and list them in
`DirectoryTransferResult.Excluded`. Downloads refuse symbolic links in local folder paths and remote names that
cannot be local names, such as `..` or a name containing `:` on Windows, before writing anything.

The first failure stops active files and prevents new ones from starting. The task throws that failure after all
active files stop. **Files already completed stay completed; a folder download does not replace the tree as one
operation.** A `Progress.Report` exception is also a failure. Exceptions from cancellation callbacks registered on
SDK-provided stream tokens are logged during cleanup and do not replace an existing transfer failure.

Set `CheckpointDirectory` to continue stopped files and recognize completed files on a later run. Keep it outside
the local tree, including through links. The SDK verifies the source before skipping a file: accepted upload bytes
must still match; a download needs the same storage version and the local file it wrote. Size and modification time
alone do not establish that the source is unchanged. Unbound downloads are downloaded again.
See [Continuing Stopped Transfers](#continuing-stopped-transfers) for checkpoint requirements.

Settings, including the client's settings and listing request options, are captured when the transfer starts.
Changing them during a run has no effect. The complete source plan stays in memory until the task ends;
`MaxConcurrentFiles` limits active transfers, not the memory needed to list a very large tree.

```csharp title="Uploading a Folder"
DirectoryTransferResult uploaded = await client.Transfers.UploadDirectoryAsync(
    @"C:\exports",
    "exports",
    new DirectoryUploadOptions
    {
        MaxConcurrentFiles = 4,
        CheckpointDirectory = @"C:\transfer-state\exports",
    });

Console.WriteLine($"{uploaded.Files.Count} files uploaded, {uploaded.Excluded.Count} excluded");
```

```csharp title="Downloading a Folder"
await client.Transfers.DownloadDirectoryAsync("reports", "reports", new DirectoryDownloadOptions
{
    CheckpointDirectory = "transfer-state/reports",
    FileOptions = new DownloadOptions
    {
        PublicationPolicy = DownloadPublicationPolicy.CreateNew,
    },
});
```

### Files and Streams

`UploadStreamAsync(stream, length, destinationPath)` reads exactly `length` bytes from the current position.
The overload without `length` reads until the stream ends. Neither seeks nor closes your stream. After failure or
cancellation, its position is undefined. A file or stream shorter than its declared length fails with
`EndOfStreamException`, without completing the upload.

Stream parts must be kept until accepted so retries send the same bytes. Choose where to keep them:

| Option | Behavior |
| --- | --- |
| `StreamPartStorage.Memory` (default) | Holds parts within the limiter's buffer bound. A part larger than the bound fails before it is read. |
| `StreamPartStorage.TemporaryFiles` | Keeps each part in a temporary file, deleting it when accepted or when the upload ends. `TemporaryDirectory` chooses the folder. |
| `SpoolMaxBytes` | Copies the entire unknown-length stream to a temporary file first, up to this bound, then uploads it as a known-length file. Nothing is sent if the copy exceeds the bound, fails or is cancelled. |

Temporary files inherit the Windows temporary folder's permissions. Choose a private `TemporaryDirectory` for
sensitive data. The same applies to a forward-only `FileAttachment` that must be copied.

#### Streams of Unknown Length

Set `MaxStreamLength` when you know an upper bound. A destination requiring a known size can refuse an unspooled
stream before sending anything. An upload that exceeds its length or part-count bound stops without sending the
excess, but parts already sent are not undone. Use `SpoolMaxBytes` if nothing should be sent before the full stream
is read.

Where the SDK chooses part sizes, an unknown-length upload starts at 8 MiB
(`AutomaticPartSizing.UnknownLengthStartSize`) and may grow. It sizes parts to fit the upper bound into the remaining
parts. If in-memory parts cannot fit that bound, it uses server-planned sizes instead
(`PartSizing.ServerPlannedReason = MemoryLimit`). With the default 128 MiB buffer limit, the maximum 5 TiB stream
cannot fit in 10,000 memory-backed parts; set a smaller `MaxStreamLength` or use temporary files.

Where Files.com plans each part, unknown-length streams are sent serially in the planned sizes. Reads are always
forward and serial, even when buffered parts can be sent in parallel. An exact part-boundary end sends no empty part.
Read-ahead defaults to `MaxReadyParts = 2` and `MaxReadyBytes = 64 MiB`; one larger part may be read ahead when no
other part is waiting. Spooling reports `SpooledBytes`; `TotalBytesKnown` stays false until the size is known.

Streams do not support checkpoints. Use a local file when you need to resume a transfer across runs.

```csharp title="Uploading Streams"
using (Stream source = await httpClient.GetStreamAsync(exportUrl))
{
    await client.Transfers.UploadStreamAsync(source, exportLength, "exports/export.zip");
}

// Read an unknown-length stream, bounded at 10 GiB, keeping parts on disk.
using (Stream source = await httpClient.GetStreamAsync(liveExportUrl))
{
    UploadResult result = await client.Transfers.UploadStreamAsync(source, "exports/live.zip", new UploadOptions
    {
        MaxStreamLength = 10L * 1024 * 1024 * 1024,
        StreamPartStorage = StreamPartStorage.TemporaryFiles,
    });
    Console.WriteLine($"Uploaded {result.Size} bytes");
}
```

### Remote Files and Folders

`client.FileSystem` provides familiar file, folder and stream operations:

| Operation | What it does |
| --- | --- |
| `OpenReadAsync(path)` | Opens a forward-only download stream. |
| `OpenWriteAsync(path)` | Opens a forward-only upload stream; disposing it completes the upload. |
| `ReadAllBytesAsync(path)`, `WriteAllBytesAsync(path, bytes)` | Reads or writes the whole file. |
| `FileExistsAsync(path)`, `DirectoryExistsAsync(path)` | Checks whether the file or folder exists. |
| `EnumerateAsync(path, recursive)` | Lists entries as enumeration advances, optionally including subfolders. |
| `CreateDirectoryAsync(path)` | Creates the folder and missing parents; does nothing if it exists. |
| `DeleteAsync(path, recursive)`, `CopyAsync(source, destination)`, `MoveAsync(source, destination)` | Waits for Files.com to finish the operation. |

Remote streams cannot seek, append or change an existing file's contents. `Seek`, `Length` and `Position` throw
`NotSupportedException`. A read stream delivers buffered bytes before raising a download failure. Disposing a write
stream raises any upload failure; to abandon the upload instead of completing it, cancel the token passed to
`OpenWriteAsync` **before disposing the stream**.

```csharp title="Reading and Writing Remote Files"
using (Stream remote = await client.FileSystem.OpenReadAsync("reports/report.csv"))
using (var reader = new StreamReader(remote))
{
    Console.WriteLine(await reader.ReadLineAsync());
}

using (Stream remote = await client.FileSystem.OpenWriteAsync("reports/summary.txt"))
using (var writer = new StreamWriter(remote))
{
    await writer.WriteLineAsync("Done");
}
```

When using `client.RemoteFiles.CopyAsync` or `MoveAsync` directly, Files.com can return a pending `FileAction`.
Call `FileAction.WaitAsync()` to wait for its migration; it throws `FileMigrationFailedException` if the migration
fails or is cancelled. Details are on `Migration` and in DEBUG logs, and may include service-supplied addresses.
`client.RemoteFiles.WaitUntilGoneAsync(path)` waits for a path to disappear, such as after recursive deletion.
Use a cancellation token to bound either wait. Each operation uses the client settings, address and request
options captured when it starts, including requests made while waiting.

```csharp title="Waiting for a Copy"
FileAction copy = await client.RemoteFiles.CopyAsync("reports",
    new Dictionary<string, object> { { "destination", "archive/reports" } });
await copy.WaitAsync(cancellationToken: cancellationToken);
```

### Progress

Set `UploadOptions.Progress`, `DownloadOptions.Progress` or the folder options' `Progress` to update your UI or job
status. Uploads report when sending starts and when a part is accepted, rather than on a timer. Mark a job complete
only after its task succeeds; a progress callback may run after task completion when reports are posted to a UI.

```csharp title="Reporting Progress"
var options = new UploadOptions
{
    Progress = new Progress<TransferProgress>(progress =>
        Console.WriteLine($"{progress.AcknowledgedBytes} bytes accepted; {progress.Elapsed}")),
};

await client.Transfers.UploadFileAsync(localPath, destinationPath, options);
Console.WriteLine("Upload complete");
```

| Field | Use |
| --- | --- |
| Upload `AcknowledgedBytes`; download `WrittenBytes` | Bytes accepted or checked and written, counted once. |
| Upload `SentBytes`; download `ReceivedBytes` | Network bytes, including retries. |
| `TotalBytes`, `TotalBytesKnown` | Total size when known; otherwise bytes read or written so far. Check the flag before calculating a percentage. |
| `Elapsed`; folder `BytesPerSecond` | Duration; folder speed based on this run's bytes only. |
| `ActiveParts`, upload `PartRetries`, download `Retries` | Parts in progress and retry counts. |
| `Concurrency` | Target and limits, requests in flight, throughput, latency, throttles, cooldown and decisions. Null for downloads arriving in one response, including after a fallback. |
| Upload `PartSizing` | Part-size source, limits, next size and growth decisions. |
| `RequestWait`, `BufferWait`; upload `AdmissionWait`, `ReadyParts` | Time waiting for shared resources or admission, and upload read-ahead. |
| `Process` | Whole-process CPU and memory, sampled at most once a second. |
| Folder `FilesDone`, `FilesTotal`, `FilesActive`, `FileLimit` | Completed, planned and active files, and the current active-file limit. |

`UploadResult.FinalProgress` and `DownloadResult.FinalProgress` retain the final snapshot. For automatic folder
transfers, `Concurrency.Shared` means the target and decisions cover files together. An upload still reports its own
`Limit = 1` and `SerialReason` when its destination requires serial parts. Download concurrency also records time at
the target, waiting for the limiter, without a ready part, and writing the destination.

#### Callback Behavior

Upload progress and trace handlers are called serially on SDK threads. Accepted-part reports happen after that
part releases its limiter resources; other parts may still hold resources. Return promptly. If your
`IProgress<T>.Report` implementation throws, the transfer fails with that exception.

`Progress<T>` posts to the context where it was created and returns immediately. Its handler exceptions occur in
that context, rather than failing the transfer, and posted callbacks may arrive after the task ends.

### How Parts Are Sent

Files.com supplies each upload part's address, size and permission to send parts in parallel or retry them. The SDK
respects those instructions. Accepted parts are never sent again. After all parts are accepted, it makes one final
completion request, which is never automatically retried.

`MaxNetworkRetries` defaults to 3; 0 disables the SDK's retries. Retryable failures are connection failures,
timeouts, HTTP 408, HTTP 429, and 5xx other than 501. Upload parts are retried only when Files.com allows it, with
the same bytes. Version-bound download parts are retried individually. Whole-response file downloads may restart;
downloads into your stream retry only before any bytes have been written.

For storage requests, HTTP 429 or 503 waits for the server's `Retry-After`, without shortening it to
`MaxNetworkRetryDelay`; without that header there is no retry wait. Other transient failures wait 100 milliseconds.
If the platform cannot schedule the requested wait, the failure is thrown instead of retrying early. Automatic mode
also pauses new part requests after throttling.

An address about to expire is renewed before use. An address refused as expired is renewed for the same part or
storage object. Other 403 responses fail the transfer. Files.com API retry settings and their replay-safety rules
are described in [Configuration](/net/overview/configuration#maximum-network-retries).

#### Addresses and Credentials

Upload parts go to the exact storage addresses Files.com provides, with the part's exact `Content-Length`, without
an API key, session ID or Workspace selection. Upload storage redirects must preserve the method and bytes and
never downgrade HTTPS to HTTP. Negotiated headers stay within their origin; credential headers such as
`Authorization` are not forwarded to another origin. Each transfer uses the client settings captured at its start.

### Cancellation and Failures

Pass a `CancellationToken` to stop a transfer or give it a total deadline. Disposing the client also stops its
transfers. Cancellation and failure stop reads, requests, limiter waits, cooldowns and retry waits. The task ends
after active parts stop and release handles and limiter resources, with `OperationCanceledException` or the first
failure.

`ReadTimeout` and `WriteTimeout` detect stalled requests, not total transfer duration. Both default to 80 seconds.
For uploads, reading your source does not count toward these network timeouts. Use a token with a deadline to limit
the entire operation.

#### Upload Completion Can Be Unknown

Before the final request, a stopped upload is not completed, and accepted parts are not undone. Once the final
request may have reached Files.com, a connection failure, unreadable response, error response or cancellation can
leave its outcome unknown, even if the response reports an error. The SDK throws `UploadCompletionUnknownException`
with the underlying failure in `InnerException`. **Check the remote file before sending another upload.**

A checkpoint recording a final request also refuses automatic replay on a later run, even with `StartOver`.

#### Other Failures

- `UploadLimitException` derives from `InvalidOperationException`. Its `Limit` identifies `StreamLength`, `PartCount`,
  `BufferSpace`, `MemoryPart`, `SpoolLength` or `FixedPartSize`; its message explains what to change. Known limits fail
  before any part is sent. Unknown-length streams can hit limits later, without sending the excess.
- Download connection failures throw `ApiConnectionException`; stalled requests throw `NetworkTimeoutException`, a
  subtype. These read failures have `RequestMayHaveBeenProcessed = false`. This flag does not undo bytes already
  written into your stream or a file already published.
- Exceptions from your source or destination stream are propagated with their original type.
- Local download publication can fail before or after the file is in place. Inspect
  `DownloadPublicationException.Placed`, as described below, before deciding how to recover.

```csharp title="Setting a Transfer Deadline"
using (var cancellation = new CancellationTokenSource(TimeSpan.FromMinutes(30)))
{
    try
    {
        await client.Transfers.UploadFileAsync(localPath, destinationPath,
            cancellationToken: cancellation.Token);
    }
    catch (UploadCompletionUnknownException e)
    {
        Console.WriteLine($"Check {e.Path}: upload completion is unknown.");
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("The upload was cancelled.");
    }
}
```

### Continuing Stopped Transfers

Set `CheckpointPath` for a local file transfer, or `CheckpointDirectory` for a folder transfer. After a failure,
cancellation or process interruption, run the same transfer with the same site, workspace, local and remote paths,
and checkpoint. Streams cannot use checkpoints.

Uploads verify accepted parts against the source bytes, then send only parts not yet accepted.
`UploadResult.ResumedParts` counts the reused parts. Downloads confirm the stored version first, then fetch missing parts where storage honors ranges. If ranges are
ignored, they fetch the same version from the beginning; `DownloadResult.ResumedBytes` counts reused bytes. Successful completion deletes the checkpoint.

Keep checkpoints on a private local disk, outside a folder transfer's local tree. They contain no credentials or
signed addresses, but their integrity determines which bytes the SDK reuses. The SDK writes complete replacement
checkpoints, at most every two seconds during a run and when it stops, flushing data before relying on it.
The SDK flushes checkpoint contents but does not flush the directory rename on Windows, so a system crash or
power loss can undo the latest checkpoint update.

#### Choosing Whether to Start Over

| Situation | Behavior |
| --- | --- |
| Upload source changed, upload expired, or granted limits changed | `TransferCheckpointException` with `SourceChanged`, `UploadExpired` or `GrantChanged`; no part is sent. Delete the checkpoint or choose `CheckpointRestartPolicy.StartOver` to start a new upload. |
| Download's stored version is gone or its address now names another object | Starts from byte zero in a new staging file. |
| Version confirmation is cancelled, fails or is refused access | Leaves the download checkpoint and stage unchanged for another attempt. |
| Download stage is shorter than recorded | Fails by default; `StartOver` discards only a stage proven to belong to the transfer and starts again. |
| Checkpoint belongs to another site, workspace or paths (`OtherTransfer`), is untrusted, or is from a newer unsupported format | Refuses it regardless of restart policy. |
| Upload checkpoint records the final request | Sends nothing and throws `UploadCompletionUnknownException`, regardless of restart policy. Check the remote outcome first. |

The final-upload record is flushed before sending the completion request. If that flush fails, the request is not
sent; a record left behind still makes later runs refuse replay. Only when the failure explicitly establishes that
the request was never sent is it safe to delete that record and upload again. If recording a stopped transfer also
fails, the SDK logs the checkpoint failure and keeps the original transfer exception.

#### Checkpoint Trust and Platform Support

The SDK rejects unsafe checkpoints with `TransferCheckpointException` (`Untrusted`) before reading them. A file
transfer sends no request first; a folder download lists the source first. Existing stages and destinations stay
unchanged, though empty directories created for the transfer may remain. Restore the required privacy to retry.

- The checkpoint directory must belong to you, SYSTEM or Administrators. Its ACL must prevent others from adding,
  removing or renaming entries, deleting the directory or changing its access.
- Directories on the resolved path are held during checkpoint access; links and junctions are refused. Impersonated
  calls use the impersonated user's identity.
- The checkpoint must be a regular file with one name, owned by you, SYSTEM or Administrators, that no other user can
  write. The SDK creates checkpoint files and missing checkpoint directories with private access.

Checkpoints also refuse recorded permissions beyond those of a new file. Restore the required directory and file
access before retrying an untrusted checkpoint.

#### Download Recovery and Older Checkpoints

A version-bound, complete stage retained after a publication refusal can be published on a later run without
fetching again, once storage confirms its version. `DownloadResult.CompletedEarlier` is true when a later run
recognizes the file already published and finishes the remaining bookkeeping. Recognition uses file identity and
length, not a content hash: an in-place edit preserving both may be treated as the original, and filesystems can
reuse deleted file identities. Avoid concurrent changes to transfer files and directories.

A checkpoint format that an older SDK cannot read is refused without changing it, regardless of `StartOver`.
Finish with an SDK that reads that format or use another checkpoint path. A partial 2.0-preview checkpoint bound by
Files.com cannot be confirmed against storage; it is refused unless `StartOver` is selected. Upload checkpoint
formats are unchanged.

Folder checkpoints recognize completed downloads only after the current storage version and local file identity
and length match. Older folder records use length and modification time for local recognition; the default policy
can still skip them after storage confirmation, but `CreateNew` and `ReplaceRegularFile` treat them as existing
files. Every resumed-byte or completed-file verification uses buffers within the limiter's bound.

```csharp title="Continuing a File Transfer"
var options = new UploadOptions { CheckpointPath = "backup.iso.checkpoint" };

// Run this same call again after interruption to reuse accepted parts.
UploadResult uploaded = await client.Transfers.UploadFileAsync(
    "backup.iso", "backups/backup.iso", options, cancellationToken);

DownloadResult downloaded = await client.Transfers.DownloadFileAsync(
    "backups/backup.iso", "restored.iso",
    new DownloadOptions { CheckpointPath = "restored.iso.checkpoint" },
    cancellationToken);
```

### Putting Downloaded Files in Place

`DownloadOptions.PublicationPolicy` controls the final local name. The policy of the current run applies even when
resuming an older checkpoint. `DownloadStreamAsync` and `OpenReadAsync` accept only the default policy, because they
have no local file to publish.

| Policy | Existing destination |
| --- | --- |
| `ReplaceExisting` (default) | Replaces it using the Windows access behavior below. |
| `CreateNew` | Refuses anything already at the name, including folders and dangling links. |
| `ReplaceRegularFile` | Creates a file or replaces a regular file, preserving the access described below. Refuses folders, links and special files, without following them. This is PowerShell's `-Force` behavior. |

A refused destination fails before downloading when already present, and is checked again at publication.
`CreateNew` never replaces an existing entry. On Windows, every policy holds the destination directory and its
ancestors while the file is downloaded, preventing directory renames. `ReplaceRegularFile` checks the file type and
replaces it in separate steps; another program changing directory entries can change the entry between those steps.

#### Recovering From Publication Failure

`DownloadPublicationException` is an `IOException`. Inspect its fields:

| Field | Meaning |
| --- | --- |
| `Problem` | Why publication failed: for example, `DestinationExists`, `DestinationNotRegularFile`, `AccessNotKept`, `NotSupported`, `StageReplaced` or `DestinationMoved`. |
| `Placed` | Whether the downloaded file was already put in place. It stays there after a subsequent failure. |
| `Retained` | Whether a private, version-bound stage was kept with a checkpoint for a later attempt. |

With a checkpoint, a complete stage is flushed and recorded before publication. A refusal can retain it privately
for another run after the destination issue is resolved. Unbound downloads cannot retain a confirmable version.
`StageReplaced` retains nothing and removes the checkpoint. A moved destination directory can retain the stage in
the originally opened directory; put that directory back at the original path before retrying.

After publication, `PlacedNotRecorded` means a folder transfer could not record the completed file. `DestinationMoved` can also have `Placed = true`: the file is in the
original directory, which is now elsewhere. These errors do not roll back the file. A later run recognizes only the
same file, in the expected directory, once its storage version is confirmed.

A folder transfer records each completed file before removing its individual checkpoint. An interrupted record
can be recovered from that checkpoint, including with `CreateNew`. If the remote version changed, the current
publication policy applies again; `CreateNew` keeps and refuses the existing local file.

```csharp title="Handling an Existing Destination"
try
{
    await client.Transfers.DownloadFileAsync("reports/report.csv", "report.csv", new DownloadOptions
    {
        PublicationPolicy = DownloadPublicationPolicy.CreateNew,
        CheckpointPath = "report.csv.checkpoint",
    });
}
catch (DownloadPublicationException e) when (e.Problem == DownloadPublicationProblem.DestinationExists)
{
    Console.WriteLine($"{e.LocalPath} exists; download retained for another attempt: {e.Retained}");
}
```

#### Local Access and Durability

The staging file has a protected ACL allowing only you, SYSTEM and Administrators, and remains open
without sharing until publication. Its folder is held while used and removed only if still the same empty folder.
Every publication policy holds the destination directory and its ancestors from transfer start, preventing renames.
Unsupported paths, such as a volume without a drive letter, fail with `NotSupported` before fetching.
The default `ReplaceExisting` and `ReplaceRegularFile` preserve the replaced file's ACL and hidden attribute;
a new file gets the directory's normal new-file access. This Windows behavior applies to all supported .NET targets.

Only the file is flushed. Resume requires a retained stage whose owner and ACL are verified private and which has
only its own name.
Stages without verifiable ACLs, including those retained by earlier Windows SDK versions, restart instead. They can
still be published with the required final access; inability to apply it fails with `AccessNotKept`.

The SDK flushes file contents before publication but does not flush the directory rename on Windows. This does
not promise that the newest filename or checkpoint update survives a system crash or power loss. A failure after the
file was put in place does not roll it back; inspect `Placed` before deciding how to recover.

### Automatic Concurrency

The default mode adjusts the number of simultaneous parts based on measured throughput, latency and temporary
failures. Upload tuning is selected for the destination from Files.com's instructions. Downloads tune concurrency
only when using version-bound parts. Other downloads remain whole-response requests.

`MaxConcurrency` caps the target; the limiter's slots and buffers can reduce actual concurrency further. Static mode
keeps the target at `MaxConcurrency` when parallel parts are supported. Downloads accept 1–1024 and default to 4 in
static mode. Server-required serial uploads remain serial in either mode.

For special cases, set `UploadOptions.AutomaticConcurrency` or `DownloadOptions.AutomaticConcurrency` to custom
`AutomaticConcurrencyOptions`. These apply throughout the transfer, do not change part sizes and cannot override a
server requirement for serial uploads. Invalid tuning fails before sending anything; static downloads refuse
`AutomaticConcurrency`.

#### Upload Startup

For the SDK-selected scaled S3 upload policy, `UploadOptions.Startup` chooses how the transfer begins:

- `UploadStartup.Default` probes from one part and raises concurrency as responses show that it helps. This is the
  default and can reach the same concurrency ceiling as Aggressive.
- `UploadStartup.Aggressive` skips those initial probes and starts at the policy's target of 150, within
  `MaxConcurrency` and the limiter. Use it for a known high-throughput network.

Both remain adaptive, with the same throttling response, backoff, retries and cancellation. This choice affects
only automatic uploads using the SDK-selected scaled S3 policy. Custom `AutomaticConcurrency`, static mode, Agents
and other destination profiles use their own starting settings. It does not change part sizes. A folder upload
shares one startup across its files.

Downloads fetching version-bound parts use the scaled S3 policy without an upload startup choice or initial upload probes. Their
initial target is 150, within explicit caps and the limiter; custom tuning uses its own start.

Automatic folder transfers learn a shared target rather than assigning that target to every file. They admit files
with some headroom for growth (target plus one quarter plus one, at least 4), within the explicit file cap or 128.
Reducing the target limits new starts without stopping files already active. Static folder transfers use an
independent part target for each file.

On .NET Framework, `ServicePointManager.DefaultConnectionLimit` also limits connections per server, usually to 2
in desktop and console applications. Raise it if you need more simultaneous parts.

#### When Downloads Use Parts

Parallel downloads currently require HTTPS Amazon S3 general-purpose endpoints. The first ranged response must
establish a strong `ETag`, size and storage object; later requests use `If-Match` and validate each range, length
and tag. Renewed addresses must identify the same object, even if another object's tag matches. Other storage and
specialized S3 endpoints, including FIPS, accelerate and access points, use whole responses.

Before a version is established, a whole-file `200`, `416` (such as an empty file), redirect or missing version
information also leads to a whole response. If storage ignores ranges **after** version binding, the fallback keeps
the version check: `VersionBound` remains true while `Concurrency` becomes null. A response containing an entire
small file is accepted only after all bytes arrive; if cut short, the next attempt determines the mode again.

The SDK rejects web-app responses (`X-Files-Frontend-App`), compressed bodies, incorrect ranges or lengths, and
whole-file responses other than `200`. Bound part requests follow no redirects; other storage redirects must not
leave HTTPS. These checks run before publication.

```csharp title="Capping or Fixing Concurrency"
// Automatic, with a cap of eight simultaneous parts.
var capped = new UploadOptions { MaxConcurrency = 8 };

// A fixed target of four, where Files.com allows parallel parts.
var fixedFour = new UploadOptions
{
    ConcurrencyMode = UploadConcurrencyMode.Static,
    MaxConcurrency = 4,
};

await client.Transfers.UploadFileAsync(localPath, destinationPath, capped);

// Skip initial probes on a known high-throughput network, where the scaled S3 policy applies.
var aggressive = new UploadOptions { Startup = UploadStartup.Aggressive };
```

### Part Sizes

The SDK chooses upload part sizes only for ordinary direct Amazon S3 multipart uploads. Elsewhere—including
remote servers, Agents, S3-compatible storage and append uploads—it follows the sizes Files.com plans.
Downloads use `DownloadOptions.PartSize`, 8 MiB by default, when fetching version-bound parts.

| Upload `PartSizingMode` | Behavior where the SDK can choose sizes |
| --- | --- |
| `Automatic` (default) | Starts from the file size; tries larger parts after concurrency growth stops improving throughput, and keeps them only if faster. Customize with `AutomaticPartSizing`. |
| `Fixed` | Uses `PartSize` for every part except the last. Invalid sizes fail before sending a part. |
| `ServerPlanned` | Uses Files.com's planned sizes. |

Chosen upload sizes stay within 5 MiB minimum except the last part, 1 GiB maximum, at most 10,000 parts (or fewer if
Files.com grants fewer), and a 5 TiB file limit. Larger files use server-planned sizes. Files.com and the destination
may impose further limits. If the file cannot fit the allowed parts, automatic mode falls back to server-planned
sizes; fixed sizes or insufficient memory-backed part capacity fail before sending.

Folder uploads test automatic size increases one file at a time, using that file's throughput at a stable shared
concurrency target. The target holds during a size trial. Throttling or a temporary failure from any file ends the
trial and restores the earlier size. Serial files do not force the other files to become serial.

Inspect `FinalProgress.PartSizing` for the source, fallback reason, limits, next size, largest part and growth count.

```csharp title="Choosing Fixed Upload Parts"
var options = new UploadOptions
{
    PartSizingMode = UploadPartSizingMode.Fixed,
    PartSize = 32 * 1024 * 1024,
    ConcurrencyMode = UploadConcurrencyMode.Static,
    MaxConcurrency = 4,
};

UploadResult result = await client.Transfers.UploadFileAsync(localPath, destinationPath, options);
Console.WriteLine($"Part-size source: {result.FinalProgress.PartSizing.Source}");
```

### Sharing Limits Between Uploads

Use a `TransferLimiter` to cap simultaneous part requests and buffered file bytes across uploads and downloads,
including transfers from different clients. Waiting transfers take turns for resources. A request slot is held
while sending, not during a retry wait, and failures or cancellation release reserved resources.

A limiter bounds requests and buffers; it does not make separate automatic transfers learn one target. Only files
within one automatic folder transfer share a learned target.

Without an explicit limiter, transfers use 128 MiB of buffer space. Single-file request slots follow the configured
maximum target; a static download defaults to 4, and an automatic download without an explicit cap or custom tuning
allows up to 1024. Automatic folder transfers allow at least 8 slots for requests outside their shared target,
such as version checks and whole-response downloads; static folder transfers use 8 slots.

File uploads read through small copy buffers rather than keeping whole parts in memory. File downloads reserve a
copy buffer of at most 80 KiB; stream downloads reserve the part itself, since ahead-of-order bytes must wait in
memory. Both reserve buffer space before a request slot. Stream uploads retain memory-backed parts until accepted,
or use temporary files. Inspect `ActiveRequests`, `BufferedBytes`, `QueuedRequests` and `QueuedBufferReservations`
on the limiter to see current usage.

```csharp title="Sharing a Transfer Limit"
var limiter = new TransferLimiter(maxConcurrentRequests: 8, maxBufferedBytes: 64 * 1024 * 1024);
var options = new UploadOptions { Limiter = limiter };

await Task.WhenAll(localPaths.Select(localPath =>
    client.Transfers.UploadFileAsync(localPath, "backups/" + Path.GetFileName(localPath), options)));
```

### Diagnostics

Set `UploadOptions.Trace` for detailed upload records. Keep the handler quick; reports follow the same callback
rules as progress.

| Record | Contents |
| --- | --- |
| `UploadAttemptTrace` | Each answered or failed part request before retry: status, throttling, network time, admission wait and sent bytes. Cancelled requests and source-read failures are not recorded. |
| `UploadDecisionTrace` | Concurrency changes and the throughput behind them. |
| `UploadSizingTrace` | Part-size decisions and the throughput behind them. |

In a folder upload, shared-target decisions are reported once through the deciding file's `FileOptions.Trace`,
with the remote folder as `Path`. Attempts and size decisions keep their file's path. Downloads use
`DownloadProgress.Concurrency` and have no trace stream. Traces, snapshots and ordinary SDK logs omit part addresses,
headers and credentials.

```csharp title="Tracing Concurrency Decisions"
var options = new UploadOptions
{
    Trace = new Progress<UploadTraceEvent>(record =>
    {
        if (record is UploadDecisionTrace decision)
        {
            Console.WriteLine($"{decision.Elapsed}: {decision.Decision} " +
                $"{decision.PreviousTarget} -> {decision.Target}");
        }
    }),
};
```

### Earlier Upload and Download Methods

Existing `RemoteFile` methods and `client.RemoteFiles` transfer methods remain available:

| Method family | Behavior |
| --- | --- |
| `RemoteFile.UploadFile`, `client.RemoteFiles.UploadFileAsync` | Sends parts serially and returns `true`. Stream overloads dispose the supplied stream. Expiring addresses are renewed for the same part and retries resend the bytes already read. The final request is not retried. |
| `RemoteFile.DownloadFile`, `client.RemoteFiles.DownloadFileAsync` | Fetches the whole file directly into the local file or stream, without staging. Rejects web-app and partial responses before writing. An address refused as expired can be renewed at most twice before any bytes are written. Downloads are not retried after writing bytes. |

Use `client.Transfers` for automatic tuning, progress, staged local downloads, shared limits and file checkpoints.

## Logs

A client logs through the `ILoggerFactory` its configuration names (`FilesConfiguration.LoggerFactory`, as
Configuration describes), under categories named after the SDK's classes, all in the `FilesCom` namespace, such as
`FilesCom.FilesApiService` and `FilesCom.Http.RetryPolicy`: the usual way for an application built on
Microsoft.Extensions.Logging, or a host, and one that configures nothing global. Each client has its own, and
`MinimumLogLevel` sets a level for one client alone. Messages are logged through the client of the operation that logs
them; your own code, before and after an operation, including after opening a remote file to write, keeps its own
logging. Logging never changes what an operation does: if the factory, `IsEnabled` or `Log` throws, that message is
lost, and the operation goes on as it would have, with its own result or failure.

A client without a logger factory logs with [log4net](https://logging.apache.org/log4net/), under loggers of the same
names, as the SDK always has. It logs nothing until your application configures log4net, and then goes wherever your
configuration sends it, at the levels you choose.

To enable logging, create a file named `log4net.config` in the same directory as
the application with the contents as shown.

Then, in the application, use that file to configure `log4net`:

```csharp
log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(System.IO.Path.Combine(System.AppContext.BaseDirectory, "log4net.config")));
```

The `FilesCom` logger sets the SDK's level alone, whatever the level of your application's other loggers.

```xml title="log4net.config"
<?xml version="1.0" encoding="UTF-8" ?>
<log4net>
  <root>
    <level value="WARN" />
    <appender-ref ref="console" />
  </root>
  <logger name="FilesCom">
    <level value="INFO" />
  </logger>
  <appender name="console" type="log4net.Appender.ConsoleAppender">
    <layout type="log4net.Layout.PatternLayout">
      <conversionPattern value="%date [%thread] %-5level %logger - %message%newline" />
    </layout>
  </appender>
</log4net>
```

### Log Levels

- `ERROR`: a request or transfer that failed, by its exception's type and HTTP status.
- `WARN`: cleanup that failed, such as deleting a temporary file, which never changes an operation's outcome.
- `INFO`: each API request, by its method and resource, such as `PATCH /users`; each retry, with its reason and
  wait; each upload part request; and how clients are set up.
- `DEBUG`: the details: each API request's full address and body, each API response's body, every failure with its
  exception, and how uploads choose their part sizes and concurrency.

Levels above `DEBUG` omit request and response paths, queries, bodies, error text, signed storage addresses and credentials. `DEBUG` logs full details for troubleshooting, including request and response bodies. Request bodies can contain passwords and other secrets you send. File bytes are never logged. Enable `DEBUG` only where the logs are protected.

## Mock Server

Files.com publishes a mock Files.com API server, which is useful for testing your use of the Files.com
SDKs and other direct integrations against the Files.com API in an integration test environment.
It never checks credentials: send any placeholder API key, and never use real Files.com credentials
with it.

### Docker Quick Start

These steps start the mock server from its published Docker image and point your tests at it:

1. Install [Docker Desktop](https://www.docker.com/products/docker-desktop/), or another Docker installation, and
   start it.
2. Download the image from [Docker Hub](https://hub.docker.com/r/filescom/files-mock-server):
   `docker pull filescom/files-mock-server:latest`
3. Start the server: `docker run --rm -p 127.0.0.1:40410:4041 filescom/files-mock-server:latest`.
   It listens at `http://127.0.0.1:40410`, which only your computer can reach. The command keeps running in that
   terminal, so run your tests from another one. Press Ctrl+C in the server's terminal to stop it.
4. Set the base URL of your SDK or client to `http://127.0.0.1:40410`, as
   [Configuration](https://developers.files.com/net/overview/configuration) describes. Send any placeholder API
   key, and never use real Files.com credentials with it.

```shell title="Start the Mock Server"
## Download the image
docker pull filescom/files-mock-server:latest

## Legacy mode, at http://127.0.0.1:40410. Press Ctrl+C to stop it.
docker run --rm -p 127.0.0.1:40410:4041 filescom/files-mock-server:latest
```

### Server Modes

The quick start runs legacy mode, the default. Legacy mode checks required parameters and parameter types, then
returns a fixed example response for each API endpoint. It does not maintain state and it does not deeply inspect
your submissions for correctness, which makes it useful for testing basic network operations and JSON encoding for
your SDK or API client.

Simulation mode keeps records, files and folders in memory, so a test can create, list, update and delete resources,
upload a file and download the same bytes, and make chosen requests fail, stall or lose their connection on purpose.
Requests it does not simulate fail with a clear error instead of returning an example response.

`FILES_MOCK_MODE` is read once at startup. Leaving it unset or empty, or setting it to `legacy`, starts legacy
mode. Setting it to `simulation` starts simulation mode. Any other value stops startup with an error. Running
simulation mode in Docker takes more settings than the quick start command, and the README's
[Docker Image Usage](https://github.com/Files-com/files-mock-server#docker-image-usage) section gives that command.
The image's `latest` tag moves to whichever server was published last, so it need not include simulation mode. To run
exactly the server the README describes, build the image from the source. The README also describes simulation mode's
control endpoints, the operations it simulates and its limits.

The source code is on [GitHub](https://github.com/Files-com/files-mock-server). To run the server from its source
with Ruby and Bundler instead of Docker, follow the README's
[Requirements](https://github.com/Files-com/files-mock-server#requirements) and
[Local Ruby Usage](https://github.com/Files-com/files-mock-server#local-ruby-usage).
