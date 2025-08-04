using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.Intermission;

namespace Pi.SetMarketData.Application.Interfaces.Intermission;

public abstract class GetByIdIntermissionRequestAbstractHandler: RequestHandler<GetByIdIntermissionRequest, GetByIdIntermissionResponse>;