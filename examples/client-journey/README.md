# Files.com .NET Client Journey Example

This console application works with Files.com through one explicit `FilesClient`:

1. Signs in with an API key, or creates a session from a username and password, using per-request options for
   its first request.
2. Creates a folder and uploads a file into it from a stream.
3. Updates the file through the object `FindAsync` returned.
4. Lists the folder one item per page.
5. Downloads the file into a stream, deletes the folder and, for a session, signs out.

Every step takes the same `CancellationToken`, which is cancelled by Ctrl+C or when the time limit passes.

## Running

Set `FILES_API_KEY`, or `FILES_USERNAME` and `FILES_PASSWORD`, then run it with your site's URL and, optionally, a
time limit in seconds (the default is 300):

```shell
export FILES_API_KEY=YOUR_API_KEY
dotnet run -- https://MY-SUBDOMAIN.files.com 60
```

It exits with 0 when every step succeeded, 1 when Files.com reported an error, 2 when the URL argument or the
credential variables are missing, and 3 when it was cancelled or ran out of time. A cancelled run can leave its
`files-sdk-example-<time>` folder behind.
