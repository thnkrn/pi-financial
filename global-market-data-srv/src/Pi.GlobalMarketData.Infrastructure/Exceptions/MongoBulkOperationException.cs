namespace Pi.GlobalMarketData.Infrastructure.Exceptions;

public class MongoBulkOperationException : Exception
{
    public IEnumerable<(int Index, string Error)> FailedOperations { get; }

    public MongoBulkOperationException(string message, IEnumerable<(int Index, string Error)> failedOperations)
        : base(message)
    {
        FailedOperations = failedOperations;
    }
}