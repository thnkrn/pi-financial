using System.ComponentModel.DataAnnotations;

namespace Pi.GlobalEquities.Models;

public class AggregateValidationException : Exception
{
    public IEnumerable<ValidationResult> Errors { get; }

    public AggregateValidationException(IEnumerable<ValidationResult> errors, string msg = null, Exception innerException = null)
        : base(msg ?? "Validation Errors", innerException)
    {
        Errors = errors ?? Enumerable.Empty<ValidationResult>();
    }
}
