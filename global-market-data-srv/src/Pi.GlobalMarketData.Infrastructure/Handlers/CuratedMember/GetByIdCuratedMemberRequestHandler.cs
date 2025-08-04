using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Application.Interfaces;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketData.Domain.Entities;

namespace Pi.GlobalMarketData.Infrastructure.Handlers;

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