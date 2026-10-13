# Files.com .NET CLI Example

A small command-line tool that lists and shows users, and uploads and downloads files, with the Files.com .NET SDK.
Each command makes one `FilesClient`, passes the same `CancellationToken` to every request and stops cleanly on
Ctrl+C.

## Signing in

Use one of these, in this order of preference:

- `--user NAME`, with the password in `FILES_PASSWORD` and, if the account uses one, a one-time password in
  `FILES_OTP`. The command signs in, does its work and signs out.
- An API key in `FILES_API_KEY`.
- An `ApiKey` or `SessionId` in the configuration file: copy `app.config.template` to `app.config` and edit it. The
  build copies it next to the program as `files-cli.dll.config`.

The configuration file also holds the other client settings. Setting `BaseUrl` to your site's URL, such as
`https://MY-SUBDOMAIN.files.com`, is recommended.

Passwords and one-time passwords are not accepted as options, because the command line is saved in shell history
and visible to other processes.

## Commands

```shell
export FILES_API_KEY=YOUR_API_KEY
dotnet run -- list-users
dotnet run -- show-user --id 1
dotnet run -- upload-file --local-path report.pdf --path reports/report.pdf
dotnet run -- download-file --path reports/report.pdf --local-path report-copy.pdf
```

`download-file` without `--local-path` saves the file under its own name in the current directory. Run
`dotnet run -- --help`, or `--help` after a command, to see every option.

## Exit codes

| Code | Meaning |
|------|---------|
| 0 | The command succeeded. |
| 1 | The arguments or sign-in settings are unusable. Nothing was sent to Files.com. |
| 2 | Files.com reported an error, or a local file could not be read or written. |
| 130 | Cancelled with Ctrl+C. Requests Files.com already received are not undone, and a partly downloaded file keeps what was written. |

## Changes in version 2.0

- The commands are `list-users`, `show-user`, `upload-file` and `download-file`. The earlier names, such as
  `ListUsers`, still work, as does `--localPath`.
- `--password` and `--otp` were removed. Set `FILES_PASSWORD` and `FILES_OTP` instead.
- `show-user --id` accepts 64-bit user IDs, and `upload-file` requires `--local-path`.
- The example no longer turns on log4net, whose configuration printed every SDK debug message, including request
  bodies, to the console. The SDK's Logs documentation explains how to turn logging on.
