using Pi.Common.CommonModels;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Application.Models.Dto;

public class OrderTransactionDto
{
    public decimal Commission { get; init; }
    public decimal CommissionUSD { get; init; }

    public decimal TotalCost { get; init; }
    public decimal TotalCostUSD { get; init; }

    public decimal Cost { get; init; }
    public decimal CostUSD { get; init; }

    public OrderTransactionDto(OrderTransaction oTrn)
    {
        Cost = Math.Abs(oTrn.GetTradeCost());
        CostUSD = Math.Abs(oTrn.GetTradeCost(Currency.USD));
        TotalCost = Math.Abs(oTrn.GetTotalCost());
        TotalCostUSD = Math.Abs(oTrn.GetTotalCost(Currency.USD));
        Commission = Math.Abs(oTrn.GetCommission());
        CommissionUSD = Math.Abs(oTrn.GetCommission(Currency.USD));
    }
}
