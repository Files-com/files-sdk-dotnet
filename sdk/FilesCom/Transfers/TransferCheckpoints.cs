using FilesCom.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FilesCom.Transfers
{
    // A transfer's checkpoint file: JSON naming the transfer it belongs to (its scope) and what it has done, so running
    // the same transfer again continues it. It holds no credentials and no signed URL: only the site's origin, the
    // workspace, the paths, and the transfer's own state, such as the entity tag of a download's version and a digest of
    // the storage resource it came from, which grant nothing by themselves. Writing replaces the file whole, so a crash leaves the previous checkpoint or the new one, never a mixture;
    // it is written only with state the transfer has made durable. It is read, written and removed only where no other
    // user can change it (CheckpointStorage), and refused before any of it is read otherwise
    // (TransferCheckpointProblem.Untrusted). A file transfer reads its checkpoint before it sends anything; a directory
    // download lists the remote folder first, and neither undoes the directories it was asked to create.
    internal static class CheckpointFile
    {
        internal const string Format = "files-sdk-dotnet-transfer-checkpoint";

        private static readonly JsonSerializerOptions Json = new JsonSerializerOptions { WriteIndented = true };

        // The checkpoint at path, or null when there is none. One that cannot be read, is kept where another user could
        // have changed it, is not a checkpoint of this version, or is of another kind fails as TransferCheckpointException.
        internal static T Read<T>(string path, string kind) where T : Checkpoint
        {
            byte[] bytes;
            try
            {
                using (StateDirectory directory = CheckpointStorage.Open(DirectoryOf(path)))
                {
                    bytes = directory.Read(NameOf(path));
                }
                if (bytes == null)
                {
                    return null;
                }
            }
            catch (UntrustedStateException e)
            {
                throw new TransferCheckpointException(path, TransferCheckpointProblem.Untrusted, e.Whole ? e.Reason : $"{e.Reason}, so another user could have changed it", e);
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                throw new TransferCheckpointException(path, TransferCheckpointProblem.Unreadable, "it cannot be read", e);
            }
            T checkpoint;
            try
            {
                checkpoint = JsonSerializer.Deserialize<T>(bytes, Json);
            }
            catch (JsonException e)
            {
                throw new TransferCheckpointException(path, TransferCheckpointProblem.Unreadable, "it is not a checkpoint this SDK wrote", e);
            }
            if (checkpoint == null || checkpoint.Format != Format || checkpoint.Kind != kind)
            {
                bool ours = checkpoint?.Format == Format;
                throw new TransferCheckpointException(path, ours ? TransferCheckpointProblem.OtherTransfer : TransferCheckpointProblem.Unreadable, ours ? $"it records a transfer of another kind, {checkpoint.Kind}" : "it is not a checkpoint this SDK wrote");
            }
            if (!checkpoint.Reads(checkpoint.Version))
            {
                throw new TransferCheckpointException(path, TransferCheckpointProblem.Unreadable, $"it was written by another version of this SDK, in format {checkpoint.Version}");
            }
            return checkpoint;
        }

        // Replaces the checkpoint at path with checkpoint, durably: a new file beside it is written and flushed to
        // storage, renamed over it, and the directory is flushed, so the new name reaches storage before this returns
        // (Durable says where the platform allows each step). A process that ends at any moment leaves the previous
        // checkpoint or the new one; so does a system crash or power loss, where storage keeps what it acknowledged.
        // Where it is checked (CheckpointStorage), the new file is this user's alone before anything is written to it,
        // and a directory another user can now change is refused, as an IOException: nothing is written.
        //
        // A checkpoint that returned from here is one a transfer may rely on, such as the record that the final request
        // of an upload is about to be sent. So where the directory's flush fails, or the file system reports it cannot
        // flush a directory at all, this throws, saying that the new checkpoint is in place but may not survive a system
        // crash: nothing that depends on it happens. On a file system that cannot flush directories every checkpoint
        // write fails that way, the first before a transfer has sent anything. Windows has no such flush here, and the
        // rename is left to the system (Durable.DirectoryFlush.NotAttempted).
        internal static void Write(string path, Checkpoint checkpoint)
        {
            checkpoint.Format = Format;
            checkpoint.Version = checkpoint.WritesVersion;
            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(checkpoint, checkpoint.GetType(), Json);
            string full = Path.GetFullPath(path);
            StateDirectory directory = null;
            try
            {
                directory = CheckpointStorage.Open(DirectoryOf(full));
                directory.Replace(NameOf(full), bytes);
            }
            catch (UntrustedStateException e)
            {
                directory?.Dispose();
                throw new IOException(e.Whole ? $"The checkpoint {full} was not written: {e.Reason}." : $"The checkpoint {full} was not written: {e.Reason}, so another user could change it.", e);
            }
            catch
            {
                directory?.Dispose();
                throw;
            }
            using (directory)
            {
                Durable.DirectoryFlush flushed;
                try
                {
                    flushed = directory.Flush();
                }
                catch (IOException e)
                {
                    throw new IOException($"The checkpoint {full} was replaced, but may not survive a system crash: {e.Message}", e);
                }
                if (flushed == Durable.DirectoryFlush.Unsupported)
                {
                    throw new IOException($"The checkpoint {full} was replaced, but the file system it is on cannot flush a directory to storage, so it may not survive a system crash. Keep checkpoints on a file system that can, such as a local disk.");
                }
            }
        }

        // Removes the checkpoint at path, where it can be (CheckpointStorage). A checkpoint left behind is refused or
        // reused by the transfer it records; nothing depends on its removal.
        internal static void Delete(string path)
        {
            try
            {
                using (StateDirectory directory = CheckpointStorage.Open(DirectoryOf(path)))
                {
                    directory.Delete(NameOf(path));
                }
            }
            catch (Exception)
            {
                // Left behind, as above.
            }
        }

        private static string DirectoryOf(string path)
        {
            return Path.GetDirectoryName(Path.GetFullPath(path));
        }

        // The checkpoint's name in its directory: a path that names a directory, not a file, is refused. The options'
        // snapshots refuse such a path before anything is read; this refuses one that names no file only once resolved.
        private static string NameOf(string path)
        {
            string name = Path.GetFileName(Path.GetFullPath(path));
            if (string.IsNullOrEmpty(name))
            {
                throw InvalidParameterException.For(nameof(UploadOptions.CheckpointPath), $"The checkpoint path {path} names no file");
            }
            return name;
        }

        // The site's origin and the workspace a transfer runs in: a checkpoint is used only by a transfer in both.
        internal static string SiteOf(OperationContext context)
        {
            return Origins.Of(new Uri(context.BaseUrl));
        }

        internal static string WorkspaceOf(OperationContext context, Dictionary<string, object> requestOptions)
        {
            object workspace = null;
            if (requestOptions != null && requestOptions.TryGetValue("workspace_id", out workspace))
            {
                return workspace?.ToString();
            }
            return string.IsNullOrWhiteSpace(context.WorkspaceId) ? null : context.WorkspaceId;
        }
    }

    // What every checkpoint records: its format, and the transfer it belongs to. A checkpoint is written in the newest
    // version of its kind's format, and read in the versions this SDK can continue. A kind's version changes only when
    // an SDK that reads the old one would misread the new: older SDKs then refuse it (TransferCheckpointProblem.
    // Unreadable) and leave it as it is.
    internal abstract class Checkpoint
    {
        // The format version this SDK writes for this checkpoint's kind: 1 unless a kind says otherwise.
        internal virtual int WritesVersion => 1;

        // Whether this SDK can continue a checkpoint of this kind written in version.
        internal virtual bool Reads(int version) => version == 1;

        [JsonPropertyName("format")]
        public string Format { get; set; }

        [JsonPropertyName("version")]
        public int Version { get; set; }

        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("site")]
        public string Site { get; set; }

        [JsonPropertyName("workspace")]
        public string Workspace { get; set; }

        [JsonPropertyName("remote_path")]
        public string RemotePath { get; set; }

        [JsonPropertyName("local_path")]
        public string LocalPath { get; set; }

        // Why the checkpoint does not belong to the transfer described, or null when it does.
        internal string ScopeMismatch(string site, string workspace, string remotePath, string localPath)
        {
            if (Site != site)
            {
                return $"it records a transfer with {Site}, not {site}";
            }
            if (Workspace != workspace)
            {
                return Workspace == null ? $"it records a transfer outside workspace {workspace}" : workspace == null ? $"it records a transfer in workspace {Workspace}" : $"it records a transfer in workspace {Workspace}, not {workspace}";
            }
            if (RemotePath != remotePath)
            {
                return $"it records a transfer of {RemotePath}, not {remotePath}";
            }
            if (LocalPath != localPath)
            {
                return $"it records a transfer with {LocalPath}, not {localPath}";
            }
            return null;
        }
    }

    // A download's checkpoint: the version it is bound to (DownloadVersion: its storage's strong entity tag, the file's
    // size, and a digest of the storage resource the tag came from), where its stage is, and the byte ranges written to
    // the stage and flushed before the checkpoint was.
    //
    // Version 2 adds its phase and the stage's identity. A partial download records the ranges written, as version 1
    // did; a complete one records, before its file is put in place, that the stage holds all of it, flushed, and which
    // file on storage the stage is (device and inode, or volume and file index), so a later run can tell the stage still
    // waiting, the same file already put in place under the destination's name, and neither. A complete download not
    // bound to a version records no entity tag: it cannot be confirmed later, so its stage is never continued, but its
    // file, once placed, is still recognized. Version 3 binds a download to its storage's entity tag and resource (etag,
    // source) instead of the download_identity token of versions 1 and 2, which earlier 2.0 previews wrote: a partial
    // download those record cannot be confirmed, so it is not continued (Problem), and a complete one is read as not
    // bound. SDKs that read only versions 1 and 2 refuse version 3, which they would otherwise misread.
    //
    // Version 4 is version 3 with the SHA-256 of each part written (extents), recorded only where the stage's identity,
    // or its staging directory's, does not identify it across runs (FileIdentity.Strong), such as on a Linux file system
    // that records no birth time. A later run cannot then show the stage to be this download's, so it never writes to it,
    // changes it or removes it; it copies the extents whose bytes still hash as recorded into a new stage of its own
    // (DownloadCoordinator's salvage) and continues there. Its written ranges are exactly the union of its extents. SDKs
    // that read only versions 1 to 3 refuse version 4, so none of them relies on a stage that only digests vouch for.
    internal sealed class DownloadCheckpoint : Checkpoint
    {
        internal const string DownloadKind = "download";

        internal const string PartialPhase = "partial";

        internal const string CompletePhase = "complete";

        internal override int WritesVersion => Extents != null ? 4 : 3;

        internal override bool Reads(int version) => version >= 1 && version <= 4;

        // The SHA-256 of each part written to the stage, by its [start, end) range, in order and not overlapping; null
        // before version 4, and then not written at all, so a version 3 checkpoint is as earlier SDKs wrote it.
        [JsonPropertyName("extents")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<DigestExtent> Extents { get; set; }

        // "partial" or "complete"; null in version 1, which records partial downloads only.
        [JsonPropertyName("phase")]
        public string Phase { get; set; }

        // The stage file's identity on storage, and its staging directory's (FileIdentity), or null in version 1.
        [JsonPropertyName("stage_identity")]
        public string StageIdentity { get; set; }

        [JsonPropertyName("staging_identity")]
        public string StagingIdentity { get; set; }

        internal bool Complete => Phase == CompletePhase;

        // The version's strong entity tag, as storage sent it; null for a download not bound to a version.
        [JsonPropertyName("etag")]
        public string ETag { get; set; }

        // The storage resource the entity tag came from (DownloadVersion.SourceOf).
        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("size")]
        public long Size { get; set; }

        [JsonPropertyName("part_size")]
        public long PartSize { get; set; }

        [JsonPropertyName("stage_directory")]
        public string StageDirectory { get; set; }

        [JsonPropertyName("stage_file")]
        public string StageFile { get; set; }

        // The permissions the file gets when published as a new file: a new file's (0666 less the umask), never more.
        [JsonPropertyName("stage_mode")]
        public int? StageMode { get; set; }

        // Written ranges as [start, end) pairs, in order, not touching.
        [JsonPropertyName("written")]
        public List<long[]> Written { get; set; }

        // The end of the last written range, or 0 when none is written.
        internal long WrittenEnd => Written == null || Written.Count == 0 ? 0 : Written[Written.Count - 1][1];

        // Why the recorded state cannot be used, or null when it can.
        internal string Problem()
        {
            if (Phase != null && Phase != PartialPhase && Phase != CompletePhase)
            {
                return $"it records a download in a phase this SDK does not know, {Phase}";
            }
            if (Complete && ETag == null)
            {
                // Complete, not bound to a version: only its stage's identity matters.
                return Size < 0 || string.IsNullOrEmpty(StageDirectory) || string.IsNullOrEmpty(StageFile) || string.IsNullOrEmpty(StageIdentity) || Source != null
                    ? "it is missing what a download needs to continue"
                    : StageMode.HasValue && (StageMode.Value & ~0x1B6) != 0 ? "it records permissions for the downloaded file beyond those of a new file" : null;
            }
            if (ETag == null)
            {
                return "it records a partial download not bound to an entity tag of its storage, such as one an earlier preview of this SDK bound to a Files.com download identity, which cannot be confirmed";
            }
            if (!DownloadVersion.IsStrongEntityTag(ETag) || string.IsNullOrEmpty(Source) || Size < 1 || PartSize < 1 || string.IsNullOrEmpty(StageDirectory) || string.IsNullOrEmpty(StageFile) || Written == null
                || (Complete && string.IsNullOrEmpty(StageIdentity)))
            {
                return "it is missing what a download needs to continue";
            }
            if (StageMode.HasValue && (StageMode.Value & ~0x1B6) != 0)
            {
                return "it records permissions for the downloaded file beyond those of a new file";
            }
            long end = 0;
            foreach (long[] range in Written)
            {
                if (range == null || range.Length != 2 || range[0] < end || range[1] <= range[0] || range[1] > Size)
                {
                    return "its written ranges are not in order within the file";
                }
                end = range[1];
            }
            if (Complete && (Written.Count != 1 || Written[0][0] != 0 || Written[0][1] != Size))
            {
                return "it records a complete download whose stage does not hold all of it";
            }
            return ExtentsProblem();
        }

        // Why the extents cannot be relied on, or null when they can: a version 4 checkpoint must have them, each a
        // [start, end) range within the file with a SHA-256 as 64 lowercase hexadecimal digits, in order and not
        // overlapping, together exactly its written ranges; an earlier version has none.
        private string ExtentsProblem()
        {
            if (Version < 4)
            {
                return Extents == null ? null : "it records digests its format does not have";
            }
            if (Extents == null)
            {
                return "it records no digests of what it wrote";
            }
            var covered = new IntervalSet();
            long end = 0;
            foreach (DigestExtent extent in Extents)
            {
                if (extent == null || extent.Start < end || extent.End <= extent.Start || extent.End > Size || !DigestExtent.IsSha256(extent.Sha256))
                {
                    return "its digests are not of ranges in order within the file";
                }
                covered.Add(extent.Start, extent.End);
                end = extent.End;
            }
            List<long[]> union = covered.ToList();
            if (union.Count != Written.Count || union.Where((range, i) => range[0] != Written[i][0] || range[1] != Written[i][1]).Any())
            {
                return "its digests do not cover exactly what it records written";
            }
            return null;
        }
    }

    // The SHA-256 of the bytes a download wrote to the [Start, End) range of its stage, in one part (DownloadCheckpoint).
    internal sealed class DigestExtent
    {
        [JsonPropertyName("start")]
        public long Start { get; set; }

        [JsonPropertyName("end")]
        public long End { get; set; }

        [JsonPropertyName("sha256")]
        public string Sha256 { get; set; }

        internal static bool IsSha256(string digest)
        {
            return digest != null && digest.Length == 64 && digest.All(c => (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f'));
        }
    }

    // An upload's checkpoint: the local file it reads, as it was (length and modification time); the upload's
    // reference and the grant Files.com started it with; every part planned so far, as [number, offset, length, last],
    // and the ETag of each part Files.com accepted; and whether the request completing it has been sent, which is
    // recorded before that request is sent, so a run that ends while it is in flight never completes the upload again
    // blindly.
    internal sealed class UploadCheckpoint : Checkpoint
    {
        internal const string UploadKind = "upload";

        [JsonPropertyName("source_length")]
        public long SourceLength { get; set; }

        [JsonPropertyName("source_modified")]
        public long SourceModified { get; set; }

        [JsonPropertyName("ref")]
        public string Ref { get; set; }

        // The part size limits of the grant, as [min_nonfinal, max_part, max_parts, max_file], or null.
        [JsonPropertyName("limits")]
        public long[] Limits { get; set; }

        // The part count Files.com advertised, or null.
        [JsonPropertyName("available_parts")]
        public long? AvailableParts { get; set; }

        // Whether the parts were sent with their offsets, which only earlier versions of this SDK did.
        [JsonPropertyName("part_offsets")]
        public bool PartOffsets { get; set; }

        // For an upload choosing its part sizes, the S3 upload its parts go to (AwsMultipartRoute.Digest); null otherwise,
        // and in checkpoints of earlier versions of this SDK.
        [JsonPropertyName("route")]
        public string Route { get; set; }

        [JsonPropertyName("sizes_chosen")]
        public bool SizesChosen { get; set; }

        [JsonPropertyName("planned")]
        public List<long[]> Planned { get; set; }

        [JsonPropertyName("accepted")]
        public List<AcceptedPart> Accepted { get; set; }

        [JsonPropertyName("final_request_sent")]
        public bool FinalRequestSent { get; set; }

        // Why the recorded state cannot be used, or null when it can.
        internal string Problem()
        {
            if (string.IsNullOrEmpty(Ref) || SourceLength < 0 || Planned == null || Accepted == null || (Limits != null && Limits.Length != 4))
            {
                return "it is missing what an upload needs to continue";
            }
            long offset = 0;
            for (int i = 0; i < Planned.Count; i++)
            {
                long[] part = Planned[i];
                if (part == null || part.Length != 4 || part[0] != i + 1 || part[1] != offset || part[2] < 0 || part[2] > SourceLength - offset
                    || (part[3] != 0 && (part[3] != 1 || i != Planned.Count - 1)))
                {
                    return "its planned parts are not in order within the file";
                }
                offset += part[2];
            }
            var planned = new HashSet<long>(Planned.Select(part => part[0]));
            if (Accepted.Any(part => part == null || !planned.Contains(part.Part)) || Accepted.Select(part => part.Part).Distinct().Count() != Accepted.Count)
            {
                return "it records accepted parts that were not planned";
            }
            return null;
        }

        // For an upload that chose its part sizes, why the parts it records are not all within limits, or null when they
        // are: every part at most the largest, every part but the last at least the smallest, no more than maxParts parts,
        // and room in the parts left for the rest of the file. A recorded part is sent again exactly as it was planned,
        // never resized within the same upload, so a checkpoint with any part outside the limits, such as the one last
        // part an earlier version of this SDK planned for a fixed size no smaller than the file, cannot be continued.
        internal string PartsOutside(PartSizeLimits limits, long maxParts)
        {
            if (Planned.Count > maxParts)
            {
                return $"it records {Planned.Count} parts, more than the {maxParts} the upload may have";
            }
            long end = 0;
            foreach (long[] part in Planned)
            {
                bool last = part[3] != 0;
                if (part[2] > limits.MaxPartBytes)
                {
                    return $"it records part {part[0]} of {part[2]} bytes, more than the {limits.MaxPartBytes} bytes the limits allow a part";
                }
                if (!last && part[2] < limits.MinNonfinalBytes)
                {
                    return $"it records part {part[0]} of {part[2]} bytes, fewer than the {limits.MinNonfinalBytes} bytes the limits allow any part but the last";
                }
                end = part[1] + part[2];
            }
            long partsLeft = maxParts - Planned.Count;
            long room = partsLeft > long.MaxValue / limits.MaxPartBytes ? long.MaxValue : partsLeft * limits.MaxPartBytes;
            if (SourceLength - end > room)
            {
                return $"the {SourceLength - end} bytes after the parts it records do not fit in the {partsLeft} parts left of at most {limits.MaxPartBytes} bytes";
            }
            return null;
        }

        internal List<PartDescriptor> PlannedParts()
        {
            return Planned.Select(part => new PartDescriptor(part[0], part[1], part[2], part[3] != 0)).ToList();
        }
    }

    // Where one file upload keeps its checkpoint, and what an earlier run of it recorded (Saved), checked before
    // anything is sent: a checkpoint of another transfer fails; one whose final request was sent fails as
    // UploadCompletionUnknownException, without sending anything. Whether the file is still the one being sent is for the
    // upload to find (UploadCoordinator): its length and the bytes of its accepted parts, not its modification time.
    internal sealed class UploadCheckpointing
    {
        private static readonly SdkLog log = SdkLog.For(typeof(UploadCheckpointing));

        private readonly string checkpointPath;
        private readonly string site;
        private readonly string workspace;
        private readonly string remotePath;
        private readonly string localPath;
        private readonly long sourceLength;
        private readonly long sourceModified;

        private UploadCheckpointing(string checkpointPath, string site, string workspace, string remotePath, string localPath, long sourceLength, long sourceModified, UploadCheckpoint saved)
        {
            this.checkpointPath = checkpointPath;
            this.site = site;
            this.workspace = workspace;
            this.remotePath = remotePath;
            this.localPath = localPath;
            this.sourceLength = sourceLength;
            this.sourceModified = sourceModified;
            Saved = saved;
        }

        internal UploadCheckpoint Saved { get; }

        internal string CheckpointPath => checkpointPath;

        internal static UploadCheckpointing Open(string checkpointPath, OperationContext context, Dictionary<string, object> requestOptions, string remotePath, string localPath, long sourceLength, DateTime sourceModifiedUtc)
        {
            string site = CheckpointFile.SiteOf(context);
            string workspace = CheckpointFile.WorkspaceOf(context, requestOptions);
            string full = Path.GetFullPath(localPath);
            UploadCheckpoint saved = CheckpointFile.Read<UploadCheckpoint>(checkpointPath, UploadCheckpoint.UploadKind);
            if (saved != null)
            {
                string mismatch = saved.ScopeMismatch(site, workspace, remotePath, full);
                if (mismatch != null)
                {
                    throw new TransferCheckpointException(checkpointPath, TransferCheckpointProblem.OtherTransfer, mismatch);
                }
                string problem = saved.Problem();
                if (problem != null)
                {
                    throw new TransferCheckpointException(checkpointPath, TransferCheckpointProblem.Unreadable, problem);
                }
                if (saved.FinalRequestSent)
                {
                    Dictionary<long, long[]> planned = saved.Planned.ToDictionary(part => part[0]);
                    List<UploadedPart> parts = saved.Accepted.Select(part => new UploadedPart(part.Part, planned[part.Part][1], planned[part.Part][2], part.ETag, 0)).ToList();
                    throw new UploadCompletionUnknownException(remotePath, saved.SourceLength, parts, checkpointPath);
                }
            }
            return new UploadCheckpointing(checkpointPath, site, workspace, remotePath, full, sourceLength, sourceModifiedUtc.Ticks, saved);
        }

        internal void Write(UploadCheckpoint checkpoint)
        {
            checkpoint.Kind = UploadCheckpoint.UploadKind;
            checkpoint.Site = site;
            checkpoint.Workspace = workspace;
            checkpoint.RemotePath = remotePath;
            checkpoint.LocalPath = localPath;
            checkpoint.SourceLength = sourceLength;
            checkpoint.SourceModified = sourceModified;
            CheckpointFile.Write(checkpointPath, checkpoint);
        }

        internal void Delete()
        {
            CheckpointFile.Delete(checkpointPath);
        }
    }

    // A directory transfer's record of the files it completed, each with what identifies its source: for a download, the
    // token of the version written and the local file it wrote, by its identity on storage and its length (version 1:
    // its length and modification time); for an upload, the SHA-256 of every part sent. A later run skips a file only
    // once that source is shown to be unchanged (DirectoryTransfer). The transfer's files in progress have checkpoints of
    // their own beside it. A directory download's record is version 2, which older SDKs refuse; version 1 is still read.
    internal sealed class DirectoryCheckpoint : Checkpoint
    {
        internal const string UploadKind = "upload-directory";

        internal const string DownloadKind = "download-directory";

        internal override int WritesVersion => Kind == DownloadKind ? 2 : 1;

        internal override bool Reads(int version) => version == 1 || (version == 2 && Kind == DownloadKind);

        [JsonPropertyName("completed")]
        public List<CompletedFile> Completed { get; set; }

        internal string Problem()
        {
            return Completed == null || Completed.Any(file => file == null || string.IsNullOrEmpty(file.RelativePath)) ? "it is missing what a directory transfer needs to continue" : null;
        }
    }

    internal sealed class CompletedFile
    {
        [JsonPropertyName("relative_path")]
        public string RelativePath { get; set; }

        [JsonPropertyName("size")]
        public long Size { get; set; }

        // The source's modification time, in ticks, or null when it has none.
        [JsonPropertyName("source_modified")]
        public long? SourceModified { get; set; }

        // For a download, the modification time of the local file it wrote, in ticks.
        [JsonPropertyName("local_modified")]
        public long? LocalModified { get; set; }

        // For a download, the strong entity tag of the version written, as its storage sent it, and the storage
        // resource it came from (DownloadVersion); null when it was not bound to one. A record an earlier 2.0 preview
        // wrote holds a download_identity token (version) instead, which is not read: its files are not shown current.
        [JsonPropertyName("etag")]
        public string ETag { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        // For a download, the identity on storage of the local file it wrote (FileIdentity); null in version 1.
        [JsonPropertyName("local_identity")]
        public string LocalIdentity { get; set; }

        // For an upload, its parts, with the SHA-256 of the bytes sent.
        [JsonPropertyName("parts")]
        public List<CompletedPart> Parts { get; set; }
    }

    internal sealed class CompletedPart
    {
        [JsonPropertyName("offset")]
        public long Offset { get; set; }

        [JsonPropertyName("length")]
        public long Length { get; set; }

        [JsonPropertyName("sha256")]
        public string Sha256 { get; set; }
    }

    internal sealed class AcceptedPart
    {
        // The SHA-256 of the bytes sent, in hexadecimal: a later run continues only while the file still has them.
        [JsonPropertyName("sha256")]
        public string Sha256 { get; set; }

        [JsonPropertyName("part")]
        public long Part { get; set; }

        [JsonPropertyName("etag")]
        public string ETag { get; set; }
    }

    // Byte ranges [start, end), kept merged and in order.
    internal sealed class IntervalSet
    {
        private readonly List<(long Start, long End)> ranges = new List<(long Start, long End)>();

        internal IntervalSet()
        {
        }

        internal IntervalSet(IEnumerable<long[]> ranges)
        {
            foreach (long[] range in ranges)
            {
                Add(range[0], range[1]);
            }
        }

        internal long Total
        {
            get
            {
                lock (ranges)
                {
                    return ranges.Sum(range => range.End - range.Start);
                }
            }
        }

        internal void Add(long start, long end)
        {
            if (end <= start)
            {
                return;
            }
            lock (ranges)
            {
                int i = 0;
                while (i < ranges.Count && ranges[i].End < start)
                {
                    i++;
                }
                while (i < ranges.Count && ranges[i].Start <= end)
                {
                    start = Math.Min(start, ranges[i].Start);
                    end = Math.Max(end, ranges[i].End);
                    ranges.RemoveAt(i);
                }
                ranges.Insert(i, (start, end));
            }
        }

        internal bool Covers(long start, long end)
        {
            lock (ranges)
            {
                return ranges.Any(range => range.Start <= start && range.End >= end);
            }
        }

        // The end of the interval that holds position, or position itself when none does.
        internal long CoveredEnd(long position)
        {
            lock (ranges)
            {
                foreach ((long Start, long End) range in ranges)
                {
                    if (range.Start <= position && position < range.End)
                    {
                        return range.End;
                    }
                }
                return position;
            }
        }

        internal List<long[]> ToList()
        {
            lock (ranges)
            {
                return ranges.Select(range => new[] { range.Start, range.End }).ToList();
            }
        }
    }
}