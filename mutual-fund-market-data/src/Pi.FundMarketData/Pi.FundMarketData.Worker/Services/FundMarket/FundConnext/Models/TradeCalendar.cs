
namespace Pi.FundMarketData.Worker.Services.FundMarket.FundConnext.Models;

public class TradeCalendar
{
    private readonly string _fundCode;
    public string FundCode { get => _fundCode; init => _fundCode = value.ToUpper(); }

    public string TransactionCode { get; init; }
    public string TradeType { get; init; }
    public DateTime TradeDate { get; init; }

    public static readonly Func<string[], string, TradeCalendar> Mapper = (dataValues, asOfDate) =>
    {
        return new TradeCalendar
        {
            FundCode = dataValues[0],
            TransactionCode = dataValues[1],
            TradeType = dataValues[2],
            TradeDate = (DateTime)Utils.UtilsMethod.StringToDateTime(dataValues[3])!
        };
    };
}
