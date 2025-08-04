namespace Pi.SetMarketData.API.Infrastructure.Exceptions;

/// <summary>
///     Represents errors that occur when the server cannot or will not process the request due to an apparent client
///     error.
/// </summary>
// ReSharper disable UnusedAutoPropertyAccessor.Global
public class BadRequestException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BadRequestException" /> class with a default error message.
    /// </summary>
    public BadRequestException() : base("The request was invalid or cannot be served.")
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BadRequestException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public BadRequestException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BadRequestException" /> class with a specified error message
    ///     and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BadRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BadRequestException" /> class with a specified error message
    ///     and additional error details.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errorDetails">Additional details about the error.</param>
    public BadRequestException(string message, object errorDetails) : base(message)
    {
        ErrorDetails = errorDetails;
    }

    /// <summary>
    ///     Gets or sets additional error details associated with this exception.
    /// </summary>

    public object? ErrorDetails { get; set; }
}