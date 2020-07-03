# Files.com .NET Client

The Files.com .NET client library provides convenient access to the Files.com API from applications using the .NET framework.

## Installation

To install the package:
    dotnet add package Files.com

Fetch the dependencies:
    dotnet restore

## Requirements

 * .NET Standard 2.0
 * .NET Framework 4.7

## Usage

### Authentication

There are multiple ways to authenticate to the API.

### app.config/web.config

In the app.config or web.config of a .NET application, add the following section:

    <configSections>
        <sectionGroup name="files.com">
            <section name="filesConfiguration" type="Files.com.FilesConfiguration, Files.com"/>
        </sectionGroup>
    </configSections>
    <files.com>
        <filesConfiguration ApiKey="my-key" />
    </files.com>

### appsettings.json

TODO: Add appsettings.json support for .NET CORE
   
### Per-Instance API Key

When instantiating a client, FilesConfiguration can be set directly:

    using Files.com;

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

