namespace Pi.Financial.FundService.Application.Exceptions;

public class RecordNotFoundException : Exception
{
    public string Msg => base.Message;

    public RecordNotFoundException()
    {
    }

    public RecordNotFoundException(string? msg) : base(msg)
    {
    }
}
