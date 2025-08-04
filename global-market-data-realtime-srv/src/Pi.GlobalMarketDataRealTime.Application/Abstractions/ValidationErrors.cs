namespace Pi.GlobalMarketDataRealTime.Application.Abstractions;

public record ValidationErrors(IReadOnlyCollection<string> Errors)
{
    public ValidationErrors() : this(Array.Empty<string>())
    {
    }
}