namespace Pi.BackofficeService.Application.Exceptions;

public class FileSizeTooLargeException : Exception
{
    public FileSizeTooLargeException(int maxSize) : base($"File size should below or equal {maxSize}")
    {
    }
}
