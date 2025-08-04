using Pi.GlobalMarketData.Domain.Models.Response;

namespace Pi.GlobalMarketData.Application.Services.MarketData.MarketSchedules;

public static class MarketSchedulesService
{
    public static MarketSchedulesResponse GetResult(
        IEnumerable<Domain.Entities.MarketSchedule> schedules
    )
    {
        var response = new SchedulesResponse();
        var _data = schedules.Select(schedule =>
        {

            if (schedule.UTCStartTime.HasValue && schedule.UTCEndTime.HasValue)
            {
                var _date = schedule.UTCStartTime.Value.Date.ToString("yyyy-MM-dd");
                DateTimeOffset _startdto = new DateTimeOffset(schedule.UTCStartTime.Value);
                DateTimeOffset _enddto = new DateTimeOffset(schedule.UTCEndTime.Value);

                var _startTime = _startdto.ToUnixTimeSeconds();
                var _endTime = _enddto.ToUnixTimeSeconds();

                return new Schedules()
                {
                    StatusName = schedule.MarketSession ?? string.Empty,
                    Date = _date,
                    StartTime = _startTime,
                    EndTime = _endTime,
                };
            }
            
            return new Schedules();
        });

        DateTimeOffset _serverdto = new DateTimeOffset(DateTime.Now.ToUniversalTime());
        response = new SchedulesResponse()
        {
            ServerTimestamp = _serverdto.ToUnixTimeSeconds(),
            Schedules = _data.ToList()
        };

        return new MarketSchedulesResponse
        {
            Code = "0",
            Message = "",
            Response = response
        };
    }
}
