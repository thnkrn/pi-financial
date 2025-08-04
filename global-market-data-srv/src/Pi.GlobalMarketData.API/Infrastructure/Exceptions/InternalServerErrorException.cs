namespace Pi.GlobalMarketData.API.Infrastructure.Exceptions;

/// <summary>
///     Represents errors that occur during application execution that are not handled by other specific exception types.
/// </summary>
// ReSharper disable UnusedAutoPropertyAccessor.Global
public class InternalServerErrorException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="InternalServerErrorException" /> class with a default error message.
    /// </summary>
    public InternalServerErrorException() : base("An internal server error occurred.")
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="InternalServerErrorException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public InternalServerErrorException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="InternalServerErrorException" /> class with a specified error message
    ///     and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public InternalServerErrorException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="InternalServerErrorException" /> class with a specified error message
    ///     and additional error details.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errorDetails">Additional details about the error.</param>
    public InternalServerErrorException(string message, object errorDetails) : base(message)
    {
        ErrorDetails = errorDetails;
    }

    /// <summary>
    ///     Gets or sets additional error details associated with this exception.
    /// </summary>

    public object? ErrorDetails { get; set; }
}