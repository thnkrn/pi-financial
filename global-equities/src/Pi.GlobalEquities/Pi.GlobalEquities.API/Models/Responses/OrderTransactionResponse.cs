using Pi.Common.CommonModels;
using Pi.GlobalEquities.Application.Models.Dto;

namespace Pi.GlobalEquities.API.Models.Responses;

public class OrderTransactionResponse
{
    public decimal Commission { get; init; }
    public decimal CommissionUSD { get; init; }

    public decimal TotalCost { get; init; }
    public decimal TotalCostUSD { get; init; }

    public decimal Cost { get; init; }
    public decimal CostUSD { get; init; }
    public OrderTransactionResponse() { }

    public OrderTransactionResponse(OrderTransaction oTrn)
    {
        if (oTrn is not null)
        {
            Cost = Math.Abs(oTrn.GetTradeCost());
            CostUSD = Math.Abs(oTrn.GetTradeCost(Currency.USD));
            TotalCost = Math.Abs(oTrn.GetTotalCost());
            TotalCostUSD = Math.Abs(oTrn.GetTotalCost(Currency.USD));
            Commission = Math.Abs(oTrn.GetCommission());
            CommissionUSD = Math.Abs(oTrn.GetCommission(Currency.USD));
        }
    }

    public OrderTransactionResponse(OrderTransactionDto oTrn)
    {
        Cost = oTrn.Cost;
        CostUSD = oTrn.CostUSD;
        TotalCost = oTrn.TotalCost;
        TotalCostUSD = oTrn.TotalCostUSD;
        Commission = oTrn.Commission;
        CommissionUSD = oTrn.CommissionUSD;
    }
}
