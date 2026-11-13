using System;
using System.Collections.Generic;

namespace FilesCom.Transfers
{
    /// <summary>
    /// Every part of an upload was accepted, but the request to complete it failed in a way that does not show
    /// whether Files.com completed the file: the connection failed, its answer could not be read, or it was an error,
    /// which Files.com can answer after completing the file in storage, whatever its status or type. Or Files.com
    /// accepted the request and was still completing the file in the background when the wait for it failed.
    /// </summary>
    /// <remarks>
    /// The SDK never sends the final request again after a failure that leaves it unknown whether Files.com received it
    /// (a redirect Files.com answers it with is followed, within the same attempt, as for any request). Repeating it
    /// after a lost answer could not tell a completed upload from a missing one: Files.com forgets an upload once it
    /// completes it. Only once Files.com has answered that it is completing the upload in the background does the SDK
    /// send the request again, to ask for the outcome; when that wait fails, Files.com may still complete the file.
    /// Check the file at <see cref="Path"/>, such as its size and modification time, before uploading it again.
    /// <see cref="Exception.InnerException"/> describes the failure.
    /// </remarks>
    public sealed class UploadCompletionUnknownException : SdkException
    {
        internal UploadCompletionUnknownException(string path, long size, IReadOnlyList<UploadedPart> parts, string reference, Exception failure)
            : this($"The request to complete the upload to {path} failed ({failure.GetType().Name}), possibly after Files.com received it, so it is not known whether the file was completed. Check it before uploading it again.", path, size, parts, reference, failure)
        {
        }

        // An upload whose checkpoint records that its final request was sent, by a run that ended before its answer.
        internal UploadCompletionUnknownException(string path, long size, IReadOnlyList<UploadedPart> parts, string reference, string checkpointPath)
            : this($"The checkpoint {checkpointPath} records that the request to complete the upload to {path} was sent, so it is not known whether the file was completed, and the request was not sent again. Check the file, then delete the checkpoint to upload it again.", path, size, parts, reference, null)
        {
        }

        private UploadCompletionUnknownException(string message, string path, long size, IReadOnlyList<UploadedPart> parts, string reference, Exception failure)
            : base(message, failure)
        {
            Path = path;
            Size = size;
            Parts = parts;
            Ref = reference;
        }

        // An upload Files.com answered it is completing in the background, whose wait for the outcome then failed.
        internal static UploadCompletionUnknownException WhileCompleting(string path, long size, IReadOnlyList<UploadedPart> parts, string reference, Exception failure)
        {
            return new UploadCompletionUnknownException($"Files.com accepted the request to complete the upload to {path}, but waiting for it to complete the file failed ({failure.GetType().Name}), so it is not known whether the file was completed: Files.com may still complete it. Check it before uploading it again.", path, size, parts, reference, failure);
        }

        /// <summary>The remote path of the upload.</summary>
        public string Path { get; }

        /// <summary>The number of bytes uploaded.</summary>
        public long Size { get; }

        /// <summary>The parts Files.com accepted, by part number.</summary>
        public IReadOnlyList<UploadedPart> Parts { get; }

        /// <summary>The upload's reference, which Files.com knows the upload by.</summary>
        public string Ref { get; }
    }
}