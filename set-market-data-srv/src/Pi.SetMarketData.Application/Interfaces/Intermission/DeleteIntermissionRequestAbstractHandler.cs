using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Commands.Intermission;

namespace Pi.SetMarketData.Application.Interfaces.Intermission;

public abstract class DeleteIntermissionRequestAbstractHandler: RequestHandler<DeleteIntermissionRequest, DeleteIntermissionResponse>;