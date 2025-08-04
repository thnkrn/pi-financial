using System.ComponentModel.DataAnnotations.Schema;

namespace Pi.FundMarketData.Repositories.SqlDatabase;

public class Holiday
{
    //Unix time, January 1, 1970 is a common base date
    private static readonly DateTime BaseDate = new(1970, 1, 1);
    [Column("dHolidayDate")]
    public int HolidayDayGranularity { get; init; }
    [Column("sHolidayName", TypeName = "varchar(50)")]
    public string SHolidayName { get; init; }
    public DateTime HolidayDate => BaseDate.AddDays(HolidayDayGranularity);
}
