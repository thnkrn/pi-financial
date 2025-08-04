using Pi.Common.Features;
using Pi.GlobalEquities.Utils;

namespace Pi.GlobalEquities.Services.Velexa;

public class VelexaMarketDataService : IMarketDataService
{
    private readonly VelexaClient _velexaClient;
    private readonly IFeatureService _featureService;
    private const int DaysToCheck = 7;
    private const string SupportNewOrderType = "global-equity-conditional-orders";

    public VelexaMarketDataService(VelexaClient velexaClient, IFeatureService featureService)
    {
        _velexaClient = velexaClient;
        _featureService = featureService;
    }

    public async Task<Dictionary<OrderType, IEnumerable<OrderDuration>>> GetSupportedOrderDetails(string symbolId, CancellationToken ct)
    {
        if (_featureService.IsOff(SupportNewOrderType))
        {
            return new Dictionary<OrderType, IEnumerable<OrderDuration>>
            {
                { OrderType.Limit, new[] { OrderDuration.Day } },
                { OrderType.Market, new[] { OrderDuration.Day } },
                { OrderType.StopLimit, new[] { OrderDuration.Day } },
                { OrderType.Stop, new[] { OrderDuration.Day } }
            };
        }

        var result = await _velexaClient.GetMarketSchedule(symbolId, true, ct);
        var details = new Dictionary<OrderType, IEnumerable<OrderDuration>>();

        var venue = symbolId.Split(".")[1];
        var intervals = FilterCurrentDateByVenue(result, venue);
        foreach (var interval in intervals)
        {
            AddOrderTypeDetails(details, OrderType.Limit, interval?.orderTypes?.limit);
            AddOrderTypeDetails(details, OrderType.StopLimit, interval?.orderTypes?.stop_limit);
            AddOrderTypeDetails(details, OrderType.Market, interval?.orderTypes?.market);
            AddOrderTypeDetails(details, OrderType.Stop, interval?.orderTypes?.stop);
            AddTpSlOrderTypeDetails(details, interval?.orderTypes?.limit, interval?.orderTypes?.stop);
        }

        return details;
    }

    private static IEnumerable<VelexaModel.IntervalResponse> FilterCurrentDateByVenue(VelexaModel.ScheduleResponse result, string venue)
    {
        var todayDateUtc = DateTime.UtcNow;

        //NOTE: find next closest trading day
        for (int i = 0; i < DaysToCheck; i++)
        {
            var localSymbolTimeNow = venue == "HKEX"
                ? DateTimeUtils.ConvertToDateTimeHk(todayDateUtc)
                : DateTimeUtils.ConvertToDateTimeUs(todayDateUtc);

            var matchedInterval = result.intervals.Where(x =>
            {
                DateTime localSymbolTimeStart, localSymbolTimeEnd;
                if (venue == "HKEX")
                {
                    localSymbolTimeStart = DateTimeUtils.ConvertToDateTimeHk(DateTimeUtils.ConvertToDateTimeUtc(x.period.start));
                    localSymbolTimeEnd = DateTimeUtils.ConvertToDateTimeHk(DateTimeUtils.ConvertToDateTimeUtc(x.period.end));
                }
                else
                {
                    localSymbolTimeStart = DateTimeUtils.ConvertToDateTimeUs(DateTimeUtils.ConvertToDateTimeUtc(x.period.start));
                    localSymbolTimeEnd = DateTimeUtils.ConvertToDateTimeUs(DateTimeUtils.ConvertToDateTimeUtc(x.period.end));
                }

                //NOTE: start or end in the same day
                return localSymbolTimeStart.Date == localSymbolTimeNow.Date || localSymbolTimeNow.Date == localSymbolTimeEnd.Date;
            }).ToArray();

            if (matchedInterval.Length != 0)
                return matchedInterval;

            todayDateUtc = todayDateUtc.AddDays(1);
        }

        return Enumerable.Empty<VelexaModel.IntervalResponse>();
    }

    private static void AddOrderTypeDetails(Dictionary<OrderType, IEnumerable<OrderDuration>> details, OrderType orderType, IEnumerable<string> orderTypeDetails)
    {
        if (orderTypeDetails == null) return;

        var orderDurations = orderTypeDetails.Select(x => x.GetOrderDuration());

        if (details.TryGetValue(orderType, out var existingDurations) && existingDurations != null)
            details[orderType] = existingDurations.Concat(orderDurations).Distinct().Where(IsSupportedDuration);
        else
            details[orderType] = orderDurations;
    }

    private static void AddTpSlOrderTypeDetails(Dictionary<OrderType, IEnumerable<OrderDuration>> details,
        IEnumerable<string> orderTypeLimitDetails, IEnumerable<string> orderTypeStopDetails)
    {
        if (orderTypeLimitDetails == null || orderTypeStopDetails == null) return;

        //NOTE: already have TpSl with Gtc
        if (details.TryGetValue(OrderType.Tpsl, out var existingDurations) && existingDurations.FirstOrDefault() == OrderDuration.Gtc)
            return;

        var limitDurationSet = new HashSet<OrderDuration>(orderTypeLimitDetails.Select(x => x.GetOrderDuration()));
        var stopDurationSet = new HashSet<OrderDuration>(orderTypeStopDetails.Select(x => x.GetOrderDuration()));

        if (limitDurationSet.Contains(OrderDuration.Gtc) && stopDurationSet.Contains(OrderDuration.Gtc))
            details[OrderType.Tpsl] = new List<OrderDuration> { OrderDuration.Gtc };
    }

    private static bool IsSupportedDuration(OrderDuration duration)
    {
        return duration != OrderDuration.Unknown && duration != OrderDuration.Ato && duration != OrderDuration.Fok &&
               duration != OrderDuration.Gtt && duration != OrderDuration.Ioc;
    }
}
