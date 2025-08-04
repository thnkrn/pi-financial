namespace Pi.FundMarketData.Worker.Services.FundMarket.FundConnext.Models;

public class FundMapping
{
    private readonly string _fundCode;
    public string FundCode { get => _fundCode; init => _fundCode = value.ToUpper(); }

    public static readonly Func<string[], string, FundMapping> Mapper = (dataValues, asOfDate) =>
    {
        return new FundMapping
        {
            FundCode = dataValues[1]
        };
    };
}
