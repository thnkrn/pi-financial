namespace Pi.WalletService.Domain.AggregatesModel.RefundInfoAggregate;

public interface IRefundInfoRepository
{
    Task<RefundInfo?> Get(Guid id);
    Task<RefundInfo> Create(RefundInfo refundInfo);
}
