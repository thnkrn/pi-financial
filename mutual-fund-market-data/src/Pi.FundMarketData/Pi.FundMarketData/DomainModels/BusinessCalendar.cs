// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Pi.FundMarketData.Utils;

namespace Pi.FundMarketData.DomainModels;

public class BusinessCalendar
{
    public const int DateRange2WeeksIncrementor = 14;
    public IList<DateTime> BusinessHolidays { get; init; }

    public IList<DateTime> GetBusinessDays(DateTime start, DateTime end)
    {
        var businessDays = new List<DateTime>();
        var startDate = start.Date;
        var startEnd = end.Date;
        for (var date = startDate; date <= startEnd; date = date.AddDays(1))
        {
            if (date.IsWeekend() || BusinessHolidays.Contains(date))
                continue;

            businessDays.Add(date);
        }

        return businessDays;
    }
}
