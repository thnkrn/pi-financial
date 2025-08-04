using Pi.SetMarketData.Application.Queries;
using Pi.SetMarketData.Application.Interfaces;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Infrastructure.Handlers;

public class GetByIdCuratedMemberRequestHandler : GetByIdCuratedMemberRequestAbstractHandler
{
    private readonly IMongoService<CuratedMember> _curatedMemberService;

    public GetByIdCuratedMemberRequestHandler(IMongoService<CuratedMember> curatedMemberService)
    {
        _curatedMemberService = curatedMemberService;
    }

    protected override async Task<GetByIdCuratedMemberResponse> Handle(GetByIdCuratedMemberRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _curatedMemberService.GetByIdAsync(request.id);
            return new GetByIdCuratedMemberResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}