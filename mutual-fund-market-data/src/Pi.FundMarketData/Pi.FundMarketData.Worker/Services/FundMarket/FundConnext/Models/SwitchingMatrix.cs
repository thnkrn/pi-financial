using Pi.FundMarketData.Utils;

namespace Pi.FundMarketData.Worker.Services.FundMarket.FundConnext.Models;

public class SwitchingMatrix
{
    private readonly string _fundCodeOut;
    public string FundCodeOut { get => _fundCodeOut; init => _fundCodeOut = value.ToUpper(); }

    private readonly string _fundCodeIn;
    public string FundCodeIn { get => _fundCodeIn; init => _fundCodeIn = value.ToUpper(); }

    public int SwitchSettlementDay { get; init; }
    public string SwitchingType { get; init; }

    public static readonly Func<string[], string, SwitchingMatrix> Mapper = (dataValues, asOfDate) =>
    {
        return new SwitchingMatrix
        {
            FundCodeOut = dataValues[0],
            FundCodeIn = dataValues[1],
            SwitchSettlementDay = (int)UtilsMethod.StringToInt(dataValues[2])!,
            SwitchingType = dataValues[3]
        };
    };
}
