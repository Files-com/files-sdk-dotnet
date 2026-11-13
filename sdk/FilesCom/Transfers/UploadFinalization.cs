using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Transfers
{
    using FilesCom.Http;
    using FilesCom.Models;
    using FilesCom.Util;

    // The final request of an upload (POST /files/{path} with action end), and the wait while Files.com completes the file
    // in the background. The request asks for that (async_finalize): for a destination such as a remote server mount or a
    // Files.com Agent, Files.com can answer HTTP 202 with {"status": "pending", "ref": the upload's ref}, and optionally a
    // "progress" from 0 to 100, while it goes on completing the file; for any other destination, and from a Files.com that
    // does not know the parameter, the answer is the file, as without it.
    //
    // The first request is sent once, without the client's automatic retries: what its failure means is the caller's to
    // say. A 202 that is not that pending answer leaves the outcome unknown, and nothing is sent again. After a pending
    // answer, the same request (the same path, parameters and options, with the context the upload started with) is sent
    // again after it, and again after each pending answer, waiting one, two, then three seconds (PollInterval), until
    // Files.com answers with the file or an error.
    // Files.com keeps the finalization by the upload's ref and answers every later request for it from there, so it is
    // never carried out twice: these requests are sent again after a failure that may be temporary, with the client's
    // retries. Files.com's error answer is the upload's outcome, thrown as it is. Any other failure once the retries are
    // spent, an answer that is neither the file nor the pending one, and an exception from the pending handler, leave the
    // outcome unknown: UploadCompletionUnknownException, with the failure inside. Cancellation stops only the wait, with an
    // OperationCanceledException that says Files.com may still complete the file.
    internal sealed class UploadFinalization
    {
        // How long the upload waits after its pendingAnswers-th pending answer before asking again: one second after the
        // first, two after the second and three after each later one, counted for each upload's finalization.
        internal static TimeSpan PollInterval(int pendingAnswers) => TimeSpan.FromSeconds(Math.Min(pendingAnswers, 3));

        private static readonly SdkLog log = SdkLog.For(typeof(UploadFinalization));

        private readonly OperationContext context;
        private readonly string path;
        private readonly Dictionary<string, object> parameters;
        private readonly Dictionary<string, object> options;
        private readonly long size;
        private readonly IReadOnlyList<UploadedPart> parts;
        private readonly System.Action<int?> pending;

        // parameters are the final request's, with its ref; size and parts describe the upload for an
        // UploadCompletionUnknownException. pending, if not null, is called with each pending answer's progress.
        internal UploadFinalization(OperationContext context, string path, Dictionary<string, object> parameters, Dictionary<string, object> options, long size, IReadOnlyList<UploadedPart> parts, System.Action<int?> pending)
        {
            this.context = context;
            this.path = path;
            this.parameters = parameters;
            this.options = options ?? new Dictionary<string, object>();
            this.size = size;
            this.parts = parts;
            this.pending = pending;
        }

        // Completes the upload and returns the file. A failure of the first request is thrown as firstFailure makes it, or
        // as it is when firstFailure is null.
        internal async Task<RemoteFile> CompleteAsync(Func<Exception, Exception> firstFailure, CancellationToken cancellationToken)
        {
            // As the generated operation refuses it, before anything is sent.
            if (!(parameters.TryGetValue("ref", out object value) && value is string reference))
            {
                throw new InvalidParameterException("Bad parameter: ref is null, which cannot be sent", "ref");
            }
            parameters["async_finalize"] = true;
            parameters["path"] = path;

            Answer answer;
            try
            {
                answer = await SendAsync(context.WithoutRetries(), cancellationToken);
            }
            catch (Exception e) when (firstFailure != null)
            {
                Exception thrown = firstFailure(e);
                if (thrown == e)
                {
                    throw;
                }
                throw thrown;
            }
            if (answer.File != null)
            {
                return answer.File;
            }

            log.Info("Files.com is completing an upload in the background; waiting for it");
            log.Debug($"Files.com is completing the upload to {path} in the background");
            OperationContext polls = context.ForReplaySafeRequests();
            for (int pendingAnswers = 1; answer.File == null; pendingAnswers++)
            {
                Report(Progress(answer.Pending, reference), reference, cancellationToken);
                try
                {
                    await WaitAsync(PollInterval(pendingAnswers), cancellationToken);
                    answer = await SendAsync(polls, cancellationToken);
                }
                catch (OperationCanceledException e) when (cancellationToken.IsCancellationRequested)
                {
                    throw Stopped(e, cancellationToken);
                }
                // Files.com's answer for the upload: its outcome.
                catch (ApiException e) when (!RetryPolicy.IsRetryable(e.httpStatus))
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw Unknown(reference, e);
                }
            }
            return answer.File;
        }

        // The progress of a pending answer, from 0 to 100, or null when it gives none or something else; an answer that is
        // not the pending completion of this upload leaves its outcome unknown. Nothing of the answer is quoted.
        private int? Progress(string body, string reference)
        {
            string problem;
            try
            {
                using (JsonDocument document = JsonDocument.Parse(body))
                {
                    JsonElement answer = document.RootElement;
                    if (answer.ValueKind != JsonValueKind.Object)
                    {
                        problem = "it is not a JSON object";
                    }
                    else if (!(answer.TryGetProperty("status", out JsonElement status) && status.ValueKind == JsonValueKind.String && status.GetString() == "pending"))
                    {
                        problem = "its status is not pending";
                    }
                    else if (!(answer.TryGetProperty("ref", out JsonElement answered) && answered.ValueKind == JsonValueKind.String && answered.GetString() == reference))
                    {
                        problem = "it does not name the upload's ref";
                    }
                    else
                    {
                        // A JSON integer from 0 to 100; anything else, such as 42.5, "42" or 101, is no progress.
                        return answer.TryGetProperty("progress", out JsonElement progress) && progress.ValueKind == JsonValueKind.Number
                            && progress.TryGetInt32(out int percent) && percent >= 0 && percent <= 100 ? percent : (int?)null;
                    }
                }
            }
            catch (JsonException)
            {
                problem = "it is not JSON";
            }
            throw Unknown(reference, new InvalidResponseException($"Files.com answered the request to complete the upload with HTTP 202, but not as the upload's pending completion: {problem}."));
        }

        // Reports a pending answer. An exception from the handler stops the wait, since the caller no longer waits for the
        // outcome: as cancellation, when the token has been cancelled, and otherwise inside an
        // UploadCompletionUnknownException, since Files.com may still complete the file.
        private void Report(int? progress, string reference, CancellationToken cancellationToken)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug($"Files.com is still completing the upload to {path}: progress {(progress.HasValue ? progress + "%" : "not given")}");
            }
            try
            {
                pending?.Invoke(progress);
            }
            catch (OperationCanceledException e) when (cancellationToken.IsCancellationRequested)
            {
                throw Stopped(e, cancellationToken);
            }
            catch (Exception e)
            {
                throw Unknown(reference, e);
            }
        }

        private UploadCompletionUnknownException Unknown(string reference, Exception failure)
        {
            return UploadCompletionUnknownException.WhileCompleting(path, size, parts, reference, failure);
        }

        private OperationCanceledException Stopped(OperationCanceledException cancellation, CancellationToken cancellationToken)
        {
            return new OperationCanceledException($"The wait for Files.com to complete the upload to {path} was cancelled. Files.com had accepted the request to complete it and may still complete the file: check it before uploading it again.", cancellation, cancellationToken);
        }

        // The wait before asking again. Debug builds only: tests observe it at the timer (RetryPolicy.WaitClock).
        private static Task WaitAsync(TimeSpan delay, CancellationToken cancellationToken)
        {
#if DEBUG && NET8_0_OR_GREATER
            TimeProvider clock = RetryPolicy.WaitClock.Value;
            if (clock != null)
            {
                return Task.Delay(delay, clock, cancellationToken);
            }
#endif
            return Task.Delay(delay, cancellationToken);
        }

        // One request, and its answer: the file, read as the generated operation reads it, or the body of a 202.
        private async Task<Answer> SendAsync(OperationContext requestContext, CancellationToken cancellationToken)
        {
            using (OperationContext.OperationScope operation = requestContext.Begin(cancellationToken))
            using (HttpResponseMessage response = await FilesClient.SendRequest(requestContext, $"/files/{Uri.EscapeDataString(path)}", HttpMethod.Post, parameters, options, operation.Token))
            {
                // SendRequest has already buffered and logged the body, honoring the token.
                string body = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    return new Answer { Pending = body };
                }
                return new Answer { File = JsonUtil.DeserializeModelResponse<RemoteFile>(body, requestContext, options) };
            }
        }

        private sealed class Answer
        {
            internal RemoteFile File;
            internal string Pending;
        }
    }
}