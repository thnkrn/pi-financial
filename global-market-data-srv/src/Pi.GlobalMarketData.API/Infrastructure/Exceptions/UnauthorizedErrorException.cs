namespace Pi.GlobalMarketData.API.Infrastructure.Exceptions;

/// <summary>
///     Exception thrown when access to a resource is denied due to insufficient permissions.
/// </summary>
// ReSharper disable UnusedAutoPropertyAccessor.Global
public class UnauthorizedErrorException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UnauthorizedErrorException" /> class with a default error message.
    /// </summary>
    public UnauthorizedErrorException() : base("Access is denied due to insufficient permissions.")
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnauthorizedErrorException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public UnauthorizedErrorException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnauthorizedErrorException" /> class with a specified error message
    ///     and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public UnauthorizedErrorException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnauthorizedErrorException" /> class with a specified error message
    ///     and additional error details.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errorDetails">Additional details about the error.</param>
    public UnauthorizedErrorException(string message, object errorDetails) : base(message)
    {
        ErrorDetails = errorDetails;
    }

    /// <summary>
    ///     Gets or sets additional error details associated with this exception.
    /// </summary>
    public object? ErrorDetails { get; set; }
}