namespace Pi.BackofficeService.Domain.Exceptions;

[Serializable]
public class TicketExecuteException : Exception
{
    public TicketExecuteException()
    {
    }

    public TicketExecuteException(string message)
        : base(message)
    {
    }

    public TicketExecuteException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
