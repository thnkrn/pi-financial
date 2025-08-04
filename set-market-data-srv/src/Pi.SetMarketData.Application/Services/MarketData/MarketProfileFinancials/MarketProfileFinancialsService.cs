using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Utils;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;

namespace Pi.SetMarketData.Application.Services.MarketData.MarketProfileFinancials;

public static class MarketProfileFinancialsService
{
    public static MarketProfileFinancialsResponse GetResult(
        MorningStarStocks? morningStarStocks,
        int limited
    )
    {
        var morningStarStock = morningStarStocks ?? new MorningStarStocks();

        var _Sales = ToListItem(morningStarStock.Sales.Values, limited);
        var _OperatingIncome = ToListItem(morningStarStock.OperatingIncomes.Values, limited);
        var _NetIncome = ToListItem(morningStarStock.NetIncomes.Values, limited);
        var _EarningsPerShare = ToListItem(morningStarStock.EarningsPerShares.Values, limited);
        var _DividendPerShare = ToListItem(morningStarStock.DividendPerShares.Values, limited);
        double[] _CashflowPerShare = [morningStarStock.CashflowPerShare];

        var _TotalAssets = ToListItem(morningStarStock.TotalAssets.Values, limited);
        var _TotalLiabilities = ToListItem(morningStarStock.TotalLiabilities.Values, limited);
        var _OperatingMargin = ToListItem(
            morningStarStock.OperatingMargin.Values.Select(value => value * 100),
            limited
        );
        var _LiabilitiesToAssets = ToListItem(
            morningStarStock.LiabilitiesToAssets.Values.Select(value => value * 100),
            limited
        );
        var _AverageShareCount = ToListItem(morningStarStock.AverageShareCount.Values, limited);

        return new MarketProfileFinancialsResponse
        {
            Code = "0",
            Message = string.Empty,
            Response = new ProfileFinancialsResponse
            {
                Sales = new Sales
                {
                    Yy = DataManipulation.ToYy(_Sales.FirstOrDefault(), _Sales.LastOrDefault()),
                    List = DataManipulation.ToListString(_Sales),
                    StatementType = morningStarStock.Sales.StatementType
                },
                OperatingIncome = new OperatingIncome
                {
                    Yy = DataManipulation.ToYy(
                        _OperatingIncome.FirstOrDefault(),
                        _OperatingIncome.LastOrDefault()
                    ),
                    List = DataManipulation.ToListString(_OperatingIncome),
                    StatementType = morningStarStock.OperatingIncomes.StatementType
                },
                NetIncome = new NetIncome
                {
                    Yy = DataManipulation.ToYy(
                        _NetIncome.FirstOrDefault(),
                        _NetIncome.LastOrDefault()
                    ),
                    List = DataManipulation.ToListString(_NetIncome),
                    StatementType = morningStarStock.NetIncomes.StatementType
                },
                EarningsPerShare = new EarningsPerShare
                {
                    Yy = DataManipulation.ToYy(
                        _EarningsPerShare.FirstOrDefault(),
                        _EarningsPerShare.LastOrDefault()
                    ),
                    List = DataManipulation.ToListString(_EarningsPerShare),
                    StatementType = morningStarStock.EarningsPerShares.StatementType
                },
                DividendPerShare = new DividendPerShare
                {
                    Yy = DataManipulation.ToYy(
                        _DividendPerShare.FirstOrDefault(),
                        _DividendPerShare.LastOrDefault()
                    ),
                    List = DataManipulation.ToListString(_DividendPerShare),
                    StatementType = morningStarStock.DividendPerShares.StatementType
                },
                CashflowPerShare = new CashflowPerShare
                {
                    Yy = DataManipulation.ToYy(
                        _CashflowPerShare.FirstOrDefault(),
                        _CashflowPerShare.LastOrDefault()
                    ),
                    List = DataManipulation.ToListString(_CashflowPerShare),
                    StatementType = ""
                },
                TotalAssets = new TotalAssets
                {
                    Yy = DataManipulation.ToYy(
                        _TotalAssets.FirstOrDefault(),
                        _TotalAssets.LastOrDefault()
                    ),
                    List = DataManipulation.ToListString(_TotalAssets),
                    StatementType = morningStarStock.TotalAssets.StatementType
                },
                TotalLiabilities = new TotalLiabilities
                {
                    Yy = DataManipulation.ToYy(
                        _TotalLiabilities.FirstOrDefault(),
                        _TotalLiabilities.LastOrDefault()
                    ),
                    List = DataManipulation.ToListString(_TotalLiabilities),
                    StatementType = morningStarStock.TotalLiabilities.StatementType
                },
                OperatingMargin = new OperatingMargin
                {
                    Yy = (
                        _OperatingMargin.LastOrDefault() - _OperatingMargin.FirstOrDefault()
                    ).ToString(DataFormat.TwoDigitFormat),
                    List = DataManipulation.ToListString(_OperatingMargin),
                    StatementType = morningStarStock.OperatingMargin.StatementType
                },
                LiabilitiesToAssets = new LiabilitiesToAssets
                {
                    Yy = DataManipulation.ToYy(
                        _LiabilitiesToAssets.FirstOrDefault(),
                        _LiabilitiesToAssets.LastOrDefault()
                    ),
                    List = DataManipulation.ToListString(_LiabilitiesToAssets),
                    StatementType = morningStarStock.LiabilitiesToAssets.StatementType
                },
                AverageShareCount = new AverageShareCount
                {
                    Yy = DataManipulation.ToYy(
                        _AverageShareCount.FirstOrDefault(),
                        _AverageShareCount.LastOrDefault()
                    ),
                    List = DataManipulation.ToListString(_AverageShareCount),
                    StatementType = morningStarStock.AverageShareCount.StatementType
                },
                Units = morningStarStock.Units,
                LatestFinancials = DataManipulation.DateToString(morningStarStock.LatestFinancials),
                Source = string.Empty
            }
        };
    }

    private static double[] ToListItem(IEnumerable<double>? list, int limited)
    {
        if (list == null || !list.Any())
            return [];

        return list.Reverse().TakeLast(limited).ToArray();
    }
}
