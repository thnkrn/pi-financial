namespace Pi.TfexService.Application.Models;

public class GetTradeDetailRequestModel(
    string accountNo,
    int page,
    int pageSize,
    DateOnly dateFrom,
    DateOnly dateTo,
    Side? side = null,
    Position? position = null)
{
    public string AccountNo { get; set; } = accountNo;
    public int Page { get; set; } = page;
    public int PageSize { get; set; } = pageSize;
    public DateOnly DateFrom { get; set; } = dateFrom;
    public DateOnly DateTo { get; set; } = dateTo;
    public Side? Side { get; set; } = side;
    public Position? Position { get; set; } = position;
}