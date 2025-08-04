namespace Pi.SetMarketData.Application.Abstractions;

public record ValidationErrors(IReadOnlyCollection<string> Errors)
{
    public ValidationErrors() : this(new string[] { })
    {
    }
}