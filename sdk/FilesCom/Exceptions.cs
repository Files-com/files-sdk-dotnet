using FilesCom.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace FilesCom
{
    public class SdkException : Exception
    {
        public string message;

        public SdkException(string message = null)
            : base(message)
        {
            this.message = message;
        }

        public override string ToString()
        {
            return this.message;
        }
    }

    public class ApiConnectionException : SdkException
    {
        public ApiConnectionException(string message = null)
            : base(message)
        { }
    }

    public class AuthenticationException : SdkException
    {
        public AuthenticationException(string message = null)
            : base(message)
        { }
    }

    public class InvalidParameterException : SdkException
    {
        public InvalidParameterException(string message = null)
            : base(message)
        { }
    }

    public class MissingParameterException : SdkException
    {
        public MissingParameterException(string message = null)
            : base(message)
        { }
    }

    public class NotImplementedException : SdkException
    {
        public NotImplementedException(string message = null)
            : base(message)
        { }
    }

    public class InvalidResponseException : SdkException
    {
        public InvalidResponseException(string message = null)
            : base(message)
        { }
    }

    public class ApiException : SdkException
    {
        public int httpStatus;
        public ResponseError responseError;
        public HttpHeaders httpHeaders;

        public ApiException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message)
        {
            this.httpStatus = httpStatus;
            this.responseError = responseError;
            this.httpHeaders = httpHeaders;
        }

        public string detail => this.responseError.detail;
        public string error => this.responseError.error;
        public string[] errors => this.responseError.errors;
        public int httpCode => this.responseError.httpCode;
        public string instance => this.responseError.instance;
        public Dictionary<string, string[]> modelErrors => this.responseError.modelErrors;
        public string title => this.responseError.title;
        public string type => this.responseError.type;
        public Dictionary<string, object> data => this.responseError.data;
    }

    public class BadRequestException : ApiException
    {
        public BadRequestException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class AgentUpgradeRequiredException : BadRequestException
    {
        public AgentUpgradeRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class AttachmentTooLargeException : BadRequestException
    {
        public AttachmentTooLargeException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class CannotDownloadDirectoryException : BadRequestException
    {
        public CannotDownloadDirectoryException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class CantMoveWithMultipleLocationsException : BadRequestException
    {
        public CantMoveWithMultipleLocationsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class DatetimeParseException : BadRequestException
    {
        public DatetimeParseException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class DestinationSameException : BadRequestException
    {
        public DestinationSameException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class DoesNotSupportSortingException : BadRequestException
    {
        public DoesNotSupportSortingException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class FolderMustNotBeAFileException : BadRequestException
    {
        public FolderMustNotBeAFileException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class FoldersNotAllowedException : BadRequestException
    {
        public FoldersNotAllowedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidBodyException : BadRequestException
    {
        public InvalidBodyException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidCursorException : BadRequestException
    {
        public InvalidCursorException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidCursorTypeForSortException : BadRequestException
    {
        public InvalidCursorTypeForSortException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidEtagsException : BadRequestException
    {
        public InvalidEtagsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidFilterAliasCombinationException : BadRequestException
    {
        public InvalidFilterAliasCombinationException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidFilterFieldException : BadRequestException
    {
        public InvalidFilterFieldException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidFilterParamException : BadRequestException
    {
        public InvalidFilterParamException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidFilterParamFormatException : BadRequestException
    {
        public InvalidFilterParamFormatException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidFilterParamValueException : BadRequestException
    {
        public InvalidFilterParamValueException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidInputEncodingException : BadRequestException
    {
        public InvalidInputEncodingException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidInterfaceException : BadRequestException
    {
        public InvalidInterfaceException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidOauthProviderException : BadRequestException
    {
        public InvalidOauthProviderException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidPathException : BadRequestException
    {
        public InvalidPathException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidReturnToUrlException : BadRequestException
    {
        public InvalidReturnToUrlException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidSortFieldException : BadRequestException
    {
        public InvalidSortFieldException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidSortFilterCombinationException : BadRequestException
    {
        public InvalidSortFilterCombinationException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidUploadOffsetException : BadRequestException
    {
        public InvalidUploadOffsetException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidUploadPartGapException : BadRequestException
    {
        public InvalidUploadPartGapException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidUploadPartSizeException : BadRequestException
    {
        public InvalidUploadPartSizeException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class MethodNotAllowedException : BadRequestException
    {
        public MethodNotAllowedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class MultipleSortParamsNotAllowedException : BadRequestException
    {
        public MultipleSortParamsNotAllowedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class NoValidInputParamsException : BadRequestException
    {
        public NoValidInputParamsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class PartNumberTooLargeException : BadRequestException
    {
        public PartNumberTooLargeException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class PathCannotHaveTrailingWhitespaceException : BadRequestException
    {
        public PathCannotHaveTrailingWhitespaceException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class ReauthenticationNeededFieldsException : BadRequestException
    {
        public ReauthenticationNeededFieldsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class RequestParamsContainInvalidCharacterException : BadRequestException
    {
        public RequestParamsContainInvalidCharacterException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class RequestParamsInvalidException : BadRequestException
    {
        public RequestParamsInvalidException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class RequestParamsRequiredException : BadRequestException
    {
        public RequestParamsRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class SearchAllOnChildPathException : BadRequestException
    {
        public SearchAllOnChildPathException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class UnrecognizedSortIndexException : BadRequestException
    {
        public UnrecognizedSortIndexException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class UnsupportedCurrencyException : BadRequestException
    {
        public UnsupportedCurrencyException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class UnsupportedHttpResponseFormatException : BadRequestException
    {
        public UnsupportedHttpResponseFormatException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class UnsupportedMediaTypeException : BadRequestException
    {
        public UnsupportedMediaTypeException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class UserIdInvalidException : BadRequestException
    {
        public UserIdInvalidException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class UserIdOnUserEndpointException : BadRequestException
    {
        public UserIdOnUserEndpointException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class UserRequiredException : BadRequestException
    {
        public UserRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }

    public class NotAuthenticatedException : ApiException
    {
        public NotAuthenticatedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class AdditionalAuthenticationRequiredException : NotAuthenticatedException
    {
        public AdditionalAuthenticationRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class ApiKeySessionsNotSupportedException : NotAuthenticatedException
    {
        public ApiKeySessionsNotSupportedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class AuthenticationRequiredException : NotAuthenticatedException
    {
        public AuthenticationRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class BundleRegistrationCodeFailedException : NotAuthenticatedException
    {
        public BundleRegistrationCodeFailedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class FilesAgentTokenFailedException : NotAuthenticatedException
    {
        public FilesAgentTokenFailedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InboxRegistrationCodeFailedException : NotAuthenticatedException
    {
        public InboxRegistrationCodeFailedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidCredentialsException : NotAuthenticatedException
    {
        public InvalidCredentialsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidOauthException : NotAuthenticatedException
    {
        public InvalidOauthException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidOrExpiredCodeException : NotAuthenticatedException
    {
        public InvalidOrExpiredCodeException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidSessionException : NotAuthenticatedException
    {
        public InvalidSessionException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidUsernameOrPasswordException : NotAuthenticatedException
    {
        public InvalidUsernameOrPasswordException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class LockedOutException : NotAuthenticatedException
    {
        public LockedOutException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class LockoutRegionMismatchException : NotAuthenticatedException
    {
        public LockoutRegionMismatchException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class OneTimePasswordIncorrectException : NotAuthenticatedException
    {
        public OneTimePasswordIncorrectException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class TwoFactorAuthenticationErrorException : NotAuthenticatedException
    {
        public TwoFactorAuthenticationErrorException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class TwoFactorAuthenticationSetupExpiredException : NotAuthenticatedException
    {
        public TwoFactorAuthenticationSetupExpiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }

    public class NotAuthorizedException : ApiException
    {
        public NotAuthorizedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class ApiKeyIsDisabledException : NotAuthorizedException
    {
        public ApiKeyIsDisabledException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class ApiKeyIsPathRestrictedException : NotAuthorizedException
    {
        public ApiKeyIsPathRestrictedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class ApiKeyOnlyForDesktopAppException : NotAuthorizedException
    {
        public ApiKeyOnlyForDesktopAppException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class ApiKeyOnlyForMobileAppException : NotAuthorizedException
    {
        public ApiKeyOnlyForMobileAppException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class ApiKeyOnlyForOfficeIntegrationException : NotAuthorizedException
    {
        public ApiKeyOnlyForOfficeIntegrationException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class BillingOrSiteAdminPermissionRequiredException : NotAuthorizedException
    {
        public BillingOrSiteAdminPermissionRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class BillingPermissionRequiredException : NotAuthorizedException
    {
        public BillingPermissionRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class BundleMaximumUsesReachedException : NotAuthorizedException
    {
        public BundleMaximumUsesReachedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class BundlePermissionRequiredException : NotAuthorizedException
    {
        public BundlePermissionRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class CannotLoginWhileUsingKeyException : NotAuthorizedException
    {
        public CannotLoginWhileUsingKeyException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class CantActForOtherUserException : NotAuthorizedException
    {
        public CantActForOtherUserException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class ContactAdminForPasswordChangeHelpException : NotAuthorizedException
    {
        public ContactAdminForPasswordChangeHelpException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class FilesAgentFailedAuthorizationException : NotAuthorizedException
    {
        public FilesAgentFailedAuthorizationException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class FolderAdminOrBillingPermissionRequiredException : NotAuthorizedException
    {
        public FolderAdminOrBillingPermissionRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class FolderAdminPermissionRequiredException : NotAuthorizedException
    {
        public FolderAdminPermissionRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class FullPermissionRequiredException : NotAuthorizedException
    {
        public FullPermissionRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class HistoryPermissionRequiredException : NotAuthorizedException
    {
        public HistoryPermissionRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InsufficientPermissionForParamsException : NotAuthorizedException
    {
        public InsufficientPermissionForParamsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InsufficientPermissionForSiteException : NotAuthorizedException
    {
        public InsufficientPermissionForSiteException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class MoverAccessDeniedException : NotAuthorizedException
    {
        public MoverAccessDeniedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class MoverPackageRequiredException : NotAuthorizedException
    {
        public MoverPackageRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class MustAuthenticateWithApiKeyException : NotAuthorizedException
    {
        public MustAuthenticateWithApiKeyException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class NeedAdminPermissionForInboxException : NotAuthorizedException
    {
        public NeedAdminPermissionForInboxException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class NonAdminsMustQueryByFolderOrPathException : NotAuthorizedException
    {
        public NonAdminsMustQueryByFolderOrPathException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class NotAllowedToCreateBundleException : NotAuthorizedException
    {
        public NotAllowedToCreateBundleException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class NotEnqueuableSyncException : NotAuthorizedException
    {
        public NotEnqueuableSyncException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class PasswordChangeNotRequiredException : NotAuthorizedException
    {
        public PasswordChangeNotRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class PasswordChangeRequiredException : NotAuthorizedException
    {
        public PasswordChangeRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class PaymentMethodErrorException : NotAuthorizedException
    {
        public PaymentMethodErrorException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class ReadOnlySessionException : NotAuthorizedException
    {
        public ReadOnlySessionException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class ReadPermissionRequiredException : NotAuthorizedException
    {
        public ReadPermissionRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class ReauthenticationFailedException : NotAuthorizedException
    {
        public ReauthenticationFailedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class ReauthenticationFailedFinalException : NotAuthorizedException
    {
        public ReauthenticationFailedFinalException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class ReauthenticationNeededActionException : NotAuthorizedException
    {
        public ReauthenticationNeededActionException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class RecaptchaFailedException : NotAuthorizedException
    {
        public RecaptchaFailedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class SelfManagedRequiredException : NotAuthorizedException
    {
        public SelfManagedRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class SiteAdminRequiredException : NotAuthorizedException
    {
        public SiteAdminRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class SiteFilesAreImmutableException : NotAuthorizedException
    {
        public SiteFilesAreImmutableException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class TwoFactorAuthenticationRequiredException : NotAuthorizedException
    {
        public TwoFactorAuthenticationRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class UserIdWithoutSiteAdminException : NotAuthorizedException
    {
        public UserIdWithoutSiteAdminException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class WriteAndBundlePermissionRequiredException : NotAuthorizedException
    {
        public WriteAndBundlePermissionRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class WritePermissionRequiredException : NotAuthorizedException
    {
        public WritePermissionRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }

    public class NotFoundException : ApiException
    {
        public NotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class ApiKeyNotFoundException : NotFoundException
    {
        public ApiKeyNotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class BundlePathNotFoundException : NotFoundException
    {
        public BundlePathNotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class BundleRegistrationNotFoundException : NotFoundException
    {
        public BundleRegistrationNotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class CodeNotFoundException : NotFoundException
    {
        public CodeNotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class FileNotFoundException : NotFoundException
    {
        public FileNotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class FileUploadNotFoundException : NotFoundException
    {
        public FileUploadNotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class GroupNotFoundException : NotFoundException
    {
        public GroupNotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InboxNotFoundException : NotFoundException
    {
        public InboxNotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class NestedNotFoundException : NotFoundException
    {
        public NestedNotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class PlanNotFoundException : NotFoundException
    {
        public PlanNotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class SiteNotFoundException : NotFoundException
    {
        public SiteNotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }

    public class ProcessingFailureException : ApiException
    {
        public ProcessingFailureException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class AgentUnavailableException : ProcessingFailureException
    {
        public AgentUnavailableException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class AlreadyCompletedException : ProcessingFailureException
    {
        public AlreadyCompletedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class AutomationCannotBeRunManuallyException : ProcessingFailureException
    {
        public AutomationCannotBeRunManuallyException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class BehaviorNotAllowedOnRemoteServerException : ProcessingFailureException
    {
        public BehaviorNotAllowedOnRemoteServerException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class BufferedUploadDisabledForThisDestinationException : ProcessingFailureException
    {
        public BufferedUploadDisabledForThisDestinationException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class BundleOnlyAllowsPreviewsException : ProcessingFailureException
    {
        public BundleOnlyAllowsPreviewsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class BundleOperationRequiresSubfolderException : ProcessingFailureException
    {
        public BundleOperationRequiresSubfolderException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class ConfigurationLockedPathException : ProcessingFailureException
    {
        public ConfigurationLockedPathException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class CouldNotCreateParentException : ProcessingFailureException
    {
        public CouldNotCreateParentException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class DestinationExistsException : ProcessingFailureException
    {
        public DestinationExistsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class DestinationFolderLimitedException : ProcessingFailureException
    {
        public DestinationFolderLimitedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class DestinationParentConflictException : ProcessingFailureException
    {
        public DestinationParentConflictException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class DestinationParentDoesNotExistException : ProcessingFailureException
    {
        public DestinationParentDoesNotExistException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class ExceededRuntimeLimitException : ProcessingFailureException
    {
        public ExceededRuntimeLimitException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class ExpiredPrivateKeyException : ProcessingFailureException
    {
        public ExpiredPrivateKeyException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class ExpiredPublicKeyException : ProcessingFailureException
    {
        public ExpiredPublicKeyException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class ExportFailureException : ProcessingFailureException
    {
        public ExportFailureException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class ExportNotReadyException : ProcessingFailureException
    {
        public ExportNotReadyException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class FailedToChangePasswordException : ProcessingFailureException
    {
        public FailedToChangePasswordException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class FileLockedException : ProcessingFailureException
    {
        public FileLockedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class FileNotUploadedException : ProcessingFailureException
    {
        public FileNotUploadedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class FilePendingProcessingException : ProcessingFailureException
    {
        public FilePendingProcessingException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class FileProcessingErrorException : ProcessingFailureException
    {
        public FileProcessingErrorException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class FileTooBigToDecryptException : ProcessingFailureException
    {
        public FileTooBigToDecryptException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class FileTooBigToEncryptException : ProcessingFailureException
    {
        public FileTooBigToEncryptException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class FileUploadedToWrongRegionException : ProcessingFailureException
    {
        public FileUploadedToWrongRegionException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class FilenameTooLongException : ProcessingFailureException
    {
        public FilenameTooLongException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class FolderLockedException : ProcessingFailureException
    {
        public FolderLockedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class FolderNotEmptyException : ProcessingFailureException
    {
        public FolderNotEmptyException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class HistoryUnavailableException : ProcessingFailureException
    {
        public HistoryUnavailableException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidBundleCodeException : ProcessingFailureException
    {
        public InvalidBundleCodeException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidFileTypeException : ProcessingFailureException
    {
        public InvalidFileTypeException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidFilenameException : ProcessingFailureException
    {
        public InvalidFilenameException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidPriorityColorException : ProcessingFailureException
    {
        public InvalidPriorityColorException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidRangeException : ProcessingFailureException
    {
        public InvalidRangeException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class InvalidSiteException : ProcessingFailureException
    {
        public InvalidSiteException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class MetadataNotSupportedOnRemotesException : ProcessingFailureException
    {
        public MetadataNotSupportedOnRemotesException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class ModelSaveErrorException : ProcessingFailureException
    {
        public ModelSaveErrorException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class MultipleProcessingErrorsException : ProcessingFailureException
    {
        public MultipleProcessingErrorsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class PathTooLongException : ProcessingFailureException
    {
        public PathTooLongException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class RecipientAlreadySharedException : ProcessingFailureException
    {
        public RecipientAlreadySharedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class RemoteServerErrorException : ProcessingFailureException
    {
        public RemoteServerErrorException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class ResourceBelongsToParentSiteException : ProcessingFailureException
    {
        public ResourceBelongsToParentSiteException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class ResourceLockedException : ProcessingFailureException
    {
        public ResourceLockedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class SubfolderLockedException : ProcessingFailureException
    {
        public SubfolderLockedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class SyncInProgressException : ProcessingFailureException
    {
        public SyncInProgressException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class TwoFactorAuthenticationCodeAlreadySentException : ProcessingFailureException
    {
        public TwoFactorAuthenticationCodeAlreadySentException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class TwoFactorAuthenticationCountryBlacklistedException : ProcessingFailureException
    {
        public TwoFactorAuthenticationCountryBlacklistedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class TwoFactorAuthenticationGeneralErrorException : ProcessingFailureException
    {
        public TwoFactorAuthenticationGeneralErrorException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class TwoFactorAuthenticationMethodUnsupportedErrorException : ProcessingFailureException
    {
        public TwoFactorAuthenticationMethodUnsupportedErrorException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class TwoFactorAuthenticationUnsubscribedRecipientException : ProcessingFailureException
    {
        public TwoFactorAuthenticationUnsubscribedRecipientException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class UpdatesNotAllowedForRemotesException : ProcessingFailureException
    {
        public UpdatesNotAllowedForRemotesException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }

    public class RateLimitedException : ApiException
    {
        public RateLimitedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class DuplicateShareRecipientException : RateLimitedException
    {
        public DuplicateShareRecipientException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class ReauthenticationRateLimitedException : RateLimitedException
    {
        public ReauthenticationRateLimitedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class TooManyConcurrentLoginsException : RateLimitedException
    {
        public TooManyConcurrentLoginsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class TooManyConcurrentRequestsException : RateLimitedException
    {
        public TooManyConcurrentRequestsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class TooManyLoginAttemptsException : RateLimitedException
    {
        public TooManyLoginAttemptsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class TooManyRequestsException : RateLimitedException
    {
        public TooManyRequestsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class TooManySharesException : RateLimitedException
    {
        public TooManySharesException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }

    public class ServiceUnavailableException : ApiException
    {
        public ServiceUnavailableException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class AutomationsUnavailableException : ServiceUnavailableException
    {
        public AutomationsUnavailableException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class MigrationInProgressException : ServiceUnavailableException
    {
        public MigrationInProgressException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class SiteDisabledException : ServiceUnavailableException
    {
        public SiteDisabledException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class UploadsUnavailableException : ServiceUnavailableException
    {
        public UploadsUnavailableException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }

    public class SiteConfigurationException : ApiException
    {
        public SiteConfigurationException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class AccountAlreadyExistsException : SiteConfigurationException
    {
        public AccountAlreadyExistsException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class AccountOverdueException : SiteConfigurationException
    {
        public AccountOverdueException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class NoAccountForSiteException : SiteConfigurationException
    {
        public NoAccountForSiteException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class SiteWasRemovedException : SiteConfigurationException
    {
        public SiteWasRemovedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class TrialExpiredException : SiteConfigurationException
    {
        public TrialExpiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class TrialLockedException : SiteConfigurationException
    {
        public TrialLockedException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
    public class UserRequestsEnabledRequiredException : SiteConfigurationException
    {
        public UserRequestsEnabledRequiredException(string message = null, int httpStatus = 0, ResponseError responseError = null, HttpHeaders httpHeaders = null)
            : base(message, httpStatus, responseError, httpHeaders)
        { }
    }
}