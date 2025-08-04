namespace Pi.Financial.FundService.Application.Exceptions;

public class InvalidDataException : Exception
{
    public string Msg => base.Message;

    public InvalidDataException(string? msg) : base(msg)
    {
    }
}
