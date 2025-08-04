namespace Pi.BackofficeService.Domain.Exceptions;

[Serializable]
public class NotFoundException : Exception
{
    public NotFoundException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public NotFoundException()
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }
}
