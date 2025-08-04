
namespace Pi.FundMarketData.Worker.Services.FundMarket.FundConnext.Models;

public class FundHoliday
{
    private readonly string _fundCode;
    public string FundCode { get => _fundCode; init => _fundCode = value.ToUpper(); }
    public DateTime HolidayDate { get; init; }

    public static readonly Func<string[], string, FundHoliday> Mapper = (dataValues, asOfDate) =>
    {
        return new FundHoliday
        {
            FundCode = dataValues[0],
            HolidayDate = (DateTime)Utils.UtilsMethod.StringToDateTime(dataValues[1])!
        };
    };
}
