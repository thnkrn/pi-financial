namespace Pi.TfexService.Listener.Utils;

public static class OperatingHoursUtils
{
    // returns current opening/closing datetime if currentDateTime is within operating hours
    // returns current closing and next opening datetime if currentDateTime is outside operating hours
    public static (DateTime, DateTime) GetOperatingDateTime(DateTime currentDateTime, TimeOnly openingTime, TimeOnly closingTime)
    {
        var currentDate = DateOnly.FromDateTime(currentDateTime);
        var currentTime = TimeOnly.FromDateTime(currentDateTime);

        var openingDateTime = currentDate.ToDateTime(openingTime, currentDateTime.Kind);
        var closingDateTime = currentDate.ToDateTime(closingTime, currentDateTime.Kind);

        // if operating hours is between days
        if (closingTime < openingTime)
        {
            // if within hours of first day
            if (currentTime >= openingTime)
            {
                closingDateTime = closingDateTime.AddDays(1);
            }

            // if within hours of second day
            else if (currentTime < closingTime)
            {
                openingDateTime = openingDateTime.AddDays(-1);
            }
        }
        else
        {
            // if before opening
            if (currentTime < openingTime)
            {
                closingDateTime = closingDateTime.AddDays(-1);
            }

            // if after opening
            else if (currentTime > closingTime)
            {
                openingDateTime = openingDateTime.AddDays(1);
            }
        }

        return (openingDateTime, closingDateTime);
    }
}