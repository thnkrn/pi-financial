namespace Pi.Financial.FundService.Application.Exceptions;

public class SyncException : Exception
{
    public string Msg => base.Message;

    public SyncException()
    {
    }

    public SyncException(string? msg) : base(msg)
    {
    }
}
