using Microsoft.EntityFrameworkCore;
using Pi.OnePort.Db2.Models;

namespace Pi.OnePort.Db2.Repositories;

public class SetRepo : ISetRepo
{
    private readonly IFisDbContext _context;
    private const string EmptyValue = " ";

    public SetRepo(IFisDbContext context)
    {
        _context = context;
    }

    public async Task<List<AccountPositionCreditBalance>> GetCreditBalanceAccountPositions(string accountNo, int page = 1, CancellationToken ct = default)
    {
        return await _context.Set<AccountPositionCreditBalance>().FromSql($"call DB2INST1.STR_PORTFOLIO_CB_NEW({accountNo}, {page});").ToListAsync(cancellationToken: ct);
    }

    public async Task<List<AccountPosition>> GetAccountPositions(string accountNo, int page = 1, CancellationToken ct = default)
    {
        return await _context.Set<AccountPosition>().FromSql($"call DB2INST1.STR_PORTFOLIO_NEW({accountNo}, {page});").ToListAsync(cancellationToken: ct);
    }

    public async Task<List<AccountAvailable>> GetAccountsAvailable(string accountNo, int page = 1, CancellationToken ct = default)
    {
        return await _context.Set<AccountAvailable>().FromSql($"call DB2INST1.STR_ACCOUNTINFO_0208({accountNo}, {page});").ToListAsync(cancellationToken: ct);
    }

    public async Task<List<AccountAvailableCreditBalance>> GetCreditBalanceAccountsAvailable(string accountNo, int page = 1, CancellationToken ct = default)
    {
        return await _context.Set<AccountAvailableCreditBalance>().FromSql($"call DB2INST1.STR_ACCOUNTINFO_CB({accountNo}, {page});").ToListAsync(cancellationToken: ct);
    }

    public async Task<List<OfflineOrder>> GetOfflineOrders(string accountNo, int page = 1, CancellationToken ct = default)
    {
        return await _context.Set<OfflineOrder>().FromSql($"call DB2INST1.STR_OFFLINEORDER({accountNo}, {page});").ToListAsync(cancellationToken: ct);
    }

    public async Task<List<Order>> GetOrders(string accountNo, int page = 1, CancellationToken ct = default)
    {
        return await _context.Set<Order>().FromSql($"call DB2INST1.STR_ORDER({accountNo}, {page});").ToListAsync(cancellationToken: ct);
    }

    public async Task<List<AccountDeal>> GetDealsByAccountNo(string accountNo, int page = 1, CancellationToken ct = default)
    {
        return await _context.Set<AccountDeal>().FromSql($"call DB2INST1.STR_DEAL_BYACC({accountNo}, {page});").ToListAsync(cancellationToken: ct);
    }

    public async Task<List<DealOrder>> GetDealsByOrderNo(int orderNo, int page = 1, CancellationToken ct = default)
    {
        return await _context.Set<DealOrder>().FromSql($"call DB2INST1.STR_DEAL({orderNo}, {page});").ToListAsync(cancellationToken: ct);
    }

    public async Task<int> CancelOfflineOrder(CancelOfflineOrderRequest request, CancellationToken ct = default)
    {
        var delFlag = request.DelFlag ? 'Y' : 'N';
        var query = $@"call DB2INST1.STR_CANCELOFFLINEORDER(
            '{request.OrderDateTime:yyyyMMdd}',
            '{request.OrderDateTime:HHmmss}',
            {request.OrderNo},
            '{request.CancelId}',
            '{request.CancelDateTime:yyyyMMdd}',
            '{request.CancelDateTime:HHmmss}',
            '{delFlag}'
        );";

        return await _context.Database.ExecuteSqlRawAsync(query, cancellationToken: ct);
    }

    public async Task<int> NewOfflineOrder(OfflineOrderRequest request, CancellationToken ct = default)
    {
        var price = Convert.ToDouble(request.Price);
        var query = $@"call DB2INST1.STR_newOfflineOrder(
            '{request.OrderDateTime:yyyyMMdd}',
            '{request.OrderDateTime:HHmmss}',
            {request.OrderNo},
            '{request.EnterId}',
            '{request.SecSymbol}',
            '{request.Side}',
            '{request.Board ?? EmptyValue}',
            '{request.AccountNo}',
            {price},
            '{request.ConditionPrice ?? EmptyValue}',
            {request.Volume},
            {request.PubVolume},
            '{request.Condition ?? EmptyValue}',
            '{request.Market}',
            {request.BrokerNo},
            '{request.TrusteeId ?? EmptyValue}',
            '{request.OrderType ?? EmptyValue}',
            '{request.ServiceType}',
            '{request.PutThoughFlag ?? EmptyValue}',
            '{request.Life ?? EmptyValue}',
            '{request.ExpireDate ?? EmptyValue}'
        );";

        return await _context.Database.ExecuteSqlRawAsync(query, cancellationToken: ct);
    }

    public async Task<int> UpdateOfflineOrder(OfflineOrderRequest request, CancellationToken ct = default)
    {
        var price = Convert.ToDouble(request.Price);
        var query = $@"call DB2INST1.STR_UpdateOfflineOrder(
            '{request.OrderDateTime:yyyyMMdd}',
            '{request.OrderDateTime:HHmmss}',
            {request.OrderNo},
            '{request.EnterId}',
            '{request.SecSymbol}',
            '{request.Side}',
            '{request.Board ?? EmptyValue}',
            '{request.AccountNo}',
            {price},
            '{request.ConditionPrice ?? EmptyValue}',
            {request.Volume},
            {request.PubVolume},
            '{request.Condition ?? EmptyValue}',
            '{request.Market}',
            {request.BrokerNo},
            '{request.TrusteeId ?? EmptyValue}',
            '{request.OrderType ?? EmptyValue}',
            '{request.ServiceType}',
            '{request.PutThoughFlag ?? EmptyValue}',
            '{request.Life ?? EmptyValue}',
            '{request.ExpireDate ?? EmptyValue}',
            'Y'
        );";

        return await _context.Database.ExecuteSqlRawAsync(query, cancellationToken: ct);
    }
}
