using Pi.Client.ItBackOffice.Model;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Services.SetTrade;
using Pi.TfexService.Application.Utils;

namespace Pi.TfexService.API.Models.Order;

public class PaginatedItOrderTradeDto(List<ItOrderTradeDto> data, int page, int pageSize, bool hasNextPage, int total)
{
    public List<ItOrderTradeDto> Trades { get; set; } = data;
    public int Page { get; set; } = page;
    public int PageSize { get; set; } = pageSize;
    public int Total { get; set; } = total;
    public bool HasNextPage { get; set; } = hasNextPage;
}

public class ItOrderTradeDto(TradeDetail tradeDetail)
{
    public string Series { get; set; } = tradeDetail.ShareCode;
    public Side Side { get; set; } = ItOrderUtils.MappingSide(tradeDetail.RefType);
    public Position Position { get; set; } = ItOrderUtils.MappingPosition(tradeDetail.Buysellsorter);
    public Currency Currency { get; set; } = Currency.THB;
    public double AveragePrice { get; set; } = tradeDetail.Price;
    public double Unit { get; set; } = tradeDetail.Unit;
    public double Amount { get; set; } = tradeDetail.Amt;
    public double CommissionAndVat { get; set; } = tradeDetail.CommSub + tradeDetail.VatSub;
    public double? TotalAmount { get; set; } = ItOrderUtils.CalculateTotalAmount(tradeDetail.RefType, tradeDetail.Amt, tradeDetail.CommSub, tradeDetail.VatSub);
    public DateTime? TradeDateTime { get; set; } = ItOrderUtils.CalculateDateTime(tradeDetail.RefDate, tradeDetail.ConfirmTime);
}
