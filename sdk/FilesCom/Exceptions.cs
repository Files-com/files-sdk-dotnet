using FilesCom.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace FilesCom
{
    /// <summary>
    /// The base of the exceptions the SDK throws: for incorrect use of the SDK, for a request that got no reliable
    /// response, and for an error response from Files.com.
    /// </summary>
    /// <remarks>
    /// Cancellation is thrown as <see cref="OperationCanceledException"/>, and a failure of a stream you give the SDK
    /// is thrown as that stream threw it, not as an SdkException.
    /// </remarks>
    public class SdkException : Exception
    {
        /// <summary>
        /// The message the exception was created with, which <see cref="ToString"/> returns. It is the same text as
        /// <see cref="Exception.Message"/>, except that it is null when the exception was created without one, where
        /// Message has .NET's default text instead. Setting it changes what ToString returns, not Message.
        /// </summary>
        public string message;

        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="message">What went wrong, or null.</param>
        public SdkException(string message = null)
            : base(message)
        {
            this.message = message;
        }

        /// <summary>
        /// Creates the exception for a failure that another exception caused.
        /// </summary>
        /// <param name="message">What went wrong, or null.</param>
        /// <param name="innerException">The exception that caused it.</param>
        protected SdkException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.message = message;
        }

        /// <summary>
        /// Whether a request that can change something, such as creating a user, failed in a way that leaves open
        /// whether it reached Files.com and took effect, so that sending it again could do it twice: after it may have
        /// been sent, its answer was lost or timed out, or it was an error response, readable or not, from Files.com or
        /// from a proxy in between, whatever its status, type or body, since Files.com answers the same errors, even its
        /// rate limit, after an operation has begun as well as before. Such a request is not sent again automatically:
        /// check whether it took effect before sending it again.
        /// </summary>
        /// <remarks>
        /// It is set only for those failures, so false alone does not show that sending the request again is safe, or
        /// that nothing changed. It is false for a request that failed before any of it was sent, or that Files.com
        /// answered, in a way the client trusts, is for a site served from another region, which it answers before acting
        /// on a request; for a request that only asks for something, such as a GET, or that Files.com can receive twice
        /// without harm; and for every other failure, including a successful answer the SDK could not use, after which
        /// the change did take effect (see <see cref="InvalidResponseException"/>).
        /// </remarks>
        public bool RequestMayHaveBeenProcessed { get; internal set; }

        /// <summary>
        /// Returns <see cref="message"/> alone, which is null when the exception was created without a message. Unlike
        /// <see cref="Exception.ToString"/>, it gives neither the exception's class nor its stack trace or inner
        /// exception.
        /// </summary>
        public override string ToString()
        {
            return this.message;
        }
    }

    /// <summary>
    /// No reliable response came from Files.com: the request could not be sent or completed, or what came back
    /// could not be read (<see cref="InvalidResponseException"/>).
    /// </summary>
    /// <remarks>
    /// When <see cref="SdkException.RequestMayHaveBeenProcessed"/> is true, the request may have reached Files.com and
    /// taken effect, though no usable answer came back: a request that changes something, such as creating a user, is
    /// not sent again automatically then, since doing so could do it twice. Check whether it took effect before
    /// sending it again.
    /// </remarks>
    public class ApiConnectionException : SdkException
    {
        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="message">What went wrong, or null.</param>
        public ApiConnectionException(string message = null)
            : base(message)
        { }

        /// <summary>
        /// Creates the exception for a request that may or may not have taken effect.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="requestMayHaveBeenProcessed">Whether the request may have reached Files.com and taken effect.</param>
        public ApiConnectionException(string message, bool requestMayHaveBeenProcessed)
            : base(message)
        {
            RequestMayHaveBeenProcessed = requestMayHaveBeenProcessed;
        }
    }

    /// <summary>
    /// A request stalled: it could not connect within <see cref="FilesConfiguration.ConnectTimeout"/>, waited longer
    /// than <see cref="FilesConfiguration.ReadTimeout"/> for its response or for more of it, or, for an upload part,
    /// could send nothing for <see cref="FilesConfiguration.WriteTimeout"/>. Its message says which.
    /// </summary>
    /// <remarks>
    /// A request that times out is retried like one whose connection failed, up to the client's
    /// <see cref="FilesConfiguration.MaxNetworkRetries"/>; this is thrown when none is left. Cancellation through a
    /// token is an <see cref="OperationCanceledException"/> instead.
    /// </remarks>
    public class NetworkTimeoutException : ApiConnectionException
    {
        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="message">Which wait timed out, or null.</param>
        public NetworkTimeoutException(string message = null)
            : base(message)
        { }

        /// <summary>
        /// Creates the exception for a request that timed out and may or may not have taken effect.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="requestMayHaveBeenProcessed">Whether the request may have reached Files.com and taken effect.</param>
        public NetworkTimeoutException(string message, bool requestMayHaveBeenProcessed)
            : base(message, requestMayHaveBeenProcessed)
        { }

        // Whether the timeout was the handler's ConnectTimeout: the connection was never made, so nothing was sent.
        internal bool BeforeSending { get; set; }
    }

    /// <summary>
    /// The SDK has no credentials for a request that needs them. It is thrown before the request is sent.
    /// </summary>
    public class AuthenticationException : SdkException
    {
        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="message">Which credentials are missing, or null.</param>
        public AuthenticationException(string message = null)
            : base(message)
        { }
    }

    /// <summary>
    /// A parameter, option or setting is not valid: a value of the wrong type, out of range, or contradicting another.
    /// It is thrown before the request is sent, and before a transfer reads or writes any file or stream.
    /// </summary>
    public class InvalidParameterException : SdkException
    {
        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="message">Which value is not valid, and why, or null.</param>
        public InvalidParameterException(string message = null)
            : base(message)
        { }

        /// <summary>
        /// Creates the exception for the value of one named parameter, option or setting.
        /// </summary>
        /// <param name="message">Which value is not valid, and why.</param>
        /// <param name="paramName">The name of the parameter, option or setting (<see cref="ParamName"/>).</param>
        public InvalidParameterException(string message, string paramName)
            : base(message)
        {
            ParamName = paramName;
        }

        /// <summary>
        /// The name of the parameter, option or setting that is not valid: a method's parameter name, such as
        /// localPath; an options property, such as PartSize; or an API parameter, such as user_id. Null when the
        /// exception names none.
        /// </summary>
        public string ParamName { get; }

        // For an argument, option or setting the SDK cannot use, named and with the reason.
        internal static InvalidParameterException For(string paramName, string reason)
        {
            return new InvalidParameterException($"Bad parameter: {paramName}: {reason}", paramName);
        }

        // For a value outside the range it may take, which the message also gives.
        internal static InvalidParameterException OutOfRange(string paramName, object value, string reason)
        {
            return new InvalidParameterException($"Bad parameter: {paramName} is {value}: {reason}", paramName);
        }
    }

    /// <summary>
    /// A required parameter is missing or null, or an object lacks the ID or path its operation needs. It is
    /// thrown before the request is sent, and before a transfer reads or writes any file or stream.
    /// </summary>
    public class MissingParameterException : SdkException
    {
        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="message">Which parameter is missing, or null.</param>
        public MissingParameterException(string message = null)
            : base(message)
        { }

        /// <summary>
        /// Creates the exception for one named parameter.
        /// </summary>
        /// <param name="message">Which parameter is missing.</param>
        /// <param name="paramName">The name of the parameter (<see cref="ParamName"/>).</param>
        public MissingParameterException(string message, string paramName)
            : base(message)
        {
            ParamName = paramName;
        }

        /// <summary>
        /// The name of the missing parameter: a method's parameter name, such as localPath, or an API parameter, such
        /// as user_id. Null when the exception names none.
        /// </summary>
        public string ParamName { get; }

        // For a required argument that is null.
        internal static MissingParameterException For(string paramName)
        {
            return new MissingParameterException($"Parameter missing: {paramName}", paramName);
        }
    }

    /// <summary>
    /// An SDK method was used where it is not supported, such as saving an object whose model Files.com cannot
    /// update. It is thrown before any request is sent.
    /// </summary>
    public class NotImplementedException : SdkException
    {
        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="message">What is not supported, or null.</param>
        public NotImplementedException(string message = null)
            : base(message)
        { }
    }

    /// <summary>
    /// What Files.com returned could not be read or used: an error response that is not a JSON error object, a
    /// successful response that is not the data the operation returns, or upload or list data that is inconsistent,
    /// found before the SDK sends a request that depends on it.
    /// </summary>
    /// <remarks>
    /// Neither its message nor an inner exception includes any part of the response, which can hold secrets such as a
    /// signed URL. The reason the response could not be read is logged at the DEBUG level.
    /// <para>
    /// An <see cref="httpStatus"/> of 0 means no error response is attached, which does not tell by itself whether a
    /// request took effect. An operation such as CreateAsync throws it after a successful response it could not use:
    /// the change took effect, although <see cref="SdkException.RequestMayHaveBeenProcessed"/> is false. A transfer
    /// also throws it when it refuses inconsistent upload data, before sending the request that would use it.
    /// </para>
    /// </remarks>
    public class InvalidResponseException : ApiConnectionException
    {
        /// <summary>
        /// Creates the exception, with no error response attached.
        /// </summary>
        /// <param name="message">What could not be read or used, or null.</param>
        public InvalidResponseException(string message = null)
            : base(message)
        { }

        /// <summary>
        /// Creates the exception for an error response that could not be read.
        /// </summary>
        /// <param name="message">What could not be read.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidResponseException(string message, int httpStatus, HttpHeaders httpHeaders)
            : base(message)
        {
            this.httpStatus = httpStatus;
            this.httpHeaders = httpHeaders;
        }

        // Named as ApiException's members are, so that both kinds of failed response read the same way.

        /// <summary>
        /// The HTTP status of the error response, or 0 when no error response is attached.
        /// </summary>
        public int httpStatus { get; }

        /// <summary>
        /// The headers of the error response, or null when no error response is attached.
        /// </summary>
        public HttpHeaders httpHeaders { get; }

        // For a transfer's storage request: whether storage answered that the request's signed URL has expired
        // (SignedUrl.RefusedAsExpired), which a new URL for the same part or range can mend.
        internal bool StorageSaysExpired { get; set; }

        // For a transfer's storage request: whether storage answered, with its whole error, that it stopped waiting for
        // the request (RetryPolicy.StorageTimedOut), which sending the request again may mend.
        internal bool StorageSaysTimedOut { get; set; }
    }

    /// <summary>
    /// An error response from Files.com. For a JSON error object, its class comes from the response's error type,
    /// never from its HTTP status: a type the SDK knows has its own class, such as <see cref="NotFoundException"/>,
    /// and any other type is an ApiException with the response's error, status and headers. Files.com's former
    /// plain-text answer that a site is served from another region, an HTTP 403 whose body starts with
    /// <c>You have connected to a URL</c> and that has an <c>X-Files-Host</c> header, is a
    /// <see cref="LockoutRegionMismatchException"/> with no <see cref="responseError"/>, whose message does not quote
    /// the body.
    /// </summary>
    /// <remarks>
    /// A JSON error's message is its error text, with the user information, query and fragment of any URL in it
    /// removed, since a signed URL keeps its credentials there. <see cref="error"/> and <see cref="responseError"/>
    /// keep the response as it came. An error does not show by itself that a request which changes something was not
    /// carried out: <see cref="SdkException.RequestMayHaveBeenProcessed"/> is true when it may have been.
    /// </remarks>
    public class ApiException : SdkException
    {
        /// <summary>
        /// The HTTP status of the error response itself, which can differ from the status its body states
        /// (<see cref="httpCode"/>). It is 0 when the exception was created without one.
        /// </summary>
        public int httpStatus;
        /// <summary>
        /// The error body this exception was given, or null when it was given none. Properties of this exception such
        /// as <see cref="error"/> and <see cref="type"/> return its fields, read from this field each time.
        /// </summary>
        public ResponseError responseError;
        /// <summary>
        /// The headers this exception was given, or null when it was given none. For an error response the SDK reads,
        /// they are the response's headers, not its content's headers such as Content-Type.
        /// </summary>
        public HttpHeaders httpHeaders;

        /// <summary>
        /// Creates the exception for an error response.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ApiException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message)
        {
            this.httpStatus = httpStatus;
            this.responseError = responseError;
            this.httpHeaders = httpHeaders;
        }

        /// <summary>
        /// The body's <c>detail</c> field (<see cref="ResponseError.detail"/>), or null when there is no
        /// <see cref="responseError"/> or the body does not give it.
        /// </summary>
        public string detail => this.responseError?.detail;
        /// <summary>
        /// The body's error text (<see cref="ResponseError.error"/>), as it came, or null when there is no
        /// <see cref="responseError"/> or the body does not give it.
        /// </summary>
        public string error => this.responseError?.error;
        /// <summary>
        /// The strings of the body's <c>errors</c> field (<see cref="ResponseError.errors"/>), or null when there is no
        /// <see cref="responseError"/> or the body does not give it.
        /// </summary>
        public string[] errors => this.responseError?.errors;
        /// <summary>
        /// The HTTP status the body states (<see cref="ResponseError.httpCode"/>), or 0 when there is no
        /// <see cref="responseError"/> or the body does not give it. The response's own status is
        /// <see cref="httpStatus"/>.
        /// </summary>
        public int httpCode => this.responseError?.httpCode ?? 0;
        /// <summary>
        /// The body's <c>instance</c> field (<see cref="ResponseError.instance"/>), or null when there is no
        /// <see cref="responseError"/> or the body does not give it.
        /// </summary>
        public string instance => this.responseError?.instance;
        /// <summary>
        /// Error texts by the name of the attribute each is about (<see cref="ResponseError.modelErrors"/>), or null
        /// when there is no <see cref="responseError"/> or the body does not give them.
        /// </summary>
        public Dictionary<string, string[]> modelErrors => this.responseError?.modelErrors;
        /// <summary>
        /// The title the API gives the error type (<see cref="ResponseError.title"/>), or null when there is no
        /// <see cref="responseError"/> or the body does not give it.
        /// </summary>
        public string title => this.responseError?.title;
        /// <summary>
        /// The error type (<see cref="ResponseError.type"/>), or null when there is no <see cref="responseError"/> or
        /// the body does not give it.
        /// </summary>
        public string type => this.responseError?.type;
        /// <summary>
        /// The body's <c>data</c> field (<see cref="ResponseError.data"/>), as it came, or null when there is no
        /// <see cref="responseError"/> or the body does not give it.
        /// </summary>
        public Dictionary<string, object> data => this.responseError?.data;
    }

    /// <summary>
    /// An error response from Files.com whose error type is in the <c>bad-request</c> group (Bad Request, HTTP 422): the base of the exceptions for the group's types, and the exception for a type in it that has no class of its own.
    /// </summary>
    public partial class BadRequestException : ApiException
    {
        /// <summary>
        /// Creates the exception for an error response of a type in this group.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public BadRequestException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/agent-upgrade-required</c> (Agent Upgrade Required, HTTP 422).
    /// </summary>
    public partial class AgentUpgradeRequiredException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public AgentUpgradeRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/attachment-too-large</c> (Attachment Too Large, HTTP 422).
    /// </summary>
    public partial class AttachmentTooLargeException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public AttachmentTooLargeException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/cannot-download-directory</c> (Cannot Download Directory, HTTP 422).
    /// </summary>
    public partial class CannotDownloadDirectoryException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public CannotDownloadDirectoryException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/cant-move-with-multiple-locations</c> (Cant Move With Multiple Locations, HTTP 422).
    /// </summary>
    public partial class CantMoveWithMultipleLocationsException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public CantMoveWithMultipleLocationsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/datetime-parse</c> (Datetime Parse, HTTP 422).
    /// </summary>
    public partial class DatetimeParseException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public DatetimeParseException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/destination-same</c> (Destination Same, HTTP 422).
    /// </summary>
    public partial class DestinationSameException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public DestinationSameException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/destination-site-mismatch</c> (Destination Site Mismatch, HTTP 422).
    /// </summary>
    public partial class DestinationSiteMismatchException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public DestinationSiteMismatchException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/does-not-support-sorting</c> (Does Not Support Sorting, HTTP 422).
    /// </summary>
    public partial class DoesNotSupportSortingException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public DoesNotSupportSortingException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/folder-must-not-be-a-file</c> (Folder Must Not Be A File, HTTP 422).
    /// </summary>
    public partial class FolderMustNotBeAFileException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public FolderMustNotBeAFileException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/folders-not-allowed</c> (Folders Not Allowed, HTTP 422).
    /// </summary>
    public partial class FoldersNotAllowedException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public FoldersNotAllowedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/internal-general-error</c> (Internal General Error, HTTP 422).
    /// </summary>
    public partial class InternalGeneralErrorException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InternalGeneralErrorException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/invalid-body</c> (Invalid Body, HTTP 422).
    /// </summary>
    public partial class InvalidBodyException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidBodyException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/invalid-cursor</c> (Invalid Cursor, HTTP 422).
    /// </summary>
    public partial class InvalidCursorException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidCursorException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/invalid-cursor-type-for-sort</c> (Invalid Cursor Type For Sort, HTTP 422).
    /// </summary>
    public partial class InvalidCursorTypeForSortException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidCursorTypeForSortException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/invalid-etags</c> (Invalid Etags, HTTP 422).
    /// </summary>
    public partial class InvalidEtagsException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidEtagsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/invalid-filter-alias-combination</c> (Invalid Filter Alias Combination, HTTP 422).
    /// </summary>
    public partial class InvalidFilterAliasCombinationException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidFilterAliasCombinationException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/invalid-filter-field</c> (Invalid Filter Field, HTTP 422).
    /// </summary>
    public partial class InvalidFilterFieldException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidFilterFieldException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/invalid-filter-param</c> (Invalid Filter Param, HTTP 422).
    /// </summary>
    public partial class InvalidFilterParamException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidFilterParamException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/invalid-filter-param-format</c> (Invalid Filter Param Format, HTTP 422).
    /// </summary>
    public partial class InvalidFilterParamFormatException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidFilterParamFormatException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/invalid-filter-param-value</c> (Invalid Filter Param Value, HTTP 422).
    /// </summary>
    public partial class InvalidFilterParamValueException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidFilterParamValueException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/invalid-input-encoding</c> (Invalid Input Encoding, HTTP 422).
    /// </summary>
    public partial class InvalidInputEncodingException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidInputEncodingException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/invalid-interface</c> (Invalid Interface, HTTP 422).
    /// </summary>
    public partial class InvalidInterfaceException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidInterfaceException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/invalid-oauth-provider</c> (Invalid Oauth Provider, HTTP 422).
    /// </summary>
    public partial class InvalidOauthProviderException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidOauthProviderException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/invalid-path</c> (Invalid Path, HTTP 422).
    /// </summary>
    public partial class InvalidPathException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidPathException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/invalid-return-to-url</c> (Invalid Return To Url, HTTP 422).
    /// </summary>
    public partial class InvalidReturnToUrlException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidReturnToUrlException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/invalid-search-query</c> (Invalid Search Query, HTTP 422).
    /// </summary>
    public partial class InvalidSearchQueryException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidSearchQueryException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/invalid-sort-field</c> (Invalid Sort Field, HTTP 422).
    /// </summary>
    public partial class InvalidSortFieldException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidSortFieldException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/invalid-sort-filter-combination</c> (Invalid Sort Filter Combination, HTTP 422).
    /// </summary>
    public partial class InvalidSortFilterCombinationException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidSortFilterCombinationException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/invalid-upload-offset</c> (Invalid Upload Offset, HTTP 422).
    /// </summary>
    public partial class InvalidUploadOffsetException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidUploadOffsetException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/invalid-upload-part-gap</c> (Invalid Upload Part Gap, HTTP 422).
    /// </summary>
    public partial class InvalidUploadPartGapException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidUploadPartGapException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/invalid-upload-part-size</c> (Invalid Upload Part Size, HTTP 422).
    /// </summary>
    public partial class InvalidUploadPartSizeException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidUploadPartSizeException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/invalid-workspace-id-header</c> (Invalid Workspace Id Header, HTTP 422).
    /// </summary>
    public partial class InvalidWorkspaceIdHeaderException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidWorkspaceIdHeaderException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/method-not-allowed</c> (Method Not Allowed, HTTP 422).
    /// </summary>
    public partial class MethodNotAllowedException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public MethodNotAllowedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/multiple-sort-params-not-allowed</c> (Multiple Sort Params Not Allowed, HTTP 422).
    /// </summary>
    public partial class MultipleSortParamsNotAllowedException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public MultipleSortParamsNotAllowedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/no-valid-input-params</c> (No Valid Input Params, HTTP 422).
    /// </summary>
    public partial class NoValidInputParamsException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public NoValidInputParamsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/offset-upload-not-allowed-with-malware-scanning</c> (Offset Upload Not Allowed With Malware Scanning, HTTP 422).
    /// </summary>
    public partial class OffsetUploadNotAllowedWithMalwareScanningException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public OffsetUploadNotAllowedWithMalwareScanningException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/part-number-too-large</c> (Part Number Too Large, HTTP 422).
    /// </summary>
    public partial class PartNumberTooLargeException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public PartNumberTooLargeException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/path-cannot-have-trailing-whitespace</c> (Path Cannot Have Trailing Whitespace, HTTP 422).
    /// </summary>
    public partial class PathCannotHaveTrailingWhitespaceException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public PathCannotHaveTrailingWhitespaceException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/reauthentication-needed-fields</c> (Reauthentication Needed Fields, HTTP 422).
    /// </summary>
    public partial class ReauthenticationNeededFieldsException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ReauthenticationNeededFieldsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/request-body-too-large</c> (Request Body Too Large, HTTP 422).
    /// </summary>
    public partial class RequestBodyTooLargeException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public RequestBodyTooLargeException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/request-params-contain-invalid-character</c> (Request Params Contain Invalid Character, HTTP 422).
    /// </summary>
    public partial class RequestParamsContainInvalidCharacterException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public RequestParamsContainInvalidCharacterException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/request-params-invalid</c> (Request Params Invalid, HTTP 422).
    /// </summary>
    public partial class RequestParamsInvalidException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public RequestParamsInvalidException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/request-params-required</c> (Request Params Required, HTTP 422).
    /// </summary>
    public partial class RequestParamsRequiredException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public RequestParamsRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/search-all-on-child-path</c> (Search All On Child Path, HTTP 422).
    /// </summary>
    public partial class SearchAllOnChildPathException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public SearchAllOnChildPathException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/unrecognized-sort-index</c> (Unrecognized Sort Index, HTTP 422).
    /// </summary>
    public partial class UnrecognizedSortIndexException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public UnrecognizedSortIndexException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/unsupported-currency</c> (Unsupported Currency, HTTP 422).
    /// </summary>
    public partial class UnsupportedCurrencyException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public UnsupportedCurrencyException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/unsupported-http-response-format</c> (Unsupported Http Response Format, HTTP 422).
    /// </summary>
    public partial class UnsupportedHttpResponseFormatException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public UnsupportedHttpResponseFormatException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/unsupported-media-type</c> (Unsupported Media Type, HTTP 422).
    /// </summary>
    public partial class UnsupportedMediaTypeException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public UnsupportedMediaTypeException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/user-id-invalid</c> (User Id Invalid, HTTP 422).
    /// </summary>
    public partial class UserIdInvalidException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public UserIdInvalidException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/user-id-on-user-endpoint</c> (User Id On User Endpoint, HTTP 422).
    /// </summary>
    public partial class UserIdOnUserEndpointException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public UserIdOnUserEndpointException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>bad-request/user-required</c> (User Required, HTTP 422).
    /// </summary>
    public partial class UserRequiredException : BadRequestException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public UserRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }

    /// <summary>
    /// An error response from Files.com whose error type is in the <c>not-authenticated</c> group: the base of the exceptions for the group's types, and the exception for a type in it that has no class of its own.
    /// </summary>
    public partial class NotAuthenticatedException : ApiException
    {
        /// <summary>
        /// Creates the exception for an error response of a type in this group.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public NotAuthenticatedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authenticated/additional-authentication-required</c> (Additional Authentication Required, HTTP 401).
    /// </summary>
    public partial class AdditionalAuthenticationRequiredException : NotAuthenticatedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public AdditionalAuthenticationRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authenticated/api-key-sessions-not-supported</c> (Api Key Sessions Not Supported, HTTP 401).
    /// </summary>
    public partial class ApiKeySessionsNotSupportedException : NotAuthenticatedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ApiKeySessionsNotSupportedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authenticated/authentication-required</c> (Authentication Required, HTTP 401).
    /// </summary>
    public partial class AuthenticationRequiredException : NotAuthenticatedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public AuthenticationRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authenticated/bundle-registration-code-failed</c> (Bundle Registration Code Failed, HTTP 401).
    /// </summary>
    public partial class BundleRegistrationCodeFailedException : NotAuthenticatedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public BundleRegistrationCodeFailedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authenticated/inbox-registration-code-failed</c> (Inbox Registration Code Failed, HTTP 401).
    /// </summary>
    public partial class InboxRegistrationCodeFailedException : NotAuthenticatedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InboxRegistrationCodeFailedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authenticated/invalid-credentials</c> (Invalid Credentials, HTTP 401).
    /// </summary>
    public partial class InvalidCredentialsException : NotAuthenticatedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidCredentialsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authenticated/invalid-oauth</c> (Invalid Oauth, HTTP 401).
    /// </summary>
    public partial class InvalidOauthException : NotAuthenticatedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidOauthException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authenticated/invalid-or-expired-code</c> (Invalid Or Expired Code, HTTP 401).
    /// </summary>
    public partial class InvalidOrExpiredCodeException : NotAuthenticatedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidOrExpiredCodeException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authenticated/invalid-session</c> (Invalid Session, HTTP 401).
    /// </summary>
    public partial class InvalidSessionException : NotAuthenticatedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidSessionException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authenticated/invalid-username-or-password</c> (Invalid Username Or Password, HTTP 401).
    /// </summary>
    public partial class InvalidUsernameOrPasswordException : NotAuthenticatedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidUsernameOrPasswordException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authenticated/locked-out</c> (Locked Out, HTTP 401).
    /// </summary>
    public partial class LockedOutException : NotAuthenticatedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public LockedOutException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public partial class LockoutRegionMismatchException : NotAuthenticatedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public LockoutRegionMismatchException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authenticated/one-time-password-incorrect</c> (One Time Password Incorrect, HTTP 401).
    /// </summary>
    public partial class OneTimePasswordIncorrectException : NotAuthenticatedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public OneTimePasswordIncorrectException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authenticated/two-factor-authentication-error</c> (Two Factor Authentication Error, HTTP 401).
    /// </summary>
    public partial class TwoFactorAuthenticationErrorException : NotAuthenticatedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public TwoFactorAuthenticationErrorException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authenticated/two-factor-authentication-setup-expired</c> (Two Factor Authentication Setup Expired, HTTP 401).
    /// </summary>
    public partial class TwoFactorAuthenticationSetupExpiredException : NotAuthenticatedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public TwoFactorAuthenticationSetupExpiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }

    /// <summary>
    /// An error response from Files.com whose error type is in the <c>not-authorized</c> group: the base of the exceptions for the group's types, and the exception for a type in it that has no class of its own.
    /// </summary>
    public partial class NotAuthorizedException : ApiException
    {
        /// <summary>
        /// Creates the exception for an error response of a type in this group.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public NotAuthorizedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/api-key-is-disabled</c> (Api Key Is Disabled, HTTP 403).
    /// </summary>
    public partial class ApiKeyIsDisabledException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ApiKeyIsDisabledException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/api-key-is-path-restricted</c> (Api Key Is Path Restricted, HTTP 403).
    /// </summary>
    public partial class ApiKeyIsPathRestrictedException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ApiKeyIsPathRestrictedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/api-key-only-for-desktop-app</c> (Api Key Only For Desktop App, HTTP 403).
    /// </summary>
    public partial class ApiKeyOnlyForDesktopAppException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ApiKeyOnlyForDesktopAppException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/api-key-only-for-file-operations</c> (Api Key Only For File Operations, HTTP 403).
    /// </summary>
    public partial class ApiKeyOnlyForFileOperationsException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ApiKeyOnlyForFileOperationsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/api-key-only-for-mobile-app</c> (Api Key Only For Mobile App, HTTP 403).
    /// </summary>
    public partial class ApiKeyOnlyForMobileAppException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ApiKeyOnlyForMobileAppException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/api-key-only-for-office-integration</c> (Api Key Only For Office Integration, HTTP 403).
    /// </summary>
    public partial class ApiKeyOnlyForOfficeIntegrationException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ApiKeyOnlyForOfficeIntegrationException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/billing-information-hidden</c> (Billing Information Hidden, HTTP 403).
    /// </summary>
    public partial class BillingInformationHiddenException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public BillingInformationHiddenException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/billing-permission-required</c> (Billing Permission Required, HTTP 403).
    /// </summary>
    public partial class BillingPermissionRequiredException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public BillingPermissionRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/bundle-maximum-uses-reached</c> (Bundle Maximum Uses Reached, HTTP 403).
    /// </summary>
    public partial class BundleMaximumUsesReachedException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public BundleMaximumUsesReachedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/bundle-permission-required</c> (Bundle Permission Required, HTTP 403).
    /// </summary>
    public partial class BundlePermissionRequiredException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public BundlePermissionRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/cannot-administer-higher-level-user</c> (Cannot Administer Higher Level User, HTTP 403).
    /// </summary>
    public partial class CannotAdministerHigherLevelUserException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public CannotAdministerHigherLevelUserException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/cannot-login-while-using-key</c> (Cannot Login While Using Key, HTTP 403).
    /// </summary>
    public partial class CannotLoginWhileUsingKeyException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public CannotLoginWhileUsingKeyException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/cant-act-for-other-user</c> (Cant Act For Other User, HTTP 403).
    /// </summary>
    public partial class CantActForOtherUserException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public CantActForOtherUserException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/contact-admin-for-password-change-help</c> (Contact Admin For Password Change Help, HTTP 403).
    /// </summary>
    public partial class ContactAdminForPasswordChangeHelpException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ContactAdminForPasswordChangeHelpException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/files-agent-failed-authorization</c> (Files Agent Failed Authorization, HTTP 403).
    /// </summary>
    public partial class FilesAgentFailedAuthorizationException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public FilesAgentFailedAuthorizationException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/folder-admin-or-billing-permission-required</c> (Folder Admin Or Billing Permission Required, HTTP 403).
    /// </summary>
    public partial class FolderAdminOrBillingPermissionRequiredException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public FolderAdminOrBillingPermissionRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/folder-admin-permission-required</c> (Folder Admin Permission Required, HTTP 403).
    /// </summary>
    public partial class FolderAdminPermissionRequiredException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public FolderAdminPermissionRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/full-permission-required</c> (Full Permission Required, HTTP 403).
    /// </summary>
    public partial class FullPermissionRequiredException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public FullPermissionRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/history-permission-required</c> (History Permission Required, HTTP 403).
    /// </summary>
    public partial class HistoryPermissionRequiredException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public HistoryPermissionRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/in-app-ai-assistant-unavailable</c> (In App Ai Assistant Unavailable, HTTP 403).
    /// </summary>
    public partial class InAppAiAssistantUnavailableException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InAppAiAssistantUnavailableException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/insufficient-permission-for-params</c> (Insufficient Permission For Params, HTTP 403).
    /// </summary>
    public partial class InsufficientPermissionForParamsException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InsufficientPermissionForParamsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/insufficient-permission-for-site</c> (Insufficient Permission For Site, HTTP 403).
    /// </summary>
    public partial class InsufficientPermissionForSiteException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InsufficientPermissionForSiteException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/mover-access-denied</c> (Mover Access Denied, HTTP 403).
    /// </summary>
    public partial class MoverAccessDeniedException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public MoverAccessDeniedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/mover-package-required</c> (Mover Package Required, HTTP 403).
    /// </summary>
    public partial class MoverPackageRequiredException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public MoverPackageRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/must-authenticate-with-api-key</c> (Must Authenticate With Api Key, HTTP 403).
    /// </summary>
    public partial class MustAuthenticateWithApiKeyException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public MustAuthenticateWithApiKeyException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/need-admin-permission-for-inbox</c> (Need Admin Permission For Inbox, HTTP 403).
    /// </summary>
    public partial class NeedAdminPermissionForInboxException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public NeedAdminPermissionForInboxException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/non-admins-must-query-by-folder-or-path</c> (Non Admins Must Query By Folder Or Path, HTTP 403).
    /// </summary>
    public partial class NonAdminsMustQueryByFolderOrPathException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public NonAdminsMustQueryByFolderOrPathException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/not-allowed-to-create-bundle</c> (Not Allowed To Create Bundle, HTTP 403).
    /// </summary>
    public partial class NotAllowedToCreateBundleException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public NotAllowedToCreateBundleException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/not-enqueuable-sync</c> (Not Enqueuable Sync, HTTP 403).
    /// </summary>
    public partial class NotEnqueuableSyncException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public NotEnqueuableSyncException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/password-change-not-required</c> (Password Change Not Required, HTTP 403).
    /// </summary>
    public partial class PasswordChangeNotRequiredException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public PasswordChangeNotRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/password-change-required</c> (Password Change Required, HTTP 403).
    /// </summary>
    public partial class PasswordChangeRequiredException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public PasswordChangeRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/payment-method-error</c> (Payment Method Error, HTTP 403).
    /// </summary>
    public partial class PaymentMethodErrorException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public PaymentMethodErrorException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/preview-only-permission-cannot-download</c> (Preview Only Permission Cannot Download, HTTP 403).
    /// </summary>
    public partial class PreviewOnlyPermissionCannotDownloadException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public PreviewOnlyPermissionCannotDownloadException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/read-only-session</c> (Read Only Session, HTTP 403).
    /// </summary>
    public partial class ReadOnlySessionException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ReadOnlySessionException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/read-permission-required</c> (Read Permission Required, HTTP 403).
    /// </summary>
    public partial class ReadPermissionRequiredException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ReadPermissionRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/reauthentication-failed</c> (Reauthentication Failed, HTTP 403).
    /// </summary>
    public partial class ReauthenticationFailedException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ReauthenticationFailedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/reauthentication-failed-final</c> (Reauthentication Failed Final, HTTP 403).
    /// </summary>
    public partial class ReauthenticationFailedFinalException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ReauthenticationFailedFinalException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/reauthentication-needed-action</c> (Reauthentication Needed Action, HTTP 403).
    /// </summary>
    public partial class ReauthenticationNeededActionException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ReauthenticationNeededActionException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/recaptcha-failed</c> (Recaptcha Failed, HTTP 403).
    /// </summary>
    public partial class RecaptchaFailedException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public RecaptchaFailedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/remote-desktop-debug-logging-disabled</c> (Remote Desktop Debug Logging Disabled, HTTP 403).
    /// </summary>
    public partial class RemoteDesktopDebugLoggingDisabledException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public RemoteDesktopDebugLoggingDisabledException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/root-folder-behavior-site-admin-required</c> (Root Folder Behavior Site Admin Required, HTTP 403).
    /// </summary>
    public partial class RootFolderBehaviorSiteAdminRequiredException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public RootFolderBehaviorSiteAdminRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/root-folder-behavior-skip-site-admin-required</c> (Root Folder Behavior Skip Site Admin Required, HTTP 403).
    /// </summary>
    public partial class RootFolderBehaviorSkipSiteAdminRequiredException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public RootFolderBehaviorSkipSiteAdminRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/self-managed-required</c> (Self Managed Required, HTTP 403).
    /// </summary>
    public partial class SelfManagedRequiredException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public SelfManagedRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/site-admin-or-partner-admin-permission-required</c> (Site Admin Or Partner Admin Permission Required, HTTP 403).
    /// </summary>
    public partial class SiteAdminOrPartnerAdminPermissionRequiredException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public SiteAdminOrPartnerAdminPermissionRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/site-admin-or-workspace-admin-or-folder-admin-permission-required</c> (Site Admin Or Workspace Admin Or Folder Admin Permission Required, HTTP 403).
    /// </summary>
    public partial class SiteAdminOrWorkspaceAdminOrFolderAdminPermissionRequiredException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public SiteAdminOrWorkspaceAdminOrFolderAdminPermissionRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/site-admin-or-workspace-admin-or-partner-admin-or-folder-admin-permission-required</c> (Site Admin Or Workspace Admin Or Partner Admin Or Folder Admin Permission Required, HTTP 403).
    /// </summary>
    public partial class SiteAdminOrWorkspaceAdminOrPartnerAdminOrFolderAdminPermissionRequiredException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public SiteAdminOrWorkspaceAdminOrPartnerAdminOrFolderAdminPermissionRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/site-admin-or-workspace-admin-or-partner-admin-permission-required</c> (Site Admin Or Workspace Admin Or Partner Admin Permission Required, HTTP 403).
    /// </summary>
    public partial class SiteAdminOrWorkspaceAdminOrPartnerAdminPermissionRequiredException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public SiteAdminOrWorkspaceAdminOrPartnerAdminPermissionRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/site-admin-or-workspace-admin-permission-required</c> (Site Admin Or Workspace Admin Permission Required, HTTP 403).
    /// </summary>
    public partial class SiteAdminOrWorkspaceAdminPermissionRequiredException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public SiteAdminOrWorkspaceAdminPermissionRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/site-admin-required</c> (Site Admin Required, HTTP 403).
    /// </summary>
    public partial class SiteAdminRequiredException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public SiteAdminRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/site-files-are-immutable</c> (Site Files Are Immutable, HTTP 403).
    /// </summary>
    public partial class SiteFilesAreImmutableException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public SiteFilesAreImmutableException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/two-factor-authentication-required</c> (Two Factor Authentication Required, HTTP 403).
    /// </summary>
    public partial class TwoFactorAuthenticationRequiredException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public TwoFactorAuthenticationRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/user-id-without-site-admin</c> (User Id Without Site Admin, HTTP 403).
    /// </summary>
    public partial class UserIdWithoutSiteAdminException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public UserIdWithoutSiteAdminException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/write-and-bundle-permission-required</c> (Write And Bundle Permission Required, HTTP 403).
    /// </summary>
    public partial class WriteAndBundlePermissionRequiredException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public WriteAndBundlePermissionRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-authorized/write-permission-required</c> (Write Permission Required, HTTP 403).
    /// </summary>
    public partial class WritePermissionRequiredException : NotAuthorizedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public WritePermissionRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }

    /// <summary>
    /// An error response from Files.com whose error type is in the <c>not-found</c> group (Not Found, HTTP 404): the base of the exceptions for the group's types, and the exception for a type in it that has no class of its own.
    /// </summary>
    public partial class NotFoundException : ApiException
    {
        /// <summary>
        /// Creates the exception for an error response of a type in this group.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public NotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-found/api-key-not-found</c> (Api Key Not Found, HTTP 404).
    /// </summary>
    public partial class ApiKeyNotFoundException : NotFoundException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ApiKeyNotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-found/bundle-path-not-found</c> (Bundle Path Not Found, HTTP 404).
    /// </summary>
    public partial class BundlePathNotFoundException : NotFoundException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public BundlePathNotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-found/bundle-registration-not-found</c> (Bundle Registration Not Found, HTTP 404).
    /// </summary>
    public partial class BundleRegistrationNotFoundException : NotFoundException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public BundleRegistrationNotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-found/code-not-found</c> (Code Not Found, HTTP 404).
    /// </summary>
    public partial class CodeNotFoundException : NotFoundException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public CodeNotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-found/file-not-found</c> (File Not Found, HTTP 404).
    /// </summary>
    public partial class FileNotFoundException : NotFoundException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public FileNotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-found/file-upload-not-found</c> (File Upload Not Found, HTTP 404).
    /// </summary>
    public partial class FileUploadNotFoundException : NotFoundException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public FileUploadNotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-found/group-not-found</c> (Group Not Found, HTTP 404).
    /// </summary>
    public partial class GroupNotFoundException : NotFoundException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public GroupNotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-found/inbox-not-found</c> (Inbox Not Found, HTTP 404).
    /// </summary>
    public partial class InboxNotFoundException : NotFoundException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InboxNotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-found/nested-not-found</c> (Nested Not Found, HTTP 404).
    /// </summary>
    public partial class NestedNotFoundException : NotFoundException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public NestedNotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-found/plan-not-found</c> (Plan Not Found, HTTP 404).
    /// </summary>
    public partial class PlanNotFoundException : NotFoundException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public PlanNotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-found/site-not-found</c> (Site Not Found, HTTP 404).
    /// </summary>
    public partial class SiteNotFoundException : NotFoundException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public SiteNotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>not-found/user-not-found</c> (User Not Found, HTTP 404).
    /// </summary>
    public partial class UserNotFoundException : NotFoundException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public UserNotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }

    /// <summary>
    /// An error response from Files.com whose error type is in the <c>processing-failure</c> group (Processing Failure, HTTP 422): the base of the exceptions for the group's types, and the exception for a type in it that has no class of its own.
    /// </summary>
    public partial class ProcessingFailureException : ApiException
    {
        /// <summary>
        /// Creates the exception for an error response of a type in this group.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ProcessingFailureException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/agent-push-update-blocked</c> (Agent Push Update Blocked, HTTP 409).
    /// </summary>
    public partial class AgentPushUpdateBlockedException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public AgentPushUpdateBlockedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/agent-unavailable</c> (Agent Unavailable, HTTP 422).
    /// </summary>
    public partial class AgentUnavailableException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public AgentUnavailableException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/ai-task-cannot-be-run-manually</c> (Ai Task Cannot Be Run Manually, HTTP 422).
    /// </summary>
    public partial class AiTaskCannotBeRunManuallyException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public AiTaskCannotBeRunManuallyException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/already-completed</c> (Already Completed, HTTP 422).
    /// </summary>
    public partial class AlreadyCompletedException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public AlreadyCompletedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/automation-cannot-be-run-manually</c> (Automation Cannot Be Run Manually, HTTP 422).
    /// </summary>
    public partial class AutomationCannotBeRunManuallyException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public AutomationCannotBeRunManuallyException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/behavior-not-allowed-on-remote-server</c> (Behavior Not Allowed On Remote Server, HTTP 422).
    /// </summary>
    public partial class BehaviorNotAllowedOnRemoteServerException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public BehaviorNotAllowedOnRemoteServerException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/buffered-upload-disabled-for-this-destination</c> (Buffered Upload Disabled For This Destination, HTTP 422).
    /// </summary>
    public partial class BufferedUploadDisabledForThisDestinationException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public BufferedUploadDisabledForThisDestinationException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/bundle-only-allows-previews</c> (Bundle Only Allows Previews, HTTP 422).
    /// </summary>
    public partial class BundleOnlyAllowsPreviewsException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public BundleOnlyAllowsPreviewsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/bundle-operation-requires-subfolder</c> (Bundle Operation Requires Subfolder, HTTP 422).
    /// </summary>
    public partial class BundleOperationRequiresSubfolderException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public BundleOperationRequiresSubfolderException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/configuration-locked-path</c> (Configuration Locked Path, HTTP 422).
    /// </summary>
    public partial class ConfigurationLockedPathException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ConfigurationLockedPathException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/could-not-create-parent</c> (Could Not Create Parent, HTTP 422).
    /// </summary>
    public partial class CouldNotCreateParentException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public CouldNotCreateParentException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/destination-exists</c> (Destination Exists, HTTP 422).
    /// </summary>
    public partial class DestinationExistsException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public DestinationExistsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/destination-folder-limited</c> (Destination Folder Limited, HTTP 422).
    /// </summary>
    public partial class DestinationFolderLimitedException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public DestinationFolderLimitedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/destination-parent-conflict</c> (Destination Parent Conflict, HTTP 422).
    /// </summary>
    public partial class DestinationParentConflictException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public DestinationParentConflictException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/destination-parent-does-not-exist</c> (Destination Parent Does Not Exist, HTTP 409).
    /// </summary>
    public partial class DestinationParentDoesNotExistException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public DestinationParentDoesNotExistException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/exceeded-runtime-limit</c> (Exceeded Runtime Limit, HTTP 422).
    /// </summary>
    public partial class ExceededRuntimeLimitException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ExceededRuntimeLimitException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/expectation-already-has-open-window</c> (Expectation Already Has Open Window, HTTP 422).
    /// </summary>
    public partial class ExpectationAlreadyHasOpenWindowException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ExpectationAlreadyHasOpenWindowException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/expectation-not-manual-trigger</c> (Expectation Not Manual Trigger, HTTP 422).
    /// </summary>
    public partial class ExpectationNotManualTriggerException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ExpectationNotManualTriggerException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/expired-private-key</c> (Expired Private Key, HTTP 422).
    /// </summary>
    public partial class ExpiredPrivateKeyException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ExpiredPrivateKeyException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/expired-public-key</c> (Expired Public Key, HTTP 422).
    /// </summary>
    public partial class ExpiredPublicKeyException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ExpiredPublicKeyException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/export-failure</c> (Export Failure, HTTP 422).
    /// </summary>
    public partial class ExportFailureException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ExportFailureException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/export-not-ready</c> (Export Not Ready, HTTP 422).
    /// </summary>
    public partial class ExportNotReadyException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ExportNotReadyException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/failed-to-change-password</c> (Failed To Change Password, HTTP 422).
    /// </summary>
    public partial class FailedToChangePasswordException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public FailedToChangePasswordException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/file-locked</c> (File Locked, HTTP 422).
    /// </summary>
    public partial class FileLockedException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public FileLockedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/file-not-uploaded</c> (File Not Uploaded, HTTP 422).
    /// </summary>
    public partial class FileNotUploadedException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public FileNotUploadedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/file-pending-processing</c> (File Pending Processing, HTTP 422).
    /// </summary>
    public partial class FilePendingProcessingException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public FilePendingProcessingException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/file-processing-error</c> (File Processing Error, HTTP 422).
    /// </summary>
    public partial class FileProcessingErrorException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public FileProcessingErrorException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/file-too-big-to-decrypt</c> (File Too Big To Decrypt, HTTP 422).
    /// </summary>
    public partial class FileTooBigToDecryptException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public FileTooBigToDecryptException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/file-too-big-to-encrypt</c> (File Too Big To Encrypt, HTTP 422).
    /// </summary>
    public partial class FileTooBigToEncryptException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public FileTooBigToEncryptException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/file-uploaded-to-wrong-region</c> (File Uploaded To Wrong Region, HTTP 422).
    /// </summary>
    public partial class FileUploadedToWrongRegionException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public FileUploadedToWrongRegionException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/filename-too-long</c> (Filename Too Long, HTTP 422).
    /// </summary>
    public partial class FilenameTooLongException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public FilenameTooLongException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/folder-locked</c> (Folder Locked, HTTP 403).
    /// </summary>
    public partial class FolderLockedException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public FolderLockedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/folder-not-empty</c> (Folder Not Empty, HTTP 422).
    /// </summary>
    public partial class FolderNotEmptyException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public FolderNotEmptyException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/history-unavailable</c> (History Unavailable, HTTP 422).
    /// </summary>
    public partial class HistoryUnavailableException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public HistoryUnavailableException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/invalid-bundle-code</c> (Invalid Bundle Code, HTTP 422).
    /// </summary>
    public partial class InvalidBundleCodeException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidBundleCodeException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/invalid-file-type</c> (Invalid File Type, HTTP 422).
    /// </summary>
    public partial class InvalidFileTypeException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidFileTypeException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/invalid-filename</c> (Invalid Filename, HTTP 422).
    /// </summary>
    public partial class InvalidFilenameException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidFilenameException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/invalid-priority-color</c> (Invalid Priority Color, HTTP 422).
    /// </summary>
    public partial class InvalidPriorityColorException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidPriorityColorException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/invalid-range</c> (Invalid Range, HTTP 422).
    /// </summary>
    public partial class InvalidRangeException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidRangeException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/invalid-site</c> (Invalid Site, HTTP 422).
    /// </summary>
    public partial class InvalidSiteException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidSiteException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/invalid-zip-file</c> (Invalid Zip File, HTTP 422).
    /// </summary>
    public partial class InvalidZipFileException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public InvalidZipFileException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/metadata-not-supported-on-remotes</c> (Metadata Not Supported On Remotes, HTTP 422).
    /// </summary>
    public partial class MetadataNotSupportedOnRemotesException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public MetadataNotSupportedOnRemotesException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/model-save-error</c> (Model Save Error, HTTP 422).
    /// </summary>
    public partial class ModelSaveErrorException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ModelSaveErrorException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/multiple-processing-errors</c> (Multiple Processing Errors, HTTP 422).
    /// </summary>
    public partial class MultipleProcessingErrorsException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public MultipleProcessingErrorsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/path-too-long</c> (Path Too Long, HTTP 422).
    /// </summary>
    public partial class PathTooLongException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public PathTooLongException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/recipient-already-shared</c> (Recipient Already Shared, HTTP 422).
    /// </summary>
    public partial class RecipientAlreadySharedException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public RecipientAlreadySharedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/remote-entry-read-only</c> (Remote Entry Read Only, HTTP 422).
    /// </summary>
    public partial class RemoteEntryReadOnlyException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public RemoteEntryReadOnlyException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/remote-server-error</c> (Remote Server Error, HTTP 422).
    /// </summary>
    public partial class RemoteServerErrorException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public RemoteServerErrorException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/resource-belongs-to-parent-site</c> (Resource Belongs To Parent Site, HTTP 422).
    /// </summary>
    public partial class ResourceBelongsToParentSiteException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ResourceBelongsToParentSiteException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/resource-locked</c> (Resource Locked, HTTP 422).
    /// </summary>
    public partial class ResourceLockedException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ResourceLockedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/subfolder-locked</c> (Subfolder Locked, HTTP 422).
    /// </summary>
    public partial class SubfolderLockedException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public SubfolderLockedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/sync-in-progress</c> (Sync In Progress, HTTP 422).
    /// </summary>
    public partial class SyncInProgressException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public SyncInProgressException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/two-factor-authentication-code-already-sent</c> (Two Factor Authentication Code Already Sent, HTTP 422).
    /// </summary>
    public partial class TwoFactorAuthenticationCodeAlreadySentException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public TwoFactorAuthenticationCodeAlreadySentException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/two-factor-authentication-country-blacklisted</c> (Two Factor Authentication Country Blacklisted, HTTP 422).
    /// </summary>
    public partial class TwoFactorAuthenticationCountryBlacklistedException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public TwoFactorAuthenticationCountryBlacklistedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/two-factor-authentication-general-error</c> (Two Factor Authentication General Error, HTTP 422).
    /// </summary>
    public partial class TwoFactorAuthenticationGeneralErrorException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public TwoFactorAuthenticationGeneralErrorException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/two-factor-authentication-method-unsupported-error</c> (Two Factor Authentication Method Unsupported Error, HTTP 422).
    /// </summary>
    public partial class TwoFactorAuthenticationMethodUnsupportedErrorException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public TwoFactorAuthenticationMethodUnsupportedErrorException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/two-factor-authentication-unsubscribed-recipient</c> (Two Factor Authentication Unsubscribed Recipient, HTTP 422).
    /// </summary>
    public partial class TwoFactorAuthenticationUnsubscribedRecipientException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public TwoFactorAuthenticationUnsubscribedRecipientException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>processing-failure/updates-not-allowed-for-remotes</c> (Updates Not Allowed For Remotes, HTTP 422).
    /// </summary>
    public partial class UpdatesNotAllowedForRemotesException : ProcessingFailureException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public UpdatesNotAllowedForRemotesException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }

    /// <summary>
    /// An error response from Files.com whose error type is in the <c>rate-limited</c> group: the base of the exceptions for the group's types, and the exception for a type in it that has no class of its own.
    /// </summary>
    public partial class RateLimitedException : ApiException
    {
        /// <summary>
        /// Creates the exception for an error response of a type in this group.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public RateLimitedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>rate-limited/duplicate-share-recipient</c> (Duplicate Share Recipient, HTTP 429).
    /// </summary>
    public partial class DuplicateShareRecipientException : RateLimitedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public DuplicateShareRecipientException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>rate-limited/reauthentication-rate-limited</c> (Reauthentication Rate Limited, HTTP 429).
    /// </summary>
    public partial class ReauthenticationRateLimitedException : RateLimitedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ReauthenticationRateLimitedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>rate-limited/too-many-concurrent-logins</c> (Too Many Concurrent Logins, HTTP 429).
    /// </summary>
    public partial class TooManyConcurrentLoginsException : RateLimitedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public TooManyConcurrentLoginsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>rate-limited/too-many-concurrent-requests</c> (Too Many Concurrent Requests, HTTP 429).
    /// </summary>
    public partial class TooManyConcurrentRequestsException : RateLimitedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public TooManyConcurrentRequestsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>rate-limited/too-many-login-attempts</c> (Too Many Login Attempts, HTTP 429).
    /// </summary>
    public partial class TooManyLoginAttemptsException : RateLimitedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public TooManyLoginAttemptsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>rate-limited/too-many-requests</c> (Too Many Requests, HTTP 429).
    /// </summary>
    public partial class TooManyRequestsException : RateLimitedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public TooManyRequestsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>rate-limited/too-many-shares</c> (Too Many Shares, HTTP 429).
    /// </summary>
    public partial class TooManySharesException : RateLimitedException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public TooManySharesException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }

    /// <summary>
    /// An error response from Files.com whose error type is in the <c>service-unavailable</c> group: the base of the exceptions for the group's types, and the exception for a type in it that has no class of its own.
    /// </summary>
    public partial class ServiceUnavailableException : ApiException
    {
        /// <summary>
        /// Creates the exception for an error response of a type in this group.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public ServiceUnavailableException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>service-unavailable/automations-unavailable</c> (Automations Unavailable, HTTP 422).
    /// </summary>
    public partial class AutomationsUnavailableException : ServiceUnavailableException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public AutomationsUnavailableException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>service-unavailable/lock-operation-busy</c> (Lock Operation Busy, HTTP 422).
    /// </summary>
    public partial class LockOperationBusyException : ServiceUnavailableException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public LockOperationBusyException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>service-unavailable/migration-in-progress</c> (Migration In Progress, HTTP 422).
    /// </summary>
    public partial class MigrationInProgressException : ServiceUnavailableException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public MigrationInProgressException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>service-unavailable/search-unavailable</c> (Search Unavailable, HTTP 422).
    /// </summary>
    public partial class SearchUnavailableException : ServiceUnavailableException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public SearchUnavailableException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>service-unavailable/site-disabled</c> (Site Disabled, HTTP 422).
    /// </summary>
    public partial class SiteDisabledException : ServiceUnavailableException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public SiteDisabledException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>service-unavailable/uploads-unavailable</c> (Uploads Unavailable, HTTP 422).
    /// </summary>
    public partial class UploadsUnavailableException : ServiceUnavailableException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public UploadsUnavailableException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }

    /// <summary>
    /// An error response from Files.com whose error type is in the <c>site-configuration</c> group: the base of the exceptions for the group's types, and the exception for a type in it that has no class of its own.
    /// </summary>
    public partial class SiteConfigurationException : ApiException
    {
        /// <summary>
        /// Creates the exception for an error response of a type in this group.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public SiteConfigurationException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>site-configuration/account-already-exists</c> (Account Already Exists, HTTP 403).
    /// </summary>
    public partial class AccountAlreadyExistsException : SiteConfigurationException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public AccountAlreadyExistsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>site-configuration/account-overdue</c> (Account Overdue, HTTP 403).
    /// </summary>
    public partial class AccountOverdueException : SiteConfigurationException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public AccountOverdueException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>site-configuration/no-account-for-site</c> (No Account For Site, HTTP 403).
    /// </summary>
    public partial class NoAccountForSiteException : SiteConfigurationException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public NoAccountForSiteException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>site-configuration/site-was-removed</c> (Site Was Removed, HTTP 403).
    /// </summary>
    public partial class SiteWasRemovedException : SiteConfigurationException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public SiteWasRemovedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>site-configuration/trial-expired</c> (Trial Expired, HTTP 403).
    /// </summary>
    public partial class TrialExpiredException : SiteConfigurationException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public TrialExpiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>site-configuration/trial-locked</c> (Trial Locked, HTTP 403).
    /// </summary>
    public partial class TrialLockedException : SiteConfigurationException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public TrialLockedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    /// <summary>
    /// An error response from Files.com whose error type is <c>site-configuration/user-requests-enabled-required</c> (User Requests Enabled Required, HTTP 403).
    /// </summary>
    public partial class UserRequestsEnabledRequiredException : SiteConfigurationException
    {
        /// <summary>
        /// Creates the exception for an error response of this type.
        /// </summary>
        /// <param name="message">The response's error text, or null.</param>
        /// <param name="httpStatus">The response's HTTP status.</param>
        /// <param name="responseError">The response's error, as it came.</param>
        /// <param name="httpHeaders">The response's headers.</param>
        public UserRequestsEnabledRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }

    // The exception for each error type the API documents, and for each error group.
    internal static class ApiErrors
    {
        private delegate ApiException Constructor(string message, int httpStatus, ResponseError responseError, HttpHeaders httpHeaders);

        private static readonly Dictionary<string, Constructor> ByType = new Dictionary<string, Constructor>(StringComparer.Ordinal)
        {
            { "bad-request", (message, httpStatus, responseError, httpHeaders) => new BadRequestException(message, httpStatus, responseError, httpHeaders) },
            { "not-authenticated", (message, httpStatus, responseError, httpHeaders) => new NotAuthenticatedException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized", (message, httpStatus, responseError, httpHeaders) => new NotAuthorizedException(message, httpStatus, responseError, httpHeaders) },
            { "not-found", (message, httpStatus, responseError, httpHeaders) => new NotFoundException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure", (message, httpStatus, responseError, httpHeaders) => new ProcessingFailureException(message, httpStatus, responseError, httpHeaders) },
            { "rate-limited", (message, httpStatus, responseError, httpHeaders) => new RateLimitedException(message, httpStatus, responseError, httpHeaders) },
            { "service-unavailable", (message, httpStatus, responseError, httpHeaders) => new ServiceUnavailableException(message, httpStatus, responseError, httpHeaders) },
            { "site-configuration", (message, httpStatus, responseError, httpHeaders) => new SiteConfigurationException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/agent-upgrade-required", (message, httpStatus, responseError, httpHeaders) => new AgentUpgradeRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/attachment-too-large", (message, httpStatus, responseError, httpHeaders) => new AttachmentTooLargeException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/cannot-download-directory", (message, httpStatus, responseError, httpHeaders) => new CannotDownloadDirectoryException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/cant-move-with-multiple-locations", (message, httpStatus, responseError, httpHeaders) => new CantMoveWithMultipleLocationsException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/datetime-parse", (message, httpStatus, responseError, httpHeaders) => new DatetimeParseException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/destination-same", (message, httpStatus, responseError, httpHeaders) => new DestinationSameException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/destination-site-mismatch", (message, httpStatus, responseError, httpHeaders) => new DestinationSiteMismatchException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/does-not-support-sorting", (message, httpStatus, responseError, httpHeaders) => new DoesNotSupportSortingException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/folder-must-not-be-a-file", (message, httpStatus, responseError, httpHeaders) => new FolderMustNotBeAFileException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/folders-not-allowed", (message, httpStatus, responseError, httpHeaders) => new FoldersNotAllowedException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/internal-general-error", (message, httpStatus, responseError, httpHeaders) => new InternalGeneralErrorException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/invalid-body", (message, httpStatus, responseError, httpHeaders) => new InvalidBodyException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/invalid-cursor", (message, httpStatus, responseError, httpHeaders) => new InvalidCursorException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/invalid-cursor-type-for-sort", (message, httpStatus, responseError, httpHeaders) => new InvalidCursorTypeForSortException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/invalid-etags", (message, httpStatus, responseError, httpHeaders) => new InvalidEtagsException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/invalid-filter-alias-combination", (message, httpStatus, responseError, httpHeaders) => new InvalidFilterAliasCombinationException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/invalid-filter-field", (message, httpStatus, responseError, httpHeaders) => new InvalidFilterFieldException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/invalid-filter-param", (message, httpStatus, responseError, httpHeaders) => new InvalidFilterParamException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/invalid-filter-param-format", (message, httpStatus, responseError, httpHeaders) => new InvalidFilterParamFormatException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/invalid-filter-param-value", (message, httpStatus, responseError, httpHeaders) => new InvalidFilterParamValueException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/invalid-input-encoding", (message, httpStatus, responseError, httpHeaders) => new InvalidInputEncodingException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/invalid-interface", (message, httpStatus, responseError, httpHeaders) => new InvalidInterfaceException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/invalid-oauth-provider", (message, httpStatus, responseError, httpHeaders) => new InvalidOauthProviderException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/invalid-path", (message, httpStatus, responseError, httpHeaders) => new InvalidPathException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/invalid-return-to-url", (message, httpStatus, responseError, httpHeaders) => new InvalidReturnToUrlException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/invalid-search-query", (message, httpStatus, responseError, httpHeaders) => new InvalidSearchQueryException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/invalid-sort-field", (message, httpStatus, responseError, httpHeaders) => new InvalidSortFieldException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/invalid-sort-filter-combination", (message, httpStatus, responseError, httpHeaders) => new InvalidSortFilterCombinationException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/invalid-upload-offset", (message, httpStatus, responseError, httpHeaders) => new InvalidUploadOffsetException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/invalid-upload-part-gap", (message, httpStatus, responseError, httpHeaders) => new InvalidUploadPartGapException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/invalid-upload-part-size", (message, httpStatus, responseError, httpHeaders) => new InvalidUploadPartSizeException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/invalid-workspace-id-header", (message, httpStatus, responseError, httpHeaders) => new InvalidWorkspaceIdHeaderException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/method-not-allowed", (message, httpStatus, responseError, httpHeaders) => new MethodNotAllowedException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/multiple-sort-params-not-allowed", (message, httpStatus, responseError, httpHeaders) => new MultipleSortParamsNotAllowedException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/no-valid-input-params", (message, httpStatus, responseError, httpHeaders) => new NoValidInputParamsException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/offset-upload-not-allowed-with-malware-scanning", (message, httpStatus, responseError, httpHeaders) => new OffsetUploadNotAllowedWithMalwareScanningException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/part-number-too-large", (message, httpStatus, responseError, httpHeaders) => new PartNumberTooLargeException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/path-cannot-have-trailing-whitespace", (message, httpStatus, responseError, httpHeaders) => new PathCannotHaveTrailingWhitespaceException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/reauthentication-needed-fields", (message, httpStatus, responseError, httpHeaders) => new ReauthenticationNeededFieldsException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/request-body-too-large", (message, httpStatus, responseError, httpHeaders) => new RequestBodyTooLargeException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/request-params-contain-invalid-character", (message, httpStatus, responseError, httpHeaders) => new RequestParamsContainInvalidCharacterException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/request-params-invalid", (message, httpStatus, responseError, httpHeaders) => new RequestParamsInvalidException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/request-params-required", (message, httpStatus, responseError, httpHeaders) => new RequestParamsRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/search-all-on-child-path", (message, httpStatus, responseError, httpHeaders) => new SearchAllOnChildPathException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/unrecognized-sort-index", (message, httpStatus, responseError, httpHeaders) => new UnrecognizedSortIndexException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/unsupported-currency", (message, httpStatus, responseError, httpHeaders) => new UnsupportedCurrencyException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/unsupported-http-response-format", (message, httpStatus, responseError, httpHeaders) => new UnsupportedHttpResponseFormatException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/unsupported-media-type", (message, httpStatus, responseError, httpHeaders) => new UnsupportedMediaTypeException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/user-id-invalid", (message, httpStatus, responseError, httpHeaders) => new UserIdInvalidException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/user-id-on-user-endpoint", (message, httpStatus, responseError, httpHeaders) => new UserIdOnUserEndpointException(message, httpStatus, responseError, httpHeaders) },
            { "bad-request/user-required", (message, httpStatus, responseError, httpHeaders) => new UserRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authenticated/additional-authentication-required", (message, httpStatus, responseError, httpHeaders) => new AdditionalAuthenticationRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authenticated/api-key-sessions-not-supported", (message, httpStatus, responseError, httpHeaders) => new ApiKeySessionsNotSupportedException(message, httpStatus, responseError, httpHeaders) },
            { "not-authenticated/authentication-required", (message, httpStatus, responseError, httpHeaders) => new AuthenticationRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authenticated/bundle-registration-code-failed", (message, httpStatus, responseError, httpHeaders) => new BundleRegistrationCodeFailedException(message, httpStatus, responseError, httpHeaders) },
            { "not-authenticated/inbox-registration-code-failed", (message, httpStatus, responseError, httpHeaders) => new InboxRegistrationCodeFailedException(message, httpStatus, responseError, httpHeaders) },
            { "not-authenticated/invalid-credentials", (message, httpStatus, responseError, httpHeaders) => new InvalidCredentialsException(message, httpStatus, responseError, httpHeaders) },
            { "not-authenticated/invalid-oauth", (message, httpStatus, responseError, httpHeaders) => new InvalidOauthException(message, httpStatus, responseError, httpHeaders) },
            { "not-authenticated/invalid-or-expired-code", (message, httpStatus, responseError, httpHeaders) => new InvalidOrExpiredCodeException(message, httpStatus, responseError, httpHeaders) },
            { "not-authenticated/invalid-session", (message, httpStatus, responseError, httpHeaders) => new InvalidSessionException(message, httpStatus, responseError, httpHeaders) },
            { "not-authenticated/invalid-username-or-password", (message, httpStatus, responseError, httpHeaders) => new InvalidUsernameOrPasswordException(message, httpStatus, responseError, httpHeaders) },
            { "not-authenticated/locked-out", (message, httpStatus, responseError, httpHeaders) => new LockedOutException(message, httpStatus, responseError, httpHeaders) },
            { "not-authenticated/lockout-region-mismatch", (message, httpStatus, responseError, httpHeaders) => new LockoutRegionMismatchException(message, httpStatus, responseError, httpHeaders) },
            { "not-authenticated/one-time-password-incorrect", (message, httpStatus, responseError, httpHeaders) => new OneTimePasswordIncorrectException(message, httpStatus, responseError, httpHeaders) },
            { "not-authenticated/two-factor-authentication-error", (message, httpStatus, responseError, httpHeaders) => new TwoFactorAuthenticationErrorException(message, httpStatus, responseError, httpHeaders) },
            { "not-authenticated/two-factor-authentication-setup-expired", (message, httpStatus, responseError, httpHeaders) => new TwoFactorAuthenticationSetupExpiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/api-key-is-disabled", (message, httpStatus, responseError, httpHeaders) => new ApiKeyIsDisabledException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/api-key-is-path-restricted", (message, httpStatus, responseError, httpHeaders) => new ApiKeyIsPathRestrictedException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/api-key-only-for-desktop-app", (message, httpStatus, responseError, httpHeaders) => new ApiKeyOnlyForDesktopAppException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/api-key-only-for-file-operations", (message, httpStatus, responseError, httpHeaders) => new ApiKeyOnlyForFileOperationsException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/api-key-only-for-mobile-app", (message, httpStatus, responseError, httpHeaders) => new ApiKeyOnlyForMobileAppException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/api-key-only-for-office-integration", (message, httpStatus, responseError, httpHeaders) => new ApiKeyOnlyForOfficeIntegrationException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/billing-information-hidden", (message, httpStatus, responseError, httpHeaders) => new BillingInformationHiddenException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/billing-permission-required", (message, httpStatus, responseError, httpHeaders) => new BillingPermissionRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/bundle-maximum-uses-reached", (message, httpStatus, responseError, httpHeaders) => new BundleMaximumUsesReachedException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/bundle-permission-required", (message, httpStatus, responseError, httpHeaders) => new BundlePermissionRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/cannot-administer-higher-level-user", (message, httpStatus, responseError, httpHeaders) => new CannotAdministerHigherLevelUserException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/cannot-login-while-using-key", (message, httpStatus, responseError, httpHeaders) => new CannotLoginWhileUsingKeyException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/cant-act-for-other-user", (message, httpStatus, responseError, httpHeaders) => new CantActForOtherUserException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/contact-admin-for-password-change-help", (message, httpStatus, responseError, httpHeaders) => new ContactAdminForPasswordChangeHelpException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/files-agent-failed-authorization", (message, httpStatus, responseError, httpHeaders) => new FilesAgentFailedAuthorizationException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/folder-admin-or-billing-permission-required", (message, httpStatus, responseError, httpHeaders) => new FolderAdminOrBillingPermissionRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/folder-admin-permission-required", (message, httpStatus, responseError, httpHeaders) => new FolderAdminPermissionRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/full-permission-required", (message, httpStatus, responseError, httpHeaders) => new FullPermissionRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/history-permission-required", (message, httpStatus, responseError, httpHeaders) => new HistoryPermissionRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/in-app-ai-assistant-unavailable", (message, httpStatus, responseError, httpHeaders) => new InAppAiAssistantUnavailableException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/insufficient-permission-for-params", (message, httpStatus, responseError, httpHeaders) => new InsufficientPermissionForParamsException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/insufficient-permission-for-site", (message, httpStatus, responseError, httpHeaders) => new InsufficientPermissionForSiteException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/mover-access-denied", (message, httpStatus, responseError, httpHeaders) => new MoverAccessDeniedException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/mover-package-required", (message, httpStatus, responseError, httpHeaders) => new MoverPackageRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/must-authenticate-with-api-key", (message, httpStatus, responseError, httpHeaders) => new MustAuthenticateWithApiKeyException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/need-admin-permission-for-inbox", (message, httpStatus, responseError, httpHeaders) => new NeedAdminPermissionForInboxException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/non-admins-must-query-by-folder-or-path", (message, httpStatus, responseError, httpHeaders) => new NonAdminsMustQueryByFolderOrPathException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/not-allowed-to-create-bundle", (message, httpStatus, responseError, httpHeaders) => new NotAllowedToCreateBundleException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/not-enqueuable-sync", (message, httpStatus, responseError, httpHeaders) => new NotEnqueuableSyncException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/password-change-not-required", (message, httpStatus, responseError, httpHeaders) => new PasswordChangeNotRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/password-change-required", (message, httpStatus, responseError, httpHeaders) => new PasswordChangeRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/payment-method-error", (message, httpStatus, responseError, httpHeaders) => new PaymentMethodErrorException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/preview-only-permission-cannot-download", (message, httpStatus, responseError, httpHeaders) => new PreviewOnlyPermissionCannotDownloadException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/read-only-session", (message, httpStatus, responseError, httpHeaders) => new ReadOnlySessionException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/read-permission-required", (message, httpStatus, responseError, httpHeaders) => new ReadPermissionRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/reauthentication-failed", (message, httpStatus, responseError, httpHeaders) => new ReauthenticationFailedException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/reauthentication-failed-final", (message, httpStatus, responseError, httpHeaders) => new ReauthenticationFailedFinalException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/reauthentication-needed-action", (message, httpStatus, responseError, httpHeaders) => new ReauthenticationNeededActionException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/recaptcha-failed", (message, httpStatus, responseError, httpHeaders) => new RecaptchaFailedException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/remote-desktop-debug-logging-disabled", (message, httpStatus, responseError, httpHeaders) => new RemoteDesktopDebugLoggingDisabledException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/root-folder-behavior-site-admin-required", (message, httpStatus, responseError, httpHeaders) => new RootFolderBehaviorSiteAdminRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/root-folder-behavior-skip-site-admin-required", (message, httpStatus, responseError, httpHeaders) => new RootFolderBehaviorSkipSiteAdminRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/self-managed-required", (message, httpStatus, responseError, httpHeaders) => new SelfManagedRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/site-admin-or-partner-admin-permission-required", (message, httpStatus, responseError, httpHeaders) => new SiteAdminOrPartnerAdminPermissionRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/site-admin-or-workspace-admin-or-folder-admin-permission-required", (message, httpStatus, responseError, httpHeaders) => new SiteAdminOrWorkspaceAdminOrFolderAdminPermissionRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/site-admin-or-workspace-admin-or-partner-admin-or-folder-admin-permission-required", (message, httpStatus, responseError, httpHeaders) => new SiteAdminOrWorkspaceAdminOrPartnerAdminOrFolderAdminPermissionRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/site-admin-or-workspace-admin-or-partner-admin-permission-required", (message, httpStatus, responseError, httpHeaders) => new SiteAdminOrWorkspaceAdminOrPartnerAdminPermissionRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/site-admin-or-workspace-admin-permission-required", (message, httpStatus, responseError, httpHeaders) => new SiteAdminOrWorkspaceAdminPermissionRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/site-admin-required", (message, httpStatus, responseError, httpHeaders) => new SiteAdminRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/site-files-are-immutable", (message, httpStatus, responseError, httpHeaders) => new SiteFilesAreImmutableException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/two-factor-authentication-required", (message, httpStatus, responseError, httpHeaders) => new TwoFactorAuthenticationRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/user-id-without-site-admin", (message, httpStatus, responseError, httpHeaders) => new UserIdWithoutSiteAdminException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/write-and-bundle-permission-required", (message, httpStatus, responseError, httpHeaders) => new WriteAndBundlePermissionRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-authorized/write-permission-required", (message, httpStatus, responseError, httpHeaders) => new WritePermissionRequiredException(message, httpStatus, responseError, httpHeaders) },
            { "not-found/api-key-not-found", (message, httpStatus, responseError, httpHeaders) => new ApiKeyNotFoundException(message, httpStatus, responseError, httpHeaders) },
            { "not-found/bundle-path-not-found", (message, httpStatus, responseError, httpHeaders) => new BundlePathNotFoundException(message, httpStatus, responseError, httpHeaders) },
            { "not-found/bundle-registration-not-found", (message, httpStatus, responseError, httpHeaders) => new BundleRegistrationNotFoundException(message, httpStatus, responseError, httpHeaders) },
            { "not-found/code-not-found", (message, httpStatus, responseError, httpHeaders) => new CodeNotFoundException(message, httpStatus, responseError, httpHeaders) },
            { "not-found/file-not-found", (message, httpStatus, responseError, httpHeaders) => new FileNotFoundException(message, httpStatus, responseError, httpHeaders) },
            { "not-found/file-upload-not-found", (message, httpStatus, responseError, httpHeaders) => new FileUploadNotFoundException(message, httpStatus, responseError, httpHeaders) },
            { "not-found/group-not-found", (message, httpStatus, responseError, httpHeaders) => new GroupNotFoundException(message, httpStatus, responseError, httpHeaders) },
            { "not-found/inbox-not-found", (message, httpStatus, responseError, httpHeaders) => new InboxNotFoundException(message, httpStatus, responseError, httpHeaders) },
            { "not-found/nested-not-found", (message, httpStatus, responseError, httpHeaders) => new NestedNotFoundException(message, httpStatus, responseError, httpHeaders) },
            { "not-found/plan-not-found", (message, httpStatus, responseError, httpHeaders) => new PlanNotFoundException(message, httpStatus, responseError, httpHeaders) },
            { "not-found/site-not-found", (message, httpStatus, responseError, httpHeaders) => new SiteNotFoundException(message, httpStatus, responseError, httpHeaders) },
            { "not-found/user-not-found", (message, httpStatus, responseError, httpHeaders) => new UserNotFoundException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/agent-push-update-blocked", (message, httpStatus, responseError, httpHeaders) => new AgentPushUpdateBlockedException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/agent-unavailable", (message, httpStatus, responseError, httpHeaders) => new AgentUnavailableException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/ai-task-cannot-be-run-manually", (message, httpStatus, responseError, httpHeaders) => new AiTaskCannotBeRunManuallyException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/already-completed", (message, httpStatus, responseError, httpHeaders) => new AlreadyCompletedException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/automation-cannot-be-run-manually", (message, httpStatus, responseError, httpHeaders) => new AutomationCannotBeRunManuallyException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/behavior-not-allowed-on-remote-server", (message, httpStatus, responseError, httpHeaders) => new BehaviorNotAllowedOnRemoteServerException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/buffered-upload-disabled-for-this-destination", (message, httpStatus, responseError, httpHeaders) => new BufferedUploadDisabledForThisDestinationException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/bundle-only-allows-previews", (message, httpStatus, responseError, httpHeaders) => new BundleOnlyAllowsPreviewsException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/bundle-operation-requires-subfolder", (message, httpStatus, responseError, httpHeaders) => new BundleOperationRequiresSubfolderException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/configuration-locked-path", (message, httpStatus, responseError, httpHeaders) => new ConfigurationLockedPathException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/could-not-create-parent", (message, httpStatus, responseError, httpHeaders) => new CouldNotCreateParentException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/destination-exists", (message, httpStatus, responseError, httpHeaders) => new DestinationExistsException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/destination-folder-limited", (message, httpStatus, responseError, httpHeaders) => new DestinationFolderLimitedException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/destination-parent-conflict", (message, httpStatus, responseError, httpHeaders) => new DestinationParentConflictException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/destination-parent-does-not-exist", (message, httpStatus, responseError, httpHeaders) => new DestinationParentDoesNotExistException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/exceeded-runtime-limit", (message, httpStatus, responseError, httpHeaders) => new ExceededRuntimeLimitException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/expectation-already-has-open-window", (message, httpStatus, responseError, httpHeaders) => new ExpectationAlreadyHasOpenWindowException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/expectation-not-manual-trigger", (message, httpStatus, responseError, httpHeaders) => new ExpectationNotManualTriggerException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/expired-private-key", (message, httpStatus, responseError, httpHeaders) => new ExpiredPrivateKeyException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/expired-public-key", (message, httpStatus, responseError, httpHeaders) => new ExpiredPublicKeyException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/export-failure", (message, httpStatus, responseError, httpHeaders) => new ExportFailureException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/export-not-ready", (message, httpStatus, responseError, httpHeaders) => new ExportNotReadyException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/failed-to-change-password", (message, httpStatus, responseError, httpHeaders) => new FailedToChangePasswordException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/file-locked", (message, httpStatus, responseError, httpHeaders) => new FileLockedException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/file-not-uploaded", (message, httpStatus, responseError, httpHeaders) => new FileNotUploadedException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/file-pending-processing", (message, httpStatus, responseError, httpHeaders) => new FilePendingProcessingException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/file-processing-error", (message, httpStatus, responseError, httpHeaders) => new FileProcessingErrorException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/file-too-big-to-decrypt", (message, httpStatus, responseError, httpHeaders) => new FileTooBigToDecryptException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/file-too-big-to-encrypt", (message, httpStatus, responseError, httpHeaders) => new FileTooBigToEncryptException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/file-uploaded-to-wrong-region", (message, httpStatus, responseError, httpHeaders) => new FileUploadedToWrongRegionException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/filename-too-long", (message, httpStatus, responseError, httpHeaders) => new FilenameTooLongException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/folder-locked", (message, httpStatus, responseError, httpHeaders) => new FolderLockedException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/folder-not-empty", (message, httpStatus, responseError, httpHeaders) => new FolderNotEmptyException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/history-unavailable", (message, httpStatus, responseError, httpHeaders) => new HistoryUnavailableException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/invalid-bundle-code", (message, httpStatus, responseError, httpHeaders) => new InvalidBundleCodeException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/invalid-file-type", (message, httpStatus, responseError, httpHeaders) => new InvalidFileTypeException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/invalid-filename", (message, httpStatus, responseError, httpHeaders) => new InvalidFilenameException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/invalid-priority-color", (message, httpStatus, responseError, httpHeaders) => new InvalidPriorityColorException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/invalid-range", (message, httpStatus, responseError, httpHeaders) => new InvalidRangeException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/invalid-site", (message, httpStatus, responseError, httpHeaders) => new InvalidSiteException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/invalid-zip-file", (message, httpStatus, responseError, httpHeaders) => new InvalidZipFileException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/metadata-not-supported-on-remotes", (message, httpStatus, responseError, httpHeaders) => new MetadataNotSupportedOnRemotesException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/model-save-error", (message, httpStatus, responseError, httpHeaders) => new ModelSaveErrorException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/multiple-processing-errors", (message, httpStatus, responseError, httpHeaders) => new MultipleProcessingErrorsException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/path-too-long", (message, httpStatus, responseError, httpHeaders) => new PathTooLongException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/recipient-already-shared", (message, httpStatus, responseError, httpHeaders) => new RecipientAlreadySharedException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/remote-entry-read-only", (message, httpStatus, responseError, httpHeaders) => new RemoteEntryReadOnlyException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/remote-server-error", (message, httpStatus, responseError, httpHeaders) => new RemoteServerErrorException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/resource-belongs-to-parent-site", (message, httpStatus, responseError, httpHeaders) => new ResourceBelongsToParentSiteException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/resource-locked", (message, httpStatus, responseError, httpHeaders) => new ResourceLockedException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/subfolder-locked", (message, httpStatus, responseError, httpHeaders) => new SubfolderLockedException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/sync-in-progress", (message, httpStatus, responseError, httpHeaders) => new SyncInProgressException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/two-factor-authentication-code-already-sent", (message, httpStatus, responseError, httpHeaders) => new TwoFactorAuthenticationCodeAlreadySentException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/two-factor-authentication-country-blacklisted", (message, httpStatus, responseError, httpHeaders) => new TwoFactorAuthenticationCountryBlacklistedException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/two-factor-authentication-general-error", (message, httpStatus, responseError, httpHeaders) => new TwoFactorAuthenticationGeneralErrorException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/two-factor-authentication-method-unsupported-error", (message, httpStatus, responseError, httpHeaders) => new TwoFactorAuthenticationMethodUnsupportedErrorException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/two-factor-authentication-unsubscribed-recipient", (message, httpStatus, responseError, httpHeaders) => new TwoFactorAuthenticationUnsubscribedRecipientException(message, httpStatus, responseError, httpHeaders) },
            { "processing-failure/updates-not-allowed-for-remotes", (message, httpStatus, responseError, httpHeaders) => new UpdatesNotAllowedForRemotesException(message, httpStatus, responseError, httpHeaders) },
            { "rate-limited/duplicate-share-recipient", (message, httpStatus, responseError, httpHeaders) => new DuplicateShareRecipientException(message, httpStatus, responseError, httpHeaders) },
            { "rate-limited/reauthentication-rate-limited", (message, httpStatus, responseError, httpHeaders) => new ReauthenticationRateLimitedException(message, httpStatus, responseError, httpHeaders) },
            { "rate-limited/too-many-concurrent-logins", (message, httpStatus, responseError, httpHeaders) => new TooManyConcurrentLoginsException(message, httpStatus, responseError, httpHeaders) },
            { "rate-limited/too-many-concurrent-requests", (message, httpStatus, responseError, httpHeaders) => new TooManyConcurrentRequestsException(message, httpStatus, responseError, httpHeaders) },
            { "rate-limited/too-many-login-attempts", (message, httpStatus, responseError, httpHeaders) => new TooManyLoginAttemptsException(message, httpStatus, responseError, httpHeaders) },
            { "rate-limited/too-many-requests", (message, httpStatus, responseError, httpHeaders) => new TooManyRequestsException(message, httpStatus, responseError, httpHeaders) },
            { "rate-limited/too-many-shares", (message, httpStatus, responseError, httpHeaders) => new TooManySharesException(message, httpStatus, responseError, httpHeaders) },
            { "service-unavailable/automations-unavailable", (message, httpStatus, responseError, httpHeaders) => new AutomationsUnavailableException(message, httpStatus, responseError, httpHeaders) },
            { "service-unavailable/lock-operation-busy", (message, httpStatus, responseError, httpHeaders) => new LockOperationBusyException(message, httpStatus, responseError, httpHeaders) },
            { "service-unavailable/migration-in-progress", (message, httpStatus, responseError, httpHeaders) => new MigrationInProgressException(message, httpStatus, responseError, httpHeaders) },
            { "service-unavailable/search-unavailable", (message, httpStatus, responseError, httpHeaders) => new SearchUnavailableException(message, httpStatus, responseError, httpHeaders) },
            { "service-unavailable/site-disabled", (message, httpStatus, responseError, httpHeaders) => new SiteDisabledException(message, httpStatus, responseError, httpHeaders) },
            { "service-unavailable/uploads-unavailable", (message, httpStatus, responseError, httpHeaders) => new UploadsUnavailableException(message, httpStatus, responseError, httpHeaders) },
            { "site-configuration/account-already-exists", (message, httpStatus, responseError, httpHeaders) => new AccountAlreadyExistsException(message, httpStatus, responseError, httpHeaders) },
            { "site-configuration/account-overdue", (message, httpStatus, responseError, httpHeaders) => new AccountOverdueException(message, httpStatus, responseError, httpHeaders) },
            { "site-configuration/no-account-for-site", (message, httpStatus, responseError, httpHeaders) => new NoAccountForSiteException(message, httpStatus, responseError, httpHeaders) },
            { "site-configuration/site-was-removed", (message, httpStatus, responseError, httpHeaders) => new SiteWasRemovedException(message, httpStatus, responseError, httpHeaders) },
            { "site-configuration/trial-expired", (message, httpStatus, responseError, httpHeaders) => new TrialExpiredException(message, httpStatus, responseError, httpHeaders) },
            { "site-configuration/trial-locked", (message, httpStatus, responseError, httpHeaders) => new TrialLockedException(message, httpStatus, responseError, httpHeaders) },
            { "site-configuration/user-requests-enabled-required", (message, httpStatus, responseError, httpHeaders) => new UserRequestsEnabledRequiredException(message, httpStatus, responseError, httpHeaders) },
        };

        // The exception for an error response: its type's exception, else, for a type such as "not-found/new-type"
        // in a known group, the group's, else ApiException. A missing, blank or unknown type is an ApiException.
        internal static ApiException For(string message, int httpStatus, ResponseError responseError, HttpHeaders httpHeaders)
        {
            string type = responseError.type ?? "";
            int groupLength = type.IndexOf('/');
            Constructor constructor;
            if (ByType.TryGetValue(type, out constructor) || (groupLength > 0 && ByType.TryGetValue(type.Substring(0, groupLength), out constructor)))
            {
                return constructor(message, httpStatus, responseError, httpHeaders);
            }
            return new ApiException(message, httpStatus, responseError, httpHeaders);
        }
    }
}