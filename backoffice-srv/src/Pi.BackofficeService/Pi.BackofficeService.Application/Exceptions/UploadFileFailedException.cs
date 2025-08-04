namespace Pi.BackofficeService.Application.Exceptions;

public class UploadFileFailedException : Exception
{
    public UploadFileFailedException(string fileName) : base($"File {fileName} upload failed")
    {
    }
}
