namespace Pi.BackofficeService.Application.Exceptions;

public class ExternalApiException : Exception
{
    public ExternalApiException(string detail, string title, int status)
    {
        Detail = detail;
        Title = title;
        Status = status;
    }

    public string Title { get; set; }
    public string Detail { get; set; }
    public int Status { get; set; }
}
