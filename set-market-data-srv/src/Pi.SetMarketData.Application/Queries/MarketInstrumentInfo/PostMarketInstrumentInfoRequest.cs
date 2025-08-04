using MassTransit.Mediator;
using Pi.SetMarketData.Domain.Models.Request;

namespace Pi.SetMarketData.Application.Queries.MarketProfileFinancials;

public record PostMarketInstrumentInfoRequest(MarketInstrumentInfoRequest Data)
    : Request<PostMarketInstrumentInfoResponse>;
