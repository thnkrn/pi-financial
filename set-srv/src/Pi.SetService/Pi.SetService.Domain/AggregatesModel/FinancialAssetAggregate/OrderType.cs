using System.ComponentModel;

namespace Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

// NOTE: they have more several type than this, please check we require all of them or not
public enum OrderType
{
    [Description("Normal")] Normal,
    [Description("Short lending/Buy Cover")]
    ShortCover,
    [Description("Sell Lending stock")] SellLending,
    [Description("Short lending/Buy Cover with Program trade")]
    ShortCoverProgram,
    [Description("Sell Lending stock with Program trade")] SellLendingProgram,
    [Description("Sell Pledge stock with Program trade")] SellPledgeStockProgram,
    [Description("Program Trading")] Program,
    [Description("Market making with program trading")]
    MarketProgram,
    [Description("Market making")] Market
}
