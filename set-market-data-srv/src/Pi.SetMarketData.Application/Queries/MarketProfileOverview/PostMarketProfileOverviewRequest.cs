using MassTransit.Mediator;
using Pi.SetMarketData.Application.Queries.MarketProfileOverview;
using Pi.SetMarketData.Domain.Models.Request;

namespace Pi.SetMarketData.Application.Queries.MarketProfileFundamentals;

public record PostMarketProfileOverViewRequest(MarketProfileOverviewRequest data)
    : Request<PostMarketProfileOverviewResponse>;
