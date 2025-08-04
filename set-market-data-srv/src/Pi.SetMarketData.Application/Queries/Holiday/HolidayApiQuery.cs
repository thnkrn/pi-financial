using System.Globalization;
using Microsoft.Extensions.Logging;
using Pi.SetMarketData.Application.Exceptions;
using Pi.SetMarketData.Application.Interfaces.Holiday;
using Pi.SetMarketData.Domain.AggregatesModels.HolidayAggregate;

namespace Pi.SetMarketData.Application.Queries.Holiday;

public class HolidayApiQuery : IHolidayApiQuery
{
    private const string IanaTimeZoneId = "Asia/Bangkok";
    private const string WindowsTimeZoneId = "SE Asia Standard Time";
    private static readonly DayOfWeek[] WeekendDays = [DayOfWeek.Saturday, DayOfWeek.Sunday];
    private static readonly CultureInfo CultureInformation = new("en-US");
    private readonly TimeZoneInfo _bangkokTimeZone;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IHolidayApiRepository _holidayApiRepository;
    private readonly ILogger<HolidayApiQuery> _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="holidayApiRepository"></param>
    /// <param name="dateTimeProvider"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public HolidayApiQuery(
        IHolidayApiRepository holidayApiRepository,
        IDateTimeProvider dateTimeProvider,
        ILogger<HolidayApiQuery> logger)
    {
        _holidayApiRepository = holidayApiRepository ?? throw new ArgumentNullException(nameof(holidayApiRepository));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _bangkokTimeZone = GetBangkokTimeZone();
    }

    public async Task<bool> IsNotBusinessDays(DateTime? date = null)
    {
        try
        {
            var utcDateTime = date?.ToUniversalTime() ?? _dateTimeProvider.UtcNow;
            var bangkokDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, _bangkokTimeZone);

            _logger.LogDebug("Bangkok DateTime: {BangkokDateTime}", FormatThaiDateTime(bangkokDateTime));

            if (WeekendDays.Contains(bangkokDateTime.DayOfWeek))
            {
                _logger.LogDebug("Is weekend: {DayOfWeek}", bangkokDateTime.DayOfWeek);
                return true;
            }

            var isHoliday = await IsHolidayAsync(bangkokDateTime.Date);
            _logger.LogDebug("Is holiday: {IsHoliday}", isHoliday);

            return isHoliday;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to determine business day status for Bangkok time zone.");
            throw new BusinessHoursDeterminationException(
                "Failed to determine business day status for Bangkok time zone", ex);
        }
    }

    private async Task<bool> IsHolidayAsync(DateTime bangkokDate)
    {
        _logger.LogDebug("Checking holiday for Bangkok date: {BangkokDate}", FormatThaiDateTime(bangkokDate));

        var isHoliday = await _holidayApiRepository.IsHoliday(bangkokDate.ToString("yyyy-MM-dd"));

        _logger.LogDebug("Repository returned isHoliday: {IsHoliday} for Bangkok date: {BangkokDate}", isHoliday,
            FormatThaiDateTime(bangkokDate));

        return isHoliday;
    }

    private static TimeZoneInfo GetBangkokTimeZone()
    {
        try
        {
            // Try IANA ID first (for non-Windows systems)
            return TimeZoneInfo.FindSystemTimeZoneById(IanaTimeZoneId);
        }
        catch
        {
            try
            {
                // Fallback to Windows ID
                return TimeZoneInfo.FindSystemTimeZoneById(WindowsTimeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
                // If both attempts fail, throw a more informative exception
                throw new TimeZoneNotFoundException(
                    $"Unable to find time zone for Thailand. Neither '{IanaTimeZoneId}' nor '{WindowsTimeZoneId}' were recognized.");
            }
        }
    }

    private static string FormatThaiDateTime(DateTime thaiDateTime)
    {
        return thaiDateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInformation);
    }
}