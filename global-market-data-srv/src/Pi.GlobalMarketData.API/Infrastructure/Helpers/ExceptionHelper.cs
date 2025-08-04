using Pi.GlobalMarketData.API.Infrastructure.Exceptions;

namespace Pi.GlobalMarketData.API.Infrastructure.Helpers;

public static class ExceptionHelper
{
    private const string DefaultNotFoundMessage = "The requested resource was not found.";
    private const string DefaultBadRequestMessage = "The request was invalid or cannot be served.";
    private const string DefaultInternalServerErrorMessage = "An internal server error occurred.";
    private const string DefaultUnauthorizedErrorMessage = "Access is denied due to insufficient permissions.";

    public static T GetDefaultResponse<T>() where T : class, new()
    {
        try
        {
            return ObjectInitializer.InitializeObject<T>();
        }
        catch (Exception ex)
        {
            throw InternalServerErrorException(DefaultInternalServerErrorMessage, ex);
        }
    }


    public static NotFoundException NotFoundException(string? message = null, string? resourceName = null,
        Exception? innerException = null, object? errorDetails = null)
    {
        message ??= DefaultNotFoundMessage;
        innerException ??= new Exception("Resource not found");

        return !string.IsNullOrEmpty(resourceName)
            ? new NotFoundException(message, resourceName, innerException) { ErrorDetails = errorDetails }
            : new NotFoundException(message, innerException) { ErrorDetails = errorDetails };
    }

    public static BadRequestException BadRequestException(string? message = null,
        Exception? innerException = null,
        object? errorDetails = null)
    {
        message ??= DefaultBadRequestMessage;
        innerException ??= new Exception(DefaultBadRequestMessage);

        return new BadRequestException(message, innerException) { ErrorDetails = errorDetails };
    }

    public static InternalServerErrorException InternalServerErrorException(string? message = null,
        Exception? innerException = null, object? errorDetails = null)
    {
        message ??= DefaultInternalServerErrorMessage;
        innerException ??= new Exception(DefaultInternalServerErrorMessage);

        return new InternalServerErrorException(message, innerException) { ErrorDetails = errorDetails };
    }

    public static UnauthorizedErrorException UnauthorizedErrorException(string? message = null,
        Exception? innerException = null, object? errorDetails = null)
    {
        message ??= DefaultUnauthorizedErrorMessage;
        innerException ??= new Exception(DefaultUnauthorizedErrorMessage);

        return new UnauthorizedErrorException(message, innerException) { ErrorDetails = errorDetails };
    }
}