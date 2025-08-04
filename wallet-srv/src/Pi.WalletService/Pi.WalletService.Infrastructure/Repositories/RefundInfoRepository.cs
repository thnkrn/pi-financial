using Microsoft.EntityFrameworkCore;
using Pi.WalletService.Domain.AggregatesModel.RefundInfoAggregate;
namespace Pi.WalletService.Infrastructure.Repositories;

public class RefundInfoRepository : IRefundInfoRepository
{
    private readonly WalletDbContext _walletDbContext;

    public RefundInfoRepository(WalletDbContext walletDbContext)
    {
        _walletDbContext = walletDbContext;
    }

    public async Task<RefundInfo?> Get(Guid id)
    {
        return await _walletDbContext
            .Set<RefundInfo>()
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<RefundInfo> Create(RefundInfo refundInfo)
    {
        if (refundInfo == null)
        {
            throw new ArgumentNullException(nameof(refundInfo));
        }
        await _walletDbContext.Set<RefundInfo>().AddAsync(refundInfo);
        await _walletDbContext.SaveChangesAsync();

        return refundInfo;
    }
}
