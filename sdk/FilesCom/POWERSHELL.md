# FilesCom.PowerShell

FilesCom.PowerShell is the Files.com PowerShell module: a binary module, built on this .NET SDK, with a command for
every operation of the Files.com API and commands that upload and download files. It is distributed on NuGet.org, not
the PowerShell Gallery, as the package `FilesCom.PowerShell`.

## Availability

The module is distributed through NuGet.org as `FilesCom.PowerShell`. If
[its page on NuGet.org](https://www.nuget.org/packages/FilesCom.PowerShell) lists no version, none has been published
yet, and the commands below find nothing. In those commands, replace `<version>` with the exact version you want, as
that page lists it.

## Requirements

- PowerShell 7.4 or later, the Core edition, on Windows, Linux or macOS. `$PSVersionTable.PSVersion` shows the
  version of the current session.
- Windows PowerShell 5.1 is not supported. The module's manifest requires PowerShell 7.4 and lists only the Core
  edition, so Windows PowerShell does not import it; use `pwsh`.
- Microsoft.PowerShell.PSResourceGet to install it, which PowerShell 7.4 and later include. Nothing is needed from
  the PowerShell Gallery.
- Access to `https://api.nuget.org`, or a package downloaded elsewhere: see [Troubleshooting](#troubleshooting).
- A Files.com API key, or another way of signing in that `Connect-FilesAccount` supports.

## What the package holds

The package holds everything the module needs, so it depends on no other PowerShell module. NuGet.org's v3 feed does
not install a PowerShell resource's dependencies, which is why they are all in the package:

- the module's own assemblies, and this SDK with every .NET assembly it depends on, which the module loads in a
  load context of its own, apart from the assemblies other modules load;
- `ThirdPartyNotices.txt`, naming every package the module ships with that package's license, and the module's
  license;
- help: `Get-Help <command> -Full` for every command, and the `about_FilesCom` topic;
- example scripts for common tasks, in the module's `examples` folder.

The module's version is its own, starting at 1.0.0, independent of this SDK's version. `FilesCom.BuildInfo.json`, in
the installed module's folder, records the exact version and SHA-256 of the `FilesCom` package it was built on.

## Installing

Register NuGet.org as a PSResourceGet repository, once per user:

```powershell
Register-PSResourceRepository -Name NuGetGallery -Uri https://api.nuget.org/v3/index.json
```

`Get-PSResourceRepository` lists the repositories already registered; if NuGet.org is there under another name, give
that name to `-Repository` below. Microsoft describes NuGet.org as a PSResourceGet repository in
[Supported repositories](https://learn.microsoft.com/powershell/gallery/powershellget/supported-repositories).

Install an exact version for the current user. Use the package's exact name: wildcard searches and searches by
command name are limited on NuGet.org and may not find it.

```powershell
Install-PSResource -Name FilesCom.PowerShell -Repository NuGetGallery -Version <version> -Scope CurrentUser
```

A repository registered this way is untrusted, so `Install-PSResource` asks before it installs from it. Answer once
you have checked the name, version and owners on the package's page. Trusting the repository, with
`Set-PSResourceRepository -Name NuGetGallery -Trusted` or `Install-PSResource -TrustRepository`, stops that question
for every package from NuGet.org; whether to do so is your decision.

Import the version you installed, list its commands and read its help:

```powershell
Import-Module FilesCom.PowerShell -RequiredVersion <version>
Get-Command -Module FilesCom.PowerShell
Get-Help <command> -Full
Get-Help about_FilesCom
```

`Get-Help <command> -Full`, such as `Get-Help Send-FilesFile -Full`, documents a command's parameters and examples,
and `about_FilesCom` explains connections, paging, errors, previewing changes and transfers. It is a binary module:
importing it runs no script. The example scripts run only where the execution policy allows them;
`Get-Help <path to script> -Full` documents each one:

```powershell
Get-ChildItem (Join-Path (Get-Module FilesCom.PowerShell).ModuleBase examples)
```

## Updating

Versions install side by side. Install the new version exactly and list what is installed:

```powershell
Install-PSResource -Name FilesCom.PowerShell -Repository NuGetGallery -Version <version> -Scope CurrentUser
Get-InstalledPSResource -Name FilesCom.PowerShell
```

Then start a new PowerShell session. A session that imported the module keeps its .NET assemblies loaded, so neither
`Remove-Module` nor a second `Import-Module` switches versions. In the new session, choose the version with
`Import-Module FilesCom.PowerShell -RequiredVersion <version>`; without `-RequiredVersion`, PowerShell loads the
highest version installed.

## Uninstalling

Close the PowerShell sessions that imported the version, then remove it, and repeat for each version
`Get-InstalledPSResource -Name FilesCom.PowerShell` lists:

```powershell
Uninstall-PSResource -Name FilesCom.PowerShell -Version <version> -Scope CurrentUser
```

`Unregister-PSResourceRepository -Name NuGetGallery` removes the repository once no other package needs it.

## Troubleshooting

- **The package is not found.** Check that the package's page lists the version; that `Get-PSResourceRepository`
  lists `NuGetGallery` with the URI above; and that the command names `-Repository NuGetGallery` and exactly
  `FilesCom.PowerShell`, not `FilesCom`. The module is not in the PowerShell Gallery, and `Install-Module`, from the
  older PowerShellGet, keeps its own list of repositories, which does not include `NuGetGallery`: use
  `Install-PSResource`.
- **A search finds nothing.** Wildcard searches, such as `Find-PSResource -Name FilesCom*`, and searches by command
  name are limited on NuGet.org. Search by the exact name:
  `Find-PSResource -Name FilesCom.PowerShell -Repository NuGetGallery`.
- **Windows PowerShell 5.1.** The module does not import there; `$PSVersionTable.PSEdition` is `Desktop`. Use `pwsh`,
  PowerShell 7.4 or later, where it is `Core`.
- **No access to api.nuget.org.** On a computer that can reach NuGet.org, choose **Download package** on
  `https://www.nuget.org/packages/FilesCom.PowerShell/<version>`. Put the `.nupkg` in a folder of its own, register
  that folder with `Register-PSResourceRepository -Name FilesComLocal -Uri <folder>`, and install as above with
  `-Repository FilesComLocal`.

## Connecting

Connect with an API key from a secret vault, using Microsoft.PowerShell.SecretManagement and a vault you have
registered:

```powershell
$apiKey = Get-Secret -Name FilesComApiKey
Connect-FilesAccount -ApiKey $apiKey -BaseUrl https://mysite.files.com
```

Without a vault, type the key at a prompt, which keeps it out of the command history:

```powershell
Connect-FilesAccount -ApiKey (Read-Host -AsSecureString 'Files.com API key') -BaseUrl https://mysite.files.com
```

`Connect-FilesAccount` can also connect in other ways, such as with a session ID or a username and password; see
[Authentication](https://developers.files.com/powershell/overview/authentication). The new connection becomes the
default of the current runspace, and any command can be given a connection with `-Connection`. Connections live only
in the current session: nothing is saved, and no profile, environment variable or configuration file is read.
`Disconnect-FilesAccount` closes a connection.

## Using the commands

Every Files.com API operation has a command, named with a standard PowerShell verb and a `Files` noun, such as
`Get-FilesUser`, `New-FilesGroup` or `Set-FilesPermission`. Listing changes nothing:

```powershell
Get-FilesChildItem -Path reports | Select-Object DisplayName, Type
Get-FilesUser | Where-Object SiteAdmin | Select-Object Username, Email, LastLoginAt
```

Commands write typed objects that you can filter, sort, export and pipe into other FilesCom commands, which then use
the connection that fetched them. Lists load one page at a time as the pipeline consumes them, and `-First` or
`Select-Object -First` stops further pages. Some fields that hold a credential, such as the key of a new API key,
show as `********` where PowerShell displays them. Treat objects holding such values as sensitive and avoid exporting
or logging them; `Get-Help about_FilesCom` describes how to use such a value when you need it.

## Transfers

`Send-FilesFile` uploads files or streams in parts, several at once where Files.com allows it, retrying failed parts
on their own. `Receive-FilesFile` downloads to a local file or a stream:

```powershell
Send-FilesFile -Path ./report.csv -RemotePath reports/2026/report.csv -MkdirParents
Receive-FilesFile -RemotePath reports/2026/report.csv -LiteralPath ./report-copy.csv
```

A downloaded file is put in place under its name only once it is complete. `Receive-FilesFile` fails rather than
replace something already at `./report-copy.csv`; `-Force` lets it replace a regular file, never a folder or link.
With `-CheckpointPath`, running the same command again continues an interrupted upload, or a download bound to a
version of the file. `Send-FilesDirectory` and `Receive-FilesDirectory` transfer whole folders. See
[Transfers](https://developers.files.com/powershell/overview/transfers) and `Get-Help Send-FilesFile -Full`.

## Previewing and confirming changes

Every command that changes something supports `-WhatIf` and `-Confirm`. `-WhatIf` describes the change without
making it: nothing is sent for it. `-Confirm` asks before the change is made. For example, preview an upload:

```powershell
Send-FilesFile -Path ./report.csv -RemotePath reports/2026/report.csv -MkdirParents -WhatIf
```

Without `-Confirm`, a command asks only when its `ConfirmImpact` is at or above `$ConfirmPreference`, which is `High`
by default. High-impact commands, such as those that delete or that change permissions, credentials or security
settings, therefore ask by default, unless you suppress it with `-Confirm:$false` or a `$ConfirmPreference` such as
`'None'`. Medium-impact commands do not ask under the default preference; give them `-WhatIf` or `-Confirm` to check
a change first.

File parameters, such as a user's avatar, take an attachment. Its local file is opened, and its stream read, only
when the command goes ahead with the change: never under `-WhatIf`, and not when a requested confirmation is
declined.

## Connection settings

Parameters of `Connect-FilesAccount` set how every command using the connection sends requests, for example:

- `-MaxNetworkRetries` (3 by default; 0 turns the engine's retries off), for a request that failed in a way that may
  be temporary. A retry of an API request waits `-RetryDelaySeconds` at first and twice as long each later time, up
  to `-MaxRetryDelaySeconds`; a server that says when to retry is waited for instead, and `-MaxRetryDelaySeconds`
  does not shorten that wait;
- `-ConnectTimeoutSeconds`, `-ReadTimeoutSeconds` and `-WriteTimeoutSeconds`, which bound how long connecting, or a
  request waiting for data, may stall, not how long a transfer takes; and `-OperationTimeout`, off by default, which
  bounds how long any one command may run;
- `-Proxy` and `-ProxyCredential`, instead of the system's proxy settings;
- `-LogLevel`, the least severe engine message the connection's commands produce. With `-Verbose`, commands show each
  request and retry, with URLs that lose credentials and queries.

See [Connect-FilesAccount](https://developers.files.com/powershell/commands/connect-filesaccount) for the others.

## Links

- [Introduction](https://developers.files.com/powershell/overview/introduction)
- [Installation](https://developers.files.com/powershell/overview/installation)
- [Authentication](https://developers.files.com/powershell/overview/authentication)
- [Transfers](https://developers.files.com/powershell/overview/transfers)
- [Command reference](https://developers.files.com/powershell/commands), with a page for each command, such as
  [Send-FilesFile](https://developers.files.com/powershell/commands/send-filesfile)
- [FilesCom.PowerShell on NuGet.org](https://www.nuget.org/packages/FilesCom.PowerShell)
- In the installed module: `Get-Help <command> -Full`, `Get-Help about_FilesCom` and the `examples` folder.
