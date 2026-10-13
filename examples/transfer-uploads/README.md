# Files.com .NET Transfer Uploads Example

This console application uploads files with `client.Transfers`, which sends a file in parts, several at a time
when Files.com allows it. Each command is one short file:

- `one` ([OneFile.cs](OneFile.cs)) uploads one file, letting the upload choose how many parts to send at once
  from what it measures, never more than six, and, where the parts go straight to Amazon S3, the parts' sizes. It prints a line of
  progress each time Files.com accepts a part, how the number of parts at once changed, and how parts were sized.
- `many` ([SharedLimit.cs](SharedLimit.cs)) uploads several files at once through one `TransferLimiter`, which
  holds all of them to six part requests and 32 MiB of buffers together, and prints what the limiter is doing.
- `cancel` ([CancelWithProgress.cs](CancelWithProgress.cs)) uploads a file from a stream with progress and stops
  it after a time limit. The stream stays open afterward: uploads from a stream never close it.
- `resume` ([Resume.cs](Resume.cs)) uploads one file with a checkpoint: stop it with Ctrl+C and run the same
  command again, and it sends only the parts Files.com has not accepted. If the file changed since, or Files.com no
  longer knows the upload, it stops with `TransferCheckpointException` and says why; delete the checkpoint to upload
  the file again as a new upload. Keep the checkpoint where only you can change it, such as a directory of yours that
  no one else can write: otherwise it stops before sending anything.
- `folder` ([Folder.cs](Folder.cs)) uploads a whole directory, four files at a time within one `TransferLimiter`, and
  with a checkpoint directory, continues where it stopped when run again, skipping the files already uploaded.
- `stdin` ([UnknownLength.cs](UnknownLength.cs)) uploads standard input, whose length is not known until it ends,
  keeping each part in a temporary file until Files.com accepts it. An optional most bytes it may hold lets parts
  stay smaller; a longer input fails the upload, which is not completed.

## Running

Set `FILES_API_KEY`, then run a command with your site's URL:

```shell
export FILES_API_KEY=YOUR_API_KEY
dotnet run -- https://MY-SUBDOMAIN.files.com one ./report.pdf reports/report.pdf
dotnet run -- https://MY-SUBDOMAIN.files.com many backups ./a.bin ./b.bin ./c.bin
dotnet run -- https://MY-SUBDOMAIN.files.com cancel ./large.iso isos/large.iso 10
tar -cz ./logs | dotnet run -- https://MY-SUBDOMAIN.files.com stdin backups/logs.tar.gz 10737418240
dotnet run -- https://MY-SUBDOMAIN.files.com resume ./large.iso isos/large.iso ./large.iso.checkpoint
dotnet run -- https://MY-SUBDOMAIN.files.com folder ./exports exports ./exports.checkpoints
```

Missing folders on Files.com are created. Ctrl+C cancels any command.

It exits with:

- 0 when every upload was completed;
- 1 when Files.com reported an error, a file could not be read, or the request completing an upload failed in a
  way that leaves unknown whether Files.com completed it (`UploadCompletionUnknownException`; the example says so);
- 2 when arguments or `FILES_API_KEY` are missing;
- 3 when an upload was cancelled or ran out of time.

A stopped upload is not completed, and nothing it sent appears as a file, unless it had already sent its final
request. That is also true after cancelling, including the `cancel` command's time limit: cancelled while the final
request was on its way, the upload may still have been completed, so check the file before uploading it again.
