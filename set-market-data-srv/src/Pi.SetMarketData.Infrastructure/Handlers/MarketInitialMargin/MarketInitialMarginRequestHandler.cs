using Microsoft.Extensions.Logging;
using Pi.SetMarketData.Application.Interfaces.MarketInitialMargin;
using Pi.SetMarketData.Application.Queries;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.MarketInitialMargin;

public class MarketInitialMarginRequestHandler : GetMarketInitialMarginAbstractHandler
{
    private readonly IMongoService<InitialMargin> _initialMarginService;
    private readonly ILogger<MarketInitialMarginRequestHandler> _logger;

    public MarketInitialMarginRequestHandler(
        IMongoService<InitialMargin> initialMarginService,
        ILogger<MarketInitialMarginRequestHandler> logger
    )
    {
        _initialMarginService = initialMarginService;
        _logger = logger;
    }

    protected override async Task<PostMarketInitialMarginResponse> Handle(
        PostMarketInitialMarginRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var asOfDate = request.Data.AsOfDate;
            var requestData = request.Data.Data;

            if (requestData == null || requestData.Count == 0)
                return new PostMarketInitialMarginResponse(
                    new MarketInitialMarginResponse
                    {
                        Status = "false",
                        Error = new InitialMarginErrorResponse
                        {
                            Code = "400",
                            Message = "Bad Request"
                        }
                    }
                );

            foreach (var data in requestData)
            {
                var symbol = data.Symbol ?? string.Empty;
                var productType = data.ProductType ?? string.Empty;
                var im = data.Im ?? string.Empty;

                var initialMargin = await _initialMarginService.GetByFilterAsync(target =>
                    target.Symbol == symbol && target.ProductType == productType
                );

                var result = new InitialMargin
                {
                    Symbol = symbol,
                    ProductType = productType,
                    Im = im,
                    AsOfDate = asOfDate,
                    CreatedAt = initialMargin?.CreatedAt ?? DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _initialMarginService.UpsertAsyncByFilter(
                    target => target.Symbol == symbol && target.ProductType == productType,
                    result
                );
            }

            return new PostMarketInitialMarginResponse(
                new MarketInitialMarginResponse
                {
                    Status = "true",
                    Data = new InitialMarginResponse
                    {
                        TotalProcessed = requestData.Count.ToString(),
                        AsOfDate = asOfDate
                    }
                }
            );
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Handle Http request failed.");
            return new PostMarketInitialMarginResponse(
                new MarketInitialMarginResponse
                {
                    Status = "false",
                    Error = new InitialMarginErrorResponse
                    {
                        Code = "500",
                        Message = "Internal server error"
                    }
                }
            );
        }
    }
}
