using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Commands;

namespace Pi.SetMarketData.Application.Interfaces;

public abstract class DeleteCuratedFilterRequestAbstractHandler: RequestHandler<DeleteCuratedFilterRequest, DeleteCuratedFilterResponse>;