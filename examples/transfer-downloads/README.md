# Files.com .NET Transfer Downloads Example

This console application downloads files with `client.Transfers` and reads them with `client.FileSystem`. Each command
is one short file:

- `one` ([OneFile.cs](OneFile.cs)) downloads one file. When Files.com binds the download to the file's version, it
  is fetched in parts, four at a time, each checked to be exactly the bytes of that version, and the file appears
  under its name only once complete. Give it a checkpoint file, stop it with Ctrl+C, and run the same command again:
  it continues from the parts already written.
- `folder` ([Folder.cs](Folder.cs)) downloads a whole remote folder, four files at a time within one
  `TransferLimiter` of six part requests, and with a checkpoint directory, continues where it stopped when run
  again, skipping the files already downloaded.
- `cat` ([Cat.cs](Cat.cs)) prints a remote text file, read line by line through a stream.

`one` and `folder` replace a local file of the same name by default. Add `--no-replace` last to leave every local file
already there as it is: the command then fails for such a file, and with a checkpoint keeps what it downloaded, so the
same command puts it in place once the name is free.

## Running

Set `FILES_API_KEY`, then run a command with your site's URL:

```shell
export FILES_API_KEY=YOUR_API_KEY
dotnet run -- https://MY-SUBDOMAIN.files.com one isos/large.iso ./large.iso ./large.iso.checkpoint
dotnet run -- https://MY-SUBDOMAIN.files.com folder exports ./exports ./exports.checkpoints
dotnet run -- https://MY-SUBDOMAIN.files.com cat reports/summary.txt
```

Ctrl+C cancels any command.

It exits with:

- 0 when every download was completed;
- 1 when Files.com reported an error, a file changed while it was downloaded (`DownloadSourceChangedException`), a file
  could not be put in place (`DownloadPublicationException`), or a local file or folder could not be written;
- 2 when arguments or `FILES_API_KEY` are missing;
- 3 when a download was cancelled.

A stopped download never leaves a partial file under the file's name: what it wrote stays in a staging folder beside
it, which the same command with the same checkpoint continues, and which a completed download removes.
