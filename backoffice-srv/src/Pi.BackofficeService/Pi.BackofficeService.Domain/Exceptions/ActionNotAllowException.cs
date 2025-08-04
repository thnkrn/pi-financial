namespace Pi.BackofficeService.Domain.Exceptions;

[Serializable]
public class ActionNotAllowException : Exception
{
    public ActionNotAllowException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public ActionNotAllowException()
    {
    }

    public ActionNotAllowException(string message)
        : base(message)
    {
    }
}
