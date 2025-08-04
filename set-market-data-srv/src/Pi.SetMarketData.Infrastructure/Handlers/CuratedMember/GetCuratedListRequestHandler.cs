using Pi.SetMarketData.Application.Interfaces;
using Pi.SetMarketData.Application.Queries;
using Pi.SetMarketData.Application.Services.MarketDataManagement;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Helpers;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers;

public class GetCuratedMemberRequestHandler: GetCuratedMemberRequestAbstractHandler
{
    private readonly IMongoService<CuratedMember> _curatedMemberService;
    private readonly IMongoService<Domain.Entities.Instrument> _instrumentService;
    
    public GetCuratedMemberRequestHandler
    (
        IMongoService<CuratedMember> curatedMemberService,
        IMongoService<Domain.Entities.Instrument> instrumentService
    )
    {
        _curatedMemberService = curatedMemberService;
        _instrumentService = instrumentService;
    }
    
    protected override async Task<GetCuratedMemberResponse> Handle(GetCuratedMemberRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var curatedMembers = await _curatedMemberService.GetAllByFilterAsync(target =>
                (request.curatedListId == null) || (target.CuratedListId == request.curatedListId)
            );

            var symbol = curatedMembers.Select(target => target.Symbol).ToHashSet();
            var instruments = await _instrumentService.GetAllByFilterAsync(target => symbol.Contains(target.Symbol));
            var logos = instruments.Select(instrument =>
            {
                var logoMarket = instrument.Venue;
                if (string.IsNullOrEmpty(logoMarket))
                    logoMarket = "SET";
                return LogoHelper.GetLogoUrl(logoMarket, instrument.SecurityType ?? string.Empty, instrument.Symbol ?? string.Empty);
            });

            var result = CuratedMemberService.GetResult(
                instruments.ToList(),
                logos.ToList()
            );
            return new GetCuratedMemberResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
