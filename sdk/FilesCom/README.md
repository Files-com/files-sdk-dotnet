# Files.com .NET Client

The Files.com .NET client library provides convenient access to the Files.com API from applications using the .NET framework.


## Installation

To install the package:
    dotnet add package FilesCom

Fetch the dependencies:
    dotnet restore


## Frameworks Supported 

- .NET 5

- .NET Core 2.1, 2.2, 3.0, 3.1

- .NET Standard 2.0

- .NET Framework 4.6.1, 4.6.2, 4.7, 4.7.1, 4.7.2, 4.8

We are open to adding support for additional frameworks where we can.  Please send us your requests.


## Usage

### Authentication

There are multiple ways to authenticate to the API.


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

TODO: Add appsettings.json support for .NET CORE


### Per-Instance API Key

When instantiating a client, FilesConfiguration can be set directly:
```csharp
    using FilesCom;

    FilesConfiguration filesConfig = new FilesConfiguration();
    filesConfig.ApiKey = "my-key";
    FilesClient client = new FilesClient(filesConfig);
```

### Files Configuration

You can set configuration for default client sessions by adding them to the app.config or appsettings.json as with the ApiKey above,
or you may set them on a config object for per-session configuration.

* `ApiKey` - Required. Must be set in either app.config, appsettings.json, or per-session.
* `BaseUrl` - to point this client at an on-premise installation of Files.com, set its URL here.
* `OpenTimeout` - open timeout in seconds (default: 30)
* `ReadTimeout` - read timeout in seconds (default: 80)
* `InitialNetworkRetryDelay` - initial retry delay in seconds (default: 0.5)
* `MaxNetworkRetries` - max retries (default: 3)
* `MaxNetworkRetryDelay` - max retry delay in seconds (default: 2)


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

#### Reading a file's text as a Stream
```csharp 
    using(MemoryStream stream = new System.IO.MemoryStream()) {
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

#### List root folder
```csharp 
    var files = await Folder.ListFor("/", null, null);
    foreach (var file in files)
    {
        Console.WriteLine("- Path: {0}", file.Path);
    }
```

#### List folder with a filter
```csharp 
    Dictionary<string, object> parameters = new Dictionary<string, object>(){
        { "filter", "*.png"},
    };
    var files = await Folder.ListFor("/", parameters, null);
    foreach (var file in files)
    {
        Console.WriteLine("- Path: {0}", file.Path);
    }
```


## Getting Support

The Files.com team is happy to help with any SDK Integration challenges you may face.

Just email support@files.com and we'll get the process started.

