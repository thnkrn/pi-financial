using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.OpenApi.Extensions;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Services.SetTrade;

namespace Pi.TfexService.API.Models.Order;

public class PaginatedSetTradeOrderDto(List<SetTradeOrderDto> data, int page, int pageSize, bool hasNextPage, int total)
{
    public List<SetTradeOrderDto> Orders { get; set; } = data;
    public int Page { get; set; } = page;
    public int PageSize { get; set; } = pageSize;
    public int Total { get; set; } = total;
    public bool HasNextPage { get; set; } = hasNextPage;
}

public class ActiveOrderDto(List<SetTradeOrderDto> data, int total)
{
    public List<SetTradeOrderDto> Orders { get; set; } = data;
    public int Total { get; set; } = total;
}

public class SetTradeOrderDto(SetTradeOrder setTradeOrder)
{
    public long OrderNo { get; set; } = setTradeOrder.OrderNo;
    public string? TfxOrderNo { get; set; } = setTradeOrder.TfxOrderNo;
    public string? AccountNo { get; set; } = setTradeOrder.AccountNo;
    public string? EntryTime { get; set; } = setTradeOrder.EntryTime?.ToUniversalTime()
        .ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
    public string? CancelTime { get; set; } = setTradeOrder.CancelTime?.ToUniversalTime().ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
    public string? Symbol { get; set; } = setTradeOrder.Symbol;
    public string? Side { get; set; } = setTradeOrder.Side?.ToString();
    public string? Position { get; set; } = setTradeOrder.Position?.ToString();
    public string? PriceType { get; set; } = setTradeOrder.PriceType?.GetDisplayName();
    public Decimal Price { get; set; } = setTradeOrder.Price;
    public Decimal MatchedPrice { get; set; } = setTradeOrder.MatchedPrice;
    public int Qty { get; set; } = setTradeOrder.Qty;
    public int MatchQty { get; set; } = setTradeOrder.MatchQty;
    public int CancelQty { get; set; } = setTradeOrder.CancelQty;
    public int NotMatchQty { get; set; } = setTradeOrder.Qty - setTradeOrder.MatchQty - setTradeOrder.CancelQty;
    public string? Status { get; set; } = setTradeOrder.Status;
    public string? ShowStatus { get; set; } = MapShowStatus(setTradeOrder.ShowStatus);
    public string? StatusMeaning { get; set; } = setTradeOrder.StatusMeaning;
    public bool CanCancel { get; set; } = setTradeOrder.CanCancel;
    public bool CanChange { get; set; } = setTradeOrder.CanChange;
    public int? PriceDigit { get; set; } = setTradeOrder.PriceDigit;
    public string? OrderTime { get; set; } = setTradeOrder.OrderTime?.ToUniversalTime().ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
    public string? Logo { get; set; } = setTradeOrder.Logo;
    public string? InstrumentCategory { get; set; } = setTradeOrder.InstrumentCategory;
    public decimal? TickSize { get; set; } = setTradeOrder.TickSize;
    public decimal? LotSize { get; set; } = setTradeOrder.LotSize;
    public decimal? Multiplier { get; set; } = setTradeOrder.Multiplier;
    public MultiplierType? MultiplierType { get; set; } = setTradeOrder.MultiplierType;
    public MultiplierUnit? MultiplierUnit { get; set; } = setTradeOrder.MultiplierUnit;

    private static string MapShowStatus(string? setTradeShowStatus)
    {
        // Remove any curl "()" and replace "P-" with "Partial"
        return setTradeShowStatus == null ? string.Empty :
            Regex.Replace(setTradeShowStatus, @"\s*\(.*?\)", string.Empty).Replace("P-", "Partial ");
    }
}
