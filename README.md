﻿# Files.com .NET Client

The Files.com .NET client library provides convenient access to the Files.com API from applications using the .NET framework.


## Installation

To install the package:
    dotnet add package FilesCom

Fetch the dependencies:
    dotnet restore


## Frameworks Supported

- .NET 6

- .NET 5

- .NET Core 2.1, 2.2, 3.0, 3.1

- .NET Standard 2.0

- .NET Framework 4.6.1, 4.6.2, 4.7, 4.7.1, 4.7.2, 4.8

.NET Framework versions below 4.6.1 are officially in end-of-life status according to Microsoft.  As a policy, Files.com does not support integrations which are considered end-of-life by their vendor.

If you need support for a newer framework version, please ask, and we'll be happy to add it.  It will most likely work out of the box anyway.


## Usage

### Authentication

There are multiple ways to authenticate in the Files.com SDK.


### app.config/web.config

In the app.config or web.config of a .NET application, add the following section:

    <configSections>
        <sectionGroup name="files.com">
            <section name="filesConfiguration" type="FilesCom.FilesConfiguration, FilesCom"/>
        </sectionGroup>
    </configSections>
    <files.com>
        <filesConfiguration ApiKey="my-key" />
    </files.com>


### appsettings.json

You can also use `appsettings.json` from .NET CORE to set the `ApiKey` property.


### Per-Instance API Key

When instantiating a client, FilesConfiguration can be set directly:
```csharp
    using FilesCom;

    FilesConfiguration filesConfig = new FilesConfiguration();
    filesConfig.ApiKey = "my-key";
    FilesClient client = new FilesClient(filesConfig);
```


### Files.com Configuration

You can set configuration for default client sessions by adding them to the `app.config` or `appsettings.json` as with the ApiKey above,
or you may set them on a config object for per-session configuration.

* `ApiKey` - Required. Must be set in either app.config, appsettings.json, or per-session.
* `BaseUrl` - to point this client at an on-premise installation of Files.com, set its URL here.
* `OpenTimeout` - open timeout in seconds (default: 30)
* `ReadTimeout` - read timeout in seconds (default: 80)
* `InitialNetworkRetryDelay` - initial retry delay in seconds (default: 0.5)
* `MaxNetworkRetries` - max retries (default: 3)
* `MaxNetworkRetryDelay` - max retry delay in seconds (default: 2)


### Pagination

For endpoints with pagination, operations such as `List` will return a `FilesList` object. This object allows for accessing pages of
results asyncronously with `LoadNextPage()` and `All()` or with auto-pagination using `ListAutoPaging()`.


### Error Handling

Unexpected errors when attempting to connect to the API inherit from the base level `SdkException` class. They all contain a `message`
to describe what went wrong.


#### Unable to connect to the API

```csharp
try
{
    await Folder.ListFor("/").All();
}
catch (ApiConnectionException e)
{
    Console.WriteLine("Unable to list root folder: " + e.message);
}
```

Errors from the API inherit from `ApiException`. They all contain more parameters to describe the error such as `httpCode`, `error`, `detail`, etc.


#### Path does not exist

```csharp
try
{
    await Folder.ListFor("/doesnotexist").All();
}
catch (NotFoundException e)
{
    Console.WriteLine($"Unable to list folder: <{e.httpStatus}> {e.error}");
}
```


### Examples

#### Writing a file example

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


#### Uploading a file from a Stream

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


#### Reading a file's text as a Stream

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


#### Reading a file and writing it to your local drive.

```csharp
    var downloadResponse = await RemoteFile.DownloadFile("Remote-Path/file.txt", "Local-Path/file.txt");
    if (downloadResponse) {
        Console.WriteLine("-- Download result: {0}" + downloadResponse+.ToString());
    } else {
        Console.WriteLine("-- Download result: Failed");
    }
```


#### List root folder (loads all pages into memory)

```csharp
    var files = Folder.ListFor("/", null, null);
    foreach (var file in await files.All())
    {
        Console.WriteLine("- Path: {0}", file.Path);
    }
```


#### List root folder with auto pagination (loads each page into memory)

```csharp
    foreach (var file in Folder.ListFor("/").ListAutoPaging())
    {
        Console.WriteLine("- Path: {0}", file.Path);
    }
```


#### List root folder with manual pagination (loads each page into memory)

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


#### List folder with a filter

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


#### Comparing Case insensitive files and paths

For related documentation see [Case Sensitivity Documentation](https://www.files.com/docs/files-and-folders/file-system-semantics/case-sensitivity).
```csharp
    if(PathUtil.same("Fïłèńämê.Txt", "filename.txt")) {
        Console.WriteLine("Paths are the same");
    }
```

#### Logging
To enable logging, create a file named `log4net.config` in the same directory as the application with the following contents:
```xml
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
Then, in the application, use that file to configure `log4net`:
```csharp
log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("./log4net.config"));
```


## Getting Support

The Files.com team is happy to help with any SDK Integration challenges you may face.

Just email support@files.com and we'll get the process started.
