namespace Pi.SetMarketData.Application.Abstractions;

public class RequestResult<T>
{
    public RequestResult(T result)
    {
        Result = result;
    }

    public RequestResult(ValidationErrors validationErrors)
    {
        ValidationErrors = validationErrors;
    }

    public T? Result { set; get; }
    public ValidationErrors ValidationErrors { get; } = new();
    public bool IsValid => !ValidationErrors.Errors.Any();
}