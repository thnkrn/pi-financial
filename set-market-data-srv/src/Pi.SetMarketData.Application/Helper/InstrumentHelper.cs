using System.Globalization;
using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Application.Helper;

public static class InstrumentHelper
{
    public static bool IsDeprecated(Instrument instrument)
    {
        // Check Equity
        if (instrument.SecurityType is InstrumentConstants.CS or InstrumentConstants.PS or InstrumentConstants.ETF
            or InstrumentConstants.DR)
        { 
            return instrument.TradingSigns?.Any(x => !string.IsNullOrEmpty(x.Sign) && x.Sign.Split(",").Contains("X")) == true;
        }

        var today = DateTime.UtcNow.Date;
        DateTime? checkDate = null;

        // Check TFEX
        if (instrument.SecurityType is InstrumentConstants.FC or InstrumentConstants.FP or InstrumentConstants.OEC
                or InstrumentConstants.OEP or InstrumentConstants.CMB or InstrumentConstants.SPT
                or InstrumentConstants.WEC or InstrumentConstants.WEP && instrument.LastTradingDate != null)
        {
            checkDate = ParseDate(instrument.LastTradingDate);
        }

        // Check Warrant
        if (instrument is { SecurityType: InstrumentConstants.W, ExerciseDate: not null })
        {
            checkDate = ParseDate(instrument.ExerciseDate);
        }

        // Check DW
        if (instrument.SecurityType is InstrumentConstants.DWC or InstrumentConstants.DWP &&
            instrument.MaturityDate != null)
        {
            checkDate = ParseDate(instrument.MaturityDate);
        }

        if (checkDate != null)
        {
            return today.Subtract((DateTime)checkDate).Days >= 5;
        }
            
        return false;
    }

    private static DateTime? ParseDate(string date)
    {
        const string datetimeFormat = "MM/dd/yyyy HH:mm:ss";
        const string dateFormat = "dd/MM/yyyy";

        if (date.Contains("00:00:00") && DateTime.TryParseExact(date, datetimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dateTime))
        {
            return dateTime;
        }

        if (DateTime.TryParseExact(date, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dateValue))
        {
            return dateValue;
        }

        return null;
    }
}
