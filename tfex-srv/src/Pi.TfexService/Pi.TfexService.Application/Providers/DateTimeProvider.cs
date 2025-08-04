using Pi.TfexService.Application.Utils;

namespace Pi.TfexService.Application.Providers;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime GetThDateTimeNow() => DateUtils.GetThDateTimeNow();
}