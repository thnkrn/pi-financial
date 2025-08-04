using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.Financial;

namespace Pi.SetMarketData.Application.Interfaces.Financial;

public abstract class GetByIdFinancialRequestAbstractHandler: RequestHandler<GetByIdFinancialRequest, GetByIdFinancialResponse>;