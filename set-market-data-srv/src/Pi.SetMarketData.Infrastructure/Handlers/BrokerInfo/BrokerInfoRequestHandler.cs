using Pi.SetMarketData.Application.Commands.BrokerInfo;
using Pi.SetMarketData.Application.Interfaces.MarketFilters;
using Pi.SetMarketData.Domain.Entities.SetSmart;
using Pi.SetMarketData.Domain.Models.Response.BrokerInfo;
using Pi.SetMarketData.Infrastructure.Exceptions;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.BrokerInfoHandler;

public class BrokerInfoRequestHandler : BrokerInfoRequestAbstractHandler
{
    private readonly IMongoService<BrokerInfo> _brokerInfoService;
    public BrokerInfoRequestHandler
    (
        IMongoService<BrokerInfo> brokerInfoService
    )
    {
        _brokerInfoService = brokerInfoService;
    }

    protected override async Task<PostBrokerInfoResponse> Handle(PostBrokerInfoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request?.Data == null) throw new ValidationException("Broker data is required"); 
            await _brokerInfoService.UpsertBrokerInfoAsync(request.Data);
            var brokerInfoResponse = new BrokerInfoResponse
            {
                Status = true,
                Data = new ResponseData
                {
                    TotalProcessed = request.Data.Data?.Count ?? 0,
                    AsOfDate = request.Data.AsOfDate
                }
            };
            return new PostBrokerInfoResponse(brokerInfoResponse);
        }
        catch (ValidationException e)
        {
            Console.WriteLine(e);
            return new PostBrokerInfoResponse(
                new BrokerInfoResponse
                {
                    Status = false,
                    Error = new ErrorResponse
                    {
                        Code = "400",
                        Message = "Bad Request"
                    }
                }
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new PostBrokerInfoResponse(
                new BrokerInfoResponse
                {
                    Status = false,
                    Error = new ErrorResponse
                    {
                        Code = "500",
                        Message = "Internal Server Error"
                    }
                }
            );
        }
    }
}