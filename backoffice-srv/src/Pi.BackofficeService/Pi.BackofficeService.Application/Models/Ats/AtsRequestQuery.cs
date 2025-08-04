namespace Pi.BackofficeService.Application.Models.Ats;

public class AtsRequestQuery
{
    public string? AtsUploadType { get; set; }
    public DateOnly? RequestDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}