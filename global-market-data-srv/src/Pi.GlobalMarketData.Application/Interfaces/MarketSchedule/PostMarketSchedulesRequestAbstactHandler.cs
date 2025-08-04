using Pi.GlobalMarketData.Application.Abstractions;
using Pi.GlobalMarketData.Application.Queries;

namespace Pi.GlobalMarketData.Application.Interfaces.MarketSchedule;

public abstract class PostMarketSchedulesRequestAbstactHandler
    : RequestHandler<PostMarketScheduleRequest, PostMarketScheduleResponse>;
