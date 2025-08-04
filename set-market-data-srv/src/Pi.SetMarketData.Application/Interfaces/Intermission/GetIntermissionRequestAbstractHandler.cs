using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.Intermission;

namespace Pi.SetMarketData.Application.Interfaces.Intermission;

public abstract class GetIntermissionRequestAbstractHandler: RequestHandler<GetIntermissionRequest, GetIntermissionResponse>;