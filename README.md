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

    using FilesCom;

    FilesConfiguration filesConfig = new FilesConfiguration();
    filesConfig.ApiKey = "my-key";
    FilesClient client = new FilesClient(filesConfig);


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


## Getting Support

The Files.com team is happy to help with any SDK Integration challenges you may face.

Just email support@files.com and we'll get the process started.

