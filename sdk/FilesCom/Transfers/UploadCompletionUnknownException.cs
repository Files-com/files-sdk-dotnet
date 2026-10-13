using System;
using System.Collections.Generic;

namespace FilesCom.Transfers
{
    /// <summary>
    /// Every part of an upload was accepted, but the request to complete it failed in a way that does not show
    /// whether Files.com completed the file: the connection failed, its answer could not be read, or it was an error,
    /// which Files.com can answer after completing the file in storage, whatever its status or type.
    /// </summary>
    /// <remarks>
    /// The SDK never retries the final request (a redirect Files.com answers it with is followed, within the same
    /// attempt, as for any request). Repeating it after a lost answer could not tell a completed upload
    /// from a missing one: Files.com forgets an upload once it completes it. Check the file at <see cref="Path"/>,
    /// such as its size and modification time, before uploading it again. <see cref="Exception.InnerException"/>
    /// describes the failure.
    /// </remarks>
    public sealed class UploadCompletionUnknownException : SdkException
    {
        internal UploadCompletionUnknownException(string path, long size, IReadOnlyList<UploadedPart> parts, Exception failure)
            : base($"The request to complete the upload to {path} failed ({failure.GetType().Name}), possibly after Files.com received it, so it is not known whether the file was completed. Check it before uploading it again.", failure)
        {
            Path = path;
            Size = size;
            Parts = parts;
        }

        // An upload whose checkpoint records that its final request was sent, by a run that ended before its answer.
        internal UploadCompletionUnknownException(string path, long size, IReadOnlyList<UploadedPart> parts, string checkpointPath)
            : base($"The checkpoint {checkpointPath} records that the request to complete the upload to {path} was sent, so it is not known whether the file was completed, and the request was not sent again. Check the file, then delete the checkpoint to upload it again.")
        {
            Path = path;
            Size = size;
            Parts = parts;
        }

        /// <summary>The remote path of the upload.</summary>
        public string Path { get; }

        /// <summary>The number of bytes uploaded.</summary>
        public long Size { get; }

        /// <summary>The parts Files.com accepted, by part number.</summary>
        public IReadOnlyList<UploadedPart> Parts { get; }
    }
}