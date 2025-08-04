using System.ComponentModel;

namespace Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

public enum StockType
{
    [Description("Normal")]
    Normal = 2,
    [Description("Short")]
    Short = 20,
    [Description("Borrow")]
    Borrow = 22,
    [Description("Lending")]
    Lending = 23,
    [Description("NewStockType8")]
    NewStockType8 = 8, // TODO: Need to check the stock type name,
    [Description("NewStockType82")]
    NewStockType82 = 82, // TODO: Need to check the stock type name
    [Description("UnknowForDebug")]
    Unknow = 999,
}
