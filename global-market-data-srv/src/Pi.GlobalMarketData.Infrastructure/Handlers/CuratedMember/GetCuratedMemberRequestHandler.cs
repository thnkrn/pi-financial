using Pi.GlobalMarketData.Application.Interfaces;
using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Application.Services.MarketDataManagement;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketData.Infrastructure.Handlers;

public class GetCuratedMemberRequestHandler : GetCuratedMemberRequestAbstractHandler
{
    private readonly IMongoService<CuratedMember> _curatedMemberService;
    private readonly IMongoService<Domain.Entities.GeInstrument> _geInstrumentService;

    public GetCuratedMemberRequestHandler(
        IMongoService<CuratedMember> curatedMemberService,
        IMongoService<Domain.Entities.GeInstrument> geInstrumentService
    )
    {
        _curatedMemberService = curatedMemberService;
        _geInstrumentService = geInstrumentService;
    }

    protected override async Task<GetCuratedMemberResponse> Handle(GetCuratedMemberRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var curatedMembers = await _curatedMemberService.GetAllByFilterAsync(target =>
                (request.curatedListId == null) || (target.CuratedListId == request.curatedListId)
            );
            var symbol = curatedMembers.Select(target => target.Symbol).ToHashSet();
            var instruments = await _geInstrumentService.GetAllByFilterAsync(target => symbol.Contains(target.Symbol));
            var logos = instruments.Select(instrument =>
            {
                var logoMarket = instrument.Venue;
                if (string.IsNullOrEmpty(logoMarket))
                    logoMarket = "SET";
                return LogoHelper.GetLogoUrl(logoMarket, instrument.Symbol ?? "");
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