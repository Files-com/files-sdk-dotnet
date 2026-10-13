# Upgrading from FilesCom 1.x to 2.0

Version 2.0 of the FilesCom package moves the SDK to supported .NET releases and to current versions of its
dependencies, and makes objects, lists and errors behave consistently. What can need changing is your
application's target framework, its references to the same dependencies and, on .NET Framework, its binding
redirects, and code that relies on one of the changes in behavior listed below.

## Frameworks

| Your application targets | FilesCom 2.0 |
|--------------------------|--------------|
| .NET 10 | Supported, using the package's .NET 10 build |
| .NET 8 | Supported until November 10, 2026, when Microsoft's support for .NET 8 ends |
| .NET Framework 4.8 or 4.8.1 | Supported, using the package's .NET Standard 2.0 build. Testing of 2.0 on .NET Framework is not complete yet |
| .NET 6, .NET 5 or .NET Core | Not supported. Move to .NET 10 or .NET 8 |
| .NET Framework 4.6.1 to 4.7.2 | Not supported. Move to .NET Framework 4.8 or 4.8.1, or to .NET 10 or .NET 8 |

Files.com develops and tests FilesCom 2.0 on .NET 10 and .NET 8. Until its testing on .NET Framework is complete,
test your .NET Framework application with 2.0 before you deploy the upgrade, and contact Files.com Support if
anything does not work as you expect.

The package no longer contains builds for .NET 6, .NET 5, .NET Core 2.1 to 3.1 or .NET Standard 2.1. NuGet still
installs the .NET Standard 2.0 build into an application on an unsupported framework. On .NET 6, .NET 5, .NET Core
and .NET Framework 4.6.1, Microsoft's packages warn at build time that they do not support the framework; on .NET
Framework 4.6.2 to 4.7.2 the build shows no warning. Files.com does not test or support these combinations.
Earlier versions of FilesCom remain available on NuGet for applications that cannot move yet.

## Dependencies

FilesCom 2.0 requires these versions, or later, of the packages it uses:

- log4net 3.5.0
- Microsoft.Extensions.DependencyInjection and Microsoft.Extensions.Http 10.0.12
- Polly.Core 8.8.0
- System.Configuration.ConfigurationManager 10.0.12
- System.Text.Json 10.0.12, except on .NET 10, which includes it
- Microsoft.Bcl.AsyncInterfaces 10.0.12, on .NET Framework and .NET Standard only, which .NET 8 and .NET 10 include

It no longer depends on System.Drawing.Common, Microsoft.Extensions.Hosting, Microsoft.Extensions.Http.Polly or
the Polly package: an application that uses them itself now needs its own references. If your project references an older version of one of these
packages, update that reference: NuGet reports it as a package downgrade (NU1605), which .NET SDK projects treat as
an error by default. If your application configures log4net itself, review the changes in log4net 3.

## .NET Framework applications

The dependencies need binding redirects in the configuration of the application that runs, such as its
`.exe.config`. An executable project writes them when it builds if both `AutoGenerateBindingRedirects` and
`GenerateBindingRedirectsOutputType` are true, as they are by default in SDK-style executable projects. A library's
own configuration does not apply, and other hosts, such as web applications, need the redirects in their own
configuration. If you maintain binding redirects by hand, update them to the versions your build resolves. After
upgrading, check the redirects in the configuration of the application that actually runs.

Windows PowerShell 5.1 does not use an application's binding redirects. Loading the SDK's assemblies directly into
it, without another way to resolve these dependency versions, has failed in Files.com's testing.

## Configuration files

The configuration section type is `FilesCom.FilesConfiguration, FilesCom`, as in the
[Authentication](README.md#authentication) example. Earlier documentation showed `Files.FilesConfiguration, Files.com`,
which does not load.

## Changes in behavior

Exceptions:

- Incorrect use of the SDK throws an `SdkException`, before any request is sent: `AuthenticationException` when
  there are no credentials, including a static method, such as `User.Find`, called before any `FilesClient` has been
  made, with no `api_key` or `session_id` in its options (was `InvalidOperationException`);
  `MissingParameterException` when a required parameter or argument, such as `RemoteFile.UploadFile`'s local path, or
  an object's ID is missing (was `ArgumentNullException` or `ArgumentException`); and `InvalidParameterException`
  for a parameter, option or setting of the wrong type, out of range or contradicting another (was
  `ArgumentException` or `ArgumentOutOfRangeException`). Where a validation names what it refuses, as those of
  method arguments, options, settings and an operation's parameters do, `ParamName` gives the name; it is null where
  an exception names none. The same holds for what is new in 2.0, such as `HttpTransport`, `FileAttachment`,
  `client.Transfers` and their options.
  Cancellation is still an `OperationCanceledException`, a failure of your own stream, handler or callback is thrown
  as it is, and a disposed client throws `ObjectDisposedException`.
- Any other use of the default client before a `FilesClient` has been made throws an `SdkException` that says to
  make one, before any request is sent (was `InvalidOperationException`): a static method whose options carry an
  `api_key` or `session_id`, `Session.Create`, a method of an object made without a client, and
  `FilesClient.StreamDownload` and `ChunkUpload`.
- An error response is thrown as the exception its error type names, whatever its HTTP status. A type the SDK
  does not know is an `ApiException`, or its group's exception, such as `NotFoundException`, for a new type in a
  known group. It used to fail with an unrelated exception, such as `ArgumentNullException`.
- An exception's message leaves out the user information, query and fragment of any URL in the error text, where
  a signed URL keeps its credentials. `ApiException.error` keeps the text as sent. The SDK's logs below DEBUG
  record a failed request by its exception type and HTTP status, without the error text, which is logged at DEBUG.
- A response that cannot be read is an `InvalidResponseException`, which is now an `ApiConnectionException`, with
  the HTTP status and headers of an error response. That includes a malformed value, such as a boolean that is not
  `true` or `false`, which used to be a `FormatException`, and a JSON `null` from an operation that returns an
  object, such as `Find` or `Create`, which used to return `null`. Neither its message nor an inner exception
  includes any part of the response, which is logged at the DEBUG level instead.

Objects:

- `Update` and `UpdateAsync` update the object they are called on with the attributes Files.com returns, as
  `Save` and `SaveAsync` now do too. They still return a separate object, which keeps the options of its request.
- `Save` and `SaveAsync` send only the attributes set on the object, in its constructor or with its properties, or
  changed in place, such as an element of an array, since it was made or last came from Files.com, and only those
  its create or update operation takes. They send each in the form its parameter takes, such as a `DateTime` as
  ISO 8601 text and a `decimal` as its exact digits. They no longer send the defaults of attributes that were never set, which made saving a new
  `User` fail, or send back the values Files.com gave the object. A `null` that is set is rejected with
  `InvalidParameterException` before any request is sent, as a `null` parameter is.
- An object made with its constructor keeps its own copies of the attributes and options dictionaries.
- Decimal properties, such as `Invoice.Amount` and `RemoteMountBackend.MinFreeCpu`, are `decimal?`, as number
  properties are `long?` and `double?`: null when the attribute was never set, which threw
  `NullReferenceException`, or when Files.com returns it as null, which read as 0. Use `.Value` or
  `.GetValueOrDefault()` where a `decimal` is needed.
- `FilesCom.Models.Action`, the entries of History lists, is now `HistoryAction`, so that it no longer conflicts
  with `System.Action` in a file that uses both namespaces. Only its name changed.
- The object methods that return `void`, such as `Destroy`, are obsolete: they neither wait for the request nor
  report a failure. Use their `Async` forms, such as `DestroyAsync`.

Retries and timeouts:

- The defaults are those of every Files.com SDK: `ReadTimeout` is 80 seconds (was 60) and
  `InitialNetworkRequestDelay` is 0.1 seconds (was 0.5). API requests wait 0.1, 0.2 and 0.4 seconds before their
  three retries, doubling up to `MaxNetworkRetryDelay`, instead of a fixed first and last wait with random ones
  between.
- Requests to storage addresses, for uploads and downloads, no longer use those delays: after HTTP 429 or 503 they
  wait what the server's `Retry-After` asks, however long, or not at all without one, and 0.1 seconds after other
  failures. A `Retry-After` longer than the platform can wait is not retried: the 429 or 503 error is thrown. Use a
  `CancellationToken` to bound how long an operation may wait.
- HTTP 429 is retried, and 501, TLS certificate failures and redirects the SDK does not follow are not. A request
  that only asks for something, such as a `GET`, is retried after a connection failure or a timeout as before. A
  request that can change something, such as `CreateAsync`, `UpdateAsync` or `DeleteAsync`, is now retried only when
  it certainly was not carried out: it failed before any of it was sent, such as when it could not connect, or
  Files.com answered that the site is served from another region. After a lost answer, a timeout, or any error
  response, readable or not, whatever its status or type, it is not sent again, since it may have taken effect, and
  its exception, whichever `SdkException` it is, has the new `RequestMayHaveBeenProcessed` set, even with
  `MaxNetworkRetries` of 0. False alone does not show that sending again is safe: see Maximum Network Retries under
  [Configuration](README.md#configuration). 1.x sent such a request again, which could create a user or folder twice. Where you relied on that,
  check whether the change took effect, then send it again yourself. The request that completes a
  `client.RemoteFiles.UploadFileAsync` upload is now never retried, as `client.Transfers` already did; a redirect
  Files.com answers a change with is followed, as before.
- A page of a list with no items that gives back the cursor it was loaded with now throws `InvalidResponseException`,
  where it ended the list as if it were complete.
- `client.RemoteFiles.UploadFileAsync` and `DownloadFileAsync`, like `client.Transfers`, give a part or a download a
  new address and send it again when storage answers that its address has expired, even before its expiry time.
- `ReadTimeout` limits how long a request waits for its response and then for each part of it, instead of only a
  download's wait for its headers; the new `WriteTimeout` does the same for sending an upload part. A request that
  runs out of either fails with `NetworkTimeoutException`, an `ApiConnectionException`, where a download's header
  timeout was an `OperationCanceledException`.
- A connection that fails while a download is arriving, with `client.RemoteFiles` or `client.Transfers`, is an
  `ApiConnectionException`, where it was the `IOException` of the network stream; a failure of your own destination
  stream is still thrown as it is. A download that has written data into a stream is not retried.
- `RemoteFile.DownloadFile` and `client.RemoteFiles.DownloadFileAsync` refuse an answer from the Files.com web app
  (`X-Files-Frontend-App`), and a partial (`206`) answer to their request for the whole file, with
  `InvalidResponseException` before writing anything. Both used to write such an answer as the file.
- A signed storage address that is about to expire is renewed with Files.com before a request is sent to it.
- A client keeps the `ConnectTimeout` it was made with. It applies on .NET 8 and later to the client's own
  handlers and to a factory's `SocketsHttpHandler` without one of its own; other handlers you give through
  `HttpTransport` keep their own connection settings. On .NET Framework there is
  no separate connect timeout: connecting counts toward the `ReadTimeout` or `WriteTimeout` that follows it, as
  Connect Timeout under [Configuration](README.md#configuration) describes. Settings out of range, such as a negative
  `MaxNetworkRetries` or a `ReadTimeout` of 0, throw `InvalidParameterException`, naming the setting, before any
  request is sent.
- Upload parts of `client.RemoteFiles.UploadFileAsync` follow storage redirects by the rules `client.Transfers` uses,
  instead of .NET's own.

File parameters:

- File parameters, such as `avatar_file`, `attachment_file` and `logo_file`, take a `FileAttachment`, and their files
  are sent, as [File Attachments](README.md#file-attachments) describes. They were typed `System.Net.Http.ByteArrayContent`, which was serialized as
  JSON without its bytes, so no file was ever sent; a `ByteArrayContent` is now refused with
  `InvalidParameterException` before anything is sent. Use `FileAttachment.FromBytes(bytes, fileName)` instead. Model
  properties for them, such as `User.AvatarFile`, are `FileAttachment` too.

Regional relocation:

- An API request that Files.com answers with `LockoutRegionMismatchException` is sent again to the site's address in
  another region, as one of the operation's retries, when the answer can be trusted and the address is allowed, as
  Regional Relocation in [Configuration](README.md#configuration) describes. The error used to be thrown at once. To keep that, set
  `FilesConfiguration.RegionRelocation` to `RegionRelocationPolicy.Disabled`; the exception now also says which
  address Files.com named and why the request was not sent there.

Lists:

- `ListAutoPaging` and `AllAsync` always start from the list's first page, even after `LoadNextPageAsync` has
  loaded some, and continue past empty pages. They no longer change `data` or `HasNextPage`.
- A list keeps a copy of the parameters it was made with, and loading pages never changes them.
- A page that is not a JSON array is an `InvalidResponseException`.

## Also new in 2.0

- Independent clients, made with `FilesClient.Create`, alongside the default client, and disposable clients:
  `Dispose` and `DisposeAsync` stop a client's operations and close its connections. See
  [Configuration](README.md#configuration).
- `HttpTransport`, to send a client's requests through your own handlers, for a proxy or instrumentation. See
  [Configuration](README.md#configuration).
- Cancellation of requests, pages and transfers with a `CancellationToken`. See [Cancellation](README.md#cancellation).
- `client.Transfers`, which uploads files and streams in parallel parts, with part sizes it chooses where Files.com
  allows, and streams whose length is not known in advance. It also downloads files bound to one version, in checked
  parallel parts, transfers whole folders sharing one limiter, and continues stopped file and folder transfers from
  checkpoints. See [File Transfers](README.md#file-transfers).
- Download checkpoints that record a digest of each part (format 4), written on Linux file systems that do not record
  when files were created, so that such a download can still be continued. Versions of this SDK that read only earlier
  formats refuse them: before changing to one, finish a stopped download or start it over with a new `CheckpointPath`.
  See Continuing Stopped Transfers in [File Transfers](README.md#file-transfers).
- `FilesConfiguration.LoggerFactory` and `MinimumLogLevel`, to log a client through your application's
  `ILoggerFactory`, at a level of its own, without configuring log4net; and `ProductName` and `ProductVersion`, to name
  your application in the `User-Agent` header before the SDK. See [Configuration](README.md#configuration).
- `client.FileSystem`, remote files as streams to read and write, with listing, existence, and deleting, copying and
  moving that return once Files.com has finished; and `FileAction.WaitAsync`, to wait for a copy or move Files.com
  finishes after it answers. See [File Transfers](README.md#file-transfers).
- `ListAutoPagingAsync`, which goes through a list with `await foreach`, a page at a time. See
  [Pagination](README.md#pagination).
- `FileAttachment`, to send files as parameters from a path, bytes or a stream. See
  [File Attachments](README.md#file-attachments).
- `RegionRelocationPolicy`, to choose whether and where operations move when a site is served from another region.
  See [Configuration](README.md#configuration).
- `UploadLimitException`, the `InvalidOperationException` that `client.Transfers` uploads throw when they need more
  than a limit allows, with a `Limit` that says which. See [File Transfers](README.md#file-transfers).
- XML documentation in the package, so editors show it for installed versions.
