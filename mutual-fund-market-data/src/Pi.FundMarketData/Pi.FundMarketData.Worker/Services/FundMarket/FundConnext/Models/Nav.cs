using Pi.FundMarketData.Utils;

namespace Pi.FundMarketData.Worker.Services.FundMarket.FundConnext.Models;

public class Nav
{
    private readonly string _fundCode;
    public string FundCode { get => _fundCode; init => _fundCode = value.ToUpper(); }

    public decimal Aum { get; init; }
    public decimal NavVal { get; init; }
    public decimal TotalUnit { get; set; }
    public DateTime NavDate { get; set; }

    public static readonly Func<string[], string, Nav> Mapper = (dataValues, asOfDate) =>
    {
        return new Nav
        {
            FundCode = dataValues[1],
            Aum = (decimal)UtilsMethod.StringToDecimal(dataValues[2])!,
            NavVal = (decimal)UtilsMethod.StringToDecimal(dataValues[3])!,
            TotalUnit = (decimal)UtilsMethod.StringToDecimal(dataValues[10])!,
            NavDate = (DateTime)UtilsMethod.StringToDateTime(dataValues[8])!
        };
    };
}
