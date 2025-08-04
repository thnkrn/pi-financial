namespace Pi.GlobalMarketData.API.Infrastructure.Exceptions;

/// <summary>
///     Exception thrown when a requested resource is not found.
/// </summary>
// ReSharper disable UnusedAutoPropertyAccessor.Global
public class NotFoundException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NotFoundException" /> class with a default error message.
    /// </summary>
    public NotFoundException() : base("The requested resource was not found.")
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NotFoundException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public NotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NotFoundException" /> class with a specified error message and the
    ///     name of the resource that was not found.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="resourceName">The name of the resource that was not found.</param>
    public NotFoundException(string message, string resourceName) : base(message)
    {
        ResourceName = resourceName;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NotFoundException" /> class with a specified error message and a
    ///     reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NotFoundException" /> class with a specified error message, the name
    ///     of the resource that was not found, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="resourceName">The name of the resource that was not found.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public NotFoundException(string message, string resourceName, Exception innerException) : base(message,
        innerException)
    {
        ResourceName = resourceName;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NotFoundException" /> class with a specified error message,
    ///     the name of the resource that was not found, and additional error details.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="resourceName">The name of the resource that was not found.</param>
    /// <param name="errorDetails">Additional details about the error.</param>
    public NotFoundException(string message, string resourceName, object errorDetails) : base(message)
    {
        ResourceName = resourceName;
        ErrorDetails = errorDetails;
    }

    /// <summary>
    ///     Gets the name of the resource that was not found.
    /// </summary>
    public string? ResourceName { get; }

    /// <summary>
    ///     Gets or sets additional error details associated with this exception.
    /// </summary>
    public object? ErrorDetails { get; set; }
}